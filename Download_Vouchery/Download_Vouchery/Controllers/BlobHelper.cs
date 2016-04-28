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
    }
}