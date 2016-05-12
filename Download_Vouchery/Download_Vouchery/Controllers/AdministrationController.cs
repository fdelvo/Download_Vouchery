using Download_Vouchery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Download_Vouchery.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        // GET: Administration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintVouchers()
        {
            return View();
        }
    }
}