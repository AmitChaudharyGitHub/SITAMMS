using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class Employee
    {
        [Required, Display(Name = "State")]
        public int State { get; set; }

        [Required, Display(Name = "City")]
        public int City { get; set; }
    }
}