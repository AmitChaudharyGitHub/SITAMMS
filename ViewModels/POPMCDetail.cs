using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class POPMCDetail
    {
        public Int64 Sno { get; set; }
        public string SupplierNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string PurchaseOrderDate { get; set; }
        public string CreatedDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Item_description { get; set; }
        public string UnitName { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string DateofVettingOfPMC { get; set; }
        public string Status { get; set; }


       
    }
}