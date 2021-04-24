using BusinessObjects.Entity;
using DataAccessLayer;
using MMS.DAL;
using MMS.Models;
using MMS_P.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MMS.ViewModels;
using MMS.Helpers;

namespace MMS.Controllers
{
    public class GateEntriesController : Controller
    {
        FactoryManager m = new FactoryManager();
        private MMSEntities objmms = new MMSEntities();
        MSP_Model objmsp = new MSP_Model();


        int SessionId = 1;
        int SiteId = 1;
        string EmpId = "EMP0000001";
        string EmpName = "Admin";
        //  List<BusinessObjects.Entity.GateEntry> MasterList;
        List<DAL.GateEntry_Mid> MasterList;
        public GateEntriesController()
        {
            try
            {
                EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
                EmpName = System.Web.HttpContext.Current.Session["EmpName"].ToString();
                SessionId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SessionId"].ToString());
                SiteId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteId"].ToString());

            }
            catch
            {
            }
        }
        // GET: GateEntries
        public ActionResult Create()
        {
            ViewBag.EmpId = EmpId;
            //ViewBag.prjtid = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName");
            //Employee Or Forword to User Name
            //string PRJID = m.GetProjectMasterManager().Find(SiteId).PRJID;
            //ViewBag.Employee = new SelectList(m.GetEmployeeMasterManager().FindByProject(PRJID), "EmpID", "FName");

            ViewBag.Status = GetStatusListP();
            ViewBag.Time = GetTimeList();
            ViewBag.Date = DateTime.Now.Date;
            return View();
        }

