using DataAccessLayer;
using MMS.DAL;
using MMS.Helpers;
using MMS.Models;
using MMS.ViewModels;
using MMS_P.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class VendorDebitNoteController : Controller
    {
        // GET: VendorDebitNote
        MMSEntities mmsObj = new MMSEntities();
        Procedure procedure = new Procedure();
        FactoryManager m = new FactoryManager();
        MSP_Model objmsps = new MSP_Model();
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;
            return View();
        }

        public JsonResult GetVendors(string ProjectId)
        {
            var vendorsList = mmsObj.tblVendorMasters.Where(x => x.PRJID.Contains(ProjectId)).Select(x => new { Text = x.Name, Value = x.VendorID });
            return Json(vendorsList.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPOForVendors(string ProjectId, string VendorId)
        {
            var poList = mmsObj.TbIndentPurchaseOrder_GST.Where(x => x.ProjectNo == ProjectId && x.SupplierNo == VendorId).Select(x => new { Text = x.PurchaseOrderNo, Value = x.PurchaseOrderNo });
            return Json(poList.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult CheckWhetherQtyIssuedOrNot(string MRNNo)
        //{
        //    if (MRNNo == "")
        //    {
        //        return Json(new { Status = 2 }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var mrn = mmsObj.GateEntries.SingleOrDefault(x => x.MRN_No_New == MRNNo).GateEntryNo;
        //        var mrnNo
        //    }
        //}

        public JsonResult GeMRNForPONo(string ProjectId, string VendorId, string PONumber)
        {

            var data = procedure.GeMRNForPONo(ProjectId, VendorId, PONumber);
            var mrnList = data.ToList();
            return Json(mrnList.ToJSON(), JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult GetSearchData(string ProjectId, string VendorId, string PONumber, string MRNNumber)
        {

            var poDetails = procedure.GetPOAndChallanDetails(ProjectId, MRNNumber, VendorId, PONumber).SingleOrDefault();

            ViewBag.PODate = poDetails.PODate;
            ViewBag.MRNDate = poDetails.MRNDate;
            ViewBag.ChallanNo = poDetails.ChallanNo;
            ViewBag.ChallanDate = poDetails.ChallanDate;
            ViewBag.GRNNo = poDetails.GRNNo;
            ViewBag.GRNDate = poDetails.GRNDate;

            var data = procedure.GetVendorItemSearch(ProjectId, MRNNumber, VendorId, PONumber);

            //TempData["ProjectId"] = ProjectId;
            TempData["GRNNo"] = poDetails.GRNNo;
            TempData["VendorId"] = VendorId;
            TempData["PONumber"] = PONumber;
            TempData["PODate"] = poDetails.PODate;

            TempData["searchData"] = data;
            TempData.Keep();

            return PartialView("_GridView", data);
        }



        [HttpPost]
        public ActionResult CreateDebitNote(FormCollection frm)
        {

            if (Session["EmpID"] != null && frm["hdn"] != null && frm["hdnProjectId"] !=null)
            {
                string[] selectedDataItemIDs = null;
                string ProjectId = frm["hdnProjectId"].ToString();

                if (!frm["hdn"].ToString().Contains(','))
                {
                    selectedDataItemIDs = new string[1];
                    selectedDataItemIDs[0] = frm["hdn"].ToString();
                }
                else
                {
                    var temp = frm["hdn"].ToString().Split(',');
                    selectedDataItemIDs = new string[temp.Length];
                    selectedDataItemIDs = temp;
                }
                string empId = Session["EmpID"].ToString();
                ViewBag.Employee = new SelectList(mmsObj.tblEmployeeMasters.Where(x => x.EmpID == empId), "EmpID", "FName");
                string vendorId = TempData["VendorId"].ToString();
                ViewBag.ProjectName = mmsObj.tblProjectMasters.SingleOrDefault(x => x.PRJID== ProjectId).ProjectName;
                ViewBag.SiteAddress = mmsObj.tblProjectParticularsDetailAs.Where(m => m.PRJID == ProjectId).FirstOrDefault().Location;
                ViewBag.GSTNo = mmsObj.tblProjectGSTNoes.SingleOrDefault(x => x.ProjectId == ProjectId).GSTNo;
                ViewBag.VendorName = mmsObj.tblVendorMasters.Where(x => x.VendorID == vendorId).SingleOrDefault().Name;
                ViewBag.VendorAddress = mmsObj.tblVendorMasters.Where(x => x.VendorID == vendorId).SingleOrDefault().Address;
                ViewBag.PONo = TempData["PONumber"].ToString();
                ViewBag.PODate = TempData["PODate"].ToString();

                var TotalCartage = mmsObj.tblMMSCartageTypes.Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
                ViewBag.TotalCartagetest = new SelectList(TotalCartage, "Value", "Text");
                var TotalGSTSLABMaster = mmsObj.tblGST_SplitTaxMaster.ToList().Select(n => new { Text = n.TaxRateType, Value = n.TaxCode }).OrderBy(s => s.Text).ToList();
                ViewBag.MyTotalGSTlst = new SelectList(TotalGSTSLABMaster, "Value", "Text");


                var data = TempData["searchData"] as List<VendorSearchItem>;
                TempData["ProjectId"] = ProjectId;
                List<VendorSearchItem> selectedData = new List<VendorSearchItem>();
                for (int i = 0; i < selectedDataItemIDs.Length; i++)
                {
                    selectedData.Add(data.SingleOrDefault(x => x.TransId == Convert.ToInt32(selectedDataItemIDs[i])));
                }

                TempData.Keep();

                return View(selectedData);
            }
            return View("_EmptyView");

        }


        public string GetDebitCode(string debitDate)
        {
            string debitCode = procedure.GetVendorDebitNote(TempData["ProjectId"].ToString(), Convert.ToDateTime(debitDate));
            TempData["DebitCode"] = debitCode;
            TempData["DebitDate"] = debitDate;
            TempData.Keep();
            return debitCode;
        }

        [HttpPost]
        public JsonResult SaveDebitNote(List<DebitNoteItem> Items, List<ItemWiseRates> ItemDescription, List<InsuranceAndGstDetails> InsuranceGStDetails, TotalCalculation TotalCalculation, string PONo, string DebitNoteCode, string DebitNoteDate, string SendTo,string Reason)
        {
            CultureInfo cInfo = new CultureInfo("en-IN");
            cInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            cInfo.DateTimeFormat.DateSeparator = "/";
            cInfo.DateTimeFormat.LongDatePattern = "dd/MM/yyyy hh:mm:ss";
            Thread.CurrentThread.CurrentCulture = cInfo;
            Thread.CurrentThread.CurrentUICulture = cInfo;

            var grn = TempData["GRNNo"].ToString();
            var gateDetails= mmsObj.GateEntries.FirstOrDefault(x => x.GateEntry_Mid_No == grn);
            var mrn = gateDetails.GateEntryNo;
            var itemNo = "";var Qty = new decimal(0);

            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                var debitQty = Convert.ToDecimal(ItemDescription[i].TransferQty);
                var d = mmsObj.tblReceiveDatas.FirstOrDefault(x => x.TypeNumber == mrn && x.ItemId == item.ItemNo);
                if (d.BalanceQuantity<debitQty)
                {
                    itemNo = item.ItemNo;
                    Qty = d.BalanceQuantity.Value;
                    break;
                }
            }

            if (itemNo != "")
            {
                TempData.Keep();
                var itemName=mmsObj.tblItemMasters.SingleOrDefault(x => x.ItemID == itemNo).ItemName;
                return Json(new { Status = 3,Msg="Item -"+itemName+" , MRN No. "+gateDetails.MRN_No_New+" - Balance Qty- "+Qty });
            }

            using (var trans=mmsObj.Database.BeginTransaction())
            {
                try
                {
                    tblVendorDebitNoteMaster debitNote = null;
                    List<tblVendorDebitNoteChild> debitNoteItems = null;

                    if (Items.Count > 0 && ItemDescription.Count > 0 && InsuranceGStDetails.Count > 0 && TotalCalculation != null)
                    {
                        debitNote = new tblVendorDebitNoteMaster();
                        debitNote.CreatedBy = Session["EmpID"].ToString();
                        debitNote.PurchaseOrderNo = PONo;
                        debitNote.ProjectNumber = TempData["ProjectId"].ToString();
                        debitNote.CreatedDate = DateTime.Now;
                        debitNote.Status = "Not Approved";
                        debitNote.ModifiedBy = Session["EmpID"].ToString();
                        debitNote.ModifiedDate = DateTime.Now;
                        debitNote.SupplierNo = TempData["VendorId"].ToString();
                        debitNote.Reason = Reason;
                        debitNote.DivisionId = Repos.GetUserDivision();

                        debitNote.SubTotal1 = TotalCalculation.SubTotal1 == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.SubTotal1);
                        debitNote.SubTotal2 = TotalCalculation.SubTotal2 == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.SubTotal2);
                        debitNote.Total = TotalCalculation.GrandTotal == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.GrandTotal);
                        debitNote.GrandTotal = TotalCalculation.Total == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total);



                        debitNote.Total_Cartage = TotalCalculation.SumOfCartageAmt == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.SumOfCartageAmt);
                        debitNote.Total_CGST = TotalCalculation.Total_CGST == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total_CGST);
                        debitNote.Total_IGST = TotalCalculation.Total_IGST == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total_IGST);
                        debitNote.Total_Taxable = TotalCalculation.Total_Taxable == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total_Taxable);
                        debitNote.Total_Insurance = TotalCalculation.Total_Insurance == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total_Insurance);

                        debitNote.Total_SGSTAndUTGST = TotalCalculation.Total_SGSTAndUTGST == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total_SGSTAndUTGST);

                        debitNote.VendorDebitNo = GetDebitCode(DebitNoteDate); //DebitNoteCode;
                        debitNote.VendorDebitDate = Convert.ToDateTime(DebitNoteDate);
                        debitNote.Send_To = SendTo;

                        debitNote.GRNNo = TempData["GRNNo"].ToString();

                        mmsObj.tblVendorDebitNoteMasters.Add(debitNote);
                        mmsObj.SaveChanges();
                        debitNoteItems = new List<tblVendorDebitNoteChild>();
                        for (int i = 0; i < Items.Count; i++)
                        {
                            var item = Items[i];
                            var itemDetails = ItemDescription[i];
                            var itemInsuranceAndGStDetails = InsuranceGStDetails[i];

                            var debitNoteItem = new tblVendorDebitNoteChild();
                            debitNoteItem.ItemNo = item.ItemNo;
                            debitNoteItem.Item_Description = itemDetails.Description;
                            debitNoteItem.GrossAmount = item.TotalGrandTotal == "" ? new decimal(0.0) : Convert.ToDecimal(item.TotalGrandTotal);
                            debitNoteItem.SubTotal1 = item.TotalSubTotal == "" ? new decimal(0.0) : Convert.ToDecimal(item.TotalSubTotal);
                            debitNoteItem.SubTotal2 = item.TotalSubTotal2 == "" ? new decimal(0.0) : Convert.ToDecimal(item.TotalSubTotal2);
                            debitNoteItem.Total = item.CTotalAmount == "" ? new decimal(0.0) : Convert.ToDecimal(item.CTotalAmount);
                            debitNoteItem.MUId = debitNote.UId;



                            debitNoteItem.CartageTypeId = itemDetails.CartageType == "" ? 0 : Convert.ToInt32(itemDetails.CartageType);
                            debitNoteItem.Cartage_rate = itemDetails.CartageRate == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.CartageRate);
                            debitNoteItem.CartageAmount = itemDetails.CartageAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.CartageAmt);
                            debitNoteItem.Discount = itemDetails.Discount == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.Discount);
                            debitNoteItem.Discounted_Amount = new decimal(0.0);
                            debitNoteItem.InsuranceAmount = itemInsuranceAndGStDetails.InsurancePerAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.InsurancePerAmt);
                            debitNoteItem.InsuranceRate = itemInsuranceAndGStDetails.InsuranceRatePer == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.InsuranceRatePer);
                            debitNoteItem.Qty = itemDetails.Qty == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.Qty);
                            debitNoteItem.TransferQty = itemDetails.TransferQty == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.TransferQty);
                            debitNoteItem.Rate = itemDetails.CRate == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.CRate);

                            debitNoteItem.PackingChargesPercentage = itemDetails.PackingChargePer == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.PackingChargePer);
                            debitNoteItem.PackingChargesAmount = itemDetails.PackingChargeAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.PackingChargeAmt);


                            debitNoteItem.GrossAmount = item.CTotalAmount == "" ? new decimal(0.0) : Convert.ToDecimal(item.CTotalAmount);
                            debitNoteItem.CGSTPercentage = itemInsuranceAndGStDetails.CGSTPer == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.CGSTPer);
                            debitNoteItem.CGSTAmount = itemInsuranceAndGStDetails.CGSTPerAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.CGSTPerAmt);
                            debitNoteItem.SGSTPercentage = itemInsuranceAndGStDetails.SGSTOrUGSTPer == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.SGSTOrUGSTPer);
                            debitNoteItem.SGSTAmount = itemInsuranceAndGStDetails.SGSTOrUTGSTPerAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.SGSTOrUTGSTPerAmt);
                            debitNoteItem.IGSTPercentage = itemInsuranceAndGStDetails.IGSTPer == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.IGSTPer);
                            debitNoteItem.IGSTAmount = itemInsuranceAndGStDetails.IGSTPerAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.IGSTPerAmt);


                            debitNoteItem.TotalGSTAmount = itemInsuranceAndGStDetails.TotalGSTAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.TotalGSTAmt);
                            debitNoteItem.TaxCode = itemInsuranceAndGStDetails.GSTSlab == "" ? "0" : itemInsuranceAndGStDetails.GSTSlab;

                            debitNoteItems.Add(debitNoteItem);
                        }

                        mmsObj.tblVendorDebitNoteChilds.AddRange(debitNoteItems);
                        mmsObj.SaveChanges();
                        trans.Commit();
                        return Json(new { Status = 1, Url = Url.Action("Index", "VendorDebitNote"), Msg=debitNote.VendorDebitNo });
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    TempData.Keep();
                    return Json(new { Status = 3, Msg=ex.Message });
                } 
            }

            TempData.Keep();
            return Json(new { Status = 2 });
        }

        public PartialViewResult GetPendingDebitNote(string ProjectId)
        {
            var data = procedure.GetVendorDebitNotes(ProjectId, "Not Approved");
            return PartialView("_PendingDebitNotes", data);
        }

        public PartialViewResult GetApprovedDebitNote(string ProjectId)
        {
            var data = procedure.GetVendorDebitNotes(ProjectId, "Approved");
            return PartialView("_ApprovedDebitNotes", data);
        }

        public ActionResult ViewDebitNote()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;
            return View();
        }


        public PartialViewResult GetDebitNoteDetails(decimal TransId)
        {
            var data = mmsObj.tblVendorDebitNoteChilds.Where(x => x.MUId == TransId).ToList();
            return PartialView("_DebitNoteDetails", data);
        }


        public JsonResult ApproveDebitNote(string TransId)
        {
            if (!string.IsNullOrEmpty(TransId) && TransId != "0")
            {
                try
                {
                    var uid = Convert.ToDecimal(TransId);
                    var data = mmsObj.tblVendorDebitNoteMasters.Where(x => x.UId == uid).SingleOrDefault();
                    data.Status = "Approved";
                    mmsObj.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    mmsObj.SaveChanges();
                    return Json(new { Status = 1, Url = Url.Action("ViewDebitNote", "VendorDebitNote") }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {

                    return Json(new { Status = 2 }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { Status = 3 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditDebitNote(string DebitCode)
        {


            var data = mmsObj.tblVendorDebitNoteMasters.FirstOrDefault(x => x.VendorDebitNo == DebitCode && x.Status.Equals("Not Approved"));
            if (data != null)
            {

                var selectedData = procedure.GetVendorItemByDebitCode(DebitCode);

                string empId = Session["EmpID"].ToString();
                ViewBag.Employee = new SelectList(mmsObj.tblEmployeeMasters.Where(x => x.EmpID == empId), "EmpID", "FName");
                string vendorId = data.SupplierNo;
                ViewBag.ProjectName = mmsObj.tblProjectMasters.SingleOrDefault(x => x.PRJID == data.ProjectNumber).ProjectName;
                ViewBag.SiteAddress = mmsObj.tblProjectParticularsDetailAs.Where(m => m.PRJID == data.ProjectNumber).FirstOrDefault().Location;
                ViewBag.GSTNo = mmsObj.tblProjectGSTNoes.SingleOrDefault(x => x.ProjectId == data.ProjectNumber).GSTNo;
                ViewBag.VendorName = mmsObj.tblVendorMasters.Where(x => x.VendorID == vendorId).SingleOrDefault().Name;
                ViewBag.VendorAddress = mmsObj.tblVendorMasters.Where(x => x.VendorID == vendorId).SingleOrDefault().Address;
                ViewBag.PONo = data.PurchaseOrderNo;
                ViewBag.PODate = mmsObj.TbIndentPurchaseOrder_GST.SingleOrDefault(x => x.PurchaseOrderNo == data.PurchaseOrderNo).POReleaseDate.Value.ToShortDateString();
                ViewBag.DebitNoteDate = data.VendorDebitDate.HasValue ? data.VendorDebitDate.Value.ToString("dd/MM/yyyy"):"";
                ViewBag.DebitNoteCode = data.VendorDebitNo;

                var TotalCartage = mmsObj.tblMMSCartageTypes.Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
                ViewBag.TotalCartagetest = new SelectList(TotalCartage, "Value", "Text");
                var TotalGSTSLABMaster = mmsObj.tblGST_SplitTaxMaster.ToList().Select(n => new { Text = n.TaxRateType, Value = n.TaxCode }).OrderBy(s => s.Text).ToList();
                ViewBag.MyTotalGSTlst = new SelectList(TotalGSTSLABMaster, "Value", "Text");

                VendorDebitNoteViewModel model = new VendorDebitNoteViewModel();
                model.Items = selectedData;
                model.TotalCalculation = new TotalCalculation { GrandTotal = data.GrandTotal.ToString(), SubTotal1 = data.SubTotal1.ToString(), SubTotal2 = data.SubTotal2.ToString(), SumOfCartageAmt = data.Total_Cartage.ToString(), Total = data.Total.ToString(), Total_CGST = data.Total_CGST.ToString(), Total_IGST = data.Total_IGST.ToString(), Total_Insurance = data.Total_Insurance.ToString(), Total_SGSTAndUTGST = data.Total_SGSTAndUTGST.ToString(), Total_Taxable = data.Total_Taxable.ToString() };
                TempData.Keep();

                return View(model);
            }
            else
            {
                return RedirectToAction("ViewDebitNote");
            }
        }

        [HttpPost]
        public JsonResult UpdateDebitNote(List<DebitNoteItem> Items, List<ItemWiseRates> ItemDescription, List<InsuranceAndGstDetails> InsuranceGStDetails, TotalCalculation TotalCalculation, string PONo, string DebitNoteCode, string DebitNoteDate, string SendTo)
        {

            try
            {
                if (!string.IsNullOrEmpty(DebitNoteCode))
                {
                    var debitNote = mmsObj.tblVendorDebitNoteMasters.SingleOrDefault(x => x.VendorDebitNo == DebitNoteCode);
                    if (debitNote != null)
                    {
                        var uid = debitNote.UId;
                        var debitNoteItems = mmsObj.tblVendorDebitNoteChilds.Where(x => x.MUId == uid).ToList();

                        debitNote.SubTotal1 = TotalCalculation.SubTotal1 == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.SubTotal1);
                        debitNote.SubTotal2 = TotalCalculation.SubTotal2 == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.SubTotal2);
                        debitNote.Total = TotalCalculation.GrandTotal == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.GrandTotal);
                        debitNote.GrandTotal = TotalCalculation.Total == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total);


                        debitNote.Total_Cartage = TotalCalculation.SumOfCartageAmt == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.SumOfCartageAmt);
                        debitNote.Total_CGST = TotalCalculation.Total_CGST == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total_CGST);
                        debitNote.Total_IGST = TotalCalculation.Total_IGST == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total_IGST);
                        debitNote.Total_Taxable = TotalCalculation.Total_Taxable == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total_Taxable);
                        debitNote.Total_Insurance = TotalCalculation.Total_Insurance == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total_Insurance);

                        debitNote.Total_SGSTAndUTGST = TotalCalculation.Total_SGSTAndUTGST == "" ? new decimal(0.0) : Convert.ToDecimal(TotalCalculation.Total_SGSTAndUTGST);

                        //debitNote.VendorDebitNo = DebitNoteCode;
                        //debitNote.VendorDebitDate = DebitNoteDate == "" ? DateTime.Now : Convert.ToDateTime(DebitNoteDate);
                        debitNote.Send_To = SendTo;

                        mmsObj.Entry(debitNote).State = System.Data.Entity.EntityState.Modified;
                        mmsObj.SaveChanges();

                        for (int i = 0; i < debitNoteItems.Count; i++)
                        {
                            var item = Items[i];
                            var itemDetails = ItemDescription[i];
                            var itemInsuranceAndGStDetails = InsuranceGStDetails[i];

                            var debitNoteItem = debitNoteItems[i];
                            debitNoteItem.ItemNo = item.ItemNo;
                            debitNoteItem.Item_Description = itemDetails.Description;
                            debitNoteItem.GrossAmount = item.TotalGrandTotal == "" ? new decimal(0.0) : Convert.ToDecimal(item.TotalGrandTotal);
                            debitNoteItem.SubTotal1 = item.TotalSubTotal == "" ? new decimal(0.0) : Convert.ToDecimal(item.TotalSubTotal);
                            debitNoteItem.SubTotal2 = item.TotalSubTotal2 == "" ? new decimal(0.0) : Convert.ToDecimal(item.TotalSubTotal2);
                            debitNoteItem.Total = item.CTotalAmount == "" ? new decimal(0.0) : Convert.ToDecimal(item.CTotalAmount);
                            //  debitNoteItem.MUId = debitNote.UId;



                            debitNoteItem.CartageTypeId = itemDetails.CartageType == "" ? 0 : Convert.ToInt32(itemDetails.CartageType);
                            debitNoteItem.Cartage_rate = itemDetails.CartageRate == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.CartageRate);
                            debitNoteItem.CartageAmount = itemDetails.CartageAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.CartageAmt);
                            debitNoteItem.Discount = itemDetails.Discount == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.Discount);
                            debitNoteItem.Discounted_Amount = new decimal(0.0);
                            debitNoteItem.InsuranceAmount = itemInsuranceAndGStDetails.InsurancePerAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.InsurancePerAmt);
                            debitNoteItem.InsuranceRate = itemInsuranceAndGStDetails.InsuranceRatePer == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.InsuranceRatePer);
                            debitNoteItem.Qty = itemDetails.Qty == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.Qty);
                            debitNoteItem.TransferQty = itemDetails.TransferQty == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.TransferQty);
                            debitNoteItem.Rate = itemDetails.CRate == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.CRate);

                            debitNoteItem.PackingChargesPercentage = itemDetails.PackingChargePer == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.PackingChargePer);
                            debitNoteItem.PackingChargesAmount = itemDetails.PackingChargeAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemDetails.PackingChargeAmt);


                            debitNoteItem.GrossAmount = item.CTotalAmount == "" ? new decimal(0.0) : Convert.ToDecimal(item.CTotalAmount);
                            debitNoteItem.CGSTPercentage = itemInsuranceAndGStDetails.CGSTPer == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.CGSTPer);
                            debitNoteItem.CGSTAmount = itemInsuranceAndGStDetails.CGSTPerAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.CGSTPerAmt);
                            debitNoteItem.SGSTPercentage = itemInsuranceAndGStDetails.SGSTOrUGSTPer == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.SGSTOrUGSTPer);
                            debitNoteItem.SGSTAmount = itemInsuranceAndGStDetails.SGSTOrUTGSTPerAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.SGSTOrUTGSTPerAmt);
                            debitNoteItem.IGSTPercentage = itemInsuranceAndGStDetails.IGSTPer == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.IGSTPer);
                            debitNoteItem.IGSTAmount = itemInsuranceAndGStDetails.IGSTPerAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.IGSTPerAmt);


                            debitNoteItem.TotalGSTAmount = itemInsuranceAndGStDetails.TotalGSTAmt == "" ? new decimal(0.0) : Convert.ToDecimal(itemInsuranceAndGStDetails.TotalGSTAmt);
                            debitNoteItem.TaxCode = itemInsuranceAndGStDetails.GSTSlab == "" ? "0" : itemInsuranceAndGStDetails.GSTSlab;
                            
                            mmsObj.Entry(debitNoteItem).State = System.Data.Entity.EntityState.Modified;
                        }
                        mmsObj.SaveChanges();
                        return Json(new { Status = 1, Url = Url.Action("ViewDebitNote", "VendorDebitNote") });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = 2 });
            }
            return Json(new { Status = 3 });
        }


        public ActionResult PrintVendorNote(string DebitCode)
        {

            try
            {
                if (!string.IsNullOrEmpty(DebitCode))
                {
                    var debitNote = mmsObj.tblVendorDebitNoteMasters.SingleOrDefault(x => x.VendorDebitNo == DebitCode);
                    ViewBag.ProjectName = mmsObj.tblProjectMasters.SingleOrDefault(x => x.PRJID == debitNote.ProjectNumber).ProjectName;
                    
                  
                    var SiteGSTNo = mmsObj.tblProjectGSTNoes.SingleOrDefault(x => x.ProjectId == debitNote.ProjectNumber).GSTNo;
                    ViewBag.SiteGSTIN = SiteGSTNo;
                    var AddressDetails = mmsObj.tblBillingAddresses.Where(m => m.ProGSTIN == SiteGSTNo).FirstOrDefault();
                    ViewBag.SiteAddress = AddressDetails.Address + "," + AddressDetails.City + "," + AddressDetails.State;

                   var vendorDetails = mmsObj.tblVendorMasters.Where(x => x.VendorID == debitNote.SupplierNo).SingleOrDefault();

                    ViewBag.VendorName = vendorDetails.Name;
                    ViewBag.VendorAddress = vendorDetails.Address;
                    ViewBag.VendorGSTNo = vendorDetails.GST_NO;

                    ViewBag.VendorStateName = mmsObj.tblStates.SingleOrDefault(x => x.StateID == vendorDetails.State).StateName;
                   // ViewBag.VendorStateCode = mmsObj.tblBillingAddresses.SingleOrDefault(x => x.ProGSTIN == vendorDetails.GST_NO).StateCode;

                    ViewBag.PONo = debitNote.PurchaseOrderNo;
                    ViewBag.PODate = mmsObj.TbIndentPurchaseOrder_GST.SingleOrDefault(x => x.PurchaseOrderNo == debitNote.PurchaseOrderNo).POReleaseDate.Value.ToShortDateString();
                    ViewBag.DebitNoteDate = Convert.ToDateTime(debitNote.VendorDebitDate).ToString("MM/dd/yyyy");
                    ViewBag.DebitNoteCode = debitNote.VendorDebitNo;
                    VendorDebitNoteViewModel model = new VendorDebitNoteViewModel();
                    var itemData = procedure.GetVendorItemByDebitCode(DebitCode);
                    model.Items = itemData;

                    if (debitNote != null)
                    {
                        //var uid = debitNote.UId;
                        //var debitNoteItems = mmsObj.tblVendorDebitNoteChilds.Where(x => x.MUId == uid).ToList();
                        return PartialView("_PrintDebitNote", model);
                    }
                }
            }
            catch (Exception ex)
            {
                return View("_EmptyView");
            }
            return View("_EmptyView");


        }


        public ActionResult GetPO_details(string id,bool ViewOnly=false)
        {

            WordToText print = new WordToText();
            string empId = Session["EmpID"].ToString();

            id = id?.Replace("#", "/");
            Session["POId"] = id.ToString();



            var a = mmsObj.tblVendorDebitNoteMasters.FirstOrDefault(x => x.VendorDebitNo == id);
            ViewBag.DebitNoteCode = id;
            ViewBag.DebitNoteDate = Convert.ToDateTime(a.VendorDebitDate).ToShortDateString();

            string PurReq = mmsObj.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == a.PurchaseOrderNo).FirstOrDefault().IndentRefNo;
            int? PurchaseTypeIdSel = mmsObj.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReq).FirstOrDefault().PurchasePIC_Type;

            var poDetails = procedure.GetChallanNoAndDateByPONo(a.PurchaseOrderNo, a.ProjectNumber, a.GRNNo).SingleOrDefault();
            if (poDetails != null)
            {
                ViewBag.ChallanNo = poDetails.ChallanNo;
                ViewBag.ChallanDate = poDetails.ChallanDate;
            }


            var SiteGSTNo = mmsObj.tblProjectGSTNoes.SingleOrDefault(x => x.ProjectId == a.ProjectNumber).GSTNo;
            ViewBag.SiteGSTIN = SiteGSTNo;
            var BillingAddressDetails = mmsObj.tblBillingAddresses.Where(m => m.ProGSTIN == SiteGSTNo).FirstOrDefault();
            ViewBag.BillingAddress = BillingAddressDetails.Address + "," + BillingAddressDetails.City + "," + BillingAddressDetails.State;


            var units = "";
            if (id != null)
            {
                var querys = (from mas in mmsObj.tblVendorDebitNoteMasters
                              join chld in mmsObj.tblVendorDebitNoteChilds
                             // Both anonymous types should have exact same number of properties having same name and datatype
                             // on new { a = (int?)mas.UId } equals new { a = chld.MUId }
                             on mas.UId equals chld.MUId
                              join itemMaster in mmsObj.tblItemMasters
                              on chld.ItemNo equals itemMaster.ItemID
                              where chld.MUId == a.UId
                              select new Purchase_Order_Slip_VM
                              {
                                  SupplierNo = mas.SupplierNo,
                                  Name = mmsObj.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Name,
                                  Address = mmsObj.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Address,
                                  Vendor_country_state_city = mmsObj.tblCityLists.Where(k3 => k3.CityID == mmsObj.tblVendorMasters.Where(ct => ct.VendorID == mas.SupplierNo).FirstOrDefault().City).FirstOrDefault().CityName + ", " +
                                            mmsObj.tblStates.Where(k2 => k2.StateID == mmsObj.tblVendorMasters.Where(st => st.VendorID == mas.SupplierNo).FirstOrDefault().State).FirstOrDefault().StateName + ", " +
                                            mmsObj.tblCountries.Where(k1 => k1.CountryID == mmsObj.tblVendorMasters.Where(gr => gr.VendorID == mas.SupplierNo).FirstOrDefault().Country).FirstOrDefault().CnName,

                                  VendorTInNO = mmsObj.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().TinNo,
                                  VendorGSTNo = mmsObj.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().GST_NO,
                                  AcilTinNo = mmsObj.tblProjectTINNoes.Where(k => k.ProjectId == mas.ProjectNumber).FirstOrDefault().TINNo,
                                  AcilGSTNo = mmsObj.tblProjectGSTNoes.Where(k => k.ProjectId == mas.ProjectNumber).FirstOrDefault().GSTNo,
                                  MobileNo = mmsObj.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().MobileNo,
                                  ContactPerson = mmsObj.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().ContactPerson,

                                  //Reference = mas.Reference,

                                  // IndentRefNo = mas.IndentRefNo,
                                  PurchaseOrderNo = mas.PurchaseOrderNo,

                                  SubTotal = chld.SubTotal1,
                                  Total = mas.Total,
                                  GrandTotal = mas.GrandTotal,
                                  ProjectNo = mas.ProjectNumber,
                                  ProjectName = mmsObj.tblProjectMasters.Where(k => k.PRJID == mas.ProjectNumber).FirstOrDefault().ProjectName,
                                  Location = mmsObj.tblProjectParticularsDetailAs.Where(k => k.PRJID == mas.ProjectNumber).FirstOrDefault().Location,

                                  PurchaseOrderDate = mmsObj.TbIndentPurchaseOrder_GST.FirstOrDefault(x => x.PurchaseOrderNo == a.PurchaseOrderNo).PurchaseOrderDate,
                                  ItemNo = mmsObj.tblItemMasters.Where(y => y.ItemID == chld.ItemNo).FirstOrDefault().ItemID,
                                  ItemName = mmsObj.tblItemMasters.Where(j => j.ItemID == chld.ItemNo).FirstOrDefault().ItemName,
                                  UnitID = mmsObj.tblItemMasters.Where(l => l.ItemID == chld.ItemNo).FirstOrDefault().UnitID,
                                  UnitName = mmsObj.tblUnitMasters.Where(l => l.UnitID == mmsObj.tblItemMasters.Where(ll => ll.ItemID == chld.ItemNo).FirstOrDefault().UnitID).FirstOrDefault().UnitCode,
                                  Amount = chld.Total,
                                  Discount = chld.Discount,
                                  Qty = chld.TransferQty,
                                  Rate = chld.Rate,

                                  //  --------------------added Newely here
                                  Cartage_Type_1 = mmsObj.tblMMSCartageTypes.Where(x => x.TransId == chld.CartageTypeId).FirstOrDefault().CartageType,
                                  CR1_Total = chld.CartageAmount,
                                  T_Total = mas.Total,
                                  P_Total = chld.PackingChargesAmount,
                                  T_SubTotal1 = mas.SubTotal1,
                                  T_SubTotal2 = mas.SubTotal2,

                                  POCreatedBy = mmsObj.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().FName + " " + mmsObj.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().LName,
                                  // UserType = UserType(mas.PurchaseOrderNo)
                                  UserType = mmsObj.tblApproval_AccountType.Where(x => x.EmployeeId == empId).FirstOrDefault().TypeName,
                                  //CheckedPoLimit = mas.CheckedBeyondPOLimit,
                                  //IsReleasePOBy = mmsObj.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().FName + " " + mmsObj.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().LName,
                                  //PoApprovedBy = mmsObj.tblEmployeeMasters.Where(x => x.EmpID == mas.ModifiedBy).FirstOrDefault().FName + " " + mmsObj.tblEmployeeMasters.Where(x => x.EmpID == mas.ModifiedBy).FirstOrDefault().LName,

                                  // Added After GST.

                                  ItemToTAmt = chld.Total,
                                  SubTotal1 = chld.SubTotal1,
                                  SubTotal2 = chld.SubTotal2,
                                  PackagePercentage = chld.PackingChargesPercentage,
                                  PackingPercentageAmt = chld.PackingChargesAmount,
                                  CartageType = mmsObj.tblMMSCartageTypes.Where(x => x.TransId == chld.CartageTypeId).FirstOrDefault().CartageType,
                                  CartageTypeRate = chld.Cartage_rate,
                                  CartageAmt = chld.CartageAmount,
                                  InsurancePercentage = chld.InsuranceRate,
                                  InsurancePercentageAmt = chld.InsuranceAmount,
                                  TaxableAmount = ((chld.IGSTAmount??0)+(chld.CGSTAmount??0)+(chld.SGSTAmount??0)),
                                  CGSTPercentage = chld.CGSTPercentage,
                                  CGSTAmt = chld.CGSTAmount??0,
                                  SGSTAndUTGSTPercentage = chld.SGSTPercentage??0,
                                  SGSTAndUTGSTAmt = chld.SGSTAmount??0,
                                  IGST = chld.IGSTPercentage??0,
                                  IGSTAmt = chld.IGSTAmount??0,
                                  GrossAmount = chld.GrossAmount??0,
                                  HSNCode = mmsObj.tblItemMasters.Where(x => x.ItemID == chld.ItemNo).FirstOrDefault().HSNCode,
                                  //TotalPackingSum = chld.PackingChargesAmount,
                                  TotalCartageSum = mas.Total_Cartage,
                                  TotalInsuranceSum = mas.Total_Insurance,
                                  TotalTaxableAmountSum = mas.Total_Taxable,
                                  TotalCGSTSum = mas.Total_CGST,
                                  TotalSCGSTAndUTGSTSum = mas.Total_SGSTAndUTGST,
                                  IGSTSum = mas.Total_IGST,
                                  //TotalAmountInWords = mas.Total_NetAmountInWords,
                                  ItemDescrp = chld.Item_Description

                              }).ToList();
                //var projtn=querys.Select(k=> k.ProjectNo == )
                var data = querys;
                View_Print_Slip_ApprovalController.PrintData dataObj = new View_Print_Slip_ApprovalController.PrintData();
                dataObj.HeaderData = data.Select(x => new View_Print_Slip_ApprovalController.HeaderData()
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
                    //TotalPackingSum =
                    TotalCartageSum = Convert.ToDecimal(Math.Round((double)(x.TotalCartageSum != null ? x.TotalCartageSum : 0), 2).ToString("0.00")),
                    TotalInsuranceSum = x.TotalInsuranceSum,
                    TotalTaxableAmountSum = Convert.ToDecimal(Math.Round((double)(x.TotalTaxableAmountSum != null ? x.TotalTaxableAmountSum : 0), 2).ToString("0.00")),
                    TotalCGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalCGSTSum != null ? x.TotalCGSTSum : 0), 2).ToString("0.00")),
                    TotalSCGSTAndUTGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalSCGSTAndUTGSTSum != null ? x.TotalSCGSTAndUTGSTSum : 0), 2).ToString("0.00")),
                    IGSTSum = Convert.ToDecimal(Math.Round((double)(x.IGSTSum != null ? x.IGSTSum : 0), 2).ToString("0.00")),
                    TotalAmountInWords = x.TotalAmountInWords
                }).FirstOrDefault();


                dataObj.ItemData = data.Select(x => new View_Print_Slip_ApprovalController.ItemData
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


                dataObj.HeaderData.UId = a.UId;
                ViewBag.Reason = a.Reason;

                ViewBag.ViewOnly = ViewOnly;

                return PartialView("_PartialView_for_Print_Debit_Note", dataObj);
            }
            return HttpNotFound();

        }

    }



}