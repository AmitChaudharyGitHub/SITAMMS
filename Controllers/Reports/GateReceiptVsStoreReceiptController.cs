using MMS.DAL;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;

namespace MMS.Controllers.Reports
{
    public class GateReceiptVsStoreReceiptController : Controller
    {
        // GET: GateReceiptVsStoreReceipt
        private MMSEntities objmms = new MMSEntities(); string EmpId = string.Empty;
        Procedure objspro = new Procedure();
        public GateReceiptVsStoreReceiptController()
        {
            EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
        }

        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;
            return View();
        }

        public ActionResult GetGrid(DateTime FromDate, DateTime Todate,string ProjectId)
        {
            List<GateEntryReceiptVsStoreEntryReceipt> obj = null;
            obj = objspro.GetGateEntryVsStoreReceiptGrid(FromDate,Todate,ProjectId);
            Session["keepdata"] = obj;
            Session["fdate"] =FromDate.ToString("dd/MM/yyyy");
            Session["tdate"] = Todate.ToString("dd/MM/yyyy");
            Session["ProId"] = ProjectId;
            if (obj != null)
            {
                return PartialView("_GetGrid", obj);
            }
            else {
                return PartialView("_EmptyView");
            }
            
       
        }

        public static String DMYToMDY(String input)
        {
            return Regex.Replace(input,
            @"\b(?<day>\d{1,2})/(?<month>\d{1,2})/(?<year>\d{2,4})\b",
            "${month}/${day}/${year}");
        }

        public FileStreamResult ReceiptVsStoreReceiptPdf()
        {
            List<GateEntryReceiptVsStoreEntryReceipt> allCust = (List<GateEntryReceiptVsStoreEntryReceipt>)Session["keepdata"];

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

            

            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();

            

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                    grid1.Column("Vendor", header: "Vendor name"),
                    grid1.Column("StatusTypeNo", header: "Purchase order No"),
                     grid1.Column("ItemGroup", header: "Item Group"),
                     grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("Item", header: "Item Name"),
                       grid1.Column("Unit", header: "Unit Name"),
                       grid1.Column("GateEntryDate", header: "Gate Receipt Date"),
                       grid1.Column("GateEntryNo", header: "Gate Receipt No."),
                       grid1.Column("ReceiveQty", header: "Gate Receipt Qty", style: "right"),
                       grid1.Column("StoreReceiptDate", header: "Store Receipt Date"),
                       grid1.Column("StoreReceiptNo", header: "Store Receipt No"),
                       grid1.Column("StoreReceiptQty", header: "Store Receipt Qty",style: "right"),
                       grid1.Column("QtyDifference", header: "Qty Difference", style: "right")
                       )
                    ).ToString();


            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "RECEIPT REPORT";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>From Date: " + fd + " To Date: " + tdate + "<div>"; ;

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

            document.Add(img);
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
            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }


        public void GetExcel()
        {
            List<GateEntryReceiptVsStoreEntryReceipt> allCust = (List<GateEntryReceiptVsStoreEntryReceipt>)Session["keepdata"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                   grid1.Column("Vendor", header: "Vendor name"),
                   grid1.Column("StatusTypeNo", header: "Purchase order No"),
                     grid1.Column("ItemGroup", header: "Item Group"),
                     grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("Item", header: "Item Name"),
                       grid1.Column("Unit", header: "Unit Name"),
                       grid1.Column("GateEntryDate", header: "Gate Receipt Date"),
                       grid1.Column("GateEntryNo", header: "Gate Receipt No."),
                       grid1.Column("ReceiveQty", header: "Gate Receipt Qty"),
                       grid1.Column("StoreReceiptDate", header: "Store Receipt Date"),
                       grid1.Column("StoreReceiptNo", header: "Store Receipt No"),
                       grid1.Column("StoreReceiptQty", header: "Store Receipt Qty"),
                       grid1.Column("QtyDifference", header: "Qty Difference")
                       )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=GateReceiptVsStoreReceipt.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridHtml);
            Response.End();
        }


    }
}