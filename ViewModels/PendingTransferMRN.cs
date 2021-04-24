using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PendingTransferMRN
    {
        public Int64 Sno { get; set; }
        public string ProjectId { get; set; }
        public decimal UId { get; set; }
        public string IntraTransferNumber { get; set; }
        public string TransferDate { get; set; }
        public string GRNDate { get; set; }
        public string GateEntryNo { get; set; }
    }
}