using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class TransferEWayBill
    {
        public string TransferNumber { get; set; }
        public string ProjectName { get; set; }
        public DateTime? TransferDate { get; set; }
        public string EWayBill { get; set; }
        public DateTime? EWayBillDate { get; set; }
        public decimal? Amount { get; set; }
        public string TransferType { get; set; }
    }
}