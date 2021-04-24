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
    public class GetOutController : Controller
    {
        // GET: GetOut
        string EmpId = string.Empty;
        private MMSEntities objmms = new MMSEntities();
        Procedure ObjP = new Procedure();

        public GetOutController()
        {
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
        }

        //change25052018
        public ActionResult Index()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.Date = DateTime.Now.Date;
            List<SelectListItem> TransferTpe = new List<SelectListItem>();
            TransferTpe.Add(new SelectListItem() { Text = "Inter Transfer (Other State)", Value = "2" });
            TransferTpe.Add(new SelectListItem() { Text = "Intra Transfer (Within State)", Value = "1" });
            TransferTpe.Add(new SelectListItem() { Text = "Vendor Debit Note", Value = "3" });
            this.ViewBag.TransferTpe = new SelectList(TransferTpe, "Value", "Text");
            return View();
        }

        public ActionResult PendingGetoutList(string ProjectId, string Type)
        {

            if (Type == "2")
            {
                var Result = ObjP.GetPendingIntraStateTransferTaxableList(ProjectId);
                if (Result != null)
                {
                    ViewBag.IntraType = Type;
                    return PartialView("_PendingGetOutGrid", Result);
                }
                else
                {
                    return PartialView("_EmptyView");
                }
            }
            else if (Type == "1")
            {
                var Result1 = ObjP.GetPending_InterStateTransferListONGateOut(ProjectId);
                if (Result1 != null)
                {
                    ViewBag.InterType = Type;
                    return PartialView("_PendingGetOutInterTransferGrid", Result1);
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

        public ActionResult UpdatedGetOutList(string ProjectId, string Type)
        {
            if (Type == "2")
            {
                var Result = ObjP.GetUpdateGetoutIntraStateTransferTaxableList(ProjectId);
                if (Result != null)
                {

                    return PartialView("_UpdatedGetoutIntraTransferList", Result);
                }
                else
                {
                    return PartialView("_EmptyView");
                }
            }
            else if (Type == "1")
            {
                var Result1 = ObjP.GetUpdated_GetoutInterStateTransferList(ProjectId);
                if (Result1 != null)
                {
                    ViewBag.InterType = Type;
                    return PartialView("_UpdatedGetOutInterTransferList", Result1);
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

        public ActionResult GateOut_Print(string TransferNo)
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


            ViewBag.StateCode = detailsForReceiverRegisteredAddress.StateCode;


            return PartialView("_GateOut_IntraPrint", dataObj);
        }

        public ActionResult GateOut_Inter_Print(string TransferNo)
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
            return PartialView("_GetOut_Inter_PrintChalan", dataObj);
        }




        public ActionResult CreateGetOutTransfer(string TransferNo, string TransferType)

        {
            ViewBag.TransferNUMBER = TransferNo.Replace("#", "/");
            if (TransferType == "1")
            {
                ViewBag.Transfer_Date = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNo.Replace("#", "/")).ToList().FirstOrDefault().TransferDate;
            }
            else if (TransferType == "3")
            {
                
                var data = objmms.tblVendorDebitNoteMasters.FirstOrDefault(x => x.VendorDebitNo == TransferNo);
                ViewBag.TransId = data.UId;
                ViewBag.Transfer_Date =Convert.ToDateTime(data.VendorDebitDate);

                var vendorData = objmms.tblVendorMasters.Where(x => x.VendorID == data.SupplierNo).SingleOrDefault();
                var projectStateId = objmms.tblProjectGSTNoes.SingleOrDefault(x => x.ProjectId == data.ProjectNumber).StateId;

                ViewBag.ProjectId = data.ProjectNumber;
                ViewBag.ProjectName = objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == data.ProjectNumber).ProjectName;


                ViewBag.State = objmms.tblStates.FirstOrDefault(x => x.StateID == projectStateId).StateName;
                ViewBag.SiteAddress = objmms.tblProjectParticularsDetailAs.Where(m => m.PRJID == data.ProjectNumber).FirstOrDefault().Location;
                ViewBag.ProjectGSTNo = objmms.tblProjectGSTNoes.SingleOrDefault(x => x.ProjectId == data.ProjectNumber).GSTNo;

                ViewBag.VendorName = vendorData.Name;
                ViewBag.VendorAddress = vendorData.Address;
                ViewBag.VendorGSTNo = vendorData.GST_NO;
                ViewBag.VendorState = objmms.tblStates.FirstOrDefault(x => x.StateID == vendorData.State).StateName;
            }
            else
            {
                ViewBag.Transfer_Date = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == TransferNo.Replace("#", "/")).ToList().FirstOrDefault().TransferDate;
            }
            ViewBag.Transfer_Type = TransferType;
            return View();
        }

        public PartialViewResult GetDebitNoteDetails(decimal TransId)
        {
            //var data = objmms.tblVendorDebitNoteChilds.Where(x => x.MUId == TransId).ToList();
            // var debitNoteData = objmms.tblVendorDebitNoteMasters.FirstOrDefault(x=>x.UId==data[0].MUId);
            List<GetOutItemDetailViewModel> lstItems = ObjP.GetVendorDebitNoteItemsForTransfer(TransId.ToString());
            return PartialView("_GetGridTransferItem", lstItems);
        }



        public ActionResult CreateGetOutInterTransfer(string TransferNo, string TransferType)
        {
            ViewBag.Transfer_NUMBER = TransferNo.Replace("#", "/");
            ViewBag.Transfer_Date = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNo.Replace("#", "/")).ToList().FirstOrDefault().TransferDate;
            ViewBag.Transfer_Type = TransferType;
            return View();
        }


        public JsonResult GetTransferDetailOnGetOutPanel(string TransferNo)
        {
            string ProjId = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == TransferNo).First().ProjectId;
            List<IntraTransferMasterDetail> res = new List<IntraTransferMasterDetail>();
            res = ObjP.GetIntraTransferProjectDetail(ProjId, TransferNo);
            if (res != null)
            {
                return Json(res.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {

                return null;
            }
        }

        public JsonResult GetInterTransferDetailOnGetOutPanel(string TransferNo)
        {
            string ProjId = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNo).First().ProjectId;
            List<InterStateTransferMasterDetailOnGetOut> res = new List<InterStateTransferMasterDetailOnGetOut>();
            res = ObjP.GetInterTransferProjectDetail(ProjId, TransferNo);
            if (res != null)
            {
                return Json(res.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {

                return null;
            }
        }


        public ActionResult GetTransferItemDetailAtGetOut(string TransferNo, string ProjectId)
        {
            var res = ObjP.GetOutTransferItemdetail(TransferNo, ProjectId);
            if (res != null)
            {
                return PartialView("_GetGridTransferItem", res);
            }
            else
            {
                return PartialView("_EmptyView");
            }



        }

        //change25052018
        public ActionResult GetVendorDebitNoteForTransfer(string ProjectId, String Status)
        {
            List<GetVendorDebitNotes> res = null;
            string partialView = "";
            if (Status == "Approved")
            {
                res = ObjP.GetVendorDebitNotesForGate(ProjectId, Status);
                partialView = "_GridViewPendigTransferDebitNoteData";
            }
            else if (Status == "Get Out")
            {
                res = ObjP.GetVendorDebitNotesForGate(ProjectId, Status);
                partialView = "_GridViewTransferedDebitNoteData";
            }
            else
            {
                partialView = "_EmptyView";
            }

            return PartialView(partialView, res);
        }
        //change25052018
        public JsonResult ApproveDebitNoteGateOut(string DebitNo)
        {
            if (!string.IsNullOrEmpty(DebitNo))
            {
                var divisionId = Repos.GetUserDivision();
                var data = objmms.tblVendorDebitNoteMasters.FirstOrDefault(x => x.VendorDebitNo == DebitNo && x.DivisionId==divisionId);
                if (data != null)
                {
                    try
                    {
                        data.Status = "Transfered";
                        //tblGetOutTransfer gateOutData = new tblGetOutTransfer();
                        //gateOutData.TransferNumber = DebitNo;
                        //gateOutData.TransferType = "Debit Note";
                        //gateOutData.CreatedDate = DateTime.Now;
                        //gateOutData.ProjectId = data.ProjectNumber;
                        //objmms.tblGetOutTransfers.Add(gateOutData);


                        objmms.Entry(data).State = EntityState.Modified;
                        objmms.SaveChanges();
                        return Json(new { Status = true }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInterTransferItemDetailAtGetOut(string TransferNo, string ProjectId)
        {
            var res = ObjP.GetOutIntraTransferItemdetail(TransferNo, ProjectId);
            if (res != null)
            {
                return PartialView("_GetGridIntraTransferItemDetail", res);
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }

        public JsonResult GetOutCode(string ProjectID, string GetOutType, string GateOutDate)
        {
            string J = string.Empty;
            J = ObjP.GetoutTransferCode(ProjectID, Convert.ToInt32(GetOutType), Convert.ToDateTime(GateOutDate));
            var L = new { getoutCode = J ?? "N/A" };
            return Json(L.ToJSON(), JsonRequestBehavior.AllowGet);

        }



        public ActionResult SaveGetout(string gridData)
        {
            var log = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(gridData);
            if (log != null)
            {
                foreach (var x in log)
                {
                    if (Convert.ToDecimal(x.Value[9]) > 0)
                    {
                        List<tblGetOutTransfer> GO = new List<tblGetOutTransfer>();
                        decimal RemQty = 0; RemQty = Convert.ToDecimal(x.Value[10]); decimal MyBalQty = Convert.ToDecimal(x.Value[10]);
                        string ProjId = x.Value[12], TransferProjId = x.Value[13], ItemGrpid = x.Value[1], ItmId = x.Value[3], UnitId = x.Value[5];
                        decimal RemIssueQty = Convert.ToDecimal(x.Value[9]);
                        var temp1 = x.Value[14];
                        var list = objmms.tblReceiveDatas.Where(x1 => x1.ProjectId == ProjId && x1.ItemGroupId == ItemGrpid && x1.ItemId == ItmId && x1.BalanceQuantity > 0).OrderBy(p => p.TypeDate).ToList();
                       

                        // iterating loop in stock List
                        foreach (var item in list)
                        {
                            tblGetOutTransfer G = new tblGetOutTransfer();
                            var balqty = item.BalanceQuantity;

                            decimal RecvQty = Convert.ToDecimal(item.BalanceQuantity); decimal a = 0;
                            if (RecvQty >= RemIssueQty && RecvQty > 0)
                            {
                                a = RecvQty - RemIssueQty;
                                decimal iq = RemIssueQty;
                                decimal qt = RecvQty;
                                decimal bq = a;
                                decimal aq = RecvQty;
                                decimal aiq = a;

                                G.ProjectId = x.Value[12];
                                G.TransferProjectId = x.Value[13];
                                G.GetOutNumber = x.Value[11];
                                G.TransferNumber = x.Value[14];
                                G.ItemGroupId = x.Value[1];
                                G.ItemId = x.Value[3];
                                G.UnitId = x.Value[5];
                                G.StockQty = Convert.ToDecimal(x.Value[7]);
                                G.DeliveryQty = iq;
                                G.GetOutQty = iq;
                                G.AvalStockAfterGetOut = aiq;
                                G.GetOutDate = Convert.ToDateTime(x.Value[15]);
                                G.CreatedDate = System.DateTime.Now;
                                G.TransferType = "Intra Transfer";
                                G.TransferChildId = 0;
                                G.ChalanNo = x.Value[16];
                                G.ChalanDate = Convert.ToDateTime(x.Value[17]);
                                G.VehicleNo = x.Value[18];
                                G.EwayBillNo = x.Value[20];
                                G.DivisionId = Repos.GetUserDivision();
                                // kk G.Transfer_Rate = item.Rate;
                                G.Session_Year = GetCurrentFinancialYear(G.GetOutDate.Value);
                                
                                GO.Add(G);
                                // for Updating TblReceiveData
                                decimal TrId = item.TransId;
                                tblReceiveData tdr = objmms.tblReceiveDatas.Where(x1 => x1.ProjectId == ProjId && x1.ItemGroupId == ItemGrpid && x1.ItemId == ItmId && x1.TransId == TrId).First();
                                tdr.BalanceQuantity = qt - iq;
                                item.IssueQuantity = item.IssueQuantity + iq;
                                tdr.ModifiedDate = System.DateTime.Now;
                                //tdr.SenderId = G.ProjectId;

                                objmms.Entry(tdr).State = EntityState.Modified;

                                // updating status
                                var temp = x.Value[14];
                                tblIntraStateTransferMaster intr = objmms.tblIntraStateTransferMasters.Where(x1 => x1.IntraTransferNumber == temp).FirstOrDefault();
                                intr.TransferOutStatus = "Get out";
                                objmms.Entry(intr).State = EntityState.Modified;

                                tblIntraTransferTaxableMaster intrTax = objmms.tblIntraTransferTaxableMasters.Where(x1 => x1.IntraTransferNumber == temp).FirstOrDefault();
                                intrTax.TransferOutStatus = "Get out";
                                objmms.Entry(intrTax).State = EntityState.Modified;



                               
                                //add entry in ns_tblIssueQty table

                                ns_tbl_IssueQuantity issueQty = new ns_tbl_IssueQuantity();
                                issueQty.IndentNo = intr.IntraTransferNumber;
                                issueQty.ProjectID = intr.ProjectId;
                                issueQty.ProjectName = objmms.tblProjectMasters.SingleOrDefault(y => y.PRJID == intr.ProjectId).ProjectName;
                                issueQty.EmployeeID = intr.CreatedBy;
                                issueQty.EmployeeName = objmms.tblEmployeeMasters.SingleOrDefault(y => y.EmpID == intr.CreatedBy).FName;
                                issueQty.ItemGroupID = item.ItemGroupId;
                                issueQty.ItemID = item.ItemId;
                                issueQty.ItemGroupName = objmms.tblItemGroupMasters.SingleOrDefault(y => y.ItemGroupID == item.ItemGroupId).GroupName;
                                var itemData = objmms.tblItemMasters.SingleOrDefault(y => y.ItemID == item.ItemId);
                                issueQty.ItemName = itemData.ItemName;
                                issueQty.UnitID = itemData.UnitID;
                                issueQty.Make = itemData.Make;
                                issueQty.PartNo = itemData.PartNo;
                                issueQty.IssueQuantity = G.DeliveryQty;
                                issueQty.Receive_Rate = item.Rate;
                                issueQty.Issue_Type = "Intra Transfer";
                                issueQty.SessionId = "1";
                                issueQty.SiteId = intr.TransferProjectId;
                                issueQty.IssuedBy = issueQty.EmployeeID;
                                issueQty.Issue_Date = intr.TransferDate;
                                issueQty.Gate_ReceiveDate = DateTime.Now;
                                issueQty.CreatedDate = DateTime.Now;
                                issueQty.GateEntry_UId = item.TransId;
                                issueQty.AvailableQuantity = item.BalanceQuantity;

                                objmms.ns_tbl_IssueQuantity.Add(issueQty);


                                if (ItmId != null && G.ProjectId != null)
                                {
                                    if (objmms.tblItemCurrentStocks.Where(l => l.ItemNo == ItmId && l.ProjectNo == G.ProjectId).Count() > 0)
                                    {
                                        tblItemCurrentStock bg = objmms.tblItemCurrentStocks.Where(l => l.ItemNo == ItmId && l.ProjectNo == G.ProjectId).First();
                                        bg.Qty = Convert.ToDecimal(bg.Qty - iq);
                                        objmms.Entry(bg).State = EntityState.Modified;
                                    }


                                }
                                else
                                {
                                    TempData["errorUpdatingCurrentstock"] = "Yes";
                                }
                                break;
                            }

                            else if (RecvQty < RemIssueQty && RecvQty > 0)
                            {
                                a = RemIssueQty - RecvQty;
                                decimal iq = RecvQty;
                                decimal qt = RecvQty;
                                // decimal bq = 0;
                                decimal aq = RecvQty;
                                decimal aiq = 0;


                                G.ProjectId = x.Value[12];
                                G.TransferProjectId = x.Value[13];
                                G.GetOutNumber = x.Value[11];
                                G.TransferNumber = x.Value[14];
                                G.ItemGroupId = x.Value[1];
                                G.ItemId = x.Value[3];
                                G.UnitId = x.Value[5];
                                G.StockQty = Convert.ToDecimal(x.Value[7]);
                                G.DeliveryQty = iq;
                                G.GetOutQty = iq;
                                G.AvalStockAfterGetOut = aiq;
                                G.GetOutDate = Convert.ToDateTime(x.Value[15]);
                                G.CreatedDate = System.DateTime.Now;
                                G.TransferType = "Intra Transfer";
                                G.TransferChildId = 0;
                                G.ChalanNo = x.Value[16];
                                G.ChalanDate = Convert.ToDateTime(x.Value[17]);
                                G.VehicleNo = x.Value[18];
                                G.EwayBillNo = x.Value[20];
                                //   G.Transfer_Rate = item.Rate;
                                G.DivisionId = Repos.GetUserDivision();
                                G.Session_Year = GetCurrentFinancialYear(G.GetOutDate.Value);
                                GO.Add(G);
                                decimal TrId = item.TransId;
                                tblReceiveData tdr = objmms.tblReceiveDatas.Where(x1 => x1.ProjectId == ProjId && x1.ItemGroupId == ItemGrpid && x1.ItemId == ItmId && x1.TransId == TrId).First();
                                tdr.BalanceQuantity = qt - iq;
                                item.IssueQuantity = item.IssueQuantity + iq;
                                tdr.ModifiedDate = System.DateTime.Now;
                                //tdr.SenderId = G.ProjectId;
                                objmms.Entry(tdr).State = EntityState.Modified;

                                // updating status
                                var temp = x.Value[14];
                                tblIntraStateTransferMaster intr = objmms.tblIntraStateTransferMasters.Where(x1 => x1.IntraTransferNumber == temp).FirstOrDefault();
                                intr.TransferOutStatus = "Get out";
                                objmms.Entry(intr).State = EntityState.Modified;

                                tblIntraTransferTaxableMaster intrTax = objmms.tblIntraTransferTaxableMasters.Where(x1 => x1.IntraTransferNumber == temp).FirstOrDefault();
                                intrTax.TransferOutStatus = "Get out";
                                objmms.Entry(intrTax).State = EntityState.Modified;



                                //add entry in ns_tblIssueQty table
                               
                                ns_tbl_IssueQuantity issueQty = new ns_tbl_IssueQuantity();
                                issueQty.IndentNo = intr.IntraTransferNumber;
                                issueQty.ProjectID = intr.ProjectId;
                                issueQty.ProjectName = objmms.tblProjectMasters.SingleOrDefault(y => y.PRJID == intr.ProjectId).ProjectName;
                                issueQty.EmployeeID = intr.CreatedBy;
                                issueQty.EmployeeName = objmms.tblEmployeeMasters.SingleOrDefault(y => y.EmpID == intr.CreatedBy).FName;
                                issueQty.ItemGroupID = item.ItemGroupId;
                                issueQty.ItemID = item.ItemId;
                                issueQty.ItemGroupName = objmms.tblItemGroupMasters.SingleOrDefault(y => y.ItemGroupID == item.ItemGroupId).GroupName;
                                var itemData = objmms.tblItemMasters.SingleOrDefault(y => y.ItemID == item.ItemId);
                                issueQty.ItemName = itemData.ItemName;
                                issueQty.UnitID = itemData.UnitID;
                                issueQty.Make = itemData.Make;
                                issueQty.PartNo = itemData.PartNo;
                                issueQty.IssueQuantity = G.DeliveryQty;
                                issueQty.Receive_Rate = item.Rate;
                                issueQty.Issue_Type = "Intra Transfer";
                                issueQty.SessionId = "1";
                                issueQty.SiteId = intr.TransferProjectId;
                                issueQty.IssuedBy = issueQty.EmployeeID;
                                issueQty.Issue_Date = intr.TransferDate;
                                issueQty.Gate_ReceiveDate = DateTime.Now;
                                issueQty.CreatedDate = DateTime.Now;
                                issueQty.GateEntry_UId = item.TransId;
                                objmms.ns_tbl_IssueQuantity.Add(issueQty);


                                // stock master table updation here
                                if (ItmId != null && G.ProjectId != null)
                                {
                                    if (objmms.tblItemCurrentStocks.Where(l => l.ItemNo == ItmId && l.ProjectNo == G.ProjectId).Count() > 0)
                                    {
                                        tblItemCurrentStock bg = objmms.tblItemCurrentStocks.Where(l => l.ItemNo == ItmId && l.ProjectNo == G.ProjectId).First();
                                        bg.Qty = Convert.ToDecimal(bg.Qty - iq);
                                        objmms.Entry(bg).State = EntityState.Modified;
                                    }

                                }
                                else
                                {
                                    TempData["errorUpdatingCurrentstock"] = "Yes";
                                }
                            }
                            else
                            {
                                a = RemIssueQty;
                            }

                            if (a > 0)
                            {
                                RemIssueQty = a;
                            }
                            else
                            {
                                break;
                            }


                        }

                        objmms.tblGetOutTransfers.AddRange(GO);
                    }

                }

                objmms.SaveChanges();
            }

            return Json("Gate Out will Successfully done.");
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

        public ActionResult SaveInterGetout(string gridData)
        {
            // change code accordingly
            var log = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(gridData);
            if (log != null)
            {
                foreach (var x in log)
                {
                    if (Convert.ToDecimal(x.Value[9]) > 0)
                    {
                        List<tblGetOutTransfer> GO = new List<tblGetOutTransfer>();
                        decimal RemQty = 0; RemQty = Convert.ToDecimal(x.Value[10]); decimal MyBalQty = Convert.ToDecimal(x.Value[10]);
                        string ProjId = x.Value[12], TransferProjId = x.Value[13], ItemGrpid = x.Value[1], ItmId = x.Value[3], UnitId = x.Value[5];
                        decimal RemIssueQty = Convert.ToDecimal(x.Value[9]);

                        var list = objmms.tblReceiveDatas.Where(x1 => x1.ProjectId == ProjId && x1.ItemGroupId == ItemGrpid && x1.ItemId == ItmId && x1.BalanceQuantity > 0).OrderBy(p => p.TypeDate).ToList();

                        var Temp1 = x.Value[14];
                        //tblInterStateTransferMaster intr = objmms.tblInterStateTransferMasters.Where(x1 => x1.InterTransferNumber == Temp1).FirstOrDefault();
                       
                        // iterating loop in stock List
                        foreach (var item in list)
                        {
                            tblGetOutTransfer G = new tblGetOutTransfer();
                            var balqty = item.BalanceQuantity;

                            decimal RecvQty = Convert.ToDecimal(item.BalanceQuantity); decimal a = 0;
                            if (RecvQty >= RemIssueQty && RecvQty>0)
                            {
                                a = RecvQty - RemIssueQty;
                                decimal iq = RemIssueQty;
                                decimal qt = RecvQty;
                                decimal bq = a;
                                decimal aq = RecvQty;
                                decimal aiq = a;



                                G.ProjectId = x.Value[12];
                                G.TransferProjectId = x.Value[13];
                                G.GetOutNumber = x.Value[11];
                                G.TransferNumber = x.Value[14];
                                G.ItemGroupId = x.Value[1];
                                G.ItemId = x.Value[3];
                                G.UnitId = x.Value[5];
                                G.StockQty = Convert.ToDecimal(x.Value[7]);
                                G.DeliveryQty = iq;
                                G.GetOutQty = iq;
                                G.AvalStockAfterGetOut = aiq;
                                G.GetOutDate = Convert.ToDateTime(x.Value[15]);
                                G.CreatedDate = System.DateTime.Now;
                                G.TransferType = "Inter Transfer";
                                G.TransferChildId = 0;
                                G.ChalanNo = x.Value[16];
                                G.ChalanDate = Convert.ToDateTime(x.Value[17]);
                                G.VehicleNo = x.Value[18];

                                G.Session_Year = GetCurrentFinancialYear(G.GetOutDate.Value);
                                G.DivisionId = Repos.GetUserDivision();

                                GO.Add(G);

                                // for Updating TblReceiveData
                                decimal TrId = item.TransId;
                                tblReceiveData tdr = objmms.tblReceiveDatas.Where(x1 => x1.ProjectId == ProjId && x1.ItemGroupId == ItemGrpid && x1.ItemId == ItmId && x1.TransId == TrId).First();
                                tdr.BalanceQuantity = qt - iq;
                                item.IssueQuantity = item.IssueQuantity + iq;
                                tdr.ModifiedDate = System.DateTime.Now;
                                //tdr.SenderId = G.ProjectId;
                                objmms.Entry(tdr).State = EntityState.Modified;

                                // updating status
                                var Temp = x.Value[14];
                                tblInterStateTransferMaster intr = objmms.tblInterStateTransferMasters.Where(x1 => x1.InterTransferNumber == Temp).FirstOrDefault();
                                intr.TransferOutStatus = "Get out";
                                objmms.Entry(intr).State = EntityState.Modified;


                                
                                //add entry in ns_tblIssueQty table
                                
                                ns_tbl_IssueQuantity issueQty = new ns_tbl_IssueQuantity();
                                issueQty.IndentNo = intr.InterTransferNumber;
                                issueQty.ProjectID = intr.ProjectId;
                                issueQty.ProjectName = objmms.tblProjectMasters.SingleOrDefault(y => y.PRJID == intr.ProjectId).ProjectName;
                                issueQty.EmployeeID = intr.CreatedBy;
                                issueQty.EmployeeName = objmms.tblEmployeeMasters.SingleOrDefault(y => y.EmpID == intr.CreatedBy).FName;
                                issueQty.ItemGroupID = item.ItemGroupId;
                                issueQty.ItemID = item.ItemId;
                                issueQty.ItemGroupName = objmms.tblItemGroupMasters.SingleOrDefault(y => y.ItemGroupID == item.ItemGroupId).GroupName;
                                var itemData = objmms.tblItemMasters.SingleOrDefault(y => y.ItemID == item.ItemId);
                                issueQty.ItemName = itemData.ItemName;
                                issueQty.UnitID = itemData.UnitID;
                                issueQty.Make = itemData.Make;
                                issueQty.PartNo = itemData.PartNo;
                                issueQty.IssueQuantity = G.DeliveryQty;
                                issueQty.Receive_Rate = item.Rate;
                                issueQty.Issue_Type = "Inter Transfer";
                                issueQty.SessionId = "1";
                                issueQty.SiteId = intr.TransferProjectId;
                                issueQty.IssuedBy = issueQty.EmployeeID;
                                issueQty.Issue_Date = intr.TransferDate;
                                issueQty.Gate_ReceiveDate = DateTime.Now;
                                issueQty.CreatedDate = DateTime.Now;
                                issueQty.GateEntry_UId = item.TransId;
                                objmms.ns_tbl_IssueQuantity.Add(issueQty);


                                if (ItmId != null && G.ProjectId != null)
                                {
                                    if (objmms.tblItemCurrentStocks.Where(l => l.ItemNo == ItmId && l.ProjectNo == G.ProjectId).Count() > 0)
                                    {
                                        tblItemCurrentStock bg = objmms.tblItemCurrentStocks.Where(l => l.ItemNo == ItmId && l.ProjectNo == G.ProjectId).First();
                                        bg.Qty = Convert.ToDecimal(bg.Qty - iq);
                                        objmms.Entry(bg).State = EntityState.Modified;
                                    }


                                }
                                else
                                {
                                    TempData["errorUpdatingCurrentstock"] = "Yes";
                                }
                                break;
                            }

                            else if (RecvQty < RemIssueQty && RecvQty > 0)
                            {
                                a = RemIssueQty - RecvQty;
                                decimal iq = RecvQty;
                                decimal qt = RecvQty;
                                // decimal bq = 0;
                                decimal aq = RecvQty;
                                decimal aiq = 0;


                                G.ProjectId = x.Value[12];
                                G.TransferProjectId = x.Value[13];
                                G.GetOutNumber = x.Value[11];
                                G.TransferNumber = x.Value[14];
                                G.ItemGroupId = x.Value[1];
                                G.ItemId = x.Value[3];
                                G.UnitId = x.Value[5];
                                G.StockQty = Convert.ToDecimal(x.Value[7]);
                                G.DeliveryQty = iq;
                                G.GetOutQty = iq;
                                G.AvalStockAfterGetOut = aiq;
                                G.GetOutDate = Convert.ToDateTime(x.Value[15]);
                                G.CreatedDate = System.DateTime.Now;
                                G.TransferType = "Inter Transfer";
                                G.TransferChildId = 0;
                                G.ChalanNo = x.Value[16];
                                G.ChalanDate = Convert.ToDateTime(x.Value[17]);
                                G.VehicleNo = x.Value[18];

                                G.Session_Year = GetCurrentFinancialYear(G.GetOutDate.Value);

                                GO.Add(G);
                                decimal TrId = item.TransId;
                                tblReceiveData tdr = objmms.tblReceiveDatas.Where(x1 => x1.ProjectId == ProjId && x1.ItemGroupId == ItemGrpid && x1.ItemId == ItmId && x1.TransId == TrId).First();
                                tdr.BalanceQuantity = qt - iq;
                                item.IssueQuantity = item.IssueQuantity + iq;
                                tdr.ModifiedDate = System.DateTime.Now;
                               // tdr.SenderId = G.ProjectId;
                                objmms.Entry(tdr).State = EntityState.Modified;

                                // updating status
                                var TransId = Convert.ToDecimal(x.Value[20]);
                                var TransferNo = x.Value[14];
                                tblInterStateTransferMaster intr = objmms.tblInterStateTransferMasters.Where(x1 => x1.InterTransferNumber == TransferNo && x1.TransId == TransId).FirstOrDefault();
                                intr.TransferOutStatus = "Get out";
                                objmms.Entry(intr).State = EntityState.Modified;


                                //tblInterStateTransferChild itemDetails = objmms.tblInterStateTransferChilds.SingleOrDefault(y => y.InterStateTransferMasterId == intr.TransId);
                                ns_tbl_IssueQuantity issueQty = new ns_tbl_IssueQuantity();
                                issueQty.IndentNo = intr.InterTransferNumber;
                                issueQty.ProjectID = intr.ProjectId;
                                issueQty.ProjectName = objmms.tblProjectMasters.SingleOrDefault(y => y.PRJID == intr.ProjectId).ProjectName;
                                issueQty.EmployeeID = intr.CreatedBy;
                                issueQty.EmployeeName = objmms.tblEmployeeMasters.SingleOrDefault(y => y.EmpID == intr.CreatedBy).FName;
                                issueQty.ItemGroupID = item.ItemGroupId;
                                issueQty.ItemID = item.ItemId;
                                issueQty.ItemGroupName = objmms.tblItemGroupMasters.SingleOrDefault(y => y.ItemGroupID == item.ItemGroupId).GroupName;
                                var itemData = objmms.tblItemMasters.SingleOrDefault(y => y.ItemID == item.ItemId);
                                issueQty.ItemName = itemData.ItemName;
                                issueQty.UnitID = itemData.UnitID;
                                issueQty.Make = itemData.Make;
                                issueQty.PartNo = itemData.PartNo;
                                issueQty.IssueQuantity = G.DeliveryQty;
                                issueQty.Receive_Rate = item.Rate;
                                issueQty.Issue_Type = "Inter Transfer";
                                issueQty.SessionId = "1";
                                issueQty.SiteId = intr.TransferProjectId;
                                issueQty.IssuedBy = issueQty.EmployeeID;
                                issueQty.Issue_Date = intr.TransferDate;
                                issueQty.Gate_ReceiveDate = DateTime.Now;
                                issueQty.CreatedDate = DateTime.Now;
                                issueQty.GateEntry_UId = item.TransId;
                                objmms.ns_tbl_IssueQuantity.Add(issueQty);




                                // stock master table updation here
                                if (ItmId != null && G.ProjectId != null)
                                {
                                    if (objmms.tblItemCurrentStocks.Where(l => l.ItemNo == ItmId && l.ProjectNo == G.ProjectId).Count() > 0)
                                    {
                                        tblItemCurrentStock bg = objmms.tblItemCurrentStocks.Where(l => l.ItemNo == ItmId && l.ProjectNo == G.ProjectId).First();
                                        bg.Qty = Convert.ToDecimal(bg.Qty - iq);
                                        objmms.Entry(bg).State = EntityState.Modified;
                                    }


                                }
                                else
                                {
                                    TempData["errorUpdatingCurrentstock"] = "Yes";
                                }
                            }
                            if (a > 0)
                            {
                                RemIssueQty = a;
                            }
                            else if (RemIssueQty > 0)
                            {

                            }
                            else
                            {
                                break;
                            }


                        }
                        objmms.tblGetOutTransfers.AddRange(GO);
                    }
                    

                }
                objmms.SaveChanges();
            }

            return Json("Gate out will Successfully done.");
        }

        public ActionResult SaveVendorDebitNoteGetOut(string gridData)
        {


            var log = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(gridData);
            if (log != null)
            {
                var debitNo = log.FirstOrDefault().Value[14];

                var master = objmms.tblVendorDebitNoteMasters.SingleOrDefault(y => y.VendorDebitNo == debitNo);
                var grn = master.GRNNo;
                var gateDetails = objmms.GateEntries.FirstOrDefault(x => x.GateEntry_Mid_No == grn);
                var mrn = gateDetails.GateEntryNo;
                var debitChildData = objmms.tblVendorDebitNoteChilds.Where(x => x.MUId == master.UId);
                var itemNo = ""; var Qty = new decimal(0);

                foreach (var item in debitChildData)
                {
                    var d = objmms.tblReceiveDatas.FirstOrDefault(s => s.TypeNumber == mrn && s.ItemId == item.ItemNo);
                    if (item.TransferQty > d.BalanceQuantity)
                    {
                        itemNo = d.ItemId;
                        Qty = d.BalanceQuantity.Value;
                        break;
                    }
                }

                if (itemNo != "")
                {
                    TempData.Keep();
                    var itemName = objmms.tblItemMasters.SingleOrDefault(x => x.ItemID == itemNo).ItemName;
                    return Json(new {Status=3, Msg = "Item -" + itemName + " , MRN No. " + gateDetails.MRN_No_New + " - Balance Qty- " + Qty });
                }

                foreach (var x in log)
                {
                    if (Convert.ToDecimal(x.Value[9]) > 0)
                    {
                        List<tblGetOutTransfer> GO = new List<tblGetOutTransfer>();
                        decimal RemQty = 0; RemQty = Convert.ToDecimal(x.Value[10]); decimal MyBalQty = Convert.ToDecimal(x.Value[10]);
                        string ProjId = x.Value[12], TransferProjId = x.Value[13], ItemGrpid = x.Value[1], ItmId = x.Value[3], UnitId = x.Value[5];
                        decimal RemIssueQty = Convert.ToDecimal(x.Value[9]);

                        var item = objmms.tblReceiveDatas.FirstOrDefault(x1 => x1.ProjectId == ProjId && x1.ItemGroupId == ItemGrpid && x1.ItemId == ItmId && x1.BalanceQuantity > 0 && x1.TypeNumber==mrn);

                        tblGetOutTransfer G = new tblGetOutTransfer();
                        var balqty = item.BalanceQuantity;
                        decimal RecvQty = Convert.ToDecimal(item.BalanceQuantity);

                        G.ProjectId = x.Value[12];
                        G.TransferProjectId = x.Value[13];
                        G.GetOutNumber = x.Value[11];
                        G.TransferNumber = x.Value[14];
                        G.ItemGroupId = x.Value[1];
                        G.ItemId = x.Value[3];
                        G.UnitId = x.Value[5];
                        G.StockQty = Convert.ToDecimal(x.Value[7]);
                        G.DeliveryQty = RemIssueQty;
                        G.GetOutQty = RemIssueQty;
                        G.AvalStockAfterGetOut = (item.BalanceQuantity-RemIssueQty);
                        G.GetOutDate = Convert.ToDateTime(x.Value[15]);
                        G.CreatedDate = DateTime.Now;
                        G.TransferType = "Vendor Debit Note";
                        G.TransferChildId = 0;
                        G.ChalanNo = x.Value[16];
                        G.ChalanDate = Convert.ToDateTime(x.Value[17]);
                        G.VehicleNo = x.Value[18];
                        G.Session_Year = GetCurrentFinancialYear(G.GetOutDate.Value);
                        G.DivisionId = Repos.GetUserDivision();
                        GO.Add(G);

                        // for Updating TblReceiveData
                        decimal TrId = item.TransId;
                        tblReceiveData tdr = objmms.tblReceiveDatas.Where(x1 => x1.ProjectId == ProjId && x1.ItemGroupId == ItemGrpid && x1.ItemId == ItmId && x1.TransId == TrId).First();
                        tdr.BalanceQuantity = (item.BalanceQuantity - RemIssueQty);
                        item.IssueQuantity = item.IssueQuantity + RemIssueQty;
                        tdr.ModifiedDate = DateTime.Now;

                        objmms.Entry(tdr).State = EntityState.Modified;

                        var uid = Convert.ToDecimal(x.Value[21]);
                        var data = objmms.tblVendorDebitNoteMasters.FirstOrDefault(xx => xx.UId == uid);
                        data.Status = "Get Out";
                        objmms.Entry(data).State = EntityState.Modified;

                        //add entry in ns_tblIssueQty table
                        var itemsDetails = objmms.tblVendorDebitNoteChilds.FirstOrDefault(y => y.MUId == data.UId && y.ItemNo==item.ItemId);
                        var itemData = objmms.tblItemMasters.SingleOrDefault(y => y.ItemID == item.ItemId);

                        ns_tbl_IssueQuantity issueQty = new ns_tbl_IssueQuantity();
                        issueQty.IndentNo = data.VendorDebitNo;
                        issueQty.ProjectID = data.ProjectNumber;
                        issueQty.ProjectName = objmms.tblProjectMasters.SingleOrDefault(y => y.PRJID == issueQty.ProjectID).ProjectName;
                        issueQty.EmployeeID = data.CreatedBy;
                        issueQty.EmployeeName = objmms.tblEmployeeMasters.SingleOrDefault(y => y.EmpID == data.CreatedBy).FName;
                        issueQty.ItemGroupID = itemData.ItemGroupID;
                        issueQty.ItemID = itemData.ItemID;
                        issueQty.ItemGroupName = objmms.tblItemGroupMasters.SingleOrDefault(y => y.ItemGroupID == itemData.ItemGroupID).GroupName;

                        issueQty.ItemName = itemData.ItemName;
                        issueQty.UnitID = itemData.UnitID;
                        issueQty.Make = itemData.Make;
                        issueQty.PartNo = itemData.PartNo;
                        issueQty.IssueQuantity = itemsDetails.TransferQty;
                        issueQty.Receive_Rate = item.Rate;
                        issueQty.Issue_Type = "Vendor Debit Note Transfer";
                        issueQty.SessionId = "1";
                        issueQty.SiteId = issueQty.ProjectID;
                        issueQty.IssuedBy = issueQty.EmployeeID;
                        issueQty.Issue_Date = Convert.ToDateTime(data.VendorDebitDate);
                        issueQty.Gate_ReceiveDate = DateTime.Now;
                        issueQty.CreatedDate = DateTime.Now;
                        issueQty.GateEntry_UId = item.TransId;

                        objmms.ns_tbl_IssueQuantity.Add(issueQty);

                        if (ItmId != null && G.ProjectId != null)
                        {
                            if (objmms.tblItemCurrentStocks.Where(l => l.ItemNo == ItmId && l.ProjectNo == G.ProjectId).Count() > 0)
                            {
                                tblItemCurrentStock bg = objmms.tblItemCurrentStocks.Where(l => l.ItemNo == ItmId && l.ProjectNo == G.ProjectId).First();
                                bg.Qty = Convert.ToDecimal(bg.Qty - RemIssueQty);
                                objmms.Entry(bg).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            TempData["errorUpdatingCurrentstock"] = "Yes";
                        }



                        objmms.tblGetOutTransfers.AddRange(GO);


                    }

                }
                objmms.SaveChanges();
            }

            return Json("Gate out will Successfully done.");
        }
    }
}