using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Download_Vouchery.Models;
using Download_Vouchery.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.SqlClient;
using System.Reflection;

namespace Download_Vouchery.Controllers
{
    public class VouchersController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public UserManager<ApplicationUser> UserManager()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            return manager;
        }

        // Gets the vouchers of the currently logged in user
        public async Task<IHttpActionResult> GetVouchers(Guid id, int pageIndex = 0, int pageSize = 10)
        {
            // Get currently logged in user
            var currentUser = UserManager().FindById(User.Identity.GetUserId());

            // Count how many vouchers the user has
            var totalCount = _db.Vouchers
                .Where(ui => ui.VoucherFileId.FileOwner.Id == currentUser.Id)
                .Count(fi => fi.VoucherFileId.FileId == id);

            // Calculate how many pages of vouchers could be retrieved
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            // Get a page of the user's vouchers
            var vouchers = await _db.Vouchers
                .Include(fo => fo.VoucherFileId.FileOwner)
                .Include(fi => fi.VoucherFileId)
                .Where(ui => ui.VoucherFileId.FileOwner.Id == currentUser.Id)
                .Where(fi => fi.VoucherFileId.FileId == id)
                .OrderBy(d => d.VoucherId)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return the voucher page and add pagination info for the angularjs pagination
            var result = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Vouchers = vouchers
            };

            return Ok(result);
        }

        // Get statistics for the user's vouchers
        [ResponseType(typeof(VoucherInfoViewModel))]
        public async Task<IHttpActionResult> GetVouchersInfo(Guid id)
        {
            // Get currently logged in user
            var currentUser = UserManager().FindById(User.Identity.GetUserId());

            // Create and fill the viewmodel for the voucher statistics
            VoucherInfoViewModel voucherInfo = new VoucherInfoViewModel
            {
                VoucherAmount = await _db.Vouchers
                    .CountAsync(i => i.VoucherFileId.FileOwner.Id == currentUser.Id &&
                                     i.VoucherFileId.FileId == id),
                VoucherAmountRedeemed = await _db.Vouchers
                    .CountAsync(i => i.VoucherFileId.FileOwner.Id == currentUser.Id &&
                                     i.VoucherFileId.FileId == id &&
                                     i.VoucherRedeemed == true)
            };

            // Calculate this property to save one database call
            voucherInfo.VoucherAmountNotRedeemed = voucherInfo.VoucherAmount - voucherInfo.VoucherAmountRedeemed;

            // Average of the voucher redemption frequency can just be evaluated when there are already redeemed vouchers, otherwise return 0
            if (voucherInfo.VoucherAmountRedeemed > 0)
            {
                voucherInfo.VoucherRedemptionFrequency = await _db.Vouchers
                .Where(fi => fi.VoucherFileId.FileOwner.Id == currentUser.Id)
                .Where(ui => ui.VoucherFileId.FileId == id)
                .Where(vr => vr.VoucherRedeemed == true)
                .AverageAsync(c => c.VoucherRedemptionCounter);
            }
            else
            {
                voucherInfo.VoucherRedemptionFrequency = 0;
            }

            return Ok(voucherInfo);
        }

        [ResponseType(typeof(Voucher))]
        public async Task<IHttpActionResult> GetVoucherDetails(Guid id)
        {
            Voucher voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            return Ok(voucher);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutVoucher(Guid id, Voucher voucher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != voucher.VoucherId)
            {
                return BadRequest();
            }

            _db.Entry(voucher).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoucherExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        
        // Help method for bulk insert
        private async Task<string> DoBulkCopy(bool keepNulls, DataTable reader, string destinationTable)
        {
            // Get database connection
            string dataBaseConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlBulkCopyOptions options = new SqlBulkCopyOptions();
            options = SqlBulkCopyOptions.KeepIdentity;

            if (keepNulls)
            {
                options = options |= SqlBulkCopyOptions.KeepNulls;
            }

            using (SqlBulkCopy bc = new SqlBulkCopy(dataBaseConnectionString, options))
            {
                // Choose the destination table
                bc.DestinationTableName = destinationTable;
                // Timeout has to be high to give the bulk copying time
                bc.BulkCopyTimeout = 100000;
                foreach (DataColumn col in reader.Columns)
                {
                    bc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                await bc.WriteToServerAsync(reader);
            }

            return "Succeded";
        }

        // Help method for bulk insert
        public DataTable ToDataTable<T>(LinkedList<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        // POST: api/Vouchers
        [ResponseType(typeof(Voucher))]
        public async Task<IHttpActionResult> PostVoucher(int voucherAmount, string id)
        {
            // Variable to validate the request
            var proceed = !(!ModelState.IsValid || voucherAmount > 10000);

            // Check that user doesn't generate more than 10000 vouchers for a file
            if (_db.Vouchers.Any(fi => fi.VoucherFileId.FileId == new Guid(id)))
            {
                var check = await _db.Vouchers.Where(fi => fi.VoucherFileId.FileId == new Guid(id)).CountAsync();
                if (check >= 10000 || check + voucherAmount > 10000)
                {
                    proceed = false;
                }
            }

            if (proceed == false)
            {
                return BadRequest("Validation failed or you tried to create more than 10000 vouchers.");
            }

            // Get the file the voucher refers to
            var voucherFile = _db.BlobUploadModels.Find(new Guid(id));

            // Create a list of vouchers for bulk copy
            LinkedList<VoucherBulkInsertViewModel> temp = new LinkedList<VoucherBulkInsertViewModel>();

            for (int i = 0; i < voucherAmount; i++) {
                var voucher = new VoucherBulkInsertViewModel {VoucherId = Guid.NewGuid()};

                voucher.VoucherCode = voucher.VoucherId.ToString().Substring(0,8);
                voucher.VoucherCreationDate = DateTime.Now;
                voucher.VoucherRedeemed = false;
                voucher.VoucherRedemptionDate = null;
                voucher.VoucherFileId_FileId = voucherFile.FileId;
                voucher.VoucherRedemptionCounter = 0;

                temp.AddFirst(voucher);
            }

            // Convert the list to a data table for bulk copying
            var reader = ToDataTable<VoucherBulkInsertViewModel>(temp);

            try
            {
                // Do the actual bulk copy
                await DoBulkCopy(false, reader, "Vouchers");
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.Created);
        }

        // DELETE: api/Vouchers/5
        [ResponseType(typeof(Voucher))]
        public async Task<IHttpActionResult> DeleteVoucher(Guid id)
        {
            Voucher voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            _db.Vouchers.Remove(voucher);
            await _db.SaveChangesAsync();

            return Ok(voucher);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VoucherExists(Guid id)
        {
            return _db.Vouchers.Count(e => e.VoucherId == id) > 0;
        }
    }
}