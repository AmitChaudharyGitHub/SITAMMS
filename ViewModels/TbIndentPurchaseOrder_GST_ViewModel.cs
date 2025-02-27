﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class TbIndentPurchaseOrder_GST_ViewModel
    {
        public decimal UId { get; set; }
        public string ProjectNo { get; set; }
        public string SupplierNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public Nullable<int> PurchaseOrderId { get; set; }
        public Nullable<System.DateTime> PurchaseOrderDate { get; set; }
        public string IndentRefNo { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> SubTotal1 { get; set; }
        public Nullable<decimal> SubTotal2 { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string SendTo { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string Vendor_Group_Id { get; set; }
        public string Edited_Status { get; set; }
        public string QuotationRefNo { get; set; }
        public Nullable<bool> CheckedBeyondPOLimit { get; set; }
        public Nullable<bool> IsPORelease { get; set; }
        public string POReleaseBy { get; set; }
        public Nullable<System.DateTime> POReleaseDate { get; set; }
        public string Remarks { get; set; }
        public string POType { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string FirstLevelApprv_Id { get; set; }
        public string FirstLevelApprv_Remarks { get; set; }
        public string FirstLevelApprv_Status { get; set; }
        public Nullable<System.DateTime> FirstLevelApprv_Date { get; set; }
        public string SecondLevelApprv_Id { get; set; }
        public string SecondLevelApprv_Remarks { get; set; }
        public string SecondLevelApprv_Status { get; set; }
        public Nullable<System.DateTime> SecondLevelApprv_Date { get; set; }
        public Nullable<decimal> Total_PAndF { get; set; }
        public Nullable<decimal> Total_Cartage { get; set; }
        public Nullable<decimal> Total_Insurance { get; set; }
        public Nullable<decimal> Total_Taxable { get; set; }
        public Nullable<decimal> Total_CGST { get; set; }
        public Nullable<decimal> Total_SGSTAndUTGST { get; set; }
        public Nullable<decimal> Total_IGST { get; set; }
        public string Total_NetAmountInWords { get; set; }
    }
}