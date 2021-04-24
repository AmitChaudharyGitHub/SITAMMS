using MMS.DAL;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class PurchaseTypeProjectLimitController : Controller
    {
        // GET: PurchaseTypeProjectLimit
        string EmpId = string.Empty; MMSEntities objmms = new MMSEntities();
        Procedure objs = new Procedure();

        public PurchaseTypeProjectLimitController()
        {
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
        }

        public ActionResult Index()
        {
            ViewBag.EmpId = EmpId;
            return View();
        }

        public JsonResult GetPurchaseType()
        {
            var res = objmms.tblPIPurchasedecisionTypes.ToList().Select(x => new { Text = x.PurchaseSelectionType, Value = x.ID }).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult AddProjectPurchaseLimt(PurchaseWiseProjectLimitViewModel data)
        {
            try
            {

            var Check = objmms.tblPurchaseLimitValueByProjectWises.Where(x => x.ProjectId == data.ProjectId).ToList().Count();
            if (Check > 0)
            {
                return Json(new { Status = "3" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                tblPurchaseLimitValueByProjectWise obj = new tblPurchaseLimitValueByProjectWise();
                obj.CreatedDate = System.DateTime.Now;
                obj.CreatedBy = EmpId;
                obj.ProjectId = data.ProjectId;
                obj.NormalLimitValue = Convert.ToInt32(data.NormalLimitValue);
                obj.NormalPurchaseType = Convert.ToInt32(data.NormalPurchaseType);
                obj.EmengencyLimitValue = Convert.ToInt32(data.EmengencyLimitValue);
                obj.EmengencyPurchaseType = Convert.ToInt32(data.EmengencyPurchaseType);
                objmms.tblPurchaseLimitValueByProjectWises.Add(obj);
                objmms.SaveChanges();
                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);

            }

            }
            catch (Exception ex)
            {
                ex.ToString();
                return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetGrid(string ProjectId)
        {
            List<PurchaseWiseProjectLimitViewModel> result = new List<PurchaseWiseProjectLimitViewModel>();
            result = null;
           
            result = objs.GetPurchaseProjectLimitgrid(ProjectId);
            if (result != null)
            {
                return PartialView("_GetGridDetail", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

    }
}