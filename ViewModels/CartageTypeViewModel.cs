using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class CartageTypeViewModel
    {
        public int TransId { get; set; }
        public string CartageType { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}