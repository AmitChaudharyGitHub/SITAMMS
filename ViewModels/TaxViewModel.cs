using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class TaxViewModel
    {
        public int TransId { get; set; }
        public string TaxCode { get; set; }
        public string TaxType { get; set; }
        public Nullable<decimal> TaxValue { get; set; }
        public Nullable<bool> IsTax { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string TaxStatusExistvalue { get; set; }
        public string Name { get; set; }
        public string TaxNumericValue { get; set; }
    }
}