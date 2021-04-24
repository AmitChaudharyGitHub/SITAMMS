using DataAccessLayer;
using MMS.DAL;
using MMS.Models;
using MMS.ViewModels;
using MMS_P.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class PurchaseOrderController : Controller
    {
        // GET: PurchaseOrder
        private MMSEntities objmms = new MMSEntities();
        FactoryManager m = new FactoryManager();
        ModelServices mobjModel = new ModelServices();
        Procedure objpro = new Procedure();
        string EmpId = string.Empty;

        public PurchaseOrderController()
        {
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetStoreKeeperDetails(string ProjectId)
        {
            var storekeeperDetails = objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(ProjectId) && x.DeptID == "DPT0000002").Select(x=>new {Text=x.FName+" "+x.LName,Value=x.EmpID });
            if (storekeeperDetails != null)
            {
                return Json(storekeeperDetails.ToList().ToJSON(), JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStoreKeeperContact(string EmpId)
        {
            var storekeeperDetail = objmms.tblEmployeeMasters.SingleOrDefault(x => x.EmpID==EmpId);
            if (storekeeperDetail != null)
            {
                return Json(new {Mobile=storekeeperDetail.MobileNo }, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreatePO_GST(string PI)
        {
            ViewBag.PI_reqNo = PI.Replace("#", "/");
            string empId = Session["EmpID"].ToString();

            string prjId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PI.Replace("#", "/")).FirstOrDefault().ProjectNo;

            var lastPO = objmms.TbIndentPurchaseOrder_GST.Where(s => s.ProjectNo == prjId).OrderByDescending(s => s.UId).Select(s => s).FirstOrDefault();

            if (lastPO != null)
            {
                ViewBag.LastPODate = lastPO.PurchaseOrderDate.Value.ToString("dd/MM/yyyy");
            }

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

            



            List<BusinessObjects.Entity.tblEmployeeMaster> em = m.GetEmployeeMasterManager().FindByProject(prjId).ToList();


            var yue = (from b2 in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(prjId) && x.EmpID != EmpId).ToList()
                       join a2 in objmms.tblApproval_AccountType.Where(x1 => x1.TypeName == "PMC" && x1.EmployeeId != EmpId)
                       on b2.EmpID equals a2.EmployeeId
                       select new { b2 }).ToList();


            List<DAL.tblEmployeeMaster> H2 = (from Z1 in yue select Z1.b2).ToList();




            var dgh = (from b in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(prjId) && x.EmpID != EmpId).ToList()
                       join a1 in objmms.tblApproval_AccountType.Where(x1 => (x1.TypeName == "PIC" || x1.TypeName == "Mang") && x1.EmployeeId != EmpId)
                       on b.EmpID equals a1.EmployeeId
                       select new { b }).ToList();
            List<DAL.tblEmployeeMaster> H = (from Z in dgh select Z.b).ToList();


            ViewBag.Employee = H2.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName }).ToList(); // for pmc

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

        public ActionResult View_GTCData()
        {
            var a = (from pd in objmms.tblGenral_Terms_Conditions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreatedDate)
                     select new MMS.ViewModels.Genral_Terms_and_conditions
                     {
                         Id = pd.Id,
                         Header_Title = pd.Header_Title,
                         GTC_Description = pd.GTC_Description,
                         ProjectId = pd.ProjectId

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

        public JsonResult GetDetailOnPOGST(string Purchasereq)
        {
            string ProjId = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Purchasereq).First().ProjectNo;
            List<PIdetailOnPOCreation> res = new List<PIdetailOnPOCreation>();
            res = objpro.GetDGST(ProjId, Purchasereq);
            if (res != null)
            {
                return Json(res.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {

                return null;
            }
        }

        public ActionResult Grid_GST(decimal IndentNo_New, int page = 1)
        {
            const int pageSize = 100;
            List<BusinessObjects.Entity.PurchaseRequisitionChild> MasterList = null;

            List<MMS.ViewModels.Rate> dsas = new List<MMS.ViewModels.Rate>();
            List<MMS.ViewModels.ItemDescriptionData> abss = new List<MMS.ViewModels.ItemDescriptionData>();

            int totalRows = 0;

            //if (SessionId != 0)
            //{


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

                        var itemds = objmms.tblItemMasters.Where(x => x.ItemID == itemid && x.ItemGroupID == igname).Select(k => new MMS.ViewModels.ItemDescriptionData { ItemDescription = k.ItemDescription, ItemId = k.ItemID }).FirstOrDefault();

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

                    var TotalCartage = objmms.tblMMSCartageTypes.Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
                    ViewBag.Cartage = new SelectList(TotalCartage, "Value", "Text");
                    var TotalGST = objmms.tblGST_SplitTaxMaster.ToList().OrderBy(p => p.TransId).Select(x => new { Text = x.TaxRateType, Value = x.TaxCode }).ToList();
                    ViewBag.GST = new SelectList(TotalGST, "Value", "Text");


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
            //}
            return View("_EmptyView");
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


        public JsonResult GetAllGSTType(string TaxCode)
        {
            MMS.DAL.tblGST_SplitTaxMaster gs = objmms.tblGST_SplitTaxMaster.Where(x => x.TaxCode == TaxCode).FirstOrDefault();
            var lst = new { tax_CGST = gs.CGST, tax_SGSTandUTGST = gs.SGST, tax_IGST = gs.IGST };
            return Json(lst.ToJSON(), JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult ADD_GSTPO(IndentPurchaserOrderNew_GST Grid, List<PI_AttachFileModel> AttachModel)
        {
            decimal? MaxUid = 0;
            using (var transaction=objmms.Database.BeginTransaction())
            {
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
                                transaction.Rollback();
                                return Json(new { Status = "6" }, JsonRequestBehavior.AllowGet); //Data is already exist.
                            }
                            else
                            {

                                MaxUid = findUidForNewRow_GST();
                                if (MaxUid != 0)
                                {
                                    Grid.Master.PurchaseOrderNo = Get_PurchaseOrderNo(Grid.Master.ProjectNo, Grid.Master.IndentRefNo, Grid.Master.PurchaseOrderDate.Value);
                                    string VarPurchaseReqNo = Grid.Master.IndentRefNo;
                                    string poindentNo = Grid.Master.PurchaseOrderNo; string Purreq = string.Empty;


                                    // Grid.Master.SessionId = SessionId;
                                    Grid.Master.CreatedBy = EmpId;
                                    Grid.Master.Status = "Pending";
                                    Grid.Master.Edited_Status = "No";

                                    // Grid.Master.Vendor_Group_Id = Grid.Master.Vendor_Group_Id;
                                    Grid.Master.CreatedDate = System.DateTime.Now;
                                    //Grid.Master.PurchaseOrderId = m.GetTbIndentPurchaseOrderMasterManager().FindMaxId(SessionId, Grid.Master.ProjectNo);
                                    Grid.Master.PurchaseOrderId = FindMaxId_GST(Grid.Master.ProjectNo);
                                    int? intPurchasePIType = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == VarPurchaseReqNo).FirstOrDefault().PurchasePIC_Type;
                                    if (intPurchasePIType >= 3)
                                    {

                                        Grid.Master.POType = objmms.tblPI_PurchaseType.Where(x => x.TrandId == intPurchasePIType).FirstOrDefault().Purchase_Code;
                                        Grid.Master.IndentRefNo = Purreq = VarPurchaseReqNo;
                                    }
                                    else
                                    {

                                        string[] IndentNo = Grid.Master.IndentRefNo.Split('/');
                                        Grid.Master.IndentRefNo = Purreq = IndentNo[0].ToString() + "/" + IndentNo[1].ToString() + "/" + IndentNo[2].ToString() + "/" + IndentNo[3].ToString();

                                        Grid.Master.POType = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == Purreq).FirstOrDefault().PurchasePIC_Type == Convert.ToInt32(1) ? "IPO" : "LP";
                                    }

                                    Grid.Master.Edited_Status = "No";
                                    var CheckedPoLimit = Grid.Master.CheckedBeyondPOLimit;
                                    if (CheckedPoLimit == Convert.ToBoolean(1))
                                    { Grid.Master.SecondLevelApprv_Id = Grid.Master.SendTo; }
                                    else { Grid.Master.FirstLevelApprv_Id = Grid.Master.SendTo; }


                                    objmms.TbIndentPurchaseOrder_GST.Add(Grid.Master);

                                    //var CheckedPoLimit = Grid.Master.CheckedBeyondPOLimit;
                                    //TbIndentPurchaseOrder_Approval_For_Print tbl = new TbIndentPurchaseOrder_Approval_For_Print();
                                    //tbl.ProjectId = Grid.Master.ProjectNo;
                                    //tbl.Project_Name = objmms.tblProjectMasters.Where(b => b.PRJID == Grid.Master.ProjectNo).First().ProjectName;
                                    //tbl.Purchase_Order_Indent_No = Grid.Master.PurchaseOrderNo;
                                    //tbl.Created_Employee_Id = EmpId;
                                    //tbl.Employee_Name = objmms.tblEmployeeMasters.Where(h => h.EmpID == Grid.Master.SendTo).First().FName;
                                    //tbl.PurchaseRefNo = Grid.Master.IndentRefNo;
                                    //tbl.Vendor_Group_Id = Grid.Master.SupplierNo;
                                    //tbl.Status = "Pending";
                                    //tbl.Send_To = Grid.Master.SendTo;
                                    //tbl.CreatedDate = DateTime.Now;
                                    //tbl.Status_Approval = "Not Approved";
                                    //tbl.EditedStatus = "No";
                                    //tbl.Remark = Grid.Master.Remarks;
                                    //if (CheckedPoLimit == Convert.ToBoolean(1))
                                    //{ tbl.SecondLevelApprv_Id = Grid.Master.SendTo; }
                                    //else { tbl.FirstLevelApprv_Id = Grid.Master.SendTo; }

                                    //objmms.TbIndentPurchaseOrder_Approval_For_Print.Add(tbl);


                                    // var  KK = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == Grid.Master.PurchaseOrderNo).Select(p => p.UId).FirstOrDefault();
                                    // string FindLastId = KK.ToString();

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





                                    if (Grid.Child2 != null)
                                        foreach (var x in Grid.Child2)
                                        {

                                            x.CreatedBy = EmpId;
                                            x.CreatedDate = System.DateTime.Now;
                                            x.Edited_Status = "No";
                                            x.Status = "Enable";
                                            x.IsActive = "Yes";
                                            objmms.tblSpecificTermsAndConditions_Child.Add(x);


                                        }

                                    if (Grid.Child3 != null)
                                        foreach (var x in Grid.Child3)
                                        {
                                            x.CreatedBy = EmpId;
                                            x.CreatedDate = System.DateTime.Now;
                                            x.Edited_Status = "No";
                                            x.Status = "Enable";
                                            x.IsActive = "Yes";
                                            objmms.tblSpecific_Instructions_Child.Add(x);


                                        }
                                    if (Grid.Child4 != null)
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
                                    // after adding p.O then P.I complete will be updated.
                                    // m.GetPurchaseRequisitionMasterManager().FindCompleteO(Grid.Master.IndentRefNo);
                                    transaction.Commit();
                                    return Json(new { Status = "1", TransNo= Grid.Master.PurchaseOrderNo }, JsonRequestBehavior.AllowGet);

                                }
                                else
                                {
                                    transaction.Rollback();
                                    return Json(new { Status = "4" }, JsonRequestBehavior.AllowGet);

                                }





                            }
                            #endregion

                        }
                        else
                        {
                            transaction.Rollback();
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
                    transaction.Rollback();
                    ex.ToString(); return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet);
                }
            }
 
        }


        public decimal? findUidForNewRow_GST()
        {
            
                //int? intIdt = db.Users.Max(u => (int?)u.UserId);
                //decimal? id = objmms.TbIndentPurchaseOrder_GST.Max(x => (decimal?)x.UId);
                decimal? id = objmms.TbIndentPurchaseOrder_GST.Select(i => i.UId).DefaultIfEmpty(0).Max(i => i);
                if (id == 0)
                {
                    return 1;
                }
                else
                {
                    return id + 1;
                }
            
        }

        public int FindMaxId_GST(string PjId)
        {
            int K = objmms.TbIndentPurchaseOrder_GST.Where(x => x.ProjectNo == PjId).Select(i => i.PurchaseOrderId).DefaultIfEmpty(0).Max(i => i).Value;
            if (K == 0)
            {
                return 1;
            }
            else
            {
                return K + 1;
            }


        }

        public JsonResult GetVendorByGroup(string Vid, string Pid)
        {
            object VendorMasters = m.GetVendorMasterManager().FindByVendorGroup(Vid, Pid);
            string jsonString = VendorMasters.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVendorDetail(string ID)
        {
            string j = null;
            if (ID != null)
            {
                var E = m.GetVendorMasterManager().Find(ID);
                if (E != null)
                {
                    var Country = objmms.tblCountries.Where(x => x.CountryID == E.Country).First().CnName;
                    var State = objmms.tblStates.Where(x => x.StateID == E.State && x.CountryID == E.Country && x.stStatus == true).First().StateName;
                    var City = objmms.tblCityLists.Where(x => x.CityID == E.City && x.StateID == E.State && x.ctStatus == true).First().CityName ?? "N/A";

                    var v = new { Address = E.Address ?? "N/A", TinNo = E.TinNo ?? "N/A", PanNo = E.PanNo ?? "N/A", VCountry = Country ?? "N/A", VState = State ?? "N/A", VCity = City ?? "N/A", CpName = E.ContactPerson ?? "N/A", CpMobile = E.MobileNo ?? "N/A", CpPhone = E.PhoneNo ?? "N/A", CpEmail = E.Email ?? "N/A", vAdhar = E.Aadhaar_No ?? "N/A", V_PIN = E.PinCode ?? "N/A", GST = E.GST_NO ?? "N/A" };
                    j = v.ToJSON();
                }
            }
            return Json(j, JsonRequestBehavior.AllowGet);
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
                              b in objmms.tblEmployeeMasters.Where(p => p.ProjectID.Contains(PRJID)).ToList() on a.EmployeeId equals b.EmpID
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
                           Value = b.EmpID,
                           Text = b.FName + " " + b.LName
                       }).ToList();

            J = lst.ToJSON();
            return Json(J, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditNew(string PO_No)
        {
            //IndentPurchaserOrder_EditNew  decimal id
            string PONO_Edit = PO_No.Replace("#", "/");
            decimal id = objmms.TbIndentPurchaseOrder_GST.Where(ko => ko.PurchaseOrderNo == PONO_Edit).FirstOrDefault().UId;


            DAL.TbIndentPurchaseOrder_GST Master = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == id).First();
            ViewBag.PO_No = Master.PurchaseOrderNo;
            List<DAL.TbIndentPurchaseOrderChild_GST> ChildList = objmms.TbIndentPurchaseOrderChild_GST.Where(x => x.MUId == Master.UId).ToList();
            ViewBag.ProjectStateID = objmms.tblProjectGSTNoes.Where(s => s.ProjectId == Master.ProjectNo).First().StateId;

            string EmpId = Session["EmpID"].ToString();
            ViewBag.Date = Master.PurchaseOrderDate;
            ViewBag.EmpId = EmpId;

            ViewBag.VGId = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", Master.Vendor_Group_Id);
          //  string SendToId = objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(x => x.Purchase_Order_Indent_No == Master.PurchaseOrderNo).FirstOrDefault().Send_To;
            ViewBag.VNAME = new SelectList(objmms.tblVendorMasters.Where(x => x.PRJID.Contains(Master.ProjectNo) && x.MulRegistrationType.Contains(Master.Vendor_Group_Id)).ToList(), "VendorID", "Name", Master.SupplierNo);

            var lst = (from a2 in objmms.tblApproval_AccountType.Where(x => x.TypeName == "Purchase").ToList()
                       join b in objmms.tblEmployeeMasters.Where(p => p.ProjectID.Contains(Master.ProjectNo)).ToList()
                       on a2.EmployeeId equals b.EmpID
                       select new SelectListItem
                       {
                           Value = b.EmpID,
                           Text = b.FName + " " + b.LName
                       }).ToList();
            // string CPName = objmms.TbDelivery_Details_PO.Where(x => x.Purchase_order_No == Master.PurchaseOrderNo && x.ProjectId ==Master.ProjectNo).FirstOrDefault().Contact_Person_Name;
            //string CPNameID = objmms.tblEmployeeMasters.Where(x => (x.FName + " "+ x.LName) == CPName).FirstOrDefault().EmpID;
           string CPNameID=objmms.TbDelivery_Details_PO.Where(x => x.Purchase_order_No == Master.PurchaseOrderNo && x.ProjectId == Master.ProjectNo).FirstOrDefault().EPContactPerID;
            ViewBag.DeleryPurchseEmp = new SelectList(lst,"Value","Text", CPNameID); //b.FName + " " + b.LName


            ViewBag.VG = m.GetVendorGroupManager().FindByVendor(Master.SupplierNo).TypeID;
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var a = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == id && x.ProjectNo == Master.ProjectNo).ToList();
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


            ViewBag.Employee = H2.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName }).ToList();
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


            var data = new IndentPurchaserOrder_EditNewGST()
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

        public JsonResult GetPODeliveryDetail(decimal Uid)
        {
            string PONo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == Uid).ToList().FirstOrDefault().PurchaseOrderNo;
            var lst = objmms.TbDelivery_Details_PO.Where(x => x.Purchase_order_No == PONo).ToList().Select(p => new { DeliverySchedule = p.Delivery_Schedule, DeliveryAddr = p.Delivery_Address, BillingAddr = p.Billing_Address, PName = p.Contact_Person_Name, ContMobile = p.Contact_Person_Mobile, ContType=p.ContactDivType, PersonID=p.EPContactPerID }).FirstOrDefault();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        public ActionResult View_GTCDataPOWise(decimal id)
        {
            string PONo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == id).First().PurchaseOrderNo;
            string ProjectNo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == id).First().ProjectNo;
           

            var a = (from pd in objmms.tblGenral_Terms_Conditions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreatedDate)
                     select new MMS.ViewModels.Genral_Terms_and_conditions
                     {
                         Id = objmms.tblGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).FirstOrDefault().Id : pd.Id,
                         Header_Title = objmms.tblGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).FirstOrDefault().Header_Title : pd.Header_Title,
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

            string PONo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == id).First().PurchaseOrderNo;
            string ProjectNo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == id).First().ProjectNo;
        


            var a = (from pd in objmms.tblSpecific_Instructions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreatedDate)
                     select new Specific_Instruction_Conditions
                     {
                         Id = objmms.tblSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).FirstOrDefault().Id : pd.Id,
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
            string PONo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == id).First().PurchaseOrderNo;
            string ProjectNo = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == id).First().ProjectNo;
            


            var a = (from pd in objmms.tblSpecificTermsAndConditions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreateDate)
                     select new Specific_Terms_And_Conditions
                     {
                         Id = objmms.tblSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).FirstOrDefault().Id : pd.Id,
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

        public JsonResult GetPODetailForEdit(string PONo)
        {
            string ProjId = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PONo).First().ProjectNo;
            List<POAndPIDetailOnPOEdit> res = new List<POAndPIDetailOnPOEdit>();
            res = objpro.GetPODEditGST(ProjId, PONo);
            if (res != null)
            {
                return Json(res.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {

                return null;
            }
        }

        public ActionResult GridEdit(decimal Uid, int page = 1)
        {
            const int pageSize = 100; int totalRows = 0; string IsExcise = string.Empty; string IsInsi1 = string.Empty; string Isinsi2 = string.Empty;
            string Isinsi3 = string.Empty; string Isinsi4 = string.Empty; string Iscartage1 = string.Empty; string Iscartage2 = string.Empty; string Istax = string.Empty;
            List<MMS.ViewModels.ItemDescriptionData> abss = new List<MMS.ViewModels.ItemDescriptionData>();
            List<MMS.ViewModels.ItemUnitDetail> lst = new List<ItemUnitDetail>();
            DAL.TbIndentPurchaseOrder_GST MGrid = objmms.TbIndentPurchaseOrder_GST.Where(x => x.UId == Uid).FirstOrDefault();

            List<DAL.TbIndentPurchaseOrderChild_GST> MasterList = null;
            totalRows = objmms.TbIndentPurchaseOrderChild_GST.Where(x => x.MUId == Uid).ToList().Count;
            MasterList = objmms.TbIndentPurchaseOrderChild_GST.Where(x => x.MUId == Uid).ToList();

            ViewBag.TCSActive = MGrid.TCSActive;

            var TCSTax = objmms.tbl_TCSTaxMaster.ToList().OrderBy(p => p.TCSRate).Select(x => new { Text = x.TCSSlab, Value = x.TCSRate }).ToList();
            ViewBag.TCS = new SelectList(TCSTax, "Value", "Text");




            if (MasterList != null)
            {
                foreach (var xx in MasterList)
                {
                    var projectid = MGrid.ProjectNo;
                    var igname = objmms.tblItemMasters.Where(x => x.ItemID == xx.ItemNo).First().ItemGroupID;
                    var itemid = xx.ItemNo;
                    DateTime datetime = DateTime.Now.Date;
                    var itemds = objmms.tblItemMasters.Where(x => x.ItemID == itemid && x.ItemGroupID == igname).Select(k => new MMS.ViewModels.ItemDescriptionData { ItemDescription = xx.Item_Description, ItemId = k.ItemID }).FirstOrDefault();
                    abss.Add(itemds);
                    var det = objmms.tblItemMasters.Where(x => x.ItemID == itemid && x.ItemGroupID == igname).Select(p => new MMS.ViewModels.ItemUnitDetail { ItemName = p.ItemName, ItemNumber = p.ItemID, UnitNumber = p.UnitID, UnitName = objmms.tblUnitMasters.Where(c => c.UnitID == p.UnitID).FirstOrDefault().UnitName }).FirstOrDefault();
                    lst.Add(det);

                    //var TotalCartage = objmms.tblMMSCartageTypes.Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
                    //ViewBag.TotalCartagetest = new SelectList(TotalCartage, "Value", "Text",xx.CartageTypeId);

                    //  IsExcise = xx.IsExcise;
                    //ViewBag.ExistStatus = new SelectList(ObjList, "Value", "Text", xx.IsExcise);

                   


                }
                if (abss != null)
                {
                    ViewBag.ViewAdd_Desc = abss;


                }
                if (lst != null)
                {
                    ViewBag.ItemUnitDetail = lst;
                }
                var TotalCartage = objmms.tblMMSCartageTypes.Where(x=>x.Status==true).Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
               ViewBag.TotalCartagetest = new SelectList(TotalCartage, "Value", "Text");

                var TotalGSTSLABMaster = objmms.tblGST_SplitTaxMaster.ToList().Select(n => new { Text = n.TaxRateType, Value = n.TaxCode }).OrderBy(s => s.Text).ToList();
                ViewBag.MyTotalGSTlst = new SelectList(TotalGSTSLABMaster, "Value", "Text");

                //  ViewBag.ExistStatus = new  SelectList(ObjList, "Value", "Text",1);
                // ViewBag.ExistStatus = ObjList;

                List<decimal?> approvedQty = new List<decimal?>();
                var QtyData = objmms.PurchaseRequisitionChilds.Where(x => x.PurRequisitionNo == MGrid.IndentRefNo);
                foreach (var item in MasterList)
                {
                    var d = QtyData.SingleOrDefault(x => x.ItemNo == item.ItemNo);
                    approvedQty.Add(((d.ApprovedQty??0)-(d.OrderedQty??0)+(item.Qty??0)));
                }

                if (approvedQty.Count > 0)
                    ViewBag.ApprovedQty = approvedQty;



                //if (MasterList[0].MUId== 41428)
                //{
                //    int index = 1;
                //    //to display approved qty
                //    var QtyData = objmms.PurchaseRequisitionChilds.Where(x => x.PurRequisitionNo == MGrid.IndentRefNo);
                //    foreach (var item in MasterList)
                //    {
                //        var i = QtyData.FirstOrDefault(x => x.ItemNo == item.ItemNo && x.SNo == index);
                //        item.Qty = i.OrderedQty;
                //        approvedQty.Add(i.ApprovedQty);
                //        index++;
                //    }

                //    if (approvedQty.Count > 0)
                //        ViewBag.ApprovedQty = approvedQty;
                //}
                //else
                //{
                    
                //}
                
               

              
                var data = new GridPurchaseOrderEdit_GST()
                {
                    TotalRows = totalRows,
                    PageSize = pageSize,
                    Child = MasterList.ToList()
                };
                if (Request.IsAjaxRequest())
                {
                   
                    return View("_GridEdit_POGST", data);
                }


            }
            return View("_EmptyView");

        }


        [HttpPost]
        public ActionResult UpdateEdited_POGST(IndentPurchaserOrderNew_GST Grid)
        {
            // note : here update those PO whose does not approved.
            using (var transaction=objmms.Database.BeginTransaction())
            {
                try
                {

                    var NotApprvDetail = objmms.TbIndentPurchaseOrder_GST.Where(xk => xk.PurchaseOrderNo == Grid.Master.PurchaseOrderNo && xk.Status == "Not Approved").ToList();
                    if (NotApprvDetail.Count() > 0)
                    {

                        #region

                        tblALLRemark alrmrks = new tblALLRemark();


                        if (Grid.Master.PurchaseOrderNo != null)
                        {


                            var CheckedPoLimit = Grid.Master.CheckedBeyondPOLimit;
                            // TbindentPurchaseOrder
                            MMS.DAL.TbIndentPurchaseOrder_GST tbms = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == Grid.Master.PurchaseOrderNo).FirstOrDefault();
                            tbms.SupplierNo = Grid.Master.SupplierNo;
                            tbms.Total = Grid.Master.Total??0;
                            tbms.SubTotal1 = Grid.Master.SubTotal1 ?? 0;
                            tbms.SubTotal2 = Grid.Master.SubTotal2 ?? 0;
                            tbms.GrandTotal = Grid.Master.GrandTotal ?? 0;
                            tbms.SendTo = Grid.Master.SendTo;
                            tbms.Total_Cartage = Grid.Master.Total_Cartage ?? 0;
                            tbms.Total_CGST = Grid.Master.Total_CGST ?? 0;
                            tbms.Total_IGST = Grid.Master.Total_IGST ?? 0;
                            tbms.Total_Insurance = Grid.Master.Total_Insurance ?? 0;
                            tbms.Total_SGSTAndUTGST = Grid.Master.Total_SGSTAndUTGST ?? 0;
                            tbms.Total_Taxable = Grid.Master.Total_Taxable ?? 0;
                            tbms.Total_PAndF = Grid.Master.Total_PAndF ?? 0;
                            tbms.Status = "Pending";
                            tbms.Reference = Grid.Master.Reference;
                            tbms.Vendor_Group_Id = Grid.Master.Vendor_Group_Id;
                            tbms.PODescription = Grid.Master.PODescription;
                            tbms.Edited_Status = "Yes";
                            tbms.CheckedBeyondPOLimit = CheckedPoLimit; // here write down query to check beyond PO limit
                            tbms.SendTo = Grid.Master.SendTo;
                            tbms.ModifiedDate = System.DateTime.Now;
                            tbms.PurchaseOrderDate = Grid.Master.PurchaseOrderDate;
                            tbms.Total_TCS = Grid.Master.Total_TCS;
                            //tbms.FirstLevelApprv_Status = null;
                            //tbms.FirstLevelApprv_Remarks = null;
                            //tbms.FirstLevelApprv_Date = null;
                            //tbms.SecondLevelApprv_Remarks = null;
                            //tbms.SecondLevelApprv_Status = null;
                            //tbms.SecondLevelApprv_Date = null;
                            if (CheckedPoLimit == Convert.ToBoolean(1))
                            { tbms.SecondLevelApprv_Id = Grid.Master.SendTo; }
                            else { tbms.FirstLevelApprv_Id = Grid.Master.SendTo; }

                            objmms.Entry(tbms).State = EntityState.Modified;

                            // for saving record in TbindentPurchaseOrder_GST_History table 

                            MMS.DAL.TbIndentPurchaseOrder_GST_History MGSTHistory = new TbIndentPurchaseOrder_GST_History();
                            MGSTHistory.ProjectNo = Grid.Master.ProjectNo;
                            MGSTHistory.SupplierNo = Grid.Master.SupplierNo;
                            MGSTHistory.PurchaseOrderNo = Grid.Master.PurchaseOrderNo;
                            MGSTHistory.PurchaseOrderId = tbms.PurchaseOrderId;
                            MGSTHistory.PurchaseOrderDate = Grid.Master.PurchaseOrderDate;
                            MGSTHistory.IndentRefNo = Grid.Master.IndentRefNo;
                            MGSTHistory.Total = Grid.Master.Total;
                            MGSTHistory.SubTotal1 = Grid.Master.SubTotal1;
                            MGSTHistory.SubTotal2 = Grid.Master.SubTotal2;
                            MGSTHistory.GrandTotal = Grid.Master.GrandTotal;
                            MGSTHistory.CreatedBy = tbms.CreatedBy;
                            MGSTHistory.CreatedDate = System.DateTime.Now;
                            MGSTHistory.SendTo = Grid.Master.SendTo;
                            MGSTHistory.Status = "Pending";
                            MGSTHistory.Reference = Grid.Master.Reference;
                            MGSTHistory.Vendor_Group_Id = Grid.Master.Vendor_Group_Id;
                            MGSTHistory.Edited_Status = "Yes";
                            MGSTHistory.QuotationRefNo = Grid.Master.QuotationRefNo;
                            MGSTHistory.CheckedBeyondPOLimit = Grid.Master.CheckedBeyondPOLimit;
                            MGSTHistory.IsPORelease = Grid.Master.IsPORelease;
                            MGSTHistory.POReleaseBy = Grid.Master.POReleaseBy;
                            MGSTHistory.POReleaseDate = Grid.Master.POReleaseDate;
                            MGSTHistory.Remarks = Grid.Master.Remarks;
                            MGSTHistory.POType = tbms.POType;
                            MGSTHistory.Total_TCS = tbms.Total_TCS;
                            //MGSTHistory.ModifiedDate = System.DateTime.Now;
                            if (CheckedPoLimit == Convert.ToBoolean(1))
                            {
                                MGSTHistory.SecondLevelApprv_Id = Grid.Master.SendTo;
                                MGSTHistory.SecondLevelApprv_Status = tbms.SecondLevelApprv_Status;
                                MGSTHistory.SecondLevelApprv_Remarks = tbms.SecondLevelApprv_Remarks;
                            }
                            else
                            {
                                MGSTHistory.FirstLevelApprv_Id = Grid.Master.SendTo;
                                MGSTHistory.FirstLevelApprv_Status = tbms.FirstLevelApprv_Status;
                                MGSTHistory.FirstLevelApprv_Remarks = tbms.FirstLevelApprv_Remarks;

                            }

                            MGSTHistory.Total_PAndF = Grid.Master.Total_PAndF;
                            MGSTHistory.Total_Cartage = Grid.Master.Total_Cartage;
                            MGSTHistory.Total_Insurance = Grid.Master.Total_Insurance;
                            MGSTHistory.Total_Taxable = Grid.Master.Total_Taxable;
                            MGSTHistory.Total_CGST = Grid.Master.Total_CGST;
                            MGSTHistory.Total_SGSTAndUTGST = Grid.Master.Total_SGSTAndUTGST;
                            MGSTHistory.Total_IGST = Grid.Master.Total_IGST;
                            MGSTHistory.Total_NetAmountInWords = Grid.Master.Total_NetAmountInWords;

                            objmms.TbIndentPurchaseOrder_GST_History.Add(MGSTHistory);

                            // for adding remarks in seperate table 

                            alrmrks.CreatedDate = System.DateTime.Now;
                            alrmrks.RemarkBy = EmpId;
                            alrmrks.RemarkDate = System.DateTime.Now;
                            alrmrks.Remarks = Grid.Master.Remarks;
                            if (objmms.tblApproval_AccountType.Where(e => e.EmployeeId == EmpId).ToList().Count() > 0)
                            {
                                alrmrks.RemarkStage = objmms.tblApproval_AccountType.Where(e => e.EmployeeId == EmpId).FirstOrDefault().TypeName;
                            }
                            else
                            {
                                alrmrks.RemarkStage = "EmpType";
                            }

                            alrmrks.PurchaseOrderNo = Grid.Master.PurchaseOrderNo;
                            objmms.tblALLRemarks.Add(alrmrks);

                            // For 

                            if (Grid.Child != null)
                            {
                                foreach (var x in Grid.Child)
                                {
                                    MMS.DAL.TbIndentPurchaseOrderChild_GST tbchld = objmms.TbIndentPurchaseOrderChild_GST.Where(x1 => x1.UId == x.UId).FirstOrDefault();
                                    tbchld.Item_Description = x.Item_Description;
                                    tbchld.Rate = x.Rate.HasValue ? x.Rate : x.NewRate;
                                    tbchld.Discount = x.Discount;
                                    tbchld.Total = x.Total;
                                    tbchld.PackingChargesPercentage = x.PackingChargesPercentage;
                                    tbchld.PackingChargesAmount = x.PackingChargesAmount;
                                    tbchld.CartageTypeId = x.CartageTypeId;
                                    tbchld.Cartage_rate = x.Cartage_rate;
                                    tbchld.CartageAmount = x.CartageAmount;
                                    tbchld.SubTotal1 = x.SubTotal1;
                                    tbchld.InsuranceRate = x.InsuranceRate;
                                    tbchld.InsuranceAmount = x.InsuranceAmount;
                                    tbchld.SubTotal2 = x.SubTotal2;
                                    tbchld.TaxCode = x.TaxCode;
                                    tbchld.TaxRateType = x.TaxRateType;
                                    tbchld.CGSTPercentage = x.CGSTPercentage;
                                    tbchld.CGSTAmount = x.CGSTAmount;
                                    tbchld.SGSTPercentage = x.SGSTPercentage;
                                    tbchld.SGSTAmount = x.SGSTAmount;
                                    tbchld.IGSTPercentage = x.IGSTPercentage;
                                    tbchld.IGSTAmount = x.IGSTAmount;
                                    tbchld.TotalGSTAmount = x.TotalGSTAmount;
                                    tbchld.GrossAmount = x.GrossAmount;
                                    tbchld.NewRate = x.NewRate;
                                    tbchld.TCS_Rate = x.TCS_Rate;
                                    tbchld.TotalTCSAmount = x.TotalTCSAmount;
                                    tbchld.TotalGrosssAmount = x.TotalGrosssAmount;
                                    if (x.Qty == 0)
                                    {
                                        tbchld.OldMId = Convert.ToInt32(x.MUId);
                                        tbchld.MUId = 0;
                                    }

                                    decimal ex_itm_per = Convert.ToDecimal(objmms.tblItemMasters.Where(itm => itm.ItemID == x.ItemNo).FirstOrDefault().Excess_Item_Percentage);
                                    decimal ex_itm_qnty = Convert.ToDecimal((x.Qty * ex_itm_per) / 100);
                                    tbchld.Item_ExcessPercentage = ex_itm_per;
                                    tbchld.Item_ExcessQuantity = ex_itm_qnty;
                                    PurchaseRequisitionChild piChild = null;
                                    if (Grid.Master.UId == 41428)
                                    {

                                        piChild = objmms.PurchaseRequisitionChilds.SingleOrDefault(y => y.PurRequisitionNo == Grid.Master.IndentRefNo && y.ItemNo == x.ItemNo && y.SNo == x.MUId);
                                        if (piChild != null)
                                        {
                                            piChild.OrderedQty = (piChild.OrderedQty - tbchld.Qty) + x.Qty;
                                        }
                                    }
                                    else
                                    {
                                        piChild = objmms.PurchaseRequisitionChilds.FirstOrDefault(y => y.PurRequisitionNo == Grid.Master.IndentRefNo && y.ItemNo == x.ItemNo);
                                        if (piChild != null)
                                        {
                                            piChild.OrderedQty = (piChild.OrderedQty - tbchld.Qty) + x.Qty;
                                        }
                                    }

                                    //added on 11052018
                                    tbchld.Qty = x.Qty;
                                    objmms.Entry(tbchld).State = EntityState.Modified;
                                    objmms.Entry(piChild).State = EntityState.Modified;
                                    // Add Child Record in child history table .

                                    MMS.DAL.TbIndentPurchaseOrderChild_GST_History chldHistory = new TbIndentPurchaseOrderChild_GST_History();
                                    chldHistory.MUId = tbchld.MUId;
                                    chldHistory.Item_Description = x.Item_Description;
                                    chldHistory.IndentId = tbchld.IndentId;
                                    chldHistory.Rate = x.Rate;
                                    chldHistory.ItemNo = tbchld.ItemNo;
                                    chldHistory.Qty = tbchld.Qty;
                                    chldHistory.Receive = 0;
                                    chldHistory.Discount = x.Discount;
                                    chldHistory.Total = x.Total;
                                    chldHistory.PackingChargesPercentage = x.PackingChargesPercentage;
                                    chldHistory.PackingChargesAmount = x.PackingChargesAmount;
                                    chldHistory.CartageTypeId = x.CartageTypeId;
                                    chldHistory.Cartage_rate = x.Cartage_rate;
                                    chldHistory.CartageAmount = x.CartageAmount;
                                    chldHistory.SubTotal1 = x.SubTotal1;
                                    chldHistory.InsuranceRate = x.InsuranceRate;
                                    chldHistory.InsuranceAmount = x.InsuranceAmount;
                                    chldHistory.SubTotal2 = x.SubTotal2;
                                    chldHistory.TaxCode = x.TaxCode;
                                    chldHistory.TaxRateType = x.TaxRateType;
                                    chldHistory.CGSTPercentage = x.CGSTPercentage;
                                    chldHistory.CGSTAmount = x.CGSTAmount;
                                    chldHistory.SGSTPercentage = x.SGSTPercentage;
                                    chldHistory.SGSTAmount = x.SGSTAmount;
                                    chldHistory.IGSTPercentage = x.IGSTPercentage;
                                    chldHistory.IGSTAmount = x.IGSTAmount;
                                    chldHistory.TotalGSTAmount = x.TotalGSTAmount;
                                    chldHistory.GrossAmount = x.GrossAmount;
                                    chldHistory.NewRate = x.NewRate;

                                    objmms.TbIndentPurchaseOrderChild_GST_History.Add(chldHistory);
                                    objmms.SaveChanges();

                                }
                            }

                            //

                            // for  tblSpecificTermsAndConditions_Child
                            if (Grid.Child2 != null)
                            {


                                foreach (var x in Grid.Child2)
                                {
                                    // Saving Record in  tblSpecificTermsAndConditions_Child history table 

                                    //MMS.DAL.tblSpecificTermsAndConditions_Child_History stch = new tblSpecificTermsAndConditions_Child_History();
                                    //stch.ProjectId = Grid.Master.ProjectNo;
                                    //stch.Purchase_Order_No = Grid.Master.ProjectNo;
                                    //stch.HeaderTitle = x.HeaderTitle;
                                    //stch.STC_Description = x.STC_Description;
                                    //stch.STC_Master_ID = x.Id;
                                    //stch.CreatedBy = EmpId;
                                    //stch.CreatedDate = System.DateTime.Now;
                                    //stch.Edited_Status = "Yes";
                                    //stch.Status = "Enable";
                                    //stch.IsActive = "Yes";
                                    //objmms.tblSpecificTermsAndConditions_Child_History.Add(stch);


                                    var exist = objmms.tblSpecificTermsAndConditions_Child.Where(k => k.Purchase_Order_No == Grid.Master.PurchaseOrderNo && k.Id == x.Id).ToList().Count();
                                    if (exist > 0)
                                    {
                                        if (x.Status == "Checked")
                                        {
                                            // update checked row value 

                                            MMS.DAL.tblSpecificTermsAndConditions_Child newtbstc = objmms.tblSpecificTermsAndConditions_Child.Where(x2 => x2.Purchase_Order_No == Grid.Master.PurchaseOrderNo && x2.Id == x.Id).FirstOrDefault();
                                            newtbstc.STC_Description = x.STC_Description;
                                            newtbstc.Edited_Status = "Yes";
                                            newtbstc.Status = "Enable";
                                            newtbstc.IsActive = "Yes";
                                            newtbstc.ModifiedBy = EmpId;
                                            newtbstc.ModifiedDate = System.DateTime.Now;

                                            objmms.Entry(newtbstc).State = EntityState.Modified;
                                            objmms.SaveChanges();


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
                                    else
                                    {

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
                                            x.ProjectId = x.ProjectId;
                                            x.ModifiedDate = System.DateTime.Now;
                                            objmms.tblSpecificTermsAndConditions_Child.Add(x);
                                            objmms.SaveChanges();

                                        }
                                        else
                                        {
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
                                    // for Saving  Record in tblSpecific_Instructions_Child history table 

                                    //MMS.DAL.tblSpecific_Instructions_Child_History sich = new tblSpecific_Instructions_Child_History();
                                    //sich.ProjectId = Grid.Master.ProjectNo;
                                    //sich.Purchase_Order_No = Grid.Master.PurchaseOrderNo;
                                    //sich.Header_Title = x.Header_Title;
                                    //sich.SI_Description = x.SI_Description;
                                    //sich.CreatedBy = System.DateTime.Now;
                                    //sich.Edited_Status = "Yes";
                                    //sich.Status = "Enable";


                                    var CheckSIexist = objmms.tblSpecific_Instructions_Child.Where(k => k.Purchase_Order_No == Grid.Master.PurchaseOrderNo && k.Id == x.Id).ToList().Count();
                                    if (CheckSIexist > 0)
                                    {
                                        if (x.Status == "Checked")
                                        {
                                            // update checked row value 
                                            MMS.DAL.tblSpecific_Instructions_Child newtbsich = objmms.tblSpecific_Instructions_Child.Where(x3 => x3.Purchase_Order_No == Grid.Master.PurchaseOrderNo && x3.Id == x.Id).FirstOrDefault();
                                            newtbsich.SI_Description = x.SI_Description;
                                            newtbsich.Edited_Status = "Yes";
                                            newtbsich.Status = "Enable";
                                            newtbsich.IsActive = "Yes";
                                            newtbsich.ModifiedBy = EmpId;
                                            newtbsich.ModifiedDate = System.DateTime.Now;
                                            objmms.Entry(newtbsich).State = EntityState.Modified;
                                            objmms.SaveChanges();
                                        }

                                        else if (x.Status == "UnChecked")
                                        {
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

                                    else
                                    {

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
                                            x.ProjectId = x.ProjectId;
                                            x.ModifiedDate = System.DateTime.Now;
                                            objmms.tblSpecific_Instructions_Child.Add(x);
                                            objmms.SaveChanges();

                                        }

                                        else
                                        {
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
                                        {// Just Update checked row.
                                            MMS.DAL.tblGenral_Terms_Conditions_Child newtbGTCH = objmms.tblGenral_Terms_Conditions_Child.Where(x4 => x4.Purchase_Order_No == Grid.Master.PurchaseOrderNo && x4.Id == x.Id).FirstOrDefault();
                                            newtbGTCH.GTC_Description = x.GTC_Description;
                                            newtbGTCH.Edited_Status = "Yes";
                                            newtbGTCH.Status = "Enable";
                                            newtbGTCH.IsActive = "Yes";
                                            newtbGTCH.ModifiedBy = EmpId;
                                            newtbGTCH.ModifiedDate = System.DateTime.Now;
                                            objmms.Entry(newtbGTCH).State = EntityState.Modified;
                                            objmms.SaveChanges();
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
                                    else
                                    {

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
                                            x.ProjectId = x.ProjectId;
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
                            tbdelpo.Contact_Person_Name = Grid.Child5.Contact_Person_Name;
                            tbdelpo.Contact_Person_Mobile = Grid.Child5.Contact_Person_Mobile;
                            tbdelpo.EPContactPerID = Grid.Child5.EPContactPerID;
                            tbdelpo.ContactDivType = Grid.Child5.ContactDivType;

                            objmms.Entry(tbdelpo).State = EntityState.Modified;

                            //
                            objmms.SaveChanges();
                            transaction.Commit();
                            return Json(new { Status = "1", IndentNo = "" }, JsonRequestBehavior.AllowGet); // succefully updated


                        }
                        else
                        {
                            return Json(new { Status = "4", IndentNo = "" }, JsonRequestBehavior.AllowGet); // Something wrong in Purchase Order.
                                                                                                            // somthing missing.
                        }


                        #endregion
                    }
                    else
                    {
                        return Json(new { Status = "3", IndentNo = "" }, JsonRequestBehavior.AllowGet);
                        // PO status is NOT Approved , so can not be edited.
                    }


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ex.ToString(); // for some problem.
                    return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            
        }


        #region Get Purchase Order No. 7_3_2020
        public string Get_PurchaseOrderNo(string ProjID, string PurReqNo, DateTime PODate)
        {
            string PONo = objpro.GetPO_NO_New(ProjID, PurReqNo, PODate);
           // var PONo_lst = new { PO_Number = PONo };
            return PONo;
        }
        #endregion


        #region Purchase Order Attachment Upload
        [HttpPost]
        public ActionResult Upload_POAttachment(PIAttachmentModel PIFiles)
        {
            string error = "";
            List<Dictionary<string, string>> FileInfoObj = new List<Dictionary<string, string>>();
            string[] fileext = { ".jpeg", ".jpg", ".png", ".doc", ".docx", ".xlsx", ".xls", ".pdf" };
            if (PIFiles != null)
            {
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

                            string path = Server.MapPath("~\\PurchaseOrderAttachment\\");
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

                    error = ex.Message;
                }
            }

            return Json(new { Data = FileInfoObj, Error = error });
        }

        #endregion

        #region PO Contact Person

        public JsonResult GetContactPerson(int DType, string ProjectId)
        {
            if (DType == 1)
            {
                //var contactPersondata = objmms.tblEmployeeMasters.Select(s => new { Text = s.FName+ " "+s.LName, Value = s.TransID });
                //return Json(contactPersondata.ToList(), JsonRequestBehavior.AllowGet);
                string J = string.Empty;
                var lst = (from a in objmms.tblApproval_AccountType.Where(x => x.TypeName == "Purchase").ToList()
                           join b in objmms.tblEmployeeMasters.Where(p => p.ProjectID.Contains(ProjectId)).ToList()
                           on a.EmployeeId equals b.EmpID
                           select new 
                           {
                               Value = b.EmpID,
                               Text = b.FName + " " + b.LName
                           }).ToList();

                //J = lst.ToJSON();
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            else {
                var contactPersondata = objmms.tbl_ContactPersonMaster.Select(s => new { Text = s.PersonName, Value = s.ContactPersonCode });
                return Json(contactPersondata.ToList(), JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetPersonDesignationContact(string PID, int DivType)
        {
            if (DivType ==1) {
                tbl_ContactPersonMaster contactObj = new tbl_ContactPersonMaster();
                var emp = objmms.tblEmployeeMasters.Where(s => s.EmpID == PID).Select(s => s).FirstOrDefault();
                contactObj.ContactNumber = emp.MobileNo;
                contactObj.Designation = objmms.tblDesignations.Where(s => s.DesgID == emp.DesgID).Select(s => s.Designation).FirstOrDefault();
                return Json(contactObj, JsonRequestBehavior.AllowGet);
            }
            else {
                var personData = objmms.tbl_ContactPersonMaster.Where(s => s.ContactPersonCode == PID).Select(s => s).FirstOrDefault();
                return Json(personData, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDivisions()
        {
            var DivisionData = objmms.tblDivisionMasters.Select(s => new { Text = s.Name, Value = s.Id });
            return Json(DivisionData.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Draft PO Entry

        public ActionResult DraftPO(string PO_No)
        {
            //IndentPurchaserOrder_EditNew  decimal id
            string PONO_Edit = PO_No.Replace("#", "/");
            decimal id = objmms.TbDraftIndentPurchaseOrder_GST.Where(ko => ko.PurchaseOrderNo == PONO_Edit).FirstOrDefault().UId;


            DAL.TbDraftIndentPurchaseOrder_GST Master = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.UId == id).First();
            ViewBag.PO_No = Master.PurchaseOrderNo;
            List<DAL.TbDraftIndentPurchaseOrderChild_GST> ChildList = objmms.TbDraftIndentPurchaseOrderChild_GST.Where(x => x.MUId == Master.UId).ToList();


            string EmpId = Session["EmpID"].ToString();
            ViewBag.Date = Master.PurchaseOrderDate;
            ViewBag.EmpId = EmpId;

            ViewBag.VGId = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", Master.Vendor_Group_Id);
            //  string SendToId = objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(x => x.Purchase_Order_Indent_No == Master.PurchaseOrderNo).FirstOrDefault().Send_To;
            ViewBag.VNAME = new SelectList(objmms.tblVendorMasters.Where(x => x.PRJID.Contains(Master.ProjectNo) && x.MulRegistrationType.Contains(Master.Vendor_Group_Id)).ToList(), "VendorID", "Name", Master.SupplierNo);

            var lst = (from a2 in objmms.tblApproval_AccountType.Where(x => x.TypeName == "Purchase").ToList()
                       join b in objmms.tblEmployeeMasters.Where(p => p.ProjectID.Contains(Master.ProjectNo)).ToList()
                       on a2.EmployeeId equals b.EmpID
                       select new SelectListItem
                       {
                           Value = b.EmpID,
                           Text = b.FName + " " + b.LName
                       }).ToList();
            // string CPName = objmms.TbDelivery_Details_PO.Where(x => x.Purchase_order_No == Master.PurchaseOrderNo && x.ProjectId ==Master.ProjectNo).FirstOrDefault().Contact_Person_Name;
            //string CPNameID = objmms.tblEmployeeMasters.Where(x => (x.FName + " "+ x.LName) == CPName).FirstOrDefault().EmpID;
            string CPNameID = objmms.TbDraftDelivery_Details_PO.Where(x => x.Purchase_order_No == Master.PurchaseOrderNo && x.ProjectId == Master.ProjectNo).FirstOrDefault().EPContactPerID;
            ViewBag.DeleryPurchseEmp = new SelectList(lst, "Value", "Text", CPNameID); //b.FName + " " + b.LName


            ViewBag.VG = m.GetVendorGroupManager().FindByVendor(Master.SupplierNo).TypeID;
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var a = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.UId == id && x.ProjectNo == Master.ProjectNo).ToList();
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


            ViewBag.Employee = H2.Select(x => new SelectListItem { Value = x.EmpID, Text = x.FName + ' ' + x.LName }).ToList();
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


            var data = new DraftIndentPurchaserOrder_EditNewGST()
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

        public JsonResult GetDraftPODeliveryDetail(decimal Uid)
        {
            string PONo = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.UId == Uid).ToList().FirstOrDefault().PurchaseOrderNo;
            var lst = objmms.TbDraftDelivery_Details_PO.Where(x => x.Purchase_order_No == PONo).ToList().Select(p => new { DeliverySchedule = p.Delivery_Schedule, DeliveryAddr = p.Delivery_Address, BillingAddr = p.Billing_Address, PName = p.Contact_Person_Name, ContMobile = p.Contact_Person_Mobile, ContType = p.ContactDivType, PersonID = p.EPContactPerID }).FirstOrDefault();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult View_DraftGTCDataPOWise(decimal id)
        {
            string PONo = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.UId == id).First().PurchaseOrderNo;
            string ProjectNo = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.UId == id).First().ProjectNo;

            var a = (from pd in objmms.tblGenral_Terms_Conditions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreatedDate)
                     select new MMS.ViewModels.Genral_Terms_and_conditions
                     {
                         Id = objmms.tblDraftGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblDraftGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).FirstOrDefault().Id : pd.Id,
                         Header_Title = objmms.tblDraftGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblDraftGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).FirstOrDefault().Header_Title : pd.Header_Title,
                         GTC_Description = objmms.tblDraftGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblDraftGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).FirstOrDefault().GTC_Description : pd.GTC_Description,
                         ProjectId = ProjectNo,
                         IsHaving = objmms.tblDraftGenral_Terms_Conditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.GTC_Master_ID == pd.Id).ToList().Count() > 0 ? true : false

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

        public ActionResult View_DraftSIDataPOWise(decimal id)
        {

            string PONo = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.UId == id).First().PurchaseOrderNo;
            string ProjectNo = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.UId == id).First().ProjectNo;



            var a = (from pd in objmms.tblSpecific_Instructions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreatedDate)
                     select new Specific_Instruction_Conditions
                     {
                         Id = objmms.tblDraftSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblDraftSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).FirstOrDefault().Id : pd.Id,
                         Header_Title = objmms.tblDraftSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblDraftSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).FirstOrDefault().Header_Title : pd.Header_Title,
                         SI_Description = objmms.tblDraftSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblDraftSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).FirstOrDefault().SI_Description : pd.SI_Description,
                         ProjectId = ProjectNo,
                         IsHaving = objmms.tblDraftSpecific_Instructions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.SI_Master_ID == pd.Id).ToList().Count() > 0 ? true : false

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


        public ActionResult View_DraftSTCDataPOWise(decimal id)
        {
            string PONo = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.UId == id).First().PurchaseOrderNo;
            string ProjectNo = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.UId == id).First().ProjectNo;



            var a = (from pd in objmms.tblSpecificTermsAndConditions.Where(k => k.Isdeleted == "No").OrderBy(c => c.CreateDate)
                     select new Specific_Terms_And_Conditions
                     {
                         Id = objmms.tblDraftSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblDraftSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).FirstOrDefault().Id : pd.Id,
                         Statement_Header = objmms.tblDraftSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblDraftSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).FirstOrDefault().HeaderTitle : pd.Statement_Header,
                         ProjectId = ProjectNo,
                         STC_Description = objmms.tblDraftSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).ToList().Count() > 0 ? objmms.tblDraftSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).FirstOrDefault().STC_Description : pd.STC_Description,
                         IsHaving = objmms.tblDraftSpecificTermsAndConditions_Child.Where(x => x.IsActive == "Yes" && x.Purchase_Order_No == PONo && x.STC_Master_ID == pd.Id).ToList().Count() > 0 ? true : false

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

        public JsonResult GetDraftPODetailForEdit(string PONo)
        {
            string ProjId = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PONo).First().ProjectNo;
            List<POAndPIDetailOnPOEdit> res = new List<POAndPIDetailOnPOEdit>();
            res = objpro.GetDraftPODEditGST(ProjId, PONo);
            if (res != null)
            {
                return Json(res.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {

                return null;
            }
        }


        public ActionResult DraftGridEdit(decimal Uid, int page = 1)
        {
            const int pageSize = 100; int totalRows = 0; string IsExcise = string.Empty; string IsInsi1 = string.Empty; string Isinsi2 = string.Empty;
            string Isinsi3 = string.Empty; string Isinsi4 = string.Empty; string Iscartage1 = string.Empty; string Iscartage2 = string.Empty; string Istax = string.Empty;
            List<MMS.ViewModels.ItemDescriptionData> abss = new List<MMS.ViewModels.ItemDescriptionData>();
            List<MMS.ViewModels.ItemUnitDetail> lst = new List<ItemUnitDetail>();
            DAL.TbDraftIndentPurchaseOrder_GST MGrid = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.UId == Uid).FirstOrDefault();

            List<DAL.TbDraftIndentPurchaseOrderChild_GST> MasterList = null;
            totalRows = objmms.TbDraftIndentPurchaseOrderChild_GST.Where(x => x.MUId == Uid).ToList().Count;
            MasterList = objmms.TbDraftIndentPurchaseOrderChild_GST.Where(x => x.MUId == Uid).ToList();


            ViewBag.TCSActive = MGrid.TCSActive;

            var TCSTax = objmms.tbl_TCSTaxMaster.ToList().OrderBy(p => p.TCSRate).Select(x => new { Text = x.TCSSlab, Value = x.TCSRate }).ToList();
            ViewBag.TCS = new SelectList(TCSTax, "Value", "Text");



            if (MasterList != null)
            {
                foreach (var xx in MasterList)
                {
                    var projectid = MGrid.ProjectNo;
                    var igname = objmms.tblItemMasters.Where(x => x.ItemID == xx.ItemNo).First().ItemGroupID;
                    var itemid = xx.ItemNo;
                    DateTime datetime = DateTime.Now.Date;
                    var itemds = objmms.tblItemMasters.Where(x => x.ItemID == itemid && x.ItemGroupID == igname).Select(k => new MMS.ViewModels.ItemDescriptionData { ItemDescription = xx.Item_Description, ItemId = k.ItemID }).FirstOrDefault();
                    abss.Add(itemds);
                    var det = objmms.tblItemMasters.Where(x => x.ItemID == itemid && x.ItemGroupID == igname).Select(p => new MMS.ViewModels.ItemUnitDetail { ItemName = p.ItemName, ItemNumber = p.ItemID, UnitNumber = p.UnitID, UnitName = objmms.tblUnitMasters.Where(c => c.UnitID == p.UnitID).FirstOrDefault().UnitName }).FirstOrDefault();
                    lst.Add(det);

                    //var TotalCartage = objmms.tblMMSCartageTypes.Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
                    //ViewBag.TotalCartagetest = new SelectList(TotalCartage, "Value", "Text",xx.CartageTypeId);

                    //  IsExcise = xx.IsExcise;
                    //ViewBag.ExistStatus = new SelectList(ObjList, "Value", "Text", xx.IsExcise);




                }
                if (abss != null)
                {
                    ViewBag.ViewAdd_Desc = abss;


                }
                if (lst != null)
                {
                    ViewBag.ItemUnitDetail = lst;
                }
                var TotalCartage = objmms.tblMMSCartageTypes.Where(x => x.Status == true).Select(a => new { Text = a.CartageType, Value = a.TransId }).OrderBy(k => k.Text).ToList();
                ViewBag.TotalCartagetest = new SelectList(TotalCartage, "Value", "Text");

                var TotalGSTSLABMaster = objmms.tblGST_SplitTaxMaster.ToList().Select(n => new { Text = n.TaxRateType, Value = n.TaxCode }).OrderBy(s => s.Text).ToList();
                ViewBag.MyTotalGSTlst = new SelectList(TotalGSTSLABMaster, "Value", "Text");

                //  ViewBag.ExistStatus = new  SelectList(ObjList, "Value", "Text",1);
                // ViewBag.ExistStatus = ObjList;

                List<decimal?> approvedQty = new List<decimal?>();
                var QtyData = objmms.PurchaseRequisitionChilds.Where(x => x.PurRequisitionNo == MGrid.IndentRefNo);
                foreach (var item in MasterList)
                {
                    var d = QtyData.SingleOrDefault(x => x.ItemNo == item.ItemNo);
                    approvedQty.Add(((d.ApprovedQty ?? 0) - (d.OrderedQty ?? 0) + (item.Qty ?? 0)));
                }

                if (approvedQty.Count > 0)
                    ViewBag.ApprovedQty = approvedQty;



                //if (MasterList[0].MUId== 41428)
                //{
                //    int index = 1;
                //    //to display approved qty
                //    var QtyData = objmms.PurchaseRequisitionChilds.Where(x => x.PurRequisitionNo == MGrid.IndentRefNo);
                //    foreach (var item in MasterList)
                //    {
                //        var i = QtyData.FirstOrDefault(x => x.ItemNo == item.ItemNo && x.SNo == index);
                //        item.Qty = i.OrderedQty;
                //        approvedQty.Add(i.ApprovedQty);
                //        index++;
                //    }

                //    if (approvedQty.Count > 0)
                //        ViewBag.ApprovedQty = approvedQty;
                //}
                //else
                //{

                //}




                var data = new GridDraftPurchaseOrderEdit_GST()
                {
                    TotalRows = totalRows,
                    PageSize = pageSize,
                    Child = MasterList.ToList()
                };
                if (Request.IsAjaxRequest())
                {

                    return View("_GridEdit_DraftPOGST", data);
                }


            }
            return View("_EmptyView");

        }


        public ActionResult PendingPODraft()
        {
            ViewBag.EmpId = Session["EmpID"].ToString();
            return View();
        }

        public ActionResult DraftPOGrid(string ProjectId, string PONumber = "", string VendorId = "")
        {
            string empId = Session["EmpID"].ToString(); bool TrueVal = true;
            var a = objmms.TbDraftIndentPurchaseOrder_GST.Where(i => i.IsPOEnter == null && i.ProjectNo == ProjectId && (i.CreatedBy == empId || i.SendTo == empId || i.FirstLevelApprv_Id == empId || i.SecondLevelApprv_Id == empId)).OrderByDescending(c => c.CreatedDate).Select(xx => new MMS.Controllers.Print_Slip_For_ApprovalController.Getdata
            {
                //Id = xx.Id,
                Purchase_Order_Indent_No = xx.PurchaseOrderNo,
                CreatedDate = xx.CreatedDate,
                //Send_To = xx.Status == "Not Approved" && xx.SecondLevelApprv_Id == null ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().LName
                //       : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().LName,
                Vendor_Group_Id = xx.Vendor_Group_Id,
                Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.SupplierNo).FirstOrDefault().Name,
                POCreatedDate = xx.CreatedDate,
                Status = xx.Status,
                // FirstLevelApprv_Date = xx.SecondLevelApprv_Status == "Not Approved" ? xx.SecondLevelApprv_Date : xx.FirstLevelApprv_Date,
                QuotID = objmms.tblMMSQuotations.Where(x => x.PurchaseReqNo == xx.IndentRefNo).FirstOrDefault().TransId,
                DiffStagePerson = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == xx.PurchaseOrderNo).FirstOrDefault().CheckedBeyondPOLimit == TrueVal ? "Purchase" : "PMC",
                Stage = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SendTo).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SendTo).FirstOrDefault().LName

            }).ToList();

            if (VendorId != "Select Vendor")
            {
                a = a.Where(x => !string.IsNullOrEmpty(x.Vendor_Name) && x.Vendor_Name.Contains(VendorId)).ToList();
            }

            if (!string.IsNullOrEmpty(PONumber))
            {
                a = a.Where(x => x.Purchase_Order_Indent_No.Contains(PONumber)).ToList();
            }

            var totalRows = a.Count();

            var data = new PrinSlip_Approval_VMGST()
            {
                TotalRows = totalRows,
                PageSize = 3500,
                GetDetails_Approved_PO_Slip = a
            };


            if (data != null && data.TotalRows != 0)
            {


                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PartialPendingDraftPO", data);
                }

                else
                {
                    return PartialView("_EmptyView");
                }

            }

            else
            {
                return View("_EmptyView");
            }



        }

        #endregion

    }
}