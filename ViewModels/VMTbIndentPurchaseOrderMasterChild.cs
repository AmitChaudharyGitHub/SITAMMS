using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class VMTbIndentPurchaseOrderMasterChild
    {
      
        public DateTime? CreatedDate { get; set; }
        public string CreatedName { get; set; }
        public DateTime? IndentRefNoDate { get; set; }
        public string IndentRefNoDate2 { get; set; }
        public string ProjectAddress { get; set; }
        public string ProjectName { get; set; }
        public string SendToName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierName { get; set; }
        public string SupplierTinNo { get; set; }
        public DAL.TbIndentPurchaseOrder_GST TbIndentPurchaseOrderMaster { get; set; }
    }
}