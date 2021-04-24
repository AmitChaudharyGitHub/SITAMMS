using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.DAL;
using MMS.Models;

namespace MMS.Controllers
{
    public class PIOrderController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        MSP_Model objmsps = new MSP_Model();
        // GET: PIOrder
        [Authorize]
        public ActionResult Index(string PId = "")

        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = System.Web.HttpContext.Current.Session["EmpId"].ToString(); 
            ViewBag.EmpId = empId;
            ViewBag.PId = PId;
            //var a = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            //var t = a.Select(x => new SelectListItem
            //{
            //    Value = x.ProjectID.ToString(),

            //    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            //});
            //ViewBag.Project = t;          

            return View();
        }
        public ActionResult ApprovedPIGrid(string PRJID)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
           // string EmpId = Session["EmpID"].ToString();
            result = objmsps.GetPIDetail(PRJID);
           
            if (result != null)
            {
                return PartialView("_Partial_PI", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

        public ActionResult Partial_PO(string PRJID)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            //string EmpId = Session["EmpID"].ToString();
            result = objmsps.GetPartialPo_Detail(PRJID);
            if (result != null)
            {
                return PartialView("_Partial_PO", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

        public ActionResult Updated_PO(string PRJID)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            //  string EmpId = Session["EmpID"].ToString();
            result = objmsps.GetUpdatedPo_Detail(PRJID);
            if (result != null)
            {
                return PartialView("_Updated_PO", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

        public ActionResult ApprvPiGridWithPurchase(string PRJID, string PurchType,string PINo)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            result = objmsps.GetPIDetailWithPurchase(PRJID, PurchType,PINo);

            if (result != null)
            {
                return PartialView("_Partial_PI", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

        public ActionResult Partial_POWithPurchase(string PRJID, string PurchType,string PINo)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            result = objmsps.GetPartialPo_DetailWithPurchase(PRJID, PurchType,PINo);
            if (result != null)
            {
                return PartialView("_Partial_PO", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }
        public ActionResult Updated_POWithPurchase(string PRJID, string PurchType,string PINo)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            result = objmsps.GetUpdatedPo_DetailWithPurchase(PRJID, PurchType,PINo);
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
            string J =null; string EmpId = string.Empty;
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            string PurchIds = objmms.tblEmployeeMasters.Where(x => x.EmpID == EmpId).ToList().FirstOrDefault().PurchaseType;
            if (PurchIds != null)
            {
                string[] purtyps = PurchIds.Split(',');
                var a = objmms.tblPI_PurchaseType.Where(xx => purtyps.Contains(xx.TrandId.ToString())).Select(y => new { Text = y.PurchaseType, Value = y.TrandId }).OrderBy(s => s.Text).ToList();
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