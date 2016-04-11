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

namespace Download_Vouchery.Controllers
{
    public class VouchersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Vouchers
        public IQueryable<Voucher> GetVouchers()
        {
            return db.Vouchers;
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

        // POST: api/Vouchers
        [ResponseType(typeof(Voucher))]
        public async Task<IHttpActionResult> PostVoucher(VoucherViewModel voucherVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            for (int i = 0; i < voucherVM.VoucherAmount; i++) { 
                var voucher = new Voucher();

                voucher.VoucherId = Guid.NewGuid();
                voucher.VoucherCode = voucher.VoucherId.ToString().Substring(0, 8);
                voucher.VoucherCreationDate = DateTime.Now;
                voucher.VoucherRedeemed = false;
                voucher.VoucherRedemptionDate = null;
                voucher.VoucherFilePath = voucherVM.VoucherFilePath;

                db.Vouchers.Add(voucher);
                       
            
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (VoucherExists(voucher.VoucherId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
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