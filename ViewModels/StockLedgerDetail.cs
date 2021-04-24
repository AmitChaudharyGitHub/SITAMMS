using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class StockLedgerDetail
    {
        public int TransId { get; set; }
        public string CreatedDate { get; set; }
        public string Datee { get; set; }
        public string UnitName { get; set; }
        public string Particulars { get; set; }
        public string IssueTo { get; set; }
        public string MRN { get; set; }
        public string IssueNo { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public Nullable<decimal> Issue_Qty { get; set; }
        public Nullable<decimal> Receive_Rate { get; set; }
        public Nullable<decimal> BalanceQty { get; set; }
        public Nullable<decimal> TransactionAmount { get; set; }
        public Nullable<decimal> NetAMount { get; set; }
    }
}