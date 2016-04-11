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
        public string VoucherFilePath { get; set; }
        [Required]
        public int VoucherAmount { get; set; }
    }
}