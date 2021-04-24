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
    public class AddRegionMasterController : Controller
    {

        // GET: AddRegionMaster
        string EmpId = string.Empty;
        Procedure objpro = new Procedure();
        private MMSEntities objmms = new MMSEntities();
        public AddRegionMasterController()
        {

            try
            {
               EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
           

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public ActionResult Index()
        {
         
            return View();
        }

        public JsonResult GetAutoRegionId()
        {
            string id = string.Empty;
             id = objpro.GetMaxRegionId();
            if (id == null)
            {
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(id, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult InsertRegion(RegionMaster RM)
        {
            tblRegion tbrg = new tblRegion();
            tbrg.RegionID = RM.RegionID;
            tbrg.RegionName = RM.RegionName;
            tbrg.RegionID = RM.RegionID;
            tbrg.RegionCode = RM.RegionCode;
            tbrg.CreatedBy = EmpId;
            tbrg.CreatedDate = System.DateTime.Now;
            tbrg.CompanyID = Session["CompanyId"].ToString();
            tbrg.Status = "E";
            objmms.tblRegions.Add(tbrg);
            objmms.SaveChanges();
            return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetGrid()
        {
            List<RegionMaster> rf = null;
            rf = objpro.GetRegionGrid();
            if (rf != null)
            {
                return PartialView("_RegionGrid", rf);
            }
            else {
                return PartialView("_EmpityView");
            }
        }
    }
}