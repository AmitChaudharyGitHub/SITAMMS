using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GRNViewModel
    {
       public List<GrnGrid> GHD { get; set; }
    }

    public class GrnGrid
    {
        public Int64 Sno { get; set; }
        public string Vendor { get; set; }
        public string ItemGroup { get; set; }

        public string Item { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public Nullable<decimal> Rate { get; set; }

        public Nullable<decimal> AllAmount { get; set; }
        public decimal UId { get; set; }
        public string ProjectNo { get; set; }
    }
}