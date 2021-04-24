using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PurchaseWiseProjectLimitViewModel
    {
        public Int64 SNO { get; set; }
        public string EmengencyPurchaseType { get; set; }
        public string ProjectId { get; set; }
        public string EmengencyLimitValue { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string NormalPurchaseType { get; set; }
        public string NormalLimitValue { get; set; }
        public string ProjectName { get; set; }
        public string EmergencyType { get; set; }
        public string Normaltype { get; set; }
    }
}