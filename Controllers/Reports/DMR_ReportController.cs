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
using System.Linq.Dynamic;
using System.IO;
using iTextSharp.text;
using System.Web.Helpers;
using iTextSharp.text.pdf;
using System.Text;

namespace MMS.Controllers.Reports
{
    public class DMR_ReportController : Controller
    {
        // GET: DMR_Report
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        string ProjectId;
        [Authorize]
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            string ProjectId = Session["ProId"].ToString();
            DMR_Report_Model rrm = new DMR_Report_Model();
            //code for bind vendor name accoding project
            var a = objmms.GateEntryChilds.Where(b => b.ProjectNo == ProjectId).OrderBy(c => c.Vendor).ToList();
            var t = a.Distinct<GateEntryChild>().Select(x => new SelectListItem
            {
                Value = x.VendorNo?.ToString().Trim(),
                Text = x.Vendor?.ToString().Trim()
            });
            ViewBag.Vendor_Name = t;

            //code for bind PO No accoding project
            var a1 = objmms.GateEntries.Where(b1 => b1.ProjectNo == ProjectId).OrderBy(c1 => c1.StatusTypeNo).ToList();
            var t1 = a1.Distinct<GateEntry>().Select(x1 => new SelectListItem
            {
                Value = x1.StatusTypeNo?.ToString().Trim(),
                Text = x1.StatusTypeNo?.ToString().Trim()
            });
            ViewBag.PONumber = t1;

            //Bind top 200 table in grid on page load
            var data_from_table = (from gatenrty in objmms.GateEntries
                                   join gatechld in objmms.GateEntryChilds on gatenrty.UId equals gatechld.MUId 
                                   join getpo in objmms.TbIndentPurchaseOrderMasters on gatenrty.StatusTypeNo equals getpo.PurchaseOrderNo
                                   where gatechld.ProjectNo == ProjectId
                                   orderby gatenrty.Date
                                   select new DMR_Report_Model
                                   {                                       
                                       GateEntryNo = gatechld.GateEntryNo,
                                       Vendor = gatechld.Vendor,
                                       BillNo = "",
                                       StatusTypeNo = gatechld.StatusTypeNo,
                                       ItemGroup = gatechld.ItemGroup,
                                       Item = gatechld.Item,
                                       ReceiveQty = gatechld.ReceiveQty,
                                       Unit = gatechld.Unit,
                                       ChallanNo = gatenrty.ChallanNo ?? "",
                                       CreatedDate= gatechld.CreatedDate,
                                       Date = gatenrty.Date,
                                       PODate = getpo.PurchaseOrderDate,
                                       Rate = gatechld.Rate,
                                       Amount = Math.Round((decimal)(gatechld.ReceiveQty * gatechld.Rate), 2)
                                   }).OrderByDescending(k => k.Date).Take(200).ToList();

