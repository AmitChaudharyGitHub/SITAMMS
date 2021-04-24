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
    public class ReceiptVsPOController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        private Procedure objProc = new Procedure();
        // GET: ReceiptVsPO
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            string ProjectId = Session["ProId"].ToString();

            PurchaseOrder_ReceiprtReport rrm = new PurchaseOrder_ReceiprtReport();

            //code for PO

            var a11 = objmms.TbIndentPurchaseOrder_GST.Where(x211 => x211.ProjectNo == ProjectId).OrderBy(e11 => e11.PurchaseOrderNo).ToList();
            var t111 = a11.Distinct<TbIndentPurchaseOrder_GST>().Select(x111 => new SelectListItem
            {
                Value = x111.PurchaseOrderNo?.ToString().Trim(),
                Text = x111.PurchaseOrderNo?.ToString().Trim()
            });

            ViewBag.PONo = t111;

            return View();
        }


        public JsonResult GetPONUmberOnly(string ProjectId)
        {
            string J = string.Empty;
            var PONoList = objmms.TbIndentPurchaseOrder_GST.Where(x => x.ProjectNo == ProjectId).ToList().Select(p => new SelectListItem { Text = p.PurchaseOrderNo, Value = p.UId.ToString() }).OrderBy(k => k.Text).ToList();
            J = PONoList.ToJSON();
            return Json(J, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReceiptVsPO(string ProjectId, string ItemGroupId, string ItemId, string VendorId, string FromDate, string ToDate, string PONo)
        {
            //string ProjectId = Session["ProId"].ToString();


            FromDate = FromDate == "" ? null : FromDate;
            ToDate = ToDate == "" ? null : ToDate;

            PONo = PONo == "Select PO Number" ? "" : PONo;

            var data_rrm = objProc.GetReceiptVsPOReport(ProjectId, ItemGroupId, ItemId, VendorId, PONo, FromDate, ToDate);



            ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
            ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
            TempData["printexcel"] = data_rrm;
            TempData["ProjectId"] = ProjectId;
            TempData.Keep();

            return PartialView("ReceiprtVsPOReport", data_rrm);
        }


        public FileStreamResult GETPdf()
        {
            List<PurchaseOrder_ReceiprtReport> allCust = (List<PurchaseOrder_ReceiprtReport>)TempData["printexcel"];

            string pid = TempData["ProjectId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                          grid1.Column("CreatedDate", header: "PO Date"),
                       grid1.Column("GateEntryNo", header: "PO No. "),
                       grid1.Column("Vendor", header: "Name of Vendor"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("Item", header: "Item Name"),
                        grid1.Column("Unit", header: "Unit"),
                       grid1.Column("Rate", header: "Rate"),
                       grid1.Column("Qty", header: "Order Qty", style: "right"),
                       grid1.Column("ReceiveQty", header: "Quantity", style: "right"),
                       grid1.Column("BalQty", header: "Bal. Qty", style: "right"),

                       grid1.Column("MRNNo", header: "MRN No"),
                       grid1.Column("MRNDate", header: "MRN Date"),
                       grid1.Column("MRNQty", header: "MRN Qty")
                        )
                    ).ToString();

            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Purchase Order Vs. Receipt Report";
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

            document.Add(img);
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

            TempData.Keep();
            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }


        public void GetExcel()
        {
            List<PurchaseOrder_ReceiprtReport> allCust = (List<PurchaseOrder_ReceiprtReport>)TempData["printexcel"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                          grid1.Column("CreatedDate", header: "PO Date"),
                       grid1.Column("GateEntryNo", header: "PO No. "),
                       grid1.Column("Vendor", header: "Name of Vendor"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("Item", header: "Item Name"),
                       grid1.Column("Rate", header: "Rate"),
                       grid1.Column("Qty", header: "Order Qty"),
                       grid1.Column("ReceiveQty", header: "Quantity"),
                       grid1.Column("BalQty", header: "Bal. Qty"),

                        grid1.Column("MRNNo", header: "MRN No"),
                       grid1.Column("MRNDate", header: "MRN Date"),
                       grid1.Column("MRNQty", header: "MRN Qty")
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=POvsReceiptReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }



    }
}