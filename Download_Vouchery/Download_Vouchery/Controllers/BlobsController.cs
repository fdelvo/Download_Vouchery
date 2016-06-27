using Download_Vouchery.Models;
using Download_Vouchery.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Download_Vouchery.Services;

namespace Download_Vouchery.Controllers
{
    public class BlobsController : ApiController
    {
        // Interface in place so you can resolve with IoC container of your choice
        private readonly IBlobService _service = new BlobService();

        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public UserManager<ApplicationUser> UserManager()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            return manager;
        }


        public async Task<IHttpActionResult> GetBlobs()
        {
            // Get the currently logged in user
            var currentUser = UserManager().FindById(User.Identity.GetUserId());

            // Get user's blobs except his profile picture
            var uploadModels = await _db.BlobUploadModels.Where(o => o.FileOwner.Id == currentUser.Id && !o.FileUrl.Contains("profilepictures")).ToListAsync();

            // Create list for displaying data
            var uploadModelStrippedList = new List<BlobUploadModelViewModel> ();

            // Fill the list with data
            foreach (BlobUploadModel i in uploadModels)
            {
                var uploadModelStripped = new BlobUploadModelViewModel();
                uploadModelStripped.FileId = i.FileId;
                uploadModelStripped.FileName = i.FileName;
                uploadModelStripped.FileOwner = i.FileOwner;
                uploadModelStripped.FileSizeInBytes = i.FileSizeInBytes;

                uploadModelStrippedList.Add(uploadModelStripped);
            }

            return Ok(uploadModelStrippedList);
        }

        public async Task<IHttpActionResult> DeleteBlob(Guid blobId)
        {
            try
            {
                var result = await _service.DeleteBlob(blobId);

                if (result != null)
                {
                    return Ok("File deleted.");
                }

                // Otherwise
                return BadRequest("Deletion failed.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Uploads one or more blob files.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(List<BlobUploadModel>))]
        public async Task<IHttpActionResult> PostBlobUpload()
        {
            try
            {
                // This endpoint only supports multipart form data
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                // Call service to perform upload, then check result to return as content
                var result = await _service.UploadBlobs(Request.Content);
                if (result != null && result.Count > 0)
                {
                    return Ok(result);
                }

                // Otherwise
                return BadRequest("Upload failed.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(List<BlobUploadModel>))]
        public async Task<IHttpActionResult> PostVoucherImage()
        {
            try
            {
                // This endpoint only supports multipart form data
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                // Call service to perform upload, then check result to return as content
                var result = await _service.UploadVoucherImage(Request.Content);
                if (result != null && result.Count > 0)
                {
                    return Ok(result);
                }

                // Otherwise
                return BadRequest("Upload failed.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(BlobUploadModel))]
        [HttpGet]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> VoucherImage()
        {
            // Get currently logged in user
            var currentUser = UserManager().FindById(User.Identity.GetUserId());

            // Get his profile picture
            var voucherImage = await _db.BlobUploadModels.FirstOrDefaultAsync(x => 
                x.FileOwner.Id == currentUser.Id
                && x.FileUrl.Contains("ProfilePicture"));

            return Ok(voucherImage);
        }

        private bool VoucherExists(Guid id)
        {
            return _db.Vouchers.Count(e => e.VoucherId == id) > 0;
        }

        public async Task<HttpResponseMessage> GetBlobDownload(string voucherCode, bool check)
        {
            // Get the voucher with the entered code
            var voucher = _db.Vouchers.FirstOrDefault(i => i.VoucherCode == voucherCode);
            var onlineVoucher = _db.OnlineVouchers.FirstOrDefault(i => i.OnlineVoucherCode == voucherCode);

            // Cancel if voucher doesn't exist
            if (voucher == null && onlineVoucher == null)
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            // Get the blob the voucher refers to
            var blob = voucher == null ? onlineVoucher.OnlineVoucherFileId : voucher.VoucherFileId;

            try
            {
                // Call the IBlobService
                var result = await _service.DownloadBlob(blob.FileId);

                if (result == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                // Reset the stream position; otherwise, download will not work
                result.BlobStream.Position = 0;

                // Create response message with blob stream as its content
                var message = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(result.BlobStream)
                };

                // Set content headers
                message.Content.Headers.ContentLength = result.BlobLength;
                message.Content.Headers.ContentType = new MediaTypeHeaderValue(result.BlobContentType);
                message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = HttpUtility.UrlDecode(result.BlobFileName),
                    Size = result.BlobLength
                };

                if (voucher != null)
                {
                    voucher.VoucherRedeemed = true;
                    if (!check)
                    {
                        voucher.VoucherRedemptionCounter++;
                    }
                    voucher.VoucherRedemptionDate = DateTime.Now;
                }
                else
                {
                    onlineVoucher.OnlineVoucherRedeemed = true;
                    if (!check)
                    {
                        onlineVoucher.OnlineVoucherRedemptionCounter++;
                    }
                    onlineVoucher.OnlineVoucherRedemptionDate = DateTime.Now;

                }

                await _db.SaveChangesAsync();

                return message;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.Message)
                };
            }
        }
    }
}
