using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Download_Vouchery.Models;
using Download_Vouchery.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ErikEJ.SqlCe;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Data.Common;
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
        public IQueryable<Voucher> GetVouchers()
        {
            var currentUser = UserManager().FindById(User.Identity.GetUserId());
            return db.Vouchers.Include(fi => fi.VoucherFileId).Include(u => u.VoucherFileId.FileOwner).Where(fi => fi.VoucherFileId.FileOwner.Id == currentUser.Id);
        }

        // GET: api/Vouchers/5
        [ResponseType(typeof(Voucher))]
        public async Task<IHttpActionResult> GetVoucher(Guid id)
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var voucherFile = db.BlobUploadModels.Find(new Guid(id));
            LinkedList<VoucherBulkInsertViewModel> temp = new LinkedList<VoucherBulkInsertViewModel>();

            for (int i = 0; i < vm.VoucherAmount; i++) {
                var voucher = new VoucherBulkInsertViewModel();

                voucher.VoucherId = Guid.NewGuid();
                voucher.VoucherCode = voucher.VoucherId.ToString().Substring(0,8);
                voucher.VoucherCreationDate = DateTime.Now;
                voucher.VoucherRedeemed = false;
                voucher.VoucherRedemptionDate = null;
                voucher.VoucherFileId_FileId = voucherFile.FileId;

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