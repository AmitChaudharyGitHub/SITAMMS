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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MMS.Helpers;

namespace MMS.Controllers
{
    public class Print_Slip_For_ApprovalController : Controller
    {
        // GET: Print_Slip_For_Approval
        private MMSEntities objmms = new MMSEntities();
       // private UnitOfWork unitOfWork = new UnitOfWork();
        Procedure objpro = new Procedure(); 
        public ActionResult Index(string PId = "")
        {
            if(Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
            ViewBag.EmpId = empId;
            ViewBag.PId = PId;

            return View();
        }

       

        public class Getdata
        {
            public int Id { get; set; }
            public string ProjectId { get; set; }
            public string Purchase_Order_Indent_No { get; set; }
            public string Created_Employee_Id { get; set; }
            public string PurchaseRefNo { get; set; }
            public string Vendor_Group_Id { get; set; }
            public string Remark { get; set; }
            public string Status { get; set; }
            public string Project_Name { get; set; }
            public string Employee_Name { get; set; }
            public string Vendor_Name { get; set; }
            public string Updated_Employee_Id { get; set; }
            public string Send_To { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public Nullable<DateTime> POCreatedDate { get; set; }
            public Nullable<System.DateTime> ModifiedDate { get; set; }
            public string Status_Approval { get; set; }
            public string ForwordTo { get; set; }
            public string Stage { get; set; }
            public string DiffStagePerson { get; set; }
            public Int32 ? QuotID { get; set; }
            public string SendBy { get; set; }
            public DateTime? FirstLevelApprv_Date{ get; set; }
                                               

        }

        public ActionResult Grid(string Status)

        {
            string empId = Session["EmpID"].ToString();
            bool TrueVal = true;
            if (Status == "Pending")
            {

                var a = objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(i => i.Status == "Pending" && i.Status_Approval == "Not Approved" && ( i.Created_Employee_Id == empId || i.Send_To ==empId|| i.ForwordTo==empId)).OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
                {
                    Id = xx.Id,
                    Purchase_Order_Indent_No = xx.Purchase_Order_Indent_No,
                    CreatedDate = xx.CreatedDate,
                    POCreatedDate = objmms.TbIndentPurchaseOrder_GST.FirstOrDefault(x => x.PurchaseOrderNo == xx.Purchase_Order_Indent_No).CreatedDate,
                    Send_To = xx.SecondLevelApprv_Id == null ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Created_Employee_Id).FirstOrDefault().FName: objmms.tblEmployeeMasters.Where(x=>x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().FName,
                    Vendor_Group_Id = xx.Vendor_Group_Id,
                    Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.Vendor_Group_Id).FirstOrDefault().Name,
                   // DiffStagePerson = xx.Status == "Pending" ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Send_To).FirstOrDefault().FName : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Updated_Employee_Id).FirstOrDefault().FName,
                    Status = xx.Status,
                    // IsBeyondPOLimit = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == xx.Purchase_Order_Indent_No).FirstOrDefault().CheckedBeyondPOLimit, 
                    DiffStagePerson = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == xx.Purchase_Order_Indent_No).FirstOrDefault().CheckedBeyondPOLimit == TrueVal ? "Purchase" : "PMC",
                    Stage =  xx.SecondLevelApprv_Id != null ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().FName : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().FName
                    // Stage =   xx.Status !="f" ? "" : "f"  
                }).ToList();
                //var saf = a.Select(k=>k.Vendor_Group_Id);
                //var ojs= objmms.tblVendorMasters.Where(b => b.VendorID == saf).First().;
                var totalRows = a.Count();

                var data = new PrinSlip_Approval_VM()
                {
                    TotalRows = totalRows,
                    PageSize = 3500,
                    GetDetails_Approved_PO_Slip=a
                };


                if (data != null && data.TotalRows != 0)
                {


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialView_for_Approval", data);
                    }

                    else
                    {
                        return PartialView("_EmptyView");
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
            }

           else if (Status == "Not Approved")
            {

                var a = objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(i => i.Status == "Not Approved" && i.Status_Approval == "Not Approved" && ( i.Created_Employee_Id == empId || i.Send_To == empId || i.ForwordTo == empId)).OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
                {
                    Id = xx.Id,
                    Purchase_Order_Indent_No = xx.Purchase_Order_Indent_No,
                    CreatedDate = xx.CreatedDate,
                    POCreatedDate = objmms.TbIndentPurchaseOrder_GST.FirstOrDefault(x => x.PurchaseOrderNo == xx.Purchase_Order_Indent_No).CreatedDate,
                    Send_To = xx.Status== "Not Approved" && xx.SecondLevelApprv_Id ==null ?  objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().FName: objmms.tblEmployeeMasters.Where(x=>x.EmpID==xx.SecondLevelApprv_Id).FirstOrDefault().FName,
                    Vendor_Group_Id = xx.Vendor_Group_Id,
                    Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.Vendor_Group_Id).FirstOrDefault().Name,
                    // DiffStagePerson = xx.Status == "Pending" ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Send_To).FirstOrDefault().FName : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Updated_Employee_Id).FirstOrDefault().FName,
                    Status = xx.Status,
                    FirstLevelApprv_Date = xx.SecondLevelApprv_Status == "Not Approved" ? xx.SecondLevelApprv_Date : xx.FirstLevelApprv_Date,
                    // IsBeyondPOLimit = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == xx.Purchase_Order_Indent_No).FirstOrDefault().CheckedBeyondPOLimit, 
                    DiffStagePerson = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == xx.Purchase_Order_Indent_No).FirstOrDefault().CheckedBeyondPOLimit == TrueVal ? "Purchase" : "PMC",
                    Stage = xx.Status == "Pending" ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Send_To).FirstOrDefault().FName : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Updated_Employee_Id).FirstOrDefault().FName

