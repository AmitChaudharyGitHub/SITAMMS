using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class ItemWiseMaxMinRateViewModel
    {
        public string ItemName { get; set; }
        public string ProjectId { get; set; }
        public string ItemId { get; set; }
        public string ItemGroupID { get; set; }
        public string UnitCode { get; set; }
        public Nullable<decimal> LowestRate { get; set; }
        public Nullable<decimal> HighestRate { get; set; }
        public string LastRecvDate { get; set; }

    }
}