using MMS.DAL;
using MMS.Helpers;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class View_PO_Details_ByUser_Created_GSTController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        // GET: View_PO_Details_ByUser_Created_GST
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();

            return View();
        }

        public class Getdata
        {
            public int Id { get; set; }
            public string ProjectId { get; set; }
            public string Purchase_Order_Indent_No { get; set; }
            public string Created_Employee_Id { get; set; }
            public string CreatedBY { get; set; }
            public string PurchaseRefNo { get; set; }
            public string Vendor_Group_Id { get; set; }
            public string Remark { get; set; }
            public string Status { get; set; }
            public string Project_Name { get; set; }
            public string Employee_Name { get; set; }
            public string Vendor_Name { get; set; }
            public string Updated_Employee_Id { get; set; }
            public string Send_To { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public Nullable<System.DateTime> ModifiedDate { get; set; }
            public string Status_Approval { get; set; }
            public string ForwordTo { get; set; }
            public Int32? QuotID { get; set; }
        }
        public ActionResult Grid()
        {
            string empId = Session["EmpID"].ToString();

            //var a = objmms.TbIndentPurchaseOrder_Approval_For_Print.OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
            //{
            //    Id = xx.Id,
            //    Purchase_Order_Indent_No = xx.Purchase_Order_Indent_No,
            //    CreatedDate = xx.CreatedDate,
            //    Vendor_Group_Id = xx.Vendor_Group_Id,
            //    Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.Vendor_Group_Id).FirstOrDefault().Name,
            //    Send_To = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Send_To).FirstOrDefault().FName,
            //    Created_Employee_Id = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Created_Employee_Id).FirstOrDefault().FName
            //}).ToList();

            var DivisionId = Repos.GetUserDivision();

            var a = objmms.TbIndentPurchaseOrder_GST.Where(x=>x.DivisionId==DivisionId).OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
            {
               // Id = xx.Id,
                Purchase_Order_Indent_No = xx.PurchaseOrderNo,
                QuotID = objmms.tblMMSQuotations.Where(x => x.PurchaseReqNo == xx.IndentRefNo).FirstOrDefault().TransId,
                CreatedDate = xx.CreatedDate,
                Vendor_Group_Id = xx.Vendor_Group_Id,
                Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.SupplierNo).FirstOrDefault().Name,
                Send_To = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SendTo).FirstOrDefault().FName,
                Created_Employee_Id = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.CreatedBy).FirstOrDefault().FName
            }).ToList();


            //var ojs= objmms.tblVendorMasters.Where(b => b.VendorID == saf).First().;
            var totalRows = a.Count();

            var data = new PrinSlip_Approval_VMPrint_VM_GST()
            {
                TotalRows = totalRows,
                PageSize = 1,
                GetDetails_Approved_PO_Slip = a
            };


            if (data != null && data.TotalRows != 0)
            {


                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PartialDetailGrid", data);
                }

                else
                {
                    return PartialView("_EmptyView");
                }

            }

            else
            {
                return View("_EmptyView");
            }

        }


        public ActionResult GetPO_details_GST(string id)
        {

            //ViewBag.prjtid = new SelectList(unitOfWork.tblProjectRepository.Get(), "PRJID", "ProjectName");
            WordToText print = new WordToText();
            string empId = Session["EmpID"].ToString();
            //var a = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            //var t = a.Select(x => new SelectListItem
            //{
            //    Value = x.ProjectID.ToString(),
            //    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            //});
            //ViewBag.prjtid = t;
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
            ViewBag.PurDate = objmms.PurchaseRequisitionMasters.SingleOrDefault(x => x.PurRequisitionNo == PurReq).Date.Value;

            var units = "";
            if (id != null)
            {
                var querys = (from mas in objmms.TbIndentPurchaseOrder_GST
                              join chld in objmms.TbIndentPurchaseOrderChild_GST
                             // Both anonymous types should have exact same number of properties having same name and datatype
                             // on new { a = (int?)mas.UId } equals new { a = chld.MUId }
                             on mas.UId equals chld.MUId into pchld
                             from pch in pchld.DefaultIfEmpty()
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
                                  Email = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Email,
                                  // SurCharge = mas.SurCharge,

                                  Reference = mas.Reference,

                                  IndentRefNo = mas.IndentRefNo,
                                  PurchaseOrderNo = mas.PurchaseOrderNo,

                                  SubTotal = pch.SubTotal1,
                                  Total = mas.Total,
                                  GrandTotal = mas.GrandTotal,
                                  ProjectNo = mas.ProjectNo,
                                  ProjectName = objmms.tblProjectMasters.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().ProjectName,
                                  Location = objmms.tblProjectParticularsDetailAs.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().Location,

                                  PurchaseOrderDate = mas.PurchaseOrderDate,
                                  ItemNo = objmms.tblItemMasters.Where(y => y.ItemID == pch.ItemNo).FirstOrDefault().GITEMCode,
                                  ItemName = objmms.tblItemMasters.Where(j => j.ItemID == pch.ItemNo).FirstOrDefault().ItemName,
                                  UnitID = objmms.tblItemMasters.Where(l => l.ItemID == pch.ItemNo).FirstOrDefault().UnitID,
                                  UnitName = objmms.tblUnitMasters.Where(l => l.UnitID == objmms.tblItemMasters.Where(ll => ll.ItemID == pch.ItemNo).FirstOrDefault().UnitID).FirstOrDefault().UnitCode,
                                  Amount = pch.Total,
                                  Discount = pch.Discount,
                                  Qty = pch.Qty,
                                  Rate = pch.NewRate,
                                 

                                  //  --------------------added Newely here-------

                                  Cartage_Type_1 = objmms.tblMMSCartageTypes.Where(x => x.TransId == pch.CartageTypeId).FirstOrDefault().CartageType,
                                  CR1_Total = pch.CartageAmount,
                                  T_Total = mas.Total,
                                  P_Total = pch.PackingChargesAmount,
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

                                  ItemToTAmt = pch.Total,
                                  SubTotal1 = pch.SubTotal1,
                                  SubTotal2 = pch.SubTotal2,
                                  PackagePercentage = pch.PackingChargesPercentage,
                                  PackingPercentageAmt = pch.PackingChargesAmount,
                                  CartageType = objmms.tblMMSCartageTypes.Where(x => x.TransId == pch.CartageTypeId).FirstOrDefault().CartageType,
                                  CartageTypeRate = pch.Cartage_rate,
                                  CartageAmt = pch.CartageAmount,
                                  InsurancePercentage = pch.InsuranceRate,
                                  InsurancePercentageAmt = pch.InsuranceAmount,
                                  TaxableAmount = pch.SubTotal2,
                                  CGSTPercentage = pch.CGSTPercentage,
                                  CGSTAmt = pch.CGSTAmount,
                                  SGSTAndUTGSTPercentage = pch.SGSTPercentage,
                                  SGSTAndUTGSTAmt = pch.SGSTAmount,
                                  IGST = pch.IGSTPercentage,
                                  IGSTAmt = pch.IGSTAmount,
                                  GrossAmount = pch.GrossAmount,
                                  HSNCode = objmms.tblItemMasters.Where(x => x.ItemID == pch.ItemNo).FirstOrDefault().HSNCode,
                                  TotalPackingSum = mas.Total_PAndF,
                                  TotalCartageSum = mas.Total_Cartage,
                                  TotalInsuranceSum= mas.Total_Insurance,
                                  TotalTaxableAmountSum = mas.SubTotal2,
                                  TotalCGSTSum=mas.Total_CGST,
                                  TotalSCGSTAndUTGSTSum = mas.Total_SGSTAndUTGST,
                                  IGSTSum=mas.Total_IGST,
                                  TotalAmountInWords=mas.Total_NetAmountInWords,
                                  ItemDescrp = pch.Item_Description,
                                  TCSRate = pch.TCS_Rate,
                                  TCSAmount = pch.TotalTCSAmount,
                                  TotalTCS = mas.Total_TCS,
                                  Tax_type = pch.TaxRateType,
                                  TCSActive = mas.TCSActive != null ? mas.TCSActive.Value : false

                              }).ToList();
                var data = querys;
                PrintData dataObj = new PrintData();
                dataObj.HeaderData = data.Select(x => new HeaderData
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
                    TotalAmountInWords = x.TotalAmountInWords,
                    OrderFor=x.PODescription,
                    Email=x.Email,
                    TotalTCSAmt = Convert.ToDecimal(Math.Round((double)(x.TotalTCS != null ? x.TotalTCS : 0), 2).ToString("0.00")),
                    TCSActive = x.TCSActive

                }).FirstOrDefault();

                ViewBag.OrderFor = data.Select(s => s.PODescription);

                try
                {

                    dataObj.ItemData = data.Select(x => new ItemData
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
                        HSNCode = x.HSNCode,
                        TaxType = x.Tax_type,
                        TCSRate = x.TCSRate.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSRate, 2).ToString("0.00")) : new decimal(0.00),
                        TCSAmount = x.TCSAmount.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSAmount, 2).ToString("0.00")) : new decimal(0.00),

                    }).ToList();
                }
                catch (Exception ex)
                {
                    dataObj.ItemData = null;
                }

                ViewBag.PrintamountInWord = print.Cal(dataObj.HeaderData.GrandTotal ?? 0);
                //for genral terms and conditions

                    #region

                    if (id != null)
                    {
                        var Genral_T_and_C = (from gtcs in objmms.tblGenral_Terms_Conditions_Child

                                              where gtcs.Purchase_Order_No == id && gtcs.Status == "Enable" && gtcs.IsActive == "Yes"

                                              select new Genral_term_conditon_Chld
                                              {
                                                  GTC_Description = gtcs.GTC_Description
                                              }).ToList();
                        var data_gtc = Genral_T_and_C;
                        ViewBag.Gtc_details = data_gtc;
                    }
                    else
                    {
                    }


                    //for Specific Instructions 
                    if (id != null)
                    {
                        var Specific_I = (from Spe_I in objmms.tblSpecific_Instructions_Child

                                          where Spe_I.Purchase_Order_No == id && Spe_I.Status == "Enable" && Spe_I.IsActive == "Yes"
                                          select new Specific_Instructions_TC_Chld
                                          {
                                              SI_Description = Spe_I.SI_Description
                                          }).ToList();
                        var data_si = Specific_I;
                        ViewBag.Si_details = data_si;
                    }

                    else
                    {
                    }

                    //for delivery details code
                    if (id != null)
                    {
                    var Specific_De = (from de_I in objmms.TbDelivery_Details_PO

                                       where de_I.Purchase_order_No == id
                                       select new DeliveryPo_details_Chld
                                       {
                                           Delivery_Schedule = de_I.Delivery_Schedule,
                                           Delivery_Address = de_I.Delivery_Address,
                                           Contact_Person_Name = de_I.Contact_Person_Name,
                                           Contact_Person_Mobile = de_I.Contact_Person_Mobile,
                                           Billing_Address = de_I.Billing_Address,
                                           DivType = de_I.ContactDivType
                                               //EPPerson = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.PersonName).FirstOrDefault(),
                                               //EPContact = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.ContactNumber).FirstOrDefault()
                                           }).ToList();

                        var data_delvry = Specific_De;
                        ViewBag.Delevery_details = data_delvry;

                    if (Specific_De.Count > 0)
                    {
                        int? purDivison = Specific_De[0].DivType;
                        if (purDivison != null)
                        {
                            var officeAddr = objmms.tbl_OfficeAddress.Where(s => s.Division == purDivison).Select(s => s).FirstOrDefault();
                            ViewBag.OfficeAddress = officeAddr.OfficeAddress;
                            ViewBag.OfficeMobile = officeAddr.Mobile;
                            ViewBag.OfficeContactName = officeAddr.ContactPerson;
                        }
                    }

                }

                    else
                    {
                    }





                    //for Specific  terms and conditions
                    if (id != null)
                    {
                        var Specific_T_and_C = (from SpecificT_C in objmms.tblSpecificTermsAndConditions_Child

                                                where SpecificT_C.Purchase_Order_No == id && SpecificT_C.Status == "Enable" && SpecificT_C.IsActive == "Yes"
                                                select new Specific_Termss_And_conditions_Chld
                                                {
                                                    STC_Description = SpecificT_C.STC_Description
                                                }).ToList();
                        var data_stc = Specific_T_and_C;
                        ViewBag.Stc_details = data_stc;
                    }
                    else
                    {
                    }

                    return PartialView("_PartialView_POPrint_Preview_User_GST", dataObj);


                    #endregion




            }
            else
            {



                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public class PrintData
        {
            public HeaderData HeaderData { get; set; }
            public List<ItemData> ItemData { get; set; }
        }
        public class HeaderData
        {
            public decimal UId { get; set; }
            public string AcilTinNo { get; set; }
            public string AcilGSTNo { get; set; }
            public string VendorGSTNo { get; set; }
            public string PurchaseOrderNo { get; set; }
            public string IndentRefNo { get; set; }
            public string ProjectName { get; set; }
            public string CreatedBy { get; set; }

            [Column(TypeName = "date")]
            [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
            public DateTime? CreatedDate { get; set; }
            public string ProjectNo { get; set; }
            public Nullable<System.DateTime> PurchaseOrderDate { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Reference { get; set; }
            public Nullable<decimal> SurCharge { get; set; }
            public Nullable<decimal> Cartage { get; set; }
            public decimal? SubTotal { get; set; }
            public decimal? Total { get; set; }
            public decimal? GrandTotal { get; set; }

            public string MobileNo { get; set; }
            public string ContactPerson { get; set; }
            public string Location { get; set; }
            public Nullable<decimal> ExciseDutyRate { get; set; }
            public Nullable<decimal> ExciseDuty { get; set; }

            public string Excise_DutyType { get; set; }
            public string Cartage_Type_1 { get; set; }
            public string Cartage_Type_2 { get; set; }
            public string Insi_Type_1 { get; set; }
            public string Tax_1 { get; set; }
            public string Insi_Type_2 { get; set; }
            public string Insi_Type_3 { get; set; }
            public string Insi_Type_4 { get; set; }
            public Nullable<decimal> T_Total { get; set; }
            public Nullable<decimal> P_Total { get; set; }
            public Nullable<decimal> Ex_Total { get; set; }
            public Nullable<decimal> CR1_Total { get; set; }
            public Nullable<decimal> T_SubTotal1 { get; set; }
            public Nullable<decimal> T_Insi1 { get; set; }



            public Nullable<decimal> T_TotTax { get; set; }
            public Nullable<decimal> CR2_Total { get; set; }
            public Nullable<decimal> T_SubTotal2 { get; set; }
            public Nullable<decimal> T_Insi2 { get; set; }
            public Nullable<decimal> T_Insi3 { get; set; }
            public Nullable<decimal> T_Insi4 { get; set; }

            public string IsReleasePOBy { get; set; }
            public string PoCreatedBy { get; set; }
            public string UserType { get; set; }
            public string CheckedPoLimit { get; set; }
            public string PoApprovedBy { get; set; }
            public string IsCartage1 { get; set; }
            public string IsCartage2 { get; set; }
            public string IsExcise { get; set; }

            public string IsTax { get; set; }
            public string Is_Insi1 { get; set; }
            public string Is_Insi2 { get; set; }
            public string Is_Insi3 { get; set; }
            public string Is_Insi4 { get; set; }
            public string Vendor_country_state_city { get; set; }
            public string VendorTInNO { get; set; }
            //New 
            public decimal? TotalPackingSum { get; set; }
            public decimal? TotalCartageSum { get; set; }
            public decimal? TotalInsuranceSum { get; set; }
            public decimal? TotalTaxableAmountSum { get; set; }
            public decimal? TotalCGSTSum { get; set; }
            public decimal? TotalSCGSTAndUTGSTSum { get; set; }
            public decimal? IGSTSum { get; set; }
            public string TotalAmountInWords { get; set; }
            public string OrderFor { get; set; }
            public string Email { get; set; }
            public Decimal? TotalTCSAmt { get; set; }
            public bool TCSActive { get; set; }
        }
        public class ItemData
        {
            public decimal UId { get; set; }
            public string AcilTinNo { get; set; }
            public decimal? Vat { get; set; }
            public string PurchaseOrderNo { get; set; }
            public string IndentRefNo { get; set; }
            public decimal? SubTotal1 { get; set; }
            public decimal? SubTotal2 { get; set; }
            public decimal? ItemToTAmt { get; set; }
            public decimal? Total { get; set; }
            public decimal? GrandTotal { get; set; }
            public string CreatedBy { get; set; }
            [Column(TypeName = "date")]
            [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
            public DateTime? CreatedDate { get; set; }
            public string ProjectNo { get; set; }
            public Nullable<System.DateTime> PurchaseOrderDate { get; set; }
            public Nullable<decimal> MUId { get; set; }
            public string ItemNo { get; set; }
            public Nullable<decimal> Qty { get; set; }
            public Nullable<decimal> Rate { get; set; }
            public decimal? Amount { get; set; }
            public decimal? Discount { get; set; }

            public string ItemName { get; set; }
            public string HSNCode { get; set; }


            public string ProjectName { get; set; }

            public string UnitID { get; set; }
            public string UnitName { get; set; }

            //for vendor info
            public string Reference { get; set; }
            public string Remark { get; set; }
            public string SupplierNo { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string ItemDescrp { get; set; }

            //added
            public decimal? PackagePercentage { get; set; }
            public decimal? PackingPercentageAmt { get; set; }
            public string CartageType { get; set; }
            public decimal? CartageTypeRate { get; set; }
            public decimal? CartageAmt { get; set; }
            public decimal? InsurancePercentage { get; set; }
            public decimal? InsurancePercentageAmt { get; set; }

            public decimal? TaxableAmount { get; set; }
            public decimal? CGSTPercentage { get; set; }
            public decimal? SGSTAndUTGSTPercentage { get; set; }
            public decimal? IGST { get; set; }

            public decimal? CGSTAmt { get; set; }
            public decimal? SGSTAndUTGSTAmt { get; set; }
            public decimal? IGSTAmt { get; set; }

            public decimal? GrossAmount { get; set; }
            public decimal? TCSRate { get; set; }
            public decimal? TCSAmount { get; set; }
            public string TaxType { get; set; }






        }

    }
}