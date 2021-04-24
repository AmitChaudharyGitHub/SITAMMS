using MMS.DAL;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class ProjectBudgetController : Controller
    {
        private MMSEntities objmms = new MMSEntities();
        Procedure objpro = new Procedure();
        // GET: ProjectBudget
        public ActionResult YearBudget()
        {
            return View();
        }

        public ActionResult MonthBudget()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetItems(string ItemGroup, int Months, string ProjectID)
        {
            List<BudgetItems> items_year = objmms.tblItemMasters.Where(s => s.ItemGroupID == ItemGroup).Select(s => new BudgetItems()
            { ItemID = s.ItemID, ItemName = s.ItemName, ItemUnit=objmms.tblUnitMasters.Where(u=>u.UnitID==s.UnitID).Select(u => u.UnitName).FirstOrDefault() }).ToList();
            int totalyears = Months / 12;
            ViewBag.TotalYears = totalyears;
            ViewBag.ProjectID = ProjectID;

            return PartialView("_PartialBudgetGrid", items_year);
        }

        [HttpPost]
        public ActionResult SaveProjectYearBudget(ProjectYearBudget budget)
        {
            string msg;
            List<tbl_ProjectYearBudget> yearsBudget = budget.YearBudget.Where(s => s.ItemQty > 0 && s.ItemRate > 0).Select(s => s).ToList();
            foreach (var budgetitem in yearsBudget)
            {
                if (objmms.tbl_ProjectYearBudget.Count(s => s.ItemID == budgetitem.ItemID && s.BudgetYear == budgetitem.BudgetYear) == 0)
                {
                    budgetitem.Createdby = Session["EmpId"].ToString();
                    budgetitem.CreatedOn = DateTime.Now;
                    if (objmms.tbl_ProjectMonthBudget.Count(s => s.ItemID == budgetitem.ItemID && s.BudgetYear == budgetitem.BudgetYear && s.ProjectID == budgetitem.ProjectID) == 0)
                    {
                        objmms.tbl_ProjectYearBudget.Add(budgetitem);
                    }
                }
            }
            int res = objmms.SaveChanges();

            if (res > 0)
                msg = "Data successfully saved!";
            else
                msg = "No Data saved!";

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveProjectMonthBudget(ProjectMonthBudget budget)
        {
            string msg = "";
            List<tbl_ProjectMonthBudget> monthbudget = budget.MonthBudget.Where(s => s.ItemQty > 0 && s.ItemRate > 0).Select(s => s).ToList();
            foreach (var budgetitem in monthbudget)
            {
                budgetitem.Createdby = Session["EmpId"].ToString();
                budgetitem.CreatedOn = DateTime.Now;
                if (objmms.tbl_ProjectMonthBudget.Count(s => s.ItemID == budgetitem.ItemID && s.BudgetYear == budgetitem.BudgetYear && s.ProjectID == budgetitem.ProjectID && s.BudgetMonth==budgetitem.BudgetMonth) == 0)
                {
                    objmms.tbl_ProjectMonthBudget.Add(budgetitem);
                }
            }

            int res = objmms.SaveChanges();

            if (res > 0)
                msg = "Data successfully saved!";
            else
                msg = "No Data saved!";

            return Json(msg);
        }

        public ActionResult YearBudgetReport()
        {
            return View();
        }

        public ActionResult MonthBudgetReport()
        {
            return View();
        }

        //********** Year Budget Report**********//
        public ActionResult GetYearProjectBudget(string ProjectID, string ItemGroup, int FromYear, int ToYear)
        {
            ViewBag.FromYear = FromYear;
            ViewBag.ToYear = ToYear;
            List<ProjectYearBudgetReport> budgetData = new List<ProjectYearBudgetReport>();
            var budget = objpro.GetProjectYearBudgetData(ProjectID, ItemGroup, FromYear, ToYear);
            foreach (var item in budget)
            {
                if (budgetData.Count(s => s.ItemID == item.ItemID) == 0)
                    budgetData.Add(new ProjectYearBudgetReport() { ItemName = item.ItemName, ItemID = item.ItemID, ItemUnit= item.ItemUnit});
            }
            foreach (var itemBudget in budgetData)
            {
                List<tbl_ProjectYearBudget> itemyearbudget = new List<tbl_ProjectYearBudget>();
                itemyearbudget = budget.Where(s => s.ItemID == itemBudget.ItemID).Select(s => new tbl_ProjectYearBudget()
                {
                    BudgetYear = s.BudgetYear.ToString(),
                    ItemQty = s.ItemQty,
                    ItemRate = s.ItemRate,
                    TotalAmt = s.TotalAmt
                }).ToList();

                int objIndex = budgetData.FindIndex(s => s.ItemID == itemBudget.ItemID);
                budgetData[objIndex].BudgetItemData = itemyearbudget;

            }

            return PartialView("_PartialYearItemBudget", budgetData);
        }

        //*********Get Year Budget Data For Enter Month Budget**************//
        public ActionResult GetYearBudget(string ProjectID, string ItemGroup, int FromYear, int ToYear)
        {
            ViewBag.FromYear = FromYear;
            ViewBag.ProjectID = ProjectID;
            List<ProjectYearBudgetReport> budgetData = new List<ProjectYearBudgetReport>();
            var budget = objpro.GetProjectYearBudgetData(ProjectID, ItemGroup, FromYear, ToYear);
            foreach (var item in budget)
            {
                string bYear = FromYear.ToString();
                if (budgetData.Count(s => s.ItemID == item.ItemID) == 0  && objmms.tbl_ProjectMonthBudget.Count(s=>s.ItemID==item.ItemID && s.BudgetYear== bYear) ==0)
                    budgetData.Add(new ProjectYearBudgetReport() { ItemName = item.ItemName, ItemID = item.ItemID, ItemUnit = item.ItemUnit });
            }

            if (budgetData.Count() == 0)
                //ViewBag.Error = "Data Already Submitted.";
                return Json(new { Error = "Data Already Submitted." }, JsonRequestBehavior.AllowGet);
            foreach (var itemBudget in budgetData)
            {
                List<tbl_ProjectYearBudget> itemyearbudget = new List<tbl_ProjectYearBudget>();
                itemyearbudget = budget.Where(s => s.ItemID == itemBudget.ItemID).Select(s => new tbl_ProjectYearBudget()
                {
                    BudgetYear = s.BudgetYear.ToString(),
                    ItemQty = s.ItemQty,
                    ItemRate = s.ItemRate,
                    TotalAmt = s.TotalAmt
                }).ToList();

                int objIndex = budgetData.FindIndex(s => s.ItemID == itemBudget.ItemID);
                budgetData[objIndex].BudgetItemData = itemyearbudget;

            }

            return PartialView("_PartialBudgetMonthGrid", budgetData);
        }

        //********Month Budget Report*******//
        public ActionResult GetMonthBudget(string ProjectID, string ItemGroup, int Year)
        {
            ViewBag.FromYear = Year;
            ViewBag.ProjectID = ProjectID;
            List<ProjectYearBudgetReport> budgetData = new List<ProjectYearBudgetReport>();
            var budget = objpro.GetProjectYearBudgetData(ProjectID, ItemGroup, Year, 0);
            foreach (var item in budget)
            {
                if (budgetData.Count(s => s.ItemID == item.ItemID) == 0)
                    budgetData.Add(new ProjectYearBudgetReport() { ItemName = item.ItemName, ItemID = item.ItemID, ItemUnit = item.ItemUnit });
            }
            foreach (var itemBudget in budgetData)
            {
                List<tbl_ProjectYearBudget> itemyearbudget = new List<tbl_ProjectYearBudget>();

                itemyearbudget = budget.Where(s => s.ItemID == itemBudget.ItemID && s.BudgetYear==Year).Select(s => new tbl_ProjectYearBudget()
                {
                    BudgetYear = s.BudgetYear.ToString(),
                    ItemQty = s.ItemQty,
                    ItemRate = s.ItemRate,
                    TotalAmt = s.TotalAmt
                }).ToList();

                int objIndex = budgetData.FindIndex(s => s.ItemID == itemBudget.ItemID);
                budgetData[objIndex].BudgetItemData = itemyearbudget;
                var monthitembudget= objpro.GetProjectMonthBudgetData(ProjectID, Year, itemBudget.ItemID);
                budgetData[objIndex].BudgetMonthItem = monthitembudget.Select(s => new tbl_ProjectMonthBudget() { ItemQty = s.ItemQty, ItemRate = s.ItemRate, TotalAmt = s.TotalAmt, BudgetMonth = s.BudgetMonth.ToString() }).ToList();

            }

            return PartialView("_PartialMonthBudget", budgetData);
        }


    }
}