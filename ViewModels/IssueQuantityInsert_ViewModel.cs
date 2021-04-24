using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class IssueQuantityInsert_ViewModel
    {
        //public string ProjectID { get; set; }
        //public string ProjectName { get; set; }
        //public string IndentNo { get; set; }
        //public string EmployeeID { get; set; }
        //public string EmployeeName { get; set; }
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
        //  public string SiteId { get; set; }
        // public Nullable<System.DateTime> CreatedDate { get; set; }
        // public Nullable<int> AutoUniID { get; set; }
         public string Status { get; set; }
        //  public string CreatedBy { get; set; }
        //  public string IsRead { get; set; }
        //  public Nullable<int> BlockId { get; set; }
        // public string BlockName { get; set; }
        // public Nullable<int> FloorId { get; set; }
        // public string FloorName { get; set; }
        //public string IndentType { get; set; }
        //public string IndentOtherDesc { get; set; }
        // public string IndentSent { get; set; }
       
    }
}