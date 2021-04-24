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
using System.Linq.Dynamic;
using System.Configuration;
using System.ComponentModel.DataAnnotations;

namespace MMS.Controllers
{
    public class Multiple_Quantity_wise_SearchController : Controller
    {
        // GET: Multiple_Quantity_wise_Search
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        FactoryManager m = new FactoryManager();

        string EmpID = "EMP0000001";
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();

            EmpID = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            ViewBag.EmpID = EmpID;
            //var a = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            //var t = a.Select(x => new SelectListItem
            //{
            //    Value = x.ProjectID.ToString(),
            //    // Text = x.ProjectID.ToString()
            //    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            //});
            //ViewBag.prjtid = t;

            //Item Group Name
            ViewBag.itemgroupname = new SelectList(objmms.tblItemGroupMasters, "ItemGroupID", "GroupName").OrderBy(k => k.Text);



            return View();
        }

        public JsonResult GetProjectByEmpId(string E = "0")
        {

            string j = null;
            if (E != "0" && E != "EMP0000001" && E != "")
            {
                string PrjString = m.GetEmployeeMasterManager().FindPrj(E);


                var Prjlist = m.GetProjectMasterManager().FindListByString(PrjString);

                j = Prjlist.ToJSON();
            }
            else
            {
                var Prjlist = m.GetProjectMasterManager().FindListByString(null);

                j = Prjlist.ToJSON();
            }


            return Json(j, JsonRequestBehavior.AllowGet);
        }


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
        public IList<DAL.tblItemMaster> Getitemname(string GroupId)
        {
            return objmms.tblItemMasters.Where(m => m.ItemGroupID == GroupId).OrderBy(k => k.ItemName).ToList();
        }

        //public JsonResult Get_MultipleQty_Search(string PrjId, string IG, string itemid)
        //{

        //    if (IG != null && itemid != null)
        //    {
        //        var querys = objmms.tblItemMasterStocks.Where(m => m.ProjectNo == PrjId && m.ItemGroup == IG && m.ItemNo == itemid).OrderBy(c => c.Id).Select(xx => new Multiple_Searching_VM
        //        {
        //            Id = xx.Id,
        //            ItemGroupID = xx.ItemGroup,
        //            //ItemGroupName = xx.ItemGroupName,                    
        //            ItemID = xx.ItemNo,
        //            ItemName = objmms.tblItemMasters.Where(l=> l.ItemGroupID ==xx.ItemGroup && l.ItemID ==xx.ItemNo).FirstOrDefault().ItemName,
        //            UnitID = xx.UnitId,
        //            UnitName =objmms.tblUnitMasters.Where(k=> k.UnitID == xx.UnitId).FirstOrDefault().UnitName,
        //            BalanceQuantity=objmms.tblItemCurrentStocks.Where(j=> j.ProjectNo ==xx.ProjectNo && j.ItemGroupNo ==xx.ItemGroup && j.ItemNo == xx.ItemNo).FirstOrDefault().Qty ?? 0,
        //            IssueQuantity = objmms.ns_tbl_IssueQuantity.Where(l=> l.ProjectID== xx.ProjectNo && l.ItemGroupID == xx.ItemGroup && l.ItemID== xx.ItemNo && l.IndentNo != "Opening").FirstOrDefault().IssueQuantity ?? 0,
        //            Opening = objmms.tblItemMasterStocks.Where(k => k.ProjectNo ==xx.ProjectNo &&  k.ItemGroup == xx.ItemGroup && k.ItemNo == xx.ItemNo).FirstOrDefault().Opening ?? 0,
        //            ReceiveQty = objmms.GateEntryChilds.Where(h => h.ProjectNo == xx.ProjectNo && h.ItemGroupNo == xx.ItemGroup && h.ItemNo == xx.ItemNo).FirstOrDefault().ReceiveQty ?? 0,

        //            CreatedDate = xx.CreatedDate.Value
        //        }).ToList();

