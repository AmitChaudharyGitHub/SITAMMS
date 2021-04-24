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
    public class Report_Quantity_IssueController : MySiteController
    {
        // GET: Report_Quantity_Issue
        [Authorize]
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

           
            return View();
        }


        


        //Write action for return database data 
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

        // Date Wise Search Data here
        
        public ActionResult DatewiseSearch(string Date1 = null, string Date2 = null)
        {
            if (Date1 != null && Date2 != null)
            {
                string prjctsId = Session["ProjectssId"].ToString();
                DateTime d1 = Convert.ToDateTime(Date1);
                DateTime d2 = Convert.ToDateTime(Date2);

                var a = (from pd in objmms.ns_tbl_IssueQuantity.Where(i => i.CreatedDate >= d1 && i.CreatedDate < d2 && i.ProjectID == prjctsId && i.IndentNo != "Opening").OrderByDescending(c => c.CreatedDate)

                         select new ViewTo_Issues_Quantity_Items
                         {
                             Id = pd.Id,
                             IndentNo=pd.IndentNo,
                             ItemGroupName = pd.ItemGroupName,
                             ItemName = pd.ItemName,
                             UnitID = pd.UnitID,
                             Make = pd.Make,
                             PartNo = pd.PartNo,
                             Quantity = pd.Quantity,
                             //BalanceQuantity=pd.BalanceQuantity,                            
                             IssueQuantity = pd.IssueQuantity,
                             CreatedDate = pd.CreatedDate
                         }).ToList();


                var totalRows = a.Count();

                var data = new VM_ViewTo_Issues_Quantity_Items()
                {
                    TotalRows = totalRows,
                    PageSize = 250,
                    Issuesitems = a.ToList()
                };

                if (data != null && data.TotalRows != 0)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_DateWiseSearch_Partial", data);
                    }

                    else
                    {
                        return PartialView("_DateWiseSearch_Partial", data);
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
            }
            else
            {
                return View("_EmptyView");
            }
        }
    }
}