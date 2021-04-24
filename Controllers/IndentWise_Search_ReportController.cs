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
    public class IndentWise_Search_ReportController : MySiteController
    {
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: QuantityIssues
        // GET: IndentWise_Search_Report
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            var a = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectID.ToString(),
                // Text = x.ProjectID.ToString()
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            });
            ViewBag.prjtid = t;
            return View();
        }


        // Get Indent No
        public JsonResult GetIndentno(string id)
        {
            List<SelectListItem> groups = new List<SelectListItem>();
            var groupList = this.Getindentname(Convert.ToString(id));

            var a = groupList.Where(b => b.Status == "Approved").OrderBy(c => c.Id).ToList();

            var indentData = a.Distinct<ns_tbl_IssueQuantity>().Select(m => new SelectListItem
            {
                Text = m.IndentNo,
                Value = m.Id.ToString(),
            });



            return Json(indentData, JsonRequestBehavior.AllowGet);
        }

        // Get State from DB by Project ID
        public IList<ns_tbl_IssueQuantity> Getindentname(string ProjectId)
        {
            return objmms.ns_tbl_IssueQuantity.Where(m => m.ProjectID == ProjectId).ToList();
        }


        [HttpPost]
        public ActionResult IndentWiseSearch()
        {
            
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //find search columns info
            var projectName = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var indentname = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            //i want in
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;


            using (MMSEntities dc = new MMSEntities())
            {
                string prjctsId = Session["ProjectssId"].ToString();

                var v = (from a in dc.ns_tbl_IssueQuantity.Where(l=> l.ProjectID == prjctsId &&  l.IndentNo != "Opening") select a);


                //SEARCHING...
                if (!string.IsNullOrEmpty(projectName))
                {
                    v = v.Where(a => a.ProjectName.Contains(projectName));
                }
                if (!string.IsNullOrEmpty(indentname))
                {
                    v = v.Where(a => a.IndentNo == indentname);
                }
                //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                recordsTotal = v.Count();
                var data = v.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal,
                    
                    data = data.Select
                    (g=> new
                    {
                            ProjectName = g.ProjectName,
                            IndentNo = g.IndentNo,
                            ItemGroupName = g.ItemGroupName,
                            ItemName = g.ItemName,
                            Quantity = g.Quantity,
                            IssueQuantity = g.IssueQuantity,
                            CreatedDate = g.CreatedDate != null ? g.CreatedDate.Value.ToString("MM/dd/yyyy") : "",
                    }).ToList()
                },
                    JsonRequestBehavior.AllowGet);
            }
        }
    }
}