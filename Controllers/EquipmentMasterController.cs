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
    public class EquipmentMasterController : Controller
    {
        FactoryManager m = new FactoryManager();
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        MSP_Model objmsps = new MSP_Model();
        string EmpID = null;
        public EquipmentMasterController()
        {
            try
            {
                EmpID = System.Web.HttpContext.Current.Session["EmpID"].ToString();
            }
            catch
            {
            }
        }

        public ActionResult Index()
        {
            return View();
        }
        // GET: EquipmentMaster
        public ActionResult Create()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            ViewBag.EmpID = EmpID;

            //code for status
            List<SelectListItem> ObjStatus = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Select Status", Value = "-1" },
                new SelectListItem { Text = "In Use", Value = "1" },
                new SelectListItem { Text = "In Storage", Value = "2" },
                new SelectListItem { Text = "Out for Repair", Value = "3" },
            };

            ViewBag.Status = ObjStatus;

            //code for status
            List<SelectListItem> ObjFuel = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Select Fuel Type", Value = "-1" },
                new SelectListItem { Text = "CNG", Value = "1" },
                new SelectListItem { Text = "DIESEL", Value = "2" },
                new SelectListItem { Text = "LPG", Value = "3" },
                new SelectListItem { Text = "PETROL", Value = "4" },
            };

            ViewBag.FuelType = ObjFuel;

            //code for condition
            List<SelectListItem> ObjList = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Select Condition", Value = "-1" },
                new SelectListItem { Text = "New", Value = "1" },
                new SelectListItem { Text = "Old", Value = "2" },
            };

            ViewBag.Condition = ObjList;

            //code for bind location
            var a = objmms.tblProjectMasters.OrderBy(c => c.ProjectName).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.PRJID.ToString(),
                Text = x.ProjectName.ToString()
            });
            ViewBag.Location = t;

            //code for bind model

            ViewBag.Model = new SelectList(Enumerable.Range(DateTime.Today.AddYears(-46).Year, 47).Select(x =>

           new SelectListItem()
           {
               Text = x.ToString(),
               Value = x.ToString()
           }), "Value", "Text");

            return View();
        }


        public ActionResult SaveEquipmentData(EquipmentMasterViewModel data)
        {
            tblMMSEquipmentMaster list = new tblMMSEquipmentMaster();
            try
            {
                var registracount = objmms.tblMMSEquipmentMasters.Where(x => x.EquipmentRegNo == data.EquipmentRegNo).FirstOrDefault();
                if (registracount == null)
                {
                    list.ProjectId = data.ProjectId;
                    list.ItemGroupId = data.ItemGroupId;
                    list.ItemId = data.ItemId;
                    list.EquipmentName = data.EquipmentName;
                    list.OwnerName = data.OwnerName;
                    list.EquipmentRegNo = data.EquipmentRegNo;
                    list.PurchaseDate = data.PurchaseDate;
                    list.ManufactureName = data.ManufactureName;
                    list.BodyColor = data.BodyColor;
                    list.BrandName = data.BrandName;
                    list.Model = data.Model;
                    list.EquipmentStatus = data.EquipmentStatus;
                    list.FuelType = data.FuelType;
                    list.EquipmentCondition = data.EquipmentCondition;
                    list.CreatedDate = System.DateTime.Now;
                    list.CreatedBy = Session["EmpID"].ToString();
                    list.Status = Convert.ToBoolean(1);
                    list.IsDeleted = Convert.ToBoolean(0);
                    list.CompanyName = Session["CompanyId"].ToString();
                    objmms.tblMMSEquipmentMasters.Add(list);
                    objmms.SaveChanges();

                    return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Equipment Registration is already exist.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SerachEquipmentMaster()
        {

            return View();
        }

        public ActionResult EquipmentGrid(string GrpId, String ItemId)
        {
            List<EquipmentGrid> list = new List<EquipmentGrid>();
            list = objmsps.GetEuipment(GrpId, ItemId);

            var data = (from a in list
                        select new EquipmentGrid
                        {
                            RowNumber = a.RowNumber,
                            ProjectName = a.ProjectName,
                            GroupName = a.GroupName,
                            ItemName = a.ItemName,
                            EquipmentName = a.EquipmentName,
                            OwnerName = a.OwnerName,
                            BrandName = a.BrandName,
                            EquipmentStatus = a.EquipmentStatus,
                            EquipmentCondition = a.EquipmentCondition,
                            BodyColor = a.BodyColor,
                            Model = a.Model,
                            PurchaseDate = a.PurchaseDate,
                            CreatedDate = a.CreatedDate
                        }).ToList();


              return PartialView("EuipGrid", data);
           

            
        }
    }
}