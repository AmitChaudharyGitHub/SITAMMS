using DataAccessLayer;
using MMS.DAL;
using MMS.Helpers;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class MaterialRevController : Controller
    {
        // GET: MaterialRev
        string EmpId = string.Empty; int SessionId = 1;
        int SiteId = 1; string EmpName = string.Empty;
        FactoryManager m = new FactoryManager();
        DAL.MMSEntities objmms = new DAL.MMSEntities();
        MSP_Model objmsp = new MSP_Model();
        Procedure objpro = new Procedure();
        public MaterialRevController()
        {
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            EmpName = System.Web.HttpContext.Current.Session["Emp_Name"].ToString();
          //  SessionId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SessionId"].ToString());
          //  SiteId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteId"].ToString());
        }
        public ActionResult Index()
        {
            return View();
        }

       


        public ActionResult Create()
        {
            ViewBag.EmpId = EmpId; ViewBag.Date = System.DateTime.Now;
            ViewBag.Status = GetStatusListP();
            return View();
        }

        public JsonResult IsBillNoDuplicate(string BillNo)
        {
            if (BillNo != "")
            {
                BillNo = BillNo.Trim();
                try
                {
                    if (objmms.GateEntries.Any(x => x.ChallanNo.ToLower() == BillNo.ToLower()))
                    {
                        return Json(new { Status = 2 }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Status = 1 }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Status = 3, Error = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Status = 2 }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreateNewMRN(string GRNNo, string Purchase_Type)
        {
            string GRNNoVal = GRNNo.Replace("#", "/");
            var data = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == GRNNoVal).FirstOrDefault();
            ViewBag.EmpId = EmpId; ViewBag.Date = System.DateTime.Now;
            ViewBag.GRN_No = GRNNoVal;
            ViewBag.GRN_No_UID = data.UId;
            ViewBag.GRN_PRoJID = data.ProjectNo;
            ViewBag.PurchaseType = Purchase_Type;
            ViewBag.GateEntryDate = data.Date.Value.ToString("dd MMM yyyy");
            ViewBag.ChallanNo = data.ChallanNo;
            ViewBag.ChallanDate = data.ChallanDate.Value.ToString("dd MMM yyyy");
            ViewBag.VehicalNo = data.VehicleNo;
            ViewBag.PONo = data.StatusTypeNo;



            int PType = Convert.ToInt32(Purchase_Type);

            ViewBag.IsShowVendor = false;

            if(PType != 5 && PType != 6 && PType != 8)
            {
                var PoData = objmms.TbIndentPurchaseOrder_GST.FirstOrDefault(x => x.PurchaseOrderNo == data.StatusTypeNo);
                ViewBag.PINo = PoData.IndentRefNo;
               var VendorDetails = objmms.tblVendorMasters.SingleOrDefault(x => x.VendorID == PoData.SupplierNo);
                var Country = objmms.tblCountries.SingleOrDefault(x => x.CountryID == VendorDetails.Country).CnName;
                var State = objmms.tblStates.SingleOrDefault(x => x.StateID == VendorDetails.State).StateName;
                var City = objmms.tblCityLists.SingleOrDefault(x => x.CityID == VendorDetails.City).CityName;
                ViewBag.VendorName = VendorDetails.Name;
                ViewBag.VendorAddress = VendorDetails.Address + City + "," + State + "," + Country;
                ViewBag.IsShowVendor = true;
            }

           
            return View();
        }


        public JsonResult DisapproveGRN(string GRNNo)
        {
            if (!string.IsNullOrEmpty(GRNNo))
            {
                try
                {
                    var data = objmms.GateEntry_Mid.FirstOrDefault(x => x.GateEntryNo == GRNNo);
                    if (data != null)
                    {

                        var mHistory = new GateEntry_Mid_history()
                        {
                            ChallanDate = data.ChallanDate,
                            ChallanNo = data.ChallanNo,
                            CreatedBy = data.CreatedBy,
                            CreatedDate = data.CreatedDate,
                            CreaterName = data.CreaterName,
                            Date = data.Date,
                            EmpId = data.EmpId,
                            EmpName = data.EmpName,
                            GateEntryId = data.GateEntryId,
                            GateEntryNo = data.GateEntryNo,
                            IsMRN_Number = data.IsMRN_Number,
                            ProjectName = data.ProjectName,
                            ProjectNo = data.ProjectNo,
                            SessionId = data.SessionId,
                            Status = data.Status,
                            StatusTypeId = data.StatusTypeId,
                            StatusTypeNo = data.StatusTypeNo,
                            Time = data.Time,
                            UId = data.UId,
                            VehicleNo = data.VehicleNo
                        };

                        objmms.GateEntry_Mid_history.Add(mHistory);

                        data.GRNStatus = "Disapproved";
                        var itemData = objmms.GateEntryChild_Mid.Where(x => x.GateEntryNo == GRNNo);
                        if (itemData != null)
                        {

                            foreach (var item in itemData)
                            {

                                var cHistory = new GateEntryChild_Mid_History()
                                {
                                    UId = item.UId,
                                    AllAmount = item.AllAmount,
                                    Amount = item.Amount,
                                    CreatedBy = item.CreatedBy,
                                    CreatedDate = item.CreatedDate,
                                    Date = item.Date,
                                    DeliveryAmount = item.DeliveryAmount,
                                    DeliveryUnitCharge = item.DeliveryUnitCharge,
                                    GateEntryDate = item.GateEntryDate,
                                    GateEntryNo = item.GateEntryNo,
                                    InOut = item.InOut,
                                    Item = item.Item,
                                    ItemGroup = item.ItemGroup,
                                    ItemNo = item.ItemNo,
                                    ItemGroupNo = item.ItemGroupNo,
                                    ModyfiedBy = item.ModyfiedBy,
                                    ModyfiedDate = item.ModyfiedDate,
                                    MRN_Receive = item.MRN_Receive,
                                    MUId = item.MUId,
                                    ProjectName = item.ProjectName,
                                    ProjectNo = item.ProjectNo,
                                    Rate = item.Rate,
                                    ReceiveQty = item.ReceiveQty,
                                    SessionId = item.SessionId,
                                    Status = item.Status,
                                    StatusChildId = item.StatusChildId,
                                    StatusTypeId = item.StatusTypeId,
                                    StatusTypeNo = item.StatusTypeNo,
                                    TaxAmount = item.TaxAmount,
                                    TaxPer = item.TaxPer,
                                    Unit = item.Unit,
                                    UnitNo = item.UnitNo,
                                    Vendor = item.Vendor,
                                    VendorNo = item.VendorNo
                                };
                                objmms.GateEntryChild_Mid_History.Add(cHistory);

                                item.ReceiveQty = new decimal(0.00);
                                objmms.Entry(item).State = EntityState.Modified;
                            }

                            objmms.Entry(data).State = EntityState.Modified;
                            objmms.SaveChanges();
                            return Json(new { Status = 1 });
                        }
                        
                        
                        return Json(new { Status = 2 });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Status = 3,Error=ex.Message });
                }
                
            }
            return Json(new {Status=2});
        }

        public ActionResult GetGridTRans(string GRNID, string PType)
        {
            GateEntryDetailOnMRNViewModel ob = new GateEntryDetailOnMRNViewModel();
            ob.GRN_Mod = new List<GRNMod>();

            ob.GRN_Mod.AddRange(objpro.GetGRNDetailOnMRNGSTForTRNS(GRNID, PType));



            if (ob != null)
            {
                return PartialView("_GrnGridOSTRN", ob);
            }
            else
            {
                return PartialView("_EmptyView");
            }

        }
        public JsonResult GetDetailOnMRNByGRN(string GRN)
        {
            string ProjectNo = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == GRN).FirstOrDefault().ProjectNo;
            string ProjectName = objmms.tblProjectMasters.Where(x=>x.PRJID ==ProjectNo).FirstOrDefault().ProjectName;
            string PONo = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == GRN).FirstOrDefault().StatusTypeNo;
            string SesId = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == GRN).FirstOrDefault().SessionId.ToString();
            string PurReqNo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PONo).FirstOrDefault().IndentRefNo;
            int? PurchaseId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReqNo).FirstOrDefault().PurchasePIC_Type;
            var PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == PurchaseId).FirstOrDefault().PurchaseType;
            string GRNDate = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == GRN).FirstOrDefault().Date.ToString();
            string MRNData = objpro.GetMRN(ProjectNo, Convert.ToInt32(SesId));
            //string MRNNO = objpro.GetMRN_New(ProjectNo);
            string MRNNO = null;

            var MList = new { ProJname = ProjectName, MRnno = MRNNO, MRNData= MRNData ,PurchaseStatus = PurchaseType + " Gate In", GRnNoval = GRN , GRN_Date = GRNDate };
            string jsonString = MList.ToJSON();

            return Json(jsonString, JsonRequestBehavior.AllowGet);
           
        }

        public JsonResult GetMRNNoByDateWise(string ProjectId, string MRNDate)
        {
           
            string FnYr = GetCurrentFinancialYear( Convert.ToDateTime(MRNDate));
            var SessId = objmms.SessionMasters.Where(x => x.Name == FnYr.ToString()).FirstOrDefault().Id;
            string MRNNO = objpro.GetMRN_New(ProjectId, SessId ,  Convert.ToDateTime(MRNDate));
            var MrnLst = new { MRN_No = MRNNO };
            string J = MrnLst.ToJSON();
            return Json(J, JsonRequestBehavior.AllowGet);
        }

        private string GetMRNNoByDate(string ProjectId, DateTime? MRNDate)
        {
            string FnYr = GetCurrentFinancialYear(MRNDate.Value);
            var SessId = objmms.SessionMasters.Where(x => x.Name == FnYr.ToString()).FirstOrDefault().Id;
            string MRNNO = objpro.GetMRN_New(ProjectId, SessId, Convert.ToDateTime(MRNDate));
            return MRNNO;
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
        public JsonResult GetAllGRNStatusWise(string Status , string ProjectId)
        {
            string j = null;
            var res = objmms.GateEntry_Mid.Where(x => x.Status == Status && x.ProjectNo == ProjectId && x.IsMRN_Number ==null ).OrderByDescending(k=>k.CreatedDate).ToList().Select(p => new { Text = p.GateEntryNo, Value = p.UId }).ToList();
            j = res.ToJSON();
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> GetStatusListP()
        {

            List<SelectListItem> tmpList = new List<SelectListItem>();

            //tmpList.Add(new SelectListItem { Text = "Local Purchase", Value = "LP" });
            //tmpList.Add(new SelectListItem { Text = "Indent Purchase Order", Value = "IPO" });
            tmpList.Add(new SelectListItem { Text = "Site Purchase Gate In", Value = "LP" });
            tmpList.Add(new SelectListItem { Text = "HO Purchase Gate In", Value = "IPO" });
            tmpList.Add(new SelectListItem { Text = "FOC Purchase", Value = "FC" });
            tmpList.Add(new SelectListItem { Text = "CASH Purchase", Value = "CH" });
            return tmpList;
        }

        public JsonResult GetMRNData(string ProjID)
        {
            string str = string.Empty; string JsonString = string.Empty;
            str = objpro.GetMRN(ProjID, 1); // checke later please. 
            JsonString = str.ToJSON();
            return Json(JsonString, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetMRNData_New(string ProjID)
        //{
        //    string str = string.Empty; string JsonString = string.Empty;
        //    str = objpro.GetMRN_New(ProjID);
        //    JsonString = str.ToJSON();
        //    return Json(JsonString, JsonRequestBehavior.AllowGet);
        //}




        public ActionResult GetGrid(string GRNID,string Type="")
        {
            GateEntryDetailOnMRNViewModel ob = new GateEntryDetailOnMRNViewModel();
            ob.GRN_Mod = new List<GRNMod>();
            ob.GRN_Mod.AddRange(objpro.GetGRNDetailOnMRNGST(GRNID));
            if (Type == "ManualReceive")
                ViewBag.Type = "ManualReceive";
            if (ob != null)
            {
                return PartialView("_GrnGrid", ob);
            }
            else {
                return PartialView("_EmptyView");
            }
           
        }

        public ActionResult InsertMRN(InsertMRN Grid)
        {
            using (var trans=objmms.Database.BeginTransaction())
            {
                try
                {
                    #region 

                    //Grd.Master.CreatedBy = EmpId;
                    //Grd.Master.CreatedDate = System.DateTime.Now;
                    //Grd.Master.InOut = "In";
                    //objmms.tblMaterialReceiveEntries.Add(Grd.Master);
                    //if (Grd.Child != null)
                    //{
                    //    foreach (var x in Grd.Child)
                    //    {
                    //        tblReceiveData obj = new tblReceiveData();
                    //        if (x.MR_ReceiveQty != null && x.MR_ReceiveQty != 0)
                    //        {
                    //            x.CreatedDate = System.DateTime.Now;
                    //            x.MRN_Id = FindMaxId(Grd.Master.ProjectId);


                    //            obj.ID = Convert.ToInt32(x.MRN_Id);
                    //            obj.IDType = "Material Receive";
                    //            obj.TypeNumber = Grd.Master.MRN_No;
                    //            obj.TypeDate = Grd.Master.MRN_Date;
                    //            obj.ReceiveQuantity = x.MR_ReceiveQty ?? 0;
                    //            obj.IssueQuantity = 0;
                    //            obj.BalanceQuantity = x.MR_ReceiveQty ?? 0;
                    //            obj.Rate = x.Rate;
                    //            obj.ProjectId = Grd.Master.ProjectId;
                    //            obj.ItemId = x.ItemId;
                    //            obj.CreatedDate = System.DateTime.Now;
                    //            obj.CreatedBy = EmpId;
                    //            obj.Status = Convert.ToBoolean(1);
                    //            obj.Isdeleted = Convert.ToBoolean(0);
                    //            obj.FinYear = objmsp.GetFinancialYear();
                    //            obj.ItemGroupId = x.ItemGroupId;

                    //            if (Grd.Master.StatusType == "LP")
                    //            {

                    //                BusinessObjects.Entity.TbCashPurchaseChild ch = m.GetTbCashPurchaseChildManager().Find(x.POChild_ID ?? 0);
                    //                ch.Receive = (ch.Receive ?? 0) + x.MR_ReceiveQty;
                    //                m.GetTbCashPurchaseChildManager().Update(ch);
                    //                m.GetTbCashPurchaseMasterManager().FindComplete(x.POID ?? 0);
                    //            }
                    //            else if (Grd.Master.StatusType == "IPO")
                    //            {
                    //                BusinessObjects.Entity.TbIndentPurchaseOrderChild ch = m.GetTbIndentPurchaseOrderChildManager().Find(x.POChild_ID ?? 0);
                    //                ch.Receive = (ch.Receive ?? 0) + x.MR_ReceiveQty;
                    //                m.GetTbIndentPurchaseOrderChildManager().Update(ch);
                    //                m.GetTbIndentPurchaseOrderMasterManager().FindComplete(x.POID ?? 0);

                    //                DAL.GateEntryChild gch = objmms.GateEntryChilds.Where(p => p.UId == x.GateEntryChild_Id).ToList().FirstOrDefault();
                    //                //gch.MRN_Receive =

                    //            }




                    //            if (Grd.Master.StatusType == "LP" || Grd.Master.StatusType == "IPO" || Grd.Master.StatusType == "Receive")
                    //            {
                    //                m.GettblItemCurrentStockManager().UpdateForAdd(Grd.Master.ProjectId, x.ItemGroupId, x.ItemId, +(x.MR_ReceiveQty ?? 0));
                    //            }
                    //        }
                    //        objmms.tblReceiveDatas.Add(obj);
                    //        objmms.tblMaterialReceiveEntryChilds.Add(x);
                    //    }

                    //}
                    //objmms.SaveChanges();
                    #endregion

                    #region
                    string FNYr = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.Date));

                    var S_Id = objmms.SessionMasters.Where(x => x.Name == FNYr.ToString()).FirstOrDefault().Id;
                    
                    //MRn No not mrn_No_new
                    var gateentryNo = objpro.GetMRN(Grid.Master.ProjectNo, S_Id);

                    //mrn_no_new 
                    var mrn_no_new = GetMRNNoByDate(Grid.Master.ProjectNo,Grid.Master.Date);

                    #region Check MRN Entry Of Given GRN already created. code on 20_03_2019 

                    if (objmms.GateEntries.Where(s => s.GateEntry_Mid_No == Grid.Master.GateEntry_Mid_No).Count() > 0)
                    {
                        trans.Rollback();
                        return Json("-1", JsonRequestBehavior.AllowGet);
                    }

                    #endregion

                    Grid.Master.DivisionId = Repos.GetUserDivision();

                    Grid.Master.SessionId = S_Id;
                    Grid.Master.CreatedBy = EmpId;
                    Grid.Master.CreaterName = EmpName;
                    Grid.Master.CreatedDate = DateTime.Now;
                    Grid.Master.GateEntryNo = gateentryNo;
                    Grid.Master.MRN_No_New = mrn_no_new;
                    // Added
                    var gr = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == Grid.Master.GateEntry_Mid_No).ToList().FirstOrDefault();

                    //RSTNo.
                    Grid.Master.RSTNo = Grid.RSTNo;

                    Grid.Master.EmpId = gr.EmpId;
                    Grid.Master.EmpName = gr.EmpName;
                    Grid.Master.ChallanNo = gr.ChallanNo;
                    Grid.Master.ChallanDate = gr.ChallanDate;
                    Grid.Master.VehicleNo = gr.VehicleNo;
                    Grid.Master.StatusTypeId = gr.StatusTypeId;
                    Grid.Master.StatusTypeNo = gr.StatusTypeNo;
                    Grid.Master.GateEntryId = FindMaxId(Grid.Master.ProjectNo, Grid.Master.SessionId, Grid.Master.DivisionId.Value);


                    objmms.GateEntries.Add(Grid.Master); objmms.SaveChanges();

                    var data = objmms.GateEntry_Mid.SingleOrDefault(x => x.GateEntryNo == Grid.Master.GateEntry_Mid_No);
                    data.ChallanDate = Convert.ToDateTime(Grid.ChallanDate);
                    data.ChallanNo = Grid.ChallanNo;
                    data.VehicleNo = Grid.VehicalNo;
                    data.EwayBillNo = Grid.EwayBillNo;
                    data.DMRDate = Convert.ToDateTime(Grid.ChallanDate);
                    objmms.Entry(data).State = EntityState.Modified;
                    objmms.SaveChanges();
                    //
                    //   Grid.Master.GateEntryId = m.GetGateEntryManager().FindMaxId(SessionId, Grid.Master.ProjectNo);
                    decimal a = objmms.GateEntries.Where(x => x.GateEntryNo == Grid.Master.GateEntryNo && x.GateEntryId == Grid.Master.GateEntryId).FirstOrDefault().UId;
                    // m.GetGateEntryManager().Add(Grid.Master);
                    if (Grid.Child != null && a != 0)
                    {

                        foreach (var x in Grid.Child)
                        {
                            x.GateEntryNo = gateentryNo;
                            tblReceiveData obj = new tblReceiveData();
                            if (x.ReceiveQty != null && x.ReceiveQty != 0)
                            {
                                x.MUId = a;
                                x.ProjectNo = Grid.Master.ProjectNo;
                                x.ProjectName = Grid.Master.ProjectName;
                                x.Date = Grid.Master.Date;
                                x.GateEntryDate = Grid.Master.Date;
                                x.CreatedBy = EmpId;
                                x.CreatedDate = System.DateTime.Now;
                                x.SessionId = S_Id;
                                x.Amount = x.ReceiveQty * (x.Rate ?? 0);
                                x.TaxAmount = x.ReceiveQty * (x.TaxPer ?? 0);
                                x.DeliveryAmount = x.ReceiveQty * (x.DeliveryUnitCharge ?? 0);
                                x.AllAmount = x.Amount + x.TaxAmount + x.DeliveryAmount;
                                x.MRN_Receive = x.ReceiveQty;
                                x.StatusTypeNo = objmms.TbIndentPurchaseOrder_GST.Where(p => p.UId == x.StatusTypeId).ToList().FirstOrDefault().PurchaseOrderNo;
                                x.Vendor = objmms.GateEntryChild_Mid.Where(l => l.UId == x.GateEntryChild_Mid_Id).ToList().FirstOrDefault().Vendor;
                                x.VendorNo = objmms.GateEntryChild_Mid.Where(l => l.UId == x.GateEntryChild_Mid_Id).ToList().FirstOrDefault().VendorNo;

                                // code for tblReceiveData
                                obj.ID = Convert.ToInt32(a);
                                obj.IDType = "GateEntry";
                                obj.TypeNumber = Grid.Master.GateEntryNo;
                                obj.TypeDate = Grid.Master.Date;
                                obj.ReceiveQuantity = x.ReceiveQty ?? 0;
                                obj.IssueQuantity = 0;
                                obj.BalanceQuantity = x.ReceiveQty ?? 0;
                                obj.Rate = x.Rate;
                                obj.ProjectId = Grid.Master.ProjectNo;
                                obj.ItemId = x.ItemNo;
                                obj.CreatedDate = System.DateTime.Now;
                                obj.CreatedBy = EmpId;
                                obj.Status = Convert.ToBoolean(1);
                                obj.Isdeleted = Convert.ToBoolean(0);
                                obj.FinYear = objmsp.GetFinancialYear();


                                // End Here
                                if (x.Status == "LP" || x.Status == "IPO" || x.Status == "CH" || x.Status == "FC" || x.Status == "RO" || x.Status == "MTR" || x.Status == "EP" || x.Status == "PP" || x.Status == "MP")
                                {
                                    BusinessObjects.Entity.tblItemMaster item = m.GetGobalItemManager().Find(x.ItemNo);
                                    x.ItemGroupNo = item.ItemGroupID;
                                    obj.ItemGroupId = item.ItemGroupID;
                                    x.UnitNo = item.UnitID;
                                    x.InOut = "In";
                                    //x.Unit = item.UnitMasterPurchase.UnitName;
                                    MMS.DAL.GateEntry_Mid gm = objmms.GateEntry_Mid.Where(k => k.UId == x.GateEntry_Mid_Id).ToList().FirstOrDefault();
                                    gm.IsMRN_Number = Grid.Master.GateEntryNo;
                                    objmms.Entry(gm).State = EntityState.Modified;
                                    objmms.SaveChanges();

                                }

                                if (x.Status == "LP" || x.Status == "EP" || x.Status == "PP" || x.Status == "MP")
                                {

                                    //BusinessObjects.Entity.TbCashPurchaseChild ch = m.GetTbCashPurchaseChildManager().Find(x.StatusChildId ?? 0);
                                    //ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                                    //m.GetTbCashPurchaseChildManager().Update(ch);
                                    //m.GetTbCashPurchaseMasterManager().FindComplete(x.StatusTypeId ?? 0);
                                    //  BusinessObjects.Entity.TbIndentPurchaseOrderChild ch = m.GetTbIndentPurchaseOrderChildManager().Find(x.StatusChildId ?? 0);
                                    //ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                                    //m.GetTbIndentPurchaseOrderChildManager().Update(ch);
                                    //m.GetTbIndentPurchaseOrderMasterManager().FindComplete(x.StatusTypeId ?? 0); // no  need in tbindentpurchaseorder gst child table.



                                    MMS.DAL.TbIndentPurchaseOrderChild_GST ch = objmms.TbIndentPurchaseOrderChild_GST.Where(x1 => x1.UId == x.StatusChildId).FirstOrDefault();
                                    ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                                    objmms.Entry(ch).State = EntityState.Modified;
                                    objmms.SaveChanges();


                                }
                                else if (x.Status == "IPO" || x.Status == "CH" || x.Status == "FC" || x.Status == "RO" || x.Status == "MTR")
                                {

                                    // BusinessObjects.Entity.TbIndentPurchaseOrderChild ch = m.GetTbIndentPurchaseOrderChildManager().Find(x.StatusChildId ?? 0);
                                    // ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                                    //m.GetTbIndentPurchaseOrderChildManager().Update(ch);
                                    // m.GetTbIndentPurchaseOrderMasterManager().FindComplete(x.StatusTypeId ?? 0);
                                    MMS.DAL.TbIndentPurchaseOrderChild_GST ch = objmms.TbIndentPurchaseOrderChild_GST.Where(x1 => x1.UId == x.StatusChildId).FirstOrDefault();
                                    ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                                    objmms.Entry(ch).State = EntityState.Modified;
                                    objmms.SaveChanges();

                                }
                                else if (x.Status == "Dispatch")
                                {
                                    x.InOut = "Out";
                                    BusinessObjects.Entity.TbOSTTransferChild ch = m.GetTbOSTTransferChildManager().Find(x.StatusChildId ?? 0);
                                    //x.ReceiveQty use As Dispatch Qty
                                    ch.Dispatch = (ch.Dispatch ?? 0) + x.ReceiveQty;
                                    m.GetTbOSTTransferChildManager().Update(ch);
                                    m.GetTbOSTTransferMasterManager().FindCompleteD(x.StatusTypeId ?? 0);
                                }
                                else if (x.Status == "Receive")
                                {
                                    x.InOut = "In";
                                    BusinessObjects.Entity.TbOSTTransferChild ch = m.GetTbOSTTransferChildManager().Find(x.StatusChildId ?? 0);
                                    ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                                    m.GetTbOSTTransferChildManager().Update(ch);
                                    m.GetTbOSTTransferMasterManager().FindCompleteR(x.StatusTypeId ?? 0);
                                }
                                // m.GetGateEntryChildManager().Add(x);
                                objmms.GateEntryChilds.Add(x);
                                objmms.tblReceiveDatas.Add(obj); objmms.SaveChanges();
                                if (x.Status == "LP" || x.Status == "IPO" || x.Status == "Receive" || x.Status == "CH" || x.Status == "FC" || x.Status == "RO" || x.Status == "MTR")
                                {
                                    m.GettblItemCurrentStockManager().UpdateForAdd(Grid.Master.ProjectNo, x.ItemGroupNo, x.ItemNo, +(x.ReceiveQty ?? 0));
                                }
                                else if (x.Status == "Dispatch")
                                {
                                    m.GettblItemCurrentStockManager().UpdateForAdd(Grid.Master.ProjectNo, x.ItemGroupNo, x.ItemNo, -(x.ReceiveQty ?? 0));
                                }
                            }
                        }





                    }
                    #endregion

                    trans.Commit();
                    return Json(new { Status = 1, TransNo = Grid.Master.MRN_No_New }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Json("2", JsonRequestBehavior.AllowGet);
                } 
            }

            
        }

        public int FindMaxId(string ProjectId , int? SessionId,int DivisionId)
        {
            // decimal idd = objmms.tblMaterialReceiveEntries.Where(x=>x.ProjectId == ProjectId).ToList().Select(p=>p.MRN_Id).DefaultIfEmpty(0).Max(i=>i);
            
            int dd = objmms.GateEntries.Where(x =>x.DivisionId==DivisionId && x.ProjectNo == ProjectId && x.SessionId == SessionId).ToList().Select(p => p.GateEntryId).DefaultIfEmpty(0).Max(i => i).Value;
            if (dd == 0)
            { return 1; }
            else { return dd + 1; }
        }

        public ActionResult InsertMRNTRN(InsertMRN Grid)
        {
            using (var trans=objmms.Database.BeginTransaction())
            {
                try
                {


                    #region

                    string FNYr = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.Date));

                    var S_Id = objmms.SessionMasters.Where(x => x.Name == FNYr.ToString()).FirstOrDefault().Id;
                    Grid.Master.SessionId = S_Id;
                    Grid.Master.CreatedBy = EmpId;
                    Grid.Master.CreaterName = EmpName;
                    Grid.Master.CreatedDate = System.DateTime.Now;
                    Grid.Master.DivisionId = Repos.GetUserDivision();
                    // Added
                    var gr = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == Grid.Master.GateEntry_Mid_No).ToList().FirstOrDefault();
                    Grid.Master.EmpId = gr.EmpId;
                    Grid.Master.EmpName = gr.EmpName;
                    Grid.Master.ChallanNo = gr.ChallanNo;
                    Grid.Master.ChallanDate = gr.ChallanDate;
                    Grid.Master.VehicleNo = gr.VehicleNo;
                    Grid.Master.StatusTypeId = gr.StatusTypeId;
                    Grid.Master.StatusTypeNo = gr.StatusTypeNo;
                    Grid.Master.GateEntryId = FindMaxId(Grid.Master.ProjectNo, Grid.Master.SessionId,Grid.Master.DivisionId.Value);

                    //MRn No not mrn_No_new
                    var gateentryNo = objpro.GetMRN(Grid.Master.ProjectNo, S_Id);

                    //mrn_no_new 
                    var mrn_no_new = GetMRNNoByDate(Grid.Master.ProjectNo, Grid.Master.Date);

                    Grid.Master.GateEntryNo = gateentryNo;
                    Grid.Master.MRN_No_New = mrn_no_new;

                    objmms.GateEntries.Add(Grid.Master); objmms.SaveChanges();

                    var data = objmms.GateEntry_Mid.SingleOrDefault(x => x.GateEntryNo == Grid.Master.GateEntry_Mid_No);
                    data.ChallanDate = Convert.ToDateTime(Grid.ChallanDate);
                    data.ChallanNo = Grid.ChallanNo;
                    data.VehicleNo = Grid.VehicalNo;
                    objmms.Entry(data).State = EntityState.Modified;
                    objmms.SaveChanges();
                    //
                    //   Grid.Master.GateEntryId = m.GetGateEntryManager().FindMaxId(SessionId, Grid.Master.ProjectNo);
                    decimal a = objmms.GateEntries.Where(x => x.GateEntryNo == Grid.Master.GateEntryNo && x.GateEntryId == Grid.Master.GateEntryId).FirstOrDefault().UId;
                    // m.GetGateEntryManager().Add(Grid.Master);
                    if (Grid.Child != null && a != 0)
                    {

                        foreach (var x in Grid.Child)
                        {
                            tblReceiveData obj = new tblReceiveData();
                            if (x.ReceiveQty != null && x.ReceiveQty != 0)
                            {
                                x.MUId = a;
                                x.ProjectNo = Grid.Master.ProjectNo;
                                x.ProjectName = Grid.Master.ProjectName;
                                x.Date = Grid.Master.Date;
                                x.GateEntryDate = Grid.Master.Date;
                                x.CreatedBy = EmpId;
                                x.CreatedDate = System.DateTime.Now;
                                x.SessionId = S_Id;
                                x.Amount = x.ReceiveQty * (x.Rate ?? 0);
                                x.TaxAmount = x.ReceiveQty * (x.TaxPer ?? 0);
                                x.DeliveryAmount = x.ReceiveQty * (x.DeliveryUnitCharge ?? 0);
                                x.AllAmount = x.Amount + x.TaxAmount + x.DeliveryAmount;
                                x.MRN_Receive = x.ReceiveQty;
                                x.StatusTypeNo = gr.StatusTypeNo;
                                x.Vendor = objmms.GateEntryChild_Mid.Where(l => l.UId == x.GateEntryChild_Mid_Id).ToList().FirstOrDefault().Vendor;
                                x.VendorNo = objmms.GateEntryChild_Mid.Where(l => l.UId == x.GateEntryChild_Mid_Id).ToList().FirstOrDefault().VendorNo;
                                // x.SNo = x.SNo;
                                // code for tblReceiveData
                                obj.ID = Convert.ToInt32(a);
                                obj.IDType = "Transfer";
                                obj.TypeNumber = Grid.Master.GateEntryNo;
                                obj.TypeDate = Grid.Master.Date;
                                obj.ReceiveQuantity = x.ReceiveQty ?? 0;
                                obj.IssueQuantity = 0;
                                obj.BalanceQuantity = x.ReceiveQty ?? 0;
                                obj.Rate = x.Rate;
                                obj.ProjectId = Grid.Master.ProjectNo;
                                obj.ItemId = x.ItemNo;
                                obj.CreatedDate = System.DateTime.Now;
                                obj.CreatedBy = EmpId;
                                obj.Status = Convert.ToBoolean(1);
                                obj.Isdeleted = Convert.ToBoolean(0);
                                obj.FinYear = objmsp.GetFinancialYear();


                                // End Here
                                if (x.Status == "OSP" || x.Status == "ISP")
                                {
                                    BusinessObjects.Entity.tblItemMaster item = m.GetGobalItemManager().Find(x.ItemNo);
                                    x.ItemGroupNo = item.ItemGroupID;
                                    obj.ItemGroupId = item.ItemGroupID;
                                    x.UnitNo = item.UnitID;
                                    x.InOut = "In";
                                    //x.Unit = item.UnitMasterPurchase.UnitName;
                                    MMS.DAL.GateEntry_Mid gm = objmms.GateEntry_Mid.Where(k => k.UId == x.GateEntry_Mid_Id).ToList().FirstOrDefault();
                                    gm.IsMRN_Number = Grid.Master.GateEntryNo;
                                    objmms.Entry(gm).State = EntityState.Modified;
                                    objmms.SaveChanges();

                                }

                                if (x.Status == "OSP")
                                {
                                    MMS.DAL.tblIntraStateTransferTaxableChild ch = objmms.tblIntraStateTransferTaxableChilds.Where(x1 => x1.TransId == x.StatusChildId).FirstOrDefault();
                                    //MMS.DAL.TbIndentPurchaseOrderChild_GST ch = objmms.TbIndentPurchaseOrderChild_GST.Where(x1 => x1.UId == x.StatusChildId).FirstOrDefault();
                                    ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                                    objmms.Entry(ch).State = EntityState.Modified;
                                    objmms.SaveChanges();


                                }
                                else if (x.Status == "ISP")
                                {

                                    MMS.DAL.tblInterStateTransferChild ch = objmms.tblInterStateTransferChilds.Where(x1 => x1.TransId == x.StatusChildId).FirstOrDefault();
                                    //MMS.DAL.TbIndentPurchaseOrderChild_GST ch = objmms.TbIndentPurchaseOrderChild_GST.Where(x1 => x1.UId == x.StatusChildId).FirstOrDefault();
                                    ch.Receive = (ch.Receive ?? 0) + x.ReceiveQty;
                                    objmms.Entry(ch).State = EntityState.Modified;
                                    objmms.SaveChanges();

                                }

                                // m.GetGateEntryChildManager().Add(x);
                                objmms.GateEntryChilds.Add(x);
                                objmms.tblReceiveDatas.Add(obj); objmms.SaveChanges();
                                if (x.Status == "OSP" || x.Status == "ISP")
                                {
                                    m.GettblItemCurrentStockManager().UpdateForAdd(Grid.Master.ProjectNo, x.ItemGroupNo, x.ItemNo, +(x.ReceiveQty ?? 0));
                                }

                            }
                        }





                    }
                    #endregion

                    trans.Commit();
                    return Json(new { Status = 1, TransNo = Grid.Master.MRN_No_New }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Json("2", JsonRequestBehavior.AllowGet);
                } 
            }


        }

        public JsonResult GetDetailOnMRNByGRNTRN(string GRN, string Purchase_Type)
        {
            string ProjectNo = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == GRN).FirstOrDefault().ProjectNo;
            string ProjectName = objmms.tblProjectMasters.Where(x => x.PRJID == ProjectNo).FirstOrDefault().ProjectName;
            string TransferNo = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == GRN).FirstOrDefault().StatusTypeNo;
            string  SesId  = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == GRN).FirstOrDefault().SessionId.ToString();
            string GRN_Date = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == GRN).FirstOrDefault().Date.ToString();
            //string PONo = objmms.GateEntry_Mid.Where(x =>x.GateEntryNo == GRN).FirstOrDefault().StatusTypeNo;
            //string PurReqNo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PONo).FirstOrDefault().IndentRefNo;
            int PurType = Convert.ToInt32(Purchase_Type);
            var PurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == PurType).FirstOrDefault().PurchaseType;
            string MRNData = objpro.GetMRN(ProjectNo, Convert.ToInt32(SesId));
            // string MRNNO = objpro.GetMRN_New(ProjectNo);
            string MRNNO = "";

            var MList = new { ProJname = ProjectName, MRnno = MRNNO, MRNData = MRNData, PurchaseStatus = PurchaseType + " Gate In", GRnNoval = GRN, GRNDate = GRN_Date };
            string jsonString = MList.ToJSON();

            return Json(jsonString, JsonRequestBehavior.AllowGet);

        }


        #region New MRN Entry Preview on 19/02/2020
        [HttpPost]
        public ActionResult NewMRNPreview(InsertMRN Grid, string Vendor, string Address)
        {
            MRNEntryPreview MRNPreviewObj = new MRNEntryPreview();
            MRNPreviewObj.RecItems = new List<GetGridDMR>();
            ViewBag.Vendor = Vendor;
            ViewBag.Address = Address;
            if (Grid != null)
            {
                try
                {
                    MRNPreviewObj.MRNDetail = new GetDetailOnNewDMR();
                    GateEntry_Mid gateEntryObj = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == Grid.Master.GateEntry_Mid_No).Select(s => s).FirstOrDefault();
                    MRNPreviewObj.MRNDetail.GateEntryNo = gateEntryObj.GateEntryNo;
                    MRNPreviewObj.MRNDetail.PONO = gateEntryObj.StatusTypeNo;
                    MRNPreviewObj.MRNDetail.VehicleNo = gateEntryObj.VehicleNo;
                    MRNPreviewObj.MRNDetail.BillNo = gateEntryObj.ChallanNo;
                    MRNPreviewObj.MRNDetail.BillDate = gateEntryObj.ChallanDate.Value.ToString("dd/MM/yyyy");
                    MRNPreviewObj.MRNDetail.PODate = Grid.Master.Date.Value.ToString("dd/MM/yyyy");
                    MRNPreviewObj.MRNDetail.MRNCreatedDAte = Grid.Master.Date.Value.ToString("dd/MM/yyyy");
                    MRNPreviewObj.MRNDetail.MRN = Grid.Master.MRN_No_New;
                    MRNPreviewObj.EwayBillNo = Grid.EwayBillNo;
                    MRNPreviewObj.DMRDate = Grid.DMRDate;
                    MRNPreviewObj.MRNDetail.GateEntryCreatedDate = gateEntryObj.Date.Value.ToString("dd/MM/yyyy");
                    DateTime? tDate = null; //objmms.TbIndentPurchaseOrder_GST.Where(s => s.PurchaseOrderNo == MRNPreviewObj.MRNDetail.PONO).Select(s => s.PurchaseOrderDate).FirstOrDefault();
                    bool? TCSActive = objmms.TbIndentPurchaseOrder_GST.Where(s => s.PurchaseOrderNo == gateEntryObj.StatusTypeNo).Select(s => s.TCSActive).FirstOrDefault();
                    MRNPreviewObj.MRNDetail.TCSActive = TCSActive != null ? TCSActive.Value : false;
                    if (MRNPreviewObj.MRNDetail.PONO.Contains("TRN/IS"))
                    {
                        tDate = objmms.tblInterStateTransferMasters.Where(s => s.InterTransferNumber == MRNPreviewObj.MRNDetail.PONO).Select(s => s.TransferDate).FirstOrDefault();
                    }
                    else if (MRNPreviewObj.MRNDetail.PONO.Contains("TRN/OS"))
                    {
                        tDate = objmms.tblIntraStateTransferMasters.Where(s => s.IntraTransferNumber == MRNPreviewObj.MRNDetail.PONO).Select(s => s.TransferDate).FirstOrDefault();
                    }
                    else
                    {
                        tDate = objmms.TbIndentPurchaseOrder_GST.Where(s => s.PurchaseOrderNo == MRNPreviewObj.MRNDetail.PONO).Select(s => s.PurchaseOrderDate).FirstOrDefault();
                    }
                    MRNPreviewObj.MRNDetail.PODate = tDate == null ? "" : tDate.Value.ToString("dd/MM/yyyy");

                    var data = objmms.GateEntry_Mid.Where(x => x.GateEntryNo == Grid.Master.GateEntry_Mid_No).FirstOrDefault();
                    //if (PType != 5 && PType != 6 && PType != 8)
                    //{
                    //    var PoData = objmms.TbIndentPurchaseOrder_GST.FirstOrDefault(x => x.PurchaseOrderNo == data.StatusTypeNo);
                    //    ViewBag.PINo = PoData.IndentRefNo;
                    //    var VendorDetails = objmms.tblVendorMasters.SingleOrDefault(x => x.VendorID == PoData.SupplierNo);
                    //    var Country = objmms.tblCountries.SingleOrDefault(x => x.CountryID == VendorDetails.Country).CnName;
                    //    var State = objmms.tblStates.SingleOrDefault(x => x.StateID == VendorDetails.State).StateName;
                    //    var City = objmms.tblCityLists.SingleOrDefault(x => x.CityID == VendorDetails.City).CityName;
                    //    ViewBag.VendorName = VendorDetails.Name;
                    //    ViewBag.VendorAddress = VendorDetails.Address + City + "," + State + "," + Country;
                    //    ViewBag.IsShowVendor = true;
                    //}

                    if (MRNPreviewObj.MRNDetail != null)
                    {
                        //MRNPreviewObj.MRNDetail.Vendor = (from tm in objmms.tblInterStateTransferMasters
                        //                                  join pm in objmms.tblProjectMasters
                        //                                  on tm.ProjectId equals pm.PRJID
                        //                                  where tm.InterTransferNumber == MRNPreviewObj.MRNDetail.PONO
                        //                                  select new { pm.ProjectName }).FirstOrDefault().ToString();


                    }
                    foreach (var item in Grid.Child)
                    {
                        GetGridDMR itemObj = Procedure.GetData<GetGridDMR>("usp_GetPreviewGridNewDMR", Grid.Master.GateEntry_Mid_No, "", item.ItemNo, item.ReceiveQty, item.CartageAmount == null ? 0 : item.CartageAmount).FirstOrDefault();
                        MRNPreviewObj.RecItems.Add(itemObj);
                        MRNPreviewObj.GrandTotal += itemObj.GrandTotal.Value;
                    }

                    //string empId = Session["EmpID"].ToString();
                    //string id = MRNPreviewObj.MRNDetail.PONO;
                    //string PurReq = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).FirstOrDefault().IndentRefNo;
                    //int? PurchaseTypeIdSel = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReq).FirstOrDefault().PurchasePIC_Type;


                }
                catch (Exception ex)
                {

                }
            }

            return PartialView("_MRNPreviewPartial", MRNPreviewObj);
        }

        #endregion


    }
}