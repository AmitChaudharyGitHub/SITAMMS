using ClosedXML.Excel;
using MMS.DAL;
using MMS.Helpers;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MMS.Controllers
{

    [Authorize]
    public class IssueReturnController : Controller
    {
        // GET: IssueReturn
        public ActionResult Index()
        {
            return View();
        }



        public JsonResult GetItemGroups()
        {
            using (var db = new MMSEntities())
            {
                var data = db.tblItemGroupMasters.Select(x => new { Text = x.GroupName, Value = x.ItemGroupID }).OrderBy(x => x.Text).ToList();
                return Json(data.ToJSON(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetItemName(string GroupId)
        {
            using (var db = new MMSEntities())
            {
                var data = db.tblItemMasters.Where(x => x.ItemGroupID == GroupId).Select(x => new { Text = x.ItemName, Value = x.ItemID }).OrderBy(x => x.Text).ToList();
                return Json(data.ToJSON(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPcs(string ProjectId)
        {
            using (var objmms = new MMSEntities())
            {
                var f = objmms.tblPcContractorMasters.Where(c => c.PRJID.Contains(ProjectId)).OrderBy(e => e.Name).ToList();

                var t = f.Distinct<tblPcContractorMaster>().Select(x => new SelectListItem
                {
                    Value = x.PcContractorID?.ToString().Trim(),
                    Text = x.Name?.ToString().Trim()
                });

                return Json(t.ToJSON(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetIssueMaterialType()
        {
            using (var objmms = new MMSEntities())
            {
                var data = objmms.tblFinancialSelectionTypes.Where(x => x.Status == true).Select(p => new { Value = p.TransId.ToString(), Text = p.FinancialType }).OrderBy(k => k.Text);
                return Json(data.ToJSON(), JsonRequestBehavior.AllowGet);
            }
        }

        public List<SelectListItem> GetPCVendorListByProjectId(string ProjectId)
        {
            List<SelectListItem> groups = new List<SelectListItem>();

            string Baseurl = "http://208.109.9.215/SITACENTRAL/";
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();

                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseTask = client.GetAsync("api/PMCDropdown/BindPCByProjectId?ProjectId=" + ProjectId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    string r = result.Content.ReadAsStringAsync().Result;
                    return new JavaScriptSerializer().Deserialize<List<SelectListItem>>(r);

                }
                else
                {
                    return null;
                }
            }
        }

        public JsonResult GetIssueQuantities(string ProjectId, string ItemGroupNo, string ItemNo, string VendorNo, string IndentNo, string FromDate, string ToDate, int IssueType = 0)
        {

            using (var objmms = new MMSEntities())
            {
                var vendors = GetPCVendorListByProjectId(ProjectId);

                var data = Procedure.GetData<VMIssueReturnQty>("GetIssueQuantities", ProjectId, ItemGroupNo, ItemNo, VendorNo, IndentNo, IssueType, FromDate != "" ? Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd") : "", ToDate != "" ? Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") : "");

                var data1 = (
                      from a in data
                      join b in vendors on a.VendorNo equals b.Value into mtchdt
                      from d in mtchdt.DefaultIfEmpty()
                      select new { a.Id, a.IndentNo, a.IssueDate, a.IssueQuantity, a.ItemGroupId, a.ItemName, a.GroupName, a.MaterialType, a.ItemNo, a.VendorNo, VendorName = (d == null ? string.Empty : d.Text), IssueTo = a.IssueTo }
                    ).ToList();

                return Json(new { Data = data1 }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetReasonMaster()
        {
            using (var db = new MMSEntities())
            {
                var data = db.tblReturnReasonMasters.Where(x => x.Status == true).Select(x => new { Text = x.Reason, Value = x.Id }).ToList().ToJSON();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ReturnQty(VMReturnQtyMaster ReturnMaster, List<VMIssueReturnQty> ReturnData)
        {
            if (ReturnData != null)
            {
                using (var db = new MMSEntities())
                {
                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {

                            var returnNoData = new JavaScriptSerializer().Deserialize<string[]>(GetTempReturnNo(ReturnMaster.ProjectId, ReturnMaster.ReturnDate, 1));
                            var returnNo = returnNoData[0];
                            if (db.tblReturnQtyMasters.Any(x => x.ReturnNo == returnNo))
                            {
                                throw new Exception("Return No. can not be duplicate. " + returnNoData[0]);
                            }

                            var reasonValue = db.tblReturnReasonMasters.SingleOrDefault(x => x.Id == ReturnMaster.Reason).Reason;

                            tblReturnQtyMaster master = new tblReturnQtyMaster()
                            {
                                CreatedBy = Session["EmpID"].ToString(),
                                Remarks = ReturnMaster.Remarks,
                                ReturnBy = ReturnMaster.ReturnBy,
                                ReturnReason = reasonValue,
                                ReturnResonId = ReturnMaster.Reason,
                                ReturnDate = Convert.ToDateTime(ReturnMaster.ReturnDate.GetDateTimeConverted()),
                                ReturnNo = returnNo,
                                FinancialYear = returnNoData[1],
                                ProjectId = ReturnMaster.ProjectId,
                                UniqueId = Convert.ToInt32(returnNoData[2]),
                                CreatedDate = DateTime.Now
                            };

                            db.tblReturnQtyMasters.Add(master);
                            db.SaveChanges();

                            foreach (var item in ReturnData)
                            {
                                var issuedData = db.ns_tbl_IssueQuantity.SingleOrDefault(x => x.Id == item.Id);
                                var receivedData = db.tblReceiveDatas.SingleOrDefault(x => x.TransId == issuedData.GateEntry_UId);

                                if (item.ReturnQty > issuedData.IssueQuantity || item.ReturnQty > receivedData.ReceiveQuantity || item.ReturnQty > receivedData.IssueQuantity)
                                {
                                    throw new Exception("Return Qty. can not be greater than Issued Qty. Issue Indent No. " + item.IndentNo);
                                }

                                tblReturnQtyChild child = new tblReturnQtyChild
                                {
                                    CreatedBy = Session["EmpID"].ToString(),
                                    IndentNo = item.IndentNo,
                                    CreatedDate = DateTime.Now,
                                    Issue_UId = issuedData.Id,
                                    MId = master.Id,
                                    Rate = receivedData.Rate,
                                    ReturnNo = returnNo,
                                    ReturnQty = item.ReturnQty
                                };

                                receivedData.IssueQuantity = receivedData.IssueQuantity - item.ReturnQty;
                                receivedData.BalanceQuantity = receivedData.BalanceQuantity + item.ReturnQty;
                                receivedData.ModifiedDate = DateTime.Now;

                                issuedData.ReturnQty = (issuedData.ReturnQty ?? 0) + item.ReturnQty;
                                issuedData.LastReturnDate = DateTime.Now;

                                var currentStockData = db.tblItemCurrentStocks.FirstOrDefault(x => x.ProjectNo == issuedData.ProjectID && x.ItemNo == issuedData.ItemID);

                                if (currentStockData == null)
                                {
                                    throw new Exception("Stock not has the Item : " + issuedData.ItemName);
                                }

                                currentStockData.Qty = currentStockData.Qty + item.ReturnQty;
                                db.Entry(currentStockData).State = System.Data.Entity.EntityState.Modified;

                                db.Entry(issuedData).State = System.Data.Entity.EntityState.Modified;
                                db.Entry(receivedData).State = System.Data.Entity.EntityState.Modified;
                                db.tblReturnQtyChilds.Add(child);

                                db.SaveChanges();
                            }
                            trans.Commit();
                            return Json(new { Status = 1, ReturnNo = returnNoData[0] });
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            return Json(new { Status = 3, Error = ex.Message });
                        }
                    }
                }
            }

            return Json(new { Status = 2 });
        }



        public string GetTempReturnNo(string ProjectId, string ReturnDate, int Type = 0)
        {
            if (!string.IsNullOrEmpty(ProjectId) && !string.IsNullOrEmpty(ReturnDate))
            {
                using (var db = new MMSEntities())
                {
                    var financialYear = Helpers.HelperMethods.GetFinancialYear(ReturnDate.GetDateTimeConverted());
                    var projectCode = db.tblProjectCodes.SingleOrDefault(x => x.ProjectID == ProjectId).ProjectCode;
                    var count = db.tblReturnQtyMasters.Where(x => x.ProjectId == ProjectId && x.FinancialYear == financialYear).Max(x => x.UniqueId);
                    count = count ?? 0;
                    var newNo = count + 1;

                    var returnNewCount = string.Format("{0:00000}", newNo);

                    var returnNo = Type == 0 ? ("Temp/" + "Return/" + projectCode + "/" + financialYear + "/" + returnNewCount) : ("Return/" + projectCode + "/" + financialYear + "/" + returnNewCount);

                    return new string[] { returnNo, financialYear, newNo.Value.ToString() }.ToJSON();
                }
            }

            return new string[] { "" }.ToJSON();
        }


        public ActionResult ReturnReport()
        {
            return View();
        }

        public JsonResult GetReturnReportData(string ProjectId,string ItemGroupId,string ItemId,string FromDate,string ToDate)
        {
            try
            {
                if(FromDate!="" && ToDate != "")
                {
                    FromDate = FromDate.GetDateTimeConverted().ToServerDateTimeFormatString();
                    ToDate = ToDate.GetDateTimeConverted().ToServerDateTimeFormatString();
                }
                else
                {
                    FromDate = "";
                    ToDate = "";
                }
                var param= new Dictionary<string, object>();
                param.Add("projectId", ProjectId);
                param.Add("itemGroupId", ItemGroupId);
                param.Add("itemId", ItemId);
                param.Add("fromDate", FromDate);
                param.Add("toDate", ToDate);
                //  var data = Procedure.GetDataBySP("GetReturnReportData",param).DataTableToJSON();
                var db = new MMSEntities();
                var data = db.Database.SqlQuery<ReturnQtyReport>("exec GetReturnReportData @projectid, @itemgroup, @itemid, @fromdate, @todate",
                    new System.Data.SqlClient.SqlParameter("@projectid", ProjectId),
                    new System.Data.SqlClient.SqlParameter("@itemgroup", ItemGroupId),
                    new System.Data.SqlClient.SqlParameter("@itemid", ItemId),
                    new System.Data.SqlClient.SqlParameter("@fromdate", FromDate),
                     new System.Data.SqlClient.SqlParameter("@todate", ToDate)).ToList();
                return Json(new {Status=1,Data=data }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Status = 3,Error=ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
            
        }


        public ActionResult ExportReturnReportData(string ProjectId, string ItemGroupId, string ItemId, string FromDate, string ToDate)
        {
            try
            {
                if (FromDate != "" && ToDate != "")
                {
                    FromDate = FromDate.GetDateTimeConverted().ToServerDateTimeFormatString();
                    ToDate = ToDate.GetDateTimeConverted().ToServerDateTimeFormatString();
                }
                else
                {
                    FromDate = "";
                    ToDate = "";
                }

                string ProjectName = new MMSEntities().tblProjectMasters.SingleOrDefault(x => x.PRJID == ProjectId).ProjectName;

                var param = new Dictionary<string, object>();
                param.Add("projectId", ProjectId);
                param.Add("itemGroupId", ItemGroupId);
                param.Add("itemId", ItemId);
                param.Add("fromDate", FromDate);
                param.Add("toDate", ToDate);
                var data = Procedure.GetDataBySP("GetReturnReportData", param);

                var workbook = new XLWorkbook();
                var sheet1 = workbook.AddWorksheet(data, "ReturnQuantityReport");


                var row = sheet1.Row(1).InsertRowsAbove(4).ToList();
                var rc1 = row[0].Cell(1);
                var rc2 = row[1].Cell(1);
                var rc3 = row[2].Cell(1);
                rc1.Style.Font.FontSize = 23;
                rc2.Style.Font.FontSize = 22;
                rc3.Style.Font.FontSize = 20;
                
                rc1.Value = "AHLUWALIA CONTRACTS (INDIA) LTD.";
                rc2.Value = "Site- " + ProjectName;
                rc3.Value = "Return Quantity Report";
                string filename = Guid.NewGuid().ToString();

                workbook.SaveAs(System.IO.Path.GetTempPath() + filename + ".xlsx");
                return File(System.IO.Path.GetTempPath() + filename + ".xlsx", "application/vnd.ms-excel", "ReturnQuantityReport.xlsx");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ReturnReport");
            }


        }
        


    }
}