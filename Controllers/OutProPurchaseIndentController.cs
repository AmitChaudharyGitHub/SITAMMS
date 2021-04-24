using BusinessObjects.Entity;
using DataAccessLayer;
using MMS.Helpers;
using MMS.Models;
using MMS.ViewModels;
using MMS_P.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class OutProPurchaseIndentController : Controller
    {
        // GET: OutProPurchaseIndent
        string EmpId = string.Empty;
        string EmpName = string.Empty;
        int SessionId = 1;
        int SiteId = 1;
        FactoryManager m = new FactoryManager();
        DAL.MMSEntities objmms = new DAL.MMSEntities();
        Procedure objmsr = new Procedure();
        public OutProPurchaseIndentController()
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["EmpId"].ToString() != null)
                {

                    EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
                    EmpName = System.Web.HttpContext.Current.Session["Emp_Name"].ToString();
                }
                else {

                }
                
            }
            catch
            {
            }
        }
        public ActionResult Index()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.Date = DateTime.Now.Date;
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.OrderDate = DateTime.Now.Date;

            ViewBag.ItemGroup = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");
            
            return View();
        }

        public JsonResult GetProjectByEmpId(string E = "0")
        {
            string D = null;

            if (E != "0" && E != "EMP0000001" && E != "")
            {
                string PrjString = m.GetEmployeeMasterManager().FindPrj(E);


                var PrjlistD = m.GetProjectMasterManager().FindListByString(PrjString);

                D = PrjlistD.ToJSON();
            }
            else
            {
                var Prjlist = m.GetProjectMasterManager().FindListByString(null);

                D = Prjlist.ToJSON();
            }
            var v = new { List1 = D };

            string jsonString = v.ToJSON();

            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseIRNo(string ProjectID,string OutPIType)
        {
          
            string res = string.Empty; string jsonString = string.Empty;
           // res = objmsr.GetOut_PI(ProjectID, Convert.ToInt32(OutPIType));
      
            List<DAL.tblEmployeeMaster> E = objmms.tblEmployeeMasters.Where(x => x.EmpID == EmpId).ToList();
            var EO = E.Select(a => new { Value = a.EmpID, Text = a.FName + ' ' + a.LName });
            if (res != null)
            {
                var v = new { List = EO, Message = res };
                jsonString = v.ToJSON();
            }
            else {
                var v = new { List = EO, Message ="N/A" };
                jsonString = v.ToJSON();
            }

            return Json(jsonString, JsonRequestBehavior.AllowGet);
    
        }

        public JsonResult GetPurchaseIRNoDateWise(string ProjectID, string OutPIType, string Date)
        {
            string res = string.Empty; string J = string.Empty;
            string FnYr = GetCurrentFinancialYear(Convert.ToDateTime(Date));
            var sesId = objmms.SessionMasters.Where(x => x.Name == FnYr.ToString()).FirstOrDefault().Id;
            res = objmsr.GetOut_PI(ProjectID, Convert.ToInt32(OutPIType), sesId, Convert.ToDateTime(Date));
            if (res != null)
            {
                var Lst = new { P_No = res };
                J = Lst.ToJSON();
            }
            return Json(J, JsonRequestBehavior.AllowGet);
        }

        public static string GetCurrentFinancialYear(DateTime InputDate)
        {
            int CurrentYear = InputDate.Year;
            int PreviousYear = InputDate.Year - 1;
            int NextYear = InputDate.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (InputDate.Month > 3)
                FinYear = CurYear + "-" + NexYear.Substring(NexYear.Length - 2);
            else
                FinYear = PreYear + "-" + CurYear.Substring(CurYear.Length - 2);
            return FinYear.Trim();
        }


        public JsonResult GetPurchaseType()
        {
            var res = objmms.tblPI_PurchaseType.Where(x=>x.TrandId >=3).ToList().Select(x => new { Text = x.PurchaseType, Value = x.TrandId }).OrderBy(p => p.Text).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SavePIDetail(PurchaseOutIR Grid)
        {
            using (var transaction=objmms.Database.BeginTransaction())
            {
                try
                {
                    DAL.PurchaseRequisitionMaster objM = new DAL.PurchaseRequisitionMaster();
                    List<DAL.PurchaseRequisitionChild> objr = new List<DAL.PurchaseRequisitionChild>();

                    Grid.MasterNew.PurRequisitionNo = Get_PurchaseIRNo(Grid.MasterNew.ProjectNo, Grid.MasterNew.PurchasePI_Type.Value.ToString(), Grid.MasterNew.Date.Value);

                    var masterLst = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Grid.MasterNew.PurRequisitionNo && x.ProjectNo == Grid.MasterNew.ProjectNo).ToList();
                    if (masterLst.Count() > 0)
                    {
                        transaction.Rollback();
                        return Json("3", JsonRequestBehavior.AllowGet); // duplicate
                    }
                    else
                    {

                        if (EmpId == null || EmpId == "")
                        {
                            transaction.Rollback();
                            return Json("4", JsonRequestBehavior.AllowGet); // Some Administrator problem. Kindly Re-load Page.
                        }
                        else
                        {

                            #region
                            string Fynm = GetCurrentFinancialYear(Convert.ToDateTime(Grid.MasterNew.Date));

                            var Session_Id = objmms.SessionMasters.Where(x => x.Name == Fynm.ToString()).FirstOrDefault().Id;


                            objM.PurRequisitionNo = Grid.MasterNew.PurRequisitionNo;
                            objM.Id = FindMaxIdofPI(Grid.MasterNew.ProjectNo, Grid.MasterNew.PurchasePI_Type.ToString(), Session_Id.ToString());
                            objM.Date = Grid.MasterNew.Date;
                            objM.ProjectNo = Grid.MasterNew.ProjectNo;
                            objM.ProjectName = Grid.MasterNew.ProjectName;
                            objM.CreatedBy = EmpId;
                            // objM.SessionId = SessionId.ToString();
                            objM.SessionId = Session_Id.ToString();
                            objM.CreatedByName = EmpName;
                            objM.CreatedDate = System.DateTime.Now;
                            objM.CompO = "Yes";
                            objM.Comp = "Yes";
                            objM.SendTo = objM.ForwardToMang = Grid.MasterNew.SendTo;
                            objM.SendToName = Grid.MasterNew.SendToName;
                            objM.PurchasePI_Type = objM.PurchasePIC_Type = Grid.MasterNew.PurchasePI_Type;
                            objM.Remarks = objM.PICRemarks = objM.MangRemarks = Grid.MasterNew.Remarks;
                            objM.DivisionId = Repos.GetUserDivision();
                            #endregion


                            #region

                            decimal a = FindMaxIdofPI(Grid.MasterNew.ProjectNo, Grid.MasterNew.PurchasePI_Type.ToString(), Session_Id.ToString());
                            decimal u_id = FindMaxUIDOfPIMaster();

                            if (Grid.ChildNew != null && u_id != 0)
                            {
                                decimal PILimit = objmms.tblPurchaseRequisitionApprovalLimitValues.Where(x => x.ProjectId == Grid.MasterNew.ProjectNo).OrderByDescending(p => p.TransId).First().LimitValue ?? 0;

                                var ss = from k in Grid.ChildNew
                                         group k by k.UId into g
                                         select new
                                         { SUM = g.Sum(o => o.ApprovedQty * o.CurrentRate) };

                                decimal totAmt = Math.Round((decimal)(ss.Sum(l => l.SUM)), 2);
                                int xx = Decimal.Compare(PILimit, totAmt);
                                bool check = false;
                                if (xx == 1)
                                { check = true; }
                                else { check = false; }

                                objM.CheckedBeyondPIC_Limit = check;
                                objM.AllItemPICStatus = "Approved";
                                objM.AllItemMangStatus = "Approved";
                                objmms.PurchaseRequisitionMasters.Add(objM);
                                objmms.SaveChanges();
                                var Pi_UID = objmms.PurchaseRequisitionMasters.Where(x => x.ProjectNo == Grid.MasterNew.ProjectNo && x.PurRequisitionNo == Grid.MasterNew.PurRequisitionNo).FirstOrDefault().UId;
                                if (Pi_UID != 0)
                                {

                                    foreach (var x in Grid.ChildNew)
                                    {
                                        DAL.PurchaseRequisitionChild objCh = new DAL.PurchaseRequisitionChild();

                                        objCh.ItemGroupName = x.ItemGroupName;
                                        objCh.ItemGroupNo = x.ItemGroupNo;
                                        objCh.ItemName = x.ItemName;
                                        objCh.ItemNo = x.ItemNo;
                                        objCh.UnitNo = x.UnitNo;
                                        objCh.Unit = x.Unit;
                                        objCh.DemandQty = x.DemandQty;
                                        objCh.LastPurchaseRate = x.LastPurchaseRate;
                                        objCh.CurrentRate = x.CurrentRate;
                                        objCh.CurrentValue = x.CurrentValue;
                                        objCh.CreatedDate = System.DateTime.Now;
                                        objCh.MId = Pi_UID;
                                        objCh.ApprovedQty = x.DemandQty;
                                        objCh.ApprovedQtyPIC = x.DemandQty;
                                        objCh.ApprovedQtyMang = x.DemandQty;
                                        objCh.PICCurrentValue = x.CurrentValue;
                                        objCh.MangCurrentValue = x.CurrentValue;
                                        objCh.OrderedQty = 0;
                                        objCh.MangStatus = "Approved";
                                        objCh.PICStatus = "Approved";
                                        objCh.PurRequisitionNo = Grid.MasterNew.PurRequisitionNo;
                                        objCh.ProjectNo = Grid.MasterNew.ProjectNo;
                                        objCh.ProjectName = Grid.MasterNew.ProjectName;
                                        objCh.Date = Grid.MasterNew.Date;
                                        int a11 = objmms.PurchaseRequisitionChilds.Where(i => i.MId == Pi_UID).Select(i => i.SNo).DefaultIfEmpty(0).Max(i => i).Value;
                                        if (a11 == 0)
                                        {
                                            objCh.SNo = 1;
                                        }
                                        else
                                        {
                                            objCh.SNo = a11 + 1;
                                        }

                                        objr.Add(objCh);

                                    }
                                }

                                objmms.PurchaseRequisitionChilds.AddRange(objr);
                                objmms.SaveChanges();
                                transaction.Commit();
                                return Json(new { Status = 1, TransNo = Grid.MasterNew.PurRequisitionNo }, JsonRequestBehavior.AllowGet);
                                #endregion
                            }
                            else {
                                transaction.Rollback();
                                return Json("2", JsonRequestBehavior.AllowGet);
                            }

                        }



                    }


                }
                catch (Exception ex) {
                    transaction.Rollback();
                    ex.ToString(); return Json("-1", JsonRequestBehavior.AllowGet); }
            }
            
        }

        public int FindMaxIdofPI(string ProjectId,string PIType, string SessionId)
        {
            int Pival = Convert.ToInt32(PIType);
            var  a1 = objmms.PurchaseRequisitionMasters.Where(x => x.ProjectNo == ProjectId && x.PurchasePI_Type == Pival &&  x.SessionId == SessionId.ToString()).ToList().Max(k => (int?)k.Id);
            int a = a1 == null ? 0 : (int)a1;
            if (a == 0)
                return 1;
            else
                return a + 1;
        }

        public decimal FindMaxUIDOfPIMaster()
        {
            decimal a = objmms.PurchaseRequisitionMasters.ToList().DefaultIfEmpty().Max(i => i.UId);
            if (a == 0)
            {
                return 1;
            }
            else {
                return a + 1;
            }
        }

        public ActionResult DisplayGrid(string ProjectId, string PIType)
        {
            List<PendingPIGrid> Result = new List<PendingPIGrid>();
            Result = objmsr.GetOutPITypeGrid(ProjectId,PIType,EmpId);
            if (Result != null)
            {
                return PartialView("_GetOutPiTypeGridDetail", Result);
            }
            else {
                return PartialView("_EmptyView");
            }
           
        }

        public ActionResult PurchaseReqViewDetail(decimal id)
        {

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisitionMaster Master = m.GetPurchaseRequisitionMasterManager().Find(id);

            if (Master.Date != null)
                Master.Date2 = Master.Date.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

            List<PurchaseRequisitionChild> ChildList = m.GetPurchaseRequisitionChildManager().FindByMaster(id);

            var a = objmms.PurchaseRequisitionChilds.Where(b => b.MId == id).ToList();

            var totalRows = ChildList.Count();
            var totRows = a.Count();
            ViewBag.PurchaseTypeId = Master.PurchasePI_Type;
            var data = new PurchaseIRNew()
            {
                MasterNew = Master,
                ChildNew = a
            };

            var FinalPurchasetypeId = objmms.PurchaseRequisitionMasters.Where(x => x.UId == Master.UId).First().PurchasePIC_Type;
            ViewBag.FinalPurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == FinalPurchasetypeId).First().PurchaseType;

            var PILimit = objmms.tblPurchaseRequisitionApprovalLimitValues.Where(x => x.ProjectId == Master.ProjectNo).OrderByDescending(p => p.TransId).First().LimitValue;
            ViewBag.LastLimitVal = PILimit;


            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }

        public ActionResult GetRemarks(string PI)
        {
            
            string PI_ID = PI.Replace("#", "/");
            string Proj_ID = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PI_ID).FirstOrDefault().ProjectNo;
            List<PI_Master_Reamrks> res = null;
            res = objmsr.GetAllItemStatusReport(PI_ID, Proj_ID);
        
            if (res != null)
            {
                return PartialView("_PurchaseOutproRemarkDetails", res);
            }
            else
            {
                return View("_EmptyView");

            }


        }


        #region Get FOC/Cash Purchase Number on 3_6_2019

        public string Get_PurchaseIRNo(string ProjectID, string OutPIType, DateTime Date)
        {
            string res = string.Empty; string J = string.Empty;
            string FnYr = GetCurrentFinancialYear(Date);
            var sesId = objmms.SessionMasters.Where(x => x.Name == FnYr.ToString()).FirstOrDefault().Id;
            res = objmsr.GetOut_PI(ProjectID, Convert.ToInt32(OutPIType), sesId, Convert.ToDateTime(Date));
            //if (res != null)
            //{
            //    var Lst = new { P_No = res };
            //    J = Lst.ToJSON();
            //}
            return res;
        }

        #endregion


    }
}