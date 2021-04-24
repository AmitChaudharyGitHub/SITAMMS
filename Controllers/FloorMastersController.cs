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

namespace MMS.Controllers
{
    public class FloorMastersController : Controller
    {
      
        FactoryManager m = new FactoryManager();
        int Id = 0;
        
       
        private void FIllBySession()
        {
            if (System.Web.HttpContext.Current.Session["Id"] != null)
                Id = Convert.ToInt32(this.Session["Id"].ToString());
          

        }
      
        // GET: FloorMasters
      // [Authorize]
        public ActionResult Index()
        {

            ViewBag.SiteId = new SelectList(m.GetProjectMasterManager().FindAll(), "TransID", "ProjectName");
          
             return View(); 


        }

        public ActionResult Grid(int BlockId = 0, int page = 1, string sort = "FloorName", string sortDir = "ASC")
        {
            const int pageSize = 5000;
            sortDir = sortDir.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? sortDir : "asc";
            var validColumns = new[] { "FloorId", "FloorName" };
            if (!validColumns.Any(c => c.Equals(sort, StringComparison.CurrentCultureIgnoreCase)))
                sort = "FloorId";

            if (BlockId != 0)
            {
                Id = BlockId;
                this.Session["Id"] = Id.ToString();
            }
            else
            {
                try { FIllBySession(); }
                catch { }
            }
            if (Id == 0)
            {
              //  Id = 1;
                this.Session["Id"] = Id;
            }
          
            
            if (Id != 0)
            {

                BlockMaster BlockMaster = m.GetBlockMasterManager().Find(Id);
                if (BlockMaster != null)
                {
                    var totalRows = m.GetFloorMasterManager().CountByBlockMaster(BlockMaster.BlockId);
                    var BlockMasters = m.GetFloorMasterManager().FindPageByBlockMaster(BlockMaster.BlockId, page, pageSize);
                
                    var data = new PagedFloorMasterModel1()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        FloorMasterBll = BlockMasters.ToList()
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

        [HttpPost]
        public ActionResult GetBlockBySiteId(int Siteid)
        {
          
            List<BlockMaster> Block = m.GetBlockMasterManager().FindByProjectMaster(Siteid);

            SelectList obgBlock = new SelectList(Block, "BlockId", "BlockName", 0);
            return Json(obgBlock);
        }

        // GET: FloorMasters/Details/5
        public ActionResult Details(int? id)
        {
          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FloorMaster floorMaster = m.GetFloorMasterManager().Find(id);
            if (floorMaster == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
            {


                return PartialView(floorMaster);
            }
            else
                return View(floorMaster);
           
        }

        // GET: FloorMasters/Create
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
            //
            
        }

        // POST: FloorMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FloorId,BlockId,FloorName")] FloorMaster floorMaster)
        {
          

            
                if (ModelState.IsValid)
                {

                    int i = m.GetFloorMasterManager().Add(floorMaster);
                    if (i != 0)
                    {
                        //TempData["OperStatus"] = "Floor added succeessfully in " + floorMaster.BlockMaster.BlockName + " Group";
                        ModelState.Clear();
                        if (Request.IsAjaxRequest())
                        {
                            return Json(new { success = true });
                        }
                        return RedirectToAction("Index");
                    }
                }

                ViewBag.SiteId = new SelectList(m.GetProjectMasterManager().FindAll(), "TransID", "ProjectName");
               

            if (Request.IsAjaxRequest())
            { return PartialView(floorMaster); }
            else
            { return View(floorMaster); }
            //
           
        }

        // GET: FloorMasters/Edit/5
        public ActionResult Edit(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FloorMaster floorMaster = m.GetFloorMasterManager().Find(id);
            if (floorMaster == null)
            {
                return HttpNotFound();
            }
            ViewBag.BlockId = new SelectList( m.GetBlockMasterManager().FindByProjectMaster(floorMaster.BlockMaster.SiteId), "BlockId", "BlockName", floorMaster.BlockId);
            if (Request.IsAjaxRequest())
            {
                return PartialView(floorMaster);
            }
            return View(floorMaster);
           
        }

        // POST: FloorMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FloorId,BlockId,FloorName")] FloorMaster floorMaster)
        {
           
        
            if (ModelState.IsValid)
            {
                 m.GetFloorMasterManager().Update(floorMaster);

                TempData["OperStatus"] = "Floor added suceessfully";
                ModelState.Clear();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true });
                }
                return RedirectToAction("Index");
            }

            ViewBag.BlockId = new SelectList(m.GetBlockMasterManager().FindByProjectMaster(floorMaster.BlockMaster.SiteId), "BlockId", "BlockName", floorMaster.BlockId);

            if (Request.IsAjaxRequest())
            { return PartialView(floorMaster); }
            return View(floorMaster);
            
            //
         
        }

        // GET: FloorMasters/Delete/5
        public ActionResult Delete(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FloorMaster floorMaster = m.GetFloorMasterManager().Find(id);
            if (floorMaster == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
            { return PartialView(floorMaster); }
            return View(floorMaster);
            //
          
        }

        // POST: FloorMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           
            bool a = m.GetFloorMasterManager().Delete(id);
            if (Request.IsAjaxRequest())
            {
                if (a == true)
                    return Json(new { success = true });
                else
                    return Json(new { success = false });
            }
            return RedirectToAction("Index");
            //
          
        }
        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }
       
    }
}
