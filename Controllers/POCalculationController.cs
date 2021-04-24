using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.Models;
using MMS.ViewModels;
using MMS.DAL;
using MMS.Helpers;

namespace MMS.Controllers
{
    [Authorize]
    public class POCalculationController : Controller
    {
        MMSEntities obj = new MMSEntities();
        Procedure objProc = new Procedure();
        // GET: POCalculation
        public ActionResult Index()
        {
            return View();
        }



        public JsonResult GetPOData(string ProjectId, string PONO)
        {
            try
            {
                var data = Procedure.GetData<GETPOData>("GETPODataByPONo", ProjectId, PONO);
                return Json(new { Status = 1, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3, Data = "", Error = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }
        [Param]
        public ActionResult ViewPoDetails(string PONO)
        {
            try
            {

                var data = Procedure.GetData<GetPOCalculation>("GetPoDataForCalculation", PONO).ToList();
                var POData = obj.TbIndentPurchaseOrder_GST.SingleOrDefault(x => x.PurchaseOrderNo == PONO);
                ViewBag.PoNo = POData.PurchaseOrderNo;
                ViewBag.PI = POData.IndentRefNo;

                var vendorData = obj.tblVendorMasters.FirstOrDefault(x => x.VendorID == POData.SupplierNo);
                ViewBag.VendorName = vendorData.Name;
                try
                {
                    ViewBag.VendorAddress = (from a in obj.tblVendorMasters
                                             join b in obj.tblCountries on a.Country equals b.CountryID
                                             join c in obj.tblStates on a.State equals c.StateID
                                             join d in obj.tblCityLists on a.City equals d.CityID
                                             where a.VendorID == POData.SupplierNo
                                             select new { Address = a.Address + " " + d.CityName + " " + c.StateName + " " + b.CnName + " " + a.PinCode }).FirstOrDefault().Address;

                }
                catch (Exception ex)
                {
                    ViewBag.VendorAddress = "";
                }

                return View(data);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3, Data = "", Error = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }

        public string GetPO(string PO)
        {
            return ("PONO=" + PO).EncryptVar();
        }

        [HttpPost]
        public JsonResult UpdatePOItem(string PO, List<GetPOCalculation> Data)
        {
            using (var trans = obj.Database.BeginTransaction())
            {

                if (!string.IsNullOrEmpty(PO) && Data != null && Data.Count > 0)
                {
                    try
                    {

                        var POData = obj.TbIndentPurchaseOrder_GST.SingleOrDefault(x => x.PurchaseOrderNo == PO);
                        var childData = obj.TbIndentPurchaseOrderChild_GST.Where(x => x.MUId == POData.UId).ToList();

                        //save history before updating the data.

                        TbIndentPurchaseOrder_GST_History hisMaster = new TbIndentPurchaseOrder_GST_History()
                        {
                            Total = POData.Total,
                            CheckedBeyondPOLimit = POData.CheckedBeyondPOLimit,
                            CreatedBy = Session["EmpID"].ToString(),
                            CreatedDate = POData.CreatedDate,
                            Edited_Status = POData.Edited_Status,
                            FirstLevelApprv_Date = POData.FirstLevelApprv_Date,
                            FirstLevelApprv_Id = POData.FirstLevelApprv_Id,
                            FirstLevelApprv_Remarks = POData.FirstLevelApprv_Remarks,
                            FirstLevelApprv_Status = POData.FirstLevelApprv_Status,
                            GrandTotal = POData.GrandTotal,
                            IndentRefNo = POData.IndentRefNo,
                            IsPORelease = POData.IsPORelease,
                            ModifiedDate = POData.ModifiedDate,
                            POReleaseBy = POData.POReleaseBy,
                            POReleaseDate = POData.POReleaseDate,
                            POType = POData.POType,
                            ProjectNo = POData.ProjectNo,
                            PurchaseOrderDate = POData.PurchaseOrderDate,
                            PurchaseOrderId = POData.PurchaseOrderId,
                            PurchaseOrderNo = POData.PurchaseOrderNo,
                            QuotationRefNo = POData.QuotationRefNo,
                            Reference = POData.Reference,
                            Remarks = POData.Remarks,
                            SecondLevelApprv_Date = POData.SecondLevelApprv_Date,
                            SecondLevelApprv_Id = POData.SecondLevelApprv_Id,
                            SecondLevelApprv_Remarks = POData.SecondLevelApprv_Remarks,
                            SecondLevelApprv_Status = POData.SecondLevelApprv_Status,
                            SendTo = POData.SendTo,
                            Status = POData.Status,
                            SubTotal1 = POData.SubTotal1,
                            SubTotal2 = POData.SubTotal2,
                            SupplierNo = POData.SupplierNo,
                            Total_Cartage = POData.Total_Cartage,
                            Total_CGST = POData.Total_CGST,
                            Total_IGST = POData.Total_IGST,
                            Total_Insurance = POData.Total_Insurance,
                            Total_NetAmountInWords = POData.Total_NetAmountInWords,
                            Total_PAndF = POData.Total_PAndF,
                            Total_SGSTAndUTGST = POData.Total_SGSTAndUTGST,
                            Total_Taxable = POData.Total_Taxable,
                            Vendor_Group_Id = POData.Vendor_Group_Id,
                            EditRemarks = "EditNew"
                        };

                        obj.TbIndentPurchaseOrder_GST_History.Add(hisMaster);

                        if (childData != null && childData.Count > 0)
                        {
                            foreach (var item in Data)
                            {
                                var d = childData.SingleOrDefault(x => x.UId == item.UId);
                                TbIndentPurchaseOrderChild_GST_History his = new TbIndentPurchaseOrderChild_GST_History
                                {
                                    CartageAmount = d.CartageAmount,
                                    CartageTypeId = d.CartageTypeId,
                                    Cartage_rate = d.Cartage_rate,
                                    CGSTPercentage = d.CGSTPercentage,
                                    CGSTAmount = d.CGSTAmount,
                                    SGSTPercentage = d.SGSTPercentage,
                                    SGSTAmount = d.SGSTAmount,
                                    Discount = d.Discount,
                                    IGSTPercentage = d.IGSTPercentage,
                                    IGSTAmount = d.IGSTAmount,
                                    InsuranceAmount = d.InsuranceAmount,
                                    InsuranceRate = d.InsuranceRate,
                                    NewRate = d.Rate,
                                    PackingChargesAmount = d.PackingChargesAmount,
                                    PackingChargesPercentage = d.PackingChargesPercentage,
                                    Rate = d.NewRate,
                                    SubTotal1 = d.SubTotal1,
                                    SubTotal2 = d.SubTotal2,
                                    UTGSTAmount = d.UTGSTAmount,
                                    UTGSTPercentage = d.UTGSTPercentage,
                                    TaxCode = d.TaxCode,
                                    TaxRateType = d.TaxRateType,
                                    Total = d.Total,
                                    GrossAmount = d.GrossAmount,
                                    Discounted_Amount = d.Discounted_Amount,
                                    IndentId = d.IndentId,
                                    ItemNo = d.ItemNo,
                                    Item_Description = d.Item_Description,
                                    MUId = d.MUId,
                                    Qty = d.Qty,
                                    Receive = d.Receive,
                                    TotalGSTAmount = d.TotalGSTAmount,
                                    TotalGrosssAmount = d.TotalGrosssAmount,
                                    EditRemarks = item.Remark
                                };

                                obj.TbIndentPurchaseOrderChild_GST_History.Add(his);



                                POData.Total -= d.Total;
                                POData.Total_CGST -= d.CGSTAmount;
                                POData.Total_IGST -= d.IGSTAmount;
                                POData.Total_Cartage -= d.CartageAmount;
                                POData.Total_Insurance -= d.InsuranceAmount;
                                POData.Total_PAndF -= d.PackingChargesAmount;
                                POData.Total_SGSTAndUTGST -= d.SGSTAmount;
                                POData.Total_Taxable -= d.TotalGSTAmount;
                                POData.SubTotal1 -= d.SubTotal1;
                                POData.SubTotal2 -= d.SubTotal2;
                                POData.GrandTotal -= d.GrossAmount;


                                d.Discounted_Amount = item.Discounted_Amount;
                                d.CartageAmount = item.CartageAmount;
                                d.CartageTypeId = item.CartageTypeId;
                                d.Cartage_rate = item.Cartage_rate;

                                d.CGSTPercentage = item.CGSTPercentage;
                                d.CGSTAmount = item.CGSTAmount;

                                d.SGSTPercentage = item.SGSTPercentage;
                                d.SGSTAmount = item.SGSTAmount;

                                d.Discount = item.Discount;

                                d.IGSTPercentage = item.IGSTPercentage;
                                d.IGSTAmount = item.IGSTAmount;


                                d.InsuranceAmount = item.InsuranceAmount;
                                d.InsuranceRate = item.InsuranceRate;

                                d.NewRate = item.Rate;
                                d.PackingChargesAmount = item.PackingChargesAmount;
                                d.PackingChargesPercentage = item.PackingChargesPercentage;
                                d.Rate = item.NewRate;
                                d.SubTotal1 = item.SubTotal1;
                                d.SubTotal2 = item.SubTotal2;
                                d.UTGSTAmount = item.UTGSTAmount;
                                d.UTGSTPercentage = item.UTGSTPercentage;

                                d.TaxCode = item.TaxCode;
                                d.TaxRateType = item.TaxRateType;
                                d.Total = item.Total;
                                d.GrossAmount = item.GrossAmount;
                                d.TotalGSTAmount = item.CGSTAmount + item.SGSTAmount + item.IGSTAmount + item.UTGSTAmount;
                                d.TotalGrosssAmount = item.TotalGrosssAmount;

                                POData.Total += item.Total;
                                POData.Total_CGST += item.CGSTAmount;
                                POData.Total_IGST += item.IGSTAmount;
                                POData.Total_Cartage += item.CartageAmount;
                                POData.Total_Insurance += item.InsuranceAmount;
                                POData.Total_PAndF += item.PackingChargesAmount;
                                POData.Total_SGSTAndUTGST += item.SGSTAmount;
                                POData.Total_Taxable += item.TaxableAmount;
                                POData.SubTotal1 += item.SubTotal1;
                                POData.SubTotal2 += item.SubTotal2;
                                POData.GrandTotal += item.GrossAmount;
                                POData.Edited_Status = "Yes";
                                POData.ModifiedDate = DateTime.Now;

                                obj.Entry(d).State = System.Data.Entity.EntityState.Modified;


                                // to update rate in gatechild_mid and gatechild table
                                var g1 = obj.GateEntryChild_Mid.Where(x => x.StatusTypeNo == POData.PurchaseOrderNo && x.ItemNo==d.ItemNo).ToList();
                                var g2 = obj.GateEntryChilds.Where(x => x.StatusTypeNo == POData.PurchaseOrderNo && x.ItemNo == d.ItemNo).ToList();


                                foreach (var g in g1)
                                {
                                    g.Rate = item.NewRate;
                                    obj.Entry(g).State = System.Data.Entity.EntityState.Modified;
                                }

                                foreach (var g in g2)
                                {
                                    g.Rate = item.NewRate;

                                    obj.Entry(g).State = System.Data.Entity.EntityState.Modified;

                                    var d1 = obj.tblReceiveDatas.SingleOrDefault(x => x.TypeNumber == g.GateEntryNo && x.ItemId == g.ItemNo);
                                    if (d1 != null)
                                    {
                                        d1.Rate = item.NewRate;

                                        obj.Entry(d1).State = System.Data.Entity.EntityState.Modified;

                                        var d2 = obj.ns_tbl_IssueQuantity.Where(x => x.GateEntry_UId == d1.TransId && x.ItemID == g.ItemNo);
                                        if (d2 != null)
                                        {
                                            foreach (var d22 in d2)
                                            {
                                                d22.Receive_Rate = item.NewRate;
                                                obj.Entry(d22).State = System.Data.Entity.EntityState.Modified;
                                            }
                                        }
                                       
                                    }
                                    
                                }


                            }

                            POData.Total_NetAmountInWords = new WordToText().Cal(POData.GrandTotal ?? 0);
                            obj.Entry(POData).State = System.Data.Entity.EntityState.Modified;
                            obj.SaveChanges();
                            trans.Commit();
                            return Json(new { Status = 1 });
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return Json(new { Status = 3, Error = ex.Message });
                    }
                }
                trans.Rollback();
                return Json(new { Status = 2 });

            }
        }


        #region DDL
        public JsonResult BindProjectNew()
        {
            string EmpId = Session["EmpID"].ToString();
            var list = objProc.GetProjectsByEmpId1(EmpId);
            string jsonString = list.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BindCartageType()
        {
            var list = obj.tblMMSCartageTypes.Select(x => new { Text = x.CartageType, Value = x.TransId }).ToList();
            //var list = objProc.GetProjectsByEmpId1(EmpId);
            string jsonString = list.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindGST()
        {
            var list = obj.tblGST_SplitTaxMaster.Select(x => new { Text = x.TaxRateType, Value = x.TaxCode }).ToList();
            //var list = objProc.GetProjectsByEmpId1(EmpId);
            string jsonString = list.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}