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
    public class tblRegistrationTypesController : Controller
    {
       
        FactoryManager m = new FactoryManager();
        string EmpID = null;
       string CompanyID = null;
       public tblRegistrationTypesController()
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
        // GET: tblRegistrationTypes
        // [Authorize]
        public ActionResult Index()
        {
            return View(m.GetVendorGroupManager().FindAll());
        }

        // GET: tblRegistrationTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblRegistrationType vendorGroup = m.GetVendorGroupManager().Find(id);
            if (vendorGroup == null)
            {
                return HttpNotFound();
            }
            return View(vendorGroup);
        }

        // GET: tblRegistrationTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tblRegistrationTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TypeID,TransID,CompanyID,RegistrationType,Status,CreatedBy,CreatedDate")] tblRegistrationType tblRegistrationType)
        {
            if (ModelState.IsValid)
            {
                tblRegistrationType.CreatedBy = EmpID;

                tblRegistrationType.CompanyID = CompanyID;
                tblRegistrationType.CreatedDate = System.DateTime.Now;
                tblRegistrationType.Status = "E";
               
                m.GetVendorGroupManager().Add(tblRegistrationType);

                return RedirectToAction("Index");
            }

            return View(tblRegistrationType);
        }

        // GET: tblRegistrationTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblRegistrationType vendorGroup = m.GetVendorGroupManager().Find(id); 
            if (vendorGroup == null)
            {
                return HttpNotFound();
            }
            return View(vendorGroup);
        }
        public JsonResult IsRegistrationTypeUnique(tblRegistrationType catalog)
        {

            return m.GetRepeatChequeManager().RegistrationTypeRepeat(catalog.RegistrationType, catalog.TransID)
                ? Json(false, JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }
        // POST: tblRegistrationTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TypeID,TransID,CompanyID,RegistrationType,Status,CreatedBy,CreatedDate")] tblRegistrationType tblRegistrationType)
        {
            if (ModelState.IsValid)
            {
              
                m.GetVendorGroupManager().Update(tblRegistrationType);
                return RedirectToAction("Index");
            }
            return View(tblRegistrationType);
        }

        // GET: tblRegistrationTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblRegistrationType vendorGroup = m.GetVendorGroupManager().Find(id);
            if (vendorGroup == null)
            {
                return HttpNotFound();
            }
            return View(vendorGroup);
        }

        // POST: tblRegistrationTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            m.GetVendorGroupManager().Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
           
            base.Dispose(disposing);
        }
    }
}
