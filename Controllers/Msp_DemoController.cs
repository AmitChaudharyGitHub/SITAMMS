using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class Msp_DemoController : MySiteController
    {
        // GET: Msp_Demo
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}