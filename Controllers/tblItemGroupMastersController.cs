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
using DataAccessLayer;

namespace MMS.Controllers
{
    public class tblItemGroupMastersController : Controller
    {
        //private Model1 db = new Model1();
        FactoryManager m = new FactoryManager();
        string EmpID = null;
       string CompanyID = null;
       public tblItemGroupMastersController()
        {
            try 
            {
                EmpID = System.Web.HttpContext.Current.Session["EmpID"].ToString();

                CompanyID = System.Web.HttpContext.Current.Session["CompanyID"].ToString();
            }
            catch
            {               
            }
        }
        // GET: tblItemGroupMasters
         //[Authorize]
        public ActionResult Index()
        {
            return View(m.GetItemTypeManager().FindAll());
        }

        // GET: tblItemGroupMasters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblItemGroupMaster itemType = m.GetItemTypeManager().Find(id);
            if (itemType == null)
            {
                return HttpNotFound();
            }
            return View(itemType);
        }

        // GET: tblItemGroupMasters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tblItemGroupMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemGroupID,TransID,CompanyID,GroupName,GroupCode,Status,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate")] tblItemGroupMaster itemType)
        {
            if (ModelState.IsValid)
            {
                itemType.CreatedBy = EmpID;

                itemType.CompanyID = CompanyID;
                itemType.CreatedDate = System.DateTime.Now;
                itemType.Status = "E";
                
               
                
                m.GetItemTypeManager().Add(itemType);
                return RedirectToAction("Index");
            }

            return View(itemType);
        }

        // GET: tblItemGroupMasters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblItemGroupMaster itemType = m.GetItemTypeManager().Find(id);
            if (itemType == null)
            {
                return HttpNotFound();
            }
            return View(itemType);
        }
        public JsonResult IsItemGroupUnique(tblItemGroupMaster catalog)
        {

             return m.GetRepeatChequeManager().ItemGroupRepeat(catalog.GroupName, catalog.TransID)
                ? Json(false, JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }
        // POST: tblItemGroupMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemGroupID,TransID,CompanyID,GroupName,GroupCode,Status,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate")] tblItemGroupMaster itemType)
        {
            if (ModelState.IsValid)
            {
                itemType.ModifiedBy = EmpID;

                itemType.CompanyID = CompanyID;
                itemType.ModifiedDate = System.DateTime.Now;
                
                
                m.GetItemTypeManager().Update(itemType);
                return RedirectToAction("Index");
            }
            return View(itemType);
        }

        // GET: tblItemGroupMasters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblItemGroupMaster itemType = m.GetItemTypeManager().Find(id);
            if (itemType == null)
            {
                return HttpNotFound();
            }
            return View(itemType);
        }

        // POST: tblItemGroupMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            m.GetItemTypeManager().Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
           
            base.Dispose(disposing);
        }
    }
}
