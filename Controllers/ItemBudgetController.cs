using DataAccessLayer;
using MMS.DAL;
using MMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class ItemBudgetController : Controller
    {
        MMSEntities mmsObj = new MMSEntities();
        Procedure procedure = new Procedure();
        FactoryManager m = new FactoryManager();
        MSP_Model objmsps = new MSP_Model();
        // GET: ItemBudget
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            string empId = Session["EmpID"].ToString();
            ViewBag.EmpId = empId;
            return View();
        }



        public PartialViewResult GetItemBudgets(string ProjectId,string ItemGroupId,string ItemCode,string FinancialYear)
        {
            ItemCode = ItemCode == "" ? null : ItemCode;
            FinancialYear = FinancialYear == "" ? GetCurrentFinancialYear(DateTime.Now) : FinancialYear;

            var data = procedure.GetBudgetData(ProjectId,ItemGroupId,ItemCode, FinancialYear);
            return PartialView("_ItemBudgetsGrid", data);
        }

        public JsonResult GetFinancialYears(string ProjectId)
        {
            var data =mmsObj.tblItemBudgets.Where(x=>x.ProjectId==ProjectId).Distinct().Select(x=>new {Text=x.FinancialYear,Value=x.FinancialYear}).ToList();
            return Json(data.ToJSON(), JsonRequestBehavior.AllowGet);
        }

        


        [HttpPost]
        public JsonResult UpdateBudgetForItem(string[] PostData)
        {
            try
            {

                if (PostData != null && PostData.Length > 0)
                {
                    var ProjectId = PostData[7];
                    var ItemCode = PostData[1];
                    var budget = mmsObj.tblItemBudgets.SingleOrDefault(x => x.ProjectId == ProjectId && x.ItemCode == ItemCode);
                    if (budget!=null)
                    {
                        budget.TotalQuantity =Convert.ToDecimal(PostData[2]);
                        budget.Rate = Convert.ToDecimal(PostData[3]);
                        budget.Amount = Convert.ToDecimal(PostData[4]);
                        budget.ExcessPercentage = Convert.ToDecimal(PostData[5]);
                        budget.ExcessQuantity = Convert.ToDecimal(PostData[6]);
                        budget.Apr = Convert.ToDecimal(PostData[8]);
                        budget.May = Convert.ToDecimal(PostData[9]);
                        budget.June = Convert.ToDecimal(PostData[10]);
                        budget.July = Convert.ToDecimal(PostData[11]);
                        budget.Aug = Convert.ToDecimal(PostData[12]);
                        budget.Sep = Convert.ToDecimal(PostData[13]);
                        budget.Oct = Convert.ToDecimal(PostData[14]);
                        budget.Nov = Convert.ToDecimal(PostData[15]);
                        budget.Dec = Convert.ToDecimal(PostData[16]);
                        budget.Jan = Convert.ToDecimal(PostData[17]);
                        budget.Feb = Convert.ToDecimal(PostData[18]);
                        budget.Mar = Convert.ToDecimal(PostData[19]);
                        budget.ModifiedDate = DateTime.Now;
                        mmsObj.Entry(budget).State = System.Data.Entity.EntityState.Modified;
                        mmsObj.SaveChanges();
                        return Json(new { Status = 1, Message = "Budget Updated successfully." });
                    }
                    else
                    {
                        budget = new tblItemBudget();
                        budget.FinancialYear = PostData[0];
                        budget.ItemCode = PostData[1];
                        budget.TotalQuantity = Convert.ToDecimal(PostData[2]);
                        budget.Rate = Convert.ToDecimal(PostData[3]);
                        budget.Amount = Convert.ToDecimal(PostData[4]);
                        budget.ExcessPercentage = Convert.ToDecimal(PostData[5]);
                        budget.ExcessQuantity = Convert.ToDecimal(PostData[6]);
                        budget.ProjectId =PostData[7];
                        budget.Apr = Convert.ToDecimal(PostData[8]);
                        budget.May = Convert.ToDecimal(PostData[9]);
                        budget.June = Convert.ToDecimal(PostData[10]);
                        budget.July = Convert.ToDecimal(PostData[11]);
                        budget.Aug = Convert.ToDecimal(PostData[12]);
                        budget.Sep = Convert.ToDecimal(PostData[13]);
                        budget.Oct = Convert.ToDecimal(PostData[14]);
                        budget.Nov = Convert.ToDecimal(PostData[15]);
                        budget.Dec = Convert.ToDecimal(PostData[16]);
                        budget.Jan = Convert.ToDecimal(PostData[17]);
                        budget.Feb = Convert.ToDecimal(PostData[18]);
                        budget.Mar = Convert.ToDecimal(PostData[19]);
                        budget.CreatedDate = DateTime.Now;
                        budget.ModifiedDate = DateTime.Now;
                        mmsObj.tblItemBudgets.Add(budget);
                        mmsObj.SaveChanges();
                        return Json(new { Status = 2, Message = "Budget added sucessfully." });
                    }

                }
                else
                {
                    return Json(new { Status = 2, Message = "Budget not updated." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3, Message = "Some error occur." });
            }
            
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

    }
}