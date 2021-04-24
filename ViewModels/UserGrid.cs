using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class UserGrid
    {
        public Nullable<Int32> SNo { get; set; }

        public string ProjectName { get; set; }
        public string EmployeeName { get; set; }
        public string CnName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get;set;}
        public string ReportingToName { get; set; }
        public string DepartmentName { get; set; }
        public string Designation { get; set; }
        public string  DOJ { get; set; }
        public string OfficialEmpID { get; set; }

    }
}