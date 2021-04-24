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
using System.Text;
using iTextSharp.text.pdf;

namespace MMS.Controllers.Reports
{
    public class Non_Moveing_Controller : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        string ProjectId;
        // GET: Non_Moveing_
        [Authorize]
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            string ProjectId = Session["ProId"].ToString();

            Non_Moveing_Models rrm = new Non_Moveing_Models();

            ViewBag.item_Group_name = new SelectList(objmms.tblItemGroupMasters, "ItemGroupID", "GroupName").OrderBy(k => k.Text);
            //ViewBag.Vendor_Name = new SelectList(objmms.GateEntryChilds, "VendorNo", "Vendor").OrderBy(k => k.Text);

            //code for bind vendor name accoding project
            var a = objmms.GateEntryChilds.Where(b => b.ProjectNo == ProjectId).OrderBy(c => c.Vendor).ToList();
            var t = a.Distinct<GateEntryChild>().Select(x => new SelectListItem
            {
                Value = x.VendorNo?.ToString().Trim(),
                Text = x.Vendor?.ToString().Trim()
            });
            ViewBag.Vendor_Name = t;

            var data_from_table = (from e in objmms.GateEntryChilds
                                   where e.ProjectNo == ProjectId
                                   orderby e.GateEntryNo
                                   select new Non_Moveing_Models
                                   {
                                       //GateEntryNo = e.GateEntryNo,
                                       //Vendor = e.Vendor,
                                       //BillNo = "",
                                       //StatusTypeNo = e.StatusTypeNo,
                                       ItemGroup = e.ItemGroup,
                                       Item = e.Item,
                                       ReceiveQty = e.ReceiveQty,
                                       Unit = e.Unit,
                                       Rate = e.Rate,
                                       Amount = e.Amount,
                                       CreatedDate=e.CreatedDate
                                   }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();

            rrm.Non_Moveing = data_from_table;
            TempData["printexcel"] = data_from_table;
            TempData.Keep("printexcel");
            Session["keepdata"] = data_from_table;
            return View("Index", rrm);
        }


