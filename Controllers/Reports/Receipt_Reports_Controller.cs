using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MMS.Models;
using MMS.DAL;
using System.Data;
using System.Data.Entity;
using System.Linq.Dynamic;
using System.Web.Helpers;
using System.IO;
using iTextSharp.text;
using System.Text;
using iTextSharp.text.pdf;

namespace MMS.Controllers.Reports
{
    public class Receipt_Reports_Controller : Controller
    {
        // GET: Receipt
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        string ProjectId;

        public object PdfWriter { get; private set; }

        [Authorize]
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            string ProjectId = Session["ProId"].ToString();

            Receipt_Report_Model rrm = new Receipt_Report_Model();

            ViewBag.item_Group_name = new SelectList(objmms.tblItemGroupMasters, "ItemGroupID", "GroupName").OrderBy(k => k.Text);
            //ViewBag.Vendor_Name = new SelectList(objmms.GateEntryChilds, "VendorNo", "Vendor").OrderBy(k => k.Text);

            //code for bind vendor name accoding project
            var a = objmms.GateEntryChilds.Where(b => b.ProjectNo == ProjectId).OrderBy(c => c.Vendor).ToList();
            var t = a.Distinct<GateEntryChild>().Select(x => new SelectListItem
            {
                Value = x.VendorNo ?.ToString().Trim(),
                Text = x.Vendor ?.ToString().Trim()             
            });
            ViewBag.Vendor_Name = t;

             var data_from_table = (from e in objmms.GateEntryChilds
                                    join f in objmms.GateEntries on e.MUId equals f.UId
                                   where e.ProjectNo == ProjectId orderby e.GateEntryNo
                                   select new Receipt_Report_Model
                      {
                          GateEntryNo = e.GateEntryNo,
                          Vendor = e.Vendor,
                          BillNo="",
                          StatusTypeNo = e.StatusTypeNo,
                          ItemGroup = e.ItemGroup,
                          Item = e.Item,
                          ReceiveQty = e.ReceiveQty,
                          Rate= e.Rate,
                          Amount=e.Amount,
                          Date=e.Date,
                          Challan=f.ChallanNo
                      }).OrderByDescending(k=>k.Date).Take(200).ToList();

