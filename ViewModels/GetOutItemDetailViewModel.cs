using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GetOutItemDetailViewModel
    {
        public int Id { get; set; }
        public decimal IntraStateTransferMasterId { get; set; }
        public string IntraTRansferNo { get; set; }
        public string ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemGroupId { get; set; }
        public string ItemGrpName { get; set; }
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public Nullable<decimal> StockQty { get; set; }
        public Nullable<decimal> DeliveryQty { get; set; }
        public Nullable<decimal> BalancedQty { get; set; }
        public Nullable<decimal> DiscountedRate { get; set; }
        public Nullable<decimal> DeliveryRate { get; set; }
        

    }
}