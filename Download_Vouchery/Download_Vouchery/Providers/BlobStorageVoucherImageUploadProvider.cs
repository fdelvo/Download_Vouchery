using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Download_Vouchery.Helpers;
using Download_Vouchery.Models;

namespace Download_Vouchery.Providers
{
    public class BlobStorageVoucherImageUploadProvider : MultipartFileStreamProvider
    {
        public List<BlobUploadModel> Uploads { get; set; }

        public BlobStorageVoucherImageUploadProvider() : base(Path.GetTempPath())
        {
            Uploads = new List<BlobUploadModel>();
        }

        public override Task ExecutePostProcessingAsync()
        {
            // NOTE: FileData is a property of MultipartFileStreamProvider and is a list of multipart
            // files that have been uploaded and saved to disk in the Path.GetTempPath() location.

            foreach (var fileData in FileData)
            {
                // Sometimes the filename has a leading and trailing double-quote character
                // when uploaded, so we trim it; otherwise, we get an illegal character exception
                var fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));

                // Retrieve reference to a blob
                var blobContainer = new BlobHelper().GetBlobSubContainer();

                var blob = blobContainer.GetBlockBlobReference(fileName);

                // Set the blob content type
                blob.Properties.ContentType = fileData.Headers.ContentType.MediaType;

                // Upload file into blob storage, basically copying it from local disk into Azure
                using (var fs = File.OpenRead(fileData.LocalFileName))
                {
                    blob.UploadFromStream(fs);
                }

                // Delete local file from disk
                File.Delete(fileData.LocalFileName);

                // Create blob upload model with properties from blob info
                var blobUpload = new BlobUploadModel
                {
                    FileName = blob.Name,
                    FileUrl = blob.Uri.AbsoluteUri,
                    FileSizeInBytes = blob.Properties.Length
                };

                // Add uploaded blob to the list
                Uploads.Add(blobUpload);
            }

            return base.ExecutePostProcessingAsync();
        }
    }
}