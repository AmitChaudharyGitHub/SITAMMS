using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.Models;
using MMS.DAL;
using System.Data;
using MMS_P.ViewModels;

namespace MMS.Controllers
{
    public class ViewSendingIndentRequisitionController : MySiteController
    {
        // GET: ViewSendingIndentRequisition

        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();


        [Authorize]
        public ActionResult Index()
        {
            //ViewBag.prjtid = new SelectList(objmms.tblIndentRequisitions, "ProjectID", "ProjectName");

            var plist = this.objmms.tblIndentRequisitions.Select(x => new SelectListItem
            {
                Value = x.ProjectID,
                Text = x.ProjectName
            }).Distinct();
            ViewBag.prjtid =plist;           
            return View();
        }

        // Get Approved And Unaapproved Data Here
        public JsonResult GetIndentnocheck(string id)
        {
            if (id == "Pending")
            {
                string empId = Session["EmpID"].ToString();

                //var a = objmms.tblIndentRequisitions.Where(b => b.IndentSent == "No" && b.Status == "Pending").GroupBy(b => b.ProjectID).Select(b => new { ProjectId = b.Key }).ToList();

                //var indentData = a.Select(m => new SelectListItem
                //{
                //    Text = objmms.tblProjectMasters.Where(x => x.PRJID == m.ProjectId).First().ProjectName,
                //    Value = m.ProjectId,
                //}).OrderBy(x => x.Text);

                var aaa = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
                var t = aaa.Select(x => new SelectListItem
                {
                    Value = x.ProjectID.ToString(),                  
                    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
                });

                return Json(t, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var a = objmms.tblIndentRequisitions.Where(b => b.IndentSent == "Yes" && b.Status == "Approved").GroupBy(b => b.ProjectID).Select(b => new { ProjectId = b.Key }).ToList();

                //var indentData = a.Select(m => new SelectListItem
                //{
                //    Text = objmms.tblProjectMasters.Where(x => x.PRJID == m.ProjectId).First().ProjectName,
                //    Value = m.ProjectId,
                //}).OrderBy(x => x.Text);

                string empId = Session["EmpID"].ToString();
                var aaa = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
                var t = aaa.Select(x => new SelectListItem
                {
                    Value = x.ProjectID.ToString(),
                    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
                });

                return Json(t, JsonRequestBehavior.AllowGet);
               

            }

        }





        public ActionResult Grid(string Status = null, string PId = null)
        {
            if (Status == "Pending")
            {

                var a = objmms.tblIndentRequisitions.Where(i => i.Status == Status && i.ProjectID == PId && i.Apporoved_Status == "No" && i.Status == "Pending").OrderBy(c => c.Id).ToList();

                var totalRows = a.Count();

                var data = new PagedIndentModel()
                {
                    TotalRows = totalRows,
                    //PageSize = 250,
                    Indent = a.Distinct<tblIndentRequisition>().ToList()
                };


                if (data != null && data.TotalRows != 0)
                {


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("Grid", data);
                    }

                    else
                    {
                        return PartialView("_EmptyView");
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
            }
            else {

                var a = objmms.tblIndentRequisitions.Where(i => i.Status == Status && i.ProjectID == PId && i.Apporoved_Status == "Yes" && i.Status == "Approved").OrderBy(c => c.Id).ToList();

                var totalRows = a.Count();

                var data = new PagedIndentModel()
                {
                    TotalRows = totalRows,
                    //PageSize = 250,
                    Indent = a.Distinct<tblIndentRequisition>().ToList()
                };


                if (data != null && data.TotalRows != 0)
                {


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("Grid", data);
                    }

                    else
                    {
                        return PartialView("_EmptyView");
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
            }


        }

       
        public ActionResult Details(string id)
        {
            //if (id != null)
            //{
                var indentNo = id.Replace("#", "/");

                var a1 = objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo).OrderBy(c => c.Id).ToList();
                var totalRows = a1.Count();
              
                var data1 = new PagedIndentAllRecordModel()
                {
                    TotalRows = totalRows,
                    PageSize = 250,
                    Indent2 = a1.ToList()
                };


                if (data1 != null && data1.TotalRows != 0)
                {

                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_ProductPartial", data1);
                    }

                    else
                    {
                        return PartialView("_ProductPartial", data1);
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
           // }

           
        }

        
        
    }
}