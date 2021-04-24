using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class Purchase_Order_Slip_VM
    {
        public decimal UId { get; set; }
        public bool? IsPORelease { get; set; }
        public string AcilTinNo { get; set; }
        public string AcilGSTNo { get; set; }
        public string VendorGSTNo { get; set; }
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
        public Nullable<decimal> SurCharge { get; set; }
        public Nullable<decimal> Cartage { get; set; }
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
        public string MobileNo { get; set; }
        public string ContactPerson { get; set; }
        public string Location { get; set; }
        public Nullable<decimal> ExciseDutyRate { get; set; }
        public Nullable<decimal> ExciseDuty { get; set; }

        // new added 
        public string Excise_DutyType { get; set; }
        public string Cartage_Type_1 { get; set; }
        public string Cartage_Type_2 { get; set; }
        public string Tax_type { get; set; }
        public string Insi_Type_1 { get; set; }
        public string Insi_Type_2 { get; set; }
        public string Insi_Type_3 { get; set; }
        public string Insi_Type_4 { get; set; }
        public Nullable<decimal> T_Total { get; set; }
        public Nullable<decimal> P_Total { get; set; }
        public Nullable<decimal> Ex_Total { get; set; }
        public Nullable<decimal> CR1_Total { get; set; }
        public Nullable<decimal> T_SubTotal1 { get; set; }
        public Nullable<decimal> T_Insi1 { get; set; }
        public Nullable<decimal> T_Tax { get; set; }
       
        public Nullable<decimal> T_TotTax { get; set; }
        public Nullable<decimal> CR2_Total { get; set; }
        public Nullable<decimal> T_SubTotal2 { get; set; }
        public Nullable<decimal> T_Insi2 { get; set; }
        public Nullable<decimal> T_Insi3 { get; set; }
        public Nullable<decimal> T_Insi4 { get; set; }
        public string IsReleasePOBy { get; set; }
        public string POCreatedBy { get; set; }
        public Nullable<bool> CheckedPoLimit { get; set; }
        public string UserType { get; set; }
        public string PoApprovedBy { get; set; }
        public string ItemDescrp { get; set; }
        public string IsCartage1 { get; set; }
        public string IsCartage2 { get; set; }
        public string IsExcise { get; set; }

        public string IsTax { get; set; }
        public string Is_Insi1 { get; set; }
        public string Is_Insi2 { get; set; }
        public string Is_Insi3 { get; set; }
        public string Is_Insi4 { get; set; }
        // end here
        // Added new Item 
        public string Vendor_country_state_city { get; set; }
        public string VendorTInNO { get; set; }



        // Added Latest After GST.
        public string HSNCode { get; set; }
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

        public decimal? ItemToTAmt { get; set; }
        public decimal? SubTotal1 { get; set; }
        public decimal? SubTotal2 { get; set; }
        public decimal? TotalPackingSum { get; set; }
        public decimal? TotalCartageSum { get; set; }
        public decimal? TotalInsuranceSum { get; set; }
        public decimal? TotalTaxableAmountSum { get; set; }
        public decimal? TotalCGSTSum { get; set; }
        public decimal? TotalSCGSTAndUTGSTSum { get; set; }
        public decimal? IGSTSum { get; set; }
        public string TotalAmountInWords { get; set; }
        public string PODescription { get; set; }

        public string Email { get; set; }
        public decimal? TCSRate { get; set; }
        public decimal? TCSAmount { get; set; }
        public decimal? TotalTCS { get; set; }
        public bool TCSActive { get; set; }

    }

    public class Genral_term_conditon_Chld
    {
        public string Header_Title { get; set; }
        public string GTC_Description { get; set; }
        public string Purchase_Order_No { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Edited_Status { get; set; }
        public string Status { get; set; }
        public string IsActive { get; set; }
        public string ProjectId { get; set; }
        public Nullable<int> GTC_Master_ID { get; set; }
    }

    public class Specific_Instructions_TC_Chld
    {
        public string ProjectId { get; set; }
        public string Purchase_Order_No { get; set; }
        public string Header_Title { get; set; }
        public string SI_Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Edited_Status { get; set; }
        public string Status { get; set; }
        public string IsActive { get; set; }
        public Nullable<int> SI_Master_ID { get; set; }
    }

    public class Specific_Termss_And_conditions_Chld
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string Purchase_Order_No { get; set; }
        public string HeaderTitle { get; set; }
        public string STC_Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Edited_Status { get; set; }
        public string Status { get; set; }
        public string IsActive { get; set; }
        public Nullable<int> STC_Master_ID { get; set; }
    }

    public class DeliveryPo_details_Chld
    {
        public string ProjectId { get; set; }
        public string Purchase_order_No { get; set; }
        public string Delivery_Schedule { get; set; }
        public string Delivery_Address { get; set; }
        public string Billing_Address { get; set; }
        public string Contact_Person_Name { get; set; }
        public string Contact_Person_Mobile { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string EditStatus { get; set; }
        public string CompanyId { get; set; }
        public string Status { get; set; }
        public string StoreKeeperName { get; set; }
        public string StoreKeeperNo { get; set; }
        public string EPPerson { get; set; }
        public string EPContact { get; set; }
        public int? DivType { get; set; }
    }
}