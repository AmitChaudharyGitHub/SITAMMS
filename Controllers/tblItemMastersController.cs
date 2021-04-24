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
using System.Linq.Dynamic;
using System.Data.SqlClient;
using MMS.Models;
using System.Configuration;
using MMS.ViewModels;
using System.Web.Helpers;
using System.Threading;
using MMS.Helpers;

namespace MMS.Controllers
{

   [Authorize]
    public class tblItemMastersController : Controller
    {

        private SqlConnection con;
        private DAL.MMSEntities objmms = new DAL.MMSEntities();
        FactoryManager m = new FactoryManager();

        static   private List<tblItemGroupMaster> ItemTypeList;
        string Id = "0";
        Procedure procedure = new Procedure();

        private tblItemGroupMaster ItemType;

        int SessionId = 1;
        //  int SiteId = 1;
        string EmpID = string.Empty;
        string EmpName = string.Empty;
        public tblItemMastersController()
        {
            try
            {
               
               EmpID = System.Web.HttpContext.Current.Session["EmpID"].ToString();
               EmpName = System.Web.HttpContext.Current.Session["Emp_Name"].ToString();
               SessionId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SessionId"].ToString());
              // SiteId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteId"].ToString());
            
            }
            catch
            {
            }

          
        }
        //Use Seession
        private void FIllBySession()
        {
            if (this.Session["Id"] != null)
                Id = this.Session["Id"].ToString();        
            ItemType = ItemTypeList.Where(a => a.ItemGroupID == Id).FirstOrDefault();
          
        }
         //Use Temp Data
      

      
        private void FIllByid(int? id)
        {
            ItemType = ItemTypeList.Where(a => a.ItemGroupID == Id).FirstOrDefault();
        }
        // GET: tblItemMasters

        //Post method to add details    
        [HttpPost]
        public ActionResult AddMake(MakeInfo obj)
        {
            AddDetails(obj);

            return View();
        }

        //To Handle connection related activities    
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["Model1"].ToString();
            con = new SqlConnection(constr);

        }
        //To add Records into database     
        private void AddDetails(MakeInfo obj)
        {
            connection();
            SqlCommand com = new SqlCommand("AddMakeMasters", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Make", obj.Make);
            com.Parameters.AddWithValue("@CreatedBy", EmpID);
            com.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString());
            con.Open();
            com.ExecuteNonQuery();
            con.Close();

        }



        public ActionResult Index()
        {

            ViewBag.ItemGroupId = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupId", "GroupName", Id);
            ViewBag.Units = new SelectList(m.GetUnitMasterManager().FindAll(), "UnitID", "UnitName");
            return View();

        }
        //Use Seession
        public ActionResult Grid(string ItemGroupId = "0", int page = 1, string sort = "Item_Name", string sortDir = "ASC")
        {
           
            const int pageSize = 15000;
            sortDir = sortDir.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? sortDir : "asc";
            var validColumns = new[] { "TransID", "ItemName", "ItemGroupID" };
            if (!validColumns.Any(c => c.Equals(sort, StringComparison.CurrentCultureIgnoreCase)))
                sort = "TransID";

            if (ItemGroupId != "0")
            {
                Id = ItemGroupId;
                this.Session["Id"] = Id.ToString();
            }
            else
            {
                try { FIllBySession(); }
                catch { }
            }
            if (Id == "0")
            {
               // Id = "IG00000001";
                this.Session["Id"] = Id;
            }
          
            if (Id != "0")
            {
               // ItemType = ItemTypeList.Where(a => a.ItemGroupID == Id).FirstOrDefault();
                ItemType = m.GetItemTypeManager().Find( Id);

                if (ItemType != null)
                {

                    var totalRows = m.GetGobalItemManager().CountByItemType(ItemType.TransID);
                    if (totalRows != 0)
                    {
                        var globalItems = m.GetGobalItemManager().FindPageByItemType(ItemType.TransID, page, pageSize);
                        var data = new PagedGlobalItemModel1()
                        {
                            TotalRows = totalRows,
                            PageSize = pageSize,
                            GlobalItemBll = globalItems.ToList()
                        };
                        return View("_GridView", data);
                    }
                    return View("_EmptyView");
                }
                else { return View("_EmptyView"); }
            }
            else { return View("_EmptyView"); }
        }

        public ActionResult Grid1(string ItemGroupId, string ItemName, string HSNCode, string UnitId,bool IsEdit=true)
        {
            TempData["IsEditable"] = IsEdit;
            DAL.MMSEntities objmms = new DAL.MMSEntities();
            //var searchData = objmms.tblItemMasters.ToList();
            var searchData = procedure.SearchItems(ItemGroupId, ItemName, HSNCode, UnitId);

            TempData["SearchData1"] = searchData;
            if (searchData.Count == 0)
                searchData = null;
            return View("_GridView", searchData);
        }

