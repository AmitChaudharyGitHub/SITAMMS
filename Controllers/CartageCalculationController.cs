using MMS.DAL;
using MMS.Helpers;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    [Authorize]
    public class CartageCalculationController : Controller
    {
        MMSEntities obj = new MMSEntities();
        Procedure objProc = new Procedure();
        public ActionResult Index()
        {
            return View();
        }



        public PartialViewResult MRNData(string MRN)
        {
            var data = Procedure.GetData<GetGridDMR>("GetDMRForCartageCalculation",MRN);
            return PartialView("_DMRGridForCartageCalculation", data.ToList());
        }

        [HttpPost]
        public JsonResult SaveCartage(int Uid, decimal CartageAmt, string Remark,int CartageTypeId = 0)
        {
            if(Uid!=0 && CartageAmt != 0)
            {
                var dataToUpdate=obj.GateEntryChilds.SingleOrDefault(x => x.UId == Uid);
                if (dataToUpdate != null)
                {
                    dataToUpdate.CartageTypeId = CartageTypeId;
                    dataToUpdate.CartageAmount = CartageAmt;
                    dataToUpdate.CartageRemark = Remark;
                    dataToUpdate.CartageRemarkDate = DateTime.Now;

                    tblALLRemark remarkData = new tblALLRemark
                    {
                        CreatedDate = DateTime.Now,
                        PurchaseOrderNo = dataToUpdate.GateEntryNo,
                        RemarkDate = dataToUpdate.CartageRemarkDate,
                        RemarkBy = Session["EmpID"].ToString(),
                        Remarks = Remark,
                        RemarkType = "CartageUpdate",
                        RemarkStage = "MRN"
                    };
                    obj.tblALLRemarks.Add(remarkData);
                    obj.Entry(dataToUpdate).State = System.Data.Entity.EntityState.Modified;
                    obj.SaveChanges();
                    return Json(new { Status = 1 });
                }
            }
            return Json(new {Status=2 });
        }


        #region BindingHelpers

        public JsonResult GetPO(string ProjectId, string VendorId)
        {
            string J = string.Empty;
            
            if(!string.IsNullOrEmpty(ProjectId) && !string.IsNullOrEmpty(VendorId))
            {
              var data=Procedure.GetData<VMDropDown>("GetPoByVendorAndProjectId", ProjectId, VendorId);
                J = data.ToJSON();
            }

            return Json(J, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetMRN(int TypeId, string ProjectId,string VendorId,string PO)
        {
            string J = string.Empty;
            if (TypeId == 1) //opening
            {
                var res = obj.GateEntries.Where(x => x.Status == "Opening" && x.ProjectNo == ProjectId && x.StatusTypeNo==PO).ToList().Select(p => new { Text = p.GateEntryNo, Value = p.GateEntryNo }).OrderByDescending(x => x.Value).ToList();
                J = res.ToJSON();
            }
            else if (TypeId == 2)//for purchase
            {
                var res = obj.GateEntries.Where(x => (x.Status == "LP" || x.Status == "CH" || x.Status == "FC" || x.Status == "IPO") && x.ProjectNo == ProjectId && x.StatusTypeNo==PO).ToList().Select(p => new { Text = p.MRN_No_New, Value = p.MRN_No_New }).OrderByDescending(x => x.Value).OrderByDescending(x => x.Value).ToList();
                J = res.ToJSON();
            }
            else if (TypeId == 3)//for transfer
            {
                var res = obj.GateEntries.Where(x => (x.Status == "ISP" || x.Status == "OSP" || x.Status == "MTR") && x.ProjectNo == ProjectId && x.StatusTypeNo==PO).ToList().Select(p => new { Text = p.GateEntryNo, Value = p.GateEntryNo }).ToList();
                J = res.ToJSON();
            }

            return Json(J, JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}