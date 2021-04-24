using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class AllItemDetail
    {
        public Int64 SNo { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemGroupID { get; set; }
        public string GroupName { get; set; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }

    }
}