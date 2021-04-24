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
using PagedList;
using DataAccessLayer;
namespace MMS.Controllers
{
    public class BlockMastersController : Controller
    {
        private Model1 db = new Model1();
        FactoryManager m = new FactoryManager();
        int Id = 0;
        
        private void FIllBySession()
        {
            if (System.Web.HttpContext.Current.Session["Id"] != null)
                Id = Convert.ToInt32(this.Session["Id"].ToString());
           
         

        }
      
        // GET: BlockMasters
        // [Authorize]
        public ActionResult Index()
        {

            ViewBag.SiteId = new SelectList(m.GetProjectMasterManager().FindAll(), "TransID", "ProjectName", Id);            
                    return View();             

          
        }
        public ActionResult Grid(int SiteId = 0, int page = 1, string sort = "BlockName", string sortDir = "ASC")
        {
            const int pageSize = 5000;
            sortDir = sortDir.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? sortDir : "asc";
            var validColumns = new[] { "BlockId", "BlockName" };
            if (!validColumns.Any(c => c.Equals(sort, StringComparison.CurrentCultureIgnoreCase)))
                sort = "BlockId";

            if (SiteId != 0)
            {
                Id = SiteId;
                this.Session["Id"] = Id.ToString();
            }
            else
            {
                try { FIllBySession(); }
                catch { }
            }
            if (Id == 0)
            {
                //Id = 3;
                this.Session["Id"] = Id;
            }
            

            if (Id != 0)
            {
                tblProjectMaster ProjectMaster = m.GetProjectMasterManager().Find(Id);
                if (ProjectMaster != null)
                {
                    var totalRows = m.GetBlockMasterManager().CountByProjectMaster(ProjectMaster.TransID);
                    var ProjectMasters = m.GetBlockMasterManager().FindPageByProjectMaster(ProjectMaster.TransID, page, pageSize);
                
                   var data = new PagedBlockMasterModel1()
                   {
                       TotalRows = totalRows,
                       PageSize = pageSize,
                       BlockMasterBll = ProjectMasters.ToList()
                   };
                   
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_GridView", data);
                    }
                    else { return View("_GridView", data); }

                }
                else { return View("_EmptyView"); }
            }
            else { return View("_EmptyView"); }


        }
       
        // GET: BlockMasters/Details/5
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlockMaster blockMaster = m.GetBlockMasterManager().Find((int)id);
            if (blockMaster == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
            {


                return PartialView(blockMaster);
            }
            else
            return View(blockMaster);
        }

        // GET: BlockMasters/Create
        public ActionResult Create()
        {
           
            ViewBag.SiteId = new SelectList(m.GetProjectMasterManager().FindAll(), "TransID", "ProjectName");
            if (Request.IsAjaxRequest())
            {
                ViewBag.IsUpdate = false;
                return PartialView();
            }
            else
            return View();
        }

        // POST: BlockMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BlockId,SiteId,BlockName")] BlockMaster blockMaster)
        {
           
            if (ModelState.IsValid)
            {

                int i = m.GetBlockMasterManager().Add(blockMaster);
                if (i != 0)
                {
                    TempData["OperStatus"] = "Block added succeessfully in " + blockMaster.tblProjectMaster.ProjectName + " Group";
                    ModelState.Clear();
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true });
                    }
                    return RedirectToAction("Index");
                }
            }


            ViewBag.SiteId = new SelectList(m.GetProjectMasterManager().FindAll(), "TransID", "ProjectName", blockMaster.SiteId);
          
           
            if (Request.IsAjaxRequest())
            { return PartialView(blockMaster); }
            else
            { return View(blockMaster); }
            
        }

        // GET: BlockMasters/Edit/5
        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlockMaster blockMaster = m.GetBlockMasterManager().Find((int)id);
            if (blockMaster == null)
            {
                return HttpNotFound();
            }
            ViewBag.SiteId = new SelectList(m.GetProjectMasterManager().FindAll(), "TransID", "ProjectName", blockMaster.SiteId);
          
            if (Request.IsAjaxRequest())
            {
                return PartialView(blockMaster);
            }
            return View(blockMaster);
            
        }

        // POST: BlockMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BlockId,SiteId,BlockName")] BlockMaster blockMaster)
        {
           
            if (ModelState.IsValid)
            {
                m.GetBlockMasterManager().Update(blockMaster);
                TempData["OperStatus"] = "Block updated succeessfully";
                ModelState.Clear();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true });
                }
                return RedirectToAction("Index");
            }

            ViewBag.SiteId = new SelectList(m.GetProjectMasterManager().FindAll(), "TransID", "ProjectName", blockMaster.SiteId);
        
            if (Request.IsAjaxRequest())
            { return PartialView(blockMaster); }
            return View(blockMaster);
           
        }

        // GET: BlockMasters/Delete/5
        public ActionResult Delete(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlockMaster blockMaster = m.GetBlockMasterManager().Find((int)id);
            if (blockMaster == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
            { return PartialView(blockMaster); }
            return View(blockMaster);
           
        }

        // POST: BlockMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           

           
            bool a = m.GetBlockMasterManager().Delete(id);
            if (Request.IsAjaxRequest())
            {
                if (a == true)
                    return Json(new { success = true });
                else
                    return Json(new { success = false });
            }
            return RedirectToAction("Index");
           
        }
        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }
       
    }
}
