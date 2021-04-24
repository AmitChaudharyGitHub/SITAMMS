using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class ReleasePOViewModel
    {
        public Int64 S_No { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string PurchaseOrderDate { get; set; }
        public string CreatedDate { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string ApprovedBY { get; set; }
        public string Remark { get; set; }
        public decimal? UId { get; set; }
        public Int32 Id { get; set; }
        public string ModifiedDate { get; set; }
        public string POReleaseBy { get; set; }
        public string POReleaseDate { get; set; }


    }
}