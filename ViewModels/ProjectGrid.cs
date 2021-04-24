using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class ProjectGrid
    {
        public Nullable<Int64> SNo { get; set; }

        public string NameOfProject { get; set; }
        public string Location { get; set; }
        public string DateOfAward { get; set; }
        public string NameOfRegion { get; set; }
        public string ProjectInchargeName { get; set; }
        public string StipulatedDateOfStart { get; set; }
        public string ActualDateOfStart { get; set; }
        public string StipulatedDateOfFinish { get; set; }
        public decimal OriginalContractValue { get; set; }

    }
}