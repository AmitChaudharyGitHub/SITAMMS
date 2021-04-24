using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class Non_Moveing_Models
    {
        public List<Non_Moveing_Models> Non_Moveing { get; set; }
        public decimal UId { get; set; }
        public Nullable<decimal> MUId { get; set; }
        public string GateEntryNo { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> StatusTypeId { get; set; }
        public string StatusTypeNo { get; set; }
        public Nullable<decimal> StatusChildId { get; set; }
        public string BillNo { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public string ItemNo { get; set; }
        public string Item { get; set; }
        public string ItemGroup { get; set; }
        public string ItemGroupNo { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Vendor { get; set; }
        public string VendorNo { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string InOut { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Unit { get; set; }
        public string UnitNo { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModyfiedBy { get; set; }
        public Nullable<System.DateTime> ModyfiedDate { get; set; }
        public Nullable<int> SessionId { get; set; }
        public Nullable<decimal> TaxPer { get; set; }
        public Nullable<decimal> DeliveryUnitCharge { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> DeliveryAmount { get; set; }
        public Nullable<decimal> AllAmount { get; set; }
    }
}