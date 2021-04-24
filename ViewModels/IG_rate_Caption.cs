using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class IG_rate_Caption
    {
        public int TransID { get; set; }
        public string CompanyID { get; set; }
        public string GroupName { get; set; }
        public string GroupCode { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ItemGroupID { get; set; }
    }

    public class IG_rate_Caption_WebGrids
    {
        public int TotalRows { get; set; }
        public IEnumerable<IG_rate_Caption> IG_DATAS { get; set; }
        public int PageSize { get; set; }
    }
}