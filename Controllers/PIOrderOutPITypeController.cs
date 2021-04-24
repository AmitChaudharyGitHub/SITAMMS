using MMS.DAL;
using MMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class PIOrderOutPITypeController : Controller
    {
        // GET: PIOrderOutPIType
        private MMSEntities objmms = new MMSEntities();
        Procedure objmsr = new Procedure();
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            ViewBag.EmpId = empId;
            return View();
        }

        

        public ActionResult ApprvPiGridWithPurchase(string PRJID, string PurchType)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            result = objmsr.GetPIDetailWithPurchase(PRJID, PurchType);

            if (result != null)
            {
                return PartialView("_Partial_PI", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

        public ActionResult Partial_POWithPurchase(string PRJID, string PurchType)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            result = objmsr.GetPartialPo_DetailWithPurchase(PRJID, PurchType);
            if (result != null)
            {
                return PartialView("_Partial_PO", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }
        public ActionResult Updated_POWithPurchase(string PRJID, string PurchType)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            result = objmsr.GetUpdatedPo_DetailWithPurchase(PRJID, PurchType);
            if (result != null)
            {
                return PartialView("_Updated_PO", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }
        public JsonResult BindEmployeePurchaseType()
        {
            string J = null; string EmpId = string.Empty;
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            string PurchIds = objmms.tblEmployeeMasters.Where(x => x.EmpID == EmpId).ToList().FirstOrDefault().PurchaseType;
            if (PurchIds != null)
            {
                string[] purtyps = PurchIds.Split(',');
                var a = objmms.tblPI_PurchaseType.Where(xx => purtyps.Contains(xx.TrandId.ToString()) && xx.TrandId >=3).Select(y => new { Text = y.PurchaseType, Value = y.TrandId }).OrderBy(s => s.Text).ToList();
                J = a.ToJSON();
                return Json(J, JsonRequestBehavior.AllowGet);


            }
            else
            {
                return Json(J, JsonRequestBehavior.AllowGet);

            }
        }

    }
}