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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MMS.Controllers
{
    public class View_PO_Details_ByUser_CreatedController : Controller
    {
        // GET: View_PO_Details_ByUser_Created
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
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
        }
        public ActionResult Grid()
        {
            string empId = Session["EmpID"].ToString();

            var a = objmms.TbIndentPurchaseOrder_Approval_For_Print.OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
            {
                Id = xx.Id,
                Purchase_Order_Indent_No = xx.Purchase_Order_Indent_No,
                CreatedDate = xx.CreatedDate,
                Vendor_Group_Id = xx.Vendor_Group_Id,
                Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.Vendor_Group_Id).FirstOrDefault().Name,
                Send_To = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Send_To).FirstOrDefault().FName,
                Created_Employee_Id = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Created_Employee_Id).FirstOrDefault().FName
                }).ToList();
            //var ojs= objmms.tblVendorMasters.Where(b => b.VendorID == saf).First().;
            var totalRows = a.Count();

                var data = new PrinSlip_Approval_VMPrint_VM()
                {
                    TotalRows = totalRows,
                    PageSize = 1,
                    GetDetails_Approved_PO_Slip = a
                };


                if (data != null && data.TotalRows != 0)
                {


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialView_Show_PO", data);
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


        public ActionResult GetPO_details(string id)
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
            var a = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == id.ToString()).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectNo,
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectNo).First().ProjectName
            });
            ViewBag.prjtid = t;


            string PurReq = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == id.ToString()).FirstOrDefault().IndentRefNo;
            int? PurchaseTypeIdSel = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReq).FirstOrDefault().PurchasePIC_Type;




            var units = "";
            if (id != null)
            {
                var querys = (from mas in objmms.TbIndentPurchaseOrderMasters
                              join chld in objmms.TbIndentPurchaseOrderChilds
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
                                  VendorGSTNo = objmms.tblVendorMasters.Where(k=>k.VendorID == mas.SupplierNo).FirstOrDefault().GST_NO,
                                  AcilTinNo = objmms.tblProjectTINNoes.Where(k => k.ProjectId == mas.ProjectNo).FirstOrDefault().TINNo,
                                  AcilGSTNo = objmms.tblProjectGSTNoes.Where(k=>k.ProjectId == mas.ProjectNo).FirstOrDefault().GSTNo,
                                  MobileNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().MobileNo,
                                  ContactPerson = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().ContactPerson,

                                  SurCharge = mas.SurCharge,
                                  Cartage = mas.Cartage,
                                  Reference = mas.Reference,
                                  Remark = chld.Remark,
                                  IndentRefNo = mas.IndentRefNo,
                                  PurchaseOrderNo = mas.PurchaseOrderNo,
                                  Vat = chld.Vat,
                                  SubTotal = chld.SubTotal,
                                  Total = mas.Total,
                                  GrandTotal = mas.GrandTotal,
                                  ProjectNo = mas.ProjectNo,
                                  ProjectName = objmms.tblProjectMasters.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().ProjectName,
                                  Location = objmms.tblProjectParticularsDetailAs.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().Location,

                                  PurchaseOrderDate = mas.PurchaseOrderDate,
                                  ItemNo = chld.ItemNo,
                                  ItemName = objmms.tblItemMasters.Where(j => j.ItemID == chld.ItemNo).FirstOrDefault().ItemName,
                                  UnitID = objmms.tblItemMasters.Where(l => l.ItemID == chld.ItemNo).FirstOrDefault().UnitID,
                                  UnitName = objmms.tblUnitMasters.Where(l => l.UnitID == objmms.tblItemMasters.Where(ll => ll.ItemID == chld.ItemNo).FirstOrDefault().UnitID).FirstOrDefault().UnitName,
                                  Amount = chld.Item_TotalAmt,
                                  Discount = chld.Discount,
                                  Qty = chld.Qty,
                                  Rate = chld.Rate,
                                  ExciseDutyRate = mas.ExciseDutyRate,
                                  ExciseDuty = mas.ExciseDuty,
                                   //  --------------------added Newely here-------
                                  ItemDescrp = chld.Item_description,
                                  Excise_DutyType = chld.ExciseDutyType,
                                  Ex_Total = chld.ExcisepercentageAmt,
                                  Cartage_Type_1 = objmms.tblMMSCartageTypes.Where(x => x.TransId == chld.Cartage1_ID).FirstOrDefault().CartageType,
                                  Cartage_Type_2 = objmms.tblMMSCartageTypes.Where(x => x.TransId == chld.Cartage2_Id).FirstOrDefault().CartageType,
                                  CR1_Total = chld.CartageAmount,
                                  CR2_Total = chld.CartageAmount2,
                                  Tax_type = chld.Tax_Type,
                                  T_Tax = chld.Tax_Amount,
                                  Insi_Type_1 = chld.Insurance1_Type,
                                  Insi_Type_2 = chld.Insurance2_Type,
                                  Insi_Type_3 = chld.Insurance3_Type,
                                  Insi_Type_4 = chld.Insurance4_Type,
                                  T_Insi1 = chld.Insurance1Amount,
                                  T_Insi2 = chld.Insurance2_Amount,
                                  T_Insi3 =chld.Insurance3_Amount,
                                  T_Insi4 = chld.Insurance4_Amount,
                                  T_Total = mas.Total,
                                  P_Total = chld.PackingCharges,
                                  T_SubTotal1 = mas.SubTotal,
                                  T_SubTotal2 = mas.SubTotal2,
                                  T_TotTax = chld.Tax_Amount,
                                  IsExcise = chld.IsExcise,
                                  IsCartage1 = chld.IsCartage1,
                                  IsCartage2 = chld.IsCartage2,
                                  IsTax = chld.IsTax,
                                  Is_Insi1 = chld.IsInsurance1,
                                  Is_Insi2 = chld.IsInsurance2,
                                  Is_Insi3 = chld.IsInsurance3,
                                  Is_Insi4 = chld.IsInsurance4,
                                  POCreatedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().LName,
                                  // UserType = UserType(mas.PurchaseOrderNo)
                                  UserType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == empId).FirstOrDefault().TypeName,
                                  CheckedPoLimit = mas.CheckedBeyondPOLimit,
                                  IsReleasePOBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().LName,
                                  PoApprovedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == (objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(y => y.Purchase_Order_Indent_No == mas.PurchaseOrderNo && y.SecondLevelApprv_Status != null).FirstOrDefault().SecondLevelApprv_Id)).FirstOrDefault().FName +
                                                 " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == (objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(y => y.Purchase_Order_Indent_No == mas.PurchaseOrderNo && y.SecondLevelApprv_Status != null).FirstOrDefault().SecondLevelApprv_Id)).FirstOrDefault().LName



                              }).ToList();
                var data = querys;
                PrintData dataObj = new PrintData();
                dataObj.HeaderData = data.Select(x => new HeaderData
                {
                    PurchaseOrderNo = x.PurchaseOrderNo,
                    Total = x.Total,
                    Location = x.Location,
                    ContactPerson = x.ContactPerson,
                    Vendor_country_state_city = x.Vendor_country_state_city,
                    VendorTInNO = x.VendorTInNO,
                    VendorGSTNo = x.VendorGSTNo,
                    MobileNo = x.MobileNo,
                    Reference = x.Reference,
                    SurCharge = x.SurCharge,
                    Cartage = x.Cartage,
                    GrandTotal = x.GrandTotal,
                    SubTotal = x.SubTotal,
                    PurchaseOrderDate = x.PurchaseOrderDate,
                    AcilTinNo = x.AcilTinNo,
                    AcilGSTNo = x.AcilGSTNo,
                    Address = x.Address,
                    Name = x.Name,
                    ProjectName = x.ProjectName,
                    IndentRefNo = x.IndentRefNo,
                    ExciseDutyRate = x.ExciseDutyRate,
                    ExciseDuty = x.ExciseDuty,
                    Excise_DutyType = x.Excise_DutyType,
                    Ex_Total = Math.Round((decimal)data.Sum(k => k.Ex_Total), 2),
                    Cartage_Type_1 = x.Cartage_Type_1,
                    CR1_Total = Math.Round((decimal)data.Sum(k => k.CR1_Total), 2),
                    Cartage_Type_2 = x.Cartage_Type_2,
                    CR2_Total = Math.Round((decimal)data.Sum(k => k.CR2_Total), 2),
                    Insi_Type_1 = x.Insi_Type_1,
                    Insi_Type_2 = x.Insi_Type_2,
                    Insi_Type_3 = x.Insi_Type_3,
                    Insi_Type_4 = x.Insi_Type_4,
                    Tax_1 = x.Tax_type,
                    T_TotTax = Math.Round((decimal)data.Sum(k => k.T_TotTax), 2),
                    T_Insi1 = Math.Round((decimal)data.Sum(k => k.T_Insi1), 2),
                    T_Insi2 = Math.Round((decimal)data.Sum(k => k.T_Insi2), 2),
                    T_Insi3 = Math.Round((decimal)data.Sum(k => k.T_Insi3), 2),
                    T_Insi4 = Math.Round((decimal)data.Sum(k => k.T_Insi4), 2),
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
                    IsExcise = x.IsExcise,
                    IsTax = x.IsTax,
                    IsCartage1 = x.IsCartage1,
                    IsCartage2 = x.IsCartage2,
                    Is_Insi1 = x.Is_Insi1,
                    Is_Insi2 = x.Is_Insi2,
                    Is_Insi3 = x.Is_Insi3,
                    Is_Insi4 = x.Is_Insi4


                }).FirstOrDefault();
                dataObj.ItemData = data.Select(x => new ItemData { ItemName = x.ItemName,ItemDescrp=x.ItemDescrp,Remark = x.Remark, UnitName = x.UnitName, Rate = x.Rate, Discount = x.Discount, SubTotal = x.SubTotal, Vat = x.Vat, GrandTotal = x.GrandTotal, Qty = x.Qty, Amount = x.Amount, Tax_type = x.Tax_type, T_Tax = x.T_Tax }).ToList();

                ViewBag.PrintamountInWord = print.Cal(dataObj.HeaderData.GrandTotal ?? 0);
                //for genral terms and conditions


                if (PurchaseTypeIdSel >= 3)
                {
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
                                               Billing_Address = de_I.Billing_Address
                                           }).ToList();

                        var data_delvry = Specific_De;
                        ViewBag.Delevery_details = data_delvry;
                    }

                    else
                    {
                    }

                    return PartialView("_PartialView_POPrint_Preview_User_ForOutPro", dataObj);
                }
                else if (PurchaseTypeIdSel < 3)
                {
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
                                               Billing_Address = de_I.Billing_Address
                                           }).ToList();

                        var data_delvry = Specific_De;
                        ViewBag.Delevery_details = data_delvry;
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

                    return PartialView("_PartialView_POPrint_Preview_User", dataObj);


                    #endregion

                }
                else {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }



            }
            else {



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
        }
        public class ItemData
        {
            public decimal UId { get; set; }
            public string AcilTinNo { get; set; }
            public decimal? Vat { get; set; }
            public string PurchaseOrderNo { get; set; }
            public string IndentRefNo { get; set; }
            public decimal? SubTotal { get; set; }
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


            public string Tax_type { get; set; }
            public Nullable<decimal> T_Tax { get; set; }






        }
    }
}