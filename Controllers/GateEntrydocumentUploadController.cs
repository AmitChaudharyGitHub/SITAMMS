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
    public class GateEntrydocumentUploadController : Controller
    {
        // GET: GateEntrydocumentUpload
        string EmpId = string.Empty; DAL.MMSEntities objmms = new DAL.MMSEntities();
        Procedure objpr = new Procedure(); 
        public GateEntrydocumentUploadController()
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
            ViewBag.Status = GetStatusListP();
           

            return View();
        }

        public List<SelectListItem> GetStatusListP()
        {

            List<SelectListItem> tmpList = new List<SelectListItem>();

            tmpList.Add(new SelectListItem { Text = "Local Purchase", Value = "LP" });
            tmpList.Add(new SelectListItem { Text = "Indent Purchase Order", Value = "IPO" });
            return tmpList;
        }
        public ActionResult GrnGrid(int Grid) 
        {
            GRNViewModel rd = new GRNViewModel();
            rd.GHD = new List<GrnGrid>();
            // List<GRNViewModel> res = null;
            rd.GHD.AddRange(objpr.GetGRnList(Grid));
             //= objpr.GetGRnList(Grid);
            if (rd != null)
            {
                return PartialView("_GrnList", rd);
            }
            else {
                return PartialView("_EmptyView");
            }
           
        }

        [HttpPost]
        public ActionResult GrnUpload(HttpPostedFileBase[] files, GRNViewModel modl)
        {
            int i = 0;  int count = 0;
            DownloadFiles obj = new DownloadFiles();
            foreach (var gchld in modl.GHD)
            {
                if (gchld != null)
                {
                    i = i + 1;



                    if (files != null)
                    {
                        int j = 0;
                        List<tblMMSGateEntryFileDirectory> objd = new List<tblMMSGateEntryFileDirectory>();

                        foreach (var fl in files)
                        {
                            if (fl != null && fl.ContentLength > 0)
                            {
                                j = j + 1;

                                if (i == j)
                                {
                                    tblMMSGateEntryFileDirectory tb = new tblMMSGateEntryFileDirectory();
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


                                    var path = Path.Combine(Server.MapPath("~/GateEntryUploadFiles"), fileName);
                                    tb.CreatedDate = System.DateTime.Now;
                                    tb.UploadedBy = EmpId;
                                    tb.FileName = Path.GetFileNameWithoutExtension(fl.FileName);
                                    tb.FilePath = path;
                                    tb.FileStatus = Convert.ToBoolean(1);
                                    tb.FileType = Path.GetExtension(fl.FileName);
                                    tb.ProjectId = gchld.ProjectNo;
                                    tb.GateEntryChild_Id = gchld.UId;
                                    objd.Add(tb);
                                    fl.SaveAs(path);


                                    count++;
                                }
                                else { }
                            }
                        }

                        objmms.tblMMSGateEntryFileDirectories.AddRange(objd);
                       

                    }

                }
            }

            objmms.SaveChanges();
            ViewBag.EmpId = EmpId;
            ViewBag.Date = DateTime.Now.Date;
            ViewBag.Status = GetStatusListP();
            return View("UploadFile");

        }

    }
}