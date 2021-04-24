using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class ItemIssueReport
    {
        public int Id { get; set; }
        public string IssueNo { get; set; }
        public Nullable<DateTime> Issuedate { get; set; }
        public string reqIntendentNo { get; set; }
        public Nullable<DateTime> Indentdate { get; set; }
        public string IssueTo { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public Nullable<decimal> Quantity { get; set; }

    }
}