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
    public class CashPurchaseChildsController : Controller
    {
        private Model1 db = new Model1();
        FactoryManager m = new FactoryManager();
        List<TbCashPurchaseMaster> MasterList;
        int SessionId = 1;
        int SiteId = 1;
        string EmpId = "EMP0000001";
       string EmpName = "Admin";
        public CashPurchaseChildsController()
        {
            try 
            {
                EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
                EmpName = System.Web.HttpContext.Current.Session["Emp_Name"].ToString();
                SessionId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SessionId"].ToString());
                 SiteId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteId"].ToString());
                  
            }
            catch(Exception ex)
            {   
                            
            }
        }
        // GET: CashPurchaseChilds
 

        #region IndexCreater
        public JsonResult GetProjectByEmpId( string E = "0")
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
        public List<tblProjectMaster> GetProjectByEmpIdG(string E = "0")
        {
            //if (E == null || E == "0" || E == "")
            //    E = EmpId;
         
            if (E != "0" && E != "EMP0000001" && E != "")
            {
                string PrjString = m.GetEmployeeMasterManager().FindPrj(E);


            var Prjlist = m.GetProjectMasterManager().FindGListByString(PrjString);

            return Prjlist;
            }
            else
            {
               var Prjlist= m.GetProjectMasterManager().FindGListByString(null);

               return Prjlist;
            }





           
        }
        public ActionResult Index2()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.fromDate = DateTime.Now.Date;
            ViewBag.toDate = ViewBag.fromDate.Date.AddDays(1);
        
            if (EmpId == "EMP0000001")
            ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName",0);
            else
                ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", EmpId);
            return View();
        }
        public ActionResult Grid2(DateTime? fromDate, DateTime? toDate, string PrjId =null, int page = 1,string f="C",string E=null,string Comp="All")
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

            if (SessionId != 0 )
            {
                if (E == null || E == "0" || E == "")
                {
                    totalRows = m.GetTbCashPurchaseMasterManager().FindForAdminCount(SessionId, PrjId, fromDate, toDate, Comp);
                    MasterList = m.GetTbCashPurchaseMasterManager().FindForAdmin(SessionId, PrjId, fromDate, toDate, page, pageSize, Comp);
                }
                else
                {
                    if (f == "C")
                    {
                        totalRows = m.GetTbCashPurchaseMasterManager().FindForCreaterCount(SessionId, E, PrjId, fromDate, toDate, Comp);
                        MasterList = m.GetTbCashPurchaseMasterManager().FindForCreater(SessionId, E, PrjId, fromDate, toDate, page, pageSize, Comp);
                    }
                    else if (f == "A")
                    {
                        totalRows = m.GetTbCashPurchaseMasterManager().FindForApprovedCount(SessionId, E, PrjId, fromDate, toDate, Comp);
                        MasterList = m.GetTbCashPurchaseMasterManager().FindForApproved(SessionId, E, PrjId, fromDate, toDate, page, pageSize, Comp);
                    }
                }
              

            
                    if (MasterList != null)
                    {
                       
                        
                       
                        var data = new PagedCashPurchaseMasterModel()
                        {
                            TotalRows = totalRows,
                            PageSize = pageSize,
                            CashPurchaseMasterBll = MasterList.ToList()
                        };
                        if (Request.IsAjaxRequest())
                        {
                            return View("_GridView", data);
                        }
                    }

               
            }

            return View("_EmptyView");
        }
        public ActionResult Grid3(string PrjId = null, int page = 1, string f = "C", string E = null, string Comp = "All")
        {
            if (E == "")
                E = null;
            if (PrjId == "")
                PrjId = null;
            const int pageSize = 15000;
         

            int totalRows = 0;

            if (SessionId != 0)
            {
                if (E == null || E == "0"||E=="")
                {
                    totalRows = m.GetTbCashPurchaseMasterManager().FindForAdminLastCount(SessionId, PrjId, Comp);
                    MasterList = m.GetTbCashPurchaseMasterManager().FindForAdminLast(SessionId, PrjId, page, pageSize, Comp);
                }
                else
                {
                    if (f == "C")
                    {
                        totalRows = m.GetTbCashPurchaseMasterManager().FindForCreaterLastCount(SessionId, E, PrjId, Comp);
                        MasterList = m.GetTbCashPurchaseMasterManager().FindForCreaterLast(SessionId, E, PrjId, page, pageSize, Comp);
                    }
                    else if (f == "A")
                    {
                        totalRows = m.GetTbCashPurchaseMasterManager().FindForApprovedLastCount(SessionId, E, PrjId, Comp);
                        MasterList = m.GetTbCashPurchaseMasterManager().FindForApprovedLast(SessionId, E, PrjId, page, pageSize, Comp);
                    }
                }



                if (MasterList != null)
                {



                    var data = new PagedCashPurchaseMasterModel()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        CashPurchaseMasterBll = MasterList.ToList()
                    };
                    if (Request.IsAjaxRequest())
                    {
                        return View("_GridView", data);
                    }
                }


            }

            return View("_EmptyView");
        }
        #endregion endindex


   


        

        // GET: CashPurchaseChilds/Details/5
      
        public ActionResult Details(decimal id)
        {
           // id = 17;
            const int pageSize = 150;
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
          
           TbCashPurchaseMaster Master =m.GetTbCashPurchaseMasterManager().Find(id) ;
           if (Master.CashPurchaseNo==null)
               return HttpNotFound();
           if (Master.Date != null)
               Master.Date2 = Master.Date.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
           //if (Master.RefDate != null)
           //    Master.RefDate2 = Master.RefDate.Value.ToShortDateString();
           //if (Master.Delivery_Date != null)
           //    Master.Delivery_Date2 = Master.Delivery_Date.Value.ToShortDateString();
            List<TbCashPurchaseChild> ChildList =m.GetTbCashPurchaseChildManager().FindByMaster(id);
            var totalRows = ChildList.Count();

            var data = new Dx1()
            {
                TotalRows = totalRows,
                PageSize = pageSize,
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
            VMTbCashPurchaseMaster Master = m.GetTbCashPurchaseMasterManager().FindO(id);
            List<VMTbCashPurchaseChild> ChildList = m.GetTbCashPurchaseChildManager().FindByMasterO(id);
            ViewBag.prjtid = new SelectList(GetProjectByEmpIdG( EmpId), "PRJID", "ProjectName",Master.ProjectId);
            //Employee Or Forword to User Name
           // string PRJID = m.GetProjectMasterManager().Find(SiteId).PRJID;
            ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindByProject(Master.ProjectId), "EmpID", "FName", Master.EmpId);

            //Item Group Name
            ViewBag.itemgroupname = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");
            ViewBag.VGId = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", Master.VGId);
            ViewBag.VNAME = new SelectList(m.GetVendorMasterManager().FindByVendorGroup(Master.VGId), "VendorId", "Name", Master.VendorId);
            ViewBag.taxtype = new SelectList(m.GetTbTaxTypeMasterManager().FindAll(), "Percent", "TaxType");
            ViewBag.Date = Master.Date;
            ViewBag.Delivery_Date = Master.Delivery_Date;
            ViewBag.RefDate = Master.RefDate;
            ViewBag.CreatedDate = Master.CreatedDate;
            // id = 17;
          
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

          
            var totalRows = ChildList.Count();

            var data = new Dx2()
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
        public ActionResult SendList2(Dx1 Grid)
        {
            try
            {
              TbCashPurchaseMaster t=  m.GetTbCashPurchaseMasterManager().Find(Grid.Master.UId);
                Grid.Master.SessionId=t.SessionId;
                Grid.Master.SiteId = t.SiteId;
                Grid.Master.CreatedBy = t.CreatedBy;
                Grid.Master.CreatedDate = t.CreatedDate;
                Grid.Master.CreaterName = t.CreaterName;
                Grid.Master.CashPurchaseId = t.CashPurchaseId;
                List<decimal> CList = m.GetTbCashPurchaseChildManager().FindByMasterAny(Grid.Master.UId);
                 m.GetTbCashPurchaseMasterManager().Update(Grid.Master);
                 if (Grid.Child != null)
                 {
                     foreach (var x in Grid.Child)
                     {

                         if (!CList.Any(a => a == x.UId))
                         {
                             x.MUId = Grid.Master.UId;
                             decimal b = m.GetTbCashPurchaseChildManager().Add(x);
                         }
                     }
                     if (CList != null)
                     foreach (decimal x in CList)
                     {

                         if (!Grid.Child.Any(a => a.UId == x))
                         {
                             m.GetTbCashPurchaseChildManager().Delete(x);
                         }
                     }
                 }
                //return RedirectToAction("Details", new { id = a });
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch { return Json("2", JsonRequestBehavior.AllowGet); }

        }
      
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult RowEdit([Bind(Include = "UId,MUId,SrNo,ItemId,Make_Specs,PurchaseUOMId,PurchaseQty,IssueUOMId,Rate,DeliveryCharges,TaxType,Vat,AmountDMR,Measurements")] CashPurchaseChild cashPurchaseChild)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cashPurchaseChild).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MUId = new SelectList(db.CashPurchaseMasters, "UId", "UId", cashPurchaseChild.MUId);
            return View(cashPurchaseChild);
        }
        // GET: CashPurchaseChilds/Create
        public ActionResult Create()
        {
            //ViewBag.prjtid = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName");
            //Employee Or Forword to User Name
          //  string PRJID = m.GetProjectMasterManager().Find(SiteId).PRJID;
          //  ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindByProject(PRJID), "EmpID", "FName");
            ViewBag.EmpId = EmpId;
            //Item Group Name
            ViewBag.itemgroupname = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");
            ViewBag.VGId = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType");
            ViewBag.taxtype = new SelectList(m.GetTbTaxTypeMasterManager().FindAll(), "Percent", "TaxType");
            ViewBag.OrderDate = DateTime.Now.Date;

          //  ViewBag.VNAME = new List<SelectListItem>() { new SelectListItem() { Text = "", Value = "" } };
            return View();
        }
        public JsonResult Getprojectcodes(string id)
        {
            List<tblEmployeeMaster> E = m.GetEmployeeMasterManager().FindByProject(id);


            var EO = E.Select(a => new { Value = a.EmpID, Text = a.FName });
          //  SelectList obgE = new SelectList(E, "EmpID", "FName", 0);

            var stateList = this.Getprojectcode(Convert.ToString(id));

            var v = new { List = EO, Message = stateList };

            string jsonString = v.ToJSON();
        
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVendorByGroup(string Vid, string Pid)
        {
            object VendorMasters = m.GetVendorMasterManager().FindByVendorGroup(Vid,Pid);
            string jsonString = VendorMasters.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemByGroup(string id)
        {
            if (id != null)
            {
                object ItemMasters = m.GetGobalItemManager().FindByItemGroup(id);
                //object ItemMasters = m.GettblItemCurrentStockManager().FindItemGroup(id);
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetItemByGroupStock(string Pid, string Gid)
        {
            if (Pid != null && Gid != null)
            {
                List<Listobj> ItemMasters = m.GettblItemCurrentStockManager().FindByItemGroup(Pid, Gid);
                Session["Items"] = ItemMasters;
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetItemDetail(int id)
        {
            if (id != 0)
            {
                tblItemMaster Item = m.GetGobalItemManager().Find(id);
                string u=null;
                try
                {
               
                    u = Item.UnitMasterPurchase.UnitName;
                }
                catch { }
                var v = new { Make = Item.Make, PartNo = Item.PartNo, Unit = u };
                string jsonString = v.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

       








        public ActionResult SendList1(Dx1 Grid)
        {
            try
            {
                Grid.Master.SessionId = SessionId;
                Grid.Master.SiteId = SiteId;
                Grid.Master.CreatedBy = EmpId;
                Grid.Master.CreaterName = EmpName;
                Grid.Master.CreatedDate = System.DateTime.Now;
                decimal a = m.GetTbCashPurchaseMasterManager().Add(Grid.Master);
                if (Grid.Child != null && a != 0)
                    foreach (var x in Grid.Child)
                    {
                        x.MUId = a;
                        x.Receive = 0;
                        decimal b = m.GetTbCashPurchaseChildManager().Add(x);
                    }
                //return RedirectToAction("Details", new { id = a });
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch { return Json("2", JsonRequestBehavior.AllowGet); }
            
        }
        public JsonResult SendList(string Obj,List<Data> Grid)
        {
            CashPurchaseMaster tbl1 = new CashPurchaseMaster();
            //tbl1.VendorId = Obj.VName;
            //tbl1.Date = Obj.Date;
            try
            {
                List<CashPurchaseChild> list = new List<CashPurchaseChild>();
                foreach (var item in Grid)
                {


                    CashPurchaseChild tbl = new CashPurchaseChild();
                    //tbl.ProjectID = item.ProjectID;
                    //tbl.IndentNo = item.IndentNo;
                    //tbl.EmployeeID = item.EmployeeID;
                    //tbl.ItemGroupID = item.ItemGroupID;
                    tbl.ItemId = item.ItemID;
                    //tbl.PurchaseUOMId = item.Unitname;
                    //tbl.Make = item.Makename;
                    //tbl.PartNo = item.Partname;
                    //tbl.Quantity = item.Quantityname;
                    //tbl.SessionId = "2015-16";
                    //tbl.SiteId = "";
                    //tbl.Date = DateTime.Now;
                    //tbl.AutoUniID = Convert.ToInt32(Session["Auuniqueid"]);
                    //tbl.Status = "Pending";


                    list.Add(tbl);

                }
                //objmms.tblIndentRequisitions.AddRange(list);
                //objmms.SaveChanges();
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("2", JsonRequestBehavior.AllowGet);
            }

        }
       
        public class Dx
        {

            public List<Data> Child { get; set; }
            public Data1 Master { get; set; }
         
        }
        public class Data1
        {
            
            public string VName { get; set; }
            public Nullable<System.DateTime> Date { get; set; }
        }
        public class Data
        {
           
            public int ItemID { get; set; }
            public string Unitname { get; set; }
            public string Makename { get; set; }
            public string Partname { get; set; }
            public string Quantityname { get; set; }
           
        }
    
        public JsonResult GetEmployeeByPRJId(string PRJID)
        {

            List<tblEmployeeMaster> E = m.GetEmployeeMasterManager().FindByProject(PRJID);

            SelectList obgE = new SelectList(E, "EmpID", "FName", 0);
            return Json(obgE);
        }
        // Get State from DB by country ID
        
        public string Getprojectcode(string ProjectID)
        {
            var sessioncode = m.GetSessionMasterManager().Find(SessionId).Name;

            //int SiteId = m.GetProjectMasterManager().Find(ProjectID).TransID;
            var idMax = m.GetTbCashPurchaseMasterManager().FindMaxId(SessionId, ProjectID);
            string Code=   m.GetProjectMasterManager().FindCode(ProjectID);
            string ProjectCode = "";
            ProjectCode ="LPO/"+ Code + "/" + sessioncode + "/" + idMax;
           
            return ProjectCode;
        }
      
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
