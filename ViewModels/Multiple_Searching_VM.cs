using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class Multiple_Searching_VM
    {
        //issue details below
        public int Id { get; set; }
        public string ProjectID { get; set; }      
        public string IndentNo { get; set; }
        public string EmployeeID { get; set; }      
        public string ItemGroupID { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public Nullable<decimal> IssueQuantity { get; set; }
        public Nullable<decimal> BalanceQuantity { get; set; }

        //OPENING  details below
        //public int Id { get; set; }     
        public string ItemGroup { get; set; }
        public string ItemNo { get; set; }
        public string ProjectNo { get; set; }

        public Nullable<decimal> Opening { get; set; }
        //public Nullable<decimal> Rate { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }     
        //public Nullable<decimal> Closing { get; set; }
        //public Nullable<decimal> SystemQty { get; set; }
        //public Nullable<decimal> PhysicalQty { get; set; }
        //public Nullable<decimal> DiffFromSystemQty { get; set; }
        //public Nullable<decimal> WastagePercent { get; set; }
        //public Nullable<decimal> WastageQty { get; set; }
        //public Nullable<decimal> ActualQtyAfterWastage { get; set; }      
        //public string OpeningType { get; set; }
        //public Nullable<decimal> TotalPurchase { get; set; }
        //public Nullable<decimal> TotalIssue { get; set; }
        //public Nullable<decimal> TheoriticalQty { get; set; }
        //public Nullable<decimal> PStockValue { get; set; }
        //public Nullable<decimal> TStockValue { get; set; }
        //public Nullable<decimal> DiffFromTheoritical { get; set; }
        //public Nullable<decimal> LastOpening { get; set; }






        //RECIEVE  details below
        public decimal UId { get; set; }
        public string ItemNos { get; set; }
        public string ItemGroupNo { get; set; }
        public Nullable<System.DateTime> CreatedDaterecieved { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
    }

    public class VM_ViewTo_AllItems
    {
        public int TotalRows { get; set; }
        public IEnumerable<Multiple_Searching_VM> itemsde { get; set; }
        public int PageSize { get; set; }
    }
}