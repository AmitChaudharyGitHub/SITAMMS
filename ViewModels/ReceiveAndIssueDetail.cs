using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class ReceiveAndIssueDetail
    {
        public int ID { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public Nullable<decimal> IssueQty { get; set; }
    }
}