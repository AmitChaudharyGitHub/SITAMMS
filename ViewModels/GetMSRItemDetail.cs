using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GetMSRItemDetail
    {
        public Int64 SNo { get; set; }
        public Nullable<decimal> UId { get; set; }
        public string ItemNo { get; set; }
        public string Item { get; set; }
        public string Unit { get; set; }
        public string UnitNo { get; set; }
        public Nullable<decimal> MRN_Receive { get; set; }
        
    }
}