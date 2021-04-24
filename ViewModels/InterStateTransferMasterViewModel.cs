using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class InterStateTransferMasterViewModel
    {
        public decimal TransId { get; set; }
        public string InterTransferNumber { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string TransferProjectId { get; set; }

        public string TransferProjectName { get; set; }
        public Nullable<int> ModeofTPT { get; set; }

        public string TransporterModeName { get; set; }
        public Nullable<int> VehicleType { get; set; }

        public string VehicleOwnerTypeName { get; set; }
        public string VehicleNo { get; set; }
        public string GRNumber { get; set; }
        public string PlaceOfSupply { get; set; }
        public string E_WayBillNo { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Remarks { get; set; }
        public string ForwardToPIC { get; set; }
        public string Status { get; set; }
        public string PICApprovalStatus { get; set; }
        public string PICRemarks { get; set; }
        public string ForwardToStore { get; set; }
        public string StoreRemarks { get; set; }
        public string StoreApprovalStatus { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> PICApprovalDate { get; set; }
        public Nullable<System.DateTime> StoreApprovalDate { get; set; }
        public string ReachingDateofDestination { get; set; }
        public string GateOutDate { get; set; }
        public string GateOutNumber { get; set; }
    }
}