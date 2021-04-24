using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class Bind_Work_Type_Master
    {
        [Required, Display(Name = "Work_type_Master")]
        public int Work_type_Master { get; set; }
    }
}