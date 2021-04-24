using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class ExciseViewModel
    {
        public int TransID { get; set; }
        public string EdCode { get; set; }
        public string EdType { get; set; }
        public Nullable<decimal> EdValue { get; set; }
        public Nullable<bool> IsExcise { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string ExciseStatusExistvalue { get; set; }
        public string Name { get; set; }
        public string EdNumericValue { get; set; }
    }
}