using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PODetailHome
    {
        public int Id { get; set; }

        public decimal  UId { get; set; }
        public string ProjectNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string PODate { get; set; }
        public string POstatus { get; set; }
        public string Stage { get; set; }
        public string PurchaseType { get; set; }
        public string PurReqNo { get; set; }
        public Nullable<decimal> POValue { get; set; }
        public string Systemdate { get; set; }
        public string PurchaseStatus { get; set; }
     


    }
}