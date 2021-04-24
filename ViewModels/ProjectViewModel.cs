using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MMS.ViewModels
{
    public class ProjectViewModel
    {
        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public DateTime DateOfAward { get; set; }
        public string LOiNO { get; set; }
        public Nullable<DateTime>  LOIDate { get; set; }
        public string Region { get; set; }
        [Required]
        public string ProjectInCharge { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public DateTime StipulatedDateOfStart { get; set; }
        [Required]
        public DateTime DateofActualStart { get; set; }
        [Required]
        public DateTime StipulatedDateOfFinish { get; set; }
        [Required]
        public decimal OrignalContractValue { get; set; }
        [Required]
        public string RegionId { get; set; }
        [Required]
        public string ProjectCode { get; set; }
        [Required]
        public string StateId { get; set; }
        [Required]
        public string TINNo { get; set; }

        public string GSTNo { get; set; }

    }
}