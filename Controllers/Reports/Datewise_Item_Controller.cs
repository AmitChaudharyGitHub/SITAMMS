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
using iTextSharp.text.pdf;
using System.Text;

namespace MMS.Controllers.Reports
{
    public class Datewise_Item_Controller : Controller
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

            Datewise_Item_Model rrm = new Datewise_Item_Model();

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

            //code for bind PO No accoding project
            var a1 = objmms.GateEntries.Where(b1 => b1.ProjectNo == ProjectId).OrderBy(c1 => c1.StatusTypeNo).ToList();
            var t1 = a1.Distinct<GateEntry>().Select(x1 => new SelectListItem
            {
                Value = x1.StatusTypeNo?.ToString().Trim(),
                Text = x1.StatusTypeNo?.ToString().Trim()
            });
            ViewBag.PONumber = t1;

            var data_from_table = (from e in objmms.GateEntryChilds
                                   where e.ProjectNo == ProjectId
                                   orderby e.Date
                                   select new Datewise_Item_Model
                                   {
                                       Date = e.Date,
                                       GateEntryNo = e.GateEntryNo,
                                       Vendor = e.Vendor,
                                       BillNo = "",
                                      /* BillDate = ""*/
                                       StatusTypeNo = e.StatusTypeNo,
                                       CreatedDate = e.CreatedDate,
                                       ItemNo = e.ItemNo,
                                       ItemGroup = e.ItemGroup,
                                       Item = e.Item,
                                       ReceiveQty = e.ReceiveQty,
                                       Unit = e.Unit,
                                       Rate = e.Rate,
                                       TaxAmount = e.TaxAmount,
                                       DeliveryAmount = e.DeliveryAmount,
                                       AllAmount = e.AllAmount,
                                       Amount = e.Amount
                                   }).OrderByDescending(k => k.Date).Take(200).ToList();

