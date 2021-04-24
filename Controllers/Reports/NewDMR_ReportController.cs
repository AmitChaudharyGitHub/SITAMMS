using iTextSharp.text.pdf;
using iTextSharp.text;
using MMS.DAL;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Text;

namespace MMS.Controllers.Reports
{
    public class NewDMR_ReportController : Controller
    {
        // GET: NewDMR_Report
        string EmpId = string.Empty;
        private MMSEntities objmms = new MMSEntities();
        Procedure objP = new Procedure();

        public NewDMR_ReportController()
        {
            try
            {
                EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            
        }

        public ActionResult Index()
        {
            if (EmpId == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            ViewBag.EmpId = EmpId;
            List<SelectListItem> ObjList = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Select Type", Value = "0" },
                new SelectListItem { Text = "Opening", Value = "1" },
                new SelectListItem { Text = "Purchase", Value = "2" },
                new SelectListItem { Text = "Transfer", Value = "3" },
            };

            ViewBag.TypeDiff = ObjList;
            return View();
        }

        public ActionResult GetRSTData(string MRNNo)
        {
            var rstNo = objmms.GateEntries.FirstOrDefault(x => x.MRN_No_New == MRNNo).RSTNo;
            if (rstNo != null)
            {
                var rstData = objmms.tbl_wmntdata.FirstOrDefault(x => x.txn_no == rstNo);
                if (rstData != null && Session["ProId"].ToString() == "AHL0000083")
                {
                    return PartialView("_RSTData", rstData);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
                return HttpNotFound();

        }

        public ActionResult GetRSTDataByRSTNo(int RSTNo)
        {
            try
            {
                var rstData = objmms.tbl_wmntdata.FirstOrDefault(x => x.txn_no == RSTNo);
                if (rstData != null && Session["ProId"].ToString() == "AHL0000083")
                {
                    return PartialView("_RSTData", rstData);
                }
                else
                {
                    return PartialView("_RSTData", null);
                }
            }
            catch (Exception ex)
            {

                return HttpNotFound();
            }

        }


        public JsonResult GetMRN(int TypeId, string ProjectId)
        {
            string J = string.Empty;
            if (TypeId == 1) //opening
            {
                var res = objmms.GateEntries.Where(x => x.Status == "Opening" && x.ProjectNo == ProjectId).ToList().Select(p => new { Text = p.GateEntryNo, Value = p.GateEntryNo }).OrderByDescending(x => x.Value).ToList();
                J = res.ToJSON();
            }
            else if (TypeId == 2)//for purchase
            {
                var res = objmms.GateEntries.Where(x => (x.Status == "LP" || x.Status == "CH" || x.Status == "FC" || x.Status == "IPO") && x.ProjectNo == ProjectId).ToList().Select(p => new { Text = p.MRN_No_New, Value = p.MRN_No_New }).OrderByDescending(x => x.Value).OrderByDescending(x => x.Value).ToList();
                J = res.ToJSON();
            }
            else if (TypeId == 3)//for transfer
            {
                var res = objmms.GateEntries.Where(x => (x.Status == "ISP" ||x.Status=="OSP" || x.Status == "MTR") && x.ProjectNo == ProjectId).ToList().Select(p => new { Text = p.MRN_No_New, Value = p.MRN_No_New }).ToList();
                J = res.ToJSON();
            }

            return Json(J, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetDetailOnDMR(string ProjectId, string MRNNo)
        {
            List<GetDetailOnNewDMR> obj = null;
            obj = objP.GetDetailONNewMDR(ProjectId, MRNNo);
            if (obj != null && obj.Count>0)
            {
                string PoNo = obj[0].PONO;
                if (obj[0].PONO.Contains("TRN/OS"))
                {
                    var data = objmms.tblIntraStateTransferMasters.SingleOrDefault(x => x.IntraTransferNumber == PoNo);
                    obj[0].PODate = Convert.ToDateTime(data.TransferDate).ToString("dd MMM yyyy");
                    obj[0].Vendor = objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == data.ProjectId).ProjectName;                   

                }
                else if (obj[0].PONO.Contains("TRN/IS"))
                {
                    var data = objmms.tblInterStateTransferMasters.SingleOrDefault(x => x.InterTransferNumber == PoNo);
                    obj[0].PODate = Convert.ToDateTime(data.TransferDate).ToString("dd MMM yyyy");
                    obj[0].Vendor = objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == data.ProjectId).ProjectName;                   
                }
                Session["DMRPreDetail"] = obj;
                return Json(obj.ToJSON(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public ActionResult GetAllDatas(string PID, string MRN, int MRNType)
        {
            List<GetGridDMR> result = null;


            if (MRNType == 1 || MRNType == 2)
            {
                string PONo = objmms.GateEntries.Where(s => s.MRN_No_New == MRN).Select(s => s.StatusTypeNo).FirstOrDefault();
                bool? tcsActive = objmms.TbIndentPurchaseOrder_GST.Where(s => s.PurchaseOrderNo == PONo).Select(s => s.TCSActive).FirstOrDefault();
                ViewBag.TCSActive = tcsActive != null ? tcsActive.Value : false;
                result = objP.GetDMRGrid(PID, MRN);
                Session["NewDMR"] = result;
                return PartialView("_DMRGrid", result.ToList());
            }
            else if (MRNType == 3)
            {
                result = objP.GetDMRGridForTransfer(PID, MRN);
                Session["NewDMR"] = result;
                return PartialView("_DMRGrid", result.ToList());
            }
            else
            {
                return PartialView("_EmptyView");
            }
        }

        public FileStreamResult DMRStockPdf()
        {
            string MRNNO = string.Empty; string MRNDate = string.Empty;
            string GRNNo = string.Empty; string GRNDate = string.Empty;
            string BillNo = string.Empty; string BillDate = string.Empty;
            string PONO = string.Empty; string PODate = string.Empty;
            string VendorName = string.Empty; string ProjectName = string.Empty;
            string Address = string.Empty;
            string VehicleNo = string.Empty; string EWayBill = string.Empty; string EWayDate = string.Empty;
            List<GetGridDMR> allCust = (List<GetGridDMR>)Session["NewDMR"];
            List<GetDetailOnNewDMR> DMRPre = (List<GetDetailOnNewDMR>)Session["DMRPreDetail"];

            var mylst = from m in DMRPre
                        select new
                        {
                            ProjectName = objmms.tblProjectMasters.Where(x => x.PRJID == m.ProjectNo).FirstOrDefault().ProjectName,
                            MRNNO = m.MRN,
                            MRNDate = m.MRNCreatedDAte,
                            GRNNo = m.GateEntryNo,
                            GRNDate = m.GateEntryCreatedDate,
                            BillNo = m.BillNo,
                            BillDate = m.BillDate,
                            VendorName = m.Vendor,
                            Address = m.Address,
                            PONO = m.PONO,
                            PODate = m.PODate,
                            VehicleNo = m.VehicleNo,
                            EWayBill = m.EWayBill,
                            EWayDate = m.EWayDate
                        };

            MRNNO = mylst.Select(x => x.MRNNO).SingleOrDefault();
            MRNDate = mylst.Select(x => x.MRNDate).SingleOrDefault();
            GRNNo = mylst.Select(x => x.GRNNo).SingleOrDefault();
            GRNDate = mylst.Select(x => x.GRNDate).SingleOrDefault();
            BillNo = mylst.Select(x => x.BillNo).SingleOrDefault();
            BillDate = mylst.Select(x => x.BillDate).SingleOrDefault();
            VendorName = mylst.Select(x => x.VendorName).SingleOrDefault();
            ProjectName = mylst.Select(x => x.ProjectName).SingleOrDefault();
            Address = mylst.Select(x => x.Address).SingleOrDefault();
            PONO = mylst.Select(x => x.PONO).SingleOrDefault();
            PODate = mylst.Select(x => x.PODate).SingleOrDefault();
            VehicleNo = mylst.Select(x => x.VehicleNo).SingleOrDefault();
            EWayBill = mylst.Select(s => s.EWayBill).SingleOrDefault();
            EWayDate = mylst.Select(s => s.EWayDate).SingleOrDefault();
            bool? tcsActive = objmms.TbIndentPurchaseOrder_GST.Where(s => s.PurchaseOrderNo == PONO).Select(s => s.TCSActive).FirstOrDefault();

            var ss = from k in allCust
                     group k by k.ItemNo into g
                     select new
                     {
                         SUMDiscountAmt = g.Sum(o => o.Discount),
                         SumPanF = g.Sum(o => o.PandF),
                         SumCartage = g.Sum(o => o.CartageAmt),
                         SumInsurance = g.Sum(o => o.InsuranceAmt),
                         SumCGST = g.Sum(o => o.CGSTAmt),
                         SumSGSTAndUTGST = g.Sum(o => o.SGSTAndUTGSTAmt),
                         SumIGST = g.Sum(o => o.IGSTAmt),
                         SumGrossAmt = g.Sum(o => o.GrandTotal),
                         SumTCS = g.Sum(o => o.TCSAmt)
                     };

            decimal? SUMDiscountAmt = Math.Round((decimal)(ss.Sum(l => l.SUMDiscountAmt)), 2);
            decimal? SumPanF = Math.Round((decimal)(ss.Sum(l => l.SumPanF)), 2);
            decimal? SumCartage = Math.Round((decimal)(ss.Sum(l => l.SumCartage)), 2);
            decimal? SumInsurance = Math.Round((decimal)(ss.Sum(l => l.SumInsurance)), 2);
            decimal? SumCGST = Math.Round((decimal)(ss.Sum(l => l.SumCGST)), 2);
            decimal? SumSGSTAndUTGST = Math.Round((decimal)(ss.Sum(l => l.SumSGSTAndUTGST)), 2);
            decimal? SumIGST = Math.Round((decimal)(ss.Sum(l => l.SumIGST)), 2);
            decimal? SumGrossAmt = Math.Round((decimal)(ss.Sum(l => l.SumGrossAmt)), 2);

            decimal? sumTCS = Math.Round((decimal)(ss.Sum(l => l.SumTCS)), 2);


            #region




            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            //string gridHtml = grid1.GetHtml(
            //        columns: grid1.Columns(
            //       grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
            //       grid1.Column("GroupName", header: "Item Group", style: "my-classL"),
            //        grid1.Column("ItemNo", header: "Item Code", style: "my-classC"),
            //                grid1.Column("ItemName", header: "Item Description", style: "my-classL"),
            //                 grid1.Column("UnitCode", header: "Unit", style: "my-classL"),
            //                  grid1.Column("QTy", header: "Qty.", style: "right"),
            //                   grid1.Column("Rate", header: "Rate", style: "right"),
            //                   grid1.Column("DiscountPercentage", header: "Discount(%)", style: "right"),
            //                    grid1.Column("Discount", header: "Amount after discount", style: "right"),
            //                     grid1.Column("PandF", header: "P&F ", style: "right"),
            //                      grid1.Column("CartageAmt", header: "Cartage", style: "right"),
            //                       grid1.Column("InsuranceAmt", header: "Insurance", style: "right"),
            //                         grid1.Column("CGSTPer", header: "CGST(%)", style: "right"),
            //grid1.Column("SGSTAndUTGSTPer", header: "SGST/UTGST(%)", style: "right"),
            //grid1.Column("IGSTPer", header: "IGST(%)", style: "right"),
            //grid1.Column("CGSTAmt", header: "CGST Amt.", style: "right"),
            //grid1.Column("SGSTAndUTGSTAmt", header: "SGST/UTGST Amt.", style: "right"),
            //grid1.Column("IGSTAmt", header: "IGST Amt.", style: "right"),
            // grid1.Column("GrandTotal", header: "Grand Toal", style: "right")
            //           )
            //        ).ToString();

            var gridColumns = new List<WebGridColumn>();
            gridColumns.Add(grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1));
            gridColumns.Add(grid1.Column("GroupName", header: "Item Group", style: "my-classL"));
            gridColumns.Add(grid1.Column("ItemNo", header: "Item Code", style: "my-classC"));
            gridColumns.Add(grid1.Column("ItemName", header: "Item Description", style: "my-classL"));
            gridColumns.Add(grid1.Column("UnitCode", header: "Unit", style: "my-classL"));
            gridColumns.Add(grid1.Column("QTy", header: "Qty.", style: "right"));
            gridColumns.Add(grid1.Column("Rate", header: "Rate", style: "right"));
            gridColumns.Add(grid1.Column("DiscountPercentage", header: "Discount(%)", style: "right"));
            gridColumns.Add(grid1.Column("Discount", header: "Amount after discount", style: "right"));
            gridColumns.Add(grid1.Column("PandF", header: "P&F ", style: "right"));
            gridColumns.Add(grid1.Column("CartageAmt", header: "Cartage", style: "right"));
            gridColumns.Add(grid1.Column("InsuranceAmt", header: "Insurance", style: "right"));

            if (tcsActive == true)
            {
                if (allCust[0].TaxType.ToLower().Contains("igst"))
                {
                    gridColumns.Add(grid1.Column("IGSTPer", header: "IGST(%)", style: "right"));
                    gridColumns.Add(grid1.Column("IGSTAmt", header: "IGST Amt.", style: "right"));
                }
                else {
                    gridColumns.Add(grid1.Column("CGSTPer", header: "CGST(%)", style: "right"));
                    gridColumns.Add(grid1.Column("SGSTAndUTGSTPer", header: "SGST/UTGST(%)", style: "right"));
                    gridColumns.Add(grid1.Column("CGSTAmt", header: "CGST Amt.", style: "right"));
                    gridColumns.Add(grid1.Column("SGSTAndUTGSTAmt", header: "SGST/UTGST Amt.", style: "right"));
                }
                gridColumns.Add(grid1.Column("TCSPer", header: "TCS(%)", style: "right"));
                gridColumns.Add(grid1.Column("TCSAmt", header: "TCS Amt", style: "right"));
            }
            else {
                gridColumns.Add(grid1.Column("CGSTPer", header: "CGST(%)", style: "right"));
                gridColumns.Add(grid1.Column("SGSTAndUTGSTPer", header: "SGST/UTGST(%)", style: "right"));
                gridColumns.Add(grid1.Column("IGSTPer", header: "IGST(%)", style: "right"));
                gridColumns.Add(grid1.Column("CGSTAmt", header: "CGST Amt.", style: "right"));
                gridColumns.Add(grid1.Column("SGSTAndUTGSTAmt", header: "SGST/UTGST Amt.", style: "right"));
                gridColumns.Add(grid1.Column("IGSTAmt", header: "IGST Amt.", style: "right"));
            }

            gridColumns.Add(grid1.Column("GrandTotal", header: "Grand Toal", style: "right"));

            string gridHtml = grid1.GetHtml(columns: grid1.Columns(gridColumns.ToArray())).ToString();

            string rstGridString = "";
            RST_Data rstData = null;
            var rstNo = objmms.GateEntries.FirstOrDefault(x => x.MRN_No_New == MRNNO).RSTNo;
            if (rstNo != null && Session["ProId"].ToString() == "AHL0000083")
            {
                rstData = objmms.tbl_wmntdata.Select(s=>new RST_Data() { txn_no=s.txn_no, date1=s.date1, time1=s.time1, weight1=s.weight1, date2=s.date2, time2=s.time2, weight2=s.weight2, entry4=s.entry4, entry1=s.entry1, entry3=s.entry3 }).FirstOrDefault(x => x.txn_no == rstNo);
                List<RST_Data> rstDataList = new List<RST_Data>();

                if (rstData != null)
                {
                    rstDataList.Add(rstData);
                    WebGrid rstGrid = new WebGrid(source: rstDataList, canPage: false, canSort: false);
                    rstGridString = rstGrid.GetHtml(
                           columns: rstGrid.Columns(
                          rstGrid.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                          rstGrid.Column("txn_no", header: "RST No.", style: "my-classL"),
                          rstGrid.Column("date1", header: "Date", style: "my-classL"),
                           rstGrid.Column("time1", header: "Time", style: "my-classC"),
                                   rstGrid.Column("weight1", header: "Gross Weight (K.G.)", style: "my-classL"),
                                    rstGrid.Column("date2", header: "Date", style: "my-classL"),
                                     rstGrid.Column("time2", header: "Time", style: "right"),
                                      rstGrid.Column("weight2", header: "Tare Weight (K.G.)", style: "right"),
                                      rstGrid.Column("NetWeight", header: "Net Weight (K.G.)", style: "right"),
                                       rstGrid.Column("entry4", header: "Vehicle", style: "right"),
                                        rstGrid.Column("entry1", header: "Vehicle No.", style: "right"),
                                          rstGrid.Column("entry3", header: "Material", style: "right"))).ToString();
                }

            }

            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + ProjectName;
            string reportName = "DMR REPORT";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<table style='font-family:arial,sans-serif;font-size:7pt;border-collapse:collapse; width:100%;'><tr><td style='border:1px solid #dddddd;text-align:left;padding: 8px;'>MRN No : " + MRNNO + " </td><td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'>MRN Date : " + MRNDate + "</td></tr><tr><td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'>GRN No : " + GRNNo + "</td><td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'>GRN Date: " + GRNDate + " </td></tr><tr><td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'>Bill No : " + BillNo + "</td><td style='border:1px solid #dddddd;text-align: left;padding: 8px;'>Bill Date : " + BillDate + "</td></tr><tr><td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'>PO No : " + PONO + "</td><td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'>PO Date : " + PODate + "</td></tr><tr><td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'>E-Way Bill No. : " + EWayBill + "</td><td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'>Date Of E-Way Bill : " + EWayDate + "</td></tr><tr><td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'>Vendor Name : " + VendorName + "</td><td>Vendor Address : " + Address + " </td></tr> <tr> <td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'>Vehicle No : " + VehicleNo + "</td><td> </td> </tr> </table><br/>";

            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4, 5, 5, 30, 50);

            var writer = PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;
            document.Open();

            Paragraph PHeader = new Paragraph(header, FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));




            Paragraph PSiteName = new Paragraph(siteName, FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));
            Paragraph PReportName = new Paragraph(reportName, FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));
            Paragraph PDateTime = new Paragraph(dateTime, FontFactory.GetFont(FontFactory.TIMES, 8, iTextSharp.text.Font.BOLD));

            Paragraph PRSTHeading = new Paragraph("Weight Information", FontFactory.GetFont(FontFactory.TIMES, 8, iTextSharp.text.Font.BOLD));

            PHeader.Alignment = Element.ALIGN_CENTER;
            PSiteName.Alignment = Element.ALIGN_CENTER;
            PReportName.Alignment = Element.ALIGN_CENTER;
            PDateTime.Alignment = Element.ALIGN_RIGHT;
            PRSTHeading.Alignment = Element.ALIGN_LEFT;

            PReportName.SpacingAfter = 10f;

            PRSTHeading.SpacingAfter = 5f;
            PRSTHeading.SpacingBefore = 5f;

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
                styleSheet.LoadTagStyle("table", "size", "5pt");
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

            string totalTable = "<table style='font-family:arial,sans-serif;border-collapse:collapse; width:100%;'>"
                + "<tr><td style='border:1px solid #dddddd;text-align:left;padding: 2px;'></td>"
                + "<td style='border:1px solid #dddddd;text-align:left;padding: 14px;'>&nbsp; </td>"
                + "<td style='border:1px solid #dddddd;text-align:left;padding: 16px;'>&nbsp;</td>"
                + "<td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'> </td>"
                + "<td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'> </td>"
                + "<td style='border:1px solid #dddddd;text-align:left;padding: 8px;'>&nbsp; </td>"
                + "<td style='border: 1px solid #dddddd;padding: 8px;'> </td><td style='border:1px solid #dddddd;padding :37px' align='right'>Total </td>"
                + "<td style='border:1px solid #dddddd;padding: 8px;' align='right'> " + SUMDiscountAmt + " </td>"
                + "<td style='border:1px solid #dddddd;padding: 8px;' align='right'> " + SumPanF + " </td>"
                + "<td style='border:1px solid #dddddd;padding: 8px;' align='right'> " + SumCartage + " </td>"
                + "<td style='border:1px solid #dddddd;padding: 8px;' align='right'> " + SumInsurance + " </td>";

            if (tcsActive == true)
            {

                if (allCust[0].TaxType.ToLower().Contains("igst"))
                {
                    totalTable += "<td style='border:1px solid #dddddd;padding: 8px;' align='right'> -- </td><td style='border:1px solid #dddddd;padding: 8px;' align='right'> " + SumIGST + "</td>";
                }
                else {
                    totalTable += "<td style='border:1px solid #dddddd;padding: 8px;' align='right'> -- </td><td style='border:1px solid #dddddd;padding: 8px;' align='right'> -- </td><td style='border:1px solid #dddddd;padding: 8px;' align='right'> " + SumCGST + " </td><td style='border:1px solid #dddddd;padding: 8px;' align='right'>" + SumSGSTAndUTGST + "</td>";
                }

                totalTable += "<td style='border:1px solid #dddddd;padding: 8px;' align='right'>-</td><td style='border:1px solid #dddddd;padding: 8px;' align='right'>" + sumTCS + "</td>";
                totalTable += "<td align='right'> " + SumGrossAmt + "</td></tr></table><br/><div>&nbsp;</div>";
            }
            else {
                totalTable += "<td style='border:1px solid #dddddd;padding: 8px;' align='right'> -- </td><td style='border:1px solid #dddddd;padding: 8px;' align='right'> -- </td><td style='border:1px solid #dddddd;padding: 8px;' align='right'> -- </td><td style='border:1px solid #dddddd;padding: 8px;' align='right'> " + SumCGST + " </td><td style='border:1px solid #dddddd;padding: 8px;' align='right'>" + SumSGSTAndUTGST + "</td><td style='border:1px solid #dddddd;padding: 8px;' align='right'> " + SumIGST + "</td><td align='right'> " + SumGrossAmt + "</td></tr></table><br/><div>&nbsp;</div>";
            }

            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            styleSheet1.LoadTagStyle("table", "float", "right");
            styleSheet1.LoadTagStyle("table", "size", "5pt");
            styleSheet1.LoadTagStyle("table", "width", "100%");

            List<IElement> element = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr1, styleSheet1);

            foreach (IElement el in element)
            {
                document.Add(el);
            }

            if (rstData != null)
            {
                document.Add(PRSTHeading);
            }

            Paragraph PLineBreak = new Paragraph(Environment.NewLine);
            Paragraph PLineBreak2 = new Paragraph(Environment.NewLine);
            document.Add(PLineBreak);
            document.Add(PLineBreak2);

            rstGridString += "<table style='font-family:arial,sans-serif;border-collapse:collapse; width:100%; margin-top:100px;'><tr><td style='border:1px solid #dddddd;text-align:left;padding: 8px;'>Prepared By : </td><td style='border:1px solid #dddddd;text-align:left;padding: 8px;'>Store Incharge : </td><td style='border:1px solid #dddddd;text-align:left;padding: 8px;'>Commercial Head/Purchaser : </td><td style='border: 1px solid #dddddd;text-align: left;padding: 8px;'>Project Incharge : </td></tr></table>";


            using (StringReader sr = new StringReader(rstGridString))
            {
                var styleSheet = new iTextSharp.text.html.simpleparser.StyleSheet();

                styleSheet.LoadTagStyle("table", "border", "1");
                styleSheet.LoadTagStyle("table", "size", "5pt");
                styleSheet.LoadTagStyle("table td", "text-align", "center");
                styleSheet.LoadTagStyle("thead, tfoot", "display", "table-row-group");
                styleSheet.LoadStyle("right", "align", "right");
                styleSheet.LoadTagStyle(iTextSharp.text.html.HtmlTags.TH, iTextSharp.text.html.HtmlTags.BGCOLOR, "#cccccc");
                List<IElement> elements = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr, styleSheet);
                foreach (IElement el in elements)
                {
                    //If the element is a table manually set its header row count
                    if (el is PdfPTable)
                    {
                        PdfPTable table = ((PdfPTable)el);
                        // table.HeaderRows = 1;
                        table.SpacingAfter = 40f;
                        document.Add(el);
                    }

                }
            }

            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");

            #endregion

        }


