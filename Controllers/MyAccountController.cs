using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MMS.Models;
using BusinessObjects.Entity;
using System.Text;
using System.Security.Cryptography;
using MMS.ViewModels;

namespace MMS.Controllers
{
    public class MyAccountController : MySiteController
    {
        
        public ActionResult Login()
        {
            return View();
        }


        public string CreateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(tblEmployeeMaster l, string ReturnUrl = "")
        {
            using (Model1 dc = new Model1())
            {               
                    string Pwd = CreateMD5Hash(l.Password);             
                    var user = dc.tblEmployeeMasters.Where(a => a.Email.Equals(l.Email) && a.Password.Equals(Pwd)).FirstOrDefault();
                    
                    if (user != null)
                    {
                        //var newUser = dc.tblLogInformations.Create();
                        var newUser = objmms.tblLogInformations.Create();
                        newUser.CompanyID = user.CompanyID;
                        newUser.PRJID = user.ProjectID;
                        newUser.UserId = user.EmpID;
                        newUser.LogInTime = DateTime.Now;
                        newUser.LogOutTime = DateTime.Now;
                        newUser.Status = "D";
                        newUser.LogActivity = DateTime.Now;

                    var dept = (from a in objmms.tblEmployeeMasters
                                join b in objmms.tblDepartments on a.DeptID equals b.DeptID
                                where a.EmpID == user.EmpID select b).FirstOrDefault();

                        // dc.tblLogInformations.Add(newUser);
                    objmms.tblLogInformations.Add(newUser); objmms.SaveChanges();
                    var data = Procedure.GetData<VMDropDown>("GetUserDivsions", user.EmpID);
                    //  dc.SaveChanges();
                    FormsAuthentication.SetAuthCookie(user.Email, false);
                        Session["EmpID"] = user.EmpID;
                        Session["ProjectssId"] = user.ProjectID;  
                        ViewData["pjtId"] = newUser.PRJID;
                        Session["Emp_Name"] = user.FName;
                        Session["ProId"] = newUser.PRJID;
                    Session["EmailCheck"] = user.Email.ToString();
                    Session["CompanyId"] = user.CompanyID;
                    Session["AccountType"] = user.AccountType;
                    Session["Dept"] = dept.DepartmentName;

                    if (data.FirstOrDefault()!=null)
                    {
                        var divisionId =Convert.ToInt32(data.FirstOrDefault().Value);
                        var colour=objmms.tblDivisionMasters.SingleOrDefault(x => x.Id == divisionId).ColourCode;
                        Session["Division"] = divisionId;
                        Session["DivisionName"] = data.FirstOrDefault().Text;
                        Session["Colour"] = colour;
                    }
                    else
                    {
                        Session["Division"] = "";
                        Session["DivisionName"] = "";
                        Session["Colour"] = "white";
                    }
                    
                    List<MMS.ViewModels.AllowPermissionViewModel> kk = new List<ViewModels.AllowPermissionViewModel>();

                    MMS.ViewModels.AllowPermissionViewModel obj = new ViewModels.AllowPermissionViewModel();
                    // obj.Master = new List<ViewModels.HData>
                    var lp = ((objmms.tblMMSUserPermissions.ToList()).Select(x => x.ProjectId).Intersect((objmms.tblEmployeeMasters.ToList()).Select(p=>p.ProjectID)).Any());


                    //var xData = (from a in objmms.tblMMSUserPermissions.Where(p =>user.ProjectID.Contains(p.ProjectId) && p.MainHeaderStatus == true).Select(k => k.MainHeaderId).Distinct().ToList()
                    //             join b in objmms.tblMainHeaders on a.Value equals b.TransId
                    //             select new ViewModels.AllowPermissionViewModel
                    //             {
                    //                 TransId = a.Value,
                    //                 HName = b.HeaderName,
                    //                 P_Child = (from t in objmms.tblMMSUserPermissions.Where(x => user.ProjectID.Contains(x.ProjectId) && x.UserId == user.EmpID)
                    //                            join
                    //                            r in objmms.tblMainHeaders on t.MainHeaderId equals r.TransId
                    //                            join c in objmms.tblSubHeaders on t.SubHeaderId equals c.STransId where t.MainHeaderId == a.Value && user.ProjectID.Contains(t.ProjectId)
                    //                            select new ViewModels.Gdata
                    //                            {
                    //                                Id = t.TransId,
                    //                                HeaderName = r.HeaderName,
                    //                                HeaderStatus = t.MainHeaderStatus,
                    //                                SubHeaderName = c.SubHeaderName,
                    //                                SubHeaderStatus = t.SubHeaderStatus,
                    //                                PageStatus = t.PageStatus,
                    //                                MenuHeaderRank = c.SubMenuOrder
                    //                            }



                    //                            ).OrderBy(o=>o.MenuHeaderRank).ToList()
                    //             }).ToList();

                    var xData = (from a in objmms.tblMMSUserPermissions.Where(p => p.MainHeaderStatus == true).Select(k => k.MainHeaderId).Distinct().OrderBy(j=>j.Value).ToList()
                                  join b in objmms.tblMainHeaders on a.Value equals b.TransId
                                  select new ViewModels.AllowPermissionViewModel
                                  {
                                      TransId = a.Value,
                                      HName = b.HeaderName,
                                      P_Child = (from t in objmms.tblMMSUserPermissions.Where(x => x.UserId == user.EmpID)
                                                 join
                                                 r in objmms.tblMainHeaders on t.MainHeaderId equals r.TransId
                                                 join c in objmms.tblSubHeaders on t.SubHeaderId equals c.STransId
                                                 where t.MainHeaderId == a.Value
                                                 select new ViewModels.Gdata
                                                 {
                                                     Id = t.TransId,
                                                     HeaderName = r.HeaderName,
                                                     HeaderStatus = t.MainHeaderStatus,
                                                     SubHeaderName = c.SubHeaderName,
                                                     SubHeaderStatus = t.SubHeaderStatus,
                                                     PageStatus = t.PageStatus,
                                                     MenuHeaderRank = c.SubMenuOrder
                                                 }
                                               ).OrderBy(o => o.MenuHeaderRank).ToList()
                                  }).ToList();
                                 













                    //var dData = (from a in objmms.tblMMSUserPermissions.Where(x => x.ProjectId == user.ProjectID && x.UserId == user.EmpID)
                    //             join
                    //             b in objmms.tblMainHeaders on a.MainHeaderId equals b.TransId
                    //             join c in objmms.tblSubHeaders on a.SubHeaderId equals c.STransId
                    //             select new ViewModels.Gdata
                    //             {
                    //                Id = a.TransId,
                    //                 HeaderName = b.HeaderName,
                    //                 HeaderStatus =  a.MainHeaderStatus,
                    //                 SubHeaderName = c.SubHeaderName,
                    //                 SubHeaderStatus = a.SubHeaderStatus,
                    //                 PageStatus = a.PageStatus
                    //            }).ToList();

                    //var DD = (from  b in objmms.tblMainHeaders  where b.HeaderStatus == true
                    //          select new ViewModels.HData
                    //          {
                    //              TransId = b.TransId,
                    //              HName =b.HeaderName

                    //          }).ToList();


                    //obj.Master = DD.ToList();
                    //obj.P_Child = dData.ToList();

                  //  kk.Add(obj);

                    Session["PermissionData"] = xData.ToList();


                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else if (user.Email == "admin@sitanet.in")
                    {
                        
                        return RedirectToAction("Index", "AdminHome");
                    }
                    else
                    {
                        return RedirectToAction("MyDashBoard", "Home");
                    }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login credentials.");
                    }           
               
            }
            ModelState.Remove("Password");
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(VMChangePassword Model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var oldPasswordHash = CreateMD5Hash(Model.OldPassword);
            string email = Session["EmailCheck"].ToString();
            var data = objmms.tblEmployeeMasters.SingleOrDefault(x => x.Password == oldPasswordHash && x.Email == email);
            if (data != null)
            {
                var NewdPasswordHash = CreateMD5Hash(Model.NewPassword);
                data.Password = NewdPasswordHash;
                objmms.Entry(data).State = System.Data.Entity.EntityState.Modified;
                objmms.SaveChanges();
                ViewBag.Done = true;
                ViewBag.Msg = "Password Change Sussesfully !";
            }
            else
            {
                ViewBag.Msg = "Incorrect Password !";
            }
            return View();
        }


        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "MyAccount");
        }
    }
}