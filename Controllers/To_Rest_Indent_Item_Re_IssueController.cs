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

namespace MMS.Controllers
{
    public class To_Rest_Indent_Item_Re_IssueController : MySiteController
    {
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: QuantityIssues

        [Authorize]
        // GET: To_Rest_Indent_Item_Re_Issue
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId=Session["EmpID"].ToString();
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

        public JsonResult GetIndentno(string id)
        {
            List<SelectListItem> groups = new List<SelectListItem>();
            var groupList = this.Getindentname(Convert.ToString(id));

            var a = groupList.Where(b => b.Apporoved_Status == "Yes" && b.IndentSent == "Yes" && b.BalanceQuantity > 0).OrderBy(c => c.Id).ToList();

            var indentData = a.Distinct<tblIndentRequisition>().Select(m => new SelectListItem
            {
                Text = m.IndentNo,
                Value = m.Id.ToString(),
            });



            return Json(indentData, JsonRequestBehavior.AllowGet);
        }

        // Get State from DB by Project ID
        public IList<tblIndentRequisition> Getindentname(string ProjectId)
        {
            return objmms.tblIndentRequisitions.Where(m => m.ProjectID == ProjectId).ToList();
        }


        // Get Employee Name By Id
        public JsonResult GetemployeeId_IndentNo(int id)
        {

            tblIndentRequisition E = objmms.tblIndentRequisitions.Where(s => s.Id == id).FirstOrDefault();
            var EO = new { Pjtnames = E.ProjectName, empnames = E.EmployeeName };
            //var Io = new { itemsids = E.ItemID };
            //TempData["StoreIds"] = Io;
            string jsonString = EO.ToJSON();

            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        
        //public IList<tblIndentRequisition> Getprojectcodes(string indntid)
        //{
        //    return objmms.tblIndentRequisitions.Where(m => m.IndentNo == indntid).ToList();
        //}


        // Get Deatils Of Items here

        public ActionResult Details(string id)
        {
            var indentNo = id;

            var inedntexistances = objmms.tblIndentRequisitions.Where(s => s.IndentNo == indentNo && s.BalanceQuantity > 0 && s.IndentSent == "Yes").Select(l => l.IndentSent).ToList();
            //string checkingIndent = inedntexistances.First().;
            if (inedntexistances.Count() != 0)
            {
                TempData["dataexist"] = "Yes";



                var a1 = objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo && m.BalanceQuantity > 0).OrderBy(c => c.Id).Select(xx => new Iss
                {
                    Id = xx.Id,
                    ItemGroupID = xx.ItemGroupID,
                    ItemGroupName = xx.ItemGroupName,
                    ItemName = xx.ItemName,
                    UnitID = xx.UnitID,
                    Make = xx.Make,
                    PartNo = xx.PartNo,
                    Quantity = xx.Quantity,
                    BalanceQuantity = xx.BalanceQuantity,
                    Approved_Quantity = xx.Approved_Quantity,
                    ItemID = xx.ItemID,
                    CreatedDate = xx.CreatedDate,
                    BalanceApprovedQty_After_Issue_Qty = xx.BalanceApprovedQty_After_Issue_Qty,
                    Available_Quantity = objmms.tblItemCurrentStocks.Where(x => x.ProjectNo == xx.ProjectID && x.ItemNo == xx.ItemID).FirstOrDefault().Qty ?? 0
                }).ToList();
                //var a1 = objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo).OrderBy(c => c.Id).ToList();            
                var totalRows = a1.Count();

                var data1 = new VMIndentRequisionses_Re_Issue()
                {
                    TotalRows = totalRows,
                    PageSize = 25,
                    Indents = a1
                };


                if (data1 != null && data1.TotalRows != 0)
                {

                    if (Request.IsAjaxRequest())
                    {

                        return PartialView("_PartialIndentView_Re_Issue", data1);
                    }

                    else
                    {
                        return PartialView("_PartialIndentView_Re_Issue", data1);
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
            }

            else
            {
                var a1 = objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo && m.BalanceQuantity > 0).OrderBy(c => c.Id).Select(xx => new Iss
                {
                    Id = xx.Id,
                    ItemGroupID = xx.ItemGroupID,
                    ItemGroupName = xx.ItemGroupName,
                    ItemName = xx.ItemName,
                    UnitID = xx.UnitID,
                    Make = xx.Make,
                    PartNo = xx.PartNo,
                    Quantity = xx.Quantity,
                    BalanceQuantity = xx.BalanceQuantity,
                    Approved_Quantity = xx.Approved_Quantity,
                    ItemID = xx.ItemID,
                    CreatedDate = xx.CreatedDate,
                    BalanceApprovedQty_After_Issue_Qty = xx.BalanceApprovedQty_After_Issue_Qty,
                    Available_Quantity = objmms.tblItemCurrentStocks.Where(x => x.ProjectNo == xx.ProjectID && x.ItemNo == xx.ItemID).FirstOrDefault().Qty ?? 0
                }).ToList();



                //var a1 = objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo).OrderBy(c => c.Id).ToList();            
                var totalRows = a1.Count();

                var data1 = new VMIndentRequisionses_Re_Issue()
                {
                    TotalRows = totalRows,
                    PageSize = 25,
                    Indents = a1
                };


                if (data1 != null && data1.TotalRows != 0)
                {

                    if (Request.IsAjaxRequest())
                    {

                        return PartialView("_PartialIndentView_Re_Issue", data1);
                    }

                    else
                    {
                        return PartialView("_PartialIndentView_Re_Issue", data1);
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
            }

        }


        public class Iss
        {
            public int Id { get; set; }
            public string ProjectID { get; set; }
            public string ProjectName { get; set; }
            public string IndentNo { get; set; }
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string ItemGroupID { get; set; }
            public string ItemGroupName { get; set; }
            public string ItemID { get; set; }
            public string ItemName { get; set; }
            public string UnitID { get; set; }
            public string Make { get; set; }
            public string PartNo { get; set; }
            public Nullable<decimal> Quantity { get; set; }
            public Nullable<decimal> BalanceQuantity { get; set; }
            public Nullable<decimal> IssueQuantity { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public Nullable<decimal> Approved_Quantity { get; set; }
            public string Apporoved_By { get; set; }
            public string Apporoved_Status { get; set; }
            public Nullable<decimal> BalanceApprovedQty_After_Issue_Qty { get; set; }
            // StockName = rt.StockName,&& pd.ProjectID ==od.ProjectNo
            public decimal? Available_Quantity { get; set; }
        }



        public ActionResult UpdateGridData(string gridData)
        {
            var log = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(gridData);
            if (log != null)
            {
                foreach (var ind in log)
                {
                    if (Convert.ToDecimal(ind.Value[11].ToString()) != 0)
                    {
                        if (Convert.ToDecimal(ind.Value[11].ToString()) <= Convert.ToDecimal(ind.Value[9].ToString()))
                        {
                            var UID = int.Parse(ind.Value[0]);

                            // var avilqty = Convert.ToDecimal(ind.Value[8].ToString()) - Convert.ToDecimal(ind.Value[9].ToString());

                            tblIndentRequisition e = objmms.tblIndentRequisitions.Where(a => a.Id == UID).First();
                            e.IssueQuantity = Convert.ToDecimal(ind.Value[11].ToString());
                            e.BalanceQuantity = Convert.ToDecimal(ind.Value[9].ToString()) - Convert.ToDecimal(ind.Value[11].ToString());
                            e.IndentSent = "Yes";
                            e.Status = "Approved";
                            var itmids = e.ItemID;
                            var gh = Convert.ToDecimal(ind.Value[10].ToString()) - Convert.ToDecimal(ind.Value[11].ToString());
                            //tblIndentRequisition e = new tblIndentRequisition() { Id = Convert.ToInt32(ind.Value[0]), ItemGroupName = ind.Value[1].ToString(), ItemName = ind.Value[2].ToString(), IssueQuantity = Convert.ToDecimal(ind.Value[7].ToString()), Status = "Approved" };
                            //_dbcontext.tblEmployees.Add(e);


                            //objmms.Entry(e).State = EntityState.Modified;


                            // ADD Or INSERT ISSUE QUANTITY IN TABLE Here
                            ns_tbl_IssueQuantity objisu = new ns_tbl_IssueQuantity();
                            objisu.ProjectID = ind.Value[12];
                            objisu.ProjectName = ind.Value[13];
                            objisu.IndentNo = ind.Value[14];
                            objisu.EmployeeName = ind.Value[15];
                            objisu.Status = ind.Value[17];

                            objisu.ItemGroupID = ind.Value[1];
                            objisu.ItemGroupName = ind.Value[2];
                            objisu.ItemID = ind.Value[3];
                            objisu.ItemName = ind.Value[4];
                            objisu.UnitID = ind.Value[5];
                            objisu.Make = ind.Value[6];
                            objisu.PartNo = ind.Value[7];
                            objisu.Quantity = Convert.ToDecimal(ind.Value[8]);
                            objisu.AvailableQuantity = Convert.ToDecimal(ind.Value[10]);
                            objisu.IssueQuantity = Convert.ToDecimal(ind.Value[11]);
                            objisu.BalanceQuantity = Convert.ToDecimal(ind.Value[9].ToString()) - Convert.ToDecimal(ind.Value[11].ToString());
                            objisu.AfterIssue_AvlQty_Stock = Convert.ToDecimal(ind.Value[10].ToString()) - Convert.ToDecimal(ind.Value[11].ToString());

                            objisu.SessionId = "1";
                            objisu.SiteId = "N/A";
                            objisu.IssuedBy = Convert.ToString(Session["EmpID"]);
                            objisu.ModifiedDate = Convert.ToDateTime(ind.Value[16]);
                            objisu.CreatedDate = DateTime.Now;
                            objmms.ns_tbl_IssueQuantity.Add(objisu);


                            // stock master table updation here
                            tblItemCurrentStock bg = objmms.tblItemCurrentStocks.Where(l => l.ItemNo == itmids && l.ProjectNo == objisu.ProjectID).First();
                            bg.Qty = Convert.ToDecimal(gh);
                            objmms.Entry(bg).State = EntityState.Modified;
                        }
                        else {
                            TempData["BalanceDataCheck"] = "Yes";
                            return Json("1", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else {
                    }
                }
                objmms.SaveChanges();
            }
            return Json("Update Successfully");
        }



        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }

    }
}