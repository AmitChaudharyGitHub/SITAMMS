using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class POApprovalwithRemarks
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string Purchase_Order_Indent_No { get; set; }
        public string Created_Employee_Id { get; set; }
        public string PurchaseRefNo { get; set; }
        public string Vendor_Group_Id { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Project_Name { get; set; }
        public string Employee_Name { get; set; }
        public string Vendor_Name { get; set; }
        public string Updated_Employee_Id { get; set; }
        public string Send_To { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Status_Approval { get; set; }
        public string ForwordTo { get; set; }
        public string EditedStatus { get; set; }
        public Nullable<bool> IsPORelease { get; set; }
        public string POReleaseBy { get; set; }
        public Nullable<System.DateTime> POReleaseDate { get; set; }
        public string FirstLevelApprv_Id { get; set; }
        public string FirstLevelApprv_Remarks { get; set; }
        public string FirstLevelApprv_Status { get; set; }
        public Nullable<System.DateTime> FirstLevelApprv_Date { get; set; }
        public string SecondLevelApprv_Id { get; set; }
        public string SecondLevelApprv_Remarks { get; set; }
        public string SecondLevelApprv_Status { get; set; }
        public Nullable<System.DateTime> SecondLevelApprv_Date { get; set; }
    }
}