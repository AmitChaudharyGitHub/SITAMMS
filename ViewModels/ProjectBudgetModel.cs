using MMS.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class ProjectBudgetModel
    {

    }

    public class BudgetItems
    {
        public string ItemName { get; set; }
        [Required]
        public string ItemID { get; set; }
        public string ItemUnit { get; set; }
        public List<string> Years { get; set; }
    }

    public class ProjectYearBudget
    {
        public List<tbl_ProjectYearBudget> YearBudget { get; set; }
    }

    public class ProjectYearBudgetReport : BudgetItems
    {
        public List<tbl_ProjectYearBudget> BudgetItemData { get; set; }
        public List<tbl_ProjectMonthBudget> BudgetMonthItem { get; set; }
    }

    public class ProjectMonthBudget
    {
        public List<tbl_ProjectMonthBudget> MonthBudget { get; set; }
    }

    public class YearBudgetItems
    {
        public string ItemName { get; set; }
        public string ItemID { get; set; }
        public int BudgetYear { get; set; }
        public decimal? ItemQty { get; set; }
        public decimal? ItemRate { get; set; }
        public decimal? TotalAmt { get; set; }
        public string ItemUnit { get; set; }
        public int BudgetMonth { get; set; }
    }

}