using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.Models;
using MMS.DAL;
using System.Web.Helpers;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using iTextSharp.text;
using MMS.ViewModels;
using iTextSharp.text.pdf;
using System.Text;
using ClosedXML.Excel;
using System.Globalization;

namespace MMS.Controllers.Reports
{
    public class MSPReportController : Controller
    {

        // GET: MSPReport
        MMSEntities objmms = new MMSEntities();
        MSP_Model objmsps = new MSP_Model();

        [Authorize]
        public ActionResult Index() //int m, int y, string PID, string GID
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;
            //var a = objmms.tblEmployeeMasters.Where(b => b.EmpID == empId).OrderBy(c => c.TransID).ToList();
            //var t = a.Select(x => new SelectListItem
            //{
            //    Value = x.ProjectID.ToString(),
              
            //    Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectID).First().ProjectName
            //});
            //ViewBag.prjtid = t;
            ViewBag.itemgroupname = new SelectList(objmms.tblItemGroupMasters, "ItemGroupID", "GroupName").OrderBy(k => k.Text);

            List<SelectListItem> tmpList = new List<SelectListItem>();
            tmpList.Add(new SelectListItem { Text = "January", Value = "1" });
            tmpList.Add(new SelectListItem { Text = "February", Value = "2" });
            tmpList.Add(new SelectListItem { Text = "March", Value = "3" });
            tmpList.Add(new SelectListItem { Text = "April", Value = "4" });
            tmpList.Add(new SelectListItem { Text = "May", Value = "5" });
            tmpList.Add(new SelectListItem { Text = "June", Value = "6" });
            tmpList.Add(new SelectListItem { Text = "July", Value = "7" });
            tmpList.Add(new SelectListItem { Text = "August", Value = "8" });
            tmpList.Add(new SelectListItem { Text = "September", Value = "9" });
            tmpList.Add(new SelectListItem { Text = "October", Value = "10" });
            tmpList.Add(new SelectListItem { Text = "November", Value = "11" });
            tmpList.Add(new SelectListItem { Text = "December", Value = "12" });

            ViewBag.Months = tmpList;

            return View(); //View("_PartialView_MSP", result);
        }

        public ActionResult GetAllDatas(int m, int y, string PID, string GID, string Names)
        {
            List<Get_MSP_Record> result = new List<Get_MSP_Record>();
            if (PID != null && GID != null && GID != "" && y != 0 && m != 0)
            {
                result = objmsps.GetAllMspData(m, y, PID, GID);
                TempData["mydata"] = result;
                Session["keepdata"] = result;
                TempData.Keep("mydata");
                Session["GID"] = GID;
                Session["m"] = m;
                Session["y"] = y;
                Session["ProId"] = PID;
            }

            else
            {
                return null;
            }

            return View("_PartialView_MSP", result);
        }


        public ActionResult ExportMSP(string PId,string GId,int m,int y)
        {
            string ProjectName = objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == PId).ProjectName;

