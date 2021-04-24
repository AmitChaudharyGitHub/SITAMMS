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
using MMS.Controllers;
using MMS.Helpers;

namespace MMS.Controllers3
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

            var yue = (from b2 in objmms.tblEmployeeMasters.Where(x=>x.ProjectID.Contains(prjId) && x.EmpID != EmpId).ToList()
                       join a2 in objmms.tblApproval_AccountType.Where(x1 =>x1.TypeName == "PMC" && x1.EmployeeId != EmpId)
                       on b2.EmpID equals a2.EmployeeId
                       select new { b2 }).ToList();


            List<DAL.tblEmployeeMaster> H2 = (from Z1 in yue select Z1.b2).ToList();
           

            //var dgh = (from b in objmms.tblEmployeeMasters.Where(x => x.ProjectID == prjId).ToList()
            //          join a1 in objmms.tblApproval_AccountType.Where(x1 => x1.ProjectId == prjId && x1.TypeName == "Purchase")
            //          on b.EmpID equals a1.EmployeeId 
            //          select new  { b }).ToList();

            var dgh = (from b in objmms.tblEmployeeMasters.Where(x=>x.ProjectID.Contains(prjId) && x.EmpID != EmpId).ToList()
                       join a1 in objmms.tblApproval_AccountType.Where(x1 => (x1.TypeName == "Purchase" || x1.TypeName == "PIC" || x1.TypeName == "Mang") &&  x1.EmployeeId != EmpId)
                       on b.EmpID equals a1.EmployeeId
                       select new { b }).ToList();
            List<DAL.tblEmployeeMaster> H = (from Z in dgh select Z.b).ToList();

           //ViewBag.Employee = new SelectList(m.GetEmployeeMasterManager().FindByProject(PRJID), "EmpID", "FName");
            // ViewBag.Employee = new SelectList(m.GetEmployeeMasterManager().FindByProject(t.FirstOrDefault().Value), "EmpID", "FName");
            //ViewBag.Employee = new SelectList(H2, "EmpID", "FName");
            ViewBag.Employee = H2.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName }).ToList();
            // ViewBag.PMCEmployee = new SelectList(H, "EmpID", "FName");
            ViewBag.PMCEmployee = H.Select(p => new SelectListItem { Value = p.EmpID, Text = p.FName + ' ' + p.LName }).ToList();

            int? PurchasePiTypeForOutPi = a.FirstOrDefault().PurchasePIC_Type;

            if (PurchasePiTypeForOutPi >= 3)
            {
                var EmpOutPITYpeLst = (from b in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(prjId) && x.EmpID != EmpId).ToList()
                                       join a1 in objmms.tblApproval_AccountType.Where(x1 => (x1.TypeName == "PIC" || x1.TypeName == "Mang") && x1.EmployeeId != EmpId)
                                       on b.EmpID equals a1.EmployeeId
                                       select new { b }).ToList();

                List<DAL.tblEmployeeMaster> HL = (from Z in EmpOutPITYpeLst select Z.b).ToList();
                ViewBag.Employee = HL.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName }).ToList();
                ViewBag.PMCEmployee = HL.Select(p => new SelectListItem { Value = p.EmpID, Text = p.FName + ' ' + p.LName }).ToList();


            }





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


        public ActionResult CreatePO_GST(string PI)
        {
            ViewBag.PI_reqNo = PI.Replace("#", "/");
            string empId = Session["EmpID"].ToString();
          
            string prjId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PI.Replace("#", "/")).FirstOrDefault().ProjectNo;
            
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

          
            List<BusinessObjects.Entity.tblEmployeeMaster> em = m.GetEmployeeMasterManager().FindByProject(prjId).ToList();
          

            var yue = (from b2 in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(prjId) && x.EmpID != EmpId).ToList()
                       join a2 in objmms.tblApproval_AccountType.Where(x1 => x1.TypeName == "PMC" && x1.EmployeeId != EmpId)
                       on b2.EmpID equals a2.EmployeeId
                       select new { b2 }).ToList();


            List<DAL.tblEmployeeMaster> H2 = (from Z1 in yue select Z1.b2).ToList();


            

            var dgh = (from b in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(prjId) && x.EmpID != EmpId).ToList()
                       join a1 in objmms.tblApproval_AccountType.Where(x1 => (x1.TypeName == "Purchase" || x1.TypeName == "PIC" || x1.TypeName == "Mang") && x1.EmployeeId != EmpId)
                       on b.EmpID equals a1.EmployeeId
                       select new { b }).ToList();
            List<DAL.tblEmployeeMaster> H = (from Z in dgh select Z.b).ToList();

            
            ViewBag.Employee = H2.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName }).ToList();
            
            ViewBag.PMCEmployee = H.Select(p => new SelectListItem { Value = p.EmpID, Text = p.FName + ' ' + p.LName }).ToList();

            int? PurchasePiTypeForOutPi = a.FirstOrDefault().PurchasePIC_Type;

            if (PurchasePiTypeForOutPi >= 3)
            {
                var EmpOutPITYpeLst = (from b in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(prjId) && x.EmpID != EmpId).ToList()
                                       join a1 in objmms.tblApproval_AccountType.Where(x1 => (x1.TypeName == "PIC" || x1.TypeName == "Mang") && x1.EmployeeId != EmpId)
                                       on b.EmpID equals a1.EmployeeId
                                       select new { b }).ToList();

                List<DAL.tblEmployeeMaster> HL = (from Z in EmpOutPITYpeLst select Z.b).ToList();
                ViewBag.Employee = HL.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName }).ToList();
                ViewBag.PMCEmployee = HL.Select(p => new SelectListItem { Value = p.EmpID, Text = p.FName + ' ' + p.LName }).ToList();


            }





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


        // Create PO for FOC/Cash Purchase Type

        public ActionResult CreateOutPro(string PI)
        {
            ViewBag.PI_reqNo = PI.Replace("#", "/");
            string empId = Session["EmpID"].ToString();
          
            string prjId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PI.Replace("#", "/")).FirstOrDefault().ProjectNo;
            
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

          //  string PRJID = m.GetProjectMasterManager().Find(SiteId).PRJID;

           
         
           int? PurchasePiTypeForOutPi = a.FirstOrDefault().PurchasePIC_Type;

            if (PurchasePiTypeForOutPi >= 3)
            {
                var EmpOutPITYpeLst = (from b in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(prjId) && x.EmpID != EmpId).ToList()
                                       join a1 in objmms.tblApproval_AccountType.Where(x1 => (x1.TypeName == "PIC" || x1.TypeName == "Mang") && x1.EmployeeId != EmpId)
                                       on b.EmpID equals a1.EmployeeId
                                       select new { b }).ToList();

                List<DAL.tblEmployeeMaster> HL = (from Z in EmpOutPITYpeLst select Z.b).ToList();
                ViewBag.Employee = HL.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName }).ToList();
                ViewBag.PMCEmployee = HL.Select(p => new SelectListItem { Value = p.EmpID, Text = p.FName + ' ' + p.LName }).ToList();


            }





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
                new SelectListItem { Text = "Yes", Value = "1" ,Selected=true },
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


        public ActionResult Grid_GST(decimal IndentNo_New, int page = 1)
        {
            const int pageSize = 100;
            List<BusinessObjects.Entity.PurchaseRequisitionChild> MasterList = null;

            List<MMS.ViewModels.Rate> dsas = new List<MMS.ViewModels.Rate>();
            List<MMS.ViewModels.ItemDescriptionData> abss = new List<MMS.ViewModels.ItemDescriptionData>();
            
            int totalRows = 0;

            if (SessionId != 0)
            {
                

                totalRows = m.GetPurchaseRequisitionChildManager().FindByMasterCount(IndentNo_New);
                MasterList = m.GetPurchaseRequisitionChildManager().FindByMaster(IndentNo_New);
                MasterList = MasterList.Where(a => a.ApprovedQty != Convert.ToDecimal(0.00) && a.ApprovedQty != null && a.ApprovedQty!=a.OrderedQty).ToList();

                if (MasterList != null)
                {
                    foreach (var mas in MasterList)
                    {
                        var projectid = mas.ProjectNo;
                        var igname = mas.ItemGroupNo;
                        var itemid = mas.ItemNo;
                        DateTime datetime = DateTime.Now.Date;
                        var existrated = objmms.ns_AddPriceCap.Where(h => h.ProjectId.Contains(projectid) && h.ItemGroupId == igname && h.ItemId == itemid && h.EffectiveDate <= datetime && h.ValidUpto >= datetime && h.EffectiveStatus == "On").DefaultIfEmpty<ns_AddPriceCap>().Select(f => new MMS.ViewModels.Rate { Rates = f.Rate, ItemId = f.ItemId }).FirstOrDefault();
                    
                        var itemds = objmms.tblItemMasters.Where(x => x.ItemID == itemid && x.ItemGroupID == igname).Select(k => new MMS.ViewModels.ItemDescriptionData { ItemDescription = k.ItemDescription, ItemId = k.ItemID, ItemExcessPercentage = k.Excess_Item_Percentage.ToString() }).FirstOrDefault();

                        dsas.Add(existrated);
                        abss.Add(itemds);


                    }
                    if (dsas != null)
                    {
                        ViewBag.ViewAdd_Rate = dsas;
                        
                    }
                    if (abss != null)
                    {
                        ViewBag.ViewAdd_Desc = abss;
                        


                    }

                    var TotalCartage = objmms.tblMMSCartageTypes.Where(x=>x.Status==true).Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
                    ViewBag.Cartage = new SelectList(TotalCartage, "Value", "Text");
                    var TotalGST = objmms.tblGST_SplitTaxMaster.ToList().OrderBy(p => p.TransId).Select(x => new { Text = x.TaxRateType, Value = x.TaxCode }).ToList();
                    ViewBag.GST = new SelectList(TotalGST, "Value", "Text");

                    var TCSTax = objmms.tbl_TCSTaxMaster.ToList().OrderBy(p => p.TCSRate).Select(x => new { Text = x.TCSSlab, Value = x.TCSRate }).ToList();
                    ViewBag.TCS = new SelectList(TCSTax, "Value", "Text");

                    var data = new GridIndentRequisitions()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        Child = MasterList.ToList()
                    };
                    if (Request.IsAjaxRequest())
                    {
                        return View("_GridViewLPGST", data);
                    }
                }
            }
            return View("_EmptyView");
        }




        public ActionResult GridEdit(decimal Uid, int page = 1)
        {
            const int pageSize = 100; int totalRows = 0; string IsExcise = string.Empty; string IsInsi1 = string.Empty; string Isinsi2 = string.Empty;
            string Isinsi3 = string.Empty; string Isinsi4 = string.Empty; string Iscartage1 = string.Empty; string Iscartage2 = string.Empty; string Istax = string.Empty;
            List<MMS.ViewModels.ItemDescriptionData> abss = new List<MMS.ViewModels.ItemDescriptionData>();
            List<MMS.ViewModels.ItemUnitDetail> lst = new List<ItemUnitDetail>();
            DAL.TbIndentPurchaseOrderMaster MGrid = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId == Uid).FirstOrDefault();

            List<DAL.TbIndentPurchaseOrderChild> MasterList = null;
            totalRows = objmms.TbIndentPurchaseOrderChilds.Where(x => x.MUId == Uid).ToList().Count;
            MasterList = objmms.TbIndentPurchaseOrderChilds.Where(x => x.MUId == Uid).ToList();
            List<SelectListItem> ObjList = new List<SelectListItem>()
                   {
                new SelectListItem { Text = "Select Status", Value = "-1" },
                new SelectListItem { Text = "Yes", Value = "1" },
                new SelectListItem { Text = "No", Value = "0" },

                   };





            if (MasterList != null)
            {
                foreach (var xx in MasterList)
                {
                    var projectid = MGrid.ProjectNo;
                    var igname = objmms.tblItemMasters.Where(x => x.ItemID == xx.ItemNo).First().ItemGroupID;
                    var itemid = xx.ItemNo;
                    DateTime datetime = DateTime.Now.Date;
                    var itemds = objmms.tblItemMasters.Where(x => x.ItemID == itemid && x.ItemGroupID == igname).Select(k => new MMS.ViewModels.ItemDescriptionData { ItemDescription = xx.Item_description, ItemId = k.ItemID }).FirstOrDefault();
                    abss.Add(itemds);
                    var det = objmms.tblItemMasters.Where(x => x.ItemID == itemid && x.ItemGroupID == igname).Select(p => new MMS.ViewModels.ItemUnitDetail { ItemName = p.ItemName, ItemNumber = p.ItemID, UnitNumber = p.UnitID, UnitName = objmms.tblUnitMasters.Where(c => c.UnitID == p.UnitID).FirstOrDefault().UnitName }).FirstOrDefault();
                    lst.Add(det);

                    var TotalCartage = objmms.tblMMSCartageTypes.Where(x => x.IsCartage == true).Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
                    ViewBag.TotalCartagetest = new SelectList(TotalCartage, "Value", "Text");

                    //  IsExcise = xx.IsExcise;
                    ViewBag.ExistStatus = new SelectList(ObjList, "Value", "Text", xx.IsExcise);


                }
                if (abss != null)
                {
                    ViewBag.ViewAdd_Desc = abss;


                }
                if (lst != null)
                {
                    ViewBag.ItemUnitDetail = lst;
                }


                //  ViewBag.ExistStatus = new  SelectList(ObjList, "Value", "Text",1);
                ViewBag.ExistStatus = ObjList;
                  var data = new GridPurchaseOrderEdit()
                {
                    TotalRows = totalRows,
                    PageSize = pageSize,
                    Child = MasterList.ToList()
                };
                if (Request.IsAjaxRequest())
                {
                    //return View("_GridEditPO", data); 
                    return View("_GridEdit_PO", data);
                }


            }
            return View("_EmptyView");

        }

        public ActionResult EditNew(string PO_No)
        {
            //IndentPurchaserOrder_EditNew  decimal id
            string PONO_Edit = PO_No.Replace("#", "/");
            decimal id = objmms.TbIndentPurchaseOrderMasters.Where(ko => ko.PurchaseOrderNo == PONO_Edit).FirstOrDefault().UId;


            DAL.TbIndentPurchaseOrderMaster Master = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId == id).First();
            ViewBag.PO_No = Master.PurchaseOrderNo;
            List<DAL.TbIndentPurchaseOrderChild> ChildList = objmms.TbIndentPurchaseOrderChilds.Where(x => x.MUId == Master.UId).ToList();
            string EmpId = Session["EmpID"].ToString();
            ViewBag.Date = Master.PurchaseOrderDate;
            ViewBag.EmpId = EmpId;
        
            ViewBag.VGId = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", Master.Vendor_Group_Id);
            string SendToId = objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(x => x.Purchase_Order_Indent_No == Master.PurchaseOrderNo).FirstOrDefault().Send_To;
            ViewBag.VNAME = new SelectList(objmms.tblVendorMasters.Where(x=>x.PRJID.Contains(Master.ProjectNo) && x.MulRegistrationType.Contains(Master.Vendor_Group_Id)).ToList(),"VendorID","Name", Master.SupplierNo);


            ViewBag.VG = m.GetVendorGroupManager().FindByVendor(Master.SupplierNo).TypeID;
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var a = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId == id && x.ProjectNo == Master.ProjectNo).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectNo.ToString(),

                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectNo.ToString()).First().ProjectName
            });
            ViewBag.prjtidsssss = t;


       var yue = (from b2 in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(Master.ProjectNo) && x.EmpID != EmpId).ToList()
                       join a2 in objmms.tblApproval_AccountType.Where(x1 => x1.TypeName == "PMC" && x1.EmployeeId != EmpId)
                       on b2.EmpID equals a2.EmployeeId
                       select new { b2 }).ToList();


            List<DAL.tblEmployeeMaster> H2 = (from Z1 in yue select Z1.b2).ToList();

           var dgh = (from b in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(Master.ProjectNo) && x.EmpID != EmpId).ToList()
                       join a1 in objmms.tblApproval_AccountType.Where(x1 => (x1.TypeName == "Purchase" || x1.TypeName == "PIC" || x1.TypeName == "Mang") && x1.EmployeeId != EmpId)
                       on b.EmpID equals a1.EmployeeId
                       select new { b }).ToList();

            List<DAL.tblEmployeeMaster> H = (from Z in dgh select Z.b).ToList();


            ViewBag.Employee = H2.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName  }).ToList();
            ViewBag.PMCEmployee = H.Select(p => new SelectListItem { Value = p.EmpID, Text = p.FName + ' ' + p.LName }).ToList();

         

            // int? PurchasePiTypeForOutPi = a.FirstOrDefault().PurchasePIC_Type;
            int? PurchasePiTypeForOutPi = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Master.IndentRefNo).FirstOrDefault().PurchasePIC_Type;

            if (PurchasePiTypeForOutPi >= 3)
            {
                var EmpOutPITYpeLst = (from b in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(Master.ProjectNo) && x.EmpID != EmpId).ToList()
                                       join a1 in objmms.tblApproval_AccountType.Where(x1 => (x1.TypeName == "PIC" || x1.TypeName == "Mang") && x1.EmployeeId != EmpId)
                                       on b.EmpID equals a1.EmployeeId
                                       select new { b }).ToList();

                List<DAL.tblEmployeeMaster> HL = (from Z in EmpOutPITYpeLst select Z.b).ToList();
                ViewBag.Employee = HL.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName }).ToList();
                ViewBag.PMCEmployee = HL.Select(p => new SelectListItem { Value = p.EmpID, Text = p.FName + ' ' + p.LName }).ToList();


            }











            ViewBag.Purchase_id = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Master.IndentRefNo).First().UId;
            ViewBag.POLimit = objmms.tblPurchaseOrderApprovalLimitValues.Where(x => x.ProjectId == Master.ProjectNo).OrderByDescending(k => k.CreatedDate).First().LimitValue;
            var qid = objmms.tblMMSQuotations.Where(x => x.PurchaseReqNo == Master.IndentRefNo).FirstOrDefault();
            if (qid == null)
            {
                ViewBag.QuotID = "N/A";
            }
            else
            {
                ViewBag.QuotID = objmms.tblMMSQuotations.Where(x => x.PurchaseReqNo == Master.IndentRefNo).FirstOrDefault().TransId;
            }


            var data = new IndentPurchaserOrder_EditNew()
            {
                Master = Master,
                Child = ChildList

            };
            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }


        public JsonResult GetPODetailForEdit(string PONo)
        {
            string ProjId = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == PONo).First().ProjectNo;
            List<POAndPIDetailOnPOEdit> res = new List<POAndPIDetailOnPOEdit>();
            res = objpro.GetPODEdit(ProjId, PONo);
            if (res != null)
            {
                return Json(res.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {

                return null;
            }
        }

        public ActionResult View_GTCDataPOWise(decimal id)
        {
            string PONo = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId == id).First().PurchaseOrderNo;
            string ProjectNo = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId == id).First().ProjectNo;
            //var a = (from pd in objmms.tblGenral_Terms_Conditions_Child.Where(k => k.IsActive == "Yes" && k.Purchase_Order_No == PONo).OrderBy(c => c.CreatedDate)
            //         select new MMS.ViewModels.Genral_Terms_and_conditions
            //         {
            //             Id = pd.Id,
            //             Header_Title = pd.Header_Title,
            //             GTC_Description = pd.GTC_Description,
            //             ProjectId = pd.ProjectId

            //         }).ToList();

            var a = (from pd in objmms.tblGenral_Terms_Conditions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreatedDate)
                     select new MMS.ViewModels.Genral_Terms_and_conditions
                     {
                         Id = objmms.tblGenral_Terms_Conditions_Child.Where(x=>x.IsActive =="Yes" && x.Purchase_Order_No ==PONo && x.GTC_Master_ID ==pd.Id).ToList().Count() > 0 ? objmms.tblGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).FirstOrDefault().Id :  pd.Id,
                         Header_Title = objmms.tblGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).FirstOrDefault().Header_Title :  pd.Header_Title,
                         GTC_Description = objmms.tblGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).FirstOrDefault().GTC_Description : pd.GTC_Description,
                         ProjectId = ProjectNo,
                         IsHaving = objmms.tblGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).ToList().Count() > 0 ? true : false

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
                    // return PartialView("_PartialView_GTC", data);  //_PartialViewPOWise_GTC
                    return PartialView("_PartialViewPOWise_GTC", data);
                }

                else
                {
                    return PartialView("_PartialViewPOWise_GTC", data);
                }

            }

            else
            {
                return View("_EmptyView");
            }

        }

        public ActionResult View_SIDataPOWise(decimal id)
        {

            string PONo = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId == id).First().PurchaseOrderNo;
            string ProjectNo = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId == id).First().ProjectNo;
            //var a = (from pd in objmms.tblSpecific_Instructions_Child.Where(k => k.IsActive == "Yes" && k.Purchase_Order_No == PONo).OrderBy(c => c.CreatedDate)
            //         select new Specific_Instruction_Conditions
            //         {
            //             Id = pd.Id,
            //             Header_Title = pd.Header_Title,
            //             SI_Description = pd.SI_Description,
            //             ProjectId = pd.ProjectId

            //         }).ToList();


            var a = (from pd in objmms.tblSpecific_Instructions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreatedDate)
                     select new Specific_Instruction_Conditions
                     {
                         Id = objmms.tblSpecific_Instructions_Child.Where(x=>x.IsActive =="Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).FirstOrDefault().Id : pd.Id,
                         Header_Title = objmms.tblSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).FirstOrDefault().Header_Title : pd.Header_Title,
                         SI_Description = objmms.tblSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).FirstOrDefault().SI_Description : pd.SI_Description,
                         ProjectId = ProjectNo,
                         IsHaving = objmms.tblSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).ToList().Count() > 0 ? true : false

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
                    //return PartialView("_PartialView_SI", data); //_PartialViewPOWise_SI
                    return PartialView("_PartialViewPOWise_SI", data);
                }

                else
                {
                    return PartialView("_PartialViewPOWise_SI", data);
                }

            }

            else
            {
                return View("_EmptyView");
            }

        }


        public ActionResult View_STCDataPOWise(decimal id)
        {
            string PONo = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId == id).First().PurchaseOrderNo;
            string ProjectNo = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId == id).First().ProjectNo;
            //var a = (from pd in objmms.tblSpecificTermsAndConditions_Child.Where(k => k.IsActive == "Yes" && k.Purchase_Order_No == PONo).OrderBy(c => c.CreatedDate)
            //         select new Specific_Terms_And_Conditions
            //         {
            //             Id = pd.Id,
            //             Statement_Header = pd.HeaderTitle,
            //             STC_Description = pd.STC_Description,
            //             ProjectId = pd.ProjectId

            //         }).ToList();


            var a = (from pd in objmms.tblSpecificTermsAndConditions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreateDate)
                     select new Specific_Terms_And_Conditions
                     {
                         Id = objmms.tblSpecificTermsAndConditions_Child.Where(x=>x.IsActive == "Yes" && x.Purchase_Order_No ==PONo && x.STC_Master_ID ==pd.Id).ToList().Count() > 0 ? objmms.tblSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).FirstOrDefault().Id : pd.Id,
                         Statement_Header = objmms.tblSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).FirstOrDefault().HeaderTitle : pd.Statement_Header,
                         ProjectId = ProjectNo,
                         STC_Description = objmms.tblSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).FirstOrDefault().STC_Description : pd.STC_Description,
                         IsHaving = objmms.tblSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).ToList().Count() > 0 ? true : false

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
                    //  return PartialView("_PartialView_STC", data); //_PartialViewPOWise_STC
                    return PartialView("_PartialViewPOWise_STC", data);
                }

                else
                {
                    return PartialView("_PartialViewPOWise_STC", data);
                }

            }

            else
            {
                return View("_EmptyView");
            }

        }

        public JsonResult GetPODeliveryDetail(decimal Uid)
        {
            string PONo = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId == Uid).ToList().FirstOrDefault().PurchaseOrderNo;
            var lst = objmms.TbDelivery_Details_PO.Where(x => x.Purchase_order_No == PONo).ToList().Select(p => new { DeliverySchedule = p.Delivery_Schedule, DeliveryAddr = p.Delivery_Address, BillingAddr = p.Billing_Address, PName = p.Contact_Person_Name, ContMobile = p.Contact_Person_Mobile }).FirstOrDefault();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetAllGSTType(string TaxCode)
        {
            MMS.DAL.tblGST_SplitTaxMaster gs = objmms.tblGST_SplitTaxMaster.Where(x => x.TaxCode == TaxCode).FirstOrDefault();
            var lst = new { tax_CGST= gs.CGST , tax_SGSTandUTGST = gs.SGST, tax_IGST =gs.IGST };
            return Json(lst.ToJSON(), JsonRequestBehavior.AllowGet);
            
        }







        [HttpPost]
        public ActionResult AddOutProPO(IndentPurchaserOrderNew_GST Grid)
        {
            decimal? MaxUid = 0;
            if (string.IsNullOrEmpty(Grid.Master.SendTo))
            {
                return  Json(new { Status = "001" }, JsonRequestBehavior.AllowGet); //Not Select Forword to
            }
            try
            {
                if (EmpId != null)
                {

                if (Grid.Child.Count() > 0)
                {

                        #region
                     var PoNo = objmms.TbIndentPurchaseOrder_GST.FirstOrDefault(x => x.PurchaseOrderNo == Grid.Master.PurchaseOrderNo);

                      if (PoNo != null)
                     {
                        return Json(new { Status = "6" }, JsonRequestBehavior.AllowGet); //Data is already exist.
                     }
                    else {

                 




                        MaxUid = findUidForNewRow_GST();
                            if (MaxUid != 0)
                            {
                                tblALLRemark alrm = new tblALLRemark();

                                string VarPurchaseReqNo = Grid.Master.IndentRefNo;
                                string poindentNo = Grid.Master.PurchaseOrderNo; string Purreq = string.Empty;



                                Grid.Master.CreatedBy = EmpId;
                                Grid.Master.Status = "Pending";
                                Grid.Master.Edited_Status = "No";
                                Grid.Master.Vendor_Group_Id = Grid.Master.Vendor_Group_Id;
                                Grid.Master.CreatedDate = System.DateTime.Now;
                                Grid.Master.Session_Year = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.PurchaseOrderDate));
                                Grid.Master.PurchaseOrderId = FindMaxId_GST(Grid.Master.ProjectNo);
                                Grid.Master.DivisionId = Repos.GetUserDivision();
                                int? intPurchasePIType = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == VarPurchaseReqNo).FirstOrDefault().PurchasePIC_Type;
                                if (intPurchasePIType >= 3)
                                {

                                    Grid.Master.POType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == intPurchasePIType).FirstOrDefault().Purchase_Code;
                                    Grid.Master.IndentRefNo = Purreq = VarPurchaseReqNo;
                                }
                                else {

                                    string[] IndentNo = Grid.Master.IndentRefNo.Split('/');
                                    Grid.Master.IndentRefNo = Purreq = IndentNo[0].ToString() + "/" + IndentNo[1].ToString() + "/" + IndentNo[2].ToString() + "/" + IndentNo[3].ToString();

                                    Grid.Master.POType = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Purreq).FirstOrDefault().PurchasePIC_Type == Convert.ToInt32(1) ? "IPO" : "LP";
                                }

                                var CheckedPoLimit = Grid.Master.CheckedBeyondPOLimit;
                                Grid.Master.SecondLevelApprv_Id = Grid.Master.SendTo;
                                //if (CheckedPoLimit == Convert.ToBoolean(1))
                                //{ Grid.Master.SecondLevelApprv_Id = Grid.Master.SendTo; }
                                //else { Grid.Master.FirstLevelApprv_Id = Grid.Master.SendTo; }

                                alrm.CreatedDate = System.DateTime.Now;
                                alrm.RemarkBy = EmpId;
                                alrm.RemarkDate = System.DateTime.Now;
                                alrm.Remarks = Grid.Master.Remarks;
                                if (objmms.tblApproval_AccountType.Where(e => e.EmployeeId == EmpId).ToList().Count() > 0)
                                {
                                    alrm.RemarkStage = objmms.tblApproval_AccountType.Where(e => e.EmployeeId == EmpId).FirstOrDefault().TypeName;
                                }
                                else
                                {
                                    alrm.RemarkStage = "EmpType";
                                }

                                alrm.PurchaseOrderNo = Grid.Master.PurchaseOrderNo;


                                decimal? OutproappLimit = objmms.tblOutProApprovalLimits.Where(d => d.ProjectId == Grid.Master.ProjectNo && d.PurchaseTypeId == intPurchasePIType).FirstOrDefault().LimitValue;

                                if (OutproappLimit < Grid.Master.GrandTotal)
                                {
                                    return Json(new { Status = "7" }, JsonRequestBehavior.AllowGet); // crossed limit
                                }
                                else {

                                objmms.TbIndentPurchaseOrder_GST.Add(Grid.Master);
                                objmms.tblALLRemarks.Add(alrm);

                                if (Grid.Child != null)
                                    foreach (var x in Grid.Child)
                                    {

                                        x.MUId = Convert.ToDecimal(MaxUid);
                                        x.Receive = 0;

                                            decimal ex_itm_per = Convert.ToDecimal(objmms.tblItemMasters.Where(itm => itm.ItemID == x.ItemNo).FirstOrDefault().Excess_Item_Percentage);
                                            decimal ex_itm_qnty = Convert.ToDecimal((x.Qty * ex_itm_per) / 100);
                                            x.Item_ExcessPercentage = ex_itm_per;
                                            x.Item_ExcessQuantity = ex_itm_qnty;

                                            objmms.TbIndentPurchaseOrderChild_GST.Add(x);

                                        if (x.IndentId != 0 && x.Qty != 0)
                                        {
                                            DAL.PurchaseRequisitionChild a = objmms.PurchaseRequisitionChilds.Where(d => d.UId == x.IndentId).First();

                                            if (a.OrderedQty == null)
                                            {
                                                a.OrderedQty = 0;
                                            }
                                            else
                                            {
                                                a.OrderedQty = a.OrderedQty + x.Qty;
                                            }

                                            objmms.Entry(a).State = EntityState.Modified;



                                        }
                                        else
                                        {
                                            break;
                                        }


                                    }

                                Grid.Child5.Status = "Valid";
                                Grid.Child5.CreatedDate = DateTime.Now;
                                Grid.Child5.CreatedBy = EmpId;
                                Grid.Child5.EditStatus = "No";
                                Grid.Child5.CompanyId = Session["CompanyId"].ToString();
                                objmms.TbDelivery_Details_PO.Add(Grid.Child5);

                                objmms.SaveChanges();
                                // after adding p.O then P.I complete will be updated.
                                m.GetPurchaseRequisitionMasterManager().FindCompleteO(Grid.Master.IndentRefNo);
                                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        else
                        {
                            return Json(new { Status = "4" }, JsonRequestBehavior.AllowGet);

                        }
                    }
                    #endregion

                }
                else
                {

                    return Json(new { Status = "3" }, JsonRequestBehavior.AllowGet);
                }

                }
                else
                {
                    return Json(new { Status = "5" }, JsonRequestBehavior.AllowGet); // Session has Expired. Please Re-login or Re-Load Page.
                }

            }
            catch (Exception ex)
            { ex.ToString(); return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet); }

        }


        [HttpPost]
        public ActionResult UpdateEdited_PO(IndentPurchaserOrderNew Grid)
        {
            // note : here update those PO whose does not approved.
            try {

                var NotApprvDetail = objmms.TbIndentPurchaseOrderMasters.Where(xk => xk.PurchaseOrderNo == Grid.Master.PurchaseOrderNo && xk.Status == "Not Approved").ToList();
                if (NotApprvDetail.Count() > 0)
                {

                    #region

                    if (Grid.Master.PurchaseOrderNo != null)
                    {

                        var CheckedPoLimit = Grid.Master.CheckedBeyondPOLimit;
                        // TbindentPurchaseOrder
                        MMS.DAL.TbIndentPurchaseOrderMaster tbms = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == Grid.Master.PurchaseOrderNo).FirstOrDefault();
                        tbms.SupplierNo = Grid.Master.SupplierNo;
                        tbms.Total = Grid.Master.Total;
                        tbms.SubTotal = Grid.Master.SubTotal;
                        tbms.SubTotal2 = Grid.Master.SubTotal2;
                        tbms.GrandTotal = Grid.Master.GrandTotal;
                        tbms.SendTo = Grid.Master.SendTo;
                        tbms.Status = "Pending";
                        tbms.Reference = Grid.Master.Reference;
                        tbms.Vendor_Group_Id = Grid.Master.Vendor_Group_Id;
                        tbms.Edited_Status = "Yes";
                        tbms.CheckedBeyondPOLimit = CheckedPoLimit; // here write down query to check beyond PO limit
                        tbms.Purchase_Id = Grid.Master.Purchase_Id;

                        objmms.Entry(tbms).State = EntityState.Modified;
                        // end TbindentPurchaseIndent

                        // TbIndentPurchaseOrder_Approval_For_Print

                        MMS.DAL.TbIndentPurchaseOrder_Approval_For_Print tbmsprint = objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(x => x.Purchase_Order_Indent_No == Grid.Master.PurchaseOrderNo).FirstOrDefault();
                        tbmsprint.Remark = Grid.Master.Remarks;
                        tbmsprint.Status = "Pending";
                        tbmsprint.Vendor_Group_Id = Grid.Master.SupplierNo;
                        tbmsprint.Employee_Name = objmms.tblEmployeeMasters.Where(h => h.EmpID == Grid.Master.SendTo).First().FName;
                        tbmsprint.Send_To = Grid.Master.SendTo;
                        tbmsprint.ModifiedDate = DateTime.Now;
                        tbmsprint.Status_Approval = "Not Approved";
                        tbmsprint.EditedStatus = "Yes";
                        tbmsprint.Remark = Grid.Master.Remarks;
                        if (CheckedPoLimit == Convert.ToBoolean(1))
                        { tbmsprint.SecondLevelApprv_Id = Grid.Master.SendTo; }
                        else { tbmsprint.FirstLevelApprv_Id = Grid.Master.SendTo; }

                        objmms.Entry(tbmsprint).State = EntityState.Modified;


                        // End TbIndentPurchaseOrder_Approval_For_Print

                        // For 

                        if (Grid.Child != null)
                        { 
                            foreach (var x in Grid.Child)
                            {
                                MMS.DAL.TbIndentPurchaseOrderChild tbchld = objmms.TbIndentPurchaseOrderChilds.Where(x1 => x1.UId == x.UId).FirstOrDefault();
                                tbchld.Rate = x.Rate;
                                tbchld.Discount = x.Discount;
                                tbchld.PackingCharges = x.PackingCharges;
                                tbchld.ExciseDutyType = x.ExciseDutyType;
                                tbchld.ExcisepercentageAmt = x.ExcisepercentageAmt;
                                tbchld.Cartage1_ID = x.Cartage1_ID;
                                tbchld.Cartage_rate = x.Cartage_rate;
                                tbchld.Insurance1_Type = x.Insurance1_Type;
                                tbchld.Insurance1Amount = x.Insurance1Amount;
                                tbchld.Tax_Type = x.Tax_Type;
                                tbchld.Tax_Amount = x.Tax_Amount;
                                tbchld.Cartage2_Id = x.Cartage2_Id;
                                tbchld.Cartage2_rate = x.Cartage2_rate;
                                tbchld.Insurance2_Type = x.Insurance2_Type;
                                tbchld.Insurance2_Amount = x.Insurance2_Amount;
                                tbchld.CartageAmount = x.CartageAmount;
                                tbchld.CartageAmount2 = x.CartageAmount2;
                                tbchld.Item_TotalAmt = x.Item_TotalAmt;
                                tbchld.Item_SubTotal1 = x.Item_SubTotal1;
                                tbchld.Item_SubTotal2 = x.Item_SubTotal2;
                                tbchld.Item_GrandTotal = x.Item_GrandTotal;
                                tbchld.Item_description = x.Item_description;
                                tbchld.IsExcise = x.IsExcise;
                                tbchld.IsInsurance1 = x.IsInsurance1;
                                tbchld.Insurance1Rate = x.Insurance1Rate;
                                tbchld.IsCartage1 = x.IsCartage1;
                                tbchld.IsInsurance2 = x.IsInsurance2;
                                tbchld.Insurance2Rate = x.Insurance2Rate;
                                tbchld.IsTax = x.IsTax;
                                tbchld.TaxPercentage = x.TaxPercentage;
                                tbchld.IsInsurance3 = x.IsInsurance3;
                                tbchld.Insurance3_Amount = x.Insurance3_Amount;
                                tbchld.IsCartage2 = x.IsCartage2;
                                tbchld.IsInsurance4 = x.IsInsurance4;
                                tbchld.Insurance4_Type = x.Insurance4_Type;
                                tbchld.Insurance4_Rate = x.Insurance4_Rate;
                                tbchld.Insurance4_Amount = x.Insurance4_Amount;

                                objmms.Entry(tbchld).State = EntityState.Modified;
                                objmms.SaveChanges();
                                 
                            }
                        }

                        //

                        // for  tblSpecificTermsAndConditions_Child
                        if (Grid.Child2 != null)
                        {

                       
                            foreach (var x in Grid.Child2)
                            {
                                var exist = objmms.tblSpecificTermsAndConditions_Child.Where(k => k.Purchase_Order_No == Grid.Master.PurchaseOrderNo && k.Id == x.Id).ToList().Count();
                                if (exist > 0)
                                {
                                    if (x.Status == "Checked")
                                    {
                                        // Just Leaving without making any changing.
                                    }
                                    else if (x.Status == "UnChecked")
                                    {
                                        // Updating Status and make it disable.
                                        MMS.DAL.tblSpecificTermsAndConditions_Child tbsptcnd = objmms.tblSpecificTermsAndConditions_Child.Where(x2 => x2.Purchase_Order_No == Grid.Master.PurchaseOrderNo && x2.Id == x.Id).FirstOrDefault();
                                        tbsptcnd.STC_Description = x.STC_Description;
                                        tbsptcnd.Edited_Status = "Yes";
                                        tbsptcnd.Status = "Disable";
                                        tbsptcnd.IsActive = "No";
                                        tbsptcnd.ModifiedBy = EmpId;
                                        tbsptcnd.ModifiedDate = System.DateTime.Now;

                                        objmms.Entry(tbsptcnd).State = EntityState.Modified;
                                        objmms.SaveChanges();
                                    }
                                }
                                else {

                                    if (x.Status == "Checked")
                                    {
                                        // Insert record in table 
                                        x.HeaderTitle = x.HeaderTitle;
                                        x.STC_Description = x.STC_Description;
                                        x.Purchase_Order_No = x.Purchase_Order_No;
                                        x.STC_Master_ID = x.Id;
                                        x.CreatedBy = EmpId;
                                        x.CreatedDate = System.DateTime.Now;
                                        x.Edited_Status = "Yes";
                                        x.Status = "Enable";
                                        x.IsActive = "Yes";
                                        x.ModifiedBy = EmpId;
                                        x.ModifiedDate = System.DateTime.Now;
                                        objmms.tblSpecificTermsAndConditions_Child.Add(x);
                                        objmms.SaveChanges();

                                    }
                                    else {
                                        //Leaving without making any changes.
                                    }

                                }
                                
                            }
                        }
                        //


                        //  for tblSpecific_Instructions_Child
                        if (Grid.Child3 != null)
                        { 
                            foreach (var x in Grid.Child3)
                            {

                                var CheckSIexist = objmms.tblSpecific_Instructions_Child.Where(k => k.Purchase_Order_No == Grid.Master.PurchaseOrderNo && k.Id == x.Id).ToList().Count();
                                if (CheckSIexist > 0)
                                {
                                    if (x.Status == "Checked")
                                    {
                                        // Just Leaving without making any changing.
                                    }

                                    else if(x.Status == "UnChecked") {
                                        // Updating Status and make it disable.

                                        MMS.DAL.tblSpecific_Instructions_Child tbsichld = objmms.tblSpecific_Instructions_Child.Where(x3 => x3.Purchase_Order_No == Grid.Master.PurchaseOrderNo && x3.Id == x.Id).FirstOrDefault();
                                        tbsichld.SI_Description = x.SI_Description;
                                        tbsichld.Edited_Status = "Yes";
                                        tbsichld.Status = "Disable";
                                        tbsichld.IsActive = "No";
                                        tbsichld.ModifiedBy = EmpId;
                                        tbsichld.ModifiedDate = System.DateTime.Now;
                                        objmms.Entry(tbsichld).State = EntityState.Modified;
                                        objmms.SaveChanges();

                                    }
                                }

                                else {

                                    if (x.Status == "Checked")
                                    {
                                        //Insert record in table 
                                        x.Purchase_Order_No = x.Purchase_Order_No;
                                        x.Header_Title = x.Header_Title;
                                        x.SI_Description = x.SI_Description;
                                        x.SI_Master_ID = x.Id;
                                        x.CreatedBy = EmpId;
                                        x.CreatedDate = System.DateTime.Now;
                                        x.Edited_Status = "Yes";
                                        x.Status = "Enable";
                                        x.IsActive = "Yes";
                                        x.ModifiedBy = EmpId;
                                        x.ModifiedDate = System.DateTime.Now;
                                        objmms.tblSpecific_Instructions_Child.Add(x);
                                        objmms.SaveChanges();

                                    }

                                    else {
                                        // Leave making without making changing.
                                    }
                                }

                            }
                        }
                        // 


                        // for 
                        if (Grid.Child4 != null)
                            foreach (var x in Grid.Child4)
                            {

                                var CheckGTExist = objmms.tblGenral_Terms_Conditions_Child.Where(k => k.Purchase_Order_No == x.Purchase_Order_No && k.Id == x.Id).ToList().Count();
                                if (CheckGTExist > 0)
                                {
                                    if (x.Status == "Checked")
                                    {// Just Leaving without making any changing.
                                    }
                                    else if (x.Status == "UnChecked")
                                    {
                                        // Updating Status and make it disable.
                                        MMS.DAL.tblGenral_Terms_Conditions_Child tbgtcd = objmms.tblGenral_Terms_Conditions_Child.Where(x4 => x4.Purchase_Order_No == Grid.Master.PurchaseOrderNo && x4.Id == x.Id).FirstOrDefault();
                                        tbgtcd.GTC_Description = x.GTC_Description;
                                        tbgtcd.Edited_Status = "Yes";
                                        tbgtcd.Status = "Disable";
                                        tbgtcd.IsActive = "No";
                                        tbgtcd.ModifiedBy = EmpId;
                                        tbgtcd.ModifiedDate = System.DateTime.Now;
                                        objmms.Entry(tbgtcd).State = EntityState.Modified;
                                        objmms.SaveChanges();
                                    }
                                }
                                else {

                                    if (x.Status == "Checked")
                                    {

                                        //Insert record in table 
                                        x.Header_Title = x.Header_Title;
                                        x.Purchase_Order_No = x.Purchase_Order_No;
                                        x.GTC_Description = x.GTC_Description;
                                        x.GTC_Master_ID = x.Id;
                                        x.CreatedBy = EmpId;
                                        x.CreatedDate = System.DateTime.Now;
                                        x.Edited_Status = "Yes";
                                        x.Status = "Enable";
                                        x.IsActive = "Yes";
                                        x.ModifiedBy = EmpId;
                                        x.ModifiedDate = System.DateTime.Now;
                                        objmms.tblGenral_Terms_Conditions_Child.Add(x);
                                        objmms.SaveChanges();
                                    }
                                    else
                                    {
                                        // Leave making without making changing.
                                    }


                                }

                            }
                        // end


                        // for 
                        MMS.DAL.TbDelivery_Details_PO tbdelpo = objmms.TbDelivery_Details_PO.Where(x5 => x5.Purchase_order_No == Grid.Master.PurchaseOrderNo).FirstOrDefault();
                        tbdelpo.Delivery_Schedule = Grid.Child5.Delivery_Schedule;
                        tbdelpo.ModifiedBy = EmpId;
                        tbdelpo.ModifiedDate = System.DateTime.Now;

                        objmms.Entry(tbdelpo).State = EntityState.Modified;

                        //


                       objmms.SaveChanges();
                        return Json(new { Status = "1", IndentNo = "" }, JsonRequestBehavior.AllowGet); // succefully updated


                    }
                    else
                    {
                        return Json(new { Status = "4", IndentNo = "" }, JsonRequestBehavior.AllowGet); // Something wrong in Purchase Order.
                        // somthing missing.
                    }


                    #endregion
                }
                else {
                      return Json(new { Status = "3", IndentNo = "" }, JsonRequestBehavior.AllowGet);
                    // PO status is NOT Approved , so can not be edited.
                }


            }
            catch (Exception ex)
            {
                ex.ToString(); // for some problem.
                return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet);
            }


            
        }

        [HttpPost]
        public ActionResult ADD_GSTPO(IndentPurchaserOrderNew_GST Grid, List<PI_AttachFileModel> AttachModel)
        {
            decimal? MaxUid = 0; int Pre_count = 0; int Post_Count = 0;
            MSP_Model objmsp = new MSP_Model();
            using (var transaction=objmms.Database.BeginTransaction())
            {
                try
                {
                    if (EmpId != null)
                    {

                        var dataToCheck = objmms.PurchaseRequisitionChilds.Where(x => x.PurRequisitionNo == Grid.Master.IndentRefNo).Select(x => new { SNo = x.SNo, ItemNo = x.ItemNo, BalanceQty = (x.ApprovedQty - x.OrderedQty) }).ToList();

                        foreach (var item in Grid.Child)
                        {
                            var isGreaterQty = dataToCheck.Any(x => x.ItemNo == item.ItemNo && x.SNo == item.UId && item.Qty > x.BalanceQty);
                            if (isGreaterQty)
                            {
                                return Json(new { Status = "9", Message = "Qty can not be greater than remaining qty. Please refresh the page." }, JsonRequestBehavior.AllowGet);
                            }

                            if (item.Rate.HasValue || item.NewRate.HasValue)
                            {

                            }
                            else
                            {
                                return Json(new { Status = "9", Message = "Rate can not be null" }, JsonRequestBehavior.AllowGet);
                            }

                            item.Rate = item.Rate.HasValue ? item.Rate : item.NewRate;

                        }

                        DateTime? indentdate = objmms.PurchaseRequisitionMasters.Where(s => s.PurRequisitionNo == Grid.Master.IndentRefNo).Select(s => s.Date).FirstOrDefault();

                        if (indentdate.Value > Grid.Master.PurchaseOrderDate.Value)
                        {
                            return Json(new { Status = "101", Message = "Purchase Order date must be greater than purchase indent date." }, JsonRequestBehavior.AllowGet);
                        }

                        string po_type= objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Grid.Master.IndentRefNo).FirstOrDefault().PurchasePIC_Type == Convert.ToInt32(1) ? "IPO" : "LP";
                        int? div_id = Repos.GetUserDivision();
                        string current_sess = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.PurchaseOrderDate));
                        var lastPO = objmms.TbIndentPurchaseOrder_GST.Where(s => s.ProjectNo == Grid.Master.ProjectNo && s.DivisionId == div_id && s.POType == po_type && s.Session_Year == current_sess).OrderByDescending(s => s.UId).Select(s => s).FirstOrDefault();
                        if (lastPO != null)
                        {
                            if (Grid.Master.PurchaseOrderDate.Value < lastPO.PurchaseOrderDate.Value)
                            {
                                return Json(new { Status = "9", Message = "Purchase Date must be greater than Last Created Purchase Date "+ lastPO.PurchaseOrderDate.Value.ToString("dd/MM/yyyy") }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        if (Grid.Child.Count() > 0)
                        {
                            if (Grid.Master.PurchaseOrderId == 0 && Grid.Master.PurchaseOrderId == null)
                            {
                                Grid.Master.PurchaseOrderNo = Get_PurchaseOrderNo(Grid.Master.ProjectNo, Grid.Master.IndentRefNo, Grid.Master.PurchaseOrderDate.Value);
                            }
                                #region
                            var PoNo = objmms.TbIndentPurchaseOrder_GST.FirstOrDefault(x => x.PurchaseOrderNo == Grid.Master.PurchaseOrderNo);

                            if (PoNo != null)
                            {
                                return Json(new { Status = "6" }, JsonRequestBehavior.AllowGet); //Data is already exist.
                            }
                            else
                            {
                                MaxUid = 1;
                                if (MaxUid != 0)
                                {

                                    tblALLRemark alrm = new tblALLRemark();

                                    string VarPurchaseReqNo = Grid.Master.IndentRefNo;
                                    string poindentNo = Grid.Master.PurchaseOrderNo; string Purreq = string.Empty;

                                    // Grid.Master.SessionId = SessionId;
                                    Grid.Master.CreatedBy = EmpId;
                                    Grid.Master.Status = "Pending";
                                    Grid.Master.Edited_Status = "No";
                                    //Grid.Master.Session_Year = objmsp.GetFinancialYear();
                                    Grid.Master.Session_Year = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.PurchaseOrderDate));

                                    // Grid.Master.Vendor_Group_Id = Grid.Master.Vendor_Group_Id;
                                    Grid.Master.CreatedDate = System.DateTime.Now;
                                    //Grid.Master.PurchaseOrderId = m.GetTbIndentPurchaseOrderMasterManager().FindMaxId(SessionId, Grid.Master.ProjectNo);

                                    //purchase id wil not issue if draft po
                                    //if (Grid.Master.PurchaseOrderId == 0 && Grid.Master.PurchaseOrderId == null)
                                    //{
                                    //    Grid.Master.PurchaseOrderId = FindMaxId_GST(Grid.Master.ProjectNo);
                                    //}

                                    Grid.Master.PurchaseOrderId = FindMaxId_GST(Grid.Master.ProjectNo);

                                    int? intPurchasePIType = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == VarPurchaseReqNo).FirstOrDefault().PurchasePIC_Type;
                                    if (intPurchasePIType >= 3)
                                    {

                                        Grid.Master.POType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == intPurchasePIType).FirstOrDefault().Purchase_Code;
                                        Grid.Master.IndentRefNo = Purreq = VarPurchaseReqNo;
                                    }
                                    else
                                    {

                                        //string[] IndentNo = Grid.Master.IndentRefNo.Split('/');
                                       // Grid.Master.IndentRefNo = Purreq = IndentNo[0].ToString() + "/" + IndentNo[1].ToString() + "/" + IndentNo[2].ToString() + "/" + IndentNo[3].ToString()+"/"+ IndentNo[4].ToString();

                                        Grid.Master.POType = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Grid.Master.IndentRefNo).FirstOrDefault().PurchasePIC_Type == Convert.ToInt32(1) ? "IPO" : "LP";
                                    }

                                    Grid.Master.DivisionId = Repos.GetUserDivision();

                                    Grid.Master.Edited_Status = "No";
                                    var CheckedPoLimit = Grid.Master.CheckedBeyondPOLimit;
                                    if (CheckedPoLimit == Convert.ToBoolean(1))
                                    { Grid.Master.SecondLevelApprv_Id = Grid.Master.SendTo; }
                                    else { Grid.Master.FirstLevelApprv_Id = Grid.Master.SendTo; }

                                    alrm.CreatedDate = System.DateTime.Now;
                                    alrm.RemarkBy = EmpId;
                                    alrm.RemarkDate = System.DateTime.Now;
                                    alrm.Remarks = Grid.Master.Remarks;
                                    if (objmms.tblApproval_AccountType.Where(e => e.EmployeeId == EmpId).ToList().Count() > 0)
                                    {
                                        alrm.RemarkStage = objmms.tblApproval_AccountType.Where(e => e.EmployeeId == EmpId).FirstOrDefault().TypeName;
                                    }
                                    else
                                    {
                                        alrm.RemarkStage = "EmpType";
                                    }

                                    alrm.PurchaseOrderNo = Grid.Master.PurchaseOrderNo;

                                    Grid.Master.GrandTotal = Grid.Master.GrandTotal ?? 0;
                                    Grid.Master.SubTotal1 = Grid.Master.SubTotal1 ?? 0;
                                    Grid.Master.SubTotal2 = Grid.Master.SubTotal2 ?? 0;
                                    Grid.Master.Total = Grid.Master.Total ?? 0;

                                    Grid.Master.Total_Cartage = Grid.Master.Total_Cartage ?? 0;
                                    Grid.Master.Total_CGST = Grid.Master.Total_CGST ?? 0;
                                    Grid.Master.Total_IGST = Grid.Master.Total_IGST ?? 0;
                                    Grid.Master.Total_Insurance = Grid.Master.Total_Insurance ?? 0;
                                    Grid.Master.Total_PAndF = Grid.Master.Total_PAndF ?? 0;
                                    Grid.Master.Total_SGSTAndUTGST = Grid.Master.Total_SGSTAndUTGST ?? 0;
                                    Grid.Master.Total_Taxable = Grid.Master.Total_Taxable ?? 0;
                                    Grid.Master.TCSActive = true;

                                    objmms.TbIndentPurchaseOrder_GST.Add(Grid.Master);
                                    objmms.tblALLRemarks.Add(alrm);

                                    objmms.SaveChanges();

                                    MaxUid = Grid.Master.UId;


                                    Pre_count = Grid.Child.Count();

                                    if (Grid.Child != null)
                                        foreach (var x in Grid.Child)
                                        {

                                            x.MUId = Convert.ToDecimal(MaxUid);
                                            x.Receive = 0;

                                            if (x.Qty == 0)
                                            {
                                                x.MUId = 0;
                                                x.OldMId= Convert.ToInt32(MaxUid);
                                            }

                                            x.GrossAmount= x.GrossAmount ?? 0;

                                            if (x.Rate != null && x.GrossAmount != null)
                                            {
                                                decimal ex_itm_per = Convert.ToDecimal(objmms.tblItemMasters.Where(itm => itm.ItemID == x.ItemNo).FirstOrDefault().Excess_Item_Percentage);
                                                decimal ex_itm_qnty = Convert.ToDecimal((x.Qty * ex_itm_per) / 100);
                                                x.Item_ExcessPercentage = ex_itm_per;
                                                x.Item_ExcessQuantity = ex_itm_qnty;

                                                Post_Count = Post_Count + 1;

                                                objmms.TbIndentPurchaseOrderChild_GST.Add(x);

                                                if (x.IndentId != 0)
                                                {
                                                    DAL.PurchaseRequisitionChild a = objmms.PurchaseRequisitionChilds.Where(d => d.UId == x.IndentId).First();

                                                    if (a.OrderedQty == null)
                                                    {
                                                        a.OrderedQty = 0;
                                                    }
                                                    else
                                                    {
                                                        a.OrderedQty = a.OrderedQty + x.Qty;
                                                    }

                                                    objmms.Entry(a).State = EntityState.Modified;

                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                string selecteditemname = objmms.tblItemMasters.Where(w => w.ItemID == x.ItemNo).FirstOrDefault().ItemName;
                                                return Json(new { Status = "7" + selecteditemname + " " }, JsonRequestBehavior.AllowGet);

                                            }


                                        }

                                    if (Grid.Child2 != null)
                                        foreach (var x in Grid.Child2)
                                        {
                                            if (x.Status == "Checked" || string.IsNullOrEmpty(x.Status))
                                            {
                                                x.CreatedBy = EmpId;
                                                x.CreatedDate = System.DateTime.Now;
                                                x.Edited_Status = "No";
                                                x.Status = "Enable";
                                                x.IsActive = "Yes";
                                                x.Purchase_Order_No = Grid.Master.PurchaseOrderNo;
                                                objmms.tblSpecificTermsAndConditions_Child.Add(x);
                                            }


                                        }

                                    if (Grid.Child3 != null)
                                        foreach (var x in Grid.Child3)
                                        {
                                            if (x.Status == "Checked" || string.IsNullOrEmpty(x.Status))
                                            {
                                                x.CreatedBy = EmpId;
                                                x.CreatedDate = System.DateTime.Now;
                                                x.Edited_Status = "No";
                                                x.Status = "Enable";
                                                x.IsActive = "Yes";
                                                x.Purchase_Order_No = Grid.Master.PurchaseOrderNo;
                                                objmms.tblSpecific_Instructions_Child.Add(x);
                                            }
                                        }
                                    if (Grid.Child4 != null)
                                        foreach (var x in Grid.Child4)
                                        {
                                            if (x.Status == "Checked" || string.IsNullOrEmpty(x.Status))
                                            {
                                                x.CreatedBy = EmpId;
                                                x.CreatedDate = System.DateTime.Now;
                                                x.Edited_Status = "No";
                                                x.Status = "Enable";
                                                x.IsActive = "Yes";
                                                x.Purchase_Order_No = Grid.Master.PurchaseOrderNo;
                                                objmms.tblGenral_Terms_Conditions_Child.Add(x);
                                            }


                                        }


                                    Grid.Child5.Status = "Valid";
                                    Grid.Child5.CreatedDate = DateTime.Now;
                                    Grid.Child5.CreatedBy = EmpId;
                                    Grid.Child5.EditStatus = "No";
                                    Grid.Child5.CompanyId = Session["CompanyId"].ToString();
                                    Grid.Child5.Purchase_order_No= Grid.Master.PurchaseOrderNo;
                                    objmms.TbDelivery_Details_PO.Add(Grid.Child5);


                                    var draftPO = objmms.TbDraftIndentPurchaseOrder_GST.Where(s => s.PurchaseOrderNo == Grid.Master.PurchaseOrderNo).FirstOrDefault();
                                    if (draftPO != null)
                                    {
                                        draftPO.IsPOEnter = 1;
                                        objmms.Entry(draftPO).State = EntityState.Modified;
                                    }

                                    if (Pre_count != 0 && Post_Count != 0)
                                    {
                                        if (Pre_count == Post_Count)
                                        {
                                            if (AttachModel != null)
                                            {
                                                foreach (PI_AttachFileModel attachFile in AttachModel)
                                                {
                                                    objmms.tbl_PurchaseOrderAttachFiles.Add(new DAL.tbl_PurchaseOrderAttachFiles() { AttachFile = attachFile.AttachFile, AttachFile_Name = attachFile.FileName, PONo = Grid.Master.PurchaseOrderNo, CreatedOn = DateTime.Now });
                                                }
                                            }
                                            objmms.SaveChanges();
                                            transaction.Commit();
                                            return Json(new { Status = "1", TransNo = Grid.Master.PurchaseOrderNo }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            return Json(new { Status = "8" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }

                                        if (AttachModel != null)
                                        {
                                            foreach (PI_AttachFileModel attachFile in AttachModel)
                                            {
                                                objmms.tbl_PurchaseOrderAttachFiles.Add(new DAL.tbl_PurchaseOrderAttachFiles() { AttachFile = attachFile.AttachFile, AttachFile_Name = attachFile.FileName, PONo = Grid.Master.PurchaseOrderNo, CreatedOn = DateTime.Now });
                                            }
                                        }

                                   
                                    objmms.SaveChanges();

                                    // after adding p.O then P.I complete will be updated.
                                    // m.GetPurchaseRequisitionMasterManager().FindCompleteO(Grid.Master.IndentRefNo);
                                    return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);

                                }
                                else
                                {
                                    return Json(new { Status = "4" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            #endregion

                        }
                        else
                        {

                            return Json(new { Status = "3" }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        transaction.Rollback();
                        return Json(new { Status = "5" }, JsonRequestBehavior.AllowGet); // Session has Expired. Please Re-login or Re-Load Page.
                    }

                }
                catch (Exception ex)
                {
                    ex.ToString(); transaction.Rollback();
                    return Json(new { Status = "2", IndentNo = "", Error = ex.Message + "\n" + ex.StackTrace }, JsonRequestBehavior.AllowGet); } 
            }
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
                    var indent_Mas = objmms.TbIndentPurchaseOrderMasters.SingleOrDefault(f => f.UId == a && f.PurchaseOrderNo == poindentNo);
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
                        var remain_qty = objmms.PurchaseRequisitionChilds.SingleOrDefault(d => d.PurRequisitionNo == Grid.Master.IndentRefNo && d.ItemNo == x.ItemNo).OrderedQty;
                        if (x.Qty >= x.LastQty)
                        {
                            var rem_qty = Appr_qty - remain_qty;
                            var rty = x.Qty - x.LastQty;
                            if (rty > 0)
                            {
                                DAL.PurchaseRequisitionChild bg = objmms.PurchaseRequisitionChilds.Where(l => l.PurRequisitionNo == Grid.Master.IndentRefNo && l.ItemNo == x.ItemNo).First();
                                var Fnl_qtys = bg.OrderedQty + Convert.ToDecimal(rty);
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
                        var stcD = objmms.tblSpecificTermsAndConditions_Child.SingleOrDefault(v => v.Id == x.Id);
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
                    IOAFP.Status = "Pending";
                    IOAFP.Status_Approval = "Not Approved";
                    IOAFP.EditedStatus = "Yes";
                    IOAFP.ModifiedDate = DateTime.Now;
                }

                //  objmms.SaveChanges();

                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
                // return Json(new { Status = "1", IndentNo = poindentNo.Replace("/", "@@") }, JsonRequestBehavior.AllowGet);
                //return Json("Update Successfully");
            }
            catch (Exception ex)
            {
                return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet);
            }

        }

        public int FindMaxId_GST(string PjId)
        {
            int K = objmms.TbIndentPurchaseOrder_GST.Where(x => x.ProjectNo == PjId).Select(i => i.PurchaseOrderId).DefaultIfEmpty(0).Max(i => i).Value;
            int B = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.ProjectNo == PjId).Select(i => i.PurchaseOrderId).DefaultIfEmpty(0).Max(i => i).Value;
            if (B > K)
                K = B;
            if (K == 0)
            {
                return 1;
            }
            else {
                return K + 1;
            }

           
        }


        public decimal? findUidForNewRow()
        {
            using (var context = new Model1())
            {
                //int? intIdt = db.Users.Max(u => (int?)u.UserId);
                decimal? id = objmms.TbIndentPurchaseOrderMasters.Max(x => (decimal?)x.UId);
                if (id == 0)
                {
                    return 1;
                }
                else {
                    return id + 1;
                }
            }
        }

        public decimal? findUidForNewRow_GST()
        {
            using (var context = new Model1())
            {
                //int? intIdt = db.Users.Max(u => (int?)u.UserId);
                //decimal? id = objmms.TbIndentPurchaseOrder_GST.Max(x => (decimal?)x.UId);
                decimal? id = objmms.TbIndentPurchaseOrder_GST.Select(i=>i.UId).DefaultIfEmpty(0).Max(i=>i);
                if (id == 0)
                {
                    return 1;
                }
                else
                {
                    return id + 1;
                }
            }
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


        public JsonResult GetPurchaseEmployeOnly(string ProjectId)
        {
            // binding user that have type purchase only on particular project.
            string J = string.Empty;
            var lst = (from a in objmms.tblApproval_AccountType.Where(x => x.TypeName == "Purchase").ToList()
                       join b in objmms.tblEmployeeMasters.Where(p => p.ProjectID.Contains(ProjectId)).ToList()
                       on a.EmployeeId equals b.EmpID
                       select new SelectListItem
                       {
                           Value=b.EmpID,
                           Text =b.FName + " " + b.LName
                       }).ToList();

            J = lst.ToJSON();
            return Json(J, JsonRequestBehavior.AllowGet);
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

                    var v = new { Address = E.Address ?? "N/A", TinNo = E.TinNo ?? "N/A", PanNo = E.PanNo ?? "N/A", VCountry = Country ?? "N/A", VState = State ?? "N/A", VCity = City ?? "N/A", CpName = E.ContactPerson ?? "N/A", CpMobile = E.MobileNo ?? "N/A", CpPhone = E.PhoneNo ?? "N/A", CpEmail = E.Email ?? "N/A", vAdhar = E.Aadhaar_No ?? "N/A", V_PIN = E.PinCode ?? "N/A", GST = E.GST_NO ?? "N/A", StateID = E.State };
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

        public JsonResult Getitemdetailss(string ItemID)
        {

            string Partno = "";

            var data = objmms.tblProjectParticularsDetailAs.Where(m => m.PRJID == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.PRJID.Length > 0)
            {
                Partno = data.Location;
            }
            // return Partno;
            return Json(new { DeliveryAddress = data.Location, BillingAddress = objpro.GetBillingAddressForPO(ItemID.Trim()) });
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

            var data = objmms.tblEmployeeMasters.Where(m => m.EmpID == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.EmpID.Length > 0)
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
        public JsonResult GetDetailOnPOGST(string Purchasereq)
        {
            string ProjId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Purchasereq).First().ProjectNo;
            List<PIdetailOnPOCreation> res = new List<PIdetailOnPOCreation>();
            string projectstate = objmms.tblProjectGSTNoes.Where(s => s.ProjectId == ProjId).Select(s => s.StateId).FirstOrDefault();
            res = objpro.GetDGST(ProjId, Purchasereq);
            if (res.Count > 0)
                res[0].StateID = projectstate;
            if (res != null)
            {
                return Json(res.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {

                return null;
            }
        }

        public JsonResult GetPONo_ByPODate(string ProjID, string PurReqNo, string PODate)
        {
            string PONo = objpro.GetPO_NO_New(ProjID, PurReqNo, Convert.ToDateTime(PODate));
            MSP_Model objmsp = new MSP_Model();
            int? divisionId = Repos.GetUserDivision();
            var sessioncode = objmsp.GetFinancialYearByDate(Convert.ToDateTime(PODate));
            var purtype_id = objmms.PurchaseRequisitionMasters.Where(s => s.PurRequisitionNo == PurReqNo).Select(s => s.PurchasePIC_Type).FirstOrDefault();
            string po_type = "";
            if (purtype_id == 1)
                po_type = "IPO";
            else if (purtype_id == 2)
                po_type = "LP";
            else
                po_type = objmms.tblPI_PurchaseType.Where(s => s.TrandId == purtype_id).Select(s => s.Purchase_Code).FirstOrDefault();
            var last_PO = objmms.TbIndentPurchaseOrder_GST.Where(s => s.ProjectNo == ProjID && s.DivisionId == divisionId && s.Session_Year == sessioncode && s.POType == po_type).OrderByDescending(s => s.UId).FirstOrDefault();

            string last_PODate = "";
            string last_PONo = "";
            if (last_PO != null)
            {
                last_PODate = last_PO.PurchaseOrderDate.Value.ToString("dd/MM/yyyy");
                last_PONo = last_PO.PurchaseOrderNo;
            }

            var PONo_lst = new { PO_Number = PONo, Last_PONo = last_PONo, Last_PODate = last_PODate };
            string J = PONo_lst.ToJSON();
            return Json(J, JsonRequestBehavior.AllowGet);
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

        public ActionResult RelesedPO(string PId = "")
        {
            ViewBag.EmpId = EmpId;
            ViewBag.PId = PId;
            ViewBag.Date = DateTime.Now.Date;
            return View();
        }
        public ActionResult GetUnReleasePO(string PRJID,string PONumber,string VendorId)
        {
            
            var Result = objpro.GetUnreleasePOGST(PRJID,PONumber,VendorId);
            if (Result != null)
            {
                return PartialView("_UnreleasePO", Result);
            }
            else
            {
                return PartialView("_EmptyView");
            }
            
        }
        public ActionResult GetReleasePO(string PRJID, string PONumber, string VendorId)
        {
            
            var Result = objpro.GetReleasePOGST(PRJID, PONumber, VendorId);
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

           // string Proj_ID = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == Uid.Replace("#", "/")).FirstOrDefault().ProjectNo;
            string PO_ID = Uid.Replace("#", "/");
            List<POAllRemarks> res = null;
            //  res = objpro.GetAllPORemarks(Proj_ID, PO_ID);
            res = objpro.GetAllNewPORemarks_GST(PO_ID);
            if (res != null)
            {
                return PartialView("_Partial_POAllRemarks", res);
            }
            else {
                return View("_EmptyView");

            }

        }

        public ActionResult DoRelease(string UID)
        {
            // var Im = objmms.TbIndentPurchaseOrderMasters.Where(x => x.UId.ToString() == UID).First();
            var Im = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId.ToString() == UID).First();

            //var IApp = objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(y => y.Purchase_Order_Indent_No == Im.PurchaseOrderNo).First();

           

            Im.IsPORelease = Convert.ToBoolean(1); Im.POReleaseBy = EmpId; Im.POReleaseDate = System.DateTime.Now;
           // IApp.IsPORelease = Convert.ToBoolean(1); IApp.POReleaseBy = EmpId;IApp.POReleaseDate = System.DateTime.Now;
           objmms.Entry(Im).State = EntityState.Modified;
        //  objmms.Entry(IApp).State = EntityState.Modified;
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

            var a = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectNo,
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectNo).First().ProjectName
            });
            ViewBag.prjtid = t;


            string PurReq = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).FirstOrDefault().IndentRefNo;
            int? PurchaseTypeIdSel = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReq).FirstOrDefault().PurchasePIC_Type;
            ViewBag.PurDate = objmms.PurchaseRequisitionMasters.SingleOrDefault(x => x.PurRequisitionNo == PurReq).Date.Value;
            var units = "";
            if (id != null)
            {
                var querys = (from mas in objmms.TbIndentPurchaseOrder_GST
                              join chld in objmms.TbIndentPurchaseOrderChild_GST
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
                                  Vendor_country_state_city = objmms.tblCityLists.Where(k3 => k3.CityID == objmms.tblVendorMasters.Where(ct => ct.VendorID == mas.SupplierNo).FirstOrDefault().City).FirstOrDefault().CityName + ", " +
                                            objmms.tblStates.Where(k2 => k2.StateID == objmms.tblVendorMasters.Where(st => st.VendorID == mas.SupplierNo).FirstOrDefault().State).FirstOrDefault().StateName + ", " +
                                            objmms.tblCountries.Where(k1 => k1.CountryID == objmms.tblVendorMasters.Where(gr => gr.VendorID == mas.SupplierNo).FirstOrDefault().Country).FirstOrDefault().CnName,

                                  VendorTInNO = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().TinNo,
                                  VendorGSTNo = objmms.tblVendorMasters.Where(k=>k.VendorID == mas.SupplierNo).FirstOrDefault().GST_NO,
                                  AcilTinNo = objmms.tblProjectTINNoes.Where(k => k.ProjectId == mas.ProjectNo).FirstOrDefault().TINNo,
                                  AcilGSTNo = objmms.tblProjectGSTNoes.Where(k=>k.ProjectId == mas.ProjectNo).FirstOrDefault().GSTNo,
                                  MobileNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().MobileNo,
                                  ContactPerson = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().ContactPerson,
                                  Email = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Email,
                                  Reference = mas.Reference,

                                  IndentRefNo = mas.IndentRefNo,
                                  PurchaseOrderNo = mas.PurchaseOrderNo,

                                  SubTotal = chld.SubTotal1,
                                  Total = mas.Total,
                                  GrandTotal = mas.GrandTotal,
                                  ProjectNo = mas.ProjectNo,
                                  ProjectName = objmms.tblProjectMasters.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().ProjectName,
                                  Location = objmms.tblProjectParticularsDetailAs.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().Location,

                                  PurchaseOrderDate = mas.PurchaseOrderDate,
                                  ItemNo = objmms.tblItemMasters.Where(y => y.ItemID == chld.ItemNo).FirstOrDefault().GITEMCode,
                                  ItemName = objmms.tblItemMasters.Where(j => j.ItemID == chld.ItemNo).FirstOrDefault().ItemName,
                                  UnitID = objmms.tblItemMasters.Where(l => l.ItemID == chld.ItemNo).FirstOrDefault().UnitID,
                                  UnitName = objmms.tblUnitMasters.Where(l => l.UnitID == objmms.tblItemMasters.Where(ll => ll.ItemID == chld.ItemNo).FirstOrDefault().UnitID).FirstOrDefault().UnitCode,
                                  Amount = chld.Total,
                                  Discount = chld.Discount,
                                  Qty = chld.Qty,
                                  Rate = chld.NewRate,


                                  //  --------------------added Newely here

                                  Cartage_Type_1 = objmms.tblMMSCartageTypes.Where(x => x.TransId == chld.CartageTypeId).FirstOrDefault().CartageType,
                                  CR1_Total = chld.CartageAmount,
                                  T_Total = mas.Total,
                                  P_Total = chld.PackingChargesAmount,
                                  T_SubTotal1 = mas.SubTotal1,
                                  T_SubTotal2 = mas.SubTotal2,

                                  POCreatedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().LName,
                                  // UserType = UserType(mas.PurchaseOrderNo)
                                  UserType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == empId).FirstOrDefault().TypeName,
                                  CheckedPoLimit = mas.CheckedBeyondPOLimit,
                                  IsReleasePOBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().LName,
                                  PoApprovedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().FName +
                                                 " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().LName,

                                  // Added After GST.

                                  ItemToTAmt = chld.Total,
                                  SubTotal1 = chld.SubTotal1,
                                  SubTotal2 = chld.SubTotal2,
                                  PackagePercentage = chld.PackingChargesPercentage,
                                  PackingPercentageAmt = chld.PackingChargesAmount,
                                  CartageType = objmms.tblMMSCartageTypes.Where(x => x.TransId == chld.CartageTypeId).FirstOrDefault().CartageType,
                                  CartageTypeRate = chld.Cartage_rate,
                                  CartageAmt = chld.CartageAmount,
                                  InsurancePercentage = chld.InsuranceRate,
                                  InsurancePercentageAmt = chld.InsuranceAmount,
                                  TaxableAmount = chld.SubTotal2,
                                  CGSTPercentage = chld.CGSTPercentage,
                                  CGSTAmt = chld.CGSTAmount,
                                  SGSTAndUTGSTPercentage = chld.SGSTPercentage,
                                  SGSTAndUTGSTAmt = chld.SGSTAmount,
                                  IGST = chld.IGSTPercentage,
                                  IGSTAmt = chld.IGSTAmount,
                                  GrossAmount = chld.TotalGrosssAmount.HasValue ? chld.TotalGrosssAmount : chld.GrossAmount,
                                  HSNCode = objmms.tblItemMasters.Where(x => x.ItemID == chld.ItemNo).FirstOrDefault().HSNCode,
                                  TotalPackingSum = mas.Total_PAndF,
                                  TotalCartageSum = mas.Total_Cartage,
                                  TotalInsuranceSum = mas.Total_Insurance,
                                  TotalTaxableAmountSum = mas.SubTotal2,
                                  TotalCGSTSum = mas.Total_CGST,
                                  TotalSCGSTAndUTGSTSum = mas.Total_SGSTAndUTGST,
                                  IGSTSum = mas.Total_IGST,
                                  TotalAmountInWords = mas.Total_NetAmountInWords,
                                  ItemDescrp = chld.Item_Description,
                                  IsPORelease=mas.IsPORelease,
                                  PODescription=mas.PODescription,
                                  TCSRate = chld.TCS_Rate,
                                  TCSAmount = chld.TotalTCSAmount,
                                  TotalTCS = mas.Total_TCS,
                                  Tax_type = chld.TaxRateType,
                                  TCSActive = mas.TCSActive != null ? mas.TCSActive.Value : false
                              }).ToList();
                //var projtn=querys.Select(k=> k.ProjectNo == )
                var data = querys;
                Print_Slip_For_ApprovalController.PrintData dataObj = new Print_Slip_For_ApprovalController.PrintData();

                dataObj.HeaderData = data.Select(x => new Print_Slip_For_ApprovalController.HeaderData
                {
                    UId = x.UId,
                    PurchaseOrderNo = x.PurchaseOrderNo,
                    Total = Convert.ToDecimal(Math.Round((double)(x.Total != null ? x.Total : 0), 2).ToString("0.00")),
                    Location = x.Location,
                    ContactPerson = x.ContactPerson,
                    Vendor_country_state_city = x.Vendor_country_state_city,
                    VendorTInNO = x.VendorTInNO,
                    VendorGSTNo = x.VendorGSTNo,
                    MobileNo = x.MobileNo,
                    Reference = x.Reference,

                    Cartage = x.Cartage,
                    GrandTotal = Convert.ToDecimal(Math.Round((double)(x.GrandTotal != null ? x.GrandTotal : 0), 2).ToString("0.00")),

                    SubTotal = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                    PurchaseOrderDate = x.PurchaseOrderDate,
                    AcilTinNo = x.AcilTinNo,
                    AcilGSTNo = x.AcilGSTNo,
                    Address = x.Address,
                    Name = x.Name,
                    ProjectName = x.ProjectName,
                    IndentRefNo = x.IndentRefNo,
                    ExciseDutyRate = x.ExciseDutyRate,
                    ExciseDuty = x.ExciseDuty,

                    //Ex_Total = Math.Round((decimal)data.Sum(k => k.Ex_Total), 2),
                    Cartage_Type_1 = x.Cartage_Type_1,
                    CR1_Total = Convert.ToDecimal(Math.Round((decimal)data.Sum(k => k.CR1_Total), 2).ToString("0.00")),



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
                    TotalPackingSum = Convert.ToDecimal(Math.Round((double)(x.TotalPackingSum != null ? x.TotalPackingSum : 0), 2).ToString("0.00")),
                    TotalCartageSum = Convert.ToDecimal(Math.Round((double)(x.TotalCartageSum != null ? x.TotalCartageSum : 0), 2).ToString("0.00")),
                    TotalInsuranceSum = x.TotalInsuranceSum,
                    TotalTaxableAmountSum = Convert.ToDecimal(Math.Round((double)(x.TotalTaxableAmountSum != null ? x.TotalTaxableAmountSum : 0), 2).ToString("0.00")),
                    TotalCGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalCGSTSum != null ? x.TotalCGSTSum : 0), 2).ToString("0.00")),
                    TotalSCGSTAndUTGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalSCGSTAndUTGSTSum != null ? x.TotalSCGSTAndUTGSTSum : 0), 2).ToString("0.00")),
                    IGSTSum = Convert.ToDecimal(Math.Round((double)(x.IGSTSum != null ? x.IGSTSum : 0), 2).ToString("0.00")),
                    TotalAmountInWords = x.TotalAmountInWords,
                    OrderFor = x.PODescription,
                    Email = x.Email,
                    TotalTCSAmt = Convert.ToDecimal(Math.Round((double)(x.TotalTCS != null ? x.TotalTCS : 0), 2).ToString("0.00")),
                    TCSActive = x.TCSActive

                }).FirstOrDefault();

                foreach (Purchase_Order_Slip_VM x in data)
                {
                    Print_Slip_For_ApprovalController.ItemData itemobj = new Print_Slip_For_ApprovalController.ItemData();
                    itemobj.ItemName = x.ItemName;
                    itemobj.ItemNo = x.ItemNo;
                    itemobj.ItemDescrp = x.ItemDescrp;
                    itemobj.Remark = x.Remark;
                    itemobj.UnitName = x.UnitName;
                    itemobj.Rate = Convert.ToDecimal(Math.Round((double)(x.Rate != null ? x.Rate : 0), 2).ToString("0.00"));
                    itemobj.Discount = Convert.ToDecimal(Math.Round((double)(x.Discount != null ? x.Discount : 0), 2).ToString("0.00"));
                    itemobj.SubTotal1 = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00"));
                    itemobj.Qty = x.Qty;
                    itemobj.ItemToTAmt = Convert.ToDecimal(Math.Round((double)(x.ItemToTAmt != null ? x.ItemToTAmt : 0), 2).ToString("0.00"));
                    itemobj.SubTotal2 = Convert.ToDecimal(Math.Round((double)(x.SubTotal2 != null ? x.SubTotal2 : 0), 2).ToString("0.00"));
                    itemobj.PackagePercentage = Convert.ToDecimal(Math.Round((double)(x.PackagePercentage != null ? x.PackagePercentage : 0), 2).ToString("0.00"));
                    itemobj.PackingPercentageAmt = Convert.ToDecimal(Math.Round((double)(x.PackingPercentageAmt != null ? x.PackingPercentageAmt : 0), 2).ToString("0.00"));
                    itemobj.CartageType = x.CartageType;
                    itemobj.CartageTypeRate = Convert.ToDecimal(Math.Round((double)(x.CartageTypeRate != null ? x.CartageTypeRate : 0), 2).ToString("0.00"));
                    itemobj.CartageAmt = Convert.ToDecimal(Math.Round((double)(x.CartageAmt != null ? x.CartageAmt : 0), 2).ToString());
                    itemobj.InsurancePercentage = x.InsurancePercentage;
                    itemobj.InsurancePercentageAmt = x.InsurancePercentageAmt;
                    itemobj.TaxableAmount = Convert.ToDecimal(Math.Round((double)(x.TaxableAmount != null ? x.TaxableAmount : 0), 2).ToString("0.00"));
                    itemobj.CGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.CGSTPercentage != null ? x.CGSTPercentage : 0), 2).ToString("0.00"));
                    itemobj.CGSTAmt = Convert.ToDecimal(Math.Round((double)(x.CGSTAmt != null ? x.CGSTAmt : 0), 2).ToString("0.00"));
                    itemobj.SGSTAndUTGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTPercentage != null ? x.SGSTAndUTGSTPercentage : 0), 2).ToString("0.00"));
                    itemobj.SGSTAndUTGSTAmt = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTAmt != null ? x.SGSTAndUTGSTAmt : 0), 2).ToString("0.00"));
                    itemobj.IGST = Convert.ToDecimal(Math.Round((double)(x.IGST != null ? x.IGST : 0), 2).ToString("0.00"));
                    itemobj.IGSTAmt = Convert.ToDecimal(Math.Round((double)(x.IGSTAmt != null ? x.IGSTAmt : 0), 2).ToString("0.00"));
                    itemobj.GrossAmount = Convert.ToDecimal(Math.Round((double)x.GrossAmount, 2).ToString("0.00"));
                    itemobj.HSNCode = x.HSNCode;
                    itemobj.IsPORelease = x.IsPORelease;
                    itemobj.TCSRate = x.TCSRate.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSRate, 2).ToString("0.00")) : new decimal(0.00);
                    itemobj.TCSAmount = x.TCSAmount.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSAmount, 2).ToString("0.00")) : new decimal(0.00);
                    itemobj.TaxType = x.Tax_type;
                }

                dataObj.ItemData = data.Select(x => new Print_Slip_For_ApprovalController.ItemData
                {
                    ItemName = x.ItemName,
                    ItemNo = x.ItemNo,
                    ItemDescrp = x.ItemDescrp,
                    Remark = x.Remark,
                    UnitName = x.UnitName,
                    Rate = Convert.ToDecimal(Math.Round((double)(x.Rate != null ? x.Rate : 0), 2).ToString("0.00")),
                    Discount = Convert.ToDecimal(Math.Round((double)(x.Discount != null ? x.Discount : 0), 2).ToString("0.00")),
                    SubTotal1 = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                    Qty = x.Qty,
                    ItemToTAmt = Convert.ToDecimal(Math.Round((double)(x.ItemToTAmt != null ? x.ItemToTAmt : 0), 2).ToString("0.00")),
                    SubTotal2 = Convert.ToDecimal(Math.Round((double)(x.SubTotal2 != null ? x.SubTotal2 : 0), 2).ToString("0.00")),
                    PackagePercentage = Convert.ToDecimal(Math.Round((double)(x.PackagePercentage != null ? x.PackagePercentage : 0), 2).ToString("0.00")),
                    PackingPercentageAmt = Convert.ToDecimal(Math.Round((double)(x.PackingPercentageAmt != null ? x.PackingPercentageAmt : 0), 2).ToString("0.00")),
                    CartageType = x.CartageType,
                    CartageTypeRate = Convert.ToDecimal(Math.Round((double)(x.CartageTypeRate != null ? x.CartageTypeRate : 0), 2).ToString("0.00")),
                    CartageAmt = Convert.ToDecimal(Math.Round((double)(x.CartageAmt != null ? x.CartageAmt : 0), 2).ToString()),
                    InsurancePercentage = x.InsurancePercentage,
                    InsurancePercentageAmt = x.InsurancePercentageAmt,
                    TaxableAmount = Convert.ToDecimal(Math.Round((double)(x.TaxableAmount != null ? x.TaxableAmount : 0), 2).ToString("0.00")),
                    CGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.CGSTPercentage != null ? x.CGSTPercentage : 0), 2).ToString("0.00")),
                    CGSTAmt = Convert.ToDecimal(Math.Round((double)(x.CGSTAmt != null ? x.CGSTAmt : 0), 2).ToString("0.00")),
                    SGSTAndUTGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTPercentage != null ? x.SGSTAndUTGSTPercentage : 0), 2).ToString("0.00")),
                    SGSTAndUTGSTAmt = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTAmt != null ? x.SGSTAndUTGSTAmt : 0), 2).ToString("0.00")),
                    IGST = Convert.ToDecimal(Math.Round((double)(x.IGST != null ? x.IGST : 0), 2).ToString("0.00")),
                    IGSTAmt = Convert.ToDecimal(Math.Round((double)(x.IGSTAmt != null ? x.IGSTAmt : 0), 2).ToString("0.00")),
                    GrossAmount = Convert.ToDecimal(Math.Round((double)x.GrossAmount, 2).ToString("0.00")),
                    HSNCode = x.HSNCode,
                    IsPORelease=x.IsPORelease,
                    TCSRate = x.TCSRate.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSRate, 2).ToString("0.00")) : new decimal(0.00),
                    TCSAmount = x.TCSAmount.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSAmount, 2).ToString("0.00")) : new decimal(0.00),
                    TaxType = x.Tax_type
                }).ToList();


                ViewBag.PrintamountInWord = print.Cal(dataObj.HeaderData.GrandTotal ?? 0);


                if (PurchaseTypeIdSel == 3 || PurchaseTypeIdSel == 4 || PurchaseTypeIdSel == 5 || PurchaseTypeIdSel == 6 || PurchaseTypeIdSel == 7 || PurchaseTypeIdSel == 8)
                {
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
                                               Billing_Address = de_I.Billing_Address,
                                               StoreKeeperName = de_I.StoreKeeperName,
                                               StoreKeeperNo = de_I.StoreKeeperContact,
                                               EPPerson = objmms.tbl_ContactPersonMaster.Where(s => s.ContactPersonCode == "").Select(s => s.PersonName).FirstOrDefault(),
                                               EPContact = objmms.tbl_ContactPersonMaster.Where(s => s.ContactPersonCode == "").Select(s => s.ContactNumber).FirstOrDefault(),
                                               DivType = de_I.ContactDivType
                                           }).ToList();

                        var data_delvry = Specific_De;
                        ViewBag.Delevery_details = data_delvry;

                        if (Specific_De.Count > 0)
                        {
                            int? purDivison = Specific_De[0].DivType;
                            if (purDivison != null)
                            {
                                var officeAddr = objmms.tbl_OfficeAddress.Where(s => s.Division == purDivison).Select(s => s).FirstOrDefault();
                                ViewBag.OfficeAddress = officeAddr.OfficeAddress;
                                ViewBag.OfficeMobile = officeAddr.Mobile;
                                ViewBag.OfficeContactName = officeAddr.ContactPerson;
                            }
                        }
                    }

                    else
                    {
                    }
                    return PartialView("_ApprovedPOdetailOnIndentSection_ForOutPro", dataObj);

                }

                else if (PurchaseTypeIdSel == 1 || PurchaseTypeIdSel == 2 || PurchaseTypeIdSel == 9 || PurchaseTypeIdSel == 10 || PurchaseTypeIdSel == 11)
                {
                    #region

                    //for genral terms and conditions

                    if (id != null)
                    {
                        var Genral_T_and_C = (from gtcs in objmms.tblGenral_Terms_Conditions_Child

                                              where gtcs.Purchase_Order_No == id && gtcs.Status == "Enable" && gtcs.IsActive == "Yes"

                                              select new Genral_term_conditon_Chld
                                              {
                                                  GTC_Description = gtcs.GTC_Description
                                              }).ToList();
                        var data_gtc = Genral_T_and_C;
                        ViewBag.Gtc_details = data_gtc;
                    }
                    else
                    {
                    }


                    //for Specific Instructions 
                    if (id != null)
                    {
                        var Specific_I = (from Spe_I in objmms.tblSpecific_Instructions_Child

                                          where Spe_I.Purchase_Order_No == id && Spe_I.Status == "Enable" && Spe_I.IsActive == "Yes"
                                          select new Specific_Instructions_TC_Chld
                                          {
                                              SI_Description = Spe_I.SI_Description
                                          }).ToList();
                        var data_si = Specific_I;
                        ViewBag.Si_details = data_si;
                    }

                    else
                    {
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
                                               Billing_Address = de_I.Billing_Address,
                                               StoreKeeperName = de_I.StoreKeeperName,
                                               StoreKeeperNo = de_I.StoreKeeperContact,
                                               DivType = de_I.ContactDivType
                                               //EPPerson = objmms.tbl_ContactPersonMaster.Where(s => s.ContactPersonCode == de_I.EPContactPerID).Select(s => s.PersonName).FirstOrDefault(),
                                               //EPContact = objmms.tbl_ContactPersonMaster.Where(s => s.ContactPersonCode == de_I.EPContactPerID).Select(s => s.ContactNumber).FirstOrDefault()
                                           }).ToList();

                        var data_delvry = Specific_De;
                        ViewBag.Delevery_details = data_delvry;

                        if (Specific_De.Count > 0)
                        {
                            int? purDivison = Specific_De[0].DivType;
                            if (purDivison != null)
                            {
                                var officeAddr = objmms.tbl_OfficeAddress.Where(s => s.Division == purDivison).Select(s => s).FirstOrDefault();
                                ViewBag.OfficeAddress = officeAddr.OfficeAddress;
                                ViewBag.OfficeMobile = officeAddr.Mobile;
                                ViewBag.OfficeContactName = officeAddr.ContactPerson;
                            }
                        }
                    }

                    else
                    {
                    }





                    //for Specific  terms and conditions
                    if (id != null)
                    {
                        var Specific_T_and_C = (from SpecificT_C in objmms.tblSpecificTermsAndConditions_Child

                                                where SpecificT_C.Purchase_Order_No == id && SpecificT_C.Status == "Enable" && SpecificT_C.IsActive == "Yes"
                                                select new Specific_Termss_And_conditions_Chld
                                                {
                                                    STC_Description = SpecificT_C.STC_Description
                                                }).ToList();
                        var data_stc = Specific_T_and_C;
                        ViewBag.Stc_details = data_stc;
                    }
                    else
                    {
                    }

                    return PartialView("_ApprovedPOdetailOnIndentSection", dataObj);


                    #endregion
                }
                else {
                    return Json(null, JsonRequestBehavior.AllowGet);

                }

                

            }
            else {



                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CreateMannualPurchaseOrder(string PI)
        {
            ViewBag.PI_reqNo = PI.Replace("#", "/");
            string empId = Session["EmpID"].ToString();

            string prjId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PI.Replace("#", "/")).FirstOrDefault().ProjectNo;

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

           // ViewBag.prjtid = new SelectList(objmms.tblProjectMasters.Where(x=>!x.ProjectName.Equals("Admin") && !x.ProjectName.Equals("Demo") && !x.ProjectName.Equals(projectName)), "PRJID", "ProjectName");

            //  string PRJID = m.GetProjectMasterManager().Find(SiteId).PRJID;



            int? PurchasePiTypeForOutPi = a.FirstOrDefault().PurchasePIC_Type;

            if (PurchasePiTypeForOutPi >= 3)
            {
                var EmpOutPITYpeLst = (from b in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(prjId) && x.EmpID != EmpId).ToList()
                                       join a1 in objmms.tblApproval_AccountType.Where(x1 => (x1.TypeName == "PIC" || x1.TypeName == "Mang") && x1.EmployeeId != EmpId)
                                       on b.EmpID equals a1.EmployeeId
                                       select new { b }).ToList();

                List<DAL.tblEmployeeMaster> HL = (from Z in EmpOutPITYpeLst select Z.b).ToList();
                ViewBag.Employee = HL.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName }).ToList();
                ViewBag.PMCEmployee = HL.Select(p => new SelectListItem { Value = p.EmpID, Text = p.FName + ' ' + p.LName }).ToList();


            }



            var vGlst = objmms.tblRegistrationTypes.Where(x => x.TypeID == "TID0000029").ToList();

            ViewBag.VGId = new SelectList(vGlst, "TypeId", "RegistrationType");

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


            return View();
        }

        [HttpPost]
        public ActionResult AddMannualTransferPO(IndentPurchaserOrderNew_GST Grid)
        {
            decimal? MaxUid = 0;

            try
            {


                if (EmpId != null)
                {



                    if (Grid.Child.Count() > 0)
                    {

                        #region
                        Grid.Master.PurchaseOrderNo = Get_PurchaseOrderNo(Grid.Master.ProjectNo, Grid.Master.IndentRefNo, Grid.Master.PurchaseOrderDate.Value);

                        var PoNo = objmms.TbIndentPurchaseOrder_GST.FirstOrDefault(x => x.PurchaseOrderNo == Grid.Master.PurchaseOrderNo);

                        if (PoNo != null)
                        {
                            return Json(new { Status = "6" }, JsonRequestBehavior.AllowGet); //Data is already exist.
                        }
                        else {






                            MaxUid = findUidForNewRow_GST();
                            if (MaxUid != 0)
                            {
                                tblALLRemark alrm = new tblALLRemark();

                                string VarPurchaseReqNo = Grid.Master.IndentRefNo;
                                string poindentNo = Grid.Master.PurchaseOrderNo; string Purreq = string.Empty;



                                Grid.Master.CreatedBy = EmpId;
                                Grid.Master.Status = "Pending";
                                Grid.Master.Edited_Status = "No";
                                Grid.Master.Vendor_Group_Id = Grid.Master.Vendor_Group_Id;
                                Grid.Master.CreatedDate = System.DateTime.Now;
                                Grid.Master.DivisionId = Repos.GetUserDivision();
                                Grid.Master.PurchaseOrderId = FindMaxId_GST(Grid.Master.ProjectNo);
                                Grid.Master.Session_Year = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.PurchaseOrderDate));
                                int? intPurchasePIType = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == VarPurchaseReqNo).FirstOrDefault().PurchasePIC_Type;
                                if (intPurchasePIType >= 3)
                                {

                                    Grid.Master.POType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == intPurchasePIType).FirstOrDefault().Purchase_Code;
                                    Grid.Master.IndentRefNo = Purreq = VarPurchaseReqNo;
                                }


                                var CheckedPoLimit = Grid.Master.CheckedBeyondPOLimit;
                                Grid.Master.SecondLevelApprv_Id = Grid.Master.SendTo;


                                alrm.CreatedDate = System.DateTime.Now;
                                alrm.RemarkBy = EmpId;
                                alrm.RemarkDate = System.DateTime.Now;
                                alrm.Remarks = Grid.Master.Remarks;
                                if (objmms.tblApproval_AccountType.Where(e => e.EmployeeId == EmpId).ToList().Count() > 0)
                                {
                                    alrm.RemarkStage = objmms.tblApproval_AccountType.Where(e => e.EmployeeId == EmpId).FirstOrDefault().TypeName;
                                }
                                else
                                {
                                    alrm.RemarkStage = "EmpType";
                                }

                                alrm.PurchaseOrderNo = Grid.Master.PurchaseOrderNo;


                                decimal? OutproappLimit = objmms.tblOutProApprovalLimits.Where(d => d.ProjectId == Grid.Master.ProjectNo && d.PurchaseTypeId == intPurchasePIType).FirstOrDefault().LimitValue;

                                if (OutproappLimit < Grid.Master.GrandTotal)
                                {
                                    return Json(new { Status = "7" }, JsonRequestBehavior.AllowGet); // crossed limit
                                }
                                else {


                                    objmms.TbIndentPurchaseOrder_GST.Add(Grid.Master);
                                    objmms.tblALLRemarks.Add(alrm);


                                    if (Grid.Child != null)
                                        foreach (var x in Grid.Child)
                                        {

                                            x.MUId = Convert.ToDecimal(MaxUid);
                                            x.Receive = 0;

                                            objmms.TbIndentPurchaseOrderChild_GST.Add(x);

                                            if (x.IndentId != 0 && x.Qty != 0)
                                            {
                                                DAL.PurchaseRequisitionChild a = objmms.PurchaseRequisitionChilds.Where(d => d.UId == x.IndentId).First();

                                                if (a.OrderedQty == null)
                                                {
                                                    a.OrderedQty = 0;
                                                }
                                                else
                                                {
                                                    a.OrderedQty = a.OrderedQty + x.Qty;
                                                }

                                                objmms.Entry(a).State = EntityState.Modified;



                                            }
                                            else
                                            {
                                                break;
                                            }


                                        }






                                    objmms.SaveChanges();



                                    // after adding p.O then P.I complete will be updated.
                                    m.GetPurchaseRequisitionMasterManager().FindCompleteO(Grid.Master.IndentRefNo);
                                    return Json(new { Status = "1", TransNo = Grid.Master.PurchaseOrderNo }, JsonRequestBehavior.AllowGet);
                                }

                            }
                            else
                            {
                                return Json(new { Status = "4" }, JsonRequestBehavior.AllowGet);

                            }





                        }
                        #endregion

                    }
                    else
                    {

                        return Json(new { Status = "3" }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new { Status = "5" }, JsonRequestBehavior.AllowGet); // Session has Expired. Please Re-login or Re-Load Page.
                }

            }
            catch (Exception ex)
            { ex.ToString(); return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet); }
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


        #region Get Purchase Order No 7_3_2020
        public string Get_PurchaseOrderNo(string ProjID, string PurReqNo, DateTime PODate)
        {
            string PONo = objpro.GetPO_NO_New(ProjID, PurReqNo, PODate);
            return PONo;
        }
        #endregion


        #region Save PO Draft
        public ActionResult ADD_GSTPODraft(IndentPurchaserOrderDraft_GST Grid, List<PI_AttachFileModel> AttachModel)
        {
            decimal? MaxUid = 0; int Pre_count = 0; int Post_Count = 0;
            MSP_Model objmsp = new MSP_Model();
            using (var transaction = objmms.Database.BeginTransaction())
            {
                try
                {
                    if (EmpId != null)
                    {

                        var dataToCheck = objmms.PurchaseRequisitionChilds.Where(x => x.PurRequisitionNo == Grid.Master.IndentRefNo).Select(x => new { SNo = x.SNo, ItemNo = x.ItemNo, BalanceQty = (x.ApprovedQty - x.OrderedQty) }).ToList();

                        foreach (var item in Grid.Child)
                        {
                            var isGreaterQty = dataToCheck.Any(x => x.ItemNo == item.ItemNo && x.SNo == item.UId && item.Qty > x.BalanceQty);
                            if (isGreaterQty)
                            {
                                return Json(new { Status = "9", Message = "Qty can not be greater than remaining qty. Please refresh the page." }, JsonRequestBehavior.AllowGet);
                            }

                            if (item.Rate.HasValue || item.NewRate.HasValue)
                            {

                            }
                            else
                            {
                                return Json(new { Status = "9", Message = "Rate can not be null" }, JsonRequestBehavior.AllowGet);
                            }

                            item.Rate = item.Rate.HasValue ? item.Rate : item.NewRate;

                        }

                        DateTime? indentdate = objmms.PurchaseRequisitionMasters.Where(s => s.PurRequisitionNo == Grid.Master.IndentRefNo).Select(s => s.Date).FirstOrDefault();

                        if (indentdate.Value > Grid.Master.PurchaseOrderDate.Value)
                        {
                            return Json(new { Status = "101", Message = "Purchase Order date must be greater than purchase indent date." }, JsonRequestBehavior.AllowGet);
                        }

                        if (Grid.Child.Count() > 0)
                        {
                            if (Grid.Master.PurchaseOrderId == 0 && Grid.Master.PurchaseOrderId == null)
                            {
                                Grid.Master.PurchaseOrderNo = Get_PurchaseOrderNo(Grid.Master.ProjectNo, Grid.Master.IndentRefNo, Grid.Master.PurchaseOrderDate.Value);
                            }
                            #region
                            var PoNo = objmms.TbIndentPurchaseOrder_GST.FirstOrDefault(x => x.PurchaseOrderNo == Grid.Master.PurchaseOrderNo);

                            if (PoNo != null)
                            {
                                return Json(new { Status = "6" }, JsonRequestBehavior.AllowGet); //Data is already exist.
                            }
                            else
                            {
                                MaxUid = 1;
                                if (MaxUid != 0)
                                {

                                    tblALLRemark alrm = new tblALLRemark();

                                    string VarPurchaseReqNo = Grid.Master.IndentRefNo;
                                    string poindentNo = Grid.Master.PurchaseOrderNo; string Purreq = string.Empty;

                                    // Grid.Master.SessionId = SessionId;
                                    Grid.Master.CreatedBy = EmpId;
                                    Grid.Master.Status = "Pending";
                                    Grid.Master.Edited_Status = "No";
                                    //Grid.Master.Session_Year = objmsp.GetFinancialYear();
                                    Grid.Master.Session_Year = GetCurrentFinancialYear(Convert.ToDateTime(Grid.Master.PurchaseOrderDate));

                                    // Grid.Master.Vendor_Group_Id = Grid.Master.Vendor_Group_Id;
                                    Grid.Master.CreatedDate = System.DateTime.Now;
                                    //Grid.Master.PurchaseOrderId = m.GetTbIndentPurchaseOrderMasterManager().FindMaxId(SessionId, Grid.Master.ProjectNo);
                                    //if (Grid.Master.PurchaseOrderId == 0 && Grid.Master.PurchaseOrderId == null)
                                    //{
                                    Grid.Master.PurchaseOrderId = FindMaxId_GST(Grid.Master.ProjectNo);
                                    // }
                                    int? intPurchasePIType = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == VarPurchaseReqNo).FirstOrDefault().PurchasePIC_Type;
                                    if (intPurchasePIType >= 3)
                                    {

                                        Grid.Master.POType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == intPurchasePIType).FirstOrDefault().Purchase_Code;
                                        Grid.Master.IndentRefNo = Purreq = VarPurchaseReqNo;
                                    }
                                    else
                                    {

                                        //string[] IndentNo = Grid.Master.IndentRefNo.Split('/');
                                        // Grid.Master.IndentRefNo = Purreq = IndentNo[0].ToString() + "/" + IndentNo[1].ToString() + "/" + IndentNo[2].ToString() + "/" + IndentNo[3].ToString()+"/"+ IndentNo[4].ToString();

                                        Grid.Master.POType = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Grid.Master.IndentRefNo).FirstOrDefault().PurchasePIC_Type == Convert.ToInt32(1) ? "IPO" : "LP";
                                    }

                                    Grid.Master.DivisionId = Repos.GetUserDivision();

                                    Grid.Master.Edited_Status = "No";
                                    var CheckedPoLimit = Grid.Master.CheckedBeyondPOLimit;
                                    if (CheckedPoLimit == Convert.ToBoolean(1))
                                    { Grid.Master.SecondLevelApprv_Id = Grid.Master.SendTo; }
                                    else { Grid.Master.FirstLevelApprv_Id = Grid.Master.SendTo; }

                                    alrm.CreatedDate = System.DateTime.Now;
                                    alrm.RemarkBy = EmpId;
                                    alrm.RemarkDate = System.DateTime.Now;
                                    alrm.Remarks = Grid.Master.Remarks;
                                    if (objmms.tblApproval_AccountType.Where(e => e.EmployeeId == EmpId).ToList().Count() > 0)
                                    {
                                        alrm.RemarkStage = objmms.tblApproval_AccountType.Where(e => e.EmployeeId == EmpId).FirstOrDefault().TypeName;
                                    }
                                    else
                                    {
                                        alrm.RemarkStage = "EmpType";
                                    }

                                    alrm.PurchaseOrderNo = Grid.Master.PurchaseOrderNo;

                                    Grid.Master.GrandTotal = Grid.Master.GrandTotal ?? 0;
                                    Grid.Master.SubTotal1 = Grid.Master.SubTotal1 ?? 0;
                                    Grid.Master.SubTotal2 = Grid.Master.SubTotal2 ?? 0;
                                    Grid.Master.Total = Grid.Master.Total ?? 0;

                                    Grid.Master.Total_Cartage = Grid.Master.Total_Cartage ?? 0;
                                    Grid.Master.Total_CGST = Grid.Master.Total_CGST ?? 0;
                                    Grid.Master.Total_IGST = Grid.Master.Total_IGST ?? 0;
                                    Grid.Master.Total_Insurance = Grid.Master.Total_Insurance ?? 0;
                                    Grid.Master.Total_PAndF = Grid.Master.Total_PAndF ?? 0;
                                    Grid.Master.Total_SGSTAndUTGST = Grid.Master.Total_SGSTAndUTGST ?? 0;
                                    Grid.Master.Total_Taxable = Grid.Master.Total_Taxable ?? 0;
                                    Grid.Master.TCSActive = true;

                                    objmms.TbDraftIndentPurchaseOrder_GST.Add(Grid.Master);
                                    objmms.tblALLRemarks.Add(alrm);

                                    objmms.SaveChanges();

                                    MaxUid = Grid.Master.UId;


                                    Pre_count = Grid.Child.Count();

                                    if (Grid.Child != null)
                                        foreach (var x in Grid.Child)
                                        {

                                            x.MUId = Convert.ToDecimal(MaxUid);
                                            x.Receive = 0;

                                            if (x.Qty == 0)
                                            {
                                                //x.MUId = 0;
                                                //x.OldMId = Convert.ToInt32(MaxUid);
                                            }

                                            x.GrossAmount = x.GrossAmount ?? 0;

                                            //if (x.Rate != null && x.GrossAmount != null)
                                            //{
                                            decimal ex_itm_per = Convert.ToDecimal(objmms.tblItemMasters.Where(itm => itm.ItemID == x.ItemNo).FirstOrDefault().Excess_Item_Percentage);
                                            decimal ex_itm_qnty = Convert.ToDecimal((x.Qty * ex_itm_per) / 100);
                                            x.Item_ExcessPercentage = ex_itm_per;
                                            x.Item_ExcessQuantity = ex_itm_qnty;

                                            Post_Count = Post_Count + 1;

                                            objmms.TbDraftIndentPurchaseOrderChild_GST.Add(x);

                                            if (x.IndentId != 0)
                                            {
                                                //DAL.PurchaseRequisitionChild a = objmms.PurchaseRequisitionChilds.Where(d => d.UId == x.IndentId).First();

                                                //if (a.OrderedQty == null)
                                                //{
                                                //    a.OrderedQty = 0;
                                                //}
                                                //else
                                                //{
                                                //    a.OrderedQty = a.OrderedQty + x.Qty;
                                                //}

                                                //objmms.Entry(a).State = EntityState.Modified;

                                            }
                                            else
                                            {
                                                break;
                                            }
                                            //}
                                            //else
                                            //{
                                            //    string selecteditemname = objmms.tblItemMasters.Where(w => w.ItemID == x.ItemNo).FirstOrDefault().ItemName;
                                            //    return Json(new { Status = "7" + selecteditemname + " " }, JsonRequestBehavior.AllowGet);

                                            //}


                                        }

                                    if (Grid.Child2 != null)
                                        foreach (var x in Grid.Child2)
                                        {

                                            x.CreatedBy = EmpId;
                                            x.CreatedDate = System.DateTime.Now;
                                            x.Edited_Status = "No";
                                            x.Status = "Enable";
                                            x.IsActive = "Yes";
                                            x.Purchase_Order_No = Grid.Master.PurchaseOrderNo;
                                            objmms.tblDraftSpecificTermsAndConditions_Child.Add(x);


                                        }

                                    if (Grid.Child3 != null)
                                        foreach (var x in Grid.Child3)
                                        {
                                            x.CreatedBy = EmpId;
                                            x.CreatedDate = System.DateTime.Now;
                                            x.Edited_Status = "No";
                                            x.Status = "Enable";
                                            x.IsActive = "Yes";
                                            x.Purchase_Order_No = Grid.Master.PurchaseOrderNo;
                                            objmms.tblDraftSpecific_Instructions_Child.Add(x);
                                        }
                                    if (Grid.Child4 != null)
                                        foreach (var x in Grid.Child4)
                                        {
                                            x.CreatedBy = EmpId;
                                            x.CreatedDate = System.DateTime.Now;
                                            x.Edited_Status = "No";
                                            x.Status = "Enable";
                                            x.IsActive = "Yes";
                                            x.Purchase_Order_No = Grid.Master.PurchaseOrderNo;
                                            objmms.tblDraftGenral_Terms_Conditions_Child.Add(x);


                                        }


                                    Grid.Child5.Status = "Valid";
                                    Grid.Child5.CreatedDate = DateTime.Now;
                                    Grid.Child5.CreatedBy = EmpId;
                                    Grid.Child5.EditStatus = "No";
                                    Grid.Child5.CompanyId = Session["CompanyId"].ToString();
                                    Grid.Child5.Purchase_order_No = Grid.Master.PurchaseOrderNo;
                                    objmms.TbDraftDelivery_Details_PO.Add(Grid.Child5);


                                    if (Pre_count != 0 && Post_Count != 0)
                                    {
                                        if (Pre_count == Post_Count)
                                        {
                                            if (AttachModel != null)
                                            {
                                                foreach (PI_AttachFileModel attachFile in AttachModel)
                                                {
                                                    objmms.tbl_PurchaseOrderAttachFiles.Add(new DAL.tbl_PurchaseOrderAttachFiles() { AttachFile = attachFile.AttachFile, AttachFile_Name = attachFile.FileName, PONo = Grid.Master.PurchaseOrderNo, CreatedOn = DateTime.Now });
                                                }
                                            }


















































                                            objmms.SaveChanges();
                                            transaction.Commit();
                                            return Json(new { Status = "1", TransNo = Grid.Master.PurchaseOrderNo }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            return Json(new { Status = "8" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }

                                    if (AttachModel != null)
                                    {
                                        foreach (PI_AttachFileModel attachFile in AttachModel)
                                        {
                                            objmms.tbl_PurchaseOrderAttachFiles.Add(new DAL.tbl_PurchaseOrderAttachFiles() { AttachFile = attachFile.AttachFile, AttachFile_Name = attachFile.FileName, PONo = Grid.Master.PurchaseOrderNo, CreatedOn = DateTime.Now });
                                        }
                                    }


                                    objmms.SaveChanges();

                                    // after adding p.O then P.I complete will be updated.
                                    // m.GetPurchaseRequisitionMasterManager().FindCompleteO(Grid.Master.IndentRefNo);
                                    return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);

                                }
                                else
                                {
                                    return Json(new { Status = "4" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            #endregion

                        }
                        else
                        {

                            return Json(new { Status = "3" }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        transaction.Rollback();
                        return Json(new { Status = "5" }, JsonRequestBehavior.AllowGet); // Session has Expired. Please Re-login or Re-Load Page.
                    }

                }
                catch (Exception ex)
                {
                    ex.ToString(); transaction.Rollback();
                    return Json(new { Status = "2", IndentNo = "", Error = ex.Message + "\n" + ex.StackTrace }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public int FindMaxId_GSTDraft(string PjId)
        {
            int K = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.ProjectNo == PjId).Select(i => i.PurchaseOrderId).DefaultIfEmpty(0).Max(i => i).Value;
            if (K == 0)
            {
                return 1;
            }
            else
            {
                return K + 1;
            }


        }


        #endregion
    }
}