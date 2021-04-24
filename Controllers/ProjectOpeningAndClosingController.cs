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
    public class ProjectOpeningAndClosingController : Controller
    {
        // GET: ProjectOpeningAndClosing
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        Procedure objP = new Procedure();
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();

            return View();
        }
        public ActionResult OpeningAndClosing(tblProjectOpeningAndClosingDate Model)
        {
            try
            {
                tblProjectOpeningAndClosingDate obj = new tblProjectOpeningAndClosingDate();
                var dup = objmms.tblProjectOpeningAndClosingDates.Where(x => x.ProjectId == Model.ProjectId && x.OpeningDate == Model.OpeningDate).ToList().FirstOrDefault();
                if (dup == null)
                {
                    var firstopeingdateforproject = objmms.tblProjectOpeningAndClosingDates.Where(x => x.ProjectId == Model.ProjectId).OrderByDescending(x => x.OpeningDate).FirstOrDefault();
                    if (firstopeingdateforproject == null)
                    {
                        obj.CompanyId = Session["CompanyId"].ToString();
                        obj.CreatedBy = Session["EmpID"].ToString();
                        obj.IsDeleted = Convert.ToBoolean(0);
                        obj.CreatedDate = System.DateTime.Now;
                        obj.ProjectId = Model.ProjectId;
                        obj.OpeningDate = Model.OpeningDate;
                        objmms.tblProjectOpeningAndClosingDates.Add(obj);
                        objmms.SaveChanges();
                        return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
                    }
                    else {

                    
                    var lstopeningdate = objmms.tblProjectOpeningAndClosingDates.Where(x => x.ProjectId == Model.ProjectId).OrderByDescending(x => x.OpeningDate).First().OpeningDate ;
                    var check = objmms.tblProjectOpeningAndClosingDates.Where(x => x.ProjectId == Model.ProjectId && Model.OpeningDate < lstopeningdate ).ToList().FirstOrDefault();
                    if (check == null)
                    {

                        obj.CompanyId = Session["CompanyId"].ToString();
                        obj.CreatedBy = Session["EmpID"].ToString();
                        obj.IsDeleted = Convert.ToBoolean(0);
                        obj.CreatedDate = System.DateTime.Now;
                        obj.ProjectId = Model.ProjectId;
                        obj.OpeningDate = Model.OpeningDate;
                        objmms.tblProjectOpeningAndClosingDates.Add(obj);
                        objmms.SaveChanges();
                        return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
                    }
                    else {
                        
                        return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);
                    }
                    }


                }
                else
                { 
                    return Json(new { Status = "3" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                
                ex.ToString();
                return Json(new { Status = "4" }, JsonRequestBehavior.AllowGet);

            }

            
        }

        public ActionResult GetGrid()
        {
            List<GetProjectOpeningDateGrid> result = new List<GetProjectOpeningDateGrid>();
            result = null;
            
            result = objP.GetProjectOenigdateGrid();

            if (result != null)
            {
                return PartialView("_getGrid", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }
    }
}