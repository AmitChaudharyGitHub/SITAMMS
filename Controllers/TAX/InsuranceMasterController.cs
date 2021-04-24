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
    public class InsuranceMasterController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        Procedure objpro = new Procedure();

        string EmpId = string.Empty;
        string EmpName = string.Empty;
        string ProjectId = string.Empty;

        public InsuranceMasterController()
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


        // GET: InsuranceMaster
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

        public ActionResult AddInsurance(InsuranceViewModel I)
        {
            tblMMSInsuranceMaster obj = new tblMMSInsuranceMaster();
            var checkInsuranceCode = objmms.tblMMSInsuranceMasters.Where(x => x.InsuranceCode == I.InsuranceCode).ToList();
            if (checkInsuranceCode.Count() > 0)
            {
                return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);
            }
            else {
                int S = Convert.ToInt16(I.InsuranceExistanceType);

                if (S == 1)
                {
                    obj.CreatedBy = EmpId;
                    obj.CreatedDate = System.DateTime.Now;
                    obj.InsuranceCode = I.InsuranceCode;
                    obj.InsuranceType = I.InsuranceType;
                    obj.InsuranceValue = Convert.ToDecimal(I.InsuranceValue);
                    obj.IsInsurance = Convert.ToBoolean(S);
                    obj.Status = Convert.ToBoolean(1);
                    obj.IsDeleted = Convert.ToBoolean(0);
                    obj.InsuranceDescription = I.InsuranceDescription;
                    objmms.tblMMSInsuranceMasters.Add(obj);
                }
                else {
                    obj.CreatedBy = EmpId;
                    obj.CreatedDate = System.DateTime.Now;
                    obj.InsuranceCode = I.InsuranceCode;
                    obj.InsuranceType = I.InsuranceType;
                    obj.InsuranceValue = Convert.ToDecimal(0.00);
                    obj.IsInsurance = Convert.ToBoolean(S);
                    obj.Status = Convert.ToBoolean(1);
                    obj.IsDeleted = Convert.ToBoolean(0);
                    obj.InsuranceDescription = I.InsuranceDescription;
                    obj.InsuranceTextValue =I.InsuranceTextValue.ToString();
                    objmms.tblMMSInsuranceMasters.Add(obj);
                }


                objmms.SaveChanges();

                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);

            }
           
        }
        public ActionResult GetInsurance()
        {
            List<InsuranceViewModel> result = new List<InsuranceViewModel>();
            result = objpro.GetInsurance();
            if (result != null)
            {
                return PartialView("_Insurance", result);
            }
            else {
                return View("_EmptyView");

            }

        }

        public ActionResult ChangeStatus(string Id)
        {
            int TrId = Convert.ToInt32(Id);
            tblMMSInsuranceMaster Im = objmms.tblMMSInsuranceMasters.Where(x => x.TransId == TrId).First();
            Im.Status = Convert.ToBoolean(0);
            objmms.Entry(Im).State = System.Data.Entity.EntityState.Modified;
            objmms.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ChangeActive(string Id)
        {
            int TrId = Convert.ToInt32(Id);
            tblMMSInsuranceMaster Im = objmms.tblMMSInsuranceMasters.Where(x => x.TransId == TrId).First();
            Im.Status = Convert.ToBoolean(1);
            objmms.Entry(Im).State = System.Data.Entity.EntityState.Modified;
            objmms.SaveChanges();
            return RedirectToAction("Index"); 
        }
    }
}