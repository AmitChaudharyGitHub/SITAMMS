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
    public class CartageTypeController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        Procedure objpro = new Procedure();

        string EmpId = string.Empty;
        string EmpName = string.Empty;
        string ProjectId = string.Empty;

        public CartageTypeController()
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
            return View();
        }

       public ActionResult SaveCartage(CartageTypeViewModel Mod)
        {
            tblMMSCartageType cart = new tblMMSCartageType();
            cart.CartageType = Mod.CartageType;
            cart.CreatedDate = System.DateTime.Now;
            cart.CreatedBy = EmpId;
            cart.IsDeleted = Convert.ToBoolean(0);
            cart.Status = Convert.ToBoolean(1);
            objmms.tblMMSCartageTypes.Add(cart);
            objmms.SaveChanges();

            return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowCartageType()
        {
            List<CartageTypeViewModel> c = new List<CartageTypeViewModel>();
            c = objpro.GetCartage();
            if (c != null)
            {
                return PartialView("_CartageType", c);
            }
            else {
                return View("_EmptyView");
            }
            
        }

        public ActionResult ChangeStatus(string Id)
        {
            int TrId = Convert.ToInt32(Id);
            tblMMSCartageType Im = objmms.tblMMSCartageTypes.Where(x => x.TransId == TrId).First();
            Im.Status = Convert.ToBoolean(0);
            objmms.Entry(Im).State = System.Data.Entity.EntityState.Modified;
            objmms.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ChangeActive(string Id)
        {
            int TrId = Convert.ToInt32(Id);
            tblMMSCartageType Im = objmms.tblMMSCartageTypes.Where(x => x.TransId == TrId).First();
            Im.Status = Convert.ToBoolean(1);
            objmms.Entry(Im).State = System.Data.Entity.EntityState.Modified;
            objmms.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}