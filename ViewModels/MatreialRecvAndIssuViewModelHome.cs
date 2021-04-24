using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class MatreialRecvAndIssuViewModelHome
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string UnitCode { get; set; }
        public string ItemGroupId { get; set; }
        public Nullable<decimal> ReceiveQuantity { get; set; }
        public Nullable<decimal> IssueQuantity { get; set; }
        public Nullable<decimal> BalanceQuantity { get; set; }
       

    }
}