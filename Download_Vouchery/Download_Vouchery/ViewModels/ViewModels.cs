using Download_Vouchery.Models;
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

    public class VoucherInfoViewModel
    {
        public int VoucherAmount { get; set; }
        public int VoucherAmountRedeemed { get; set; }
        public int VoucherAmountNotRedeemed { get; set; }
        public double VoucherRedemptionFrequency { get; set; }
    }

    public class VoucherBulkInsertViewModel 
    {
        public Guid VoucherId { get; set; }

        public string VoucherCode { get; set; }

        public bool VoucherRedeemed { get; set; }

        public Guid VoucherFileId_FileId { get; set; }

        public DateTime VoucherCreationDate { get; set; }
        public DateTime? VoucherRedemptionDate { get; set; }

        public int VoucherRedemptionCounter { get; set; }
    }

    public class BlobUploadModelViewModel
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public long FileSizeInBytes { get; set; }
        public long FileSizeInKb { get { return (long)Math.Ceiling((double)FileSizeInBytes / 1024); } }
        public double FileSizeInMb { get { return (double)FileSizeInKb / 1024; } }
        public virtual ApplicationUser FileOwner { get; set; }
    }
}