using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.Models;
using MMS.DAL;
using System.Data;
using MMS_P.ViewModels;
using MMS.ViewModels;
using System.Data.Entity;
using DataAccessLayer;

namespace MMS.Controllers
{
    public class SourceLocationController : Controller
    {
        FactoryManager m = new FactoryManager();
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: SourceLocation
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var a = objmms.tblProjectMasters.OrderBy(b => b.ProjectName).ToList();
            var t = a.Distinct<tblProjectMaster>().Select(x => new SelectListItem
            {
                Value = x.PRJID.ToString(),
                Text = x.ProjectName.ToString()
            });
            ViewBag.prjtid = t;

            string MaxId = objmms.tblSourceLocations.Max(b => b.LocationId);
            if (MaxId == null)
                MaxId = "LO00000000";
            var v = Common.IncrementID(MaxId, 3);
            

            return View();
        }
        public ActionResult Create()
        {
            var a1 = objmms.tblProjectMasters.OrderBy(b1 => b1.ProjectName).ToList();
            var t1 = a1.Distinct<tblProjectMaster>().Select(x1 => new SelectListItem
            {
                Value = x1.PRJID.ToString(),
                Text = x1.ProjectName.ToString()
            });
            ViewBag.prjtid = t1;

            if (Request.IsAjaxRequest())
            {
                ViewBag.IsUpdate = false;
                return PartialView("Create");
            }
            else

                return View();
        }
    }
    //public ActionResult Create()
    //{
    //    if (Session["EmpID"] == null)
    //    {
    //        return RedirectToAction("Login", "MyAccount");
    //    }

    //    string empId = Session["EmpID"].ToString();

    //    try
    //    {

            

    //                x.ProjectId = Session["ProjectssId"].ToString();

    //                x.CreatedBy = empId;
    //                x.CreatedDate = DateTime.Now;
    //                x.Status_Checked = "Yes";
    //                x.CompanyId = "COMP000001";

    //                //x.MUId = a;
    //                //x.Receive = 0;
    //                //m.GetTbIndentPurchaseOrderChildManager().Add(x);
    //                objmms.ns_Price_Cap_Enable_Item_Group.Add(x);
    //                // objmms.SaveChanges();

               
    //        objmms.SaveChanges();

    //        return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);

    //    }
    //    catch (Exception ex)
    //    {
    //        return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet);
    //    }

    //}
    public static class Common
    {
        public static string IncrementID(string startValue, int numNonDigits)
        {
            string nonDigits = startValue.Substring(0, numNonDigits);
            int len = startValue.Length - numNonDigits;
            int number = int.Parse(startValue.Substring(numNonDigits));
            number++;
            if (number >= Math.Pow(10, len)) number = 1; // start again at 1
            return String.Format("{0}{1:D" + len.ToString() + "}", nonDigits, number);
        }
    }
}