            rrm.Datewise_Item = data_from_table;
            TempData["printexcel"] = data_from_table;
            Session["keepdata"] = data_from_table;
            TempData.Keep("printexcel");
            return View("Index", rrm);
        }


        public ActionResult Datewise_Item_Search(string Item_Group_Name, string Vendors_Name, string PONumber, string FromDate, string ToDate)
        {
            string ProjectId = Session["ProId"].ToString();
            List<Datewise_Item_Model> rrm = null;
            var data_rrm = rrm;

            if (PONumber != null && PONumber != "" && Item_Group_Name != null && Item_Group_Name !="" && Vendors_Name != null && Vendors_Name != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);


                data_rrm = (from e in objmms.GateEntryChilds
                            where e.ProjectNo == ProjectId && e.StatusTypeNo == PONumber && e.VendorNo == Vendors_Name && e.ItemGroupNo == Item_Group_Name && e.Date >= date1 && e.Date <= date2
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Item_Group_Name != null && Item_Group_Name != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);


                data_rrm = (from e in objmms.GateEntryChilds
                            where e.ProjectNo == ProjectId && e.ItemGroupNo == Item_Group_Name && e.Date >= date1 && e.Date <= date2
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else if (Vendors_Name != null && Vendors_Name != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);


                data_rrm = (from e in objmms.GateEntryChilds
                            where e.ProjectNo == ProjectId && e.VendorNo == Vendors_Name && e.Date >= date1 && e.Date <= date2
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else if (PONumber != null && PONumber != "" && FromDate != null && ToDate != null && FromDate != "" && ToDate != "")
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);


                data_rrm = (from e in objmms.GateEntryChilds
                            where e.ProjectNo == ProjectId && e.StatusTypeNo == PONumber && e.Date >= date1 && e.Date <= date2
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
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
                            where e.ProjectNo == ProjectId && e.Date >= date1 && e.Date <= date2
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Item_Group_Name.Trim() == null && Item_Group_Name == "" && Vendors_Name.Trim() == null && Vendors_Name.Trim() == "" && PONumber.Trim() == null && PONumber.Trim() == "")
            {
                data_rrm = (from e in objmms.GateEntryChilds
                            where e.ProjectNo == ProjectId
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else if (Item_Group_Name.Trim() != null && Item_Group_Name.Trim() != "")
            {
                data_rrm = (from e in objmms.GateEntryChilds
                            where e.ItemGroupNo == Item_Group_Name && e.ProjectNo == ProjectId
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
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
                            where e.VendorNo == Vendors_Name && e.ProjectNo == ProjectId
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else if (PONumber.Trim() != null && PONumber != "")
            {
                data_rrm = (from e in objmms.GateEntryChilds
                            where e.StatusTypeNo == PONumber && e.ProjectNo == ProjectId
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Vendors_Name.Trim() != null && Vendors_Name != "" && FromDate != null && ToDate != null)
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.GateEntryChilds
                            where e.VendorNo == Vendors_Name && e.ProjectNo == ProjectId && e.Date >= date1 && e.Date < date2
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Item_Group_Name.Trim() != null && Item_Group_Name != "" && FromDate != null && ToDate != null)
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.GateEntryChilds
                            where e.ItemGroupNo == Item_Group_Name && e.ProjectNo == ProjectId && e.Date >= date1 && e.Date < date2
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (PONumber.Trim() != null && PONumber != "" && FromDate != null && ToDate != null)
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.GateEntryChilds
                            where e.StatusTypeNo == PONumber && e.ProjectNo == ProjectId && e.Date >= date1 && e.Date < date2
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }
            else if (Item_Group_Name.Trim() != null && Item_Group_Name != "" && Vendors_Name.Trim() != null && Vendors_Name != "" && PONumber.Trim() != null && PONumber != "" && FromDate != null && ToDate != null)
            {
                DateTime date1 = Convert.ToDateTime(FromDate);
                DateTime date2 = Convert.ToDateTime(ToDate);

                data_rrm = (from e in objmms.GateEntryChilds
                            where e.StatusTypeNo == PONumber && e.ItemGroupNo == Item_Group_Name && e.VendorNo == Vendors_Name && e.ProjectNo == ProjectId && e.Date >= date1 && e.Date < date2
                            orderby e.Date
                            select new Datewise_Item_Model
                            {
                                Date = e.Date,
                                GateEntryNo = e.GateEntryNo,
                                Vendor = e.Vendor,
                                BillNo = "",
                                /* BillDate = ""*/
                                StatusTypeNo = e.StatusTypeNo,
                                CreatedDate = e.CreatedDate,
                                ItemNo = e.ItemNo,
                                ItemGroup = e.ItemGroup,
                                Item = e.Item,
                                ReceiveQty = e.ReceiveQty,
                                Unit = e.Unit,
                                Rate = e.Rate,
                                TaxAmount = e.TaxAmount,
                                DeliveryAmount = e.DeliveryAmount,
                                AllAmount = e.AllAmount,
                                Amount = e.Amount
                            }).OrderByDescending(k => k.Date).ToList();
                ViewBag.displySumqty = data_rrm.Sum(k => k.ReceiveQty);
                ViewBag.displySumamt = data_rrm.Sum(k => k.Amount);
                TempData["printexcel"] = data_rrm;
                Session["keepdata"] = data_rrm;
                TempData.Keep("printexcel");
            }

            else {

            }
            return PartialView("_Datewise_Item_Partial", data_rrm);
        }
        public void GetExcel()
        {
            List<Datewise_Item_Model> allCust = (List<Datewise_Item_Model>)Session["keepdata"]; 

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid1.GetHtml(
                columns: grid1.Columns(
                          grid1.Column("Date", header: "Date"),
                       grid1.Column("GateEntryNo", header: "GRN No."),
                       grid1.Column("Vendor", header: "Name of Vendor"),
                       //grid1.Column("ItemNo", header: "Item No"),
                       //grid1.Column("BillNo", header: "Bill No."),
                       grid1.Column("StatusTypeNo", header: "PO No. "),
                       grid1.Column("CreatedDate", header: "PO Date."),
                       grid1.Column("ItemGroup", header: "Item Group Name"),
                       grid1.Column("Item", header: "Item Name"),
                       grid1.Column("ReceiveQty", header: "Quantity"),
                       grid1.Column("Rate", header: "Rate"),
                       grid1.Column("TaxAmount", header: "Tax Amount"),
                       grid1.Column("DeliveryAmount", header: "Freight amount"),
                       grid1.Column("Amount", header: "Amount"),
                       grid1.Column("AllAmount", header: "Total Amount")
                        )
                    ).ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=VendorWiseReport.xls");
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }
        public FileStreamResult VendorWiseReceiptPdf()
        {
            List<Datewise_Item_Model> allCust = (List<Datewise_Item_Model>)Session["keepdata"];

            string pid = Session["ProId"].ToString();

            var a = objmms.tblProjectMasters.Where(b => b.PRJID == pid).OrderByDescending(c => c.ProjectName).Take(1).ToList().Select(x => x.ProjectName).First();

            WebGrid grid1 = new WebGrid(source: allCust, canPage: false, canSort: false);
            string gridHtml = grid1.GetHtml(
                    columns: grid1.Columns(
                         grid1.Column("Date", header: "Date", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.Date)),
                       grid1.Column("GateEntryNo", header: "GRN No."),
                       grid1.Column("Vendor", header: "Vendor Name"),
                       //grid1.Column("ItemNo", header: "Item No"),
                       //grid1.Column("BillNo", header: "Bill No."),
                       grid1.Column("StatusTypeNo", header: "PO No. "),
                       grid1.Column("CreatedDate", header: "PO Date.", format: (item) => string.Format("{0:dd-MMM-yyyy}", item.CreatedDate)),
                       grid1.Column("ItemGroup", header: "Group Name"),
                       grid1.Column("Item", header: "Item Name"),
                       grid1.Column("ReceiveQty", header: "Qty", style: "right"),
                       grid1.Column("Rate", header: "Rate", style: "right"),
                       grid1.Column("TaxAmount", header: "Tax Amount", style: "right"),
                       grid1.Column("DeliveryAmount", header: "Freight Amt", style: "right"),
                       grid1.Column("Amount", header: "Amt",style:"right"),
                       grid1.Column("AllAmount", header: "Tot Amt", style: "right")
                        )
                    ).ToString();

            StringBuilder DataString = new StringBuilder();
            string header = "AHLUWALIA CONTRACTS (INDIA) LTD.";
            string siteName = "Site-" + a;
            string reportName = "Vendor Wise Receipt";
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