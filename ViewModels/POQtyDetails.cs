using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class POQtyDetails
    {
        public Int64 SNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string PurchaseOrderDate { get; set; }
        public string SupplierName { get; set; }
        public string SupplierNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Item_description { get; set; }
        public Nullable<decimal> POQty { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public Nullable<decimal> BalanceQty { get; set; }
        
    }
}