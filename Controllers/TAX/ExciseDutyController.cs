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
    public class ExciseDutyController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        Procedure objpro = new Procedure();
        // GET: ExciseDuty
        string EmpId = string.Empty;
        string EmpName = string.Empty;
        string ProjectId = string.Empty;

        public  ExciseDutyController()
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
            List<SelectListItem> objExiceExist = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Select Exice Status", Value = "-1" },
                new SelectListItem { Text = "No", Value = "0" },
                new SelectListItem { Text = "Yes", Value = "1" },
           };
            ViewBag.ExciseDutyStatus = objExiceExist;
            return View();
        }

        public ActionResult AddExcise(ExciseViewModel ex)
        {
            tblMMSExciseDuty obj = new tblMMSExciseDuty();
            var checkEdCode = objmms.tblMMSExciseDuties.Where(x => x.EdCode == ex.EdCode).ToList();
            if (checkEdCode.Count() > 0)
            {
                return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);
            }
            else {
                // save code here.
                int  S = Convert.ToInt16(ex.ExciseStatusExistvalue);
                if (S == 1)
                {
                    obj.CreatedBy = EmpId;
                    obj.CreatedDate = System.DateTime.Now;
                    obj.EdCode = ex.EdCode;
                    obj.EdType = ex.EdType;
                    obj.EdValue = ex.EdValue;
                    obj.IsExcise = Convert.ToBoolean(S);
                    obj.Status = Convert.ToBoolean(1);
                    obj.IsDeleted = Convert.ToBoolean(0);
                    obj.Remarks = ex.Remarks;
                    objmms.tblMMSExciseDuties.Add(obj);
                }
                else {
                    obj.CreatedBy = EmpId;
                    obj.CreatedDate = System.DateTime.Now;
                    obj.EdCode = ex.EdCode;
                    obj.EdType = ex.EdType;
                    obj.EdValue = Convert.ToDecimal(0.00);
                    obj.IsExcise = Convert.ToBoolean(S);
                    obj.EdNumericValue = ex.EdNumericValue.ToString();
                    obj.Status = Convert.ToBoolean(1);
                    obj.IsDeleted = Convert.ToBoolean(0);
                    obj.Remarks = ex.Remarks;
                    objmms.tblMMSExciseDuties.Add(obj);

                }

               
                objmms.SaveChanges();

                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
            }

          
        }

        public ActionResult GetExice()
        {
            List<_ExciseViewModel> result = new List<_ExciseViewModel>();
            result = objpro.GetExcise();
            if (result != null)
            {
                return PartialView("_ExciseDuties",result);
            }
            else {
                return View("_EmptyView"); 

            }
            
        }


        public ActionResult ChangeStatus(string Id)
        {
            int TrId = Convert.ToInt32(Id);
            tblMMSExciseDuty Im = objmms.tblMMSExciseDuties.Where(x => x.TransID == TrId).First();
            Im.Status = Convert.ToBoolean(0);
            objmms.Entry(Im).State = System.Data.Entity.EntityState.Modified;
            objmms.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ChangeActive(string Id)
        {
            int TrId = Convert.ToInt32(Id);
            tblMMSExciseDuty Im = objmms.tblMMSExciseDuties.Where(x => x.TransID == TrId).First();
            Im.Status = Convert.ToBoolean(1);
            objmms.Entry(Im).State = System.Data.Entity.EntityState.Modified;
            objmms.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}