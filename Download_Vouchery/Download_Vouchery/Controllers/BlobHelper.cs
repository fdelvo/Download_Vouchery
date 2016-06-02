using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Download_Vouchery.Models;
using System.Web.Http;

namespace Download_Vouchery.Controllers
{
    public class BlobHelper : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public UserManager<ApplicationUser> UserManager()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            return manager;
        }

        public CloudBlobContainer GetBlobContainer(string fileOwnerId = "default")
        {
            // Pull these from config
            var blobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];

            var blobStorageContainerName = fileOwnerId;

            if (fileOwnerId == "default")
            {
                blobStorageContainerName = UserManager().FindById(User.Identity.GetUserId()).Id.ToString();
            }
            
            // Create blob client and return reference to the container
            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(blobStorageContainerName);
        }

        public CloudBlobContainer GetBlobSubContainer(string fileOwnerId = "default")
        {
            // Pull these from config
            var blobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];

            var blobStorageContainerName = fileOwnerId;

            if (fileOwnerId == "default")
            {
                blobStorageContainerName = UserManager().FindById(User.Identity.GetUserId()).Id.ToString();
            }

            // Create blob client and return reference to the container
            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();

            blobClient.GetContainerReference("profilepictures");
            var container = blobClient.GetContainerReference("profilepictures");
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            var subContainer = blobClient.GetContainerReference("profilepictures/" + blobStorageContainerName);

            var folders = blobClient
                .GetContainerReference("profilepictures")
                .GetDirectoryReference(blobStorageContainerName)
                .ListBlobs(true);

            foreach (IListBlobItem blob in folders)
            {
                if (blob.GetType() == typeof(CloudBlob) || blob.GetType().BaseType == typeof(CloudBlob))
                {
                    ((CloudBlob)blob).DeleteIfExists();
                }
            }

            return subContainer;
        }
    }
}