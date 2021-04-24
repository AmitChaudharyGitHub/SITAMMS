using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    [Authorize]
    public class BillUploadController : Controller
    {
        DAL.MMSEntities db = new DAL.MMSEntities();
       
        // GET: BillUpload
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPurchaseOrders(string ProjectId)
        {
            var pos=db.GateEntries.Where(x => x.ProjectNo == ProjectId && x.Status!="OSP" && x.Status != "ISP" && x.Status!= "Opening").OrderBy(x => x.StatusTypeNo).Select(x => new { Text = x.StatusTypeNo, Value = x.StatusTypeNo }).Distinct().ToList();
            return Json(pos.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMRN(string ProjectId,string PO)
        {
            var mrns = db.GateEntries.Where(x => x.ProjectNo == ProjectId && x.StatusTypeNo==PO && x.MRN_No_New!=null).OrderBy(x => x.MRN_No_New).Select(x => new { Text = x.MRN_No_New, Value = x.MRN_No_New }).ToList();
            return Json(mrns.ToJSON(), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetBillData(string ProjectId,string PONo,string MRN)
        {
            if (!string.IsNullOrEmpty(ProjectId))
            {
                var pendingData = (from a in db.GateEntries
                                   join b in db.TbIndentPurchaseOrder_GST on a.StatusTypeNo equals b.PurchaseOrderNo
                                   join c in db.tblVendorMasters on b.SupplierNo equals c.VendorID
                                   where a.BillFileId == null && a.ProjectNo==ProjectId && a.Status != "OSP" && a.Status != "ISP" 
                                     && a.Status != "Opening" && (PONo=="" || a.StatusTypeNo==PONo) && (MRN=="" || a.MRN_No_New==MRN)
                                     orderby a.MRN_No_New
                                     select new {a.UId,a.StatusTypeNo,a.MRN_No_New,a.ChallanNo,a.ChallanDate,VendorName=c.Name }).ToList().ToJSON();

                var updatedData = (from a in db.GateEntries
                                   join b in db.TbIndentPurchaseOrder_GST on a.StatusTypeNo equals b.PurchaseOrderNo
                                   join c in db.tblVendorMasters on b.SupplierNo equals c.VendorID
                                   join d in db.tblUploadedBills on a.BillFileId equals d.Id
                                   where a.BillFileId != null && a.ProjectNo == ProjectId && a.Status != "OSP" && a.Status != "ISP"
                                     && a.Status != "Opening" && (PONo == "" || a.StatusTypeNo == PONo) && (MRN == "" || a.MRN_No_New == MRN)
                                   orderby a.MRN_No_New
                                   select new { a.UId, a.StatusTypeNo, a.MRN_No_New, a.ChallanNo, a.ChallanDate,a.BillFileId,VendorName = c.Name,Remarks=d.Remarks }).ToList().ToJSON();

                return Json(new { Status = 1,PendingData=pendingData,UpdatedData=updatedData}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = 2 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadBillFile(HttpPostedFileBase[] billFile, int hdn,string remarks)
        {
            if (billFile.Count() == 0 || hdn == 0)
            {
                TempData["Msg"] = "Error in uploading file.";
                return RedirectToAction("index");
            }
            else
            {
                using (var trans=db.Database.BeginTransaction())
                {
                    try
                    {
                        var mrn = db.GateEntries.SingleOrDefault(x => x.UId == hdn);
                        var fileName = Guid.NewGuid().ToString();
                        var ext = Path.GetExtension(billFile[0].FileName);
                        if (ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".pdf")
                        {
                            billFile[0].SaveAs(Server.MapPath("~/UploadedBillFiles") + "/" + fileName + ext);
                            var file = new DAL.tblUploadedBill
                            {
                                CreatedBy = Session["EmpID"].ToString(),
                                CreatedDate = DateTime.Now,
                                Extension = ext,
                                Filename = billFile[0].FileName,
                                FileName1 = fileName,
                                FileType = billFile[0].ContentType,
                                status = true,
                                MRN=mrn.MRN_No_New,
                                Remarks=remarks
                            };

                            db.tblUploadedBills.Add(file);
                            db.SaveChanges();

                            
                            mrn.BillFileId = file.Id;
                            db.Entry(mrn).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            trans.Commit();

                            TempData["Msg"] = "File Uploaded Successfully.";
                            return RedirectToAction("index");
                        }
                        else
                        {
                            TempData["Msg"] = "Please select a valid file.";
                            return RedirectToAction("index");
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        TempData["Msg"] = "Error in file uploading.";
                        return RedirectToAction("index");
                    }
                }
               
            }
        }

        public JsonResult GetAllUploadedBills(string MRN)
        {
            return Json(db.tblUploadedBills.Where(x => x.MRN == MRN).OrderBy(x => x.Id).Select(x => new { x.Id, x.Filename, x.Remarks,x.CreatedDate }).ToList().ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public FileResult GetUploadBillFile(int f)
        {
            var file = db.tblUploadedBills.SingleOrDefault(x => x.Id == f);
            var s=System.IO.File.Open(Server.MapPath("~/UploadedBillFiles") + "/" +file.FileName1+file.Extension,FileMode.Open);
            return File(s, file.FileType,file.Filename);
        }

    }
}