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

    public class OnlineVoucherInfoViewModel
    {
        public int VoucherAmount { get; set; }
        public int VoucherAmountRedeemed { get; set; }
        public int VoucherAmountNotRedeemed { get; set; }
        public double VoucherRedemptionFrequency { get; set; }
        public int VoucherAmountShared { get; set; }
        public int VoucherAmountNotShared { get; set; }
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

    public class OnlineVoucherBulkInsertViewModel
    {
        public Guid OnlineVoucherId { get; set; }

        public string OnlineVoucherCode { get; set; }

        public bool OnlineVoucherRedeemed { get; set; }

        public Guid OnlineVoucherFileId_FileId { get; set; }

        public DateTime OnlineVoucherCreationDate { get; set; }
        public DateTime? OnlineVoucherRedemptionDate { get; set; }

        public int OnlineVoucherRedemptionCounter { get; set; }

        public bool OnlineVoucherShared { get; set; }
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