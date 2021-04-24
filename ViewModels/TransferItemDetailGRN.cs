using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class TransferItemDetailGRN
    {
        public int Id { get; set; }
        public string TransferNo { get; set; }
        public string TrasferProjectId { get; set; }
        public string Item_id { get; set; }
        public string Item_name { get; set; }
        public string Itemgrp_Id { get; set; }
        public string Itemgrp_Name { get; set; }
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public Nullable<decimal> Qunatity { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public int MTransId { get; set; }
        public int Ch_TransId { get; set; }
        public string TransferType { get; set; }
        public string GetOutNo { get; set; }
        public string GetOutDate { get; set; }
        public Nullable<decimal> GateReceivedQty { get; set; }
        public Nullable<decimal> StoreReceive { get; set; }
        public int GateEntryChildID { get; set; }
    }
}