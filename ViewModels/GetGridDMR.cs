using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GetGridDMR
    {
        public Int64 SNO { get; set; }
        public int Uid { get; set; }
        public string GroupName { get; set; }
        public string ItemName { get; set; }
        public string UnitCode { get; set; }
        public  Nullable<decimal> QTy { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> PandF { get; set; }
        public int? CartageTypeId { get; set; }
        public string CartageType { get; set; }
        public Nullable<decimal> CartageAmt { get; set; }
        public Nullable<decimal> InsuranceAmt { get; set; }
        public Nullable<decimal> CGSTPer { get; set; }
        public Nullable<decimal> SGSTAndUTGSTPer { get; set; }
        public Nullable<decimal> IGSTPer { get; set; }
        public Nullable<decimal> CGSTAmt { get; set; }
        public Nullable<decimal> SGSTAndUTGSTAmt { get; set; }
        public Nullable<decimal> IGSTAmt { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
        public string ItemNo { get; set; }
        public Nullable<decimal> DiscountPercentage { get; set; }
        public string CartageRemark { get; set; }
        public string CartageRemarkDate { get; set; }
        public string EWayBillNo { get; set; }
        public string EWayDate { get; set; }
        public Nullable<decimal> TCSPer { get; set; }
        public Nullable<decimal> TCSAmt { get; set; }
        public string TaxType { get; set; }
    }
}