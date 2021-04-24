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
    public class IssueReportController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        string ProjectId;
        // GET: IssueReport
        [Authorize]
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            string ProjectId = Session["ProId"].ToString();

            IssueReport rrm = new IssueReport();

            //code for indent no.

            var a = objmms.ns_tbl_IssueQuantity.Where(x2 => x2.PcContractorId != null && x2.ProjectID == ProjectId).OrderBy(e => e.Id).ToList();
            var t1 = a.Distinct<ns_tbl_IssueQuantity>().Select(x1 => new SelectListItem
            {
                Value = x1.IndentNo?.ToString().Trim(),
                Text = x1.IndentNo?.ToString().Trim()
            }
            );

            ViewBag.Indent_Number = t1;

            //code for bind pc contractor name accoding project

            var b = objmms.tblPcContractorMasters.Where(c => c.PRJID.Contains(ProjectId)).OrderBy(e => e.Name).ToList();

            var t = b.Distinct<tblPcContractorMaster>().Select(x => new SelectListItem
            {
                Value = x.PcContractorID?.ToString().Trim(),
                Text = x.Name?.ToString().Trim()
            });

            ViewBag.SubContractor_Name = t;

            //code for item group

            var a1 = objmms.tblItemGroupMasters.OrderBy(e1 => e1.GroupName).ToList();
            var t11 = a1.Distinct<tblItemGroupMaster>().Select(x11 => new SelectListItem
            {
                Value = x11.ItemGroupID?.ToString().Trim(),
                Text = x11.GroupName?.ToString().Trim()
            }
            );

            ViewBag.Item_Group_Name = t11;

            var data_from_table = (from e in objmms.ns_tbl_IssueQuantity
                                   join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID 
                                   where e.ProjectID == ProjectId
                                   orderby e.CreatedDate descending
                                   select new IssueReport
                                   {
                                       CreatedDate = e.CreatedDate,
                                       Id = e.Id,
                                       IndentNo = e.IndentNo,
                                       EmployeeName = f.Name,
                                       ItemGroupName = e.ItemGroupName,
                                       ItemName = e.ItemName,
                                       IssueQuantity = e.IssueQuantity
                                   }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();

            rrm.Datewise_Item = data_from_table;
            TempData["printexcel"] = data_from_table;
            TempData.Keep("printexcel");
            return View("Index", rrm);
        }

        public void GetExcel()
        {
            List<IssueReport> allCust = (List<IssueReport>)TempData["printexcel"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                    grid1.Column("CreatedDate", header: "PO Date."),
                       grid1.Column("Id", header: "Issue No. "),
                       grid1.Column("IndentNo", header: "Indent No"),
                       grid1.Column("EmployeeName", header: "Sub Contractor Name"),
                       grid1.Column("ItemGroupName", header: "Item Group Name"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("IssueQuantity", header: "Issue Qty")
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=VendorWiseReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }
        public FileStreamResult GETPdf()
        {
            List<IssueReport> allCust = (List<IssueReport>)TempData["printexcel"];

            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();


            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                          grid1.Column("CreatedDate", header: "PO Date.", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.CreatedDate)),
                       grid1.Column("Id", header: "Issue No. "),
                       grid1.Column("IndentNo", header: "Indent No"),
                       grid1.Column("EmployeeName", header: "Sub Contractor Name"),
                       grid1.Column("ItemGroupName", header: "Item Group Name"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("IssueQuantity", header: "Issue Qty",style:"right")
                        )
                    ).ToString();

           
         
            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "ISSUE TO PC REPORT";
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


        public ActionResult Receipt_Report_Search(string Item_Group_Name, string Indent_Number, string SubContractor_Name, string FromDate, string ToDate)
        {
            string ProjectId = Session["ProId"].ToString();
            List<IssueReport> rrm = null;
            var data_rrm = rrm;

            if (Item_Group_Name == "" && Indent_Number == "" && SubContractor_Name == "" && FromDate == "" && ToDate == "")
            {
                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                                       join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                                       where e.ProjectID == ProjectId
                                       orderby e.CreatedDate descending
                                       select new IssueReport
                                       {
                                           CreatedDate = e.CreatedDate,
                                           Id = e.Id,
                                           IndentNo = e.IndentNo,
                                           EmployeeName = f.Name,
                                           ItemGroupName = e.ItemGroupName,
                                           ItemName = e.ItemName,
                                           IssueQuantity = e.IssueQuantity
                                       }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();


                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Item_Group_Name.Trim() == null && Indent_Number.Trim() == null && SubContractor_Name.Trim() == null)
            {
                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                            join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                            where e.ProjectID == ProjectId
                            orderby e.CreatedDate descending
                            select new IssueReport
                            {
                                CreatedDate = e.CreatedDate,
                                Id = e.Id,
                                IndentNo = e.IndentNo,
                                EmployeeName = f.Name,
                                ItemGroupName = e.ItemGroupName,
                                ItemName = e.ItemName,
                                IssueQuantity = e.IssueQuantity
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();
                
                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else if (Indent_Number.Trim() != null && Indent_Number != "" && Item_Group_Name.Trim() != null && Item_Group_Name != "" && SubContractor_Name.Trim() != null && SubContractor_Name != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                            join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                            where e.IndentNo == Indent_Number && e.ItemGroupID == Item_Group_Name && e.PcContractorId == SubContractor_Name && e.ProjectID == ProjectId && e.CreatedDate >= date1 && e.CreatedDate <= date2
                            orderby e.CreatedDate descending
                            select new IssueReport
                            {
                                CreatedDate = e.CreatedDate,
                                Id = e.Id,
                                IndentNo = e.IndentNo,
                                EmployeeName = f.Name,
                                ItemGroupName = e.ItemGroupName,
                                ItemName = e.ItemName,
                                IssueQuantity = e.IssueQuantity
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();


                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Item_Group_Name.Trim() != null && SubContractor_Name.Trim() != null && Indent_Number.Trim() != null && Item_Group_Name.Trim() != "" && SubContractor_Name.Trim() != "" && Indent_Number.Trim() != "")
            {
                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                            join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                            where e.ProjectID == ProjectId && e.PcContractorId == SubContractor_Name && e.ItemGroupID == Item_Group_Name && e.IndentNo == Indent_Number
                            orderby e.CreatedDate descending
                            select new IssueReport
                            {
                                CreatedDate = e.CreatedDate,
                                Id = e.Id,
                                IndentNo = e.IndentNo,
                                EmployeeName = f.Name,
                                ItemGroupName = e.ItemGroupName,
                                ItemName = e.ItemName,
                                IssueQuantity = e.IssueQuantity
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();


                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else if (Item_Group_Name.Trim() != null && Item_Group_Name != "")
            {
                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                            join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                            where e.ProjectID == ProjectId && e.ItemGroupID == Item_Group_Name
                            orderby e.CreatedDate descending
                            select new IssueReport
                            {
                                CreatedDate = e.CreatedDate,
                                Id = e.Id,
                                IndentNo = e.IndentNo,
                                EmployeeName = f.Name,
                                ItemGroupName = e.ItemGroupName,
                                ItemName = e.ItemName,
                                IssueQuantity = e.IssueQuantity
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();

               
                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (SubContractor_Name.Trim() != null && SubContractor_Name != "")
            {
                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                            join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                            where e.ProjectID == ProjectId && e.PcContractorId == SubContractor_Name
                            orderby e.CreatedDate descending
                            select new IssueReport
                            {
                                CreatedDate = e.CreatedDate,
                                Id = e.Id,
                                IndentNo = e.IndentNo,
                                EmployeeName = f.Name,
                                ItemGroupName = e.ItemGroupName,
                                ItemName = e.ItemName,
                                IssueQuantity = e.IssueQuantity
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();

                
                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Indent_Number.Trim() != null && Indent_Number != "")
            {
                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                            join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                            where e.ProjectID == ProjectId && e.IndentNo == Indent_Number
                            orderby e.CreatedDate descending
                            select new IssueReport
                            {
                                CreatedDate = e.CreatedDate,
                                Id = e.Id,
                                IndentNo = e.IndentNo,
                                EmployeeName = f.Name,
                                ItemGroupName = e.ItemGroupName,
                                ItemName = e.ItemName,
                                IssueQuantity = e.IssueQuantity
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();


                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                            join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                            where e.ProjectID == ProjectId && e.CreatedDate >= date1 && e.CreatedDate <= date2
                            orderby e.CreatedDate descending
                            select new IssueReport
                            {
                                CreatedDate = e.CreatedDate,
                                Id = e.Id,
                                IndentNo = e.IndentNo,
                                EmployeeName = f.Name,
                                ItemGroupName = e.ItemGroupName,
                                ItemName = e.ItemName,
                                IssueQuantity = e.IssueQuantity
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();

                
                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (SubContractor_Name.Trim() != null && SubContractor_Name != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                            join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                            where e.PcContractorId == SubContractor_Name && e.ProjectID == ProjectId && e.CreatedDate >= date1 && e.CreatedDate <= date2
                            orderby e.CreatedDate descending
                            select new IssueReport
                            {
                                CreatedDate = e.CreatedDate,
                                Id = e.Id,
                                IndentNo = e.IndentNo,
                                EmployeeName = f.Name,
                                ItemGroupName = e.ItemGroupName,
                                ItemName = e.ItemName,
                                IssueQuantity = e.IssueQuantity
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();
                

                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Item_Group_Name.Trim() != null && Item_Group_Name != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                            join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                            where e.ItemGroupID == Item_Group_Name && e.ProjectID == ProjectId && e.CreatedDate >= date1 && e.CreatedDate <= date2
                            orderby e.CreatedDate descending
                            select new IssueReport
                            {
                                CreatedDate = e.CreatedDate,
                                Id = e.Id,
                                IndentNo = e.IndentNo,
                                EmployeeName = f.Name,
                                ItemGroupName = e.ItemGroupName,
                                ItemName = e.ItemName,
                                IssueQuantity = e.IssueQuantity
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();

                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Indent_Number.Trim() != null && Indent_Number != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                            join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                            where e.IndentNo == Indent_Number && e.ProjectID == ProjectId && e.CreatedDate >= date1 && e.CreatedDate <= date2
                            orderby e.CreatedDate descending
                            select new IssueReport
                            {
                                CreatedDate = e.CreatedDate,
                                Id = e.Id,
                                IndentNo = e.IndentNo,
                                EmployeeName = f.Name,
                                ItemGroupName = e.ItemGroupName,
                                ItemName = e.ItemName,
                                IssueQuantity = e.IssueQuantity
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();

                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Item_Group_Name.Trim() != null && Item_Group_Name != "" && SubContractor_Name.Trim() != null && SubContractor_Name != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.ns_tbl_IssueQuantity
                            join f in objmms.tblPcContractorMasters on e.PcContractorId equals f.PcContractorID
                            where e.ItemGroupID == Item_Group_Name && e.PcContractorId == SubContractor_Name && e.ProjectID == ProjectId && e.CreatedDate >= date1 && e.CreatedDate <= date2
                            orderby e.CreatedDate descending
                            select new IssueReport
                            {
                                CreatedDate = e.CreatedDate,
                                Id = e.Id,
                                IndentNo = e.IndentNo,
                                EmployeeName = f.Name,
                                ItemGroupName = e.ItemGroupName,
                                ItemName = e.ItemName,
                                IssueQuantity = e.IssueQuantity
                            }).OrderByDescending(k => k.CreatedDate).Take(200).ToList();


                //ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                //ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else {

            }
            return PartialView("IssueReport", data_rrm);
        }
        public ActionResult ItemIssueReport()
        {
            ViewBag.EmpID = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            return View();
        }

        public ActionResult itemIssueGrid(string PrjId, string IG, string IT, string FromDate, string ToDate)
        {
            List<ItemIssueReport> issurpt = null;
            var data = issurpt;

            if (PrjId != "" && IG != "" && IT != "" && PrjId != null && IG != null && IT != null && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data = (from x in objmms.ns_tbl_IssueQuantity
                        join x1 in objmms.tblUnitMasters on x.UnitID equals x1.UnitID 
                        where x.ProjectID == PrjId && x.ItemGroupID == IG && x.ItemID == IT && x.CreatedDate >= date1 && x.CreatedDate <= date2
                        orderby x.ItemName
                        select new ItemIssueReport
                        {
                            IssueNo = x.Id.ToString(),
                            Issuedate = x.CreatedDate,
                            reqIntendentNo = x.IndentNo,
                            Indentdate = x.CreatedDate,
                            IssueTo = x.EmployeeName,
                            ItemName = x.ItemName,
                            UnitName = x1.UnitName,
                            Quantity = x.IssueQuantity
                        }).ToList();

                TempData["printexcel1"] = data;
                TempData.Keep("printexcel1");
            }
            else if (PrjId != "" && IG != "" && IT != "" && PrjId != null && IG != null && IT != null)
            {
                data = (from x in objmms.ns_tbl_IssueQuantity
                        join x1 in objmms.tblUnitMasters on x.UnitID equals x1.UnitID
                        where x.ProjectID == PrjId && x.ItemGroupID == IG && x.ItemID == IT
                        orderby x.ItemName
                        select new ItemIssueReport
                        {
                            IssueNo = x.Id.ToString(),
                            Issuedate = x.CreatedDate,
                            reqIntendentNo = x.IndentNo,
                            Indentdate = x.CreatedDate,
                            IssueTo = x.EmployeeName,
                            ItemName = x.ItemName,
                            UnitName = x1.UnitName,
                            Quantity = x.IssueQuantity
                        }).ToList();

                TempData["printexcel1"] = data;
                TempData.Keep("printexcel1");
            }
            else if (PrjId != "" && IG != "" && PrjId != null && IG != null && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data = (from x in objmms.ns_tbl_IssueQuantity
                        join x1 in objmms.tblUnitMasters on x.UnitID equals x1.UnitID
                        where x.ProjectID == PrjId && x.ItemGroupID == IG && x.CreatedDate >= date1 && x.CreatedDate <= date2
                        orderby x.ItemName
                        select new ItemIssueReport
                        {
                            IssueNo = x.Id.ToString(),
                            Issuedate = x.CreatedDate,
                            reqIntendentNo = x.IndentNo,
                            Indentdate = x.CreatedDate,
                            IssueTo = x.EmployeeName,
                            ItemName = x.ItemName,
                            UnitName = x1.UnitName,
                            Quantity = x.IssueQuantity
                        }).ToList();

                TempData["printexcel1"] = data;
                TempData.Keep("printexcel1");
            }
            else if (PrjId != "" && IG != "" && PrjId != null && IG != null)
            {
                data = (from x in objmms.ns_tbl_IssueQuantity
                        join x1 in objmms.tblUnitMasters on x.UnitID equals x1.UnitID
                        where x.ProjectID == PrjId && x.ItemGroupID == IG
                        orderby x.ItemName
                        select new ItemIssueReport
                        {
                            IssueNo = x.Id.ToString(),
                            Issuedate = x.CreatedDate,
                            reqIntendentNo = x.IndentNo,
                            Indentdate = x.CreatedDate,
                            IssueTo = x.EmployeeName,
                            ItemName = x.ItemName,
                            UnitName = x1.UnitName,
                            Quantity = x.IssueQuantity
                        }).ToList();

                TempData["printexcel1"] = data;
                TempData.Keep("printexcel1");
            }
            else if (PrjId != "" && IG == "" && IT == "" && FromDate != "" && ToDate != "" && PrjId != null && IG == null && IT == null && FromDate != null && ToDate != null)
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data = (from x in objmms.ns_tbl_IssueQuantity
                        join x1 in objmms.tblUnitMasters on x.UnitID equals x1.UnitID
                        where x.ProjectID == PrjId && x.CreatedDate >= date1 && x.CreatedDate <= date2
                        orderby x.ItemName
                        select new ItemIssueReport
                        {
                            IssueNo = x.Id.ToString(),
                            Issuedate = x.CreatedDate,
                            reqIntendentNo = x.IndentNo,
                            Indentdate = x.CreatedDate,
                            IssueTo = x.EmployeeName,
                            ItemName = x.ItemName,
                            UnitName = x1.UnitName,
                            Quantity = x.IssueQuantity
                        }).ToList();

                TempData["printexcel1"] = data;
                TempData.Keep("printexcel1");
            }
            return PartialView("ItemIssueGrid", data);
        }
        public FileStreamResult GETPdfIssueReport()
        {
            List<ItemIssueReport> allCust = (List<ItemIssueReport>)TempData["printexcel1"];

            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();


            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                       grid1.Column("IssueNo", header: "Issue No"),
                       grid1.Column("Issuedate", header: "Issue Date ", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Issuedate)),
                       grid1.Column("reqIntendentNo", header: "Req Indent No"),
                       grid1.Column("Indentdate", header: "Indent Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Indentdate)),
                       grid1.Column("IssueTo", header: "Issue to"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("Quantity", header: "Qty",style:"right")
                        )
                    ).ToString();

           
            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "ISSUE REPORT";
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

        public void GetExcelIssueReport()
        {
            List<ItemIssueReport> allCust = (List<ItemIssueReport>)TempData["printexcel1"];

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                    grid1.Column("IssueNo", header: "Issue No"),
                       grid1.Column("Issuedate", header: "Issue Date "),
                       grid1.Column("reqIntendentNo", header: "Req Indent No"),
                       grid1.Column("Indentdate", header: "Indent Date"),
                       grid1.Column("IssueTo", header: "Issue to"),
                       grid1.Column("ItemName", header: "Item Name"),
                       grid1.Column("UnitName", header: "Unit"),
                       grid1.Column("Quantity", header: "Qty")
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=VendorWiseReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }
    }
}