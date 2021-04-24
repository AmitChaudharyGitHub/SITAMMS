using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Entity;
using MMS_P.ViewModels;
using DataAccessLayer;
using Newtonsoft.Json;
using System.Net;
using MMS.DAL;
using MMS.ViewModels;
using MMS.Models;

namespace MMS.Controllers
{
    public class ConversionMasterController : Controller
    {

        FactoryManager m = new FactoryManager();
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        MSP_Model objmsps = new MSP_Model();
        string Id = "0";
        string EmpID = null;
        // GET: ConversionMaster


        public ConversionMasterController()
        {
            try
            {
                EmpID = System.Web.HttpContext.Current.Session["EmpID"].ToString();


            }
            catch
            {
            }
        }
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            ViewBag.EmpID = EmpID;
            return View();
        }


        public JsonResult GetIAllUnite()
        {
            var Unite = objmms.tblUnitMasters.ToList().Select(x => new { Text = x.UnitName, Value = x.UnitID }).OrderBy(k => k.Text).ToList();

            return Json(Unite, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSourceLocationProjectWise(string PrID)
        {
            var SLItem = objmms.tblSourceLocations.Where(x => x.ProjectId == PrID).ToList().Select(p => new { Text = p.Location, Value = p.LocationId }).OrderBy(k => k.Text).ToList();
            return Json(SLItem, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllItemGroup()
        {
            var ItemGrp = objmms.tblItemGroupMasters.ToList().Select(x => new { Text = x.GroupName, Value = x.ItemGroupID }).OrderBy(k => k.Text).ToList();
            return Json(ItemGrp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllItemGroupWise(string GrpId)
        {
            var ItemDetail = objmms.tblItemMasters.Where(x => x.ItemGroupID == GrpId).ToList().Select(p => new { Text = p.ItemName, Value = p.ItemID }).OrderBy(k => k.Text).ToList();
            ViewBag.ddlItemName = new SelectList(ItemDetail.ToList(), "ItemID", "ItemName");
            return Json(ItemDetail, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetAllFromConversionUnit()
        {
            var unite = objmms.tblUnitConversionMasters.ToList().Select(x => new { Text = x.UnitName, Value = x.UnitCode }).Distinct().OrderBy(k => k.Text).ToList();
            return Json(unite, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUniteConversion(string UnitId)
        {
            var conU = objmms.tblUnitConversionMasters.Where(x => x.UnitCode == UnitId).ToList().Select(k => new { Text = k.UnitConversionName, Value = k.UnitConversionCode }).OrderBy(p => p.Text).ToList();
            return Json(conU, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRate(string To_UnitConversionId, string From_UnitId)
        {
            var Gr = objmms.tblUnitConversionMasters.Where(x => x.UnitCode == From_UnitId && x.UnitConversionCode == To_UnitConversionId).Select(p => p.UnitRate).FirstOrDefault();
            return Json(Gr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Index(ConversionViewModel ConMd)
        {
            tblUnitConversionMaster uc = new tblUnitConversionMaster();
            uc.CreatedBy = Session["EmpID"].ToString();
            uc.CreatedDate = System.DateTime.Now;
            uc.Status = "E";
            uc.UnitCode = ConMd.UnitCode.ToString();
            uc.UnitName = ConMd.UnitName.ToString();
            uc.UnitConversionCode = ConMd.UnitConversionCode.ToString();
            uc.UnitConversionName = ConMd.UnitConversionName.ToString();
            uc.UnitRate = ConMd.UnitRate;

            var duplicate = objmms.tblUnitConversionMasters.Where(x => x.UnitCode == ConMd.UnitCode && x.UnitConversionCode == ConMd.UnitConversionCode).FirstOrDefault();
            if (duplicate == null)
            {
                objmms.tblUnitConversionMasters.Add(uc);
                objmms.SaveChanges();
                // return Json("1", JsonRequestBehavior.AllowGet);
                return RedirectToAction("Index");
            }
            else {
                return Json("2", JsonRequestBehavior.AllowGet);
            }

            // return null;

        }

        public ActionResult _GetConvetedUnit()
        {
            List<tblUnitConversionMaster> dta = new List<tblUnitConversionMaster>();
            dta = objmms.tblUnitConversionMasters.ToList();
            if (dta != null)
            {
                return View("ConversationGrid", dta);
            }
            else {
                return View("_EmptyView");
            }
        }
        public ActionResult _SoureceLocationMaster()
        {
            List<tblSourceLocation> SLData = objmms.tblSourceLocations.ToList();
            if (SLData != null)
            {
                return View("_SourceLocationGrid", SLData);
            }
            else {
                return null;
            }
        }

        public ActionResult SoureceLocationMaster()
        {
            ViewBag.EmpID = EmpID;

            return View();
        }
        [HttpPost]
        public ActionResult SoureceLocationMaster(SourceLocationMaster SM)
        {
            tblSourceLocation SL = new tblSourceLocation();
            SL.CompanyId = Session["CompanyId"].ToString();
            SL.CreatedBy = Session["EmpID"].ToString();
            SL.CreatedDate = System.DateTime.Now;
            SL.Location = SM.Location.Trim().ToString();
            SL.ProjectName = SM.ProjectName.Trim().ToString();
            SL.Status = "E";
            SL.ProjectId = SM.ProjectId;
            var duplicate = objmms.tblSourceLocations.Where(x => x.ProjectId == SM.ProjectId && x.Location == SM.Location).FirstOrDefault();
            if (duplicate == null)
            {
                string MaxId = objmms.tblSourceLocations.Max(a => a.LocationId);
                if (MaxId == null)
                    MaxId = "PSL0000000";
                SL.LocationId = Common.IncrementID(MaxId, 3);
                objmms.tblSourceLocations.Add(SL);
                objmms.SaveChanges();
                return RedirectToAction("SoureceLocationMaster");

            }
            else {
                return Json("2", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ConversionUnitMapping()
        {
            ViewBag.EmpID = EmpID;

            return View();
        }

        public ActionResult Add_Mapped(ConversationMappingViewModel Model)
        {
            tblUniteConversionMapped obj = new tblUniteConversionMapped();
            try
            {
                obj.CompanyId = Session["CompanyId"].ToString();
                obj.IsDeleted = Convert.ToBoolean(0);
                obj.CreatedBy = Session["EmpID"].ToString();
                obj.CreatedDate = System.DateTime.Now;
                obj.IsMapped = Convert.ToBoolean(1);
                obj.Status = "E";
                obj.ProjectId = Model.ProjectId;
                obj.SourceLocationId = Model.SourceLocationId;
                obj.ItemGroupId = Model.ItemGroupId;
                obj.ItemId = Model.ItemId;
                obj.UnitFromId = Model.UnitFromId;
                obj.UnitConversionId = Model.UnitConversionId;
                obj.UnitConversationRate = Model.UnitConversationRate;
                var tt = objmms.tblUniteConversionMappeds.Add(obj);
                objmms.SaveChanges();
                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CreateMapping()
        {
            return View();
        }

        public ActionResult MappedGrid()
        {
            List<GetMappedData> conv = new List<GetMappedData>();
            conv = objmsps.GetMappedData();
            if (conv != null)
            {
                return PartialView("_MappedConversionGrid", conv);

            }
            else {
                return PartialView("_MappedConversionGrid", conv);
            }



        }
    }
}