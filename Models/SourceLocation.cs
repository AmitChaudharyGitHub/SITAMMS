using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class SourceLocation
    {
        public int TransId { get; set; }
        public string CompanyId { get; set; }
        public string ProjectId { get; set; }
        public string LocationId { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}