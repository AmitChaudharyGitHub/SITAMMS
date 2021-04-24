using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class IntraStateTransferMasterViewModel
    {
        public decimal TransId { get; set; }
        public string IntraTransferNumber { get; set; }
        public string ProjectId { get; set; }
        public string TransferProjectId { get; set; }
        public Nullable<int> ModeofTPT { get; set; }
        public Nullable<int> VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public string PlaceOfSupply { get; set; }
        public string TaxPayableOnReverseCharges { get; set; }
        public string E_WayBillNo { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}