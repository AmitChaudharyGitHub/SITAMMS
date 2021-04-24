using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Entity;
using BusinessLogicLayer;
using MMS_P.ViewModels;
using DataAccessLayer;
using System.Globalization;

namespace MMS.Controllers
{
    public class OSTController : Controller
    {
        private Model1 db = new Model1();
        FactoryManager m = new FactoryManager();
        List<P_VMTbOSTTransferMaster> MasterList;
        int SessionId = 1;
        int SiteId = 1;
        string EmpId = "EMP0000001";
        string EmpName = "Admin";

        public OSTController()
        {
            try 
            {
                EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
                EmpName = System.Web.HttpContext.Current.Session["EmpName"].ToString();
                SessionId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SessionId"].ToString());
                 SiteId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteId"].ToString());
                  
            }
            catch
            {               
            }
        }

        // GET: OST
        public ActionResult Index()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.fromDate = DateTime.Now.Date;
            ViewBag.toDate = ViewBag.fromDate.Date.AddDays(1);

            if (EmpId == "EMP0000001")
                ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", 0);
            else
                ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", EmpId);
            return View();
        }

        public ActionResult Grid2(string Search ,DateTime? fromDate=null, DateTime? toDate=null, string PrjId = null, int page = 1, string f = "To", string E = null)
        {
            if (E == "")
                E = null;
            if (PrjId == "")
                PrjId = null;
            if (toDate.HasValue) toDate = toDate.Value.AddDays(1);
            const int pageSize = 15000;

            if (!fromDate.HasValue) fromDate = DateTime.Now.Date;
            if (!toDate.HasValue) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);
            if (toDate < fromDate) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);

            int totalRows = 0;

            if (SessionId != 0)
            {
                
                totalRows = m.GetTbOSTTransferMasterManager().FindByDateOrLastCount(Search, SessionId, f, PrjId, fromDate, toDate);
                MasterList = m.GetTbOSTTransferMasterManager().FindByDateOrLast(Search, SessionId, f, PrjId, fromDate, toDate, page, pageSize);
                


                if (MasterList != null)
                {



                    var data = new PagedOSTMasterModel()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        Master = MasterList.ToList()
                    };
                    if (Request.IsAjaxRequest())
                    {
                        return View("_GridView", data);
                    }
                }


            }

            return View("_EmptyView");
        }
        public ActionResult DebitCredit()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.fromDate = DateTime.Now.Date;
            ViewBag.toDate = ViewBag.fromDate.Date.AddDays(1);

            if (EmpId == "EMP0000001")
                ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", 0);
            else
                ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", EmpId);
            return View();
        }
        public ActionResult DebitCreditGrid(string Deb_Cred = "Debit", string DProjectNo = null, string RProjectNo = null, string Comp = "Complete", DateTime? fromDate = null, DateTime? toDate = null, int page = 1)
        {
            if (DProjectNo == "null")
                DProjectNo = null;
            if (RProjectNo == "null")
                RProjectNo = null;
           
            const int pageSize = 15000;

            int totalRows = 0;

            if (SessionId != 0)
            {

                totalRows = m.GetTbOSTTransferMasterManager().FindDebitCreditCount( SessionId,Deb_Cred ,DProjectNo,RProjectNo,Comp, fromDate, toDate);
                MasterList = m.GetTbOSTTransferMasterManager().FindDebitCredit(SessionId, Deb_Cred, DProjectNo, RProjectNo, Comp, fromDate, toDate, page, pageSize);



                if (MasterList != null)
                {



                    var data = new PagedOSTMasterModel()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        Master = MasterList.ToList()
                    };
                    if (Request.IsAjaxRequest())
                    {
                        return View("_GridViewDC", data);
                    }
                }


            }

            return View("_EmptyView");
        }
        

        public JsonResult GetProjectByEmpId(string E = "0")
        {

            string D = null;
            string R = null;
            var Prjlist = m.GetProjectMasterManager().FindListByString(null);
            if (E != "0" && E != "EMP0000001" && E != "")
            {
                string PrjString = m.GetEmployeeMasterManager().FindPrj(E);


                var PrjlistD = m.GetProjectMasterManager().FindListByString(PrjString);

                D = PrjlistD.ToJSON();
            }
            else
            {
               

                D = Prjlist.ToJSON();
               // return Json(D, JsonRequestBehavior.AllowGet);
            }

            R = Prjlist.ToJSON();

            var v = new { List1 = D, List2 = R };
            //var v = new { List1 = D };
            string jsonString = v.ToJSON();


            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectByEmpId1(string E = "0")
        {
            //if (E == null || E == "0" || E == "")
            //    E = EmpId;
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
        public ActionResult Create()
        {
            
            ViewBag.EmpId = EmpId;
            //Item Group Name
        
         
            ViewBag.OrderDate = DateTime.Now.Date;

            ViewBag.TaxType = new SelectList(m.GetTbTaxTypeMasterManager().FindAll(), "Percent", "TaxType");

            ViewBag.ItemGroup = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");
            return View();
        }

        public ActionResult SendList1(OST Grid)
        {
            try
            {
                Grid.Master.SessionId = SessionId;          
                Grid.Master.CreatedBy = EmpId;              
                Grid.Master.CreatedDate = System.DateTime.Now;
                decimal a = m.GetTbOSTTransferMasterManager().Add(Grid.Master);
                if (Grid.Child != null && a != 0)
                    foreach (var x in Grid.Child)
                    {
                        x.MUId = a;
                        x.Dispatch = 0;
                        x.Receive = 0;
                        decimal b = m.GetTbOSTTransferChildManager().Add(x);
                    }
                //return RedirectToAction("Details", new { id = a });
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch { return Json("2", JsonRequestBehavior.AllowGet); }

        }

        public ActionResult Details(decimal id)
        {
             //id = 2;
         
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            P_VMTbOSTTransferMaster Master = m.GetTbOSTTransferMasterManager().FindVM(id);
            if (Master.TransferDocumentDate != null)
                Master.TransferDocumentDate2 = Master.TransferDocumentDate.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            if (Master.ChallanDate != null)
                Master.ChallanDate2 = Master.ChallanDate.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            List<VMTbOSTTransferChild> ChildList = m.GetTbOSTTransferChildManager().FindByMasterVM(id);


            var totalRows = ChildList.Count();

            var data = new OSTDetails()
            {
             
                Master = Master,
                Child = ChildList
            };


            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);


            //


        }
        public ActionResult Edit(decimal id)
        {
            
         
          
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            P_VMTbOSTTransferMaster Master = m.GetTbOSTTransferMasterManager().FindVM(id);
            List<VMTbOSTTransferChild> ChildList = m.GetTbOSTTransferChildManager().FindByMasterVM(id);
            ViewBag.TransferDocumentDate = Master.TransferDocumentDate;
            ViewBag.ChallanDate = Master.ChallanDate;

            ViewBag.EmpId = EmpId;


            var data = new VMOST()
            {

                Master = Master,
                Child = ChildList
            };


            if (data == null)
            {
                return HttpNotFound();
            }
         
            ViewBag.TaxType = new SelectList(m.GetTbTaxTypeMasterManager().FindAll(), "Percent", "TaxType");
            ViewBag.ItemGroup = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");

            return View(data);


            //


        }

        public ActionResult SendList2(OST Grid)
        {
            try
            {
                TbOSTTransferMaster t = m.GetTbOSTTransferMasterManager().Find(Grid.Master.UId);
                Grid.Master.SessionId = t.SessionId;     
                Grid.Master.CreatedDate = t.CreatedDate;
                Grid.Master.ModifyBy = EmpId;
                Grid.Master.ModifyDate = System.DateTime.Now; 
              
                List<decimal> CList = m.GetTbOSTTransferChildManager().FindAllIdByMaster(Grid.Master.UId);
                m.GetTbOSTTransferMasterManager().Update(Grid.Master);
                if (Grid.Child != null)
                {
                    foreach (var x in Grid.Child)
                    {

                        if (!CList.Any(a => a == x.UId))
                        {
                            x.MUId = Grid.Master.UId;
                            x.Dispatch = 0;
                            x.Receive = 0;
                            decimal b = m.GetTbOSTTransferChildManager().Add(x);
                        }
                    }
                    if (CList != null)
                        foreach (decimal x in CList)
                        {

                            if (!Grid.Child.Any(a => a.UId == x))
                            {
                                m.GetTbOSTTransferChildManager().Delete(x);
                            }
                        }
                }
                //return RedirectToAction("Details", new { id = a });
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch { return Json("2", JsonRequestBehavior.AllowGet); }

        }

        public JsonResult GetOstNo(string ProjectID)
        {
            var sessioncode = m.GetSessionMasterManager().Find(SessionId).Name;                    
            var idMax = m.GetTbOSTTransferMasterManager().FindMaxId(SessionId, ProjectID);
            string Code = m.GetProjectMasterManager().FindCode(ProjectID);
            string ProjectCode = "";
            ProjectCode = "TR/" + Code + "/" + sessioncode + "/" + idMax;

            string jsonString = ProjectCode.ToJSON();

            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetItemByGroup(string id)
        {
            if (id != null)
            {
                object ItemMasters = m.GetGobalItemManager().FindByItemGroup1(id);
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetItemDetail(string id, string PrjId=null)
        {
            if (id != "" && id != "0")
            {
                tblItemMaster Item = m.GetGobalItemManager().Find(id);
                string u = null;
                string un = null;
                decimal AverageRate = 0;
                try
                {
                    if (PrjId != null && PrjId != "0" && PrjId != "")
                        AverageRate = m.GettblItemCurrentStockManager().Average_in_GC(SessionId, PrjId, id);
                    u = Item.UnitMasterPurchase.UnitName;
                    un = Item.UnitMasterPurchase.UnitID;
                }
                catch { }

                var v = new { Make = Item.Make, PartNo = Item.PartNo, Unit = u, UnitNo = un,Rate =decimal.Round( AverageRate,2)  };
                string jsonString = v.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
    }
}