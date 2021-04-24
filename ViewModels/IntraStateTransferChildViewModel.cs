using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class IntraStateTransferChildViewModel
    {
        public decimal TransId { get; set; }
        public Nullable<decimal> IntraStateTransferMasterId { get; set; }
        public string IntraStateTransferNumber { get; set; }
        public string ProjectId { get; set; }
        public string ItemGroupId { get; set; }
        public string ItemId { get; set; }

        public string ItemName { get; set; }
        public string UnitId { get; set; }

        public string UnitCode { get; set; }
        public Nullable<decimal> StockQty { get; set; }
        public Nullable<decimal> DeliveryQty { get; set; }
        public Nullable<decimal> BalancedQty { get; set; }
        public Nullable<decimal> DiscountedRate { get; set; }
        public Nullable<decimal> DeliveryRate { get; set; }
        public Nullable<decimal> EstimatedValue { get; set; }
    }
}