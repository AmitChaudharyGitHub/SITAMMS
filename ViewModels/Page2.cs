using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogicLayer;

using PagedList;
using BusinessObjects.Entity;
using MMS.Models;
//using MMS.DAL;

namespace MMS_P.ViewModels
{

    public class PagedItemStockModel
    {
        public int TotalRows { get; set; }
        public List<VMItemCurrentStock> Master { get; set; }
        public int PageSize { get; set; }
    }
    public class GateEntryDetailModel
    {

        public List<GateEntryChild> Child { get; set; }
        public GateEntry Master { get; set; }

    }
    public class PagedGateEntryMasterModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<GateEntry> Master { get; set; }
        public int PageSize { get; set; }
    }
    public class PageVendorMasterModelPage
    {
        public int TotalRows { get; set; }
        public IEnumerable<BusinessObjects.Entity.tblPcContractorMaster> PcContractorrMasterBll { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedGateEntryChildModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<GateEntryChild> Child { get; set; }
        public int PageSize { get; set; }
    }
    public class IndentPurchaserOrderDetails
    {
        public List<VMTbIndentPurchaseOrderChild> Child { get; set; }
        public VMTbIndentPurchaseOrderMaster Master { get; set; }
    }
    public class IndentPurchaserOrderPaged
    {
        public int TotalRows { get; set; }
        public IEnumerable<VMTbIndentPurchaseOrderMaster> Master { get; set; }
        public int PageSize { get; set; }
    }
    public class IndentPurchaserOrder
    {
        public List<TbIndentPurchaseOrderChild> Child { get; set; }

        public TbIndentPurchaseOrderMaster Master { get; set; }

    }
    public class GridIndentPurchaserOrderChild
    {
        public int TotalRows { get; set; }

        public int PageSize { get; set; }
        public List<VMTbIndentPurchaseOrderChild> Child { get; set; }
        public string Vendor { get; set; }
        public string VendorNo { get; set; }
    }

    public class GridIndentPurchaserOrderChildNew
    {
        public int TotalRows { get; set; }

        public int PageSize { get; set; }
        public List<MMS.ViewModels.VMTbIndentPurchaseOrderChildViewModel> Child { get; set; }
        public string Vendor { get; set; }
        public string VendorNo { get; set; }
    }
    public class GridIndentRequisitions
    {
        public int TotalRows { get; set; }

        public int PageSize { get; set; }
        public List<PurchaseRequisitionChild> Child { get; set; }


    }
    public class GridTbCashPurchaseChild
    {
        public int TotalRows { get; set; }

        public int PageSize { get; set; }
        public List<TbCashPurchaseChild> Child { get; set; }

        public string Vendor { get; set; }
        public string VendorNo { get; set; }
    }

    public class GateEntry_UpdateChild
    {

        public GateEntry Master { get; set; }
        public List<GateEntryChild> Child { get; set; }


    }
    public class GateEntry_UpdateChild_Mid
    {

        public MMS.DAL.GateEntry_Mid Master { get; set; }
        public List<MMS.DAL.GateEntryChild_Mid> Child { get; set; }


    }
    public class Ch
    {
        public decimal UId { get; set; }

        public int Receive { get; set; }

    }
    public class Dx1
    {
        public int TotalRows { get; set; }

        public int PageSize { get; set; }
        public List<TbCashPurchaseChild> Child { get; set; }
        public TbCashPurchaseMaster Master { get; set; }

    }
    public class Dx2
    {

        public List<VMTbCashPurchaseChild> Child { get; set; }
        public VMTbCashPurchaseMaster Master { get; set; }

    }
    public class VMOST
    {

        public List<VMTbOSTTransferChild> Child { get; set; }
        public P_VMTbOSTTransferMaster Master { get; set; }

    }
    public class OST
    {

        public List<TbOSTTransferChild> Child { get; set; }
        public TbOSTTransferMaster Master { get; set; }

    }


    public class PurchaseIR
    {

        public List<PurchaseRequisitionChild> Child { get; set; }
        public PurchaseRequisitionMaster Master { get; set; }

    }

    public class PurchaseIRNew
    {
        public List<MMS.DAL.PurchaseRequisitionChild> ChildNew { get; set; }
        public PurchaseRequisitionMaster MasterNew { get; set; }

        public List<PurchaseRequisitionChildViewModel> ChildNew1 { get; set; }
    }

    public class PurchaseOutIR
    {
        public List<MMS.DAL.PurchaseRequisitionChild> ChildNew { get; set; }
        public MMS.DAL.PurchaseRequisitionMaster MasterNew { get; set; }
    }

    public class PurchaseApprovalIR
    {
        public List<MMS.DAL.PurchaseRequisitionChild> Childk { get; set; }
        public MMS.DAL.PurchaseRequisitionMaster Masterk { get; set; }
    }


    public class GetPendingPIHome
    {
        public List<MMS.ViewModels.PurchaseIndentMasterViewModel> MHome { get; set; }
    }
    public class PagedPurchaseIRMasterModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<PurchaseRequisitionMaster> Master { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedOSTMasterModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<P_VMTbOSTTransferMaster> Master { get; set; }
        public int PageSize { get; set; }
    }

    public class OSTDetails
    {
        public List<VMTbOSTTransferChild> Child { get; set; }
        public P_VMTbOSTTransferMaster Master { get; set; }
    }

    public class GridOSTChild
    {
        public int TotalRows { get; set; }

        public int PageSize { get; set; }
        public List<VMTbOSTTransferChild> Child { get; set; }

        public string ReceivingSite { get; set; }
        public string ReceivingSiteNo { get; set; }

        public string DispatchingSite { get; set; }
        public string DispatchingSiteNo { get; set; }

        public string ChallanNo { get; set; }
        public string VehicleNo { get; set; }
        public DateTime? ChallanDate { get; set; }
    }

    public class PagedPurchaseLimitMasterModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.DAL.tblPurchaseRequisitionApprovalLimitValue> MasterNew { get; set; }
        public int PageSize { get; set; }
    }
    public class GridPurchaseOrderEdit
    {
        public int TotalRows { get; set; }

        public int PageSize { get; set; }
        public List<MMS.DAL.TbIndentPurchaseOrderChild> Child { get; set; }


    }

    public class GridPurchaseOrderEdit_GST
    {
        public int TotalRows { get; set; }

        public int PageSize { get; set; }
        public List<MMS.DAL.TbIndentPurchaseOrderChild_GST> Child { get; set; }


    }

    public class IndentPurchaserOrder_EditNew
    {
        public List<MMS.DAL.TbIndentPurchaseOrderChild> Child { get; set; }

        public MMS.DAL.TbIndentPurchaseOrderMaster Master { get; set; }



    }

    public class IndentPurchaserOrder_EditNewGST
    {
        public List<MMS.DAL.TbIndentPurchaseOrderChild_GST> Child { get; set; }

        public MMS.DAL.TbIndentPurchaseOrder_GST Master { get; set; }



    }

    public class InterStateTransfer
    {
        public List<MMS.ViewModels.InterStateTransferChildViewModel> Child { get; set; }

        public MMS.ViewModels.InterStateTransferMasterViewModel Master { get; set; }
    }

    public class IntraStateTransfer
    {
        public List<MMS.ViewModels.IntraStateTransferChildViewModel> Child { get; set; }

        public MMS.ViewModels.IntraStateTransferMasterViewModel Master { get; set; }
    }

    public class Grid_IntraStateTransfer
    {
        public int TotalRows { get; set; }

        public int PageSize { get; set; }
        public List<MMS.ViewModels.IntraStateTransferChildViewModel> Child { get; set; }


    }


    public class GetDetailOnPrint
    {
        public MMS.ViewModels.IntraTransferMasterDetail Header { get; set; }

        public List<MMS.ViewModels.IntraStateTransferTaxableChildViewModel> ItemData { get; set; }
    }

    public class GetInterDetailOnPrint
    {
    public MMS.ViewModels.InterStateTransferMasterDetailOnGetOut Header { get; set; }
        
    public  List<MMS.ViewModels.InterStateTransferChildPrintViewModel> ItemData { get; set; }
   }

    public class GridGRN_TransferItemList
    {
       public List<MMS.ViewModels.TransferItemDetailGRN> Child { get; set; }
        //public string Vendor { get; set; }
        //public string VendorNo { get; set; }
    }

    // add on 28_04_2019 by sachin sir
    public class EditGRNViewModel
    {
        public MMS.DAL.GateEntry_Mid Master { get; set; }
        public List<EditGRNChildViewModel> Child { get; set; }
    }

    public class EditGRNChildViewModel {
        public decimal? ReceiveQty { get; set; }
        public decimal? StatusChildId { get; set; }
        public int GateEntryCID { get; set; }
    }
    //*********** end*************

    #region PO Draft Class

    public class DraftIndentPurchaserOrder_EditNewGST
    {
        public List<MMS.DAL.TbDraftIndentPurchaseOrderChild_GST> Child { get; set; }

        public MMS.DAL.TbDraftIndentPurchaseOrder_GST Master { get; set; }



    }

    public class GridDraftPurchaseOrderEdit_GST
    {
        public int TotalRows { get; set; }

        public int PageSize { get; set; }
        public List<MMS.DAL.TbDraftIndentPurchaseOrderChild_GST> Child { get; set; }


    }

    #endregion

}