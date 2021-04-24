using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MMS.DAL;
using MMS.ViewModels.File;

namespace MMS.Controllers
{
    public class QuotationController : Controller
    {

        Procedure objpro = new Procedure();
        DAL.MMSEntities objmms = new DAL.MMSEntities();
        string EmpId = string.Empty;
        string EmpName = string.Empty;
      //  string ProjectId = string.Empty;
        public QuotationController()
        {
            try
            {
               // ProjectId = System.Web.HttpContext.Current.Session["ProjectssId"].ToString();
                EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
                EmpName = System.Web.HttpContext.Current.Session["Emp_Name"].ToString();
                
            }
            catch(Exception ex)
            {
                ex.ToString();
            }
        }


        // GET: Quotation
        public ActionResult Index()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.Date = DateTime.Now.Date;
            return View();
        }


        public ActionResult GetRemarks(string PI)
        {
           // string Proj_ID = Session["ProjectssId"].ToString();

            string PI_ID = PI.Replace("#", "/");
            string Proj_ID = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PI_ID.ToString()).FirstOrDefault().ProjectNo;

            List<PI_Master_Reamrks> res = null;
            res = objpro.GetAllItemStatusReport(PI_ID, Proj_ID);
           if (res != null)
            {
                return PartialView("_PIReamrksOnQuotation", res);
            }
            else {
                return View("_EmptyView");

            }


        }


        public ActionResult GetQuotationRemark(int TrandId)
        {
            List<GetQuotRemarks> res = null;
            res = objpro.GetQuotRemarks(TrandId);
            if (res != null)
            {
                return PartialView("_Quot_Remarks", res);
            }
            else
            {
                return View("_EmptyView");
            }
          
        }

        public ActionResult AddQuotationList(string ProjectId, string IndentNo = "")
        {
            var result = objpro.GetQtDetail(ProjectId, IndentNo);
            if (result != null)
            {
                return PartialView("_PendingQuotation", result);
            }
            else
            {
                return View("_EmptyView");
            }

        }

        public ActionResult GetUploadQuot(string ProjectId, string IndentNo = "")
        {
            List<QuotDetail> result = null;
            result = objpro.GetUploadedQtDetail(ProjectId, IndentNo);
            if (result != null)
            {
                return PartialView("_UploadQuotation", result);
            }
            else
            {
                return View("_EmptyView");
            }
        }

        public ActionResult AddQuotation(decimal Uid,string Edit="")
        {
            Quot_PIDetail obj = new Quot_PIDetail();
            MMS.DAL.PurchaseRequisitionMaster Master = objmms.PurchaseRequisitionMasters.Where(x => x.UId == Uid).FirstOrDefault();
            var Ptype = objmms.tblPI_PurchaseType.Where(x => x.TrandId == Master.PurchasePI_Type).ToList().First().PurchaseType;
            var P_ststus = objmms.tbl_beforPIdecesionPurchase.Where(k1 => k1.PurReqNo == Master.PurRequisitionNo).ToList().Count() > 0 ? objmms.tbl_beforPIdecesionPurchase.Where(k1 => k1.PurReqNo == Master.PurRequisitionNo).FirstOrDefault().Purchase_Mang_DecesionType : 0;
            var dis_Pur_Status = P_ststus !=0 ?  objmms.tblPIPurchasedecisionTypes.Where(k => k.ID == P_ststus).FirstOrDefault().PurchaseSelectionType : "N/A";
            var child = (from a in objmms.PurchaseRequisitionChilds.Where(x => x.ProjectNo == Master.ProjectNo && x.MId == Master.UId).ToList()
                         select new PIChild_Quot
                         {
                             UId = a.UId,
                             MId = a.MId,
                             PurRequisitionNo = a.PurRequisitionNo,
                             ItemGroupName = a.ItemGroupName,
                             ItemName = a.ItemName,
                             UnitName = a.Unit,
                             AppQuantity = a.ApprovedQty,
                             CurrentRate = a.CurrentRate,
                             TotalValue = Math.Round(Convert.ToDecimal(a.ApprovedQty * a.CurrentRate), 2),
                             Remark = a.Remarks
                         }).ToList();

            var d = new PIMaster_Quot()
            {
                ProjectName = Master.ProjectName,
                PurchaseReqNo = Master.PurRequisitionNo,
                UID = Master.UId,
                PurchaseType = Ptype,
                ProjectId = Master.ProjectNo,
                PIdeliveryDate = Master.DeliveryDate,
                PurchaseStatus = dis_Pur_Status,

            };

            if (d != null)
            {
                int? PurchaseTypeId = 0;
                var findType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList().Count() > 0 ? objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).First().TypeName : "User";
                if (findType == "PIC" || findType == "User")
                {
                    PurchaseTypeId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Master.PurRequisitionNo).FirstOrDefault().PurchasePI_Type;
                }
                else if (findType == "Mang")
                {
                    PurchaseTypeId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Master.PurRequisitionNo).FirstOrDefault().PurchasePIC_Type;

                }
                else
                {
                    PurchaseTypeId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Master.PurRequisitionNo).FirstOrDefault().PurchaseMang_Type;
                }
                if (PurchaseTypeId.HasValue)
                {
                    d.PurchaseType = objmms.tblPI_PurchaseType.SingleOrDefault(x => x.TrandId == PurchaseTypeId).PurchaseType;
                }
                
            }



            var data = new Quot_PIDetail()
            {
                Pmaster = d,
                Pchild = child
            };

            if (!string.IsNullOrEmpty(Edit))
            {
                var editData=objmms.tblMMSQuotations.FirstOrDefault(x=>x.UID==Uid);
                if (editData != null)
                {
                    d.Quotref = editData.QuotationRefNo;
                    d.Quotremarks = editData.Remarks;
                    var documents=objmms.tblMMSQuotation_Document.Where(x => x.QuotationTransId == editData.TransId).ToList();
                    ViewBag.Documents = documents;

                    ViewBag.QuotationTransId = editData.TransId;
                }
            }

            ViewBag.ID = Uid;

            return View(data);

           
        }

        [HttpPost]
      // [ValidateAntiForgeryToken]
        public ActionResult SaveQuot(Quot_PIDetail px, IEnumerable<HttpPostedFileBase> files, FormCollection frm)
        {
            DownloadFiles obj = new DownloadFiles();
            tblMMSQuotation Qm = new tblMMSQuotation();

            if(files.Any(x=>x.ContentType!= "application/pdf" 
            && x.ContentType != "application/docx"
            && x.ContentType != "application/doc"
            && x.ContentType != "application/xlsx"
            && x.ContentType != "application/xls"
            && x.ContentType != "image/jpg"
            && x.ContentType != "text/plain"
            && x.ContentType != "image/png"
            && x.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            && x.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
            {
                TempData["error"] = "Please upload a valid file.";
                if(frm["isEdit"] != null && frm["isEdit"] != "")
                  return RedirectToAction("AddQuotation", "Quotation", new { Uid = px.Pmaster.UID, Edit = 1 });
                else
                    return RedirectToAction("AddQuotation", "Quotation", new { Uid = px.Pmaster.UID});
            }

            if (frm["isEdit"] != null && frm["isEdit"]!="")
            {
                tblMMSQuotation dataToUpdate = objmms.tblMMSQuotations.FirstOrDefault(x=>x.UID==px.Pmaster.UID);
                if (dataToUpdate != null)
                {
                    dataToUpdate.QuotationRefNo = px.Pmaster.Quotref;
                    dataToUpdate.Remarks = px.Pmaster.Quotremarks;
                    objmms.Entry(dataToUpdate).State = System.Data.Entity.EntityState.Modified;
                }
            }
            else
            {
                Qm.CreatedBy = EmpId;
                Qm.CreatedDate = System.DateTime.Now;
                Qm.IsDeleted = Convert.ToBoolean(0);
                Qm.ProjectId = px.Pmaster.ProjectId;
                Qm.PurchaseReqNo = px.Pmaster.PurchaseReqNo;
                Qm.QuotationRefNo = px.Pmaster.Quotref;
                Qm.QuotationStatus = "Approved";
                Qm.Remarks = px.Pmaster.Quotremarks;
                Qm.Status = Convert.ToBoolean(1);
                Qm.TotalItemValue = Convert.ToDecimal(0.00);
                Qm.UID = px.Pmaster.UID;
                objmms.tblMMSQuotations.Add(Qm);
            }


            string directory = Server.MapPath("~/UploadedFiles");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            int count = 0;
            if (files != null)
            {
                List<tblMMSQuotation_Document> QdL = new List<tblMMSQuotation_Document>();
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        tblMMSQuotation_Document Qd = new tblMMSQuotation_Document();
                        // var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        //var fileName = string.Empty;
                        //var filesCol = obj.GetFiles().ToList();
                        //var CurrentFileName = (from fls in filesCol
                        //                       where fls.FileName == file.FileName
                        //                       select fls).ToList();
                        //if (CurrentFileName.Count() > 0)
                        //{
                        //    fileName = Path.GetFileNameWithoutExtension(file.FileName)  + "-Copy(" + CurrentFileName.Count() + " )" + Path.GetExtension(file.FileName) ;
                        //}
                        //else
                        //{
                        //    fileName = Path.GetFileNameWithoutExtension(file.FileName) + Path.GetExtension(file.FileName);
                        //}


                        var path = Path.Combine(Server.MapPath("~/UploadedFiles/"), Guid.NewGuid().ToString());


                        Qd.CreatedBy = EmpId;
                        Qd.CreatedDate = System.DateTime.Now;
                        Qd.FileName = Path.GetFileNameWithoutExtension(file.FileName);
                        Qd.FilePath = path;
                        Qd.FileStatus = Convert.ToBoolean(1);
                        Qd.FileType = Path.GetExtension(file.FileName);
                        Qd.ProjectId = px.Pmaster.ProjectId;
                        Qd.PurchaseReqNo = px.Pmaster.PurchaseReqNo;
                        if (frm["isEdit"] != null && frm["isEdit"] != "")
                            Qd.QuotationTransId =Convert.ToInt32(frm["isEdit"]);
                        else
                            Qd.QuotationTransId = FindMaxId(px.Pmaster.ProjectId);
                        


                        QdL.Add(Qd);
                    
                       file.SaveAs(path);
                    

                       count++;
                    }
                }
                objmms.tblMMSQuotation_Document.AddRange(QdL);
            }
           // objmms.tblMMSQuotations.Add(Qm);
           
            objmms.SaveChanges();
            ViewBag.Message = string.Format("Quotation is sucessfully uploaded");
            ViewBag.EmpId = EmpId;
            ViewBag.Date = DateTime.Now.Date;
            return View("Index");
        }


        public int FindMaxId(string PjId)
        {
           
             
               int a = objmms.tblMMSQuotations.Select(i => i.TransId).DefaultIfEmpty(0).Max();

                if (a == 0)
                    return 1;
                else
                    return a + 1;
               
           
        }


        public ActionResult GetDownloadFileDetail(int TransId)
        {
            DownloadFiles obj = new DownloadFiles();
            List<DownLoadFileInformation> list = new List<DownLoadFileInformation>();
            List<tblMMSQuotation_Document> md = objmms.tblMMSQuotation_Document.Where(x => x.QuotationTransId == TransId).ToList();
            foreach (var i in md)
            {
                DownLoadFileInformation fr = new DownLoadFileInformation();
                //var Fname = md.Where(x => x.DocID == i.DocID).ToList().Select(p => p.FileName + p.FileType).FirstOrDefault();
                // var Fext = objmms.tblMMSQuotation_Document.Where(x => x.QuotationTransId == TransId).ToList().Select(p => p.FileType).Max();
                //string result = Fname;

                //fr = obj.GetSelectedFile(result.ToString());

                fr = obj.GetSelectedFile(i.DocID,i.FilePath,i.FileName);

                list.Add(fr);
            }


            if (list != null)
            {
                return PartialView("_downLoadQuotFile", list);
            }
            else {
                return View("_EmptyView");
            }
      
        }
        public FileResult Download(int FileID)
        {
            DownloadFiles obj = new DownloadFiles();
            int CurrentFileID = Convert.ToInt32(FileID);

            var fileData=objmms.tblMMSQuotation_Document.SingleOrDefault(x => x.DocID == FileID);


            string contentType = string.Empty;

            if (fileData.FileType.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (fileData.FileType.Contains(".PDF"))
            {
                contentType = "application/PDF";
            }

            else if (fileData.FileType.Contains(".docx"))
            {
                contentType = "application/docx";
            }
            else if (fileData.FileType.Contains(".doc"))
            {
                contentType = "application/doc";
            }
            else if (fileData.FileType.Contains(".xlsx"))
            {
                contentType = "application/xlsx";
            }
            else if (fileData.FileType.Contains(".jpg") || fileData.FileType.Contains(".jpeg"))
            {
                contentType = "image/jpg";
            }
            else if (fileData.FileType.Contains(".txt"))
            {
                contentType = "text/plain";
            }
            else if (fileData.FileType.Contains(".xls"))
            {
                contentType = "application/xls";
            }
            else if (fileData.FileType.Contains(".png"))
            {
                contentType = "image/png";
            }
            var data = System.IO.File.ReadAllBytes(fileData.FilePath);
            return File(data, contentType, fileData.FileName+fileData.FileType);
        }


        public ActionResult ViewQuotDetail(string Id)
        {
            // var data = new QuotDetail();
            List<QuotDetail> qt = null;
            qt = objpro.GetParticularQtDetail(Id);
            QuotDetail h = qt.FirstOrDefault();

            // data = qt;
            if (h != null)
                return View(h);
            else
            {
                ViewBag.Message = "No Quotation/Attachment found.";
                return View("_EmptyView1");
            }
                
        }
    }
}