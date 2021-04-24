using MMS.DAL;
using MMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class GateEntryListClassificationController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        string EmpId = string.Empty;
        Procedure objp = new Procedure();
        // GET: GateEntryListClassification

        public GateEntryListClassificationController()
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


        public ActionResult PendingGRN(string ProjId, string PurchaseType,string VendorId,string PoNo)
        {
            int recordsTotal = 0;
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            List<MMS.ViewModels.PendingAndPartialGRN> res = new List<ViewModels.PendingAndPartialGRN>();
            res = null;
            res = objp.GetPendigGRNGST(ProjId, PurchaseType,VendorId,PoNo);
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = res.ToList().Count();
            var data = res.Skip(skip).Take(pageSize).ToList();
            if (res != null)
            {
              //  return PartialView("_PendingGRN", res);
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            else {
                return PartialView("_EmptyView");
            }
          
        }

        public JsonResult DisapprovedGRN(string ProjectId, string PurchaseType, string VendorId, string PONo)
        {

            try
            {
                var data = objp.GetDisapprovedGRNGST(ProjectId, PurchaseType, VendorId, PONo).ToList();

                return Json(new { Status = 1, Data = data, PType = PurchaseType },JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3, Error=ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PendingGRN_GetOut(string ProjId, string PurchaseType,string VendorId,string PONo)
        {
            int recordsTotal = 0;
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            List<MMS.ViewModels.PendingTRansferGRN> res = new List<ViewModels.PendingTRansferGRN>();
            res = null;
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            if (PurchaseType == "6")
            {
                res = objp.GetPendigTRansferGRNList_Intra(ProjId, PurchaseType, VendorId, PONo);
                recordsTotal = res.ToList().Count();
                var data = res.Skip(skip).Take(pageSize).ToList();
                if (res != null)
                {
                    ViewBag.PurchaseType = PurchaseType;
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
                }
                else { return null; }
            }
            else if (PurchaseType == "5")
            {
                res = objp.GetPendigTRansferGRNList_Inter(ProjId, PurchaseType,VendorId,PONo);
                recordsTotal = res.ToList().Count();
                var data = res.Skip(skip).Take(pageSize).ToList();
                if (res != null)
                {
                    ViewBag.PurchaseType = PurchaseType;
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
                }
                else { return null; }
            }

           
            else
            {
                return PartialView("_EmptyView");
            }

        }

        public ActionResult PartialGRN_GetOut(string ProjId, string PurchaseType,string VendorId,string PONo)
        {
            int recordsTotal = 0;
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            List<MMS.ViewModels.PendingTRansferGRN> res = new List<ViewModels.PendingTRansferGRN>();
            res = null;
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            if (PurchaseType == "6")
            {
                res = objp.GetPartialTRansferGRNList_Intra(ProjId, PurchaseType, VendorId,PONo);
                recordsTotal = res.ToList().Count();
                var data = res.Skip(skip).Take(pageSize).ToList();
                if (res != null)
                {
                    ViewBag.PurchaseType = PurchaseType;
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
                }
                else { return null; }
            }
            else if (PurchaseType == "5")
            {
                res = objp.GetPartialTRansferGRNList_Inter(ProjId, PurchaseType, VendorId, PONo);
                recordsTotal = res.ToList().Count();
                var data = res.Skip(skip).Take(pageSize).ToList();
                if (res != null)
                {
                    ViewBag.PurchaseType = PurchaseType;
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
                }
                else { return null; }
            }


            else
            {
                return PartialView("_EmptyView");
            }
        }


        public ActionResult PartialGRN(string ProjId, string PurchaseType,string VendorId,string PoNo)
        {
            int recordsTotal_Partial = 0;
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            List<MMS.ViewModels.PendingAndPartialGRN> res = new List<ViewModels.PendingAndPartialGRN>();
            res = null;
            res = objp.GetPartialGRNGST(ProjId, PurchaseType,VendorId,PoNo);
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal_Partial = res.ToList().Count();
            var data_partial = res.Skip(skip).Take(pageSize).ToList();
            if (res != null)
            {
                // return PartialView("_PendingGRN", res);
                return Json(new { draw = draw, recordsFiltered = recordsTotal_Partial, recordsTotal = recordsTotal_Partial, data = data_partial }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }

        public ActionResult UpdatedGRN(string ProjId, string PurchaseType,string VendorId,string PONo)
        {
            int recordsTotal_Updated = 0;
            var draw_Updated = Request.Form.GetValues("draw").FirstOrDefault();
            List<MMS.ViewModels.UpdatedGRN> res = new List<ViewModels.UpdatedGRN>();
            res = null;
            res = objp.GetUpdatedGRNGST(ProjId, PurchaseType,VendorId,PONo);
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal_Updated = res.ToList().Count();
            var data_Updated = res.Skip(skip).Take(pageSize).ToList();
            if (res != null)
            {
                // return PartialView("_UpdatedGRN", res);
                return Json(new { draw = draw_Updated, recordsFiltered = recordsTotal_Updated, recordsTotal = recordsTotal_Updated, data = data_Updated }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }


        public ActionResult UpdatedGRN_GetOut(string ProjId, string PurchaseType,string VendorId,string PONo)
        {
            int recordsTotal = 0;
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            List<MMS.ViewModels.PendingTRansferGRN> res = new List<ViewModels.PendingTRansferGRN>();
            res = null;
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            if (PurchaseType == "6")
            {
                res = objp.GetUpdatedTRansferGRNList_Intra(ProjId, PurchaseType,VendorId,PONo);
                recordsTotal = res.ToList().Count();
                var data = res.Skip(skip).Take(pageSize).ToList();
                if (res != null)
                {
                    ViewBag.PurchaseType = PurchaseType;
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
                }
                else { return null; }
            }
            else if (PurchaseType == "5")
            {
                res = objp.GetUpdatedTRansferGRNList_Inter(ProjId, PurchaseType, VendorId, PONo);
                recordsTotal = res.ToList().Count();
                var data = res.Skip(skip).Take(pageSize).ToList();
                if (res != null)
                {
                    ViewBag.PurchaseType = PurchaseType;
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
                }
                else { return null; }
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