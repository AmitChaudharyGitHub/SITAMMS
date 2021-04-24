using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels.File
{
    public class QuotDetail
    {
        public int TransId { get; set; }
        public decimal? Uid { get; set; }
        public string PurchaseReqNo { get; set; }
        public string QuotationRefNo { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string Name { get; set; }
    }
}