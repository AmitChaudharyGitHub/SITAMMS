using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{

    public class VMItemPurchaseReport
    {
        public decimal Id { get; set; }
        public string Region { get; set; }
        public string ProjectName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string MRNNo { get; set; }
        public string MRNDate { get; set; }
        public string PODate { get; set; }
        public string PONO{ get; set; }
        public string Vendor { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Rate { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string UnitName { get; set; }

    }
    public class VMItemData
    {
        public string ItemId { get; set; }
        public decimal Qty { get; set; }
    }

    public class VMDropDown
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class VMReturnQtyMaster
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string ReturnDate { get; set; }
        public string ReturnBy { get; set; }
        public int Reason { get; set; }
        public string TemporaryReturnNo { get; set; }
        public string Remarks { get; set; }
    }

    public class VMIssueReturnQty
    {
        public int Id { get; set; }
        public string IndentNo { get; set; }
        public string IssueDate { get; set; }
        public string VendorNo { get; set; }
        public string VendorName { get; set; }
        public string IssueTo { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ItemGroupId { get; set; }
        public string GroupName { get; set; }
        public decimal? IssueQuantity { get; set; }
        public string MaterialType { get; set; }
        public decimal ReturnQty { get; set; }



    }



    public class UserViewModel
    {
     
        public string CompanyID { get; set; }
        // public string ProjectID { get; set; }
        public string[] PRID { get; set; }
        public string EmpID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AccountType { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string AltEmail { get; set; }
        public string ReportingTo { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Status { get; set; }
        public string DeptID { get; set; }
        public string DesgID { get; set; }
        public Nullable<System.DateTime> DOJ { get; set; }
        public string OfficialEmpID { get; set; }
        public string EmpType { get; set; }
        public string TypeName { get; set; }
        public string[] PurType { get; set; }
    }


    public class ReturnQtyReport
    {
        public string ReturnDate { get; set; }
        public String ReturnNo { get; set; }
        public string IndentNo { get; set; }
        public string GroupName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string ReturnBy { get; set; }
        public string Remarks { get; set; }
    }

}