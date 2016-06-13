using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Download_Vouchery.Helpers;
using Download_Vouchery.Models;
using Download_Vouchery.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Download_Vouchery.Services
{
    public interface IBlobService
    {
        Task<List<BlobUploadModel>> UploadBlobs(HttpContent httpContent);
        Task<List<BlobUploadModel>> UploadVoucherImage(HttpContent httpContent);
        Task<BlobDownloadModel> DownloadBlob(Guid blobId);
        Task<IHttpActionResult> DeleteBlob(Guid blobId);
    }

    public class BlobService : ApiController, IBlobService
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public UserManager<ApplicationUser> UserManager()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            return manager;
        }

        public async Task<List<BlobUploadModel>> UploadBlobs(HttpContent httpContent)
        {
            // Reference to the UploadProvider
            var blobUploadProvider = new BlobStorageUploadProvider();

            // Get currently logged in user
            var currentUser = UserManager().FindById(User.Identity.GetUserId());

            // Perform upload task
            var list = await httpContent.ReadAsMultipartAsync(blobUploadProvider)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        throw task.Exception;
                    }

                    var provider = task.Result;
                    return provider.Uploads.ToList();
                });

            // Create a BlobUploadModel for every uploaded file
            foreach (var item in list)
            {
                item.FileId = Guid.NewGuid();
                item.FileOwner = currentUser;
                _db.BlobUploadModels.Add(item);
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (BlobUploadModelExists(item.FileId))
                    {
                        throw;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return list;
        }

        // Basicall the same as method UploadBlob() but calls another provider
        public async Task<List<BlobUploadModel>> UploadVoucherImage(HttpContent httpContent)
        {
            var blobUploadProvider = new BlobStorageVoucherImageUploadProvider();

            var currentUser = UserManager().FindById(User.Identity.GetUserId());

            var list = await httpContent.ReadAsMultipartAsync(blobUploadProvider)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        throw task.Exception;
                    }

                    var provider = task.Result;
                    return provider.Uploads.ToList();
                });

            foreach (var item in list)
            {
                if (_db.BlobUploadModels.Any(x => x.FileName == item.FileName))
                {
                    continue;
                }

                if (_db.BlobUploadModels.Any(x => x.FileOwner.Id == currentUser.Id && x.FileUrl.Contains("profilepictures")))
                {
                    var blob = _db.BlobUploadModels.FirstOrDefault(x => x.FileOwner.Id == currentUser.Id && x.FileUrl.Contains("profilepictures"));
                    _db.BlobUploadModels.Remove(blob);
                    await _db.SaveChangesAsync();
                }

                item.FileId = Guid.NewGuid();
                item.FileOwner = currentUser;
                _db.BlobUploadModels.Add(item);
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (BlobUploadModelExists(item.FileId))
                    {
                        throw;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return list;
        }

        private bool BlobUploadModelExists(Guid id)
        {
            return _db.BlobUploadModels.Count(e => e.FileId == id) > 0;
        }

        public string GetBlobName(Guid blobId)
        {
            var name = _db.BlobUploadModels.Find(blobId);

            return name.FileName;
        }

        public async Task<BlobDownloadModel> DownloadBlob(Guid blobId)
        {
            // TODO: You must implement this helper method. It should retrieve blob info
            // from your database, based on the blobId. The record should contain the
            // blobName, which you should return as the result of this helper method.
            var blobName = GetBlobName(blobId);
            var blobTemp = _db.BlobUploadModels.Find(blobId);

            if (!String.IsNullOrEmpty(blobName))
            {
                var container = new BlobHelper().GetBlobContainer(blobTemp.FileOwner.Id);
                var blob = container.GetBlockBlobReference(blobName);

                // Download the blob into a memory stream. Notice that we're not putting the memory
                // stream in a using statement. This is because we need the stream to be open for the
                // API controller in order for the file to actually be downloadable. The closing and
                // disposing of the stream is handled by the Web API framework.
                var ms = new MemoryStream();
                await blob.DownloadToStreamAsync(ms);

                // Strip off any folder structure so the file name is just the file name
                var lastPos = blob.Name.LastIndexOf('/');
                var fileName = blob.Name.Substring(lastPos + 1, blob.Name.Length - lastPos - 1);

                // Build and return the download model with the blob stream and its relevant info
                var download = new BlobDownloadModel
                {
                    BlobStream = ms,
                    BlobFileName = fileName,
                    BlobLength = blob.Properties.Length,
                    BlobContentType = blob.Properties.ContentType
                };

                return download;
            }

            // Otherwise
            return null;
        }

        public async Task<IHttpActionResult> DeleteBlob(Guid blobId)
        {
            var blobName = GetBlobName(blobId);
            var blobTemp = _db.BlobUploadModels.Find(blobId);

            if (!String.IsNullOrEmpty(blobName))
            {
                var container = new BlobHelper().GetBlobContainer(blobTemp.FileOwner.Id);
                var blob = container.GetBlockBlobReference(blobName);

                await blob.DeleteIfExistsAsync();

                var file = _db.BlobUploadModels.Find(blobId);
                var vouchersOfFile = _db.Vouchers.Where(f => f.VoucherFileId.FileId == file.FileId);
                _db.BlobUploadModels.Remove(file);
                _db.Vouchers.RemoveRange(vouchersOfFile);

                await _db.SaveChangesAsync();

                return Ok("File deleted.");
            }

            // Otherwise
            return BadRequest("Could not delete the file.");
        }
    }
}
