using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GetProjectOpeningDateGrid
    {
        public Int64 Sno { get; set; }
        public string ProjectName { get; set; }
        public string OpeningDate { get; set; }
        public string CreatedDate { get; set; }
    }
}