            rrm.Receipt_Report = data_from_table;
            ViewBag.displySumqty = data_from_table.Sum(k => k.ReceiveQty);
            ViewBag.displySumamt = data_from_table.Sum(k => k.Amount);
            TempData["printexcel"] = data_from_table;
            Session["keepdata"] = data_from_table;
            TempData.Keep("printexcel");
            return View("Index", rrm);
        }

        
        public ActionResult Receipt_Report_Search(string Item_Group_Name, string Vendors_Name, string FromDate, string ToDate)
        {
            string ProjectId = Session["ProId"].ToString();
            List<Receipt_Report_Model> rrm = null;
            var data_rrm = rrm;
            
            
           if (Item_Group_Name.Trim() != null && Item_Group_Name != "" && Vendors_Name.Trim() != null && Vendors_Name != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.GateEntryChilds
                            join f in objmms.GateEntries on e.MUId equals f.UId
                            where e.ItemGroupNo == Item_Group_Name && e.VendorNo == Vendors_Name && e.ProjectNo == ProjectId && e.Date >= date1 && e.Date <= date2
                            select new Receipt_Report_Model
                            {
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                StatusTypeNo = e.StatusTypeNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Rate = e.Rate,
                                Amount = e.Amount,
                                Date = e.Date,
                                Challan = f.ChallanNo
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else if (Vendors_Name.Trim() != null && Vendors_Name != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.GateEntryChilds
                            join f in objmms.GateEntries on e.MUId equals f.UId
                            where e.VendorNo == Vendors_Name && e.ProjectNo == ProjectId && e.Date >= date1 && e.Date <= date2
                            select new Receipt_Report_Model
                            {
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                StatusTypeNo = e.StatusTypeNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Rate = e.Rate,
                                Amount = e.Amount,
                                Date = e.Date,
                                Challan = f.ChallanNo
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Item_Group_Name.Trim() != null && Item_Group_Name != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.GateEntryChilds
                            join f in objmms.GateEntries on e.MUId equals f.UId
                            where e.ItemGroupNo == Item_Group_Name && e.ProjectNo == ProjectId && e.Date >= date1 && e.Date <= date2
                            select new Receipt_Report_Model
                            {
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                StatusTypeNo = e.StatusTypeNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Rate = e.Rate,
                                Amount = e.Amount,
                                Date = e.Date,
                                Challan = f.ChallanNo
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Item_Group_Name == "" && Vendors_Name == "" && FromDate == "" && ToDate == "")
            {
                data_rrm = (from e in objmms.GateEntryChilds
                            join f in objmms.GateEntries on e.MUId equals f.UId
                            where e.ProjectNo == ProjectId
                                       orderby e.GateEntryNo
                                       select new Receipt_Report_Model
                                       {
                                           GateEntryNo = e.GateEntryNo,
                                           Vendor = e.Vendor,
                                           BillNo = "",
                                           StatusTypeNo = e.StatusTypeNo,
                                           ItemGroup = e.ItemGroup,
                                           Item = e.Item,
                                           ReceiveQty = e.ReceiveQty,
                                           Rate = e.Rate,
                                           Amount = e.Amount,
                                           Date = e.Date,
                                           Challan=f.ChallanNo
                                       }).OrderByDescending(k => k.Date).Take(200).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Item_Group_Name.Trim() == null && Vendors_Name.Trim() == null)
            {
                data_rrm = (from e in objmms.GateEntryChilds
                            join f in objmms.GateEntries on e.MUId equals f.UId
                            where e.ProjectNo == ProjectId
                            orderby e.GateEntryNo
                            select new Receipt_Report_Model
                            {
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                StatusTypeNo = e.StatusTypeNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Rate = e.Rate,
                                Amount = e.Amount,
                                Date = e.Date,
                                Challan=f.ChallanNo
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else if (Item_Group_Name.Trim() != null && Vendors_Name.Trim() != null && Item_Group_Name.Trim() != "" && Vendors_Name.Trim() != "")
            {
                data_rrm = (from e in objmms.GateEntryChilds
                            join f in objmms.GateEntries on e.MUId equals f.UId
                            where e.VendorNo == Vendors_Name && e.ItemGroupNo == Item_Group_Name && e.ProjectNo == ProjectId
                            select new Receipt_Report_Model
                            {
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                StatusTypeNo = e.StatusTypeNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Rate = e.Rate,
                                Amount = e.Amount,
                                Date = e.Date,
                                Challan=f.ChallanNo
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else if (Item_Group_Name.Trim() != null && Item_Group_Name != "")
            {
                data_rrm = (from e in objmms.GateEntryChilds
                            join f in objmms.GateEntries on e.MUId equals f.UId
                            where e.ItemGroupNo == Item_Group_Name && e.ProjectNo == ProjectId
                            select new Receipt_Report_Model
                            {
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                StatusTypeNo = e.StatusTypeNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Rate = e.Rate,
                                Amount = e.Amount,
                                Date = e.Date,
                                Challan=f.ChallanNo
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Vendors_Name.Trim() != null && Vendors_Name != "")
            {
                data_rrm = (from e in objmms.GateEntryChilds
                            join f in objmms.GateEntries on e.MUId equals f.UId
                            where e.VendorNo == Vendors_Name && e.ProjectNo == ProjectId
                            select new Receipt_Report_Model
                            {
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                StatusTypeNo = e.StatusTypeNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Rate = e.Rate,
                                Amount = e.Amount,
                                Date = e.Date,
                                Challan=f.ChallanNo
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);
                
                data_rrm = (from e in objmms.GateEntryChilds
                            join f in objmms.GateEntries on e.MUId equals f.UId
                            where e.ProjectNo == ProjectId && e.Date >= date1 && e.Date <= date2
                            select new Receipt_Report_Model
                            {
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                StatusTypeNo = e.StatusTypeNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Rate = e.Rate,
                                Amount = e.Amount,
                                Date = e.Date,
                                Challan=f.ChallanNo
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else {

            }
            return PartialView("_Receipt_Report_Partial", data_rrm);
        }

        public void GetExcel()
        {
            List<Receipt_Report_Model> allCust = (List<Receipt_Report_Model>)Session["keepdata"];
           
            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                        grid1.Column("Date", header: "Date"),
                        grid1.Column("Challan", header: "Challan No"),
                         grid1.Column("GateEntryNo", header: "GRN No."),
                       grid1.Column("Vendor", header: "Name of Vendor"),
                       //grid1.Column("BillNo", header: "Bill No."),                     
                       grid1.Column("ItemGroup", header: "Item Group Name"),
                       grid1.Column("Item", header: "Item Name"),
                       grid1.Column("StatusTypeNo", header: "PO No. "),
                       grid1.Column("ReceiveQty", header: "Quantity"),
                       grid1.Column("Rate", header: "Rate"),
                       grid1.Column("Amount", header: "Amount")
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=ReceiptReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }
        public FileStreamResult Receipt_ReportPdf()
        {
            List<Receipt_Report_Model> allCust = (List<Receipt_Report_Model>)Session["keepdata"];

            string pid = Session["ProId"].ToString();

            var a = objmms.GateEntries.Where(b => b.ProjectNo == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();

            

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                        grid1.Column("Date", header: "Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Date)),
                        grid1.Column("Challan", header: "Challan No"),
                         grid1.Column("GateEntryNo", header: "GRN No."),
                       grid1.Column("Vendor", header: "Vendor Name"),
                       //grid1.Column("BillNo", header: "Bill No."),                     
                       grid1.Column("ItemGroup", header: "Item Group Name"),
                       grid1.Column("Item", header: "Item Name"),
                       grid1.Column("StatusTypeNo", header: "PO No. "),
                       grid1.Column("ReceiveQty", header: "Qty.",style:"right"),
                       grid1.Column("Rate", header: "Rate",style:"right"),
                       grid1.Column("Amount", header: "Amt.",style:"right")
                        )
                    ).ToString();

    
            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Receipt Report";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
         
            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 50, 50, 30, 50);

            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, output);
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
            return new FileStreamResult(output, "application/pdf");
        }
    }
}