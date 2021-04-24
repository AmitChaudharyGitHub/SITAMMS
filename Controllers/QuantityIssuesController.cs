using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.Models;
using MMS.DAL;
using System.Data;
using MMS_P.ViewModels;
using MMS.ViewModels;
using System.Data.Entity;
using DataAccessLayer;
using System.Net.Http.Headers;
using System.Globalization;
using System.Web.Script.Serialization;
using MMS.Helpers;

namespace MMS.Controllers
{
    public class QuantityIssuesController : MySiteController
    {
        FactoryManager m = new FactoryManager();
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: QuantityIssues

        [Authorize]
        public ActionResult Index(string Indent=null)
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            string ProjectId = Session["ProId"].ToString();
            //Item Indent Name
            ViewBag.indentno = new SelectList(objmms.tblIndentRequisitions, "Id", "IndentNo");

            var a = objmms.tblIndentRequisitions.Where(b => b.IndentSent == "No").OrderBy(c => c.IndentNo).ToList();
            ViewBag.indentno = a.Distinct<tblIndentRequisition>().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.IndentNo
            });

            var f = objmms.tblPcContractorMasters.Where(c => c.PRJID.Contains(ProjectId)).OrderBy(e => e.Name).ToList();

            var t = f.Distinct<tblPcContractorMaster>().Select(x => new SelectListItem
            {
                Value = x.PcContractorID?.ToString().Trim(),
                Text = x.Name?.ToString().Trim()
            });

         //   ViewBag.pettycontractor = t;
           
            //ViewBag.pettycontractor = new SelectList(m.GetPcContractorMasterManager().FindALL(), "PcContractorID", "Name");

            //Item Project Name
            //ViewBag.prjtid = new SelectList(unitOfWork.tblProjectRepository.Get(), "PRJID", "ProjectName");
            //Item Group Name
            ViewBag.itemgroupname = new SelectList(unitOfWork.tblItemgroupmasterRepository.Get(), "ItemGroupID", "GroupName");
            //Select Block Name
            ViewBag.blockname = new SelectList(unitOfWork.tblBlockmasterRepository.Get(), "BlockId", "BlockName");
            List<SelectListItem> ObjList = new List<SelectListItem>()
                   {
                new SelectListItem { Text = "Select PC", Value = "1" },
                new SelectListItem { Text = "Other", Value = "0" },
                   };
            ViewBag.SelectPCType = ObjList;

            ViewBag.SelectType = objmms.tblFinancialSelectionTypes.Where(x=>x.Status==true).Select(p=> new SelectListItem { Value=p.TransId.ToString(), Text=p.FinancialType }).OrderBy(k=>k.Text);

            if (!string.IsNullOrEmpty(Indent))
            {
                var indentDate = objmms.tblIndentRequisitions.FirstOrDefault(x => x.IndentNo == Indent).IndentDate;
                TempData["indent"] = Indent;
                ViewBag.IndentDate = indentDate.Value.ToString("dd-MMM-yyyy");
            }
            return View();
        }

        public JsonResult GetIndentDate(int IndentNo)
        {
            if (IndentNo != 0)
            {
                var indentDate=objmms.tblIndentRequisitions.FirstOrDefault(x => x.Id == IndentNo).IndentDate.Value.ToString("dd-MMM-yyyy");

                return Json(new { IndentDate = indentDate }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { IndentDate = "" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getprojectempblokflorcodes(int id)
        {
            tblIndentRequisition E = objmms.tblIndentRequisitions.Where(s => s.Id == id).FirstOrDefault();
            var EO = new { Pjtnames = E.ProjectName, empnames = E.EmployeeName };
            //var Io = new { itemsids = E.ItemID };
            //TempData["StoreIds"] = Io;
            string jsonString = EO.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNatureOfWork()
        {
            var data = Procedure.GetData<SelectListItem>("GetNatureOfWork");
            return Json(data.ToJSON(),JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMachines()
        {
            var data = Procedure.GetData<SelectListItem>("GetMachines");
            return Json(data.ToJSON(), JsonRequestBehavior.AllowGet);
        }
        

        // Get State from DB by country ID
        public IList<tblIndentRequisition> Getprojectcode(string indntid)
        {
            return objmms.tblIndentRequisitions.Where(m => m.IndentNo == indntid).ToList();
        }

        // get indent no.

        public JsonResult GetIndentno(string id)
        {
            List<SelectListItem> groups = new List<SelectListItem>();
            var groupList = this.Getindentname(Convert.ToString(id));
           
            var a = groupList.Where(b => b.IndentSent == "No" && b.Apporoved_Status == "Yes" || b.IndentSent == "Yes" && b.BalanceApprovedQty_After_Issue_Qty > 0).OrderBy(c => c.Id).ToList();

            var indentData = a.Distinct<tblIndentRequisition>().Select(m => new SelectListItem
            {
                Text = m.IndentNo,
                Value = m.Id.ToString(),
            });
            return Json(indentData, JsonRequestBehavior.AllowGet);
        }

        // Get State from DB by Project ID
        public IList<tblIndentRequisition> Getindentname(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            return objmms.tblIndentRequisitions.Where(m => m.ProjectID == ProjectId && m.DivisionId==divisionId).ToList();
        }

        // Get Approved And Unaapproved Data Here
        public JsonResult GetIndentnocheck(string id)
        {
            if (id == "Approved" && Session["EmpID"] != null)
            {
                string empId = Session["EmpID"].ToString();

                // var a = objmms.tblIndentRequisitions.Where(b => b.IndentSent == "No" && b.Apporoved_Status == "Yes" || b.IndentSent=="Yes" && b.Approved_Quantity > 0).GroupBy(b => b.ProjectID).Select(b => new {ProjectId=b.Key }).ToList();

                var aaa = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();

                var t = aaa.Select(x => new SelectListItem
                {
                    Value = x.ProjectID.ToString(),
                    // Text = x.ProjectID.ToString()
                    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
                });
                //ViewBag.prjtid = t;

                //var indentData = a.Select(m => new SelectListItem
                //{
                //    Text = objmms.tblProjectMasters.Where(x => x.PRJID == m.ProjectId).First().ProjectName,
                //    Value = m.ProjectId,  
                //}).OrderBy(x => x.Text);

                return Json(t, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var a = objmms.tblIndentRequisitions.Where(b => b.IndentSent == "No" && b.Apporoved_Status == "Yes" || b.IndentSent == "Yes" && b.Approved_Quantity > 0).GroupBy(b => b.ProjectID).Select(b => new { ProjectId = b.Key }).ToList();

                //var indentData = a.Select(m => new SelectListItem
                //{
                //    Text = objmms.tblProjectMasters.Where(x => x.PRJID == m.ProjectId).First().ProjectName,
                //    Value = m.ProjectId,
                //}).OrderBy(x => x.Text);

                string empId = Session["EmpID"].ToString();
                var aaa = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();

                var t = aaa.Select(x => new SelectListItem
                {
                    Value = x.ProjectID.ToString(),
                    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
                });

                return Json(t, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult QtyVerification(string ProjectId, List<VMItemData> data,string Date)
        {
            if (ProjectId != "" && data != null && Date != "")
            {
                DateTime dateTime = DateTime.ParseExact(Date, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                string Itemname = "";
                decimal Qty = new decimal(0);
                bool isValid = true;
                foreach (var item in data)
                {
                    var balQty = objmms.tblReceiveDatas.Where(x => x.ProjectId == ProjectId && x.TypeDate <= dateTime && x.ItemId == item.ItemId && x.BalanceQuantity > 0).Sum(x => x.BalanceQuantity);
                    if (balQty< item.Qty)
                    {
                        Itemname = objmms.tblItemMasters.SingleOrDefault(x => x.ItemID == item.ItemId).ItemName;
                        Qty = balQty.Value;
                        isValid = false;
                        break;
                    }

                    Qty = balQty == null ? 0 : balQty.Value;
                }

                if(isValid==true && Qty>0)
                return Json(new { Status = 1 }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Status = 2,Msg="Item \""+Itemname+"\" Total Qty till Issue Date : "+Qty }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { Status = 3, Qty = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        // Get Data from DB by Project ID
        //public IList<tblIndentRequisition> Getindentnamess(string ProjectId)
        //{
        //    return objmms.tblIndentRequisitions.Where(m => m.ProjectID == ProjectId).ToList();
        //}

        public ActionResult Details(string id)
        {
            var indentNo = id;

            var inedntexistances = objmms.tblIndentRequisitions.Where(s => s.IndentNo == indentNo && s.IssueQuantity > 0 && s.IndentSent == "Yes").Select(l => l.IndentSent).ToList();
            //string checkingIndent = inedntexistances.First().;
            if (inedntexistances.Count() != 0)
            {
                TempData["dataexist"] = "Yes";

                //var chkQty = objmms.tblItemCurrentStocks.Where(x => x.ProjectNo == x.ProjectNo && x.ItemNo == x.ItemNo && x.Qty > 0).Select(l => l.Qty).ToList();

                var a1 = objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo && m.BalanceApprovedQty_After_Issue_Qty > 0).OrderBy(c => c.Id).Select(xx => new Iss
                {
                    Id = xx.Id,
                    ItemGroupID = xx.ItemGroupID,
                    ItemGroupName = xx.ItemGroupName,
                    ItemName = xx.ItemName,
                    UnitID = xx.UnitID,
                    Make = xx.Make,
                    PartNo = xx.PartNo,
                    Quantity = xx.Quantity,
                    BalanceQuantity = xx.BalanceQuantity,
                    Approved_Quantity = xx.Approved_Quantity,
                    ItemID = xx.ItemID,
                    BalanceApprovedQty_After_Issue_Qty = xx.BalanceApprovedQty_After_Issue_Qty,
                    // Available_Quantity = objmms.tblItemCurrentStocks.Where(x => x.ProjectNo == xx.ProjectID && x.ItemNo == xx.ItemID).FirstOrDefault().Qty ?? 0
                    Available_Quantity = (from a in objmms.tblReceiveDatas.Where(x => x.ProjectId == xx.ProjectID && x.ItemGroupId == xx.ItemGroupID && x.ItemId == xx.ItemID).ToList()
                                          group a by a.TransId into g
                                          select new { SUM = g.Sum(o => o.BalanceQuantity) }).Sum(l => l.SUM) ?? 0


                }).ToList();
                //var a1 = objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo).OrderBy(c => c.Id).ToList();    
               var totalRows = a1.Count();

                var data1 = new VMIndentRequisionses()
                {
                    TotalRows = totalRows,
                    PageSize = 250000,
                    Indents = a1
                };

                if (data1 != null && data1.TotalRows != 0)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialIndentView", data1);
                    }
                    else
                    {
                        return PartialView("_PartialIndentView", data1);
                    }
                }
                else
                {
                    return View("_EmptyView");
                }
            }
            else
            {
                //var chkQty = objmms.tblItemCurrentStocks.Where(x => x.ProjectNo == x.ProjectNo && x.ItemNo == x.ItemNo && x.Qty > 0).Select(l => l.Qty).ToList();
                //foreach (var Checkin in chkQty)
                //{
                //    decimal prints = Convert.ToDecimal(Checkin);
                //}
                var a1 = objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo && m.BalanceApprovedQty_After_Issue_Qty > 0).OrderBy(c => c.Id).Select(xx => new Iss
                {
                    Id = xx.Id,
                    ItemGroupID = xx.ItemGroupID,
                    ItemGroupName = xx.ItemGroupName,
                    ItemName = xx.ItemName,
                    UnitID = xx.UnitID,
                    Make = xx.Make,
                    PartNo = xx.PartNo,
                    Quantity = xx.Quantity,
                    BalanceQuantity = xx.BalanceQuantity,
                    Approved_Quantity = xx.Approved_Quantity,
                    ItemID = xx.ItemID,
                    BalanceApprovedQty_After_Issue_Qty = xx.BalanceApprovedQty_After_Issue_Qty,
                    //  Available_Quantity = objmms.tblItemCurrentStocks.Where(x => x.ProjectNo == xx.ProjectID && x.ItemNo == xx.ItemID).FirstOrDefault().Qty ?? 0
                    Available_Quantity = (from a in objmms.tblReceiveDatas.Where(x => x.ProjectId == xx.ProjectID && x.ItemGroupId == xx.ItemGroupID && x.ItemId == xx.ItemID).ToList()
                                          group a by a.TransId into g
                                          select new { SUM = g.Sum(o => o.BalanceQuantity) }).Sum(l => l.SUM) ?? 0


                }).ToList();

                //var a1 = objmms.tblIndentRequisitions.Where(m => m.IndentNo == indentNo).OrderBy(c => c.Id).ToList();            
                var totalRows = a1.Count();

                var data1 = new VMIndentRequisionses()
                {
                    TotalRows = totalRows,
                    PageSize = 25,
                    Indents = a1
                };

                if (data1 != null && data1.TotalRows != 0)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialIndentView", data1);
                    }
                    else
                    {
                        return PartialView("_PartialIndentView", data1);
                    }
                }
                else
                {
                    return View("_EmptyView");
                }
            }
        }

        public class Iss
        {
            public int Id { get; set; }
            public string ProjectID { get; set; }
            public string ProjectName { get; set; }
            public string IndentNo { get; set; }
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string ItemGroupID { get; set; }
            public string ItemGroupName { get; set; }
            public string ItemID { get; set; }
            public string ItemName { get; set; }
            public string UnitID { get; set; }
            public string Make { get; set; }
            public string PartNo { get; set; }
            public Nullable<decimal> Quantity { get; set; }
            public Nullable<decimal> BalanceQuantity { get; set; }
            public Nullable<decimal> IssueQuantity { get; set; }

            public Nullable<decimal> Approved_Quantity { get; set; }
            public string Apporoved_By { get; set; }
            public string Apporoved_Status { get; set; }
            public Nullable<decimal> BalanceApprovedQty_After_Issue_Qty { get; set; }
            // StockName = rt.StockName,&& pd.ProjectID ==od.ProjectNo
            public decimal? Available_Quantity { get; set; }
            //public SelectList Condition { get; set; }
        }
        //public class Condition
        //{
        //    public int ID { get; set; }
        //    public string Condition_Type { get; set; }
        //}
        //public SelectList GetConditionType()
        //{
        //    List<Condition> type = new List<Condition>();
        //    type.Add(new Condition { ID = 1, Condition_Type = "Debited" });
        //    type.Add(new Condition { ID = 2, Condition_Type = "Non Debited" });
        //    type.Add(new Condition { ID = 3, Condition_Type = "Returnable" });
        //    SelectList objMStatus = new SelectList(type, "ID", "Condition_Type");
        //    return objMStatus;
        //}
        public ActionResult UpdateGridData(string gridData, string DivType)
        {
            var log = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(gridData);
            if (log != null)
            {
                foreach (var ind in log)
                {
                    if (Convert.ToDecimal(ind.Value[11].ToString()) != 0)
                    {
                        if (Convert.ToDecimal(ind.Value[11].ToString()) <= Convert.ToDecimal(ind.Value[9].ToString()))
                        {

                            if (Convert.ToDecimal(ind.Value[11].ToString()) <= Convert.ToDecimal(ind.Value[10].ToString()))
                            {

                                var UID = int.Parse(ind.Value[0]);


                                // var avilqty = Convert.ToDecimal(ind.Value[8].ToString()) - Convert.ToDecimal(ind.Value[9].ToString());

                                tblIndentRequisition e = objmms.tblIndentRequisitions.Where(a => a.Id == UID).First();
                                e.IssueQuantity = Convert.ToDecimal(ind.Value[11].ToString());

                                e.BalanceApprovedQty_After_Issue_Qty = Convert.ToDecimal(ind.Value[9].ToString()) - Convert.ToDecimal(ind.Value[11].ToString());
                                e.IndentSent = "Yes";
                                e.Status = "Approved";
                                var itmids = e.ItemID;
                                var gh = Convert.ToDecimal(ind.Value[10].ToString()) - Convert.ToDecimal(ind.Value[11].ToString());
                                //tblIndentRequisition e = new tblIndentRequisition() { Id = Convert.ToInt32(ind.Value[0]), ItemGroupName = ind.Value[1].ToString(), ItemName = ind.Value[2].ToString(), IssueQuantity = Convert.ToDecimal(ind.Value[7].ToString()), Status = "Approved" };
                                //_dbcontext.tblEmployees.Add(e);

                                //objmms.Entry(e).State = EntityState.Modified;

                                // ADD Or INSERT ISSUE QUANTITY IN TABLE Here
                                if (Convert.ToDecimal(ind.Value[11]) > 0)
                                {
                                    List<ns_tbl_IssueQuantity> obj = new List<ns_tbl_IssueQuantity>();
                                    decimal RemQty = 0; RemQty = Convert.ToDecimal(ind.Value[11]); decimal MyBalQty = Convert.ToDecimal(ind.Value[9]);
                                    decimal lastBalqty = 0; decimal RemainIssueQty = 0; string ProjId = ind.Value[12], ItemGrpid = ind.Value[1], ItmId = ind.Value[3];
                                    decimal RemIssueQty = Convert.ToDecimal(ind.Value[11]); decimal AvailRemQty = 0;
                                    var list = objmms.tblReceiveDatas.Where(x => x.ProjectId == ProjId && x.ItemGroupId == ItemGrpid && x.ItemId == ItmId && x.BalanceQuantity > 0).OrderBy(p => p.TypeDate).ToList();
                                    foreach (var item in list)
                                    {
                                        ns_tbl_IssueQuantity chobj = new ns_tbl_IssueQuantity();
                                        var balqty = item.BalanceQuantity;

                                        decimal RecvQty = Convert.ToDecimal(item.BalanceQuantity); decimal a = 0;


                                        chobj.BlockId = ind.Value[21] == "" ? 0 : Convert.ToInt32(ind.Value[21]);
                                        chobj.BlockName = ind.Value[22] == "" ? null : ind.Value[22];
                                        chobj.FloorId = ind.Value[23] == "" ? 0 : Convert.ToInt32(ind.Value[23]);
                                        chobj.FloorName = ind.Value[24] == "" ? null : ind.Value[24];
                                        chobj.IndentType = ind.Value[25];
                                        chobj.IndentOtherDesc = ind.Value[26];
                                        chobj.NatureOfWorkId = ind.Value[27];
                                        //add line on 18/02/2019
                                        chobj.IssueBill_No = ind.Value[29];
                                        //end
                                        chobj.MachineId = string.IsNullOrEmpty(ind.Value[28])?0:Convert.ToInt32(ind.Value[28]);
                                        if (RecvQty >= RemIssueQty)
                                        {
                                            a = RecvQty - RemIssueQty;
                                            decimal iq = RemIssueQty;
                                            decimal qt = RecvQty;
                                            decimal bq = a;
                                            decimal aq = RecvQty;
                                            decimal aiq = a;

                                            #region
                                            string it_id = ind.Value[3];
                                            chobj.ProjectID = ind.Value[12];
                                            chobj.ProjectName = ind.Value[13];
                                            chobj.IndentNo = ind.Value[14];
                                            chobj.EmployeeName = ind.Value[15];
                                            chobj.EmployeeID = e.EmployeeID;
                                            chobj.Status = ind.Value[17];
                                            chobj.PcContractorId = ind.Value[18];
                                            chobj.ItemGroupID = ind.Value[1];
                                            chobj.ItemGroupName = ind.Value[2];
                                            chobj.ItemID = ind.Value[3];
                                            chobj.ItemName = objmms.tblItemMasters.Where(x => x.ItemID == it_id).FirstOrDefault().ItemName;      //ind.Value[4];
                                            chobj.UnitID = ind.Value[5];
                                            chobj.Make = ind.Value[6];
                                            chobj.PartNo = ind.Value[7];
                                            chobj.Quantity = qt;

                                            chobj.BalanceQuantity = bq;
                                            chobj.AvailableQuantity = aq;
                                            chobj.AfterIssue_AvlQty_Stock = aiq;

                                            chobj.SessionId = "1";
                                            chobj.SiteId = Session["ProjectssId"].ToString();
                                            chobj.IssuedBy = Convert.ToString(Session["EmpID"]);
                                            chobj.CreatedDate = Convert.ToDateTime(ind.Value[16]);
                                            chobj.ModifiedDate = DateTime.Now;
                                            chobj.Issue_Date = Convert.ToDateTime(ind.Value[16]);
                                            chobj.Gate_ReceiveDate = item.TypeDate;
                                            chobj.GateEntry_UId = item.TransId;
                                            chobj.Receive_Rate = item.Rate;
                                            chobj.Issue_Type = "GateEntry_Issue";
                                            chobj.IssueQuantity = iq;
                                            chobj.Optional_Name = ind.Value[19];
                                            chobj.Financial_Type = Convert.ToInt32(ind.Value[20]);
                                            chobj.WorkTypeCode = DivType;
                                            obj.Add(chobj);
                                            // for Updating TblReceiveData
                                            decimal TrId = item.TransId;
                                            tblReceiveData tdr = objmms.tblReceiveDatas.Where(x => x.ProjectId == ProjId && x.ItemGroupId == ItemGrpid && x.ItemId == ItmId && x.TransId == TrId).First();
                                            tdr.BalanceQuantity = qt - iq;
                                            item.IssueQuantity = item.IssueQuantity + iq;
                                            tdr.ModifiedDate = System.DateTime.Now;
                                            objmms.Entry(tdr).State = EntityState.Modified;
                                            // End here
                                            // stock master table updation here
                                            if (itmids != null && chobj.ProjectID != null)
                                            {
                                                tblItemCurrentStock bg = objmms.tblItemCurrentStocks.Where(l => l.ItemNo == itmids && l.ProjectNo == chobj.ProjectID).First();
                                                bg.Qty = Convert.ToDecimal(bg.Qty - iq);
                                                objmms.Entry(bg).State = EntityState.Modified;

                                            }
                                            else
                                            {
                                                TempData["errorUpdatingCurrentstock"] = "Yes";
                                            }
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
                                            string it_id = ind.Value[3];
                                            chobj.ProjectID = ind.Value[12];
                                            chobj.ProjectName = ind.Value[13];
                                            chobj.IndentNo = ind.Value[14];
                                            chobj.EmployeeName = ind.Value[15];
                                            chobj.EmployeeID = e.EmployeeID;
                                            chobj.Status = ind.Value[17];
                                            chobj.PcContractorId = ind.Value[18];
                                            chobj.ItemGroupID = ind.Value[1];
                                            chobj.ItemGroupName = ind.Value[2];
                                            chobj.ItemID = ind.Value[3];
                                            chobj.ItemName = objmms.tblItemMasters.Where(x => x.ItemID == it_id).FirstOrDefault().ItemName;   //ind.Value[4];
                                            chobj.UnitID = ind.Value[5];
                                            chobj.Make = ind.Value[6];
                                            chobj.PartNo = ind.Value[7];
                                            chobj.Quantity = qt;

                                            chobj.BalanceQuantity = bq;
                                            chobj.AvailableQuantity = aq;
                                            chobj.AfterIssue_AvlQty_Stock = aiq;
                                            chobj.IssueQuantity = iq;
                                            chobj.SessionId = "1";
                                            chobj.SiteId = Session["ProjectssId"].ToString();
                                            chobj.IssuedBy = Convert.ToString(Session["EmpID"]);
                                            chobj.CreatedDate = Convert.ToDateTime(ind.Value[16]);
                                            chobj.ModifiedDate = DateTime.Now;
                                            chobj.Issue_Date = Convert.ToDateTime(ind.Value[16]);
                                            chobj.Gate_ReceiveDate = item.TypeDate;
                                            chobj.GateEntry_UId = item.TransId;
                                            chobj.Receive_Rate = item.Rate;
                                            chobj.Issue_Type = "GateEntry_Issue";
                                            chobj.Optional_Name = ind.Value[19];
                                            chobj.Financial_Type = Convert.ToInt32(ind.Value[20]);
                                            chobj.WorkTypeCode = DivType;
                                            obj.Add(chobj);

                                            // for Updating TblReceiveData
                                            decimal TrId = item.TransId;
                                            tblReceiveData tdr = objmms.tblReceiveDatas.Where(x => x.ProjectId == ProjId && x.ItemGroupId == ItemGrpid && x.ItemId == ItmId && x.TransId == TrId).First();
                                            tdr.BalanceQuantity = qt - iq;
                                            item.IssueQuantity = item.IssueQuantity + iq;
                                            tdr.ModifiedDate = System.DateTime.Now;
                                            objmms.Entry(tdr).State = EntityState.Modified;
                                            // End here
                                            // stock master table updation here
                                            if (itmids != null && chobj.ProjectID != null)
                                            {
                                                tblItemCurrentStock bg = objmms.tblItemCurrentStocks.Where(l => l.ItemNo == itmids && l.ProjectNo == chobj.ProjectID).First();
                                                bg.Qty = Convert.ToDecimal(bg.Qty - iq);
                                                objmms.Entry(bg).State = EntityState.Modified;

                                            }
                                            else
                                            {
                                                TempData["errorUpdatingCurrentstock"] = "Yes";
                                            }
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

                                    objmms.ns_tbl_IssueQuantity.AddRange(obj);
                                }


                            }
                            else
                            {
                                return Json("2", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            TempData["BalanceDataCheck"] = "Yes";
                            return Json("1", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                    }

                }
                objmms.SaveChanges();
            }
            return Json("Update Successfully");
        }

        // here code for view quantity issues


        // Here Code is for Another View
        public ActionResult View_To_Quantity_Issues()
        {
            string empId = Session["EmpID"].ToString();

            var aaa = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            var t = aaa.Select(x => new SelectListItem
            {
                Value = x.ProjectID.ToString(),
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            });

            ViewBag.prjtid = t;

            //var plist = this.objmms.tblIndentRequisitions.Select(x => new SelectListItem
            //{
            //    Value = x.ProjectID,
            //    Text = x.ProjectName
            //}).Distinct();
            //ViewBag.prjtid = plist;

            //ViewBag.prjtid2 = new SelectList(objmms.tblProjectMasters, "PRJID", "ProjectName");
            return View();
        }

        public ActionResult _PartialView_For_View_Quantity_Issue(string Status = null, string PId = null)
        {

            if (Status == "Yes")
            {
                var a = (from pd in objmms.tblIndentRequisitions.Where(i => i.IndentSent == Status && i.ProjectID == PId && i.IssueQuantity > 0).OrderBy(c => c.Id)
                         select new ViewTo_Issues_Quantity_Items
                         {
                             Id = pd.Id,
                             IndentNo = pd.IndentNo,
                             ItemGroupName = pd.ItemGroupName,
                             ItemName = pd.ItemName,
                             UnitID = pd.UnitID,
                             Make = pd.Make,
                             PartNo = pd.PartNo,
                             Quantity = pd.Quantity,
                             BalanceQuantity = pd.BalanceQuantity,
                             Approved_Quantity = pd.Approved_Quantity,
                             IssueQuantity = pd.IssueQuantity,
                             CreatedDate = pd.CreatedDate
                         }).ToList();

                var totalRows = a.Count();

                var data = new VM_ViewTo_Issues_Quantity_Items()
                {
                    TotalRows = totalRows,
                    PageSize = 25,
                    Issuesitems = a.ToList()
                };
                if (data != null && data.TotalRows != 0)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialView_For_View_Quantity_Issue", data);
                    }

                    else
                    {
                        return PartialView("_PartialView_For_View_Quantity_Issue", data);
                    }
                }
                else
                {
                    return View("_EmptyView");
                }
            }
            else
            {
                var a = (from pd in objmms.tblIndentRequisitions.Where(i => i.IndentSent == "No" && i.ProjectID == PId && i.IssueQuantity > 0).OrderBy(c => c.Id)
                         select new ViewTo_Issues_Quantity_Items
                         {
                             Id = pd.Id,
                             IndentNo = pd.IndentNo,
                             ItemGroupName = pd.ItemGroupName,
                             ItemName = pd.ItemName,
                             UnitID = pd.UnitID,
                             Make = pd.Make,
                             PartNo = pd.PartNo,
                             Quantity = pd.Quantity,
                             BalanceQuantity = pd.BalanceQuantity,
                             Approved_Quantity = pd.Approved_Quantity,
                             IssueQuantity = pd.IssueQuantity,
                             CreatedDate = pd.CreatedDate
                         }).ToList();

                var totalRows = a.Count();

                var data = new VM_ViewTo_Issues_Quantity_Items()
                {
                    TotalRows = totalRows,
                    PageSize = 25,
                    Issuesitems = a.ToList()
                };

                if (data != null && data.TotalRows != 0)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialView_For_View_Quantity_Issue", data);
                    }
                    else
                    {
                        return PartialView("_PartialView_For_View_Quantity_Issue", data);
                    }
                }
                else
                {
                    return View("_EmptyView");
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }

        //public JsonResult GetPCVendorByProjectId(string ProjectId)
        //{
        //    List<SelectListItem> groups = new List<SelectListItem>();

        //    string Baseurl = "http://208.109.9.215/SITACENTRAL/";
        //    using (var client = new System.Net.Http.HttpClient())
        //    {
        //        client.BaseAddress = new Uri(Baseurl);
        //        client.DefaultRequestHeaders.Clear();
        //        //Define request data format  
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        var responseTask = client.GetAsync("api/PMCDropdown/BindPCByProjectId?ProjectId=" + ProjectId);
        //        responseTask.Wait();
        //        var result = responseTask.Result;
        //        if (result.IsSuccessStatusCode)
        //        {

        //            string stringData = result.Content.ReadAsStringAsync().Result;

        //            return Json(stringData, JsonRequestBehavior.AllowGet);
        //        }
        //        else {
        //            return Json(null, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}

        //public JsonResult GetPCVendorByProjectId1(string ProjectId)
        //{
        //    List<SelectListItem> groups = new List<SelectListItem>();

        //    string Baseurl = "http://208.109.9.215/SITACENTRAL/";
        //    using (var client = new System.Net.Http.HttpClient())
        //    {
        //        client.BaseAddress = new Uri(Baseurl);
        //        client.DefaultRequestHeaders.Clear();
        //        //Define request data format  
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        var responseTask = client.GetAsync("api/PMCDropdown/BindPCByProjectId?ProjectId=" + ProjectId);
        //        responseTask.Wait();
        //        var result = responseTask.Result;
        //        if (result.IsSuccessStatusCode)
        //        {

        //            string stringData = result.Content.ReadAsStringAsync().Result;

        //            var model = new JavaScriptSerializer().Deserialize<List<DropDownListItem>>(stringData);
        //            var issuedPCData = objmms.ns_tbl_IssueQuantity.Where(x=>x.ProjectID==ProjectId && x.PcContractorId!=null && x.PcContractorId != "").Select(x=>x.PcContractorId).ToArray();

        //            return Json(stringData, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            return Json(null, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}

        public JsonResult GetPCVendorByProjectId(string ProjectId, string DivType)
        {
            if (DivType == "WTP0000001")
            {
                var vendors = objmms.tblPcContractorMasters.Where(s => (s.WorkTypeCode.Contains(DivType) || s.WorkTypeCode == null) && s.PRJID.Contains(ProjectId)).Select(s => new { Text = s.Name, Value = s.PcContractorID }).OrderBy(s=>s.Text).ToList();
                return Json(vendors.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var vendors = objmms.tblPcContractorMasters.Where(s => s.WorkTypeCode.Contains(DivType) && s.PRJID.Contains(ProjectId)).Select(s => new { Text = s.Name, Value = s.PcContractorID }).OrderBy(s => s.Text).ToList();
                return Json(vendors.ToJSON(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPCVendorByProjectId1(string ProjectId, string DivType)
        {
            if (DivType == "WTP0000001")
            {
                var vendors = objmms.tblPcContractorMasters.Where(s => (s.WorkTypeCode.Contains(DivType) || s.WorkTypeCode == null) && s.PRJID.Contains(ProjectId)).Select(s => new { Text = s.Name, Value = s.PcContractorID }).OrderBy(s => s.Text).ToList();
                return Json(vendors.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else {
                var vendors = objmms.tblPcContractorMasters.Where(s => s.WorkTypeCode.Contains(DivType) && s.PRJID.Contains(ProjectId)).Select(s => new { Text = s.Name, Value = s.PcContractorID }).OrderBy(s => s.Text).ToList();
                return Json(vendors.ToJSON(), JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult AddNewBlock(string projectId, string blockName)
        {
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(blockName))
                return Json(new { Status = 2 });

            try
            {
                int pID = objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == projectId).TransID;

                if (!objmms.BlockMasters.Any(x => x.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase) && x.SiteId==pID))
                {
                    objmms.BlockMasters.Add(new BlockMaster { SiteId = pID, BlockName = blockName });
                    objmms.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3 });
            }
            return Json(new { Status = 1 });
        }

        public JsonResult AddNewMachine(string projectId, string machineName)
        {
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(machineName))
                return Json(new { Status = 2 });

            try
            {
                
                if (!objmms.tblMachineMasters.Any(x => x.MachineName.Equals(machineName, StringComparison.OrdinalIgnoreCase)))
                {
                    objmms.tblMachineMasters.Add(new tblMachineMaster
                     { ProjectId =projectId,
                        MachineName = machineName,
                        CreatedBy =Session["EmpID"].ToString(),
                        CreatedDate =DateTime.Now,Status=true });
                    objmms.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3 });
            }
            return Json(new { Status = 1 });
        }



        public JsonResult AddNewFloor(string projectId, string floorName)
        {
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(floorName))
                return Json(new { Status = 2 });

            try
            {
                int pID = objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == projectId).TransID;
                if (!objmms.FloorMasters.Any(x => x.FloorName.Equals(floorName, StringComparison.OrdinalIgnoreCase) && x.SiteId== pID))
                {
                    objmms.FloorMasters.Add(new FloorMaster { SiteId = pID, FloorName = floorName });
                    objmms.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3 });
            }
            return Json(new { Status = 1 });
        }

    }
}