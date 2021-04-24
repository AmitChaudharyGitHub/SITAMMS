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
using DataAccessLayer;

namespace MMS.Controllers
{
    public class HomeController : MySiteController
    {
        private MMSEntities objmms = new MMSEntities();
        private MSP_Model objmsp = new MSP_Model(); 
        private Procedure objp = new Procedure();
        string EmpId = string.Empty;
        FactoryManager m = new FactoryManager();

        public HomeController()
        {
            if (System.Web.HttpContext.Current.Session["EmpId"] != null)
            {
                EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            }
           
            
        }


        [Authorize]
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            var Prject = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).First().ProjectID;
            //var t = a.Select(x => new SelectListItem
            //{
            //    //Value = x.ProjectID.ToString(),
            //    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            //});
         //   ViewBag.prjtidname = objmms.tblProjectMasters.Where(b => b.PRJID == Prject).First().ProjectName;

            //for total count projects
            var totalProjects = objmms.tblProjectMasters.Count();
            ViewBag.Totalprjc = totalProjects;

            //for top project details code here
            //var projectDetails = objmms.tblProjectParticularsDetailAs.OrderByDescending(c=> c.CreatedDate).Select(b => new { b.NameOfProject, b.OriginalContractValue }).Take(5);
            //ViewBag.Pjectsdata = projectDetails;


        //    ViewBag.prjtid = new SelectList(objmms.tblProjectMasters, "PRJID", "ProjectName").OrderBy(k => k.Text);

