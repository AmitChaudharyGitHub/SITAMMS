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

namespace MMS.Controllers
{
    public class Add_Rate_Cap_POController : Controller
    {
        // GET: Add_Rate_Cap_PO
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            var a = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectID.ToString(),
                // Text = x.ProjectID.ToString()
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            });
            ViewBag.prjtid = t;           

            //Item Group Name
            ViewBag.itemgroupname = new SelectList(objmms.tblItemGroupMasters, "ItemGroupID", "GroupName").OrderBy(k => k.Text);
            return View();
        }

        // Json Call to get state
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

        // Get State from DB by country ID
        public IList<tblItemMaster> Getitemname(string GroupId)
        {
            return objmms.tblItemMasters.Where(m => m.ItemGroupID == GroupId).OrderBy(k => k.ItemName).ToList();
        }

        //Unit Name Binding Here

        public JsonResult Getitemnunit(string id)
        {

            var itemssListsu = this.Getitemdetailsses(Convert.ToString(id));
            return Json(itemssListsu, JsonRequestBehavior.AllowGet);
        }

        public string Getitemdetailsses(string ItemID)
        {

            string Unitid = "";

            var data = objmms.tblItemMasters.Where(m => m.ItemID == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.ItemID.Length > 0)
            {
                Unitid = data.UnitID;

                if (Unitid != null)
                {
                    var unitsnames = objmms.tblUnitMasters.Where(m => m.UnitID == Unitid.Trim().ToUpper()).FirstOrDefault();
                    Unitid = unitsnames.UnitName;
                }

            }
            return Unitid;
        }










    }
}