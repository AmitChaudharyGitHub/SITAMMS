using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class ItemOpeningDeleteDetail
    {
        public int Id { get; set; }
        public string ItemId { get; set; }
        public string UnitId { get; set; }
        public string ItemGroupId { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public Nullable<decimal> IssueQty { get; set; }
        public string IsMRN { get; set; }
        public string GroupName { get; set; }
        public string UnitName { get; set; }
        public string ItemName { get; set; }
    }
}