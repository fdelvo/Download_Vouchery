using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Download_Vouchery.ViewModels
{
    public class VoucherViewModel
    {
        [Required]
        public int VoucherAmount { get; set; }
    }

    public class VoucherBulkInsertViewModel 
    {
        public Guid VoucherId { get; set; }

        public string VoucherCode { get; set; }

        public bool VoucherRedeemed { get; set; }

        public Guid VoucherFileId_FileId { get; set; }

        public DateTime VoucherCreationDate { get; set; }
        public DateTime? VoucherRedemptionDate { get; set; }
    }


    public class BlobUploadModelViewModel
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public long FileSizeInBytes { get; set; }
        public string FileOwner { get; set; }
        public string FileOwnerId { get; set; }
    }
}