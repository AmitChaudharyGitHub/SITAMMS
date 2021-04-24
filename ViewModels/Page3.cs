using MMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class IndentPurchaserOrderNew
    {
        public List<TbIndentPurchaseOrderChild> Child { get; set; }
        public List<tblSpecificTermsAndConditions_Child> Child2 { get; set; }

        public List<tblSpecific_Instructions_Child> Child3 { get; set; }

        public List<tblGenral_Terms_Conditions_Child> Child4 { get; set; }
        public TbDelivery_Details_PO Child5 { get; set; }
        public TbIndentPurchaseOrderMaster Master { get; set; }



    }

    public class IndentPurchaserOrderNew_GST
    {
        public List<TbIndentPurchaseOrderChild_GST> Child { get; set; }
        public List<tblSpecificTermsAndConditions_Child> Child2 { get; set; }

        public List<tblSpecific_Instructions_Child> Child3 { get; set; }

        public List<tblGenral_Terms_Conditions_Child> Child4 { get; set; }
        public TbDelivery_Details_PO Child5 { get; set; }
        public TbIndentPurchaseOrder_GST Master { get; set; }



    }




    public class Add_Caption_Rate_Item_group
    {
        public List<ns_Price_Cap_Enable_Item_Group> Child2 { get; set; }
    }
    public class Add_Source_Location
    {
        public List<tblSourceLocation> Child2 { get; set; }
    }

    public class AddPurchaseProjectWiseLimitValue
    {
        public MMS.ViewModels.PurchaseWiseProjectLimitViewModel LTVal {get; set;}
    }


    public class IntraTransferTaxInvoice
    {
        public List<MMS.ViewModels.IntraStateTransferTaxableChildViewModel> Child { get; set; }
        public MMS.ViewModels.IntraTransferTaxableMasterViewModel Master { get; set; }




    }


    public class IndentPurchaserOrderDraft_GST
    {
        public List<TbDraftIndentPurchaseOrderChild_GST> Child { get; set; }
        public List<tblDraftSpecificTermsAndConditions_Child> Child2 { get; set; }

        public List<tblDraftSpecific_Instructions_Child> Child3 { get; set; }

        public List<tblDraftGenral_Terms_Conditions_Child> Child4 { get; set; }
        public TbDraftDelivery_Details_PO Child5 { get; set; }
        public TbDraftIndentPurchaseOrder_GST Master { get; set; }



    }

}