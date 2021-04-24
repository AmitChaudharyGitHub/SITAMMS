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
    public class TaxController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        Procedure objpro = new Procedure();
        // GET: ExciseDuty
        string EmpId = string.Empty;
        string EmpName = string.Empty;
        string ProjectId = string.Empty;

        public TaxController()
        {
            try
            {
                ProjectId = System.Web.HttpContext.Current.Session["ProjectssId"].ToString();
                EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
                EmpName = System.Web.HttpContext.Current.Session["Emp_Name"].ToString();

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public ActionResult Index()
        {
            List<SelectListItem> objTaxExist = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Select Tax Status", Value = "-1" },
                new SelectListItem { Text = "No", Value = "0" },
                new SelectListItem { Text = "Yes", Value = "1" },
           };
            ViewBag.TaxStatus = objTaxExist;
            return View();
        }


        public ActionResult AddTax(TaxViewModel ex)
        {
            tblMMSTaxValue obj = new tblMMSTaxValue();
            var checktaxCode = objmms.tblMMSTaxValues.Where(x => x.TaxCode == ex.TaxCode).ToList();
            if (checktaxCode.Count() > 0)
            {
                return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);
            }
            else {
                // save code here.
                int S = Convert.ToInt16(ex.TaxStatusExistvalue);

                if (S == 1)
                {
                    obj.CreatedBy = EmpId;
                    obj.CreatedDate = System.DateTime.Now;
                    obj.TaxCode = ex.TaxCode;
                    obj.TaxType = ex.TaxType;
                    obj.TaxValue = ex.TaxValue;
                    obj.IsTax = Convert.ToBoolean(S);
                    obj.Status = Convert.ToBoolean(1);
                    obj.IsDeleted = Convert.ToBoolean(0);
                    obj.Description = ex.Description;
                    objmms.tblMMSTaxValues.Add(obj);
                }
                else {
                    obj.CreatedBy = EmpId;
                    obj.CreatedDate = System.DateTime.Now;
                    obj.TaxCode = ex.TaxCode;
                    obj.TaxType = ex.TaxType;
                    obj.TaxValue = Convert.ToDecimal(0.00);
                    obj.IsTax = Convert.ToBoolean(S);
                    obj.Status = Convert.ToBoolean(1);
                    obj.IsDeleted = Convert.ToBoolean(0);
                    obj.Description = ex.Description;
                    obj.TaxNumericValue = ex.TaxNumericValue.ToString();
                    objmms.tblMMSTaxValues.Add(obj);
                }

              
                objmms.SaveChanges();

                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
            }


        }


        public ActionResult GetTax()
        {
            List<_TaxViewModel> result = new List<_TaxViewModel>();
            result = objpro.GetTax();
            if (result != null)
            {
                return PartialView("_Taxdetail", result);
            }
            else {
                return View("_EmptyView");

            }

        }

        public ActionResult ChangeStatus(string Id)
        {
            int TrId = Convert.ToInt32(Id);
            tblMMSTaxValue Im = objmms.tblMMSTaxValues.Where(x => x.TransId == TrId).First();
            Im.Status = Convert.ToBoolean(0);
            objmms.Entry(Im).State = System.Data.Entity.EntityState.Modified;
            objmms.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ChangeActive(string Id)
        {
            int TrId = Convert.ToInt32(Id);
            tblMMSTaxValue Im = objmms.tblMMSTaxValues.Where(x => x.TransId == TrId).First();
            Im.Status = Convert.ToBoolean(1);
            objmms.Entry(Im).State = System.Data.Entity.EntityState.Modified;
            objmms.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}