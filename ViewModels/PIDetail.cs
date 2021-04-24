using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PIDetail
    {
        public int TransId { get; set; }
        public string PIDate { get; set; }
        public string ItemGroup { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public Nullable<decimal> PIQty { get; set; }
        public Nullable<decimal> ApprovePIQty { get; set; }
        public string PONo { get; set; }
        public string Unit { get; set; }
    }
}