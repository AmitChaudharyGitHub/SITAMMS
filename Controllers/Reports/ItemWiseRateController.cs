using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.Models;
using MMS.DAL;
using System.IO;
using iTextSharp.text;
using System.Web.Helpers;
using iTextSharp.text.pdf;
using System.Text;
using System.Text.RegularExpressions;

namespace MMS.Controllers.Reports
{
    public class ItemWiseRateController : Controller
    {
        MMSEntities objmms = new MMSEntities();
        Procedure procedure = new Procedure();
        // GET: ItemWiseRate
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;
            var ItemGroup = objmms.tblItemGroupMasters.OrderBy(a => a.GroupName).ToList();
            var t = ItemGroup.Select(x => new SelectListItem
            {
                Value = x.ItemGroupID.ToString(),
                Text = x.GroupName.ToString()
            });

            ViewBag.ItemGroups = t;



            return View();
        }


        public PartialViewResult GetAllData(string ProjectId,string ItemGroup,string Itemid,string FromDate,string ToDate)
        {
            var data = procedure.GetItemWiseRateHistory(ProjectId, ItemGroup, Itemid, FromDate, ToDate);
            TempData["searchData"]=data;
            TempData["fromDate"] = FromDate;
            TempData["toDate"] = ToDate;
            return PartialView("_GridView",data);
        }


        public ActionResult PrintPdf()
        {
           var data= TempData["searchData"]  as List<ItemWiseRateReport>;
            WebGrid grid1 = new WebGrid(source: data, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
           grid1.Column("", header: "S.No",format:item=>item.WebGrid.Rows.IndexOf(item)+1),
           grid1.Column("ProjectName", header: "Project Name", style: "text-align: center"),
           grid1.Column("ReceiveDate", header: "Receive Date", style: "text-align: center"),
           grid1.Column("VendorName", header: "Vendor Name", style: "addressStyle"),
           grid1.Column("PONo", header: "PO No", style: "text-align: center"),
           grid1.Column("PODate", header: "PO Date", style: "text-align: center"),
           grid1.Column("ItemNo", header: "Item No", style: "text-align: center;"),
           grid1.Column("itemName", header: "item Name", style: "text-align: center;"),
           grid1.Column("Rate", header: "Rate", style: "right")
                        )
                    ).ToString();


            string fDate = TempData["fromDate"].ToString();
            string tDate = TempData["fromDate"].ToString();
            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + data[0].ProjectName;
            string reportName = $"Item Wise Rate Report";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;


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
            TempData.Keep();
            return new FileStreamResult(output, "application/pdf");
        }

        public JsonResult GetItemByGroupStock(string Gid)
        {
            if (Gid != null)
            {
                var data=objmms.tblItemMasters.Where(x=>x.ItemGroupID==Gid).Select(x => new { Value = x.ItemID, Text = x.ItemName }).ToList();
                string jsonString = data.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public void GetExcel()
        {
            var data = TempData["searchData"] as List<ItemWiseRateReport>;

            WebGrid grid1 = new WebGrid(source: data, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                    grid1.Column("", header: "S.No", format: item => item.WebGrid.Rows.IndexOf(item) + 1),
                   grid1.Column("ProjectName", header: "Project Name", style: "text-align: center"),
           grid1.Column("ReceiveDate", header: "Receive Date", style: "text-align: center"),
           grid1.Column("VendorName", header: "Vendor Name", style: "addressStyle"),
           grid1.Column("PONo", header: "PO No", style: "text-align: center"),
           grid1.Column("PODate", header: "PO Date", style: "text-align: center"),
           grid1.Column("ItemNo", header: "Item No", style: "text-align: center;"),
           grid1.Column("itemName", header: "item Name", style: "text-align: center;"),
           grid1.Column("Rate", header: "Rate", style: "text-align: center;")
                        )
                    ).ToString();
            TempData.Keep();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=ItemWiseRateReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();

        }

    }
}