        //        return Json(querys, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        //var querys = objmms.ns_tbl_IssueQuantity.Where(m => m.ProjectID == PrjId && m.ItemGroupID == IG && m.ItemID == itemid && m.IndentNo != "Opening").OrderBy(c => c.Id).Select(xx => new Multiple_Searching_VM
        //        //{
        //        //    Id = xx.Id,
        //        //    ItemGroupID = xx.ItemGroupID,
        //        //    ItemGroupName = xx.ItemGroupName,
        //        //    ItemName = xx.ItemName,
        //        //    ItemID = xx.ItemID,
        //        //    UnitID = xx.UnitID,
        //        //    //UnitName =objmms.tblUnitMasters.Where(k=> k.UnitID == xx.UnitID).FirstOrDefault().UnitName,
        //        //    BalanceQuantity = objmms.tblItemCurrentStocks.Where(j => j.ProjectNo == xx.ProjectID && j.ItemGroupNo == xx.ItemGroupID && j.ItemNo == xx.ItemID).FirstOrDefault().Qty ?? 0,
        //        //    IssueQuantity = xx.IssueQuantity,
        //        //    Opening = objmms.tblItemMasterStocks.Where(k => k.ProjectNo == xx.ProjectID && k.ItemGroup == xx.ItemGroupID && k.ItemNo == xx.ItemID).FirstOrDefault().Opening ?? 0,
        //        //    ReceiveQty = objmms.GateEntryChilds.Where(h => h.ProjectNo == xx.ProjectID && h.ItemGroupNo == xx.ItemGroupID && h.ItemNo == xx.ItemID).FirstOrDefault().ReceiveQty ?? 0,

        //        //    CreatedDate = xx.CreatedDate
        //        //}).ToList();
        //    }
        //  return null;
        //}




        //public ActionResult Get_All_Addition_Qty_Search(string PrjId, string IG)
        //{

        //    if (PrjId != null && IG != null)
        //    {
        //        //var querys = objmms.tblItemMasterStocks.Where(m => m.ProjectNo == PrjId && m.ItemGroup == IG && m.ItemNo == itemid).OrderBy(c => c.Id).Select(xx => new Multiple_Searching_VM
        //        //{
        //        //    Id = xx.Id,
        //        //    ItemGroupID = xx.ItemGroup,
        //        //    //ItemGroupName = xx.ItemGroupName,                    
        //        //    ItemID = xx.ItemNo,
        //        //    ItemName = objmms.tblItemMasters.Where(l => l.ItemGroupID == xx.ItemGroup && l.ItemID == xx.ItemNo).FirstOrDefault().ItemName,
        //        //    UnitID = xx.UnitId,
        //        //    UnitName = objmms.tblUnitMasters.Where(k => k.UnitID == xx.UnitId).FirstOrDefault().UnitName,
        //        //    BalanceQuantity = objmms.tblItemCurrentStocks.Where(j => j.ProjectNo == xx.ProjectNo && j.ItemGroupNo == xx.ItemGroup && j.ItemNo == xx.ItemNo).FirstOrDefault().Qty ?? 0,
        //        //    IssueQuantity = objmms.ns_tbl_IssueQuantity.Where(l => l.ProjectID == xx.ProjectNo && l.ItemGroupID == xx.ItemGroup && l.ItemID == xx.ItemNo && l.IndentNo != "Opening").FirstOrDefault().IssueQuantity ?? 0,
        //        //    Opening = objmms.tblItemMasterStocks.Where(k => k.ProjectNo == xx.ProjectNo && k.ItemGroup == xx.ItemGroup && k.ItemNo == xx.ItemNo).FirstOrDefault().Opening ?? 0,
        //        //    ReceiveQty = objmms.GateEntryChilds.Where(h => h.ProjectNo == xx.ProjectNo && h.ItemGroupNo == xx.ItemGroup && h.ItemNo == xx.ItemNo).FirstOrDefault().ReceiveQty ?? 0,

        //        //    CreatedDate = xx.CreatedDate.Value
        //        //}).ToList();              
               

