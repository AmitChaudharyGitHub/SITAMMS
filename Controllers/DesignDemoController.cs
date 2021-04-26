using DataAccessLayer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MMS.DAL;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class DesignDemoController : Controller
    {
        // GET: DesignDemo

        MMSEntities objmms = new MMSEntities();
        Procedure objmsps = new Procedure();
        FactoryManager m = new FactoryManager();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PIdetail()
        {
            ViewBag.EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            return View();
        }

        public ActionResult POdetail()
        {
            ViewBag.EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            return View();
        }

        public ActionResult POPMCdetail()
        {
            ViewBag.EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            return View();
        }
   
        public ActionResult POQtyDetail()
        {
            ViewBag.EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            return View();
        }
        public ActionResult StockLedgerDetail()
        {
            ViewBag.EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            ViewBag.ItemGroup = new SelectList(m.GetItemTypeManager().FindAll(), "ItemGroupID", "GroupName");
            return View();
        }


        public ActionResult GetDemoData(string ProjectId, string PI,string PO,string VendorId,string ItemGroupId,string ItemId, DateTime? fromdate, DateTime? todate)
        {

            List<PIDetail> result = new List<PIDetail>();
            if(fromdate!=null && todate != null)
            {
                //DateTime.ParseExact(fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime date1 = fromdate.Value;
                DateTime date2 = todate.Value;
                result = objmsps.GetPIDetailWithPO(ProjectId, PI, PO, VendorId, ItemGroupId, ItemId, date1.ToString("yyyy-MM-dd"), date2.ToString("yyyy-MM-dd"));
            }
            else
            {
                result = objmsps.GetPIDetailWithPO(ProjectId, PI, PO, VendorId, ItemGroupId, ItemId, "", "");
            }
            
            Session["fdate"] = fromdate;
            Session["tdate"] = todate;
            Session["ProId"] = ProjectId;
            Session["PI"] = PI;

            Session["keepPIDetaildata"] = result;
            return PartialView("_PIDetailWithPO", result.ToList());
        }

        #region helpers

        public JsonResult GetPOByPI(string PI)
        {
            var data = objmms.TbIndentPurchaseOrder_GST.Where(x => x.IndentRefNo == PI).Select(x => new { Text = x.PurchaseOrderNo, Value = x.PurchaseOrderNo }).OrderBy(x => x.Text).ToList().ToJSON();
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPO(string ProjectId)
        {
            var data = objmms.TbIndentPurchaseOrder_GST.Where(x => x.ProjectNo==ProjectId).Select(x => new { Text = x.PurchaseOrderNo, Value = x.PurchaseOrderNo }).OrderBy(x => x.Text).ToList().ToJSON();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVendors(string PId)
        {
            var data=Helpers.HelperMethods.GetPCList(PId);
            return Json(data.ToJSON(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemGroups()
        {
            return Json(Helpers.HelperMethods.GetItemGroups(objmms).ToJSON(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemsByGroupId(string GID)
        {
            return Json(Helpers.HelperMethods.GetGroupItems(GID,objmms).ToJSON(), JsonRequestBehavior.AllowGet);
        }

        #endregion



        public FileStreamResult PIDetailRecordPdf()
        {
            List<PIDetail> allCust = (List<PIDetail>)Session["keepPIDetaildata"];

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

            string PI = Session["PI"].ToString();



            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();

            //var ss = from k in allCust
            //         group k by k.ItemId into g
            //         select new
            //         { SUM = g.Sum(o => o.ReceiveAmt) };
            //var totAmt = Math.Round((decimal)(ss.Sum(l => l.SUM)), 2);

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("PIDate", header: "PI Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.PIDate)),
                       grid1.Column("ItemGroup", header: "Item Group"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item Name"),
                         grid1.Column("Unit", header: "Unit"),
                       grid1.Column("PIQty", header: "PI Qty.", format: (item) => string.Format("{0:0.000}", item.PIQty), style: "alignR"),
                       grid1.Column("ApprovePIQty", header: "Approve Qty.", format: (item) => string.Format("{0:0.000}", item.ApprovePIQty), style: "alignR"),
                       grid1.Column("PONo", header: "PO. No.")
                       )
                    ).ToString();

            //string exportData = String.Format("<html><head>{0}</head><body>{1}</body></html>", "<div style='text-align:right; font-size: 11px;'>Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "</div><div style='text-align:center;'>AHLUWALIA CONTRACTS (INDIA) LTD.</div><div style='text-align:center;'> Site - " + a + " </div><div style='text-align:center'>Details of Purchase Indent vis a vis Purchase order </div> <div style='text-align:left'>Purchase Indent : "+ PI+ " </div><div style='text-align:left'>From Date: " + fd + " To Date: " + tdate + " </div><style type='text/css' media='all'>table{border-spacing: 0; border-collapse: separate; width: 100%;  font-size: 11px;}table,th,td{border:1px solid #333333;}th{background-color:#cccccc;}th{text-align:center;} td{ padding:5px;}.alignR{text-align:right;}</style>", gridHtml);
            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Details of Purchase Indent vis a vis Purchase order";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div>Purchase Indent : " + PI + " </div><div>From Date: " + fd + " To Date: " + tdate + " </div><br/>";

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

        public static String DMYToMDY(String input)
        {
            return Regex.Replace(input,
            @"\b(?<day>\d{1,2})/(?<month>\d{1,2})/(?<year>\d{2,4})\b",
            "${month}/${day}/${year}");
        }


        public JsonResult BindVendorPrjectWiseAndPOWise(string ProjectId, string PONo)
        {
            string j = string.Empty;
            string SupplierId = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PONo).FirstOrDefault().SupplierNo;
            var ven = objmms.tblVendorMasters.Where(x => x.PRJID.Contains(ProjectId) && x.VendorID == SupplierId).OrderBy(a => a.Name).ToList();
            var t = ven.Distinct<DAL.tblVendorMaster>().Select(x => new SelectListItem
            {
                Value = x.VendorID.ToString(),
                Text = x.Name.ToString()
            });

            j = t.ToJSON();
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVendorDetails(string VendorId)
        {
            string J = string.Empty;
            tblVendorMaster ven = objmms.tblVendorMasters.Where(x => x.VendorID == VendorId).FirstOrDefault();
            string CountryName = objmms.tblCountries.Where(c => c.CountryID == ven.Country).FirstOrDefault().CnName;
            string StateName = objmms.tblStates.Where(s => s.StateID == ven.State).FirstOrDefault().StateName;
            string CityName = objmms.tblCityLists.Where(ct => ct.CityID == ven.City).FirstOrDefault().CityName;
            string Address = ven.Address;
            var V = new { Country = CountryName ?? "N/A", State = StateName ?? "N/A", city = CityName ?? "N/A", Addrs = Address ?? "N/A" };
            J = V.ToJSON();
            return Json(J, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetPODetailGrid(string ProjectId, string PONo, string fromdate, string todate, string Ven)
        {
            List<POQtyDetails> result = new List<POQtyDetails>();
            result = objmsps.GetPOQithQtyDetail(ProjectId, PONo);
            Session["fdate"] = fromdate;
            Session["tdate"] = todate;
            Session["Ven"] = Ven;
            Session["ProId"] = ProjectId;

            if (fromdate != "" && todate != "" && (Ven != "" && Ven != "0"))
            {
                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var lst = result.Where(x => DateTime.Parse(x.PurchaseOrderDate) >= date1 && DateTime.Parse(x.PurchaseOrderDate) <= date2 && x.SupplierNo == Ven).OrderByDescending(x1 => x1.PurchaseOrderDate).ToList();

                Session["keepPOQtydata"] = lst;
                return PartialView("_POQtyDetails", lst.ToList());
            }
            else if (fromdate != "" && todate != "")
            {

                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var lst = result.Where(x => DateTime.Parse(x.PurchaseOrderDate) >= date1 && DateTime.Parse(x.PurchaseOrderDate) <= date2).OrderByDescending(x1 => x1.PurchaseOrderDate).ToList();

                Session["keepPOQtydata"] = lst;
                return PartialView("_POQtyDetails", lst.ToList());

            }
            else if (Ven != "" && Ven != "0")
            {

                var lst = result.Where(x => x.SupplierNo == Ven).OrderByDescending(x1 => x1.PurchaseOrderDate).ToList();

                Session["keepPOQtydata"] = lst;
                return PartialView("_POQtyDetails", lst.ToList());
            }
            else if (ProjectId != "0" && fromdate == "" && todate == "" && (Ven == "0" || Ven == ""))
            {
                Session["keepPOQtydata"] = result;
                return PartialView("_POQtyDetails", result.ToList());
            }
            else
            {
                return View("_EmptyView");
            }

        }


        public FileStreamResult POQtyPdf()
        {
            List<POQtyDetails> allCust = (List<POQtyDetails>)Session["keepPOQtydata"];

            string fd, tdate, fd1, td2, vend, vend1;
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

            vend1 = Session["Ven"].ToString();
            if (vend1 == "")
            {
                vend = "-";
            }
            else
            {
                vend = objmms.tblVendorMasters.Where(c1 => c1.VendorID == vend1).ToList().Select(x => x.Name).FirstOrDefault();
            }

            string pid = Session["ProId"].ToString();
            string CountryName = string.Empty; string StateName = string.Empty; string CityName = string.Empty; string Address = string.Empty;
            if (objmms.tblVendorMasters.Where(x => x.VendorID == vend1).ToList().Count() > 0)
            {
                tblVendorMaster ven = objmms.tblVendorMasters.Where(x => x.VendorID == vend1).FirstOrDefault();

                if (ven.Country != null && ven.Country != "")
                { CountryName = objmms.tblCountries.Where(c => c.CountryID == ven.Country).FirstOrDefault().CnName; }
                else { CountryName = "-"; }
                if (ven.State != null && ven.State != "")
                { StateName = objmms.tblStates.Where(s => s.StateID == ven.State).FirstOrDefault().StateName; }
                else { StateName = "-"; }
                if (ven.City != null && ven.City != "")
                { CityName = objmms.tblCityLists.Where(ct => ct.CityID == ven.City).FirstOrDefault().CityName; }
                else { CityName = "-"; }
                if (ven.Address != null && ven.Address != "")
                { Address = ven.Address; }
                else
                {
                    Address = "-";
                }

            }
            else
            {
                CountryName = "-";
                StateName = "-";
                CityName = "-";
                Address = "-";
            }



            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();

            //var ss = from k in allCust
            //         group k by k.ItemId into g
            //         select new
            //         { SUM = g.Sum(o => o.ReceiveAmt) };
            //var totAmt = Math.Round((decimal)(ss.Sum(l => l.SUM)), 2);

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                   grid1.Column("PurchaseOrderNo", header: "Purchase Order No."),
                     grid1.Column("PurchaseOrderDate", header: "PO Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.PurchaseOrderDate)),

                       grid1.Column("SupplierName", header: "Name Of Vendor"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("Item_description", header: "Item Description"),
                       grid1.Column("POQty", header: "PO Qty"),
                       grid1.Column("ReceiveQty", header: "Receive Qty", format: (item) => string.Format("{0:0.000}", item.ReceiveQty), style: "alignR"),
                       grid1.Column("BalanceQty", header: "Balance Qty", format: (item) => string.Format("{0:0.000}", item.BalanceQty), style: "alignR")
                       )
                    ).ToString();


            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Details of Purchase Order Pending (in part/ full)";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div>From Date: " + fd + " To Date: " + tdate + " </div><div> Vendor: " + vend + " </div> <br></br> <div style='text-align:left'>Address : " + Address + " <br></br> Country : " + CountryName + " , State : " + StateName + " , City : " + CityName + " </div>";

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



        public ActionResult GetPOItemDetailInGrid(string ProjectId, string PONo, string fromdate, string todate, string Ven)
        {
            List<PoDetails> result = new List<PoDetails>();
            result = objmsps.GetPODetail(ProjectId, PONo);
            Session["fdatePO"] = fromdate;
            Session["tdatePO"] = todate;
            Session["VenPO"] = Ven;
            Session["ProIdPO"] = ProjectId;


            if (fromdate != "" && todate != "" && (Ven != "" && Ven != "0"))
            {
                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var lst = result.Where(x => DateTime.Parse(x.PurchaseOrderDate) >= date1 && DateTime.Parse(x.PurchaseOrderDate) <= date2 && x.SupplierNo == Ven).OrderByDescending(x1 => x1.PurchaseOrderDate).ToList();

                Session["keepPO"] = lst;
                return PartialView("_PODetailGrid", lst.ToList());
            }
            else if (fromdate != "" && todate != "")
            {

                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var lst = result.Where(x => DateTime.Parse(x.PurchaseOrderDate) >= date1 && DateTime.Parse(x.PurchaseOrderDate) <= date2).OrderByDescending(x1 => x1.PurchaseOrderDate).ToList();

                Session["keepPO"] = lst;
                return PartialView("_PODetailGrid", lst.ToList());

            }
            else if (Ven != "" && Ven != "0")
            {

                var lst = result.Where(x => x.SupplierNo == Ven).OrderByDescending(x1 => x1.PurchaseOrderDate).ToList();

                Session["keepPO"] = lst;
                return PartialView("_PODetailGrid", lst.ToList());
            }
            else if (ProjectId != "0" && fromdate == "" && todate == "" && (Ven == "0" || Ven == ""))
            {
                Session["keepPO"] = result;
                return PartialView("_PODetailGrid", result.ToList());
            }

            else
            {
                return View("_EmptyView");
            }

        }



        public FileStreamResult PODetailItemWisepdf()
        {
            List<PoDetails> allCust = (List<PoDetails>)Session["keepPO"];

            string fd, tdate, fd1, td2, vend, vend1;
            fd1 = Session["fdatePO"].ToString();
            td2 = Session["tdatePO"].ToString();
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

            vend1 = Session["VenPO"].ToString();
            if (vend1 == "")
            {
                vend = "-";
            }
            else
            {
                vend = objmms.tblVendorMasters.Where(c1 => c1.VendorID == vend1).ToList().Select(x => x.Name).FirstOrDefault();
            }

            string CountryName = string.Empty; string StateName = string.Empty; string CityName = string.Empty; string Address = string.Empty;
            if (objmms.tblVendorMasters.Where(x => x.VendorID == vend1).ToList().Count() > 0)
            {

                tblVendorMaster ven = objmms.tblVendorMasters.Where(x => x.VendorID == vend1).FirstOrDefault();

                if (ven.Country != null && ven.Country != "")
                { CountryName = objmms.tblCountries.Where(c => c.CountryID == ven.Country).FirstOrDefault().CnName; }
                else { CountryName = "-"; }
                if (ven.State != null && ven.State != "")
                { StateName = objmms.tblStates.Where(s => s.StateID == ven.State).FirstOrDefault().StateName; }
                else { StateName = "-"; }
                if (ven.City != null && ven.City != "")
                { CityName = objmms.tblCityLists.Where(ct => ct.CityID == ven.City).FirstOrDefault().CityName; }
                else { CityName = "-"; }
                if (ven.Address != null && ven.Address != "")
                { Address = ven.Address; }
                else
                {
                    Address = "-";
                }

            }
            else
            {
                CountryName = "-";
                StateName = "-";
                CityName = "-";
                Address = "-";
            }

            string pid = Session["ProIdPO"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();

            //var ss = from k in allCust
            //         group k by k.ItemId into g
            //         select new
            //         { SUM = g.Sum(o => o.ReceiveAmt) };
            //var totAmt = Math.Round((decimal)(ss.Sum(l => l.SUM)), 2);

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                   grid1.Column("PurchaseOrderNo", header: "Purchase Order No."),
                     grid1.Column("PurchaseOrderDate", header: "PO Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.PurchaseOrderDate)),

                       grid1.Column("SupplierName", header: "Name Of Vendor"),
                       grid1.Column("ItemName", header: "Item Name"),

                       grid1.Column("Item_description", header: "Item Description"),
                       grid1.Column("UnitName", header: "Unit Name"),
                       grid1.Column("POQty", header: "PO Qty", format: (item) => string.Format("{0:0.000}", item.POQty), style: "alignR"),
                       grid1.Column("Rate", header: "Rate", format: (item) => string.Format("{0:0.00}", item.Rate), style: "alignR"),
                       grid1.Column("Amount", header: "Amount", format: (item) => string.Format("{0:0.00}", item.Amount), style: "alignR"),
                       grid1.Column("Discount", header: "Discount", format: (item) => string.Format("{0:0.00}", item.Discount), style: "alignR"),
                       grid1.Column("PackingCharges", header: "Packing Charges", format: (item) => string.Format("{0:0.00}", item.PackingCharges), style: "alignR"),
                       grid1.Column("ExcisepercentageAmt", header: "Excise Amt", format: (item) => string.Format("{0:0.00}", item.ExcisepercentageAmt), style: "alignR"),
                       grid1.Column("CartageAmount", header: "Carta,ge Amount", format: (item) => string.Format("{0:0.00}", item.CartageAmount), style: "alignR"),
                       grid1.Column("Tax_Amount", header: "Tax Amount", format: (item) => string.Format("{0:0.00}", item.Tax_Amount), style: "alignR"),
                       grid1.Column("InsuranceAmount", header: "Insurance Amount", format: (item) => string.Format("{0:0.00}", item.InsuranceAmount), style: "alignR"),
                       grid1.Column("Item_GrandTotal", header: "Total Amount", format: (item) => string.Format("{0:0.00}", item.Item_GrandTotal), style: "alignR")

                       )
                    ).ToString();



            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Details of Purchase Order";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div>From Date: " + fd + " To Date: " + tdate + " </div><div style='text-align:left'> Vendor: " + vend + " </div> <br></br> <div style='text-align:left'> Address : " + Address + " <br></br> Country : " + CountryName + " , State : " + StateName + " , City : " + CityName + " </div><br/>";

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



        public ActionResult GetPOPMCDetailItemWiseInGrid(string ProjectId, string fromdate, string todate)
        {
            List<POPMCDetail> result = new List<POPMCDetail>();
            result = objmsps.GetPOPMCDetailWithItem(ProjectId);

            Session["fdatePOPMC"] = fromdate;
            Session["tdatePOPMC"] = todate;
            Session["ProIdPOPMC"] = ProjectId;

            if (fromdate == "" && todate == "")
            {
                Session["KeepPOPOMC"] = result;
                return PartialView("_POPMCDetail", result.ToList());
            }
            else if (fromdate != "" && todate != "")
            {
                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var lst = result.Where(x => DateTime.Parse(x.PurchaseOrderDate) >= date1 && DateTime.Parse(x.PurchaseOrderDate) <= date2).OrderByDescending(x1 => x1.PurchaseOrderDate).ToList();
                Session["KeepPOPOMC"] = lst;
                return PartialView("_POPMCDetail", lst.ToList());
            }
            else
            {
                return View("_EmptyView");
            }

        }
        public ActionResult GetStockLedgerDetail(string ProjectId, string ItemGroupId, string ItemId)
        {
            List<StockLedgerDetail> result = new List<StockLedgerDetail>();
            result = objmsps.GetStockLedgerDetail(ProjectId, ItemGroupId, ItemId);
            Session["StockProId"] = ProjectId;
            Session["StockItemGrId"] = ItemGroupId;
            Session["StockItemId"] = ItemId;
            decimal? receieveQtyTotal = 0;
            decimal? issueQtyTotal = 0;
            if (result != null)
            {
                receieveQtyTotal = result.Sum(x => x.ReceiveQty);
                issueQtyTotal = result.Sum(x => x.Issue_Qty);
            }

            ViewBag.ReceieveQtyTotal = Session["ReceiveQtyTotal"] = receieveQtyTotal;
            ViewBag.IssueQtyTotal = Session["IssueQtyTotal"] = issueQtyTotal;
            if (result != null)
            {
                Session["KeepLedger"] = result;
                if(result.Count>0)
                    ViewBag.Unit = result[0].UnitName;

                return PartialView("_StockLedgerDetail", result.ToList());
            }
            else
            {
                return View("_EmptyView");
            }

        }


        public void GetExcelStockLedgerDetail()
        {
            List<StockLedgerDetail> allCust = (List<StockLedgerDetail>)Session["KeepLedger"];
            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);


            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("Datee", header: "Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Datee)),
                       grid1.Column("Particulars", header: "Vendor Name"),
                       //grid1.Column("IssueTo", header: "Issue To", style: "alignC"),
                       grid1.Column("MRN", header: "MRN", style: "alignC"),
                       grid1.Column("IssueNo", header: "Issue No."),
                       grid1.Column("ReceiveQty", header: "Receive Qty.", style: "alignR"),
                       grid1.Column("Issue_Qty", header: "Issue Qty.", style: "alignR"),
                       grid1.Column("BalanceQty", header: "Balance Qty.", format: (item) => string.Format("{0:0.00}", item.BalanceQty), style: "alignR"),
                       grid1.Column("Receive_Rate", header: "Transaction Rate"),
                       grid1.Column("TransactionAmount", header: "Transaction Amount", style: "alignR"),
                       grid1.Column("NetAMount", header: "Balance Amount", format: (item) => string.Format("{0:0.00}", item.NetAMount), style: "alignR")
                       )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=StockLedgerReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridHtml);
            Response.End();
        }


        public FileStreamResult StockLedgerRecordPdf()
        {
            List<StockLedgerDetail> allCust = (List<StockLedgerDetail>)Session["KeepLedger"];
            var recdQtyTotal = Convert.ToDecimal(Session["ReceiveQtyTotal"]);
            var issueQtyTotal = Convert.ToDecimal(Session["IssueQtyTotal"]);
            string pid = Session["StockProId"].ToString(); string GrpId = Session["StockItemGrId"].ToString(); string ItmId = Session["StockItemId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();
            var ItemGrpName = objmms.tblItemGroupMasters.Where(x => x.ItemGroupID == GrpId).Select(x => x.GroupName).First();
            var ItemName = objmms.tblItemMasters.Where(x => x.ItemID == ItmId).Select(x => x.ItemName).First();


            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("Datee", header: "Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Datee)),
                       grid1.Column("Particulars", header: "Vendor Name"),
                       //grid1.Column("IssueTo", header: "Issue To", style: "alignC"),
                       grid1.Column("MRN", header: "MRN", style: "alignC"),
                       grid1.Column("IssueNo", header: "Issue No."),
                       grid1.Column("ReceiveQty", header: "Receive Qty.", style: "alignR"),
                       grid1.Column("Issue_Qty", header: "Issue Qty.", style: "alignR"),
                       grid1.Column("BalanceQty", header: "Balance Qty.", format: (item) => string.Format("{0:0.00}", item.BalanceQty), style: "alignR"),
                       grid1.Column("Receive_Rate", header: "Transaction Rate", style: "alignR"),
                       grid1.Column("TransactionAmount", header: "Transaction Amount", style: "alignR"),
                       grid1.Column("NetAMount", header: "Balance Amount", format: (item) => string.Format("{0:0.00}", item.NetAMount), style: "alignR")
                       )
                    ).ToString();



            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Stock Ledger Report";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div>Item Group Name: " + ItemGrpName + " Item Name: " + ItemName + " </div><br/>";

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
            DataString.Append($"<div style='text-align:right'><strong>Unit:</strong>{allCust[0].UnitName}</div>");
            DataString.Append(gridHtml);
            writer.PageEvent = new HeaderFooter("ACIL");
            using (StringReader sr = new StringReader(DataString.ToString()))
            {

                var styleSheet = new iTextSharp.text.html.simpleparser.StyleSheet();

                styleSheet.LoadTagStyle("table", "border", "1");
                styleSheet.LoadTagStyle("table", "size", "12px");
                styleSheet.LoadTagStyle("table td", "text-align", "center");
                styleSheet.LoadTagStyle("thead, tfoot", "display", "table-row-group");
                styleSheet.LoadStyle("alignR", "align", "right");
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
            string totalTable = $"<table width='100%'><tr><td colspan='5' align='right'>Total</td><td align='right'>{recdQtyTotal}</td><td align='right'>{issueQtyTotal}</td><td colspan='4'></td></tr></table>";
            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            styleSheet1.LoadTagStyle("table", "float", "right");
            styleSheet1.LoadTagStyle("table", "size", "16px");
            styleSheet1.LoadTagStyle("table", "width", "472");
            //styleSheet1.LoadTagStyle("table th", "width", "80");
            styleSheet1.LoadStyle("alignR", "align", "right");
            styleSheet1.LoadStyle("wth", "width", "30");
            List<IElement> element = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr1, styleSheet1);

            foreach (IElement el in element)
            {
                document.Add(el);
            }


            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }


        public JsonResult GetPIByProjectId(string ProjectId)
        {
            string J = string.Empty;
            var PIList = objmms.PurchaseRequisitionMasters.Where(x => x.ProjectNo == ProjectId).ToList().OrderBy(k => k.UId).Select(p => new SelectListItem { Text = p.PurRequisitionNo, Value = p.UId.ToString() }).ToList();
            J = PIList.ToJSON();
            return Json(J, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPONUmberOnly(string ProjectId)
        {
            string J = string.Empty;
            var PONoList = objmms.TbIndentPurchaseOrder_GST.Where(x => x.ProjectNo == ProjectId).ToList().Select(p => new SelectListItem { Text = p.PurchaseOrderNo, Value = p.UId.ToString() }).OrderBy(k => k.Text).ToList();
            J = PONoList.ToJSON();
            return Json(J, JsonRequestBehavior.AllowGet);
        }


        public FileStreamResult POPMCInPdf()
        {
            List<POPMCDetail> allCust = (List<POPMCDetail>)Session["KeepPOPOMC"];

            string fd, tdate, fd1, td2;
            fd1 = Session["fdatePOPMC"].ToString();
            td2 = Session["tdatePOPMC"].ToString();
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


            string pid = Session["ProIdPOPMC"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();



            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("SupplierNo", header: "Name Of Vendor"),
                       grid1.Column("PurchaseOrderNo", header: "Purchase Order No."),
                       grid1.Column("PurchaseOrderDate", header: "Purchase Order Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.PurchaseOrderDate)),
                       grid1.Column("CreatedDate", header: "Date Forwarding to PMC", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.CreatedDate)),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("Item_description", header: "Item description"),
                        grid1.Column("UnitName", header: "Unit Name"),
                         grid1.Column("Qty", header: "Qty"),
                         grid1.Column("Rate", header: "Rate"),
                         grid1.Column("Amount", header: "Amount"),
                          grid1.Column("DateofVettingOfPMC", header: "Date of Vetting Of PMC"),
                           grid1.Column("Status", header: "Agreed")

                       )
                    ).ToString();

            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Details of Purchase Indent vis a vis Purchase order";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>From Date: " + fd + " To Date: " + tdate + " </div><br/>";

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
            List<StockLedgerDetail> allCust = (List<StockLedgerDetail>)Session["KeepLedger"];
            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);


            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("Datee", header: "Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Datee)),
                       grid1.Column("Particulars", header: "Vendor Name"),
                       //grid1.Column("IssueTo", header: "Issue To", style: "alignC"),
                       grid1.Column("MRN", header: "MRN", style: "alignC"),
                       grid1.Column("IssueNo", header: "Issue No."),
                       grid1.Column("ReceiveQty", header: "Receive Qty.", style: "alignR"),
                       grid1.Column("Issue_Qty", header: "Issue Qty.", style: "alignR"),
                       grid1.Column("BalanceQty", header: "Balance Qty.", format: (item) => string.Format("{0:0.00}", item.BalanceQty), style: "alignR"),
                       grid1.Column("Receive_Rate", header: "Transaction Rate"),
                       grid1.Column("TransactionAmount", header: "Transaction Amount", style: "alignR"),
                       grid1.Column("NetAMount", header: "Balance Amount", format: (item) => string.Format("{0:0.00}", item.NetAMount), style: "alignR")
                       )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=StockLedgerReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridHtml);
            Response.End();
        }

 

        public ActionResult PIvsPOandReceivedQty()
        {
            if (Session["EmpId"] == null)
                return RedirectToAction("Login","MyAccount");

            ViewBag.EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            return View();
        }

        public ActionResult GetPIvsPOandQtyData(string ProjectId, string PI, string PO, string VendorId, string ItemGroupId, string ItemId, string fromdate, string todate)
        {

            List<PIDetailWithPoAndQtyViewModel> result = new List<PIDetailWithPoAndQtyViewModel>();
            PI=PI == "Select P.I" ? "" : PI;

            Session["fdate"] = fromdate;
            Session["tdate"] = todate;
            Session["ProId"] = ProjectId;

            if (fromdate != "" && todate != "")
            {
                // DateTime date1 = DateTime.ParseExact(fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                // DateTime date2 = DateTime.ParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //// result = objmsps.GetPIDetailWithPOAndQty(ProjectId, PI, fromdate, todate);
                // result = objmsps.GetPIDetailWithPOAndQty(ProjectId, PI, PO, VendorId, ItemGroupId, ItemId, date1.ToString("yyyy-MM-dd"), date2.ToString("yyyy-MM-dd"));
                //above code replace with this on 10_07_20
                result = objmsps.GetPIDetailWithPOAndQty(ProjectId, PI, PO, VendorId, ItemGroupId, ItemId, fromdate, todate);
            }
            else
            {
                //result = objmsps.GetPIDetailWithPOAndQty(ProjectId, PI, fromdate, todate);
                result = objmsps.GetPIDetailWithPOAndQty(ProjectId, PI, PO, VendorId, ItemGroupId, ItemId, "", "");
            }

            if (result != null)
            {
                Session["keepPIDetailDataWithQty"] = result;

                ViewBag.TotalPIQty = decimal.Round(result.Sum(x => x.PIQty).Value, 3);
                ViewBag.TotalApprovedPIQty = decimal.Round(result.Sum(x => x.ApprovePIQty).Value, 3);
                ViewBag.TotalPOQty = decimal.Round(result.Sum(x => x.POQty).Value, 3);
                ViewBag.TotalPOAmount = decimal.Round(result.Sum(x => x.Amount).Value, 2);
                ViewBag.TotalRecdQty = decimal.Round(result.Sum(x => x.ReceivedQty).Value, 3);
                ViewBag.TotalBalancedQty = decimal.Round(result.Sum(x => x.BalanceQty).Value, 3);

                return PartialView("_PIvsPOandQtyGrid", result.ToList());
            }
            else
            {
                return View("_EmptyView");
            }
        }

        public ActionResult PIvsPOandReceivedQtyPdf()
        {
            List<PIDetailWithPoAndQtyViewModel> allCust = (List<PIDetailWithPoAndQtyViewModel>)Session["keepPIDetailDataWithQty"];

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

            decimal? TotalPIQty = 0,TotalApprovedPIQty=0,TotalPOQty=0,TotalPOAmount=0,TotalRecdQty=0,TotalBalancedQty=0;
            if (allCust != null)
            {
                TotalPIQty = decimal.Round(allCust.Sum(x => x.PIQty).Value, 3);
                TotalApprovedPIQty = decimal.Round(allCust.Sum(x => x.ApprovePIQty).Value, 3);
                TotalPOQty = decimal.Round(allCust.Sum(x => x.POQty).Value, 3);
                TotalPOAmount = decimal.Round(allCust.Sum(x => x.Amount).Value, 2);
                TotalRecdQty = decimal.Round(allCust.Sum(x => x.ReceivedQty).Value, 3);
                TotalBalancedQty = decimal.Round(allCust.Sum(x => x.BalanceQty).Value, 3);
            }

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("PurRequisitionNo", header: "PI No."),
                     grid1.Column("PIDate", header: "PI Date"),
                       grid1.Column("ItemGroup", header: "Item Group"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("Unit", header: "Unit"),
                       grid1.Column("PIQty", header: "PI Qty."),
                       grid1.Column("ApprovePIQty", header: "Approved PI Qty."),
                       grid1.Column("PONo", header: "PO No."),
                       grid1.Column("PODate", header: "PO Date"),
                       grid1.Column("POQty", header: "PO Qty"),
                       grid1.Column("Rate", header: "Rate"),
                       grid1.Column("Amount", header: "Amount"),
                       grid1.Column("MRNNo", header: "MRN No"),
                       grid1.Column("MRNDate", header: "MRN Date"),
                       grid1.Column("ReceivedQty", header: "Received Qty"),
                       grid1.Column("BalanceQty", header: "Balance Qty")
                       )
                    ).ToString();

            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Statement for Purchase Indent vs Purchase Order and Received Qty.";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>From Date: " + fd + " To Date: " + tdate + " </div><br/>";

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

            string totalTable = "<table border-top:0; width:100%;'><tr><td colspan='7' align='right'><strong>Total</strong></td><td>" + TotalPIQty + "</td><td>"+TotalApprovedPIQty+ "</td><td></td><td></td><td>" + TotalPOQty+ "</td><td></td><td>" + TotalPOAmount+ "</td><td></td><td></td><td>" + TotalRecdQty+"</td><td>"+TotalBalancedQty+"</td></tr></table>";
            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            styleSheet1.LoadTagStyle("table", "size", "16px");

            List<IElement> element = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr1, styleSheet1);

            foreach (IElement el in element)
            {
                document.Add(el);
            }

            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

        public void PIvsPOandReceivedQtyExcel()
        {
            List<PIDetailWithPoAndQtyViewModel> allCust = (List<PIDetailWithPoAndQtyViewModel>)Session["keepPIDetailDataWithQty"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("PurRequisitionNo", header: "PI No."),
                     grid1.Column("PIDate", header: "PI Date"),
                       grid1.Column("ItemGroup", header: "Item Group"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("Unit", header: "Unit"),
                       grid1.Column("PIQty", header: "PI Qty."),
                       grid1.Column("ApprovePIQty", header: "Approved PI Qty."),
                       grid1.Column("PONo", header: "PO No."),
                       grid1.Column("PODate", header: "PO Date"),
                       grid1.Column("POQty", header: "PO Qty"),
                       grid1.Column("Rate", header: "Rate"),
                       grid1.Column("Amount", header: "Amount"),
                       grid1.Column("MRNNo", header: "MRN No"),
                       grid1.Column("MRNDate", header: "MRN Date"),
                       grid1.Column("ReceivedQty", header: "Received Qty"),
                       grid1.Column("BalanceQty", header: "Balance Qty")
                       )
                    ).ToString();

            GetExcel(gridHtml, "PIvsPOandReceivedQty");
        }



        public JsonResult GetPOForProjects(string ProjectId)
        {
            var poList = objmms.TbIndentPurchaseOrder_GST.Where(x => x.ProjectNo == ProjectId && x.Status == "Approved").OrderByDescending(x=>x.CreatedDate).Select(x => new SelectListItem { Text = x.PurchaseOrderNo, Value = x.PurchaseOrderNo }).ToList();
            return Json(poList.ToJSON(), JsonRequestBehavior.AllowGet);
        }



        public ActionResult POMaterialStatus()
        {
            if (Session["EmpId"] == null)
                return RedirectToAction("Login", "MyAccount");

            ViewBag.EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            return View();
        }

        public ActionResult GetPOMaterialStatus(string ProjectId, string PO, string fromdate, string todate)
        {
            List<PIDetailWithPoAndQtyViewModel> result = new List<PIDetailWithPoAndQtyViewModel>();
            

            result = objmsps.GetPODataWithQty(ProjectId, PO, fromdate, todate);
            Session["fdate"] = fromdate;
            Session["tdate"] = todate;
            Session["ProId"] = ProjectId;
            Session["PO"] = PO;

            if (result != null)
            {
                Session["keepPOMaterialStatus"] = result;
                ViewBag.TotalPOQty = decimal.Round(result.Sum(x => x.POQty).Value, 3);
                ViewBag.TotalPOAmount = decimal.Round(result.Sum(x => x.Amount).Value, 2);
                ViewBag.TotalReceivedQty = decimal.Round(result.Sum(x => x.ReceivedQty).Value, 3);
                ViewBag.TotalReceivedAmount = decimal.Round(result.Sum(x => x.ReceiveAmount).Value, 2);
                ViewBag.TotalBalanceQty = decimal.Round(result.Sum(x => x.BalanceQty).Value, 3);
                ViewBag.TotalBalanceAmount = decimal.Round(result.Sum(x => x.BalanceAmount).Value, 2);

                return PartialView("_POMaterialStatusGrid", result.ToList());
            }
            else
            {
                return View("_EmptyView");
            }
        }

        public ActionResult POMaterialStatusPdf()
        {
            List<PIDetailWithPoAndQtyViewModel> result = (List<PIDetailWithPoAndQtyViewModel>)Session["keepPOMaterialStatus"];

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

            decimal? TotalPOQty = 0,TotalPOAmount=0,TotalReceivedQty=0,TotalReceivedAmount=0,TotalBalanceQty=0,TotalBalanceAmount=0;
            if (result != null)
            {
                TotalPOQty = decimal.Round(result.Sum(x => x.POQty).Value, 3);
                TotalPOAmount = decimal.Round(result.Sum(x => x.Amount).Value, 2);
                TotalReceivedQty = decimal.Round(result.Sum(x => x.ReceivedQty).Value, 3);
                TotalReceivedAmount = decimal.Round(result.Sum(x => x.ReceiveAmount).Value, 2);
                TotalBalanceQty = decimal.Round(result.Sum(x => x.BalanceQty).Value, 3);
                TotalBalanceAmount = decimal.Round(result.Sum(x => x.BalanceAmount).Value, 2);
            }

            WebGrid grid1 = new WebGrid(source: result, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("PONo", header: "PO No"),
                       grid1.Column("PODate", header: "PO Date"),
                       grid1.Column("VendorName", header: "Name of Vendor"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemGroup", header: "Item Group"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("Unit", header: "Unit"),
                       grid1.Column("POQty", header: "PO Qty"),
                       grid1.Column("Rate", header: "Rate"),
                       grid1.Column("Amount", header: "Amount"),
                       grid1.Column("MRNNo", header: "MRN No"),
                       grid1.Column("MRNDate", header: "MRN Date"),
                       grid1.Column("ReceivedQty", header: "Received Qty"),
                       grid1.Column("ReceiveAmount", header: "Received Amount"),
                       grid1.Column("BalanceQty", header: "Balance Qty"),
                       grid1.Column("BalanceAmount", header: "Balance Amount")
                       )
                    ).ToString();


            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Material Status Against Purchase Order";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>From Date: " + fd + " To Date: " + tdate + " </div><br/>";

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

            string totalTable = "<table border-top:0; width:100%;'><tr><td colspan='8' align='right'><strong>Total</strong></td><td>" + TotalPOQty + "</td><td></td><td>"+TotalPOAmount+ "</td><td></td><td></td><td>"+TotalReceivedQty+ "</td><td>"+TotalReceivedAmount+ "</td><td>"+TotalBalanceQty+ "</td><td>"+TotalBalanceAmount+"</td></tr></table>";
            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            styleSheet1.LoadTagStyle("table", "size", "16px");

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

        public void POMaterialStatusExcel()
        {
            List<PIDetailWithPoAndQtyViewModel> result = (List<PIDetailWithPoAndQtyViewModel>)Session["keepPOMaterialStatus"];

            WebGrid grid1 = new WebGrid(source: result, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("PONo", header: "PO No"),
                       grid1.Column("PODate", header: "PO Date"),
                       grid1.Column("VendorName", header: "Name of Vendor"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemGroup", header: "Item Group"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("Unit", header: "Unit"),
                       grid1.Column("POQty", header: "PO Qty"),
                       grid1.Column("Rate", header: "Rate"),
                       grid1.Column("Amount", header: "Amount"),
                       grid1.Column("MRNNo", header: "MRN No"),
                       grid1.Column("MRNDate", header: "MRN Date"),
                       grid1.Column("ReceivedQty", header: "Received Qty"),
                       grid1.Column("ReceiveAmount", header: "Received Amount"),
                       grid1.Column("BalanceQty", header: "Balance Qty"),
                       grid1.Column("BalanceAmount", header: "Balance Amount")
                       )
                    ).ToString();

            GetExcel(gridHtml, "POMaterialStatus");
        }


        public ActionResult DebitNoteSummary()
        {
            if (Session["EmpId"] == null)
                return RedirectToAction("Login", "MyAccount");

            ViewBag.EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            return View();
        }

        public ActionResult GetDebitNoteSummaryData(string ProjectId, int DebitNoteType, string fromdate, string todate)
        {
            List<DebitNoteSummaryViewModel> result = new List<DebitNoteSummaryViewModel>();

            result = objmsps.GetDebitNoteSummaryData(ProjectId, DebitNoteType, fromdate, todate);
            Session["fdate"] = fromdate;
            Session["tdate"] = todate;
            Session["ProId"] = ProjectId;
            //Session["DebitNoteType"] = DebitNoteType;
            decimal? total = 0;
            if (result != null)
            {
                Session["keepDebitNoteSummaryData"] = result;
                total = decimal.Round(result.Sum(x => x.Amount).Value, 2);
                ViewBag.Total = total;
                return PartialView("_DebitNoteSummaryGrid", result.ToList());
            }
            else
            {
                return View("_EmptyView");
            }
        }

        public ActionResult DebitNoteSummaryPdf()
        {
            List<DebitNoteSummaryViewModel> allCust = (List<DebitNoteSummaryViewModel>)Session["keepDebitNoteSummaryData"];

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

            decimal? total = 0;
            if (allCust != null)
            {
                total =decimal.Round(allCust.Sum(x => x.Amount).Value,2);
            }

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                   grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                     grid1.Column("VendorPCName", header: "Name Of Vendor"),
                       grid1.Column("DebitNoteNo", header: "Debit Note No"),
                       grid1.Column("DebitNoteDate", header: "Debit Note Date"),
                       grid1.Column("Amount", header: "Debit Note Amount")
                       )
                    ).ToString();

            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Summary of Debit Note";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>From Date: " + fd + " To Date: " + tdate + " </div><br/>";

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

            string totalTable = "<table border-top:0; width:100%;'><tr><td colspan='4' align='right'><strong>Total</strong></td><td>" + total + "</td></tr></table>";
            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            styleSheet1.LoadTagStyle("table", "size", "16px");

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

        public void DebitNoteSummaryExcel()
        {
            List<DebitNoteSummaryViewModel> allCust = (List<DebitNoteSummaryViewModel>)Session["keepDebitNoteSummaryData"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridHtml = grid1.GetHtml(
                     columns: grid1.Columns(
                    grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                      grid1.Column("VendorPCName", header: "Name Of Vendor"),
                        grid1.Column("DebitNoteNo", header: "Debit Note No"),
                        grid1.Column("DebitNoteDate", header: "Debit Note Date"),
                        grid1.Column("Amount", header: "Debit Note Amount")
                        )
                     ).ToString();

            GetExcel(gridHtml, "DebitNoteSummary");
        }


        public void GetExcel(string GridHtml,string FileName)
        {
            
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename="+FileName+".xls");
            Response.ContentType = "application/excel";
            Response.Write(GridHtml);
            Response.End();
        }
    }
}