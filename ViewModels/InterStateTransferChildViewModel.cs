using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class InterStateTransferChildViewModel
    {
        public decimal TransId { get; set; }
        public Nullable<decimal> InterStateTransferMasterId { get; set; }
        public string InterStateTransferNumber { get; set; }
        public string ProjectId { get; set; }
        public string ItemGroupId { get; set; }

        public string ItemGroupName { get; set; }
        public string ItemName { get; set; }

        public string UnitCode { get; set; }

        public string HSNCode { get; set; }

        public string GIETMCode { get; set; }
        public string ItemId { get; set; }
        public Nullable<decimal> StockQty { get; set; }
        public Nullable<decimal> DeliveryQty { get; set; }
        public Nullable<decimal> BalancedQty { get; set; }
        public Nullable<decimal> DiscountedRate { get; set; }
        public Nullable<decimal> DeliveryRate { get; set; }
        public Nullable<decimal> EstimatedValue { get; set; }
        public Nullable<decimal> PICApprovalQty { get; set; }
        public Nullable<decimal> PICBalancedQty { get; set; }
        public Nullable<decimal> PICApprovalRate { get; set; }
        public Nullable<decimal> PICEstimatedValue { get; set; }
        public Nullable<decimal> StoreApprovalQty { get; set; }
        public Nullable<decimal> StoreBalancedQty { get; set; }
        public Nullable<decimal> StoreApprovalRate { get; set; }
        public Nullable<decimal> StoreAppovalEstimatedValue { get; set; }
    }
}