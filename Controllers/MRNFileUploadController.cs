using MMS.DAL;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class MRNFileUploadController : Controller
    {
        // GET: MRNFileUpload
        string EmpId = string.Empty; DAL.MMSEntities objmms = new DAL.MMSEntities();
        Procedure objpr = new Procedure();

        public MRNFileUploadController()
        {
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UploadFile()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.Status = GetStatusList();


            return View();
        }

        public List<SelectListItem> GetStatusList()
        {

            List<SelectListItem> tmpList = new List<SelectListItem>();

            tmpList.Add(new SelectListItem { Text = "Emergency Purchase", Value = "LP" });
            tmpList.Add(new SelectListItem { Text = "Normal Purchase", Value = "IPO" });
            return tmpList;
        }

        public JsonResult GetMRN(string Status,string ProjectId)
        {
            string j = null ;
            var res = objmms.GateEntries.Where(x => x.Status == Status && x.ProjectNo == ProjectId).ToList().Select(p => new { Text = p.GateEntryNo, Value = p.UId }).ToList();
            j = res.ToJSON();
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMRNGrid(string MRNNo)
        {
            GRNViewModel rd = new GRNViewModel();
            rd.GHD = new List<GrnGrid>();
            rd.GHD.AddRange(objpr.GetMRNList(MRNNo));
           
            if (rd != null)
            {
                return PartialView("_MRNGrid", rd);
            }
            else {
                return PartialView("_EmptyView");
            }
        }
        public ActionResult MRNFileUploadData(HttpPostedFileBase[] files, GRNViewModel modl)
        {
            int i = 0; int count = 0;
            DownloadFiles obj = new DownloadFiles();
            foreach (var gchld in modl.GHD)
            {
                if (gchld != null)
                {
                    i = i + 1;



                    if (files != null)
                    {
                        int j = 0;
                        List<tblMMSMRNEntryFileDirectory> objd = new List<tblMMSMRNEntryFileDirectory>();

                        foreach (var fl in files)
                        {
                            if (fl != null && fl.ContentLength > 0)
                            {
                                j = j + 1;

                                if (i == j)
                                {
                                    tblMMSMRNEntryFileDirectory tb = new tblMMSMRNEntryFileDirectory();
                                    var fileName = string.Empty;
                                    var filesCol = obj.GetFilesForGateEntry().ToList();
                                    var CurrentFileName = (from fls in filesCol
                                                           where fls.FileName == fl.FileName
                                                           select fls).ToList();
                                    if (CurrentFileName.Count() > 0)
                                    {
                                        fileName = Path.GetFileNameWithoutExtension(fl.FileName) + "-Copy(" + CurrentFileName.Count() + " )" + Path.GetExtension(fl.FileName);
                                    }
                                    else
                                    {
                                        fileName = Path.GetFileNameWithoutExtension(fl.FileName) + Path.GetExtension(fl.FileName);
                                    }


                                    var path = Path.Combine(Server.MapPath("~/MRNFileUpload"), fileName);
                                    tb.CreatedDate = System.DateTime.Now;
                                    tb.UploadedBy = EmpId;
                                    tb.FileName = Path.GetFileNameWithoutExtension(fl.FileName);
                                    tb.FilePath = path;
                                    tb.FileStatus = Convert.ToBoolean(1);
                                    tb.FileType = Path.GetExtension(fl.FileName);
                                    tb.ProjectId = gchld.ProjectNo;
                                    tb.MRN_Child_Id = gchld.UId;
                                    objd.Add(tb);
                                    fl.SaveAs(path);


                                    count++;
                                }
                                else { }
                            }
                        }

                        objmms.tblMMSMRNEntryFileDirectories.AddRange(objd);


                    }

                }
            }

            objmms.SaveChanges();
            ViewBag.EmpId = EmpId;
            ViewBag.Date = DateTime.Now.Date;
            ViewBag.Status = GetStatusList();
            return View("UploadFile");
        }


    }
}