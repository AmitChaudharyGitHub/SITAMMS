using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class StockDetail
    {
        public List<StockDetail> Stock { get; set; }
        public string ProjectName { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> Quantity { get; set; }
    }
}