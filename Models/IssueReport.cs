using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class IssueReport
    {
        public List<IssueReport> Datewise_Item { get; set; }
        public int Id { get; set; }
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string IndentNo { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string ItemGroupID { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string UnitID { get; set; }
        public string Make { get; set; }
        public string PartNo { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> BalanceQuantity { get; set; }
        public Nullable<decimal> IssueQuantity { get; set; }
        public string SessionId { get; set; }
        public string SiteId { get; set; }
        public string IssuedBy { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> AvailableQuantity { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<decimal> AfterIssue_AvlQty_Stock { get; set; }
        public string PcContractorId { get; set; }
    }
}