        public ActionResult CreateNew(string PONo)
        {
            string PurchaseOrderNo = PONo.Replace("#", "/");
            ViewBag.PO_No = PurchaseOrderNo;
            ViewBag.EmpId = EmpId;
            decimal POID = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PurchaseOrderNo).FirstOrDefault().UId;
            ViewBag.POID = POID;
            ViewBag.PRJID = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PurchaseOrderNo).FirstOrDefault().ProjectNo;
            ViewBag.Date = DateTime.Now.Date;
            return View();
        }

        public JsonResult IsBillNoDuplicate(string BillNo)
        {
            if (BillNo != "")
            {
                BillNo = BillNo.Trim();
                try
                {
                    if (objmms.GateEntry_Mid.Any(x => x.ChallanNo.ToLower() == BillNo.ToLower()))
                    {
                        return Json(new { Status = 2 },JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Status = 1 }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Status = 3, Error=ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new {Status=2 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditGRN(int UId, int GPType)
        {

            var Data = objmms.GateEntry_Mid.SingleOrDefault(x => x.UId == UId);
            string PurchaseOrderNo = Data.StatusTypeNo;
            ViewBag.PO_No = PurchaseOrderNo;
            ViewBag.EmpId = EmpId;
            if (PurchaseOrderNo.Contains("TRN/IS"))
            {
                var poData = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == PurchaseOrderNo).FirstOrDefault();
                decimal POID = poData.TransId;
                ViewBag.POID = POID;
                ViewBag.PRJID = poData.ProjectId;
                ViewBag.PType = GPType;
            }
            else if (PurchaseOrderNo.Contains("TRN/OS"))
            {

                var poData = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == PurchaseOrderNo).FirstOrDefault();
                decimal POID = poData.TransId;
                ViewBag.POID = POID;
                ViewBag.PRJID = poData.ProjectId;
                ViewBag.PType = GPType;
            }
            else
            {
                var poData = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PurchaseOrderNo).FirstOrDefault();
                decimal POID = poData.UId;
                ViewBag.POID = POID;
                ViewBag.PRJID = poData.ProjectNo;
                ViewBag.PType = GPType;
            }

            ViewBag.GRNDate = Data.Date;
            ViewBag.GRNNo = Data.GateEntryNo;
            ViewBag.ChallanNo = Data.ChallanNo;
            ViewBag.ChallanDate = Data.ChallanDate;
            ViewBag.VehicalNo = Data.VehicleNo;

            return View();
        }



        // replace method with below method on 16_04_2020
        public JsonResult GetDetailOnGateEntryPage(string PurchaseOrder, int PType = 0)
        {
            var PurchaseType = "";
            //comment code on 16_04_2020 
            //string ProjectNo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PurchaseOrder).FirstOrDefault().ProjectNo;
            // replace above code line on 16_04_2020
            /////////////////////////////////////////////////
            string PurReqNo = "";
            int? PurchaseId = 0;
            string ProjectNo = "";
            if (PurchaseOrder.Contains("TRN/IS"))
            {
                ProjectNo = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == PurchaseOrder).FirstOrDefault().ProjectId;
                PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == PType).FirstOrDefault().PurchaseType;
            }
            else if (PurchaseOrder.Contains("TRN/OS"))
            {

                ProjectNo = objmms.tblIntraStateTransferMasters.Where(x => x.IntraTransferNumber == PurchaseOrder).FirstOrDefault().ProjectId;
                PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == PType).FirstOrDefault().PurchaseType;
            }
            else
            {
                var poData = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PurchaseOrder).FirstOrDefault().ProjectNo;
                PurReqNo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PurchaseOrder).FirstOrDefault().IndentRefNo;
                PurchaseId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReqNo).FirstOrDefault().PurchasePIC_Type;
                ProjectNo = poData;
                if (PurchaseId >= 3 && PurchaseId != 7)
                {
                    PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == PurchaseId).FirstOrDefault().PurchaseType + " " + "Purchase";
                }
                else
                {
                    PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == PurchaseId).FirstOrDefault().PurchaseType;
                }
            }
            /////////////////////////////////////////////////////////
            string ProjectName = objmms.tblProjectMasters.Where(x => x.PRJID == ProjectNo).FirstOrDefault().ProjectName;
            var gateEntryNo = this.GateEntryNo(ProjectNo);

            //var PurReqNo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PurchaseOrder).FirstOrDefault().IndentRefNo;
            //int ? PurchaseId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReqNo).FirstOrDefault().PurchasePIC_Type;
            //if (PurchaseId >= 3 && PurchaseId != 7)
            //{
            //     PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == PurchaseId).FirstOrDefault().PurchaseType + " " + "Purchase";
            //}
            //else {
            //     PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == PurchaseId).FirstOrDefault().PurchaseType;
            //}


            var List = new { ProjName = ProjectName, GateEntryNo = gateEntryNo, PurchaseType = PurchaseType, PONo = PurchaseOrder };

            string jsonString = List.ToJSON();

            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        //replace method with above method on 16_04_2020
        //public JsonResult GetDetailOnGateEntryPage(string PurchaseOrder)
        //{
        //    var PurchaseType = "";
        //    string ProjectNo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PurchaseOrder).FirstOrDefault().ProjectNo;
        //    string ProjectName = objmms.tblProjectMasters.Where(x => x.PRJID == ProjectNo).FirstOrDefault().ProjectName;
        //    var gateEntryNo = this.GateEntryNo(ProjectNo);

        //    var PurReqNo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PurchaseOrder).FirstOrDefault().IndentRefNo;
        //    int ? PurchaseId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReqNo).FirstOrDefault().PurchasePIC_Type;
        //    if (PurchaseId >= 3 && PurchaseId != 7)
        //    {
        //         PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == PurchaseId).FirstOrDefault().PurchaseType + " " + "Purchase";
        //    }
        //    else {
        //         PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == PurchaseId).FirstOrDefault().PurchaseType;
        //    }


        //    var List = new { ProjName = ProjectName, GateEntryNo = gateEntryNo, PurchaseType=PurchaseType ,PONo = PurchaseOrder};

        //    string jsonString = List.ToJSON();

        //    return Json(jsonString, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetGRNNoByDateWise(string ProjectNo, string GRN_Mid_Date)
        {
             var gateEntryNo = this.GateEntryNoByDateWise(ProjectNo, Convert.ToDateTime(GRN_Mid_Date));
            var GNRNo_lst = new { GRN_NO = gateEntryNo };
            return Json(GNRNo_lst.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        private string GetGRNNoByDateWiseString(string ProjectNo, string GRN_Mid_Date)
        {
            var gateEntryNo = this.GateEntryNoByDateWise(ProjectNo, Convert.ToDateTime(GRN_Mid_Date));
            return gateEntryNo;
        }

        public ActionResult TCreateNew(string TransferNo, string TransferType)
        {
            string TransferNumber = TransferNo.Replace("#", "/");
            ViewBag.Transfer_Number = TransferNumber;
            ViewBag.EmpId = EmpId;
            ViewBag.TransferType = TransferType;
            if (TransferType == "6")
            {
                // for Intra Transfer
                decimal TransID = objmms.tblIntraTransferTaxableMasters.Where(x => x.IntraTransferNumber == TransferNumber).FirstOrDefault().TransId;
                ViewBag.TransID = TransID;
                ViewBag.PRJID = objmms.tblIntraTransferTaxableMasters.Where(x => x.IntraTransferNumber == TransferNumber).FirstOrDefault().TransferProjectId;
                ViewBag.ChalanDate = objmms.tblGetOutTransfers.Where(x => x.TransferNumber == TransferNumber).FirstOrDefault().ChalanDate;

            }
            else
            {
                // for Inter transfe
                decimal TransID = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).FirstOrDefault().TransId;
                ViewBag.TransID = TransID;
                ViewBag.PRJID = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).FirstOrDefault().TransferProjectId;
                ViewBag.ChalanDate = objmms.tblGetOutTransfers.Where(x => x.TransferNumber == TransferNumber).FirstOrDefault().ChalanDate;

            }


            ViewBag.Date = DateTime.Now.Date;
            return View();
        }

        public JsonResult GetTransferDetailOnGRNCreatePage(string TransferNumber, string TransferType)
        {
            if (TransferType == "6")
            {
                string ProjectNo = objmms.tblIntraTransferTaxableMasters.Where(x => x.IntraTransferNumber == TransferNumber).FirstOrDefault().TransferProjectId;
                string ProjectName = objmms.tblProjectMasters.Where(x => x.PRJID == ProjectNo).FirstOrDefault().ProjectName;
                string ChalaNo = objmms.tblGetOutTransfers.ToList().Where(x => x.TransferNumber == TransferNumber && x.TransferProjectId == ProjectNo).FirstOrDefault().ChalanNo;
                DateTime ChalanDate = objmms.tblGetOutTransfers.ToList().Where(x => x.TransferNumber == TransferNumber && x.TransferProjectId == ProjectNo).FirstOrDefault().ChalanDate ?? System.DateTime.Now;
                string Vehicleno = objmms.tblGetOutTransfers.ToList().Where(x => x.TransferNumber == TransferNumber && x.TransferProjectId == ProjectNo).FirstOrDefault().VehicleNo;
                var gateEntryNo = this.GateEntryNo(ProjectNo);
                int Id = Convert.ToInt32(TransferType);
                string PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == Id).FirstOrDefault().PurchaseType + " " + "Purchase";
                var List = new { ProjName = ProjectName, GateEntryNo = gateEntryNo, PurchaseType = PurchaseType, TransferNo = TransferNumber, Chalano = ChalaNo, Chalandt = ChalanDate, VehicleNumber = Vehicleno };

                return Json(List.ToJSON(), JsonRequestBehavior.AllowGet);



            }
            else if (TransferType == "5")
            {
                string ProjectNo = objmms.tblInterStateTransferMasters.Where(x => x.InterTransferNumber == TransferNumber).FirstOrDefault().TransferProjectId;
                string ProjectName = objmms.tblProjectMasters.Where(x => x.PRJID == ProjectNo).FirstOrDefault().ProjectName;
                string ChalaNo = objmms.tblGetOutTransfers.ToList().Where(x => x.TransferNumber == TransferNumber && x.TransferProjectId == ProjectNo).FirstOrDefault().ChalanNo;
                DateTime ChalanDate = objmms.tblGetOutTransfers.ToList().Where(x => x.TransferNumber == TransferNumber && x.TransferProjectId == ProjectNo).FirstOrDefault().ChalanDate ?? System.DateTime.Now;
                string Vehicleno = objmms.tblGetOutTransfers.ToList().Where(x => x.TransferNumber == TransferNumber && x.TransferProjectId == ProjectNo).FirstOrDefault().VehicleNo;
                var gateEntryNo = this.GateEntryNo(ProjectNo);
                int Id = Convert.ToInt32(TransferType);
                string PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == Id).FirstOrDefault().PurchaseType + " " + "Purchase";
                var List = new { ProjName = ProjectName, GateEntryNo = gateEntryNo, PurchaseType = PurchaseType, TransferNo = TransferNumber, Chalano = ChalaNo, Chalandt = ChalanDate, VehicleNumber = Vehicleno };

                return Json(List.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else { return null; }


        }




        public ActionResult CreateT()
        {
            ViewBag.EmpId = EmpId;
            //ViewBag.prjtid = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName");
            //Employee Or Forword to User Name
            //string PRJID = m.GetProjectMasterManager().Find(SiteId).PRJID;
            //ViewBag.Employee = new SelectList(m.GetEmployeeMasterManager().FindByProject(PRJID), "EmpID", "FName");

            ViewBag.Status = GetStatusListT();
            ViewBag.Time = GetTimeList();
            ViewBag.Date = DateTime.Now.Date;
            return View();
        }
        public ActionResult Grid(string Type ,decimal PurchaseNo,  int page = 1)
        {
            
            const int pageSize = 55;      

         

            int totalRows = 0;

            if (SessionId != 0)
            {
                if (Type == "LP")
                {
                    //List<BusinessObjects.Entity.TbCashPurchaseChild> MasterList = null;
                    //totalRows = m.GetTbCashPurchaseChildManager().FindByMasterCount(PurchaseNo);
                    //MasterList = m.GetTbCashPurchaseChildManager().FindByMaster(PurchaseNo);
                    //if (MasterList != null)
                    //{
                    //    BusinessObjects.Entity.TbCashPurchaseMaster V = m.GetTbCashPurchaseMasterManager().Find(PurchaseNo);


                    //    var data = new GridTbCashPurchaseChild()
                    //    {
                    //        TotalRows = totalRows,
                    //        PageSize = pageSize,
                    //        Child = MasterList.ToList(),
                    //        Vendor= V.VendorName,
                    //        VendorNo = V.VendorId,
                    //    };
                    //    if (Request.IsAjaxRequest())
                    //    {
                    //        return View("_GridViewLP", data);
                    //    }
                    //}

                    List<VMTbIndentPurchaseOrderChild> MasterList = null;
                    totalRows = m.GetTbIndentPurchaseOrderChildManager().FindByMasterCount(PurchaseNo);
                    //m.GetTbIndentPurchaseOrderChildManager().FindByMasterCount(PurchaseNo);
                    MasterList = m.GetTbIndentPurchaseOrderChildManager().FindByMasterVM(PurchaseNo);
                    if (MasterList != null)
                    {
                        VMTbIndentPurchaseOrderMaster V = m.GetTbIndentPurchaseOrderMasterManager().FindVM(PurchaseNo);


                        var data = new GridIndentPurchaserOrderChild()
                        {
                            TotalRows = totalRows,
                            PageSize = pageSize,
                            Child = MasterList.ToList(),

                            Vendor = V.SupplierName,
                            VendorNo = V.TbIndentPurchaseOrderMaster.SupplierNo
                        };
                        if (Request.IsAjaxRequest())
                        {
                            return View("_GridLPTest", data);
                            //  _GridViewLP return View("_GridViewLP")
                        }
                    }


                }
               else   if (Type == "IPO")
                {
                    List<VMTbIndentPurchaseOrderChild> MasterList = null;
                    totalRows = m.GetTbIndentPurchaseOrderChildManager().FindByMasterCount(PurchaseNo);
                    MasterList = m.GetTbIndentPurchaseOrderChildManager().FindByMasterVM(PurchaseNo);
                    if (MasterList != null)
                    {
                      VMTbIndentPurchaseOrderMaster  V = m.GetTbIndentPurchaseOrderMasterManager().FindVM(PurchaseNo);


                        var data = new GridIndentPurchaserOrderChild()
                        {
                            TotalRows = totalRows,
                            PageSize = pageSize,
                            Child = MasterList.ToList(),
                          
                            Vendor= V.SupplierName,
                            VendorNo = V.TbIndentPurchaseOrderMaster.SupplierNo
                        };
                        if (Request.IsAjaxRequest())
                        {
                            return View("_GridViewIPO", data);
                        }
                    }
                }

                else if (Type == "Dispatch" || Type == "Receive")
                {
                    List<VMTbOSTTransferChild> MasterList = null;
                    totalRows = m.GetTbOSTTransferChildManager().FindByMasterCount(PurchaseNo);
                    MasterList = m.GetTbOSTTransferChildManager().FindByMasterVM(PurchaseNo);
                    if (MasterList != null)
                    {
                        P_VMTbOSTTransferMaster V = m.GetTbOSTTransferMasterManager().FindVM(PurchaseNo);


                        var data = new GridOSTChild()
                        {
                            TotalRows = totalRows,
                            PageSize = pageSize,
                            Child = MasterList.ToList(),

                            ReceivingSite=V.ReceiveSiteName,
                            ReceivingSiteNo =V.ReceiveSiteNo,

                            DispatchingSite = V.DispatchSiteName,
                            DispatchingSiteNo = V.DispatchSiteNo,

                            ChallanNo=V.ChallanNo,
                            VehicleNo=V.VehicleNo,
                            ChallanDate=V.ChallanDate
                        };
                        if (Request.IsAjaxRequest())
                        {
                            if (Type == "Dispatch")
                            return View("_GridViewDispatch", data);
                            else if (Type == "Receive")
                                return View("_GridViewReceive", data);
                        }
                    }
                }

            }

            return View("_EmptyView");
        }
        // GateEntry_UpdateChild



        public ActionResult GridTest(string Type, decimal PurchaseNo, int page = 1)
        {
            const int pageSize = 55;
            int totalRows = 0;

            if (SessionId != 0)
            {
                if (Type == "Intra Transfer (Within State)")
                {

                    var transferDetails = objmms.tblInterStateTransferMasters.FirstOrDefault(x => x.TransId == PurchaseNo);

                    MMS.Models.Procedure OBJP = new Procedure();
                    List<TransferItemDetailGRN> lst = null;
                    lst = OBJP.Get_TransferItemdetailONGRN_Inter(transferDetails.InterTransferNumber);

                    var data = new GridGRN_TransferItemList()
                    { Child = lst };
                    ViewBag.IsEdit = true;
                    if (Request.IsAjaxRequest())
                    {
                        return View("_TransferItemsListONGRN", data);

                    }
                    return View();


                }
                else if (Type == "Inter Transfer (Other State)")
                {
                    var transferDetails = objmms.tblIntraStateTransferMasters.FirstOrDefault(x => x.TransId == PurchaseNo);

                    MMS.Models.Procedure OBJP = new Procedure();
                    List<TransferItemDetailGRN> lst = null;
                    lst = OBJP.Get_TransferItemdetailONGRN(transferDetails.IntraTransferNumber);

                    var data = new GridGRN_TransferItemList()
                    { Child = lst };
                    ViewBag.IsEdit = true;
                    if (Request.IsAjaxRequest())
                    {
                        return View("_TransferItemsListONGRN", data);

                    }
                    return View();

                }
               else if (Type == "Site Purchase" || Type == "Mechanical Purchase" || Type == "Plumbing Purchase" || Type == "Electrical Purchase")
                {


                    List<VMTbIndentPurchaseOrderChildViewModel> MasterList = null;
                    totalRows = objmms.TbIndentPurchaseOrderChild_GST.Where(x => x.MUId == PurchaseNo).ToList().Count();

                    MasterList = (from p in objmms.TbIndentPurchaseOrderChild_GST.Where(i => i.MUId == PurchaseNo && i.Status != "Closed")

                                  join it in objmms.tblItemMasters on p.ItemNo equals it.ItemID into g
                                  from it1 in g.DefaultIfEmpty()
                                  join ig in objmms.tblItemGroupMasters on it1.ItemGroupID equals ig.ItemGroupID into g1
                                  from ig1 in g1.DefaultIfEmpty()
                                  join u in objmms.tblUnitMasters on it1.UnitID equals u.UnitID into g2
                                  from u1 in g2.DefaultIfEmpty()
                                  join ind in objmms.PurchaseRequisitionChilds on p.IndentId equals ind.UId into g3
                                  from ind1 in g3.DefaultIfEmpty()
                                  select new VMTbIndentPurchaseOrderChildViewModel
                                  {
                                      TbIndentPurchaseOrderChild = p,
                                      ItemName = (it1 == null) ? null : it1.ItemName,
                                      Make = (it1 == null) ? null : it1.Make,
                                      PartNo = (it1 == null) ? null : it1.PartNo,
                                      ItemGroup = (ig1 == null) ? null : ig1.GroupName,
                                      ItemNo = (it1 == null) ? null : it1.ItemID,
                                      Unit = (u1 == null) ? null : u1.UnitName,
                                      Qty = p.Qty,
                                      IndentNo = (ind1 == null) ? null : ind1.PurRequisitionNo,
                                      //GateReceivedQty = objmms.GateEntryChild_Mid.Where(x=>x.StatusChildId == p.UId && x.StatusTypeNo == (objmms.TbIndentPurchaseOrder_GST.Where(c => c.UId == p.MUId).FirstOrDefault().PurchaseOrderNo)).FirstOrDefault().ReceiveQty ,
                                      GateReceivedQty = (from k in objmms.GateEntryChild_Mid.Where(x => x.StatusChildId == p.UId && x.StatusTypeNo == (objmms.TbIndentPurchaseOrder_GST.Where(c => c.UId == p.MUId).FirstOrDefault().PurchaseOrderNo)).ToList() select k.ReceiveQty).Sum(),

                                      PIDate = ind1.Date,
                                      PODate = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == p.MUId).FirstOrDefault().PurchaseOrderDate,
                                      Ex_Item_Pecntg = it1.Excess_Item_Percentage ?? 0,
                                      Ex_Item_Qty = (p.Qty + (p.Item_ExcessQuantity ?? 0))
                                  }).ToList();

                    if (MasterList != null)
                    {


                        VMTbIndentPurchaseOrderMasterChild V = (
                             from p in objmms.TbIndentPurchaseOrder_GST.Where(i => i.UId == PurchaseNo)
                             join pc in objmms.tblProjectParticularsDetailAs on p.ProjectNo equals pc.PRJID into g
                             from pc1 in g.DefaultIfEmpty()
                             join c in objmms.tblVendorMasters on p.SupplierNo equals c.VendorID into g1
                             from c1 in g1.DefaultIfEmpty()
                             join b in objmms.PurchaseRequisitionMasters.Select(xx => new { PurRequisitionNo = xx.PurRequisitionNo, CreatedDate = xx.Date }).Distinct() on p.IndentRefNo equals b.PurRequisitionNo into g2
                             from b1 in g2.DefaultIfEmpty()
                             join e in objmms.tblEmployeeMasters on p.CreatedBy equals e.EmpID into g3
                             from e1 in g3.DefaultIfEmpty()
                             join ee in objmms.tblEmployeeMasters on p.SendTo equals ee.EmpID into g4
                             from ee1 in g4.DefaultIfEmpty()
                             select new VMTbIndentPurchaseOrderMasterChild
                             {
                                 TbIndentPurchaseOrderMaster = p,
                                 ProjectName = pc1.NameOfProject,
                                 ProjectAddress = pc1.Location,

                                 SupplierName = (c1 == null) ? null : c1.Name,
                                 SupplierAddress = (c1 == null) ? null : c1.Address,
                                 SupplierTinNo = (c1 == null) ? null : c1.TinNo,
                                 IndentRefNoDate = b1.CreatedDate,
                                 CreatedName = e1.FName,
                                 CreatedDate = p.CreatedDate,
                                 SendToName = ee1.FName
                             }).FirstOrDefault();



                        foreach (var item in MasterList)
                        {
                            // var itemData=objmms.tblItemMasters.SingleOrDefault(x => x.ItemID == item.ItemNo);
                            if ((item.TbIndentPurchaseOrderChild.Item_ExcessPercentage == null || item.TbIndentPurchaseOrderChild.Item_ExcessPercentage == 0) && item.Ex_Item_Pecntg != 0)
                            {
                                item.TbIndentPurchaseOrderChild.Item_ExcessPercentage = item.Ex_Item_Pecntg;
                                item.Ex_Item_Qty = Convert.ToDecimal(Math.Round((((item.TbIndentPurchaseOrderChild.Qty.Value * item.Ex_Item_Pecntg.Value) / 100) + item.TbIndentPurchaseOrderChild.Qty.Value), 3));
                                var itemToUpdate = objmms.TbIndentPurchaseOrderChild_GST.SingleOrDefault(x => x.UId == item.TbIndentPurchaseOrderChild.UId);

                                itemToUpdate.Item_ExcessPercentage = item.Ex_Item_Pecntg;
                                itemToUpdate.Item_ExcessQuantity = Convert.ToDecimal(Math.Round(((item.TbIndentPurchaseOrderChild.Qty.Value * item.Ex_Item_Pecntg.Value) / 100), 3));
                                objmms.Entry(itemToUpdate).State = EntityState.Modified;
                                objmms.SaveChanges();
                            }
                        }


                        var data = new GridIndentPurchaserOrderChildNew()
                        {
                            TotalRows = totalRows,
                            PageSize = pageSize,
                            Child = MasterList.ToList(),

                            Vendor = V.SupplierName,
                            VendorNo = V.TbIndentPurchaseOrderMaster.SupplierNo
                        };
                        if (Request.IsAjaxRequest())
                        {
                            return View("_GridLPTest", data);
                            //  _GridViewLP return View("_GridViewLP")
                        }
                    }


                }
                else if (Type == "HO Purchase" || Type == "RO Purchase")
                {
                    List<VMTbIndentPurchaseOrderChildViewModel> MasterList = null;
                    totalRows = objmms.TbIndentPurchaseOrderChild_GST.Where(x => x.MUId == PurchaseNo).ToList().Count();

                    MasterList = (from p in objmms.TbIndentPurchaseOrderChild_GST.Where(i => i.MUId == PurchaseNo)

                                  join it in objmms.tblItemMasters on p.ItemNo equals it.ItemID into g
                                  from it1 in g.DefaultIfEmpty()
                                  join ig in objmms.tblItemGroupMasters on it1.ItemGroupID equals ig.ItemGroupID into g1
                                  from ig1 in g1.DefaultIfEmpty()
                                  join u in objmms.tblUnitMasters on it1.UnitID equals u.UnitID into g2
                                  from u1 in g2.DefaultIfEmpty()
                                  join ind in objmms.PurchaseRequisitionChilds on p.IndentId equals ind.UId into g3
                                  from ind1 in g3.DefaultIfEmpty()
                                  select new VMTbIndentPurchaseOrderChildViewModel
                                  {
                                      TbIndentPurchaseOrderChild = p,

                                      ItemName = (it1 == null) ? null : it1.ItemName,
                                      Make = (it1 == null) ? null : it1.Make,
                                      PartNo = (it1 == null) ? null : it1.PartNo,
                                      ItemGroup = (ig1 == null) ? null : ig1.GroupName,

                                      Unit = (u1 == null) ? null : u1.UnitName,

                                      IndentNo = (ind1 == null) ? null : ind1.PurRequisitionNo,
                                      //GateReceivedQty = objmms.GateEntryChild_Mid.Where(x => x.StatusChildId == p.UId && x.StatusTypeNo == (objmms.TbIndentPurchaseOrder_GST.Where(c => c.UId == p.MUId).FirstOrDefault().PurchaseOrderNo)).FirstOrDefault().ReceiveQty,
                                      GateReceivedQty = (from k in objmms.GateEntryChild_Mid.Where(x => x.StatusChildId == p.UId && x.StatusTypeNo == (objmms.TbIndentPurchaseOrder_GST.Where(c => c.UId == p.MUId).FirstOrDefault().PurchaseOrderNo)).ToList() select k.ReceiveQty).Sum(),

                                      PIDate = ind1.Date,
                                      PODate = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == p.MUId).FirstOrDefault().PurchaseOrderDate,
                                      Ex_Item_Pecntg = it1.Excess_Item_Percentage ?? 0,
                                      Ex_Item_Qty = (p.Qty + (p.Item_ExcessQuantity ?? 0))
                                  }).ToList();





                    if (MasterList != null)
                    {
                        VMTbIndentPurchaseOrderMasterChild V = (
                             from p in objmms.TbIndentPurchaseOrder_GST.Where(i => i.UId == PurchaseNo)
                             join pc in objmms.tblProjectParticularsDetailAs on p.ProjectNo equals pc.PRJID into g
                             from pc1 in g.DefaultIfEmpty()
                             join c in objmms.tblVendorMasters on p.SupplierNo equals c.VendorID into g1
                             from c1 in g1.DefaultIfEmpty()
                             join b in objmms.PurchaseRequisitionMasters.Select(xx => new { PurRequisitionNo = xx.PurRequisitionNo, CreatedDate = xx.Date }).Distinct() on p.IndentRefNo equals b.PurRequisitionNo into g2
                             from b1 in g2.DefaultIfEmpty()
                             join e in objmms.tblEmployeeMasters on p.CreatedBy equals e.EmpID into g3
                             from e1 in g3.DefaultIfEmpty()
                             join ee in objmms.tblEmployeeMasters on p.SendTo equals ee.EmpID into g4
                             from ee1 in g4.DefaultIfEmpty()
                             select new VMTbIndentPurchaseOrderMasterChild
                             {
                                 TbIndentPurchaseOrderMaster = p,
                                 ProjectName = pc1.NameOfProject,
                                 ProjectAddress = pc1.Location,

                                 SupplierName = (c1 == null) ? null : c1.Name,
                                 SupplierAddress = (c1 == null) ? null : c1.Address,
                                 SupplierTinNo = (c1 == null) ? null : c1.TinNo,
                                 IndentRefNoDate = b1.CreatedDate,
                                 CreatedName = e1.FName,
                                 CreatedDate = p.CreatedDate,
                                 SendToName = ee1.FName
                             }).FirstOrDefault();

                        foreach (var item in MasterList)
                        {
                            // var itemData=objmms.tblItemMasters.SingleOrDefault(x => x.ItemID == item.ItemNo);
                            if ((item.TbIndentPurchaseOrderChild.Item_ExcessPercentage == null || item.TbIndentPurchaseOrderChild.Item_ExcessPercentage == 0) && item.Ex_Item_Pecntg != 0)
                            {
                                item.TbIndentPurchaseOrderChild.Item_ExcessPercentage = item.Ex_Item_Pecntg;
                                item.Ex_Item_Qty = Convert.ToDecimal(Math.Round((((item.TbIndentPurchaseOrderChild.Qty.Value * item.Ex_Item_Pecntg.Value) / 100) + item.TbIndentPurchaseOrderChild.Qty.Value), 3));
                                var itemToUpdate = objmms.TbIndentPurchaseOrderChild_GST.SingleOrDefault(x => x.UId == item.TbIndentPurchaseOrderChild.UId);

                                itemToUpdate.Item_ExcessPercentage = item.Ex_Item_Pecntg;
                                itemToUpdate.Item_ExcessQuantity = Convert.ToDecimal(Math.Round(((item.TbIndentPurchaseOrderChild.Qty.Value * item.Ex_Item_Pecntg.Value) / 100), 3));
                                objmms.Entry(itemToUpdate).State = EntityState.Modified;
                                objmms.SaveChanges();
                            }
                        }


                        var data = new GridIndentPurchaserOrderChildNew()
                        {
                            TotalRows = totalRows,
                            PageSize = pageSize,
                            Child = MasterList.ToList(),

                            Vendor = V.SupplierName,
                            VendorNo = V.TbIndentPurchaseOrderMaster.SupplierNo
                        };
                        if (Request.IsAjaxRequest())
                        {
                            return View("_GridViewIPO", data);
                        }
                    }
                }


                else if (Type == "CASH Purchase" || Type == "FOC Purchase" || Type == "Manual Transfer Receive Purchase")
                {
                    List<VMTbIndentPurchaseOrderChildViewModel> MasterList = null;
                    totalRows = objmms.TbIndentPurchaseOrderChild_GST.Where(x => x.MUId == PurchaseNo).ToList().Count();

                    MasterList = (from p in objmms.TbIndentPurchaseOrderChild_GST.Where(i => i.MUId == PurchaseNo)

                                  join it in objmms.tblItemMasters on p.ItemNo equals it.ItemID into g
                                  from it1 in g.DefaultIfEmpty()
                                  join ig in objmms.tblItemGroupMasters on it1.ItemGroupID equals ig.ItemGroupID into g1
                                  from ig1 in g1.DefaultIfEmpty()
                                  join u in objmms.tblUnitMasters on it1.UnitID equals u.UnitID into g2
                                  from u1 in g2.DefaultIfEmpty()
                                  join ind in objmms.PurchaseRequisitionChilds on p.IndentId equals ind.UId into g3
                                  from ind1 in g3.DefaultIfEmpty()
                                  select new VMTbIndentPurchaseOrderChildViewModel
                                  {
                                      TbIndentPurchaseOrderChild = p,

                                      ItemName = (it1 == null) ? null : it1.ItemName,
                                      Make = (it1 == null) ? null : it1.Make,
                                      PartNo = (it1 == null) ? null : it1.PartNo,
                                      ItemGroup = (ig1 == null) ? null : ig1.GroupName,

                                      Unit = (u1 == null) ? null : u1.UnitName,

                                      IndentNo = (ind1 == null) ? null : ind1.PurRequisitionNo,
                                      //GateReceivedQty = objmms.GateEntryChild_Mid.Where(x => x.StatusChildId == p.UId).FirstOrDefault().ReceiveQty,
                                      GateReceivedQty = (from k in objmms.GateEntryChild_Mid.Where(x => x.StatusChildId == p.UId && x.StatusTypeNo == (objmms.TbIndentPurchaseOrder_GST.Where(c => c.UId == p.MUId).FirstOrDefault().PurchaseOrderNo)).ToList() select k.ReceiveQty).Sum(),
                                      PIDate = ind1.Date,
                                      PODate = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == p.MUId).FirstOrDefault().PurchaseOrderDate,
                                      Ex_Item_Pecntg = it1.Excess_Item_Percentage ?? 0,
                                      Ex_Item_Qty = (p.Qty + (p.Item_ExcessQuantity ?? 0))
                                  }).ToList();


                    if (Type == "Manual Transfer Receive Purchase")
                    {
                        ViewBag.Type = "Manual Transfer Receive Purchase";
                    }



                    if (MasterList != null)
                    {
                        VMTbIndentPurchaseOrderMasterChild V = (
                             from p in objmms.TbIndentPurchaseOrder_GST.Where(i => i.UId == PurchaseNo)
                             join pc in objmms.tblProjectParticularsDetailAs on p.ProjectNo equals pc.PRJID into g
                             from pc1 in g.DefaultIfEmpty()
                             join c in objmms.tblVendorMasters on p.SupplierNo equals c.VendorID into g1
                             from c1 in g1.DefaultIfEmpty()
                             join b in objmms.PurchaseRequisitionMasters.Select(xx => new { PurRequisitionNo = xx.PurRequisitionNo, CreatedDate = xx.Date }).Distinct() on p.IndentRefNo equals b.PurRequisitionNo into g2
                             from b1 in g2.DefaultIfEmpty()
                             join e in objmms.tblEmployeeMasters on p.CreatedBy equals e.EmpID into g3
                             from e1 in g3.DefaultIfEmpty()
                             join ee in objmms.tblEmployeeMasters on p.SendTo equals ee.EmpID into g4
                             from ee1 in g4.DefaultIfEmpty()
                             select new VMTbIndentPurchaseOrderMasterChild
                             {
                                 TbIndentPurchaseOrderMaster = p,
                                 ProjectName = pc1.NameOfProject,
                                 ProjectAddress = pc1.Location,

                                 SupplierName = (c1 == null) ? null : c1.Name,
                                 SupplierAddress = (c1 == null) ? null : c1.Address,
                                 SupplierTinNo = (c1 == null) ? null : c1.TinNo,
                                 IndentRefNoDate = b1.CreatedDate,
                                 CreatedName = e1.FName,
                                 CreatedDate = p.CreatedDate,
                                 SendToName = ee1.FName
                             }).FirstOrDefault();

                        foreach (var item in MasterList)
                        {
                            // var itemData=objmms.tblItemMasters.SingleOrDefault(x => x.ItemID == item.ItemNo);
                            if ((item.TbIndentPurchaseOrderChild.Item_ExcessPercentage == null || item.TbIndentPurchaseOrderChild.Item_ExcessPercentage == 0) && item.Ex_Item_Pecntg != 0)
                            {
                                item.TbIndentPurchaseOrderChild.Item_ExcessPercentage = item.Ex_Item_Pecntg;
                                item.Ex_Item_Qty = Convert.ToDecimal(Math.Round((((item.TbIndentPurchaseOrderChild.Qty.Value * item.Ex_Item_Pecntg.Value) / 100) + item.TbIndentPurchaseOrderChild.Qty.Value), 3));
                                var itemToUpdate = objmms.TbIndentPurchaseOrderChild_GST.SingleOrDefault(x => x.UId == item.TbIndentPurchaseOrderChild.UId);

                                itemToUpdate.Item_ExcessPercentage = item.Ex_Item_Pecntg;
                                itemToUpdate.Item_ExcessQuantity = Convert.ToDecimal(Math.Round(((item.TbIndentPurchaseOrderChild.Qty.Value * item.Ex_Item_Pecntg.Value) / 100), 3));
                                objmms.Entry(itemToUpdate).State = EntityState.Modified;
                                objmms.SaveChanges();
                            }
                        }


                        var data = new GridIndentPurchaserOrderChildNew()
                        {
                            TotalRows = totalRows,
                            PageSize = pageSize,
                            Child = MasterList.ToList(),

                            Vendor = V.SupplierName,
                            VendorNo = V.TbIndentPurchaseOrderMaster.SupplierNo
                        };
                        if (Request.IsAjaxRequest())
                        {
                            return View("_GridViewIPO", data);
                        }
                    }
                }




                else if (Type == "Dispatch" || Type == "Receive")
                {
                    List<VMTbOSTTransferChild> MasterList = null;
                    totalRows = m.GetTbOSTTransferChildManager().FindByMasterCount(PurchaseNo);
                    MasterList = m.GetTbOSTTransferChildManager().FindByMasterVM(PurchaseNo);
                    if (MasterList != null)
                    {
                        P_VMTbOSTTransferMaster V = m.GetTbOSTTransferMasterManager().FindVM(PurchaseNo);


                        var data = new GridOSTChild()
                        {
                            TotalRows = totalRows,
                            PageSize = pageSize,
                            Child = MasterList.ToList(),

                            ReceivingSite = V.ReceiveSiteName,
                            ReceivingSiteNo = V.ReceiveSiteNo,

                            DispatchingSite = V.DispatchSiteName,
                            DispatchingSiteNo = V.DispatchSiteNo,

                            ChallanNo = V.ChallanNo,
                            VehicleNo = V.VehicleNo,
                            ChallanDate = V.ChallanDate
                        };
                        if (Request.IsAjaxRequest())
                        {
                            if (Type == "Dispatch")
                                return View("_GridViewDispatch", data);
                            else if (Type == "Receive")
                                return View("_GridViewReceive", data);
                        }
                    }
                }






            }

            return View("_EmptyView");

        }



        public ActionResult AddM_UpC(GateEntry_UpdateChild_Mid Grid)
        {

            using (var trans=objmms.Database.BeginTransaction())
            {
                try
                {

                    #region

                    string FNYr = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.Date));

                    var S_Id = objmms.SessionMasters.Where(x => x.Name == FNYr.ToString()).FirstOrDefault().Id;

                    DateTime? purchDate = null;
                    if (Grid.Master.StatusTypeNo.Contains("PO") || Grid.Master.StatusTypeNo.Contains("SP") || Grid.Master.StatusTypeNo.Contains("HO") || Grid.Master.StatusTypeNo.Contains("MTR"))
                    {
                        purchDate = objmms.TbIndentPurchaseOrder_GST.Where(s => s.PurchaseOrderNo == Grid.Master.StatusTypeNo).Select(s => s.PurchaseOrderDate).FirstOrDefault();
                    }
                    else if (Grid.Master.StatusTypeNo.Contains("IS"))
                    {
                        purchDate=objmms.tblInterStateTransferMasters.Where(s=>s.InterTransferNumber== Grid.Master.StatusTypeNo).Select(s => s.TransferDate).FirstOrDefault();
                    }
                    else if (Grid.Master.StatusTypeNo.Contains("OS"))
                    {
                        purchDate = objmms.tblIntraStateTransferMasters.Where(s => s.IntraTransferNumber == Grid.Master.StatusTypeNo).Select(s => s.TransferDate).FirstOrDefault();
                    }

                    if (purchDate != null)
                    {
                        if (purchDate.Value > Grid.Master.Date.Value)
                        {
                            return Json("101");
                        }
                    }
                    Grid.Master.SessionId = S_Id;
                    Grid.Master.CreatedBy = EmpId;
                    Grid.Master.CreaterName = EmpName;
                    Grid.Master.CreatedDate = System.DateTime.Now;
                    //  Grid.Master.GateEntryId = m.GetGateEntryManager().FindMaxId(SessionId, Grid.Master.ProjectNo);
                    var divisionId = Repos.GetUserDivision();

                    var DivisionCode = objmms.tblDivisionMasters.SingleOrDefault(x=>x.Id==divisionId).DivisionCode;

                    Grid.Master.GateEntryId = G_MidMaxId(Grid.Master.SessionId, Grid.Master.ProjectNo, divisionId);

                    // decimal a = m.GetGateEntryManager().Add(Grid.Master);

                    var grn = GetGRNNoByDateWiseString(Grid.Master.ProjectNo, Grid.Master.Date.ToString());

                    if (string.IsNullOrEmpty(grn))
                    {
                        throw new Exception("GRN can not be null");
                    }

                    Grid.Master.GateEntryNo = grn;

                    Grid.Master.DivisionId = divisionId;

                    //----
                    objmms.GateEntry_Mid.Add(Grid.Master);
                    objmms.SaveChanges();
                    decimal a1 = objmms.GateEntry_Mid.Where(i =>i.DivisionId==divisionId && i.SessionId == Grid.Master.SessionId && i.ProjectNo == Grid.Master.ProjectNo).Select(i => i.UId).DefaultIfEmpty(0).Max(i => i);
                    decimal a = a1;
                    //---
                    if (Grid.Child != null && a != 0)
                    {

                        foreach (var x in Grid.Child)
                        {
                            // tblReceiveData obj = new tblReceiveData(); * Here No stock Update*

                            if (x.ReceiveQty != null && x.ReceiveQty != 0)
                            {
                                x.MUId = a;
                                x.ProjectNo = Grid.Master.ProjectNo;
                                x.ProjectName = Grid.Master.ProjectName;
                                x.GateEntryNo = Grid.Master.GateEntryNo;
                                x.Date = Grid.Master.Date;
                                x.GateEntryDate = Grid.Master.Date;
                                x.CreatedBy = EmpId;
                                x.CreatedDate = System.DateTime.Now;
                                x.SessionId = S_Id;
                                x.Amount = x.ReceiveQty * (x.Rate ?? 0);
                                x.TaxAmount = x.ReceiveQty * (x.TaxPer ?? 0);
                                x.DeliveryAmount = x.ReceiveQty * (x.DeliveryUnitCharge ?? 0);
                                x.AllAmount = x.Amount + x.TaxAmount + x.DeliveryAmount;
                                //x.SNo = ++i;
                                // added from below 

                                BusinessObjects.Entity.tblItemMaster item = m.GetGobalItemManager().Find(x.ItemNo);
                                x.ItemGroupNo = item.ItemGroupID;
                                x.UnitNo = item.UnitID;
                                x.InOut = "In";
                                //--

                                // code for tblReceiveData
                                #region
                                //obj.ID = Convert.ToInt32(a);
                                //obj.IDType = "GateEntry";
                                //obj.TypeNumber = Grid.Master.GateEntryNo;
                                //obj.TypeDate = Grid.Master.Date;
                                //obj.ReceiveQuantity = x.ReceiveQty ?? 0;
                                //obj.IssueQuantity = 0;
                                //obj.BalanceQuantity = x.ReceiveQty ?? 0;
                                //obj.Rate = x.Rate;
                                //obj.ProjectId = Grid.Master.ProjectNo;
                                //obj.ItemId = x.ItemNo;
                                //obj.CreatedDate = System.DateTime.Now;
                                //obj.CreatedBy = EmpId;
                                //obj.Status = Convert.ToBoolean(1);
                                //obj.Isdeleted = Convert.ToBoolean(0);
                                //obj.FinYear = objmsp.GetFinancialYear();


                                // End Here
                                //if (x.Status == "LP" || x.Status == "IPO")
                                //{
                                //    BusinessObjects.Entity.tblItemMaster item = m.GetGobalItemManager().Find(x.ItemNo);
                                //    x.ItemGroupNo = item.ItemGroupID;
                                //    obj.ItemGroupId = item.ItemGroupID;
                                //    x.UnitNo = item.UnitID;
                                //    x.InOut = "In";
                                //    //x.Unit = item.UnitMasterPurchase.UnitName;
                                //}

                                //if (x.Status == "LP")
                                //{

                                //    BusinessObjects.Entity.TbCashPurchaseChild ch = m.GetTbCashPurchaseChildManager().Find(x.StatusChildId ?? 0);
                                //    ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                                //    m.GetTbCashPurchaseChildManager().Update(ch);
                                //    m.GetTbCashPurchaseMasterManager().FindComplete(x.StatusTypeId ?? 0);
                                //}
                                //else if (x.Status == "IPO")
                                //{

                                //    BusinessObjects.Entity.TbIndentPurchaseOrderChild ch = m.GetTbIndentPurchaseOrderChildManager().Find(x.StatusChildId ?? 0);
                                //    ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                                //    m.GetTbIndentPurchaseOrderChildManager().Update(ch);
                                //    m.GetTbIndentPurchaseOrderMasterManager().FindComplete(x.StatusTypeId ?? 0);
                                //}
                                //else if (x.Status == "Dispatch")
                                //{
                                //    x.InOut = "Out";
                                //    TbOSTTransferChild ch = m.GetTbOSTTransferChildManager().Find(x.StatusChildId ?? 0);
                                //    //x.ReceiveQty use As Dispatch Qty
                                //    ch.Dispatch = (ch.Dispatch ?? 0) + x.ReceiveQty;
                                //    m.GetTbOSTTransferChildManager().Update(ch);
                                //    m.GetTbOSTTransferMasterManager().FindCompleteD(x.StatusTypeId ?? 0);
                                //}
                                //else if (x.Status == "Receive")
                                //{
                                //    x.InOut = "In";
                                //    TbOSTTransferChild ch = m.GetTbOSTTransferChildManager().Find(x.StatusChildId ?? 0);
                                //    ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                                //    m.GetTbOSTTransferChildManager().Update(ch);
                                //    m.GetTbOSTTransferMasterManager().FindCompleteR(x.StatusTypeId ?? 0);
                                //}


                                //m.GetGateEntryChildManager().Add(x);
                                //objmms.tblReceiveDatas.Add(obj);
                                //objmms.SaveChanges();
                                //if (x.Status == "LP" || x.Status == "IPO" || x.Status == "Receive")
                                //{
                                //    m.GettblItemCurrentStockManager().UpdateForAdd(Grid.Master.ProjectNo, x.ItemGroupNo, x.ItemNo, +(x.ReceiveQty ?? 0));
                                //}
                                //else if (x.Status == "Dispatch")
                                //{
                                //    m.GettblItemCurrentStockManager().UpdateForAdd(Grid.Master.ProjectNo, x.ItemGroupNo, x.ItemNo, -(x.ReceiveQty ?? 0));
                                //}
                                #endregion

                                objmms.GateEntryChild_Mid.Add(x);

                            }
                        }
                        objmms.SaveChanges();
                    }
                    #endregion
                    // return Json("1", JsonRequestBehavior.AllowGet);
                    trans.Commit();
                    return Json(new { Status = 1, TransNo = Grid.Master.GateEntryNo }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex) { ex.ToString();
                    trans.Rollback();
                    return Json("2", JsonRequestBehavior.AllowGet); } 
            }

        }

        public ActionResult UpdateGateEntry(GateEntry_UpdateChild_Mid Grid)
        {
            using (var trans=objmms.Database.BeginTransaction())
            {
                try
                {
                    #region
                    if (Grid.Master.GateEntryNo == null)
                    {
                        return Json("3", JsonRequestBehavior.AllowGet);
                    }

                    var masterDataToUpdate = objmms.GateEntry_Mid.SingleOrDefault(s => s.GateEntryNo == Grid.Master.GateEntryNo);

                    masterDataToUpdate.Date = Grid.Master.Date;
                    masterDataToUpdate.ChallanNo = Grid.Master.ChallanNo;
                    masterDataToUpdate.ChallanDate = Grid.Master.ChallanDate;
                    masterDataToUpdate.VehicleNo = Grid.Master.VehicleNo;
                    masterDataToUpdate.GRNStatus = null;
                    objmms.Entry(masterDataToUpdate).State = EntityState.Modified;

                    //----

                    var childs = objmms.GateEntryChild_Mid.Where(x => x.GateEntryNo == Grid.Master.GateEntryNo);

                    //---
                    if (Grid.Child != null)
                    {

                        foreach (var x in childs)
                        {

                            var data = Grid.Child.SingleOrDefault(y => y.StatusChildId == x.StatusChildId);

                            if (data != null)
                            {


                                //x.MUId = a;
                                // x.ProjectNo = Grid.Master.ProjectNo;
                                //x.ProjectName = Grid.Master.ProjectName;
                                x.Date = Grid.Master.Date;
                                x.GateEntryDate = Grid.Master.Date;
                                x.ModyfiedDate = DateTime.Now;
                                x.ReceiveQty = data.ReceiveQty;
                                x.Rate = data.Rate;
                                x.Amount = data.ReceiveQty * (data.Rate ?? 0);
                                x.TaxAmount = data.ReceiveQty * (data.TaxPer ?? 0);
                                x.DeliveryAmount = data.ReceiveQty * (data.DeliveryUnitCharge ?? 0);
                                x.AllAmount = x.Amount + x.TaxAmount + x.DeliveryAmount;
                                x.ModyfiedBy = Session["EmpID"].ToString();
                                objmms.Entry(x).State = EntityState.Modified;
                            }
                        }
                    }
                    objmms.SaveChanges();
                    #endregion
                    // return Json("1", JsonRequestBehavior.AllowGet);
                    trans.Commit();
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex) { ex.ToString();
                    trans.Rollback();
                    return Json("2", JsonRequestBehavior.AllowGet); } 
            }

        }


        //***********add on 28/04/2020******************

        public ActionResult UpdateGateEntryForTransfer(EditGRNViewModel Grid)
        {
            using (var trans = objmms.Database.BeginTransaction())
            {
                try
                {
                    #region
                    if (Grid.Master.GateEntryNo == null)
                    {
                        return Json("3", JsonRequestBehavior.AllowGet);
                    }

                    var masterDataToUpdate = objmms.GateEntry_Mid.SingleOrDefault(s => s.GateEntryNo == Grid.Master.GateEntryNo);

                    //masterDataToUpdate.Date = Grid.Master.Date;
                    masterDataToUpdate.ChallanNo = Grid.Master.ChallanNo;
                    masterDataToUpdate.ChallanDate = Grid.Master.ChallanDate;
                    masterDataToUpdate.VehicleNo = Grid.Master.VehicleNo;
                    masterDataToUpdate.GRNStatus = null;
                    objmms.Entry(masterDataToUpdate).State = EntityState.Modified;

                    //----

                    var childs = objmms.GateEntryChild_Mid.Where(x => x.GateEntryNo == Grid.Master.GateEntryNo);

                    //---
                    if (Grid.Child != null)
                    {

                        foreach (var x in childs)
                        {

                            var data = Grid.Child.SingleOrDefault(y => y.StatusChildId == x.StatusChildId && y.GateEntryCID == x.UId);

                            if (data != null)
                            {
                                x.ModyfiedDate = DateTime.Now;
                                x.ReceiveQty = data.ReceiveQty;
                                x.Amount = data.ReceiveQty * (x.Rate ?? 0);
                                x.TaxAmount = data.ReceiveQty * (x.TaxPer ?? 0);
                                x.DeliveryAmount = data.ReceiveQty * (x.DeliveryUnitCharge ?? 0);
                                x.AllAmount = x.Amount + x.TaxAmount + x.DeliveryAmount;
                                x.ModyfiedBy = Session["EmpID"].ToString();
                                objmms.Entry(x).State = EntityState.Modified;
                            }
                        }
                    }
                    objmms.SaveChanges();
                    #endregion
                    // return Json("1", JsonRequestBehavior.AllowGet);
                    trans.Commit();
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    trans.Rollback();
                    return Json("2", JsonRequestBehavior.AllowGet);
                }
            }

        }


        //*************end**********************


        public ActionResult AddDisp_Data(GateEntry_UpdateChild Grd)
        {

            if (Grd.Child != null)
            {
               

                foreach (var x in Grd.Child)
                {
                    decimal RemIssueQty = Convert.ToDecimal(x.ReceiveQty);
                    if (x.ReceiveQty != null && x.ReceiveQty != 0)
                    {
                        List<DAL.ns_tbl_IssueQuantity> obj = new List<DAL.ns_tbl_IssueQuantity>();
                        var list = objmms.tblReceiveDatas.Where(p => p.ProjectId == Grd.Master.ProjectNo && p.ItemGroupId == x.ItemGroupNo && p.ItemId == x.ItemNo && p.BalanceQuantity > 0).OrderBy(k => k.TypeDate).ToList();
                        if (list.Count() > 0)
                        {

                            foreach (var item in list)
                            {
                                DAL.ns_tbl_IssueQuantity chobj = new DAL.ns_tbl_IssueQuantity();
                                decimal RecvQty = Convert.ToDecimal(item.BalanceQuantity); decimal a = 0;
                                if (RecvQty >= RemIssueQty)
                                {
                                    a = RecvQty - RemIssueQty;
                                    decimal iq = RemIssueQty;
                                    decimal qt = RecvQty;
                                    decimal bq = a;
                                    decimal aq = RecvQty;
                                    decimal aiq = a;
                                    #region

                                    chobj.ProjectID = Grd.Master.ProjectNo;
                                    chobj.ProjectName = Grd.Master.ProjectName;
                                    chobj.IndentNo = x.StatusTypeNo;
                                    chobj.EmployeeName = Grd.Master.EmpName;
                                    chobj.EmployeeID = Grd.Master.EmpId;
                                    chobj.Status = "Approved";
                                    chobj.PcContractorId = null;
                                    chobj.ItemGroupID = x.ItemGroupNo;
                                    chobj.ItemGroupName = x.ItemGroup;
                                    chobj.ItemID = x.ItemNo;
                                    chobj.ItemName = x.Item;
                                    chobj.UnitID = x.Unit;
                                    chobj.Make = "";
                                    chobj.PartNo = "";
                                    chobj.Quantity = qt;

                                    chobj.BalanceQuantity = bq;
                                    chobj.AvailableQuantity = aq;
                                    chobj.AfterIssue_AvlQty_Stock = aiq;

                                    chobj.SessionId = "1";
                                    chobj.SiteId = x.VendorNo;
                                    chobj.IssuedBy = Convert.ToString(Session["EmpID"]);
                                    chobj.CreatedDate = System.DateTime.Now;
                                    chobj.ModifiedDate = DateTime.Now;
                                    chobj.Issue_Date = Grd.Master.Date;
                                    chobj.Gate_ReceiveDate = Grd.Master.Date; ;
                                    chobj.GateEntry_UId = item.TransId;
                                    chobj.Receive_Rate = item.Rate;
                                    chobj.Issue_Type = "Transfer";
                                    chobj.IssueQuantity = iq;
                                    obj.Add(chobj);
                                    // for Updating TblReceiveData
                                    decimal TrId = item.TransId;
                                    tblReceiveData tdr = objmms.tblReceiveDatas.Where(o => o.ProjectId == Grd.Master.ProjectNo && o.ItemGroupId == x.ItemGroupNo && o.ItemId == x.ItemNo && o.TransId == TrId).First();
                                    tdr.BalanceQuantity = qt - iq;
                                    item.IssueQuantity = item.IssueQuantity + iq;
                                    tdr.ModifiedDate = System.DateTime.Now;
                                    objmms.Entry(tdr).State = EntityState.Modified;
                                    // End here
                                    // stock master table updation here

                                    #endregion

                                    break;

                                }
                                else if (RecvQty < RemIssueQty)
                                {
                                    a = RemIssueQty - RecvQty;
                                    decimal iq = RecvQty;
                                    decimal qt = RecvQty;
                                    decimal bq = 0;
                                    decimal aq = RecvQty;
                                    decimal aiq = 0;

                                    #region

                                    chobj.ProjectID = Grd.Master.ProjectNo;
                                    chobj.ProjectName = Grd.Master.ProjectName;
                                    chobj.IndentNo = x.StatusTypeNo;
                                    chobj.EmployeeName = Grd.Master.EmpName;
                                    chobj.EmployeeID = Grd.Master.EmpId;
                                    chobj.Status = "Approved";
                                    chobj.PcContractorId = null;
                                    chobj.ItemGroupID = x.ItemGroupNo;
                                    chobj.ItemGroupName = x.ItemGroup;
                                    chobj.ItemID = x.ItemNo;
                                    chobj.ItemName = x.Item;
                                    chobj.UnitID = x.Unit;
                                    chobj.Make = "";
                                    chobj.PartNo = "";
                                    chobj.Quantity = qt;

                                    chobj.BalanceQuantity = bq;
                                    chobj.AvailableQuantity = aq;
                                    chobj.AfterIssue_AvlQty_Stock = aiq;

                                    chobj.SessionId = "1";
                                    chobj.SiteId = x.VendorNo;
                                    chobj.IssuedBy = Convert.ToString(Session["EmpID"]);
                                    chobj.CreatedDate = System.DateTime.Now;
                                    chobj.ModifiedDate = DateTime.Now;
                                    chobj.Issue_Date = Grd.Master.Date;
                                    chobj.Gate_ReceiveDate = Grd.Master.Date; ;
                                    chobj.GateEntry_UId = item.TransId;
                                    chobj.Receive_Rate = item.Rate;
                                    chobj.Issue_Type = "Transfer";
                                    chobj.IssueQuantity = iq;
                                    obj.Add(chobj);
                                    // for Updating TblReceiveData
                                    decimal TrId = item.TransId;
                                    tblReceiveData tdr = objmms.tblReceiveDatas.Where(o => o.ProjectId == Grd.Master.ProjectNo && o.ItemGroupId == x.ItemGroupNo && o.ItemId == x.ItemNo && o.TransId == TrId).First();
                                    tdr.BalanceQuantity = qt - iq;
                                    item.IssueQuantity = item.IssueQuantity + iq;
                                    tdr.ModifiedDate = System.DateTime.Now;
                                    objmms.Entry(tdr).State = EntityState.Modified;
                                    // End here
                                    // stock master table updation here

                                    #endregion


                                }
                                if (a > 0)
                                {
                                    RemIssueQty = a;
                                }
                                else
                                {
                                    break;
                                }

                            }

                            x.InOut = "Out";
                            TbOSTTransferChild ch = m.GetTbOSTTransferChildManager().Find(x.StatusChildId ?? 0);
                            //x.ReceiveQty use As Dispatch Qty
                            ch.Dispatch = (ch.Dispatch ?? 0) + x.ReceiveQty;
                            m.GetTbOSTTransferChildManager().Update(ch);
                            m.GetTbOSTTransferMasterManager().FindCompleteD(x.StatusTypeId ?? 0);

                            m.GettblItemCurrentStockManager().UpdateForAdd(Grd.Master.ProjectNo, x.ItemGroupNo, x.ItemNo, -(x.ReceiveQty ?? 0));
                            objmms.ns_tbl_IssueQuantity.AddRange(obj);
                            objmms.SaveChanges();


                        }
                        else {
                             return Json("3", JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                }


            return Json("1", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddReceive_Data(GateEntry_UpdateChild Grv)
        {


            try
            {

           

            Grv.Master.SessionId = SessionId;
            Grv.Master.CreatedBy = EmpId;
            Grv.Master.CreaterName = EmpName;
            Grv.Master.CreatedDate = System.DateTime.Now;
            Grv.Master.GateEntryId = m.GetGateEntryManager().FindMaxId(SessionId, Grv.Master.ProjectNo);
            decimal a = m.GetGateEntryManager().Add(Grv.Master);
            if (Grv.Child != null && a != 0)
            {
                foreach (var x in Grv.Child)
                {
                    tblReceiveData obj = new tblReceiveData();
                    if (x.ReceiveQty != null && x.ReceiveQty != 0)
                    {
                        x.MUId = a;
                        x.ProjectNo = Grv.Master.ProjectNo;
                        x.ProjectName = Grv.Master.ProjectName;
                        x.Date = Grv.Master.Date;
                        x.GateEntryDate = Grv.Master.Date;
                        x.CreatedBy = EmpId;
                        x.CreatedDate = System.DateTime.Now;
                        x.SessionId = SessionId;
                        x.Amount = x.ReceiveQty * (x.Rate ?? 0);
                        x.TaxAmount = x.ReceiveQty * (x.TaxPer ?? 0);
                        x.DeliveryAmount = x.ReceiveQty * (x.DeliveryUnitCharge ?? 0);
                        x.AllAmount = x.Amount + x.TaxAmount + x.DeliveryAmount;
                        // code for tblReceiveData
                        obj.ID = Convert.ToInt32(a);
                        obj.IDType = "GateEntry";
                        obj.TypeNumber = Grv.Master.GateEntryNo;
                        obj.TypeDate = Grv.Master.Date;
                        obj.ReceiveQuantity = x.ReceiveQty ?? 0;
                        obj.IssueQuantity = 0;
                        obj.BalanceQuantity = x.ReceiveQty ?? 0;
                        obj.Rate = x.Rate;
                        obj.ProjectId = Grv.Master.ProjectNo;
                        obj.ItemId = x.ItemNo;
                        obj.ItemGroupId = x.ItemGroupNo;
                        obj.CreatedDate = System.DateTime.Now;
                        obj.CreatedBy = EmpId;
                        obj.Status = Convert.ToBoolean(1);
                        obj.Isdeleted = Convert.ToBoolean(0);
                        obj.FinYear = objmsp.GetFinancialYear();
                         if (x.Status == "Receive")
                        {
                            x.InOut = "In";
                            TbOSTTransferChild ch = m.GetTbOSTTransferChildManager().Find(x.StatusChildId ?? 0);
                            ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                            m.GetTbOSTTransferChildManager().Update(ch);
                            m.GetTbOSTTransferMasterManager().FindCompleteR(x.StatusTypeId ?? 0);

                            m.GettblItemCurrentStockManager().UpdateForAdd(Grv.Master.ProjectNo, x.ItemGroupNo, x.ItemNo, +(x.ReceiveQty ?? 0));
                        }

                        m.GetGateEntryChildManager().Add(x);
                        objmms.tblReceiveDatas.Add(obj); objmms.SaveChanges();

                    }

                    }
                }
            return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return Json("2", JsonRequestBehavior.AllowGet);
            }


        }


        public ActionResult AddM_UpIPO(GateEntry_UpdateChild Grid)
        {
            try
            {
                Grid.Master.SessionId = SessionId;
                Grid.Master.CreatedBy = EmpId;
                Grid.Master.CreaterName = EmpName;
                Grid.Master.CreatedDate = System.DateTime.Now;
                Grid.Master.GateEntryId = m.GetGateEntryManager().FindMaxId(SessionId, Grid.Master.ProjectNo);
                decimal a = m.GetGateEntryManager().Add(Grid.Master);
                if (Grid.Child != null && a != 0)
                    foreach (var x in Grid.Child)
                    {
                        x.MUId = a;
                        m.GetGateEntryChildManager().Add(x);
                        BusinessObjects.Entity.TbIndentPurchaseOrderChild ch = m.GetTbIndentPurchaseOrderChildManager().Find(x.StatusChildId??0);
                        ch.Receive = ch.Receive + x.ReceiveQty;
                        m.GetTbIndentPurchaseOrderChildManager().Update(ch);
                    }

                // return Json("1", JsonRequestBehavior.AllowGet);
                return Json("Update Successfully");
            }
            catch { return Json("2", JsonRequestBehavior.AllowGet); }

        }


        public ActionResult Edit(decimal id)
        {
            //id = 9;
            BusinessObjects.Entity.GateEntry Master = m.GetGateEntryManager().Find(id);
            List<BusinessObjects.Entity.GateEntryChild> ChildList = m.GetGateEntryChildManager().FindByMaster(id);
            ViewBag.Date = Master.Date;
          
            ViewBag.ChallanDate = Master.ChallanDate;
            ViewBag.Status = GetStatusList();
            ViewBag.Time = GetTimeList();
          
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var data = new GateEntry_UpdateChild()
            {

                Master = Master,
                Child = ChildList
            };


            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);


            //


        }
        public ActionResult UpM_UpC(GateEntry_UpdateChild Grid)
        {
            try
            {

                Grid.Master.CreatedDate = m.GetGateEntryManager().Find(Grid.Master.UId).CreatedDate;

                m.GetGateEntryManager().Update(Grid.Master);
                List<decimal> CList = m.GetGateEntryChildManager().FindAllIdByMaster(Grid.Master.UId);

                if (Grid.Child != null)
                {
                    foreach (var x in Grid.Child)
                    {

                        if (CList.Any(a => a == x.UId))
                        {
                            decimal? QtyPre = m.GetGateEntryChildManager().Find(x.UId).ReceiveQty;
                            decimal? QtyNow = x.ReceiveQty;
                            decimal Q = 0;
                        
                            x.MUId = Grid.Master.UId;
                            x.Date = Grid.Master.Date;
                            x.ModyfiedBy = EmpId;
                            x.ModyfiedDate = System.DateTime.Now;
                            m.GetGateEntryChildManager().Update(x);
                            Q = (QtyPre ?? 0) - (QtyNow ?? 0);
                            if (x.Status == "LP" || x.Status == "IPO" || x.Status == "Receive")
                            {
                                                            
                                m.GettblItemCurrentStockManager().UpdateForAdd(Grid.Master.ProjectNo, x.ItemGroupNo, x.ItemNo, -Q);
                            }
                            else if (x.Status == "Dispatch")
                            {                              
                               
                                m.GettblItemCurrentStockManager().UpdateForAdd(Grid.Master.ProjectNo, x.ItemGroupNo, x.ItemNo, +Q);
                            }
                          
                            if(x.Status=="LP")
                                m.GetTbCashPurchaseChildManager().UpdateLP_ReceiveBalance(x.StatusChildId??0, -Q);
                            else if (x.Status == "IPO")
                                m.GetTbIndentPurchaseOrderChildManager().UpdateIPO_ReceiveBalance(x.StatusChildId ?? 0, -Q);
                            else if (x.Status == "Receive")
                                m.GetTbOSTTransferChildManager().UpdateOST_ReceiveBalance(x.StatusChildId ?? 0, -Q);
                            else if (x.Status == "Dispatch")
                                m.GetTbOSTTransferChildManager().UpdateOST_DispatchBalance(x.StatusChildId ?? 0, -Q);
                        }
                        else if (!CList.Any(a => a == x.UId))
                        {
                            x.MUId = Grid.Master.UId;
                            x.ProjectNo = Grid.Master.ProjectNo;
                            x.ProjectName = Grid.Master.ProjectName;
                            x.Date =Grid.Master.Date;
                    
                            x.ItemGroupNo = m.GetGobalItemManager().Find(x.ItemNo).ItemGroupID;
                            decimal b = m.GetGateEntryChildManager().Add(x);
                            if (x.Status == "LP" || x.Status == "IPO" || x.Status == "Receive")
                            {

                                m.GettblItemCurrentStockManager().UpdateForAdd(Grid.Master.ProjectNo, x.ItemGroupNo, x.ItemNo, +(x.ReceiveQty ?? 0));
                            }
                            else if (x.Status == "Dispatch")
                            {

                                m.GettblItemCurrentStockManager().UpdateForAdd(Grid.Master.ProjectNo, x.ItemGroupNo, x.ItemNo, -(x.ReceiveQty ?? 0));
                            }

                            if (x.Status == "LP")
                                m.GetTbCashPurchaseChildManager().UpdateLP_ReceiveBalance(x.StatusChildId ?? 0, x.ReceiveQty);
                            else if (x.Status == "IPO")
                                m.GetTbIndentPurchaseOrderChildManager().UpdateIPO_ReceiveBalance(x.StatusChildId ?? 0, x.ReceiveQty);
                            else if (x.Status == "Receive")
                                m.GetTbOSTTransferChildManager().UpdateOST_ReceiveBalance(x.StatusChildId ?? 0, x.ReceiveQty);
                            else if (x.Status == "Dispatch")
                                m.GetTbOSTTransferChildManager().UpdateOST_DispatchBalance(x.StatusChildId ?? 0, x.ReceiveQty);
                        }
                    }
                    if (CList != null)
                        foreach (decimal x in CList)
                        {

                            if (!Grid.Child.Any(a => a.UId == x))
                            {
                                BusinessObjects.Entity.GateEntryChild y = m.GetGateEntryChildManager().Find(x);
                                m.GetTbIndentPurchaseOrderChildManager().Delete(x);
                                if (y.Status == "LP" || y.Status == "IPO" || y.Status == "Receive")
                                {

                                    m.GettblItemCurrentStockManager().UpdateForAdd(Grid.Master.ProjectNo, y.ItemGroupNo, y.ItemNo, -(y.ReceiveQty ?? 0));
                                }
                                else if (y.Status == "Dispatch")
                                {

                                    m.GettblItemCurrentStockManager().UpdateForAdd(Grid.Master.ProjectNo, y.ItemGroupNo, y.ItemNo, +(y.ReceiveQty ?? 0));
                                }
                             
                                if (y.Status == "LP")
                                    m.GetTbCashPurchaseChildManager().UpdateLP_ReceiveBalance(y.StatusChildId ?? 0, -y.ReceiveQty);
                                else if (y.Status == "IPO")
                                    m.GetTbIndentPurchaseOrderChildManager().UpdateIPO_ReceiveBalance(y.StatusChildId ?? 0, -y.ReceiveQty);
                                else if (y.Status == "Receive")
                                    m.GetTbOSTTransferChildManager().UpdateOST_ReceiveBalance(y.StatusChildId ?? 0, -y.ReceiveQty);
                                else if (y.Status == "Dispatch")
                                    m.GetTbOSTTransferChildManager().UpdateOST_DispatchBalance(y.StatusChildId ?? 0, -y.ReceiveQty);
                            }
                        }
                }


                return Json("1", JsonRequestBehavior.AllowGet);
                //return Json("Update Successfully");
            }
            catch { return Json("2", JsonRequestBehavior.AllowGet); }

        }

        //public ActionResult UpdateGridData(string gridData)
        //{
        //    var log = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(gridData);
        //    if (log != null)
        //    {
        //        foreach (var ind in log)
        //        {
        //            tblEmployee e = _dbcontext.tblEmployees.Where(a => a.EmployeeId == Convert.ToInt32(ind.Value[0].ToString())).First();
        //            e.Name = ind.Value[1].ToString();
        //            e.Gender = ind.Value[2].ToString();
        //            e.City = ind.Value[3].ToString();
                  
        //            _dbcontext.Entry(e).State = EntityState.Modified;

        //        }
        //        _dbcontext.SaveChanges();
        //    }
        //    return Json("Update Successfully");
        //}
        public JsonResult GetProjectByEmpId(string E = "0")
        {
            //if (E == null || E == "0" || E == "")
            //    E = EmpId;
            string j = null;
            if (E != "0" && E != "EMP0000001" && E != "")
            {
                string PrjString = m.GetEmployeeMasterManager().FindPrj(E);


                var Prjlist = m.GetProjectMasterManager().FindListByString(PrjString);

                j = Prjlist.ToJSON();
            }
            else
            {
                var Prjlist = m.GetProjectMasterManager().FindListByString(null);

                j = Prjlist.ToJSON();
            }





            return Json(j, JsonRequestBehavior.AllowGet);
        }
        public List<BusinessObjects.Entity.tblProjectMaster> GetProjectByEmpIdG(string E = "0")
        {
            //if (E == null || E == "0" || E == "")
            //    E = EmpId;
            string j = null;
            if (E != "0" && E != "EMP0000001" && E != "")
            {
                string PrjString = m.GetEmployeeMasterManager().FindPrj(E);


                var Prjlist = m.GetProjectMasterManager().FindGListByString(PrjString);

                return Prjlist;
            }
            else
            {
                var Prjlist = m.GetProjectMasterManager().FindGListByString(null);

                return Prjlist;
            }






        }
         public JsonResult GetStatusTypeNo(string ProjectNo,string Type = "0")
        {
            //if (E == null || E == "0" || E == "")
            //    E = EmpId;
            string j = null;
            if (Type=="LP")
            {
                // var Prjlist = m.GetTbCashPurchaseMasterManager().FindListByString(SessionId, ProjectNo);
                var Prjlist = objmms.TbIndentPurchaseOrderMasters.Where(x => x.ProjectNo == ProjectNo && x.POType == Type && x.IsPORelease ==true ).ToList().Select(p => new { Text = p.PurchaseOrderNo, Value = p.UId }).ToList();

                j = Prjlist.ToJSON();
            }
           else  if (Type == "IPO")
            {
               // var Prjlist = m.GetTbIndentPurchaseOrderMasterManager().FindListByString(SessionId, ProjectNo);
                var Prjlist = objmms.TbIndentPurchaseOrderMasters.Where(x => x.ProjectNo == ProjectNo && x.POType == Type && x.IsPORelease == true).ToList().Select(p => new { Text = p.PurchaseOrderNo, Value = p.UId }).ToList();


                j = Prjlist.ToJSON();
            }

            else if (Type == "FC")
            {
                // var Prjlist = m.GetTbIndentPurchaseOrderMasterManager().FindListByString(SessionId, ProjectNo);
                var Prjlist = objmms.TbIndentPurchaseOrderMasters.Where(x => x.ProjectNo == ProjectNo && x.POType == Type && x.IsPORelease == true).ToList().Select(p => new { Text = p.PurchaseOrderNo, Value = p.UId }).ToList();


                j = Prjlist.ToJSON();
            }
            else if (Type == "CH")
            {
                // var Prjlist = m.GetTbIndentPurchaseOrderMasterManager().FindListByString(SessionId, ProjectNo);
                var Prjlist = objmms.TbIndentPurchaseOrderMasters.Where(x => x.ProjectNo == ProjectNo && x.POType == Type && x.IsPORelease == true).ToList().Select(p => new { Text = p.PurchaseOrderNo, Value = p.UId }).ToList();


                j = Prjlist.ToJSON();
            }

            else if (Type == "Dispatch")
            {
                var Prjlist = m.GetTbOSTTransferMasterManager().FindListByStringD(SessionId, ProjectNo);

                j = Prjlist.ToJSON();
            }
            else if (Type == "Receive")
            {
                var Prjlist = m.GetTbOSTTransferMasterManager().FindListByStringR(SessionId, ProjectNo);

                j = Prjlist.ToJSON();
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectGateEntryNo(string id)
        {
            List<BusinessObjects.Entity.tblEmployeeMaster> E = m.GetEmployeeMasterManager().FindByProject(id);


            var EO = E.Select(a => new { Value = a.EmpID, Text = a.FName });
            //  SelectList obgE = new SelectList(E, "EmpID", "FName", 0);

            var stateList = this.GateEntryNo(Convert.ToString(id));

            var v = new { List = EO, Message = stateList };

            string jsonString = v.ToJSON();

            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public string GateEntryNo(string ProjectID)
        {
            // var sessioncode = m.GetSessionMasterManager().Find(SessionId).Name;
            var sessioncode = objmsp.GetFinancialYear();
            //int SiteId = m.GetProjectMasterManager().Find(ProjectID).TransID;
            // old latest  var idMax = m.GetGateEntryManager().FindMaxId(SessionId, ProjectID);
            var divisionId = Repos.GetUserDivision();
            var divisionCode = objmms.tblDivisionMasters.SingleOrDefault(x => x.Id == divisionId).DivisionCode;

            var idMax = G_MidMaxId(SessionId, ProjectID, divisionId);

            string Code = m.GetProjectMasterManager().FindCode(ProjectID);
            string ProjectCode = "";
            if (divisionCode == "")
            {
                ProjectCode = "GRN/" + Code + "/" + sessioncode + "/" + idMax;
            }
            else
            {
                ProjectCode = "GRN/" + Code + "/"+divisionCode+"/" + sessioncode + "/" + idMax;
            }
            

            return ProjectCode;
        }

        public string GateEntryNoByDateWise(string ProjectID, DateTime GRNMid_Date)
        {
            
            string FNYr = GetCurrentFinancialYear(GRNMid_Date);
            var divisionId = Repos.GetUserDivision();
            var divisionCode = objmms.tblDivisionMasters.SingleOrDefault(x => x.Id == divisionId).DivisionCode;
            // var sessioncode = objmsp.GetFinancialYear();
            var S_Id = objmms.SessionMasters.Where(x => x.Name == FNYr.ToString()).FirstOrDefault().Id;
            //int SiteId = m.GetProjectMasterManager().Find(ProjectID).TransID;
            // old latest  var idMax = m.GetGateEntryManager().FindMaxId(SessionId, ProjectID);
            var idMax = G_MidMaxId(S_Id, ProjectID, divisionId);
            string Code = m.GetProjectMasterManager().FindCode(ProjectID);
            string ProjectCode = "";

            if (divisionCode == "")
            {
                ProjectCode = "GRN/" + Code + "/" + FNYr + "/" + idMax;
            }
            else
            {
                ProjectCode = "GRN/" + Code + "/" + divisionCode + "/" + FNYr + "/" + idMax;
            }
         
            return ProjectCode;
        }

        public static string GetCurrentFinancialYear(DateTime InputDate)
        {
            int CurrentYear = InputDate.Year;
            int PreviousYear = InputDate.Year - 1;
            int NextYear = InputDate.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (InputDate.Month > 3)
                FinYear = CurYear + "-" + NexYear.Substring(NexYear.Length - 2);
            else
                FinYear = PreYear + "-" + CurYear.Substring(CurYear.Length - 2);
            return FinYear.Trim();
        }
        public List<SelectListItem> GetStatusList()
        {

            List<SelectListItem> tmpList = new List<SelectListItem>();
            tmpList.Add(new SelectListItem { Text = "Local Purchase", Value = "LP" });
            tmpList.Add(new SelectListItem { Text = "Indent Purchase Order", Value = "IPO" });
            tmpList.Add(new SelectListItem { Text = "Dispatch To Other Site", Value = "Dispatch" });
            tmpList.Add(new SelectListItem { Text = "Receive From Other Site", Value = "Receive" });
            return tmpList;
        }
        public List<SelectListItem> GetStatusListP()
        {

            List<SelectListItem> tmpList = new List<SelectListItem>();

            //tmpList.Add(new SelectListItem { Text = "Local Purchase", Value = "LP" });
            //tmpList.Add(new SelectListItem { Text = "Indent Purchase Order", Value = "IPO" });
            tmpList.Add(new SelectListItem { Text = "Site Purchase", Value = "LP" });
            tmpList.Add(new SelectListItem { Text = "HO Purchase", Value = "IPO" });
            tmpList.Add(new SelectListItem { Text = "FOC Purchase", Value = "FC" });
            tmpList.Add(new SelectListItem { Text = "CASH Purchase", Value = "CH" });
            return tmpList;
        }
        public List<SelectListItem> GetStatusListT()
        {

            List<SelectListItem> tmpList = new List<SelectListItem>();
          
            tmpList.Add(new SelectListItem { Text = "Dispatch To Other Site", Value = "Dispatch" });
            tmpList.Add(new SelectListItem { Text = "Receive From Other Site", Value = "Receive" });
            return tmpList;
        }
        public List<SelectListItem> GetTimeList()
        {

            List<SelectListItem> tmpList = new List<SelectListItem>();
            tmpList.Add(new SelectListItem { Text = "1 AM", Value = "1 AM" });
            tmpList.Add(new SelectListItem { Text = "2 AM", Value = "2 AM" });
            tmpList.Add(new SelectListItem { Text = "3 AM", Value = "3 AM" });
            tmpList.Add(new SelectListItem { Text = "4 AM", Value = "4 AM" });
            tmpList.Add(new SelectListItem { Text = "5 AM", Value = "5 AM" });
            tmpList.Add(new SelectListItem { Text = "6 AM", Value = "6 AM" });
            tmpList.Add(new SelectListItem { Text = "7 AM", Value = "7 AM" });
            tmpList.Add(new SelectListItem { Text = "8 AM", Value = "8 AM" });
            tmpList.Add(new SelectListItem { Text = "9 AM", Value = "9 AM" });
            tmpList.Add(new SelectListItem { Text = "10 AM", Value = "10 AM" });
            tmpList.Add(new SelectListItem { Text = "11 AM", Value = "11 AM" });
            tmpList.Add(new SelectListItem { Text = "12 AM", Value = "12 AM" });
            tmpList.Add(new SelectListItem { Text = "1 PM", Value = "1 PM" });
            tmpList.Add(new SelectListItem { Text = "2 PM", Value = "2 PM" });
            tmpList.Add(new SelectListItem { Text = "3 PM", Value = "3 PM" });
            tmpList.Add(new SelectListItem { Text = "4 PM", Value = "4 PM" });
            tmpList.Add(new SelectListItem { Text = "5 PM", Value = "5 PM" });
            tmpList.Add(new SelectListItem { Text = "6 PM", Value = "6 PM" });
            tmpList.Add(new SelectListItem { Text = "7 PM", Value = "7 PM" });
            tmpList.Add(new SelectListItem { Text = "8 PM", Value = "8 PM" });
            tmpList.Add(new SelectListItem { Text = "9 PM", Value = "9 PM" });
            tmpList.Add(new SelectListItem { Text = "10 PM", Value = "10 PM" });
            tmpList.Add(new SelectListItem { Text = "11 PM", Value = "11 PM" });
            tmpList.Add(new SelectListItem { Text = "12 PM", Value = "12 PM" });
            return tmpList;
        }





        public ActionResult Index()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.fromDate = DateTime.Now.Date;
            ViewBag.toDate = ViewBag.fromDate.Date.AddDays(1);

            if (EmpId == "EMP0000001")
                ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", 0);
            else
                ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", EmpId);
            return View();
        }

        #region IndexCreater
        //public ActionResult Grid2(DateTime? fromDate, DateTime? toDate, string PrjId = null, int page = 1, string f = "C", string E = null)
        //{
        //    if (E == "")
        //        E = null;
        //    if (PrjId == "")
        //        PrjId = null;
        //    if (toDate.HasValue) toDate = toDate.Value.AddDays(1);
        //    const int pageSize = 15000;

        //    if (!fromDate.HasValue) fromDate = DateTime.Now.Date;
        //    if (!toDate.HasValue) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);
        //    if (toDate < fromDate) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);

        //    int totalRows = 0;

        //    if (SessionId != 0)
        //    {
        //        if (E == null || E == "0" || E == "")
        //        {
        //            totalRows = m.GetGateEntryManager().FindForAdminCount("P",SessionId, PrjId, fromDate, toDate);
        //            MasterList = m.GetGateEntryManager().FindForAdmin("P", SessionId, PrjId, fromDate, toDate, page, pageSize);
        //        }
        //        else
        //        {
        //            if (f == "C")
        //            {
        //                totalRows = m.GetGateEntryManager().FindForCreaterCount("P", SessionId, E, PrjId, fromDate, toDate);
        //                MasterList = m.GetGateEntryManager().FindForCreater("P", SessionId, E, PrjId, fromDate, toDate, page, pageSize);
        //            }
        //            else if (f == "A")
        //            {
        //                totalRows = m.GetGateEntryManager().FindForApprovedCount("P", SessionId, E, PrjId, fromDate, toDate);
        //                MasterList = m.GetGateEntryManager().FindForApproved("P", SessionId, E, PrjId, fromDate, toDate, page, pageSize);
        //            }
        //        }



        //        if (MasterList != null)
        //        {



        //            var data = new PagedGateEntryMasterModel()
        //            {
        //                TotalRows = totalRows,
        //                PageSize = pageSize,
        //               Master = MasterList.ToList()
        //            };
        //            if (Request.IsAjaxRequest())
        //            {
        //                return View("_GridView", data);
        //            }
        //        }


        //    }

        //    return View("_EmptyView");
        //}
        //public ActionResult Grid3(string PrjId = null, int page = 1, string f = "C", string E = null)
        //{
        //    if (E == "")
        //        E = null;
        //    if (PrjId == "")
        //        PrjId = null;
        //    const int pageSize = 15000;


        //    int totalRows = 0;

        //    if (SessionId != 0)
        //    {
        //        if (E == null || E == "0" || E == "")
        //        {
        //            totalRows = m.GetGateEntryManager().FindForAdminLastCount("P", SessionId, PrjId);
        //            MasterList = m.GetGateEntryManager().FindForAdminLast("P", SessionId, PrjId, page, pageSize);
        //        }
        //        else
        //        {
        //            if (f == "C")
        //            {
        //                totalRows = m.GetGateEntryManager().FindForCreaterLastCount("P", SessionId, E, PrjId);
        //                MasterList = m.GetGateEntryManager().FindForCreaterLast("P", SessionId, E, PrjId, page, pageSize);
        //            }
        //            else if (f == "A")
        //            {
        //                totalRows = m.GetGateEntryManager().FindForApprovedLastCount("P",SessionId, E, PrjId);
        //                MasterList = m.GetGateEntryManager().FindForApprovedLast("P",SessionId, E, PrjId, page, pageSize);
        //            }
        //        }



        //        if (MasterList != null)
        //        {



        //            var data = new PagedGateEntryMasterModel()
        //            {
        //                TotalRows = totalRows,
        //                PageSize = pageSize,
        //                Master = MasterList.ToList()
        //            };
        //            if (Request.IsAjaxRequest())
        //            {
        //                return View("_GridView", data);
        //            }
        //        }


        //    }

        //    return View("_EmptyView");
        //}



        //public ActionResult IndexT()
        //{
        //    ViewBag.EmpId = EmpId;
        //    ViewBag.fromDate = DateTime.Now.Date;
        //    ViewBag.toDate = ViewBag.fromDate.Date.AddDays(1);

        //    if (EmpId == "EMP0000001")
        //        ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", 0);
        //    else
        //        ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", EmpId);
        //    return View();
        //}
        //public ActionResult Grid2T(DateTime? fromDate, DateTime? toDate, string PrjId = null, int page = 1, string f = "C", string E = null)
        //{
        //    if (E == "")
        //        E = null;
        //    if (PrjId == "")
        //        PrjId = null;
        //    if (toDate.HasValue) toDate = toDate.Value.AddDays(1);
        //    const int pageSize = 15000;

        //    if (!fromDate.HasValue) fromDate = DateTime.Now.Date;
        //    if (!toDate.HasValue) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);
        //    if (toDate < fromDate) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);

        //    int totalRows = 0;

        //    if (SessionId != 0)
        //    {
        //        if (E == null || E == "0" || E == "")
        //        {
        //            totalRows = m.GetGateEntryManager().FindForAdminCount("T",SessionId, PrjId, fromDate, toDate);
        //            MasterList = m.GetGateEntryManager().FindForAdmin("T", SessionId, PrjId, fromDate, toDate, page, pageSize);
        //        }
        //        else
        //        {
        //            if (f == "C")
        //            {
        //                totalRows = m.GetGateEntryManager().FindForCreaterCount("T", SessionId, E, PrjId, fromDate, toDate);
        //                MasterList = m.GetGateEntryManager().FindForCreater("T", SessionId, E, PrjId, fromDate, toDate, page, pageSize);
        //            }
        //            else if (f == "A")
        //            {
        //                totalRows = m.GetGateEntryManager().FindForApprovedCount("T", SessionId, E, PrjId, fromDate, toDate);
        //                MasterList = m.GetGateEntryManager().FindForApproved("T", SessionId, E, PrjId, fromDate, toDate, page, pageSize);
        //            }
        //        }



        //        if (MasterList != null)
        //        {



        //            var data = new PagedGateEntryMasterModel()
        //            {
        //                TotalRows = totalRows,
        //                PageSize = pageSize,
        //                Master = MasterList.ToList()
        //            };
        //            if (Request.IsAjaxRequest())
        //            {
        //                return View("_GridViewT", data);
        //            }
        //        }


        //    }

        //    return View("_EmptyView");
        //}
        //public ActionResult Grid3T(string PrjId = null, int page = 1, string f = "C", string E = null)
        //{
        //    if (E == "")
        //        E = null;
        //    if (PrjId == "")
        //        PrjId = null;
        //    const int pageSize = 15000;


        //    int totalRows = 0;

        //    if (SessionId != 0)
        //    {
        //        if (E == null || E == "0" || E == "")
        //        {
        //            totalRows = m.GetGateEntryManager().FindForAdminLastCount("T", SessionId, PrjId);
        //            MasterList = m.GetGateEntryManager().FindForAdminLast("T", SessionId, PrjId, page, pageSize);
        //        }
        //        else
        //        {
        //            if (f == "C")
        //            {
        //                totalRows = m.GetGateEntryManager().FindForCreaterLastCount("T", SessionId, E, PrjId);
        //                MasterList = m.GetGateEntryManager().FindForCreaterLast("T", SessionId, E, PrjId, page, pageSize);
        //            }
        //            else if (f == "A")
        //            {
        //                totalRows = m.GetGateEntryManager().FindForApprovedLastCount("T", SessionId, E, PrjId);
        //                MasterList = m.GetGateEntryManager().FindForApprovedLast("T", SessionId, E, PrjId, page, pageSize);
        //            }
        //        }



        //        if (MasterList != null)
        //        {



        //            var data = new PagedGateEntryMasterModel()
        //            {
        //                TotalRows = totalRows,
        //                PageSize = pageSize,
        //                Master = MasterList.ToList()
        //            };
        //            if (Request.IsAjaxRequest())
        //            {
        //                return View("_GridViewT", data);
        //            }
        //        }


        //    }

        //    return View("_EmptyView");
        //}
        #endregion endindex

        #region DMR
        public JsonResult GetItemByGroup(string id)
        {
            if (id != null)
            {
                object ItemMasters = m.GetGobalItemManager().FindByItemGroup(id);
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        // newly added

        public int G_MidMaxId(int? SessionId , string PjId,int DivisionId)
        {
            int a = objmms.GateEntry_Mid.Where(i =>i.DivisionId==DivisionId && i.SessionId == SessionId && i.ProjectNo == PjId).Select(i => i.GateEntryId).DefaultIfEmpty(0).Max(i => i).Value;
            if (a == 0)
                return 1;
            else
                return a + 1;
        }
     // end here

        public JsonResult GetItemByGroup1(string id)
        {
            if (id != null)
            {
                object ItemMasters = m.GetGobalItemManager().FindByItemGroup1(id);
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetVendorByGroup(string Vid, string Pid)
        {
            object VendorMasters = m.GetVendorMasterManager().FindByVendorGroup(Vid, Pid);
            string jsonString = VendorMasters.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVendorItemByGateEntry( string Pid)
        {
            object VendorMasters = m.GetGateEntryChildManager().FindVendorNameByGateEntryChild( Pid);
            string Vendor = VendorMasters.ToJSON();
            object ItemMasters = m.GetGateEntryChildManager().FindItemNameByGateEntryChild(Pid);
            string Item = ItemMasters.ToJSON();
            object ItemGroups = m.GetGateEntryChildManager().FindItemGroupByGateEntryChild(Pid);
            string ItemGroup = ItemGroups.ToJSON();

            var v = new { Vendor = Vendor, Item = Item, ItemGroup = ItemGroup };
            //var v = new { Vendor = Vendor, ItemGroup = ItemGroup };
           
            string jsonString = v.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DMR()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.fromDate = DateTime.Now.Date;
            ViewBag.toDate = ViewBag.fromDate.Date.AddDays(1);
           // ViewBag.ItemGroup = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");
            ViewBag.VGId = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", "TID0000001");
            ViewBag.VNAME = new SelectList(m.GetVendorMasterManager().FindByVendorGroup("TID0000001"), "VendorId", "Name", "For All Vendors ");
           
                ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", EmpId);
            return View();
        }
        public ActionResult DMRGrid(string ReceiveType,DateTime? fromDate, DateTime? toDate, string PrjId = null, int page = 1, string IG = null, string IT = null,string Vg=null)
        {
           // For All Vendors

            if (Vg.ToString().Trim() == "For All Vendors")
                Vg = null;
            if (PrjId == "")
                PrjId = null;
            if (toDate.HasValue) toDate = toDate.Value.AddDays(1);
            const int pageSize = 15000;

            if (!fromDate.HasValue) fromDate = DateTime.Now.Date;
            if (!toDate.HasValue) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);
            if (toDate < fromDate) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);

            int totalRows = 0;
            List<BusinessObjects.Entity.GateEntryChild> MasterList = null;
            if (SessionId != 0)
            {


                if (PrjId != null && PrjId != "0" && PrjId != "")
                {
                    if (Vg == null || Vg == "0"|| Vg == "")
                    {

                        if ((IG == null || IG == "0" || IG == "") && (IT == null || IT == "0" || IT == ""))
                        {
                            totalRows = m.GetGateEntryChildManager().FindByProjectCount(ReceiveType,PrjId, fromDate, toDate);
                            MasterList = m.GetGateEntryChildManager().FindByProject(ReceiveType, PrjId, fromDate, toDate, page, pageSize);

                        }
                        else if ((IG != null && IG != "0" && IG != "") && (IT == null || IT == "0" || IT == ""))
                        {
                            totalRows = m.GetGateEntryChildManager().FindByIGCount(ReceiveType, PrjId, IG, fromDate, toDate);
                            MasterList = m.GetGateEntryChildManager().FindByIG(ReceiveType, PrjId, IG, fromDate, toDate, page, pageSize);

                        }
                        else if ((IG != null && IG != "0" && IG != "") && (IT != null && IT != "0" && IT != ""))
                        {
                            totalRows = m.GetGateEntryChildManager().FindByITCount(ReceiveType, PrjId, IT, fromDate, toDate);
                            MasterList = m.GetGateEntryChildManager().FindByIT(ReceiveType, PrjId, IT, fromDate, toDate, page, pageSize);

                        }
                    }
                    else
                    {
                        if ((IG == null || IG == "0" || IG == "") && (IT == null || IT == "0" || IT == ""))
                        {
                            totalRows = m.GetGateEntryChildManager().FindByVendorCount(ReceiveType, PrjId, Vg, fromDate, toDate);
                            MasterList = m.GetGateEntryChildManager().FindByVendor(ReceiveType, PrjId, Vg, fromDate, toDate, page, pageSize);

                        }
                        else if ((IG != null && IG != "0" && IG != "") && (IT == null || IT == "0" || IT == ""))
                        {
                            totalRows = m.GetGateEntryChildManager().FindByVendorIGCount(ReceiveType, PrjId, Vg, IG, fromDate, toDate);
                            MasterList = m.GetGateEntryChildManager().FindByVendorIG(ReceiveType, PrjId, Vg, IG, fromDate, toDate, page, pageSize);

                        }
                        else if ((IG != null && IG != "0" && IG != "") && (IT != null && IT != "0" && IT != ""))
                        {
                            totalRows = m.GetGateEntryChildManager().FindByVendorITCount(ReceiveType, PrjId, Vg, IT, fromDate, toDate);
                            MasterList = m.GetGateEntryChildManager().FindByVendorIT(ReceiveType, PrjId, Vg, IT, fromDate, toDate, page, pageSize);

                        }

                    }
                }
                if (MasterList != null)
                {



                    var data = new PagedGateEntryChildModel()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                       Child = MasterList.ToList()
                    };
                    if (Request.IsAjaxRequest())
                    {
                        if (ReceiveType == "Purchase")
                        {
                            return View("_DMRPurchaseView", data);
                        }
                        else if (ReceiveType == "Dispatch")
                        {
                            return View("_DMRDispatchView", data);
                        }
                        else if (ReceiveType == "Receive")
                        {
                            return View("_DMRReceiveView", data);
                        }
                        return View("_DMRAllView", data);
                    }
                }


            }

            return View("_EmptyView");
        }
      
        #endregion endindex
       
        public ActionResult Details(decimal id)
        {
            // id = 17;
          
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BusinessObjects.Entity.GateEntry Master = m.GetGateEntryManager().Find(id);
            if (Master.Date != null)
            Master.Date2 = Master.Date.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            if (Master.ChallanDate!=null)
                Master.ChallanDate2 = Master.ChallanDate.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            if (Master.Status == "LP")
                Master.Status = "Local Purchase";
            else if(Master.Status == "IPO")
                Master.Status = "Indent Purchase Order";
            List<BusinessObjects.Entity.GateEntryChild> ChildList = m.GetGateEntryChildManager().FindByMaster(id);
            var totalRows = ChildList.Count();

            var data = new GateEntryDetailModel()
            {
                
                Master = Master,
                Child = ChildList
            };


            if (data == null)
            {
                return HttpNotFound();
            }
            if(Master.Status=="Dispatch")
            return View("DetailsDispatch",data);
            else   if (Master.Status == "Receive")
                return View("DetailsReceive", data);
            else
                return View( data);
            //


        }

        public JsonResult GetGRN(int PONumber)
        {
            string j = null;
            var Glst = objmms.GateEntries.Where(x => x.StatusTypeId == PONumber).ToList().Select( y=> new { Text=y.GateEntryNo,Value=y.UId}).ToList();
            j = Glst.ToJSON();
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        //here code for cascading item name accoding to item group name

        //public JsonResult GetItemByGroup(string id)
        //{
        //    if (id != null)
        //    {
        //        object ItemMasters = m.GetGobalItemManager().FindByItemGroup1(id);
        //        string jsonString = ItemMasters.ToJSON();
        //        return Json(jsonString, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //}

        public ActionResult GetTransferListAtGRN(string TransferNo, string Type)
        {
            MMS.Models.Procedure OBJP = new Procedure();
            List<TransferItemDetailGRN> lst = null;
            if (Type == "6")
            { lst = OBJP.Get_TransferItemdetailONGRN(TransferNo); }
            else if (Type == "5") { lst = OBJP.Get_TransferItemdetailONGRN_Inter(TransferNo); }

            var data = new GridGRN_TransferItemList()
            { Child = lst };
            if (Request.IsAjaxRequest())
            {
                return View("_TransferItemsListONGRN", data);

            }
            return View();
        }




    }
}