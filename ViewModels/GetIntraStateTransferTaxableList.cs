using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GetIntraStateTransferTaxableList
    {
        public Int64 SNo { get; set; }
        public Nullable<decimal> TransId { get; set; }
        public string ProjectId { get; set; }
        public string IntraTransferNumber { get; set; }
        public string TransferDate { get; set; }
        public string TransferProjectId { get; set; }
        public string TransferProjectName { get; set; }
        public string CreatedDate { get; set; }
        public string SendTo { get; set; }
        public string Status { get; set; }
        public string Reject_Remark { get; set; }
        public DateTime? Reject_Date { get; set; }

    }
}