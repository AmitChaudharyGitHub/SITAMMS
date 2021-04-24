using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class VMTbIndentPurchaseOrderChildViewModel
    {
       
        public string IndentNo { get; set; }
        public string ItemGroup { get; set; }
        public string ItemNo { get; set; }
        public decimal? Qty{ get; set; }
        public string ItemName { get; set; }
        public string Make { get; set; }
        public string PartNo { get; set; }
        public DAL.TbIndentPurchaseOrderChild_GST TbIndentPurchaseOrderChild { get; set; }
        public string Unit { get; set; }

        public decimal ? GateReceivedQty { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ? PIDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ? PODate { get; set; }

        public Nullable<decimal> Ex_Item_Pecntg { get; set; }
        public Nullable<decimal> Ex_Item_Qty { get; set; }


    }
}