using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class Quot_PIDetail
    {
        public PIMaster_Quot Pmaster { get; set; }

        public List<PIChild_Quot> Pchild { get; set; }
    }

    public class PIMaster_Quot
    {
        public Nullable<decimal> UID { get; set; }
        public string PurchaseReqNo { get; set; }

        public string PurchaseType { get; set; }

        public string ProjectName { get; set; }

        public string Quotref { get; set; }
        public string Quotremarks { get; set; }
        public string ProjectId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? PIdeliveryDate { get; set; }
        public string PurchaseStatus { get; set; }
    }

    public class PIChild_Quot
    {
        public decimal UId { get; set; }
        public Nullable<decimal> MId { get; set; }
        public string PurRequisitionNo { get; set; }

        public string ItemGroupName { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }

        public Nullable<decimal> AppQuantity { get; set; }

        public Nullable<decimal> CurrentRate { get; set; }

        public Nullable<decimal> TotalValue { get; set; }

        public string Remark { get; set; }


    }


}