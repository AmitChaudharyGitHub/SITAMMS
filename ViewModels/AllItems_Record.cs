using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class DashView2
    {
        public int TotalRows { get; set; }
        public IList<Child> C { get; set; }
        public int PageSize { get; set; }
    }
    public class DashView
    {
        public Master M { get; set; }
        public List<Child> C { get; set; }
    }
    public class Master
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public decimal? Qty { get; set; }
    }

    public class Child
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal? Qty { get; set; }
    }
}