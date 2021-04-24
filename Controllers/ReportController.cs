using ClosedXML.Excel;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class ReportController : Controller
    {
        DAL.MMSEntities objmms = new DAL.MMSEntities();

        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BinCard()
        {
            return View();
        }

        public ActionResult ItemPurchaseReport()
        {
            
            return View();
        }

        public JsonResult GetStates()
        {
            var states=objmms.tblStates.Select(x => new { Value = x.StateID, Text = x.StateName }).OrderBy(x => x.Text).ToList();
            return Json(states.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectsByState(string StateId)
        {
            var projects = (from a in objmms.tblProjectMasters
                          join b in objmms.tblProjectGSTNoes on a.PRJID equals b.ProjectId
                          join c in objmms.tblStates on b.StateId equals c.StateID
                          where c.StateID==StateId
                          select new { Text = a.ProjectName, Value = a.PRJID }
                          ).OrderBy(x => x.Text).ToList();
            return Json(projects.ToJSON(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetItemsByProjectId(string ProjectId)
        {
            var items = (from a in objmms.tblItemCurrentStocks
                         join b in objmms.tblItemMasters on a.ItemNo equals b.ItemID
                         where a.ProjectNo == ProjectId
                         select new { Text = b.ItemName, Value = a.ItemNo }).OrderBy(x => x.Text).ToList();
            return Json(items.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItems()
        {
            var items = (from a in objmms.tblItemMasters
                         where a.ItemID!=null && a.ItemName!=null
                         select new { Text = a.ItemName.Trim(), Value = a.ItemID }).OrderBy(x => x.Text).ToList();
            return Json(items.ToJSON(), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ItemPurchaseReportData(string StateId,string ProjectId,string ItemId,string ItemCode,string FromDate,string ToDate)
        {
           if(StateId!="" && (ItemId!="" || ItemCode != ""))
            {
                try
                {
                    if(FromDate!="" && ToDate != "")
                    {
                        FromDate = DateTime.ParseExact(FromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        ToDate = DateTime.ParseExact(ToDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        FromDate = "";
                        ToDate = "";
                    }
                    var data = Procedure.GetData<VMItemPurchaseReport>("GetItemPurchaseReport",StateId,ProjectId,ItemId,ItemCode,FromDate,ToDate);
                    var json = Json(new { Status = 1, Data = data.ToJSON() }, JsonRequestBehavior.AllowGet);
                    json.MaxJsonLength = 99999999;
                    return json;
                }
                catch (Exception ex)
                {

                    return Json(new { Status = 3,Error=ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
           else
            {
                return Json(new {Status=2 }, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult ItemPurchaseReportExcel(string s, string p, string i, string ic, string f, string t)
        {
            if (s != ""  && (i != "" || ic != ""))
            {
                try
                {
                    if (f != "" && t != "")
                    {
                        f = DateTime.ParseExact(f, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        t = DateTime.ParseExact(t, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        f = "";
                        t = "";
                    }
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("stateId",s);
                    param.Add("projectId",p);
                    param.Add("itemId",i);
                    param.Add("itemCode",ic);
                    param.Add("fromDate",f);
                    param.Add("toDate",t);

                    var data = Procedure.GetDataBySP("GetItemPurchaseReport",param);

                    var workbook = new XLWorkbook();
                    var sheet1 = workbook.AddWorksheet(data, "ItemPurchaseReport");


                    var row = sheet1.Row(1).InsertRowsAbove(3).ToList();
                    var rc1 = row[0].Cell(1);
                    var rc2 = row[1].Cell(1);

                    rc1.Style.Font.FontSize = 23;
                    rc2.Style.Font.FontSize = 22;

                    rc1.Value = "AHLUWALIA CONTRACTS (INDIA) LTD.";
                    rc2.Value = "Item Purchase Report";

                    string filename = Guid.NewGuid().ToString();

                    workbook.SaveAs(System.IO.Path.GetTempPath() + filename + ".xlsx");
                    return File(System.IO.Path.GetTempPath() + filename + ".xlsx", "application/vnd.ms-excel", "ItemPurchaseReport.xlsx");

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
        }



    }
}