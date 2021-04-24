using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMS.DAL;

namespace MMS.ViewModels
{
    public class IndentRequisionses
    {

      

        public int Id { get; set; }
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
        public Nullable<decimal> Approved_Quantity { get; set; }
        public Nullable<decimal> IssueQuantity { get; set; }
        public string SessionId { get; set; }
      //  public string SiteId { get; set; }
       // public Nullable<System.DateTime> CreatedDate { get; set; }
       // public Nullable<int> AutoUniID { get; set; }
       // public string Status { get; set; }
      //  public string CreatedBy { get; set; }
      //  public string IsRead { get; set; }
      //  public Nullable<int> BlockId { get; set; }
       // public string BlockName { get; set; }
       // public Nullable<int> FloorId { get; set; }
       // public string FloorName { get; set; }
        //public string IndentType { get; set; }
        //public string IndentOtherDesc { get; set; }
       // public string IndentSent { get; set; }

        public Nullable<decimal> BalanceApprovedQty_After_Issue_Qty { get; set; }
       // public int Id { get; set; }
        //public string ItemID { get; set; }
        public string StockName { get; set; }
        public Nullable<decimal> Available_Quantity { get; set; }
    }

    public class VMIndentRequisionses
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.Controllers.QuantityIssuesController.Iss> Indents { get; set; }
        public int PageSize { get; set; }
    }


    // For Re-Issue Quantity Items

    public class IndentRequisionses_Re_Issue
    {
        public int Id { get; set; }
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
        public Nullable<decimal> Approved_Quantity { get; set; }
        public Nullable<decimal> IssueQuantity { get; set; }
        public string SessionId { get; set; }
        //  public string SiteId { get; set; }
         public Nullable<System.DateTime> CreatedDate { get; set; }
        // public Nullable<int> AutoUniID { get; set; }
        // public string Status { get; set; }
        //  public string CreatedBy { get; set; }
        //  public string IsRead { get; set; }
        //  public Nullable<int> BlockId { get; set; }
        // public string BlockName { get; set; }
        // public Nullable<int> FloorId { get; set; }
        // public string FloorName { get; set; }
        //public string IndentType { get; set; }
        //public string IndentOtherDesc { get; set; }
        // public string IndentSent { get; set; }

        public Nullable<decimal> BalanceApprovedQty_After_Issue_Qty { get; set; }
        // public int Id { get; set; }
        //public string ItemID { get; set; }
        public string StockName { get; set; }
        public Nullable<decimal> Available_Quantity { get; set; }
    }

    public class VMIndentRequisionses_Re_Issue
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.Controllers.To_Rest_Indent_Item_Re_IssueController.Iss> Indents { get; set; }
        public int PageSize { get; set; }
    }






    public class Approved_IndentRequisionses
    {
        public int Id { get; set; }
        public string IndentNo { get; set; }
        public string ItemGroupID { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string UnitID { get; set; }
        public string Make { get; set; }
        public string PartNo { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        //public Nullable<decimal> BalanceQuantity { get; set; }
        //public Nullable<decimal> IssueQuantity { get; set; }
        public string SessionId { get; set; }
        public Nullable<decimal> Approved_Quantity { get; set; }
    }


    public class Approved_VMIndentRequisionses
    {
        public int TotalRows { get; set; }
        public IEnumerable<Approved_IndentRequisionses> AppIndents { get; set; }
        public int PageSize { get; set; }
    }

    //for unapproved

    public class Unapproved_IndentRequisionses
    {
        public int Id { get; set; }
        public string ItemGroupID { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string UnitID { get; set; }
        public string Make { get; set; }
        public string PartNo { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        //public Nullable<decimal> BalanceQuantity { get; set; }
        //public Nullable<decimal> IssueQuantity { get; set; }
        public string SessionId { get; set; }
        public Nullable<decimal> Approved_Quantity { get; set; }
    }

    public class Unapproved_VMIndentRequisionses
    {
        public int TotalRows { get; set; }
        public IEnumerable<Unapproved_IndentRequisionses> AppIndentss { get; set; }
        public int PageSize { get; set; }
    }


    //here code for view to quantity issues

    public class ViewTo_Issues_Quantity_Items
    {
        public int Id { get; set; }
        //public string ProjectID { get; set; }
        //public string ProjectName { get; set; }
        public string IndentNo { get; set; }
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
        public Nullable<decimal> Approved_Quantity { get; set; }
        public Nullable<decimal> IssueQuantity { get; set; }
         public Nullable<System.DateTime> CreatedDate { get; set; }
       
    }

    public class VM_ViewTo_Issues_Quantity_Items
    {
        public int TotalRows { get; set; }
        public IEnumerable<ViewTo_Issues_Quantity_Items> Issuesitems { get; set; }
        public int PageSize { get; set; }
    }


    public class Get_All_Current_Items_Stocks_Project_wise_Admin
    {
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string ProjectNo { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public string ItemGroupNo { get; set; }

    }

    public class View_Model_Get_All_Current_Items_Stocks_Project_wise_Admin
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.DAL.tblItemCurrentStock> GetItemsDetailsAdmin { get; set; }
        public int PageSize { get; set; }
    }


    //this is code for Print Slip Approval to Admin/User

    public class PrinSlip_Approval_VM
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.Controllers.Print_Slip_For_ApprovalController.Getdata> GetDetails_Approved_PO_Slip { get; set; }
        public int PageSize { get; set; }
    }

    public class PrinSlip_Approval_VMGST
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.Controllers.Print_Slip_For_ApprovalController.Getdata> GetDetails_Approved_PO_Slip { get; set; }
        public int PageSize { get; set; }
    }

    public class PrinSlip_Approval_VMPrint_VM
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.Controllers.View_PO_Details_ByUser_CreatedController.Getdata> GetDetails_Approved_PO_Slip { get; set; }
        public int PageSize { get; set; }
    }

    public class PrinSlip_Approval_VMPrint_VM_GST
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.Controllers.View_PO_Details_ByUser_Created_GSTController.Getdata> GetDetails_Approved_PO_Slip { get; set; }
        public int PageSize { get; set; }
    }













    public class PrinSlip_Approval_VM_User
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.DAL.TbIndentPurchaseOrder_Approval_For_Print> GetDetails_Approved_PO_Slip { get; set; }
        public int PageSize { get; set; }
    }


    public class PrinSlip_Approval_VM_PrintPreview_User
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.DAL.TbIndentPurchaseOrder_Approval_For_Print> GetDetails_Approved_PO_Slip { get; set; }
        public int PageSize { get; set; }
    }
    //get aal data from master table
    public class Get_All_Data_Indentpurchase_Master_VM
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.DAL.TbIndentPurchaseOrderMaster> GetDetails_PO{ get; set; }
        public int PageSize { get; set; }
    }






}