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
using System.Linq.Dynamic;

namespace MMS.Controllers
{
    public class AdminHomeController : Controller
    {

        private MMSEntities objmms = new MMSEntities();
        // GET: AdminHome
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            var Prject = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).First().ProjectID;
            //var t = a.Select(x => new SelectListItem
            //{
            //    //Value = x.ProjectID.ToString(),
            //    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            //});
            ViewBag.prjtidname = objmms.tblProjectMasters.Where(b => b.PRJID == Prject).First().ProjectName;

            //for total count projects
            var totalProjects = objmms.tblProjectMasters.Where(x=>x.PRJID != "AHL0000000").Count();
            ViewBag.Totalprjc = totalProjects;

            //for top project details code here
            //var projectDetails = objmms.tblProjectParticularsDetailAs.OrderByDescending(c=> c.CreatedDate).Select(b => new { b.NameOfProject, b.OriginalContractValue }).Take(5);
            //ViewBag.Pjectsdata = projectDetails;


            ViewBag.prjtid = new SelectList(objmms.tblProjectMasters, "PRJID", "ProjectName").OrderBy(k => k.Text);
           
            return View();
        }

        public JsonResult GetAllDMRDetails()
        {
            var detaildmr = objmms.GateEntryChilds.OrderByDescending(n => n.CreatedDate).Select(x => new
            {
                Name = x.Item,
                Quantity = x.ReceiveQty,
                Po = x.StatusTypeNo
            }).ToList().Take(5);
            return Json(detaildmr, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProjectContractDetails()
        {
            
            var contacts = objmms.tblProjectParticularsDetailAs.OrderByDescending(n=> n.CreatedDate).Select(x => new
            {               
                Name = x.NameOfProject,             
                Value = x.OriginalContractValue
            }).ToList().Take(5);
            return Json(contacts, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Grid(string PId = null)
        {
            List<DashView> d = ListDashView(PId);
            Session["PId1"] = PId;
            return PartialView("_AllDetailsOfitems", d);
        }

        public ActionResult GridPartial(string PId = null, string GId = null)
        {
            var s = Session["PId1"].ToString();
            DashView2 d = DashView22(s, GId);

            return PartialView("_ShowItemDetails", d);
        }

        public List<DashView> ListDashView(string ProjectNo)
        {
            List<DashView> d = new List<ViewModels.DashView>();

            var GroupList = from T1 in objmms.tblItemGroupMasters.Where(a => a.GroupName == "CEMENT" || a.GroupName == "CONSUMABLE" || a.GroupName == "ADMIXTURE" || a.GroupName == "LUBRICANT AND FUEL" || a.GroupName == "MS SHUTTERING (SCAFFOLDING MATERIAL)" || a.GroupName == "STEEL" || a.GroupName == "DIESEL" || a.GroupName == "RAW MATERIAL").OrderByDescending(a => a.ItemGroupID)

                            select new
                            {
                                T1.ItemGroupID
                            };


            foreach (var a in GroupList)
            {

                d.Add(DashView(ProjectNo, a.ItemGroupID));
            }
            return d;


        }

        public DashView2 DashView22(string ProjectNo, string GroupNo)
        {

            if (ProjectNo == null || ProjectNo == "0" || ProjectNo == "")
            {



                var C = from T1 in objmms.tblItemCurrentStocks.Where(a => a.ItemGroupNo == GroupNo)
                        join T2 in objmms.tblItemMasters on T1.ItemNo equals T2.ItemID

                        group T1 by new { T1.ItemNo, T2.ItemName } into g
                        select new Child
                        {
                            ItemId = g.Key.ItemNo,
                            ItemName = g.Key.ItemName,
                            Qty = g.Sum(t => t.Qty)
                        };


                DashView2 DashView = new DashView2() { C = C.ToList() };
                return DashView;

            }
            else
            {


                var C = from T1 in objmms.tblItemCurrentStocks.Where(a => a.ProjectNo == ProjectNo && a.ItemGroupNo == GroupNo)
                        join T2 in objmms.tblItemMasters on T1.ItemNo equals T2.ItemID

                        group T1 by new { T1.ItemNo, T2.ItemName } into g
                        select new Child
                        {
                            ItemId = g.Key.ItemNo,
                            ItemName = g.Key.ItemName,
                            Qty = g.Sum(t => t.Qty)
                        };


                DashView2 DashView = new DashView2()
                {

                    C = C.ToList()
                };
                return DashView;




            }

        }
        public DashView DashView(string ProjectNo, string GroupNo)
        {

            if (ProjectNo == null || ProjectNo == "0" || ProjectNo == "")
            {
                var M = from T1 in objmms.tblItemCurrentStocks.Where(a => a.ItemGroupNo == GroupNo)
                        join T2 in objmms.tblItemGroupMasters on T1.ItemGroupNo equals T2.ItemGroupID

                        group T1 by new { T1.ItemGroupNo, T2.GroupName } into g
                        select new Master
                        {
                            GroupId = g.Key.ItemGroupNo,
                            GroupName = g.Key.GroupName,
                            Qty = g.Sum(t => t.Qty)
                        };


                var C = from T1 in objmms.tblItemCurrentStocks.Where(a => a.ItemGroupNo == GroupNo)
                        join T2 in objmms.tblItemMasters on T1.ItemNo equals T2.ItemID

                        group T1 by new { T1.ItemNo, T2.ItemName } into g
                        select new Child
                        {
                            ItemId = g.Key.ItemNo,
                            ItemName = g.Key.ItemName,
                            Qty = g.Sum(t => t.Qty)
                        };


                DashView DashView = new DashView() { M = M.First(), C = C.ToList() };
                return DashView;

            }
            else
            {
                var M = from T1 in objmms.tblItemCurrentStocks.Where(a => a.ProjectNo == ProjectNo && a.ItemGroupNo == GroupNo)
                        join T2 in objmms.tblItemGroupMasters on T1.ItemGroupNo equals T2.ItemGroupID

                        group T1 by new { T1.ItemGroupNo, T2.GroupName } into g
                        select new Master
                        {
                            GroupId = g.Key.ItemGroupNo,
                            GroupName = g.Key.GroupName,
                            Qty = g.Sum(t => t.Qty)
                        };

                var C = from T1 in objmms.tblItemCurrentStocks.Where(a => a.ProjectNo == ProjectNo && a.ItemGroupNo == GroupNo)
                        join T2 in objmms.tblItemMasters on T1.ItemNo equals T2.ItemID

                        group T1 by new { T1.ItemNo, T2.ItemName } into g
                        select new Child
                        {
                            ItemId = g.Key.ItemNo,
                            ItemName = g.Key.ItemName,
                            Qty = g.Sum(t => t.Qty)
                        };


                DashView DashView = new DashView()
                {
                    M = M.FirstOrDefault() ?? new Master
                    {
                        GroupId = "0",
                        GroupName = "",
                        Qty = 0
                    },
                    C = C.ToList()
                };
                return DashView;




            }

        }
    }
}