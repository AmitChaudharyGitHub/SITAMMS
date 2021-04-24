using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.Models;
using MMS.ViewModels;
using MMS.DAL;
using MMS.Helpers;

namespace MMS.Controllers
{
    [Authorize]
    public class POCloserController : Controller
    {
        // GET: POCloser

        MMSEntities obj = new MMSEntities();
        Procedure objProc = new Procedure();
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetPOData(string ProjectId, string VendorId, string PONO, string FromDate, string ToDate, string Received)
        {
            try
            {
                var data = Procedure.GetData<VMPOReport>("GetPOReportByDate", ProjectId, VendorId, PONO, FromDate, ToDate, Received);
                return Json(new { Status = 1, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3, Data = "", Error = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [Param]
        public ActionResult PoDetails(string UId)
        {
            try
            {
                decimal u = Convert.ToDecimal(UId);
                var data = Procedure.GetData<VMGetPOItemDetails>("GetPoDetails", u).ToList();
                ViewBag.PoNo = obj.TbIndentPurchaseOrder_GST.SingleOrDefault(x => x.UId == u).PurchaseOrderNo;
                return View(data);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 3, Data = "", Error = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }

        public string GetString(string UId)
        {
            return ("UId=" + UId).EncryptVar();
        }


        public ActionResult UpdateClose(decimal[] UIds, decimal MUid, int POCloser, string Remark)
        {
            if (UIds.Length > 0)
            {
                foreach (var UId in UIds)
                {
                    var POdata = obj.TbIndentPurchaseOrderChild_GST.SingleOrDefault(x => x.UId == UId);
                    if (POdata != null)
                    {
                        POdata.Status = "Closed";

                        obj.tblClosedPurchaseOrders.Add(new tblClosedPurchaseOrder
                        {
                            ItemUiD = UId,
                            POUId = MUid,
                            ReasonForClose = POCloser,
                            Remarks = Remark,
                            CreatedDate = DateTime.Now,
                            CreatedBy = Session["EmpID"].ToString()
                        });
                        obj.Entry(POdata).State = System.Data.Entity.EntityState.Modified;

                    }

                }
                obj.SaveChanges();
                //var data = objProc.GetPOReportByDate(POdata.ProjectNo, FromDate, ToDate, Received);
                //return PartialView("_PoCloser", data);

                return Json(new { Status = 1 });
            }

            return RedirectToAction("PoDetails", "POCloser");

        }

        public JsonResult BindVendor(string ProjectId)
        {
            var list = obj.tblVendorMasters.Where(x => x.PRJID.Contains(ProjectId)).Select(a => new { Value = a.VendorID, Text = a.Name }).OrderBy(a => a.Text).ToList();
            string jsonString = list.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindProjectNew()
        {
            string EmpId = Session["EmpID"].ToString();
            var list = objProc.GetProjectsByEmpId1(EmpId);
            string jsonString = list.ToJSON();
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetResonCloser()
        {
            var data = obj.tblReasonCloserMasters.Select(x => new { Text = x.ReasonForCloserName, Value = x.Id }).ToList();
            return Json(data.ToJSON(), JsonRequestBehavior.AllowGet);
        }
    }
}