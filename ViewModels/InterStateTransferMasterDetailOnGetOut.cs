using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class InterStateTransferMasterDetailOnGetOut
    {
        public Nullable<decimal> InterTransferMasterTransId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectAddress { get; set; }
        public string ProjectState { get; set; }
        public string ProjectGSTIN { get; set; }
        public string TRansferProjectID { get; set; }
        public DateTime Transferdate { get; set; }
        public string TransferProjectName { get; set; }
        public string TransferProjectAddress { get; set; }
        public string TransferProjectState { get; set; }
        public string TarnsferProjectGSTIN { get; set; }
        public string ModeOfTPT { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public string EWayBillNo { get; set; }
       
        public string GRNumber { get; set; }
        public string TRansferNo { get; set; }
        public string  ReachingDateofDestination { get; set; }
        public string GateOutDate { get; set; }
        public string GateOutNumber { get; set; }
        public DateTime? E_WayBill_Date { get; set; }
    }
}