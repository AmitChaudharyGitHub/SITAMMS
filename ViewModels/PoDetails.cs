using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PoDetails
    {
        public Int64 Sno { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string PurchaseOrderDate { get; set; }
        public string SupplierNo { get; set; }
        public string SupplierName { get; set; }
        public string ItemName { get; set; }
        public string Item_description { get; set; }
        public string UnitName { get; set; }
        public Nullable<decimal> POQty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> PackingCharges { get; set; }
        public Nullable<decimal> ExcisepercentageAmt { get; set; }
        public Nullable<decimal> CartageAmount { get; set; }
        public Nullable<decimal> Tax_Amount { get; set; }
        public Nullable<decimal> InsuranceAmount { get; set; }
        public Nullable<decimal> Item_GrandTotal { get; set; }
    }
}