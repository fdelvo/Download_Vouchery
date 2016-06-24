using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Download_Vouchery.Models
{
    public class OnlineVoucher
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid OnlineVoucherId { get; set; }
        [Required]
        public string OnlineVoucherCode { get; set; }
        [Required]
        public bool OnlineVoucherRedeemed { get; set; }
        [Required]
        public virtual BlobUploadModel OnlineVoucherFileId { get; set; }
        [Required]
        public DateTime OnlineVoucherCreationDate { get; set; }
        public DateTime? OnlineVoucherRedemptionDate { get; set; }
        public int OnlineVoucherRedemptionCounter { get; set; }
        public  string OnlineVoucherEmail { get; set; }
    }
}