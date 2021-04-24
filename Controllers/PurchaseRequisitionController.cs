using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Entity;
using BusinessLogicLayer;
using MMS_P.ViewModels;
using DataAccessLayer;
using System.Globalization;
using MMS.Models;
using MMS.ViewModels;
using MMS.Helpers;

namespace MMS.Controllers
{
    public class PurchaseRequisitionController : Controller
    {
        private Model1 db = new Model1();
        FactoryManager m = new FactoryManager();
        List<PurchaseRequisitionMaster> MasterList;
        MSP_Model objmsp = new MSP_Model();
        Procedure objpro = new Procedure();
        DAL.MMSEntities objmms = new DAL.MMSEntities();
        int SessionId = 1;
        int SiteId = 1;
        string EmpId = string.Empty;
       string EmpName = string.Empty;
        // GET: PurchaseRequisition
        public PurchaseRequisitionController()
        {
            try
            {
                EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
                EmpName = System.Web.HttpContext.Current.Session["Emp_Name"].ToString();
                SessionId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SessionId"].ToString());
                SiteId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteId"].ToString());
            }
            catch
            {
            }
        }
        public JsonResult GetProjectByEmpId(string E = "0")
        {
            string D = null;

            if (E != "0" && E != "EMP0000001" && E != "")
            {
                string PrjString = m.GetEmployeeMasterManager().FindPrj(E);


                var PrjlistD = m.GetProjectMasterManager().FindListByString(PrjString);

                D = PrjlistD.ToJSON();
            }
            else
            {
                var Prjlist = m.GetProjectMasterManager().FindListByString(null);

                D = Prjlist.ToJSON();
            }
            var v = new { List1 = D };

            string jsonString = v.ToJSON();

            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPurchaseIRNo(string ProjectID)
        {
            List<tblEmployeeMaster> E1 = m.GetEmployeeMasterManager().FindByProject(ProjectID);

            List<tblEmployeeMaster> E = (from a in E1 join b in objmms.tblApproval_AccountType.Where(p=>p.TypeName== "PIC") on a.EmpID equals b.EmployeeId select a).ToList();


            var EO = E.Select(a => new { Value = a.EmpID, Text = a.FName +' '+ a.LName });

             var v = new { List = EO, Message = "N/A" };

            string jsonString = v.ToJSON();

            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseIRnoDateWise(string ProjectId, string PurDate)
        {
            string Fynm = GetCurrentFinancialYear(Convert.ToDateTime(PurDate));

            var SessionId = objmms.SessionMasters.Where(x => x.Name == Fynm.ToString()).FirstOrDefault().Id;

            var sessioncode = m.GetSessionMasterManager().Find(SessionId).Name;
             //var idMax = m.GetPurchaseRequisitionMasterManager().FindMaxId(SessionId.ToString(), ProjectId);
            var sessionIDString = SessionId.ToString();

            var divisionId = Repos.GetUserDivision();

            var idMax = 0;
            int a = objmms.PurchaseRequisitionMasters.Where(i =>i.DivisionId==divisionId && i.SessionId == sessionIDString && i.ProjectNo == ProjectId && i.PurRequisitionNo.Contains("PurchaseIR")).Select(i => i.Id).DefaultIfEmpty(0).Max(i => i).Value;

            idMax=a == 0 ? 1 : a + 1;

            string Code = m.GetProjectMasterManager().FindCode(ProjectId);
            string ProjectCode = "";
            string DivisionCode = objmms.tblDivisionMasters.SingleOrDefault(x => x.Id == divisionId).DivisionCode;
            ProjectCode = "PurchaseIR/" + Code + "/"+DivisionCode+"/" + sessioncode + "/" + idMax;

            string last_poNo = "";
            string last_poDate = "";
            var last_po = objmms.PurchaseRequisitionMasters.Where(i => i.DivisionId == divisionId && i.SessionId == sessionIDString && i.ProjectNo == ProjectId && i.PurRequisitionNo.Contains("PurchaseIR")).OrderByDescending(s => s.UId).Select(s => s).FirstOrDefault();
            if (last_po != null)
            {
                last_poDate = last_po.Date.Value.ToString("dd/MM/yyyy");
                last_poNo = last_po.PurRequisitionNo;
            }

            var lst = new { PrNO = ProjectCode, Last_PODate = last_poDate, Last_PONo = last_poNo };
            return Json(lst.ToJSON(), JsonRequestBehavior.AllowGet);

        }


        public Tuple<string,int> GetPurchaseIRNOByDate(string ProjectId, DateTime PurDate)
        {
            string Fynm = GetCurrentFinancialYear(PurDate);

            var SessionId = objmms.SessionMasters.Where(x => x.Name == Fynm.ToString()).FirstOrDefault().Id;

            var sessioncode = m.GetSessionMasterManager().Find(SessionId).Name;
            //var idMax = m.GetPurchaseRequisitionMasterManager().FindMaxId(SessionId.ToString(), ProjectId);
            var sessionIDString = SessionId.ToString();

            var divisionId = Repos.GetUserDivision();

            var idMax = 0;
            int a = objmms.PurchaseRequisitionMasters.Where(i =>i.DivisionId==divisionId &&  i.SessionId == sessionIDString && i.ProjectNo == ProjectId && i.PurRequisitionNo.Contains("PurchaseIR")).Select(i => i.Id).DefaultIfEmpty(0).Max(i => i).Value;

            idMax = a == 0 ? 1 : a + 1;

            string Code = m.GetProjectMasterManager().FindCode(ProjectId);
            string DivisionCode = objmms.tblDivisionMasters.SingleOrDefault(x=>x.Id==divisionId).DivisionCode;
            string ProjectCode = "";

            ProjectCode = "PurchaseIR/" + Code +"/"+DivisionCode+ "/" + sessioncode + "/" + idMax;
            //var lst = new { PrNO = ProjectCode };
            return new Tuple<string, int>(ProjectCode,idMax);

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
        public JsonResult GetItemByGroup(string id)
        {
            if (id != null)
            {
                object ItemMasters = objmms.tblItemMasters.Where(x => x.ItemGroupID == id).Select(x => new { Text = x.ItemName, Value = x.ItemID }).OrderBy(x=>x.Text).ToList();
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetPIItemByGroup(string id)
        {
            if (id != null)
            {
                object ItemMasters = objmms.tblItemMasters.Where(x => x.ItemGroupID == id && x.Disable == null).Select(x => new { Text = x.ItemName, Value = x.ItemID }).OrderBy(x => x.Text).ToList();
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetItemByGroup1(string id)
        {
            if (id != null)
            {
                object ItemMasters = objmms.tblItemMasters.Where(x=>x.ItemGroupID==id).Select(x=>new {Text=x.ItemName,Value=x.GITEMCode}).OrderBy(x => x.Text).ToList();
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetItemByGroupStock(string Pid, string Gid)
        {
            if (Pid != null && Gid != null)
            {
                List<Listobj> ItemMasters = m.GettblItemCurrentStockManager().FindByItemGroup(Pid, Gid);
                Session["Items"] = ItemMasters;
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetItemCode(string ItemId)
        {
            if (ItemId!="")
            {
                return Json(new { ItemCode=objmms.tblItemMasters.SingleOrDefault(x=>x.ItemID==ItemId).GITEMCode}, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetItemCodes(string Id)
        {
            if (Id != "")
            {
                return Json(objmms.tblItemMasters.Where(x => x.ItemGroupID == Id && x.Disable==null).OrderBy(x => x.ItemName).Select(x => new { Text = x.GITEMCode, Value = x.ItemID }).ToList().ToJSON(), JsonRequestBehavior.AllowGet);
            }
            return null;
        }



        public JsonResult GetItemDetail(string id, string PrjId = null)
        {
            if (id != "" && id != "0")
            {
                tblItemMaster Item = m.GetGobalItemManager().Find(id);
                string u = null;
                string un = null;
                try
                {
                    u = Item.UnitMasterPurchase.UnitName;
                    un = Item.UnitMasterPurchase.UnitID;
                }
                catch { }
                var Pid = Session["ProjectssId"].ToString();
                decimal IR = 0; decimal ? Totalrecv = 0; decimal ? Totalbalancd = 0;
                var IRate = objmms.tblReceiveDatas.Where(b => b.ProjectId == Pid && b.ItemId == id).OrderByDescending(c => c.TransId).Take(1).ToList();
                Totalrecv = objmms.tblReceiveDatas.Where(b => b.ProjectId == Pid && b.ItemId == id).Sum(x => x.ReceiveQuantity) ?? 0;
                Totalbalancd = objmms.tblReceiveDatas.Where(b => b.ProjectId == Pid && b.ItemId == id).Sum(x => x.BalanceQuantity) ?? 0;

                if (IRate.Count() > 0)
                {
                    var IR1 = objmms.tblReceiveDatas.Where(b => b.ProjectId == Pid && b.ItemId == id).OrderByDescending(c => c.TransId).Take(1).ToList().Select(d => d.Rate).First();
                    IR = Convert.ToDecimal(IR1);
                }
                else
                {
                    IR = 0;
                }

                var v = new { Make = Item.Make, PartNo = Item.PartNo, Unit = u, UnitNo = un, LPRate = IR, CRate = IR , TotRecv = Totalrecv , TotBalncd =Totalbalancd };
                string jsonString = v.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
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
        public ActionResult IndexNew(string PId = "")
        {
             ViewBag.EmpId = EmpId;
            ViewBag.PId = PId;
            ViewBag.Date = DateTime.Now.Date;

            var list = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList();
            if (list.Count() > 0)
            { ViewBag.TypeSelect = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).First().TypeName; }
            else { ViewBag.TypeSelect = "User"; }


            return View();
        }

        public ActionResult PendingPIGrid(string PRJID,string PINo)
        {
            var divisionId = Repos.GetUserDivision();
            var Result = objmsp.GetAllPendingPI(PRJID,EmpId,PINo,divisionId);
            if (Result != null)
            {
               // var d = objmms.tblApproval_AccountType.SingleOrDefault(x => x.EmployeeId == EmpId);
                //if (d!=null && d.TypeName== "Purchase")
                //{
                //    return PartialView("_PendingPIView", Result);
                //}

                return PartialView("_Pending_PI", Result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
         
        }




        public ActionResult ApprovedPIGrid(string PRJID, string PINo)
        {
            var divisionId = Repos.GetUserDivision();
            var Result = objmsp.GetAllApprovedPI(PRJID,EmpId, PINo,divisionId);
           
            if (Result != null)
            {
                return PartialView("_Approved_PI", Result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

        public ActionResult IndexApproval()
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
        public ActionResult Grid2(string Search, string PrjId = null, string f = "From", string E = null, string Comp = "InComplete", DateTime? fromDate = null, DateTime? toDate = null, int page = 1)
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
                totalRows = m.GetPurchaseRequisitionMasterManager().FindByDateOrLastCount(Search, SessionId.ToString(), f, PrjId, E, fromDate, toDate, Comp);
                MasterList = m.GetPurchaseRequisitionMasterManager().FindByDateOrLast(Search, SessionId.ToString(), f, PrjId, E, fromDate, toDate, Comp, page, pageSize);

                if (MasterList != null)
                {
                    var data = new PagedPurchaseIRMasterModel()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        Master = MasterList.ToList()
                    };
                    if (Request.IsAjaxRequest())
                    {
                        if (f == "From")
                        {
                            return View("_GridView", data);
                        }
                        else if (f == "To")
                        {
                            if (Comp == "InComplete")
                            {
                                return View("_GridViewA", data);
                            }
                            else if (Comp == "Complete")
                            {
                                return View("_GridViewA1", data);
                            }
                        }
                    }
                }
            }
            return View("_EmptyView");
        }
        public ActionResult Create()
        {
            ViewBag.EmpId = EmpId;
            //Item Group Name

            ViewBag.OrderDate = DateTime.Now.Date;

            ViewBag.ItemGroup = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");

            return View();
        }
        public ActionResult CreatePI()
        {
            ViewBag.EmpId = EmpId;
            //Item Group Name

            ViewBag.OrderDate = DateTime.Now.Date;
            //ViewBag.LastPIDate= DateTime ? lastPI = objmms.PurchaseRequisitionMasters.Where(s => s.ProjectNo == Grid.MasterNew.ProjectNo).OrderByDescending(s => s.UId).Select(s => s.Date).FirstOrDefault();
            ViewBag.ItemGroup = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");

            return View();
        }

        public ActionResult PIPrint(decimal id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisitionMaster Master = m.GetPurchaseRequisitionMasterManager().Find(id);

            if (Master.Date != null)
                Master.Date2 = Master.Date.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            ViewBag.ProjectName = objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == Master.ProjectNo).ProjectName;

            ViewBag.PurchaseIndentDate = Master.Date;
            ViewBag.PurchaseIn_deliverydate = Master.DeliveryDate;
            int? dtype = objmms.tbl_beforPIdecesionPurchase.Where(s => s.PurReqNo == Master.PurRequisitionNo).Select(s => s.Purchase_PI_DecesionType).FirstOrDefault();
            ViewBag.DesType = objmms.tblPIPurchasedecisionTypes.Where(s => s.ID == dtype).Select(s => s.PurchaseSelectionType).FirstOrDefault();
            //if (Master.PurchasePI_Type != 8)
            //{
            //    ViewBag.PIType = objmms.tblPIPurchasedecisionTypes.SingleOrDefault(x => x.ID == Master.PurchasePI_Type).PurchaseSelectionType;
            //}
            //else
            //{
            //    ViewBag.PIType = "Manual Transfer";
            //}


            ViewBag.PIType = "Normal";

            ViewBag.SiteId = objmms.tblPurchaseLimitValueByProjectWises.Where(x => x.ProjectId == Master.ProjectNo).FirstOrDefault().EmengencyPurchaseType;
            ViewBag.SiteLimitVal = objmms.tblPurchaseLimitValueByProjectWises.Where(x => x.ProjectId == Master.ProjectNo).FirstOrDefault().EmengencyLimitValue;
            ViewBag.HOId = objmms.tblPurchaseLimitValueByProjectWises.Where(x => x.ProjectId == Master.ProjectNo).FirstOrDefault().NormalPurchaseType;
            ViewBag.HoLimitVal = objmms.tblPurchaseLimitValueByProjectWises.Where(x => x.ProjectId == Master.ProjectNo).FirstOrDefault().NormalLimitValue;

            var PIDetails = objmms.PurchaseRequisitionMasters.SingleOrDefault(x => x.UId == id);

            //ViewBag.PICApproval = PIDetails.AllItemPICStatus=="Approved"?PIDetails.SendToName:"";
            //ViewBag.HeadApproval = PIDetails.AllItemMangStatus == "Approved" ?objmms.tblEmployeeMasters.SingleOrDefault(x=>x.EmpID==PIDetails.ForwardToMang).FName : "";

            ViewBag.PICApproval = PIDetails.AllItemPICStatus == "Approved" ? objmms.tblEmployeeMasters.Where(x => x.EmpID == PIDetails.SendTo).Select(s => s.FName + " " + s.LName).FirstOrDefault() : "";
            ViewBag.HeadApproval = PIDetails.AllItemMangStatus == "Approved" ? objmms.tblEmployeeMasters.Where(x => x.EmpID == PIDetails.ForwardToMang).Select(s => s.FName + " " + s.LName).FirstOrDefault() : "";
            Master.CreatedByName = objmms.tblEmployeeMasters.Where(x => x.EmpID == PIDetails.CreatedBy).Select(s => s.FName + " " + s.LName).FirstOrDefault();

            List<PurchaseRequisitionChildViewModel> ChildList =objpro.GetPurchaseIndentItemDetails(id).OrderBy(x=>x.UID).ToList();


           

            //var a = objmms.PurchaseRequisitionChilds.Where(b => b.MId == id).ToList();

            //var itemMaster = objmms.tblItemMasters.ToList();
            //if(ChildList!=null && ChildList.Count > 0)
            //{
            //    foreach (var item in ChildList)
            //    {
            //        item.HSNCode = itemMaster.SingleOrDefault(x => x.ItemID == item.ItemNo).HSNCode;
            //    }
            //}

            var totalRows = ChildList.Count();
            ViewBag.PurchaseDescisionType = objmms.tbl_beforPIdecesionPurchase.Where(x => x.ProjectId == Master.ProjectNo && x.PurReqNo == Master.PurRequisitionNo).ToList().Count() > 0 ? objmms.tbl_beforPIdecesionPurchase.Where(x => x.ProjectId == Master.ProjectNo && x.PurReqNo == Master.PurRequisitionNo).FirstOrDefault().Purchase_PIC_DecesionType : 0;
            var findType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList().Count() > 0 ? objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).First().TypeName : "User";
            if (findType == "PIC" || findType == "User")
            {
                ViewBag.PurchaseTypeId = Master.PurchasePI_Type;
            }
            else if (findType == "Mang")
            {
                ViewBag.PurchaseTypeId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Master.PurRequisitionNo).FirstOrDefault().PurchasePIC_Type;
                
            }
            else
            {
                ViewBag.PurchaseTypeId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Master.PurRequisitionNo).FirstOrDefault().PurchaseMang_Type;
            }
            int? forwordToId = 0;
            if (ViewBag.PurchaseTypeId != null)
            {
                forwordToId = Convert.ToInt32(ViewBag.PurchaseTypeId);
            }
            else
            {
                forwordToId = Master.PurchasePI_Type;
            }

            ViewBag.PIForwardTo = objmms.tblPI_PurchaseType.SingleOrDefault(x => x.TrandId == forwordToId).PurchaseType;

            int piTypeId =Convert.ToInt32(ViewBag.PurchaseDescisionType);
            if (piTypeId == 0)
            {
                ViewBag.PIType = "-";
            }
            else
            {
                ViewBag.PIType = objmms.tblPIPurchasedecisionTypes.SingleOrDefault(x => x.ID == piTypeId).PurchaseSelectionType;
            }
                

            //ViewBag.CreatedBy =objmms.tblEmployeeMasters.SingleOrDefault(x=>x.EmpID==Mas);

            //ViewBag.PurchaseType = objmms.tblPI_PurchaseType.ToList().Select(p => new SelectListItem
            //{
            //    Text = p.PurchaseType , 
            //    Value = p.TrandId,
            //    Selected = true,
            //}
            //)

            var data = new PurchaseIRNew()
            {
                MasterNew = Master,
                ChildNew1 = ChildList
            };

            var list = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList();
            if (list.Count() > 0)
            { ViewBag.TypeSelect = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).First().TypeName; }
            else { ViewBag.TypeSelect = "User"; }

            var PILimit = objmms.tblPurchaseRequisitionApprovalLimitValues.Where(x => x.ProjectId == Master.ProjectNo).OrderByDescending(p => p.TransId).First().LimitValue;
            ViewBag.LastLimitVal = PILimit;

            var checkFullyApprovedMangStatus = objmms.PurchaseRequisitionMasters.Where(x => x.ProjectNo == Master.ProjectNo && x.UId == Master.UId && x.AllItemMangStatus == null).ToList();
            ViewBag.CheckMangExist = checkFullyApprovedMangStatus.Count();

            if (data == null)
            {
                return HttpNotFound();
            }

            var piData = objmms.PurchaseRequisitionMasters.SingleOrDefault(x => x.UId == id);

            ViewBag.StoreRemarks = piData.Remarks;
            ViewBag.PICRemarks = piData.PICRemarks;
            ViewBag.MangRemarks = piData.MangRemarks;

            return View(data);
        }


        public JsonResult Getitemn_balance(string PrjId = null, string IG = null, string itemid = null)
        {

            decimal itemsListBalance = this.Getitemitem_BalanceCheck(PrjId, IG, Convert.ToString(itemid));
            return Json(itemsListBalance, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetRemarks(string PI)
        {
          //  string Proj_ID = Session["ProjectssId"].ToString();
            string PI_ID = PI.Replace("#", "/") ;
            string Proj_ID = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PI_ID).FirstOrDefault().ProjectNo;
            List<PI_Master_Reamrks> res = null;
            res = objpro.GetAllItemStatusReport(PI_ID, Proj_ID);
           // var MRemarks  = objmms.PurchaseRequisitionMasters.Where(x=>x.PurRequisitionNo == PI).ToList().Select(p=>p.Remarks)
            if (res != null)
            {
                return PartialView("_Purchase_Master_Remarks_Detail", res);
            }
            else {
                  return View("_EmptyView");

            }
                
            
        }




        public decimal Getitemitem_BalanceCheck(string PrjIds, string Ig, string ItemId)
        {
            decimal TotalSum = 0;
            var ItemCheck = objmms.tblReceiveDatas.Where(x => x.ProjectId == PrjIds && x.ItemGroupId == Ig && x.ItemId == ItemId).ToList();
            if (ItemCheck.Count() > 0)
            {
                var su = from k in objmms.tblReceiveDatas.Where(x => x.ProjectId == PrjIds && x.ItemGroupId == Ig && x.ItemId == ItemId).ToList()
                         group k by k.TransId into g
                         select new
                         { SUM = g.Sum(o => o.BalanceQuantity) };

                TotalSum = Math.Round((decimal)(su.Sum(l => l.SUM)), 3);
            }
            else {
                TotalSum = 0;
            }
         return TotalSum ;
        }

        public ActionResult Edit(decimal id)
        {
            id = 2;

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PurchaseRequisitionMaster Master = m.GetPurchaseRequisitionMasterManager().Find(id);
            List<PurchaseRequisitionChild> ChildList = m.GetPurchaseRequisitionChildManager().FindByMaster(id);
            ViewBag.Date = Master.Date;

            ViewBag.EmpId = EmpId;

            var data = new PurchaseIR()
            {
                Master = Master,
                Child = ChildList
            };

            if (data == null)
            {
                return HttpNotFound();
            }

            ViewBag.ItemGroup = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");
            ViewBag.SendTo1 = new SelectList(m.GetEmployeeMasterManager().FindByProject(Master.ProjectNo), "EmpID", "FName", Master.SendTo);

            return View(data);
        }
        public ActionResult Details(decimal id)
        {
            //id = 2;

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisitionMaster Master = m.GetPurchaseRequisitionMasterManager().Find(id);

            if (Master.Date != null)
                Master.Date2 = Master.Date.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

            List<PurchaseRequisitionChild> ChildList = m.GetPurchaseRequisitionChildManager().FindByMaster(id);

            var totalRows = ChildList.Count();

            var data = new PurchaseIR()
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
        public ActionResult DetailsA(decimal id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisitionMaster Master = m.GetPurchaseRequisitionMasterManager().Find(id);

            if (Master.Date != null)
                Master.Date2 = Master.Date.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

            List<PurchaseRequisitionChild> ChildList = m.GetPurchaseRequisitionChildManager().FindByMaster(id);

            var totalRows = ChildList.Count();

            var data = new PurchaseIR()
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

        public JsonResult GetForwardType()
        {
            string ProjectId = Session["ProjectssId"].ToString();
            //var ll = (from a in objmms.tblApproval_AccountType.Where(x => x.ProjectId == ProjectId && x.TypeName == "Mang").ToList()
            //          join
            //          b in objmms.tblEmployeeMasters.Where(p => p.ProjectID == ProjectId).ToList() on a.EmployeeId equals b.EmpID 
            //          select new SelectListItem
            //          {
            //              Value = a.EmployeeId,
            //              Text = b.FName
            //          }).Distinct().ToList();


            var ll = (from a in objmms.tblApproval_AccountType.Where(x => x.TypeName == "Mang").ToList()
                      join
                      b in objmms.tblEmployeeMasters.Where(k=>k.ProjectID.Contains(ProjectId)).ToList() on a.EmployeeId equals b.EmpID
                      select new SelectListItem
                      {
                          Value = a.EmployeeId,
                          Text = b.FName + ' '+ b.LName
                      }).Distinct().ToList();



            //  var Dept = objmms.tblDepartments.Where(p => p.Status == "E").ToList().Select(x => new { Text = x.DepartmentName, Value = x.DeptID }).OrderBy(k => k.Text).ToList();
            return Json(ll, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetPurchaseType()
        {
            var empId = Session["EmpID"].ToString();
            //var empPurchaseType = objmms.tblEmployeeMasters.SingleOrDefault(x => x.EmpID == empId).PurchaseType;
           // var res = objmms.tblPI_PurchaseType.Where(x=>empPurchaseType.Contains(x.TrandId.ToString())).Select(x => new { Text = x.PurchaseType, Value = x.TrandId }).OrderBy(p => p.Text).ToList();
           var res=objmms.tblPI_PurchaseType.Where(x=>x.TrandId==1 || x.TrandId==2).Select(x => new { Text = x.PurchaseType, Value = x.TrandId }).OrderBy(p => p.Text).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseDecesionType()

        {
            var res = objmms.tblPIPurchasedecisionTypes.ToList().Select(x => new { Text = x.PurchaseSelectionType, Value = x.ID }).OrderBy(p => p.Text).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }


        //public JsonResult GetPurchaseType(int PurchaseTypeId)
        //{
        //    var res = objmms.tblPI_PurchaseType.Where(k=>k.TrandId == PurchaseTypeId).ToList().Select(x => new { Text = x.PurchaseType, Value = x.TrandId  }).OrderBy(p => p.Text).ToList();
        //    return Json(res, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult DetailsAPI(decimal id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisitionMaster Master = m.GetPurchaseRequisitionMasterManager().Find(id);

            var piAttach = objmms.tbl_PurchaseRequistionAttachFiles.Where(s => s.PurReqNo == Master.PurRequisitionNo).Select(s => s).ToList();
            ViewBag.Attach = piAttach;

            if (Master.Date != null)
                Master.Date2 = Master.Date.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            Session["ProjectssId"] = Master.ProjectNo;

            ViewBag.PurchaseIndentDate = Master.Date;
            ViewBag.PurchaseIn_deliverydate = Master.DeliveryDate;


            ViewBag.SiteId = objmms.tblPurchaseLimitValueByProjectWises.Where(x=>x.ProjectId == Master.ProjectNo).FirstOrDefault().EmengencyPurchaseType;
            ViewBag.SiteLimitVal = objmms.tblPurchaseLimitValueByProjectWises.Where(x => x.ProjectId == Master.ProjectNo).FirstOrDefault().EmengencyLimitValue;
            ViewBag.HOId = objmms.tblPurchaseLimitValueByProjectWises.Where(x => x.ProjectId == Master.ProjectNo).FirstOrDefault().NormalPurchaseType;
            ViewBag.HoLimitVal = objmms.tblPurchaseLimitValueByProjectWises.Where(x => x.ProjectId == Master.ProjectNo).FirstOrDefault().NormalLimitValue;

            ViewBag.ID = id;

            List<PurchaseRequisitionChild> ChildList = m.GetPurchaseRequisitionChildManager().FindByMaster(id);

            var a = objmms.PurchaseRequisitionChilds.Where(b => b.MId == id).ToList();
            
            var totalRows = ChildList.Count();
            var totRows = a.Count();
            var findType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList().Count() > 0 ? objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).First().TypeName : "User";
            if (findType == "PIC" || findType == "User" || findType == "Purchase")
            {
                ViewBag.PurchaseTypeId = Master.PurchasePI_Type;
                ViewBag.PurchaseDescisionType = objmms.tbl_beforPIdecesionPurchase.Where(x => x.ProjectId == Master.ProjectNo && x.PurReqNo == Master.PurRequisitionNo).ToList().Count() > 0 ? objmms.tbl_beforPIdecesionPurchase.Where(x => x.ProjectId == Master.ProjectNo && x.PurReqNo == Master.PurRequisitionNo).FirstOrDefault().Purchase_PI_DecesionType : 0;
            }
            else if (findType == "Mang")
            {
                ViewBag.PurchaseTypeId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Master.PurRequisitionNo).FirstOrDefault().PurchasePIC_Type;
                ViewBag.PurchaseDescisionType = objmms.tbl_beforPIdecesionPurchase.Where(x => x.ProjectId == Master.ProjectNo && x.PurReqNo == Master.PurRequisitionNo).ToList().Count() > 0 ?  objmms.tbl_beforPIdecesionPurchase.Where(x => x.ProjectId == Master.ProjectNo && x.PurReqNo == Master.PurRequisitionNo).FirstOrDefault().Purchase_PIC_DecesionType : 0;
            }

           
            //ViewBag.PurchaseType = objmms.tblPI_PurchaseType.ToList().Select(p => new SelectListItem
            //{
            //    Text = p.PurchaseType , 
            //    Value = p.TrandId,
            //    Selected = true,
            //}
            //)

            var data = new PurchaseIRNew()
            {
                MasterNew = Master,
                ChildNew = a
            };
         
            var list = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).ToList();
            if (list.Count() > 0)
            { ViewBag.TypeSelect = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).First().TypeName; }
            else { ViewBag.TypeSelect = "User"; }
            
            var PILimit = objmms.tblPurchaseRequisitionApprovalLimitValues.Where(x => x.ProjectId == Master.ProjectNo).OrderByDescending(p => p.TransId).First().LimitValue;
            ViewBag.LastLimitVal = PILimit;

            var checkFullyApprovedMangStatus = objmms.PurchaseRequisitionMasters.Where(x => x.ProjectNo == Master.ProjectNo && x.UId == Master.UId && x.AllItemMangStatus == null).ToList();
            ViewBag.CheckMangExist = checkFullyApprovedMangStatus.Count();

            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }
        public ActionResult SendList1(PurchaseIRNew Grid, List<PI_AttachFileModel> AttachModel)
        {
            using (var transaction=objmms.Database.BeginTransaction())
            {
                try
                {

                    Grid.MasterNew.PurRequisitionNo = GetPurchaseIndentNumber(Grid.MasterNew.ProjectNo, Grid.MasterNew.Date.Value);

                    int? div_Id = Repos.GetUserDivision();
                    string Fy_nm = GetCurrentFinancialYear(Convert.ToDateTime(Grid.MasterNew.Date));
                    var ses_Id = objmms.SessionMasters.Where(x => x.Name == Fy_nm.ToString()).FirstOrDefault().Id.ToString();
                    DateTime? lastPI = objmms.PurchaseRequisitionMasters.Where(s => s.ProjectNo == Grid.MasterNew.ProjectNo && s.PurchasePI_Type == Grid.MasterNew.PurchasePI_Type && s.DivisionId == div_Id && s.SessionId == ses_Id).OrderByDescending(s => s.UId).Select(s => s.Date).FirstOrDefault();
                    if (lastPI != null)
                    {
                        if (Grid.MasterNew.Date < lastPI.Value)
                            return Json(new { LastPIDate = lastPI.Value.ToString("dd/MM/yyyy") }, JsonRequestBehavior.AllowGet);
                    }

                    if (Grid.MasterNew.SendTo == "0" || string.IsNullOrEmpty(Grid.MasterNew.SendTo))
                    {
                        return Json("6", JsonRequestBehavior.AllowGet); // mandatory data were not provided.
                    }
                    var masterLst = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Grid.MasterNew.PurRequisitionNo && x.ProjectNo == Grid.MasterNew.ProjectNo).ToList();
                    if (masterLst.Count() > 0)
                    {
                        return Json("222", JsonRequestBehavior.AllowGet); // multiple groups
                    }
                    else
                    {
                        var iGroupCount = Grid.ChildNew.GroupBy(x => x.ItemGroupNo).Select(g => new { g.Key }).Count();

                        if (iGroupCount>1)
                        {
                            return Json("5", JsonRequestBehavior.AllowGet);
                        }

                        if (EmpId == null || EmpId == "")
                        {
                            return Json("4", JsonRequestBehavior.AllowGet); // Some Administrator problem. Kindly Re-load Page.
                        }
                        else if (Grid.ChildNew.GroupBy(x => x.ItemNo).Any(g => g.Count() > 1))
                        {
                            return Json("5", JsonRequestBehavior.AllowGet); // two same items.
                        }
                        else
                        {

                            #region
                            // Note : From PurchaseIRCreate Js , I have take Purchase decesion value on Comp column name , that's actually not required 
                            // on PI creation time , but we need that purchase decesion value , so just store it value on comp and after using this 
                            // value to insert into tbl_beforPIdecesionPurchase table , then again make comp column value to null .
                            string Fynm = GetCurrentFinancialYear(Convert.ToDateTime(Grid.MasterNew.Date));

                            var Session_Id = objmms.SessionMasters.Where(x => x.Name == Fynm.ToString()).FirstOrDefault().Id;


                            //Grid.MasterNew.SessionId = SessionId.ToString();
                            Grid.MasterNew.SessionId = Session_Id.ToString();
                            Grid.MasterNew.CreatedBy = EmpId;
                            Grid.MasterNew.CreatedByName = EmpName;
                            Grid.MasterNew.CreatedDate = DateTime.Now;
                            Grid.MasterNew.ModifiedDate = DateTime.Now;
                            //Grid.MasterNew.
                            

                           

                            var poData=GetPurchaseIRNOByDate(Grid.MasterNew.ProjectNo, Grid.MasterNew.Date.Value);

                            decimal a = m.GetPurchaseRequisitionMasterManager().Add(Grid.MasterNew);

                            var data = objmms.PurchaseRequisitionMasters.SingleOrDefault(x => x.UId == a);

                            var divisionId = Repos.GetUserDivision();

                            data.RemarksDate = DateTime.Now;

                            data.Id = poData.Item2;
                            data.PurRequisitionNo = poData.Item1;
                            data.DivisionId = divisionId;
                            objmms.Entry(data).State = EntityState.Modified;
                            objmms.SaveChanges();

                            Grid.MasterNew.PurRequisitionNo = poData.Item1;

                            MMS.DAL.tbl_beforPIdecesionPurchase pidec = new DAL.tbl_beforPIdecesionPurchase();
                            pidec.CreatedDate = System.DateTime.Now;
                            pidec.ProjectId = Grid.MasterNew.ProjectNo;
                            pidec.PurReqNo = Grid.MasterNew.PurRequisitionNo;
                            pidec.Purchase_PI_DecesionType = Convert.ToInt32(Grid.MasterNew.Comp);
                            objmms.tbl_beforPIdecesionPurchase.Add(pidec);

                            // after consuming Comp , then make it null;
                            Grid.MasterNew.Comp = null;

                            if (Grid.ChildNew != null && a != 0)
                                foreach (var x in Grid.ChildNew)
                                {
                                    x.CreatedDate = System.DateTime.Now;
                                    x.MId = a;
                                    x.ApprovedQty = 0;
                                    x.ApprovedQtyPIC = 0;
                                    x.OrderedQty = 0;
                                    x.MangStatus = "N";
                                    x.PICStatus = "N";
                                    x.PurRequisitionNo = Grid.MasterNew.PurRequisitionNo;
                                    x.ProjectNo = Grid.MasterNew.ProjectNo;
                                    x.ProjectName = Grid.MasterNew.ProjectName;
                                    x.Date = Grid.MasterNew.Date;
                                    int a11 = objmms.PurchaseRequisitionChilds.Where(i => i.MId == a).Select(i => i.SNo).DefaultIfEmpty(0).Max(i => i).Value;
                                    if (a11 == 0)
                                    {
                                        x.SNo = 1;
                                    }
                                    else
                                    {
                                        x.SNo = a11 + 1;
                                    }
                                    objmms.PurchaseRequisitionChilds.Add(x);
                                    objmms.SaveChanges();
                                    //decimal b = m.GetPurchaseRequisitionChildManager().Add(x);
                                }

                            if (AttachModel != null)
                            {
                                foreach (PI_AttachFileModel attachFile in AttachModel)
                                {
                                    objmms.tbl_PurchaseRequistionAttachFiles.Add(new DAL.tbl_PurchaseRequistionAttachFiles() { AttachFile = attachFile.AttachFile, AttachFile_Name = attachFile.FileName, PurReqNo = Grid.MasterNew.PurRequisitionNo, CreatedOn = DateTime.Now });

                                }
                            }
                            objmms.SaveChanges();
                            transaction.Commit();
                            return Json(new { Status = 1, TransNo = Grid.MasterNew.PurRequisitionNo }, JsonRequestBehavior.AllowGet);
                            #endregion


                        }



                    }


                }
                catch (Exception ex) { ex.ToString();
                    transaction.Rollback();
                    return Json("2", JsonRequestBehavior.AllowGet);
                } 
            }
        }
        public JsonResult GetEmployeeByPRJId(string PRJID)
        {
            List<tblEmployeeMaster> E = m.GetEmployeeMasterManager().FindByProject(PRJID);

            SelectList obgE = new SelectList(E, "EmpID", "FName", 0);
            return Json(obgE);
        }
        public ActionResult SendList2(PurchaseIR Grid)
        {
            try
            {
                PurchaseRequisitionMaster t = m.GetPurchaseRequisitionMasterManager().Find(Grid.Master.UId);
                Grid.Master.SessionId = t.SessionId;
                Grid.Master.CreatedDate = t.CreatedDate;
                Grid.Master.ModifiedBy = EmpId;
                Grid.Master.ModifiedDate = System.DateTime.Now;

                List<decimal> CList = m.GetPurchaseRequisitionChildManager().FindAllIdByMaster(Grid.Master.UId);

                if (Grid.Child != null)
                {
                    foreach (var x in Grid.Child)
                    {
                        if (!CList.Any(a => a == x.UId))
                        {
                            Grid.Master.Comp = "No";

                            x.CreatedDate = System.DateTime.Now;
                            x.MId = Grid.Master.UId;
                            x.ApprovedQty = 0;
                            x.OrderedQty = 0;
                            x.PurRequisitionNo = Grid.Master.PurRequisitionNo;
                            x.ProjectNo = Grid.Master.ProjectNo;
                            x.ProjectName = Grid.Master.ProjectName;
                            x.Date = Grid.Master.Date;
                            decimal b = m.GetPurchaseRequisitionChildManager().Add(x);
                        }
                    }
                    if (CList != null)
                        foreach (decimal x in CList)
                        {
                            if (!Grid.Child.Any(a => a.UId == x))
                            {
                                m.GetTbOSTTransferChildManager().Delete(x);
                            }
                        }
                }
                m.GetPurchaseRequisitionMasterManager().Update(Grid.Master);
                //return RedirectToAction("Details", new { id = a });
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch { return Json("2", JsonRequestBehavior.AllowGet); }
        }

        // PurchaseIR to PurchaseApprovalIR
        public ActionResult UpdateList2(PurchaseApprovalIR Grid)
        { //PurchaseApprovalIR
            var PrjID = string.Empty; var EmptypeCheck = string.Empty; var CheckType = string.Empty;
            CheckType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == EmpId).First().TypeName;
            using (var trans=objmms.Database.BeginTransaction())
            {
                try
                {

                    if (Grid.Masterk != null)
                    {
                        PrjID = objmms.PurchaseRequisitionMasters.Where(x => x.UId == Grid.Masterk.UId).ToList().First().ProjectNo;
                        if (string.IsNullOrEmpty(Grid.Masterk.ForwardToMang))
                        {
                            EmptypeCheck = "";
                        }
                        else
                        {
                            // EmptypeCheck = objmms.tblApproval_AccountType.Where(p => p.ProjectId == PrjID && p.EmployeeId == Grid.Masterk.ForwardToMang).First().TypeName ;
                            EmptypeCheck = objmms.tblApproval_AccountType.Where(p => p.EmployeeId == Grid.Masterk.ForwardToMang).First().TypeName;
                        }

                    }
                    else
                    { // return empty list 
                    }

                    if (Grid.Childk != null)
                    {
                        decimal PILimit = objmms.tblPurchaseRequisitionApprovalLimitValues.Where(x => x.ProjectId == PrjID).OrderByDescending(p => p.TransId).First().LimitValue ?? 0;

                        var ss = from k in Grid.Childk
                                 group k by k.UId into g
                                 select new
                                 { SUM = g.Sum(o => o.ApprovedQty * o.CurrentRate) };

                        decimal totAmt = Math.Round((decimal)(ss.Sum(l => l.SUM)), 2);

                        int xx = Decimal.Compare(PILimit, totAmt);
                        bool check = false;
                        if (xx == 1)
                        { check = true; }
                        else { check = false; }


                        // do coding here
                        decimal LastReceivedRate = 0;
                        foreach (var x in Grid.Childk)
                        {
                            var dlst = objmms.tblReceiveDatas.Where(p => p.ProjectId == PrjID && p.ItemGroupId == x.ItemGroupNo && p.ItemId == x.ItemNo).ToList();
                            if (dlst.Count() > 0)
                            {
                                LastReceivedRate = objmms.tblReceiveDatas.Where(p => p.ProjectId == PrjID && p.ItemGroupId == x.ItemGroupNo && p.ItemId == x.ItemNo).OrderByDescending(k => k.CreatedDate).First().Rate ?? 0;
                            }
                            else
                            {
                                LastReceivedRate = 0;
                            }



                            if (x.UId != 0 && x.ApprovedQty >= 0)
                            {
                                var PurchaseReqNo = objmms.PurchaseRequisitionChilds.Where(p => p.UId == x.UId).First().PurRequisitionNo;
                                MMS.DAL.PurchaseRequisitionChild a = objmms.PurchaseRequisitionChilds.Where(p => p.UId == x.UId).First();




                                a.LastPurchaseRate = LastReceivedRate;

                                // for master
                                MMS.DAL.PurchaseRequisitionMaster ax = objmms.PurchaseRequisitionMasters.Where(k => k.UId == a.MId).First();
                                ax.CompO = "No";


                                ax.Comp = "Yes";
                                ax.DeliveryDate = Grid.Masterk.DeliveryDate;

                                ax.ModifiedDate = DateTime.Now;



                                // for saving Histroy record for pic and mang
                                if (CheckType == "PIC")
                                {
                                    ax.PICDate = DateTime.Now;
                                    if (check)
                                    {
                                        if (EmptypeCheck == null || EmptypeCheck == "")
                                        {
                                            if (string.IsNullOrEmpty(Grid.Masterk.ForwardToMang))
                                            {
                                                ax.ForwardToMang = null;
                                            }
                                            else
                                            {
                                                ax.ForwardToMang = Grid.Masterk.ForwardToMang;
                                            }
                                            if (a.ApprovedQty == null)
                                                a.ApprovedQty = 0;
                                            a.ApprovedQty = x.ApprovedQty;
                                            ax.ForwardToMang = EmpId;

                                            ax.PICRemarks = Grid.Masterk.PICRemarks;
                                            ax.PurchasePIC_Type = Grid.Masterk.PurchasePIC_Type;
                                            ax.PurchaseMang_Type = Grid.Masterk.PurchasePIC_Type;
                                            a.ApprovedQtyPIC = x.ApprovedQty;
                                            a.ApprovedQtyMang = x.ApprovedQty;
                                            a.PICStatus = x.PICStatus;
                                            a.MangStatus = "Not Required";
                                            a.PICCurrentValue = x.PICCurrentValue;
                                            ax.AllItemPICStatus = x.PICStatus;
                                            ax.CheckedBeyondPIC_Limit = Convert.ToBoolean(1);
                                            ax.AllItemMangStatus = "Not Required";
                                            // insert pic history along with mang
                                            MMS.DAL.tblPIC_PI_Approval_History pic = new DAL.tblPIC_PI_Approval_History();
                                            MMS.DAL.tblMang_PI_Approval_History mang = new DAL.tblMang_PI_Approval_History();
                                            pic.ApprovedPICQty = x.ApprovedQty;
                                            pic.CreatedDate = System.DateTime.Now;
                                            pic.ItemGroupId = x.ItemGroupNo;
                                            pic.ItemId = x.ItemNo;
                                            pic.UnitId = x.UnitNo;
                                            pic.PurchaseReqChild_ID = Convert.ToInt32(x.UId);
                                            pic.PurchaseReqNo = PurchaseReqNo;
                                            pic.ProjectId = PrjID;
                                            pic.Remarks = x.Remarks;
                                            objmms.tblPIC_PI_Approval_History.Add(pic);
                                            mang.ApprovedMangQty = x.ApprovedQty;
                                            mang.CreatedDate = System.DateTime.Now;
                                            mang.ItemGroupId = x.ItemGroupNo;
                                            mang.ItemId = x.ItemNo;
                                            mang.UnitId = x.UnitNo;
                                            mang.ProjectId = PrjID;
                                            mang.PurchaseReqChild_ID = Convert.ToInt32(x.UId);
                                            mang.PurchaseReqNo = PurchaseReqNo;
                                            mang.Remarks = x.Remarks;
                                            objmms.tblMang_PI_Approval_History.Add(mang);

                                            MMS.DAL.tbl_beforPIdecesionPurchase pidesc = objmms.tbl_beforPIdecesionPurchase.Where(x1 => x1.PurReqNo == PurchaseReqNo && x1.ProjectId == PrjID).FirstOrDefault();
                                            pidesc.Purchase_PIC_DecesionType = Convert.ToInt32(Grid.Masterk.Comp);
                                            pidesc.Purchase_Mang_DecesionType = Convert.ToInt32(Grid.Masterk.Comp);
                                            objmms.Entry(pidesc).State = EntityState.Modified;

                                            ax.Comp = null;



                                        }
                                    }
                                    else
                                    {
                                        if (EmptypeCheck != "")
                                        {
                                            if (a.ApprovedQty == null)
                                                a.ApprovedQty = 0;
                                            a.ApprovedQty = x.ApprovedQty;
                                            if (string.IsNullOrEmpty(Grid.Masterk.ForwardToMang))
                                            {
                                                ax.ForwardToMang = null;
                                            }
                                            else
                                            {
                                                ax.ForwardToMang = Grid.Masterk.ForwardToMang;
                                            }
                                            a.ApprovedQtyPIC = x.ApprovedQty;
                                            a.PICStatus = "Fw:Management";
                                            a.PICCurrentValue = x.PICCurrentValue;
                                            ax.AllItemPICStatus = "Fw:Management";
                                            ax.CheckedBeyondPIC_Limit = Convert.ToBoolean(0);
                                            ax.AllItemMangStatus = null;
                                            ax.PICRemarks = Grid.Masterk.PICRemarks;
                                            ax.PurchasePIC_Type = Grid.Masterk.PurchasePIC_Type;
                                            MMS.DAL.tblPIC_PI_Approval_History pic = new DAL.tblPIC_PI_Approval_History();
                                            //MMS.DAL.tblMang_PI_Approval_History mang = new DAL.tblMang_PI_Approval_History();
                                            pic.ApprovedPICQty = x.ApprovedQty;
                                            pic.CreatedDate = System.DateTime.Now;
                                            pic.ProjectId = PrjID;
                                            pic.ItemGroupId = x.ItemGroupNo;
                                            pic.ItemId = x.ItemNo;
                                            pic.UnitId = x.UnitNo;
                                            pic.PurchaseReqChild_ID = Convert.ToInt32(x.UId);
                                            pic.PurchaseReqNo = PurchaseReqNo;
                                            pic.Remarks = x.Remarks;
                                            objmms.tblPIC_PI_Approval_History.Add(pic);

                                            MMS.DAL.tbl_beforPIdecesionPurchase pidesc = objmms.tbl_beforPIdecesionPurchase.Where(x1 => x1.PurReqNo == PurchaseReqNo && x1.ProjectId == PrjID).FirstOrDefault();
                                            if (pidesc != null)
                                            {
                                                pidesc.Purchase_PIC_DecesionType = Convert.ToInt32(Grid.Masterk.Comp);

                                                objmms.Entry(pidesc).State = EntityState.Modified;

                                                ax.Comp = null;
                                            }

                                        }
                                        else
                                        {
                                            // return messg of must select forward To
                                        }
                                    }





                                }
                                else if (CheckType == "Mang")
                                {
                                    ax.MangDate = DateTime.Now;
                                    if (a.ApprovedQty == null)
                                        a.ApprovedQty = 0;
                                    a.ApprovedQty = x.ApprovedQty;
                                    a.MangStatus = x.MangStatus;
                                    a.ApprovedQtyMang = x.ApprovedQty;
                                    a.MangCurrentValue = x.PICCurrentValue;
                                    ax.AllItemMangStatus = "Approved";
                                    ax.MangRemarks = Grid.Masterk.PICRemarks;
                                    ax.PurchaseMang_Type = Grid.Masterk.PurchasePIC_Type;
                                    // only insert mang history
                                    MMS.DAL.tblMang_PI_Approval_History mang = new DAL.tblMang_PI_Approval_History();
                                    mang.ApprovedMangQty = x.ApprovedQty;
                                    mang.CreatedDate = System.DateTime.Now;
                                    mang.ItemGroupId = x.ItemGroupNo;
                                    mang.ItemId = x.ItemNo;
                                    mang.UnitId = x.UnitNo;
                                    mang.ProjectId = PrjID;
                                    mang.PurchaseReqChild_ID = Convert.ToInt32(x.UId);
                                    mang.PurchaseReqNo = PurchaseReqNo;
                                    mang.Remarks = x.Remarks;
                                    objmms.tblMang_PI_Approval_History.Add(mang);


                                    MMS.DAL.tbl_beforPIdecesionPurchase pidesc = objmms.tbl_beforPIdecesionPurchase.Where(x1 => x1.PurReqNo == PurchaseReqNo && x1.ProjectId == PrjID).FirstOrDefault();
                                    if (pidesc != null)
                                    {
                                        pidesc.Purchase_Mang_DecesionType = Convert.ToInt32(Grid.Masterk.Comp);

                                        objmms.Entry(pidesc).State = EntityState.Modified;

                                        ax.Comp = null;
                                    }


                                }




                                objmms.Entry(a).State = EntityState.Modified;

                                objmms.Entry(ax).State = EntityState.Modified;

                                objmms.SaveChanges();

                            }

                            //  m.GetPurchaseRequisitionChildManager().Update_ApproveBalance(x.UId, x.CurrentQty);
                        }
                        // m.GetPurchaseRequisitionMasterManager().FindComplete(Grid.Master.UId);




                    }
                    trans.Commit();
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex) { ex.ToString(); trans.Rollback(); return Json("2", JsonRequestBehavior.AllowGet); } 
            }
        }

        public ActionResult PurchaseReqViewDetail(decimal id)
        {

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisitionMaster Master = m.GetPurchaseRequisitionMasterManager().Find(id);

            if (Master.Date != null)
                Master.Date2 = Master.Date.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

            List<PurchaseRequisitionChild> ChildList = m.GetPurchaseRequisitionChildManager().FindByMaster(id);
            DateTime? Closed_Date = objmms.PurchaseRequisitionMasters.FirstOrDefault(s => s.UId == id).Dis_Date;
            if (Closed_Date != null)
            {
                ViewBag.Closed_Date = Closed_Date.Value.ToString("dd/MM/yyyy hh:mm:ss tt");
            }

            ViewBag.ID = id;

            var a = objmms.PurchaseRequisitionChilds.Where(b => b.MId == id).ToList();

            var totalRows = ChildList.Count();
            var totRows = a.Count();
            ViewBag.PurchaseTypeId = Master.PurchasePI_Type;

            var piAttach = objmms.tbl_PurchaseRequistionAttachFiles.Where(s => s.PurReqNo == Master.PurRequisitionNo).Select(s=>s).ToList();
            ViewBag.Attach = piAttach;

            Master.CreatedByName = objmms.tblEmployeeMasters.Where(e => e.EmpID == Master.CreatedBy).Select(s => s.FName + " " + s.LName).FirstOrDefault();

            var data = new PurchaseIRNew()
            {
                MasterNew = Master,
                ChildNew = a,
            };

            var FinalPurchasetypeId = objmms.PurchaseRequisitionMasters.Where(x => x.UId == Master.UId).FirstOrDefault().PurchasePIC_Type;
            if (FinalPurchasetypeId == null)
            {
                FinalPurchasetypeId= objmms.PurchaseRequisitionMasters.Where(x => x.UId == Master.UId).FirstOrDefault().PurchasePI_Type;
            }
            var detail = objmms.tblPI_PurchaseType.Where(x => x.TrandId == FinalPurchasetypeId).FirstOrDefault();

            ViewBag.FinalPurchaseType =detail==null?"-":detail.PurchaseType;

            var PILimit = objmms.tblPurchaseRequisitionApprovalLimitValues.Where(x => x.ProjectId == Master.ProjectNo).OrderByDescending(p => p.TransId).First().LimitValue;
            ViewBag.LastLimitVal = PILimit;


            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }

        public JsonResult GetSelectedPurchaseType(string Projectid)
        {
            var pitype = objmms.tblPurchaseLimitValueByProjectWises.Where(x => x.ProjectId == Projectid).ToList();
            if (pitype != null)
            {
                return Json(pitype.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }


        public PartialViewResult GetItemDetails(string ItemGroupId, string ItemName, string HSNCode, string UnitId)
        {
            //replace code with new code line on 19_05_2019
            // var searchData = new Procedure().SearchItems(ItemGroupId, ItemName, HSNCode, UnitId);
            var searchData = new Procedure().SearchEnableItems(ItemGroupId, ItemName, HSNCode, UnitId);
            return PartialView("_GridItemList",searchData);
        }


        #region Purchase Indent No. Generate on 3_3_2020

        private string GetPurchaseIndentNumber(string ProjectId, DateTime PurDate)
        {
            string Fynm = GetCurrentFinancialYear(PurDate);

            var SessionId = objmms.SessionMasters.Where(x => x.Name == Fynm.ToString()).FirstOrDefault().Id;

            var sessioncode = m.GetSessionMasterManager().Find(SessionId).Name;
            //var idMax = m.GetPurchaseRequisitionMasterManager().FindMaxId(SessionId.ToString(), ProjectId);
            var sessionIDString = SessionId.ToString();

            var divisionId = Repos.GetUserDivision();

            var idMax = 0;
            int a = objmms.PurchaseRequisitionMasters.Where(i => i.DivisionId == divisionId && i.SessionId == sessionIDString && i.ProjectNo == ProjectId && i.PurRequisitionNo.Contains("PurchaseIR")).Select(i => i.Id).DefaultIfEmpty(0).Max(i => i).Value;

            idMax = a == 0 ? 1 : a + 1;

            string Code = m.GetProjectMasterManager().FindCode(ProjectId);
            string ProjectCode = "";
            string DivisionCode = objmms.tblDivisionMasters.SingleOrDefault(x => x.Id == divisionId).DivisionCode;
            ProjectCode = "PurchaseIR/" + Code + "/" + DivisionCode + "/" + sessioncode + "/" + idMax;

            return ProjectCode;
        }

        #endregion


        #region Purchase Indent Attachment Upload
        [HttpPost]
        public ActionResult Upload_PIAttachment(PIAttachmentModel PIFiles)
        {
            string error = "";
            List<Dictionary<string, string>> FileInfoObj = new List<Dictionary<string, string>>();
            string[] fileext = { ".jpeg", ".jpg", ".png", ".doc", ".docx", ".xlsx", ".xls", ".pdf" };
            if (PIFiles != null) {
                try
                {
                    foreach (PIAttachFile attachfile in PIFiles.PIAttachFiles)
                    {
                        if (attachfile.AttachFile != null)
                        {
                            if (!fileext.Contains(System.IO.Path.GetExtension(attachfile.AttachFile.FileName)))
                            {

                                return Json(new { Data = FileInfoObj, Error = "File Format not valid." });
                            }

                            string path = Server.MapPath("~\\PurchaseIndentAttachment\\");
                            if (!System.IO.Directory.Exists(path))
                            {
                                System.IO.Directory.CreateDirectory(path);
                            }

                            string filename = DateTime.Now.ToString("ddMMyyyyhhmmssms") + attachfile.AttachFile.FileName;
                            attachfile.AttachFile.SaveAs(path + filename);
                            Dictionary<string, string> infoObj = new Dictionary<string, string>();
                            infoObj.Add("AttachFile", filename);
                            infoObj.Add("FileName", attachfile.FileName);
                            FileInfoObj.Add(infoObj);
                        }
                    }
                }
                catch (Exception ex)
                {

                    error=ex.Message;
                }
            }

            return Json(new { Data = FileInfoObj, Error = error });
        }

        #endregion

        #region Close PI On 23_12_20

        [HttpPost]
        public ActionResult Close_PI(string PINo, string Remark)
        {
            int res = 0;
            if (!string.IsNullOrEmpty(PINo) && !string.IsNullOrEmpty(Remark))
            {
                try
                {
                    var piObj = objmms.PurchaseRequisitionMasters.Where(s => s.PurRequisitionNo == PINo).FirstOrDefault();
                    if (piObj != null)
                    {
                        piObj.Is_Dis = 1;
                        piObj.Dis_Remark = Remark;
                        piObj.Dis_Date = DateTime.Now;
                        piObj.Dis_By = Session["EmpId"].ToString();

                        objmms.Entry(piObj).State = System.Data.Entity.EntityState.Modified;
                        res = objmms.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Res = res, Error = ex.Message });
                }
            }

            if (res > 0)
                return Json(new { Res = res, Error = "" });
            else
                return Json(new { Res = res, Error = "Record Not Found." });
        }

        public ActionResult ClosePIGrid(string PRJID)
        {
            List<GET_PI_ORDER> result = new List<GET_PI_ORDER>();
            result = null;
            // string EmpId = Session["EmpID"].ToString();
            result = objmsp.GetClosedPo_Detail(PRJID);

            if (result != null)
            {
                return PartialView("_Partial_ClosedPI", result);
            }
            else
            {
                return PartialView("_EmptyView");

            }
        }

        #endregion

    }
}