using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GetDetailOnNewDMR
    {
        public Int64 SNO { get; set; }
        public string MRN { get; set; }
        public string ProjectNo { get; set; }
        public string MRNCreatedDAte { get; set; }
        public string GateEntryNo { get; set; }
        public string GateEntryCreatedDate { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public string PONO { get; set; }
        public string PODate { get; set; }
        public string Vendor { get; set; }

        public string Address { get; set; }
        public string VehicleNo { get; set; }
        public string EWayBill { get; set; }
        public string EWayDate { get; set; }
        public bool TCSActive { get; set; }
    }
}