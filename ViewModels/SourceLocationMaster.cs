using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class SourceLocationMaster
    {
        public int TransId { get; set; }
        public string CompanyId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string LocationId { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
       
    }
}