            ViewBag.displyTotalSum = data_from_table.Sum(k => k.Amount);
            rrm.Dmr_Reports = data_from_table;
            TempData["printexcel"] = data_from_table;
            TempData.Keep("printexcel");
            Session["keepdata"] = data_from_table;
            return View("Index", rrm);
        }
        public ActionResult Get_All_DMR_Report(string FromDate, string ToDate, string Vendors_Name, string PONumber)
        {
            string ProjectId = Session["ProId"].ToString();
        

            List<DMR_Report_Model> rrm = null;
            var data_rrm = rrm;
            if (Vendors_Name == "" && Vendors_Name == null && PONumber == "" && PONumber == null && FromDate == "" && ToDate == "" && FromDate == null && ToDate == null)
            {
                data_rrm = (from gatenrty in objmms.GateEntries
                            join gatechld in objmms.GateEntryChilds on gatenrty.UId equals gatechld.MUId
                            join getpo in objmms.TbIndentPurchaseOrderMasters on gatenrty.StatusTypeNo equals getpo.PurchaseOrderNo
                            where gatechld.ProjectNo == ProjectId
                            orderby gatenrty.Date
                            select new DMR_Report_Model
                            {
                                GateEntryNo = gatechld.GateEntryNo,
                                Vendor = gatechld.Vendor,
                                BillNo = "",
                                StatusTypeNo = gatechld.StatusTypeNo,
                                ItemGroup = gatechld.ItemGroup,
                                Item = gatechld.Item,
                                ReceiveQty = gatechld.ReceiveQty,
                                Unit = gatechld.Unit,
                                ChallanNo = gatenrty.ChallanNo ?? "",
                                CreatedDate = gatechld.CreatedDate,
                                Date = gatenrty.Date,
                                PODate = getpo.PurchaseOrderDate,
                                Rate = gatechld.Rate,
                                Amount = Math.Round((decimal)(gatechld.ReceiveQty * gatechld.Rate), 2)
                            }).OrderByDescending(k => k.Date).Take(200).ToList();
                ViewBag.displyTotalSum = Math.Round((decimal)(data_rrm.Sum(k => k.Amount)), 2);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
                return PartialView("_Dmr_Report_Partial", data_rrm);
            }
            else if (FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                if (Vendors_Name.Trim() == null && Vendors_Name == "")
                {
                    DateTime date1 = Convert.ToDateTime(FromDate);
                    DateTime date2 = Convert.ToDateTime(ToDate);
                    Session["FromDate"] = DateTime.Parse(FromDate).ToString("dd-MMM-yyyy");
                    Session["ToDate"] = DateTime.Parse(ToDate).ToString("dd-MMM-yyyy");

                    data_rrm = (from gatenrty in objmms.GateEntries
                                join gatechld in objmms.GateEntryChilds on gatenrty.UId equals gatechld.MUId
                                join getpo in objmms.TbIndentPurchaseOrderMasters on gatenrty.StatusTypeNo equals getpo.PurchaseOrderNo
                                where gatechld.ProjectNo == ProjectId && gatechld.Date >= date1 && gatechld.Date <= date2
                                orderby gatenrty.Date
                                select new DMR_Report_Model
                                {
                                    GateEntryNo = gatechld.GateEntryNo,
                                    Vendor = gatechld.Vendor,
                                    BillNo = "",
                                    StatusTypeNo = gatechld.StatusTypeNo,
                                    ItemGroup = gatechld.ItemGroup,
                                    Item = gatechld.Item,
                                    ReceiveQty = gatechld.ReceiveQty,
                                    Unit = gatechld.Unit,
                                    ChallanNo = gatenrty.ChallanNo ?? "",
                                    CreatedDate = gatechld.CreatedDate,
                                    Date = gatenrty.Date,
                                    PODate = getpo.PurchaseOrderDate,
                                    Rate = gatechld.Rate,
                                    Amount = Math.Round((decimal)(gatechld.ReceiveQty * gatechld.Rate), 2)
                                }).OrderByDescending(k => k.Date).ToList();
                    ViewBag.displyTotalSum = Math.Round((decimal)(data_rrm.Sum(k => k.Amount)), 2);
                    TempData["printexcel"] = data_rrm;
                    Session["keepdata"] = data_rrm;
                    TempData.Keep("printexcel");
                    return PartialView("_Dmr_Report_Partial", data_rrm);
                }
                else
                {
                    if (Vendors_Name.Trim() != null && Vendors_Name != "")
                    {
                        DateTime date1 = Convert.ToDateTime(FromDate);
                        DateTime date2 = Convert.ToDateTime(ToDate);
                        Session["FromDate"] = DateTime.Parse(FromDate).ToString("dd-MMM-yyyy");
                        Session["ToDate"] = DateTime.Parse(ToDate).ToString("dd-MMM-yyyy");
                        data_rrm = (from gatenrty in objmms.GateEntries
                                    join gatechld in objmms.GateEntryChilds on gatenrty.UId equals gatechld.MUId
                                    join getpo in objmms.TbIndentPurchaseOrderMasters on gatenrty.StatusTypeNo equals getpo.PurchaseOrderNo
                                    where gatechld.VendorNo == Vendors_Name && gatechld.ProjectNo == ProjectId && gatechld.Date >= date1 && gatechld.Date <= date2
                                    orderby gatenrty.Date
                                    select new DMR_Report_Model
                                    {
                                        GateEntryNo = gatechld.GateEntryNo,
                                        Vendor = gatechld.Vendor,
                                        BillNo = "",
                                        StatusTypeNo = gatechld.StatusTypeNo,
                                        ItemGroup = gatechld.ItemGroup,
                                        Item = gatechld.Item,
                                        ReceiveQty = gatechld.ReceiveQty,
                                        Unit = gatechld.Unit,
                                        ChallanNo = gatenrty.ChallanNo ?? "",
                                        CreatedDate = gatechld.CreatedDate,
                                        Date = gatenrty.Date,
                                        PODate = getpo.PurchaseOrderDate,
                                        Rate = gatechld.Rate,
                                        Amount = Math.Round((decimal)(gatechld.ReceiveQty * gatechld.Rate), 2)
                                    }).OrderByDescending(k => k.Date).ToList();
                        ViewBag.displyTotalSum = Math.Round((decimal)(data_rrm.Sum(k => k.Amount)), 2);
                        TempData["printexcel"] = data_rrm;
                        Session["keepdata"] = data_rrm;
                        TempData.Keep("printexcel");
                        return PartialView("_Dmr_Report_Partial", data_rrm);
                    }
                    else
                    {
                        DateTime date1 = Convert.ToDateTime(FromDate);
                        DateTime date2 = Convert.ToDateTime(ToDate);
                        Session["FromDate"] = DateTime.Parse(FromDate).ToString("dd-MMM-yyyy");
                        Session["ToDate"] = DateTime.Parse(ToDate).ToString("dd-MMM-yyyy");

                        data_rrm = (from gatenrty in objmms.GateEntries
                                    join gatechld in objmms.GateEntryChilds on gatenrty.UId equals gatechld.MUId
                                    join getpo in objmms.TbIndentPurchaseOrderMasters on gatenrty.StatusTypeNo equals getpo.PurchaseOrderNo
                                    where gatechld.ProjectNo == ProjectId && gatechld.Date >= date1 && gatechld.Date <= date2
                                    orderby gatenrty.Date
                                    select new DMR_Report_Model
                                    {
                                        GateEntryNo = gatechld.GateEntryNo,
                                        Vendor = gatechld.Vendor,
                                        BillNo = "",
                                        StatusTypeNo = gatechld.StatusTypeNo,
                                        ItemGroup = gatechld.ItemGroup,
                                        Item = gatechld.Item,
                                        ReceiveQty = gatechld.ReceiveQty,
                                        Unit = gatechld.Unit,
                                        ChallanNo = gatenrty.ChallanNo ?? "",
                                        CreatedDate = gatechld.CreatedDate,
                                        Date = gatenrty.Date,
                                        PODate = getpo.PurchaseOrderDate,
                                        Rate = gatechld.Rate,
                                        Amount = Math.Round((decimal)(gatechld.ReceiveQty * gatechld.Rate), 2)
                                    }).OrderByDescending(k => k.Date).ToList();
                        ViewBag.displyTotalSum = Math.Round((decimal)(data_rrm.Sum(k => k.Amount)), 2) ;
                        TempData["printexcel"] = data_rrm;
                        Session["keepdata"] = data_rrm;
                        TempData.Keep("printexcel");
                        return PartialView("_Dmr_Report_Partial", data_rrm);
                    }
                }
            }
            //else if (Vendors_Name.Trim() != null && Vendors_Name != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            //{
            //    DateTime date1 = Convert.ToDateTime(FromDate);
            //    DateTime date2 = Convert.ToDateTime(ToDate);

            //    data_rrm = (from gatenrty in objmms.GateEntries
            //                join gatechld in objmms.GateEntryChilds on gatenrty.UId equals gatechld.MUId
            //                where gatechld.VendorNo == Vendors_Name && gatechld.ProjectNo == ProjectId && gatechld.CreatedDate >= date1 && gatechld.CreatedDate < date2
            //                select new DMR_Report_Model
            //                {
            //                    GateEntryNo = gatechld.GateEntryNo,
            //                    Vendor = gatechld.Vendor,
            //                    BillNo = "",
            //                    StatusTypeNo = gatechld.StatusTypeNo,
            //                    ItemGroup = gatechld.ItemGroup,
            //                    Item = gatechld.Item,
            //                    ReceiveQty = gatechld.ReceiveQty,
            //                    Unit = gatechld.Unit,
            //                    ChallanNo = gatenrty.ChallanNo ?? "",
            //                    CreatedDate = gatechld.CreatedDate
            //                }).ToList();
            //}
            else if (Vendors_Name.Trim() != null && Vendors_Name != "")
            {
                data_rrm = (from gatenrty in objmms.GateEntries
                            join gatechld in objmms.GateEntryChilds on gatenrty.UId equals gatechld.MUId
                            join getpo in objmms.TbIndentPurchaseOrderMasters on gatenrty.StatusTypeNo equals getpo.PurchaseOrderNo
                            where gatechld.VendorNo == Vendors_Name && gatechld.ProjectNo == ProjectId
                            orderby gatenrty.Date
                            select new DMR_Report_Model
                            {
                                GateEntryNo = gatechld.GateEntryNo,
                                Vendor = gatechld.Vendor,
                                BillNo = "",
                                StatusTypeNo = gatechld.StatusTypeNo,
                                ItemGroup = gatechld.ItemGroup,
                                Item = gatechld.Item,
                                ReceiveQty = gatechld.ReceiveQty,
                                Unit = gatechld.Unit,
                                ChallanNo = gatenrty.ChallanNo ?? "",
                                CreatedDate = gatechld.CreatedDate,
                                Date = gatenrty.Date,
                                PODate = getpo.PurchaseOrderDate,
                                Rate = gatechld.Rate,
                                Amount = Math.Round((decimal)(gatechld.ReceiveQty * gatechld.Rate), 2)
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displyTotalSum = Math.Round((decimal)(data_rrm.Sum(k => k.Amount)), 2);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
                return PartialView("_Dmr_Report_Partial", data_rrm);
            }
            else if (PONumber.Trim() != null && PONumber != "")
            {
                data_rrm = (from gatenrty in objmms.GateEntries
                            join gatechld in objmms.GateEntryChilds on gatenrty.UId equals gatechld.MUId
                            join getpo in objmms.TbIndentPurchaseOrderMasters on gatenrty.StatusTypeNo equals getpo.PurchaseOrderNo
                            where gatechld.StatusTypeNo == PONumber && gatechld.ProjectNo == ProjectId
                            orderby gatenrty.Date
                            select new DMR_Report_Model
                            {
                                GateEntryNo = gatechld.GateEntryNo,
                                Vendor = gatechld.Vendor,
                                BillNo = "",
                                StatusTypeNo = gatechld.StatusTypeNo,
                                ItemGroup = gatechld.ItemGroup,
                                Item = gatechld.Item,
                                ReceiveQty = gatechld.ReceiveQty,
                                Unit = gatechld.Unit,
                                ChallanNo = gatenrty.ChallanNo ?? "",
                                CreatedDate = gatechld.CreatedDate,
                                Date = gatenrty.Date,
                                PODate = getpo.PurchaseOrderDate,
                                Rate = gatechld.Rate,
                                Amount = Math.Round((decimal)(gatechld.ReceiveQty * gatechld.Rate), 2)
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displyTotalSum = Math.Round((decimal)(data_rrm.Sum(k => k.Amount)), 2);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
                return PartialView("_Dmr_Report_Partial", data_rrm);
            }
            else {

                //ViewBag.Error = "There some error in view";
                //return View("index");
                return View("_EmptyView");
            }

            
           
        }
        public void GetExcel()
        {
            List<DMR_Report_Model> allCust = (List<DMR_Report_Model>)Session["keepdata"];           
                  
            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(

                       grid1.Column("Date", header: "Recv. Date"),
                    grid1.Column("GateEntryNo", header: "DMR No."),
                       grid1.Column("ChallanNo", header: "Chl No."),
                       grid1.Column("Vendor", header: "Vendor Name", canSort: false),
                       grid1.Column("PODate", header: "PO Date"),
                       grid1.Column("StatusTypeNo", header: "PO No. "),
                        //grid1.Column("ItemGroup", header: "Item Group Name", canSort: false),
                        grid1.Column("Item", header: "Item Name"),
                         grid1.Column("Unit", header: "Unit", canSort: false),
                       //grid1.Column("BillNo", header: "Bill No."),
                       grid1.Column("ReceiveQty", header: "Qty."),
                       grid1.Column("Rate", header: "Rate"),
                       grid1.Column("Amount", header: "Amount")
                        //grid1.Column("CreatedDate", header: "Date", format: @<text>@item.CreatedDate.ToString("dd/MM/yy")</text>, canSort: false)

                        //    grid1.Column("Vendor", header: "Name of Vendor", canSort: false),
                        //grid1.Column("ItemGroup", header: "Item Group Name", canSort: false),
                        // grid1.Column("Item", header: "Item Name", canSort: false),
                        //  grid1.Column("Unit", header: "Unit", canSort: false),
                        //grid1.Column("ChallanNo", header: "Challan No.", canSort: false),
                        ////grid1.Column("BillNo", header: "Bill No."),
                        //grid1.Column("StatusTypeNo", header: "PO No. "),
                        //grid1.Column("ReceiveQty", header: "Quantity", canSort: false),
                        //grid1.Column("CreatedDate", header: "Date")
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=DMRReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }
        public FileStreamResult DMRReportPdf()
        {
            List<DMR_Report_Model> allCust = (List<DMR_Report_Model>)Session["keepdata"];

            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();
            var ss = from k in allCust
                     group k by k.GateEntryNo into g
                     select new
                     { SUM = g.Sum(o => o.Amount) };
            var totAmt = Math.Round((decimal)(ss.Sum(l => l.SUM)), 2) ;

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(

                       grid1.Column("Date", header: "Recv. Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Date)),
                        grid1.Column("GateEntryNo", header: "DMR No."),
                       grid1.Column("ChallanNo", header: "Chl No."),
                       grid1.Column("Vendor", header: "Vendor Name", canSort: false),
                       grid1.Column("PODate", header: "PO Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.PODate)),
                       grid1.Column("StatusTypeNo", header: "PO No. "),
                        //grid1.Column("ItemGroup", header: "Item Group Name", canSort: false),
                        grid1.Column("Item", header: "Item Name"),
                         grid1.Column("Unit", header: "Unit", canSort: false),
                       //grid1.Column("BillNo", header: "Bill No."),
                       grid1.Column("ReceiveQty", header: "Qty.", style: "right"),
                       grid1.Column("Rate", header: "Rate", style: "right"),
                       grid1.Column("Amount", header: "Amount", format: (item) => string.Format("{0:0.00}", item.Amount), style: "right")                       
                       )
                    ).ToString();

            //string exportData = String.Format("<html><head>{0}</head><body>{1}</body></html>", "<style>table{ border-spacing: 1px; border-collapse: collapse; width: 100%; font-size: 10px;}</style>", gridHtml);
            //string exportData = String.Format("<html><head>{0}</head><body>{1}<p style='text-align:right; font-size: 11px;'>Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "</p></body></html>", "<div style='text-align:center;'>AHLUWALIA CONTRACTS (INDIA) LTD.</div><div style='text-align:center;'> Site - " + a + " </div><div style='text-align:center'>DAILY MATERIAL RECEIPT REPORT</div><style type='text/css' media='all'>table{border-spacing: 0; border-collapse: separate; width: 100%; font-size: 10px;}table,th,td{border:1px solid #333333;}th{background-color:#cccccc;}th{text-align:center;}th,td{padding:5px;}</style>", gridHtml);
            // string exportData = String.Format("<html><head>{0}</head><body>{1}{2}</body></html>", "<div style='text-align:right; font-size: 11px;padding-bottom:50px;'>Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "</div><div style='text-align:center;'>AHLUWALIA CONTRACTS (INDIA) LTD.</div><div style='text-align:center;'> Site - " + a + " </div><div style='text-align:center'>DAILY MATERIAL RECEIPT REPORT</div><div>From Date : "+ Session["FromDate"]  + ", To Date : "+  Session["ToDate"] + " </div> <style type='text/css' media='all'>table{border-spacing: 0; border-collapse: separate; width: 100%;  font-size: 11px;}table,th,td{border:1px solid #333333;}th{background-color:#cccccc;}th{text-align:center;} td{padding:5px;}.alignR{text-align:right;}</style>", gridHtml, "<div> <div style='text-align:right;font-size: 11px;float:right; padding-right:0px'>Total Amount : " + totAmt + "</div></div>");

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "DAILY MATERIAL RECEIPT REPORT";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div>From Date : " + Session["FromDate"] + ", To Date : " + Session["ToDate"] + " </div>";

            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 30, 50);

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

        //public FileStreamResult ExportData()
        //{
        //    List<DMR_Report_Model> allCust = (List<DMR_Report_Model>)Session["keepdata"];

        //    WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
        //    string gridHtml = grid1.GetHtml(
        //            columns: grid1.Columns(
        //                 grid1.Column("Vendor", header: "Name of Vendor", canSort: false),
        //               grid1.Column("ItemGroup", header: "Item Group Name", canSort: false),
        //                grid1.Column("Item", header: "Item Name", canSort: false),
        //                 grid1.Column("Unit", header: "Unit", canSort: false),
        //               grid1.Column("ChallanNo", header: "Challan No.", canSort: false),
        //               //grid1.Column("BillNo", header: "Bill No."),
        //               grid1.Column("StatusTypeNo", header: "PO No. "),
        //               grid1.Column("ReceiveQty", header: "Quantity", canSort: false),
        //               grid1.Column("CreatedDate", header: "Date")
        //                )
        //            ).ToString();

        //    StringBuilder pdfExportData = new StringBuilder();
        //    pdfExportData.AppendLine("<html><head><style>.gridTable{font - family: Arial,Helvetica,sans - serif; font - size: 14px; font - weight: normal; width: 100; display: table; border - collapse: collapse; border: solid px #C5C5C5;background-color: white;}</style>");  
        //    pdfExportData.AppendLine("</head><body>" + gridHtml + "</body></html>");

        //        var bytes = System.Text.Encoding.UTF8.GetBytes(pdfExportData.ToString());
        //        using (var stream = new MemoryStream(bytes))
        //        {
        //            var outStream = new MemoryStream();
        //            var doc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50);
        //            var writer = PdfWriter.GetInstance(doc, outStream);
        //            writer.CloseStream = false;
        //            doc.Open();

        //            var xmlWorker = iTextSharp.tool.xml.XMLWorkerHelper.GetInstance();
        //            xmlWorker.ParseXHtml(writer, doc, stream, System.Text.Encoding.UTF8);
        //            doc.Close();
        //            outStream.Position = 0;
        //            return new FileStreamResult(outStream, "application/pdf");
        //        }
        //    }

        }
}