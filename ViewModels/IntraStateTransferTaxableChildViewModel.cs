using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class IntraStateTransferTaxableChildViewModel
    {
        public decimal TransId { get; set; }
        public Nullable<decimal> MTransId { get; set; }
        public string ItemId { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Receive { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> IntraTRansferChildId { get; set; }
        public Nullable<decimal> PackingChargesPercentage { get; set; }
        public Nullable<decimal> PackingChargesAmount { get; set; }
        public Nullable<int> CartageTypeId { get; set; }
        public Nullable<decimal> Cartage_rate { get; set; }
        public Nullable<decimal> CartageAmount { get; set; }
        public Nullable<decimal> SubTotal1 { get; set; }
        public Nullable<decimal> InsuranceRate { get; set; }
        public Nullable<decimal> InsuranceAmount { get; set; }
        public Nullable<decimal> SubTotal2 { get; set; }
        public string TaxCode { get; set; }
        public string TaxRateType { get; set; }
        public Nullable<decimal> CGSTPercentage { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> SGSTPercentage { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> IGSTPercentage { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public Nullable<decimal> TotalGSTAmount { get; set; }
        public Nullable<decimal> GrossAmount { get; set; }
        public string Item_Description { get; set; }
        public Nullable<decimal> NewRate { get; set; }
        public Nullable<decimal> Discounted_Amount { get; set; }

        public string ItemName { get; set; }

        public string HSNCode { get; set; }

        public string GIETMCode { get; set; }

        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string CartageTypeName { get; set; }


    }
}