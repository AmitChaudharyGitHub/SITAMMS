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
    public class tblUnitMastersController : Controller
    {
       // private Model1 db = new Model1();
        FactoryManager m = new FactoryManager();
    
       string EmpID = null;
       string CompanyID = null;
        public tblUnitMastersController()
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
        // GET: tblUnitMasters
         //[Authorize]
        public ActionResult Index()
        {

            return View(m.GetUnitMasterManager().FindAll());
        }

        // GET: tblUnitMasters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUnitMaster unitMaster = m.GetUnitMasterManager().Get((int)id);
            if (unitMaster == null)
            {
                return HttpNotFound();
            }
            return View(unitMaster);
        }

        // GET: tblUnitMasters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tblUnitMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UnitID,TransID,UnitName,UnitCode,Status,CreatedBy,CreatedDate,CompanyID")] tblUnitMaster tblUnitMaster)
        {
            if (ModelState.IsValid)
            {
                tblUnitMaster.CreatedBy = EmpID;

                tblUnitMaster.CompanyID = CompanyID;
                tblUnitMaster.CreatedDate = System.DateTime.Now;
                tblUnitMaster.Status = "E";
                m.GetUnitMasterManager().Add(tblUnitMaster);
                
                return RedirectToAction("Index");
            }

            return View(tblUnitMaster);
        }
        public JsonResult IsUnitNameUnique(tblUnitMaster catalog)
        {
        
            
              return    m.GetRepeatChequeManager().UnitNameRepeat(catalog.UnitName, catalog.TransID)
                ? Json(false, JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }
        // GET: tblUnitMasters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUnitMaster unitMaster = m.GetUnitMasterManager().Get((int)id);
            if (unitMaster == null)
            {
                return HttpNotFound();
            }
            return View(unitMaster);
        }

        // POST: tblUnitMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UnitID,TransID,UnitName,UnitCode,Status,CreatedBy,CreatedDate,CompanyID")] tblUnitMaster tblUnitMaster)
        {
            if (ModelState.IsValid)
            {
                m.GetUnitMasterManager().Update(tblUnitMaster) ;

                return RedirectToAction("Index");
            }
            return View(tblUnitMaster);
        }

        // GET: tblUnitMasters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUnitMaster unitMaster = m.GetUnitMasterManager().Get((int)id);
            if (unitMaster == null)
            {
                return HttpNotFound();
            }
            return View(unitMaster);
        }

        // POST: tblUnitMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            m.GetUnitMasterManager().Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            
            base.Dispose(disposing);
        }
    }
}
