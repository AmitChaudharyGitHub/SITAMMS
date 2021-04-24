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


namespace MMS.Controllers
{
    public class Price_Cap_For_Item_GroupController : Controller
    {
        // GET: Price_Cap_For_Item_Group
        private MMSEntities objmms = new MMSEntities();
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();
            return View();
        }

        public ActionResult View_IGData()
        {
            var a = (from pd in objmms.tblItemGroupMasters.Where(k => k.GroupName != null).OrderBy(c => c.TransID)
                     select new IG_rate_Caption
                     {
                         ItemGroupID=pd.ItemGroupID,
                         GroupName=pd.GroupName                       

                     }).ToList();


            var totalRows = a.Count();

            var data = new IG_rate_Caption_WebGrids()
            {
                TotalRows = totalRows,
                PageSize = 250,
                IG_DATAS = a.ToList()
            };


            if (data != null && data.TotalRows != 0)
            {



                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PartialView_forEnable_IG", data);
                }

                else
                {
                    return PartialView("_PartialView_forEnable_IG", data);
                }

            }

            else
            {
                return View("_EmptyView");
            }

        }


        //[HttpPost]
        //public ActionResult ADD_ItemGroup(ns_Price_Cap_Enable_Item_Group IG_Rate_tbl)
        //{
        //    if (Session["EmpID"] == null)
        //    {
        //        return RedirectToAction("Login", "MyAccount");
        //    }

        //   string empId = Session["EmpID"].ToString();
        //    //here code for data Save in TbIndentPurchaseOrder_Approval_For_Print Table for admin Approval
        //    List<ns_Price_Cap_Enable_Item_Group> list = new List<ns_Price_Cap_Enable_Item_Group>();
        //    ns_Price_Cap_Enable_Item_Group tbl = new ns_Price_Cap_Enable_Item_Group();
        //    tbl.ProjectId = Session["ProjectssId"].ToString();
            
        //    tbl.ItemGroupId = IG_Rate_tbl.ItemGroupId;
            
        //    tbl.ItemGroupName = IG_Rate_tbl.ItemGroupName;
        //    tbl.CreatedBy = empId;
        //    tbl.CreatedDate = DateTime.Now;
        //    tbl.Status_Checked = "Yes";                    
        //    tbl.CompanyId = "COMP000001";
        //    list.Add(tbl);
        //    objmms.ns_Price_Cap_Enable_Item_Group.AddRange(list);
        //    objmms.SaveChanges();

        //    //if (ModelState.IsValid)
        //    //{
        //    //    tblGenral_Terms_Conditions emp = objmms.tblGenral_Terms_Conditions.Single(em => em.Id == gtc_tbl.Id);
        //    //    emp.Header_Title = gtc_tbl.Header_Title;
        //    //    emp.GTC_Description = gtc_tbl.GTC_Description;
        //    //    emp.CompanyId = "COMP000001";
        //    //    objmms.tblGenral_Terms_Conditions.Add(gtc_tbl);
        //    //    //objmms.Entry(emp).State = EntityState.Modified;
        //    //    objmms.SaveChanges();                           
        //    //}
        //    return View("Index");
        //}

        public ActionResult ADD_ItemGroups_Name(Add_Caption_Rate_Item_group Grid)
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            string empId = Session["EmpID"].ToString();

            try
            {
                              
                if (Grid.Child2 != null && empId !=null)
                    foreach (var x in Grid.Child2)
                    {

                        x.ProjectId = Session["ProjectssId"].ToString();

                        x.CreatedBy = empId;
                        x.CreatedDate = DateTime.Now;
                        x.Status_Checked = "Yes";
                        x.CompanyId = "COMP000001";

                        //x.MUId = a;
                        //x.Receive = 0;
                        //m.GetTbIndentPurchaseOrderChildManager().Add(x);
                        objmms.ns_Price_Cap_Enable_Item_Group.Add(x);
                        // objmms.SaveChanges();

                    }                                     

                objmms.SaveChanges();

                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                return Json(new { Status = "2", IndentNo = "" }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}