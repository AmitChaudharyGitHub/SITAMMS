using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PendingAndPartialGRN
    {
        public int Id { get; set; }
        public string GRN { get; set; }
        public string VendorName { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string PurReqNo { get; set; }
        public string PODate { get; set; }
        public string PIDate { get; set; }
        public decimal Uid { get; set; }
        

    }
}