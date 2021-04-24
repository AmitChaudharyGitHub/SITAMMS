using MMS.DAL;
using MMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers.Reports
{
    public class FormattedReportController : Controller
    {
        // GET: FormattedReport
        MMSEntities objmms = new MMSEntities(); string EmpId = string.Empty;
        Procedure objpro = new Procedure();
        public FormattedReportController()
        {
            try
            {
                EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            }
            catch (Exception ex)
            {
                ex.ToJSON();
            }
        }
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult PIdetail()
        {
            return View();
        }

        public ActionResult POdetail()
        {
            return View();
        }

        public ActionResult POPMCdetail()
        {
            return View();
        }
        public ActionResult POQtyDetail()
        {
            return View();
        }
        public ActionResult StockLedgerDetail()
        {
            return View();
        }

    }
}