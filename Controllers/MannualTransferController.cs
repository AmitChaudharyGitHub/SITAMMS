using BusinessObjects.Entity;
using DataAccessLayer;
using MMS.DAL;
using MMS.Helpers;
using MMS.Models;
using MMS.ViewModels;
using MMS_P.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class MannualTransferController : Controller
    {
        // GET: MannualTransfer
        string EmpId = string.Empty;
        string EmpName = string.Empty;
        int SessionId = 1;
        DAL.MMSEntities objmms = new DAL.MMSEntities();
        Procedure objmsr = new Procedure();
        FactoryManager m = new FactoryManager();
        public MannualTransferController()
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

        public ActionResult AddMannualTransfer()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.OrderDate = DateTime.Now.Date;

            ViewBag.ItemGroup = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");

            return View();
        }

        public JsonResult GetPurchaseType()
        {
            var res = objmms.tblPI_PurchaseType.Where(x => x.TrandId ==8).ToList().Select(x => new { Text = x.PurchaseType, Value = x.TrandId }).OrderBy(p => p.Text).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplayGrid(string ProjectId, string PIType)
        {
            List<PendingPIGrid> Result = new List<PendingPIGrid>();
            Result = objmsr.GetOutPITypeGrid(ProjectId, PIType, EmpId);
            if (Result != null)
            {
                return PartialView("_GetMannualTrasnferList", Result);
            }
            else {
                return PartialView("_EmptyView");
            }

        }

        public ActionResult MannualTransferViewDetail(decimal id)
        {

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessObjects.Entity.PurchaseRequisitionMaster Master = m.GetPurchaseRequisitionMasterManager().Find(id);

            if (Master.Date != null)
                Master.Date2 = Master.Date.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

            List<BusinessObjects.Entity.PurchaseRequisitionChild> ChildList = m.GetPurchaseRequisitionChildManager().FindByMaster(id);

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
                return PartialView("_GetManualTransferRemarks", res);
            }
            else
            {
                return View("_EmptyView");

            }


        }

        public JsonResult GetPurchaseIRNo(string ProjectID, string OutPIType)
        {

            string res = string.Empty; string jsonString = string.Empty;
           

            List<DAL.tblEmployeeMaster> E = objmms.tblEmployeeMasters.Where(x => x.EmpID == EmpId).ToList();
            var EO = E.Select(a => new { Value = a.EmpID, Text = a.FName + ' ' + a.LName });
            if (res != null)
            {
                var v = new { List = EO, Message = res };
                jsonString = v.ToJSON();
            }
            else {
                var v = new { List = EO, Message = "N/A" };
                jsonString = v.ToJSON();
            }

            return Json(jsonString, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPurchaseIRNODateWise(string ProjectID, string OutPIType, string ManPurDate)
        {
            string res = string.Empty; string J = string.Empty;
            string Fynm = GetCurrentFinancialYear(Convert.ToDateTime(ManPurDate));
            var SessionId = objmms.SessionMasters.Where(x => x.Name == Fynm.ToString()).FirstOrDefault().Id;
            res = objmsr.GetManual_PI(ProjectID, Convert.ToInt32(OutPIType), SessionId,Convert.ToDateTime(ManPurDate));
            if (res != null)
            {
                var vd = new { Message = res };
                J = vd.ToJSON();
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

        public ActionResult SavePIDetail(PurchaseOutIR Grid)
        {
            using (var transaction = objmms.Database.BeginTransaction())
            {

                try
                {
                    DAL.PurchaseRequisitionMaster objM = new DAL.PurchaseRequisitionMaster();
                    List<DAL.PurchaseRequisitionChild> objr = new List<DAL.PurchaseRequisitionChild>();

                    Grid.MasterNew.PurRequisitionNo = GetMannualTransferNo(Grid.MasterNew.ProjectNo, Grid.MasterNew.PurchasePI_Type.Value.ToString(), Grid.MasterNew.Date.Value);
                    var masterLst = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Grid.MasterNew.PurRequisitionNo && x.ProjectNo == Grid.MasterNew.ProjectNo).ToList();
                    if (masterLst.Count() > 0)
                    {
                        return Json("3", JsonRequestBehavior.AllowGet); // duplicate
                    }
                    else
                    {

                        if (EmpId == null || EmpId == "")
                        {
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
                                return Json("1", JsonRequestBehavior.AllowGet);
                                #endregion
                            }
                            else {
                                return Json("2", JsonRequestBehavior.AllowGet);
                            }

                        }



                    }


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ex.ToString(); return Json("2", JsonRequestBehavior.AllowGet);
                }
            }
        }


        public int FindMaxIdofPI(string ProjectId, string PIType, string Session)
        {
            int Pival = Convert.ToInt32(PIType);
            var a1 = objmms.PurchaseRequisitionMasters.Where(x => x.ProjectNo == ProjectId && x.PurchasePI_Type == Pival && x.SessionId==Session).ToList().Max(k => (int?)k.Id);
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

        public ActionResult UpDateMannualTransfer()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            ViewBag.EmpId = empId;
            return View();
        }

        public ActionResult ApprvPiGridWithMannualTransfer(string PRJID, string PurchType)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            result = objmsr.GetPIDetailWithPurchase(PRJID, PurchType);

            if (result != null)
            {
                return PartialView("_PendingMannualTransfer", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

        public ActionResult Partial_POWithMannualTransfer(string PRJID, string PurchType)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            result = objmsr.GetPartialPo_DetailWithPurchase(PRJID, PurchType);
            if (result != null)
            {
                return PartialView("_PartialMannualTransfer", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }
        public ActionResult Updated_POWitMannualTransfer(string PRJID, string PurchType)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            result = objmsr.GetUpdatedPo_DetailWithPurchase(PRJID, PurchType);
            if (result != null)
            {
                return PartialView("_UpdatedMannualTransfer", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }
        public JsonResult BindEmployeePurchaseTypeAtMannual()
        {
            string J = null; string EmpId = string.Empty;
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            string PurchIds = objmms.tblEmployeeMasters.Where(x => x.EmpID == EmpId).ToList().FirstOrDefault().PurchaseType;
            if (PurchIds != null)
            {
                string[] purtyps = PurchIds.Split(',');
                var a = objmms.tblPI_PurchaseType.Where(xx => purtyps.Contains(xx.TrandId.ToString()) && xx.TrandId == 8).Select(y => new { Text = y.PurchaseType, Value = y.TrandId }).OrderBy(s => s.Text).ToList();
                J = a.ToJSON();
                return Json(J, JsonRequestBehavior.AllowGet);


            }
            else
            {
                return Json(J, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult MannualTransferPurchaseOrderAtPIC()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            ViewBag.EmpId = empId;
            return View();
        }

        public ActionResult GetPending_MannualPOGrid(string ProjectId, string Type)
        {
          

            List<Print_Slip_For_ApprovalController.Getdata> result = new List<Print_Slip_For_ApprovalController.Getdata>();
            result = null;
            result = objmsr.Get_PendingMannualPO(ProjectId,EmpId);

            var totalRows = result.Count();

            var data = new PrinSlip_Approval_VMGST()
            {
                TotalRows = totalRows,
                PageSize = 3500,
                GetDetails_Approved_PO_Slip = result
            };

            if (data != null && data.TotalRows != 0)
            {
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PendingMannualPurchaseOrderGrid", data);
                }

                else
                {
                    return PartialView("_EmptyView");
                }
               
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

        public ActionResult GetUpdated_MannualPOGrid(string ProjectId, string Type)
        {

            List<Print_Slip_For_ApprovalController.Getdata> result = new List<Print_Slip_For_ApprovalController.Getdata>();
            result = null;
            result = objmsr.Get_UpdatedMannualPO(ProjectId, EmpId);

            var totalRows = result.Count();

            var data = new PrinSlip_Approval_VMGST()
            {
                TotalRows = totalRows,
                PageSize = 3500,
                GetDetails_Approved_PO_Slip = result
            };

            if (data != null && data.TotalRows != 0)
            {
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_UpdatedMannualTransferPOGrid", data);
                }

                else
                {
                    return PartialView("_EmptyView");
                }

            }
            else
            {
                return PartialView("_EmptyView");

            }
        }


        public ActionResult GetMannualTransferPO_details(string id)
        {


            WordToText print = new WordToText();
            string empId = Session["EmpID"].ToString();

            try
            {

            



            //main code start here
            id = id?.Replace("#", "/");
            Session["POId"] = id.ToString();

            var a = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectNo,
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectNo).First().ProjectName
            });
            ViewBag.prjtid = t;


            string PurReq = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).FirstOrDefault().IndentRefNo;
            int? PurchaseTypeIdSel = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReq).FirstOrDefault().PurchasePIC_Type;




            var units = "";
            if (id != null)
            {
                var querys = (from mas in objmms.TbIndentPurchaseOrder_GST
                              join chld in objmms.TbIndentPurchaseOrderChild_GST
                             // Both anonymous types should have exact same number of properties having same name and datatype
                             // on new { a = (int?)mas.UId } equals new { a = chld.MUId }
                             on mas.UId equals chld.MUId
                              where mas.PurchaseOrderNo == id
                              select new Purchase_Order_Slip_VM
                              {
                                  SupplierNo = mas.SupplierNo,
                                  Name = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Name,
                                  Address = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Address,
                                  Vendor_country_state_city = objmms.tblCityLists.Where(k3 => k3.CityID == objmms.tblVendorMasters.Where(ct => ct.VendorID == mas.SupplierNo).FirstOrDefault().City).FirstOrDefault().CityName + ", " +
                                            objmms.tblStates.Where(k2 => k2.StateID == objmms.tblVendorMasters.Where(st => st.VendorID == mas.SupplierNo).FirstOrDefault().State).FirstOrDefault().StateName + ", " +
                                            objmms.tblCountries.Where(k1 => k1.CountryID == objmms.tblVendorMasters.Where(gr => gr.VendorID == mas.SupplierNo).FirstOrDefault().Country).FirstOrDefault().CnName,

                                  VendorTInNO = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().TinNo,
                                  VendorGSTNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().GST_NO,
                                  AcilTinNo = objmms.tblProjectTINNoes.Where(k => k.ProjectId == mas.ProjectNo).FirstOrDefault().TINNo,
                                  AcilGSTNo = objmms.tblProjectGSTNoes.Where(k => k.ProjectId == mas.ProjectNo).FirstOrDefault().GSTNo,
                                  MobileNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().MobileNo,
                                  ContactPerson = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().ContactPerson,

                                  Reference = mas.Reference,

                                  IndentRefNo = mas.IndentRefNo,
                                  PurchaseOrderNo = mas.PurchaseOrderNo,

                                  SubTotal = chld.SubTotal1,
                                  Total = mas.Total,
                                  GrandTotal = mas.GrandTotal,
                                  ProjectNo = mas.ProjectNo,
                                  ProjectName = objmms.tblProjectMasters.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().ProjectName,
                                  Location = objmms.tblProjectParticularsDetailAs.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().Location,

                                  PurchaseOrderDate = mas.PurchaseOrderDate,
                                  ItemNo = objmms.tblItemMasters.Where(y => y.ItemID == chld.ItemNo).FirstOrDefault().GITEMCode,
                                  ItemName = objmms.tblItemMasters.Where(j => j.ItemID == chld.ItemNo).FirstOrDefault().ItemName,
                                  UnitID = objmms.tblItemMasters.Where(l => l.ItemID == chld.ItemNo).FirstOrDefault().UnitID,
                                  UnitName = objmms.tblUnitMasters.Where(l => l.UnitID == objmms.tblItemMasters.Where(ll => ll.ItemID == chld.ItemNo).FirstOrDefault().UnitID).FirstOrDefault().UnitCode,
                                  Amount = chld.Total,
                                  Discount = chld.Discount,
                                  Qty = chld.Qty,
                                  Rate = chld.NewRate,

                                  //  --------------------added Newely here
                                  Cartage_Type_1 = objmms.tblMMSCartageTypes.Where(x => x.TransId == chld.CartageTypeId).FirstOrDefault().CartageType,
                                  CR1_Total = chld.CartageAmount,
                                  T_Total = mas.Total,
                                  P_Total = chld.PackingChargesAmount,
                                  T_SubTotal1 = mas.SubTotal1,
                                  T_SubTotal2 = mas.SubTotal2,

                                  POCreatedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().LName,
                                  // UserType = UserType(mas.PurchaseOrderNo)
                                  UserType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == empId).FirstOrDefault().TypeName,
                                  CheckedPoLimit = mas.CheckedBeyondPOLimit,
                                  IsReleasePOBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().LName,
                                  PoApprovedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().FName +
                                                 " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().LName,

                                  // Added After GST.

                                  ItemToTAmt = chld.Total,
                                  SubTotal1 = chld.SubTotal1,
                                  SubTotal2 = chld.SubTotal2,
                                  PackagePercentage = chld.PackingChargesPercentage,
                                  PackingPercentageAmt = chld.PackingChargesAmount,
                                  CartageType = objmms.tblMMSCartageTypes.Where(x => x.TransId == chld.CartageTypeId).FirstOrDefault().CartageType,
                                  CartageTypeRate = chld.Cartage_rate,
                                  CartageAmt = chld.CartageAmount,
                                  InsurancePercentage = chld.InsuranceRate,
                                  InsurancePercentageAmt = chld.InsuranceAmount,
                                  TaxableAmount = chld.SubTotal2,
                                  CGSTPercentage = chld.CGSTPercentage,
                                  CGSTAmt = chld.CGSTAmount,
                                  SGSTAndUTGSTPercentage = chld.SGSTPercentage,
                                  SGSTAndUTGSTAmt = chld.SGSTAmount,
                                  IGST = chld.IGSTPercentage,
                                  IGSTAmt = chld.IGSTAmount,
                                  GrossAmount = chld.GrossAmount,
                                  HSNCode = objmms.tblItemMasters.Where(x => x.ItemID == chld.ItemNo).FirstOrDefault().HSNCode,
                                  TotalPackingSum = mas.Total_PAndF,
                                  TotalCartageSum = mas.Total_Cartage,
                                  TotalInsuranceSum = mas.Total_Insurance,
                                  TotalTaxableAmountSum = mas.SubTotal2,
                                  TotalCGSTSum = mas.Total_CGST,
                                  TotalSCGSTAndUTGSTSum = mas.Total_SGSTAndUTGST,
                                  IGSTSum = mas.Total_IGST,
                                  TotalAmountInWords = mas.Total_NetAmountInWords,
                                  ItemDescrp = chld.Item_Description












                              }).ToList();
                //var projtn=querys.Select(k=> k.ProjectNo == )
                var data = querys;
                Print_Slip_For_ApprovalController.PrintData dataObj = new Print_Slip_For_ApprovalController.PrintData();
                dataObj.HeaderData = data.Select(x => new Print_Slip_For_ApprovalController.HeaderData
                {
                    PurchaseOrderNo = x.PurchaseOrderNo,
                    Total = Convert.ToDecimal(Math.Round((double)(x.Total != null ? x.Total : 0), 2).ToString("0.00")),
                    Location = x.Location,
                    ContactPerson = x.ContactPerson,
                    Vendor_country_state_city = x.Vendor_country_state_city,
                    VendorTInNO = x.VendorTInNO,
                    VendorGSTNo = x.VendorGSTNo,
                    MobileNo = x.MobileNo,
                    Reference = x.Reference,

                    Cartage = x.Cartage,
                    GrandTotal = Convert.ToDecimal(Math.Round((double)(x.GrandTotal != null ? x.GrandTotal : 0), 2).ToString("0.00")),

                    SubTotal = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                    PurchaseOrderDate = x.PurchaseOrderDate,
                    AcilTinNo = x.AcilTinNo,
                    AcilGSTNo = x.AcilGSTNo,
                    Address = x.Address,
                    Name = x.Name,
                    ProjectName = x.ProjectName,
                    IndentRefNo = x.IndentRefNo,
                    ExciseDutyRate = x.ExciseDutyRate,
                    ExciseDuty = x.ExciseDuty,

                    //Ex_Total = Math.Round((decimal)data.Sum(k => k.Ex_Total), 2),
                    Cartage_Type_1 = x.Cartage_Type_1,
                    CR1_Total = Convert.ToDecimal(Math.Round((decimal)data.Sum(k => k.CR1_Total), 2).ToString("0.00")),




                    T_Total = x.T_Total,
                    P_Total = Math.Round((decimal)data.Sum(k => k.P_Total), 2),
                    T_SubTotal1 = x.T_SubTotal1,
                    T_SubTotal2 = x.T_SubTotal2,
                    ProjectNo = x.ProjectNo,
                    PoCreatedBy = x.POCreatedBy,
                    UserType = x.UserType,
                    CheckedPoLimit = x.CheckedPoLimit.ToString(),
                    PoApprovedBy = x.PoApprovedBy,
                    IsReleasePOBy = x.IsReleasePOBy,

                    TotalPackingSum = Convert.ToDecimal(Math.Round((double)(x.TotalPackingSum != null ? x.TotalPackingSum : 0), 2).ToString("0.00")),
                    TotalCartageSum = Convert.ToDecimal(Math.Round((double)(x.TotalCartageSum != null ? x.TotalCartageSum : 0), 2).ToString("0.00")),
                    TotalInsuranceSum = x.TotalInsuranceSum,
                    TotalTaxableAmountSum = Convert.ToDecimal(Math.Round((double)(x.TotalTaxableAmountSum != null ? x.TotalTaxableAmountSum : 0), 2).ToString("0.00")),
                    TotalCGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalCGSTSum != null ? x.TotalCGSTSum : 0), 2).ToString("0.00")),
                    TotalSCGSTAndUTGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalSCGSTAndUTGSTSum != null ? x.TotalSCGSTAndUTGSTSum : 0), 2).ToString("0.00")),
                    IGSTSum = Convert.ToDecimal(Math.Round((double)(x.IGSTSum != null ? x.IGSTSum : 0), 2).ToString("0.00")),
                    TotalAmountInWords = x.TotalAmountInWords



                }).FirstOrDefault();

                dataObj.ItemData = data.Select(x => new Print_Slip_For_ApprovalController.ItemData
                {
                    ItemName = x.ItemName,
                    ItemNo = x.ItemNo,
                    ItemDescrp = x.ItemDescrp,
                    Remark = x.Remark,
                    UnitName = x.UnitName,
                    Rate = Convert.ToDecimal(Math.Round((double)(x.Rate != null ? x.Rate : 0), 2).ToString("0.00")),
                    Discount = Convert.ToDecimal(Math.Round((double)(x.Discount != null ? x.Discount : 0), 2).ToString("0.00")),
                    SubTotal1 = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                    Qty = x.Qty,
                    ItemToTAmt = Convert.ToDecimal(Math.Round((double)(x.ItemToTAmt != null ? x.ItemToTAmt : 0), 2).ToString("0.00")),
                    SubTotal2 = Convert.ToDecimal(Math.Round((double)(x.SubTotal2 != null ? x.SubTotal2 : 0), 2).ToString("0.00")),
                    PackagePercentage = Convert.ToDecimal(Math.Round((double)(x.PackagePercentage != null ? x.PackagePercentage : 0), 2).ToString("0.00")),
                    PackingPercentageAmt = Convert.ToDecimal(Math.Round((double)(x.PackingPercentageAmt != null ? x.PackingPercentageAmt : 0), 2).ToString("0.00")),
                    CartageType = x.CartageType,
                    CartageTypeRate = Convert.ToDecimal(Math.Round((double)(x.CartageTypeRate != null ? x.CartageTypeRate : 0), 2).ToString("0.00")),
                    CartageAmt = Convert.ToDecimal(Math.Round((double)(x.CartageAmt != null ? x.CartageAmt : 0), 2).ToString()),
                    InsurancePercentage = x.InsurancePercentage,
                    InsurancePercentageAmt = x.InsurancePercentageAmt,
                    TaxableAmount = Convert.ToDecimal(Math.Round((double)(x.TaxableAmount != null ? x.TaxableAmount : 0), 2).ToString("0.00")),
                    CGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.CGSTPercentage != null ? x.CGSTPercentage : 0), 2).ToString("0.00")),
                    CGSTAmt = Convert.ToDecimal(Math.Round((double)(x.CGSTAmt != null ? x.CGSTAmt : 0), 2).ToString("0.00")),
                    SGSTAndUTGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTPercentage != null ? x.SGSTAndUTGSTPercentage : 0), 2).ToString("0.00")),
                    SGSTAndUTGSTAmt = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTAmt != null ? x.SGSTAndUTGSTAmt : 0), 2).ToString("0.00")),
                    IGST = Convert.ToDecimal(Math.Round((double)(x.IGST != null ? x.IGST : 0), 2).ToString("0.00")),
                    IGSTAmt = Convert.ToDecimal(Math.Round((double)(x.IGSTAmt != null ? x.IGSTAmt : 0), 2).ToString("0.00")),
                    GrossAmount = Convert.ToDecimal(Math.Round((double)x.GrossAmount, 2).ToString("0.00")),
                    HSNCode = x.HSNCode
                }).ToList();


                ViewBag.PrintamountInWord = print.Cal(dataObj.HeaderData.GrandTotal ?? 0);

           


               return PartialView("_PendingMannualPOPrintforApproval", dataObj);


            }

            else {
                return Json(null, JsonRequestBehavior.AllowGet);
            }


            }
            catch (Exception ex)
            {
                ex.ToString();
                return Json(null, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult SaveRemark(POApprovalwithRemarks ListDatas)
        { 
            string empId = Session["EmpID"].ToString();

            string emptype = objmms.tblApproval_AccountType.Where(p => p.EmployeeId == empId.ToString()).First().TypeName;

            try
            {
                if (ModelState.IsValid)
                {
                    tblALLRemark alrmrks = new tblALLRemark();

                    if (ListDatas.Status_Approval == "Approved")
                    {
                       
                        //if (emptype == "Purchase")
                        //{
                        //    TbIndentPurchaseOrder_GST emp1 = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).FirstOrDefault();
                        //    emp1.Status = "Approved";
                        //    emp1.SendTo = ListDatas.ForwordTo;
                        //    emp1.SecondLevelApprv_Id = ListDatas.ForwordTo;
                        //    objmms.Entry(emp1).State = EntityState.Modified;
                        //    alrmrks.CreatedDate = System.DateTime.Now;
                        //    alrmrks.RemarkBy = empId;
                        //    alrmrks.RemarkDate = System.DateTime.Now;
                        //    alrmrks.Remarks = ListDatas.Remark;
                        //    alrmrks.RemarkStage = emptype;
                        //    alrmrks.PurchaseOrderNo = ListDatas.Purchase_Order_Indent_No;
                        //    objmms.tblALLRemarks.Add(alrmrks);

                        //}
                        if (emptype == "PIC" || emptype == "Mang")
                        {
                           

                            TbIndentPurchaseOrder_GST emp = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).FirstOrDefault();

                            emp.Status = ListDatas.Status_Approval;
                            // new added

                            emp.SecondLevelApprv_Remarks = ListDatas.Remark;
                            emp.SecondLevelApprv_Status = "Approved";
                            emp.SecondLevelApprv_Date = System.DateTime.Now;
                            emp.IsPORelease = Convert.ToBoolean(1);
                            emp.POReleaseBy = EmpId;
                            emp.POReleaseDate = System.DateTime.Now;




                            objmms.Entry(emp).State = EntityState.Modified;

                            alrmrks.CreatedDate = System.DateTime.Now;
                            alrmrks.RemarkBy = empId;
                            alrmrks.RemarkDate = System.DateTime.Now;
                            alrmrks.Remarks = ListDatas.Remark;
                            alrmrks.RemarkStage = emptype;
                            alrmrks.PurchaseOrderNo = ListDatas.Purchase_Order_Indent_No;
                            objmms.tblALLRemarks.Add(alrmrks);



                           
                        }
                        else { }




                    }
                    else
                    {
                        
                        if (emptype == "PIC" || emptype == "Mang" )
                        {
                           
                            TbIndentPurchaseOrder_GST emp = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).FirstOrDefault();

                           
                            emp.SendTo = ListDatas.ForwordTo;
                            emp.Status = ListDatas.Status_Approval;
                            emp.SecondLevelApprv_Remarks = ListDatas.Remark;
                            emp.SecondLevelApprv_Status = "Not Approved";
                            emp.SecondLevelApprv_Date = System.DateTime.Now;


                            objmms.Entry(emp).State = EntityState.Modified;


                            alrmrks.CreatedDate = System.DateTime.Now;
                            alrmrks.RemarkBy = empId;
                            alrmrks.RemarkDate = System.DateTime.Now;
                            alrmrks.Remarks = ListDatas.Remark;
                            alrmrks.RemarkStage = emptype;
                            alrmrks.PurchaseOrderNo = ListDatas.Purchase_Order_Indent_No;
                            objmms.tblALLRemarks.Add(alrmrks);


                           
                        }
                        else {


                        }



                    }
                    objmms.SaveChanges();
                }
                return View("Index");
            }
            catch
            {
                return Json("2", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUpdatedAndReleasedMannualPO(string id)
        {

            WordToText print = new WordToText();
            string empId = Session["EmpID"].ToString();





            //main code start here
            id = id?.Replace("#", "/");
            Session["POId"] = id.ToString();

            var a = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectNo,
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectNo).First().ProjectName
            });
            ViewBag.prjtid = t;


            string PurReq = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).FirstOrDefault().IndentRefNo;
            int? PurchaseTypeIdSel = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReq).FirstOrDefault().PurchasePIC_Type;




            var units = "";
            if (id != null)
            {
                var querys = (from mas in objmms.TbIndentPurchaseOrder_GST
                              join chld in objmms.TbIndentPurchaseOrderChild_GST
                             // Both anonymous types should have exact same number of properties having same name and datatype
                             // on new { a = (int?)mas.UId } equals new { a = chld.MUId }
                             on mas.UId equals chld.MUId
                              where mas.PurchaseOrderNo == id
                              select new Purchase_Order_Slip_VM
                              {
                                  SupplierNo = mas.SupplierNo,
                                  Name = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Name,
                                  Address = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Address,
                                  Vendor_country_state_city = objmms.tblCityLists.Where(k3 => k3.CityID == objmms.tblVendorMasters.Where(ct => ct.VendorID == mas.SupplierNo).FirstOrDefault().City).FirstOrDefault().CityName + ", " +
                                            objmms.tblStates.Where(k2 => k2.StateID == objmms.tblVendorMasters.Where(st => st.VendorID == mas.SupplierNo).FirstOrDefault().State).FirstOrDefault().StateName + ", " +
                                            objmms.tblCountries.Where(k1 => k1.CountryID == objmms.tblVendorMasters.Where(gr => gr.VendorID == mas.SupplierNo).FirstOrDefault().Country).FirstOrDefault().CnName,

                                  VendorTInNO = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().TinNo,
                                  VendorGSTNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().GST_NO,
                                  AcilTinNo = objmms.tblProjectTINNoes.Where(k => k.ProjectId == mas.ProjectNo).FirstOrDefault().TINNo,
                                  AcilGSTNo = objmms.tblProjectGSTNoes.Where(k => k.ProjectId == mas.ProjectNo).FirstOrDefault().GSTNo,
                                  MobileNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().MobileNo,
                                  ContactPerson = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().ContactPerson,

                                  Reference = mas.Reference,

                                  IndentRefNo = mas.IndentRefNo,
                                  PurchaseOrderNo = mas.PurchaseOrderNo,

                                  SubTotal = chld.SubTotal1,
                                  Total = mas.Total,
                                  GrandTotal = mas.GrandTotal,
                                  ProjectNo = mas.ProjectNo,
                                  ProjectName = objmms.tblProjectMasters.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().ProjectName,
                                  Location = objmms.tblProjectParticularsDetailAs.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().Location,

                                  PurchaseOrderDate = mas.PurchaseOrderDate,
                                  ItemNo = objmms.tblItemMasters.Where(y => y.ItemID == chld.ItemNo).FirstOrDefault().GITEMCode,
                                  ItemName = objmms.tblItemMasters.Where(j => j.ItemID == chld.ItemNo).FirstOrDefault().ItemName,
                                  UnitID = objmms.tblItemMasters.Where(l => l.ItemID == chld.ItemNo).FirstOrDefault().UnitID,
                                  UnitName = objmms.tblUnitMasters.Where(l => l.UnitID == objmms.tblItemMasters.Where(ll => ll.ItemID == chld.ItemNo).FirstOrDefault().UnitID).FirstOrDefault().UnitCode,
                                  Amount = chld.Total,
                                  Discount = chld.Discount,
                                  Qty = chld.Qty,
                                  Rate = chld.NewRate,

                                  //  --------------------added Newely here
                                  Cartage_Type_1 = objmms.tblMMSCartageTypes.Where(x => x.TransId == chld.CartageTypeId).FirstOrDefault().CartageType,
                                  CR1_Total = chld.CartageAmount,
                                  T_Total = mas.Total,
                                  P_Total = chld.PackingChargesAmount,
                                  T_SubTotal1 = mas.SubTotal1,
                                  T_SubTotal2 = mas.SubTotal2,

                                  POCreatedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().LName,
                                  // UserType = UserType(mas.PurchaseOrderNo)
                                  UserType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == empId).FirstOrDefault().TypeName,
                                  CheckedPoLimit = mas.CheckedBeyondPOLimit,
                                  IsReleasePOBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().LName,
                                  PoApprovedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().FName +
                                                 " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().LName,

                                  // Added After GST.

                                  ItemToTAmt = chld.Total,
                                  SubTotal1 = chld.SubTotal1,
                                  SubTotal2 = chld.SubTotal2,
                                  PackagePercentage = chld.PackingChargesPercentage,
                                  PackingPercentageAmt = chld.PackingChargesAmount,
                                  CartageType = objmms.tblMMSCartageTypes.Where(x => x.TransId == chld.CartageTypeId).FirstOrDefault().CartageType,
                                  CartageTypeRate = chld.Cartage_rate,
                                  CartageAmt = chld.CartageAmount,
                                  InsurancePercentage = chld.InsuranceRate,
                                  InsurancePercentageAmt = chld.InsuranceAmount,
                                  TaxableAmount = chld.SubTotal2,
                                  CGSTPercentage = chld.CGSTPercentage,
                                  CGSTAmt = chld.CGSTAmount,
                                  SGSTAndUTGSTPercentage = chld.SGSTPercentage,
                                  SGSTAndUTGSTAmt = chld.SGSTAmount,
                                  IGST = chld.IGSTPercentage,
                                  IGSTAmt = chld.IGSTAmount,
                                  GrossAmount = chld.GrossAmount,
                                  HSNCode = objmms.tblItemMasters.Where(x => x.ItemID == chld.ItemNo).FirstOrDefault().HSNCode,
                                  TotalPackingSum = mas.Total_PAndF,
                                  TotalCartageSum = mas.Total_Cartage,
                                  TotalInsuranceSum = mas.Total_Insurance,
                                  TotalTaxableAmountSum = mas.SubTotal2,
                                  TotalCGSTSum = mas.Total_CGST,
                                  TotalSCGSTAndUTGSTSum = mas.Total_SGSTAndUTGST,
                                  IGSTSum = mas.Total_IGST,
                                  TotalAmountInWords = mas.Total_NetAmountInWords,
                                  ItemDescrp = chld.Item_Description












                              }).ToList();
                //var projtn=querys.Select(k=> k.ProjectNo == )
                var data = querys;
                Print_Slip_For_ApprovalController.PrintData dataObj = new Print_Slip_For_ApprovalController.PrintData();
                dataObj.HeaderData = data.Select(x => new Print_Slip_For_ApprovalController.HeaderData
                {
                    PurchaseOrderNo = x.PurchaseOrderNo,
                    Total = Convert.ToDecimal(Math.Round((double)(x.Total != null ? x.Total : 0), 2).ToString("0.00")),
                    Location = x.Location,
                    ContactPerson = x.ContactPerson,
                    Vendor_country_state_city = x.Vendor_country_state_city,
                    VendorTInNO = x.VendorTInNO,
                    VendorGSTNo = x.VendorGSTNo,
                    MobileNo = x.MobileNo,
                    Reference = x.Reference,

                    Cartage = x.Cartage,
                    GrandTotal = Convert.ToDecimal(Math.Round((double)(x.GrandTotal != null ? x.GrandTotal : 0), 2).ToString("0.00")),

                    SubTotal = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                    PurchaseOrderDate = x.PurchaseOrderDate,
                    AcilTinNo = x.AcilTinNo,
                    AcilGSTNo = x.AcilGSTNo,
                    Address = x.Address,
                    Name = x.Name,
                    ProjectName = x.ProjectName,
                    IndentRefNo = x.IndentRefNo,
                    ExciseDutyRate = x.ExciseDutyRate,
                    ExciseDuty = x.ExciseDuty,

                    //Ex_Total = Math.Round((decimal)data.Sum(k => k.Ex_Total), 2),
                    Cartage_Type_1 = x.Cartage_Type_1,
                    CR1_Total = Convert.ToDecimal(Math.Round((decimal)data.Sum(k => k.CR1_Total), 2).ToString("0.00")),




                    T_Total = x.T_Total,
                    P_Total = Math.Round((decimal)data.Sum(k => k.P_Total), 2),
                    T_SubTotal1 = x.T_SubTotal1,
                    T_SubTotal2 = x.T_SubTotal2,
                    ProjectNo = x.ProjectNo,
                    PoCreatedBy = x.POCreatedBy,
                    UserType = x.UserType,
                    CheckedPoLimit = x.CheckedPoLimit.ToString(),
                    PoApprovedBy = x.PoApprovedBy,
                    IsReleasePOBy = x.IsReleasePOBy,

                    TotalPackingSum = Convert.ToDecimal(Math.Round((double)(x.TotalPackingSum != null ? x.TotalPackingSum : 0), 2).ToString("0.00")),
                    TotalCartageSum = Convert.ToDecimal(Math.Round((double)(x.TotalCartageSum != null ? x.TotalCartageSum : 0), 2).ToString("0.00")),
                    TotalInsuranceSum = x.TotalInsuranceSum,
                    TotalTaxableAmountSum = Convert.ToDecimal(Math.Round((double)(x.TotalTaxableAmountSum != null ? x.TotalTaxableAmountSum : 0), 2).ToString("0.00")),
                    TotalCGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalCGSTSum != null ? x.TotalCGSTSum : 0), 2).ToString("0.00")),
                    TotalSCGSTAndUTGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalSCGSTAndUTGSTSum != null ? x.TotalSCGSTAndUTGSTSum : 0), 2).ToString("0.00")),
                    IGSTSum = Convert.ToDecimal(Math.Round((double)(x.IGSTSum != null ? x.IGSTSum : 0), 2).ToString("0.00")),
                    TotalAmountInWords = x.TotalAmountInWords



                }).FirstOrDefault();

                dataObj.ItemData = data.Select(x => new Print_Slip_For_ApprovalController.ItemData
                {
                    ItemName = x.ItemName,
                    ItemNo = x.ItemNo,
                    ItemDescrp = x.ItemDescrp,
                    Remark = x.Remark,
                    UnitName = x.UnitName,
                    Rate = Convert.ToDecimal(Math.Round((double)(x.Rate != null ? x.Rate : 0), 2).ToString("0.00")),
                    Discount = Convert.ToDecimal(Math.Round((double)(x.Discount != null ? x.Discount : 0), 2).ToString("0.00")),
                    SubTotal1 = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                    Qty = x.Qty,
                    ItemToTAmt = Convert.ToDecimal(Math.Round((double)(x.ItemToTAmt != null ? x.ItemToTAmt : 0), 2).ToString("0.00")),
                    SubTotal2 = Convert.ToDecimal(Math.Round((double)(x.SubTotal2 != null ? x.SubTotal2 : 0), 2).ToString("0.00")),
                    PackagePercentage = Convert.ToDecimal(Math.Round((double)(x.PackagePercentage != null ? x.PackagePercentage : 0), 2).ToString("0.00")),
                    PackingPercentageAmt = Convert.ToDecimal(Math.Round((double)(x.PackingPercentageAmt != null ? x.PackingPercentageAmt : 0), 2).ToString("0.00")),
                    CartageType = x.CartageType,
                    CartageTypeRate = Convert.ToDecimal(Math.Round((double)(x.CartageTypeRate != null ? x.CartageTypeRate : 0), 2).ToString("0.00")),
                    CartageAmt = Convert.ToDecimal(Math.Round((double)(x.CartageAmt != null ? x.CartageAmt : 0), 2).ToString()),
                    InsurancePercentage = x.InsurancePercentage,
                    InsurancePercentageAmt = x.InsurancePercentageAmt,
                    TaxableAmount = Convert.ToDecimal(Math.Round((double)(x.TaxableAmount != null ? x.TaxableAmount : 0), 2).ToString("0.00")),
                    CGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.CGSTPercentage != null ? x.CGSTPercentage : 0), 2).ToString("0.00")),
                    CGSTAmt = Convert.ToDecimal(Math.Round((double)(x.CGSTAmt != null ? x.CGSTAmt : 0), 2).ToString("0.00")),
                    SGSTAndUTGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTPercentage != null ? x.SGSTAndUTGSTPercentage : 0), 2).ToString("0.00")),
                    SGSTAndUTGSTAmt = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTAmt != null ? x.SGSTAndUTGSTAmt : 0), 2).ToString("0.00")),
                    IGST = Convert.ToDecimal(Math.Round((double)(x.IGST != null ? x.IGST : 0), 2).ToString("0.00")),
                    IGSTAmt = Convert.ToDecimal(Math.Round((double)(x.IGSTAmt != null ? x.IGSTAmt : 0), 2).ToString("0.00")),
                    GrossAmount = Convert.ToDecimal(Math.Round((double)x.GrossAmount, 2).ToString("0.00")),
                    HSNCode = x.HSNCode
                }).ToList();
                ViewBag.PrintamountInWord = print.Cal(dataObj.HeaderData.GrandTotal ?? 0);




                return PartialView("_ApprovedMannualTransferPrintedPO", dataObj);


            }

            else {
                return Json(null, JsonRequestBehavior.AllowGet);
            }



        }

        #region Get Mannual Transfer No. on 9_3_2019

        public string GetMannualTransferNo(string ProjectID, string OutPIType, DateTime ManPurDate)
        {
            string res = string.Empty; string J = string.Empty;
            string Fynm = GetCurrentFinancialYear(ManPurDate);
            var SessionId = objmms.SessionMasters.Where(x => x.Name == Fynm.ToString()).FirstOrDefault().Id;
            res = objmsr.GetManual_PI(ProjectID, Convert.ToInt32(OutPIType), SessionId, Convert.ToDateTime(ManPurDate));


            return res;
        }

        #endregion
    }
}