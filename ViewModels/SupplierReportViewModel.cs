using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class SupplierReportViewModel
    {
        public Int64 SNO { get; set; }
        public Nullable<DateTime> PODate { get; set; }
       
        public string PONo { get; set; }
        public string GRNNO { get; set; }
        public string MRNNo { get; set; }
        public string ChalanNo { get; set; }
        public Nullable<DateTime> Chalandate { get; set; }
        public string VehicleNo { get; set; }
        public string ItemNo { get; set; }
        public string ItemDescription { get; set; }
        public string GroupName { get; set; }
        public string UnitCode { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> DiscountedAmt { get; set; }
        public Nullable<decimal> PandFAmt { get; set; }
        public Nullable<decimal> CartageAmt { get; set; }
        public Nullable<decimal> InsuranceAmt { get; set; }
        public string GSTType { get; set; }
        public Nullable<decimal> GSTAmt { get; set; }
        public Nullable<decimal> GrossAmt { get; set; }
        

    }
}