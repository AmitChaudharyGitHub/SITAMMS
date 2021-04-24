using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class MMSGSTView
    {

        public int TransId { get; set; }
        public string TaxType { get; set; }
        public string TaxSlabCode { get; set; }
        public string TaxSlab { get; set; }
        public Nullable<decimal> TaxSlabPercentage { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
    }
}