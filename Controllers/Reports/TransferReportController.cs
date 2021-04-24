using iTextSharp.text;
using iTextSharp.text.pdf;
using MMS.DAL;
using MMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MMS.Controllers.Reports
{
    public class TransferReportController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        private Procedure procedure = new Procedure();
        // GET: TransferReport
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            ViewBag.Projects = new SelectList(objmms.tblProjectMasters.Where(x => !x.ProjectName.Contains("Admin") && !x.ProjectName.Contains("DEMO")).OrderBy(x => x.ProjectName), "PRJID", "ProjectName");
            var transferTypes=new List<tblTransfertypeselection>();
            transferTypes.AddRange(objmms.tblTransfertypeselections);
            ViewBag.TransferTypes = new SelectList(transferTypes, "Id", "TransferType");
            return View();
        }

        public ActionResult Receipt()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            ViewBag.Projects = new SelectList(objmms.tblProjectMasters.Where(x => !x.ProjectName.Contains("Admin") && !x.ProjectName.Contains("DEMO")).OrderBy(x => x.ProjectName), "PRJID", "ProjectName");
            var transferTypes = new List<tblTransfertypeselection>();
            transferTypes.AddRange(objmms.tblTransfertypeselections);
            transferTypes.Add(new tblTransfertypeselection {Id=3,TransferType="Manual Transfer" });
            ViewBag.TransferTypes = new SelectList(transferTypes, "Id", "TransferType");
            return View();
        }

        public PartialViewResult SearchData(string ProjectId, string TransferProjectId, string ItemGroupId, string ItemId, string TransferType, string FromDate, string ToDate, int page = 1)
        {

            TempData["PId"] = ProjectId;
            TempData["TransferProjectId"] = TransferProjectId == "" ? null : TransferProjectId;
            TempData["ItemGroupId"] = ItemGroupId == "" ? null : ItemGroupId;
            TempData["ItemId"] = ItemId == "" ? null : ItemId;
            TempData["TransferType"] = TransferType == "" ? null : TransferType;
            TempData["FromDate"] = FromDate == "" ? null : FromDate;
            TempData["ToDate"] = ToDate == "" ? null : ToDate;
           
            var data = procedure.GetTransferData(ProjectId, TransferProjectId, ItemGroupId, ItemId, TransferType,FromDate,ToDate);
            decimal total = 0;
            if(data!=null && data.Count > 0)
            {
                
                foreach (var item in data)
                {
                    total = total + item.Amount;
                }
            }
            ViewBag.TotalAmount = total;
            TempData["TransferSearchData"] = data;
            return PartialView("_WebGrid", data);
        }


        public PartialViewResult SearchTransferReceiptData(string ProjectId, string TransferProjectId, string ItemGroupId, string ItemId, string TransferType, string FromDate, string ToDate, int page = 1)
        {

            TempData["PId"] = ProjectId;
            TempData["TransferProjectId"] = TransferProjectId == "" ? null : TransferProjectId;
            TempData["ItemGroupId"] = ItemGroupId == "" ? null : ItemGroupId;
            TempData["ItemId"] = ItemId == "" ? null : ItemId;
            TempData["TransferType"] = TransferType == "" ? null : TransferType;
            TempData["FromDate"] = FromDate == "" ? null : FromDate;
            TempData["ToDate"] = ToDate == "" ? null : ToDate;

            var data = procedure.GetTransferDataForReceipt(ProjectId, TransferProjectId, ItemGroupId, ItemId, TransferType,FromDate,ToDate);


            decimal total = 0;
            decimal qtyTotal = 0;
            if (data != null && data.Count > 0)
            {

                foreach (var item in data)
                {
                    total = total + item.Amount;
                    qtyTotal += item.ReceiveQty;
                }
            }


            ViewBag.TotalAmount = total;

            ViewBag.IsItemSelected = ItemId == "" ? false : true;
            ViewBag.TotalQty = qtyTotal;
            TempData["TransferReceiptSearchData"] = data;
            return PartialView("_WebGridTransferReceipt", data);
        }


        //public PartialViewResult GetData(int?page)
        //{
        //    string tProjectId = "";
        //    string grpId = "";
        //    string itemId = "";
        //    string tType = "";
        //    int pageSize = 15;
        //    int pageIndex = 1;
        //    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

        //    if (TempData["TransferProjectId"] != null)
        //        tProjectId = TempData["TransferProjectId"].ToString();

        //    if (TempData["ItemGroupId"] != null)
        //        grpId = TempData["ItemGroupId"].ToString();


        //    if (TempData["ItemId"] != null)
        //        itemId = TempData["ItemId"].ToString();

        //    if (TempData["TransferType"] != null)
        //        tType = TempData["TransferType"].ToString();


        //    var data = procedure.GetTransferData(TempData["PId"].ToString(),tProjectId,grpId,itemId,tType);
        //    TempData.Keep();
        //    return PartialView("_WebGrid", data);
        //}


        public JsonResult GetTransferProjects(string TransferType,string ProjectId)
        {
            List<GetTransferProjects> transferProjects = null;
            if (TransferType == "1")
            {
                transferProjects = procedure.GetInterStateTransferProjects(ProjectId);
            }
            else if (TransferType == "2")
            {
                transferProjects = procedure.GetIntraStateTransferProjects(ProjectId);
            }
            return Json(transferProjects.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferProjectsForReceipt(string TransferType,string ProjectId)
        {
            List<GetTransferProjects> transferProjects = null;

            transferProjects = procedure.GetTransferProjectsForReceipt(ProjectId,TransferType);

            return Json(transferProjects.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferProjectsItemGroups(string TransferProjectId)
        {
            var itemGroups = procedure.GetItemGroupsForTransfer(TransferProjectId);
            return Json(itemGroups.ToJSON(), JsonRequestBehavior.AllowGet);
        }


        public FileStreamResult TransferReceiptPdf()
        {

            List<GetTransferDataForReceiptReport> searchData = (List<GetTransferDataForReceiptReport>)TempData["TransferReceiptSearchData"];

            WebGrid grid1 = new WebGrid(source: searchData, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                            grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                          grid1.Column("TransferNumber", header: "Transfer Number"),
                       grid1.Column("TransferCreatedDate", header: "Transfer Created Date", format: (item) => string.Format("{0:dd MMM yyyy}", item.TransferCreatedDate)),
                       grid1.Column("GateEntryNo", header: "Gate Entry No"),
                       grid1.Column("GateEntryDate", header: "Gate Entry Date", format: (item) => string.Format("{0:dd MMM yyyy}", item.GateEntryDate)),
                       grid1.Column("TransferFrom", header: "Transfer From"),
                       grid1.Column("ItemGroup", header: "Item Group"),
                       grid1.Column("ItemCode", header: "Item Code"),
                      grid1.Column("Item", header: "Item Name"),
                         grid1.Column("Unit", header: "Unit"),
                       grid1.Column("ReceiveQty", header: "Received Qty",style:"right"),
                          grid1.Column("Rate", header: "Rate"),
                        grid1.Column("Amount", header: "Amount",style:"right")
                        )
                    ).ToString();



            string pid = TempData["PId"].ToString();
            string siteName = "Site-" + objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == pid).ProjectName;
            string ItemGroup = "--";
            string ItemName = "--";
            string fDate = "--";
            string tDate = "--";

            if (TempData["ItemGroupId"] != null)
            {
                string ItemGroupId = TempData["ItemGroupId"].ToString();
                ItemGroup = objmms.tblItemGroupMasters.SingleOrDefault(x => x.ItemGroupID == ItemGroupId).GroupName;
            }


            if (TempData["ItemId"] != null)
            {
                string ItemId = TempData["ItemId"].ToString();
                ItemName = objmms.tblItemMasters.SingleOrDefault(x => x.ItemID == ItemId).ItemName;
            }
            if (TempData["FromDate"] != null)
            {
                fDate = TempData["FromDate"].ToString();
            }

            if (TempData["ToDate"] != null)
            {
                tDate = TempData["ToDate"].ToString();
            }

            decimal total = 0;

            if (searchData != null && searchData.Count > 0)
            {

                foreach (var item in searchData)
                {
                    total = total + item.Amount;
                }
            }

            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;


            var totalAmt = String.Format("{0:0.00}", total);

            System.Text.StringBuilder DataString = new System.Text.StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string reportName = "Transfer Receipt Report";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;

            string additionalData = "<div style='text-align:left'>From Date : " + fDate + " To Date : " + tDate + " </div><div style='text-align:left'>Item Group Name : " + ItemGroup + " ,   Item Name : " + ItemName + "</div><br/>";

            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 50, 50, 30, 50);

            var writer = PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;
            //writer.PageEvent = new HeaderFooter(header, siteName, reportName, "ACIL");
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
            string totalTable = "<table style='border-top:0; width:100px;'><tr><td colspan='9'><td align='right'><strong>Total</strong></td><td align='right'>" + totalAmt + "</td></tr></table>";
            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            styleSheet1.LoadTagStyle("table", "float", "right");
            styleSheet1.LoadTagStyle("table", "size", "16px");
            styleSheet1.LoadStyle("right", "align", "right");
            List<IElement> element = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr1, styleSheet1);

            foreach (IElement el in element)
            {
                document.Add(el);
            }

            document.Close();
            output.Position = 0;
            TempData.Keep();
            return new FileStreamResult(output, "application/pdf");
        }

        public void TransferReceiptExcel()
        {
            List<GetTransferDataForReceiptReport> searchData = (List<GetTransferDataForReceiptReport>)TempData["TransferReceiptSearchData"];

            WebGrid grid1 = new WebGrid(source: searchData, canPage: false, canSort: false);

           string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                            grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                          grid1.Column("TransferNumber", header: "Transfer Number"),
                       grid1.Column("TransferCreatedDate", header: "Transfer Created Date", format: (item) => string.Format("{0:dd MMM yyyy}", item.TransferCreatedDate)),
                       grid1.Column("GateEntryNo", header: "Gate Entry No"),
                       grid1.Column("GateEntryDate", header: "Gate Entry Date", format: (item) => string.Format("{0:dd MMM yyyy}", item.GateEntryDate)),
                       grid1.Column("TransferFrom", header: "Transfer From"),
                       grid1.Column("ItemGroup", header: "Item Group"),
                       grid1.Column("ItemCode", header: "Item Code"),
                      grid1.Column("Item", header: "Item Name"),
                       grid1.Column("Unit", header: "Unit"),
                       grid1.Column("ReceiveQty", header: "Received Qty",style:"right"),
                        grid1.Column("Rate", header: "Rate"),
                        grid1.Column("Amount", header: "Amount",style:"right")
                        )
                    ).ToString();
            TempData.Keep();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=TransferReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridHtml);
            Response.End();
        }


        public FileStreamResult GETPdf()
        {

            List<GetTransferDataForReport> searchData = (List<GetTransferDataForReport>)TempData["TransferSearchData"];

            WebGrid grid1 = new WebGrid(source: searchData, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                            grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                          grid1.Column("TransferNumber", header: "Transfer Number"),
                       grid1.Column("TransferDate", header: "Transfer Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.TransferDate)),
                       grid1.Column("TransferCreatedDate", header: "Transfer Created Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.TransferCreatedDate)),
                       grid1.Column("GetOutNumber", header: "Gate Out Number"),
                       grid1.Column("GetOutDate", header: "Gate Out Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.GetOutDate)),
                       grid1.Column("GateOutCratedDate", header: "Gate Out Crated Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.GateOutCratedDate)),
                       grid1.Column("ReachToSite", header: "Reach To Site", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.ReachToSite)),
                      grid1.Column("TransferTo", header: "Transfer To"),
                       grid1.Column("ItemGroup", header: "Item Group"),
                        grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("Unit", header: "Unit"),
                       grid1.Column("TransferQty", header: "Transfer Qty",style:"right"),                       
                       grid1.Column("GetOutQty", header: "Gate Out Qty",style:"right"),
                       grid1.Column("TransferRate", header: "Rate"),
                       grid1.Column("Amount", header: "Amount",style:"right")
                        )
                    ).ToString();

       

            string pid = TempData["PId"].ToString();
            string siteName ="Site-"+objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == pid).ProjectName;
            string ItemGroup = "--";
            string ItemName = "--";
            string fDate = "--";
            string tDate = "--";

            if (TempData["ItemGroupId"] != null)
            {
                string ItemGroupId = TempData["ItemGroupId"].ToString();
                ItemGroup = objmms.tblItemGroupMasters.SingleOrDefault(x => x.ItemGroupID == ItemGroupId).GroupName;
            }


            if (TempData["ItemId"] != null)
            {
                string ItemId = TempData["ItemId"].ToString();
                ItemName = objmms.tblItemMasters.SingleOrDefault(x => x.ItemID == ItemId).ItemName;
            }
            if (TempData["FromDate"] != null)
            {
                fDate = TempData["FromDate"].ToString();
            }

            if (TempData["ToDate"] != null)
            {
                tDate = TempData["ToDate"].ToString();
            }

            decimal total = 0;

            if (searchData != null && searchData.Count > 0)
            {

                foreach (var item in searchData)
                {
                    total = total + item.Amount;
                }
            }

            var totalAmt = String.Format("{0:0.00}", total);

            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            System.Text.StringBuilder DataString = new System.Text.StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string reportName = "Transfer Report";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;

            string additionalData = "<div style='text-align:left'>From Date : " + fDate + " To Date : " + tDate + " </div><div style='text-align:left'>Item Group Name : " + ItemGroup + " ,   Item Name : " + ItemName + "</div><br/>";

            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 50, 50, 30, 50);

            var writer = PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;
            //writer.PageEvent = new HeaderFooter(header, siteName, reportName, "ACIL");
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
            string totalTable = "<table align='right' style='float:right;border-top:0; width:80px;'><tr><td><strong>Total</strong></td><td class='right'>" + totalAmt + "</td></tr></table>";
            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            styleSheet1.LoadTagStyle("table", "float", "right");
            styleSheet1.LoadTagStyle("table", "size", "16px");
            styleSheet1.LoadTagStyle("table", "width", "105");
            styleSheet1.LoadStyle("right", "align", "right");
            List<IElement> element = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr1, styleSheet1);

            foreach (IElement el in element)
            {
                document.Add(el);
            }

            document.Close();
            output.Position = 0;
            TempData.Keep();
            return new FileStreamResult(output, "application/pdf");
        }

        public void GetExcel()
        {
            List<GetTransferDataForReport> searchData = (List<GetTransferDataForReport>)TempData["TransferSearchData"];

            WebGrid grid1 = new WebGrid(source: searchData, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                       grid1.Column("TransferNumber", header: "Transfer Number"),
                       grid1.Column("TransferDate", header: "Transfer Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.TransferDate)),
                       grid1.Column("TransferCreatedDate", header: "Transfer Created Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.TransferCreatedDate)),
                       grid1.Column("GetOutNumber", header: "Gate Out Number"),
                       grid1.Column("GetOutDate", header: "Gate Out Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.GetOutDate)),
                       grid1.Column("GateOutCratedDate", header: "Gate Out Crated Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.GateOutCratedDate)),
                       grid1.Column("ReachToSite", header: "Reach To Site", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.ReachToSite)),
                      grid1.Column("TransferTo", header: "Transfer To"),
                       grid1.Column("ItemGroup", header: "Item Group"),
                        grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("TransferQty", header: "Transfer Qty"),                       
                       grid1.Column("GetOutQty", header: "Gate Out Qty"),
                       grid1.Column("TransferRate", header: "Rate"),
                       grid1.Column("Amount", header: "Amount")
                        )
                    ).ToString();
            TempData.Keep();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=TransferReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }


    }
}