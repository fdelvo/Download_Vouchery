using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Download_Vouchery.Models
{
    public class BlobUploadModel
    {
        [Key]
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public long FileSizeInBytes { get; set; }
        public long FileSizeInKb { get { return (long)Math.Ceiling((double)FileSizeInBytes / 1024); } }
        public virtual ApplicationUser FileOwner { get; set; }
    }
}