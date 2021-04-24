using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class UpdatedMRN
    {
        public Int64 Sno { get; set; }
        public decimal UId { get; set; }

        public string PurchaseOrderNo { get; set; }
        public string IndentRefNo { get; set; }
        public string Name { get; set; }
        public string PurchaseOrderDate { get; set; }
        public string PIDate { get; set; }
        public string MRNDate { get; set; }
        public string MRN_No_New { get; set; }
        public string GateEntryMidNo { get; set; }
        public string GateEntryMidDate { get; set; }
        public string ProjectNo { get; set; }
        public string PurchasePIC_Type { get; set; }
        public string VendorID { get; set; }
    }
}