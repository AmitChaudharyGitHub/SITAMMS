using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class GateEntryChild
    {
        public decimal UId { get; set; }
        public Nullable<decimal> MUId { get; set; }
        public string GateEntryNo { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> StatusTypeId { get; set; }
        public string StatusTypeNo { get; set; }
        public Nullable<decimal> StatusChildId { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public string ItemNo { get; set; }
        public string Item { get; set; }
        public string ItemGroup { get; set; }
        public string ItemGroupNo { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Vendor { get; set; }
        public string VendorNo { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string InOut { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Unit { get; set; }
        public string UnitNo { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModyfiedBy { get; set; }
        public Nullable<System.DateTime> ModyfiedDate { get; set; }
        public Nullable<int> SessionId { get; set; }
        public Nullable<decimal> TaxPer { get; set; }
        public Nullable<decimal> DeliveryUnitCharge { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> DeliveryAmount { get; set; }
        public Nullable<decimal> AllAmount { get; set; }
        public virtual tblItemCurrentStock tblItemCurrentStock { get; set; }
        public virtual  GateEntry GateEntry { get; set; }
        public virtual TbIndentPurchaseOrderMaster TbIndentPurchaseOrderMaster { get; set; }
 




    }
    public class TbIndentPurchaseOrderMaster
    {
        public decimal UId { get; set; }
        public string ProjectNo { get; set; }
        public string SupplierNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public Nullable<int> PurchaseOrderId { get; set; }
        public Nullable<System.DateTime> PurchaseOrderDate { get; set; }
        public Nullable<int> SessionId { get; set; }
        public string IndentRefNo { get; set; }
        public string AcilTinNo { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> Vat { get; set; }
        public Nullable<decimal> VatAmount { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> SurCharge { get; set; }
        public Nullable<decimal> Cartage { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
        public string Rupees { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string SendTo { get; set; }
        public string Complete { get; set; }
        public string Status { get; set; }
        public string Forword_To { get; set; }
        public string SendFrom { get; set; }
        public string Reference { get; set; }
        public virtual ICollection<PurchaseOrderNo> PurchaseOrderNos { get; set; }
    }
    public class tblItemCurrentStock
    {
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string ProjectNo { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public string ItemGroupNo { get; set; }
    }
    public class GateEntry
    {
        public decimal UId { get; set; }
        public Nullable<decimal> MUId { get; set; }
        public string GateEntryNo { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> StatusTypeId { get; set; }
        public string StatusTypeNo { get; set; }
        public Nullable<decimal> StatusChildId { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public string ItemNo { get; set; }
        public string Item { get; set; }
        public string ItemGroup { get; set; }
        public string ItemGroupNo { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Vendor { get; set; }
        public string VendorNo { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string InOut { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Unit { get; set; }
        public string UnitNo { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModyfiedBy { get; set; }
        public Nullable<System.DateTime> ModyfiedDate { get; set; }
        public Nullable<int> SessionId { get; set; }
        public Nullable<decimal> TaxPer { get; set; }
        public Nullable<decimal> DeliveryUnitCharge { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> DeliveryAmount { get; set; }
        public Nullable<decimal> AllAmount { get; set; }
    }
}