using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers.Reports
{
    public class PoCloserReportController : Controller
    {
        // GET: PoCloserReport
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPOData(string ProjectId, string VendorId, string PONO, string FromDate, string ToDate, string Received)
        {
            try
            {
                var data = Procedure.GetData<VMPOReport>("GetPOCloserReport", ProjectId, VendorId, PONO, FromDate, ToDate, Received);
                return Json(new { Status = 1, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3, Data = "", Error = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }


        public JsonResult ItemReport(decimal UId)
        {
            try
            {
                var data = Procedure.GetData<VMGetPOItemDetails>("ResionCloserReport", UId).ToList();
                //ViewBag.PoNo = obj.TbIndentPurchaseOrder_GST.SingleOrDefault(x => x.UId == UId).PurchaseOrderNo;
                return Json(new { Status = 1, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3, Data = "", Error = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }
    }
}