using MMS.DAL;
using MMS.Models;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class PurchaseOrderLimitController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        MSP_Model objmsps = new MSP_Model();
        // GET: PurchaseOrderLimit
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();

            return View();
        }
        public ActionResult LimitValueDetail(tblPurchaseOrderApprovalLimitValue Model)
        {
            try
            {
                tblPurchaseOrderApprovalLimitValue obj = new tblPurchaseOrderApprovalLimitValue();

                obj.ProjectId = Model.ProjectId;
                obj.LimitValue = Model.LimitValue;
                obj.CreatedBy = Session["EmpID"].ToString();
                obj.CreatedDate = System.DateTime.Now;
                objmms.tblPurchaseOrderApprovalLimitValues.Add(obj);
                objmms.SaveChanges();
                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ex.ToString();
                return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetAllDatas()
        {
            List<POGET_LIMIT_VALUE> result = new List<POGET_LIMIT_VALUE>();
            result = objmsps.GetAllPOLimitValue();

            if (result != null)
            {
                var lst = result.ToList();
                return PartialView("_PartialView_POLimit", lst);
            }
            else
            {
                return View("_EmptyView");
            }
        }
    }
}