        public  JsonResult EnableDisableItem(int TransId)
        {
            if (TransId != 0)
            {
                var item = objmms.tblItemMasters.SingleOrDefault(x => x.TransID == TransId);
                if (item.Disable == "Disabled")
                    item.Disable = null;
                else
                    item.Disable = "Disabled";
                objmms.Entry(item).State = EntityState.Modified;
                objmms.SaveChanges();
                return Json(new { Status = 1 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = 2 }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult SearchGrid(string Name, int page = 1, string sort = "Item_Name", string sortDir = "ASC")
        {
            const int pageSize = 15000;
            sortDir = sortDir.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? sortDir : "asc";
            var validColumns = new[] { "TransID", "ItemName", "ItemGroupID" };
            if (!validColumns.Any(c => c.Equals(sort, StringComparison.CurrentCultureIgnoreCase)))
                sort = "TransID";




            if (Name != "")
            {
                // ItemType = ItemTypeList.Where(a => a.ItemGroupID == Id).FirstOrDefault();
                //  ItemType = m.GetItemTypeManager().Find(Id);



                var totalRows = m.GetGobalItemManager().CountByItemSearch(Name);
                if (totalRows != 0)
                {
                    var globalItems = m.GetGobalItemManager().SearchPageByItem(Name, page, pageSize);
                    var data = new PagedGlobalItemModel1()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        GlobalItemBll = globalItems.ToList()
                    };
                    return View("_GridView", data);
                }
                return View("_EmptyView");
            }
            else { return View("_EmptyView"); }
        }
        // //Use Temp Data

        // GET: tblItemMasters/Details/5
        public ActionResult Details(int? id)
        {
          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblItemMaster tblItemMaster = m.GetGobalItemManager().Find((int)id);
            if (tblItemMaster == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
            {

                return PartialView(tblItemMaster);
            }
            else
                return View(tblItemMaster);
        }
      
        // GET: tblItemMasters/Create
        public ActionResult Create()
        {
            ViewBag.Make = new SelectList(objmms.tblMakeMasters, "Id", "Make").OrderBy(k => k.Text);

            ViewBag.ItemGroupID = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName", Id);
            ViewBag.IssuingUnitID = new SelectList(m.GetUnitMasterManager().FindAll(), "UnitID", "UnitName");
            ViewBag.UnitID = new SelectList(m.GetUnitMasterManager().FindAll(), "UnitID", "UnitName");
            if (Request.IsAjaxRequest())
            {
                ViewBag.IsUpdate = false;
                return PartialView("Create");
            }
            else

                return View();
        }
     
        // POST: tblItemMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        //changes to restrict duplicate data
        public ActionResult Create([Bind(Include = "TransID,CompanyID,ItemID,ItemGroupID,ItemName,UnitID,Make,PartNo,Status,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IssuingUnitID,ConversionRatio,Returnable,HSNCode")] tblItemMaster tblItemMaster)
        {


            // TempData["Id"] = tblItemMaster.ItemGroupID;
            ViewBag.ItemGroupID = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName", Id);
            ViewBag.IssuingUnitID = new SelectList(m.GetUnitMasterManager().FindAll(), "UnitID", "UnitName");
            ViewBag.UnitID = new SelectList(m.GetUnitMasterManager().FindAll(), "UnitID", "UnitName");
            if (!ModelState.IsValid)
            {

                return PartialView("Create", tblItemMaster);
            }
            else
            {

                tblItemMaster.CreatedBy = EmpID;


                tblItemMaster.CreatedDate = System.DateTime.Now;
                tblItemMaster.CompanyID = "COMP000001";

                var itemExists = new Model1().tblItemMasters.Any(x => x.ItemName.Equals(tblItemMaster.ItemName, StringComparison.OrdinalIgnoreCase) && x.ItemGroupID == tblItemMaster.ItemGroupID);
                if (itemExists)
                {
                    TempData["OperStatus"] = "Item name already exists";
                    if (Request.IsAjaxRequest())
                    { return PartialView("Create", tblItemMaster); }
                    return View("Create", tblItemMaster);
                }
                int i = m.GetGobalItemManager().Add(tblItemMaster);
                MSP_Model obj = new MSP_Model();

                if (i != 0)
                {
                    TempData["OperStatus"] = "Item added succeessfully in  Group";
                    ModelState.Clear();
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true });
                    }
                    return RedirectToAction("Index", "tblItemMasters");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(); }
            return View();

        }




        //public ActionResult Create([Bind(Include = "TransID,CompanyID,ItemID,ItemGroupID,ItemName,UnitID,Make,PartNo,Status,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IssuingUnitID,ConversionRatio,Returnable,HSNCode")] tblItemMaster tblItemMaster)
        //{


        //   // TempData["Id"] = tblItemMaster.ItemGroupID;
        //    ViewBag.ItemGroupID = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName", Id);
        //    ViewBag.IssuingUnitID = new SelectList(m.GetUnitMasterManager().FindAll(), "UnitID", "UnitName");
        //    ViewBag.UnitID = new SelectList(m.GetUnitMasterManager().FindAll(), "UnitID", "UnitName");
        //    if (!ModelState.IsValid)
        //    {


        //        return PartialView("Create", tblItemMaster);
        //    }
        //    else
        //    {

        //       tblItemMaster.CreatedBy = EmpID; 


        //        tblItemMaster.CreatedDate = System.DateTime.Now;
        //        tblItemMaster.CompanyID = "COMP000001";
        //        int i = m.GetGobalItemManager().Add(tblItemMaster);
        //        if (i != 0)
        //        {
        //            TempData["OperStatus"] = "Item added succeessfully in  Group";
        //            ModelState.Clear();
        //            if (Request.IsAjaxRequest())
        //            {
        //                return Json(new { success = true });
        //            }
        //            return RedirectToAction("Index", "tblItemMasters");
        //        }
        //    }
        //    if (Request.IsAjaxRequest())
        //    { return PartialView(); }
        //    return View();

        //}
        public JsonResult IsItemNameUnique(tblItemMaster catalog)
        {

          

            return m.GetRepeatChequeManager().GlobalItemRepeat(catalog.ItemName, catalog.ItemGroupID, catalog.TransID) ? Json(false, JsonRequestBehavior.AllowGet) : Json(true, JsonRequestBehavior.AllowGet);

        }
        // GET: tblItemMasters/Edit/5
        public ActionResult Edit(int? id)
        {
          
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblItemMaster tblItemMaster = m.GetGobalItemManager().Find((int)id);
            if (tblItemMaster == null)
            {
                return HttpNotFound();
            }
            ViewBag.IssuingUnitID = new SelectList(m.GetUnitMasterManager().FindAll(), "UnitID", "UnitName", tblItemMaster.IssuingUnitID);
            ViewBag.UnitID = new SelectList(m.GetUnitMasterManager().FindAll(), "UnitID", "UnitName", tblItemMaster.UnitID);

            if (Request.IsAjaxRequest())
            {


                ViewBag.IsUpdate = true;
                return PartialView(tblItemMaster);

            }
            else
                return View(tblItemMaster);
        }
  
        // POST: tblItemMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransID,CompanyID,ItemID,ItemGroupID,ItemName,UnitID,Make,PartNo,Status,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IssuingUnitID,ConversionRatio,Returnable,HSNCode")] tblItemMaster tblItemMaster)
        {
         
           
            if (!ModelState.IsValid)
            {
                ViewBag.IssuingUnitID = new SelectList(m.GetUnitMasterManager().FindAll(), "UnitID", "UnitName", tblItemMaster.IssuingUnitID);
                ViewBag.UnitID = new SelectList(m.GetUnitMasterManager().FindAll(), "UnitID", "UnitName", tblItemMaster.UnitID);
                if (Request.IsAjaxRequest())
                { return PartialView("Edit", tblItemMaster); }
                return View("Edit");
            }
            else
            {
                 tblItemMaster.ModifiedBy = EmpID;

                tblItemMaster Item = m.GetGobalItemManager().Find(tblItemMaster.TransID);
                tblItemMaster.CreatedBy = Item.CreatedBy;
                tblItemMaster.CompanyID = "COMP000001";
                tblItemMaster.Status = "E";
               
                tblItemMaster.CreatedDate = Item.CreatedDate;
                tblItemMaster.ModifiedDate = System.DateTime.Now;
                m.GetGobalItemManager().Update(tblItemMaster);

                TempData["OperStatus"] = "Item updated succeessfully";
                ModelState.Clear();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true });
                }
                return RedirectToAction("Index");
               
            }
          
        }
       
        // GET: tblItemMasters/Delete/5
        public ActionResult Delete(int? id)
        {
         
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblItemMaster tblItemMaster = m.GetGobalItemManager().Find((int)id);
            if (tblItemMaster == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
            {

                return PartialView(tblItemMaster);
            }
            return View(tblItemMaster);
        }
   
        // POST: tblItemMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
          
            bool a = m.GetGobalItemManager().Delete((int)id);
            if (Request.IsAjaxRequest())
            {
                if (a == true)
                    return Json(new { success = true });
                else
                    return Json(new { success = false });
            }
            return RedirectToAction("Index");
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

        //new
        public JsonResult GetItemCodeByGroup(string id)
        {
            if (id != null)
            {
                object ItemMasters = objmms.tblItemMasters.Where(x => x.ItemGroupID == id && x.Disable==null).Select(x => new { Text = x.GITEMCode, Value = x.ItemID }).ToList();
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetItemByGroupStock(string Pid,string Gid)
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
        //public JsonResult GetServiceNames(string term)
        //{
        //    var results = db.Services.Where(s => term == null || s.ServiceName.ToLower().Contains(term.ToLower())).Select(x => new { id = x.ServiceID, value = x.ServiceName }).Take(5).ToList();

        //    return Json(results, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult SearchItemByGroupStock(string term)
        {
            List<Listobj> ItemMasters = (List<Listobj>)System.Web.HttpContext.Current.Session["Items"];
          // List<Listobj> ItemMasters =(List<Listobj>) System.Web.HttpContext.Current.Session["Items"];
          
            if (ItemMasters != null && term != null)
            {
                var results = ItemMasters.Where(s => term == null || s.Text.ToLower().Contains(term.ToLower())).Take(5).ToList();
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetItemGroupStock(string Pid)
        {
            //m.GettblItemMasterStockManager().ScriptDS(null,Pid,1);
            if (Pid != null )
            {
                object ItemMasters = m.GettblItemCurrentStockManager().FindItemGroup(Pid);
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetItemDetail(string id)
        {
            if (id != "0")
            {
                tblItemMaster Item = m.GetGobalItemManager().Find(id);
                string u = null;
                try
                {

                    u = Item.UnitMasterPurchase.UnitName;
                }
                catch { }
                //var v = new { Make = Item.Make, PartNo = Item.PartNo, Unit = u };
                var v = new { UnitNo = Item.UnitID, Unit = u };
                string jsonString = v.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetItemDetail2(string id, string pid)
        {
            if (id != "0")
            {
                tblItemMaster Item = m.GetGobalItemManager().Find(id);
               // decimal Qty = m.GettblItemCurrentStockManager().Find(pid,id).Qty??0;

          MonthlyValues mv=      m.GettblItemMasterStockManager().FindMonthlyValues(SessionId, pid, id);
                string u = null;
                try
                {

                    u = Item.UnitMasterPurchase.UnitName;
                }
                catch { }
                //var v = new { Make = Item.Make, PartNo = Item.PartNo, Unit = u };

                //comment code on 14_04_2020
                ///// var v = new { UnitNo = Item.UnitID, Unit = u ,Opening=mv.Opening, Purchase = mv.Purchase, Issue = mv.Issue };
                // string jsonString = v.ToJSON();
                // return Json(jsonString, JsonRequestBehavior.AllowGet);

                //add code on 14_04_2020
                if (mv != null)
                {
                    var v = new { UnitNo = Item.UnitID, Unit = u, Opening = mv.Opening, Purchase = mv.Purchase, Issue = mv.Issue };
                    string jsonString = v.ToJSON();
                    return Json(jsonString, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var v = new { UnitNo = Item.UnitID, Unit = u, Opening = 0, Purchase = 0, Issue = 0 };
                    string jsonString = v.ToJSON();
                    return Json(jsonString, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }
        //[Authorize]
        public ActionResult ItemOpening()
        {

           
            ViewBag.ItemGroups = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");

            ViewBag.EmpID = EmpID;

                return View();
        }
        public ActionResult SaveOpening(string Project, string ItemGroup, string Item, string Unit, decimal Opening, decimal Rate=0)
        {
            try
            {
                tblItemMasterStock Stock = new tblItemMasterStock();
                Stock.ItemGroup = ItemGroup;
                Stock.ItemNo = Item;
                Stock.ProjectNo = Project;
                Stock.UnitId = Unit;
                Stock.Opening = Opening;
                Stock.Rate = Rate;

                Stock.Sesson = SessionId;
                Stock.CreatedDate = System.DateTime.Now;
                Stock.CreatedBy = EmpID;
                int i = m.GettblItemMasterStockManager().Add(Stock);
                m.GettblItemCurrentStockManager().UpdateForOpening(Stock.ProjectNo,Stock.ItemGroup, Stock.ItemNo, Stock.Opening??0);
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch { return Json("2", JsonRequestBehavior.AllowGet); }
           
            
        }

        public ActionResult ItemOpeningFirst()
        {


            ViewBag.ItemGroups = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");
            var projId = Session["ProjectssId"].ToString();

            if (objmms.tblProjectOpeningAndClosingDates.Any(o => o.ProjectId == projId))
            {
               var opening= objmms.tblProjectOpeningAndClosingDates.Where(x => x.ProjectId == projId).OrderByDescending(x => x.OpeningDate).FirstOrDefault().OpeningDate;
                ViewBag.LastOpeningDateprojectWise = string.Format("{0:dd-MMM-yyyy}", opening) ;
            }
           
           ViewBag.EmpID = EmpID;

            return View();
        }
                   
        public ActionResult SaveOpeningFirst(string Project, string ItemGroup, DateTime OpeningDate, string Item, string Unit, decimal TotalPurchase, decimal Rate = 0, decimal TotalIssue = 0, decimal Theoritical = 0, decimal Physical = 0, decimal Difference = 0, decimal TStockValue = 0, decimal PStockValue = 0,string ProjectName=null, decimal LastOpening = 0 )
        {
            bool Isbool = true; bool IsboolIner = false;
            string ValidateAdmin = string.Empty;
            ValidateAdmin = objmms.tblEmployeeMasters.Where(x => x.EmpID == EmpID).FirstOrDefault().EmpID;
            if (ValidateAdmin == "EMP0000001")
            {
                return Json("2", JsonRequestBehavior.AllowGet);
            }
            else {

                
            #region 
            try
            {

                var a = objmms.GateEntryChilds.Where(b => b.ProjectNo == Project && b.ItemNo == Item).FirstOrDefault();
                if (a == null)
                {
                    var c = objmms.tblItemMasterStocks.Where(d => d.ProjectNo == Project && d.ItemNo == Item).FirstOrDefault();
                    if (c == null)
                    {
                        tblItemMasterStock Stock = new tblItemMasterStock();
                        Stock.ItemGroup = ItemGroup;
                        Stock.ItemNo = Item;
                        Stock.ProjectNo = Project;
                        Stock.UnitId = Unit;
                        Stock.Opening = Physical;
                        Stock.Rate = Rate;
                        Stock.Sesson = SessionId;
                        Stock.CreatedDate = System.DateTime.Now;
                        Stock.CreatedBy = EmpID;

                        Stock.Closing = Physical;
                        Stock.SystemQty = 0;
                        Stock.PhysicalQty = Physical;
                        Stock.DiffFromSystemQty = null;
                        Stock.WastagePercent = null;
                        Stock.WastageQty = null;
                        Stock.ActualQtyAfterWastage = null;
                        Stock.Month = System.DateTime.Now.Month.ToString();
                        Stock.Year = System.DateTime.Now.Year.ToString();

                        Stock.OpeningType = "First";
                        Stock.TotalPurchase = TotalPurchase;
                        Stock.TotalIssue = TotalIssue;
                        Stock.TheoriticalQty = Theoritical;
                        Stock.PStockValue = PStockValue;
                        Stock.TStockValue = TStockValue;
                        Stock.DiffFromTheoritical = Difference;
                        Stock.LastOpening = LastOpening;
                        Stock.OpeningDate = OpeningDate;
                        Stock.CreatedDateNew = System.DateTime.Now;




                        int i = m.GettblItemMasterStockManager().Add(Stock);
                        IsboolIner = true;
                        m.GettblItemCurrentStockManager().UpdateForOpening(Stock.ProjectNo, Stock.ItemGroup, Stock.ItemNo, Stock.Opening ?? 0);
                        IsboolIner = false;
                        if (i == 1)
                        {
                            
                            tblItemMasterFirstOpening S = new tblItemMasterFirstOpening();
                            S.ItemGroup = ItemGroup;
                            S.ItemNo = Item;
                            S.ProjectNo = Project;
                            S.UnitId = Unit;

                            S.Rate = Rate;
                            S.Sesson = SessionId;
                            S.CreatedDate = System.DateTime.Now;
                            S.CreatedBy = EmpID;
                            S.TotalPurchase = TotalPurchase;
                            S.TotalIssue = TotalIssue;
                            S.TheoriticalQty = Theoritical;
                            S.PStockValue = PStockValue;
                            S.TStockValue = TStockValue;
                            S.DiffFromTheoritical = Difference;
                            S.LastOpening = LastOpening;
                            S.CreatedDateNew = System.DateTime.Now;
                            S.OpeningDate = OpeningDate;

                            m.GettblItemMasterStockManager().AddOpeningFirst(S);
                            IsboolIner = true;
                            try { Isbool =SendinLocalPurchase(S, ProjectName, Physical, OpeningDate, Stock.Id); } catch { Isbool = false; }

                        }

                        if (Isbool == IsboolIner)
                        {
                            return Json("1", JsonRequestBehavior.AllowGet);
                        }
                        else {
                            return Json("2", JsonRequestBehavior.AllowGet);
                        }
                       

                    }
                    else {
                        // 3
                        return Json("3", JsonRequestBehavior.AllowGet);

                    }

                }

                else
                {
                    //4
                    return Json("4", JsonRequestBehavior.AllowGet);
                }
            }


            catch { return Json("2", JsonRequestBehavior.AllowGet); }
                #endregion


            }

        }

        public bool SendinLocalPurchase(tblItemMasterFirstOpening S, string ProjectName, decimal PhyCalQuantity,DateTime OpngDate,int ItemStockmasterId)
        {
            bool CheckedBool = false;  bool CheckedInnerBool = false;
            int SiteId = m.GetProjectMasterManager().Find(S.ProjectNo).TransID; ;
            tblItemMaster Item = m.GetGobalItemManager().Find(S.ItemNo);
            int ItemId = Item.TransID;
            string ItemName = Item.ItemName;
            string ItemGroup = m.GetItemTypeManager().Find(S.ItemGroup).GroupName;
            try
            {
                string LPNo = string.Empty; int CashPurId = 0;
                var idMax = m.GetTbCashPurchaseMasterManager().FindMaxId(SessionId, S.ProjectNo);
                if (idMax > 1)
                {
                    LPNo = GetCashprojectcode(S.ProjectNo);
                    CashPurId = idMax;
                }
                else {
                    LPNo = Getprojectcode(S.ProjectNo, "LPO/");
                    CashPurId = 1;
                }



                var ML1 = m.GetTbCashPurchaseMasterManager().Find(LPNo);

                if (ML1 == null)
                {
                    DAL.TbCashPurchaseMaster ML = new DAL.TbCashPurchaseMaster();
                    //ML = new TbCashPurchaseMaster();
                    ML.CashPurchaseId = CashPurId;
                    ML.CashPurchaseNo = LPNo;
                    ML.Complete = "Yes";
                    ML.CreatedBy = Session["EmpID"].ToString();
                    ML.CreatedDate = System.DateTime.Now;
                    ML.CreaterName = Session["Emp_Name"].ToString();
                    ML.Date = System.DateTime.Now;
                    ML.GrandTotal = S.PStockValue;
                    ML.ProjectId = S.ProjectNo;
                    ML.ProjectName = ProjectName;
                    ML.VendorId = "1";
                    ML.VendorName = "Dummy";
                    ML.EmpId = Session["EmpID"].ToString();
                    ML.EmpName = Session["Emp_Name"].ToString();
                    ML.SessionId = SessionId;
                    ML.SiteId = SiteId;
                    try
                    {
                        objmms.TbCashPurchaseMasters.Add(ML);
                        objmms.SaveChanges();
                        CheckedInnerBool = true;
                        //decimal a = m.GetTbCashPurchaseMasterManager().Add(ML);
                    }
                    catch (Exception ex) { ex.ToString(); CheckedInnerBool = false; }

                }
                //else
                //{


                //    ML.CreatedBy = EmpID;
                //    ML.CreatedDate = System.DateTime.Now;
                //    ML.CreaterName = EmpName;
                //    ML.Date = System.DateTime.Now;
                //    ML.GrandTotal = ML.GrandTotal??0+ S.PStockValue??0;


                //    try { m.GetTbCashPurchaseMasterManager().Update(ML); } catch { }

                //}
                var findID = objmms.TbCashPurchaseMasters.Where(x => x.CashPurchaseNo == LPNo).FirstOrDefault().UId;

                TbCashPurchaseChild CL = new TbCashPurchaseChild();
                CL.Amount = S.PStockValue;
                CL.ItemGroup = S.ItemGroup;
                CL.ItemId = ItemId;
                // CL.MUId =ML.UId;
                CL.MUId = findID;
                CL.PurchaseQty = S.TotalPurchase;
                CL.Rate = S.Rate;
                CL.Receive = S.TotalPurchase;
                CL.Unit = S.UnitId;
                try { decimal b = m.GetTbCashPurchaseChildManager().Add(CL); CheckedInnerBool = true; } catch (Exception ex) { ex.ToString(); CheckedInnerBool = false; }

                string GRNo = string.Empty; int GateEntryId = 0;
                var CheckProjExist = objmms.GateEntries.Where(x => x.ProjectNo == S.ProjectNo).FirstOrDefault();
                if (CheckProjExist != null)
                {
                    var GateVal = objmms.GateEntries.Where(x => x.ProjectNo == S.ProjectNo).OrderByDescending(x => x.UId).FirstOrDefault().GateEntryId;
                    int MaxGateval = GateVal.Value + 1;
                    GRNo = GetProjectCodeForExistingGateEntry(S.ProjectNo, MaxGateval);
                    GateEntryId = MaxGateval;
                }
                else {
                    GRNo = Getprojectcode(S.ProjectNo, "GRN/");
                    GateEntryId = 1;
                }





                GateEntry MG = m.GetGateEntryManager().Find(GRNo);

                if (MG == null)
                {
                    MG = new GateEntry();
                    MG.GateEntryNo = GRNo;
                    MG.CreatedBy = Session["EmpID"].ToString();
                    MG.CreatedDate = System.DateTime.Now; ;
                    MG.CreaterName = Session["Emp_Name"].ToString();
                    MG.Date = System.DateTime.Now;
                    MG.EmpId = Session["EmpID"].ToString();
                    MG.EmpName = Session["Emp_Name"].ToString();
                    MG.ProjectName = ProjectName;
                    MG.ProjectNo = S.ProjectNo;
                    MG.SessionId = SessionId;
                    MG.Status = "Opening";
                    //MG.StatusTypeId = ML.UId;
                    MG.StatusTypeId = findID;
                    MG.StatusTypeNo = LPNo;
                    MG.GateEntryId = GateEntryId;
                    try { decimal a = m.GetGateEntryManager().Add(MG); CheckedInnerBool = true; } catch { CheckedInnerBool = false; }

                }
                //else
                //{



                //    MG.CreatedBy = Session["EmpID"].ToString();
                //    MG.CreatedDate = System.DateTime.Now; ;
                //    MG.CreaterName = Session["Emp_Name"].ToString(); ;
                //    MG.Date = System.DateTime.Now;
                //    MG.EmpId = Session["EmpID"].ToString();
                //    MG.EmpName = Session["Emp_Name"].ToString();



                //    try { m.GetGateEntryManager().Update(MG); } catch { }

                //}
                GateEntryChild CG = new GateEntryChild();
                CG.AllAmount = S.PStockValue;
                CG.Amount = S.PStockValue;
                CG.CreatedBy = Session["EmpID"].ToString();
                CG.CreatedDate = System.DateTime.Now;
                CG.Date = System.DateTime.Now;
                CG.GateEntryNo = GRNo;
                CG.InOut = "In";

                CG.Item = ItemName;
                CG.ItemGroup = ItemGroup;
                CG.ItemGroupNo = S.ItemGroup;
                CG.ItemNo = S.ItemNo;


                CG.MUId = MG.UId;
                CG.ProjectName = ProjectName;
                CG.ProjectNo = S.ProjectNo;



                CG.Rate = S.Rate;
                CG.ReceiveQty = S.TotalPurchase;
                CG.SessionId = SessionId;
                CG.Status = "Opening";


                CG.StatusChildId = CL.UId;
                //CG.StatusTypeId = ML.UId;
                CG.StatusTypeId = findID;
                CG.StatusTypeNo = LPNo; // check
                                        // CG.Unit = S.PhysicalQty;

                CG.UnitNo = S.UnitId;
                CG.Vendor = "Dummy Vendor";
                CG.VendorNo = "1";
                CG.GateEntryDate = System.DateTime.Now;

                //CL.SrNo = S.PhysicalQty;
                try { /*m.GetTbCashPurchaseChildManager().Add(CL);*/ m.GetGateEntryChildManager().Add(CG); CheckedInnerBool = true; } catch { CheckedInnerBool = false; }




                DAL.ns_tbl_IssueQuantity I = new DAL.ns_tbl_IssueQuantity();
                I.IndentNo = "Opening";
                I.CreatedDate = System.DateTime.Now;
                I.EmployeeID = Session["EmpID"].ToString();
                I.EmployeeName = Session["Emp_Name"].ToString();
                I.IssuedBy = Session["EmpID"].ToString();
                I.IssueQuantity = S.TotalIssue;
                I.ItemGroupID = S.ItemGroup;
                I.ItemGroupName = ItemGroup;

                I.ItemID = S.ItemNo;
                I.ItemName = ItemName;
                I.ProjectID = S.ProjectNo;
                I.ProjectName = ProjectName;


                I.SessionId = SessionId.ToString();
                I.SiteId = S.ProjectNo;
                I.UnitID = S.UnitId;
                I.Issue_Date = OpngDate;
                I.Gate_ReceiveDate = OpngDate;
                I.GateEntry_UId = ItemStockmasterId;
                I.Receive_Rate = S.Rate;
                I.Issue_Type = "Opening";
                try { objmms.ns_tbl_IssueQuantity.Add(I); objmms.SaveChanges(); CheckedInnerBool = true; /*m.GettblItemMasterStockManager().AddIssue(I); */} catch(Exception ex) { ex.ToString(); CheckedInnerBool = false; }


                DAL.tblReceiveData RD = new DAL.tblReceiveData();
                MSP_Model objmsp = new MSP_Model();
                RD.ID = Convert.ToInt32(MG.UId);
                RD.IDType = "Opening";
                RD.TypeNumber = GRNo;
                RD.TypeDate = OpngDate;
                RD.ReceiveQuantity = S.TotalPurchase;
                RD.IssueQuantity = S.TotalIssue;
                RD.BalanceQuantity = PhyCalQuantity;
                RD.Rate = S.Rate;
                RD.ProjectId = S.ProjectNo;
                RD.ItemGroupId = S.ItemGroup;
                RD.ItemId = S.ItemNo;
                RD.CreatedDate = System.DateTime.Now;
                RD.CreatedBy = S.CreatedBy;
                RD.Status = Convert.ToBoolean(1);
                RD.Isdeleted = Convert.ToBoolean(0);
                RD.FinYear = objmsp.GetFinancialYear();
                try { objmms.tblReceiveDatas.Add(RD); objmms.SaveChanges(); CheckedInnerBool = true; } catch (Exception ex) { ex.ToString(); CheckedInnerBool = false; }

                if (CheckedInnerBool != CheckedBool)
                {
                    CheckedBool = true;
                   
                }
                return CheckedBool;
            }
            catch { return CheckedBool = false; }
        }
        public string Getprojectcode(string ProjectID,string s)
        {
            var sessioncode = m.GetSessionMasterManager().Find(SessionId).Name;

            //int SiteId = m.GetProjectMasterManager().Find(ProjectID).TransID;
            //var idMax = m.GetTbCashPurchaseMasterManager().FindMaxId(SessionId, ProjectID);
            string Code = m.GetProjectMasterManager().FindCode(ProjectID);
            string ProjectCode = "";
            ProjectCode = s + Code + "/" + sessioncode + "/" + 1;

            return ProjectCode;
        }


        public string GetCashprojectcode(string ProjectID)
        {
            MSP_Model objmsp = new MSP_Model();
            var sessioncode = objmsp.GetFinancialYear();
            var idMax = m.GetTbCashPurchaseMasterManager().FindMaxId(SessionId, ProjectID);
            string Code = m.GetProjectMasterManager().FindCode(ProjectID);
            string ProjectCode = "";
            ProjectCode = "LPO/" + Code + "/" + sessioncode + "/" + idMax;

            return ProjectCode;
        }


        public string GetProjectCodeForExistingGateEntry(string ProjectId, int Value)
        {
            MSP_Model objmsp = new MSP_Model();
            var sessioncode = objmsp.GetFinancialYear();
            string Code = m.GetProjectMasterManager().FindCode(ProjectId);
            string ProjectCode = "";
            ProjectCode = "GRN/" + Code + "/" + sessioncode + "/" + Value;

            return ProjectCode;
        }

        public ActionResult ItemOpeningMonthly()
        {

            ViewBag.Month = GetMonthList();
            ViewBag.Year = GetYearList();
            ViewBag.ItemGroups = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");

            ViewBag.EmpID = EmpID;

            return View();
        }

        public ActionResult ItemStock()
        {


           // ViewBag.ItemGroups = new SelectList(m.GettblItemCurrentStockManager().FindItemGroup(), "ItemGroupID", "GroupName", "For All Item Groups");

            ViewBag.EmpID = EmpID;

            return View();
        }
       

        public ActionResult StockGrid( string PrjId = null, int page = 1, string IG = null, string IT = null)
        {

           
            const int pageSize = 15000;

            int Report = 1;

            int totalRows = 0;
            List<VMItemCurrentStock> MasterList = null;
            if (SessionId != 0)
            {


                if (PrjId != null && PrjId != "0" && PrjId != "")
                {
                   
                        totalRows = m.GettblItemCurrentStockManager().FindVMCount(PrjId,IG, IT);
                        MasterList = m.GettblItemCurrentStockManager().FindVM(PrjId,IG,IT, page, pageSize);

                        if (IT != null && IT != "0" && IT != "")
                   {
                       decimal AverageRate = m.GettblItemCurrentStockManager().Average_in_GC(SessionId, PrjId, IT);
                       if (MasterList != null && MasterList.Count() == 1)
                       {
                           Report =2;
                           MasterList.First().AverageRate =decimal.Round( AverageRate,2);
                           MasterList.First().AverageAmount =decimal.Round( (AverageRate * (MasterList.First().Qty??0)),2);
                       }
                   }
                  
                }
                if (MasterList != null)
                {



                    var data = new PagedItemStockModel()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        Master = MasterList.ToList()
                    };
                    if (Request.IsAjaxRequest())
                    {
                        if (Report == 1)
                        {
                            return View("_StockView", data);
                        }
                        else if (Report == 2)
                        {
                            return View("_StockView2", data);
                        }
                       
                    }
                }


            }

            return View("_EmptyView");
        }
      
        public ActionResult MonthlyReport()
        {
            ViewBag.toDate = DateTime.Now.Date;

            // ViewBag.ItemGroups = new SelectList(m.GettblItemCurrentStockManager().FindItemGroup(), "ItemGroupID", "GroupName", "For All Item Groups");
            ViewBag.fromDate = DateTime.Now.Date.AddDays(-31);

            ViewBag.EmpID = EmpID;

            return View();
        }
        public ActionResult ReportGrid(string PrjId = null, string IG = null,  string IT = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (IT == "" || IT == "0" || IT == "null")
                IT = null;
            if (IG == "" || IG == "0" || IG == "null")
                IG = null;
            if (fromDate != null && toDate != null)
            {
                if (fromDate>= toDate )
                {
                    return null;
                }
            }


            if (SessionId != 0)
            {


                if (PrjId != null && PrjId != "0" && PrjId != "")
                {

                  
                        object Stocks = m.GettblItemCurrentStockManager().FindMonthlyReport(SessionId,PrjId,IG, IT , fromDate ,  toDate );
                        string jsonString = Stocks.ToJSON();
                        return Json(jsonString, JsonRequestBehavior.AllowGet);
                  
                   

                }

                return null;

            }

            return null;
        }



        public ActionResult MonthlyItemReportGrid(string PrjId = null, string IG = null)
        {

            if (SessionId != 0)
            {


                if (PrjId != null && PrjId != "0" && PrjId != "")
                {


                    //object Stocks = m.GettblItemMasterStockManager().FindMonthlyValuesForItem(SessionId, PrjId, IG);
                    //string jsonString = Stocks.ToJSON();
                    //return Json(jsonString, JsonRequestBehavior.AllowGet);



                }

                return null;

            }

            return null;
        }












        //public object FindAllGroupLastOpeningInSite(string ItemGroupId, string ProjectId, int SessionId)
        //{
        //    if (ItemGroupId != null && ProjectId != null)
        //    {
        //        object Stocks = m.GettblItemMasterStockManager().FindAllGroupLastOpeningInSite(ItemGroupId, ProjectId, SessionId);
        //        return Stocks;
        //    }
        //    return null;
        //}
        //public object FindOneItemLastOpeningInSite(string ItemId, string ProjectId, int SessionId)
        //{
        //    if (ItemId != null && ProjectId != null)
        //    {
        //        object Stocks = m.GettblItemMasterStockManager().FindOneItemLastOpeningInSite(ItemId, ProjectId, SessionId);
        //        return Stocks;
        //    }
        //    return null;
        //}
        public JsonResult FindAllGroupLastOpeningInSite(string ItemGroup, string Project)
        {
            if (ItemGroup != null && Project != null)
            {
                object Stocks = m.GettblItemMasterStockManager().FindAllGroupLastOpeningInSite(ItemGroup, Project, SessionId);
                string jsonString = Stocks.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [HttpPost]
        public ActionResult LoadData()
        {
            //Get parameters
            var ItemGroup = Request.Form.GetValues("ItemGroup").FirstOrDefault();
            var Project = Request.Form.GetValues("Project").FirstOrDefault();
            // get Start (paging start index) and length (page size for paging)
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Get Sort columns value
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;


            List<VMtblItemMasterStock> v = m.GettblItemMasterStockManager().FindAllGroupLastOpeningInSite(ItemGroup, Project, SessionId);
                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    //v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
           
        }

        public JsonResult FindOneItemLastOpeningInSite(string Item, string Project)
        {
            if (Item != null && Project != null)
            {
                object Stocks = m.GettblItemMasterStockManager().FindOneItemLastOpeningInSite(Item, Project, SessionId);
                string jsonString = Stocks.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public List<SelectListItem> GetMonthList()
        {

            List<SelectListItem> tmpList = new List<SelectListItem>();
            tmpList.Add(new SelectListItem { Text = "Jan", Value = "Jan" });
            tmpList.Add(new SelectListItem { Text = "Feb", Value = "Feb" });
            tmpList.Add(new SelectListItem { Text = "Mar", Value = "Mar" });
            tmpList.Add(new SelectListItem { Text = "Apr", Value = "Apr" });
            tmpList.Add(new SelectListItem { Text = "May", Value = "May" });
            tmpList.Add(new SelectListItem { Text = "Jun", Value = "Jun" });
            tmpList.Add(new SelectListItem { Text = "Jul", Value = "Jul" });
            tmpList.Add(new SelectListItem { Text = "Aug", Value = "Aug" });
            tmpList.Add(new SelectListItem { Text = "Sep", Value = "Sep" });
            tmpList.Add(new SelectListItem { Text = "Oct", Value = "Oct" });
            tmpList.Add(new SelectListItem { Text = "Nov", Value = "Nov" });
            tmpList.Add(new SelectListItem { Text = "Dec", Value = "Dec" });
           
            return tmpList;
        }


        public List<SelectListItem> GetYearList()
        {

            List<SelectListItem> tmpList = new List<SelectListItem>();
            tmpList.Add(new SelectListItem { Text = "2016", Value = "2016" });
            tmpList.Add(new SelectListItem { Text = "2017", Value = "2017" });
            tmpList.Add(new SelectListItem { Text = "2018", Value = "2018" });
            tmpList.Add(new SelectListItem { Text = "2019", Value = "2019" });
            tmpList.Add(new SelectListItem { Text = "2020", Value = "2020" });
            tmpList.Add(new SelectListItem { Text = "2021", Value = "2021" });
           

            return tmpList;
        }

        public void GetAllItemToExcel()
        {
            //Procedure objspr = new Procedure();
            //List<AllItemDetail> gridlst = new List<AllItemDetail>();
            //gridlst = null;
            //gridlst = objspr.GetAllItemListDetails();

            var searchData = TempData["SearchData1"] as List<GetItemsList>;

            if (searchData != null)
            {

                WebGrid grid1 = new WebGrid(source: searchData, canPage: false, canSort: false);

                string gridData = grid1.GetHtml(
                    columns: grid1.Columns(
                          grid1.Column(header: "S.No.", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                           grid1.Column("ItemID", header: "Item ID", style: "text-align:right"),
                           grid1.Column("ItemName", header: "Item Name", style: "text-align:right"),
                           grid1.Column("ItemGroupID", header: "Group Id", style: "text-align:right"),
                           grid1.Column("GroupName", header: "Group Name", style: "text-align:right"),
                            grid1.Column("HSNCode", header: "HSN Code", style: "text-align: center"),
                            grid1.Column("GITEMCode", header: "GITEM Code", style: "text-align: center"),
                           grid1.Column("UnitName", header: "Unit Name", style: "text-align:right"),
                            grid1.Column("Disable", header: "Status", style: "text-align:right")
                            )
                        ).ToString();

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=AllItemDetailList.xls");
                Response.ContentType = "application/excel";
                Response.Write(gridData);
                Response.End();

                TempData.Keep();
            }


        }



    }
}
