using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GSTSplit_TaxMasterViewModel
    {
        public int TransId { get; set; }
        public string TaxSlabCode { get; set; }
        public string TaxSlab { get; set; }
        public Nullable<decimal> TaxSlabPercentage { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> UGST { get; set; }
        public Nullable<decimal> IGST { get; set; }
        public string TaxCode { get; set; }
        public string TaxRateType { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}