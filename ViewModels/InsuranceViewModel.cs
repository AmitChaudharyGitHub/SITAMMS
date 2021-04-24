using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class InsuranceViewModel
    {
        public int TransId { get; set; }
        public string InsuranceCode { get; set; }
        public string InsuranceType { get; set; }
        public string InsuranceValue { get; set; }
        public Nullable<bool> IsInsurance { get; set; }
        public string InsuranceDescription { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string InsuranceExistanceType { get; set; }
        public string InsuranceTextValue { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}