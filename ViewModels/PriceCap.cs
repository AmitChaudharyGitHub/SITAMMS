using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PriceCap
    {
        public string[] ProjectId { get; set; }
        public string ItemGroupId { get; set; }
        public string ItemId { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string EditedStatus { get; set; }
        public string RegionId { get; set; }
        public Nullable<System.DateTime> ValidUpto { get; set; }
    }
    public class PriceCap_report
    {
        public List<PriceCap_report> AddPriceCap_Reports { get; set; }
        public int Id { get; set; }
        public string[] ProjectId { get; set; }
        public string[] ProjectName { get; set; }
        public string ItemGroupId { get; set; }
        public string ItemGroupName{ get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        //public string ModifiedBy { get; set; }
        //public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string EditedStatus { get; set; }
        //public string RegionId { get; set; }
        public string EffectiveStatus { get; set; }
        public Nullable<System.DateTime> ValidUpto { get; set; }
    }

    public class SendRateNotification
    {       
        public string SendBy { get; set; }
        public string ForwordTo { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string ReforwordBy { get; set; }
        public string Status { get; set; }
    }
}