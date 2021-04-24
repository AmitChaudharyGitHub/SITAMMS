using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PurchaseIndentMasterViewModel
    {
        public decimal UId { get; set; }
        public Nullable<int> Id { get; set; }
        public string PurRequisitionNo { get; set; }

        public Nullable<System.DateTime> Date { get; set; }

        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
    }
}