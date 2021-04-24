using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class POAndPIDetailOnPOEdit
    {
        public Nullable<decimal> UId { get; set; }
        public string PurRequisitionNo { get; set; }
        public string Date { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public string Location { get; set; }
        public string QuotationRefNo { get; set; }
        public string PONumber { get; set; }
        public string SupplierNo { get; set; }
        public string Send_To { get; set; }
        public string Vendor_Group_Id { get; set; }
        public string Reference { get; set; }
        public string SendTo { get; set; }
        public int PurchaseOrderId { get; set; }
    }
}