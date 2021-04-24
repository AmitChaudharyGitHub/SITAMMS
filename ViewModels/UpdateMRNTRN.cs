using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class UpdateMRNTRN
    {
        public Int64 Sno { get; set; }
        public decimal UId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string IntraTransferNumber { get; set; }
        public string TransferDate { get; set; }
        public string MRNDate { get; set; }
        public string MRN_No_New { get; set; }
        public string GateEntryMidNo { get; set; }
        public string GateEntryMidDate { get; set; }
      

    }
}