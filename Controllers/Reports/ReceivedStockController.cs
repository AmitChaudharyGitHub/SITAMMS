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
using System.Linq.Dynamic;
using System.Data.SqlClient;
using MMS.Models;
using System.Configuration;
using MMS.DAL;
using System.IO;
using iTextSharp.text;
using System.Web.Helpers;
using iTextSharp.text.pdf;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MMS.Controllers.Reports
{
    public class ReceivedStockController : Controller
    {
        MMSEntities objmms = new MMSEntities();
        MSP_Model objmsps = new MSP_Model();
        FactoryManager m = new FactoryManager();
        [Authorize]
        // GET: ReceivedStock
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;
            List<SelectListItem> ObjList = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Select Type", Value = "" },
                new SelectListItem { Text = "Opening", Value = "Opening" },
                new SelectListItem { Text = "Purchase", Value = "Purchase" },
            };

            ViewBag.TypeDiff = ObjList;

            return View();
        }
        public ActionResult GetAllDatas(string PID, string fromdate, string todate , string TypeSel, string RN)
        {
            List<GET_RECEIVED_STOCK> result = new List<GET_RECEIVED_STOCK>();
            result = objmsps.GetAllReceivedStockUpdated(PID,TypeSel,RN);

            if (fromdate != "" && todate != "")
            {
                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                // DateTime.ParseExact("05/07/2015", "MM/dd/yyyy", CultureInfo.InvariantCulture); Convert.ToDateTime(fromdate)
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var lst = result.Where(x => x.ReceiveDate >= date1 && x.ReceiveDate <= date2).OrderByDescending(x1 => x1.ReceiveDate).ToList();
                Session["ProId"] = PID;
                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = lst.Sum(k => k.ReceiveAmt);
                Session["fdate"] = fromdate;
                Session["tdate"] = todate;
                return PartialView("_PartialView_ReceiveStock", lst);
            }
            else if (PID != "" && fromdate == "" && todate == "")
            {
                Session["ProId"] = PID;
                Session["keepdata"] = result;
                ViewBag.displyTotalSum = result.Sum(k => k.ReceiveAmt);
                Session["fdate"] = fromdate;
                Session["tdate"] = todate;
                return PartialView("_PartialView_ReceiveStock", result);
            }
            else
            {
                return View("_EmptyView");
            }
        }

        public void GetExcel()
        {
            List<GET_RECEIVED_STOCK> allCust = (List<GET_RECEIVED_STOCK>)Session["keepdata"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                        grid1.Column("ReceiveDate", header: "Receipt Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.ReceiveDate)),
                       grid1.Column("GRNNo", header: "GRN No"),
                       grid1.Column("ChallanNo", header: "Challan No"),
                       grid1.Column("StatusTypeNo", header: "PO No."),
                       grid1.Column("Vendor", header: "Vendor"),
                       grid1.Column("GroupName", header: "Group"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("ReceiveQty", header: "Quantity", format: (item) => string.Format("{0:0.00}", item.ReceiveQty)),
                       grid1.Column("ReceiveAmt", header: "Amount", format: (item) => string.Format("{0:0.00}", item.ReceiveAmt))
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=DMRReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }

        public static String DMYToMDY(String input)
        {
            return Regex.Replace(input,
            @"\b(?<day>\d{1,2})/(?<month>\d{1,2})/(?<year>\d{2,4})\b",
            "${month}/${day}/${year}");
        }
        public FileStreamResult CurrentStockPdf()
        {
            List<GET_RECEIVED_STOCK> allCust = (List<GET_RECEIVED_STOCK>)Session["keepdata"];

            string pid = Session["ProId"].ToString();

            string fd, tdate, fd1, td2;
            fd1 = Session["fdate"].ToString();
            td2 = Session["tdate"].ToString();
            if (fd1 == "")
            {
                fd = "-";
            }
            else
            {
                fd = DMYToMDY(fd1);
            }
            if (td2 == "")
            {
                tdate = "-";
            }
            else
            {
                tdate = DMYToMDY(td2);
            }

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();

            var ss = from k in allCust
                     group k by k.ItemId into g
                     select new
                     { SUM = g.Sum(o => o.ReceiveAmt) };
            var totAmt = Math.Round((decimal)(ss.Sum(l => l.SUM)), 2);

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                       grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                       grid1.Column("ReceiveDate", header: "Receipt Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.ReceiveDate)),
                       grid1.Column("GRNNo", header: "GRN No"),
                       grid1.Column("ChallanNo", header: "Challan No"),
                       grid1.Column("StatusTypeNo", header: "PO No."),
                       grid1.Column("Vendor", header: "Vendor"),
                       grid1.Column("GroupName", header: "Group"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("ReceiveQty", header: "Quantity", format: (item) => string.Format("{0:0.00}", item.ReceiveQty),style: "right"),
                       grid1.Column("ReceiveAmt", header: "Amount", format: (item) => string.Format("{0:0.00}", item.ReceiveAmt), style: "right")
                       )
                    ).ToString();

           

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Daily Material Receipt";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>From Date: " + fd + " To Date: " + tdate + " </div><br/> ";

            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 50, 50, 30, 50);

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


            document.Add(PDateTime);
            document.Add(PHeader);
            document.Add(PSiteName);
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
            string totalTable = "<table align='right' style='float:right;border-top:0; width:80px;'><tr><td><strong>Total</strong></td><td class='right'>" + totAmt + "</td></tr></table>";
            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            styleSheet1.LoadTagStyle("table", "float", "right");
            styleSheet1.LoadTagStyle("table", "size", "16px");
            styleSheet1.LoadTagStyle("table", "width", "190");
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

        public JsonResult GetOpeningAndPurchaseValue(string Typetext, string ProjectId)
        {
            string J = string.Empty; 
            if (Typetext == "Opening")
            {
                var res = objmms.GateEntries.Where(x => x.Status == "Opening" && x.ProjectNo == ProjectId).ToList().Select(p => new { Text = p.GateEntryNo, Value = p.GateEntryNo }).ToList();
                J = res.ToJSON();
            }
            else {
                var res = objmms.GateEntries.Where(x => x.Status != "Opening" && x.ProjectNo == ProjectId).ToList().Select(p => new { Text = p.MRN_No_New, Value = p.MRN_No_New }).OrderByDescending(x=>x.Value).ToList();
                J = res.ToJSON();
            }

            return Json(J, JsonRequestBehavior.AllowGet);

        }
    }
}