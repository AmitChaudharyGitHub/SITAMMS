using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class MovableStatus
    {
        public int TransId { get; set; }
        public string MovingType { get; set; }
        public string MaxMonth { get; set; }
        public string MinMonth { get; set; }
        public string CreatedBy { get; set; }
        public string IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}