using BusinessObjects.Entity;
using DataAccessLayer;
using MMS_P.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MMS.DAL;
using MMS.Models;
using MMS.ViewModels;
using System.Data.Entity;

namespace MMS.Controllers
{
    public class IndentPurchaseOrdersController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        FactoryManager m = new FactoryManager();
        ModelServices mobjModel = new ModelServices();
        Procedure objpro = new Procedure();

        int SessionId = 1;
        int SiteId = 1;
        //string EmpId = "EMP0000001";
        //string EmpName = "Admin";
        string EmpId = string.Empty;
        string EmpName = string.Empty;
        string empsId;

        List<VMTbIndentPurchaseOrderMaster> MasterList;
        public IndentPurchaseOrdersController()
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
        public ActionResult Index()
        {

            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

             empsId = Session["EmpID"].ToString();


            ViewBag.EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString(); ;
            ViewBag.fromDate = DateTime.Now.Date;
            ViewBag.toDate = ViewBag.fromDate.Date.AddDays(1);

            if (EmpId == "EMP0000001")
                ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", 0);
            else
                ViewBag.empName = new SelectList(m.GetEmployeeMasterManager().FindAll(), "EmpID", "FName", EmpId);
            return View();
        }
        public ActionResult Grid2(DateTime? fromDate, DateTime? toDate, string PrjId = null, int page = 1, string f = "C", string E = null)
        {
            if (E == "")
                E = null;
            if (PrjId == "")
                PrjId = null;
            if (toDate.HasValue) toDate = toDate.Value.AddDays(1);
            const int pageSize = 15000;

            if (!fromDate.HasValue) fromDate = DateTime.Now.Date;
            if (!toDate.HasValue) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);
            if (toDate < fromDate) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);

            int totalRows = 0;

            if (SessionId != 0)
            {
                if (E == null || E == "0" || E == "")
                {
                    m.GetTbOSTTransferMasterManager().FindByDateOrLast("Date", SessionId, PrjId, null, fromDate, toDate, page, pageSize);
                    totalRows = m.GetTbIndentPurchaseOrderMasterManager().FindForAdminCount(SessionId, PrjId, fromDate, toDate);
                    MasterList = m.GetTbIndentPurchaseOrderMasterManager().FindForAdmin(SessionId, PrjId, fromDate, toDate, page, pageSize);
                }
                else
                {
                    if (f == "C")
                    {
                        totalRows = m.GetTbIndentPurchaseOrderMasterManager().FindForCreaterCount(SessionId, E, PrjId, fromDate, toDate);
                        MasterList = m.GetTbIndentPurchaseOrderMasterManager().FindForCreater(SessionId, E, PrjId, fromDate, toDate, page, pageSize);
                    }
                    else if (f == "A")
                    {
                        totalRows = m.GetTbIndentPurchaseOrderMasterManager().FindForApprovedCount(SessionId, E, PrjId, fromDate, toDate);
                        MasterList = m.GetTbIndentPurchaseOrderMasterManager().FindForApproved(SessionId, E, PrjId, fromDate, toDate, page, pageSize);
                    }
                }



                if (MasterList != null)
                {



                    var data = new IndentPurchaserOrderPaged()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                       Master = MasterList.ToList()
                    };
                    if (Request.IsAjaxRequest())
                    {
                        return View("_GridView", data);
                    }
                }


            }

            return View("_EmptyView");
        }
        public ActionResult Grid3(string PrjId = null, int page = 1, string f = "C", string E = null)
        {
            if (E == "")
                E = null;
            if (PrjId == "")
                PrjId = null;
            const int pageSize = 15000;


            int totalRows = 0;

            if (SessionId != 0)
            {
                if (E == null || E == "0" || E == "")
                {
                    totalRows = m.GetTbIndentPurchaseOrderMasterManager().FindForAdminLastCount(SessionId, PrjId);
                    MasterList = m.GetTbIndentPurchaseOrderMasterManager().FindForAdminLast(SessionId, PrjId, page, pageSize);
                }
                else
                {
                    if (f == "C")
                    {
                        totalRows = m.GetTbIndentPurchaseOrderMasterManager().FindForCreaterLastCount(SessionId, E, PrjId);
                        MasterList = m.GetTbIndentPurchaseOrderMasterManager().FindForCreaterLast(SessionId, E, PrjId, page, pageSize);
                    }
                    else if (f == "A")
                    {
                        totalRows = m.GetTbIndentPurchaseOrderMasterManager().FindForApprovedLastCount(SessionId, E, PrjId);
                        MasterList = m.GetTbIndentPurchaseOrderMasterManager().FindForApprovedLast(SessionId, E, PrjId, page, pageSize);
                    }
                }



                if (MasterList != null)
                {



                    var data = new IndentPurchaserOrderPaged()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        Master = MasterList.ToList()
                    };
                    if (Request.IsAjaxRequest())
                    {
                        return View("_GridView", data);
                    }
                }


            }

            return View("_EmptyView");
        }

        // GET: GateEntries //
        public ActionResult Create(string PI)
        {

            ViewBag.PI_reqNo = PI.Replace("#", "/");
            string empId = Session["EmpID"].ToString();
            // string prjId = Session["ProjectssId"].ToString();
            string prjId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PI.Replace("#", "/")).FirstOrDefault().ProjectNo;
            //var a = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            //var t = a.Select(x => new SelectListItem
            //{
            //    Value = x.ProjectID.ToString(),

            //    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            //});
            //ViewBag.prjtidsssss = t;
            var a = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PI.Replace("#", "/")).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectNo.ToString(),

                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectNo.ToString()).First().ProjectName
            });
            ViewBag.prjtidsssss = t;



            ViewBag.EmpId = EmpId;
            ViewBag.AcilTinNo = "we34dd4565";
            ViewBag.prjtid = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName");

            string PRJID = m.GetProjectMasterManager().Find(SiteId).PRJID;

            //  List<BusinessObjects.Entity.tblEmployeeMaster> em = m.GetEmployeeMasterManager().FindByProject(t.FirstOrDefault().Value).ToList();
            List<BusinessObjects.Entity.tblEmployeeMaster> em = m.GetEmployeeMasterManager().FindByProject(prjId).ToList();
            // var  ll_pmc = (from a1 in objmms.tblApproval_AccountType.Where(x => x.ProjectId == PRJID && x.TypeName == "Purchase").ToList()
            //  join  b1 in em on a1.EmployeeId equals b1.EmpID select new { b1}).ToList();

            //var yue = (from b2 in objmms.tblEmployeeMasters.Where(x => x.ProjectID == prjId).ToList()
            //           join a2 in objmms.tblApproval_AccountType.Where(x1 => x1.ProjectId == prjId && x1.TypeName == "PMC")
            //           on b2.EmpID equals a2.EmployeeId
            //           select new { b2 }).ToList();

            var yue = (from b2 in objmms.tblEmployeeMasters.Where(x=>x.ProjectID.Contains(prjId)).ToList()
                       join a2 in objmms.tblApproval_AccountType.Where(x1 =>x1.TypeName == "PMC")
                       on b2.EmpID equals a2.EmployeeId
                       select new { b2 }).ToList();


            List<DAL.tblEmployeeMaster> H2 = (from Z1 in yue select Z1.b2).ToList();
           

            //var dgh = (from b in objmms.tblEmployeeMasters.Where(x => x.ProjectID == prjId).ToList()
            //          join a1 in objmms.tblApproval_AccountType.Where(x1 => x1.ProjectId == prjId && x1.TypeName == "Purchase")
            //          on b.EmpID equals a1.EmployeeId 
            //          select new  { b }).ToList();

            var dgh = (from b in objmms.tblEmployeeMasters.Where(x=>x.ProjectID.Contains(prjId)).ToList()
                       join a1 in objmms.tblApproval_AccountType.Where(x1 => x1.TypeName == "Purchase")
                       on b.EmpID equals a1.EmployeeId
                       select new { b }).ToList();

            List<DAL.tblEmployeeMaster> H = (from Z in dgh select Z.b).ToList();


            //ViewBag.Employee = new SelectList(m.GetEmployeeMasterManager().FindByProject(PRJID), "EmpID", "FName");
            // ViewBag.Employee = new SelectList(m.GetEmployeeMasterManager().FindByProject(t.FirstOrDefault().Value), "EmpID", "FName");
            //ViewBag.Employee = new SelectList(H2, "EmpID", "FName");
            ViewBag.Employee = H2.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName }).ToList();
            // ViewBag.PMCEmployee = new SelectList(H, "EmpID", "FName");
            ViewBag.PMCEmployee = H.Select(p => new SelectListItem { Value = p.EmpID, Text = p.FName + ' ' + p.LName }).ToList();
            ViewBag.VGId = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType");

            ViewBag.Date = DateTime.Now.Date;
            ViewBag.Purchase_id = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PI.Replace("#", "/")).First().UId;
            ViewBag.POLimit = objmms.tblPurchaseOrderApprovalLimitValues.Where(x => x.ProjectId == prjId).OrderByDescending(k => k.CreatedDate).First().LimitValue;
            var qid = objmms.tblMMSQuotations.Where(x => x.PurchaseReqNo == PI.Replace("#", "/")).FirstOrDefault();
            if (qid == null)
            {
                ViewBag.QuotID = "N/A";
            }
            else
            {
                ViewBag.QuotID = objmms.tblMMSQuotations.Where(x => x.PurchaseReqNo == PI.Replace("#", "/")).FirstOrDefault().TransId;
            }
           
            List<SelectListItem> ObjList = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Select Cartage Type", Value = "-1" },
                new SelectListItem { Text = "Per Item Wise", Value = "1" },
                new SelectListItem { Text = "Lumpsum", Value = "2" },
                new SelectListItem { Text = "F.O.R", Value = "3" },
            };

            ViewBag.Cart = ObjList;

          
            return View();
        }
        public ActionResult Grid(decimal IndentNo_New, int page = 1)
        {
            const int pageSize = 100;
            List<BusinessObjects.Entity.PurchaseRequisitionChild> MasterList = null;

            List<MMS.ViewModels.Rate> dsas = new List<MMS.ViewModels.Rate>();
            List<MMS.ViewModels.ItemDescriptionData> abss = new List<MMS.ViewModels.ItemDescriptionData>();
          // List<MMS.ViewModels.LastPurchaseRate> lstpur = new List<MMS.ViewModels.LastPurchaseRate>();
            int totalRows = 0;

            if (SessionId != 0)
            {
                //totalRows = m.GetProjectMasterManager().CountIndentByIndentNo(IndentNo);
                //MasterList = m.GetProjectMasterManager().FindIndentByIndentNo(IndentNo);

                totalRows = m.GetPurchaseRequisitionChildManager().FindByMasterCount(IndentNo_New);
                MasterList = m.GetPurchaseRequisitionChildManager().FindByMaster(IndentNo_New);
                MasterList = MasterList.Where(a => a.ApprovedQty != Convert.ToDecimal(0.00) && a.ApprovedQty != null).ToList();

                if (MasterList != null)
                {
                    foreach (var mas in MasterList)
                    {
                        var projectid = mas.ProjectNo;
                        var igname = mas.ItemGroupNo;
                        var itemid = mas.ItemNo; 
                        DateTime datetime = DateTime.Now.Date;
                        var existrated = objmms.ns_AddPriceCap.Where(h => h.ProjectId.Contains(projectid) && h.ItemGroupId == igname && h.ItemId == itemid && h.EffectiveDate <= datetime && h.ValidUpto >= datetime && h.EffectiveStatus == "On").DefaultIfEmpty<ns_AddPriceCap>().Select(f => new MMS.ViewModels.Rate { Rates = f.Rate, ItemId = f.ItemId }).FirstOrDefault();
                        //  dsas.Add(Convert.ToDecimal(existrated));
                        //  ViewBag.ViewAdd_Rate.add(existrated);
                       var itemds = objmms.tblItemMasters.Where(x => x.ItemID == itemid && x.ItemGroupID == igname).Select(k => new MMS.ViewModels.ItemDescriptionData { ItemDescription = k.ItemDescription , ItemId=k.ItemID}).FirstOrDefault();

                        //var dlst = objmms.tblReceiveDatas.Where(p => p.ProjectId == projectid && p.ItemGroupId == igname && p.ItemId == itemid).ToList();
                        //if (dlst.Count > 0)
                        //{
                         // var LastpuedRate = objmms.tblReceiveDatas.Where(p => p.ProjectId == projectid && p.ItemGroupId == igname && p.ItemId == itemid).OrderByDescending(k => k.CreatedDate).Select(y=> new MMS.ViewModels.LastPurchaseRate { LastPurchase_Rate=y.Rate , ItemId=mas.ItemNo ?? 0 }).FirstOrDefault();
                        //}
                        //else {
                        //    var LastpuedRate  = 0;
                        //}
                    // lstpur.Add(LastpuedRate);
                        dsas.Add(existrated);
                        abss.Add(itemds);


                    }
                    if (dsas != null)
                    {
                        ViewBag.ViewAdd_Rate = dsas;
                        // ViewBag.ViewRemaing_Qty2 = dsa[1].ToString();
                    }
                    if (abss != null)
                    {
                        ViewBag.ViewAdd_Desc = abss;
                        // ViewBag.ViewRemaing_Qty2 = dsa[1].ToString();

                    }

                    List<SelectListItem> ObjList = new List<SelectListItem>()
                   {
                new SelectListItem { Text = "Select Status", Value = "-1" },
                new SelectListItem { Text = "Yes", Value = "1" },
                new SelectListItem { Text = "No", Value = "0" },

                   };

                    ViewBag.ExistStatus = ObjList;



                    //if (lstpur != null)
                    //{
                    //    ViewBag.ViewlastPurchaseRateValue = lstpur;
                    //}
                    var data = new GridIndentRequisitions()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        Child = MasterList.ToList()
                    };
                    if (Request.IsAjaxRequest())
                    {
                        return View("_GridViewLP", data);
                    }
                }
            }
            return View("_EmptyView");
        }

        //where u call this in js
        public ActionResult AddM_AddC(IndentPurchaserOrderNew Grid)
        {

            try
            {
                var PoNo = objmms.TbIndentPurchaseOrderMasters.FirstOrDefault(x => x.PurchaseOrderNo == Grid.Master.PurchaseOrderNo);

                if (PoNo != null)
                {
                    return Json("Data is already exist.", JsonRequestBehavior.AllowGet);
                }
                else {
                    string poindentNo = Grid.Master.PurchaseOrderNo; string Purreq = string.Empty;

                    string[] IndentNo = Grid.Master.IndentRefNo.Split('/');
                    Grid.Master.IndentRefNo = Purreq = IndentNo[0].ToString() + "/" + IndentNo[1].ToString() + "/" + IndentNo[2].ToString() + "/" + IndentNo[3].ToString();
                    Grid.Master.SessionId = SessionId;
                    Grid.Master.CreatedBy = EmpId;
                    Grid.Master.Status = "Pending";
                    Grid.Master.Edited_Status = "No";
                    Grid.Master.Vendor_Group_Id = Grid.Master.Vendor_Group_Id;
                    Grid.Master.CreatedDate = System.DateTime.Now;
                    Grid.Master.PurchaseOrderId = m.GetTbIndentPurchaseOrderMasterManager().FindMaxId(SessionId, Grid.Master.ProjectNo);
                    Grid.Master.POType = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Purreq).FirstOrDefault().PurchasePIC_Type == Convert.ToInt32(1) ? "IPO" : "LP";
                    objmms.TbIndentPurchaseOrderMasters.Add(Grid.Master);

                    var CheckedPoLimit = Grid.Master.CheckedBeyondPOLimit;
                    TbIndentPurchaseOrder_Approval_For_Print tbl = new TbIndentPurchaseOrder_Approval_For_Print();
                    tbl.ProjectId = Grid.Master.ProjectNo;
                    tbl.Project_Name = objmms.tblProjectMasters.Where(b => b.PRJID == Grid.Master.ProjectNo).First().ProjectName;
                    tbl.Purchase_Order_Indent_No = Grid.Master.PurchaseOrderNo;
                    tbl.Created_Employee_Id = EmpId;
                    tbl.Employee_Name = objmms.tblEmployeeMasters.Where(h => h.EmpID == Grid.Master.SendTo).First().FName;
                    tbl.PurchaseRefNo = Grid.Master.IndentRefNo;
                    tbl.Vendor_Group_Id = Grid.Master.SupplierNo;
                    tbl.Status = "Pending";
                    tbl.Send_To = Grid.Master.SendTo;
                    tbl.CreatedDate = DateTime.Now;
                    tbl.Status_Approval = "Not Approved";
                    tbl.EditedStatus = "No";
                    tbl.Remark = Grid.Master.Remarks;
                    if (CheckedPoLimit == Convert.ToBoolean(1))
                    { tbl.SecondLevelApprv_Id = Grid.Master.SendTo; }
                    else { tbl.FirstLevelApprv_Id = Grid.Master.SendTo; }

                    objmms.TbIndentPurchaseOrder_Approval_For_Print.Add(tbl);
                   objmms.SaveChanges();

                    var FindLastId = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == Grid.Master.PurchaseOrderNo).Select(p => p.UId).First();
                    if (FindLastId != 0)
                    {
                        if (Grid.Child != null && FindLastId != 0)
                            foreach (var x in Grid.Child)
                            {
                                int sr = 0;
                                x.MUId = FindLastId;
                                x.Receive = 0;
                                x.SrNo = sr + 1;
                                objmms.TbIndentPurchaseOrderChilds.Add(x);
                         m.GetPurchaseRequisitionChildManager().Update_OrderBalance(x.IndentId ?? 0, x.Qty, true);
                            }

                        m.GetPurchaseRequisitionMasterManager().FindCompleteO(Grid.Master.IndentRefNo);

                        if (Grid.Child2 != null && FindLastId != 0)
                            foreach (var x in Grid.Child2)
                            {

                                x.CreatedBy = EmpId;
                                x.CreatedDate = System.DateTime.Now;
                                x.Edited_Status = "No";
                                x.Status = "Enable";
                                x.IsActive = "Yes";
                                objmms.tblSpecificTermsAndConditions_Child.Add(x);


                            }

                        if (Grid.Child3 != null && FindLastId != 0)
                            foreach (var x in Grid.Child3)
                            {
                                x.CreatedBy = EmpId;
                                x.CreatedDate = System.DateTime.Now;
                                x.Edited_Status = "No";
                                x.Status = "Enable";
                                x.IsActive = "Yes";
                                objmms.tblSpecific_Instructions_Child.Add(x);


                            }
                        if (Grid.Child4 != null && FindLastId != 0)
                            foreach (var x in Grid.Child4)
                            {
                                x.CreatedBy = EmpId;
                                x.CreatedDate = System.DateTime.Now;
                                x.Edited_Status = "No";
                                x.Status = "Enable";
                                x.IsActive = "Yes";
                                objmms.tblGenral_Terms_Conditions_Child.Add(x);


                            }


                        Grid.Child5.Status = "Valid";
                        Grid.Child5.CreatedDate = DateTime.Now;
                        Grid.Child5.CreatedBy = EmpId;
                        Grid.Child5.EditStatus = "No";
                        Grid.Child5.CompanyId = Session["CompanyId"].ToString();
                        objmms.TbDelivery_Details_PO.Add(Grid.Child5);
                       objmms.SaveChanges();
                        return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Server Problem.Contact to Administration.", JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            { ex.ToString(); return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet); }

        }
        public int? FindMaxSrNo(decimal? MUId)
        {
            using (var context = new Model1())
            {
                int a = objmms.TbIndentPurchaseOrderChilds.Where(i => i.MUId == MUId).Select(i => i.SrNo).DefaultIfEmpty(0).Max(i => i).Value;
                if (a == 0)
                    return 1;
                else
                    return a + 1;
                // return null;
            }
        }
        public ActionResult Edit(decimal id)
        {
            //id = 6;
            BusinessObjects.Entity.TbIndentPurchaseOrderMaster Master = m.GetTbIndentPurchaseOrderMasterManager().Find(id);
            List<BusinessObjects.Entity.TbIndentPurchaseOrderChild> ChildList = m.GetTbIndentPurchaseOrderChildManager().FindByMaster(id);
            ViewBag.Date = Master.PurchaseOrderDate;
          //  ViewBag.CreatedDate = Master.CreatedDate.ToString().Substring(0,10);
            ViewBag.EmpId = EmpId;
            ViewBag.VGId = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType");
            ViewBag.VG = m.GetVendorGroupManager().FindByVendor(Master.SupplierNo).TypeID;
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


          

            var data = new IndentPurchaserOrder()
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
        public ActionResult UpM_UpC(IndentPurchaserOrder Grid)
        {
            try
            {
                            
                Grid.Master.CreatedDate = m.GetTbIndentPurchaseOrderMasterManager().Find(Grid.Master.UId).CreatedDate; 
               
                 m.GetTbIndentPurchaseOrderMasterManager().Update(Grid.Master);
                 List<decimal> CList = m.GetTbIndentPurchaseOrderChildManager().FindAllIdByMaster(Grid.Master.UId);

                 if (Grid.Child != null)
                 {
                     foreach (var x in Grid.Child)
                     {

                         if (CList.Any(a => a == x.UId))
                         {
                             decimal? QtyPre = m.GetTbIndentPurchaseOrderChildManager().Find(x.UId).Qty;
                             decimal? QtyNow = x.Qty;
                             x.MUId = Grid.Master.UId;
                             m.GetTbIndentPurchaseOrderChildManager().Update(x);
                             if (QtyPre > QtyNow)
                             {
                                 //m.GetProjectMasterManager().UpdateIndent_OrderBalance(x.IndentId, QtyPre - QtyNow, false);
                                m.GetPurchaseRequisitionChildManager().Update_OrderBalance(x.IndentId ?? 0, x.Qty, false);
                            }
                             else if (QtyPre < QtyNow)
                             {
                                m.GetPurchaseRequisitionChildManager().Update_OrderBalance(x.IndentId ?? 0, x.Qty, true);
                                //m.GetProjectMasterManager().UpdateIndent_OrderBalance(x.IndentId, QtyNow - QtyPre,true);
                             }
                         }
                        else  if (!CList.Any(a => a == x.UId))
                         {
                             x.MUId = Grid.Master.UId;
                             decimal b = m.GetTbIndentPurchaseOrderChildManager().Add(x);
                            m.GetPurchaseRequisitionChildManager().Update_OrderBalance(x.IndentId ?? 0, x.Qty, true);
                        }
                     }
                     if (CList != null)
                         foreach (decimal x in CList)
                         {

                             if (!Grid.Child.Any(a => a.UId == x))
                             {
                                BusinessObjects.Entity.TbIndentPurchaseOrderChild y =  m.GetTbIndentPurchaseOrderChildManager().Find(x);
                                 m.GetTbIndentPurchaseOrderChildManager().Delete(x);
                              
                                 //m.GetProjectMasterManager().UpdateIndent_OrderBalance(y.IndentId, y.Qty, false);
                                m.GetPurchaseRequisitionChildManager().Update_OrderBalance(y.IndentId ?? 0, y.Qty, false);

                            }
                         }
                 }
            

                return Json("1", JsonRequestBehavior.AllowGet);
                //return Json("Update Successfully");
            }
            catch { return Json("2", JsonRequestBehavior.AllowGet); }

        }


        public ActionResult Details(decimal id)
        {
            // id = 17;
     
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            VMTbIndentPurchaseOrderMaster Master = m.GetTbIndentPurchaseOrderMasterManager().FindVM(id);
            if (Master.TbIndentPurchaseOrderMaster.PurchaseOrderDate != null)
                Master.TbIndentPurchaseOrderMaster.PurchaseOrderDate2 = Master.TbIndentPurchaseOrderMaster.PurchaseOrderDate.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            if (Master.IndentRefNoDate != null)
                Master.IndentRefNoDate2 = Master.IndentRefNoDate.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            List<VMTbIndentPurchaseOrderChild> ChildList = m.GetTbIndentPurchaseOrderChildManager().FindByMasterVM(id);
          

            var data = new IndentPurchaserOrderDetails()
            {
             
              Child=ChildList,
                Master = Master
           
            };


            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);


            //


        }
        public JsonResult GetProjectByEmpId(string E = "0")
        {
            
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

        public JsonResult GetProjectPurchaseOrderNo(string PRJID)
        {
            if (PRJID != null)
            { string Address=null;
                var E = m.GetEmployeeMasterManager().FindListByProject(PRJID);
                //var I = m.GetProjectMasterManager().FindIndentListByPrj(PRJID);
                var I = m.GetPurchaseRequisitionMasterManager().FindIndentListByPrj(PRJID);
                tblProjectMasterDetail P = m.GetProjectMasterManager().FindDetail(PRJID);
                if(P!=null)
                Address =P.Address;
                string PurchaseOrderNo = this.PurchaseOrderNo(PRJID);

                var v = new { List = E, No = PurchaseOrderNo, Address = Address, List1 = I };

                string jsonString = v.ToJSON();

                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public JsonResult GetForwardEmployeeType(string PRJID)
        {
            if (PRJID != null)
            {

                //var ll_pmc = (from a in objmms.tblApproval_AccountType.Where(x => x.ProjectId == PRJID && x.TypeName == "PMC").ToList()
                //              join
                //              b in objmms.tblEmployeeMasters.Where(p => p.ProjectID == PRJID).ToList() on a.EmployeeId equals b.EmpID
                //              select new SelectListItem
                //              {
                //                  Value = a.EmployeeId,
                //                  Text = b.FName
                //              }).Distinct().ToList();

                var ll_pmc = (from a in objmms.tblApproval_AccountType.Where(x => x.TypeName == "PMC").ToList()
                              join
                              b in objmms.tblEmployeeMasters.Where(p=>p.ProjectID.Contains(PRJID)).ToList() on a.EmployeeId equals b.EmpID
                              select new SelectListItem
                              {
                                  Value = a.EmployeeId,
                                  Text = b.FName
                              }).Distinct().ToList();



                //var ll_purchase = (from a in objmms.tblApproval_AccountType.Where(x => x.ProjectId == PRJID && x.TypeName == "Purchase").ToList()
                //                   join
                //                   b in objmms.tblEmployeeMasters.Where(p => p.ProjectID == PRJID).ToList() on a.EmployeeId equals b.EmpID
                //                   select new SelectListItem
                //                   {
                //                       Value = a.EmployeeId,
                //                       Text = b.FName
                //                   }).Distinct().ToList();


                var ll_purchase = (from a in objmms.tblApproval_AccountType.Where(x => x.TypeName == "Purchase").ToList()
                                   join
                                   b in objmms.tblEmployeeMasters.Where(p => p.ProjectID.Contains(PRJID)).ToList() on a.EmployeeId equals b.EmpID
                                   select new SelectListItem
                                   {
                                       Value = a.EmployeeId,
                                       Text = b.FName
                                   }).Distinct().ToList();

                var v = new { PMCList = ll_pmc, Pur_List = ll_purchase };
                string jsonString = v.ToJSON();

                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        private string PurchaseOrderNo(string ProjectID)
        {
            var sessioncode = m.GetSessionMasterManager().Find(SessionId).Name;
                     
            var idMax = m.GetTbIndentPurchaseOrderMasterManager().FindMaxId(SessionId, ProjectID);
            string Code = m.GetProjectMasterManager().FindCode(ProjectID);
            string ProjectCode = "";
            ProjectCode = "ACIL/" + Code + "/" + sessioncode + "/" + idMax;

            return ProjectCode;
        }

        public JsonResult GetEmployeeByPRJId(string PRJID)
        {
            string j = null;

            var E = m.GetEmployeeMasterManager().FindListByProject(PRJID);
             j = E.ToJSON();
             return Json(j, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVendorByGroups(string Vid, string Pid, string POID)
        {
            //object VendorMasters = m.GetVendorMasterManager().FindByVendorGroup(Vid, Pid);
            var VendorMasterss = objmms.TbIndentPurchaseOrderMasters.Where(k => k.ProjectNo == Pid && k.Vendor_Group_Id == Vid && k.PurchaseOrderNo==POID).ToList();
            var t = VendorMasterss.Select(x => new SelectListItem
            {
                Value = x.SupplierNo.ToString(),           
                Text = objmms.tblVendorMasters.Where(b => b.VendorID == x.SupplierNo).First().Name
            });
            object VendorMasters = t;
            string jsonString = VendorMasters.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVendorByGroup(string Vid, string Pid)
        {
            object VendorMasters = m.GetVendorMasterManager().FindByVendorGroup(Vid, Pid);           
            string jsonString = VendorMasters.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExciseBySelect(string Type)
        {
            Boolean IsTpe; string jsonString = string.Empty;
            if (Type == "1") {
                 IsTpe = true;
               var excise = objmms.tblMMSExciseDuties.Where(x => x.IsExcise == IsTpe).Select(a => new { Text = a.EdType, Value = a.EdValue }).OrderBy(k => k.Text).ToList();
                 jsonString = excise.ToJSON();
            } else { IsTpe = false;
               var  excise = objmms.tblMMSExciseDuties.Where(x => x.IsExcise == IsTpe).Select(a => new { Text = a.EdType, Value = a.EdNumericValue }).OrderBy(k => k.Text).ToList();
                 jsonString = excise.ToJSON();
            }

          return Json(jsonString,JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCartage1BySelect(string Type)
        {
            Boolean IsTpe; string jsonString = string.Empty;
            if (Type == "1")
            {
                IsTpe = true;
                var TotalCartage = objmms.tblMMSCartageTypes.Where(x => x.IsCartage ==IsTpe).Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
                jsonString = TotalCartage.ToJSON();
            }
            else {
                IsTpe = false;
                var TotalCartage = objmms.tblMMSCartageTypes.Where(x => x.IsCartage == IsTpe).Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
                jsonString = TotalCartage.ToJSON();
            }

            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInsuranceAll(string Type)
        {
            Boolean IsTpe; string jsonString = string.Empty;
            if (Type == "1")
            {
                IsTpe = true;
                var Insurance = objmms.tblMMSInsuranceMasters.Where(x => x.IsInsurance == IsTpe).Select(a => new { Text = a.InsuranceType, Value = a.InsuranceValue }).OrderBy(k => k.Text).ToList();
                jsonString = Insurance.ToJSON();
            }
            else {
                IsTpe = false;
                var Insurance = objmms.tblMMSInsuranceMasters.Where(x => x.IsInsurance == IsTpe).Select(a => new { Text = a.InsuranceType, Value = a.InsuranceValue }).OrderBy(k => k.Text).ToList();
                jsonString = Insurance.ToJSON();
            }

            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTaxAll(string Type)
        {
            Boolean IsTpe; string jsonString = string.Empty;
            if (Type == "1")
            {
                IsTpe = true;
                var tax = objmms.tblMMSTaxValues.Where(x => x.IsTax == IsTpe).Select(a => new { Text = a.TaxType, Value = a.TaxValue }).OrderBy(k => k.Text).ToList();
                jsonString = tax.ToJSON();
            }
            else {
                IsTpe = false;
                var tax = objmms.tblMMSTaxValues.Where(x => x.IsTax == IsTpe).Select(a => new { Text = a.TaxType, Value = a.TaxNumericValue }).OrderBy(k => k.Text).ToList();
                jsonString = tax.ToJSON();
            }

            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetVendorDetail(string ID)
        {
            string j = null;
            if (ID!=null)
            {
            var E = m.GetVendorMasterManager().Find(ID);
            if (E != null)
            {
                    var Country = objmms.tblCountries.Where(x => x.CountryID == E.Country).First().CnName;
                    var State = objmms.tblStates.Where(x => x.StateID == E.State && x.CountryID == E.Country && x.stStatus == true).First().StateName;
                    var City = objmms.tblCityLists.Where(x => x.CityID == E.City && x.StateID == E.State && x.ctStatus == true).First().CityName ?? "N/A";

                var v = new { Address = E.Address ?? "N/A", TinNo = E.TinNo ?? "N/A", PanNo=E.PanNo ?? "N/A", VCountry=Country ?? "N/A", VState = State ?? "N/A", VCity = City ?? "N/A", CpName = E.ContactPerson ?? "N/A", CpMobile =E.MobileNo ?? "N/A", CpPhone=E.PhoneNo ?? "N/A", CpEmail=E.Email ?? "N/A", vAdhar = E.Aadhaar_No ?? "N/A", V_PIN =E.PinCode ?? "N/A"};
                j = v.ToJSON();
            }
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        //my code start here
        //public JsonResult GetAll_Genral_T_And_C()
        //{
        //    var detaildmr = objmms.tblGenral_Terms_Conditions.OrderByDescending(n => n.CreatedDate).Select(x => new
        //    {
        //        Header_Title = x.Header_Title,
        //        GTC_Description = x.GTC_Description,
        //        Isdeleted = x.Isdeleted
        //    }).ToList();
        //    return Json(detaildmr, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult View_GTCData()
        {          
                var a = (from pd in objmms.tblGenral_Terms_Conditions.Where(k=>k.Isdeleted=="No").OrderBy(c => c.CreatedDate)
                         select new MMS.ViewModels.Genral_Terms_and_conditions
                         {
                             Id = pd.Id,
                             Header_Title = pd.Header_Title,
                             GTC_Description = pd.GTC_Description,                          
                             ProjectId=pd.ProjectId
                             
                         }).ToList();


                var totalRows = a.Count();

                var data = new Genral_Terms_and_conditions_WebGrids()
                {
                    TotalRows = totalRows,
                    PageSize = 250,
                    GTCDATAS = a.ToList()
                };


                if (data != null && data.TotalRows != 0)
                {



                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialView_GTC", data);
                    }

                    else
                    {
                        return PartialView("_PartialView_GTC", data);
                    }

                }

                else
                {
                    return View("_EmptyView");
                }              

        }

       [HttpPost]
        public ActionResult Update_GTC(tblGenral_Terms_Conditions_Child gtc_tbl)
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            empsId = Session["EmpID"].ToString();
            //here code for data Save in TbIndentPurchaseOrder_Approval_For_Print Table for admin Approval
            List<tblGenral_Terms_Conditions_Child> list = new List<tblGenral_Terms_Conditions_Child>();
            tblGenral_Terms_Conditions_Child tbl = new tblGenral_Terms_Conditions_Child();
            tbl.ProjectId = gtc_tbl.ProjectId;
            //tbl.ProjectName = objmms.tblProjectMasters.Where(b => b.PRJID == gtc_tbl.ProjectId).First().ProjectName;
            tbl.Purchase_Order_No = gtc_tbl.Purchase_Order_No;
            tbl.CreatedBy = empsId;
            tbl.Header_Title = gtc_tbl.Header_Title;
            tbl.GTC_Description = gtc_tbl.GTC_Description;          
            tbl.CreatedDate = DateTime.Now;

            tbl.IsActive = "Yes";
            tbl.Edited_Status = "Yes";
            tbl.Status = "Enable";
            tbl.GTC_Master_ID = gtc_tbl.GTC_Master_ID;
            list.Add(tbl);
            objmms.tblGenral_Terms_Conditions_Child.AddRange(list);
            objmms.SaveChanges();

           

            //if (ModelState.IsValid)
            //{
            //    tblGenral_Terms_Conditions emp = objmms.tblGenral_Terms_Conditions.Single(em => em.Id == gtc_tbl.Id);
            //    emp.Header_Title = gtc_tbl.Header_Title;
            //    emp.GTC_Description = gtc_tbl.GTC_Description;
            //    emp.CompanyId = "COMP000001";
            //    objmms.tblGenral_Terms_Conditions.Add(gtc_tbl);
            //    //objmms.Entry(emp).State = EntityState.Modified;
            //    objmms.SaveChanges();                           
            //}
            return RedirectToAction("Create");
        }


        [HttpPost]
        public ActionResult Delete_GTC(tblGenral_Terms_Conditions_Child gtc_tbl)
        {           
            if (ModelState.IsValid)
            {
                tblGenral_Terms_Conditions_Child emp = objmms.tblGenral_Terms_Conditions_Child.Single(em => em.Id == gtc_tbl.Id);
                emp.IsActive = "No";
                emp.Status = "Disabled";
                //objmms.tblGenral_Terms_Conditions.Add(gtc_tbl);
                objmms.Entry(emp).State = EntityState.Modified;
                objmms.SaveChanges();
            }
            return RedirectToAction("Create");
        }


        public ActionResult View_SIData()
        {
            var a = (from pd in objmms.tblSpecific_Instructions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreatedDate)
                     select new Specific_Instruction_Conditions
                     {
                         Id = pd.Id,
                         Header_Title = pd.Header_Title,
                         SI_Description = pd.SI_Description,
                         ProjectId = pd.ProjectId

                     }).ToList();


            var totalRows = a.Count();

            var data = new Specific_Instruction_Conditions_WebGrids()
            {
                TotalRows = totalRows,
                PageSize = 250,
                SI_DATAS = a.ToList()
            };


            if (data != null && data.TotalRows != 0)
            {



                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PartialView_SI", data);
                }

                else
                {
                    return PartialView("_PartialView_SI", data);
                }

            }

            else
            {
                return View("_EmptyView");
            }

        }

        [HttpPost]
        public ActionResult Update_SI(tblSpecific_Instructions_Child gtc_tbl)
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            empsId = Session["EmpID"].ToString();
            //here code for data Save in TbIndentPurchaseOrder_Approval_For_Print Table for admin Approval
            List<tblSpecific_Instructions_Child> list = new List<tblSpecific_Instructions_Child>();
            tblSpecific_Instructions_Child tbl = new tblSpecific_Instructions_Child();
            tbl.ProjectId = gtc_tbl.ProjectId;
            //tbl.ProjectName = objmms.tblProjectMasters.Where(b => b.PRJID == gtc_tbl.ProjectId).First().ProjectName;
            tbl.Purchase_Order_No = gtc_tbl.Purchase_Order_No;
            tbl.CreatedBy = empsId;
            tbl.Header_Title = gtc_tbl.Header_Title;
            tbl.SI_Description = gtc_tbl.SI_Description;
            tbl.CreatedDate = DateTime.Now;
            tbl.IsActive = "Yes";
            tbl.Edited_Status = "Yes";
            tbl.Status = "Enable";
            tbl.SI_Master_ID = gtc_tbl.SI_Master_ID;
            list.Add(tbl);
            objmms.tblSpecific_Instructions_Child.AddRange(list);
            objmms.SaveChanges();



            //if (ModelState.IsValid)
            //{
            //    tblGenral_Terms_Conditions emp = objmms.tblGenral_Terms_Conditions.Single(em => em.Id == gtc_tbl.Id);
            //    emp.Header_Title = gtc_tbl.Header_Title;
            //    emp.GTC_Description = gtc_tbl.GTC_Description;
            //    emp.CompanyId = "COMP000001";
            //    objmms.tblGenral_Terms_Conditions.Add(gtc_tbl);
            //    //objmms.Entry(emp).State = EntityState.Modified;
            //    objmms.SaveChanges();                           
            //}
            return RedirectToAction("Create");
        }

        [HttpPost]
        public ActionResult Delete_SI(tblSpecific_Instructions_Child gtc_tbl)
        {
            if (ModelState.IsValid)
            {
                tblSpecific_Instructions_Child emp = objmms.tblSpecific_Instructions_Child.Single(em => em.Id == gtc_tbl.Id);
                emp.IsActive = "No";
                emp.Status = "Disabled";
               // objmms.tblSpecific_Instructions.Add(gtc_tbl);
                objmms.Entry(emp).State = EntityState.Modified;
                objmms.SaveChanges();               
            }
            return PartialView("_PartialView_SI");
        }

        //Specific Terms and condition code here
        public ActionResult View_STCData()
        {
            var a = (from pd in objmms.tblSpecificTermsAndConditions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreateDate)
                     select new Specific_Terms_And_Conditions
                     {
                         Id = pd.Id,
                         Statement_Header = pd.Statement_Header,
                         STC_Description = pd.STC_Description,
                         ProjectId = pd.ProjectId

                     }).ToList();


            var totalRows = a.Count();

            var data = new Specific_Terms_And_Conditions_WebGrids()
            {
                TotalRows = totalRows,
                PageSize = 250,
                STC_DATAS = a.ToList()
            };


            if (data != null && data.TotalRows != 0)
            {



                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PartialView_STC", data);
                }

                else
                {
                    return PartialView("_PartialView_STC", data);
                }

            }

            else
            {
                return View("_EmptyView");
            }

        }

        [HttpPost]
        public ActionResult Update_STC(tblSpecificTermsAndConditions_Child gtc_tbl)
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            empsId = Session["EmpID"].ToString();
            //here code for data Save in TbIndentPurchaseOrder_Approval_For_Print Table for admin Approval
            List<tblSpecificTermsAndConditions_Child> list = new List<tblSpecificTermsAndConditions_Child>();
            tblSpecificTermsAndConditions_Child tbl = new tblSpecificTermsAndConditions_Child();
            tbl.ProjectId = gtc_tbl.ProjectId;
            //tbl.ProjectName = objmms.tblProjectMasters.Where(b => b.PRJID == gtc_tbl.ProjectId).First().ProjectName;
            tbl.Purchase_Order_No = gtc_tbl.Purchase_Order_No;
            tbl.CreatedBy = empsId;
            tbl.HeaderTitle = gtc_tbl.HeaderTitle;
            tbl.STC_Description = gtc_tbl.STC_Description;
            tbl.CreatedDate = DateTime.Now;           
            tbl.IsActive = "Yes";
            tbl.Edited_Status = "Yes";
            tbl.Status = "Enable";
            tbl.STC_Master_ID = gtc_tbl.STC_Master_ID;
            list.Add(tbl);
            objmms.tblSpecificTermsAndConditions_Child.AddRange(list);
            objmms.SaveChanges();



            //if (ModelState.IsValid)
            //{
            //    tblGenral_Terms_Conditions emp = objmms.tblGenral_Terms_Conditions.Single(em => em.Id == gtc_tbl.Id);
            //    emp.Header_Title = gtc_tbl.Header_Title;
            //    emp.GTC_Description = gtc_tbl.GTC_Description;
            //    emp.CompanyId = "COMP000001";
            //    objmms.tblGenral_Terms_Conditions.Add(gtc_tbl);
            //    //objmms.Entry(emp).State = EntityState.Modified;
            //    objmms.SaveChanges();                           
            //}
            return RedirectToAction("Create");
        }

        [HttpPost]
        public ActionResult Delete_STC(tblSpecificTermsAndConditions_Child gtc_tbl)
        {
            if (ModelState.IsValid)
            {
                tblSpecificTermsAndConditions_Child emp = objmms.tblSpecificTermsAndConditions_Child.Single(em => em.Id == gtc_tbl.Id);
                emp.IsActive = "No";
                emp.Status = "Disabled";
                // objmms.tblSpecific_Instructions.Add(gtc_tbl);
                objmms.Entry(emp).State = EntityState.Modified;
                objmms.SaveChanges();
            }
            return PartialView("_PartialView_STC");
        }


        public JsonResult GetProjectAdress(string id)
        {

            var itemssLists = this.Getitemdetailss(Convert.ToString(id));
            return Json(itemssLists, JsonRequestBehavior.AllowGet);
        }

        public string Getitemdetailss(string ItemID)
        {

            string Partno = "";

            var data = objmms.tblProjectParticularsDetailAs.Where(m => m.PRJID == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.PRJID.Length > 0)
            {
                Partno = data.Location;
            }
            return Partno;
        }



        //for employee info

        public JsonResult GetEmployeeName(string id)
        {

            var itemssLists = this.GetEmployeedetailss(Convert.ToString(id));
            return Json(itemssLists, JsonRequestBehavior.AllowGet);
        }

        public string GetEmployeedetailss(string ItemID)
        {

            string nameno = "";

            var data = objmms.tblEmployeeMasters.Where(m => m.ProjectID == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.ProjectID.Length > 0)
            {
                nameno = data.FName;
            }
            return nameno;
        }

        public JsonResult GetEmployeeContact(string id)
        {

            var itemssLists = this.GetEmployeemobile(Convert.ToString(id));
            return Json(itemssLists, JsonRequestBehavior.AllowGet);
        }

        public string GetEmployeemobile(string ItemID)
        {

            string mobileno = "";

            var data = objmms.tblEmployeeMasters.Where(m => m.ProjectID == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.ProjectID.Length > 0)
            {
                mobileno = data.MobileNo;
            }
            return mobileno;
        }



        private string PurchaseOrderNos(string ProjectID)
        {
            var sessioncode = m.GetSessionMasterManager().Find(SessionId).Name;

            var idMax = m.GetTbIndentPurchaseOrderMasterManager().FindMaxId(SessionId, ProjectID);
            string Code = m.GetProjectMasterManager().FindCode(ProjectID);
            string ProjectCode = "";
            ProjectCode = "ACIL/" + Code + "/" + sessioncode + "/" + idMax;

            return ProjectCode;
        }

        public JsonResult GetProjectPurchaseOrderNo_For_Edit(string PRJID)
        {
            if (PRJID != null)
            {
                string Address = null;
                var E = m.GetEmployeeMasterManager().FindListByProject(PRJID);
                //var I = m.GetProjectMasterManager().FindIndentListByPrj(PRJID);
                // var I = m.GetPurchaseRequisitionMasterManager().FindIndentListByPrj(PRJID);
                var I = objmms.TbIndentPurchaseOrderMasters.Where(a => a.ProjectNo == PRJID).Select(a=>a.IndentRefNo).ToList();
                tblProjectMasterDetail P = m.GetProjectMasterManager().FindDetail(PRJID);
                if (P != null)
                    Address = P.Address;

                                          
                ////id = 6;
                //BusinessObjects.Entity.TbIndentPurchaseOrderMaster Master = m.GetTbIndentPurchaseOrderMasterManager().Find(ids);
                string PurchaseOrderNo = this.PurchaseOrderNos(PRJID);

                var v = new { List = E, No = PurchaseOrderNo, Address = Address, List1 = I };

                string jsonString = v.ToJSON();

                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public ActionResult Not_Approved_PO_Edit(string id)
        {          
            string empId = Session["EmpID"].ToString();
            var a = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectID.ToString(),
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            });
            ViewBag.prjtid = t;
           
            //main code start here
            id = id?.Replace("#", "/");
            Session["POId"] = id.ToString();
            var indent_no = objmms.TbIndentPurchaseOrderMasters.Where(s => s.PurchaseOrderNo == id).ToList();
            var mm = indent_no.Select(g => new SelectListItem
            {
                Value=g.IndentRefNo,
                Text=g.IndentRefNo
            });
            ViewBag.Indent_refe_Num = mm;
            var Indent_ref = objmms.TbIndentPurchaseOrderMasters.Where(s => s.PurchaseOrderNo == id).FirstOrDefault().IndentRefNo;
            //remaing Approved Quantity in lable
            //var App_Qty = objmms.PurchaseRequisitionChilds.Where(h => h.PurRequisitionNo == Indent_ref).ToList();
            //List<decimal> dsa = new List<decimal>();
            //foreach (var newqty in App_Qty)
            //{
            //    var App_Qtyfinal = newqty.ApprovedQty;
            //    var order_qty = newqty.OrderedQty;
            //    var Remaing_Qty = App_Qtyfinal - order_qty;
            //    dsa.Add(Convert.ToDecimal(Remaing_Qty));
            //}
            //if (dsa != null)
            //{
            //    ViewBag.ViewRemaing_Qty = dsa;
            //    // ViewBag.ViewRemaing_Qty2 = dsa[1].ToString();
            //}
            
            //for delivery details
            var prjt = objmms.TbDelivery_Details_PO.Where(s => s.Purchase_order_No == id).ToList();
            var pj = prjt.Select(g => new SelectListItem
            {
                Value = g.ProjectId,
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == g.ProjectId).First().ProjectName
            });
            ViewBag.Project_Dtls2 = pj;
            var idd = (from i in objmms.TbIndentPurchaseOrderMasters
                       where i.PurchaseOrderNo == id
                       select i.UId).SingleOrDefault();
            decimal ids = Convert.ToDecimal(idd);
            BusinessObjects.Entity.TbIndentPurchaseOrderMaster Master = m.GetTbIndentPurchaseOrderMasterManager().Find(ids);
            List<BusinessObjects.Entity.TbIndentPurchaseOrderChild> ChildList = m.GetTbIndentPurchaseOrderChildManager().FindByMaster(ids);
            ViewBag.VGId = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType");
            //ViewBag.VG = m.GetVendorGroupManager().FindByVendor(Master.SupplierNo).TypeID;          
            //ViewBag.VG = objmms.tblVendorTypeMasters.Where(a=>a.TypeID==Master.SupplierNo);
            var GetVen_id = objmms.TbIndentPurchaseOrderMasters.Where(k=>k.PurchaseOrderNo == id).FirstOrDefault().Vendor_Group_Id;
            ViewBag.VG = new SelectList(objmms.tblRegistrationTypes, "TypeID", "RegistrationType", GetVen_id).OrderBy(k => k.Text);
            ViewBag.VNAME = new SelectList(objmms.tblVendorMasters, "VendorID", "Name", Master.SupplierNo).OrderBy(k => k.Text);
            //var aaa = objmms.TbIndentPurchaseOrderMasters.Where(k => k.PurchaseOrderNo == id).ToList();
            //ViewBag.VG = aaa.Select(x => new SelectListItem
            //{
            //    Value = x.Vendor_Group_Id.ToString(),
            //    Text = objmms.tblRegistrationTypes.Where(b => b.TypeID == x.Vendor_Group_Id).First().RegistrationType
            //});
            var units = "";
            if (id != null)
            {
              
                var querys = (from mas in objmms.TbIndentPurchaseOrderMasters
                              join chld in objmms.TbIndentPurchaseOrderChilds
                             // Both anonymous types should have exact same number of properties having same name and datatype
                             // on new { a = (int?)mas.UId } equals new { a = chld.MUId }
                             on mas.UId equals chld.MUId
                              where mas.PurchaseOrderNo == id
                              select new Purchase_Order_Slip__For_Edit
                              {
                                  SupplierNo = mas.SupplierNo,
                                  Name = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Name,
                                  Address = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Address,
                                  AcilTinNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().TinNo,
                                  MobileNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().MobileNo,
                                  ContactPerson = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().ContactPerson,
                                  Delivery_Schedule=objmms.TbDelivery_Details_PO.Where(k=>k.Purchase_order_No == id).FirstOrDefault().Delivery_Schedule,
                                  UId=mas.UId,
                                  MUId=chld.MUId,
                                  UId_chld=chld.UId,

                                  SurCharge = mas.SurCharge,
                                  Cartage = mas.Cartage,
                                  Reference = mas.Reference,
                                  Remark = chld.Remark,
                                  IndentRefNo = mas.IndentRefNo,
                                  PurchaseOrderNo = mas.PurchaseOrderNo,
                                  CreatedDate = mas.CreatedDate,
                                  Vat = chld.Vat,
                                  SubTotal = chld.SubTotal,
                                  GrandTotal = mas.GrandTotal,
                                  Total = mas.Total,
                                  ProjectNo = mas.ProjectNo,
                                  ProjectName = objmms.tblProjectMasters.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().ProjectName,
                                  Location = objmms.tblProjectParticularsDetailAs.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().Location,
                                  //for remaing qty from Purchase requisition table
                                  ApprovedQty= objmms.PurchaseRequisitionChilds.Where(d => d.PurRequisitionNo == mas.IndentRefNo && d.ItemNo == chld.ItemNo).FirstOrDefault().ApprovedQty,
                                  OrderedQty= objmms.PurchaseRequisitionChilds.Where(d => d.PurRequisitionNo == mas.IndentRefNo && d.ItemNo == chld.ItemNo).FirstOrDefault().OrderedQty,


                                  PurchaseOrderDate = mas.PurchaseOrderDate,
                                  ItemNo = chld.ItemNo,
                                  ItemName = objmms.tblItemMasters.Where(j => j.ItemID == chld.ItemNo).FirstOrDefault().ItemName,
                                  UnitID = objmms.tblItemMasters.Where(l => l.ItemID == chld.ItemNo).FirstOrDefault().UnitID,
                                  UnitName = objmms.tblUnitMasters.Where(l => l.UnitID == objmms.tblItemMasters.Where(ll => ll.ItemID == chld.ItemNo).FirstOrDefault().UnitID).FirstOrDefault().UnitName,
                                  Amount = chld.Amount,
                                  Discount = chld.Discount,
                                  Qty = chld.Qty,
                                  Rate = chld.Rate
                              }).ToList();
                //var projtn=querys.Select(k=> k.ProjectNo == )
                var data = querys;
                PrintData_PO_Edit dataObj = new PrintData_PO_Edit();
                dataObj.HeaderData = data.Select(x => new HeaderData_PO_Edit {UId=x.UId, Delivery_Schedule=x.Delivery_Schedule,SupplierNo=x.SupplierNo,PurchaseOrderNo = x.PurchaseOrderNo, Total = x.Total, Location = x.Location, ContactPerson = x.ContactPerson, MobileNo = x.MobileNo, Reference = x.Reference, SurCharge = x.SurCharge, Cartage = x.Cartage, GrandTotal = x.GrandTotal, SubTotal = x.SubTotal, PurchaseOrderDate = x.PurchaseOrderDate, AcilTinNo = x.AcilTinNo, Address = x.Address, Name = x.Name, ProjectName = x.ProjectName, IndentRefNo = x.IndentRefNo }).FirstOrDefault();

                dataObj.ItemData = data.Select(x => new ItemData_PO_Edit {ApprovedQty=x.ApprovedQty, OrderedQty=x.OrderedQty, UId=x.UId_chld, MUId=x.MUId, ItemNo=x.ItemNo, ItemName = x.ItemName, Remark = x.Remark, UnitID=x.UnitID, UnitName = x.UnitName, Rate = x.Rate, Discount = x.Discount, SubTotal = x.SubTotal, Vat = x.Vat, GrandTotal = x.GrandTotal, Qty = x.Qty, Amount = x.Amount }).ToList();

                ViewBag.last_Qty = dataObj.ItemData.ToList();

                //for genral terms and conditions
                var GTSquerys = (from gts in objmms.tblGenral_Terms_Conditions_Child
                                 where gts.Purchase_Order_No == id
                                 select new All_Terms_And_Condition
                                 {
                                     Id=gts.Id,
                                     Header_Title=gts.Header_Title,
                                     GTC_Description=gts.GTC_Description,
                                     ProjectId =gts.ProjectId,
                                     Purchase_Order_No=gts.Purchase_Order_No,
                                     IsActive=gts.IsActive,
                                     Status=gts.Status
                                 }).ToList();
                var gts_all_row = GTSquerys;

                dataObj.GTC_Edit_Data = gts_all_row.Select(g => new Genral_term_conditon_Chld_For_Edit {Id=g.Id, Header_Title=g.Header_Title, GTC_Description=g.GTC_Description,ProjectId=g.ProjectId,Purchase_Order_No=g.Purchase_Order_No}).ToList();
                //for Specific Instructions
                var SIquerys = (from gts in objmms.tblSpecific_Instructions_Child
                                 where gts.Purchase_Order_No == id
                                 select new All_Terms_And_Condition
                                 {
                                     Id = gts.Id,
                                     Header_Title = gts.Header_Title,
                                     GTC_Description = gts.SI_Description,
                                     ProjectId = gts.ProjectId,
                                     Purchase_Order_No = gts.Purchase_Order_No,
                                     IsActive = gts.IsActive,
                                     Status = gts.Status
                                 }).ToList();
                var si_all_row = SIquerys;

                dataObj.SI_Edit_Data = si_all_row.Select(g => new Specific_Instructions_TC_Chld_For_Edit { Id = g.Id, Header_Title = g.Header_Title, SI_Description = g.GTC_Description, ProjectId = g.ProjectId, Purchase_Order_No = g.Purchase_Order_No }).ToList();

                //for Specific terms and conditions
                var STCquerys = (from gts in objmms.tblSpecificTermsAndConditions_Child
                                where gts.Purchase_Order_No == id
                                select new All_Terms_And_Condition
                                {
                                    Id = gts.Id,
                                    Header_Title = gts.HeaderTitle,
                                    GTC_Description = gts.STC_Description,
                                    ProjectId = gts.ProjectId,
                                    Purchase_Order_No = gts.Purchase_Order_No,
                                    IsActive = gts.IsActive,
                                    Status = gts.Status
                                }).ToList();
                var stc_all_row = STCquerys;

                dataObj.STC_Edit_Data = stc_all_row.Select(g => new Specific_Termss_And_conditions_Chld_For_Edit { Id = g.Id, HeaderTitle = g.HeaderTitle, STC_Description = g.GTC_Description, ProjectId = g.ProjectId, Purchase_Order_No = g.Purchase_Order_No }).ToList();







                return View(dataObj);

            }
            else {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
       }

        //get delivery details for edit
        public JsonResult GetDeliveryAdress(string id)
        {

            var itemssLists = this.Getitemdetailss(Convert.ToString(id));
            return Json(itemssLists, JsonRequestBehavior.AllowGet);
        }

        public string GetPrjtdetailss(string ItemID)
        {

            string Del_Addres = "";           

            var data = objmms.TbDelivery_Details_PO.Where(m => m.ProjectId == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.ProjectId.Length > 0)
            {
                Del_Addres = data.Delivery_Address;
            }
            return Del_Addres;
        }
        public JsonResult GetEmployeeNames(string id)
        {

            var itemssLists = this.GetEmployeedetailsss(Convert.ToString(id));
            return Json(itemssLists, JsonRequestBehavior.AllowGet);
        }

        public string GetEmployeedetailsss(string ItemID)
        {

            string namename = "";

            var data = objmms.TbDelivery_Details_PO.Where(m => m.ProjectId == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.ProjectId.Length > 0)
            {
                namename = data.Contact_Person_Name;
            }
            return namename;
        }


        public JsonResult GetEmployeeContacts(string id)
        {

            var itemssLists = this.GetEmployeemobiles(Convert.ToString(id));
            return Json(itemssLists, JsonRequestBehavior.AllowGet);
        }

        public string GetEmployeemobiles(string ItemID)
        {

            string mobileno = "";

            var data = objmms.TbDelivery_Details_PO.Where(m => m.ProjectId == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.ProjectId.Length > 0)
            {
                mobileno = data.Contact_Person_Mobile;
            }
            return mobileno;
        }



        public ActionResult Update_PO(IndentPurchaserOrderNew Grid)
        {
            //IndentPurchaserOrder Grid = new IndentPurchaserOrder();
            //Grid.Child = GridNew.Child;
            try
            {
                // u know the meaning of this Request.QueryString[Grid.Master.PurchaseOrderNo]
                string poindentNo = Grid.Master.PurchaseOrderNo;// i want to send this value to another controller.unit u will not call that controller with action how u can pass data in query string idiot
                                                                //Session["poindentNo"] = Grid.Master.PurchaseOrderNo;
                                                                // Session["poindentPuchase_Id"] = Grid.Master.PurchaseOrderId;
                string[] IndentNo = Grid.Master.IndentRefNo.Split('/');
                Grid.Master.IndentRefNo = IndentNo[0].ToString() + "/" + IndentNo[1].ToString() + "/" + IndentNo[2].ToString() + "/" + IndentNo[3].ToString();
                Grid.Master.SessionId = SessionId;
                Grid.Master.CreatedBy = EmpId;
                Grid.Master.Status = "Pending";
                Grid.Master.Edited_Status = "Yes";
                Grid.Master.CreatedDate = System.DateTime.Now;
                Grid.Master.PurchaseOrderId = m.GetTbIndentPurchaseOrderMasterManager().FindMaxId(SessionId, Grid.Master.ProjectNo);
                //decimal a = m.GetTbIndentPurchaseOrderMasterManager().Add(Grid.Master);
                //Grid.Master = objmms.TbIndentPurchaseOrderMasters.Add(Grid.Master);13 sep comment

                //objmms.SaveChanges();
                
                decimal a = Grid.Master.UId;

                if (Grid.Master != null && a != 0)
                {
                    var indent_Mas = objmms.TbIndentPurchaseOrderMasters.SingleOrDefault(f=>f.UId == a && f.PurchaseOrderNo ==poindentNo);
                    indent_Mas.Total = Grid.Master.Total;
                    indent_Mas.GrandTotal = Grid.Master.GrandTotal;
                    indent_Mas.Cartage = Grid.Master.Cartage;
                    indent_Mas.SurCharge = Grid.Master.SurCharge;
                    indent_Mas.Reference = Grid.Master.Reference;
                    indent_Mas.SendFrom = EmpId;
                    indent_Mas.SendTo = Grid.Master.SendTo;
                    indent_Mas.SupplierNo = Grid.Master.SupplierNo;
                }


                if (Grid.Child != null && a != 0)
                    foreach (var x in Grid.Child)
                    {
                        int sr = 0;
                        //x.MUId = a;
                        x.Receive = 0;
                        x.SrNo = sr + 1;                       
                        var nfg = objmms.TbIndentPurchaseOrderChilds.SingleOrDefault(aa => aa.UId == x.UId && aa.MUId == x.MUId);
                        nfg.Rate = x.Rate;
                        nfg.Qty = x.Qty;
                        nfg.Amount = x.Amount;
                        nfg.Discount = x.Discount;
                        nfg.Vat = x.Vat;
                        nfg.SubTotal = x.SubTotal;
                        nfg.Remark = x.Remark;
                        //objmms.Entry(nfg).State = EntityState.Modified;
                        // objmms.SaveChanges();
                        // m.GetPurchaseRequisitionChildManager().Update_OrderBalance(x.IndentId ?? 0, x.Qty, true);

                        // for update in purchase Remaining quantity here
                        var Appr_qty = objmms.PurchaseRequisitionChilds.SingleOrDefault(d => d.PurRequisitionNo == Grid.Master.IndentRefNo && d.ItemNo == x.ItemNo).ApprovedQty;
                        var remain_qty = objmms.PurchaseRequisitionChilds.SingleOrDefault(d=>d.PurRequisitionNo == Grid.Master.IndentRefNo && d.ItemNo == x.ItemNo).OrderedQty;
                        if (x.Qty >= x.LastQty)
                        {
                            var rem_qty = Appr_qty - remain_qty;
                            var rty = x.Qty - x.LastQty;
                            if (rty > 0)
                            {
                                DAL.PurchaseRequisitionChild bg = objmms.PurchaseRequisitionChilds.Where(l => l.PurRequisitionNo == Grid.Master.IndentRefNo && l.ItemNo == x.ItemNo).First();
                                var Fnl_qtys=bg.OrderedQty+ Convert.ToDecimal(rty);
                                bg.OrderedQty = Convert.ToDecimal(Fnl_qtys);
                                objmms.Entry(bg).State = EntityState.Modified;
                            }
                            else
                            {
                                
                            }
                        }
                        else
                        {
                            var rty = x.Qty - x.LastQty;
                            DAL.PurchaseRequisitionChild bg2 = objmms.PurchaseRequisitionChilds.Where(l => l.PurRequisitionNo == Grid.Master.IndentRefNo && l.ItemNo == x.ItemNo).First();
                            var Fnl_qtys2 = bg2.OrderedQty + Convert.ToDecimal(rty);
                            bg2.OrderedQty = Convert.ToDecimal(Fnl_qtys2);
                            objmms.Entry(bg2).State = EntityState.Modified;
                        }
                    }
                m.GetPurchaseRequisitionMasterManager().FindCompleteO(Grid.Master.IndentRefNo);

                if (Grid.Child2 != null && a != 0)
                    foreach (var x in Grid.Child2)
                    {
                       var stcD = objmms.tblSpecificTermsAndConditions_Child.SingleOrDefault(v=> v.Id ==x.Id);
                        stcD.ModifiedBy = EmpId;
                        stcD.ModifiedDate = System.DateTime.Now;
                        stcD.Edited_Status = "Yes";
                        stcD.STC_Description = x.STC_Description;                    
                        //objmms.tblSpecificTermsAndConditions_Child.Add(x);
                        // objmms.SaveChanges();

                    }

                if (Grid.Child3 != null && a != 0)
                    foreach (var x in Grid.Child3)
                    {
                        var siD = objmms.tblSpecific_Instructions_Child.SingleOrDefault(v => v.Id == x.Id);
                        siD.ModifiedBy = EmpId;
                        siD.ModifiedDate = System.DateTime.Now;
                        siD.Edited_Status = "Yes";
                        siD.SI_Description = x.SI_Description;                                          
                        //objmms.tblSpecific_Instructions_Child.Add(x);
                        //objmms.SaveChanges();

                    }
                if (Grid.Child4 != null && a != 0)
                    foreach (var x in Grid.Child4)
                    {
                        var gtcD = objmms.tblGenral_Terms_Conditions_Child.SingleOrDefault(v => v.Id == x.Id);
                        gtcD.ModifiedBy = EmpId;
                        gtcD.ModifiedDate = System.DateTime.Now;
                        gtcD.Edited_Status = "Yes";
                        gtcD.GTC_Description = x.GTC_Description;  
                    }

                if (Grid.Child5 != null && a != 0)
              {
                var delivery_Sch = objmms.TbDelivery_Details_PO.SingleOrDefault(n => n.Purchase_order_No == Grid.Child5.Purchase_order_No);
                    delivery_Sch.ModifiedDate = DateTime.Now;
                    delivery_Sch.ModifiedBy = EmpId;
                    delivery_Sch.EditStatus = "Yes";
                    delivery_Sch.Delivery_Schedule = Grid.Child5.Delivery_Schedule;
                  
                }

                //here code for data Save in TbIndentPurchaseOrder_Approval_For_Print Table for admin Approval
                if (poindentNo != null)
                {
                    var IOAFP = objmms.TbIndentPurchaseOrder_Approval_For_Print.SingleOrDefault(y => y.Purchase_Order_Indent_No == poindentNo && y.PurchaseRefNo == Grid.Master.IndentRefNo);
                    IOAFP.Status= "Pending";
                    IOAFP.Status_Approval= "Not Approved";
                    IOAFP.EditedStatus = "Yes";
                    IOAFP.ModifiedDate = DateTime.Now;
                }               

                objmms.SaveChanges();

                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
                // return Json(new { Status = "1", IndentNo = poindentNo.Replace("/", "@@") }, JsonRequestBehavior.AllowGet);
                //return Json("Update Successfully");
            }
            catch (Exception ex)
            {
                return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult ViewPriceCaps(string id)
        {
            var data = mobjModel.GetPriceCapDetailByItem(id);
            if (data != null)
            {
                if (Request.IsAjaxRequest())
                {
                    PricecapModel2 empObj = new PricecapModel2();
                    ViewBag.Createdby = new SelectList(objmms.tblEmployeeMasters, "EmpID", "FName", data.CreatedBy);
                    empObj.Rate = data.Rate;
                    ViewBag.IsUpdate = true;
                    return PartialView("_Partial_View_For_Rate_Status", empObj);
                }
                else
                    return View(data);
            }
            else
            {
                TempData["msgshow"] = "Rate is nor exist in this record";
                TempData.Keep("msgshow");
                return View(data);
            }
        }
        [HttpPost]
        public ActionResult SendPriceCapDetails(SendRateNotification apc)
        {
            ns_RateNotification nsapc = new ns_RateNotification();
            nsapc.CreatedDate = System.DateTime.Now.ToString();
            nsapc.CreatedBy = Session["EmpID"].ToString();            

            nsapc.SendBy = Session["EmpID"].ToString();
            nsapc.ForwordTo = apc.ForwordTo;
            nsapc.Rate = apc.Rate;
            nsapc.Status = "Pending";
            nsapc.Remark = apc.Remark;           
           
                var i = objmms.ns_RateNotification.Add(nsapc);
                if (i != null)
                {
                    if (Request.IsAjaxRequest())
                    {
                        objmms.SaveChanges();
                        return Json("1", JsonRequestBehavior.AllowGet);
                        //return RedirectToAction("Create");
                    }
                    return RedirectToAction("Create");

                }
                else {
                }         
            return Json("Send Successfully");

        }


        public JsonResult GetEmpname(string id)
        {
            List<SelectListItem> groups = new List<SelectListItem>();
            var groupList = this.Getprojectname(Convert.ToString(id));
            var employeeData = groupList.Select(m => new SelectListItem()
            {
                Text = m.FName,
                Value = m.EmpID.ToString(),
            });
            return Json(employeeData, JsonRequestBehavior.AllowGet);
        }
        
        // Get State from DB by Project ID
        public IList<DAL.tblEmployeeMaster> Getprojectname(string ProjectId)
        {
            return objmms.tblEmployeeMasters.Where(a => a.ProjectID.Contains(ProjectId)).DefaultIfEmpty<DAL.tblEmployeeMaster>().ToList();
        }

        public JsonResult GetDetailOnPO(string Purchasereq)
        {
            string ProjId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Purchasereq).First().ProjectNo;
            List<PIdetailOnPOCreation> res = new List<PIdetailOnPOCreation>();
            res = objpro.GetD(ProjId,Purchasereq);
            if (res != null)
            {
                return Json(res.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else {

                return null;
            }
        }

        public ActionResult GetPurchaseReqDetailOnPO(decimal id) 
        {

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessObjects.Entity.PurchaseRequisitionMaster Master = m.GetPurchaseRequisitionMasterManager().Find(id);

            if (Master.Date != null)
                Master.Date2 = Master.Date.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

            List<BusinessObjects.Entity.PurchaseRequisitionChild> ChildList = m.GetPurchaseRequisitionChildManager().FindByMaster(id);

            var a = objmms.PurchaseRequisitionChilds.Where(b => b.MId == id).ToList();

            var totalRows = ChildList.Count();
            var totRows = a.Count();
            ViewBag.PurchaseTypeId = Master.PurchasePI_Type;
            var data = new PurchaseIRNew()
            {
                MasterNew = Master,
                ChildNew = a
            };

            var FinalPurchasetypeId = objmms.PurchaseRequisitionMasters.Where(x => x.UId == Master.UId).First().PurchasePIC_Type;
            ViewBag.FinalPurchaseType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == FinalPurchasetypeId).First().PurchaseType;

            var PILimit = objmms.tblPurchaseRequisitionApprovalLimitValues.Where(x => x.ProjectId == Master.ProjectNo).OrderByDescending(p => p.TransId).First().LimitValue;
            ViewBag.LastLimitVal = PILimit;


            if (data == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PurRequsionDetailOnPO", data);
        
    }

        public ActionResult GetRemarks(string PI)
        {
            string Proj_ID = Session["ProjectssId"].ToString();
            string PI_ID = PI.Replace("#", "/");
            List<PI_Master_Reamrks> res = null;
            res = objpro.GetAllItemStatusReport(PI_ID, Proj_ID);
            
            if (res != null)
            {
                return PartialView("_Purchase_Master_Remarks_Detail", res);
            }
            else {
                return View("_EmptyView");

            }


        }

        public ActionResult RelesedPO()
        {
            ViewBag.EmpId = EmpId;
            ViewBag.Date = DateTime.Now.Date;
            return View();
        }
        public ActionResult GetUnReleasePO(string PRJID)
        {
            
            var Result = objpro.GetUnreleasePO(PRJID);
            if (Result != null)
            {
                return PartialView("_UnreleasePO", Result);
            }
            else
            {
                return PartialView("_EmptyView");
            }
            
        }
        public ActionResult GetReleasePO(string PRJID)
        {
            
            var Result = objpro.GetReleasePO(PRJID);
            if (Result != null)
            {
                return PartialView("_ReleasePOList", Result);
            }
            else
            {
                return PartialView("_EmptyView");
            }

        }


        public ActionResult GetPoRemarks(string Uid)
        {

            string Proj_ID = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == Uid.Replace("#", "/")).FirstOrDefault().ProjectNo;
            string PO_ID = Uid.Replace("#", "/");
            List<POAllRemarks> res = null;
            res = objpro.GetAllPORemarks(Proj_ID, PO_ID);

            if (res != null)
            {
                return PartialView("_Partial_POAllRemarks", res);
            }
            else {
                return View("_EmptyView");

            }




            //List<PORemarks> remarks = null;
            // remarks = objpro.GetPORemarks(Uid);
            //if (remarks != null)
            //{
            //    return PartialView("_PORemarks", remarks);
            //}
            //else {
            //    return View("_EmptyView");
            //}

        }

        public ActionResult DoRelease(string UID)
        {
            var Im = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId.ToString() == UID).First();
            var IApp = objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(y => y.Purchase_Order_Indent_No == Im.PurchaseOrderNo).First();
            Im.IsPORelease = Convert.ToBoolean(1); Im.POReleaseBy = EmpId; Im.POReleaseDate = System.DateTime.Now;
            IApp.IsPORelease = Convert.ToBoolean(1); IApp.POReleaseBy = EmpId;IApp.POReleaseDate = System.DateTime.Now;
           objmms.Entry(Im).State = EntityState.Modified;
          objmms.Entry(IApp).State = EntityState.Modified;
           objmms.SaveChanges();
            ViewBag.EmpId = EmpId;
            ViewBag.Date = DateTime.Now.Date;
            return RedirectToAction("RelesedPO");
            //return View("RelesedPO");
        }

        public ActionResult GetPO_details_with_Indent(string id)
        {
            WordToText print = new WordToText();
            string empId = Session["EmpID"].ToString();
            //var a = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            //var t = a.Select(x => new SelectListItem
            //{
            //    Value = x.ProjectID.ToString(),
            //    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            //});
            //ViewBag.prjtid = t;

            //main code start here
            id = id?.Replace("#", "/");
            Session["POId"] = id.ToString();

            var a = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == id.ToString()).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectNo,
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectNo).First().ProjectName
            });
            ViewBag.prjtid = t;



            var units = "";
            if (id != null)
            {
                var querys = (from mas in objmms.TbIndentPurchaseOrderMasters
                              join chld in objmms.TbIndentPurchaseOrderChilds
                             // Both anonymous types should have exact same number of properties having same name and datatype
                             // on new { a = (int?)mas.UId } equals new { a = chld.MUId }
                             on mas.UId equals chld.MUId
                              where mas.PurchaseOrderNo == id
                              select new Purchase_Order_Slip_VM
                              {
                                  UId =mas.UId,
                                  SupplierNo = mas.SupplierNo,
                                  Name = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Name,
                                  Address = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Address,
                                  AcilTinNo = objmms.tblProjectTINNoes.Where(k => k.ProjectId == mas.ProjectNo).FirstOrDefault().TINNo,
                                  MobileNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().MobileNo,
                                  ContactPerson = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().ContactPerson,

                                  SurCharge = mas.SurCharge,
                                  Cartage = mas.Cartage,
                                  Reference = mas.Reference,
                                  Remark = chld.Remark,
                                  IndentRefNo = mas.IndentRefNo,
                                  PurchaseOrderNo = mas.PurchaseOrderNo,
                                  Vat = chld.Vat,
                                  SubTotal = chld.SubTotal,
                                  GrandTotal = mas.GrandTotal,
                                  Total = mas.Total,
                                  ProjectNo = mas.ProjectNo,
                                  ProjectName = objmms.tblProjectMasters.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().ProjectName,
                                  Location = objmms.tblProjectParticularsDetailAs.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().Location,

                                  PurchaseOrderDate = mas.PurchaseOrderDate,
                                  ItemNo = chld.ItemNo,
                                  ItemName = objmms.tblItemMasters.Where(j => j.ItemID == chld.ItemNo).FirstOrDefault().ItemName,
                                  UnitID = objmms.tblItemMasters.Where(l => l.ItemID == chld.ItemNo).FirstOrDefault().UnitID,
                                  UnitName = objmms.tblUnitMasters.Where(l => l.UnitID == objmms.tblItemMasters.Where(ll => ll.ItemID == chld.ItemNo).FirstOrDefault().UnitID).FirstOrDefault().UnitName,
                                  // Amount = chld.Amount,
                                  Amount = chld.Item_TotalAmt,
                                  Discount = chld.Discount,
                                  Qty = chld.Qty,
                                  Rate = chld.Rate,
                                  ExciseDutyRate = mas.ExciseDutyRate,
                                  ExciseDuty = mas.ExciseDuty,

                                  //  --------------------added Newely here-------
                                  ItemDescrp =chld.Item_description,
                                  Excise_DutyType = chld.ExciseDutyType,
                                  Ex_Total = chld.ExcisepercentageAmt,
                                  Cartage_Type_1 = objmms.tblMMSCartageTypes.Where(x => x.TransId == chld.Cartage1_ID).FirstOrDefault().CartageType,
                                  Cartage_Type_2 = objmms.tblMMSCartageTypes.Where(x => x.TransId == chld.Cartage2_Id).FirstOrDefault().CartageType,
                                  CR1_Total = chld.CartageAmount,
                                  CR2_Total = chld.CartageAmount2,
                                  Tax_type = chld.Tax_Type,
                                  T_Tax = chld.Tax_Amount,
                                  Insi_Type_1 = chld.Insurance1_Type,
                                  Insi_Type_2 = chld.Insurance2_Type,
                                  Insi_Type_3 = chld.Insurance3_Type,
                                  Insi_Type_4 = chld.Insurance4_Type,
                                  T_Insi1 = chld.Insurance1Amount,
                                  T_Insi2 = chld.Insurance2_Amount,
                                  T_Insi3 =chld.Insurance3_Amount,
                                  T_Insi4 = chld.Insurance4_Amount,
                                  T_Total = mas.Total,
                                  P_Total = chld.PackingCharges,
                                  T_SubTotal1 = mas.SubTotal,
                                  T_SubTotal2 = mas.SubTotal2,
                                  T_TotTax = chld.Tax_Amount,
                                  IsExcise = chld.IsExcise,
                                  IsCartage1 = chld.IsCartage1,
                                  IsCartage2 = chld.IsCartage2,
                                  IsTax = chld.IsTax,
                                  Is_Insi1 = chld.IsInsurance1,
                                  Is_Insi2 = chld.IsInsurance2,
                                  Is_Insi3 = chld.IsInsurance3,
                                  Is_Insi4 = chld.IsInsurance4,
                                  POCreatedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().LName,
                                  // UserType = UserType(mas.PurchaseOrderNo)
                                  UserType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == empId).FirstOrDefault().TypeName,
                                  CheckedPoLimit = mas.CheckedBeyondPOLimit,
                                  IsReleasePOBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().LName,
                                  PoApprovedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == (objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(y => y.Purchase_Order_Indent_No == mas.PurchaseOrderNo).FirstOrDefault().SecondLevelApprv_Id)).FirstOrDefault().FName +
                                                 " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == (objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(y => y.Purchase_Order_Indent_No == mas.PurchaseOrderNo).FirstOrDefault().SecondLevelApprv_Id)).FirstOrDefault().LName










                              }).ToList();
                //var projtn=querys.Select(k=> k.ProjectNo == )
                var data = querys;
                Print_Slip_For_ApprovalController.PrintData dataObj = new Print_Slip_For_ApprovalController.PrintData();
                
                dataObj.HeaderData = data.Select(x => new Print_Slip_For_ApprovalController.HeaderData
                {
                    UId =x.UId,
                    PurchaseOrderNo = x.PurchaseOrderNo,
                    Total = x.Total,
                    Location = x.Location,
                    ContactPerson = x.ContactPerson,
                    MobileNo = x.MobileNo,
                    Reference = x.Reference,
                    SurCharge = x.SurCharge,
                    Cartage = x.Cartage,
                    GrandTotal = x.GrandTotal,
                    SubTotal = x.SubTotal,
                    PurchaseOrderDate = x.PurchaseOrderDate,
                    AcilTinNo = x.AcilTinNo,
                    Address = x.Address,
                    Name = x.Name,
                    ProjectName = x.ProjectName,
                    IndentRefNo = x.IndentRefNo,
                    ExciseDutyRate = x.ExciseDutyRate,
                    ExciseDuty = x.ExciseDuty,
                    Excise_DutyType = x.Excise_DutyType,
                    Ex_Total = Math.Round((decimal)data.Sum(k => k.Ex_Total), 2),
                    Cartage_Type_1 = x.Cartage_Type_1,
                    CR1_Total = Math.Round((decimal)data.Sum(k => k.CR1_Total), 2),
                    Cartage_Type_2 = x.Cartage_Type_2,
                    CR2_Total = Math.Round((decimal)data.Sum(k => k.CR2_Total), 2),
                    Insi_Type_1 = x.Insi_Type_1,
                    Insi_Type_2 = x.Insi_Type_2,
                    Insi_Type_3 = x.Insi_Type_3,
                    Insi_Type_4 = x.Insi_Type_4,
                    Tax_1 = x.Tax_type,
                    T_TotTax = Math.Round((decimal)data.Sum(k => k.T_TotTax), 2),
                    T_Insi1 = Math.Round((decimal)data.Sum(k => k.T_Insi1), 2),
                    T_Insi2 = Math.Round((decimal)data.Sum(k => k.T_Insi2), 2),
                    T_Insi3 = Math.Round((decimal)data.Sum(k => k.T_Insi3), 2),
                    T_Insi4 = Math.Round((decimal)data.Sum(k => k.T_Insi4), 2),
                    T_Total = x.T_Total,
                    P_Total = Math.Round((decimal)data.Sum(k => k.P_Total), 2),
                    T_SubTotal1 = x.T_SubTotal1,
                    T_SubTotal2 = x.T_SubTotal2,
                    ProjectNo = x.ProjectNo,
                    PoCreatedBy = x.POCreatedBy,
                    UserType = x.UserType,
                    CheckedPoLimit = x.CheckedPoLimit.ToString(),
                    PoApprovedBy = x.PoApprovedBy,
                    IsReleasePOBy = x.IsReleasePOBy,
                    IsExcise = x.IsExcise,
                    IsTax = x.IsTax,
                    IsCartage1 = x.IsCartage1,
                    IsCartage2 = x.IsCartage2,
                    Is_Insi1 = x.Is_Insi1,
                    Is_Insi2 = x.Is_Insi2,
                    Is_Insi3 = x.Is_Insi3,
                    Is_Insi4 = x.Is_Insi4


                }).FirstOrDefault();

                dataObj.ItemData = data.Select(x => new Print_Slip_For_ApprovalController.ItemData { ItemName = x.ItemName,ItemDescrp=x.ItemDescrp, Remark = x.Remark, UnitName = x.UnitName, Rate = x.Rate, Discount = x.Discount, SubTotal = x.SubTotal, Vat = x.Vat, GrandTotal = x.GrandTotal, Qty = x.Qty, Amount = x.Amount, Tax_type = x.Tax_type, T_Tax = x.T_Tax }).ToList();

                ViewBag.PrintamountInWord = print.Cal(dataObj.HeaderData.GrandTotal ?? 0);

                //for vendor details
                if (id != null)
                {

                }



                //for genral terms and conditions

                if (id != null)
                {
                    var Genral_T_and_C = (from gtcs in objmms.tblGenral_Terms_Conditions_Child

                                          where gtcs.Purchase_Order_No == id

                                          select new Genral_term_conditon_Chld
                                          {
                                              GTC_Description = gtcs.GTC_Description
                                          }).ToList();
                    var data_gtc = Genral_T_and_C;
                    ViewBag.Gtc_details = data_gtc;
                }
                else {
                }


                //for Specific Instructions 
                if (id != null)
                {
                    var Specific_I = (from Spe_I in objmms.tblSpecific_Instructions_Child

                                      where Spe_I.Purchase_Order_No == id
                                      select new Specific_Instructions_TC_Chld
                                      {
                                          SI_Description = Spe_I.SI_Description
                                      }).ToList();
                    var data_si = Specific_I;
                    ViewBag.Si_details = data_si;
                }

                else {
                }

                //for delivery details code
                if (id != null)
                {
                    var Specific_De = (from de_I in objmms.TbDelivery_Details_PO

                                       where de_I.Purchase_order_No == id
                                       select new DeliveryPo_details_Chld
                                       {
                                           Delivery_Schedule = de_I.Delivery_Schedule,
                                           Delivery_Address = de_I.Delivery_Address,
                                           Contact_Person_Name = de_I.Contact_Person_Name,
                                           Contact_Person_Mobile = de_I.Contact_Person_Mobile,
                                           Billing_Address = de_I.Billing_Address
                                       }).ToList();

                    var data_delvry = Specific_De;
                    ViewBag.Delevery_details = data_delvry;
                }

                else {
                }





                //for Specific  terms and conditions
                if (id != null)
                {
                    var Specific_T_and_C = (from SpecificT_C in objmms.tblSpecificTermsAndConditions_Child

                                            where SpecificT_C.Purchase_Order_No == id
                                            select new Specific_Termss_And_conditions_Chld
                                            {
                                                STC_Description = SpecificT_C.STC_Description
                                            }).ToList();
                    var data_stc = Specific_T_and_C;
                    ViewBag.Stc_details = data_stc;
                }
                else {
                }

                return PartialView("_ApprovedPOdetailOnIndentSection", dataObj);

            }
            else {



                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

    }
}