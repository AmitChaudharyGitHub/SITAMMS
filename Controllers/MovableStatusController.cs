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
using System.Web.Helpers;
using System.IO;
using iTextSharp.text;
using DataAccessLayer;
using System.Text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;

namespace MMS.Controllers
{
    public class MovableStatusController : Controller
    {
        FactoryManager m = new FactoryManager();
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        MSP_Model objmsps = new MSP_Model();

        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            ViewBag.MovingStatus = new SelectList(objmms.tblMovingDescriptions, "MovingId", "MovingType");



            return View();
        }

        [HttpPost]
        public ActionResult Index(MovableStatus MovStatus)
        {
            tblMovableStatusMaster sm = new tblMovableStatusMaster();
            sm.CreatedDate = System.DateTime.Now;
            sm.IsActive = "E";
            sm.CreatedBy = Session["EmpID"].ToString();
            sm.MinMonth = MovStatus.MinMonth.ToString();
            sm.MaxMonth = MovStatus.MaxMonth.ToString();
            sm.MovingType = MovStatus.MovingType.ToString();
            var inst = objmms.tblMovableStatusMasters.Add(sm);
            if (inst != null)
            {
                objmms.SaveChanges();
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index");



        }

        public ActionResult MovableReport()
        {
            ViewBag.EmpID = Session["EmpID"].ToString();


            ViewBag.Status = new SelectList(objmms.tblMovingDescriptions, "MovingId", "MovingType");
            return View();
        }

        public ActionResult _GetforStatus()
        {
            List<GetMovingData> rr = new List<GetMovingData>();
            rr = objmsps.GetAllMovingDiscription();
            if (rr != null)
            {
                return View("_MovableDiscription", rr);
            }
            else {
                return View("_EmptyView");
            }
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

        public JsonResult GetItemGroupStock(string Pid)
        {

            if (Pid != null)
            {
                object ItemMasters = m.GettblItemCurrentStockManager().FindItemGroup(Pid);
                string jsonString = ItemMasters.ToJSON();
                return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public ActionResult ExportMovingReport(string PID,string Status="")
        {
            var data = objmsps.PrintAgaingReport(PID, Status);
            var workbook = new XLWorkbook();
            workbook.AddWorksheet(data, "MovingReport");

            string filename = Guid.NewGuid().ToString();

            workbook.SaveAs(System.IO.Path.GetTempPath() + filename + ".xlsx");
            return File(System.IO.Path.GetTempPath() + filename + ".xlsx", "application/vnd.ms-excel", "MovingReport.xlsx");
        }

        public ActionResult GetAllMovingStatus(string PID, string GID, string STA)
        {
            List<NewMovableStatus> result = new List<NewMovableStatus>();
            if (STA == "Select Ageing Status")
            {
                STA = "";
            }
            if (PID != null && GID != null && GID != "")
            {
                result = objmsps.GetNew_AllItemStatusReport(PID, GID, STA);
                TempData["Statusdata"] = result;
            }
            else
            {
                result = objmsps.GetNew_AllItemStatusReport(PID, GID, STA);
                TempData["Statusdata"] = result;
            }
            Session["GID"] = GID;
            Session["STA"] = STA;
            Session["ProId"] = PID;

            var data = (from a in result
                        select new NewMovableStatus
                        {
                            ID = a.ID,
                            ItemId = a.ItemId,
                            ItemName = a.ItemName,
                            UniteName = a.UniteName,
                            CurrentStockQty = a.CurrentStockQty,
                            CurrentStockAmt = Convert.ToDecimal(String.Format("{0:0.00}", a.CurrentStockAmt)),
                            MOVINGSTATUS = a.MOVINGSTATUS,
                            LstIssueDate = a.LstIssueDate != null ? a.LstIssueDate : "N/A",
                            LstReceiveDate = a.LstReceiveDate != null ? a.LstReceiveDate : "N/A",
                            LastReceiveQuantity = a.LastReceiveQuantity,
                            LastReciveAmount=a.LastReciveAmount,
                            LastIssueQuantity=a.LastIssueQuantity,
                            LastIssueAmount=a.LastIssueAmount
                        }).ToList();

            ViewBag.displySumamt = data.Sum(k => k.CurrentStockAmt);

            TempData["printexcel"] = data;
            TempData.Keep("printexcel");
            Session["keepdata"] = data;

            return PartialView("_MovableReportGrid", data);
        }


        public ActionResult ExportAgingReport(string ProjectId, string GroupId, string Status)
        {
            string ProjectName = objmms.tblProjectMasters.SingleOrDefault(x => x.PRJID == ProjectId).ProjectName;

         
            var data = objmsps.PrintAgingReport(ProjectId, GroupId,Status);
            var workbook = new XLWorkbook();
            var sheet1 = workbook.AddWorksheet(data, "AgingReport");


            var row = sheet1.Row(1).InsertRowsAbove(4).ToList();
            var rc1 = row[0].Cell(1);
            var rc2 = row[1].Cell(1);
            var rc3 = row[2].Cell(1);
            rc1.Style.Font.FontSize = 23;
            rc2.Style.Font.FontSize = 22;
            rc3.Style.Font.FontSize = 20;

            rc1.Value = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            rc2.Value = "Site- " + ProjectName;
            rc3.Value = "Aging Report as on "+(DateTime.Now.ToString("dd-MM-yyyy"));


            string filename = Guid.NewGuid().ToString();

            workbook.SaveAs(Path.GetTempPath() + filename + ".xlsx");
            return File(Path.GetTempPath() + filename + ".xlsx", "application/vnd.ms-excel", "AgingReport.xlsx");
        }



        public void GetExcel()
        {
            List<NewMovableStatus> allCust = (List<NewMovableStatus>)Session["keepdata"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                          grid1.Column("ID", header: "S.No."),
                          grid1.Column("ItemId", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UniteName", header: "Unit"),
                          //grid1.Column("LastRcvQty", header: "Last Receive Qty"),
                          // @*grid1.Column("ItemReceiveDate", header: " Last Receive Date", format: @< text > @item.ItemReceiveDate.ToString("dd/MM/yy") </ text >), *@
                          //grid1.Column("LastIssueQty", header: "Last Issue Qty"),
                          //@*grid1.Column("ItemIssueDate", header: " Last Issue Date", format: @< text > @item.ItemIssueDate.ToString("dd/MM/yy") </ text >), *@
                          grid1.Column("CurrentStockQty", header: "Available Stock"),
                          grid1.Column("CurrentStockAmt", header: "Stock Value"),
                       grid1.Column("MOVINGSTATUS", header: "Status"),
                         grid1.Column("LstReceiveDate", header: " Last Receive Date"),

                         grid1.Column("LastReceiveQuantity", header: " Last Receive Qty"),
                         grid1.Column("LastReciveAmount", header: " Last Receive Amount"),

                          grid1.Column("LstIssueDate", header: " Last Issue Date"),

                          grid1.Column("LastIssueQuantity", header: " Last Issue Qty"),
                          grid1.Column("LastIssueAmount", header: " Last Issue Amount")
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=AgingReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }
        public FileStreamResult AgeingReportPdf()
        {
            List<NewMovableStatus> allCust = (List<NewMovableStatus>)Session["keepdata"];

            string gn, gn1, st, st1;
            gn = Session["GID"].ToString();
            st = Session["STA"].ToString();

            if (gn == "")
            {
                gn1 = "-";
            }
            else
            {
                gn1 = objmms.tblItemGroupMasters.Where(a2 => a2.ItemGroupID == gn).ToList().Select(a1 => a1.GroupName).First();
            }
            if (st == "")
            {
                st1 = "-";
            }
            else
            {
                st1 = st;
            }

            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();
            var ss = from k in allCust
                     group k by k.ItemName into g
                     select new
                     { SUM = g.Sum(o => o.CurrentStockAmt) };
            var totAmt = ss.Sum(l => l.SUM);

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                          grid1.Column("ID", header: "S.No."),
                          grid1.Column("ItemId", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UniteName", header: "Unit"),
                          //grid1.Column("LastRcvQty", header: "Last Receive Qty"),
                          // @*grid1.Column("ItemReceiveDate", header: " Last Receive Date", format: @< text > @item.ItemReceiveDate.ToString("dd/MM/yy") </ text >), *@
                          //grid1.Column("LastIssueQty", header: "Last Issue Qty"),
                          //@*grid1.Column("ItemIssueDate", header: " Last Issue Date", format: @< text > @item.ItemIssueDate.ToString("dd/MM/yy") </ text >), *@
                          grid1.Column("CurrentStockQty", header: "Available Stock", style: "alignR"),
                          grid1.Column("CurrentStockAmt", header: "Stock Value", style: "alignR"),
                       grid1.Column("MOVINGSTATUS", header: "Status"),
                         grid1.Column("LstReceiveDate", header: " Last Receive Date", style: "alignC"),
                         grid1.Column("LastReceiveQuantity", header: " Last Receive Qty", style: "alignC"),
                         grid1.Column("LastReciveAmount", header: " Last Receive Amount", style: "alignC"),
                          grid1.Column("LstIssueDate", header: " Last Issue Date", style: "alignC"),
                          grid1.Column("LastIssueQuantity", header: " Last Issue Qty", style: "alignC"),
                          grid1.Column("LastIssueAmount", header: " Last Issue Amount", style: "alignC")

                        )
                    ).ToString();


            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Ageing Report";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>Group Name: " + gn1 + ", Ageing Status: " + st1 + " </div><br/>";


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
            string totalTable = "<table align='right' style='float:right;border-top:0; width:80px;'><tr><td><strong>Total</strong></td><td>" + totAmt + "</td></tr></table>";
            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            styleSheet1.LoadTagStyle("table", "float", "right");
            styleSheet1.LoadTagStyle("table", "size", "16px");
            styleSheet1.LoadTagStyle("table", "width", "190");

            List<IElement> element = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr1, styleSheet1);

            foreach (IElement el in element)
            {
                document.Add(el);
            }

            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }
    }
}