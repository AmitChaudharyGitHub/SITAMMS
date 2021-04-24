using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class ConversionViewModel
    {
        public int TransId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public Nullable<decimal> UnitRate { get; set; }
        public string UnitConversionCode { get; set; }
        public string UnitConversionName { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}