            string Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m);

            var data = objmsps.PrintMSP(PId,GId,m,y);
            var workbook = new XLWorkbook();
           var sheet1= workbook.AddWorksheet(data,"MSPReport");


            var row = sheet1.Row(1).InsertRowsAbove(4).ToList();
            var rc1  =row[0].Cell(1);
            var rc2 = row[1].Cell(1);
            var rc3 = row[2].Cell(1);
            var rc4 = row[3].Cell(1);
            rc1.Style.Font.FontSize = 23;
            rc2.Style.Font.FontSize = 22;
            rc3.Style.Font.FontSize = 20;
            rc4.Style.Font.FontSize = 18;

            rc1.Value = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            rc2.Value = "Site- "+ProjectName;
            rc3.Value = "MSP Report";
            rc4.Value = $"Monthly Stock Position For The Month Of: {Month} - {y} ";


            var dataList = data.AsEnumerable();

            var d = (from a in dataList
                    group a by a.Field<string>("Group Name") into g
                    select new{ GroupName = g.Key,
                        OpeningBalanceQty = g.Sum(x => x.Field<decimal>("Opening Balance Qty")),
                        RecvUptoLastMonth = g.Sum(x => x.Field<decimal>("Recd Upto Last Month")),
                        RecvDuringCurrentMonth = g.Sum(x => x.Field<decimal>("Recd During Current Month")),
                        RecvTotalUpto = g.Sum(x => x.Field<decimal>("Recd Total Upto")),
                        IssuedUptoLastMonth = g.Sum(x => x.Field<decimal>("Issued Upto Last Month")),
                        IssuedDuringCurrentMonth = g.Sum(x => x.Field<decimal>("Issued During Current Month")),
                        IssuedTotalUpto = g.Sum(x => x.Field<decimal>("Issued Total Upto")),
                        ClosingBalance = g.Sum(x => x.Field<decimal>("Closing Balance")),

                        OpeningBalanceAmt = g.Sum(x => x.Field<decimal>("Opening Balance Amt")),
                        RecvAmtUptoLastMonth = g.Sum(x => x.Field<decimal>("Recd Amt Upto Last Month")),
                        RecvAmtDuringCurrentMonth = g.Sum(x => x.Field<decimal>("Recd Amt During Current Month")),
                        RecvAmtTotalUpto = g.Sum(x => x.Field<decimal>("Recd Amt Total Upto")),
                        IssuedAmtUptoLastMonth = g.Sum(x => x.Field<decimal>("Issued Amt Upto Last Month")),
                        IssuedAmtDuringCurrentMonth = g.Sum(x => x.Field<decimal>("Issued Amt During Current Month")),
                        IssuedAmtTotalUpto = g.Sum(x => x.Field<decimal>("Issued Amt Total Upto")),
                        ClosingBalanceAmt = g.Sum(x => x.Field<decimal>("Closing Balance Amt"))
                    }).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Group Name");
            dt.Columns.Add("Opening Balance Qty Sum");
            dt.Columns.Add("Recd Upto Last Month Sum");
            dt.Columns.Add("Recd During Current Month Sum");
            dt.Columns.Add("Recd Total Upto Sum");
            dt.Columns.Add("Issued Upto Last Month Sum");
            dt.Columns.Add("Issued During Current Month Sum");
            dt.Columns.Add("Issued Total Upto Sum");
            dt.Columns.Add("Closing Balance Sum");

            dt.Columns.Add("Opeing Balance Amt Sum");
            dt.Columns.Add("Recd Amt Upto Last Month Sum");
            dt.Columns.Add("Recd Amt During Current Month Sum");
            dt.Columns.Add("Recd Amt Total Upto Sum");
            dt.Columns.Add("Issued Amt Upto Last Month Sum");
            dt.Columns.Add("Issued Amt During Current Month Sum");
            dt.Columns.Add("Issued Amt Total Upto Sum");
            dt.Columns.Add("Closing Balance Amt Sum");

            List<DataRow> rows = new List<DataRow>();

            foreach (var item in d)
            {
                dt.Rows.Add(item.GroupName, item.OpeningBalanceQty, item.RecvUptoLastMonth, item.RecvDuringCurrentMonth, item.RecvTotalUpto, item.IssuedUptoLastMonth, item.IssuedDuringCurrentMonth, item.IssuedTotalUpto, item.ClosingBalance,
                    item.OpeningBalanceAmt, item.RecvAmtUptoLastMonth, item.RecvAmtDuringCurrentMonth, item.RecvAmtTotalUpto, item.IssuedAmtUptoLastMonth, item.IssuedAmtDuringCurrentMonth, item.IssuedAmtTotalUpto, item.ClosingBalanceAmt
                    );
            }

            if (dt.Rows.Count > 0 && d!=null && d.Count>0)
            {
                dt.Rows.Add("Grand Total-",d.Sum(x=>x.OpeningBalanceQty), 
                    d.Sum(x => x.RecvUptoLastMonth), 
                    d.Sum(x => x.RecvDuringCurrentMonth),
                    d.Sum(x => x.RecvTotalUpto), 
                    d.Sum(x => x.IssuedUptoLastMonth),
                    d.Sum(x => x.IssuedDuringCurrentMonth),
                    d.Sum(x => x.IssuedTotalUpto),
                    d.Sum(x => x.ClosingBalance),
                    d.Sum(x => x.OpeningBalanceAmt),
                    d.Sum(x => x.RecvAmtUptoLastMonth),
                    d.Sum(x => x.RecvAmtDuringCurrentMonth),
                    d.Sum(x => x.RecvAmtTotalUpto),
                    d.Sum(x => x.IssuedAmtUptoLastMonth), 
                    d.Sum(x => x.IssuedAmtDuringCurrentMonth),
                    d.Sum(x => x.IssuedAmtTotalUpto),
                    d.Sum(x => x.ClosingBalanceAmt)
                    );
            }

           var sheet2= workbook.AddWorksheet(dt, "GroupWiseSum");

            var r = sheet2.Row(1).InsertRowsAbove(4).ToList();
            var rc5 = r[0].Cell(1);
            var rc6 = r[1].Cell(1);
            var rc7 = r[2].Cell(1);
            var rc8 = r[3].Cell(1);
            rc5.Style.Font.FontSize = 23;
            rc6.Style.Font.FontSize = 22;
            rc7.Style.Font.FontSize = 20;
            rc8.Style.Font.FontSize = 18;

            rc5.Value = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            rc6.Value = "Site- " + ProjectName;
            rc7.Value = "MSP Report";
            rc8.Value = $"Monthly Stock Position For The Month Of: {Month} - {y} ";


            string filename = Guid.NewGuid().ToString();

            workbook.SaveAs(Path.GetTempPath() + filename + ".xlsx");
            return File(Path.GetTempPath() + filename + ".xlsx", "application/vnd.ms-excel", "MSPReport.xlsx");
        }


        public ActionResult ExportMSP1(string PId, string GId, int m, int y)
        {
            string ProjectName = objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == PId).ProjectName;

            string Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m);

            var data = objmsps.PrintMSP1(PId, GId, m, y);
            var workbook = new XLWorkbook();
            var sheet1 = workbook.AddWorksheet(data, "MSPReport");

            var row = sheet1.Row(1).InsertRowsAbove(4).ToList();
            var rc1 = row[0].Cell(1);
            var rc2 = row[1].Cell(1);
            var rc3 = row[2].Cell(1);
            var rc4 = row[3].Cell(1);
            rc1.Style.Font.FontSize = 23;
            rc2.Style.Font.FontSize = 22;
            rc3.Style.Font.FontSize = 20;
            rc4.Style.Font.FontSize = 18;

            rc1.Value = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            rc2.Value = "Site- " + ProjectName;
            rc3.Value = "MSP Report";
            rc4.Value = $"Monthly Stock Position For The Month Of: {Month} - {y} ";


            var dataList = data.AsEnumerable();

            var d = (from a in dataList
                     group a by a.Field<string>("Group Name") into g
                     select new
                     {
                         GroupName = g.Key,
                         OpeningBalanceQty = g.Sum(x => x.Field<decimal>("Opening Balance Qty")),
                         RecvUptoLastMonth = g.Sum(x => x.Field<decimal>("Recd Upto Last Month")),
                         RecvDuringCurrentMonth = g.Sum(x => x.Field<decimal>("Recd During Current Month")),
                         RecvTotalUpto = g.Sum(x => x.Field<decimal>("Recd Total Upto")),
                         IssuedUptoLastMonth = g.Sum(x => x.Field<decimal>("Issued Upto Last Month")),
                         IssuedDuringCurrentMonth = g.Sum(x => x.Field<decimal>("Issued During Current Month")),
                         IssuedTotalUpto = g.Sum(x => x.Field<decimal>("Issued Total Upto")),
                         ClosingBalance = g.Sum(x => x.Field<decimal>("Closing Balance")),

                         OpeningBalanceAmt = g.Sum(x => x.Field<decimal>("Opening Balance Amt")),
                         RecvAmtUptoLastMonth = g.Sum(x => x.Field<decimal>("Recd Amt Upto Last Month")),
                         RecvAmtDuringCurrentMonth = g.Sum(x => x.Field<decimal>("Recd Amt During Current Month")),
                         RecvAmtTotalUpto = g.Sum(x => x.Field<decimal>("Recd Amt Total Upto")),
                         IssuedAmtUptoLastMonth = g.Sum(x => x.Field<decimal>("Issued Amt Upto Last Month")),
                         IssuedAmtDuringCurrentMonth = g.Sum(x => x.Field<decimal>("Issued Amt During Current Month")),
                         IssuedAmtTotalUpto = g.Sum(x => x.Field<decimal>("Issued Amt Total Upto")),
                         ClosingBalanceAmt = g.Sum(x => x.Field<decimal>("Closing Balance Amt"))
                     }).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Group Name");
            dt.Columns.Add("Opening Balance Qty Sum");
            dt.Columns.Add("Recd Upto Last Month Sum");
            dt.Columns.Add("Recd During Current Month Sum");
            dt.Columns.Add("Recd Total Upto Sum");
            dt.Columns.Add("Issued Upto Last Month Sum");
            dt.Columns.Add("Issued During Current Month Sum");
            dt.Columns.Add("Issued Total Upto Sum");
            dt.Columns.Add("Closing Balance Sum");

            dt.Columns.Add("Opeing Balance Amt Sum");
            dt.Columns.Add("Recd Amt Upto Last Month Sum");
            dt.Columns.Add("Recd Amt During Current Month Sum");
            dt.Columns.Add("Recd Amt Total Upto Sum");
            dt.Columns.Add("Issued Amt Upto Last Month Sum");
            dt.Columns.Add("Issued Amt During Current Month Sum");
            dt.Columns.Add("Issued Amt Total Upto Sum");
            dt.Columns.Add("Closing Balance Amt Sum");

            List<DataRow> rows = new List<DataRow>();

            foreach (var item in d)
            {
                dt.Rows.Add(item.GroupName, item.OpeningBalanceQty, item.RecvUptoLastMonth, item.RecvDuringCurrentMonth, item.RecvTotalUpto, item.IssuedUptoLastMonth, item.IssuedDuringCurrentMonth, item.IssuedTotalUpto, item.ClosingBalance,
                    item.OpeningBalanceAmt, item.RecvAmtUptoLastMonth, item.RecvAmtDuringCurrentMonth, item.RecvAmtTotalUpto, item.IssuedAmtUptoLastMonth, item.IssuedAmtDuringCurrentMonth, item.IssuedAmtTotalUpto, item.ClosingBalanceAmt
                    );
            }

            if (dt.Rows.Count > 0 && d != null && d.Count > 0)
            {
                dt.Rows.Add("Grand Total-", d.Sum(x => x.OpeningBalanceQty),
                    d.Sum(x => x.RecvUptoLastMonth),
                    d.Sum(x => x.RecvDuringCurrentMonth),
                    d.Sum(x => x.RecvTotalUpto),
                    d.Sum(x => x.IssuedUptoLastMonth),
                    d.Sum(x => x.IssuedDuringCurrentMonth),
                    d.Sum(x => x.IssuedTotalUpto),
                    d.Sum(x => x.ClosingBalance),
                    d.Sum(x => x.OpeningBalanceAmt),
                    d.Sum(x => x.RecvAmtUptoLastMonth),
                    d.Sum(x => x.RecvAmtDuringCurrentMonth),
                    d.Sum(x => x.RecvAmtTotalUpto),
                    d.Sum(x => x.IssuedAmtUptoLastMonth),
                    d.Sum(x => x.IssuedAmtDuringCurrentMonth),
                    d.Sum(x => x.IssuedAmtTotalUpto),
                    d.Sum(x => x.ClosingBalanceAmt)
                    );
            }

            var sheet2 = workbook.AddWorksheet(dt, "GroupWiseSum");

            var r = sheet2.Row(1).InsertRowsAbove(4).ToList();
            var rc5 = r[0].Cell(1);
            var rc6 = r[1].Cell(1);
            var rc7 = r[2].Cell(1);
            var rc8 = r[3].Cell(1);
            rc5.Style.Font.FontSize = 23;
            rc6.Style.Font.FontSize = 22;
            rc7.Style.Font.FontSize = 20;
            rc8.Style.Font.FontSize = 18;

            rc5.Value = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            rc6.Value = "Site- " + ProjectName;
            rc7.Value = "MSP Report";
            rc8.Value = $"Monthly Stock Position For The Month Of: {Month} - {y} ";


            string filename = Guid.NewGuid().ToString();

            workbook.SaveAs(Path.GetTempPath() + filename + ".xlsx");
            return File(Path.GetTempPath() + filename + ".xlsx", "application/vnd.ms-excel", "MSPReport.xlsx");
        }


        public ActionResult Filter(string Names)
        {
            if (Names != null && Names != "")
            {
                List<Get_MSP_Record> result = (List<Get_MSP_Record>)TempData["mydata"];
                var filters = result.Where(k => k.ItemDescription == Names.Trim()).ToList();
                result = filters;
                return View("_PartialView_MSP", result);
            }
            else
            {
                List<Get_MSP_Record> result = (List<Get_MSP_Record>)TempData["mydata"];
                return View("_PartialView_MSP", result);
            }
        }

        public void GetExcel()
        {
            List<Get_MSP_Record> allCust = (List<Get_MSP_Record>)Session["keepdata"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                        grid1.Column("ItemCode", header: "Item Code", canSort: false),
                        grid1.Column("ItemDescription", header: "ItemDescription", canSort: false),
                        grid1.Column("Unit", header: "Unit Name", canSort: false),
                         grid1.Column("RateForThisMonth", header: "Rate For This Month", canSort: false),
                          grid1.Column("OpeningBalanceQty", header: "Opening Balance Qty", canSort: false),
                        grid1.Column("RecvUptoLastMonth", header: "Recd.Upto Last Month", canSort: false),
                        grid1.Column("RecvDuringCurrentMonth", header: "Recd.During Current Month"),
                        grid1.Column("RecvTotalUpto", header: "Recd. Total Upto "),
                        grid1.Column("IssuedUptoLastMonth", header: "Issued Upto Last Month", canSort: false),
                        grid1.Column("IssuedDuringCurrentMonth", header: "Issued During Current Month"),
                         grid1.Column("IssuedTotalUpto", header: "Issued Total Upto", canSort: false),
                          grid1.Column("ClosingBalance", header: "Closing Balance", canSort: false),
                           grid1.Column("OpeningBalanceAmt", header: "Opening Balance Amt", canSort: false),
                         grid1.Column("RecvAmtUptoLastMonth", header: "Recd. Amt Upto Last Month", canSort: false),
                         grid1.Column("RecvAmtDuringCurrentMonth", header: "Recd. Amt During Current Month"),
                         grid1.Column("RecvAmtTotalUpto", header: "Recd. Amt Total Upto"),
                         grid1.Column("IssuedAmtUptoLastMonth", header: "Issued Amt Upto Last Month", canSort: false),
                         grid1.Column("IssuedAmtDuringCurrentMonth", header: "Issued Amt During Curren tMonth", canSort: false),
                         grid1.Column("IssuedAmtTotalUpto", header: "Issued Amt Total Upto"),
                          grid1.Column("ClosingBalanceAmt", header: "Closing Balance Amt", canSort: false),
                          grid1.Column("CurrentMonthReturnQty", header: "Current Month Return Qty", canSort: false),
                          grid1.Column("CurrentMonthReturnAmt", header: "Current Month Return Amt", canSort: false)
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=MSPReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }

        public FileStreamResult GETPdf()
        {
            List<Get_MSP_Record> allCust = (List<Get_MSP_Record>)Session["keepdata"];

            string gn = Session["GID"].ToString();
            string mn = Session["m"].ToString();
            string yr = Session["y"].ToString();
            var gn1 = objmms.tblItemGroupMasters.Where(a2 => a2.ItemGroupID == gn).ToList().Select(a1 => a1.GroupName).First();

            int month = Convert.ToInt16(mn);
            var monthAbbr = System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(month);

            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();

            var Opnblamt = from k in allCust
                     group k by k.ItemDescription into g
                     select new
                     { SUM_OpeningBLAmt = g.Sum(o => o.OpeningBalanceAmt) ,
                         SUM_RcvdAmtUptoLastMonth = g.Sum(o=>o.RecvAmtUptoLastMonth),
                         SUM_RcvdAmtDuringCurrntMonth = g.Sum(o=>o.RecvAmtDuringCurrentMonth),
                         SUM_RcvdAmtTotalUpto = g.Sum(o=>o.RecvAmtTotalUpto),
                         SUM_IssueAmtUptoLastMonth = g.Sum(o=>o.IssuedAmtUptoLastMonth),
                         SUM_IssueAmtDurrCurrntMonth = g.Sum(o=>o.IssuedAmtDuringCurrentMonth),
                         SUM_IssAmtTotalUpto = g.Sum(o=>o.IssuedAmtTotalUpto),
                         SUM_ClosingBLAmt = g.Sum(o=>o.ClosingBalanceAmt)
                     };
            var totOpeningBLAmt = Math.Round((decimal)(Opnblamt.Sum(l => l.SUM_OpeningBLAmt)), 2);
            var totRcvdAmtUptoLastMonth = Math.Round((decimal)(Opnblamt.Sum(l => l.SUM_RcvdAmtUptoLastMonth)), 2);
            var totRcvdAmtDuringCurrntMonth = Math.Round((decimal)(Opnblamt.Sum(l => l.SUM_RcvdAmtDuringCurrntMonth)), 2);
            var totRcvdAmtTotalUpto = Math.Round((decimal)(Opnblamt.Sum(l => l.SUM_RcvdAmtTotalUpto)), 2);
            var totIssueAmtUptoLastMonth = Math.Round((decimal)(Opnblamt.Sum(l => l.SUM_IssueAmtUptoLastMonth)), 2);
            var totIssueAmtDurrCurrntMonth = Math.Round((decimal)(Opnblamt.Sum(l => l.SUM_IssueAmtDurrCurrntMonth)), 2);
            var totIssAmtTotalUpto = Math.Round((decimal)(Opnblamt.Sum(l => l.SUM_IssAmtTotalUpto)), 2);
            var totClosingBLAmt = Math.Round((decimal)(Opnblamt.Sum(l => l.SUM_ClosingBLAmt)), 2);

            //ViewBag.OpeningBLAmt = allCust.Sum(k => k.OpeningBalanceAmt);
            //ViewBag.RcvdAmtUptoLastMonth = allCust.Sum(k => k.RecvAmtUptoLastMonth);
            //ViewBag.RcvdAmtDuringCurrntMonth = allCust.Sum(k => k.RecvAmtDuringCurrentMonth);
            //ViewBag.RcvdAmtTotalUpto = allCust.Sum(k => k.RecvAmtTotalUpto);
            //ViewBag.IssueAmtUptoLastMonth = allCust.Sum(k => k.IssuedAmtUptoLastMonth);
            //ViewBag.IssueAmtDurrCurrntMonth = allCust.Sum(k => k.IssuedAmtDuringCurrentMonth);
            //ViewBag.IssAmtTotalUpto = allCust.Sum(k => k.IssuedAmtTotalUpto);
            //ViewBag.ClosingBLAmt = allCust.Sum(k => k.ClosingBalanceAmt);



            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridData = grid1.GetHtml(
                 tableStyle: "full",
                columns: grid1.Columns(
                        grid1.Column(header:"S.N",format: (item) => item.WebGrid.Rows.IndexOf(item) + 1,style: "small"),
                        grid1.Column("ItemCode", header: "Item Code", canSort: false),
                        grid1.Column("ItemDescription", header: "Item Desc", canSort: false),
                        grid1.Column("Unit", header: "Unit", canSort: false,style:"widthR"),
                        //grid1.Column("RateForThisMonth", header: "Rate For This Month", canSort: false),
                        grid1.Column("OpeningBalanceQty", header: "Opening Balance Qty", canSort: false,style: "right"),
                        grid1.Column("RecvUptoLastMonth", header: "Recd Upto Last Month", canSort: false, style: "right"),
                        grid1.Column("RecvDuringCurrentMonth", header: "Recd During Current Month", style: "right"),
                        grid1.Column("RecvTotalUpto", header: "Recd Total Upto ", style: "right"),
                        grid1.Column("IssuedUptoLastMonth", header: "Issued Upto Last Month", canSort: false, style: "right"),
                        grid1.Column("IssuedDuringCurrentMonth", header: "Issued During Current Month", style: "right"),
                        grid1.Column("IssuedTotalUpto", header: "Issued Total Upto", canSort: false, style: "right"),
                        grid1.Column("ClosingBalance", header: "Closing Balance", canSort: false, style: "right c1"),
                        grid1.Column("OpeningBalanceAmt", header: "Opening Balance Amt", canSort: false, style: "right c2"),
                        grid1.Column("RecvAmtUptoLastMonth", header: "Recd Amt Upto Last Month", canSort: false, style: "right c3"),
                        grid1.Column("RecvAmtDuringCurrentMonth", header: "Recd Amt During Current Month", style: "right c4"),
                        grid1.Column("RecvAmtTotalUpto", header: "Recd Amt Total Upto", style: "right c5"),
                        grid1.Column("IssuedAmtUptoLastMonth", header: "Issued Amt Upto Last Month", canSort: false, style: "right c6"),
                         grid1.Column("IssuedAmtDuringCurrentMonth", header: "Issued Amt During Curren tMonth", canSort: false, style: "right c7"),
                        grid1.Column("IssuedAmtTotalUpto", header: "Issued Amt Total Upto", style: "right c8"),
                        grid1.Column("ClosingBalanceAmt", header: "Closing Balance Amt", canSort: false, style: "right c9"),
                        grid1.Column("CurrentMonthReturnQty", header: "Current Month Return Qty", style: "right"),
                          grid1.Column("CurrentMonthReturnAmt", header: "Current Month Return Amt", style: "right")
                        )
                    ).ToString();

            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "MSP Report";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>Monthly Stock Position For The Month Of: " + monthAbbr + "  - " + yr + " </div><div style='text-align:left'>Group Name: " + gn1 + " </div><br/>";

            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 9, 9, 30, 50);

            var writer = PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;
            document.Open();

            Paragraph PHeader = new Paragraph(header, FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));
            Paragraph PSiteName = new Paragraph(siteName, FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));
            Paragraph PReportName = new Paragraph(reportName, FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));
            Paragraph PDateTime = new Paragraph(dateTime, FontFactory.GetFont(FontFactory.TIMES, 8, iTextSharp.text.Font.BOLD));


            PHeader.Alignment = Element.ALIGN_CENTER;
            PSiteName.Alignment = Element.ALIGN_CENTER;
            PReportName.Alignment = Element.ALIGN_CENTER;
            PDateTime.Alignment = Element.ALIGN_RIGHT;

            PReportName.SpacingAfter = 10f;

            document.Add(img);
            document.Add(PDateTime);
            document.Add(PHeader);
            document.Add(PSiteName);
            document.Add(PReportName);

            DataString.Append(additionalData);
            DataString.Append(gridData);
            writer.PageEvent = new HeaderFooter("ACIL");
            using (StringReader sr = new StringReader(DataString.ToString()))
            {

                var styleSheet = new iTextSharp.text.html.simpleparser.StyleSheet();

                styleSheet.LoadTagStyle("table", "border", "1");
                styleSheet.LoadTagStyle("table", "size", "12px");
                styleSheet.LoadTagStyle("table td", "text-align", "center");
                styleSheet.LoadTagStyle("thead, tfoot", "display", "table-row-group");
                styleSheet.LoadStyle("right", "align", "right");
                styleSheet.LoadStyle("widthR", "width", "15");
                styleSheet.LoadStyle("small","width","8");
                styleSheet.LoadTagStyle(iTextSharp.text.html.HtmlTags.TH, iTextSharp.text.html.HtmlTags.BGCOLOR, "#cccccc");

                //Get our list of elements (only 1 in this case)
                List<IElement> elements = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr, styleSheet);

                foreach (IElement el in elements)
                {
                    //If the element is a table manually set its header row count
                    if (el is PdfPTable)
                    {
                        PdfPTable table = ((PdfPTable)el);
                        table.HeaderRows = 1;
                    }
                    document.Add(el);
                }

            }
            string totalTable = "<table width='100%' align='right' class='fr' style='border-top:0;'><tr><td class='c10' colspan='12' align='right'><strong>Total</strong></td><td class='right c9'>" + totOpeningBLAmt + "</td><td class='right c9'>" + totRcvdAmtUptoLastMonth + "</td><td class='right c9'>" + totRcvdAmtDuringCurrntMonth + "</td><td class='right c9'>" + totRcvdAmtTotalUpto + "</td><td class='right c9'>" + totIssueAmtUptoLastMonth + "</td><td class='right c10'>" + totIssueAmtDurrCurrntMonth + "</td><td class='right c10'>" + totIssAmtTotalUpto + "</td><td class='right c10'>" + totClosingBLAmt + "</td></tr></table>";
            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            //styleSheet1.LoadTagStyle("table", "float", "right");
            styleSheet1.LoadTagStyle("table", "size", "16px");
            styleSheet1.LoadTagStyle("table", "width", "70%");
            styleSheet1.LoadStyle("right", "align", "right");
            List<IElement> element = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr1, styleSheet1);

            foreach (IElement el in element)
            {
                document.Add(el);
            }


            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }


        public ActionResult POVettedReport()
        {
            Session["VettedPOList"] = null ;
            Session["Month_Name"] = null;
            Session["Year"] = null;
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            List<SelectListItem> tmpList = new List<SelectListItem>();
            tmpList.Add(new SelectListItem { Text = "January", Value = "1" });
            tmpList.Add(new SelectListItem { Text = "February", Value = "2" });
            tmpList.Add(new SelectListItem { Text = "March", Value = "3" });
            tmpList.Add(new SelectListItem { Text = "April", Value = "4" });
            tmpList.Add(new SelectListItem { Text = "May", Value = "5" });
            tmpList.Add(new SelectListItem { Text = "June", Value = "6" });
            tmpList.Add(new SelectListItem { Text = "July", Value = "7" });
            tmpList.Add(new SelectListItem { Text = "August", Value = "8" });
            tmpList.Add(new SelectListItem { Text = "September", Value = "9" });
            tmpList.Add(new SelectListItem { Text = "October", Value = "10" });
            tmpList.Add(new SelectListItem { Text = "November", Value = "11" });
            tmpList.Add(new SelectListItem { Text = "December", Value = "12" });

            ViewBag.Months = tmpList;
            return View();
        }

        public ActionResult GetPOVettedReport(string Month, string Year,string MonthName)
        {
            string EmpId = string.Empty;
            Procedure obj = new Procedure();
            List<GetPOVettingReport> POv = null;

            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            EmpId = Session["EmpID"].ToString();
            if (Month != null && Year != "")
            {
                POv = obj.GetAllPOVettedReport(Convert.ToInt32(Month), Convert.ToInt32(Year), EmpId);

                Session["VettedPOList"] = POv.ToList();
                Session["Month_Name"] = MonthName;
                Session["Year"] = Year;
                return PartialView("_GetVettedPODetail", POv.ToList());
            }
            else {
                return PartialView("_EmptyView");
            }
        }

        public FileStreamResult PO_Vetted_Report()
        {
            string MonthName = string.Empty; string Year = string.Empty;
            List<GetPOVettingReport> lst = (List<GetPOVettingReport>)Session["VettedPOList"];
            MonthName = Session["Month_Name"].ToString();
            Year = Session["Year"].ToString();
            if (MonthName != null && Year != null)
            {

                var ss = from k in lst.ToList()
                         group k by k.ProjectId into g
                         select new
                         {
                             SumofCountVPOUpto = g.Sum(x => x.TotalNoOfPOUptoLastMonth),
                             SumofTotalVPUpto = g.Sum(x => x.NetGrandTotalOfTotalNoOfPoUptoLastMonth),
                             SumofCountVPODuring = g.Sum(x => x.TotalNoOfPODuringCurrentMonth),
                             SumofTotalVPODuring = g.Sum(x => x.NetGrandTotalOfTotalNoofPODuringCurrentMonth),
                             SumofCountCommulativePO = g.Sum(x => x.NetTotalPO),
                             SumofTotalCommulativePO = g.Sum(x => x.NetCommulativeGrand)
                         };

                WebGrid grid1 = new WebGrid(source: lst, canPage: false, canSort: false);
                string gridHtml = grid1.GetHtml(
                        columns: grid1.Columns(
                       grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                       grid1.Column("ProjectName", header: "Name of Project", style: "my-classL"),
                            grid1.Column("TotalNoOfPOUptoLastMonth", header: "No. Of POs Vetted upto Last Month", style: "right"),
                             grid1.Column("NetGrandTotalOfTotalNoOfPoUptoLastMonth", header: "Amount of POs Vetted upto Last Month", style: "right"),
                              grid1.Column("TotalNoOfPODuringCurrentMonth", header: "No. of POs Vetted Duiring Period", style: "right"),
                               grid1.Column("NetGrandTotalOfTotalNoofPODuringCurrentMonth", header: "Amount of POs Vetted Duiring Period", style: "right"),
                               grid1.Column("NetTotalPO", header: "No. of Cumulative  POs Vetted upto date", style: "right"),
                                grid1.Column("NetCommulativeGrand", header: "Amount of Cumulative  POs Vetted upto date", style: "right")
                           )
                        ).ToString();

                string ImageUrl = Server.MapPath("/images/logo.png");
                Image img = Image.GetInstance(ImageUrl);
                img.ScaleToFit(40f, 40f);
                img.Alignment = Element.ALIGN_CENTER;

                StringBuilder DataString = new StringBuilder();
                string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
                string reportName = "PO Vetting REPORT";
                string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                string additionalData = "<div style='text-align:left'>Month : " + MonthName + ", Year: " + Year + " </div><br/>";

                var output = new MemoryStream();
                var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 50, 50, 30, 50);

                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                Paragraph PHeader = new Paragraph(header, FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));
                Paragraph PReportName = new Paragraph(reportName, FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));
                Paragraph PDateTime = new Paragraph(dateTime, FontFactory.GetFont(FontFactory.TIMES, 8, iTextSharp.text.Font.BOLD));


                PHeader.Alignment = Element.ALIGN_CENTER;
                PReportName.Alignment = Element.ALIGN_CENTER;
                PDateTime.Alignment = Element.ALIGN_RIGHT;

                PReportName.SpacingAfter = 10f;

                document.Add(img);
                document.Add(PDateTime);
                document.Add(PHeader);
                document.Add(PReportName);

                DataString.Append(additionalData);
                DataString.Append(gridHtml);
                writer.PageEvent = new HeaderFooter("ACIL");
                using (StringReader sr = new StringReader(DataString.ToString()))
                {

                    var styleSheet = new iTextSharp.text.html.simpleparser.StyleSheet();

                    styleSheet.LoadTagStyle("table", "border", "1");
                    styleSheet.LoadTagStyle("table", "size", "12px");
                    styleSheet.LoadTagStyle("table td", "text-align", "center");
                    styleSheet.LoadTagStyle("thead, tfoot", "display", "table-row-group");
                    styleSheet.LoadStyle("right", "align", "right");
                    styleSheet.LoadTagStyle(iTextSharp.text.html.HtmlTags.TH, iTextSharp.text.html.HtmlTags.BGCOLOR, "#cccccc");

                    //Get our list of elements (only 1 in this case)
                    List<IElement> elements = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr, styleSheet);

                    foreach (IElement el in elements)
                    {
                        //If the element is a table manually set its header row count
                        if (el is PdfPTable)
                        {
                            PdfPTable table = ((PdfPTable)el);
                            table.HeaderRows = 1;
                        }
                        document.Add(el);
                    }

                }
               
                document.Close();
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");

            }
            else {
                return new FileStreamResult(null, "application/pdf");
            }


        }

        public void GetVettedExcel()
        {
            List<GetPOVettingReport> lst = (List<GetPOVettingReport>)Session["VettedPOList"];

            WebGrid grid1 = new WebGrid(source: lst, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                      columns: grid1.Columns(
                     grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("ProjectName", header: "Name of Project", style: "my-classL"),
                          grid1.Column("TotalNoOfPOUptoLastMonth", header: "No. Of POs Vetted upto Last Month", style: "my-classR"),
                           grid1.Column("NetGrandTotalOfTotalNoOfPoUptoLastMonth", header: "Amount of POs Vetted upto Last Month", style: "my-classR"),
                            grid1.Column("TotalNoOfPODuringCurrentMonth", header: "No. of POs Vetted Duiring Period", style: "my-classR"),
                             grid1.Column("NetGrandTotalOfTotalNoofPODuringCurrentMonth", header: "Amount of POs Vetted Duiring Period", style: "my-classR"),
                             grid1.Column("NetTotalPO", header: "No. of Cumulative  POs Vetted upto date", style: "my-classR"),
                              grid1.Column("NetCommulativeGrand", header: "Amount of Cumulative  POs Vetted upto date", style: "my-classR")

                           )
                        ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=POVettedReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridHtml);
            Response.End();

        }

    }
}