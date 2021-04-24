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
using BusinessObjects.Entity;
using DataAccessLayer;

namespace MMS.Controllers.Price_Cap
{
    public class AddPriceCapController : Controller
    {
        // GET: AddPriceCap
        FactoryManager m = new FactoryManager();
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        ModelServices mobjModel = new ModelServices();
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            return View();
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.PRJID = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName");
            //Item Group Name
            ViewBag.itemgroupname = new SelectList(objmms.tblItemGroupMasters, "ItemGroupID", "GroupName").OrderBy(k => k.Text);
            //Bind top 200 table in grid on page load
            PriceCap_report rrm = new PriceCap_report();
            var data_from_table = (from apcs in objmms.ns_AddPriceCap   
                                   where (apcs.EditedStatus=="No" && apcs.EffectiveStatus=="On")                                                                 
                                   select new PriceCap_report
                                   {
                                        //string s = apcs.ProjectId;
                                        //string[] values = s.Split(',');
                                        //ProjectId = apcs.ProjectId,
                                        Id =apcs.Id,
                                       ItemGroupId = apcs.ItemGroupId,
                                       ItemGroupName =objmms.tblItemGroupMasters.Where(j=>j.ItemGroupID == apcs.ItemGroupId).FirstOrDefault().GroupName,
                                       ItemId = apcs.ItemId,
                                       ItemName =objmms.tblItemMasters.Where(k=>k.ItemID==apcs.ItemId).FirstOrDefault().ItemName,
                                       Rate = apcs.Rate,
                                       EffectiveDate = apcs.EffectiveDate,
                                       ValidUpto=apcs.ValidUpto,
                                       Status = apcs.Status
                                       //ChallanNo = gatenrty.ChallanNo ?? "",
                                       //CreatedDate = apcs.CreatedDate .OrderByDescending(k => k.CreatedDate).
                                   }).OrderByDescending(p=>p.Id).Take(200).ToList();

            rrm.AddPriceCap_Reports = data_from_table;
            return View("Create", rrm);
            
        }
        [HttpPost]
        public ActionResult Create(PriceCap apc)
        {
            ns_AddPriceCap nsapc = new ns_AddPriceCap();
            nsapc.CreatedDate = System.DateTime.Now;
            nsapc.CreatedBy = Session["EmpID"].ToString();
            
            string PRJID = String.Empty;
            string RegTYpeID = String.Empty;
            string[] ss = apc.ProjectId;

            foreach (string value in ss)
            {
                PRJID += value;
                PRJID += ",";
            }
            nsapc.ProjectId = PRJID.TrimEnd(',');

            nsapc.ItemGroupId = apc.ItemGroupId;
            nsapc.ItemId = apc.ItemId;
            nsapc.Rate = apc.Rate;
            nsapc.EffectiveStatus = "On";
            nsapc.EffectiveDate = apc.EffectiveDate;
            nsapc.ValidUpto = apc.ValidUpto;
            bool stsh =Convert.ToBoolean(apc.Status);
            if (stsh == true)
            {
                nsapc.Status = "Approved";
            }
            else {
                nsapc.Status = "Not Approved";
            }           
            nsapc.EditedStatus = "No";


            var checkduplicacy = objmms.ns_AddPriceCap.Where(j => j.ProjectId.Contains(nsapc.ProjectId) && j.ItemGroupId == apc.ItemGroupId && j.ItemId == apc.ItemId).FirstOrDefault();
            if (checkduplicacy == null)
            {
                var i = objmms.ns_AddPriceCap.Add(nsapc);
                if (i != null)
                {
                    if (Request.IsAjaxRequest())
                    {
                        objmms.SaveChanges();
                        return Json("1", JsonRequestBehavior.AllowGet);
                        //return RedirectToAction("Create");
                    }
                    return RedirectToAction("Create");

                }
                else {
                }                
            }
            else
            {
                return Json("2", JsonRequestBehavior.AllowGet);
            }
            return Json("Add Successfully");

        }