            return View();
        }
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult GetAllPendingPurIndentDetails()
        {
            string prjId = Session["ProId"].ToString();

            //var detaildmr = objmms.PurchaseRequisitionMasters.Where(b => b.ProjectNo == prjId && b.Comp == null).OrderByDescending(n => n.CreatedDate).Select(x => new
            //{               
            //    SiteName = x.ProjectName,
            //    IndentNo = x.PurRequisitionNo,
            //    CreatedDate = x.Date.ToString()
            //}).ToList().Take(5);
            var detaildmr = objmms.PurchaseRequisitionMasters.Where(b => b.ProjectNo == prjId && b.Comp == null).OrderByDescending(n => n.CreatedDate).Select(x => new PurchaseIndentMasterViewModel
            {

                ProjectName = x.ProjectName,
                PurRequisitionNo = x.PurRequisitionNo,
                UId = x.UId,
                Date = x.Date
            }).ToList().Take(5);
            var data = new GetPendingPIHome()
            {
                MHome = detaildmr.ToList()
           };

            return PartialView("_PendingPurchasePIDetail", data);
           // return Json(detaildmr, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllPendingOrderDetails()
        {
            string prjId = Session["ProId"].ToString();

            var detaildmr = objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(b => b.ProjectId == prjId && b.Status == "Pending").OrderByDescending(n => n.CreatedDate).Select(x => new
            {
                PName = x.Project_Name,
                OrderNo = x.Purchase_Order_Indent_No,
                OrderDate = x.CreatedDate.ToString()
            }).ToList().Take(5);
            return Json(detaildmr, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllPendingIndentDetails()
        {
            string prjId = Session["ProId"].ToString();
            
            var a = objmms.tblIndentRequisitions.Where(b => b.ProjectID == prjId && b.Apporoved_Status == "No").OrderByDescending(n => n.CreatedDate).ToList().Take(5);

            var detaildmr = a.Distinct<tblIndentRequisition>().Select(x => new
            {
                PrName = x.ProjectName,
                InNo = x.IndentNo,
                IDate = Convert.ToDateTime(x.CreatedDate).ToString("dd-MMM-yyyy")
            });
            return Json(detaildmr, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllStockDetails()
        {
            string prjId = Session["ProId"].ToString();

            var detaildmr = (from e in objmms.tblItemCurrentStocks
                             join f in objmms.tblProjectMasters on e.ProjectNo equals f.PRJID
                             join g in objmms.tblItemMasters on e.ItemNo equals g.ItemID
                             join h in objmms.tblUnitMasters on g.UnitID equals h.UnitID
                             where e.ProjectNo == prjId
                                   orderby g.ItemName
                                   select new StockDetail
                                   {
                                       ProjectName = f.ProjectName,
                                       ItemName = g.ItemName,
                                       Unit = h.UnitName,
                                       Quantity = Math.Round((decimal)e.Qty, 2)
                                   }).OrderBy(k => k.ProjectName).Take(5).ToList();
            return Json(detaildmr, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyDashBoard()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string EmpId = Session["EmpID"].ToString();
            ViewBag.EmpId = EmpId;
            ViewBag.ItemGroup =ViewBag.ItemGroup2 = ViewBag.ItemGroup3 = ViewBag.ItemGroup4 = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");
            ViewBag.Status = new SelectList(objmms.tblMovingDescriptions, "MovingId", "MovingType");
            //return View("MyDashBoard");
            return View("MyDashBoard1");
        }

        public JsonResult GetDivisions()
        {
            var userId = Session["EmpID"].ToString();
            var data=Procedure.GetData<VMDropDown>("GetUserDivsions", userId);
            return Json(data.ToJSON(),JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetDivision(string value,string text)
        {
            Session["Division"] = value;
            Session["DivisionName"] = text;
            var colour = objmms.tblDivisionMasters.SingleOrDefault(x => x.Name == text);
            Session["Colour"] = colour.ColourCode;
            return Json("success", JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetPI(string Status,string Stage,int PIType=2)
        {
            try
            {
               if(Status=="HO" && Stage == "Pending" && PIType==1)
                {
                    var data = new Procedure().GetPIForHO(EmpId, Status, Stage, PIType);
                    return Json(new { Status = 1, Data = data }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = new Procedure().GetPI(EmpId, Status, Stage, PIType);
                    return Json(new { Status = 1, Data = data }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = 2,Error=ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

     

        public JsonResult GetData(string Status, string Stage, string Type = "PO")
        {
            try
            {
                var data = new Procedure().GetPO(EmpId, Status, Stage,Type);
                var Role = (from EM in objmms.tblEmployeeMasters
                            join RM in objmms.tblRolemasters on EM.RoleId equals RM.RoleId
                            select new { EM.EmpID, EM.RoleId }).FirstOrDefault().RoleId;

                bool isPMC = false;
                if (Stage == "PMC")
                {
                    if (objmms.UserRoles.Any(x => x.EmpId == EmpId && x.RoleId == 1))
                    {
                        isPMC = true;
                    }
                }
                return Json(new { Status = 1, Role, IsPMC = isPMC, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 2, Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetSumOfAllIssueAndRecev_AllAllocatedProject(string ProjectId)
        {
            var res = objp.GetSumOFIssueAndRecvforAllProject(ProjectId);
            if (res != null)
            {
                return Json(res.ToJSON(), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return null;
            }

        }


        [HttpPost]
        public ActionResult GetMaterialRecvAndIssue()
        {
            int recordsTotal = 0;
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var lst = objp.GetMaterialRecvAndIssueDash();
            var Itemgrp = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            if (!string.IsNullOrEmpty(Itemgrp))
            {
                lst = lst.Where(a => a.ItemGroupId == Itemgrp).ToList();
            }
            recordsTotal = lst.ToList().Count();
           return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = lst }, JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult GetRecvAndIssurGrp(string ProjectId, string ItemGrpId, string ItemId)
        {
            var lst2 = objp.GetReceiveAndIssueGraph(ProjectId, ItemGrpId, ItemId);
            return Json(lst2.ToJSON(), JsonRequestBehavior.AllowGet);
        }


        #region OldDashboradCode
        public ActionResult GetAllMovingHome(string PID, string GID, string STA)
        {
            List<NewMovableStatus> result = new List<NewMovableStatus>();
            if (STA == "Select Ageing Status")
            {
                STA = "";
            }
            if (PID != null && GID != null && GID != "")
            {
                result = objmsp.GetNew_AllItemStatusReport(PID, GID, STA);

            }
            else
            {
                result = objmsp.GetNew_AllItemStatusReport(PID, GID, STA);

            }

            try
            {
                var data = (from a in result
                            select new NewMovableStatus
                            {
                                ID = a.ID,
                                ItemName = a.ItemName,
                                UniteName = a.UniteName,
                                CurrentStockQty = a.CurrentStockQty,
                                CurrentStockAmt = Convert.ToDecimal(String.Format("{0:0.00}", a.CurrentStockAmt)),
                                MOVINGSTATUS = a.MOVINGSTATUS,
                                LstIssueDate = a.LstIssueDate != null ? a.LstIssueDate : "N/A",
                                LstReceiveDate = a.LstReceiveDate != null ? a.LstReceiveDate : "N/A"
                            }).ToList();

                return PartialView("_GridAgingHome", data);
            }
            catch (Exception ex) { }

            return PartialView("_GridAgingHome", null);

        }

        public ActionResult GetPendingPIGrid(string ProjectId)
        {
            var Result = objmsp.GetAllPendingPI(ProjectId, EmpId, "",0);
            if (Result != null)
            {
                return PartialView("_PendingPIGridHome", Result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

        public ActionResult GetPendingPIGrid1(string ProjectId)
        {
            var Result = objmsp.GetPurchaseIndentPendingPIC(EmpId);
            if (Result != null)
            {
                return PartialView("_PendingPIGridHome", Result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

        public ActionResult GetPendingPOHome(string ProjectId)
        {
            var res = objp.GetPODashBoard(ProjectId);
            if (res != null)
            {
                return PartialView("_POGridHome", res);
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }

        public JsonResult GetAgeingPercentgHome(string ProjectId, string ItemgrpId, string Status)
        {
            var list = objp.GetMovingStatusPercentage(ProjectId, ItemgrpId, Status);
            return Json(list.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMaxAndMinRateHome()
        {
            int recordsTotal = 0;
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Itemgrp = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var projid = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();

            if (!string.IsNullOrEmpty(Itemgrp) && !string.IsNullOrEmpty(projid))
            {
                var lst = objp.GetMaxMINRateHome(projid, Itemgrp).Where(b => b.ProjectId == projid && b.ItemGroupID == Itemgrp);
                recordsTotal = lst.ToList().Count();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = lst }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                recordsTotal = 0;
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = "" }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion


        #region all methods for bind dashboard grid code on 3_3_2020

        public JsonResult GetPOPMCPending(string Status, string Stage, string Type = "PO")
        {
            try
            {
                var data = new Procedure().GetPO(EmpId, Status, Stage, Type, "Usp_popmcpending");
                var Role = (from EM in objmms.tblEmployeeMasters
                            join RM in objmms.tblRolemasters on EM.RoleId equals RM.RoleId
                            select new { EM.EmpID, EM.RoleId }).FirstOrDefault().RoleId;

                bool isPMC = false;
                if (Stage == "PMC")
                {
                    if (objmms.UserRoles.Any(x => x.EmpId == EmpId && x.RoleId == 1))
                    {
                        isPMC = true;
                    }
                }
                return Json(new { Status = 1, Role, IsPMC = isPMC, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 2, Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetPOPICPending(string Status, string Stage, string Type = "PO")
        {
            try
            {
                var data = new Procedure().GetPO(EmpId, Status, Stage, Type, "Usp_popicpending");
                var Role = (from EM in objmms.tblEmployeeMasters
                            join RM in objmms.tblRolemasters on EM.RoleId equals RM.RoleId
                            select new { EM.EmpID, EM.RoleId }).FirstOrDefault().RoleId;

                bool isPMC = false;
                bool isPIC = false;
                if (Stage == "PMC")
                {
                    if (objmms.UserRoles.Any(x => x.EmpId == EmpId && x.RoleId == 1))
                    {
                        isPMC = true;
                    }
                }

                if (objmms.tblApproval_AccountType.Any(x => x.EmployeeId == EmpId && x.TypeName == "PIC"))
                {
                    isPIC = true;
                }
                return Json(new { Status = 1, Role, IsPMC = isPMC, IsPIC = isPIC, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 2, Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetPOClusterPending(string Status, string Stage, string Type = "PO")
        {
            try
            {
                var data = new Procedure().GetPO(EmpId, Status, Stage, Type, "Usp_poclusterpending");
                var Role = (from EM in objmms.tblEmployeeMasters
                            join RM in objmms.tblRolemasters on EM.RoleId equals RM.RoleId
                            select new { EM.EmpID, EM.RoleId }).FirstOrDefault().RoleId;

                bool isPMC = false;
                if (Stage == "PMC")
                {
                    if (objmms.UserRoles.Any(x => x.EmpId == EmpId && x.RoleId == 1))
                    {
                        isPMC = true;
                    }
                }
                return Json(new { Status = 1, Role, IsPMC = isPMC, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 2, Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetPurchaserDisaprove(string Status, string Stage, string Type = "PO")
        {
            try
            {
                var data = new Procedure().GetPO(EmpId, Status, Stage, Type, "Usp_purchaserdisapproved");
                var Role = (from EM in objmms.tblEmployeeMasters
                            join RM in objmms.tblRolemasters on EM.RoleId equals RM.RoleId
                            select new { EM.EmpID, EM.RoleId }).FirstOrDefault().RoleId;

                bool isPMC = false;
                if (Stage == "PMC")
                {
                    if (objmms.UserRoles.Any(x => x.EmpId == EmpId && x.RoleId == 1))
                    {
                        isPMC = true;
                    }
                }
                return Json(new { Status = 1, Role, IsPMC = isPMC, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 2, Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetPONoRelease(string Status, string Stage, string Type = "PO")
        {
            try
            {
                var data = new Procedure().GetPO(EmpId, Status, Stage, Type, "Usp_ponorelease");
                var Role = (from EM in objmms.tblEmployeeMasters
                            join RM in objmms.tblRolemasters on EM.RoleId equals RM.RoleId
                            select new { EM.EmpID, EM.RoleId }).FirstOrDefault().RoleId;

                bool isPMC = false;
                if (Stage == "PMC")
                {
                    if (objmms.UserRoles.Any(x => x.EmpId == EmpId && x.RoleId == 1))
                    {
                        isPMC = true;
                    }
                }
                return Json(new { Status = 1, Role, IsPMC = isPMC, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 2, Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTransferPICPending(string Status, string Stage, string Type = "PO")
        {
            try
            {
                var data = new Procedure().GetPO(EmpId, Status, Stage, Type, "Usp_transferpicpending");
                var Role = (from EM in objmms.tblEmployeeMasters
                            join RM in objmms.tblRolemasters on EM.RoleId equals RM.RoleId
                            select new { EM.EmpID, EM.RoleId }).FirstOrDefault().RoleId;

                bool isPMC = false;
                if (Stage == "PMC")
                {
                    if (objmms.UserRoles.Any(x => x.EmpId == EmpId && x.RoleId == 1))
                    {
                        isPMC = true;
                    }
                }
                return Json(new { Status = 1, Role, IsPMC = isPMC, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 2, Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

    }
}