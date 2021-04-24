using DataAccessLayer;
using MMS.DAL;
using MMS_P.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class ManagePIPurchaseTypeController : Controller
    {
        // GET: ManagePIPurchaseType
        MMSEntities objmms = new MMSEntities(); FactoryManager m = new FactoryManager();
        string EmpId = string.Empty;

        public ManagePIPurchaseTypeController()
        {
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
        }
        public ActionResult Index()
        {
          
            ViewBag.EmpId = EmpId;
            return View();
        }

        public JsonResult GetPIPurchaseType(string PI)
        {
            string J = string.Empty;
            if (PI != null)
            {
                var  Pitype = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PI).FirstOrDefault().PurchasePIC_Type;
                if(Pitype !=null)
                {
                    var purchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == Pitype).FirstOrDefault().PurchaseType;
                    var V = new { PurchaseType = purchaseType ?? "N/A" };
                    J = V.ToJSON();
                }

            }
            return Json(J,JsonRequestBehavior.AllowGet);
        }

        public ActionResult ToModifiedPurchaseReqType(string PUrchaseType, string PINo)
        {
           
            PurchaseRequisitionMaster lst = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PINo).FirstOrDefault();
            var IsPOCount = objmms.TbIndentPurchaseOrderMasters.Where(x => x.IndentRefNo == PINo).ToList();
            if (IsPOCount.Count() > 0)
            {
                return Json('2', JsonRequestBehavior.AllowGet);

            }
            else {
                lst.PurchasePIC_Type = Convert.ToInt16(PUrchaseType);
                lst.PurchasePICChangedBy = EmpId;
                objmms.Entry(lst).State = EntityState.Modified;
                objmms.SaveChanges();

                return Json('1', JsonRequestBehavior.AllowGet);
                
            }

            
        }

        public ActionResult GetAllPIDetail(decimal id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessObjects.Entity.PurchaseRequisitionMaster   Master = m.GetPurchaseRequisitionMasterManager().Find(id);

            if (Master.Date != null)
                Master.Date2 = Master.Date.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

            List<BusinessObjects.Entity.PurchaseRequisitionChild> ChildList = m.GetPurchaseRequisitionChildManager().FindByMaster(id);

            var a = objmms.PurchaseRequisitionChilds.Where(b => b.MId == id).ToList();

            var totalRows = ChildList.Count();
            var totRows = a.Count();
           // ViewBag.PurchaseTypeId = Master.PurchasePI_Type;
            var data = new PurchaseIRNew()
            {
                MasterNew = Master,
                ChildNew = a
            };

            var FinalPurchasetypeId = objmms.PurchaseRequisitionMasters.Where(x => x.UId == Master.UId).First().PurchasePIC_Type;
            ViewBag.FinalPurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == FinalPurchasetypeId).First().PurchaseType;

            var PILimit = objmms.tblPurchaseRequisitionApprovalLimitValues.Where(x => x.ProjectId == Master.ProjectNo).OrderByDescending(p => p.TransId).First().LimitValue;
            ViewBag.LastLimitVal = PILimit;


            if (data == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PIDetails", data);
        }

    }
}