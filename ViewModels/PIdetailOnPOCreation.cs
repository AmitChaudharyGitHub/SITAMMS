using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PIdetailOnPOCreation
    {
        public Nullable<decimal> UId { get; set; }
        public string PurRequisitionNo { get; set; }
        public string Date { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public string Location { get; set; }
        public string QuotationRefNo { get; set; }
        public string PONumber { get; set; }
        public string DeliveryDate { get; set; }
        public string PurchaseStatus { get; set; }
        public string StateID { get; set; }
    }
}