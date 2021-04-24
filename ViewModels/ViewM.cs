using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{

  
    public class DropDownListItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

    public class GETPOData
    {
        public string PurchaseOrderNo { get; set; }
        public string Name { get; set; }
        public string PurchaseOrderDate { get; set; }
        public string Status { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public decimal? ReceivedQty { get; set; }

    }
    public class VMPOReport
    {
        public decimal? UId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string PurchaseOrderDate { get; set; }
        public string VendorName { get; set; }
        public string PODescription { get; set; }
        public decimal? GrossAmount { get; set; }
        public string Status { get; set; }
        public string Received { get; set; }
    }


        public class GetPOCalculation
    {
        public string PurchaseOrderNo { get; set; }
        public decimal? UId { get; set; }
        public string ItemName { get; set; }
        public string ItemID { get; set; }
        public string UnitName { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Rate { get; set; }        
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public decimal? NewRate { get; set; }
        public decimal? PackingChargesPercentage { get; set; }
        public decimal? PackingChargesAmount { get; set; }
        public string CartageType { get; set; }
        public decimal? Cartage_rate { get; set; }
        public decimal? CartageAmount { get; set; }
        public decimal? InsuranceRate { get; set; }
        public decimal? InsuranceAmount { get; set; }
        public decimal? TaxableAmount { get; set; }        
        public string TaxRateType { get; set; }
        public decimal? CGSTPercentage { get; set; }
        public decimal? SGSTPercentage { get; set; }
        public decimal? IGSTPercentage { get; set; }
        public decimal? CGSTAmount { get; set; }
        public decimal? SGSTAmount { get; set; }
        public decimal? IGSTAmount { get; set; }
        public decimal? GrossAmount { get; set; }
        public int? CartageTypeId { get; set; }
        public string TaxCode { get; set; }
        public decimal? SubTotal1 { get; set; }
        public decimal? SubTotal2 { get; set; }
        public decimal? UTGSTAmount { get; set; }
        public decimal? UTGSTPercentage { get; set; }
        public decimal? Discounted_Amount { get; set; }
        public decimal? TotalGrosssAmount { get; set; }
        public string Remark { get; set; }
    }

   


    public class VMGetPOItemDetails
    {
        public decimal? UId { get; set; }
        public decimal MUId { get; set; }
        public string ItemName { get; set; }
        public string ItemNo { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Receive { get; set; }
        public decimal? BalanceQty { get; set; }
        public decimal? Rate { get; set; }
        public string Status { get; set; }
        public string ReasonForCloserName { get; set; }
        public string Remarks { get; set; }
    }

}