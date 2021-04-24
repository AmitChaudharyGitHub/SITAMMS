using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class RegionMaster
    {
        public string CompanyID { get; set; }
        public string RegionID { get; set; }
        public string RegionName { get; set; }
        public string RegionCode { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        
    }
}