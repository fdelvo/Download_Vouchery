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
        private ApplicationDbContext db = new ApplicationDbContext();

        public UserManager<ApplicationUser> UserManager()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            return manager;
        }

        // GET: api/Vouchers
        public async Task<IHttpActionResult> GetVouchers(Guid id, int pageIndex = 0, int pageSize = 10)
        {
            var currentUser = UserManager().FindById(User.Identity.GetUserId());

            var totalCount = db.Vouchers
                .Where(ui => ui.VoucherFileId.FileOwner.Id == currentUser.Id)
                .Count(fi => fi.VoucherFileId.FileId == id);

            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var vouchers = await db.Vouchers
                .Include(fo => fo.VoucherFileId.FileOwner)
                .Include(fi => fi.VoucherFileId)
                .Where(ui => ui.VoucherFileId.FileOwner.Id == currentUser.Id)
                .Where(fi => fi.VoucherFileId.FileId == id)
                .OrderBy(d => d.VoucherId)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Vouchers = vouchers
            };

            return Ok(result);
        }

        [ResponseType(typeof(VoucherInfoViewModel))]
        public async Task<IHttpActionResult> GetVouchersInfo(Guid id)
        {
            var currentUser = UserManager().FindById(User.Identity.GetUserId());
            VoucherInfoViewModel voucherInfo = new VoucherInfoViewModel
            {
                VoucherAmount = await db.Vouchers
                    .CountAsync(i => i.VoucherFileId.FileOwner.Id == currentUser.Id &&
                                     i.VoucherFileId.FileId == id),
                VoucherAmountRedeemed = await db.Vouchers
                    .CountAsync(i => i.VoucherFileId.FileOwner.Id == currentUser.Id &&
                                     i.VoucherFileId.FileId == id &&
                                     i.VoucherRedeemed == true)
            };



            voucherInfo.VoucherAmountNotRedeemed = voucherInfo.VoucherAmount - voucherInfo.VoucherAmountRedeemed;

            if (voucherInfo.VoucherAmountRedeemed > 0)
            {
                voucherInfo.VoucherRedemptionFrequency = await db.Vouchers
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

        // GET: api/Vouchers/5
        [ResponseType(typeof(Voucher))]
        public async Task<IHttpActionResult> GetVoucherDetails(Guid id)
        {
            Voucher voucher = await db.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            return Ok(voucher);
        }

        // PUT: api/Vouchers/5
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

            db.Entry(voucher).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
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

        

        private async Task<string> DoBulkCopy(bool keepNulls, DataTable reader, string destinationTable)
        {
            string dataBaseConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlBulkCopyOptions options = new SqlBulkCopyOptions();
            options = SqlBulkCopyOptions.KeepIdentity;
            if (keepNulls)
            {
                options = options |= SqlBulkCopyOptions.KeepNulls;
            }
            using (SqlBulkCopy bc = new SqlBulkCopy(dataBaseConnectionString, options))
            {
                bc.DestinationTableName = destinationTable;
                bc.BulkCopyTimeout = 100000;
                foreach (DataColumn col in reader.Columns)
                {
                    bc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                await bc.WriteToServerAsync(reader);
            }

            return "Succeded";
        }

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
        public async Task<IHttpActionResult> PostVoucher(VoucherViewModel vm, string id)
        {
            var proceed = !(!ModelState.IsValid || vm.VoucherAmount > 10000);

            if (db.Vouchers.Any(fi => fi.VoucherFileId.FileId == new Guid(id)))
            {
                var check = await db.Vouchers.Where(fi => fi.VoucherFileId.FileId == new Guid(id)).CountAsync();
                if (check >= 10000 || check + vm.VoucherAmount > 10000)
                {
                    proceed = false;
                }
            }

            if (proceed == false)
            {
                return BadRequest("Validation failed or you tried to create more than 10000 vouchers.");
            }

            var voucherFile = db.BlobUploadModels.Find(new Guid(id));
            LinkedList<VoucherBulkInsertViewModel> temp = new LinkedList<VoucherBulkInsertViewModel>();

            for (int i = 0; i < vm.VoucherAmount; i++) {
                var voucher = new VoucherBulkInsertViewModel {VoucherId = Guid.NewGuid()};

                voucher.VoucherCode = voucher.VoucherId.ToString().Substring(0,8);
                voucher.VoucherCreationDate = DateTime.Now;
                voucher.VoucherRedeemed = false;
                voucher.VoucherRedemptionDate = null;
                voucher.VoucherFileId_FileId = voucherFile.FileId;
                voucher.VoucherRedemptionCounter = 0;

                temp.AddFirst(voucher);
            }

            var reader = ToDataTable<VoucherBulkInsertViewModel>(temp);

            try
            {
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
            Voucher voucher = await db.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            db.Vouchers.Remove(voucher);
            await db.SaveChangesAsync();

            return Ok(voucher);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VoucherExists(Guid id)
        {
            return db.Vouchers.Count(e => e.VoucherId == id) > 0;
        }
    }
}