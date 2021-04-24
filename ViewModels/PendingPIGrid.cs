using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PendingPIGrid
    {
        public Nullable<Int64> Sno { get; set; }
        public Nullable<decimal> UID { get; set; }
        public string PurRequisitionNo { get; set; }
        public string CreatedDate { get; set; }
        public string Date { get; set; }
        //public string SendToName { get; set; }
        //public string MasterPICStatus { get; set; }
        //public string MasterMangStatus { get; set; }
        public string SendTo { get; set; }
        public string Stage { get; set; }
        public string Status { get; set; }
        public string PurchaseType { get; set; }
        public string DeliveryDate { get; set; }
        public string ApprovedDate { get; set; }

    }



    public class DashBoardGrid
    {
        public string ProjectId { get; set; }
        public string Project { get; set; }
        public int? Pending { get; set; }
        public string PendingAt { get; set; }


    }
}