                    // Stage =   xx.Status !="f" ? "" : "f"  
                   
                }).ToList();
                //var saf = a.Select(k=>k.Vendor_Group_Id);
                //var ojs= objmms.tblVendorMasters.Where(b => b.VendorID == saf).First().;
                var totalRows = a.Count();

                var data = new PrinSlip_Approval_VM()
                {
                    TotalRows = totalRows,
                    PageSize = 3500,
                    GetDetails_Approved_PO_Slip = a
                };


                if (data != null && data.TotalRows != 0)
                {


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialView_For_DisApproval", data);
                    }

                    else
                    {
                        return PartialView("_EmptyView");
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
            }


            else {

                var a = objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(i => i.Status == "Approved" && i.Status_Approval == "Approved" && (i.Created_Employee_Id == empId || i.FirstLevelApprv_Id==empId || i.SecondLevelApprv_Id ==empId)).OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
                {
                    Id = xx.Id,
                    Purchase_Order_Indent_No = xx.Purchase_Order_Indent_No,
                    CreatedDate = xx.CreatedDate,
                    POCreatedDate = objmms.TbIndentPurchaseOrder_GST.FirstOrDefault(x => x.PurchaseOrderNo == xx.Purchase_Order_Indent_No).CreatedDate,
                    Send_To = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Send_To).FirstOrDefault().FName,
                    Vendor_Group_Id = xx.Vendor_Group_Id,
                    Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.Vendor_Group_Id).FirstOrDefault().Name,
                    Status = xx.Status,
                    FirstLevelApprv_Date = xx.SecondLevelApprv_Status == "Approved" ? xx.SecondLevelApprv_Date : xx.FirstLevelApprv_Date,
                    Created_Employee_Id = objmms.tblEmployeeMasters.Where(x=>x.EmpID==xx.Created_Employee_Id).FirstOrDefault().FName,
                    DiffStagePerson = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().FName !=null ? "Purchase" : "--",
                    //Stage = xx.Status == "Pending" ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Send_To).FirstOrDefault().FName : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Updated_Employee_Id).FirstOrDefault().FName
                    Stage = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().FName
                }).ToList();
                //var saf = a.Select(k=>k.Vendor_Group_Id);
                //var ojs= objmms.tblVendorMasters.Where(b => b.VendorID == saf).First().;
                var totalRows = a.Count();

                var data = new PrinSlip_Approval_VM()
                {
                    TotalRows = totalRows,
                   // PageSize = 3500,
                    GetDetails_Approved_PO_Slip = a
                };


                if (data != null && data.TotalRows != 0)
                {


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialView_for_Approval_Approved", data);
                    }

                    else
                    {
                        return PartialView("_EmptyView");
                    }

                }

                else
                {
                    return View("_EmptyView");
                }

            }


        }

        public ActionResult GridFilterByProject(string Status, string PrjId,string PONumber="",string VendorId="")
        {
            string empId = Session["EmpID"].ToString();
            bool TrueVal = true;
            var divisionId = Repos.GetUserDivision();
            if (Status == "Pending" && PrjId !="")
            {

                //var a = objmms.TbIndentPurchaseOrder_GST.Where(i => i.DivisionId == divisionId && i.Status == "Pending" && i.ProjectNo == PrjId && (i.CreatedBy == empId || i.SendTo == empId || i.SecondLevelApprv_Id==empId)).OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
                //i.DivisionId==divisionId && i.Status == "Pending" && i.FirstLevelApprv_Status!= "Not Approved" && i.SecondLevelApprv_Status!= "Not Approved" && i.ProjectNo == PrjId && (i.CreatedBy == empId || i.SendTo == empId) && (((i.SecondLevelApprv_Id != empId && i.SecondLevelApprv_Date!= null) ||(i.SecondLevelApprv_Id == empId) || i.SendTo == empId) || ((i.FirstLevelApprv_Id != empId && i.FirstLevelApprv_Date != null) || (i.FirstLevelApprv_Id == empId) || i.SendTo == empId))
                var a = objmms.TbIndentPurchaseOrder_GST.Where(i =>i.DivisionId==divisionId && i.Status == "Pending" && i.ProjectNo == PrjId && (i.SendTo == empId)).OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
                {
                    
                    Purchase_Order_Indent_No = xx.PurchaseOrderNo,
                    QuotID = objmms.tblMMSQuotations.Where(x => x.PurchaseReqNo == xx.IndentRefNo).FirstOrDefault().TransId,
                    // below assign PO date to CreatedDate for only show data,because not too much interested to change in view and it's model.
                   CreatedDate = xx.PurchaseOrderDate,
                 POCreatedDate = xx.CreatedDate,
                    // Actually it will be send By . so here we store send by value to send_To for as per required
                    // remove condition && xx.SecondLevelApprv_Date == null from first condition
                 Send_To =(xx.SecondLevelApprv_Id!=null) ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.CreatedBy).FirstOrDefault().LName : ( xx.FirstLevelApprv_Date ==null ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.CreatedBy).FirstOrDefault().LName  : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().LName ) , 


                 //Send_To = xx.SecondLevelApprv_Id == null ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().FName + " "+ objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().FName : 
                 //   objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().LName,
                    Vendor_Group_Id = xx.Vendor_Group_Id,
                    Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.SupplierNo).FirstOrDefault().Name,
                    // DiffStagePerson = xx.Status == "Pending" ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Send_To).FirstOrDefault().FName : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Updated_Employee_Id).FirstOrDefault().FName,
                    Status = xx.Status,
                    // IsBeyondPOLimit = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == xx.Purchase_Order_Indent_No).FirstOrDefault().CheckedBeyondPOLimit, 
                    DiffStagePerson = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == xx.PurchaseOrderNo).FirstOrDefault().CheckedBeyondPOLimit == TrueVal ? "Purchase" : "PMC",
                    Stage = xx.SecondLevelApprv_Id != null ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().LName : 
                    objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().LName
                    // Stage =   xx.Status !="f" ? "" : "f"  
                }).ToList();

                if (VendorId!="Select Vendor")
                {
                    a = a.Where(x => !string.IsNullOrEmpty(x.Vendor_Name) && x.Vendor_Name.Contains(VendorId)).ToList();
                }

                if (!string.IsNullOrEmpty(PONumber))
                {
                   a= a.Where(x => x.Purchase_Order_Indent_No.Contains(PONumber)).ToList();
                }

                var totalRows = a.Count();

                var data = new PrinSlip_Approval_VMGST()
                {
                    TotalRows = totalRows,
                    PageSize = 3500,
                    GetDetails_Approved_PO_Slip = a
                };


                if (data != null && data.TotalRows != 0)
                {


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialView_for_Approval", data);
                    }

                    else
                    {
                        return PartialView("_EmptyView");
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
            }
            else if (Status == "Not Approved" && PrjId != "")
            {

                var a = objmms.TbIndentPurchaseOrder_GST.Where(i => i.DivisionId == divisionId && ((i.Status == "Not Approved" && i.SecondLevelApprv_Status == "Not Approved") ||(i.FirstLevelApprv_Status == "Not Approved" && i.Status== "Pending")) && i.ProjectNo == PrjId && (i.CreatedBy == empId || i.SendTo == empId || i.FirstLevelApprv_Id ==empId || i.SecondLevelApprv_Id==empId)).OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
                {

                    Purchase_Order_Indent_No = xx.PurchaseOrderNo,
                    QuotID = objmms.tblMMSQuotations.Where(x => x.PurchaseReqNo == xx.IndentRefNo).FirstOrDefault().TransId,
                    CreatedDate = xx.PurchaseOrderDate,
                    POCreatedDate = xx.CreatedDate,
                    Send_To = xx.Status == "Not Approved" && xx.SecondLevelApprv_Status == null ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().FName : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().FName,
                    Vendor_Group_Id = xx.Vendor_Group_Id,
                    Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.SupplierNo).FirstOrDefault().Name,
                    // DiffStagePerson = xx.Status == "Pending" ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Send_To).FirstOrDefault().FName : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Updated_Employee_Id).FirstOrDefault().FName,
                    Status = xx.Status,

                    FirstLevelApprv_Date = xx.SecondLevelApprv_Status == "Not Approved" ? xx.SecondLevelApprv_Date : xx.FirstLevelApprv_Date,
                    // IsBeyondPOLimit = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == xx.Purchase_Order_Indent_No).FirstOrDefault().CheckedBeyondPOLimit, 
                    DiffStagePerson = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == xx.PurchaseOrderNo).FirstOrDefault().CheckedBeyondPOLimit == TrueVal ? "Purchase" : "PMC",
                    Stage = xx.Status == "Not Approved" ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.CreatedBy).FirstOrDefault().LName : "",

                    // Stage =   xx.Status !="f" ? "" : "f"  

                }).ToList();


                if (VendorId != "Select Vendor")
                {
                    a = a.Where(x => !string.IsNullOrEmpty(x.Vendor_Name) && x.Vendor_Name.Contains(VendorId)).ToList();
                }

                if (!string.IsNullOrEmpty(PONumber))
                {
                    a = a.Where(x => x.Purchase_Order_Indent_No.Contains(PONumber)).ToList();
                }


                var totalRows = a.Count();

                var data = new PrinSlip_Approval_VMGST()
                {
                    TotalRows = totalRows,
                    PageSize = 3500,
                    GetDetails_Approved_PO_Slip = a
                };


                if (data != null && data.TotalRows != 0)
                {


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialView_For_DisApproval", data);
                    }

                    else
                    {
                        return PartialView("_EmptyView");
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
            }
            else if (Status == "ApprovedPMC" && PrjId != "")
            {

                var a = objmms.TbIndentPurchaseOrder_GST.Where(i => i.DivisionId == divisionId && i.Status == "ApprovedPMC" && i.ProjectNo == PrjId && (i.CreatedBy == empId || i.SendTo == empId || i.FirstLevelApprv_Id == empId || i.SecondLevelApprv_Id == empId)).OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
                {

                    Purchase_Order_Indent_No = xx.PurchaseOrderNo,
                    QuotID = objmms.tblMMSQuotations.Where(x => x.PurchaseReqNo == xx.IndentRefNo).FirstOrDefault().TransId,
                    CreatedDate = xx.PurchaseOrderDate,
                    POCreatedDate = xx.CreatedDate,
                    Send_To = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SendTo).FirstOrDefault().FName + " " +  objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SendTo).FirstOrDefault().LName,
                    Vendor_Group_Id = xx.Vendor_Group_Id,
                    Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.SupplierNo).FirstOrDefault().Name,
                   SendBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().LName,
                    Status = "Pending",
                    DiffStagePerson = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == xx.PurchaseOrderNo).FirstOrDefault().CheckedBeyondPOLimit == TrueVal ? "Purchase" : "PMC"
                  


                }).ToList();

                if (VendorId != "Select Vendor")
                {
                    a = a.Where(x => !string.IsNullOrEmpty(x.Vendor_Name) && x.Vendor_Name.Contains(VendorId)).ToList();
                }

                if (!string.IsNullOrEmpty(PONumber))
                {
                    a = a.Where(x => x.Purchase_Order_Indent_No.Contains(PONumber)).ToList();
                }


                var totalRows = a.Count();

                var data = new PrinSlip_Approval_VMGST()
                {
                    TotalRows = totalRows,
                    PageSize = 3500,
                    GetDetails_Approved_PO_Slip = a
                };


                if (data != null && data.TotalRows != 0)
                {


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_GetapprovedByPMC", data);
                    }

                    else
                    {
                        return PartialView("_EmptyView");
                    }

                }

                else
                {
                    return View("_EmptyView");
                }
            }
            else
            {

                var a = objmms.TbIndentPurchaseOrder_GST.Where(i => i.DivisionId == divisionId && i.Status == "Approved"  && i.ProjectNo == PrjId  && (i.CreatedBy == empId || i.FirstLevelApprv_Id == empId || i.SecondLevelApprv_Id == empId)).OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
                {
                    
                    Purchase_Order_Indent_No = xx.PurchaseOrderNo,
                    QuotID = objmms.tblMMSQuotations.Where(x => x.PurchaseReqNo == xx.IndentRefNo).FirstOrDefault().TransId,
                    CreatedDate = xx.PurchaseOrderDate,
                    POCreatedDate = xx.CreatedDate,
                    Send_To = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().LName,
                    Vendor_Group_Id = xx.Vendor_Group_Id,
                    Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.SupplierNo).FirstOrDefault().Name,
                    Status = xx.Status,
                    FirstLevelApprv_Date = xx.SecondLevelApprv_Status == "Approved" ? xx.SecondLevelApprv_Date : xx.FirstLevelApprv_Date,
                    Created_Employee_Id = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.CreatedBy).FirstOrDefault().LName,
                    DiffStagePerson = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().FName != null ? "Purchase" : "--",
                    //Stage = xx.Status == "Pending" ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Send_To).FirstOrDefault().FName : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.Updated_Employee_Id).FirstOrDefault().FName
                    Stage = objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().LName
                }).ToList();
                
                var totalRows = a.Count();

                if (VendorId != "Select Vendor")
                {
                    a = a.Where(x =>!string.IsNullOrEmpty(x.Vendor_Name) && x.Vendor_Name.Contains(VendorId)).ToList();
                }

                if (!string.IsNullOrEmpty(PONumber))
                {
                    a = a.Where(x => x.Purchase_Order_Indent_No.Contains(PONumber)).ToList();
                }


                var data = new PrinSlip_Approval_VM()
                {
                    TotalRows = totalRows,
                    // PageSize = 3500,
                    GetDetails_Approved_PO_Slip = a
                };


                if (data != null && data.TotalRows != 0)
                {


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_PartialView_for_Approval_Approved", data);
                    }

                    else
                    {
                        return PartialView("_EmptyView");
                    }

                }

                else
                {
                    return View("_EmptyView");
                }

            }

        }


        public ActionResult GetPO_details_with_Indent(string id, string status)
        {
            ViewBag.POStatus = status;
            WordToText print = new WordToText();
            string empId = Session["EmpID"].ToString();
         
            //main code start here
            id = id?.Replace("#", "/");
            Session["POId"] = id.ToString();

            var a = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value=x.ProjectNo,
                Text= objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectNo).First().ProjectName
            });
            ViewBag.prjtid = t;

            string PurReq = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).FirstOrDefault().IndentRefNo;
            int ? PurchaseTypeIdSel = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReq).FirstOrDefault().PurchasePIC_Type;
            ViewBag.PurDate = objmms.PurchaseRequisitionMasters.SingleOrDefault(x => x.PurRequisitionNo == PurReq).Date.Value;

            var units ="";
            if (id != null)
            {
                var querys = (from mas in objmms.TbIndentPurchaseOrder_GST
                              join chld in objmms.TbIndentPurchaseOrderChild_GST

                             // Both anonymous types should have exact same number of properties having same name and datatype
                             // on new { a = (int?)mas.UId } equals new { a = chld.MUId }
                             on mas.UId equals chld.MUId into pchld
                              from pch in pchld.DefaultIfEmpty()
                              where mas.PurchaseOrderNo == id
                              select new Purchase_Order_Slip_VM
                              {
                                  SupplierNo = mas.SupplierNo,
                                  Name = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Name,
                                  Address = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Address,
                                  Vendor_country_state_city = objmms.tblCityLists.Where(k3 => k3.CityID == objmms.tblVendorMasters.Where(ct => ct.VendorID == mas.SupplierNo).FirstOrDefault().City).FirstOrDefault().CityName + ", " +
                                            objmms.tblStates.Where(k2 => k2.StateID == objmms.tblVendorMasters.Where(st => st.VendorID == mas.SupplierNo).FirstOrDefault().State).FirstOrDefault().StateName + ", " +
                                            objmms.tblCountries.Where(k1 => k1.CountryID == objmms.tblVendorMasters.Where(gr => gr.VendorID == mas.SupplierNo).FirstOrDefault().Country).FirstOrDefault().CnName,

                                  VendorTInNO = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().TinNo,
                                  VendorGSTNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().GST_NO,
                                  AcilTinNo = objmms.tblProjectTINNoes.Where(k=>k.ProjectId == mas.ProjectNo).FirstOrDefault().TINNo,
                                  AcilGSTNo = objmms.tblProjectGSTNoes.Where(k=>k.ProjectId == mas.ProjectNo).FirstOrDefault().GSTNo,
                                  MobileNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().MobileNo,
                                  ContactPerson = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().ContactPerson,
                                  Email=objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Email,



                                  Reference = mas.Reference,

                                  IndentRefNo = mas.IndentRefNo,
                                  PurchaseOrderNo = mas.PurchaseOrderNo,

                                  SubTotal = pch.SubTotal1,
                                  Total = mas.Total,
                                  GrandTotal = mas.GrandTotal,
                                  ProjectNo = mas.ProjectNo,
                                  ProjectName = objmms.tblProjectMasters.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().ProjectName,
                                  Location = objmms.tblProjectParticularsDetailAs.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().Location,

                                  PurchaseOrderDate = mas.PurchaseOrderDate,
                                  ItemNo = objmms.tblItemMasters.Where(y => y.ItemID == pch.ItemNo).FirstOrDefault().GITEMCode,
                                  ItemName = objmms.tblItemMasters.Where(j => j.ItemID == pch.ItemNo).FirstOrDefault().ItemName,
                                  UnitID = objmms.tblItemMasters.Where(l => l.ItemID == pch.ItemNo).FirstOrDefault().UnitID,
                                  UnitName = objmms.tblUnitMasters.Where(l => l.UnitID == objmms.tblItemMasters.Where(ll => ll.ItemID == pch.ItemNo).FirstOrDefault().UnitID).FirstOrDefault().UnitCode,
                                  Amount = pch.Total,
                                  Discount = pch.Discount,
                                  Qty = pch.Qty,
                                  Rate = pch.NewRate,


                                  //  --------------------added Newely here

                                  Cartage_Type_1 = objmms.tblMMSCartageTypes.Where(x => x.TransId == pch.CartageTypeId).FirstOrDefault().CartageType,
                                  CR1_Total = pch.CartageAmount,
                                  T_Total = mas.Total,
                                  P_Total = pch.PackingChargesAmount,
                                  T_SubTotal1 = mas.SubTotal1,
                                  T_SubTotal2 = mas.SubTotal2,

                                  POCreatedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().LName,
                                  // UserType = UserType(mas.PurchaseOrderNo)
                                  UserType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == empId).FirstOrDefault().TypeName,
                                  CheckedPoLimit = mas.CheckedBeyondPOLimit,
                                  IsReleasePOBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().LName,
                                  PoApprovedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().FName +
                                                 " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().LName,

                                  // Added After GST.

                                  ItemToTAmt = pch.Total,
                                  SubTotal1 = pch.SubTotal1,
                                  SubTotal2 = pch.SubTotal2,
                                  PackagePercentage = pch.PackingChargesPercentage,
                                  PackingPercentageAmt = pch.PackingChargesAmount,
                                  CartageType = objmms.tblMMSCartageTypes.Where(x => x.TransId == pch.CartageTypeId).FirstOrDefault().CartageType,
                                  CartageTypeRate = pch.Cartage_rate,
                                  CartageAmt = pch.CartageAmount,
                                  InsurancePercentage = pch.InsuranceRate,
                                  InsurancePercentageAmt = pch.InsuranceAmount,
                                  TaxableAmount = pch.SubTotal2,
                                  CGSTPercentage = pch.CGSTPercentage,
                                  CGSTAmt = pch.CGSTAmount,
                                  SGSTAndUTGSTPercentage = pch.SGSTPercentage,
                                  SGSTAndUTGSTAmt = pch.SGSTAmount,
                                  IGST = pch.IGSTPercentage,
                                  IGSTAmt = pch.IGSTAmount,
                                  GrossAmount = pch.GrossAmount,
                                  HSNCode = objmms.tblItemMasters.Where(x => x.ItemID == pch.ItemNo).FirstOrDefault().HSNCode,
                                  TotalPackingSum = mas.Total_PAndF,
                                  TotalCartageSum = mas.Total_Cartage,
                                  TotalInsuranceSum = mas.Total_Insurance,
                                  TotalTaxableAmountSum = mas.SubTotal2,
                                  TotalCGSTSum = mas.Total_CGST,
                                  TotalSCGSTAndUTGSTSum = mas.Total_SGSTAndUTGST,
                                  IGSTSum = mas.Total_IGST,
                                  TotalAmountInWords = mas.Total_NetAmountInWords,
                                  ItemDescrp = pch.Item_Description,
                                  IsPORelease=mas.IsPORelease,
                                  PODescription=mas.PODescription,
                                  TCSRate = pch.TCS_Rate,
                                  TCSAmount = pch.TotalTCSAmount,
                                  TotalTCS = mas.Total_TCS,
                                  Tax_type = pch.TaxRateType,
                                  TCSActive = mas.TCSActive != null ? mas.TCSActive.Value : false
                              }).ToList();
       
                var data = querys;
                PrintData dataObj = new PrintData();
                
                dataObj.HeaderData = data.Select(x => new HeaderData
                {
                    PurchaseOrderNo = x.PurchaseOrderNo,
                    Total = Convert.ToDecimal(Math.Round((double)(x.Total != null ? x.Total : 0), 2).ToString("0.00")),
                    Location = x.Location,
                    ContactPerson = x.ContactPerson,
                    Vendor_country_state_city = x.Vendor_country_state_city,
                    VendorTInNO = x.VendorTInNO,
                    VendorGSTNo = x.VendorGSTNo,
                    MobileNo = x.MobileNo,
                    Reference = x.Reference,

                    Cartage = x.Cartage,
                    GrandTotal = Convert.ToDecimal(Math.Round((double)(x.GrandTotal != null ? x.GrandTotal : 0), 2).ToString("0.00")),
                    SubTotal = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                    PurchaseOrderDate = x.PurchaseOrderDate,
                    AcilTinNo = x.AcilTinNo,
                    AcilGSTNo = x.AcilGSTNo,
                    Address = x.Address,
                    Name = x.Name,
                    ProjectName = x.ProjectName,
                    IndentRefNo = x.IndentRefNo,
                   

                    //Ex_Total = Math.Round((decimal)data.Sum(k => k.Ex_Total), 2),
                    Cartage_Type_1 = x.Cartage_Type_1,
                    CR1_Total = Convert.ToDecimal(Math.Round((decimal)data.Sum(k => k.CR1_Total), 2).ToString("0.00")),



                    T_Total = x.T_Total,
                    P_Total = Math.Round((decimal)data.Sum(k => k.P_Total), 2),
                    T_SubTotal1 = x.T_SubTotal1,
                    T_SubTotal2 = x.T_SubTotal2,
                    ProjectNo = x.ProjectNo,
                    PoCreatedBy = x.POCreatedBy,
                    UserType = x.UserType,
                    CheckedPoLimit = x.CheckedPoLimit.ToString(),
                    PoApprovedBy = x.PoApprovedBy,
                    IsReleasePOBy = x.IsReleasePOBy,
                    TotalPackingSum = Convert.ToDecimal(Math.Round((double)(x.TotalPackingSum != null ? x.TotalPackingSum : 0), 2).ToString("0.00")),
                    TotalCartageSum = Convert.ToDecimal(Math.Round((double)(x.TotalCartageSum != null ? x.TotalCartageSum : 0), 2).ToString("0.00")),
                    TotalInsuranceSum = x.TotalInsuranceSum,
                    TotalTaxableAmountSum = Convert.ToDecimal(Math.Round((double)(x.TotalTaxableAmountSum != null ? x.TotalTaxableAmountSum : 0), 2).ToString("0.00")),
                    TotalCGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalCGSTSum != null ? x.TotalCGSTSum : 0), 2).ToString("0.00")),
                    TotalSCGSTAndUTGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalSCGSTAndUTGSTSum != null ? x.TotalSCGSTAndUTGSTSum : 0), 2).ToString("0.00")),
                    IGSTSum = Convert.ToDecimal(Math.Round((double)(x.IGSTSum != null ? x.IGSTSum : 0), 2).ToString("0.00")),
                    TotalAmountInWords = x.TotalAmountInWords,
                    IsPORelease=x.IsPORelease,
                    OrderFor=x.PODescription,
                    Email=x.Email,
                    TotalTCSAmt = Convert.ToDecimal(Math.Round((double)(x.TotalTCS != null ? x.TotalTCS : 0), 2).ToString("0.00")),
                    TCSActive = x.TCSActive
                }).FirstOrDefault();


                if (dataObj.HeaderData != null)
                {
                    dataObj.HeaderData.PIDate = objmms.PurchaseRequisitionMasters.FirstOrDefault(x => x.PurRequisitionNo == dataObj.HeaderData.IndentRefNo).Date;
                }

                    try
                {

                    dataObj.ItemData = data.Select(x => new ItemData
                    {
                        ItemName = x.ItemName,
                        ItemNo = x.ItemNo,
                        ItemDescrp = x.ItemDescrp,
                        Remark = x.Remark,
                        UnitName = x.UnitName,
                        Rate = Convert.ToDecimal(Math.Round((double)(x.Rate != null ? x.Rate : 0), 2).ToString("0.00")),
                        Discount = Convert.ToDecimal(Math.Round((double)(x.Discount != null ? x.Discount : 0), 2).ToString("0.00")),
                        SubTotal1 = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                        Qty = x.Qty,
                        ItemToTAmt = Convert.ToDecimal(Math.Round((double)(x.ItemToTAmt != null ? x.ItemToTAmt : 0), 2).ToString("0.00")),
                        SubTotal2 = Convert.ToDecimal(Math.Round((double)(x.SubTotal2 != null ? x.SubTotal2 : 0), 2).ToString("0.00")),
                        PackagePercentage = Convert.ToDecimal(Math.Round((double)(x.PackagePercentage != null ? x.PackagePercentage : 0), 2).ToString("0.00")),
                        PackingPercentageAmt = Convert.ToDecimal(Math.Round((double)(x.PackingPercentageAmt != null ? x.PackingPercentageAmt : 0), 2).ToString("0.00")),
                        CartageType = x.CartageType,
                        CartageTypeRate = Convert.ToDecimal(Math.Round((double)(x.CartageTypeRate != null ? x.CartageTypeRate : 0), 2).ToString("0.00")),
                        CartageAmt = Convert.ToDecimal(Math.Round((double)(x.CartageAmt != null ? x.CartageAmt : 0), 2).ToString()),
                        InsurancePercentage = x.InsurancePercentage,
                        InsurancePercentageAmt = x.InsurancePercentageAmt,
                        TaxableAmount = Convert.ToDecimal(Math.Round((double)(x.TaxableAmount != null ? x.TaxableAmount : 0), 2).ToString("0.00")),
                        CGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.CGSTPercentage != null ? x.CGSTPercentage : 0), 2).ToString("0.00")),
                        CGSTAmt = Convert.ToDecimal(Math.Round((double)(x.CGSTAmt != null ? x.CGSTAmt : 0), 2).ToString("0.00")),
                        SGSTAndUTGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTPercentage != null ? x.SGSTAndUTGSTPercentage : 0), 2).ToString("0.00")),
                        SGSTAndUTGSTAmt = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTAmt != null ? x.SGSTAndUTGSTAmt : 0), 2).ToString("0.00")),
                        IGST = Convert.ToDecimal(Math.Round((double)(x.IGST != null ? x.IGST : 0), 2).ToString("0.00")),
                        IGSTAmt = Convert.ToDecimal(Math.Round((double)(x.IGSTAmt != null ? x.IGSTAmt : 0), 2).ToString("0.00")),
                        GrossAmount = x.GrossAmount.HasValue?Convert.ToDecimal(Math.Round((double)x.GrossAmount, 2).ToString("0.00")):new decimal(0.00),
                        HSNCode = x.HSNCode,
                        TCSRate = x.TCSRate.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSRate, 2).ToString("0.00")) : new decimal(0.00),
                        TCSAmount = x.TCSAmount.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSAmount, 2).ToString("0.00")) : new decimal(0.00),
                        TaxType = x.Tax_type
                    }).ToList();

                }
                catch (Exception ex)
                {
                    dataObj.ItemData = null;
                }
                ViewBag.PrintamountInWord = print.Cal(dataObj.HeaderData.GrandTotal ?? 0);

                


                if (PurchaseTypeIdSel == 3 || PurchaseTypeIdSel == 4 || PurchaseTypeIdSel == 5|| PurchaseTypeIdSel == 6 || PurchaseTypeIdSel == 7 || PurchaseTypeIdSel == 8)
                {
                    //for delivery details code
                    if (id != null)
                    {
                        var Specific_De = (from de_I in objmms.TbDelivery_Details_PO

                                           where de_I.Purchase_order_No == id
                                           select new DeliveryPo_details_Chld
                                           {
                                               Delivery_Schedule = de_I.Delivery_Schedule,
                                               Delivery_Address = de_I.Delivery_Address,
                                               Contact_Person_Name = de_I.Contact_Person_Name,
                                               Contact_Person_Mobile = de_I.Contact_Person_Mobile,
                                               Billing_Address = de_I.Billing_Address,
                                               StoreKeeperName = de_I.StoreKeeperName,
                                               StoreKeeperNo = de_I.StoreKeeperContact,
                                               EPPerson = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.PersonName).FirstOrDefault(),
                                               EPContact = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.ContactNumber).FirstOrDefault(),
                                               DivType = de_I.ContactDivType
                                           }).ToList();

                        var data_delvry = Specific_De;
                        ViewBag.Delevery_details = data_delvry;

                        if (Specific_De.Count > 0)
                        {
                            int? purDivison = Specific_De[0].DivType;
                            if (purDivison != null)
                            {
                                var officeAddr = objmms.tbl_OfficeAddress.Where(s => s.Division == purDivison).Select(s => s).FirstOrDefault();
                                ViewBag.OfficeAddress = officeAddr.OfficeAddress;
                                ViewBag.OfficeMobile = officeAddr.Mobile;
                                ViewBag.OfficeContactName = officeAddr.ContactPerson;
                            }
                        }
                    }

                    else
                    {
                    }
                    return PartialView("_PartialView_for_Approved_Print_SlipTest_ForOutPro", dataObj);
                }
                else if (PurchaseTypeIdSel== 1 || PurchaseTypeIdSel == 2 || PurchaseTypeIdSel == 9 || PurchaseTypeIdSel == 10|| PurchaseTypeIdSel == 11)
                {
                    #region

                    //for genral terms and conditions

                    if (id != null)
                    {
                        var Genral_T_and_C = (from gtcs in objmms.tblGenral_Terms_Conditions_Child

                                              where gtcs.Purchase_Order_No == id && gtcs.Status == "Enable" && gtcs.IsActive == "Yes"

                                              select new Genral_term_conditon_Chld
                                              {
                                                  GTC_Description = gtcs.GTC_Description
                                              }).ToList();
                        var data_gtc = Genral_T_and_C;
                        ViewBag.Gtc_details = data_gtc;
                    }
                    else
                    {
                    }


                    //for Specific Instructions 
                    if (id != null)
                    {
                        var Specific_I = (from Spe_I in objmms.tblSpecific_Instructions_Child

                                          where Spe_I.Purchase_Order_No == id && Spe_I.Status == "Enable" && Spe_I.IsActive == "Yes"
                                          select new Specific_Instructions_TC_Chld
                                          {
                                              SI_Description = Spe_I.SI_Description
                                          }).ToList();
                        var data_si = Specific_I;
                        ViewBag.Si_details = data_si;
                    }

                    else
                    {
                    }

                    //for delivery details code
                    if (id != null)
                    {
                        var Specific_De = (from de_I in objmms.TbDelivery_Details_PO

                                           where de_I.Purchase_order_No == id
                                           select new DeliveryPo_details_Chld
                                           {
                                               Delivery_Schedule = de_I.Delivery_Schedule,
                                               Delivery_Address = de_I.Delivery_Address,
                                               Contact_Person_Name = de_I.Contact_Person_Name,
                                               Contact_Person_Mobile = de_I.Contact_Person_Mobile,
                                               Billing_Address = de_I.Billing_Address,
                                               StoreKeeperName = de_I.StoreKeeperName,
                                               StoreKeeperNo = de_I.StoreKeeperContact,
                                               EPPerson = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.PersonName).FirstOrDefault(),
                                               EPContact = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.ContactNumber).FirstOrDefault(),
                                               DivType = de_I.ContactDivType
                                           }).ToList();

                        var data_delvry = Specific_De;
                        ViewBag.Delevery_details = data_delvry;

                        if (Specific_De.Count > 0)
                        {
                            int? purDivison = Specific_De[0].DivType;
                            if (purDivison != null)
                            {
                                var officeAddr = objmms.tbl_OfficeAddress.Where(s => s.Division == purDivison).Select(s => s).FirstOrDefault();
                                ViewBag.OfficeAddress = officeAddr.OfficeAddress;
                                ViewBag.OfficeMobile = officeAddr.Mobile;
                                ViewBag.OfficeContactName = officeAddr.ContactPerson;
                            }
                        }
                    }

                    else
                    {
                    }





                    //for Specific  terms and conditions
                    if (id != null)
                    {
                        var Specific_T_and_C = (from SpecificT_C in objmms.tblSpecificTermsAndConditions_Child

                                                where SpecificT_C.Purchase_Order_No == id && SpecificT_C.Status == "Enable" && SpecificT_C.IsActive == "Yes"
                                                select new Specific_Termss_And_conditions_Chld
                                                {
                                                    STC_Description = SpecificT_C.STC_Description
                                                }).ToList();
                        var data_stc = Specific_T_and_C;
                        ViewBag.Stc_details = data_stc;
                    }
                    else { }
                    #endregion


                    return PartialView("_PartialView_for_Approved_Print_SlipTest", dataObj);

                   
                }

                else {


                    return Json(null, JsonRequestBehavior.AllowGet);
                }




            }
            else {
               
                

                return Json(null, JsonRequestBehavior.AllowGet);
            }

           




        }



        public ActionResult GetPO_details_with_Indent_Approved(string id)
        {
            WordToText print = new WordToText();
            string empId = Session["EmpID"].ToString();
           

            //main code start here
            id = id?.Replace("#", "/");
            Session["POId"] = id.ToString();

            var a = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectNo,
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectNo).First().ProjectName
            });
            ViewBag.prjtid = t;

            string PurReq = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).FirstOrDefault().IndentRefNo;
            int? PurchaseTypeIdSel = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReq).FirstOrDefault().PurchasePIC_Type;

            ViewBag.PurDate = objmms.PurchaseRequisitionMasters.SingleOrDefault(x => x.PurRequisitionNo == PurReq).Date.Value;


            var units = "";
            if (id != null)
            {
                var querys = (from mas in objmms.TbIndentPurchaseOrder_GST
                              join chld in objmms.TbIndentPurchaseOrderChild_GST
                             // Both anonymous types should have exact same number of properties having same name and datatype
                             // on new { a = (int?)mas.UId } equals new { a = chld.MUId }
                             on mas.UId equals chld.MUId into pchld
                             from pch in pchld.DefaultIfEmpty()
                              where mas.PurchaseOrderNo == id
                              select new Purchase_Order_Slip_VM
                              {
                                  SupplierNo = mas.SupplierNo,
                                  Name = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Name,
                                  Address = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Address,
                                  Vendor_country_state_city = objmms.tblCityLists.Where(k3 => k3.CityID == objmms.tblVendorMasters.Where(ct => ct.VendorID == mas.SupplierNo).FirstOrDefault().City).FirstOrDefault().CityName + ", " +
                                            objmms.tblStates.Where(k2 => k2.StateID == objmms.tblVendorMasters.Where(st => st.VendorID == mas.SupplierNo).FirstOrDefault().State).FirstOrDefault().StateName + ", " +
                                            objmms.tblCountries.Where(k1 => k1.CountryID == objmms.tblVendorMasters.Where(gr => gr.VendorID == mas.SupplierNo).FirstOrDefault().Country).FirstOrDefault().CnName,

                                  VendorTInNO = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().TinNo,
                                  VendorGSTNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().GST_NO,
                                  AcilTinNo = objmms.tblProjectTINNoes.Where(k => k.ProjectId == mas.ProjectNo).FirstOrDefault().TINNo,
                                  AcilGSTNo = objmms.tblProjectGSTNoes.Where(k=>k.ProjectId == mas.ProjectNo).FirstOrDefault().GSTNo,
                                  MobileNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().MobileNo,
                                  ContactPerson = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().ContactPerson,
                                  Email = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Email,

                                  Reference = mas.Reference,

                                  IndentRefNo = mas.IndentRefNo,
                                  PurchaseOrderNo = mas.PurchaseOrderNo,

                                  SubTotal = pch.SubTotal1,
                                  Total = mas.Total,
                                  GrandTotal = mas.GrandTotal,
                                  ProjectNo = mas.ProjectNo,
                                  ProjectName = objmms.tblProjectMasters.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().ProjectName,
                                  Location = objmms.tblProjectParticularsDetailAs.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().Location,

                                  PurchaseOrderDate = mas.PurchaseOrderDate,
                                  ItemNo = objmms.tblItemMasters.Where(y => y.ItemID == pch.ItemNo).FirstOrDefault().GITEMCode,
                                  ItemName = objmms.tblItemMasters.Where(j => j.ItemID == pch.ItemNo).FirstOrDefault().ItemName,
                                  UnitID = objmms.tblItemMasters.Where(l => l.ItemID == pch.ItemNo).FirstOrDefault().UnitID,
                                  UnitName = objmms.tblUnitMasters.Where(l => l.UnitID == objmms.tblItemMasters.Where(ll => ll.ItemID == pch.ItemNo).FirstOrDefault().UnitID).FirstOrDefault().UnitCode,
                                  Amount = pch.Total,
                                  Discount = pch.Discount,
                                  Qty = pch.Qty,
                                  Rate = pch.NewRate,


                                  //  --------------------added Newely here

                                  Cartage_Type_1 = objmms.tblMMSCartageTypes.Where(x => x.TransId == pch.CartageTypeId).FirstOrDefault().CartageType,
                                  CR1_Total = pch.CartageAmount,
                                  T_Total = mas.Total,
                                  P_Total = pch.PackingChargesAmount,
                                  T_SubTotal1 = mas.SubTotal1,
                                  T_SubTotal2 = mas.SubTotal2,

                                  POCreatedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().LName,
                                  // UserType = UserType(mas.PurchaseOrderNo)
                                  UserType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == empId).FirstOrDefault().TypeName,
                                  CheckedPoLimit = mas.CheckedBeyondPOLimit,
                                  IsReleasePOBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().LName,
                                  PoApprovedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().FName +
                                                 " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().LName,

                                  // Added After GST.

                                  ItemToTAmt = pch.Total,
                                  SubTotal1 = pch.SubTotal1,
                                  SubTotal2 = pch.SubTotal2,
                                  PackagePercentage = pch.PackingChargesPercentage,
                                  PackingPercentageAmt = pch.PackingChargesAmount,
                                  CartageType = objmms.tblMMSCartageTypes.Where(x => x.TransId == pch.CartageTypeId).FirstOrDefault().CartageType,
                                  CartageTypeRate = pch.Cartage_rate,
                                  CartageAmt = pch.CartageAmount,
                                  InsurancePercentage = pch.InsuranceRate,
                                  InsurancePercentageAmt = pch.InsuranceAmount,
                                  TaxableAmount = pch.SubTotal2,
                                  CGSTPercentage = pch.CGSTPercentage,
                                  CGSTAmt = pch.CGSTAmount,
                                  SGSTAndUTGSTPercentage = pch.SGSTPercentage,
                                  SGSTAndUTGSTAmt = pch.SGSTAmount,
                                  IGST = pch.IGSTPercentage,
                                  IGSTAmt = pch.IGSTAmount,
                                  GrossAmount = pch.GrossAmount,
                                  HSNCode = objmms.tblItemMasters.Where(x => x.ItemID == pch.ItemNo).FirstOrDefault().HSNCode,
                                  TotalPackingSum = mas.Total_PAndF,
                                  TotalCartageSum = mas.Total_Cartage,
                                  TotalInsuranceSum = mas.Total_Insurance,
                                  TotalTaxableAmountSum = mas.SubTotal2,
                                  TotalCGSTSum = mas.Total_CGST,
                                  TotalSCGSTAndUTGSTSum = mas.Total_SGSTAndUTGST,
                                  IGSTSum = mas.Total_IGST,
                                  TotalAmountInWords = mas.Total_NetAmountInWords,
                                  ItemDescrp = pch.Item_Description,
                                  IsPORelease=mas.IsPORelease,
                                  PODescription=mas.PODescription,
                                  TCSRate = pch.TCS_Rate,
                                  TCSAmount = pch.TotalTCSAmount,
                                  TotalTCS = mas.Total_TCS,
                                  Tax_type = pch.TaxRateType,
                                  TCSActive = mas.TCSActive != null ? mas.TCSActive.Value : false
                              }).ToList();
                //var projtn=querys.Select(k=> k.ProjectNo == )
                var data = querys;
                PrintData dataObj = new PrintData();
                dataObj.HeaderData = data.Select(x => new HeaderData
                {
                    PurchaseOrderNo = x.PurchaseOrderNo,

                    Total = Convert.ToDecimal(Math.Round((double)(x.Total != null ? x.Total : 0), 2).ToString("0.00")),
                    Location = x.Location,
                    ContactPerson = x.ContactPerson,
                    Vendor_country_state_city = x.Vendor_country_state_city,
                    VendorTInNO = x.VendorTInNO,
                    VendorGSTNo = x.VendorGSTNo,
                    MobileNo = x.MobileNo,
                    Reference = x.Reference,

                    Cartage = x.Cartage,
                    GrandTotal = Convert.ToDecimal(Math.Round((double)(x.GrandTotal != null ? x.GrandTotal : 0), 2).ToString("0.00")),
                    SubTotal = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                    PurchaseOrderDate = x.PurchaseOrderDate,
                    AcilTinNo = x.AcilTinNo,
                    AcilGSTNo = x.AcilGSTNo,
                    Address = x.Address,
                    Name = x.Name,
                    ProjectName = x.ProjectName,
                    IndentRefNo = x.IndentRefNo,
                    ExciseDutyRate = x.ExciseDutyRate,
                    ExciseDuty = x.ExciseDuty,

                    //Ex_Total = Math.Round((decimal)data.Sum(k => k.Ex_Total), 2),
                    Cartage_Type_1 = x.Cartage_Type_1,
                    CR1_Total = Convert.ToDecimal(Math.Round((decimal)data.Sum(k => k.CR1_Total), 2).ToString("0.00")),



                    T_Total = x.T_Total,
                    P_Total = Math.Round((decimal)data.Sum(k => k.P_Total), 2),
                    T_SubTotal1 = x.T_SubTotal1,
                    T_SubTotal2 = x.T_SubTotal2,
                    ProjectNo = x.ProjectNo,
                    PoCreatedBy = x.POCreatedBy,
                    UserType = x.UserType,
                    CheckedPoLimit = x.CheckedPoLimit.ToString(),
                    PoApprovedBy = x.PoApprovedBy,
                    IsReleasePOBy = x.IsReleasePOBy,
                    TotalPackingSum = Convert.ToDecimal(Math.Round((double)(x.TotalPackingSum != null ? x.TotalPackingSum : 0), 2).ToString("0.00")),
                    TotalCartageSum = Convert.ToDecimal(Math.Round((double)(x.TotalCartageSum != null ? x.TotalCartageSum : 0), 2).ToString("0.00")),
                    TotalInsuranceSum = x.TotalInsuranceSum,
                    TotalTaxableAmountSum = Convert.ToDecimal(Math.Round((double)(x.TotalTaxableAmountSum != null ? x.TotalTaxableAmountSum : 0), 2).ToString("0.00")),
                    TotalCGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalCGSTSum != null ? x.TotalCGSTSum : 0), 2).ToString("0.00")),
                    TotalSCGSTAndUTGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalSCGSTAndUTGSTSum != null ? x.TotalSCGSTAndUTGSTSum : 0), 2).ToString("0.00")),
                    IGSTSum = Convert.ToDecimal(Math.Round((double)(x.IGSTSum != null ? x.IGSTSum : 0), 2).ToString("0.00")),
                    TotalAmountInWords = x.TotalAmountInWords,
                    IsPORelease = x.IsPORelease,
                    OrderFor = x.PODescription,
                    Email = x.Email,
                    TotalTCSAmt = Convert.ToDecimal(Math.Round((double)(x.TotalTCS != null ? x.TotalTCS : 0), 2).ToString("0.00")),
                    TCSActive = x.TCSActive
                }).FirstOrDefault();

                try
                {
                    dataObj.ItemData = data.Select(x => new ItemData
                    {
                        ItemName = x.ItemName,
                        ItemNo = x.ItemNo,
                        ItemDescrp = x.ItemDescrp,
                        Remark = x.Remark,
                        UnitName = x.UnitName,
                        Rate = Convert.ToDecimal(Math.Round((double)(x.Rate != null ? x.Rate : 0), 2).ToString("0.00")),
                        Discount = Convert.ToDecimal(Math.Round((double)(x.Discount != null ? x.Discount : 0), 2).ToString("0.00")),
                        SubTotal1 = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                        Qty = x.Qty,
                        ItemToTAmt = Convert.ToDecimal(Math.Round((double)(x.ItemToTAmt != null ? x.ItemToTAmt : 0), 2).ToString("0.00")),
                        SubTotal2 = Convert.ToDecimal(Math.Round((double)(x.SubTotal2 != null ? x.SubTotal2 : 0), 2).ToString("0.00")),
                        PackagePercentage = Convert.ToDecimal(Math.Round((double)(x.PackagePercentage != null ? x.PackagePercentage : 0), 2).ToString("0.00")),
                        PackingPercentageAmt = Convert.ToDecimal(Math.Round((double)(x.PackingPercentageAmt != null ? x.PackingPercentageAmt : 0), 2).ToString("0.00")),
                        CartageType = x.CartageType,
                        CartageTypeRate = Convert.ToDecimal(Math.Round((double)(x.CartageTypeRate != null ? x.CartageTypeRate : 0), 2).ToString("0.00")),
                        CartageAmt = Convert.ToDecimal(Math.Round((double)(x.CartageAmt != null ? x.CartageAmt : 0), 2).ToString()),
                        InsurancePercentage = x.InsurancePercentage,
                        InsurancePercentageAmt = x.InsurancePercentageAmt,
                        TaxableAmount = Convert.ToDecimal(Math.Round((double)(x.TaxableAmount != null ? x.TaxableAmount : 0), 2).ToString("0.00")),
                        CGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.CGSTPercentage != null ? x.CGSTPercentage : 0), 2).ToString("0.00")),
                        CGSTAmt = Convert.ToDecimal(Math.Round((double)(x.CGSTAmt != null ? x.CGSTAmt : 0), 2).ToString("0.00")),
                        SGSTAndUTGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTPercentage != null ? x.SGSTAndUTGSTPercentage : 0), 2).ToString("0.00")),
                        SGSTAndUTGSTAmt = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTAmt != null ? x.SGSTAndUTGSTAmt : 0), 2).ToString("0.00")),
                        IGST = Convert.ToDecimal(Math.Round((double)(x.IGST != null ? x.IGST : 0), 2).ToString("0.00")),
                        IGSTAmt = Convert.ToDecimal(Math.Round((double)(x.IGSTAmt != null ? x.IGSTAmt : 0), 2).ToString("0.00")),
                        GrossAmount = Convert.ToDecimal(Math.Round((double)x.GrossAmount, 2).ToString("0.00")),
                        HSNCode = x.HSNCode,
                        TaxType = x.Tax_type,
                        TCSRate = x.TCSRate.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSRate, 2).ToString("0.00")) : new decimal(0.00),
                        TCSAmount = x.TCSAmount.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSAmount, 2).ToString("0.00")) : new decimal(0.00),
                    }).ToList();
                }
                catch (Exception ex)
                {

                    dataObj.ItemData = null;
                }
                ViewBag.PrintamountInWord = print.Cal(dataObj.HeaderData.GrandTotal ?? 0);

                if (PurchaseTypeIdSel == 3 || PurchaseTypeIdSel == 4 || PurchaseTypeIdSel == 5 || PurchaseTypeIdSel == 6 || PurchaseTypeIdSel == 7 || PurchaseTypeIdSel == 8)
                {
                    //for delivery details code
                    if (id != null)
                    {
                        var Specific_De = (from de_I in objmms.TbDelivery_Details_PO

                                           where de_I.Purchase_order_No == id
                                           select new DeliveryPo_details_Chld
                                           {
                                               Delivery_Schedule = de_I.Delivery_Schedule,
                                               Delivery_Address = de_I.Delivery_Address,
                                               Contact_Person_Name = de_I.Contact_Person_Name,
                                               Contact_Person_Mobile = de_I.Contact_Person_Mobile,
                                               Billing_Address = de_I.Billing_Address,
                                               StoreKeeperName=de_I.StoreKeeperName,
                                               StoreKeeperNo=de_I.StoreKeeperContact,
                                               EPPerson = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.PersonName).FirstOrDefault(),
                                               EPContact = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.ContactNumber).FirstOrDefault(),
                                               DivType = de_I.ContactDivType
                                           }).ToList();

                        var data_delvry = Specific_De;
                        ViewBag.Delevery_details = data_delvry;

                        if (Specific_De.Count > 0)
                        {
                            int? purDivison = Specific_De[0].DivType;
                            if (purDivison != null)
                            {
                                var officeAddr = objmms.tbl_OfficeAddress.Where(s => s.Division == purDivison).Select(s => s).FirstOrDefault();
                                ViewBag.OfficeAddress = officeAddr.OfficeAddress;
                                ViewBag.OfficeMobile = officeAddr.Mobile;
                                ViewBag.OfficeContactName = officeAddr.ContactPerson;
                            }
                        }
                    }

                    else
                    {
                    }

                    return PartialView("_PartialView_for_Approved_Print_Slip_UltiTestForOutPro", dataObj);
                }
                else if (PurchaseTypeIdSel == 1 || PurchaseTypeIdSel == 2 || PurchaseTypeIdSel == 9 || PurchaseTypeIdSel == 10 || PurchaseTypeIdSel == 11)
                {
                    #region

                    //for genral terms and conditions

                    if (id != null)
                    {
                        var Genral_T_and_C = (from gtcs in objmms.tblGenral_Terms_Conditions_Child

                                              where gtcs.Purchase_Order_No == id && gtcs.Status == "Enable" && gtcs.IsActive == "Yes"

                                              select new Genral_term_conditon_Chld
                                              {
                                                  GTC_Description = gtcs.GTC_Description
                                              }).ToList();
                        var data_gtc = Genral_T_and_C;
                        ViewBag.Gtc_details = data_gtc;
                    }
                    else
                    {
                    }


                    //for Specific Instructions 
                    if (id != null)
                    {
                        var Specific_I = (from Spe_I in objmms.tblSpecific_Instructions_Child

                                          where Spe_I.Purchase_Order_No == id && Spe_I.Status == "Enable" && Spe_I.IsActive == "Yes"
                                          select new Specific_Instructions_TC_Chld
                                          {
                                              SI_Description = Spe_I.SI_Description
                                          }).ToList();
                        var data_si = Specific_I;
                        ViewBag.Si_details = data_si;
                    }

                    else
                    {
                    }

                    //for delivery details code
                    if (id != null)
                    {
                        var Specific_De = (from de_I in objmms.TbDelivery_Details_PO

                                           where de_I.Purchase_order_No == id
                                           select new DeliveryPo_details_Chld
                                           {
                                               Delivery_Schedule = de_I.Delivery_Schedule,
                                               Delivery_Address = de_I.Delivery_Address,
                                               Contact_Person_Name = de_I.Contact_Person_Name,
                                               Contact_Person_Mobile = de_I.Contact_Person_Mobile,
                                               Billing_Address = de_I.Billing_Address,
                                               StoreKeeperName = de_I.StoreKeeperName,
                                               StoreKeeperNo = de_I.StoreKeeperContact,
                                               DivType = de_I.ContactDivType
                                               //EPPerson = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.PersonName).FirstOrDefault(),
                                               //EPContact = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.ContactNumber).FirstOrDefault()
                                           }).ToList();

                        var data_delvry = Specific_De;
                        ViewBag.Delevery_details = data_delvry;

                        if (Specific_De.Count > 0)
                        {
                            int? purDivison = Specific_De[0].DivType;
                            if (purDivison != null)
                            {
                                var officeAddr = objmms.tbl_OfficeAddress.Where(s => s.Division == purDivison).Select(s => s).FirstOrDefault();
                                ViewBag.OfficeAddress = officeAddr.OfficeAddress;
                                ViewBag.OfficeMobile = officeAddr.Mobile;
                                ViewBag.OfficeContactName = officeAddr.ContactPerson;
                            }
                        }
                    }

                    else
                    {
                    }





                    //for Specific  terms and conditions
                    if (id != null)
                    {
                        var Specific_T_and_C = (from SpecificT_C in objmms.tblSpecificTermsAndConditions_Child

                                                where SpecificT_C.Purchase_Order_No == id && SpecificT_C.Status == "Enable" && SpecificT_C.IsActive == "Yes"
                                                select new Specific_Termss_And_conditions_Chld
                                                {
                                                    STC_Description = SpecificT_C.STC_Description
                                                }).ToList();
                        var data_stc = Specific_T_and_C;
                        ViewBag.Stc_details = data_stc;
                    }
                    else
                    {
                    }

                    return PartialView("_PartialView_for_Approved_Print_Slip_UltiTest", dataObj);

                    #endregion
                }
                else {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

            }
            else {

                    return Json(null, JsonRequestBehavior.AllowGet);
            }

        }






        public class PrintData
        {
            public HeaderData HeaderData { get; set; }
            public List< ItemData> ItemData { get; set;}
        }
        public class HeaderData
        {
            public decimal UId { get; set; }
            public bool? IsPORelease { get; set; }
            public string AcilTinNo { get; set; }
            public string AcilGSTNo { get; set; }
            public string VendorGSTNo { get; set; }
            public string PurchaseOrderNo { get; set; }
            public string IndentRefNo { get; set; }
            public string ProjectName { get; set; }
            public string CreatedBy { get; set; }

            [Column(TypeName = "date")]
            [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
            public DateTime? CreatedDate { get; set; }
            public string ProjectNo { get; set; }
            public Nullable<System.DateTime> PurchaseOrderDate { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Reference { get; set; }
            public Nullable<decimal> SurCharge { get; set; }
            public Nullable<decimal> Cartage { get; set; }
            public decimal? SubTotal { get; set; }
            public decimal? Total { get; set; }
            public decimal? GrandTotal { get; set; }

            public string MobileNo { get; set; }
            public string ContactPerson { get; set; }
            public string Location { get; set; }
            public Nullable<decimal> ExciseDutyRate { get; set; }
            public Nullable<decimal> ExciseDuty { get; set; }

            public string Excise_DutyType { get; set; }
            public string Cartage_Type_1 { get; set; }
            public string Cartage_Type_2 { get; set; }
            public string Insi_Type_1 { get; set; }
            public string Tax_1 { get; set; }
            public string Insi_Type_2 { get; set; }
            public string Insi_Type_3 { get; set; }
            public string Insi_Type_4 { get; set; }
            public Nullable<decimal> T_Total { get; set; }
            public Nullable<decimal> P_Total { get; set; }
            public Nullable<decimal> Ex_Total { get; set; }
            public Nullable<decimal> CR1_Total { get; set; }
            public Nullable<decimal> T_SubTotal1 { get; set; }
            public Nullable<decimal> T_Insi1 { get; set; }



            public Nullable<decimal> T_TotTax { get; set; }
            public Nullable<decimal> CR2_Total { get; set; }
            public Nullable<decimal> T_SubTotal2 { get; set; }
            public Nullable<decimal> T_Insi2 { get; set; }
            public Nullable<decimal> T_Insi3 { get; set; }
            public Nullable<decimal> T_Insi4 { get; set; }

            public string IsReleasePOBy { get; set; }
            public string PoCreatedBy { get; set; }
            public string UserType { get; set; }
            public string CheckedPoLimit { get; set; }
            public string PoApprovedBy { get; set; }
            public string IsCartage1 { get; set; }
            public string IsCartage2 { get; set; }
            public string IsExcise { get; set; }

            public string IsTax { get; set; }
            public string Is_Insi1 { get; set; }
            public string Is_Insi2 { get; set; }
            public string Is_Insi3 { get; set; }
            public string Is_Insi4 { get; set; }
            public string Vendor_country_state_city { get; set; }
            public string VendorTInNO { get; set; }
            //New 
            public decimal? TotalPackingSum { get; set; }
            public decimal? TotalCartageSum { get; set; }
            public decimal? TotalInsuranceSum { get; set; }
            public decimal? TotalTaxableAmountSum { get; set; }
            public decimal? TotalCGSTSum { get; set; }
            public decimal? TotalSCGSTAndUTGSTSum { get; set; }
            public decimal? IGSTSum { get; set; }
            public string TotalAmountInWords { get; set; }

            public DateTime? PIDate { get; set; }      
            public string OrderFor { get; set; }
            public string Email { get; set; }
            public decimal? TotalTCSAmt { get; set; }
            public bool TCSActive { get; set; }
        }
        public class ItemData
        {
            public decimal UId { get; set; }
            public bool? IsPORelease { get; set; }
            public string AcilTinNo { get; set; }
            public decimal? Vat { get; set; }
            public string PurchaseOrderNo { get; set; }
            public string IndentRefNo { get; set; }
            public decimal? SubTotal1 { get; set; }
            public decimal? SubTotal2 { get; set; }
            public decimal? ItemToTAmt { get; set; }
            public decimal? Total { get; set; }
            public decimal? GrandTotal { get; set; }
            public string CreatedBy { get; set; }
            [Column(TypeName = "date")]
            [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
            public DateTime? CreatedDate { get; set; }
            public string ProjectNo { get; set; }
            public Nullable<System.DateTime> PurchaseOrderDate { get; set; }
            public Nullable<decimal> MUId { get; set; }
            public string ItemNo { get; set; }
            public Nullable<decimal> Qty { get; set; }
            public Nullable<decimal> Rate { get; set; }
            public decimal? Amount { get; set; }
            public decimal? Discount { get; set; }

            public string ItemName { get; set; }
            public string HSNCode { get; set; }


            public string ProjectName { get; set; }

            public string UnitID { get; set; }
            public string UnitName { get; set; }

            //for vendor info
            public string Reference { get; set; }
            public string Remark { get; set; }
            public string SupplierNo { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string ItemDescrp { get; set; }

            //added
            public decimal? PackagePercentage { get; set; }
            public decimal? PackingPercentageAmt { get; set; }
            public string CartageType { get; set; }
            public decimal? CartageTypeRate { get; set; }
            public decimal? CartageAmt { get; set; }
            public decimal? InsurancePercentage { get; set; }
            public decimal? InsurancePercentageAmt { get; set; }

            public decimal? TaxableAmount { get; set; }
            public decimal? CGSTPercentage { get; set; }
            public decimal? SGSTAndUTGSTPercentage { get; set; }
            public decimal? IGST { get; set; }

            public decimal? CGSTAmt { get; set; }
            public decimal? SGSTAndUTGSTAmt { get; set; }
            public decimal? IGSTAmt { get; set; }

            public decimal? GrossAmount { get; set; }

            public decimal? TCSRate { get; set; }
            public decimal? TCSAmount { get; set; }

            public string TaxType { get; set; }





        }
        //get record from genral t& c







        public JsonResult GetEmpname(string id)
        {
            List<SelectListItem> groups = new List<SelectListItem>();
            var groupList = this.Getprojectname(Convert.ToString(id));
            var employeeData = groupList.Select(m => new SelectListItem()
            {
                Text = m.FName,
                Value = m.EmpID.ToString(),
            });
            return Json(employeeData, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetPurchaseEmpname(string id)
        {
            List<SelectListItem> groups = new List<SelectListItem>();
            var groupList = (from a in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(id)) join b in objmms.tblApproval_AccountType.Where(p =>p.TypeName == "Purchase") on a.EmpID equals b.EmployeeId select new { a }).ToList();
            List<DAL.tblEmployeeMaster> grlst = (from s in groupList select s.a).ToList();
            var employeeData = grlst.Select(m => new SelectListItem()
            {
                Text = m.FName,
                Value = m.EmpID.ToString(),
            });
            return Json(employeeData, JsonRequestBehavior.AllowGet);
        }



        // Get State from DB by Project ID
        public IList<tblEmployeeMaster> Getprojectname(string ProjectId)
        {
            return objmms.tblEmployeeMasters.Where(a => a.ProjectID.Contains(ProjectId)).DefaultIfEmpty<tblEmployeeMaster>().ToList();
        }

        public ActionResult SaveRemark(POApprovalwithRemarks ListDatas)
        {  // POApprovalwithRemarks TbIndentPurchaseOrder_Approval_For_Print
            string empId = Session["EmpID"].ToString();

            string emptype = objmms.tblApproval_AccountType.Where(p => p.EmployeeId == empId.ToString()).First().TypeName;

            try
            {
                if (ModelState.IsValid)
                {
                    tblALLRemark alrmrks = new tblALLRemark();

                    if (ListDatas.Status_Approval == "Approved")
                    {
                        if (emptype == "PMC")
                        {
                            //  TbIndentPurchaseOrder_Approval_For_Print emp = objmms.TbIndentPurchaseOrder_Approval_For_Print.Single(em => em.Purchase_Order_Indent_No == ListDatas.Purchase_Order_Indent_No);

                            TbIndentPurchaseOrder_GST emp = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).FirstOrDefault();

                            //emp.Status_Approval = "Not Approved";
                            //emp.Status = "Pending";
                            emp.Status = "ApprovedPMC";
                            //emp.ForwordTo = ListDatas.ForwordTo;
                            // emp.Remark = ListDatas.Remark;
                            //emp.ModifiedDate = DateTime.Now;
                            // emp.Updated_Employee_Id = empId.ToString();
                            // new added
                            emp.FirstLevelApprv_Remarks = ListDatas.Remark;
                            emp.FirstLevelApprv_Status = "Approved";
                            emp.FirstLevelApprv_Date = System.DateTime.Now;
                            // emp.SecondLevelApprv_Id = ListDatas.ForwordTo;
                            emp.SendTo = ListDatas.ForwordTo;


                            objmms.Entry(emp).State = EntityState.Modified;

                            // save remarks in tblALLRemark table.
                            alrmrks.CreatedDate = System.DateTime.Now;
                            alrmrks.RemarkBy = empId;
                            alrmrks.RemarkDate = System.DateTime.Now;
                            alrmrks.Remarks = ListDatas.Remark;
                            alrmrks.RemarkStage = emptype;
                            alrmrks.PurchaseOrderNo = ListDatas.Purchase_Order_Indent_No;
                            objmms.tblALLRemarks.Add(alrmrks);


                            //if (ListDatas.Purchase_Order_Indent_No != null)
                            //{
                            //    TbIndentPurchaseOrderMaster bg = objmms.TbIndentPurchaseOrderMasters.Where(l => l.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).First();
                            //    bg.Status = "Approved";
                            //    objmms.Entry(bg).State = EntityState.Modified;
                            //}
                            //else
                            //{ }
                        }
                        else if (emptype == "Purchase")
                        {
                            TbIndentPurchaseOrder_GST emp1 = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).FirstOrDefault();
                            emp1.Status = "Pending";
                            emp1.SendTo = ListDatas.ForwordTo;
                            emp1.SecondLevelApprv_Id = ListDatas.ForwordTo;
                            objmms.Entry(emp1).State = EntityState.Modified;
                            alrmrks.CreatedDate = System.DateTime.Now;
                            alrmrks.RemarkBy = empId;
                            alrmrks.RemarkDate = System.DateTime.Now;
                            alrmrks.Remarks = ListDatas.Remark;
                            alrmrks.RemarkStage = emptype;
                            alrmrks.PurchaseOrderNo = ListDatas.Purchase_Order_Indent_No;
                            objmms.tblALLRemarks.Add(alrmrks);

                        }
                        else if (emptype == "PIC" || emptype == "Mang" || emptype == "Purchase Head")
                        {
                            //  TbIndentPurchaseOrder_Approval_For_Print emp = objmms.TbIndentPurchaseOrder_Approval_For_Print.Single(em => em.Purchase_Order_Indent_No == ListDatas.Purchase_Order_Indent_No);

                            TbIndentPurchaseOrder_GST emp = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).FirstOrDefault();

                            //emp.Status_Approval = ListDatas.Status_Approval;
                            //emp.Status = "Approved";
                            // emp.ForwordTo = ListDatas.ForwordTo;
                            //  emp.Remark = ListDatas.Remark;
                            //emp.ModifiedDate = DateTime.Now;
                            //emp.Updated_Employee_Id = empId.ToString();
                            emp.Status = ListDatas.Status_Approval;
                            // new added
                            emp.SecondLevelApprv_Remarks = ListDatas.Remark;
                            emp.SecondLevelApprv_Status = "Approved";
                            emp.SecondLevelApprv_Date = System.DateTime.Now;
                           
                           // emp.SecondLevelApprv_Id = ListDatas.ForwordTo;


                            objmms.Entry(emp).State = EntityState.Modified;

                            alrmrks.CreatedDate = System.DateTime.Now;
                            alrmrks.RemarkBy = empId;
                            alrmrks.RemarkDate = System.DateTime.Now;
                            alrmrks.Remarks = ListDatas.Remark;
                            alrmrks.RemarkStage = emptype;
                            alrmrks.PurchaseOrderNo = ListDatas.Purchase_Order_Indent_No;
                            objmms.tblALLRemarks.Add(alrmrks);



                            //if (ListDatas.Purchase_Order_Indent_No != null)
                            //{
                            //    TbIndentPurchaseOrderMaster bg = objmms.TbIndentPurchaseOrderMasters.Where(l => l.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).First();
                            //    bg.Status = "Approved";
                            //    objmms.Entry(bg).State = EntityState.Modified;
                            //}
                            //else
                            //{ }
                        }
                        else { }

                        

                        
                    }
                    else
                    {
                        if (emptype == "PMC")
                        {
                            //TbIndentPurchaseOrder_Approval_For_Print emp = objmms.TbIndentPurchaseOrder_Approval_For_Print.Single(em => em.Purchase_Order_Indent_No == ListDatas.Purchase_Order_Indent_No);
                            TbIndentPurchaseOrder_GST emp = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).FirstOrDefault();


                            //emp.Status_Approval = ListDatas.Status_Approval;
                            //emp.Status = "Not Approved";
                            //emp.EditedStatus = "No";
                            //emp.ForwordTo = ListDatas.ForwordTo;
                            //// emp.Remark = ListDatas.Remark;
                            //emp.ModifiedDate = DateTime.Now;
                            //emp.Updated_Employee_Id = empId.ToString();
                            // objmms.tblSpecific_Instructions.Add(gtc_tbl);

                            emp.Status = "Not Approved";
                            emp.SendTo = ListDatas.ForwordTo;
                            emp.Edited_Status = "No";

                            emp.FirstLevelApprv_Remarks = ListDatas.Remark;
                            emp.FirstLevelApprv_Status = "Not Approved";
                            emp.FirstLevelApprv_Date = System.DateTime.Now;


                            objmms.Entry(emp).State = EntityState.Modified;

                            alrmrks.CreatedDate = System.DateTime.Now;
                            alrmrks.RemarkBy = empId;
                            alrmrks.RemarkDate = System.DateTime.Now;
                            alrmrks.Remarks = ListDatas.Remark;
                            alrmrks.RemarkStage = emptype;
                            alrmrks.PurchaseOrderNo = ListDatas.Purchase_Order_Indent_No;
                            objmms.tblALLRemarks.Add(alrmrks);



                            //if (ListDatas.Purchase_Order_Indent_No != null)
                            //{
                            //    TbIndentPurchaseOrderMaster bg = objmms.TbIndentPurchaseOrderMasters.Where(l => l.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).First();
                            //    bg.Status = "Not Approved";
                            //    objmms.Entry(bg).State = EntityState.Modified;
                            //}
                            //else
                            //{ }
                        }
                        else if (emptype == "Purchase" || emptype == "PIC" || emptype == "Mang" || emptype == "Purchase Head")
                        {
                            //TbIndentPurchaseOrder_Approval_For_Print emp = objmms.TbIndentPurchaseOrder_Approval_For_Print.Single(em => em.Purchase_Order_Indent_No == ListDatas.Purchase_Order_Indent_No);
                            TbIndentPurchaseOrder_GST emp = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).FirstOrDefault();

                            //emp.Status_Approval = ListDatas.Status_Approval;
                            //emp.Status = "Not Approved";
                            //emp.EditedStatus = "No";
                            //emp.ForwordTo = ListDatas.ForwordTo;
                            //emp.Remark = ListDatas.Remark;
                            //emp.ModifiedDate = DateTime.Now;
                            //emp.Updated_Employee_Id = empId.ToString();
                            // objmms.tblSpecific_Instructions.Add(gtc_tbl);
                            emp.SendTo = ListDatas.ForwordTo;
                            emp.Status = ListDatas.Status_Approval;
                            emp.SecondLevelApprv_Remarks = ListDatas.Remark;
                            emp.SecondLevelApprv_Status = "Not Approved";
                            emp.SecondLevelApprv_Date = System.DateTime.Now;


                            objmms.Entry(emp).State = EntityState.Modified;


                            alrmrks.CreatedDate = System.DateTime.Now;
                            alrmrks.RemarkBy = empId;
                            alrmrks.RemarkDate = System.DateTime.Now;
                            alrmrks.Remarks = ListDatas.Remark;
                            alrmrks.RemarkStage = emptype;
                            alrmrks.PurchaseOrderNo = ListDatas.Purchase_Order_Indent_No;
                            objmms.tblALLRemarks.Add(alrmrks);


                            //if (ListDatas.Purchase_Order_Indent_No != null)
                            //{
                            //    TbIndentPurchaseOrderMaster bg = objmms.TbIndentPurchaseOrderMasters.Where(l => l.PurchaseOrderNo == ListDatas.Purchase_Order_Indent_No).First();
                            //    bg.Status = "Not Approved";
                            //    objmms.Entry(bg).State = EntityState.Modified;
                            //}
                            //else
                            //{ }
                        }
                        else {


                        }


                        
                    }
                 objmms.SaveChanges();
                }
                return View("Index");
            }
            catch
            {
                return Json("2", JsonRequestBehavior.AllowGet);
            }
        }

        public string UserType(string PurchOrder)
        {
            string  PmcId = objmms.TbIndentPurchaseOrder_Approval_For_Print.Where(x => x.Purchase_Order_Indent_No == PurchOrder).FirstOrDefault().FirstLevelApprv_Id;
            string  emptype = objmms.tblApproval_AccountType.Where(p => p.EmployeeId == PmcId.ToString()).First().TypeName;
            return emptype;
        }

        public ActionResult GetAllPORemarks(string PO)
        {
           // string Proj_ID = Session["ProjectssId"].ToString();

            string PO_ID = PO.Replace("#", "/");
            //string Proj_ID = objmms.TbIndentPurchaseOrderMasters.Where(x => x.PurchaseOrderNo == PO_ID.ToString()).FirstOrDefault().ProjectNo;
            string Proj_ID = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PO_ID.ToString()).FirstOrDefault().ProjectNo;
            List<POAllRemarks> res = null;
            // res = objpro.GetAllPORemarks(Proj_ID, PO_ID); //GetAllPORemarksGST
            // res = objpro.GetAllPORemarksGST(Proj_ID, PO_ID);
            res = objpro.GetAllNewPORemarks_GST(PO_ID);
            if (res != null)
            {
                return PartialView("_Partial_POAllRemarks", res);
            }
            else {
                return View("_EmptyView");

            }
        }


        public ActionResult DisApprovealPOList(string PId = "")
        {
            string empId = Session["EmpID"].ToString();

            ViewBag.EmpId = empId;
            ViewBag.PId = PId;
            return View();
        }
       
        public ActionResult DisApprovedPOGrid(string ProjectId,string PONumber="",string VendorId="")
        {
            string empId = Session["EmpID"].ToString(); bool TrueVal = true;
            //(i.Status == "Pending" && i.SecondLevelApprv_Status == "Not Approved") || (i.FirstLevelApprv_Status == "Not Approved" && i.Status == "Not Approved") ||
            var a = objmms.TbIndentPurchaseOrder_GST.Where(i => i.Status == "Not Approved" && i.ProjectNo == ProjectId && (i.CreatedBy == empId || i.SendTo == empId || i.FirstLevelApprv_Id == empId || i.SecondLevelApprv_Id == empId)).OrderByDescending(c => c.CreatedDate).Select(xx => new Getdata
            {
                //Id = xx.Id,
                Purchase_Order_Indent_No = xx.PurchaseOrderNo,
                CreatedDate = xx.CreatedDate,
                Send_To = xx.Status == "Not Approved" && xx.SecondLevelApprv_Id == null ? objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.FirstLevelApprv_Id).FirstOrDefault().LName
                     : objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SecondLevelApprv_Id).FirstOrDefault().LName,
                Vendor_Group_Id = xx.Vendor_Group_Id,
                Vendor_Name = objmms.tblVendorMasters.Where(x => x.VendorID == xx.SupplierNo).FirstOrDefault().Name,
                POCreatedDate = xx.CreatedDate,
                Status = xx.Status,
                FirstLevelApprv_Date = xx.SecondLevelApprv_Status == "Not Approved" ? xx.SecondLevelApprv_Date : xx.FirstLevelApprv_Date,
                QuotID = objmms.tblMMSQuotations.Where(x => x.PurchaseReqNo == xx.IndentRefNo).FirstOrDefault().TransId,
                DiffStagePerson = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == xx.PurchaseOrderNo).FirstOrDefault().CheckedBeyondPOLimit == TrueVal ? "Purchase" : "PMC",
                Stage =  objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SendTo).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == xx.SendTo).FirstOrDefault().LName
                       
            }).ToList();

            if (VendorId != "Select Vendor")
            {
                a = a.Where(x => !string.IsNullOrEmpty(x.Vendor_Name) && x.Vendor_Name.Contains(VendorId)).ToList();
            }

            if (!string.IsNullOrEmpty(PONumber))
            {
                a = a.Where(x => x.Purchase_Order_Indent_No.Contains(PONumber)).ToList();
            }

            var totalRows = a.Count();

            var data = new PrinSlip_Approval_VMGST()
            {
                TotalRows = totalRows,
                PageSize = 3500,
                GetDetails_Approved_PO_Slip = a
            };


            if (data != null && data.TotalRows != 0)
            {


                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PartialView_For_DisApprovalEdit", data);
                }

                else
                {
                    return PartialView("_EmptyView");
                }

            }

            else
            {
                return View("_EmptyView");
            }



        }

        public ActionResult GetPOUserOnly(string PurchaseorderNo, string ProjectNo)
        {
            string POcreatedBy = objmms.TbIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == PurchaseorderNo && x.ProjectNo == ProjectNo).FirstOrDefault().CreatedBy;
            var Emp = objmms.tblEmployeeMasters.Where(x => x.EmpID == POcreatedBy).ToList().Select(x => new { Text = x.FName + ' ' + x.LName, Value = x.EmpID }).ToList();
            return Json(Emp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewForwared(string id)
        {
            List<SelectListItem> groups = new List<SelectListItem>();
            var groupList = (from a in objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(id)) join b in objmms.tblApproval_AccountType.Where(p => p.TypeName == "Purchase Head" || p.TypeName == "PIC" || p.TypeName == "Mang") on a.EmpID equals b.EmployeeId select new { a }).ToList();
            List<DAL.tblEmployeeMaster> grlst = (from s in groupList select s.a).ToList();
            var employeeData = grlst.Select(m => new SelectListItem()
            {
                Text = m.FName + " " + m.LName,
                Value = m.EmpID.ToString(),
            });
            return Json(employeeData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDraftPO_details_with_Indent(string id)
        {
            WordToText print = new WordToText();
            string empId = Session["EmpID"].ToString();

            //main code start here
            id = id?.Replace("#", "/");
            Session["POId"] = id.ToString();

            var a = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).ToList();
            var t = a.Select(x => new SelectListItem
            {
                Value = x.ProjectNo,
                Text = objmms.tblProjectMasters.Where(b => b.PRJID == x.ProjectNo).First().ProjectName
            });
            ViewBag.prjtid = t;

            string PurReq = objmms.TbDraftIndentPurchaseOrder_GST.Where(x => x.PurchaseOrderNo == id.ToString()).FirstOrDefault().IndentRefNo;
            int? PurchaseTypeIdSel = objmms.PurchaseRequisitionMasters.Where(x => x.PurRequisitionNo == PurReq).FirstOrDefault().PurchasePIC_Type;
            ViewBag.PurDate = objmms.PurchaseRequisitionMasters.SingleOrDefault(x => x.PurRequisitionNo == PurReq).Date.Value;

            var units = "";
            if (id != null)
            {
                var querys = (from mas in objmms.TbDraftIndentPurchaseOrder_GST
                              join chld in objmms.TbDraftIndentPurchaseOrderChild_GST

                             // Both anonymous types should have exact same number of properties having same name and datatype
                             // on new { a = (int?)mas.UId } equals new { a = chld.MUId }
                             on mas.UId equals chld.MUId into pchld
                              from pch in pchld.DefaultIfEmpty()
                              where mas.PurchaseOrderNo == id
                              select new Purchase_Order_Slip_VM
                              {
                                  SupplierNo = mas.SupplierNo,
                                  Name = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Name,
                                  Address = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Address,
                                  Vendor_country_state_city = objmms.tblCityLists.Where(k3 => k3.CityID == objmms.tblVendorMasters.Where(ct => ct.VendorID == mas.SupplierNo).FirstOrDefault().City).FirstOrDefault().CityName + ", " +
                                            objmms.tblStates.Where(k2 => k2.StateID == objmms.tblVendorMasters.Where(st => st.VendorID == mas.SupplierNo).FirstOrDefault().State).FirstOrDefault().StateName + ", " +
                                            objmms.tblCountries.Where(k1 => k1.CountryID == objmms.tblVendorMasters.Where(gr => gr.VendorID == mas.SupplierNo).FirstOrDefault().Country).FirstOrDefault().CnName,

                                  VendorTInNO = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().TinNo,
                                  VendorGSTNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().GST_NO,
                                  AcilTinNo = objmms.tblProjectTINNoes.Where(k => k.ProjectId == mas.ProjectNo).FirstOrDefault().TINNo,
                                  AcilGSTNo = objmms.tblProjectGSTNoes.Where(k => k.ProjectId == mas.ProjectNo).FirstOrDefault().GSTNo,
                                  MobileNo = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().MobileNo,
                                  ContactPerson = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().ContactPerson,
                                  Email = objmms.tblVendorMasters.Where(k => k.VendorID == mas.SupplierNo).FirstOrDefault().Email,



                                  Reference = mas.Reference,

                                  IndentRefNo = mas.IndentRefNo,
                                  PurchaseOrderNo = mas.PurchaseOrderNo,

                                  SubTotal = pch.SubTotal1,
                                  Total = mas.Total,
                                  GrandTotal = mas.GrandTotal,
                                  ProjectNo = mas.ProjectNo,
                                  ProjectName = objmms.tblProjectMasters.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().ProjectName,
                                  Location = objmms.tblProjectParticularsDetailAs.Where(k => k.PRJID == mas.ProjectNo).FirstOrDefault().Location,

                                  PurchaseOrderDate = mas.PurchaseOrderDate,
                                  ItemNo = objmms.tblItemMasters.Where(y => y.ItemID == pch.ItemNo).FirstOrDefault().GITEMCode,
                                  ItemName = objmms.tblItemMasters.Where(j => j.ItemID == pch.ItemNo).FirstOrDefault().ItemName,
                                  UnitID = objmms.tblItemMasters.Where(l => l.ItemID == pch.ItemNo).FirstOrDefault().UnitID,
                                  UnitName = objmms.tblUnitMasters.Where(l => l.UnitID == objmms.tblItemMasters.Where(ll => ll.ItemID == pch.ItemNo).FirstOrDefault().UnitID).FirstOrDefault().UnitCode,
                                  Amount = pch.Total,
                                  Discount = pch.Discount,
                                  Qty = pch.Qty,
                                  Rate = pch.NewRate,


                                  //  --------------------added Newely here

                                  Cartage_Type_1 = objmms.tblMMSCartageTypes.Where(x => x.TransId == pch.CartageTypeId).FirstOrDefault().CartageType,
                                  CR1_Total = pch.CartageAmount,
                                  T_Total = mas.Total,
                                  P_Total = pch.PackingChargesAmount,
                                  T_SubTotal1 = mas.SubTotal1,
                                  T_SubTotal2 = mas.SubTotal2,

                                  POCreatedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.CreatedBy).FirstOrDefault().LName,
                                  // UserType = UserType(mas.PurchaseOrderNo)
                                  UserType = objmms.tblApproval_AccountType.Where(x => x.EmployeeId == empId).FirstOrDefault().TypeName,
                                  CheckedPoLimit = mas.CheckedBeyondPOLimit,
                                  IsReleasePOBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().FName + " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.POReleaseBy).FirstOrDefault().LName,
                                  PoApprovedBy = objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().FName +
                                                 " " + objmms.tblEmployeeMasters.Where(x => x.EmpID == mas.SecondLevelApprv_Id).FirstOrDefault().LName,

                                  // Added After GST.

                                  ItemToTAmt = pch.Total,
                                  SubTotal1 = pch.SubTotal1,
                                  SubTotal2 = pch.SubTotal2,
                                  PackagePercentage = pch.PackingChargesPercentage,
                                  PackingPercentageAmt = pch.PackingChargesAmount,
                                  CartageType = objmms.tblMMSCartageTypes.Where(x => x.TransId == pch.CartageTypeId).FirstOrDefault().CartageType,
                                  CartageTypeRate = pch.Cartage_rate,
                                  CartageAmt = pch.CartageAmount,
                                  InsurancePercentage = pch.InsuranceRate,
                                  InsurancePercentageAmt = pch.InsuranceAmount,
                                  TaxableAmount = pch.SubTotal2,
                                  CGSTPercentage = pch.CGSTPercentage,
                                  CGSTAmt = pch.CGSTAmount,
                                  SGSTAndUTGSTPercentage = pch.SGSTPercentage,
                                  SGSTAndUTGSTAmt = pch.SGSTAmount,
                                  IGST = pch.IGSTPercentage,
                                  IGSTAmt = pch.IGSTAmount,
                                  GrossAmount = pch.GrossAmount,
                                  HSNCode = objmms.tblItemMasters.Where(x => x.ItemID == pch.ItemNo).FirstOrDefault().HSNCode,
                                  TotalPackingSum = mas.Total_PAndF,
                                  TotalCartageSum = mas.Total_Cartage,
                                  TotalInsuranceSum = mas.Total_Insurance,
                                  TotalTaxableAmountSum = mas.SubTotal2,
                                  TotalCGSTSum = mas.Total_CGST,
                                  TotalSCGSTAndUTGSTSum = mas.Total_SGSTAndUTGST,
                                  IGSTSum = mas.Total_IGST,
                                  TotalAmountInWords = mas.Total_NetAmountInWords,
                                  ItemDescrp = pch.Item_Description,
                                  IsPORelease = mas.IsPORelease,
                                  PODescription = mas.PODescription,
                                  TCSRate = pch.TCS_Rate,
                                  TCSAmount = pch.TotalTCSAmount,
                                  TotalTCS = mas.Total_TCS,
                                  Tax_type = pch.TaxRateType,
                                  TCSActive = mas.TCSActive != null ? mas.TCSActive.Value : false
                              }).ToList();

                var data = querys;
                PrintData dataObj = new PrintData();

                dataObj.HeaderData = data.Select(x => new HeaderData
                {
                    PurchaseOrderNo = x.PurchaseOrderNo,
                    Total = Convert.ToDecimal(Math.Round((double)(x.Total != null ? x.Total : 0), 2).ToString("0.00")),
                    Location = x.Location,
                    ContactPerson = x.ContactPerson,
                    Vendor_country_state_city = x.Vendor_country_state_city,
                    VendorTInNO = x.VendorTInNO,
                    VendorGSTNo = x.VendorGSTNo,
                    MobileNo = x.MobileNo,
                    Reference = x.Reference,

                    Cartage = x.Cartage,
                    GrandTotal = Convert.ToDecimal(Math.Round((double)(x.GrandTotal != null ? x.GrandTotal : 0), 2).ToString("0.00")),
                    SubTotal = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                    PurchaseOrderDate = x.PurchaseOrderDate,
                    AcilTinNo = x.AcilTinNo,
                    AcilGSTNo = x.AcilGSTNo,
                    Address = x.Address,
                    Name = x.Name,
                    ProjectName = x.ProjectName,
                    IndentRefNo = x.IndentRefNo,


                    //Ex_Total = Math.Round((decimal)data.Sum(k => k.Ex_Total), 2),
                    Cartage_Type_1 = x.Cartage_Type_1,
                    CR1_Total = Convert.ToDecimal(Math.Round((decimal)data.Sum(k => k.CR1_Total), 2).ToString("0.00")),



                    T_Total = x.T_Total,
                    P_Total = Math.Round((decimal)data.Sum(k => k.P_Total), 2),
                    T_SubTotal1 = x.T_SubTotal1,
                    T_SubTotal2 = x.T_SubTotal2,
                    ProjectNo = x.ProjectNo,
                    PoCreatedBy = x.POCreatedBy,
                    UserType = x.UserType,
                    CheckedPoLimit = x.CheckedPoLimit.ToString(),
                    PoApprovedBy = x.PoApprovedBy,
                    IsReleasePOBy = x.IsReleasePOBy,
                    TotalPackingSum = Convert.ToDecimal(Math.Round((double)(x.TotalPackingSum != null ? x.TotalPackingSum : 0), 2).ToString("0.00")),
                    TotalCartageSum = Convert.ToDecimal(Math.Round((double)(x.TotalCartageSum != null ? x.TotalCartageSum : 0), 2).ToString("0.00")),
                    TotalInsuranceSum = x.TotalInsuranceSum,
                    TotalTaxableAmountSum = Convert.ToDecimal(Math.Round((double)(x.TotalTaxableAmountSum != null ? x.TotalTaxableAmountSum : 0), 2).ToString("0.00")),
                    TotalCGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalCGSTSum != null ? x.TotalCGSTSum : 0), 2).ToString("0.00")),
                    TotalSCGSTAndUTGSTSum = Convert.ToDecimal(Math.Round((double)(x.TotalSCGSTAndUTGSTSum != null ? x.TotalSCGSTAndUTGSTSum : 0), 2).ToString("0.00")),
                    IGSTSum = Convert.ToDecimal(Math.Round((double)(x.IGSTSum != null ? x.IGSTSum : 0), 2).ToString("0.00")),
                    TotalAmountInWords = x.TotalAmountInWords,
                    IsPORelease = x.IsPORelease,
                    OrderFor = x.PODescription,
                    Email = x.Email,
                    TotalTCSAmt = Convert.ToDecimal(Math.Round((double)(x.TotalTCS != null ? x.TotalTCS : 0), 2).ToString("0.00")),
                    TCSActive = x.TCSActive
                }).FirstOrDefault();


                if (dataObj.HeaderData != null)
                {
                    dataObj.HeaderData.PIDate = objmms.PurchaseRequisitionMasters.FirstOrDefault(x => x.PurRequisitionNo == dataObj.HeaderData.IndentRefNo).Date;
                }

                try
                {

                    dataObj.ItemData = data.Select(x => new ItemData
                    {
                        ItemName = x.ItemName,
                        ItemNo = x.ItemNo,
                        ItemDescrp = x.ItemDescrp,
                        Remark = x.Remark,
                        UnitName = x.UnitName,
                        Rate = Convert.ToDecimal(Math.Round((double)(x.Rate != null ? x.Rate : 0), 2).ToString("0.00")),
                        Discount = Convert.ToDecimal(Math.Round((double)(x.Discount != null ? x.Discount : 0), 2).ToString("0.00")),
                        SubTotal1 = Convert.ToDecimal(Math.Round((double)(x.SubTotal != null ? x.SubTotal : 0), 2).ToString("0.00")),
                        Qty = x.Qty,
                        ItemToTAmt = Convert.ToDecimal(Math.Round((double)(x.ItemToTAmt != null ? x.ItemToTAmt : 0), 2).ToString("0.00")),
                        SubTotal2 = Convert.ToDecimal(Math.Round((double)(x.SubTotal2 != null ? x.SubTotal2 : 0), 2).ToString("0.00")),
                        PackagePercentage = Convert.ToDecimal(Math.Round((double)(x.PackagePercentage != null ? x.PackagePercentage : 0), 2).ToString("0.00")),
                        PackingPercentageAmt = Convert.ToDecimal(Math.Round((double)(x.PackingPercentageAmt != null ? x.PackingPercentageAmt : 0), 2).ToString("0.00")),
                        CartageType = x.CartageType,
                        CartageTypeRate = Convert.ToDecimal(Math.Round((double)(x.CartageTypeRate != null ? x.CartageTypeRate : 0), 2).ToString("0.00")),
                        CartageAmt = Convert.ToDecimal(Math.Round((double)(x.CartageAmt != null ? x.CartageAmt : 0), 2).ToString()),
                        InsurancePercentage = x.InsurancePercentage,
                        InsurancePercentageAmt = x.InsurancePercentageAmt,
                        TaxableAmount = Convert.ToDecimal(Math.Round((double)(x.TaxableAmount != null ? x.TaxableAmount : 0), 2).ToString("0.00")),
                        CGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.CGSTPercentage != null ? x.CGSTPercentage : 0), 2).ToString("0.00")),
                        CGSTAmt = Convert.ToDecimal(Math.Round((double)(x.CGSTAmt != null ? x.CGSTAmt : 0), 2).ToString("0.00")),
                        SGSTAndUTGSTPercentage = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTPercentage != null ? x.SGSTAndUTGSTPercentage : 0), 2).ToString("0.00")),
                        SGSTAndUTGSTAmt = Convert.ToDecimal(Math.Round((double)(x.SGSTAndUTGSTAmt != null ? x.SGSTAndUTGSTAmt : 0), 2).ToString("0.00")),
                        IGST = Convert.ToDecimal(Math.Round((double)(x.IGST != null ? x.IGST : 0), 2).ToString("0.00")),
                        IGSTAmt = Convert.ToDecimal(Math.Round((double)(x.IGSTAmt != null ? x.IGSTAmt : 0), 2).ToString("0.00")),
                        GrossAmount = x.GrossAmount.HasValue ? Convert.ToDecimal(Math.Round((double)x.GrossAmount, 2).ToString("0.00")) : new decimal(0.00),
                        HSNCode = x.HSNCode,
                        TCSRate = x.TCSRate.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSRate, 2).ToString("0.00")) : new decimal(0.00),
                        TCSAmount = x.TCSAmount.HasValue ? Convert.ToDecimal(Math.Round((double)x.TCSAmount, 2).ToString("0.00")) : new decimal(0.00),
                        TaxType = x.Tax_type
                    }).ToList();

                }
                catch (Exception ex)
                {
                    dataObj.ItemData = null;
                }
                ViewBag.PrintamountInWord = print.Cal(dataObj.HeaderData.GrandTotal ?? 0);




                if (PurchaseTypeIdSel == 3 || PurchaseTypeIdSel == 4 || PurchaseTypeIdSel == 5 || PurchaseTypeIdSel == 6 || PurchaseTypeIdSel == 7 || PurchaseTypeIdSel == 8)
                {
                    //for delivery details code
                    if (id != null)
                    {
                        var Specific_De = (from de_I in objmms.TbDraftDelivery_Details_PO

                                           where de_I.Purchase_order_No == id
                                           select new DeliveryPo_details_Chld
                                           {
                                               Delivery_Schedule = de_I.Delivery_Schedule,
                                               Delivery_Address = de_I.Delivery_Address,
                                               Contact_Person_Name = de_I.Contact_Person_Name,
                                               Contact_Person_Mobile = de_I.Contact_Person_Mobile,
                                               Billing_Address = de_I.Billing_Address,
                                               StoreKeeperName = de_I.StoreKeeperName,
                                               StoreKeeperNo = de_I.StoreKeeperContact,
                                               EPPerson = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.PersonName).FirstOrDefault(),
                                               EPContact = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.ContactNumber).FirstOrDefault(),
                                               DivType = de_I.ContactDivType
                                           }).ToList();

                        var data_delvry = Specific_De;
                        ViewBag.Delevery_details = data_delvry;

                        if (Specific_De.Count > 0)
                        {
                            int? purDivison = Specific_De[0].DivType;
                            if (purDivison != null)
                            {
                                var officeAddr = objmms.tbl_OfficeAddress.Where(s => s.Division == purDivison).Select(s => s).FirstOrDefault();
                                ViewBag.OfficeAddress = officeAddr.OfficeAddress;
                                ViewBag.OfficeMobile = officeAddr.Mobile;
                                ViewBag.OfficeContactName = officeAddr.ContactPerson;
                            }
                        }
                    }

                    else
                    {
                    }
                    return PartialView("_PartialView_for_Approved_Print_SlipTest_ForOutPro", dataObj);
                }
                else if (PurchaseTypeIdSel == 1 || PurchaseTypeIdSel == 2 || PurchaseTypeIdSel == 9 || PurchaseTypeIdSel == 10 || PurchaseTypeIdSel == 11)
                {
                    #region

                    //for genral terms and conditions

                    if (id != null)
                    {
                        var Genral_T_and_C = (from gtcs in objmms.tblDraftGenral_Terms_Conditions_Child

                                              where gtcs.Purchase_Order_No == id && gtcs.Status == "Enable" && gtcs.IsActive == "Yes"

                                              select new Genral_term_conditon_Chld
                                              {
                                                  GTC_Description = gtcs.GTC_Description
                                              }).ToList();
                        var data_gtc = Genral_T_and_C;
                        ViewBag.Gtc_details = data_gtc;
                    }
                    else
                    {
                    }


                    //for Specific Instructions 
                    if (id != null)
                    {
                        var Specific_I = (from Spe_I in objmms.tblDraftSpecific_Instructions_Child

                                          where Spe_I.Purchase_Order_No == id && Spe_I.Status == "Enable" && Spe_I.IsActive == "Yes"
                                          select new Specific_Instructions_TC_Chld
                                          {
                                              SI_Description = Spe_I.SI_Description
                                          }).ToList();
                        var data_si = Specific_I;
                        ViewBag.Si_details = data_si;
                    }

                    else
                    {
                    }

                    //for delivery details code
                    if (id != null)
                    {
                        var Specific_De = (from de_I in objmms.TbDraftDelivery_Details_PO

                                           where de_I.Purchase_order_No == id
                                           select new DeliveryPo_details_Chld
                                           {
                                               Delivery_Schedule = de_I.Delivery_Schedule,
                                               Delivery_Address = de_I.Delivery_Address,
                                               Contact_Person_Name = de_I.Contact_Person_Name,
                                               Contact_Person_Mobile = de_I.Contact_Person_Mobile,
                                               Billing_Address = de_I.Billing_Address,
                                               StoreKeeperName = de_I.StoreKeeperName,
                                               StoreKeeperNo = de_I.StoreKeeperContact,
                                               EPPerson = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.PersonName).FirstOrDefault(),
                                               EPContact = objmms.tbl_ContactPersonMaster.Where(s => s.ContactNumber == de_I.EPContactPerID).Select(s => s.ContactNumber).FirstOrDefault(),
                                               DivType = de_I.ContactDivType
                                           }).ToList();

                        var data_delvry = Specific_De;
                        ViewBag.Delevery_details = data_delvry;

                        if (Specific_De.Count > 0)
                        {
                            int? purDivison = Specific_De[0].DivType;
                            if (purDivison != null && purDivison != 0)
                            {
                                var officeAddr = objmms.tbl_OfficeAddress.Where(s => s.Division == purDivison).Select(s => s).FirstOrDefault();
                                ViewBag.OfficeAddress = officeAddr.OfficeAddress;
                                ViewBag.OfficeMobile = officeAddr.Mobile;
                                ViewBag.OfficeContactName = officeAddr.ContactPerson;
                            }
                        }
                    }

                    else
                    {
                    }





                    //for Specific  terms and conditions
                    if (id != null)
                    {
                        var Specific_T_and_C = (from SpecificT_C in objmms.tblDraftSpecificTermsAndConditions_Child

                                                where SpecificT_C.Purchase_Order_No == id && SpecificT_C.Status == "Enable" && SpecificT_C.IsActive == "Yes"
                                                select new Specific_Termss_And_conditions_Chld
                                                {
                                                    STC_Description = SpecificT_C.STC_Description
                                                }).ToList();
                        var data_stc = Specific_T_and_C;
                        ViewBag.Stc_details = data_stc;
                    }
                    else { }
                    #endregion


                    return PartialView("PartialView_PrintDraftPO", dataObj);


                }

                else {


                    return Json(null, JsonRequestBehavior.AllowGet);
                }




            }
            else {



                return Json(null, JsonRequestBehavior.AllowGet);
            }






        }

    }
}