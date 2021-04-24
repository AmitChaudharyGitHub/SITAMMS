using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.DAL;
using System.Data;
using MMS_P.ViewModels;
using MMS.ViewModels;
using System.Data.Entity;
using System.Linq.Dynamic;
using BusinessObjects.Entity;
using DataAccessLayer;
using MMS.Models;


namespace MMS.Controllers
{
    public class Purchase_Order_SlipController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: Purchase_Order_Slip
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();


            //ViewBag.prjtid = new SelectList(unitOfWork.tblProjectRepository.Get(), "PRJID", "ProjectName");
            //ViewBag.prjtid2 = new SelectList(unitOfWork.tblProjectRepository.Get(), "PRJID", "ProjectName");
            //ViewBag.prjtid3 = new SelectList(unitOfWork.tblProjectRepository.Get(), "PRJID", "ProjectName");
            return View();
        }

        [HttpPost]
        public ActionResult Index(tblSpecificTermsAndCondition CI)
        {
            //System.Threading.Thread.Sleep(4000);
            string message = "";
            if (ModelState.IsValid)
            {
                try
                {
                     objmms.tblSpecificTermsAndConditions.Add(CI);
                      objmms.SaveChanges();

                        message = "Successfully Saved!";
                    
                }
                catch (Exception ex)
                {
                    message = "Error! Please try again.";
                }
            }
            else
            {
                message = "Please provide required fields value.";
            }
            if (Request.IsAjaxRequest())
            {
                return new JsonResult { Data = message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                ViewBag.Message = message;
                return View(CI);
            }
        }

        //

        public JsonResult SaveList(List<STC_Data> ListData)
        {
            string empId = Session["EmpID"].ToString();
            try
            {
                List<tblSpecificTermsAndCondition> list = new List<tblSpecificTermsAndCondition>();
                foreach (var item in ListData)
                {
                    tblSpecificTermsAndCondition tbl = new tblSpecificTermsAndCondition();
                    tbl.ProjectId = item.ProjectId;
                    tbl.ProjectName = item.ProjectName;
                    tbl.Statement_Header = item.Statement_Header;
                    tbl.STC_Description = item.STC_Description;
                    tbl.Company_Id = "COMP000001";
                    tbl.Isdeleted = "No";
                    tbl.CreateDate = DateTime.Now;
                    tbl.CreatedBy = empId.ToString();
                    list.Add(tbl);

                }
                objmms.tblSpecificTermsAndConditions.AddRange(list);
                objmms.SaveChanges();
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("2", JsonRequestBehavior.AllowGet);
            }
        }
        public class STC_Data
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
            public string Statement_Header { get; set; }
            public string STC_Description { get; set; }
            public string Company_Id { get; set; }
            public Nullable<System.DateTime> CreateDate { get; set; }
            public string CreatedBy { get; set; }
        }

        //Save data for Specific Instructions

        public class SI_Data
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
            public string Header_Title { get; set; }
            public string SI_Description { get; set; }
            public string CompanyId { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public string CreatedBy { get; set; }
        }

        public JsonResult SI_SaveList(List<SI_Data> ListDatas)
        {
            string empId = Session["EmpID"].ToString();
            try
            {
                List<tblSpecific_Instructions> list2 = new List<tblSpecific_Instructions>();
                foreach (var item in ListDatas)
                {
                    tblSpecific_Instructions tbl = new tblSpecific_Instructions();
                    tbl.ProjectId = item.ProjectId;
                    tbl.ProjectName = item.ProjectName;
                    tbl.Header_Title = item.Header_Title;
                    tbl.SI_Description = item.SI_Description;
                    tbl.CompanyId = "COMP000001";
                    tbl.Isdeleted = "No";
                    tbl.CreatedDate = DateTime.Now;
                    tbl.CreatedBy = empId.ToString();
                    list2.Add(tbl);

                }
                objmms.tblSpecific_Instructions.AddRange(list2);
                objmms.SaveChanges();
                return Json("SIOB", JsonRequestBehavior.AllowGet);
            }
            catch(Exception dbEx)
            {
                return Json("SIOB_error", JsonRequestBehavior.AllowGet);
            }
        }

        //for genral terms and conditions

        public class GTC_Data
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
            public string Header_Title { get; set; }
            public string GTC_Description { get; set; }
            public string CompanyId { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public string CreatedBy { get; set; }
        }


        public JsonResult GTC_SaveList(List<GTC_Data> ListDatas)
        {
            string empId = Session["EmpID"].ToString();
            try
            {
                List<tblGenral_Terms_Conditions> list2 = new List<tblGenral_Terms_Conditions>();
                foreach (var item in ListDatas)
                {
                    tblGenral_Terms_Conditions tbl = new tblGenral_Terms_Conditions();
                    tbl.ProjectId = item.ProjectId;
                    tbl.ProjectName = item.ProjectName;
                    tbl.Header_Title = item.Header_Title;
                    tbl.GTC_Description = item.GTC_Description;
                    tbl.CompanyId = "COMP000001";
                    tbl.Isdeleted = "No";
                    tbl.CreatedDate = DateTime.Now;
                    tbl.CreatedBy = empId.ToString();
                    list2.Add(tbl);

                }
                objmms.tblGenral_Terms_Conditions.AddRange(list2);
                objmms.SaveChanges();
                return Json("GTCOB", JsonRequestBehavior.AllowGet);
            }
            catch (Exception dbEx)
            {
                return Json("GTCOB_error", JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult View_STCData()
        {
            var a = (from pd in objmms.tblSpecificTermsAndConditions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreateDate)
                     select new Specific_Terms_And_Conditions
                     {
                         Id = pd.Id,
                         Statement_Header = pd.Statement_Header,
                         STC_Description = pd.STC_Description,
                         ProjectId = pd.ProjectId

                     }).ToList();


            var totalRows = a.Count();

            var data = new Specific_Terms_And_Conditions_WebGrids()
            {
                TotalRows = totalRows,
                PageSize = 250,
                STC_DATAS = a.ToList()
            };


            if (data != null && data.TotalRows != 0)
            {



                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PartialView_Show_All_Data_STC", data);
                }

                else
                {
                    return PartialView("_PartialView_Show_All_Data_STC", data);
                }

            }

            else
            {
                return View("_EmptyView");
            }

        }

        [HttpPost]
        public ActionResult Delete_STC(tblSpecificTermsAndCondition gtc_tbl)
        {
            if (ModelState.IsValid)
            {
                tblSpecificTermsAndCondition emp = objmms.tblSpecificTermsAndConditions.Single(em => em.Id == gtc_tbl.Id);
                emp.Isdeleted = "Yes";
                // objmms.tblSpecific_Instructions.Add(gtc_tbl);
                objmms.Entry(emp).State = EntityState.Modified;
                objmms.SaveChanges();
            }
            return PartialView("_PartialView_Show_All_Data_STC");
        }



        public ActionResult View_SIData()
        {
            var a = (from pd in objmms.tblSpecific_Instructions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreatedDate)
                     select new Specific_Instruction_Conditions
                     {
                         Id = pd.Id,
                         Header_Title = pd.Header_Title,
                         SI_Description = pd.SI_Description,
                         ProjectId = pd.ProjectId

                     }).ToList();


            var totalRows = a.Count();

            var data = new Specific_Instruction_Conditions_WebGrids()
            {
                TotalRows = totalRows,
                PageSize = 250,
                SI_DATAS = a.ToList()
            };


            if (data != null && data.TotalRows != 0)
            {



                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PartialView_Show_All_Data_SI", data);
                }

                else
                {
                    return PartialView("_PartialView_Show_All_Data_SI", data);
                }

            }

            else
            {
                return View("_EmptyView");
            }

        }

        [HttpPost]
        public ActionResult Delete_SI(tblSpecific_Instructions gtc_tbl)
        {
            if (ModelState.IsValid)
            {
                tblSpecific_Instructions emp = objmms.tblSpecific_Instructions.Single(em => em.Id == gtc_tbl.Id);
                emp.Isdeleted = "Yes";
                // objmms.tblSpecific_Instructions.Add(gtc_tbl);
                objmms.Entry(emp).State = EntityState.Modified;
                objmms.SaveChanges();
            }
            return PartialView("_PartialView_Show_All_Data_SI");
        }


        public ActionResult View_GTCData()
        {
            var a = (from pd in objmms.tblGenral_Terms_Conditions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreatedDate)
                     select new MMS.ViewModels.Genral_Terms_and_conditions
                     {
                         Id = pd.Id,
                         Header_Title = pd.Header_Title,
                         GTC_Description = pd.GTC_Description,
                         ProjectId = pd.ProjectId

                     }).ToList();


            var totalRows = a.Count();

            var data = new Genral_Terms_and_conditions_WebGrids()
            {
                TotalRows = totalRows,
                PageSize = 250,
                GTCDATAS = a.ToList()
            };


            if (data != null && data.TotalRows != 0)
            {



                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PartialView_Show_All_Data_GTC", data);
                }

                else
                {
                    return PartialView("_PartialView_Show_All_Data_GTC", data);
                }

            }

            else
            {
                return View("_EmptyView");
            }

        }


        [HttpPost]
        public ActionResult Delete_GTC(tblGenral_Terms_Conditions gtc_tbl)
        {
            if (ModelState.IsValid)
            {
                tblGenral_Terms_Conditions emp = objmms.tblGenral_Terms_Conditions.Single(em => em.Id == gtc_tbl.Id);
                emp.Isdeleted = "Yes";
                //objmms.tblGenral_Terms_Conditions.Add(gtc_tbl);
                objmms.Entry(emp).State = EntityState.Modified;
                objmms.SaveChanges();
            }
            return PartialView("_PartialView_Show_All_Data_GTC");
        }
























        public ActionResult ShowDetails_Of_STC(string projectid)
        {
            if (projectid == null)
            {
                List<STC_ViewModel> allOrder = new List<STC_ViewModel>();
                // here MyDatabaseEntities is our data context           
                var o = objmms.tblSpecificTermsAndConditions.OrderByDescending(a => a.CreateDate);
                foreach (var i in o)
                {
                    var od = objmms.tblSpecificTermsAndConditions.Where(a => a.ProjectId.Equals(i.ProjectId)).ToList();
                    allOrder.Add(new STC_ViewModel { obStc = i, allDetails = od });
                }

                return PartialView("_Partial_View_For_STC", allOrder);
            }
            else {
                List<STC_ViewModel> allOrder = new List<STC_ViewModel>();
                // here MyDatabaseEntities is our data context           
                var o = objmms.tblSpecificTermsAndConditions.OrderByDescending(a => a.CreateDate);
                foreach (var i in o)
                {
                    var od = objmms.tblSpecificTermsAndConditions.Where(a => a.ProjectId.Equals(i.ProjectId)).ToList();
                    allOrder.Add(new STC_ViewModel { obStc = i, allDetails = od });
                }

                return PartialView("_Partial_View_For_STC", allOrder);
            }
        }
    }
}