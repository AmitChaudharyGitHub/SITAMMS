using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class AddAndApprovQuotation
    {

        public Nullable<decimal> UId { get; set; }
        public string PINumber { get; set; }
        public Nullable<DateTime> PIDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<decimal> PIValue { get; set; }
        public int PurchasePIC_Type { get; set; }
        public string PurchaseType { get; set; }
        public string ForwardToMang { get; set; }
        public string ApprovedBy { get; set; }

    }
}