        //        var qr = (from iss in objmms.ns_tbl_IssueQuantity
        //                 join gate in objmms.GateEntryChilds on new { a=iss.ProjectID, b=iss.ItemGroupID} equals new { a=gate.ProjectNo, b=gate.ItemGroupNo}
        //                 join opning in objmms.tblItemMasterStocks on new { a = gate.ProjectNo, b = gate.ItemGroupNo } equals new { a=opning.ProjectNo, b=opning.ItemGroup}
        //                 join balstock in objmms.tblItemCurrentStocks on new { a = opning.ProjectNo, b = opning.ItemGroup } equals new { a = balstock.ProjectNo, b = balstock.ItemGroupNo }
        //                 where iss.ProjectID == PrjId && iss.ItemGroupID == IG
        //                  select new Multiple_Searching_VM
        //                 {
        //                     Id = iss.Id,
        //                     ItemGroupID = iss.ItemGroupID,
        //                     ItemID = iss.ItemID,
        //                     ItemName = objmms.tblItemMasters.Where(k => k.ItemID == iss.ItemID).FirstOrDefault().ItemName,
        //                     UnitID = iss.UnitID,

        //                     UnitName = objmms.tblUnitMasters.Where(k => k.UnitID == iss.UnitID).FirstOrDefault().UnitName,
        //                     //BalanceQuantity = objmms.tblItemCurrentStocks.Where(j => j.ProjectNo == PrjId && j.ItemGroupNo == IG && j.ItemNo == itemid).FirstOrDefault().Qty ?? 0,
        //                     IssueQuantity = iss.IssueQuantity,
        //                     Opening = opning.Opening,
        //                     BalanceQuantity = balstock.Qty,
        //                     //ReceiveQty = gate.ReceiveQty
        //                     //Opening = objmms.tblItemMasterStocks.Where(k => k.ProjectNo == xx.ProjectID && k.ItemGroup == xx.ItemGroupID && k.ItemNo == xx.ItemID).FirstOrDefault().Opening ?? 0,
        //                     ReceiveQty = objmms.GateEntryChilds.Where(h => h.ProjectNo == PrjId && h.ItemGroupNo == IG).FirstOrDefault().ReceiveQty ?? 0,
        //                     //CreatedDate = xx.CreatedDate
        //                 }).ToList();

        //        var data = qr;
        //        PrintData dataObj = new PrintData();
        //        dataObj.ItemData = data.Select(x => new ItemData { ItemName = x.ItemName, UnitName = x.UnitName, Opening = x.Opening, IssueQuantity = x.IssueQuantity, ReceiveQty = x.ReceiveQty, BalanceQuantity = x.BalanceQuantity }).ToList();
        //        return PartialView("_PartialView_AllQuantity_Details_Final", dataObj);

        //        //var multiqueys = (from iss in objmms.ns_tbl_IssueQuantity
        //        //                  from gate in objmms.GateEntryChilds
        //        //                  from opning in objmms.tblItemMasterStocks
        //        //                  from balstock in objmms.tblItemCurrentStocks
        //        //                  where (iss.ProjectID == PrjId && iss.ItemGroupID == IG  && iss.IndentNo != "Opening")
        //        //                  && (gate.ProjectNo == PrjId && gate.ItemGroupNo == IG )
        //        //                  && (opning.ProjectNo == PrjId && opning.ItemGroup == IG)
        //        //                  && (balstock.ProjectNo == PrjId && balstock.ItemGroupNo == IG)

        //        //                  select new Multiple_Searching_VM
        //        //                  {
        //        //                      Id = iss.Id,
        //        //                      ItemGroupID = iss.ItemGroupID,
        //        //                      ItemID = iss.ItemID,
        //        //                      ItemName = objmms.tblItemMasters.Where(k => k.ItemID == iss.ItemID).FirstOrDefault().ItemName,
        //        //                      UnitID = iss.UnitID,

