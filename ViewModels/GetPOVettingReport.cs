using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GetPOVettingReport
    {
        
            public int TransID { get; set; }
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
            public int TotalNoOfPOUptoLastMonth { get; set; }
            public Nullable<decimal> NetGrandTotalOfTotalNoOfPoUptoLastMonth { get; set; }
            public int TotalNoOfPODuringCurrentMonth { get; set; }
            public Nullable<decimal> NetGrandTotalOfTotalNoofPODuringCurrentMonth { get; set; }
            public int NetTotalPO { get; set; }
            public Nullable<decimal> NetCommulativeGrand { get; set; }

        

    }   
}