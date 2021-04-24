using MMS.DAL;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers.TAX
{
    public class GSTController : Controller
    {
        // GET: GST
        string EmpID = string.Empty;
        private MMSEntities objmms = new MMSEntities();
        Procedure objP = new Procedure();
        public GSTController()
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
            return View();
        }

        public JsonResult GetGSTCode()
        {
            string J = string.Empty;
            J = objP.GetGSTCode();
            var K = new { Code=J };
            return Json(K.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddGST(MMSGSTView Obj)
        {
            tblMMSGSTTaxType gst = new tblMMSGSTTaxType();
            var CheckExist = objmms.tblMMSGSTTaxTypes.Where(x => x.TaxSlabCode == Obj.TaxSlabCode).ToList();
            if (CheckExist.Count() > 0)
            {
                return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);
            }
            else {

                gst.TaxType = Obj.TaxType;
                gst.TaxSlabCode = Obj.TaxSlabCode;
                gst.TaxSlab = Obj.TaxSlab;
                gst.TaxSlabPercentage = Obj.TaxSlabPercentage;
                gst.CreatedDate = System.DateTime.Now;
                gst.CreatedBy = EmpID;
                gst.Description = Obj.Description;
                objmms.tblMMSGSTTaxTypes.Add(gst);
                objmms.SaveChanges();
                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
            }

         
        }

        public ActionResult GetMMSGST()
        {
            List<MMSGSTView> result = new List<MMSGSTView>();
            result = objP.GetMMSGSTTaxMaster();
            if (result != null)
            {
                return PartialView("_MMSGSTMasterGrid", result);
            }
            else
            {
                return View("_EmptyView");

            }
        }

        public ActionResult AddGST_master()
        {
            return View();
        }

        public JsonResult GetGSTSLAB()
        {
          
            var res = objmms.tblMMSGSTTaxTypes.ToList().Select(x => new { Text = x.TaxSlab, Value = x.TaxSlabCode }).ToList();
           
            return Json(res,JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddSplitGSTMaster(GSTSplit_TaxMasterViewModel obj)
        {
            tblGST_SplitTaxMaster gs_splt = new tblGST_SplitTaxMaster();
            var checkExist = objmms.tblGST_SplitTaxMaster.Where(x => x.TaxRateType == obj.TaxRateType).ToList();
            if (checkExist.Count() > 0)
            {
                return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);
            }
            else {

                gs_splt.TaxSlabCode = obj.TaxSlabCode;
                gs_splt.TaxSlab = obj.TaxSlab;
                gs_splt.TaxSlabPercentage = objmms.tblMMSGSTTaxTypes.Where(x => x.TaxSlabCode == obj.TaxSlabCode).FirstOrDefault().TaxSlabPercentage;
                gs_splt.CGST = obj.CGST;
                gs_splt.SGST = obj.SGST;
                gs_splt.UGST = obj.UGST;
                gs_splt.IGST = obj.IGST;
                gs_splt.TaxRateType = obj.TaxRateType;
                gs_splt.TaxCode = objP.GetGSTTaxRateCode(); 
                gs_splt.CreatedBy = EmpID;
                gs_splt.CreatedDate = System.DateTime.Now;
                objmms.tblGST_SplitTaxMaster.Add(gs_splt);
                objmms.SaveChanges();
                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
            }
           
        }

        public ActionResult GetSplitGSTMaster()
        {
            List<GSTSplit_TaxMasterViewModel> result = new List<GSTSplit_TaxMasterViewModel>();
            result = objP.GetSplitGSTTaxMaster();
            if (result != null)
            {
                return PartialView("_SplitGSTViewGrid", result);
            }
            else
            {
                return View("_EmptyView");

            }
        }


    }
}