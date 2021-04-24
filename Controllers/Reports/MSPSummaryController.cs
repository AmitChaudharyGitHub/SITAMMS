using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MMS.DAL;
using MMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MMS.Controllers.Reports
{
    public class MSPSummaryController : Controller
    {
        MMSEntities objmms = new MMSEntities();
        MSP_Model objmsps = new MSP_Model();
        Procedure procedure = new Procedure();
        // GET: MSPSummary
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;

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

        public PartialViewResult GetMSPSummary(int m, int y, string PID)
        {
            Session["m"] = m;
            Session["y"] = y;
            Session["PId"] = PID;
            var data=procedure.GetMSPSummary(m,y,PID);
            TempData["searchData"] = data;
            return PartialView("_MSPSummary",data);
        }


        public ActionResult ExportMSPSummary(int m, int y, string PID)
        {
            var data = objmsps.PrintMSPSummary(PID,m,y);
            var workbook = new XLWorkbook();
            workbook.AddWorksheet(data, "MSPSummary");

            string filename = Guid.NewGuid().ToString();

            workbook.SaveAs(System.IO.Path.GetTempPath() + filename + ".xlsx");
            return File(System.IO.Path.GetTempPath() + filename + ".xlsx", "application/vnd.ms-excel", "MSPSummary.xlsx");
        }


        public void GetExcel()
        {
            List<MSPSummary> data = (List<MSPSummary>)TempData["searchData"];

            WebGrid grid1 = new WebGrid(source: data, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
               columns: grid1.Columns(
                       grid1.Column("Id", header: "SNo."),
                       grid1.Column("ItemGroupName", header: "Item Group Name", canSort: false),
                       grid1.Column("OpeningBalance", header: "Opening Balance", canSort: false, style: "right"),
                       grid1.Column("ReceivedUptoLastMonth", header: "Received Upto LastMonth", canSort: false, style: "right"),
                        grid1.Column("ReceivedDuringCurrentMonth", header: "Received During Current Month", style: "right", canSort: false),
                         grid1.Column("ReceivedTotalUptoDate", header: "Received Total UptoDate", style: "right", canSort: false),
                       grid1.Column("IssuedUptoLastMonth", header: "Issued Upto Last Month", canSort: false, style: "right"),
                       grid1.Column("IssuedDuringCurrentMonth", header: "IssuedUptoLastMonth", style: "right"),
                       grid1.Column("TotalIssuedUptoDate", header: "Total Issued Upto Date ", style: "right"),
                         grid1.Column("ClosingBalance", header: "Closing Balance", canSort: false, style: "right"),
                          grid1.Column("Remarks", header: "Remarks", canSort: false)

                       )
                   ).ToString();

            TempData.Keep();
            Response.ClearContent();
            TempData.Keep();
            Response.AddHeader("content-disposition", "attachment; filename=MSPSummaryReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }

        public FileStreamResult GETPdf()
        {
            string mn = Session["m"].ToString();
            string yr = Session["y"].ToString();
           
            int month = Convert.ToInt16(mn);
            var monthAbbr = System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(month);

            string pid = Session["PId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();



            List<MSPSummary> data = (List<MSPSummary>)TempData["searchData"];

            WebGrid grid1 = new WebGrid(source: data, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                        grid1.Column("Id", header: "SNo."),
                        grid1.Column("ItemGroupName", header: "Item Group Name", canSort: false),
                        grid1.Column("OpeningBalance", header: "Opening Balance", canSort: false, style: "right"),
                        grid1.Column("ReceivedUptoLastMonth", header: "Received Upto LastMonth", canSort: false, style: "right"),
                         grid1.Column("ReceivedDuringCurrentMonth", header: "Received During Current Month", style: "right", canSort: false),
                          grid1.Column("ReceivedTotalUptoDate", header: "Received Total UptoDate", style: "right", canSort: false),
                        grid1.Column("IssuedUptoLastMonth", header: "Issued Upto Last Month", canSort: false, style: "right"),
                        grid1.Column("IssuedDuringCurrentMonth", header: "IssuedUptoLastMonth", style: "right"),
                        grid1.Column("TotalIssuedUptoDate", header: "Total Issued Upto Date ", style: "right"),
                          grid1.Column("ClosingBalance", header: "Closing Balance", canSort: false, style: "right"),
                           grid1.Column("Remarks", header: "Remarks", canSort: false)

                        )
                    ).ToString();

            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "MSP Summary Report";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>Monthly Stock Position For The Month Of: " + monthAbbr + "  - " + yr + " </div><br/>";

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
                styleSheet.LoadStyle("small", "width", "8");
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
            //string totalTable = "<table width='100%' align='right' class='fr' style='border-top:0;'><tr><td class='c10' colspan='12' align='right'><strong>Total</strong></td><td class='right c9'>" + totOpeningBLAmt + "</td><td class='right c9'>" + totRcvdAmtUptoLastMonth + "</td><td class='right c9'>" + totRcvdAmtDuringCurrntMonth + "</td><td class='right c9'>" + totRcvdAmtTotalUpto + "</td><td class='right c9'>" + totIssueAmtUptoLastMonth + "</td><td class='right c10'>" + totIssueAmtDurrCurrntMonth + "</td><td class='right c10'>" + totIssAmtTotalUpto + "</td><td class='right c10'>" + totClosingBLAmt + "</td></tr></table>";
            //StringReader sr1 = new StringReader(totalTable);
            //var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            //styleSheet1.LoadTagStyle("table", "border", "1");
            ////styleSheet1.LoadTagStyle("table", "float", "right");
            //styleSheet1.LoadTagStyle("table", "size", "16px");
            //styleSheet1.LoadTagStyle("table", "width", "70%");
            //styleSheet1.LoadStyle("right", "align", "right");
            //List<IElement> element = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr1, styleSheet1);

            //foreach (IElement el in element)
            //{
            //    document.Add(el);
            //}

            TempData.Keep();
            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

    }
}