        //        //                      UnitName =objmms.tblUnitMasters.Where(k=> k.UnitID == iss.UnitID).FirstOrDefault().UnitName,
        //        //                      //BalanceQuantity = objmms.tblItemCurrentStocks.Where(j => j.ProjectNo == PrjId && j.ItemGroupNo == IG && j.ItemNo == itemid).FirstOrDefault().Qty ?? 0,
        //        //                      IssueQuantity = iss.IssueQuantity,
        //        //                      Opening = opning.Opening,
        //        //                      BalanceQuantity = balstock.Qty,
        //        //                      ReceiveQty = gate.ReceiveQty
        //        //                      //Opening = objmms.tblItemMasterStocks.Where(k => k.ProjectNo == xx.ProjectID && k.ItemGroup == xx.ItemGroupID && k.ItemNo == xx.ItemID).FirstOrDefault().Opening ?? 0,
        //        //                      //ReceiveQty = objmms.GateEntryChilds.Where(h => h.ProjectNo == xx.ProjectID && h.ItemGroupNo == xx.ItemGroupID && h.ItemNo == xx.ItemID).FirstOrDefault().ReceiveQty ?? 0,
        //        //                      //CreatedDate = xx.CreatedDate

        //        //                  }).ToList();

        //        //var data = multiqueys;
        //        //PrintData dataObj = new PrintData();
        //        //dataObj.ItemData = data.Select(x => new ItemData { ItemName = x.ItemName, UnitName = x.UnitName, Opening = x.Opening, IssueQuantity = x.IssueQuantity, ReceiveQty = x.ReceiveQty, BalanceQuantity = x.BalanceQuantity}).ToList();
        //        //return PartialView("_PartialView_AllQuantity_Details_Final", dataObj);

        //    }
        //    //    var querys = objmms.ns_tbl_IssueQuantity.Where(m => m.ProjectID == PrjId && m.ItemGroupID == IG && m.ItemID == itemid && m.IndentNo != "Opening").OrderBy(c => c.Id).Select(xx => new Multiple_Searching_VM
        //    //    {
        //    //        Id = xx.Id,
        //    //        ItemGroupID = xx.ItemGroupID,
        //    //        ItemGroupName = xx.ItemGroupName,
        //    //        ItemName = xx.ItemName,
        //    //        ItemID = xx.ItemID,
        //    //        UnitID = xx.UnitID,
        //    //        //UnitName =objmms.tblUnitMasters.Where(k=> k.UnitID == xx.UnitID).FirstOrDefault().UnitName,
        //    //        BalanceQuantity = objmms.tblItemCurrentStocks.Where(j => j.ProjectNo == xx.ProjectID && j.ItemGroupNo == xx.ItemGroupID && j.ItemNo == xx.ItemID).FirstOrDefault().Qty ?? 0,
        //    //        IssueQuantity = xx.IssueQuantity,
        //    //        Opening = objmms.tblItemMasterStocks.Where(k => k.ProjectNo == xx.ProjectID && k.ItemGroup == xx.ItemGroupID && k.ItemNo == xx.ItemID).FirstOrDefault().Opening ?? 0,
        //    //        ReceiveQty = objmms.GateEntryChilds.Where(h => h.ProjectNo == xx.ProjectID && h.ItemGroupNo == xx.ItemGroupID && h.ItemNo == xx.ItemID).FirstOrDefault().ReceiveQty ?? 0,
        //    //        CreatedDate = xx.CreatedDate
        //    //        }).ToList();




        //    //    var data = querys;
        //    //    PrintData dataObj = new PrintData();
        //    //    dataObj.ItemData = data.Select(x => new ItemData { ItemName = x.ItemName,  UnitName = x.UnitName, Opening=x.Opening, IssueQuantity=x.IssueQuantity, ReceiveQty=x.ReceiveQty, BalanceQuantity=x.BalanceQuantity, CreatedDate=x.CreatedDate }).ToList();
        //    //    return PartialView("_PartialView_AllQuantity_Details_Final", dataObj);
        //    //}
        //    else
        //    {
        //        return null;
        //    }
            
        //}



        public class PrintData
        {
            //public HeaderData HeaderData { get; set; }
            public List<ItemData> ItemData { get; set; }
        }
       