        public void GetExcel()
        {
            List<GetGridDMR> allCust = (List<GetGridDMR>)Session["NewDMR"];

            List<GetDetailOnNewDMR> DMRPre = (List<GetDetailOnNewDMR>)Session["DMRPreDetail"];
            string poNo = DMRPre[0].PONO;
            bool? tcsActive = objmms.TbIndentPurchaseOrder_GST.Where(s => s.PurchaseOrderNo == poNo).Select(s => s.TCSActive).FirstOrDefault();

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            //string gridData = grid1.GetHtml(
            //    columns: grid1.Columns(
            //          grid1.Column("GroupName", header: "Item Group"),
            //          grid1.Column("ItemNo", header: "Item Code", style: "my-classC"),
            //                grid1.Column("ItemName", header: "Item Description"),
            //                 grid1.Column("UnitCode", header: "Unit"),
            //                  grid1.Column("QTy", header: "Qty."),
            //                   grid1.Column("Rate", header: "Rate"),
            //                    grid1.Column("Discount", header: "Discount"),
            //                     grid1.Column("PandF", header: "P&F "),
            //                      grid1.Column("CartageAmt", header: "Cartage"),
            //                       grid1.Column("InsuranceAmt", header: "Insurance"),
            //                         grid1.Column("CGSTPer", header: "CGST(%)"),
            //                          grid1.Column("SGSTAndUTGSTPer", header: "SGST/UTGST(%)"),
            //                           grid1.Column("IGSTPer", header: "IGST(%)"),
            //                            grid1.Column("CGSTAmt", header: "CGST Amt."),
            //                             grid1.Column("SGSTAndUTGSTAmt", header: "SGST/UTGST Amt."),
            //                              grid1.Column("IGSTAmt", header: "IGST Amt."),
            //                               grid1.Column("GrandTotal", header: "Grand Toal")
            //            )
            //        ).ToString();

            var gridColumns = new List<WebGridColumn>();
            gridColumns.Add(grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1));
            gridColumns.Add(grid1.Column("GroupName", header: "Item Group", style: "my-classL"));
            gridColumns.Add(grid1.Column("ItemNo", header: "Item Code", style: "my-classC"));
            gridColumns.Add(grid1.Column("ItemName", header: "Item Description", style: "my-classL"));
            gridColumns.Add(grid1.Column("UnitCode", header: "Unit", style: "my-classL"));
            gridColumns.Add(grid1.Column("QTy", header: "Qty.", style: "right"));
            gridColumns.Add(grid1.Column("Rate", header: "Rate", style: "right"));
            gridColumns.Add(grid1.Column("DiscountPercentage", header: "Discount(%)", style: "right"));
            gridColumns.Add(grid1.Column("Discount", header: "Amount after discount", style: "right"));
            gridColumns.Add(grid1.Column("PandF", header: "P&F ", style: "right"));
            gridColumns.Add(grid1.Column("CartageAmt", header: "Cartage", style: "right"));
            gridColumns.Add(grid1.Column("InsuranceAmt", header: "Insurance", style: "right"));

