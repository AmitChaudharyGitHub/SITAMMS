using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.Models;
using MMS.DAL;
using System.Data;
using MMS_P.ViewModels;
using MMS.ViewModels;
using System.Data.Entity;
using System.Linq.Dynamic;

namespace MMS.Controllers
{
    public class Date_WiseController : MySiteController
    {
        // GET: Date_Wise
        [Authorize]
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }


            return View();
        }
        public ActionResult loaddata()
        {
            using (MMSEntities dc = new MMSEntities())
            {
                string prjctsId = Session["ProjectssId"].ToString();

                var dataList = dc.ns_tbl_IssueQuantity.Where(j => j.ProjectID == prjctsId && j.IndentNo != "Opening").OrderBy(a => a.Id).ToList();
                return Json(new
                {
                    data = dataList.Select
                        (x => new
                        {
                            IndentNo = x.IndentNo,
                            ItemGroupName = x.ItemGroupName,
                            ItemName = x.ItemName,
                            Quantity = x.Quantity,
                            IssueQuantity = x.IssueQuantity,
                            CreatedDate = x.CreatedDate != null ? x.CreatedDate.Value.ToString("MM/dd/yyyy") : "",
                        }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}