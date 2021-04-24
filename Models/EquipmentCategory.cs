using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class EquipmentCategory
    {
        public List<EquipmentCategory> Category_Item { get; set; }
        public int TransId { get; set; }
        public string Category { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}