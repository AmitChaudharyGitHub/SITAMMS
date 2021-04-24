using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class ConversationMappingViewModel
    {
        public int TransId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string SourceLocationId { get; set; }
        public string SourceLocationName { get; set; }
        public string ItemGroupId { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string UnitFromId { get; set; }
        public string UnitConversionId { get; set; }
        public string UnitFromName { get; set; }
        public string UnitConversationName { get; set; }
        public Nullable<decimal> UnitConversationRate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Status { get; set; }
        public string CompanyId { get; set; }
        public bool IsMapped { get; set; }
        public bool IsDeleted { get; set; }

    }
}