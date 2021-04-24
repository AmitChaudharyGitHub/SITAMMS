using MMS.DAL;
using MMS.Helpers;
using MMS.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class POManagementController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        private Procedure procedure = new Procedure();
        [AdminCheck]
        public ActionResult Index()
        {
            return View();
        }



        public JsonResult UpdatePO(string PONo)
        {
            if (PONo != null)
            {
                var data = objmms.TbIndentPurchaseOrder_GST.FirstOrDefault(x => x.PurchaseOrderNo == PONo);
                var Uid = data.UId;
                var recdQty = objmms.TbIndentPurchaseOrderChild_GST.Where(x => x.MUId == Uid).Sum(x => x.Receive);
                if(recdQty.HasValue && recdQty.Value > 0)
                {
                    return Json(new { Status = 2 }, JsonRequestBehavior.AllowGet);
                }
                try
                {
                    data.Status = "Not Approved";
                    if (data.SecondLevelApprv_Status != null)
                    {
                        data.SecondLevelApprv_Status = "Not Approved";
                    }
                    else
                    {
                        data.FirstLevelApprv_Status = "Not Approved";
                    }
                    data.ModifiedDate = DateTime.Now;

                    if (data.IsPORelease != null)
                    {
                        data.IsPORelease = null;
                        data.POReleaseBy = null;
                        data.POReleaseDate = null;
                    }

                    objmms.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    objmms.SaveChanges();

                    return Json(new { Status = 1 }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Status = 3,Msg=ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new {Status=3 },JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult GetPODetails(string ProjectId, string PONo)
        {
            var data = procedure.GetPODataForDisapproval(ProjectId, PONo);

            return PartialView(data);
        }

        public JsonResult GetProjects()
        {

            var t = objmms.tblProjectMasters.Where(x=>!x.ProjectName.Contains("Admin") && !x.ProjectName.Contains("Demo")).Select(x => new { Text = x.ProjectName, Value = x.PRJID }).OrderBy(x=>x.Text).ToList();
            return Json(t.ToJSON(),JsonRequestBehavior.AllowGet);
        }
    }
}