        public ActionResult EditPriceCap(int id)
        {
            var data = mobjModel.GetPriceCapDetail(id);

            if (Request.IsAjaxRequest())
            {
                PricecapModel empObj = new PricecapModel();

                string s = data.ProjectId; 
                string[] values = s.Split(',');
               // ViewBag.PRJID2 = values;
                ViewBag.ProjectId = new MultiSelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName", values);
                ViewBag.ItemGroupId = new SelectList(objmms.tblItemGroupMasters, "ItemGroupID", "GroupName", data.ItemGroupId).OrderBy(k => k.Text);
                ViewBag.ItemId = new SelectList(objmms.tblItemMasters, "ItemID", "ItemName", data.ItemId).OrderBy(k => k.Text);              
                empObj.ItemGroupId = data.ItemGroupId;
                empObj.ItemId = data.ItemId;
                empObj.Rate = data.Rate;
                empObj.EffectiveDate = data.EffectiveDate;
                empObj.Status = data.Status;
                //empObj.ModifiedBy = Session["EmpID"].ToString();

                ViewBag.IsUpdate = true;
                return View("_EditPriceCap", empObj);
            }
            else
                return View(data);
        }

        public ActionResult UpdatePriceCap(PricecapModel emp, string Command)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditPriceCap", emp);
            }
            else
            {
                ns_AddPriceCap empObj = new ns_AddPriceCap();
                empObj.Id = emp.Id;
                string PRJID = String.Empty;
                string[] ss = emp.ProjectId;
                foreach (string value in ss)
                {
                    PRJID += value;
                    PRJID += ",";
                }
                empObj.ProjectId = PRJID.TrimEnd(',');               
                empObj.ItemGroupId = emp.ItemGroupId;
                empObj.ItemId = emp.ItemId;
                empObj.Rate = emp.Rate;
                empObj.EffectiveDate = emp.EffectiveDate;
                empObj.Status = emp.Status;
                empObj.ModifiedBy = Session["EmpID"].ToString();              

                bool IsSuccess = mobjModel.UpdatePriceCap(empObj);
                if (IsSuccess)
                {
                    TempData["OperStatus"] = "Record updated succeessfully";
                    ModelState.Clear();
                    return RedirectToAction("Create", "AddPriceCap");
                }
            }

            return PartialView("_EditPriceCap");
        }

    
       public ActionResult UpdatePriceCapDelete(int id)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditPriceCap");
            }
            else
            {              
                bool IsSuccess = mobjModel.UpdatePriceCapDelete(id);
                if (IsSuccess)
                {
                    TempData["OperStatus"] = "Record updated succeessfully";
                    ModelState.Clear();
                    return RedirectToAction("Create", "AddPriceCap");
                }
            }
            
            return PartialView("_EditPriceCap");

        }





        // Json Call to get Group Name
        public JsonResult GetGroupitem(string id)
        {
            List<SelectListItem> groups = new List<SelectListItem>();
            var groupList = this.Getitemname(Convert.ToString(id));
            var stateData = groupList.Select(m => new SelectListItem()
            {
                Text = m.ItemName,
                Value = m.ItemID.ToString(),

            });
            return Json(stateData, JsonRequestBehavior.AllowGet);
        }

        // Get Item from DB by group ID
        public IList<DAL.tblItemMaster> Getitemname(string GroupId)
        {
            return objmms.tblItemMasters.Where(m => m.ItemGroupID == GroupId).OrderBy(k => k.ItemName).ToList();
        }
        public JsonResult GetGroupitems(string id)
        {
            List<SelectListItem> groups = new List<SelectListItem>();
            var groupList = this.Getitemnames(Convert.ToString(id));
            var stateData = groupList.Select(m => new SelectListItem()
            {
                Text = m.ItemName,
                Value = m.ItemID.ToString(),

            });
            return Json(stateData, JsonRequestBehavior.AllowGet);
        }

        // Get Item from DB by group ID
        public IList<DAL.tblItemMaster> Getitemnames(string GroupId)
        {
            return objmms.tblItemMasters.Where(m => m.ItemGroupID == GroupId).OrderBy(k => k.ItemName).ToList();
        }
    }
}