        public class ItemData
        {
            public int Id { get; set; }
            public string ProjectID { get; set; }          
            public string ItemGroupID { get; set; }
            public string ItemGroupName { get; set; }
            public string ItemID { get; set; }
            public string ItemName { get; set; }
            public string UnitID { get; set; }
            public string UnitName { get; set; }
            public Nullable<decimal> IssueQuantity { get; set; }
            public Nullable<decimal> BalanceQuantity { get; set; }

            //OPENING  details below
            //public int Id { get; set; }     
            public string ItemGroup { get; set; }
            public string ItemNo { get; set; }
            public string ProjectNo { get; set; }

            public Nullable<decimal> Opening { get; set; }



            //RECIEVE  details below
            public decimal UId { get; set; }
            public string ItemNos { get; set; }
            public string ItemGroupNo { get; set; }
            public Nullable<System.DateTime> CreatedDaterecieved { get; set; }
            public Nullable<decimal> ReceiveQty { get; set; }
        }






        public ActionResult Details(string PrjId, string IG)
        {


            if (PrjId !=null && IG != null)
            {
                var qr = (from iss in objmms.ns_tbl_IssueQuantity.Where (a=> a.ProjectID == PrjId && a.ItemGroupID == IG  && a.IndentNo != "Opening")
                          join gate in objmms.GateEntryChilds on new { a = iss.ProjectID, b = iss.ItemGroupID } equals new { a = gate.ProjectNo, b = gate.ItemGroupNo }
                          join opning in objmms.tblItemMasterStocks on new { a = gate.ProjectNo, b = gate.ItemGroupNo } equals new { a = opning.ProjectNo, b = opning.ItemGroup }
                          join balstock in objmms.tblItemCurrentStocks on new { a = opning.ProjectNo, b = opning.ItemGroup } equals new { a = balstock.ProjectNo, b = balstock.ItemGroupNo }
                          
                          //&& (gate.ProjectNo == PrjId && gate.ItemGroupNo == IG)
                          //&& (opning.ProjectNo == PrjId && opning.ItemGroup == IG)
                          //&& (balstock.ProjectNo == PrjId && balstock.ItemGroupNo == IG)
                          select new Multiple_Searching_VM
                          {
                              Id = iss.Id,
                              ItemGroupID = iss.ItemGroupID,
                              ItemID = iss.ItemID,
                              ItemName = objmms.tblItemMasters.Where(k => k.ItemID == iss.ItemID).FirstOrDefault().ItemName,
                              UnitID = iss.UnitID,

                              UnitName = objmms.tblUnitMasters.Where(k => k.UnitID == iss.UnitID).FirstOrDefault().UnitName,
                              //BalanceQuantity = objmms.tblItemCurrentStocks.Where(j => j.ProjectNo == PrjId && j.ItemGroupNo == IG && j.ItemNo == itemid).FirstOrDefault().Qty ?? 0,
                              IssueQuantity = iss.IssueQuantity,
                              Opening = opning.Opening,
                              BalanceQuantity = balstock.Qty,
                              ReceiveQty = gate.ReceiveQty
                              //Opening = objmms.tblItemMasterStocks.Where(k => k.ProjectNo == xx.ProjectID && k.ItemGroup == xx.ItemGroupID && k.ItemNo == xx.ItemID).FirstOrDefault().Opening ?? 0,
                             // ReceiveQty = objmms.GateEntryChilds.Where(h => h.ProjectNo == PrjId && h.ItemGroupNo == IG).FirstOrDefault().ReceiveQty ?? 0,
                              //CreatedDate = xx.CreatedDate
                          }).ToList();

                var data = qr;
                //var a1 = objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo).OrderBy(c => c.Id).ToList();            
                var totalRows = data.Count();
                var data1 = new VM_ViewTo_AllItems()
                {
                    TotalRows = totalRows,
                    PageSize = 250,
                    itemsde = qr.ToList()
                };


                if (data1 != null && data1.TotalRows != 0)
                {

                    if (Request.IsAjaxRequest())
                    {

                        return PartialView("_Partial_VIew_All_Quantity_Details", data1);
                    }

                    else
                    {
                        return View("_EmptyView");
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
                // }

            }
            else
            {
                return View("_EmptyView");
            }
        }



    }
}