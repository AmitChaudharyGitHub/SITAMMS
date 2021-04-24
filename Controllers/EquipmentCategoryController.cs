using BusinessObjects.Entity;
using DataAccessLayer;
using MMS_P.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MMS.DAL;
using MMS.Models;
using MMS.ViewModels;
using System.Data.Entity;

namespace MMS.Controllers
{
    public class EquipmentCategoryController : Controller
    {
        FactoryManager m = new FactoryManager();
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        string EmpID = null;
        public EquipmentCategoryController()
        {
            try
            {
                EmpID = System.Web.HttpContext.Current.Session["EmpID"].ToString();
            }
            catch
            {
            }
        }
        [Authorize]
        // GET: EquipmentCategory
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult GetCategory()
        {
            EquipmentCategory rrm = new EquipmentCategory();
            var data_from_table = (from e in objmms.tblEquipmentCategories
                                   orderby e.Category
                                   select new EquipmentCategory
                                   {
                                       TransId = e.TransId,
                                       Category = e.Category
                                   }).ToList();

            rrm.Category_Item = data_from_table;
            if (rrm.Category_Item.Count > 0)
            {
                return PartialView("_EquipmentCategoryGrid", rrm);
            }
            else
            {
                return View("_EmptyView");
            }
        }
        public ActionResult Create()
        {
            return View();
        }
    }
}