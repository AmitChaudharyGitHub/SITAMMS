using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class Purchase_Order_Slip__For_Edit
    {
        public decimal UId { get; set; }
        public decimal UId_chld { get; set; }
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
        public string Delivery_Schedule { get; set; }
        //remaing qty
        public decimal? ApprovedQty { get; set; }
        //public decimal? CurrentQty { get; set; }
        public decimal? OrderedQty { get; set; }
        public decimal? DemandQty { get; set; }
    }
    public class All_Terms_And_Condition
    {
        //gtc data
        public int Id { get; set; }
        public string Header_Title { get; set; }
        [Display(Name = "GTC Description")]
        public string GTC_Description { get; set; }
        public string Purchase_Order_No { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Edited_Status { get; set; }
        public string Status { get; set; }
        public string IsActive { get; set; }
        public string ProjectId { get; set; }
        public Nullable<int> GTC_Master_ID { get; set; }

        //si data
        [Display(Name = "SI Description")]
        public string SI_Description { get; set; }             
        public Nullable<int> SI_Master_ID { get; set; }
       //stc data
        public string HeaderTitle { get; set; }
        [Display(Name = "STC Description")]
        public string STC_Description { get; set; }       
        public Nullable<int> STC_Master_ID { get; set; }
    }
    public class PrintData_PO_Edit
    {
        public HeaderData_PO_Edit HeaderData { get; set; }
        public List<ItemData_PO_Edit> ItemData { get; set; }
        public List<Genral_term_conditon_Chld_For_Edit> GTC_Edit_Data { get; set; }
        public List<Specific_Instructions_TC_Chld_For_Edit> SI_Edit_Data { get; set; }
        public List<Specific_Termss_And_conditions_Chld_For_Edit> STC_Edit_Data { get; set; }
    }
    public class HeaderData_PO_Edit
    {
        public decimal UId { get; set; }
        public string SupplierNo { get; set; }
        public string AcilTinNo { get; set; }
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
        public string Delivery_Schedule { get; set; }
    }
    public class ItemData_PO_Edit
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
        //remaing qty
        public decimal? ApprovedQty { get; set; }
        // public decimal? CurrentQty { get; set; }   
        public decimal? OrderedQty { get; set; }
        public decimal? DemandQty { get; set; }
    }

    public class Genral_term_conditon_Chld_For_Edit
    {
        public int Id { get; set; }
        public string Header_Title { get; set; }
        [Display(Name = "GTC Description")]
        public string GTC_Description { get; set; }
        public string Purchase_Order_No { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Edited_Status { get; set; }
        public string Status { get; set; }
        public string IsActive { get; set; }
        public string ProjectId { get; set; }
        public Nullable<int> GTC_Master_ID { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public class Specific_Instructions_TC_Chld_For_Edit
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string Purchase_Order_No { get; set; }
        public string Header_Title { get; set; }
        [Display(Name = "SI Description")]
        public string SI_Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Edited_Status { get; set; }
        public string Status { get; set; }
        public string IsActive { get; set; }
        public Nullable<int> SI_Master_ID { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public class Specific_Termss_And_conditions_Chld_For_Edit
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string Purchase_Order_No { get; set; }
        public string HeaderTitle { get; set; }
        [Display(Name = "STC Description")]
        public string STC_Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Edited_Status { get; set; }
        public string Status { get; set; }
        public string IsActive { get; set; }
        public Nullable<int> STC_Master_ID { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public class DeliveryPo_details_Chld_For_Edit
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
    }
}