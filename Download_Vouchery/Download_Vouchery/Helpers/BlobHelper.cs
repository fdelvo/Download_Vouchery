using System.Configuration;
using System.Web.Http;
using Download_Vouchery.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Download_Vouchery.Helpers
{
    public class BlobHelper : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public UserManager<ApplicationUser> UserManager()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            return manager;
        }

        public CloudBlobContainer GetBlobContainer(string fileOwnerId = "default")
        {
            // Get ConnectionString from Config
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
            // Get ConnectionString from Config
            var blobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];

            var userProfilePictureBlob = fileOwnerId;

            if (fileOwnerId == "default")
            {
                userProfilePictureBlob = UserManager().FindById(User.Identity.GetUserId()).Id.ToString();
            }

            // Create blob client and return reference to the container
            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();

            // Get profile pictures, create if it doesn't exist, set access to public
            blobClient.GetContainerReference("profilepictures");
            var container = blobClient.GetContainerReference("profilepictures");
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            var subContainer = blobClient.GetContainerReference("profilepictures/" + userProfilePictureBlob);

            // Get the profile picture of the user in his own blob
            var folders = blobClient
                .GetContainerReference("profilepictures")
                .GetDirectoryReference(userProfilePictureBlob)
                .ListBlobs(true);

            // Delete the existing profile picture
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