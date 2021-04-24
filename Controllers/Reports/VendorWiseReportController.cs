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
    public class VendorWiseReportController : Controller
    {
        MMSEntities objmms = new MMSEntities();
        MSP_Model objmsps = new MSP_Model();
        FactoryManager m = new FactoryManager();
        [Authorize]
        // GET: VendorWiseReport
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;
            #region
            //  ViewBag.Prjid = Session["ProjectssId"].ToString();

            // var Prj = Session["ProjectssId"].ToString();

            //var ven = objmms.tblVendorMasters.Where(x => x.PRJID.Contains(Prj)).OrderBy(a => a.Name).ToList();
            //var t = ven.Distinct<DAL.tblVendorMaster>().Select(x => new SelectListItem
            //{
            //    Value = x.VendorID.ToString(),
            //    Text = x.Name.ToString()
            //});
            //ViewBag.Vendor = t;

            // List<GET_RECEIVED_STOCK> result = new List<GET_RECEIVED_STOCK>();
            //  result = objmsps.GetAllReceivedStock(Prj);



            //    var lst = result.Where(x => x.ReceiveDate == date1).OrderBy(x1 => x1.GroupName).ToList();
            //if (lst.Count > 0)
            //{
            //    Session["keepdata"] = lst;
            //    ViewBag.displyTotalSum = lst.Sum(k => k.ReceiveAmt);
            //    return PartialView("_PartialView_VendorWise", lst);
            //}
            //else
            //{
            //    return View();
            //}
            #endregion
            DateTime date1 = System.DateTime.Now;
            return View();
        }
        public ActionResult GetAllDatas(string PID, string fromdate, string todate, string Ven , string ItemId, string ItemGrpId)
        {
            Session["fdate"] = ""; Session["tdate"] = ""; Session["Ven"] = ""; Session["ProId"] = ""; Session["Itmid"] = ""; Session["ItmGrpid"] = "";
            
            Session["fdate"] = fromdate;
            Session["tdate"] = todate;
            Session["Ven"] = Ven;
            Session["ProId"] = PID;
            Session["Itmid"] = ItemId;
            Session["ItmGrpid"] = ItemGrpId;
            ViewBag.IsItemSelected = ItemId == "" ? false : true;
            if(fromdate != "" && todate != "")
            {
                fromdate = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                todate= DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            }
            List<GET_RECEIVED_STOCK> result = new List<GET_RECEIVED_STOCK>();
            result = objmsps.GetAllReceivedStock(PID,ItemGrpId,ItemId,Ven,fromdate,todate);

            if (result != null)
            {
                ViewBag.displyTotalSum = result.Sum(k => k.ReceiveAmt);
                
                if (ItemId != "")
                    ViewBag.TotalReceiveQty = result.Sum(k => k.ReceiveQty);
                else
                    ViewBag.TotalReceiveQty = 0;
            }
            else
            {
                ViewBag.TotalReceiveQty = 0;
                ViewBag.displyTotalSum = 0;
            }
            
            Session["keepdata"] = result;
           
            return PartialView("_PartialView_VendorWise", result);


        }
        public void GetExcel()
        {
            List<GET_RECEIVED_STOCK> allCust = (List<GET_RECEIVED_STOCK>)Session["keepdata"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                      grid1.Column("ReceiveDate", header: "Receipt Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.ReceiveDate)),
                       grid1.Column("GRNNo", header: "MRN No."),
                       grid1.Column("ChallanNo", header: "Challan No"),
                       grid1.Column("VehicleNo", header: "Vehicle No"),
                       grid1.Column("StatusTypeNo", header: "PO No."),
                       grid1.Column("Vendor", header: "Vendor"),
                       grid1.Column("GroupName", header: "Group"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("ReceiveQty", header: "Quantity", format: (item) => string.Format("{0:0.000}", item.ReceiveQty)),
                       grid1.Column("Rate", header: "Rate"),
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

            string fd, tdate, fd1, td2, vend, vend1 , ItemId , ItemgrpId; 
            fd1 = Session["fdate"].ToString();
            td2 = Session["tdate"].ToString();
            if (fd1 == "")
            {
                fd = "--";
            }
            else
            {
                fd = DMYToMDY(fd1);
            }
            if (td2 == "")
            {
                tdate = "--";
            }
            else
            {
                tdate = DMYToMDY(td2);
            }

            vend1 = Session["Ven"].ToString();
            if (vend1 == "")
            {
                vend = "--";
            }
            else
            {
                vend = objmms.tblVendorMasters.Where(c1 => c1.VendorID == vend1).ToList().Select(x => x.Name).FirstOrDefault();
            }
            ItemId = Session["Itmid"].ToString();
            ItemgrpId = Session["ItmGrpid"].ToString();

            if (ItemId == "" || ItemId == "0")
            {
                ItemId = "--";
            }
            else {
                ItemId = objmms.tblItemMasters.Where(it => it.ItemID == ItemId).FirstOrDefault().ItemName;
            }

            if (ItemgrpId == "" || ItemgrpId == "0")
            {
                ItemgrpId = "--";
            }
            else {
                ItemgrpId = objmms.tblItemGroupMasters.Where(Ig => Ig.ItemGroupID == ItemgrpId).FirstOrDefault().GroupName;
            }

            string pid = Session["ProId"].ToString();

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
                       grid1.Column("GRNNo", header: "MRN No."),
                       grid1.Column("ChallanNo", header: "Challan No"),
                       grid1.Column("VehicleNo", header: "Vehicle No"),
                       grid1.Column("StatusTypeNo", header: "PO No."),
                       grid1.Column("Vendor", header: "Vendor"),
                       grid1.Column("GroupName", header: "Group"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("ReceiveQty", header: "Quantity", format: (item) => string.Format("{0:0.00}", item.ReceiveQty),style: "right"),
                       grid1.Column("Rate", header: "Rate", format: (item) => string.Format("{0:0.00}", item.Rate), style: "right"),
                       grid1.Column("ReceiveAmt", header: "Amount", format: (item) => string.Format("{0:0.00}", item.ReceiveAmt), style: "right")
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
            string additionalData = "<div style='text-align:left'>From Date : " + fd + " To Date : " + tdate + " </div><div style='text-align:left'> Vendor : " + vend + " ,  Item Group Name : " + ItemgrpId + " ,   Item Name : " + ItemId + "    </div><br/>";

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
           
            string totalTable = "";
            if (Session["ITMId"] != null && Session["ITMId"].ToString() != "")
            {
                totalTable = "<table align='right' style='float:right;border-top:0; width:80px;'><tr><td><strong>Total Qty.</strong></td><td class='right'>" + allCust.Sum(x => x.ReceiveQty) + "</td><td><strong>Total Amt.</strong></td><td class='right'>" + totAmt + "</td></tr></table>";
            }
            else
            {
                totalTable = "<table align='right' style='float:right;border-top:0; width:40px;'><tr><td><strong>Total</strong></td><td class='right'>" + totAmt + "</td></tr></table>";
            }

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

        public JsonResult GetProjectByEmpId(string E = "0")
        {

            string j = null;
            if (E != "0" && E != "EMP0000001" && E != "")
            {
                string PrjString = m.GetEmployeeMasterManager().FindPrj(E);


                var Prjlist = m.GetProjectMasterManager().FindListByString(PrjString);

                j = Prjlist.ToJSON();
            }
            else
            {
                var Prjlist = m.GetProjectMasterManager().FindListByString(null);

                j = Prjlist.ToJSON();
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindVendorPrjectWise(string ProjectId)
        {
            string j = string.Empty;
            var ven = objmms.tblVendorMasters.Where(x => x.PRJID.Contains(ProjectId)).OrderBy(a => a.Name).ToList();
            var t = ven.Select(x => new SelectListItem
            {
                Value = x.VendorID.ToString(),
                Text = x.Name.ToString()
            });

            j = t.ToJSON();
            return Json(j,JsonRequestBehavior.AllowGet);
        }


        public ActionResult SupplierReport()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;
            return View();
        }

        public ActionResult GetSuppllierGrid(string ProjectId, string VendorId)
        {
            MMS.Models.Procedure objs = new Procedure();
            List<MMS.ViewModels.SupplierReportViewModel> res = new List<ViewModels.SupplierReportViewModel>();
            res = objs.GetSupllierAllDetail(ProjectId, VendorId);
            Session["VId"] = VendorId;
            Session["Projid"] = ProjectId;
            if (res != null)
            {
                Session["SuppDet"] = res;
                return PartialView("_GetSupplierGrid", res);
            }
            else {
                return View("_EmptyView");
            }
        }


        public FileStreamResult SuppllierPdf()
        {
            List<MMS.ViewModels.SupplierReportViewModel> allCust = (List<MMS.ViewModels.SupplierReportViewModel>)Session["SuppDet"];

            string vend;
           

            vend = Session["VId"].ToString();
            if (vend == "")
            {
                vend = "-";
            }
            else
            {
                vend = objmms.tblVendorMasters.Where(c1 => c1.VendorID == vend).ToList().Select(x => x.Name).FirstOrDefault();
            }

            string pid = Session["Projid"].ToString();

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
                     grid1.Column("PONo", header: "PO No ", style: "my-classL"),
                        grid1.Column("PODate", header: "PO date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.PODate)),
                       grid1.Column("GRNNO", header: "GRN No.", style: "alignL"),
                       grid1.Column("MRNNo", header: "DMR/ MRN No", style: "alignL"),
                       grid1.Column("ChalanNo", header: "Inv/Bill/ Ch. No.", style: "alignL"),
                       grid1.Column("Chalandate", header: "Inv/Bill/ Ch. Date", style: "alignL", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Chalandate)),
                       grid1.Column("VehicleNo", header: "Vehicle No.", style: "alignL"),
                       grid1.Column("ItemNo", header: "Item Code", style: "alignC"),
                       grid1.Column("ItemDescription", header: "Item Description", style: "alignL"),
                        grid1.Column("UnitCode", header: "Unit", style: "alignL"),
                         grid1.Column("Qty", header: "Qty.", format: (item) => string.Format("{0:0.00}", item.Qty), style: "right"),
                          grid1.Column("Rate", header: "Net Rate (After Disc.)", format: (item) => string.Format("{0:0.00}", item.Rate), style: "right"),
                           grid1.Column("DiscountedAmt", header: "Discounted Amount", format: (item) => string.Format("{0:0.00}", item.DiscountedAmt), style: "right"),
                            grid1.Column("PandFAmt", header: "P&F Amount", format: (item) => string.Format("{0:0.00}", item.PandFAmt), style: "right"),
                             grid1.Column("CartageAmt", header: "Cartage  (Amt.)", format: (item) => string.Format("{0:0.00}", item.CartageAmt), style: "right"),
                             grid1.Column("InsuranceAmt", header: "Insurance Amt.", format: (item) => string.Format("{0:0.00}", item.InsuranceAmt), style: "right"),
                            grid1.Column("GSTType", header: "GST(%)", style: "my-classL"),
                          grid1.Column("GSTAmt", header: "GST Amt.", format: (item) => string.Format("{0:0.00}", item.GSTAmt), style: "right"),
                      //grid1.Column("ReceiveAmt", header: "Other if Any (Am.t).", format: (item) => string.Format("{0:0.00}", item.ReceiveAmt), style: "my-classR"),
                      grid1.Column("GrossAmt", header: " Total Amt.", format: (item) => string.Format("{0:0.00}", item.GrossAmt), style: "right")
                    
                       )
                    ).ToString();



            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Supplier Report";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'> Vendor: " + vend + " </div><br/>";

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
           
            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

    }
}