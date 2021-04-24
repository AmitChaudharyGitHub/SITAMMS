using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PendingMRN
    {
        public Int64 Sno { get; set; }
        public decimal UId { get; set; }
       
        public string PurchaseOrderNo { get; set; }
        public string IndentRefNo { get; set; }
        public string Name { get; set; }
        public string PurchaseOrderDate { get; set; }
        public string PIDate { get; set; }
        public string GRNDate { get; set; }
        public string GateEntryNo { get; set; }
        public string VendorID { get; set; }




    }
}