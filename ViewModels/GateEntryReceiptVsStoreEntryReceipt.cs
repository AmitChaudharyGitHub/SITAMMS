using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GateEntryReceiptVsStoreEntryReceipt
    {
        public string ItemGroup { get; set; }
        public string ItemCode { get; set; }
        public string Item { get; set; }
        public string Unit { get; set; }
        public string GateEntryDate { get; set; }
        public string GateEntryNo { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public string StoreReceiptDate { get; set; }
        public string StoreReceiptNo { get; set; }
        public Nullable<decimal> StoreReceiptQty { get; set; }
        public Nullable<decimal> QtyDifference { get; set; }

        public string Vendor { get; set; }
        public string StatusTypeNo { get; set; }


    }
}