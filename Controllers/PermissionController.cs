using DataAccessLayer;
using MMS.DAL;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class PermissionController : Controller
    {
        // GET: Permission
        FactoryManager m = new FactoryManager();
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


        public JsonResult GetProjectByEmpId(string E = "0")
        {

            string j = null;
            if (E != "0" && E != "EMP0000001" && E != "")
            {
                string PrjString = m.GetEmployeeMasterManager().FindPrj(E);


                var Prjlist = m.GetProjectMasterManager().FindListByString(PrjString);

                j = Prjlist.ToJSON();
            }
            else
            {
                var Prjlist = m.GetProjectMasterManager().FindListByString(null);

                j = Prjlist.ToJSON();
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserProjectWise(string PrID)
        {
            // var L = objmms.tblEmployeeMasters.Where(x => x.ProjectID == PrID).ToList().Select(p => new { Text = p.FName, Value = p.EmpID }).OrderBy(k => k.Text).ToList();
            var L = objmms.tblEmployeeMasters.Where(x => x.ProjectID.Contains(PrID)).ToList().Select(p => new { Text = p.FName, Value = p.EmpID }).OrderBy(k => k.Text).ToList();
            return Json(L, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PermissionGrid()
        {
            List<PermissionViewModel> pList = new List<PermissionViewModel>();
            var o = objmms.tblMainHeaders.OrderBy(a => a.TransId);
            foreach(var i in  o)
                {

                var od = objmms.tblSubHeaders.Where(x => x.MainHeaderId == i.TransId).OrderBy(p=>p.SubMenuOrder).ToList();
                pList.Add(new PermissionViewModel { Main =i , Sub = od });
            }
            return PartialView("_PermissionGrid",pList);
        }



        public ActionResult InsertGrid(PermissionGrid Grid)
        {
            List<tblMMSUserPermission> list = new List<tblMMSUserPermission>();
            
            if (Grid.SubGrid != null)
            {

               
                foreach (var x in Grid.SubGrid)
                {
                    // check if user name is exist or not
                    var ex = objmms.tblMMSUserPermissions.Where(y => y.UserId == x.UserId && y.SubHeaderId==x.SubMenuId).FirstOrDefault();
                    if (ex == null)
                    {
                        tblMMSUserPermission obj = new tblMMSUserPermission();
                        obj.CompanyId = Session["CompanyId"].ToString();
                        obj.UserId = x.UserId;
                        // obj.ProjectId = x.ProjectId;
                        obj.SubHeaderId = x.SubMenuId;
                        obj.MainHeaderId = objmms.tblSubHeaders.Where(p => p.STransId == x.SubMenuId).Select(k => k.MainHeaderId).First();
                        obj.MainHeaderStatus = objmms.tblMainHeaders.Where(o => o.TransId == obj.MainHeaderId).Select(l => l.HeaderStatus).First();
                        obj.SubHeaderStatus = objmms.tblSubHeaders.Where(p => p.STransId == x.SubMenuId).Select(r => r.SubHeaderStatus).First();
                        obj.PageStatus = Convert.ToBoolean(1);
                        obj.CreatedDate = System.DateTime.Now;

                        list.Add(obj);

                    }
                    //else {
                    //    break;
                    //}


                    
                }

            }
            if (list.Count() > 0) {
                objmms.tblMMSUserPermissions.AddRange(list);    objmms.SaveChanges();
                return Json("1", JsonRequestBehavior.AllowGet);
            } else { return Json("2", JsonRequestBehavior.AllowGet); }
            
        }

    }
}