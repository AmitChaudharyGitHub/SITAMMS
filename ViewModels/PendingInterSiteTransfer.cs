using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PendingInterSiteTransfer
    {
        public Int64 Sno { get; set; }
        public Nullable<decimal> TransId { get; set; }

        public string ProjectName { get; set; }

        public string TransferProjectname { get; set; }

        public string InterTransferNumber { get; set; }

        public string VehicleType { get; set; }

        public string TranType { get; set; }

        public string VehicleNo { get; set; }

        public string TransferDate { get; set; }

        public string Status { get; set; }

        public string Statge { get; set; }
        public string CreatedDate { get; set; }
        public string Reject_Remark { get; set; }
        public DateTime? Reject_Date { get; set; }



    }
}