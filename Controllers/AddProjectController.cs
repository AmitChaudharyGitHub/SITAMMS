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
    public class AddProjectController : Controller
    {
        // GET: AddProject
        FactoryManager m = new FactoryManager();
        private MMSEntities objmms = new MMSEntities();
       MMS.Models.MSP_Model objmsp = new MMS.Models.MSP_Model();
        string EmpID = null;


        public AddProjectController()
        {
            try
            {
                EmpID = System.Web.HttpContext.Current.Session["EmpID"].ToString();
            }
            catch
            {
            }
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BindRegion()
        {
            string j = string.Empty;
            var reglst = objmms.tblRegions.ToList().Select(x => new { Text = x.RegionName, Value = x.RegionID }).ToList();
            j = reglst.ToJSON();
            return Json(j, JsonRequestBehavior.AllowGet);
        }




        public ActionResult ProjectGrid()
        {
            List<ProjectGrid> obj = new List<ViewModels.ProjectGrid>();
            string CompId = Session["CompanyId"].ToString();
            var Result = objmsp.GetAllProjectDetail(CompId);
            var data = (from a in Result
                        select new ProjectGrid
                        {
                            SNo =a.SNo,
                            NameOfProject=a.NameOfProject,
                            Location = a.Location,
                            DateOfAward = a.DateOfAward,
                            NameOfRegion = a.NameOfRegion,
                            ProjectInchargeName = a.ProjectInchargeName,
                            StipulatedDateOfStart = a.StipulatedDateOfStart,
                            ActualDateOfStart = a.ActualDateOfStart,
                            StipulatedDateOfFinish = a.StipulatedDateOfFinish,
                            OriginalContractValue = a.OriginalContractValue
                        }).ToList();


            return PartialView("_ProjectGrid", data);
        }


        #region

        public ActionResult Create()
        {
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            ViewBag.EmpID = EmpID;
           return View();
        }

        public ActionResult SaveProject(ProjectViewModel PV)
        {
            string CompId = Session["CompanyId"].ToString();
            string Getproj = objmsp.GetProjectNumberAdmin(CompId);

            if (Getproj != null)
            {
                try
                {



                    tblProjectMaster proj = new tblProjectMaster();
                    tblProjectParticularsDetailA projA = new tblProjectParticularsDetailA();
                    tblProjectCode projcd = new tblProjectCode();
                    tblProjectTINNo prjtin = new tblProjectTINNo();
                    tblProjectGSTNo prjGstn = new tblProjectGSTNo();
                    var checkName = objmms.tblProjectMasters.Where(x => x.ProjectName == PV.ProjectName.ToString().Trim()).ToList();
                    if (checkName != null)
                    {
                        projA.CompanyID = CompId.ToString();
                        projA.PRJID = objmsp.GetProjectNumberAdmin(CompId);
                        projA.NameOfProject = PV.ProjectName.ToString().Trim();
                        projA.Location = PV.Location;
                        projA.DateOfAward = PV.DateOfAward;
                        projA.LOINo = PV.LOiNO;
                        projA.LOIDate = PV.LOIDate;
                        projA.NameOfRegion = PV.Region;
                        projA.ProjectInchargeName = PV.ProjectInCharge;
                        projA.ProjectInchargeMobileNo = PV.MobileNumber;
                        projA.StipulatedDateOfStart = PV.StipulatedDateOfStart;
                        projA.ActualDateOfStart = PV.DateofActualStart;
                        projA.StipulatedDateOfFinish = PV.StipulatedDateOfFinish;
                        projA.OriginalContractValue = PV.OrignalContractValue;
                        projA.Status = "E";
                        projA.CreatedBy = EmpID;
                        projA.CreatedDate = System.DateTime.Now;
                        projA.ModifiedBy = EmpID;
                        projA.ModifiedDate = System.DateTime.Now;

                        proj.CompanyID = CompId.ToString();
                        proj.PRJID = objmsp.GetProjectNumberAdmin(CompId);
                        proj.ProjectName = PV.ProjectName;
                        proj.StartDate = PV.DateofActualStart;
                        proj.EndDate = PV.StipulatedDateOfFinish;
                        proj.Status = "E";
                        proj.CreatedBy = EmpID;
                        proj.CreatedDate = System.DateTime.Now;
                        proj.FinalStatus = "E";

                        projcd.CompanyID = CompId.ToString();
                        projcd.ProjectID = objmsp.GetProjectNumberAdmin(CompId);
                        projcd.RegionID = PV.RegionId;
                        projcd.Status = "E";
                        projcd.ProjectCode = PV.ProjectCode;
                        projcd.CreatedBy = EmpID;
                        projcd.CreatedDate = System.DateTime.Now;

                        prjtin.ProjectId = objmsp.GetProjectNumberAdmin(CompId);
                        prjtin.StateId = PV.StateId;
                        prjtin.TINNo = PV.TINNo;

                        prjGstn.ProjectId = objmsp.GetProjectNumberAdmin(CompId);
                        prjGstn.StateId = PV.StateId;
                        prjGstn.GSTNo = PV.GSTNo;



                        objmms.tblProjectParticularsDetailAs.Add(projA);
                        objmms.tblProjectMasters.Add(proj);
                        objmms.tblProjectCodes.Add(projcd);
                        objmms.tblProjectTINNoes.Add(prjtin);
                        objmms.tblProjectGSTNoes.Add(prjGstn);
                        objmms.SaveChanges();
                        return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet); // save successfully.

                    }
                    else
                    {
                        return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet); // Name already exist.
                    }

                }

                catch (Exception ex)
                { ex.ToString(); return Json(new { Status = "4" }, JsonRequestBehavior.AllowGet); } // problem in saving record 




            }
            else
            {
                return Json(new { Status = "3" }, JsonRequestBehavior.AllowGet); // server problem.

            }


        }





        public ActionResult UserList()
        {
            //Show UserList
            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            ViewBag.EmpID = EmpID;
            
            return View();
        }


        public ActionResult UserGrid()
        {
            List<UserGrid> obj = new List<ViewModels.UserGrid>();
            string CompId = Session["CompanyId"].ToString();
            var Result = objmsp.GetAllUserDetail(CompId);
            var data = (from a in Result
                        select new UserGrid
                        {
                           SNo =a.SNo,
                           ProjectName = a.ProjectName,
                           EmployeeName = a.EmployeeName,
                           CnName = a.CnName,
                           StateName = a.StateName,
                           CityName = a.CityName,
                           MobileNo = a.MobileNo,
                           Email = a.Email,
                           ReportingToName = a.ReportingToName,
                           DepartmentName = a.DepartmentName,
                           Designation = a.Designation,
                           DOJ = a.DOJ,
                           OfficialEmpID = a.OfficialEmpID 
                        }).ToList();


            return PartialView("_UserGrid", data);
        }



        public JsonResult GetUserType()
        {
             var Utype = objmms.tblAccountTypes.Where(p=>p.ID !=1 && p.ID !=4).ToList().Select(x => new { Text = x.AccountType, Value = x.ID }).OrderBy(k => k.Text).ToList(); 
             return Json(Utype, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetUserDept()
        {
            var Dept = objmms.tblDepartments.Where(p=>p.Status =="E").ToList().Select(x => new { Text = x.DepartmentName, Value = x.DeptID }).OrderBy(k => k.Text).ToList();
            return Json(Dept,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserDesig(string DeptId)
        {
            var Design = objmms.tblDesignations.Where(p => p.DeptID == DeptId && p.Status == "E").ToList().Select(x => new { Text = x.Designation, Value = x.DesgID }).OrderBy(k => k.Text).ToList();
            return Json(Design, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportingTo()
        {
            var ReptTo = objmms.tblEmployeeMasters.Where(p => p.Status == "E" && p.AccountType != "0").ToList().Select(x => new { Text = x.FName + " " + x.LName, Value = x.EmpID }).OrderBy(k => k.Text).ToList();
            return Json(ReptTo, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCountry()
        {
            var Cotry = objmms.tblCountries.Where(p=>p.CnStatus ==true).ToList().Select(x => new { Text = x.CnName, Value = x.CountryID }).OrderBy(p => p.Text).ToList();
            return Json(Cotry, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetState(string CountryID)
        {
            var Statelist = objmms.tblStates.Where(p => p.CountryID == CountryID && p.stStatus == true).ToList().Select(x => new { Text = x.StateName, Value = x.StateID }).OrderBy(k => k.Text).ToList();
            return Json(Statelist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCity(string StateId)
        {
            var CityLst = objmms.tblCityLists.Where(p => p.StateID == StateId && p.ctStatus == true).ToList().Select(x => new { Text = x.CityName, Value = x.CityID }).OrderBy(k => k.Text).ToList();
            return Json(CityLst, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult AddUser()
        {

            if (Session["EmpID"] == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }

            ViewBag.EmpID = EmpID;
            ViewBag.TodayDate = System.DateTime.Now;

            List<SelectListItem> objEmployeeType = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Select Employee Type", Value = "-1" },
                new SelectListItem { Text = "Permanent", Value = "0" },
                new SelectListItem { Text = "Contractual", Value = "1" },
                new SelectListItem { Text = "Retainership", Value = "2" }
               
            };
            ViewBag.EmployeeType = objEmployeeType;

            List<SelectListItem> objType = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Select Type", Value = "-1" },
                new SelectListItem { Text = "PIC", Value = "1" },
                new SelectListItem { Text = "Mang", Value = "2" },
               new SelectListItem { Text = "PMC", Value = "3" },
               new SelectListItem { Text = "Purchase", Value = "4" },

            };
            ViewBag.TypeObj = objType;


            // Add User Here
            //< asp:ListItem Selected = "True" Text = "Permanent" Value = "0" ></ asp:ListItem >

            //                                     < asp:ListItem Text = "Retainership" Value = "2" ></ asp:ListItem >

            //                                            < asp:ListItem Text = "Contractual" Value = "1" ></ asp:ListItem >


            //if (Session["AID"].ToString() == "1")
            //{
            //    ddlUserType.Items.Clear();
            //    ddlUserType.Items.Add("User");
            //    ddlUserType.Items.Add("SubAdmin");
            //}
            //if (Session["AID"].ToString() == "3")
            //{
            //    ddlUserType.Items.Clear();
            //    ddlUserType.Items.Add("User");
            //}

            ViewBag.PRJID = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName");
            ViewBag.PurType = new SelectList(objmms.tblPI_PurchaseType.ToList(), "TrandId", "PurchaseType");




            return View();
        }


        public ActionResult SaveUser(UserViewModel UV)
        {
            string CompId = Session["CompanyId"].ToString();
            string GetMaxEmpId = objmsp.GetEmployeeNumber(CompId);
            string PRJID = String.Empty;string PrType = String.Empty;
            if (GetMaxEmpId != null)
            {
                tblEmployeeMaster obj = new tblEmployeeMaster(); tblApproval_AccountType obj1 = new tblApproval_AccountType();
                var checkUserName = objmms.tblEmployeeMasters.Where(x => x.FName == UV.FName && x.LName == UV.LName && x.Status == "E").ToList();
                if (checkUserName != null)
                {
                    string[] ss = UV.PRID; string[] pp = UV.PurType;
                    foreach (string value in ss)
                    {
                        PRJID += value;
                        PRJID += ",";
                    }

                    foreach (string va in pp)
                    {
                        PrType += va;
                        PrType += ",";
                    }
                    obj.ProjectID = PRJID.TrimEnd(',');
                    obj.PurchaseType = PrType.TrimEnd(',');

                    obj.CompanyID = CompId;
                  //  obj.ProjectID = UV.ProjectID;
                    obj.EmpID = objmsp.GetEmployeeNumber(CompId);
                    obj.FName = UV.FName;
                    obj.LName = UV.LName;
                    obj.Address = UV.Address;
                    obj.Country = UV.Country;
                    obj.State = UV.State;
                    obj.City = UV.City;
                    obj.ZipCode = UV.ZipCode;
                    obj.MobileNo = UV.MobileNo;
                    obj.Email = UV.Email;
                    obj.Password =  objmsp.CreateMD5Hash(UV.Password);
                    obj.AccountType = UV.AccountType;
                    obj.DateOfBirth = UV.DateOfBirth;
                    obj.AltEmail = UV.AltEmail;
                    obj.ReportingTo = UV.ReportingTo;
                    obj.CreatedBy = EmpID;
                    obj.CreatedDate = System.DateTime.Now;
                    obj.Status = "E";
                    obj.DeptID = UV.DeptID;
                    obj.DesgID = UV.DesgID;
                    obj.DOJ = UV.DOJ;
                    obj.OfficialEmpID = UV.OfficialEmpID;
                    obj.EmpType = UV.EmpType;
                    objmms.tblEmployeeMasters.Add(obj);
                    if (UV.TypeName != "Select Type")
                    {
                        obj1.AccountType = Convert.ToInt32(UV.AccountType);
                        obj1.TypeName = UV.TypeName;
                       // obj1.ProjectId = UV.ProjectID;
                        obj1.DepartmentId = UV.DeptID;
                        obj1.CreatedDate = System.DateTime.Now;
                        obj1.CreatedBy = EmpID;
                        obj1.EmployeeId = obj.EmpID;
                        objmms.tblApproval_AccountType.Add(obj1);
                    }
                    

                    
                  objmms.SaveChanges();
                    return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);

                }
                else {
                    return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);  // user name is already exist.
                }
            }
            else {
                return Json(new { Status = "3" }, JsonRequestBehavior.AllowGet); //admin problem.

            }

            
        }

    }
}