using MMS.DAL;
using MMS.Helpers;
using MMS.Models;
using MMS.ViewModels;
using MMS_P.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class IntraTransferController : Controller
    {
        // GET: IntraTransfer
        string EmpId = string.Empty;
        private MMSEntities objmms = new MMSEntities();
        Procedure ObjP = new Procedure();
        public IntraTransferController()
        {
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();

        }
        public ActionResult Index(string PId = "")
        {
            ViewBag.EmpId = EmpId;
            ViewBag.PId = PId;
            ViewBag.Date = DateTime.Now.Date;
            ViewBag.EmpType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList().Count() > 0 ? objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).FirstOrDefault().TypeName : "Emp";
            return View();
        }

        public ActionResult Transfer()
        {

            List<SelectListItem> lst = new List<SelectListItem>();  //List<SelectListItem> TTr = new List<SelectListItem>();
            lst.Add(new SelectListItem { Text = "Goods", Value = "1" });
            lst.Add(new SelectListItem { Text = "Asset", Value = "2" });

            ViewBag.TType = new SelectList(lst, "Value", "Text");
            ViewBag.EmpType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList().Count() > 0 ? objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).FirstOrDefault().TypeName : "Emp";

            return View();
        }

        public JsonResult GetTransferTypeselection(string SelectedType)
        {

            if (SelectedType == "1")
            {
                var lst = objmms.tblTransfertypeselections.ToList().Select(x => new { Text = x.TransferType, Value = x.Id }).ToList();
                return Json(lst.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else if (SelectedType == "2")
            {
                var lst = objmms.tblTransfertypeselections.ToList().Select(x => new { Text = x.TransferType, Value = x.Id }).ToList();
                return Json(lst.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else {

                return null;
            }


        }


        public ActionResult IntraInvoiceDocument()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.Date = DateTime.Now.Date;
            ViewBag.EmpType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList().Count() > 0 ? objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).FirstOrDefault().TypeName : "Emp";
            return View();
        }

        public ActionResult InterTransfer()
        {
            ViewBag.InfraDate = DateTime.Now.Date;
            ViewBag.EmpId = EmpId;
            return View();
        }

        public ActionResult CreateIntraInvoiceDocument()
        {
            ViewBag.OuterTranDate = DateTime.Now.Date;
            ViewBag.EmpId = EmpId;
            return View();
        }

        public JsonResult GetAllProjectExceptSelectedProject(string ProjectId, string TransferType)
        {
            DAL.tblProjectGSTNo proj = objmms.tblProjectGSTNoes.Where(x => x.ProjectId == ProjectId).First();
            string StateId = proj.StateId.TrimEnd();
            // var Lsit = objmms.tblProjectMasters.ToList().Where(x => x.PRJID != ProjectId).Select(x => new { Value = x.PRJID, Text = x.ProjectName }).ToList();
            if (TransferType == "Inter")
            {
                var Lsit = (from p in objmms.tblProjectMasters
                            join g in objmms.tblProjectGSTNoes on p.PRJID equals g.ProjectId
                            where g.StateId == StateId && p.PRJID != ProjectId
                            select new
                            {
                                Value = p.PRJID,
                                Text = p.ProjectName
                            }).ToList();

                return Json(Lsit.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else if (TransferType == "Intra")
            {
                var Lsit = (from p in objmms.tblProjectMasters
                            join g in objmms.tblProjectGSTNoes on p.PRJID equals g.ProjectId
                            where g.StateId != StateId && p.PRJID != ProjectId
                            select new
                            {
                                Value = p.PRJID,
                                Text = p.ProjectName
                            }).ToList();

                return Json(Lsit.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Lsit = objmms.tblProjectMasters.ToList().Where(x => x.PRJID != ProjectId).Select(x => new { Value = x.PRJID, Text = x.ProjectName }).ToList();
                return Json(Lsit.ToJSON(), JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetProjectDetail(string ProjectId, string TransferType)
        {
            string J = string.Empty;
            if (ProjectId != null)
            {
                string ProjId = ProjectId;

                DAL.tblProjectGSTNo proj = objmms.tblProjectGSTNoes.Where(x => x.ProjectId == ProjId).First();
                string StateId = proj.StateId.TrimEnd();
                string State = objmms.tblStates.Where(x => x.StateID == StateId).FirstOrDefault().StateName;
                string projectAddress = objmms.tblProjectParticularsDetailAs.Where(x => x.PRJID == ProjectId).FirstOrDefault().Location;
                string ProjectName = objmms.tblProjectParticularsDetailAs.Where(x => x.PRJID == ProjectId).FirstOrDefault().NameOfProject;

                List<tblEmployeeMaster> E1 = objmms.tblEmployeeMasters.ToList().Where(x => x.ProjectID.Contains(ProjId)).ToList();
                List<tblEmployeeMaster> E = (from a in E1 join b in objmms.tblApproval_AccountType.Where(p => p.TypeName == "PIC") on a.EmpID equals b.EmployeeId select a).ToList();

                var EO = E.Select(a => new { Value = a.EmpID, Text = a.FName + ' ' + a.LName });

                var List = new { Lt = EO, ProjectName = ProjectName ?? "N/A", Projectadd = projectAddress ?? "N/A", GSTNo = proj.GSTNo ?? "N/A", StateName = State ?? "N/A" };
                J = List.ToJSON();

            }

            return Json(J, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTRansferCode(string ProjectID, string TransferType, string TransferDate)
        {
            string J = string.Empty;
            J = ObjP.GetInterStateTransferCode(ProjectID, TransferType, Convert.ToDateTime(TransferDate));

            int? divisionId = Repos.GetUserDivision();
            MSP_Model objmsp = new MSP_Model();
            var sessioncode = objmsp.GetFinancialYearByDate(Convert.ToDateTime(TransferDate));
            string last_ISDate = "";
            string last_ISNo = "";

            if (TransferType.ToUpper() == "IS")
            {
                var last_IsTrans = objmms.tblInterStateTransferMasters.Where(s => s.ProjectId == ProjectID && s.DivisionId == divisionId && s.Session_Year == sessioncode).OrderByDescending(s => s.TransId).FirstOrDefault();
                if (last_IsTrans != null)
                {
                    last_ISNo = last_IsTrans.InterTransferNumber;
                    last_ISDate = last_IsTrans.TransferDate.Value.ToString("dd/MM/yyyy");
                }
            }

            if (TransferType.ToUpper() == "OS")
            {
                var last_IsTrans = objmms.tblIntraStateTransferMasters.Where(s => s.ProjectId == ProjectID && s.DivisionId == divisionId && s.Session_Year == sessioncode).OrderByDescending(s => s.TransId).FirstOrDefault();
                if (last_IsTrans != null)
                {
                    last_ISNo = last_IsTrans.IntraTransferNumber;
                    last_ISDate = last_IsTrans.TransferDate.Value.ToString("dd/MM/yyyy");
                }
            }


            var L = new { TransferCode = J ?? "N/A", Last_ISDate = last_ISDate, Last_IsNo = last_ISNo };
            return Json(L.ToJSON(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetTransportationMode()
        {

            var List = objmms.tblTransportationModeNews.ToList().Select(x => new SelectListItem { Value = x.TransId.ToString(), Text = x.Mode_of_TPT }).ToList();
            return Json(List.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVehicleType()
        {
            var List = objmms.tblVehicleTypes.ToList().Select(x => new SelectListItem { Value = x.TransId.ToString(), Text = x.Vehicle_Type }).ToList();
            return Json(List.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemGroupMaster()
        {
            var List = objmms.tblItemGroupMasters.ToList().Select(x => new SelectListItem { Value = x.ItemGroupID, Text = x.GroupName }).ToList();

            return Json(List.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTaxPayableStatusOnReverseCharges()
        {
            var List = objmms.tblTaxPayableOnReverseCharges.ToList().Select(x => new SelectListItem { Value = x.TransId.ToString(), Text = x.TaxPayableStatus }).ToList();
            return Json(List.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemDetail(string ProjectId, string ItemGroupId, string ItemId)
        {
            DAL.tblItemMaster ItemDetail = objmms.tblItemMasters.ToList().Where(x => x.ItemGroupID == ItemGroupId && x.ItemID == ItemId).ToList().FirstOrDefault();
            var UniteCode = objmms.tblUnitMasters.ToList().Where(x => x.UnitID == ItemDetail.UnitID).FirstOrDefault().UnitCode;
            string HSNCode = ItemDetail.HSNCode;
            string GIETM = ItemDetail.GITEMCode;
            string ProId = ProjectId;
            decimal IR = 0; decimal? Totalrecv = 0; decimal? Totalbalancd = 0;
            var IRate = objmms.tblReceiveDatas.Where(b => b.ProjectId == ProId && b.ItemId == ItemId && b.ItemGroupId == ItemGroupId).OrderByDescending(c => c.TransId).Take(1).ToList();
            Totalrecv = objmms.tblReceiveDatas.Where(b => b.ProjectId == ProId && b.ItemId == ItemId && b.ItemGroupId == ItemGroupId).Sum(x => x.ReceiveQuantity) ?? 0;
            Totalbalancd = objmms.tblReceiveDatas.Where(b => b.ProjectId == ProId && b.ItemId == ItemId && b.ItemGroupId == ItemGroupId).Sum(x => x.BalanceQuantity) ?? 0;
            if (IRate.Count() > 0)
            {
                var IR1 = objmms.tblReceiveDatas.Where(b => b.ProjectId == ProId && b.ItemId == ItemId && b.ItemGroupId == ItemGroupId).OrderByDescending(c => c.TransId).Take(1).ToList().Select(d => d.Rate).First();
                IR = Convert.ToDecimal(IR1);
            }
            else
            {
                IR = 0;
            }

            var List = new { UniteCode = UniteCode, HhnCode = HSNCode, Gietm = GIETM, Stockqty = Totalbalancd, BalQty = Totalbalancd, DiscountedRate = IR };


            return Json(List.ToJSON(), JsonRequestBehavior.AllowGet);
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


        public ActionResult AddInterStateTransfer(InterStateTransfer Grid)
        {
            List<tblInterStateTransferChild> T = new List<tblInterStateTransferChild>();
            decimal MaxTransId = GetMaxID();
            if (MaxTransId != 0)
            {
                MSP_Model objmsp = new MSP_Model();
                int? divisionId = Repos.GetUserDivision();
                var sessioncode = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.TransferDate));
                var last_IS = objmms.tblInterStateTransferMasters.Where(s => s.ProjectId == Grid.Master.ProjectId && s.Session_Year == sessioncode && s.DivisionId == divisionId).OrderByDescending(s=>s.TransId).FirstOrDefault();

                if (last_IS != null)
                {
                    if (Grid.Master.TransferDate < last_IS.TransferDate)
                        return Json(new { status = 0, last_tran = last_IS.TransferDate.Value.ToString("dd/MM/yyyy") });
                }

                using (var transaction = objmms.Database.BeginTransaction())
                {
                    try
                    {
                        Grid.Master.InterTransferNumber = Get_TransferNumber(Grid.Master.ProjectId, "IS", Grid.Master.TransferDate.Value);

                       tblInterStateTransferMaster tistM = new tblInterStateTransferMaster();
                        tistM.InterTransferNumber = Grid.Master.InterTransferNumber;
                        tistM.ProjectId = Grid.Master.ProjectId;
                        tistM.TransferProjectId = Grid.Master.TransferProjectId;
                        tistM.TransferDate = Grid.Master.TransferDate;
                        tistM.ModeofTPT = Grid.Master.ModeofTPT;
                        tistM.VehicleType = Grid.Master.VehicleType;
                        tistM.VehicleNo = Grid.Master.VehicleNo;
                        tistM.GRNumber = Grid.Master.GRNumber;
                        tistM.Remarks = Grid.Master.Remarks;
                        tistM.ForwardToPIC = Grid.Master.ForwardToPIC;
                        tistM.CreatedDate = System.DateTime.Now;
                        tistM.CreatedBy = EmpId;
                        tistM.Status = "Forward";
                        tistM.Session_Year = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.TransferDate));
                        tistM.IsDeleted = Convert.ToBoolean(0);
                        tistM.ReachingDateofDestination = Convert.ToDateTime(Grid.Master.ReachingDateofDestination);
                        tistM.DivisionId = Repos.GetUserDivision();
                        objmms.tblInterStateTransferMasters.Add(tistM);


                        if (Grid.Child != null && MaxTransId != 0)
                        {
                            foreach (var x in Grid.Child)
                            {
                                var stockDetails = ObjP.GetTotalAmountDetails(Grid.Master.ProjectId, x.ItemId, x.DeliveryQty);

                                if (stockDetails != null && stockDetails.Count > 0)
                                {
                                    foreach (var singleStockDetail in stockDetails)
                                    {
                                        tblInterStateTransferChild tistC = new tblInterStateTransferChild();
                                        tistC.InterStateTransferMasterId = MaxTransId;
                                        tistC.InterStateTransferNumber = Grid.Master.InterTransferNumber;
                                        tistC.ProjectId = Grid.Master.ProjectId;
                                        tistC.ItemGroupId = x.ItemGroupId;
                                        tistC.ItemId = x.ItemId;
                                        tistC.StockQty = singleStockDetail.StockBalQty;
                                        tistC.DeliveryQty = singleStockDetail.Qty;
                                        tistC.BalancedQty = singleStockDetail.StockBalQty - singleStockDetail.Qty;
                                        tistC.DiscountedRate = singleStockDetail.Rate;
                                        tistC.DeliveryRate = singleStockDetail.Rate;
                                        tistC.EstimatedValue = singleStockDetail.Qty * singleStockDetail.Rate;
                                        T.Add(tistC);
                                    }
                                }
                            }

                            objmms.tblInterStateTransferChilds.AddRange(T);
                        }


                        objmms.SaveChanges();
                        transaction.Commit();
                        return Json(new { Status = 1, TransNo = Grid.Master.InterTransferNumber }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json("-1", JsonRequestBehavior.AllowGet);
                    }
                }

            }
            else {
                return Json("2", JsonRequestBehavior.AllowGet);
            }

        }

        public decimal GetMaxID()
        {
            // int ID = Convert.ToDecimal(objmms.tblInterStateTransferMasters.ToList().Select( x => x.TransId).DefaultIfEmpty(0).Max(i=> Convert.ToInt32(i)).Value);
            //objmms.GateEntries.Where(x => x.ProjectNo == ProjectId && x.SessionId == SessionId).ToList().Select(p => p.GateEntryId).DefaultIfEmpty(0).Max(i => i).Value;
            decimal? ID = objmms.tblInterStateTransferMasters.ToList().Max(u => (decimal?)u.TransId);

            if (ID == 0 || ID == null)
            {
                return 1;
            }
            else {
                return (decimal)ID + 1;
            }

        }

        public decimal GetIntraMaxID()
        {
            // int ID = Convert.ToDecimal(objmms.tblInterStateTransferMasters.ToList().Select( x => x.TransId).DefaultIfEmpty(0).Max(i=> Convert.ToInt32(i)).Value);
            //objmms.GateEntries.Where(x => x.ProjectNo == ProjectId && x.SessionId == SessionId).ToList().Select(p => p.GateEntryId).DefaultIfEmpty(0).Max(i => i).Value;
            decimal? ID = objmms.tblIntraStateTransferMasters.ToList().Max(u => (decimal?)u.TransId);

            if (ID == 0 || ID == null)
            {
                return 1;
            }
            else {
                return (decimal)ID + 1;
            }

        }

        public decimal GetIntraTaxableMaxId()
        {
            decimal? ID = objmms.tblIntraTransferTaxableMasters.ToList().Max(u => (decimal?)u.TransId);

            if (ID == 0 || ID == null)
            {
                return 1;
            }
            else {
                return (decimal)ID + 1;
            }
        }

        public ActionResult PendingInterTransfer(string ProjectId)
        {
            var Result = ObjP.GetInterStateTransferList(ProjectId);
            if (Result != null)
            {
                return PartialView("_PendingGridInterState", Result);
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }

        public ActionResult UpdatedTransfer(string ProjectId)
        {
            
            var Result = ObjP.GetApprovedInterStateTransferList_BeforGateOut(ProjectId);
            if (Result != null)
            {
                return PartialView("_UpdatedInterTransferGrid", Result);
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }


        public ActionResult InterTransferDetail(decimal TarnsId)
        {
            // InterStateTransfer vm = new InterStateTransfer(); 
            List<InterStateTransferChildViewModel> ch = new List<InterStateTransferChildViewModel>();
            InterStateTransferMasterViewModel vm = new InterStateTransferMasterViewModel();
            tblInterStateTransferMaster objt = objmms.tblInterStateTransferMasters.Where(x => x.TransId == TarnsId).FirstOrDefault();
            vm.ProjectId = objt.ProjectId; vm.TransId = TarnsId;
            vm.TransferProjectId = objt.TransferProjectId;
            vm.ProjectName = objmms.tblProjectMasters.Where(x => x.PRJID == objt.ProjectId).FirstOrDefault().ProjectName;
            vm.TransferProjectName = objmms.tblProjectMasters.Where(x => x.PRJID == objt.TransferProjectId).FirstOrDefault().ProjectName;
            vm.InterTransferNumber = objt.InterTransferNumber;
            vm.TransporterModeName = objmms.tblTransportationModeNews.Where(x => x.TransId == objt.ModeofTPT).FirstOrDefault().Mode_of_TPT;
            vm.VehicleOwnerTypeName = objmms.tblVehicleTypes.Where(x => x.TransId == objt.VehicleType).FirstOrDefault().Vehicle_Type;
            vm.TransferDate = objt.TransferDate;
            vm.ForwardToPIC = objt.ForwardToPIC;
            vm.Status = objt.Status;
            vm.ReachingDateofDestination = objt.ReachingDateofDestination.ToString();
            ViewBag.EmpType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList().Count() > 0 ? objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).FirstOrDefault().TypeName : "Emp";


            ViewBag.InterTransferDate = objt.TransferDate;


            List<tblInterStateTransferChild> objc = objmms.tblInterStateTransferChilds.Where(x => x.InterStateTransferMasterId == TarnsId && x.InterStateTransferNumber == objt.InterTransferNumber).ToList();

            foreach (var item in objc)
            {
                InterStateTransferChildViewModel chk = new InterStateTransferChildViewModel();

                chk.ProjectId = item.ProjectId; chk.InterStateTransferMasterId = item.InterStateTransferMasterId; chk.InterStateTransferNumber = item.InterStateTransferNumber; chk.ItemGroupId = item.ItemGroupId; chk.ItemId = item.ItemId; chk.StockQty = item.StockQty;
                chk.DeliveryQty = item.DeliveryQty; chk.DeliveryRate = item.DeliveryRate; chk.BalancedQty = item.BalancedQty; chk.DiscountedRate = item.DiscountedRate;
                chk.EstimatedValue = item.EstimatedValue;
                chk.ItemGroupName = objmms.tblItemGroupMasters.Where(x => x.ItemGroupID == item.ItemGroupId).FirstOrDefault().GroupName;
                chk.ItemName = objmms.tblItemMasters.Where(x => x.ItemID == item.ItemId).FirstOrDefault().ItemName;
                chk.HSNCode = objmms.tblItemMasters.Where(x => x.ItemID == item.ItemId).FirstOrDefault().HSNCode;
                chk.GIETMCode = objmms.tblItemMasters.Where(x => x.ItemID == item.ItemId).FirstOrDefault().GITEMCode;
                string UniteId = objmms.tblItemMasters.Where(x => x.ItemID == item.ItemId).FirstOrDefault().UnitID;
                chk.UnitCode = objmms.tblUnitMasters.Where(x => x.UnitID == UniteId).FirstOrDefault().UnitCode;
                chk.TransId = item.TransId;
                ch.Add(chk);

            }

            var data = new InterStateTransfer()
            {
                Master = vm,
                Child = ch
            };



            return View(data);
        }

        public JsonResult BindStore(string TransferNo)
        {
            string CreatedBy = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNo).FirstOrDefault().CreatedBy;
            var Elist = objmms.tblEmployeeMasters.Where(x => x.EmpID == CreatedBy).Select(p => new { Value = p.EmpID, Text = p.FName + " " + p.LName }).ToList();
            return Json(Elist.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateTransfer(InterStateTransfer Grid)
        {

            tblInterStateTransferMaster M = objmms.tblInterStateTransferMasters.Where(x => x.TransId == Grid.Master.TransId).FirstOrDefault();

            if (Grid != null)
            {
                if (Grid.Child.Count() > 0)
                {

                    M.ForwardToStore = Grid.Master.ForwardToStore;
                    M.PICApprovalDate = System.DateTime.Now;
                    M.PICRemarks = Grid.Master.PICRemarks;
                    M.PICApprovalStatus = Grid.Master.PICApprovalStatus;
                    M.Status = Grid.Master.PICApprovalStatus;


                    foreach (var Chld in Grid.Child)
                    {
                        tblInterStateTransferChild ich = objmms.tblInterStateTransferChilds.Where(x => x.TransId == Chld.TransId).FirstOrDefault();

                        ich.PICApprovalQty = Chld.PICApprovalQty;
                        ich.PICApprovalRate = Chld.PICApprovalRate;
                        ich.PICBalancedQty = Chld.PICBalancedQty;
                        ich.PICEstimatedValue = Chld.PICEstimatedValue;


                        objmms.Entry(M).State = EntityState.Modified;
                        objmms.Entry(ich).State = EntityState.Modified;


                    }

                    objmms.SaveChanges();
                    return Json("1", JsonRequestBehavior.AllowGet);
                }

            }

            return View();
        }

        public ActionResult AddIntraStateTrasferDocument(IntraStateTransfer Grid)
        {
            List<tblIntraStateTransferChild> T = new List<tblIntraStateTransferChild>();
            decimal MaxTransId = GetIntraMaxID();
            if (MaxTransId != 0)
            {
                MSP_Model objmsp = new MSP_Model();
                int? divisionId = Repos.GetUserDivision();
                var sessioncode = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.TransferDate));
                var last_IS = objmms.tblIntraStateTransferMasters.Where(s => s.ProjectId == Grid.Master.ProjectId && s.Session_Year == sessioncode && s.DivisionId == divisionId).OrderByDescending(s => s.TransId).FirstOrDefault();

                if (last_IS != null)
                {
                    if (Grid.Master.TransferDate < last_IS.TransferDate)
                        return Json(new { status = 0, last_tran = last_IS.TransferDate.Value.ToString("dd/MM/yyyy") });
                }

                using (var transaction=objmms.Database.BeginTransaction())
                {
                    try
                    {
                        Grid.Master.IntraTransferNumber = Get_TransferNumber(Grid.Master.ProjectId, "OS", Grid.Master.TransferDate.Value);
                        tblIntraStateTransferMaster tistM = new tblIntraStateTransferMaster();
                        tistM.IntraTransferNumber = Grid.Master.IntraTransferNumber;
                        tistM.ProjectId = Grid.Master.ProjectId;
                        tistM.TransferProjectId = Grid.Master.TransferProjectId;
                        tistM.TransferDate = Grid.Master.TransferDate;
                        tistM.ModeofTPT = Grid.Master.ModeofTPT;
                        tistM.VehicleType = Grid.Master.VehicleType;
                        tistM.VehicleNo = Grid.Master.VehicleNo;
                        tistM.E_WayBillNo = Grid.Master.E_WayBillNo;
                        tistM.TaxPayableOnReverseCharges = Grid.Master.TaxPayableOnReverseCharges;
                        tistM.Remarks = Grid.Master.Remarks;
                        tistM.CreatedDate = System.DateTime.Now;
                        tistM.CreatedBy = EmpId;
                        tistM.Session_Year = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.TransferDate));
                        tistM.IsDeleted = Convert.ToBoolean(0);
                        tistM.DivisionId = Repos.GetUserDivision();


                        objmms.tblIntraStateTransferMasters.Add(tistM);


                        if (Grid.Child != null && MaxTransId != 0)
                        {
                            foreach (var x in Grid.Child)
                            {

                                var stockDetails = ObjP.GetTotalAmountDetails(Grid.Master.ProjectId, x.ItemId, x.DeliveryQty);
                                if (stockDetails != null && stockDetails.Count > 0)
                                {
                                    foreach (var singleStockDetail in stockDetails)
                                    {
                                        tblIntraStateTransferChild tistC = new tblIntraStateTransferChild();
                                        tistC.IntraStateTransferMasterId = MaxTransId;
                                        tistC.IntraStateTransferNumber = Grid.Master.IntraTransferNumber;
                                        tistC.ProjectId = Grid.Master.ProjectId;
                                        tistC.ItemGroupId = x.ItemGroupId;
                                        tistC.ItemId = x.ItemId;
                                        tistC.StockQty = singleStockDetail.StockBalQty;
                                        tistC.DeliveryQty = singleStockDetail.Qty;
                                        tistC.BalancedQty = singleStockDetail.StockBalQty - singleStockDetail.Qty;
                                        tistC.DiscountedRate = singleStockDetail.Rate;
                                        tistC.DeliveryRate = singleStockDetail.Rate;
                                        tistC.EstimatedValue = singleStockDetail.Qty * singleStockDetail.Rate;
                                        tistC.UnitId = objmms.tblItemMasters.ToList().Where(x1 => x1.ItemID == x.ItemId).ToList().FirstOrDefault().UnitID;
                                        T.Add(tistC);
                                    }
                                }
                            }

                            objmms.tblIntraStateTransferChilds.AddRange(T);
                        }

                        TempData["TransferNumber"] = Grid.Master.IntraTransferNumber;
                        objmms.SaveChanges();
                        transaction.Commit();
                        return Json(new { Status = 1, TransNo = Grid.Master.IntraTransferNumber }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json("-1", JsonRequestBehavior.AllowGet);
                    }
                }

            }
            else {
                return Json("2", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult IntraStateTrasferDocumentList(string ProjectId)
        {
            var Result = ObjP.GetIntraStateTransferList(ProjectId);
            ViewBag.EmpType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList().Count() > 0 ? objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).FirstOrDefault().TypeName : "Emp";

            if (Result != null)
            {
                return PartialView("_PendingIntraStateTransferGrid", Result);
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }

        public ActionResult CreateTaxableIntraStateDocument(string IntraTransferNumber)
        {

            ViewBag.IntraTransferName = IntraTransferNumber.Replace("#", "/");
            ViewBag.TransferDate = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == IntraTransferNumber.Replace("#", "/")).ToList().FirstOrDefault().TransferDate;
            return View();

        }


        public JsonResult GetDetailOnIntraTaxInvoiceDocument(string IntraTransferNo)
        {
            string ProjId = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == IntraTransferNo).First().ProjectId;
            List<IntraTransferMasterDetail> res = new List<IntraTransferMasterDetail>();
            res = ObjP.GetIntraTransferProjectDetail(ProjId, IntraTransferNo);
            if (res != null)
            {
                return Json(res.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {

                return null;
            }
        }

        public JsonResult BindPICOnly(string projectId)
        {
            List<tblEmployeeMaster> E1 = objmms.tblEmployeeMasters.ToList().Where(x => x.ProjectID.Contains(projectId)).ToList();
            List<tblEmployeeMaster> E = (from a in E1 join b in objmms.tblApproval_AccountType.Where(p => p.TypeName == "PIC") on a.EmpID equals b.EmployeeId select a).ToList();

            var EO = E.Select(a => new { Value = a.EmpID, Text = a.FName + ' ' + a.LName });
            var List = new { Lst = EO };
            return Json(List.ToJSON(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetIntraTransferGrid(string IntraTransferNo, int page = 1)
        {
            const int pageSize = 100; int totalRows = 0;

            List<IntraStateTransferChildViewModel> chld = new List<IntraStateTransferChildViewModel>();

            List<tblIntraStateTransferChild> objc = objmms.tblIntraStateTransferChilds.Where(x => x.IntraStateTransferNumber == IntraTransferNo).ToList();


            totalRows = objc.Count();
            if (objc.Count() != 0)
            {
                foreach (var item in objc)
                {
                    IntraStateTransferChildViewModel S = new IntraStateTransferChildViewModel();
                    S.TransId = item.TransId;
                    S.ItemName = objmms.tblItemMasters.Where(x => x.ItemID == item.ItemId).FirstOrDefault().ItemName;
                    S.ItemId = item.ItemId;
                    S.ItemGroupId = item.ItemGroupId;
                    S.UnitCode = objmms.tblUnitMasters.Where(x => x.UnitID == item.UnitId).FirstOrDefault().UnitCode;
                    S.StockQty = item.StockQty;
                    S.DeliveryQty = item.DeliveryQty;
                    S.DiscountedRate = item.DiscountedRate;
                    S.DeliveryRate = item.DeliveryRate;
                    S.BalancedQty = item.BalancedQty;
                    S.EstimatedValue = item.EstimatedValue;

                    chld.Add(S);


                }
            }





            var TotalCartage = objmms.tblMMSCartageTypes.Where(x => x.Status == true).Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
            ViewBag.Cartage = new SelectList(TotalCartage, "Value", "Text");
            var TotalGST = objmms.tblGST_SplitTaxMaster.Where(s=>s.TaxRateType.Substring(0,4)=="IGST").ToList().OrderBy(p => p.TransId).Select(x => new { Text = x.TaxRateType, Value = x.TaxCode }).ToList();
            ViewBag.GST = new SelectList(TotalGST, "Value", "Text");

            var data = new Grid_IntraStateTransfer()
            {
                TotalRows = totalRows,
                PageSize = pageSize,
                Child = chld.ToList()
            };
            if (Request.IsAjaxRequest())
            {
                return View("_GetTransferDocumentGrid", data);
            }
            return View("_EmptyView");
        }
        [HttpPost]
        public ActionResult AddIntraTransferTaxInvoice(IntraTransferTaxInvoice Grid)
        {
            tblIntraTransferTaxableMaster TTM = new tblIntraTransferTaxableMaster();
            List<tblIntraStateTransferTaxableChild> TTC = new List<tblIntraStateTransferTaxableChild>();

            decimal MaxId = 0;
            try
            {
                MaxId = GetIntraTaxableMaxId();

                if (MaxId != 0)
                {
                    if (Grid.Master != null)
                    {
                        if (Grid.Child.Count() > 0)
                        {

                            var CheckDuplicate = objmms.tblIntraTransferTaxableMasters.ToList().Where(x => x.IntraTransferNumber == Grid.Master.IntraTransferNumber).ToList();
                            if (CheckDuplicate.Count() == 0)
                            {
                                #region

                                TTM.ProjectId = Grid.Master.ProjectId;
                                TTM.IntraTransferNumber = Grid.Master.IntraTransferNumber;
                                TTM.IntraTransferMasterDocumentId = Grid.Master.IntraTransferMasterDocumentId;
                                TTM.TransferDate = Grid.Master.TransferDate;
                                TTM.TransferProjectId = Grid.Master.TransferProjectId;
                                TTM.CreatedBy = EmpId;
                                TTM.CreatedDate = System.DateTime.Now;
                                TTM.SendTo = Grid.Master.SendTo;
                                TTM.Status = "Forward";
                                TTM.Remarks = Grid.Master.Remarks;
                                TTM.PICApprovalId = Grid.Master.PICApprovalId;
                                TTM.Total = Grid.Master.Total;
                                TTM.SubTotal1 = Grid.Master.SubTotal1;
                                TTM.SubTotal2 = Grid.Master.SubTotal2;
                                TTM.GrandTotal = Grid.Master.GrandTotal;
                                TTM.Total_PAndF = Grid.Master.Total_PAndF;
                                TTM.Total_Cartage = Grid.Master.Total_Cartage;
                                TTM.Total_Insurance = Grid.Master.Total_Insurance;
                                TTM.Total_Taxable = Grid.Master.Total_Taxable;
                                TTM.Total_CGST = Grid.Master.Total_CGST;
                                TTM.Total_SGSTAndUTGST = Grid.Master.Total_SGSTAndUTGST;
                                TTM.Total_IGST = Grid.Master.Total_IGST;
                                TTM.Total_NetAmountInWords = Grid.Master.Total_NetAmountInWords;
                                TTM.ReachingDateofDestination = Grid.Master.ReachingDateofDestination;
                                objmms.tblIntraTransferTaxableMasters.Add(TTM);

                                // for child

                                foreach (var item in Grid.Child)
                                {
                                    tblIntraStateTransferTaxableChild VM = new tblIntraStateTransferTaxableChild();

                                    VM.MTransId = MaxId;
                                    VM.ItemId = item.ItemId;
                                    VM.Qty = item.Qty;
                                    VM.Rate = item.Rate;
                                    VM.Discount = item.Discount;
                                    VM.Total = item.Total;
                                    VM.IntraTRansferChildId = item.IntraTRansferChildId;
                                    VM.PackingChargesPercentage = item.PackingChargesPercentage;
                                    VM.PackingChargesAmount = item.PackingChargesAmount;
                                    VM.CartageTypeId = item.CartageTypeId;
                                    VM.Cartage_rate = item.Cartage_rate;
                                    VM.CartageAmount = item.CartageAmount;
                                    VM.SubTotal1 = item.SubTotal1;
                                    VM.InsuranceRate = item.InsuranceRate;
                                    VM.InsuranceAmount = item.InsuranceAmount;
                                    VM.SubTotal2 = item.SubTotal2;
                                    VM.TaxCode = item.TaxCode;
                                    VM.TaxRateType = item.TaxRateType;
                                    VM.CGSTPercentage = item.CGSTPercentage;
                                    VM.CGSTAmount = item.CGSTAmount;
                                    VM.SGSTPercentage = item.SGSTPercentage;
                                    VM.SGSTAmount = item.SGSTAmount;
                                    VM.IGSTPercentage = item.IGSTPercentage;
                                    VM.IGSTAmount = item.IGSTAmount;
                                    VM.GrossAmount = item.GrossAmount;
                                    VM.Item_Description = item.Item_Description;
                                    VM.NewRate = item.NewRate;
                                    VM.Discounted_Amount = item.Discounted_Amount;

                                    TTC.Add(VM);

                                }

                                objmms.tblIntraStateTransferTaxableChilds.AddRange(TTC);

                                ///  objmms.SaveChanges();

                                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);

                                #endregion
                            }

                            else {
                                return Json(new { Status = "6" }, JsonRequestBehavior.AllowGet);
                            }



                        }

                        else {
                            return Json(new { Status = "3" }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else {
                        return Json(new { Status = "4" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else {

                    return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ex.ToString(); return Json(new { Status = "5", IndentNo = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddNewFormat(string gridData, string Prntgrid)
        {
            tblIntraTransferTaxableMaster TTM = new tblIntraTransferTaxableMaster();
            List<tblIntraStateTransferTaxableChild> TTC = new List<tblIntraStateTransferTaxableChild>();

            decimal MaxId = 0;

            var log = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(gridData);
            var log1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(Prntgrid);

            if (log1 != null)
            {
                MaxId = GetIntraTaxableMaxId();
                if (MaxId != 0)
                {

                    try
                    {
                        if (log != null)
                        {
                            foreach (var ind1 in log1)
                            {
                                if (ind1.Value[6].ToString() == "NaN" || ind1.Value[7].ToString() == "NaN" || ind1.Value[8].ToString() == "" || ind1.Value[8].ToString() == "NaN")
                                {
                                    return Json(new { Status = "8" }, JsonRequestBehavior.AllowGet);
                                }
                                else {


                                    // for master
                                    // check duplicate 
                                    var CheckDuplicate = objmms.tblIntraTransferTaxableMasters.ToList().Where(x => x.IntraTransferNumber == ind1.Value[1].ToString()).ToList();
                                    if (CheckDuplicate.Count() == 0)
                                    {
                                        TTM.ProjectId = ind1.Value[2].ToString();
                                        TTM.IntraTransferNumber = ind1.Value[1].ToString();
                                        TTM.IntraTransferMasterDocumentId = Convert.ToDecimal(ind1.Value[4].ToString());
                                        TTM.TransferDate = Convert.ToDateTime(ind1.Value[0].ToString());
                                        TTM.TransferProjectId = ind1.Value[3].ToString();
                                        TTM.CreatedBy = EmpId;
                                        TTM.CreatedDate = System.DateTime.Now;
                                        TTM.SendTo = ind1.Value[9].ToString();
                                        TTM.Status = "Forward";
                                        TTM.Remarks = ind1.Value[11].ToString();
                                        TTM.PICApprovalId = ind1.Value[10].ToString();
                                        TTM.Total = Convert.ToDecimal(ind1.Value[5].ToString());
                                        TTM.SubTotal1 = Convert.ToDecimal(ind1.Value[6].ToString());
                                        TTM.SubTotal2 = Convert.ToDecimal(ind1.Value[7].ToString());
                                        TTM.GrandTotal = Convert.ToDecimal(ind1.Value[8].ToString());
                                        TTM.Total_PAndF = Convert.ToDecimal(ind1.Value[12].ToString());
                                        TTM.Total_Cartage = Convert.ToDecimal(ind1.Value[13].ToString());
                                        TTM.Total_Insurance = Convert.ToDecimal(ind1.Value[14].ToString());
                                        TTM.Total_Taxable = Convert.ToDecimal(ind1.Value[15].ToString());
                                        TTM.Total_CGST = Convert.ToDecimal(ind1.Value[16].ToString());
                                        TTM.Total_SGSTAndUTGST = Convert.ToDecimal(ind1.Value[17].ToString());
                                        TTM.Total_IGST = Convert.ToDecimal(ind1.Value[18].ToString());
                                        TTM.Total_NetAmountInWords = ind1.Value[19].ToString();
                                        TTM.ReachingDateofDestination = Convert.ToDateTime(ind1.Value[20].ToString());
                                        TTM.DivisionId = Repos.GetUserDivision();
                                        objmms.tblIntraTransferTaxableMasters.Add(TTM);
                                    }
                                    else {
                                        return Json(new { Status = "6" }, JsonRequestBehavior.AllowGet);
                                    }

                                }
                            }

                            // for child

                            foreach (var ind in log)
                            {
                                tblIntraStateTransferTaxableChild VM = new tblIntraStateTransferTaxableChild();

                                if (ind.Value[2].ToString() == "" || ind.Value[24].ToString() == "" || ind.Value[25].ToString() == "" || ind.Value[26].ToString() == "" || ind.Value[21].ToString() == "")
                                {
                                    return Json(new { Status = "8" }, JsonRequestBehavior.AllowGet);
                                }
                                else {




                                    VM.MTransId = MaxId;
                                    VM.ItemId = ind.Value[22].ToString();
                                    VM.Qty = Convert.ToDecimal(ind.Value[1].ToString());
                                    VM.Rate = Convert.ToDecimal(ind.Value[2].ToString());
                                    VM.Discount = ind.Value[4].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[4].ToString());
                                    VM.Total = Convert.ToDecimal(ind.Value[24].ToString());
                                    VM.IntraTRansferChildId = Convert.ToDecimal(ind.Value[25].ToString());
                                    VM.PackingChargesPercentage = ind.Value[5].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[5].ToString());
                                    VM.PackingChargesAmount = ind.Value[6].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[6].ToString());
                                    if (ind.Value[7].ToString() == "")
                                    { }
                                    else
                                    {
                                        VM.CartageTypeId = Convert.ToInt32(ind.Value[7].ToString());
                                        VM.Cartage_rate = ind.Value[8].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[8].ToString());

                                    }
                                    VM.CartageAmount = ind.Value[9].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[9].ToString());
                                    VM.SubTotal1 = Convert.ToDecimal(ind.Value[25].ToString());
                                    VM.InsuranceRate = ind.Value[10] == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[10].ToString());
                                    VM.InsuranceAmount = ind.Value[11].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[11].ToString());
                                    VM.SubTotal2 = Convert.ToDecimal(ind.Value[26].ToString());
                                    VM.TaxCode = ind.Value[12].ToString() == "" ? null : ind.Value[12].ToString();
                                    VM.TaxRateType = ind.Value[13].ToString();
                                    VM.CGSTPercentage = ind.Value[14].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[14].ToString());
                                    VM.CGSTAmount = ind.Value[15].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[15].ToString());
                                    VM.SGSTPercentage = ind.Value[16].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[16].ToString());
                                    VM.SGSTAmount = ind.Value[17].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[17].ToString());
                                    VM.IGSTPercentage = ind.Value[18].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[18].ToString());
                                    VM.IGSTAmount = ind.Value[19].ToString() == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(ind.Value[19].ToString());
                                    VM.GrossAmount = Convert.ToDecimal(ind.Value[21].ToString());
                                    VM.Item_Description = ind.Value[0].ToString() == "" ? null : ind.Value[0].ToString();
                                    VM.NewRate = Convert.ToDecimal(ind.Value[3].ToString());
                                    VM.Discounted_Amount = null;
                                    VM.TotalGSTAmount = Convert.ToDecimal(ind.Value[20].ToString());

                                    TTC.Add(VM);


                                }


                            }
                            objmms.tblIntraStateTransferTaxableChilds.AddRange(TTC);
                            if (TTC.ToList().Count() > 0)
                            {
                                objmms.SaveChanges();
                                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
                            }
                            else {
                                return Json(new { Status = "7" }, JsonRequestBehavior.AllowGet);

                            }



                        }
                        else
                        {
                            return Json(new { Status = "3" }, JsonRequestBehavior.AllowGet);
                        }


                    }
                    catch (Exception ex)
                    {
                        ex.ToString(); return Json(new { Status = "5", IndentNo = "" }, JsonRequestBehavior.AllowGet);
                    }



                }
                else {
                    return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);
                }
            }
            else {
                return Json(new { Status = "4" }, JsonRequestBehavior.AllowGet);
            }






        }

        public ActionResult IntraTransferTaxableList()
        {
            ViewBag.EmpId = EmpId;
            return View();
        }


        public ActionResult IntraStateTransferTaxableList(string ProjectId)
        {
            var Result = ObjP.GetIntraStateTransferTaxableList(ProjectId);
            ViewBag.EmpType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList().Count() > 0 ? objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).FirstOrDefault().TypeName : "Emp";
            if (Result != null)
            {
                return PartialView("_PendingIntraStateTRansferTaxableGrid", Result);
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }


        public ActionResult UpdateIntraStateTransferTaxableList(string ProjectId)
        {
            var Result = ObjP.GetUpdateIntraStateTransferTaxableList(ProjectId);

            if (Result != null)
            {
                return PartialView("_GetUpdatedIntraStateTaxableList", Result);
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }


        public ActionResult IntraStateTransferTaxableApproval(string TransferNo)
        {
            string TransferNumber = TransferNo.Replace("#", "/");
            GetDetailOnPrint dataObj = new GetDetailOnPrint();
            string ProjId = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == TransferNumber).First().ProjectId;
            decimal MId = objmms.tblIntraTransferTaxableMasters.Where(x => x.IntraTransferNumber == TransferNumber).FirstOrDefault().TransId;
            List<IntraTransferMasterDetail> res = new List<IntraTransferMasterDetail>(); List<IntraStateTransferTaxableChildViewModel> Ches = new List<IntraStateTransferTaxableChildViewModel>();
            res = ObjP.GetIntraTransferProjectDetail(ProjId, TransferNumber);
            Ches = ObjP.GetIntraStateTransferTaxableChildList(MId);

            dataObj.Header = res.FirstOrDefault();
            dataObj.ItemData = Ches.ToList();


            var detailsForSenderRegisteredAddress = objmms.tblBillingAddresses.SingleOrDefault(x => x.ProGSTIN == dataObj.Header.ProjectGSTIN);
            ViewBag.SenderRegisteredOfficeAddress = detailsForSenderRegisteredAddress.Address;
            ViewBag.SenderRegisteredOfficeState = detailsForSenderRegisteredAddress.State;
            ViewBag.SenderRegisteredOfficeCity = detailsForSenderRegisteredAddress.City;
            ViewBag.SenderRegisteredOfficePin = detailsForSenderRegisteredAddress.Pincode;
            ViewBag.SenderStateCode = detailsForSenderRegisteredAddress.StateCode;

            var detailsForReceiverRegisteredAddress = objmms.tblBillingAddresses.SingleOrDefault(x => x.ProGSTIN == dataObj.Header.TarnsferProjectGSTIN);
            ViewBag.ReceiverRegisteredOfficeAddress = detailsForReceiverRegisteredAddress.Address;
            ViewBag.ReceiverRegisteredOfficeState = detailsForReceiverRegisteredAddress.State;
            ViewBag.ReceiverRegisteredOfficeCity = detailsForReceiverRegisteredAddress.City;
            ViewBag.ReceiverRegisteredOfficePin = detailsForReceiverRegisteredAddress.Pincode;


            ViewBag.StateCode = detailsForSenderRegisteredAddress.StateCode;

            return PartialView("_PendingIntraStateTransferTaxable_ForApproval", dataObj);
        }

        public JsonResult ApprovalStatus()
        {
            var List = objmms.tblApprovalStatus.ToList().Select(x => new { Value = x.TransId, Text = x.ApprovalStatus }).ToList();
            return Json(List.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult addPICApproval(IntraTransferTaxableMasterViewModel Mod)
        {
            try
            {
                tblIntraTransferTaxableMaster tx = objmms.tblIntraTransferTaxableMasters.Where(x => x.IntraTransferNumber == Mod.IntraTransferNumber && x.TransId == Mod.TransId).FirstOrDefault();

                tx.PICApprovalDate = System.DateTime.Now;
                tx.PICApprovalStatus = Mod.PICApprovalStatus;
                tx.Status = Mod.PICApprovalStatus;
                tx.PICReamarks = Mod.PICReamarks;
                objmms.Entry(tx).State = EntityState.Modified;
                objmms.SaveChanges();
                return Json(new { Status = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3, Error = ex.Message });
            }
        }

        public ActionResult InterTransfer_Print(string TransferNo)
        {
            string TransferNumber = TransferNo.Replace("#", "/");
            GetInterDetailOnPrint dataObj = new GetInterDetailOnPrint();
            string ProjId = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).First().ProjectId;
            decimal MId = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).FirstOrDefault().TransId;
            List<InterStateTransferMasterDetailOnGetOut> res = new List<InterStateTransferMasterDetailOnGetOut>(); List<InterStateTransferChildPrintViewModel> Ches = new List<InterStateTransferChildPrintViewModel>();
            res = ObjP.GetInterTransferProjectDetail(ProjId, TransferNumber);
            Ches = ObjP.Get_InterItemDetailOnPrint(MId);
            dataObj.Header = res.FirstOrDefault();
            dataObj.ItemData = Ches.ToList();
            ViewBag.ProjectBillAddress = objmms.tblBillingAddresses.Where(s => s.State == dataObj.Header.ProjectState).FirstOrDefault();
            ViewBag.TransferStateCode = objmms.tblBillingAddresses.Where(s => s.State == dataObj.Header.TransferProjectState).Select(s => s.StateCode).FirstOrDefault();
            return PartialView("_Pending_InterState_Transfer_Print", dataObj);
        }
        public ActionResult IntraTransfer_Print(string TransferNo)
        {
            string TransferNumber = TransferNo.Replace("#", "/");
            GetDetailOnPrint dataObj = new GetDetailOnPrint();
            string ProjId = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == TransferNumber).First().ProjectId;
            decimal MId = objmms.tblIntraTransferTaxableMasters.Where(x => x.IntraTransferNumber == TransferNumber).FirstOrDefault().TransId;
            List<IntraTransferMasterDetail> res = new List<IntraTransferMasterDetail>(); List<IntraStateTransferTaxableChildViewModel> Ches = new List<IntraStateTransferTaxableChildViewModel>();
            res = ObjP.GetIntraTransferProjectDetail(ProjId, TransferNumber);
            Ches = ObjP.GetIntraStateTransferTaxableChildList(MId);

            dataObj.Header = res.FirstOrDefault();

            var detailsForSenderRegisteredAddress = objmms.tblBillingAddresses.SingleOrDefault(x => x.ProGSTIN == dataObj.Header.ProjectGSTIN);
            ViewBag.SenderRegisteredOfficeAddress = detailsForSenderRegisteredAddress.Address;
            ViewBag.SenderRegisteredOfficeState = detailsForSenderRegisteredAddress.State;
            ViewBag.SenderRegisteredOfficeCity = detailsForSenderRegisteredAddress.City;
            ViewBag.SenderRegisteredOfficePin = detailsForSenderRegisteredAddress.Pincode;
            ViewBag.SenderStateCode = detailsForSenderRegisteredAddress.StateCode;

            var detailsForReceiverRegisteredAddress = objmms.tblBillingAddresses.SingleOrDefault(x => x.ProGSTIN == dataObj.Header.TarnsferProjectGSTIN);
            ViewBag.ReceiverRegisteredOfficeAddress = detailsForReceiverRegisteredAddress.Address;
            ViewBag.ReceiverRegisteredOfficeState = detailsForReceiverRegisteredAddress.State;
            ViewBag.ReceiverRegisteredOfficeCity = detailsForReceiverRegisteredAddress.City;
            ViewBag.ReceiverRegisteredOfficePin = detailsForReceiverRegisteredAddress.Pincode;


            ViewBag.StateCode = detailsForReceiverRegisteredAddress.StateCode;


            dataObj.ItemData = Ches.ToList();




            return PartialView("_Pending_IntraTransfer_Print", dataObj);
        }


        #region Get Transfer Number code on 3_07_2020

        public string Get_TransferNumber(string ProjectID, string TransferType, DateTime TransferDate)
        {
            string J = string.Empty;
            J = ObjP.GetInterStateTransferCode(ProjectID, TransferType, TransferDate);
            string L = J ?? "N/A";
            return L;
        }

        #endregion


        #region Transfer E-wayBil
        public ActionResult TransferEwayBill()
        {
            ViewBag.EmpId = EmpId;
            return View();
        }

        public ActionResult TransferEWayBillReport()
        {
            ViewBag.EmpId = EmpId;
            return View();
        }

        public ActionResult RejectTransfer()
        {
            ViewBag.EmpId = EmpId;
            return View();
        }

        public ActionResult RejectedTransfer()
        {
            ViewBag.EmpId = EmpId;
            return View();
        }

        public ActionResult PendingTransferEwayBill(string ProjectID, string TransferType, DateTime? TransferDate)
        {
            ViewBag.TransferType = TransferType;
            ViewBag.Status = "Pending";
            List<TransferEWayBill> transferData = new List<TransferEWayBill>();
            if (TransferType == "InterTransfer")
            {
                transferData = objmms.tblInterStateTransferMasters.Where(s => string.IsNullOrEmpty(s.E_WayBillNo) && s.ProjectId == ProjectID && (s.TransferDate == TransferDate || TransferDate == null)).Select(
                s => new TransferEWayBill()
                {
                    TransferNumber = s.InterTransferNumber,
                    TransferDate = s.TransferDate,
                    Amount = objmms.tblInterStateTransferChilds.Where(t => t.InterStateTransferMasterId == s.TransId).Sum(t => t.EstimatedValue),
                    ProjectName = objmms.tblProjectMasters.Where(p => p.PRJID == s.ProjectId).Select(p => p.ProjectName).FirstOrDefault()
                }
                ).ToList();
                transferData = transferData.Where(s => s.Amount >= 50000).Select(s => s).ToList();
            }

            if (TransferType == "IntraTransfer")
            {
                transferData = objmms.tblIntraStateTransferMasters.Where(s => string.IsNullOrEmpty(s.E_WayBillNo) && s.ProjectId == ProjectID && (s.TransferDate == TransferDate || TransferDate == null)).Select(
                s => new TransferEWayBill()
                {
                    TransferNumber = s.IntraTransferNumber,
                    TransferDate = s.TransferDate,
                    Amount = objmms.tblIntraStateTransferChilds.Where(t => t.IntraStateTransferMasterId == s.TransId).Sum(t => t.EstimatedValue),
                    ProjectName = objmms.tblProjectMasters.Where(p => p.PRJID == s.ProjectId).Select(p => p.ProjectName).FirstOrDefault()
                }
                ).ToList();
                transferData = transferData.Where(s => s.Amount >= 50000).Select(s => s).ToList();
            }
            return PartialView("_PartialPendingEwayBill", transferData);
        }

        public ActionResult UpdatedTransferEwayBill(string ProjectID, string TransferType, DateTime? TransferDate)
        {
            List<TransferEWayBill> transferData = new List<TransferEWayBill>();
            ViewBag.TransferType = TransferType;
            if (TransferType == "InterTransfer")
            {
                transferData = objmms.tblInterStateTransferMasters.Where(s => !string.IsNullOrEmpty(s.E_WayBillNo) 
                && s.ProjectId == ProjectID && (s.TransferDate == TransferDate || 
                TransferDate == null)).Select(
                   s => new TransferEWayBill()
                   {
                       TransferNumber = s.InterTransferNumber,
                       TransferDate = s.TransferDate,
                       EWayBill = s.E_WayBillNo,
                       EWayBillDate = s.E_WayBill_Date,
                       Amount = objmms.tblInterStateTransferChilds.Where(t => t.InterStateTransferMasterId == s.TransId).Sum(t => t.EstimatedValue),
                       ProjectName = objmms.tblProjectMasters.Where(p => p.PRJID == s.ProjectId).Select(p => p.ProjectName).FirstOrDefault()
                   }
                   ).ToList();
            }

            if (TransferType == "IntraTransfer")
            {
                transferData = objmms.tblIntraStateTransferMasters.Where(s => !string.IsNullOrEmpty(s.E_WayBillNo) && 
                s.ProjectId == ProjectID && (s.TransferDate == TransferDate || 
                TransferDate == null)).Select(
              s => new TransferEWayBill()
              {
                  TransferNumber = s.IntraTransferNumber,
                  TransferDate = s.TransferDate,
                  EWayBill = s.E_WayBillNo,
                  EWayBillDate = s.E_WayBill_Date,
                  Amount = objmms.tblIntraStateTransferChilds.Where(t => t.IntraStateTransferMasterId == s.TransId).Sum(t => t.EstimatedValue),
                  ProjectName = objmms.tblProjectMasters.Where(p => p.PRJID == s.ProjectId).Select(p => p.ProjectName).FirstOrDefault()
              }
              ).ToList();
            }

                return PartialView("_PartialUpdatedEwayBill", transferData);
        }


        [HttpPost]
        public ActionResult SaveTransferEWayBill(List<TransferEWayBill> TransferData, string TransferType)
        {
            int res = 0;
            if (TransferType == "InterTransfer")
            {
                foreach (var interTran in TransferData)
                {
                    var TransferObj = objmms.tblInterStateTransferMasters.Where(s => s.InterTransferNumber == interTran.TransferNumber).FirstOrDefault();
                    TransferObj.E_WayBillNo = interTran.EWayBill;
                    TransferObj.E_WayBill_Date = interTran.EWayBillDate;
                    objmms.Entry(TransferObj).State = EntityState.Modified;

                }
                res= objmms.SaveChanges();
            }

            if (TransferType == "IntraTransfer")
            {
                foreach (var interTran in TransferData)
                {
                    var TransferObj = objmms.tblIntraStateTransferMasters.Where(s => s.IntraTransferNumber == interTran.TransferNumber).FirstOrDefault();
                    TransferObj.E_WayBillNo = interTran.EWayBill;
                    TransferObj.E_WayBill_Date = interTran.EWayBillDate;
                }
                res=objmms.SaveChanges();
            }

            if (res > 0)
                return Json("Successfully Submitted");
            else
                return Json("Something went wrong.");

        }

        public ActionResult Export_InterTransferItems(string TransferNumber)
        {
            GetInterDetailOnPrint dataObj = new GetInterDetailOnPrint();
            string ProjId = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).First().ProjectId;
            decimal MId = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).FirstOrDefault().TransId;
            List<InterStateTransferChildPrintViewModel> Ches = new List<InterStateTransferChildPrintViewModel>();
            Ches = ObjP.Get_InterItemDetailOnPrint(MId);
            dataObj.ItemData = Ches.ToList();
            
            return PartialView("_Partial_InterState_Transfer_Items", dataObj);
        }

        public ActionResult Export_IntraTransferItems(string TransferNumber)
        {
            GetDetailOnPrint dataObj = new GetDetailOnPrint();
            string ProjId = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == TransferNumber).First().ProjectId;
            decimal MId = objmms.tblIntraTransferTaxableMasters.Where(x => x.IntraTransferNumber == TransferNumber).FirstOrDefault().TransId;
            List<IntraStateTransferTaxableChildViewModel> Ches = new List<IntraStateTransferTaxableChildViewModel>();
            Ches = ObjP.GetIntraStateTransferTaxableChildList(MId);
            dataObj.ItemData = Ches.ToList();

            return PartialView("_Partial_IntraTransfer_Items", dataObj);
        }


        public ActionResult GetTransferWithoutGateOut( string ProjectId, string TransferType)
        {
            ViewBag.TransferType = TransferType;
            if (TransferType == "IntraTransfer")
            {
                var Result = ObjP.GetPendingIntraStateTransferTaxableList(ProjectId);
                if (Result != null)
                {
                    return PartialView("_PartialTransferForReject", Result);
                }
                else
                {
                    return PartialView("_EmptyView");
                }
            }
            else if (TransferType == "InterTransfer")
            {
                var Result1 = ObjP.GetPending_InterStateTransferListONGateOut(ProjectId);
                if (Result1 != null)
                {
                    return PartialView("_PartialInterTransferForReject", Result1);
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


        public ActionResult GetRejectedTransfer(string ProjectID, string TransferType)
        {
            if (TransferType == "IntraTransfer")
            {
                var Result = ObjP.GetRejectedIntraTransfer(ProjectID);
                if (Result != null)
                {
                    return PartialView("_PartialRejectTransferForReject", Result);
                }
                else
                {
                    return PartialView("_EmptyView");
                }
            }
            else if (TransferType == "InterTransfer")
            {
                var Result1 = ObjP.GetRejectedInterTransfer(ProjectID);
                if (Result1 != null)
                {
                    return PartialView("_PartialRejectInterTransferForReject", Result1);
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

        [HttpPost]
        public ActionResult SaveRejectTransfer(string TransferNo, string Remarks, string TransferType)
        {
            int status = 0;
            if (TransferType == "InterTransfer")
            {
                var transfer = objmms.tblInterStateTransferMasters.Where(s => s.InterTransferNumber == TransferNo).FirstOrDefault();
                transfer.Reject_Date = DateTime.Now;
                transfer.Is_Reject = 1;
                transfer.Reject_Remark = Remarks;
                objmms.Entry(transfer).State = EntityState.Modified;
                status=objmms.SaveChanges();
            }
            if (TransferType == "IntraTransfer")
            {
                var transfer = objmms.tblIntraStateTransferMasters.Where(s => s.IntraTransferNumber == TransferNo).FirstOrDefault();
                transfer.Reject_Date = DateTime.Now;
                transfer.Is_Reject = 1;
                transfer.Reject_Remark = Remarks;
                objmms.Entry(transfer).State = EntityState.Modified;
                status=objmms.SaveChanges();
            }

            return Json(status);
        }


        public ActionResult RejectdIntraTransfer_Print(string TransferNo)
        {
            string TransferNumber = TransferNo.Replace("#", "/");
            GetDetailOnPrint dataObj = new GetDetailOnPrint();
            string ProjId = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == TransferNumber).First().ProjectId;
            decimal MId = objmms.tblIntraTransferTaxableMasters.Where(x => x.IntraTransferNumber == TransferNumber).FirstOrDefault().TransId;
            List<IntraTransferMasterDetail> res = new List<IntraTransferMasterDetail>(); List<IntraStateTransferTaxableChildViewModel> Ches = new List<IntraStateTransferTaxableChildViewModel>();
            res = ObjP.GetIntraTransferProjectDetail(ProjId, TransferNumber);
            Ches = ObjP.GetIntraStateTransferTaxableChildList(MId);

            dataObj.Header = res.FirstOrDefault();

            var detailsForSenderRegisteredAddress = objmms.tblBillingAddresses.SingleOrDefault(x => x.ProGSTIN == dataObj.Header.ProjectGSTIN);
            ViewBag.SenderRegisteredOfficeAddress = detailsForSenderRegisteredAddress.Address;
            ViewBag.SenderRegisteredOfficeState = detailsForSenderRegisteredAddress.State;
            ViewBag.SenderRegisteredOfficeCity = detailsForSenderRegisteredAddress.City;
            ViewBag.SenderRegisteredOfficePin = detailsForSenderRegisteredAddress.Pincode;
            ViewBag.SenderStateCode = detailsForSenderRegisteredAddress.StateCode;

            var detailsForReceiverRegisteredAddress = objmms.tblBillingAddresses.SingleOrDefault(x => x.ProGSTIN == dataObj.Header.TarnsferProjectGSTIN);
            ViewBag.ReceiverRegisteredOfficeAddress = detailsForReceiverRegisteredAddress.Address;
            ViewBag.ReceiverRegisteredOfficeState = detailsForReceiverRegisteredAddress.State;
            ViewBag.ReceiverRegisteredOfficeCity = detailsForReceiverRegisteredAddress.City;
            ViewBag.ReceiverRegisteredOfficePin = detailsForReceiverRegisteredAddress.Pincode;


            ViewBag.StateCode = detailsForReceiverRegisteredAddress.StateCode;


            dataObj.ItemData = Ches.ToList();




            return PartialView("_Rejected_IntraTransfer_Print", dataObj);
        }
        public ActionResult RejectdInterTransfer_Print(string TransferNo)
        {
            string TransferNumber = TransferNo.Replace("#", "/");
            GetInterDetailOnPrint dataObj = new GetInterDetailOnPrint();
            string ProjId = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).First().ProjectId;
            decimal MId = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).FirstOrDefault().TransId;
            List<InterStateTransferMasterDetailOnGetOut> res = new List<InterStateTransferMasterDetailOnGetOut>(); List<InterStateTransferChildPrintViewModel> Ches = new List<InterStateTransferChildPrintViewModel>();
            res = ObjP.GetInterTransferProjectDetail(ProjId, TransferNumber);
            Ches = ObjP.Get_InterItemDetailOnPrint(MId);
            dataObj.Header = res.FirstOrDefault();
            dataObj.ItemData = Ches.ToList();
            ViewBag.ProjectBillAddress = objmms.tblBillingAddresses.Where(s => s.State == dataObj.Header.ProjectState).FirstOrDefault();
            ViewBag.TransferStateCode = objmms.tblBillingAddresses.Where(s => s.State == dataObj.Header.TransferProjectState).Select(s => s.StateCode).FirstOrDefault();
            return PartialView("_Rejected_InterState_Transfer_Print", dataObj);
        }


        public ActionResult UpdateEWayBillInterTransfer_Print(string TransferNo)
        {
            string TransferNumber = TransferNo.Replace("#", "/");
            GetInterDetailOnPrint dataObj = new GetInterDetailOnPrint();
            string ProjId = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).First().ProjectId;
            decimal MId = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).FirstOrDefault().TransId;
            List<InterStateTransferMasterDetailOnGetOut> res = new List<InterStateTransferMasterDetailOnGetOut>(); List<InterStateTransferChildPrintViewModel> Ches = new List<InterStateTransferChildPrintViewModel>();
            res = ObjP.GetInterTransferProjectDetail(ProjId, TransferNumber);
            Ches = ObjP.Get_InterItemDetailOnPrint(MId);
            dataObj.Header = res.FirstOrDefault();
            dataObj.ItemData = Ches.ToList();
            ViewBag.ProjectBillAddress = objmms.tblBillingAddresses.Where(s => s.State == dataObj.Header.ProjectState).FirstOrDefault();
            ViewBag.TransferStateCode = objmms.tblBillingAddresses.Where(s => s.State == dataObj.Header.TransferProjectState).Select(s => s.StateCode).FirstOrDefault();
            ViewBag.EWayBillDate= objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).First().E_WayBill_Date;
            return PartialView("_Partial_UpdateEWayBill_InterTransfer", dataObj);
        }
        public ActionResult UpdateEWayBillIntraTransfer_Print(string TransferNo)
        {
            string TransferNumber = TransferNo.Replace("#", "/");
            GetDetailOnPrint dataObj = new GetDetailOnPrint();
            string ProjId = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == TransferNumber).First().ProjectId;
            decimal MId = objmms.tblIntraTransferTaxableMasters.Where(x => x.IntraTransferNumber == TransferNumber).FirstOrDefault().TransId;
            List<IntraTransferMasterDetail> res = new List<IntraTransferMasterDetail>(); List<IntraStateTransferTaxableChildViewModel> Ches = new List<IntraStateTransferTaxableChildViewModel>();
            res = ObjP.GetIntraTransferProjectDetail(ProjId, TransferNumber);
            Ches = ObjP.GetIntraStateTransferTaxableChildList(MId);

            dataObj.Header = res.FirstOrDefault();

            var detailsForSenderRegisteredAddress = objmms.tblBillingAddresses.SingleOrDefault(x => x.ProGSTIN == dataObj.Header.ProjectGSTIN);
            ViewBag.SenderRegisteredOfficeAddress = detailsForSenderRegisteredAddress.Address;
            ViewBag.SenderRegisteredOfficeState = detailsForSenderRegisteredAddress.State;
            ViewBag.SenderRegisteredOfficeCity = detailsForSenderRegisteredAddress.City;
            ViewBag.SenderRegisteredOfficePin = detailsForSenderRegisteredAddress.Pincode;
            ViewBag.SenderStateCode = detailsForSenderRegisteredAddress.StateCode;

            var detailsForReceiverRegisteredAddress = objmms.tblBillingAddresses.SingleOrDefault(x => x.ProGSTIN == dataObj.Header.TarnsferProjectGSTIN);
            ViewBag.ReceiverRegisteredOfficeAddress = detailsForReceiverRegisteredAddress.Address;
            ViewBag.ReceiverRegisteredOfficeState = detailsForReceiverRegisteredAddress.State;
            ViewBag.ReceiverRegisteredOfficeCity = detailsForReceiverRegisteredAddress.City;
            ViewBag.ReceiverRegisteredOfficePin = detailsForReceiverRegisteredAddress.Pincode;


            ViewBag.StateCode = detailsForReceiverRegisteredAddress.StateCode;


            dataObj.ItemData = Ches.ToList();

            DateTime? EwayBillDate = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == TransferNumber).First().E_WayBill_Date;
            ViewBag.EWayBillDate = EwayBillDate;

            return PartialView("_Partial_UpdateEWayBill_IntraTransfer", dataObj);
        }

        #endregion

    }
}