            if (tcsActive == true)
            {
                if (allCust[0].TaxType.ToLower().Contains("igst"))
                {
                    gridColumns.Add(grid1.Column("IGSTPer", header: "IGST(%)", style: "right"));
                    gridColumns.Add(grid1.Column("IGSTAmt", header: "IGST Amt.", style: "right"));
                }
                else {
                    gridColumns.Add(grid1.Column("CGSTPer", header: "CGST(%)", style: "right"));
                    gridColumns.Add(grid1.Column("SGSTAndUTGSTPer", header: "SGST/UTGST(%)", style: "right"));
                    gridColumns.Add(grid1.Column("CGSTAmt", header: "CGST Amt.", style: "right"));
                    gridColumns.Add(grid1.Column("SGSTAndUTGSTAmt", header: "SGST/UTGST Amt.", style: "right"));
                }
                gridColumns.Add(grid1.Column("TCSPer", header: "TCS(%)", style: "right"));
                gridColumns.Add(grid1.Column("TCSAmt", header: "TCS Amt", style: "right"));
            }
            else {
                gridColumns.Add(grid1.Column("CGSTPer", header: "CGST(%)", style: "right"));
                gridColumns.Add(grid1.Column("SGSTAndUTGSTPer", header: "SGST/UTGST(%)", style: "right"));
                gridColumns.Add(grid1.Column("IGSTPer", header: "IGST(%)", style: "right"));
                gridColumns.Add(grid1.Column("CGSTAmt", header: "CGST Amt.", style: "right"));
                gridColumns.Add(grid1.Column("SGSTAndUTGSTAmt", header: "SGST/UTGST Amt.", style: "right"));
                gridColumns.Add(grid1.Column("IGSTAmt", header: "IGST Amt.", style: "right"));
            }

