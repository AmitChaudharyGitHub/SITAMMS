
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class GateEntryDetailOnMRNViewModel
    {
 
      public List<GRNMod> GRN_Mod { get; set; }
    }

    public class GRNMod
    {
        public int Sno { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public string GateEntryNo { get; set; }

        public string PONumber { get; set; }
        public string ItemGroupNo { get; set; }
        public string ItemGroup { get; set; }
        public string Item { get; set; }
        public string ItemNo { get; set; }
        public string UnitNo { get; set; }
        public string Unit { get; set; }
        public string PurchaseIndentNo { get; set; }
        public Nullable<decimal> POQty { get; set; }
        public Nullable<decimal> GateEntryRecvQty { get; set; }
        public string MRN_Receive { get; set; }
        public string ChallanNo { get; set; }
        public string VehicleNo { get; set; }
        public Nullable <decimal> GateEntryChildId { get; set; }
        public Int32 GateEntryId { get; set; }
        public Nullable<decimal> POChildId { get; set; }
        public Nullable<decimal> POID { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Receive { get; set; }

        public Nullable<decimal> Item_ExcessPercentage { get; set; }
        public Nullable<decimal> Item_ExcessQuantity { get; set; }
        public int? CartageTypeId { get; set; }
        public string CartageType { get; set; }

    }

    public class InsertMRN
    {
        //public DAL.tblMaterialReceiveEntry Master { get; set; }
        //public List<DAL.tblMaterialReceiveEntryChild> Child { get; set; }

      //  public BusinessObjects.Entity.GateEntry Master { get; set; }
         public MMS.DAL.GateEntry Master { get; set; }
        public List<MMS.DAL.GateEntryChild> Child { get; set; }
        public string  ChallanNo { get; set; }
        public string ChallanDate { get; set; }
        public string VehicalNo { get; set; }

        public int? RSTNo { get; set; }

        public string EwayBillNo { get; set; }
        public string DMRDate { get; set; }
    }
}