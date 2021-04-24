using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects.Entity;
using DataAccessLayer;
using System.Linq.Dynamic;
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
    public class CurrentStockController : Controller
    {
        MMSEntities objmms = new MMSEntities();
        MSP_Model objmsps = new MSP_Model();
        FactoryManager m = new FactoryManager();
        // GET: CurrentStock
        [Authorize]
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
        public JsonResult GetItemByGroupStock(string Pid, string Gid)
        {
            if (Pid != null && Gid != null)
            {
                List<Listobj> ItemMasters = m.GettblItemCurrentStockManager().FindByItemGroup(Pid, Gid);
                Session["Items"] = ItemMasters;
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public ActionResult GetAllDatas(string PID, string GID, string ITID)
        {
            List<GET_CURRENT_STOCK> result = new List<GET_CURRENT_STOCK>();
            Session["GID"] = GID;
            Session["ITID"] = ITID;
            Session["ProId"] = PID;
            result = objmsps.GetAllCurrentStock(PID);

            if (GID != "" && ITID != "")
            {
                var lst = result.Where(x => x.ItemId == ITID && x.ItemGroupId == GID).OrderBy(x1 => x1.ItemName).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = lst.Sum(k => k.CurrentStockAmt);
                return PartialView("_PartialView_CurrentStock", lst);
            }
            else if (GID != null && GID !="")
            {
                var lst = result.Where(x => x.ItemGroupId == GID).OrderBy(x1 => x1.ItemName).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = lst.Sum(k => k.CurrentStockAmt);
                return PartialView("_PartialView_CurrentStock", lst);
            }
            else if (PID != "" && GID == "" && ITID == "")
            {
                Session["keepdata"] = result;
                ViewBag.displyTotalSum = result.Sum(k => k.CurrentStockAmt);
                return PartialView("_PartialView_CurrentStock", result);
            }
            else
            {
                return View("_EmptyView");
            }            
        }

        public void GetExcel()
        {
            List<GET_CURRENT_STOCK> allCust = (List<GET_CURRENT_STOCK>)Session["keepdata"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                      grid1.Column("GroupName", header: "Group"),
                      grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("ReceiveQty", header: "Receipt"),
                       grid1.Column("IssueQty", header: "Issued"),
                       grid1.Column("CurrentStockQty", header: "Available"),
                       grid1.Column("CurrentStockAmt", header: "Stock Value")
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=CurrentStock.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }

        public FileStreamResult CurrentStockPdf()
        {
            List<GET_CURRENT_STOCK> allCust = (List<GET_CURRENT_STOCK>)Session["keepdata"];

            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();
           
            var ss = from k in allCust
                     group k by k.ItemId into g
                     select new
                     { SUM = g.Sum(o => o.CurrentStockAmt) };
            var totAmt = Math.Round((decimal)(ss.Sum(l => l.SUM)), 2);

            string gn = Session["GID"].ToString();
            var gn1 = objmms.tblItemGroupMasters.Where(a2 => a2.ItemGroupID == gn).ToList().Select(a1 => a1.GroupName).FirstOrDefault();

            string itn = Session["ITID"].ToString();
            var itn1 = "";
            if (itn == "")
            {
                itn1 = "-";
            }
            else
            {
                itn1 = objmms.tblItemMasters.Where(a3 => a3.ItemID == itn).ToList().Select(a4 => a4.ItemName).First();
            }

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                       grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                       grid1.Column("GroupName", header: "Group"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("ReceiveQty", header: "Receipt",style: "right"),
                       grid1.Column("IssueQty", header: "Issued", style: "right"),
                       grid1.Column("CurrentStockQty", header: "Available", style: "right"),
                       grid1.Column("CurrentStockAmt", header: "Stock Value", style: "right")
                       )
                    ).ToString();

            //  string exportData = String.Format("<html><head>{0}</head><body>{1}{2}</body></html>", "<div style='text-align:right; font-size: 11px;'>Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "</div><div style='text-align:center;'>AHLUWALIA CONTRACTS (INDIA) LTD.</div><div style='text-align:center;'> Site - " + a + " </div><div style='text-align:center'>CURRENT STOCK</div><div style='text-align:left'>Group Name: " + gn1 + " Item Name: " + itn1 + " </div><style type='text/css' media='all'>table{border-spacing: 0; border-collapse: separate; width: 100%;  font-size: 11px;}table,th,td{border:1px solid #333333;}th{background-color:#cccccc;}th{text-align:center;} td{padding:5px;}.alignR{text-align:right;}</style>", gridHtml, "<div> <div style='text-align:right;font-size: 11px;float:right; padding-right:0px'>Total Amount : " + totAmt + "</div></div>");
            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "CURRENT STOCK";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>Group Name: " + gn1 + " Item Name: " + itn1 + " </div><br/>";

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

        public ActionResult DailyMaterialIssueReport()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;
            return View();
        }

       
        public ActionResult DailyMaterialIssueReportGrid(string PID, string  FromDate , string  ToDate)
        {
            List<GET_CURRENT_STOCK> result = new List<GET_CURRENT_STOCK>(); 
            result = objmsps.GetDailyIssuedStock(PID, FromDate, ToDate);
           
            Session["fdate"] = FromDate;
            Session["tdate"] = ToDate;
            Session["ProId"] = PID;
            if (result !=null)
            {
                Session["keepdata"] = result;
                ViewBag.displyTotalSum = result.Sum(k => k.IssueAmt);
                return PartialView("_DailyMasterial_IssueGrid", result);
            }
            
            else {
                return View("_EmptyView");
            }

          

          
        }

        public FileStreamResult DailyIssueStockPdf()
        {
            List<GET_CURRENT_STOCK> allCust = (List<GET_CURRENT_STOCK>)Session["keepdata"];

            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();

            var ss = from k in allCust
                     group k by k.ItemId into g
                     select new
                     { SUM = g.Sum(o => o.IssueAmt) };
            var totAmt = Math.Round((decimal)(ss.Sum(l => l.SUM)), 2);

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





            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                       grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                       grid1.Column("GroupName", header: "Item Group Name "),
                       grid1.Column("ItemId", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("IssueQty", header: "Qty.", style: "right"),
                       grid1.Column("IssueAmt", header: "Amount", style: "right")
                       )
                    ).ToString();

            //string exportData = String.Format("<html><head>{0}</head><body>{1}{2}</body></html>", "<div style='text-align:right; font-size: 11px;'>Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "</div><div style='text-align:center;'>AHLUWALIA CONTRACTS (INDIA) LTD.</div><div style='text-align:center;'> Site - " + a + " </div><div style='text-align:center'>Summary Report of Daily Material Issue</div><div style='text-align:left'>From Date: " + fd + "  To Date: " + tdate + " </div><style type='text/css' media='all'>table{border-spacing: 0; border-collapse: separate; width: 100%;  font-size: 11px;}table,th,td{border:1px solid #333333;}th{background-color:#cccccc;}th{text-align:center;} td{padding:5px;}.alignR{text-align:right;}</style>", gridHtml, "<div> <div style='text-align:right;font-size: 11px;float:right; padding-right:0px'>Total Amount : " + totAmt + "</div></div>");

            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-"+a;
            string reportName = "Summary Report of Daily Material Issue";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>From Date: " + fd + "  To Date: " + tdate + " </div><br/>";

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
            string totalTable = "<table align='right' style='float:right;border-top:0; width:80px;'><tr><td><strong>Total</strong></td><td class='right'>" + totAmt + "</td></tr></table>";
            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            styleSheet1.LoadTagStyle("table", "float", "right");
            styleSheet1.LoadTagStyle("table", "size", "16px");
            styleSheet1.LoadTagStyle("table", "width", "212");
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


        public static String DMYToMDY(String input)
        {
            return Regex.Replace(input,
            @"\b(?<day>\d{1,2})/(?<month>\d{1,2})/(?<year>\d{2,4})\b",
            "${month}/${day}/${year}");
        }

        public void DailyMaterialIssueExcel()
        {
            List<GET_CURRENT_STOCK> allCust = (List<GET_CURRENT_STOCK>)Session["keepdata"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                      grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                       grid1.Column("GroupName", header: "Item Group Name "),
                       grid1.Column("ItemId", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("IssueQty", header: "Qty.", style: "right"),
                       grid1.Column("IssueAmt", header: "Amount", style: "right")
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=Summary_Report_Of_Daily_Material_Issue.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }

    }
}