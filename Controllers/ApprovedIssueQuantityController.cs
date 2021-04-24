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
    public class ApprovedIssueQuantityController : MySiteController
    {
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        
        // GET: ApprovedIssueQuantity
        [Authorize]
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();

            var a = objmms.tblIndentRequisitions.Where(b => b.Apporoved_Status == "No").OrderBy(c => c.IndentNo).ToList();
            ViewBag.indentno = a.Distinct<tblIndentRequisition>().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.IndentNo
            });


            //Item Project Name
            //ViewBag.prjtid = new SelectList(unitOfWork.tblProjectRepository.Get(), "PRJID", "ProjectName");

            
            var aaa = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            var t = aaa.Select(x => new SelectListItem
            {
                Value = x.ProjectID.ToString(),
                // Text = x.ProjectID.ToString()
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            });
            ViewBag.prjtid = t;

            return View();
        }


        public JsonResult Getprojectempblokflorcodes(int id)
        {

            tblIndentRequisition E = objmms.tblIndentRequisitions.Where(s => s.Id == id).FirstOrDefault();
            var EO = new { Pjtnames = E.ProjectName, empnames = E.EmployeeName };
            //var Io = new { itemsids = E.ItemID };
            //TempData["StoreIds"] = Io;
            string jsonString = EO.ToJSON();

            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }


        // get indent no.

        public JsonResult GetIndentno(string id)
        {
            List<SelectListItem> groups = new List<SelectListItem>();
            var groupList = this.Getindentname(Convert.ToString(id));
            string empId = Session["EmpID"].ToString();

            var a = groupList.Where(b => b.Apporoved_Status == "No" && b.EmployeeID == empId).OrderBy(c => c.Id).ToList();

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


        // 





        public ActionResult Details(string id)
        {
            var indentNo = id;

            var a1 = (from pd in objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo && m.Apporoved_Status=="No" && m.Approved_Quantity == 0).OrderBy(c => c.Id)

                      select new Approved_IndentRequisionses
                      {
                          Id = pd.Id,
                         ItemGroupID = pd.ItemGroupID,
                          ItemGroupName = pd.ItemGroupName,
                          ItemID = pd.ItemID,
                          ItemName = pd.ItemName,
                          UnitID = pd.UnitID,
                          Make = pd.Make,
                          PartNo = pd.PartNo,
                          Quantity = pd.Quantity,
                          Approved_Quantity=pd.Approved_Quantity                          

                      }).ToList();
            //var a1 = objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo).OrderBy(c => c.Id).ToList();            
            var totalRows = a1.Count();
            var data1 = new Approved_VMIndentRequisionses()
            {
                TotalRows = totalRows,
                PageSize = 250,
                AppIndents = a1.ToList()
            };


            if (data1 != null && data1.TotalRows != 0)
            {

                if (Request.IsAjaxRequest())
                {

                    return PartialView("_PartialGridView_For_Approved_Indents", data1);
                }

                else
                {
                    return PartialView("_PartialGridView_For_Approved_Indents", data1);
                }

            }

            else
            {
                return View("_EmptyView");
            }
            // }


        }


        public ActionResult UpdateGridData(string gridData)
        {
            var log = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(gridData);
            if (log != null)
            {
                foreach (var ind in log)
                {
                    if (Convert.ToString(ind.Value[10].ToString()) ==null)
                    {
                        return Json("3", JsonRequestBehavior.AllowGet);
                    }

                    if (Convert.ToDecimal(ind.Value[9].ToString()) != 0)
                    {
                                        var UID = int.Parse(ind.Value[0]);
                                        var ApprID = Convert.ToDecimal(ind.Value[9]);
                        if (ApprID > 0)
                        {
                        
                            tblIndentRequisition e = objmms.tblIndentRequisitions.Where(a => a.Id == UID).First();
                            //if (e.CreatedBy != Session["EmpID"].ToString())
                            //{
                            e.Approved_Quantity = Convert.ToDecimal(ind.Value[9].ToString());
                            e.BalanceApprovedQty_After_Issue_Qty = Convert.ToDecimal(ind.Value[9].ToString());
                            decimal BalQuantity = Convert.ToDecimal(ind.Value[8].ToString()) - Convert.ToDecimal(ind.Value[9].ToString());
                            e.BalanceQuantity = BalQuantity;
                            e.Apporoved_By = Session["EmpID"].ToString();
                            e.Apporoved_Status = "Yes";
                            e.Status = "Approved";
                            //var itmids = e.ItemID;

                            //tblIndentRequisition e = new tblIndentRequisition() { Id = Convert.ToInt32(ind.Value[0]), ItemGroupName = ind.Value[1].ToString(), ItemName = ind.Value[2].ToString(), IssueQuantity = Convert.ToDecimal(ind.Value[7].ToString()), Status = "Approved" };
                            //_dbcontext.tblEmployees.Add(e);

                            objmms.Entry(e).State = EntityState.Modified;

                            // ADD Or INSERT ISSUE QUANTITY IN TABLE Here
                            string Itmid = ind.Value[3];
                            ns_tbl_ApprovedItemQuantity objisu = new ns_tbl_ApprovedItemQuantity();
                            objisu.ProjectID = ind.Value[10];
                            objisu.ProjectName = ind.Value[11];
                            objisu.IndentNo = ind.Value[12];
                            objisu.EmployeeName = ind.Value[13];

                            objisu.ItemGroupID = ind.Value[1];
                            objisu.ItemGroupName = ind.Value[2];
                            objisu.ItemID = ind.Value[3];
                            objisu.ItemName = objmms.tblItemMasters.Where(x=>x.ItemID == Itmid).FirstOrDefault().ItemName ;
                            objisu.UnitID = ind.Value[5];
                            objisu.Make = ind.Value[6];
                            objisu.PartNo = ind.Value[7];
                            objisu.Quantity = Convert.ToDecimal(ind.Value[8]);
                            objisu.ApprovedQuantity = Convert.ToDecimal(ind.Value[9]);
                            //objisu.IssueQuantity = Convert.ToDecimal(ind.Value[8]);
                            //objisu.BalanceQuantity = Convert.ToDecimal(ind.Value[6].ToString()) - Convert.ToDecimal(ind.Value[8].ToString());


                            objisu.SessionId = "1";
                            objisu.SiteId = "N/A";
                            objisu.ApproveddBy = Convert.ToString(Session["EmpID"]);
                            objisu.Status = "Approved";
                            objisu.CreatedDate = DateTime.Now;

                            objmms.ns_tbl_ApprovedItemQuantity.Add(objisu);
                            objmms.SaveChanges();

                        }
                    else
                    {

                    }

                 }
                   

                }
               
            }
            return Json("Update Successfully");
        }

        public class Datas
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
            public Nullable<decimal> ApprovedQuantity { get; set; }
            public string SessionId { get; set; }
            public string SiteId { get; set; }
            public string ApproveddBy { get; set; }
            public string Status { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
        }

        // Here Code is for Another View
        public ActionResult ViewApprovedStock_Indent()
        {
            string empId = Session["EmpID"].ToString();          

            var aaa = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            var t = aaa.Select(x => new SelectListItem
            {
                Value = x.ProjectID.ToString(),
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            });

            //var plist = this.objmms.tblIndentRequisitions.Select(x => new SelectListItem
            //{
            //    Value = x.ProjectID,
            //    Text = x.ProjectName
            //}).Distinct();
            ViewBag.prjtid = t;

            //ViewBag.prjtid2 = new SelectList(objmms.tblProjectMasters, "PRJID", "ProjectName");
            return View();
        }


        public ActionResult _PartialGridApprovedStocks_View(string Status = null, string PId = null)
        {

            if(Status =="Yes")
            {
                 var a = (from pd in objmms.tblIndentRequisitions.Where(i => i.Apporoved_Status == Status && i.ProjectID == PId).OrderBy(c => c.IndentNo)
                     select new Approved_IndentRequisionses
                      {
                          Id = pd.Id,
                          IndentNo = pd.IndentNo,
                          ItemGroupName = pd.ItemGroupName,
                          ItemName = pd.ItemName,
                          UnitID = pd.UnitID,
                          Make = pd.Make,
                          PartNo = pd.PartNo,
                          Quantity = pd.Quantity,
                          Approved_Quantity=pd.Approved_Quantity                          
                      }).ToList();


            var totalRows = a.Count();

            var data = new Approved_VMIndentRequisionses()
            {
                TotalRows = totalRows,
                PageSize = 250,
                AppIndents = a.ToList()
            };


            if (data != null && data.TotalRows != 0)
            {



                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PartialGridApprovedStocks_View", data);
                }

                else
                {
                    return PartialView("_PartialGridApprovedStocks_View", data);
                }

            }

            else
            {
                return View("_EmptyView");
            }

        }

            else
            {

                var aa = (from pd in objmms.tblIndentRequisitions.Where(i => i.Apporoved_Status == Status && i.ProjectID == PId).OrderBy(c => c.IndentNo)
                          select new Unapproved_IndentRequisionses
                          {
                              Id = pd.Id,
                              ItemGroupName = pd.ItemGroupName,
                              ItemName = pd.ItemName,
                              UnitID = pd.UnitID,
                              Make = pd.Make,
                              PartNo = pd.PartNo,
                              Quantity = pd.Quantity,
                              Approved_Quantity = pd.Approved_Quantity
                          }).ToList();


                var totalRows = aa.Count();

                var data = new Unapproved_VMIndentRequisionses()
                {
                    TotalRows = totalRows,
                    PageSize = 250,
                    AppIndentss = aa.ToList()
                };


                if (data != null && data.TotalRows != 0)
                {



                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialGrid_Un_ApprovedStocks_View", data);
                    }

                    else
                    {
                        return PartialView("_PartialGrid_Un_ApprovedStocks_View", data);
                    }

                }

                else
                {
                    return View("_EmptyView");
                }

              
            }
            



        }

        //public ActionResult _PartialGridUn_ApprovedStocks_View(string Status = null, string PId = null)
        //{

        //    if (Status == "No")
        //    {
        //        var aa = (from pd in objmms.tblIndentRequisitions.Where(i => i.Apporoved_Status == Status && i.ProjectID == PId).OrderBy(c => c.IndentNo)
        //                  select new Unapproved_IndentRequisionses
        //                  {
        //                      Id = pd.Id,
        //                      ItemGroupName = pd.ItemGroupName,
        //                      ItemName = pd.ItemName,
        //                      UnitID = pd.UnitID,
        //                      Make = pd.Make,
        //                      PartNo = pd.PartNo,
        //                      Quantity = pd.Quantity,
        //                      Approved_Quantity = pd.Approved_Quantity
        //                  }).ToList();


        //        var totalRows = aa.Count();

        //        var data = new Unapproved_VMIndentRequisionses()
        //        {
        //            TotalRows = totalRows,
        //            PageSize = 25,
        //            AppIndentss = aa.ToList()
        //        };


        //        if (data != null && data.TotalRows != 0)
        //        {



        //            if (Request.IsAjaxRequest())
        //            {
        //                return PartialView("_PartialGrid_Un_ApprovedStocks_View", data);
        //            }

        //            else
        //            {
        //                return PartialView("_PartialGrid_Un_ApprovedStocks_View", data);
        //            }

        //        }

        //        else
        //        {
        //            return View("_EmptyView");
        //        }

        //    }

        //    else
        //    {
        //        return View("_EmptyView");
        //    }
           



        //}

        

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }





    }
}