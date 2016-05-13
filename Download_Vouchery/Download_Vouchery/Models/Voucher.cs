using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Download_Vouchery.Models
{
    public class Voucher
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid VoucherId { get; set; }
        [Required]
        public string VoucherCode { get; set; }
        [Required]
        public bool VoucherRedeemed { get; set; }
        [Required]
        public virtual BlobUploadModel VoucherFileId { get; set; }
        [Required]
        public DateTime VoucherCreationDate { get; set; }
        public DateTime? VoucherRedemptionDate { get; set; }
        public int VoucherRedemptionCounter { get; set; }
    }
}