        public ActionResult Non_Moveing_Search(string Item_Group_Name, string FromDate, string ToDate)
        {
            string ProjectId = Session["ProId"].ToString();
            List<Non_Moveing_Models> rrm = null;
            var data_rrm = rrm;

            if ( Item_Group_Name=="" && FromDate == "" && ToDate == "")
            {
                data_rrm = (from e in objmms.GateEntryChilds
                            where e.ProjectNo == ProjectId
                            orderby e.GateEntryNo
                            select new Non_Moveing_Models
                            {
                                //GateEntryNo = e.GateEntryNo,
                                //Vendor = e.Vendor,
                                //BillNo = "",
                                //StatusTypeNo = e.StatusTypeNo,
                                ItemGroup = e.ItemGroup,
                                Unit = e.Unit,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Rate = e.Rate,
                                Amount = e.Amount,
                                CreatedDate = e.CreatedDate
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
                Session["keepdata"] = data_rrm;
            }



            else if (FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                if (Item_Group_Name.Trim() == null && Item_Group_Name == "")
                {
                    DateTime date1 = Convert.ToDateTime(FromDate);
                    DateTime date2 = Convert.ToDateTime(ToDate);

                    data_rrm = (from e in objmms.GateEntryChilds
                                where e.ProjectNo == ProjectId && e.CreatedDate >= date1 && e.CreatedDate < date2
                                select new Non_Moveing_Models
                                {
                                    //GateEntryNo = e.GateEntryNo,
                                    //Vendor = e.Vendor,
                                    //BillNo = "",
                                    //StatusTypeNo = e.StatusTypeNo,
                                    ItemGroup = e.ItemGroup,
                                    Item = e.Item,
                                    Unit = e.Unit,
                                    ReceiveQty = e.ReceiveQty,
                                    Rate = e.Rate,
                                    Amount = e.Amount,
                                    CreatedDate = e.CreatedDate
                                }).OrderByDescending(k => k.CreatedDate).ToList();
                    ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                    ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                    TempData["printexcel"] = data_rrm;
                    TempData.Keep("printexcel");
                    Session["keepdata"] = data_rrm;
                }
           

            else
            {
                if (Item_Group_Name.Trim() != null && Item_Group_Name != "")
                {
                    DateTime date1 = Convert.ToDateTime(FromDate);
                    DateTime date2 = Convert.ToDateTime(ToDate);

                    data_rrm = (from e in objmms.GateEntryChilds
                                where e.ItemGroupNo == Item_Group_Name && e.ProjectNo == ProjectId && e.CreatedDate >= date1 && e.CreatedDate < date2
                                select new Non_Moveing_Models
                                {
                                    //GateEntryNo = e.GateEntryNo,
                                    //Vendor = e.Vendor,
                                    //BillNo = "",
                                    //StatusTypeNo = e.StatusTypeNo,
                                    ItemGroup = e.ItemGroup,
                                    Item = e.Item,
                                    Unit = e.Unit,
                                    ReceiveQty = e.ReceiveQty,
                                    Rate = e.Rate,
                                    Amount = e.Amount,
                                    CreatedDate = e.CreatedDate
                                }).OrderByDescending(k => k.CreatedDate).ToList();
                        ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                        ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                        TempData["printexcel"] = data_rrm;
                        TempData.Keep("printexcel");
                        Session["keepdata"] = data_rrm;
                    }
                else
                {
                    DateTime date1 = Convert.ToDateTime(FromDate);
                    DateTime date2 = Convert.ToDateTime(ToDate);

                    data_rrm = (from e in objmms.GateEntryChilds
                                where  e.ProjectNo == ProjectId && e.CreatedDate >= date1 && e.CreatedDate < date2
                                select new Non_Moveing_Models
                                {
                                    //GateEntryNo = e.GateEntryNo,
                                    //Vendor = e.Vendor,
                                    //BillNo = "",
                                    //StatusTypeNo = e.StatusTypeNo,
                                    ItemGroup = e.ItemGroup,
                                    Item = e.Item,
                                    Unit = e.Unit,
                                    ReceiveQty = e.ReceiveQty,
                                    Rate = e.Rate,
                                    Amount = e.Amount,
                                    CreatedDate = e.CreatedDate
                                }).OrderByDescending(k => k.CreatedDate).ToList();
                        ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                        ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                        TempData["printexcel"] = data_rrm;
                        TempData.Keep("printexcel");
                        Session["keepdata"] = data_rrm;
                    }
            }

            }

            else if (Item_Group_Name.Trim() != null && Item_Group_Name != "")
            {
                data_rrm = (from e in objmms.GateEntryChilds
                            where e.ItemGroupNo == Item_Group_Name && e.ProjectNo == ProjectId
                            select new Non_Moveing_Models
                            {
                                //GateEntryNo = e.GateEntryNo,
                                //Vendor = e.Vendor,
                                //BillNo = "",
                                //StatusTypeNo = e.StatusTypeNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                Unit = e.Unit,
                                ReceiveQty = e.ReceiveQty,
                                Rate = e.Rate,
                                Amount = e.Amount,
                                CreatedDate = e.CreatedDate
                            }).OrderByDescending(k => k.CreatedDate).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
                Session["keepdata"] = data_rrm;
            }


            else {

            }
            return PartialView("_Non_Moveing_Partial", data_rrm);
        }
        public void GetExcel()
        {
            List<Non_Moveing_Models> allCust = (List<Non_Moveing_Models>)Session["keepdata"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                           grid1.Column("ItemGroup", header: "Item Group Name"),
                       grid1.Column("Item", header: "Item Name"),
                       grid1.Column("Unit", header: "Unit"),
                       grid1.Column("ReceiveQty", header: "Quantity"),
                       grid1.Column("Rate", header: "Rate"),
                       grid1.Column("Amount", header: "Amount"),
                       grid1.Column("CreatedDate", header: "Date")
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=NonMovingReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }
        public FileStreamResult GETPdf()
        {
            List<Non_Moveing_Models> allCust = (List<Non_Moveing_Models>)Session["keepdata"];


            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();


            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                         grid1.Column("ItemGroup", header: "Item Group Name"),
                       grid1.Column("Item", header: "Item Name"),
                       grid1.Column("Unit", header: "Unit"),
                       grid1.Column("ReceiveQty", header: "Quantity",style:"right"),
                       grid1.Column("Rate", header: "Rate",style:"right"),
                       grid1.Column("Amount", header: "Amount",style:"right"),
                       grid1.Column("CreatedDate", header: "Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.CreatedDate))
                        )
                    ).ToString();

           


            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Ageing Report";
            string dateTime = "Created Date: " + DateTime.Now.ToString("dd/MM/yyyy") + " :" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
        
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