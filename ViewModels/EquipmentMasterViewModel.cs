using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class EquipmentMasterViewModel
    {
        public Nullable<long> RowNumber { get; set; }
        public string ItemGroupId { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string EquipmentName { get; set; }
        public string OwnerName { get; set; }
        public string EquipmentRegNo { get; set; }
        public string ProjectId { get; set; }

        public string ProjectName { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string ManufactureName { get; set; }
        public string BodyColor { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public string EquipmentStatus { get; set; }
        public string FuelType { get; set; }
        public string EquipmentCondition { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public string CompanyName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}