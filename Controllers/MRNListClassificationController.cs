using MMS.DAL;
using MMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace MMS.Controllers
{
    public class MRNListClassificationController : Controller
    {
        // GET: MRNListClassification
        private MMSEntities objmms = new MMSEntities();
        string EmpId = string.Empty;
        Procedure objp = new Procedure();
        public MRNListClassificationController()
        {
            try
            {
                EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();


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
            ViewBag.EmpId = EmpId;
            return View();
        }

        public ActionResult PendingMRN(string ProjId, string PurchaseType,string VendorId , string PurchaseOrderNo)
        {
            List<MMS.ViewModels.PendingMRN> res = new List<ViewModels.PendingMRN>();
            res = null;
            res = objp.GetPendingMRNGST(ProjId, PurchaseType);

            if (VendorId != "0" && PurchaseOrderNo != "")
            {
                res = res.ToList().Where(x => x.VendorID == VendorId && x.PurchaseOrderNo == PurchaseOrderNo).ToList();
            }
            else if (VendorId != "0" && PurchaseOrderNo == "")
            {
                res = res.ToList().Where(x => x.VendorID == VendorId).ToList();
            }
            else if (VendorId == "0" && PurchaseOrderNo != "")
            {
                res = res.ToList().Where(x =>x.PurchaseOrderNo == PurchaseOrderNo).ToList();
            }

            if (res != null)
            {
                return PartialView("_PartialMRN", res);
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }

        public ActionResult UpdatedMRN(string ProjId, string PurchaseType)
        {
            List<MMS.ViewModels.UpdatedMRN> res = new List<ViewModels.UpdatedMRN>();
            res = null;
          //  res = objp.GetUpdatedMRNGST(ProjId, PurchaseType);
            res = objp.GetUpdatedMRNGST(ProjId, Convert.ToString(2));
            if (res != null)
            {
                return PartialView("_TestUpdatedMRN", res);
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }

        public ActionResult PendingMRNTRN(string ProjId, string PurchaseType, string fromSiteId, string PurchaseOrderNo)
        {
          
            List<MMS.ViewModels.PendingTransferMRN> res = new List<ViewModels.PendingTransferMRN>();
            res = null;
            res = objp.GetPendingMRNTRN(ProjId, PurchaseType);
            if (fromSiteId != "0" && PurchaseOrderNo != "")
            {
                res = res.ToList().Where(x => x.ProjectId == fromSiteId && x.IntraTransferNumber == PurchaseOrderNo).ToList();
            }
            else if (fromSiteId != "0" && PurchaseOrderNo == "")
            {
                res = res.ToList().Where(x => x.ProjectId == fromSiteId).ToList();
            }
            else if (fromSiteId == "0" && PurchaseOrderNo != "")
            {
                res = res.ToList().Where(x => x.IntraTransferNumber == PurchaseOrderNo).ToList();
            }
           if (res != null)
            {
                ViewBag.PurchaseType = PurchaseType;
                return PartialView("_PendingTRNMRN", res);
            }
            else
            {
                return PartialView("_EmptyView");
            }
           


        }
      [HttpPost]
        public ActionResult UpdatedMRN_TRN(string ProjId, string PurchaseType, string fromSiteId, string PurchaseOrderNo)
        {
             int recordsTotal = 0;
             var draw = Request.Form.GetValues("draw").FirstOrDefault();
            if (ProjId != null && PurchaseType != null)
            {

                var lst = objp.GetUpdatedMRNGST_TRN(ProjId, Convert.ToString(PurchaseType));
                ViewBag.PurchaseType = PurchaseType;
                if (fromSiteId != "0" && PurchaseOrderNo != "")
                {
                    lst = lst.ToList().Where(x => x.ProjectId == fromSiteId && x.IntraTransferNumber == PurchaseOrderNo).ToList();
                }
                else if (fromSiteId != "0" && PurchaseOrderNo == "")
                {
                    lst = lst.ToList().Where(x => x.ProjectId == fromSiteId).ToList();
                }
                else if (fromSiteId == "0" && PurchaseOrderNo != "")
                {
                    lst = lst.ToList().Where(x => x.IntraTransferNumber == PurchaseOrderNo).ToList();
                }

                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                recordsTotal = lst.ToList().Count();
                var data = lst.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = 0 }, JsonRequestBehavior.AllowGet);
            }


        }


        // for testing Purchase
        [HttpPost]
        public ActionResult UpdatedMRN_NewFormat(string ProjId, string PurchaseType, string VendorId, string PurchaseOrderNo)
        {
            int recordsTotal = 0;
          //  var projId = Request.Form.GetValues("columns[9][search][value]").FirstOrDefault();
          //  var Purchse_typeid = Request.Form.GetValues("columns[10][search][value]").FirstOrDefault();
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            if (ProjId != null && PurchaseType != null )
            {

                var lst = objp.GetUpdatedMRNGST(ProjId, Convert.ToString(PurchaseType));

                if (VendorId != "0" && PurchaseOrderNo != "")
                {
                    lst = lst.ToList().Where(x => x.VendorID == VendorId && x.PurchaseOrderNo == PurchaseOrderNo).ToList();
                }
                else if (VendorId != "0" && PurchaseOrderNo == "")
                {
                    lst = lst.ToList().Where(x => x.VendorID == VendorId).ToList();
                }
                else if (VendorId == "0" && PurchaseOrderNo != "")
                {
                    lst = lst.ToList().Where(x => x.PurchaseOrderNo == PurchaseOrderNo).ToList();
                }







                //Get parameters

                // get Start (paging start index) and length (page size for paging)

                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value
                //  var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                // var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;



              

                



                recordsTotal = lst.ToList().Count();
                var data = lst.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = 0 }, JsonRequestBehavior.AllowGet);
            }
            //  return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);
          
        }


     
    

        // end

        public JsonResult BindEmployeePurchaseType()
        {
            string J = null; string EmpId = string.Empty;
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            string PurchIds = objmms.tblEmployeeMasters.Where(x => x.EmpID == EmpId).ToList().FirstOrDefault().PurchaseType;
            if (PurchIds != null)
            {
                string[] purtyps = PurchIds.Split(',');
                var a = objmms.tblPI_PurchaseType.Where(xx => purtyps.Contains(xx.TrandId.ToString()) && xx.TrandId > 0).Select(y => new { Text = y.PurchaseType, Value = y.TrandId }).OrderBy(s => s.Text).ToList();
                J = a.ToJSON();
                return Json(J, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(J, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult GetMRS(string MRN_NO)
        {
             List<MMS.ViewModels.MRS> lst = null;
          
            lst = objp.GetMRS(MRN_NO);
            if (lst != null)
            {
               
                return Json(lst.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {

                return null;
            }
        }

        public JsonResult GetMRSatTRANS(string MRN_NO, string PurTypeId)
        {
            List<MMS.ViewModels.MRS> lst = null;
            if (PurTypeId == "5")
            { lst = objp.GetMRS_TRANS_Inter(MRN_NO); } else if(PurTypeId == "6") { lst = objp.GetMRS_TRANS_Intra(MRN_NO); }

           
            if (lst != null)
            {

                return Json(lst.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {

                return null;
            }
        }


        public ActionResult GetMRSDetails(string MRN_NO , string PurTypeId)
        {
            string MRNVal = MRN_NO.Replace("#", "/");
            ViewBag.MRNNO = MRNVal;
            ViewBag.PurchType = PurTypeId;
            return View();

        }

        public ActionResult GetItemdetailGrid(decimal Uid)
        {
            List<MMS.ViewModels.GetMSRItemDetail> itemlst = null;
            itemlst = objp.GetMRSItemDeyails(Uid);
            if (itemlst != null)
            {
                return PartialView("_GridItemdetail", itemlst);
            }
            else
            {
                return View("_EmptyView");

            }
        }


        public JsonResult GetVendorListByprojectWise(string ProjectId)
        {
            var lst = objmms.tblVendorMasters.ToList().Where( x => x.PRJID !=null  && x.PRJID.Contains(ProjectId)).Select(x => new { Value = x.VendorID, Text = x.Name }).OrderBy(x=>x.Text).ToList();
            return Json(lst.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFromSiteNamefromTransferList(string PurchaseTypeId, string ProjectId)
        {
            if (PurchaseTypeId == "5")
            {
                var lst = objmms.tblInterStateTransferMasters.Where(x => x.TransferProjectId == ProjectId).Select(x => new { Value = x.ProjectId, Text = (objmms.tblProjectMasters.Where(k => k.PRJID == x.ProjectId).FirstOrDefault().ProjectName) }).Distinct().ToList();
                return Json(lst.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else if (PurchaseTypeId == "6")
            {
                var lst = objmms.tblIntraTransferTaxableMasters.Where(x => x.TransferProjectId == ProjectId).Select(x => new { Value = x.ProjectId, Text = (objmms.tblProjectMasters.Where(k => k.PRJID == x.ProjectId).FirstOrDefault().ProjectName) }).Distinct().ToList();
                return Json(lst.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else { return Json(null, JsonRequestBehavior.AllowGet); }
           
        }

    }
}