            gridColumns.Add(grid1.Column("GrandTotal", header: "Grand Toal", style: "right"));

            string gridData = grid1.GetHtml(columns: grid1.Columns(gridColumns.ToArray())).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=DMRReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }
    }


    public class RST_Data
    {
        public int Id { get; set; }
        public Nullable<int> txn_no { get; set; }
        public string wtype { get; set; }
        public Nullable<decimal> weight1 { get; set; }
        public string mchar1 { get; set; }
        public string date1 { get; set; }
        public string time1 { get; set; }
        public Nullable<decimal> weight2 { get; set; }
        public string mchar2 { get; set; }
        public string date2 { get; set; }
        public string time2 { get; set; }
        public Nullable<decimal> charge1 { get; set; }
        public Nullable<decimal> charge2 { get; set; }
        public string entry1 { get; set; }
        public string entry2 { get; set; }
        public string entry3 { get; set; }
        public string entry4 { get; set; }
        public string entry5 { get; set; }
        public string entry6 { get; set; }
        public string entry7 { get; set; }
        public string entry8 { get; set; }
        public string entry9 { get; set; }
        public string entry10 { get; set; }
        public string username { get; set; }
        public string username2 { get; set; }
        public Nullable<int> syn { get; set; }
        public Nullable<decimal> NetWeight { get {
                return this.weight1 - this.weight2;
            } }
    }
}