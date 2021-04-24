using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MMS.Models;
using MMS.DAL;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MMS.Helpers;

namespace MMS.Controllers
{
    public class IndentRequistionController : MySiteController
    {
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        MSP_Model objmsp = new MSP_Model();
        private Procedure procedure = new Procedure();
        // GET: IndentRequistion
        
        [Authorize]
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            var a = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectID.ToString(),
                // Text = x.ProjectID.ToString()
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            });
            ViewBag.prjtid = t;


            //ViewBag.prjtid = new SelectList(objmms.tblProjectMasters, "PRJID", "ProjectName");
            //Employee Or Forword to User Name Binding
            //ViewBag.empName = new SelectList(objmms.tblEmployeeMasters, "EmpID", "FName");

            //Item Group Name
            ViewBag.itemgroupname = new SelectList(objmms.tblItemGroupMasters, "ItemGroupID", "GroupName").OrderBy(k => k.Text);

            //Select Block Name
            ViewBag.blockname = new SelectList(objmms.BlockMasters, "BlockId", "BlockName");

            ViewBag.empName1 = new SelectList(objmms.tblEmployeeMasters.Where(x => x.EmpID == empId), "EmpID", "FName");

            return View();
        }

        // Json Call to get Project Code
        public JsonResult Getprojectcodes(string id,string date )
        {

            // var stateList = this.Getprojectcode(Convert.ToString(id));           
            var stateList = this.GetprojectcodeByDate(Convert.ToString(id),Convert.ToDateTime(date));
            return Json(stateList, JsonRequestBehavior.AllowGet);
        }

        // Get issue requisition no. method for backend generate on 01_04_20

        public string GetIssueReqNo(string id, string date)
        {

            // var stateList = this.Getprojectcode(Convert.ToString(id));           
            // return this.GetprojectcodeByDate(Convert.ToString(id), Convert.ToDateTime(date));
            //objmms.Database.UseTransaction(dbTransaction);
            var sessioncode = objmsp.GetFinancialYearByDate(Convert.ToDateTime(date));

            var divisionId = Repos.GetUserDivision();

            string ProjectCode = "";

            var data = objmms.tblProjectCodes.Where(m => m.ProjectID == id).SingleOrDefault();

            string transferId = "";
            //  var doesIdExist = objmms.tblIndentRequisitions.Where(u => u.ProjectID == id && u.SessionId == sessioncode && u.DivisionId == divisionId);

            int doesIdExist = procedure.IndentID_Exists(id, sessioncode, divisionId);
            if (doesIdExist != 0)
            {
                ///For Max Id Indent Number
                var idMax = (from emp in objmms.tblIndentRequisitions
                             where emp.ProjectID == id && emp.SessionId == sessioncode && emp.DivisionId == divisionId
                             select emp.AutoUniID).Max() + 1;
                if (idMax == null)
                {
                    idMax = 1;
                }
                transferId = idMax.ToString();
                Session["Auuniqueid"] = transferId;
            }
            else
            {
                var idMax = (from emp in objmms.tblIndentRequisitions
                             select emp.AutoUniID).Max() + 1;
                if (idMax == null)
                {
                    idMax = 1;
                }
            }
            string DivisionCode = "";
            DivisionCode = objmms.tblDivisionMasters.SingleOrDefault(x => x.Id == divisionId).DivisionCode;

            if (data != null && data.ProjectID.Length > 0)
            {
                if (DivisionCode != "")
                    ProjectCode = data.ProjectCode + "/" + DivisionCode + "/" + sessioncode + "/" + transferId;
                else
                    ProjectCode = data.ProjectCode + "/" + sessioncode + "/" + transferId;
            }

            return ProjectCode;

        }
        //end//////////

        // Get State from DB by 
        public IList<tblProjectCode> GetprojectcodeList(string ProjectID)
        {
            return objmms.tblProjectCodes.Where(m => m.ProjectID == ProjectID).ToList();
        }

        //Check
        public JsonResult IsQtyAllowedToIssue(string ProjectId,string ItemId,string Date,decimal IssueQty)
        {
            if(ProjectId!="" && ItemId!="" && Date != "")
            {
                DateTime dateTime = DateTime.ParseExact(Date, "dd-MMM-yyyy",CultureInfo.InvariantCulture);
                var balQty=objmms.tblReceiveDatas.Where(x => x.ProjectId == ProjectId && x.TypeDate <= dateTime && x.ItemId == ItemId && x.BalanceQuantity > 0).Sum(x => x.BalanceQuantity);
                 balQty=balQty ?? 0;
                if (balQty>=IssueQty)
                return Json(new { Status = 1, Qty = balQty }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Status = 2, Qty = balQty }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = 3, Qty = 0 },JsonRequestBehavior.AllowGet);
            }
        }

        public object GetprojectcodeByDate(string ProjectID,DateTime date)
        {
            // var sessioncode = (from c in objmms.SessionMasters select  c.Name ).First();
            var sessioncode = objmsp.GetFinancialYearByDate(date);

            var divisionId = Repos.GetUserDivision();

            string ProjectCode = "";

            var data = objmms.tblProjectCodes.Where(m => m.ProjectID == ProjectID).SingleOrDefault();

            string transferId = "";
            var doesIdExist = objmms.tblIndentRequisitions.Where(u => u.ProjectID == ProjectID && u.SessionId == sessioncode && u.DivisionId==divisionId);
            if (doesIdExist != null)
            {
                ///For Max Id Indent Number
                var idMax = (from emp in objmms.tblIndentRequisitions
                             where emp.ProjectID == ProjectID && emp.SessionId == sessioncode && emp.DivisionId==divisionId
                             select emp.AutoUniID).Max() + 1;
                if (idMax == null)
                {
                    idMax = 1;
                }
                transferId = idMax.ToString();
                Session["Auuniqueid"] = transferId;
            }
            else
            {
                var idMax = (from emp in objmms.tblIndentRequisitions
                             select emp.AutoUniID).Max() + 1;
                if (idMax == null)
                {
                    idMax = 1;
                }
            }
            string DivisionCode = "";
            DivisionCode = objmms.tblDivisionMasters.SingleOrDefault(x => x.Id == divisionId).DivisionCode;

            if (data != null && data.ProjectID.Length > 0)
            {
                if(DivisionCode!="")
                    ProjectCode = data.ProjectCode + "/"+DivisionCode+"/" + sessioncode + "/" + transferId;
                else
                    ProjectCode = data.ProjectCode + "/" + sessioncode + "/" + transferId;
            }

            var last_IR = objmms.tblIndentRequisitions.Where(s => s.ProjectID == ProjectID && s.SessionId == sessioncode && s.DivisionId == divisionId).OrderByDescending(s => s.Id).FirstOrDefault();

            string last_no = "";
            string last_date = "";
            if (last_IR != null)
            {
                last_no = last_IR.IndentNo;
                last_date = last_IR.IndentDate.Value.ToString("dd/MM/yyyy");
            }

            return new { New_No = ProjectCode, Last_No = last_no, Last_Date = last_date };

        }

        // get indent project code and session
        public string Getprojectcode(string ProjectID)
        {
            // var sessioncode = (from c in objmms.SessionMasters select  c.Name ).First();
            var sessioncode = objmsp.GetFinancialYear();

            var divisionId = Repos.GetUserDivision();


            string ProjectCode = "";
            
           var data= objmms.tblProjectCodes.Where(m => m.ProjectID == ProjectID).SingleOrDefault();

            string transferId="";
           var doesIdExist = objmms.tblIndentRequisitions.Where(u => u.ProjectID == ProjectID && u.SessionId == sessioncode && u.DivisionId == divisionId);
           if (doesIdExist != null)
           {
               ///For Max Id Indent Number
               var idMax = (from emp in objmms.tblIndentRequisitions
                            where emp.ProjectID == ProjectID && emp.SessionId==sessioncode && emp.DivisionId == divisionId
                            select emp.AutoUniID).Max() + 1;
               if (idMax == null)
               {
                   idMax = 1;
               }
               transferId = idMax.ToString();
               Session["Auuniqueid"] = transferId;
           }
           else
           {
               var idMax = (from emp in objmms.tblIndentRequisitions
                            select emp.AutoUniID).Max() + 1;
               if (idMax == null)
               {
                   idMax = 1;
               }
           }

           var DivisionCode = objmms.tblDivisionMasters.SingleOrDefault(x => x.Id == divisionId).DivisionCode;

            if (data != null && data.ProjectID.Length > 0 )
           {
                if (DivisionCode != "")
                    ProjectCode = data.ProjectCode + "/"+DivisionCode+"/" + sessioncode + "/" + transferId;
                else
                    ProjectCode = data.ProjectCode + "/" + sessioncode + "/" + transferId;
            }
           
           return ProjectCode;

                    

        }

        // for Item Name Binding
        // Json Call to get state
        public JsonResult GetGroupitem(string ProjectId,string id)
        {
            List<SelectListItem> groups = new List<SelectListItem>();
            //var groupList = this.Getitemname(Convert.ToString(id));
            //var stateData = groupList.Select(m => new SelectListItem()
            //{
            //    Text = m.ItemName,
            //    Value = m.ItemID.ToString(),

            //});
            var data = (from a in objmms.tblItemCurrentStocks
                        join b in objmms.tblItemMasters on a.ItemNo equals b.ItemID
                        where a.ItemGroupNo == id && a.ProjectNo == ProjectId
                        orderby b.ItemName ascending
                        select new { Text = b.ItemName, Value = a.ItemNo }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Get State from DB by country ID
        public IList<tblItemMaster> Getitemname(string GroupId)
        {
            return objmms.tblItemMasters.Where(m => m.ItemGroupID == GroupId).OrderBy(k => k.ItemName).ToList();
        }



        // Json Call to get Project AND FLOOR 

        // public JsonResult GetFloorname(int bid)
        // {
        //     List<SelectListItem> fgroups = new List<SelectListItem>();
        //     var fgroupList = this.Getblockname(bid);
        //     var blockData = fgroupList.Select(m => new SelectListItem()
        //     {
        //         Text = m.FloorName,
        //         Value = m.FloorId.ToString(),
        //     });
        //     return Json(blockData, JsonRequestBehavior.AllowGet);
        // }

        //// Get FLOOR from DB by BLOCK ID
        // public IList<FloorMaster> Getblockname(int GroupfId)
        // {
        //     return objmms.FloorMasters.Where(m => m.BlockId == GroupfId).ToList();
        // }

        //15052018
        public JsonResult GetFloorName(string fid)
        {
            List<SelectListItem> fgroups = new List<SelectListItem>();
            int pID = objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == fid).TransID;
            var data = objmms.FloorMasters.Where(x=>x.SiteId==pID).Select(x => new { Text = x.FloorName, Value = x.FloorId }).OrderBy(x=>x.Value).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        // Json Call to get BLOCK By Project Id

        public int ReturnTransId(string PrjtsId)
        {
         
            int  sae= objmms.tblProjectMasters.Where(m => m.PRJID == PrjtsId).Select(s=> s.TransID).FirstOrDefault();
            return sae;
        }
         public string ReturnPrjId(int PrjtsId)
        {
         
            string  sae= objmms.tblProjectMasters.Where(m => m.TransID == PrjtsId).Select(s=> s.PRJID).FirstOrDefault();
            return sae;
        }
        public JsonResult GetBlocknames(string fid)
        {
            List<SelectListItem> fgroups = new List<SelectListItem>();
            int pID = objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == fid).TransID;
            var fgroupLists = this.Getblockid(pID);
            var blocksData = fgroupLists.Select(m => new SelectListItem()
            {
                Text = m.BlockName,
                Value = m.BlockId.ToString(),
            }).OrderBy(x=>x.Text).ToList();
            return Json(blocksData, JsonRequestBehavior.AllowGet);
        }

        // Get FLOOR from DB by Project ID
        public IList<BlockMaster> Getblockid(int Getid)
        {

            return objmms.BlockMasters.Where(m => m.SiteId == Getid).ToList();
        }



        // Json Call to get Employee Name
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
        public IList<tblEmployeeMaster> Getprojectname(string ProjectId)
        {
            
            return (from a in objmms.tblApproval_AccountType.Where(x => x.TypeName == "PIC").ToList()
                    join
                    b in objmms.tblEmployeeMasters.Where(p => p.ProjectID.Contains(ProjectId)).ToList() on a.EmployeeId equals b.EmpID
                    select b).Distinct().ToList().DefaultIfEmpty<tblEmployeeMaster>().ToList(); 

        }



        // Json Call to get Part No and Make
        public JsonResult Getitemnmakepartno(string id)
        {

            var itemssList = this.Getitemdetails(Convert.ToString(id));
            return Json(itemssList, JsonRequestBehavior.AllowGet);
        }       


        public string Getitemdetails(string ItemID)
        {                       
            string ProjectItemid = "";
          
            var data = objmms.tblItemMasters.Where(m => m.ItemID == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.ItemID.Length > 0)
            {
                ProjectItemid = data.Make;               
            }
            return ProjectItemid;           
        }


        public JsonResult Getitemnmake(string id)
        {

            var itemssLists = this.Getitemdetailss(Convert.ToString(id));
            return Json(itemssLists, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemPrice(string ProjectId, string ItemGroupId, string ItemId)
        {
            decimal? price = new decimal(0.0);
            if (ProjectId != "" && ItemGroupId != "" && ItemId != "")
            {
                //var item = objmms.tblItemMasterStocks.SingleOrDefault(x => x.ProjectNo == ProjectId && x.ItemGroup == ItemGroupId && x.ItemNo == ItemId);
                var item = objmms.tblReceiveDatas.Where(x => x.ProjectId == ProjectId && x.ItemGroupId == ItemGroupId && x.ItemId == ItemId).OrderByDescending(x=>x.ID).FirstOrDefault();
                price = item == null ? new decimal(0.0) : item.Rate;

            }
            return Json(new { Rate = price.ToString() });

        }

        public string Getitemdetailss(string ItemID)
        {
            
            string Partno = "";
            
            var data = objmms.tblItemMasters.Where(m => m.ItemID == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.ItemID.Length > 0)
            {           
                Partno = data.PartNo;           
            }
            return Partno;
        }



        // here code for check balance item list

        public JsonResult Getitemn_balance(string PrjId = null, string IG = null, string itemid =null)
        {

            var itemsListBalance = this.Getitemitem_BalanceCheck(PrjId, IG, Convert.ToString(itemid));
            return Json(itemsListBalance, JsonRequestBehavior.AllowGet);
        }

        public string Getitemitem_BalanceCheck(string PrjIds, string Ig, string ItemId)
        {
            decimal allrecord = 0;
            //string ProjectId = "";
            //string ItemGroupId = "";
            //string ItemId = "";

            var data = objmms.tblItemCurrentStocks.Where(m => m.ProjectNo ==PrjIds && m.ItemGroupNo == Ig && m.ItemNo == ItemId.Trim().ToUpper()).FirstOrDefault();
            if (data != null)
            {
                allrecord = Convert.ToDecimal(data.Qty);
                //if (Unitid != null)
                //{
                //    var unitsnames = objmms.tblUnitMasters.Where(m => m.UnitID == Unitid.Trim().ToUpper()).FirstOrDefault();
                //    Unitid = unitsnames.UnitName;
                //}
            }
            return allrecord.ToString();
        }
        public JsonResult Getitemnunit(string id)
        {

            var itemssListsu = this.Getitemdetailsses(Convert.ToString(id));
            return Json(itemssListsu, JsonRequestBehavior.AllowGet);
        }

        public string Getitemdetailsses(string ItemID)
        {

            string Unitid = "";         

            var data = objmms.tblItemMasters.Where(m => m.ItemID == ItemID.Trim().ToUpper()).FirstOrDefault();
            if (data != null && data.ItemID.Length > 0)
            {
                Unitid = data.UnitID;
                
                if (Unitid != null)
                {
                    var unitsnames = objmms.tblUnitMasters.Where(m => m.UnitID == Unitid.Trim().ToUpper()).FirstOrDefault();
                    Unitid = unitsnames.UnitName;
                }
                
            }
            return Unitid;
        }


        // here webgrid code

        public JsonResult SendList_old(List<Data> ListData)
        {
            try
            {
                List<tblIndentRequisition> list = new List<tblIndentRequisition>();
                foreach (var item in ListData)
                {
                    if (item.ProjectID == null || item.EmployeeID == null)
                    {
                        return Json("3", JsonRequestBehavior.AllowGet);

                    }

                    tblIndentRequisition tbl = new tblIndentRequisition();
                    tbl.ProjectID = item.ProjectID;
                    tbl.IndentNo = item.IndentNo;
                    tbl.EmployeeID = item.EmployeeID;
                    tbl.ItemGroupID = item.ItemGroupID;
                    tbl.ItemID = item.ItemID;
                    tbl.UnitID = item.Unitname;
                    tbl.Make = item.Makename;
                    tbl.PartNo = item.Partname;
                    tbl.Quantity = Convert.ToDecimal(item.Quantityname);
                    tbl.BalanceQuantity = Convert.ToDecimal(item.Quantityname);
                    tbl.IssueQuantity = Convert.ToDecimal("0.0");
                    // tbl.SessionId = "1";
                    tbl.SessionId = objmsp.GetFinancialYear(); 
                    tbl.SiteId = "";
                    tbl.CreatedDate = DateTime.Now;
                    tbl.AutoUniID = Convert.ToInt32(Session["Auuniqueid"]);
                    tbl.Status = "Pending";
                    tbl.ProjectName = item.ProjectName;
                    tbl.EmployeeName = item.EmployeeName;
                    tbl.ItemGroupName = item.ItemGroupName;
                    tbl.ItemName = item.ItemName;
                    tbl.CreatedBy = Convert.ToString(Session["EmpID"]);
                    tbl.IsRead = "No";

                    tbl.BlockId = item.BlockId;
                    tbl.BlockName = item.BlockName;
                    tbl.FloorId = item.FloorId;
                    tbl.FloorName = item.FloorName;
                    tbl.IndentType = item.IndentType;
                    tbl.IndentSent = "No";
                    tbl.IndentOtherDesc = item.IndentOtherDesc;
                    tbl.Approved_Quantity = Convert.ToDecimal(0.0);
                    tbl.Apporoved_Status = "No";
                    tbl.BalanceApprovedQty_After_Issue_Qty = 0;

                    list.Add(tbl);

                }
                objmms.tblIndentRequisitions.AddRange(list);
               objmms.SaveChanges();
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch{
                return Json("2", JsonRequestBehavior.AllowGet);
            }

        }


        public decimal GetAmount(string ProjectId,string ItemId,decimal Qty)
        {
            return procedure.GetTotalAmount(ProjectId, ItemId, Qty);
        }

        public decimal GetAmount1(string ProjectName, string ItemId, decimal Qty)
        {
            ProjectName = ProjectName.Trim();
            var projectId = objmms.tblProjectMasters.SingleOrDefault(x => x.ProjectName.Equals(ProjectName)).PRJID;
            return procedure.GetTotalAmount(projectId, ItemId, Qty);
        }


        public PartialViewResult AmountDetails(string ProjectId,string ItemId,decimal Qty)
        {
            var details= procedure.GetTotalAmountDetails(ProjectId, ItemId, Qty);
            if(details!=null && details.Count > 0)
            {
                ViewBag.Total = details.Sum(x => x.Qty * x.Rate);
            }
            else
            {
                ViewBag.Total = new decimal(0);
            }
            return PartialView("AmountDetails",details);
        }


        public PartialViewResult AmountDetailsByProjectName(string ProjectName, string ItemId, decimal Qty)
        {
            ProjectName = ProjectName.Trim();
            var projectId = objmms.tblProjectMasters.SingleOrDefault(x => x.ProjectName.Equals(ProjectName)).PRJID;
            var details = procedure.GetTotalAmountDetails(projectId, ItemId, Qty);
            if (details != null && details.Count > 0)
            {
                ViewBag.Total = details.Sum(x => x.Qty * x.Rate);
            }
            else
            {
                ViewBag.Total = new decimal(0);
            }
            return PartialView("AmountDetails", details);
        }





        //public JsonResult SendList(List<Data> ListData, string IndentDate)
        //{
        //    if (ListData == null || string.IsNullOrEmpty(IndentDate))
        //    {
        //        return Json(new { Result = 4, TansNo = "" }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (ListData.Count == 0)
        //    {
        //        return Json(new { Result = 4, TansNo = "" }, JsonRequestBehavior.AllowGet);
        //    }
        //    //  using (var transaction=objmms.Database.BeginTransaction())
        //    //  {
        //    try
        //    {

        //        var divisionId = Repos.GetUserDivision();
        //        List<tblIndentRequisition> list = new List<tblIndentRequisition>();
        //        List<ns_tbl_ApprovedItemQuantity> approvedItemQtyList = new List<ns_tbl_ApprovedItemQuantity>();
        //        var IDate = Convert.ToDateTime(IndentDate);
        //        string issuereq_num = "";///GetIssueReqNo(ListData[0].ProjectID, IndentDate);
        //        foreach (var item in ListData)
        //        {
        //            if (item.ProjectID == null || item.EmployeeID == null)
        //            {
        //                // transaction.Commit();
        //                return Json(new { Result = 3, TansNo = "" }, JsonRequestBehavior.AllowGet);
        //            }
        //            tblIndentRequisition tbl = new tblIndentRequisition();
        //            tbl.ProjectID = item.ProjectID;
        //            tbl.IndentNo = issuereq_num; // item.IndentNo;
        //            tbl.EmployeeID = item.EmployeeID;
        //            tbl.ItemGroupID = item.ItemGroupID;
        //            tbl.ItemID = item.ItemID;
        //            tbl.UnitID = item.Unitname;
        //            tbl.Make = item.Makename;
        //            tbl.PartNo = item.Partname;
        //            tbl.Quantity = Convert.ToDecimal(item.Quantityname);
        //            //tbl.BalanceQuantity = Convert.ToDecimal("0.0");
        //            tbl.IssueQuantity = Convert.ToDecimal("0.0");
        //            // tbl.SessionId = "1";
        //            tbl.SessionId = objmsp.GetFinancialYearByDate(IDate);
        //            tbl.SiteId = "";
        //            tbl.IndentDate = IDate;
        //            tbl.CreatedDate = DateTime.Now;
        //            tbl.AutoUniID = 0;// Convert.ToInt32(Session["Auuniqueid"]);
        //                              //tbl.Status = "Pending";
        //            tbl.ProjectName = item.ProjectName;
        //            tbl.EmployeeName = item.EmployeeName;
        //            tbl.ItemGroupName = item.ItemGroupName;
        //            tbl.ItemName = item.ItemName;
        //            tbl.CreatedBy = Convert.ToString(Session["EmpID"]);
        //            tbl.IsRead = "No";

        //            //tbl.BlockId = item.BlockId;
        //            //tbl.BlockName = item.BlockName;
        //            //tbl.FloorId = item.FloorId;
        //            //tbl.FloorName = item.FloorName;

        //            tbl.IndentType = item.IndentType;
        //            tbl.IndentSent = "No";
        //            tbl.IndentOtherDesc = item.IndentOtherDesc;


        //            tbl.Approved_Quantity = tbl.Quantity;
        //            tbl.Apporoved_By = Session["EmpID"].ToString();
        //            tbl.Apporoved_Status = "Yes";
        //            tbl.Status = "Approved";
        //            tbl.BalanceQuantity = tbl.Quantity - tbl.Approved_Quantity;
        //            tbl.BalanceApprovedQty_After_Issue_Qty = tbl.Quantity;
        //            tbl.DivisionId = divisionId;

        //            list.Add(tbl);

        //            //    ns_tbl_ApprovedItemQuantity objisu = new ns_tbl_ApprovedItemQuantity();
        //            //    objisu.ProjectID = tbl.ProjectID;
        //            //    objisu.ProjectName = tbl.ProjectName;
        //            //    objisu.IndentNo = tbl.IndentNo;
        //            //    objisu.EmployeeName = tbl.EmployeeName;

        //            //    objisu.ItemGroupID = tbl.ItemGroupID;
        //            //    objisu.ItemGroupName = tbl.ItemGroupName;
        //            //    objisu.ItemID = tbl.ItemID;
        //            //    objisu.ItemName = tbl.ItemName;
        //            //    objisu.UnitID = tbl.UnitID;
        //            //    objisu.Make = tbl.Make;
        //            //    objisu.PartNo = tbl.PartNo;
        //            //    objisu.Quantity = tbl.Quantity;
        //            //    objisu.ApprovedQuantity = tbl.Approved_Quantity;

        //            //    objisu.SessionId = "1";
        //            //    objisu.SiteId = "N/A";
        //            //    objisu.ApproveddBy = Convert.ToString(Session["EmpID"]);
        //            //    objisu.Status = "Approved";
        //            //    objisu.CreatedDate = DateTime.Now;

        //            //    approvedItemQtyList.Add(objisu);
        //        }
        //        //objmms.tblIndentRequisitions.AddRange(list);
        //        //objmms.ns_tbl_ApprovedItemQuantity.AddRange(approvedItemQtyList);
        //        //objmms.SaveChanges();

        //        var res = procedure.Save_IndentRequisition(list);
        //        //  transaction.Commit();
        //        return Json(res, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        //  transaction.Rollback();
        //        return Json(new { Result = 2, TansNo = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }

        //    //}

        //}


        public JsonResult SendList(List<Data> ListData, string IndentDate)
        {
            if (ListData == null || string.IsNullOrEmpty(IndentDate))
            {
                return Json(new { Result = 4, TansNo = "" }, JsonRequestBehavior.AllowGet);
            }
            if (ListData.Count == 0)
            {
                return Json(new { Result = 4, TansNo = "" }, JsonRequestBehavior.AllowGet);
            }
            //  using (var transaction=objmms.Database.BeginTransaction())
            //  {
            if (ListData.Count > 0)
            {
                var IR_Date = Convert.ToDateTime(IndentDate);
                int? div_Id = Repos.GetUserDivision();
                string f_y = objmsp.GetFinancialYearByDate(IR_Date);
                string prj_id = ListData[0].ProjectID;
                var last_IR = objmms.tblIndentRequisitions.Where(s => s.DivisionId == div_Id && s.ProjectID == prj_id && s.SessionId == f_y).OrderByDescending(s=>s.Id).Select(s => s).FirstOrDefault();
                if (last_IR != null)
                {
                    if (Convert.ToDateTime(IndentDate) < last_IR.IndentDate)
                        return Json(new { Result = 0, TansNo = last_IR.IndentDate.Value.ToString("dd/MM/yyyy") });
                }
            }
            try
            {

                var divisionId = Repos.GetUserDivision();
                List<tblIndentRequisition> list = new List<tblIndentRequisition>();
                List<ns_tbl_ApprovedItemQuantity> approvedItemQtyList = new List<ns_tbl_ApprovedItemQuantity>();
                var IDate = Convert.ToDateTime(IndentDate);
                string issuereq_num = "";///GetIssueReqNo(ListData[0].ProjectID, IndentDate);

                DataTable requisitionDT = new DataTable();
                requisitionDT.Columns.Add("ProjectID", typeof(string));
                requisitionDT.Columns.Add("ProjectName", typeof(string));
                requisitionDT.Columns.Add("IndentNo", typeof(string));
                requisitionDT.Columns.Add("EmployeeID", typeof(string));
                requisitionDT.Columns.Add("EmployeeName", typeof(string));
                requisitionDT.Columns.Add("ItemGroupID", typeof(string));
                requisitionDT.Columns.Add("ItemGroupName", typeof(string));
                requisitionDT.Columns.Add("ItemID", typeof(string));
                requisitionDT.Columns.Add("ItemName", typeof(string));
                requisitionDT.Columns.Add("UnitID", typeof(string));
                requisitionDT.Columns.Add("Make", typeof(string));
                requisitionDT.Columns.Add("PartNo", typeof(string));
                requisitionDT.Columns.Add("Quantity", typeof(decimal));
                requisitionDT.Columns.Add("BalanceQuantity", typeof(decimal));
                requisitionDT.Columns.Add("IssueQuantity", typeof(decimal));
                requisitionDT.Columns.Add("SessionId", typeof(string));
                requisitionDT.Columns.Add("SiteId", typeof(string));
                requisitionDT.Columns.Add("CreatedDate", typeof(DateTime));
                requisitionDT.Columns.Add("AutoUniID", typeof(int));
                requisitionDT.Columns.Add("Status", typeof(string));
                requisitionDT.Columns.Add("CreatedBy", typeof(string));
                requisitionDT.Columns.Add("IsRead", typeof(string));
                requisitionDT.Columns.Add("BlockId", typeof(int));
                requisitionDT.Columns.Add("BlockName", typeof(string));
                requisitionDT.Columns.Add("FloorId", typeof(int));
                requisitionDT.Columns.Add("FloorName", typeof(string));
                requisitionDT.Columns.Add("IndentType", typeof(string));
                requisitionDT.Columns.Add("IndentOtherDesc", typeof(string));
                requisitionDT.Columns.Add("IndentSent", typeof(string));
                requisitionDT.Columns.Add("Approved_Quantity", typeof(decimal));
                requisitionDT.Columns.Add("Apporoved_By", typeof(string));
                requisitionDT.Columns.Add("Apporoved_Status", typeof(string));
                requisitionDT.Columns.Add("OrderBalance", typeof(decimal));
                requisitionDT.Columns.Add("BalanceApprovedQty_After_Issue_Qty", typeof(decimal));
                requisitionDT.Columns.Add("IndentDate", typeof(DateTime));
                requisitionDT.Columns.Add("DivisionId", typeof(int));
                foreach (var item in ListData)
                {
                    if (item.ProjectID == null || item.EmployeeID == null)
                    {
                        // transaction.Commit();
                        return Json(new { Result = 3, TansNo = "" }, JsonRequestBehavior.AllowGet);
                    }
                    //tblIndentRequisition tbl = new tblIndentRequisition();
                    //tbl.ProjectID = item.ProjectID;
                    //tbl.IndentNo = issuereq_num; // item.IndentNo;
                    //tbl.EmployeeID = item.EmployeeID;
                    //tbl.ItemGroupID = item.ItemGroupID;
                    //tbl.ItemID = item.ItemID;
                    //tbl.UnitID = item.Unitname;
                    //tbl.Make = item.Makename;
                    //tbl.PartNo = item.Partname;
                    //tbl.Quantity = Convert.ToDecimal(item.Quantityname);
                    ////tbl.BalanceQuantity = Convert.ToDecimal("0.0");
                    //tbl.IssueQuantity = Convert.ToDecimal("0.0");
                    //// tbl.SessionId = "1";
                    //tbl.SessionId = objmsp.GetFinancialYearByDate(IDate);
                    //tbl.SiteId = "";
                    //tbl.IndentDate = IDate;
                    //tbl.CreatedDate = DateTime.Now;
                    //tbl.AutoUniID = 0;// Convert.ToInt32(Session["Auuniqueid"]);
                    ////tbl.Status = "Pending";
                    //tbl.ProjectName = item.ProjectName;
                    //tbl.EmployeeName = item.EmployeeName;
                    //tbl.ItemGroupName = item.ItemGroupName;
                    //tbl.ItemName = item.ItemName;
                    //tbl.CreatedBy = Convert.ToString(Session["EmpID"]);
                    //tbl.IsRead = "No";

                    ////tbl.BlockId = item.BlockId;
                    ////tbl.BlockName = item.BlockName;
                    ////tbl.FloorId = item.FloorId;
                    ////tbl.FloorName = item.FloorName;

                    //tbl.IndentType = item.IndentType;
                    //tbl.IndentSent = "No";
                    //tbl.IndentOtherDesc = item.IndentOtherDesc;


                    //tbl.Approved_Quantity = tbl.Quantity;
                    //tbl.Apporoved_By = Session["EmpID"].ToString();
                    //tbl.Apporoved_Status = "Yes";
                    //tbl.Status = "Approved";
                    //tbl.BalanceQuantity = tbl.Quantity - tbl.Approved_Quantity;
                    //tbl.BalanceApprovedQty_After_Issue_Qty = tbl.Quantity;
                    //tbl.DivisionId = divisionId;

                    //list.Add(tbl);

                    requisitionDT.Rows.Add(item.ProjectID, item.ProjectName, item.IndentNo, item.EmployeeID, item.EmployeeName, item.ItemGroupID, item.ItemGroupName,
                        item.ItemID, item.ItemName, item.Unitname, item.Makename, item.Partname, Convert.ToDecimal(item.Quantityname), Convert.ToDecimal(item.Quantityname) - item.Approved_Quantity, Convert.ToDecimal("0.0"),
                        objmsp.GetFinancialYearByDate(IDate), item.SiteId, DateTime.Now, 0, "Approved", Convert.ToString(Session["EmpID"]), "No", null, null, null, null, item.IndentType, item.IndentOtherDesc,
                        "No", Convert.ToDecimal(item.Quantityname), Session["EmpID"].ToString(), "Yes", null, Convert.ToDecimal(item.Quantityname), IDate, divisionId);

                    //    ns_tbl_ApprovedItemQuantity objisu = new ns_tbl_ApprovedItemQuantity();
                    //    objisu.ProjectID = tbl.ProjectID;
                    //    objisu.ProjectName = tbl.ProjectName;
                    //    objisu.IndentNo = tbl.IndentNo;
                    //    objisu.EmployeeName = tbl.EmployeeName;

                    //    objisu.ItemGroupID = tbl.ItemGroupID;
                    //    objisu.ItemGroupName = tbl.ItemGroupName;
                    //    objisu.ItemID = tbl.ItemID;
                    //    objisu.ItemName = tbl.ItemName;
                    //    objisu.UnitID = tbl.UnitID;
                    //    objisu.Make = tbl.Make;
                    //    objisu.PartNo = tbl.PartNo;
                    //    objisu.Quantity = tbl.Quantity;
                    //    objisu.ApprovedQuantity = tbl.Approved_Quantity;

                    //    objisu.SessionId = "1";
                    //    objisu.SiteId = "N/A";
                    //    objisu.ApproveddBy = Convert.ToString(Session["EmpID"]);
                    //    objisu.Status = "Approved";
                    //    objisu.CreatedDate = DateTime.Now;

                    //    approvedItemQtyList.Add(objisu);
                }
                //objmms.tblIndentRequisitions.AddRange(list);
                //objmms.ns_tbl_ApprovedItemQuantity.AddRange(approvedItemQtyList);
                //objmms.SaveChanges();

                var res = procedure.Save_IndentRequisitionTable(requisitionDT);
                //  transaction.Commit();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //  transaction.Rollback();
                return Json(new { Result = 2, TansNo = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            //}

        }



        public class Data
        {
            public string ProjectID { get; set; }
            [Required]
            public string IndentNo { get; set; }
            public string EmployeeID { get; set; }
            public string ItemGroupID { get; set; }
            public string ItemID { get; set; }
            public string Unitname { get; set; }
            public string Makename { get; set; }
            public string Partname { get; set; }
            public string Quantityname { get; set; }
            public string SessionId { get; set; }
            public string SiteId { get; set; }
            public Nullable<System.DateTime> Date { get; set; }
            public Nullable<decimal> BalanceQuantity { get; set; }
            public Nullable<decimal> IssueQuantity { get; set; }
            public string ProjectName { get; set; }
            public string EmployeeName { get; set; }           
            public string ItemGroupName { get; set; }
            public string ItemName { get; set; }
            public string CreatedBy { get; set; }
            public string IsRead { get; set; }

            public Nullable<int> BlockId { get; set; }
            public string BlockName { get; set; }
            public Nullable<int> FloorId { get; set; }
            public string FloorName { get; set; }
            public string IndentType { get; set; }
            public string IndentOtherDesc { get; set; }
            public string IndentSent { get; set; }
            public Nullable<decimal> Approved_Quantity { get; set; }
            public string Apporoved_Status { get; set; }
            public Nullable<decimal> BalanceApprovedQty_After_Issue_Qty { get; set; }
        }





        





        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }

    }
}