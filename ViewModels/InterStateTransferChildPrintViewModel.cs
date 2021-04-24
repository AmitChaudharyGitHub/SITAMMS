using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class InterStateTransferChildPrintViewModel
    {
        public decimal TransId { get; set; }
        public Nullable<decimal> InterStateTransferMasterId { get; set; }
        public string ItemId { get; set; }

        public string ItemName { get; set; }
        public string HSNCode { get; set; }
        public string UnitCode { get; set; }
        public string UnitId { get; set; }
        public Nullable<decimal> StockQty { get; set; }
        public Nullable<decimal> DeliveryQty { get; set; }
        public Nullable<decimal> StockBalancedQty { get; set; }
        public Nullable<decimal> DeliveryRate { get; set; }
        public Nullable<decimal> DiscountedRate { get; set; }
        public Nullable<decimal> PICEstimatedValue { get; set; }
    }
}