using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogicLayer;
using MMS.DAL;
using PagedList;
using BusinessObjects.Entity;
namespace MMS_P.ViewModels
{

    public class PagedIndentModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.DAL.tblIndentRequisition> Indent { get; set; }
        public int PageSize { get; set; }
        // nested webgrid
        //public tblIndentRequisition obj_tblIndentRequisition { get; set; }
        //public List<tblIndentRequisition> detalsIndent { get; set; }

    }

    public class PagedIndentAllRecordModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<MMS.DAL.tblIndentRequisition> Indent2 { get; set; }
        public int PageSize { get; set; }

        // nested webgrid
        //public tblIndentRequisition obj_tblIndentRequisition { get; set; }
        //public List<tblIndentRequisition> detalsIndent { get; set; }

    }


    public class PagedGlobalItemModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<GlobalItemBll> GlobalItemBll { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedGlobalItemModel1
    {
        public int TotalRows { get; set; }
        public IEnumerable<BusinessObjects.Entity.tblItemMaster> GlobalItemBll { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedVendorMasterModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<VendorMasterBll> VendorMasterBll { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedVendorMasterModel1
    {
        public int TotalRows { get; set; }
        public IEnumerable<BusinessObjects.Entity.tblVendorMaster> VendorMasterBll { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedProjectMasterModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<ProjectMasterBll> ProjectMasterBll { get; set; }
        public int PageSize { get; set; }
    }

    public class PagedBlockMasterModel
    {
        public int TotalRows { get; set; }
        //public IPagedList<BlockMasterBll> BlockMasterBll { get; set; }
        public IEnumerable<BlockMasterBll> BlockMasterBll { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedBlockMasterModel1
    {
        public int TotalRows { get; set; }
        //public IPagedList<BlockMasterBll> BlockMasterBll { get; set; }
        public IEnumerable<BusinessObjects.Entity.BlockMaster> BlockMasterBll { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedFloorMasterModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<FloorMasterBll> FloorMasterBll { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedFloorMasterModel1
    {
        public int TotalRows { get; set; }
        public IEnumerable<BusinessObjects.Entity.FloorMaster> FloorMasterBll { get; set; }
        public int PageSize { get; set; }
    }
    //public class Dx1
    //{
    //    public int TotalRows { get; set; }

    //    public int PageSize { get; set; }
    //    public List<TbCashPurchaseChild> Child { get; set; }
    //    public TbCashPurchaseMaster Master { get; set; }

    //}
    public class PagedCashPurchaseMasterModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<BusinessObjects.Entity.TbCashPurchaseMaster> CashPurchaseMasterBll { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedCashPurchaseMasterDetailModel
    {
        
        public CashPurchaseMasterBll Master { get; set; }
        public int TotalRows { get; set; }
        public IEnumerable<CashPurchaseChildBll> Child { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedCashPurchaseChildModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<CashPurchaseChildBll> CashPurchaseChildBll { get; set; }
        public int PageSize { get; set; }
    }
}