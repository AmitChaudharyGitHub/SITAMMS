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
    public class TransferStockController : Controller
    {
        MMSEntities objmms = new MMSEntities();
        MSP_Model objmsps = new MSP_Model();
        FactoryManager m = new FactoryManager();
        // GET: TransferStock
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

            
        
            //List<GET_TRANSFERRED_STOCK> result = new List<GET_TRANSFERRED_STOCK>();
            //result = objmsps.GetAllTransferredStock(Prjid);

            //var lst = result.Where(x => x.ProjectID == Prjid).OrderBy(a => a.TransferTo).ToList();
            //var lstSiteName = lst.Distinct<GET_TRANSFERRED_STOCK>().Select(x1 => new SelectListItem
            //{
            //    Value = x1.SiteId.ToString(),
            //    Text = x1.TransferTo.ToString()
            //});
            //ViewBag.TransferSites = lstSiteName;

            //var lst1 = result.Where(x => x.ProjectID == Prjid).OrderBy(a1 => a1.TransferNo).ToList();
            //var lstTransferNo = lst.Distinct<GET_TRANSFERRED_STOCK>().Select(x11 => new SelectListItem
            //{
            //    Value = x11.TransferNo.ToString(),
            //    Text = x11.TransferNo.ToString()
            //});
            //ViewBag.TransferNos = lstTransferNo;

            return View();
        }
        public ActionResult GetAllDatas(string PID, string fromdate, string todate, string ItemGroups, string Items, string TransferSites, string TransferNos)
        {
            List<GET_TRANSFERRED_STOCK> result = new List<GET_TRANSFERRED_STOCK>();
            result = objmsps.GetAllTransferredStock(PID);

            Session["fdate"] = fromdate;
            Session["tdate"] = todate;
            Session["GID"] = ItemGroups;
            Session["ITID"] = Items;
            Session["TSite"] = TransferSites;
            Session["TNo"] = TransferNos;
            Session["ProId"] = PID;

            if (fromdate != "" && todate != "" && ItemGroups != "" && Items != "" && TransferSites != "" && TransferNos != "")
            {
                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var lst = result.Where(x => x.TransferDate >= date1 && x.TransferDate <= date2 && x.ItemGroupID == ItemGroups && x.ItemID == Items && x.SiteId == TransferSites && x.TransferNo == TransferNos).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else if (fromdate != "" && todate != "" && ItemGroups != "" && Items != "" && TransferSites != "")
            {
                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var lst = result.Where(x => x.TransferDate >= date1 && x.TransferDate <= date2 && x.ItemGroupID == ItemGroups && x.ItemID == Items && x.SiteId == TransferSites).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else if (fromdate != "" && todate != "" && ItemGroups != "" && Items != "")
            {
                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                var lst = result.Where(x => x.TransferDate >= date1 && x.TransferDate <= date2 && x.ItemGroupID == ItemGroups && x.ItemID == Items).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else if (fromdate != "" && todate != "" && ItemGroups != "")
            {
                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var lst = result.Where(x => x.TransferDate >= date1 && x.TransferDate <= date2 && x.ItemGroupID == ItemGroups).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else if (fromdate != "" && todate != "")
            {
                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var lst = result.Where(x => x.TransferDate >= date1 && x.TransferDate <= date2).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else if (ItemGroups != "" && Items != "" && TransferSites != "" && TransferNos != "")
            {
                var lst = result.Where(x => x.ItemGroupID == ItemGroups && x.ItemID == Items && x.SiteId == TransferSites && x.TransferNo == TransferNos).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else if (ItemGroups != "" && Items != "" && TransferSites != "")
            {
                var lst = result.Where(x => x.ItemGroupID == ItemGroups && x.ItemID == Items && x.SiteId == TransferSites).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else if (ItemGroups != "" && Items != "")
            {
                var lst = result.Where(x => x.ItemGroupID == ItemGroups && x.ItemID == Items).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else if (ItemGroups != "" && Items != "" && TransferNos != "")
            {
                var lst = result.Where(x => x.ItemGroupID == ItemGroups && x.ItemID == Items && x.TransferNo == TransferNos).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else if (ItemGroups != "")
            {
                var lst = result.Where(x => x.ItemGroupID == ItemGroups).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else if (TransferNos != "" && TransferNos !="0")
            {
                var lst = result.Where(x => x.TransferNo == TransferNos).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else if (TransferSites != "" && TransferSites !="0")
            {
                var lst = result.Where(x => x.SiteId == TransferSites).OrderByDescending(x1 => x1.TransferDate).ToList();

                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = Math.Round((decimal)(lst.Sum(k => k.Amount)), 2);
                return PartialView("_PartialView_TransferredStock", lst);
            }
            else
            {
                return View("_EmptyView");
            }
        }
        public void GetExcel()
        {
            List<GET_TRANSFERRED_STOCK> allCust = (List<GET_TRANSFERRED_STOCK>)Session["keepdata"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                       grid1.Column("TransferDate", header: "Transfer Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.TransferDate)),
                       grid1.Column("TransferNo", header: "Transfer No"),
                       grid1.Column("ItemGroupName", header: "Group"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("Quantity", header: "Quantity", format: (item) => string.Format("{0:0.00}", item.Quantity)),
                       grid1.Column("Amount", header: "Amount", format: (item) => string.Format("{0:0.00}", item.Amount)),
                       grid1.Column("TransferTo", header: "Transfer To")
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
        public FileStreamResult TransferStockPdf()
        {
            List<GET_TRANSFERRED_STOCK> allCust = (List<GET_TRANSFERRED_STOCK>)Session["keepdata"];

            string fd, tdate, fd1, td2, gn, gn1, itn, TSite, TSite1, TNo, TNo1;
            fd1 = Session["fdate"].ToString();
            td2 = Session["tdate"].ToString();
            gn = Session["GID"].ToString();
            itn = Session["ITID"].ToString();
            TSite = Session["TSite"].ToString();
            TNo1 = Session["TNo"].ToString();
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
            if (gn == "")
            {
                gn1 = "-";
            }
            else
            {
                gn1 = objmms.tblItemGroupMasters.Where(a2 => a2.ItemGroupID == gn).ToList().Select(a1 => a1.GroupName).First();
            }
            var itn1 = "";
            if (itn == "")
            {
                itn1 = "-";
            }
            else
            {
                itn1 = objmms.tblItemMasters.Where(a3 => a3.ItemID == itn).ToList().Select(a4 => a4.ItemName).First();
            }
            if (TSite == "")
            {
                TSite1 = "-";
            }
            else
            {
                TSite1 = objmms.tblProjectMasters.Where(b111 => b111.PRJID == TSite).OrderByDescending(c111 => c111.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();
            }
            if (TNo1 == "")
            {
                TNo = "-";
            }
            else
            {
                TNo = TNo1;
            }

            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();

            var ss = from k in allCust
                     group k by k.ItemID into g
                     select new
                     { SUM = g.Sum(o => o.Amount) };
            var totAmt = Math.Round((decimal)(ss.Sum(l => l.SUM)), 2);

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                    grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("TransferDate", header: "Transfer Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.TransferDate)),
                       grid1.Column("TransferNo", header: "Transfer No"),
                       grid1.Column("ItemGroupName", header: "Group"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("Quantity", header: "Quantity", format: (item) => string.Format("{0:0.00}", item.Quantity),style: "right"),
                       grid1.Column("Amount", header: "Amount", format: (item) => string.Format("{0:0.00}", item.Amount),style: "right"),
                       grid1.Column("TransferTo", header: "Transfer To")
                       )
                    ).ToString();

            

            
            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "MATERIAL TRANSFER REPORT";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>From Date: " + fd + " To Date: " + tdate + " </div><div style='text-align:left'>Group Name: " + gn1 + " Item Name: " + itn1 + "</div><div style='text-align:left'>Transfer To: " + TSite1 + "  Transfer No: " + TNo + "</div>";

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

        public JsonResult BindTransferName(string ProjectId)
        {
            string J = string.Empty;
            List<GET_TRANSFERRED_STOCK> result = new List<GET_TRANSFERRED_STOCK>();
            result = objmsps.GetAllTransferredStock(ProjectId);

            var lst = result.Where(x => x.ProjectID == ProjectId).OrderBy(a => a.TransferTo).ToList();
            var lstSiteName = lst.Distinct<GET_TRANSFERRED_STOCK>().Select(x1 => new SelectListItem
            {
                Value = x1.SiteId.ToString(),
                Text = x1.TransferTo.ToString()
            });


           

            J = lstSiteName.ToJSON();
            return Json(J,JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindTransferNo(string ProjectId)
        {
            string J = string.Empty;
            List<GET_TRANSFERRED_STOCK> result = new List<GET_TRANSFERRED_STOCK>();
            result = objmsps.GetAllTransferredStock(ProjectId);
            var lst1 = result.Where(x => x.ProjectID == ProjectId).OrderBy(a1 => a1.TransferNo).ToList();
            var lstTransferNo = lst1.Distinct<GET_TRANSFERRED_STOCK>().Select(x11 => new SelectListItem
            {
                Value = x11.TransferNo.ToString(),
                Text = x11.TransferNo.ToString()
            });
            J = lstTransferNo.ToJSON();
            return Json(J, JsonRequestBehavior.AllowGet);
        }
    }
}