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
    public class OnlineVouchersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public UserManager<ApplicationUser> UserManager()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            return manager;
        }

        public async Task<IHttpActionResult> GetOnlineVouchers(Guid id, int pageIndex = 0, int pageSize = 10)
        {
            var currentUser = UserManager().FindById(User.Identity.GetUserId());

            var onlineVouchers = await db.OnlineVouchers
                .Include(fo => fo.OnlineVoucherFileId.FileOwner)
                .Include(fi => fi.OnlineVoucherFileId)
                .Where(ui => ui.OnlineVoucherFileId.FileOwner.Id == currentUser.Id)
                .Where(fi => fi.OnlineVoucherFileId.FileId == id)
                .OrderBy(d => d.OnlineVoucherId)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(onlineVouchers);
        }

        [ResponseType(typeof(OnlineVoucherInfoViewModel))]
        public async Task<IHttpActionResult> GetOnlineVouchersInfo(Guid id)
        {
            var currentUser = UserManager().FindById(User.Identity.GetUserId());
            OnlineVoucherInfoViewModel voucherInfo = new OnlineVoucherInfoViewModel();

            voucherInfo.VoucherAmount = await db.OnlineVouchers
                .CountAsync(i => i.OnlineVoucherFileId.FileOwner.Id == currentUser.Id &&
                    i.OnlineVoucherFileId.FileId == id);

            voucherInfo.VoucherAmountRedeemed = await db.OnlineVouchers
                .CountAsync(i => i.OnlineVoucherFileId.FileOwner.Id == currentUser.Id &&
                    i.OnlineVoucherFileId.FileId == id &&
                    i.OnlineVoucherRedeemed == true);

            voucherInfo.VoucherAmountNotRedeemed = voucherInfo.VoucherAmount - voucherInfo.VoucherAmountRedeemed;

            voucherInfo.VoucherAmountShared = await db.OnlineVouchers
                .CountAsync(i => i.OnlineVoucherFileId.FileOwner.Id == currentUser.Id &&
                    i.OnlineVoucherFileId.FileId == id &&
                    i.OnlineVoucherShared == true);

            voucherInfo.VoucherAmountNotShared = voucherInfo.VoucherAmount - voucherInfo.VoucherAmountShared;

            if (voucherInfo.VoucherAmountRedeemed > 0)
            {
                voucherInfo.VoucherRedemptionFrequency = await db.OnlineVouchers
                .Where(fi => fi.OnlineVoucherFileId.FileOwner.Id == currentUser.Id)
                .Where(ui => ui.OnlineVoucherFileId.FileId == id)
                .Where(vr => vr.OnlineVoucherRedeemed == true)
                .AverageAsync(c => c.OnlineVoucherRedemptionCounter);
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
        [ResponseType(typeof(OnlineVoucher))]
        public async Task<IHttpActionResult> PostOnlineVoucher(VoucherViewModel vm, string id)
        {
            var proceed = true;

            if (!ModelState.IsValid || vm.VoucherAmount > 10000)
            {
                proceed = false;
            }

            if (proceed == false)
            {
                return BadRequest("Validation failed or you tried to create more than 10000 vouchers at once.");
            }

            var voucherFile = db.BlobUploadModels.Find(new Guid(id));
            LinkedList<OnlineVoucherBulkInsertViewModel> temp = new LinkedList<OnlineVoucherBulkInsertViewModel>();

            for (int i = 0; i < vm.VoucherAmount; i++)
            {
                var voucher = new OnlineVoucherBulkInsertViewModel();

                voucher.OnlineVoucherId = Guid.NewGuid();
                voucher.OnlineVoucherCode = voucher.OnlineVoucherId.ToString().Substring(0, 8);
                voucher.OnlineVoucherCreationDate = DateTime.Now;
                voucher.OnlineVoucherRedeemed = false;
                voucher.OnlineVoucherRedemptionDate = null;
                voucher.OnlineVoucherFileId_FileId = voucherFile.FileId;
                voucher.OnlineVoucherRedemptionCounter = 0;
                voucher.OnlineVoucherShared = false;

                temp.AddFirst(voucher);
            }

            var reader = ToDataTable<OnlineVoucherBulkInsertViewModel>(temp);

            try
            {
                await DoBulkCopy(false, reader, "OnlineVouchers");
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