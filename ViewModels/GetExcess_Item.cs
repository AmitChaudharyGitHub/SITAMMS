using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GetExcess_Item
    {
        public Int32 TrandId { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }

        public string UnitName { get; set; }
        public string HSNCode { get; set; }
        public string GIETMCode { get; set; }
        public Nullable<decimal> PercentageOfExcess { get; set; }

    }

    public class GetGrid_ExcessITEMS
    {
        public int TotalRows { get; set; }
        public IEnumerable<GetExcess_Item> EX_IT { get; set; }
        public int PageSize { get; set; }
    }

}