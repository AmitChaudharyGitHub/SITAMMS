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
using System.Threading;
using MMS.Helpers;

namespace MMS.Controllers.Reports
{
    public class IssueStockController : Controller
    {
        MMSEntities objmms = new MMSEntities();
        MSP_Model objmsps = new MSP_Model();
        FactoryManager m = new FactoryManager();
        // GET: IssueStock
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

        public JsonResult GetBlocksByProject(string ProjectId)
        {
            var data=objmms.ns_tbl_IssueQuantity.Where(x=>x.ProjectID==ProjectId && x.BlockName!=null).OrderBy(x=>x.BlockName).Select(x => new { Text = x.BlockName,Value=x.BlockName}).Distinct().ToList();
            return Json(data.ToJSON(),JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllDatas(string BlockName,string PID, string fromdate, string todate, string IssuedTo, string ItemGroups , string ItemId)
        {
            IssuedTo = IssuedTo ?? "0";

            Session["ProId"] = ""; Session["fdate"] = ""; Session["tdate"] = ""; Session["IssTo"] = ""; Session["GID"] = ""; Session["ITMId"] = ""; 
            List<GET_ISSUED_STOCK> result = new List<GET_ISSUED_STOCK>();
            result = objmsps.GetAllIssuedStockForReport(PID, ItemGroups,ItemId,IssuedTo,BlockName);
            Session["ProId"] = PID;
            Session["fdate"] = fromdate;
            Session["tdate"] = todate;
            Session["IssTo"] = IssuedTo;
            Session["GID"] = ItemGroups;
            Session["ITMId"] = ItemId;

            ViewBag.IsItemSelected = ItemId == "" ? false : true;

            var pcVendorList = Helpers.HelperMethods.GetPCList(PID);
            if (pcVendorList != null)
            {

            }
            if (todate == "" || fromdate=="")
            {
                Session["keepdata"] = result;
                ViewBag.displyTotalSum = result.Sum(k => k.IssueAmount);
                ViewBag.IssueQtyTotal = result.Sum(x => x.IssueQuantity);
                return PartialView("_PartialView_IssuedStock", result);
            }
            else
            {
                DateTime date1 = DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(todate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                var lst = result.Where(x => x.Issue_Date >= date1 && x.Issue_Date <= date2).OrderByDescending(x1 => x1.IssueNo).ToList();
                Session["keepdata"] = lst;
                ViewBag.displyTotalSum = lst.Sum(k => k.IssueAmount);
                ViewBag.IssueQtyTotal = lst.Sum(x => x.IssueQuantity);
                return PartialView("_PartialView_IssuedStock", lst);
            }
            
        }
        public void GetExcel()
        {
            List<GET_ISSUED_STOCK> allCust = (List<GET_ISSUED_STOCK>)Session["keepdata"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                       grid1.Column("Issue_Date", header: "Issue Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Issue_Date)),
                       grid1.Column("IssueNo", header: "MRN No"),
                       grid1.Column("IndentNo", header: "Requisition No"),
                       grid1.Column("Type", header: "Issue Type"),
                       grid1.Column("ItemGroupName", header: "Group"),
                        grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("BlockName", header: "Block Name"),
                       grid1.Column("IssueQuantity", header: "Quantity", format: (item) => string.Format("{0:0.00}", item.IssueQuantity)),
                       grid1.Column("IssueAmount", header: "Amount", format: (item) => string.Format("{0:0.00}", item.IssueAmount)),
                       grid1.Column("IssuedTo", header: "Issued To PC / Work"),
                       grid1.Column("Optional_Name", header: "Issued Name"),
                       grid1.Column("FinancialType", header: "Issued Type")
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=IssueStock.xls");
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
        public FileStreamResult IssueStockPdf()
        {
            List<GET_ISSUED_STOCK> allCust = (List<GET_ISSUED_STOCK>)Session["keepdata"];

            string fd, tdate, fd1, td2, gn1, Is1 , itmid;

            

            var totAmt = Math.Round((decimal)(allCust.Sum(o => o.IssueAmount)), 2);

            fd1 = Session["fdate"].ToString();
            td2 = Session["tdate"].ToString();
            itmid = Session["ITMId"].ToString();
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
            string gn = Session["GID"].ToString();
            if (gn == "")
            {
                gn1 = "--";
            }
            else
            {
                gn1 = objmms.tblItemGroupMasters.Where(a2 => a2.ItemGroupID == gn).ToList().Select(a1 => a1.GroupName).FirstOrDefault();
            }
            string Is2 = Session["IssTo"].ToString();
            if (Is2 == "")
            {
                Is1 = "--";
            }
            else
            {
                Is1 = objmms.tblPcContractorMasters.Where(a5 => a5.PcContractorID == Is2).ToList().Select(a1111 => a1111.Name).FirstOrDefault();
            }

            if (itmid == "")
            {
                itmid = "--";
            }
            else {
                itmid = objmms.tblItemMasters.Where(it => it.ItemID == itmid).FirstOrDefault().ItemName;
            }

            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();
            

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                       grid1.Column(header: "S.No", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1),
                      grid1.Column("Issue_Date", header: "Issue Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Issue_Date)),
                       grid1.Column("IssueNo", header: "MRN No"),
                       grid1.Column("IndentNo", header: "Requisition No"),
                       grid1.Column("Type", header: "Issue Type"),
                       grid1.Column("ItemGroupName", header: "Group"),
                       grid1.Column("ItemCode", header: "Item Code"),
                       grid1.Column("ItemName", header: "Item"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("BlockName", header: "Block Name"),
                       grid1.Column("IssuedTo", header: "Issued To PC / Work"),
                       grid1.Column("Optional_Name", header: "Issued Name"),
                       grid1.Column("FinancialType", header: "Issued Type"),
                        grid1.Column("IssueQuantity", header: "Quantity", format: (item) => string.Format("{0:0.00}", item.IssueQuantity), style: "right"),
                       grid1.Column("IssueAmount", header: "Amount", format: (item) => string.Format("{0:0.00}", item.IssueAmount), style: "right")
                       )
                    ).ToString();


            string ImageUrl = Server.MapPath("/images/logo.png");
            Image img = Image.GetInstance(ImageUrl);
            img.ScaleToFit(40f, 40f);
            img.Alignment = Element.ALIGN_CENTER;


            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "ISSUED STOCK";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            string additionalData = "<div style='text-align:left'>From Date: " + fd + " To Date: " + tdate + " </div><div style='text-align:left'>Group Name: " + gn1 + " , Item  Name : " + itmid + " , Issue To: " + Is1 + " </div><br/>";

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
                styleSheet.LoadStyle("right","align","right");
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
                totalTable = "<table width='100%' style='border-top:0;'><tr><td colspan='11' align='right'><strong>Total Qty.</strong></td><td class='right'>" + allCust.Sum(x => x.IssueQuantity) + "</td><td align='right'><strong>Total Amt.</strong></td><td class='right'>" + totAmt + "</td></tr></table>";
            }
            else
            {
                totalTable = "<table width='100%'  style='border-top:0;'><tr><td colspan='12' align='right'><strong>Total</strong></td><td class='right'>" + totAmt + "</td></tr></table>";
            }
            StringReader sr1 = new StringReader(totalTable);
            var styleSheet1 = new iTextSharp.text.html.simpleparser.StyleSheet();
            styleSheet1.LoadTagStyle("table", "border", "1");
            styleSheet1.LoadTagStyle("table", "float", "right");
            styleSheet1.LoadTagStyle("table", "size", "16px");
            styleSheet1.LoadTagStyle("table", "width", "200");
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

        public JsonResult BindIssueTo(string ProjectId)
        {
            string J = string.Empty;
            var param = new Dictionary<string, object>();
            param.Add("projectId",ProjectId);
            J=Procedure.GetDataBySP("GetPCVendorList",param).DataTableToJSON();
           
            return Json(J, JsonRequestBehavior.AllowGet);

        }

        //change18042018
        public ActionResult PCDebitNote()
        {
            TempData["Ids"] = null;
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
            ViewBag.SelectType = objmms.tblFinancialSelectionTypes.Where(x => x.Status == true).Select(p => new SelectListItem { Value = p.TransId.ToString(), Text = p.FinancialType }).OrderBy(k => k.Text);
            return View();
        }

        public string GetDebitCode(string projectId, string debitDate, string contractorType)
        {
            Procedure procedure = new Procedure();
            DateTime DebitDate = DateTime.ParseExact(debitDate,"dd/MM/yyyy",CultureInfo.InvariantCulture);
            string debitCode = procedure.GetDebitNote(projectId, DebitDate, contractorType);
            TempData["DebitCode"] = debitCode;
            TempData["DebitDate"] = debitDate;
            return debitCode;
        }


        public static string GetCurrentFinancialYear(DateTime InputDate)
        {
            int CurrentYear = InputDate.Year;
            int PreviousYear = InputDate.Year - 1;
            int NextYear = InputDate.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (InputDate.Month > 3)
                FinYear = CurYear + "-" + NexYear.Substring(NexYear.Length - 2);
            else
                FinYear = PreYear + "-" + CurYear.Substring(CurYear.Length - 2);
            return FinYear.Trim();
        }

        //change13042018
        [HttpPost]
        public ActionResult PCDebitNote(FormCollection formData)
        {
            string debitnotecode = "";
            if (TempData["Ids"] != null)
            {
                string empId = Session["EmpID"].ToString();

   
                DateTime debitDate = DateTime.ParseExact(TempData["DebitDate"].ToString(), "dd/MM/yyyy",
                                   CultureInfo.InvariantCulture);
                List<string> notes = null;
                if (TempData["ProId"] != null && TempData["IssTo"] != null && TempData["GID"] != null && TempData["ITMId"] != null)
                {
                    List<GET_ISSUED_STOCK> result = TempData["keepdata"] as List<GET_ISSUED_STOCK>;
                    Dictionary<string, string> dic = TempData["Ids"] as Dictionary<string, string>;
                    notes = new List<string>();

                    foreach (var key in dic.Keys)
                    {
                        if (dic[key] != "")
                        {
                            var item = result.SingleOrDefault(x => x.IssueNo == Convert.ToInt32(key));
                            debitnotecode = Get_DebitNoteNo(TempData["ProId"].ToString(), debitDate, TempData["ContractorType"].ToString());
                            objmms.tblMMSPCDebitNotes.Add(new tblMMSPCDebitNote { ProjectId = TempData["ProId"].ToString(),
                                Issue_Req_No = item.IndentNo,
                                ItemId = item.ItemID,
                                Status = true,
                                CreatedDate = DateTime.Now,
                                PC_Code = TempData["IssTo"].ToString(),
                                CreatedBy = empId,
                                DebitNote_Date = debitDate,
                                Details_of_Billing_Department = dic[key],
                                DebitNote_Code = debitnotecode,   ///TempData["DebitCode"].ToString(),
                                IssueTypeId = item.IssueNo,
                                Issue_Req_Date = item.Issue_Date,
                                Contractor_Type = TempData["ContractorType"].ToString(),
                                DivisionId=Repos.GetUserDivision(),
                                Session_Year = GetCurrentFinancialYear(debitDate) });
                            var issueQuantityData = objmms.ns_tbl_IssueQuantity.SingleOrDefault(x => x.Id == item.IssueNo);
                            issueQuantityData.IsDebit_Note = true;
                            issueQuantityData.DebitNote_Date = debitDate;
                            objmms.Entry(issueQuantityData).State = EntityState.Modified;
                        }
                    }
                    objmms.SaveChanges();

                }

            }
            TempData["DebitNoteNo"] = debitnotecode;
            return RedirectToAction("PCDebitNote");
        }

        //change18042018
        public ActionResult GetAllDataForDebitNotes(string PID, string fromdate, string todate, string IssuedTo, string ItemGroups, string ItemId, string SelectType, string ContractorType)
         {
            CultureInfo cInfo = new CultureInfo("en-IN");
            cInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            cInfo.DateTimeFormat.DateSeparator = "/";
            cInfo.DateTimeFormat.LongDatePattern = "dd/MM/yyyy hh:mm:ss";
            Thread.CurrentThread.CurrentCulture = cInfo;
            Thread.CurrentThread.CurrentUICulture = cInfo;

            List<GET_ISSUED_STOCK> result = new List<GET_ISSUED_STOCK>();
            if(fromdate!="" && todate != "")
            {
                fromdate = Convert.ToDateTime(fromdate).ToString("yyyy/MM/dd");
                todate = Convert.ToDateTime(todate).ToString("yyyy/MM/dd");
            }
            result = objmsps.GetPendingDebitNote(PID,fromdate,todate,IssuedTo,ItemGroups,ItemId,SelectType,ContractorType);
           // result = result.Where(x => !x.IsDebitNote.HasValue).ToList();
            TempData["ProId"] = PID;
            TempData["IssTo"] = IssuedTo;
            TempData["GID"] = ItemGroups;
            TempData["ITMId"] = ItemId;
            TempData["SelectType"] = SelectType;
            TempData["ContractorType"] = ContractorType;
            TempData["fDate"] = fromdate == "" ? null : fromdate;
            TempData["tDate"] = todate == "" ? null : todate;
            Session["ProId"] = PID;

           
            TempData["keepdata"] = result;
            return PartialView("_PartialView_IssuedStock_Debit_Notes", result);
        }

        //change17042018
        public ActionResult SearchDataForUpdatedDebitNotes(string PID, string FromDate, string ToDate, string ContractorType, string IssuedTo)
        {
            TempData["Ids"] = null;
            CultureInfo cInfo = new CultureInfo("en-IN");
            cInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            cInfo.DateTimeFormat.DateSeparator = "/";
            cInfo.DateTimeFormat.LongDatePattern = "dd/MM/yyyy hh:mm:ss";
            Thread.CurrentThread.CurrentCulture = cInfo;
            Thread.CurrentThread.CurrentUICulture = cInfo;

            List<GET_UPDATED_DEBIT_NOTE> result = new List<GET_UPDATED_DEBIT_NOTE>();
            ContractorType=ContractorType == "" ? null:ContractorType;
            IssuedTo=IssuedTo == "0" ? null : IssuedTo;
            result = objmsps.GetUpdatedDebitNotesForSearch(PID, ContractorType, IssuedTo);
            if (ToDate != "" && FromDate != "")
            {
                DateTime fDate = Convert.ToDateTime(FromDate);
                DateTime tDate = Convert.ToDateTime(ToDate);
                result = result.Where(x => x.DebitNote_Date >= fDate && x.DebitNote_Date <= tDate).ToList();
            }

            TempData.Keep();

            return PartialView("_PartialView_UpdatedDebitNote", result);
        }

        public ActionResult GetUpdatedDebitNotes(string DebitNoteCode)
        {
            List<GET_UPDATED_DEBIT_NOTE> result = new List<GET_UPDATED_DEBIT_NOTE>();
            result = objmsps.GetUpdatedDebitNotes(Session["ProId"].ToString(), DebitNoteCode);

            return PartialView("_PartialView_UpdatedDebitNote", result);
        }

        //change18042018
        public FileStreamResult UpdatedDebitNotePdf(string DebitCode, string PID)
        {
            string debitNoteDate = "--";
            string debitNoteCode = "--";
            string contractorType = "--";
            string issueTo = "--";
            string fDate = "--";
            string tDate = "";
            string pid = PID;

            var projectName = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();
            List<GET_ISSUED_STOCK> issuedStocksWithDebitNote = objmsps.GetIssuedStockByDebitCodeAndProjectId(DebitCode, pid);

            debitNoteCode = DebitCode;

            debitNoteDate = (from model in objmms.tblMMSPCDebitNotes
                             where model.DebitNote_Code == DebitCode
                             select model).FirstOrDefault().DebitNote_Date.Value.ToString("dd/MM/yyyy");


            if (TempData["IssTo"] != null)
            {
                issueTo = TempData["IssTo"].ToString();
                var d = objmms.tblPcContractorMasters.SingleOrDefault(x => x.PcContractorID == issueTo);

                if (d != null)
                {
                    issueTo = d.Name;
                }
                else
                {
                    issueTo = Helpers.HelperMethods.GetPCs(pid, issueTo);
                }
            }
            if (TempData["ContractorType"] != null)
            {
                contractorType = TempData["ContractorType"].ToString();
            }
            if (TempData["fDate"] != null && TempData["tDate"] != null)
            {
                fDate = Convert.ToDateTime(TempData["fDate"]).ToString("dd/MM/yyyy");
                tDate = Convert.ToDateTime(TempData["tDate"]).ToString("dd/MM/yyyy");
            }



            //if (fDate == "")
            //{
            //    fDate = "--";
            //}
            //else
            //{
            //    fDate = DMYToMDY(fDate);
            //}
            //if (tDate == "")
            //{
            //    tDate = "--";
            //}
            //else
            //{
            //    tDate = DMYToMDY(tDate);
            //}



            WebGrid grid1 = new WebGrid(source: issuedStocksWithDebitNote, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                        grid1.Column(header: "S.No.", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1, style: "std"),
                       grid1.Column("Issue_Date", header: "Issue Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Issue_Date), style: "mtd"),
                           grid1.Column("IndentNo", header: "Requisition No", style: "mtd"),
                           grid1.Column("ItemCode", header: "Item Code", style: "mtd"),
                           grid1.Column("HSNCode", header: "HSN Code", style: "mtd"),
                           grid1.Column("ItemGroupName", header: "Item Group", style: "mtd"),
                           grid1.Column("ItemName", header: "Item Name", style: "mtd"),
                           grid1.Column("UnitName", header: "Unit", style: "mtd"),
                            grid1.Column("Optional_Name", header: "Issued Name", style: "mtd"),
                            grid1.Column("FinancialType", header: "Issued Type", style: "mtd"),
                            grid1.Column("DebitNote", header: "Remarks", style: "mtd"),
                            grid1.Column("IssueQuantity", header: "Quantity", format: (item) => string.Format("{0:0.000}", item.IssueQuantity), style: "right"),
                            grid1.Column("IssueAmount", header: "Amount", format: (item) => string.Format("{0:0.00}", item.IssueAmount), style: "right")
                       )
                    ).ToString();

            decimal totalAmount = new decimal(0);

            foreach (var item in issuedStocksWithDebitNote)
            {
                totalAmount += Convert.ToDecimal(item.IssueAmount);
            }

            var total = String.Format("{0:0.00}", totalAmount);
            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + projectName;
            string reportName = "PC Debit Note";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            //string additionalData = "<div style='text-align:left'>From Date: " + fDate + " To Date: " + tDate + " </div><div style='text-align:left'>Issue To: " + issueTo + ", Contractor Type: " + contractorType + " </div><div style='text-align:left'>Debit Note Date:" + debitNoteDate + ", Debit Note Code:" + debitNoteCode + "</div>";
            string additionalData = "<div style='text-align:left'>Issue To: " + issueTo + ", Contractor Type: " + contractorType + " </div><div style='text-align:left'>Debit Note Date:" + debitNoteDate + ", Debit Note Code:" + debitNoteCode + "</div>";

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

            string totalTable = "<table align='right' style='float:right;border-top:0; width:80px;'><tr><td><strong>Total</strong></td><td class='right'>" + total + "</td></tr></table>";
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
            TempData.Keep();
            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

        //change17042018
        public ActionResult SelectedDebitNotePdf(string DebitCode, string DebitDate)
        {
            string debitNoteDate = "--";
            string debitNoteCode = "--";
            string contractorType = "--";
            string issueTo = "--";
            string fDate = "--";
            string tDate = "";


            if (Session["ProId"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string pid = Session["ProId"].ToString();


            Dictionary<string, string> dic = TempData["Ids"] as Dictionary<string, string>;
            

            var projectName = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();
            if (dic != null)
            {
                int[] arrIds = new int[dic.Keys.Count];
                string[] ids = dic.Keys.ToArray();
                for (int i = 0; i < dic.Keys.Count; i++)
                {
                    arrIds[i] = Convert.ToInt32(ids[i]);
                }
                List<GET_ISSUED_STOCK> selectedData = objmsps.GetAllIssuedStock(pid);

                selectedData = selectedData.Where(x => arrIds.Contains(x.IssueNo)).ToList();

                foreach (var item in selectedData)
                {
                    item.DebitNote = dic[item.IssueNo.ToString()];
                }

                if (TempData["IssTo"] != null)
                {
                    issueTo = TempData["IssTo"].ToString();
                    issueTo = objmms.tblPcContractorMasters.SingleOrDefault(x => x.PcContractorID == issueTo).Name;
                }
                if (TempData["ContractorType"] != null)
                {
                    contractorType = TempData["ContractorType"].ToString();
                }
                if (TempData["fDate"] != null && TempData["tDate"] != null)
                {
                    fDate = Convert.ToDateTime(TempData["fDate"]).ToString("dd/MM/yyyy");
                    tDate = Convert.ToDateTime(TempData["tDate"]).ToString("dd/MM/yyyy");
                }

                if (DebitCode != null)
                {
                    debitNoteCode = DebitCode;
                }

                if (DebitDate != null)
                {
                    debitNoteDate = DebitDate;
                }

                WebGrid grid1 = new WebGrid(source: selectedData, canPage: false, canSort: false);
                string gridHtml = grid1.GetHtml(
                        columns: grid1.Columns(
                            grid1.Column(header: "S.No.", format: (item) => item.WebGrid.Rows.IndexOf(item) + 1, style: "std"),
                           grid1.Column("Issue_Date", header: "Issue Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Issue_Date), style: "mtd"),
                               grid1.Column("IndentNo", header: "Requisition No", style: "mtd"),
                               grid1.Column("ItemCode", header: "Item Code", style: "mtd"),
                               grid1.Column("HSNCode", header: "HSN Code", style: "mtd"),
                               grid1.Column("ItemGroupName", header: "Item Group", style: "mtd"),
                               grid1.Column("ItemName", header: "Item", style: "mtd"),
                               grid1.Column("UnitName", header: "Unit", style: "mtd"),
                               grid1.Column("IssueQuantity", header: "Quantity", format: (item) => string.Format("{0:0.000}", item.IssueQuantity), style: "right"),
                               grid1.Column("IssueAmount", header: "Amount", format: (item) => string.Format("{0:0.00}", item.IssueAmount), style: "right"),
                                grid1.Column("Optional_Name", header: "Issued Name", style: "mtd"),
                                grid1.Column("FinancialType", header: "Issued Type", style: "mtd"),
                                grid1.Column("DebitNote", header: "Debit Note", style: "mtd")
                           )
                        ).ToString();
               

                StringBuilder DataString = new StringBuilder();
                string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
                string siteName = "Site-" + projectName;
                string reportName = "PC Debit Note Preview";
                string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                string additionalData = "<div style='text-align:left'>From Date: " + fDate + " To Date: " + tDate + " </div><div style='text-align:left'>Issue To: " + issueTo + " </div><div style='text-align:left'>Debit Note Date:" + debitNoteDate + ", Debit Note Code:" + debitNoteCode + "</div>";

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
                TempData.Keep();
                return new FileStreamResult(output, "application/pdf");
            }
            return new HttpNotFoundResult();
        }

        //change17042018
        [HttpPost]
        public JsonResult SaveResult(string id, bool isChecked, string note)
        {

            Dictionary<string, string> dic = null;
            if (TempData["Ids"] == null)
            {
                dic = new Dictionary<string, string>();
                if (isChecked)
                {
                    dic.Add(id, note);
                }
            }
            else
            {
                dic = TempData["Ids"] as Dictionary<string, string>;
                
                if (isChecked)
                {
                    if (dic.ContainsKey(id))
                        dic[id] = note;
                    else
                        dic.Add(id, note);
                }
                else
                {
                    if (dic.ContainsKey(id))
                        dic.Remove(id);
                }
            }

            TempData["Ids"] = dic;
            TempData.Keep();
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        //change18042018
        public ActionResult GetSavedResult()
        {
            Dictionary<string, string> dic = null;
            if (TempData["Ids"] != null)
            {
                dic = TempData["Ids"] as Dictionary<string, string>;
                TempData.Keep();
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            return new HttpNotFoundResult();
        }


        [HttpPost]
        public void SaveMultipleData(ViewHolder arr)
        {
            Dictionary<string, string> dic = null;

            if (TempData["Ids"] == null)
            {
                dic = new Dictionary<string, string>();
                for (int i = 0; i < arr.Ids.Length; i++)
                {
                    if (arr.Notes[i] != "")
                    {
                        dic.Add(arr.Ids[i], arr.Notes[i]);
                    }
                }
            }
            else
            {
                dic = TempData["Ids"] as Dictionary<string, string>;
                for (int i = 0; i < arr.Ids.Length; i++)
                {
                    if (arr.Notes[i] != "")
                    {
                        if (dic.ContainsKey(arr.Ids[i]))
                            dic[arr.Ids[i]] = arr.Notes[i];
                        else
                            dic.Add(arr.Ids[i], arr.Notes[i]);
                    }
                }
            }
            TempData["Ids"] = dic;
        }
        //change18042018
        [HttpPost]
        public void RemoveMultipleData(ViewHolder arr)
        {
            Dictionary<string, string> dic = null;

            if (TempData["Ids"] != null)
            {
                dic = TempData["Ids"] as Dictionary<string, string>;
                for (int i = 0; i < arr.Ids.Length; i++)
                {
                    if (dic.ContainsKey(arr.Ids[i]))
                    {
                        dic.Remove(arr.Ids[i]);
                    }
                }
                TempData["Ids"] = dic;
            }
        }

       public ActionResult PCDebitNoteReport()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;

            return View();
        }


        #region get debit not no. on 9_3_2020
        
        public string Get_DebitNoteNo(string projectId, DateTime debitDate, string contractorType)
        {
            Procedure procedure = new Procedure();
          //  DateTime DebitDate = DateTime.ParseExact(debitDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string debitCode = procedure.GetDebitNote(projectId, debitDate, contractorType);
            TempData["DebitCode"] = debitCode;
            TempData["DebitDate"] = debitDate;
            return debitCode;
        }

        #endregion

    }
    //change18042018
    public class ViewHolder
    {
        public string[] Ids { get; set; }
        public string[] Notes { get; set; }
    }
}