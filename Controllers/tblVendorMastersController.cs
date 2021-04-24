using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Entity;
using BusinessLogicLayer;
using MMS_P.ViewModels;
using DataAccessLayer;
using Newtonsoft.Json;
using MMS.ViewModels;
using MMS.Models;
using System.Web.Helpers;

namespace MMS.Controllers
{
    public class tblVendorMastersController : Controller
    {

        FactoryManager m = new FactoryManager();

        static private List<tblRegistrationType> VendorGroupList;
        string Id = "0";
        string EmpID = null;
        private tblRegistrationType VendorGroup;
        Procedure procedure = new Procedure();
        public tblVendorMastersController()
        {
            try
            {
                EmpID = System.Web.HttpContext.Current.Session["EmpID"].ToString();
            }
            catch
            {
            }

            //if (VendorGroupList == null)
            //    VendorGroupList = m.GetVendorGroupManager().FindAll();
            //if (VendorGroup == null)
            //    VendorGroup = VendorGroupList.FirstOrDefault();
        }
        private void FIllBySession()
        {
            if (System.Web.HttpContext.Current.Session["Id"] != null)
                Id = System.Web.HttpContext.Current.Session["Id"].ToString();

            VendorGroup = VendorGroupList.Where(a => a.TypeID == Id).FirstOrDefault();

        }
        private void FIllByid(int? id)
        {


            VendorGroup = VendorGroupList.Where(a => a.TypeID == Id).FirstOrDefault();

        }

        // GET: tblVendorMasters
        //[Authorize]
        public ActionResult Index()

        {
            ViewBag.VGId = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", Id);
            ViewBag.Projects = new SelectList(m.GetProjectMasterManager().FindAll().Where(x => !x.ProjectName.Contains("Admin") && !x.ProjectName.Contains("DEMO")), "PRJID", "ProjectName");
            ViewBag.States = new SelectList(GetState("FD49C22E-4C33-4DEA-BC9E-0EDBD7F191E4"), "StateID", "StateName");
            //ViewBag.BlackListStatus = new SelectList(new MSP_Model().GetStatus(), "Status", "Status");
            return View();
        }

        public ActionResult Grid1(string projectId, string vendorName, string vendorGroup, string state, string city, string blackListStatus)
        {

            var searchData = procedure.SearchVendors(projectId, vendorName, vendorGroup, state, city, blackListStatus);

            var data = searchData.ToList();
            TempData["searchData"] = data;

            return View("_GridView", data);

        }

        //public JsonResult GetReagions()
        //{
        //    var data = new MMS.DAL.MMSEntities().tblRegions.Select(x => new { Text = x.RegionName, Value = x.TransID }).OrderBy(x => x.Text).ToList();
        //    return Json(data.ToJSON(), JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Grid(string VGId = "0", int page = 1, string sort = "Name", string sortDir = "ASC")
        {
            const int pageSize = 15000;
            sortDir = sortDir.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? sortDir : "asc";
            var validColumns = new[] { "VendorId", "Name", "City" };
            if (!validColumns.Any(c => c.Equals(sort, StringComparison.CurrentCultureIgnoreCase)))
                sort = "VendorId";

            if (VGId != "0")
            {
                Id = VGId;
                this.Session["Id"] = Id.ToString();
            }
            else
            {
                try { FIllBySession(); }
                catch { }
            }
            if (Id == "0")
            {
                //Id = "VID0000001";
                this.Session["Id"] = Id;
            }

            if (Id != "0")
            {
                //VendorGroup = VendorGroupList.Where(a => a.TypeID == Id).FirstOrDefault();
                VendorGroup = m.GetVendorGroupManager().Find(Id);
                if (VendorGroup != null)
                {
                    var totalRows = m.GetVendorMasterManager().CountByVendorGroup(VendorGroup.TransID);
                    var VendorMasters = m.GetVendorMasterManager().FindPageByVendorGroup(VendorGroup.TransID, page, pageSize);
                   // var ven = m.GetVendorMasterManager().FindPageByVendorGroupNew(VendorGroup.TypeID, page, pageSize);
                    //var VendorMasters = m.GetVendorMasterManager().FindPageByVendorGroup(VendorGroup.TypeID, page, pageSize);

                    var data = new PagedVendorMasterModel1()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        //  VendorMasterBll = VendorMasters.ToList()
                        VendorMasterBll = VendorMasters.ToList()
                    };
                    return View("_GridView", data);
                }
                else { return View("_EmptyView"); }
            }
            else { return View("_EmptyView"); }
        }

        // GET: tblVendorMasters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblVendorMaster vendorMaster = m.GetVendorMasterManager().Find((int)id);

            string[] projectIds = vendorMaster.PRJID.Split(',');

            List<string> projectNames = new List<string>();
            var ProjectManager = m.GetProjectMasterManager();
            for (int i = 0; i < projectIds.Length; i++)
            {
                if (projectIds[i] != "")
                    projectNames.Add(ProjectManager.Find(projectIds[i]).ProjectName);
            }
            //vendorMaster.PRJID = projectNames.ToString();
            ViewBag.ProjectNames = new SelectList(projectNames);


            string[] RegistrationType = null;
            if (vendorMaster.MulRegistrationType.Contains(','))
            {
                RegistrationType = vendorMaster.MulRegistrationType.Split(',');
            }
            else
            {
                RegistrationType = new string[1];
                RegistrationType[0] = vendorMaster.MulRegistrationType;
            }

            List<string> registrationName = new List<string>();
            DAL.MMSEntities objmms = new DAL.MMSEntities();

            for (int i = 0; i < RegistrationType.Length; i++)
            {
                string regId = RegistrationType[i];
                registrationName.Add(objmms.tblRegistrationTypes.FirstOrDefault(x => x.TypeID == regId).RegistrationType);
            }
            ViewBag.RegistrationName = new SelectList(registrationName);


            try
            {
                // just added viewbag for projid

                vendorMaster.Country = m.GetCountryMasterManager().FindCountryName(vendorMaster.Country);
                vendorMaster.State = m.GetCountryMasterManager().FindStateName(vendorMaster.State);
                vendorMaster.City = m.GetCountryMasterManager().FindCityName(vendorMaster.City);
            }
            catch { }

            if (vendorMaster == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
            {


                return PartialView(vendorMaster);
            }
            else
                return View(vendorMaster);
        }

        public JsonResult GetProjectsByRegions(TestVendorClass Model)
        {
            var selectedRegions = Model.Regions;
            var obj = new DAL.MMSEntities();
            var data = (from a in obj.tblProjectMasters
                        join b in obj.tblProjectCodes on a.PRJID equals b.ProjectID
                        where selectedRegions.Contains(b.RegionID)
                        orderby a.ProjectName
                        select new { Text = a.ProjectName, Value = a.PRJID }
                ).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: tblVendorMasters/Create
        public ActionResult Create()
        {

            ViewBag.Country = new SelectList(m.GetCountryMasterManager().FindAllCountry(), "CountryID", "CnName", "FD49C22E-4C33-4DEA-BC9E-0EDBD7F191E4");
            try
            {

                ViewBag.State = new SelectList(GetState("FD49C22E-4C33-4DEA-BC9E-0EDBD7F191E4"), "StateID", "StateName");

            }
            catch
            {

            }
            ViewBag.PRJID = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName");
            ViewBag.RegistrationType = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", Id);
            ViewBag.VendorTypeCode = new SelectList(m.GetVendorTypeManager().FindAll(), "TypeId", "VendorType");
            var data = new MMS.DAL.MMSEntities().tblRegions.Select(x => new { Text = x.RegionName, Value = x.RegionID }).OrderBy(x => x.Text).ToList();
            ViewBag.Regions = new SelectList(data, "Value", "Text");
                 
            if (Request.IsAjaxRequest())
            {
                ViewBag.IsUpdate = false;
                return PartialView();
            }
            else
                return View();
        }
        [HttpPost]
        public ActionResult GetStateByCountryId(string CountryId)
        {
            var S = GetState(CountryId);

            SelectList State = new SelectList(S, "StateID", "StateName");
            return Json(State);
        }
        private List<tblState> GetState(string CountryId)
        {

            try
            {
                if (CountryId != null)
                    return m.GetCountryMasterManager().FindAllState(CountryId);
                else
                    return null;
            }
            catch { return null; }
        }
        [HttpPost]
        public ActionResult GetCityByStateId(string StateId)
        {
            var C = GetCity(StateId);

            SelectList City = new SelectList(C, "CityID", "CityName");
            return Json(City);
        }
        private List<tblCityList> GetCity(string StateId)
        {
            try
            {
                if (StateId != null)
                {
                    return m.GetCountryMasterManager().FindAllCity(StateId);
                }
                else
                    return null;

            }
            catch { return null; }



        }
        // POST: tblVendorMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "VendorID,TransID,CompanyID,PRJID,Name,Address,Country,State,City,MobileNo,Email,VendorType,Status,CreatedBy,CreatedDate,VendorTypeCode,PanNo,TinNo,EccNo,Vat,RegistrationType,PinCode,SalesTaxNo,CSTNo,ServiceTaxNo,PhoneNo,BlackListStatus")] tblVendorMaster tblVendorMaster)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        tblVendorMaster.CreatedBy = EmpID;

        //        tblVendorMaster.CreatedDate = System.DateTime.Now;
        //        tblVendorMaster.VendorType = m.GetVendorTypeManager().Find(tblVendorMaster.RegistrationType).VendorType;
        //        //strat
        //        tblVendorMaster.VendorTypeCode = m.GetVendorTypeManager().Find(tblVendorMaster.RegistrationType).TypeID;
        //        tblVendorMaster.RegistrationType = tblVendorMaster.RegistrationType;
        //        //end
        //       // int i = 0;
        //         int i = m.GetVendorMasterManager().Add(tblVendorMaster);
        //        if (i != 0)
        //        {
        //            TempData["OperStatus"] = "Vendor added succeessfully in  Group";
        //            ModelState.Clear();
        //            if (Request.IsAjaxRequest())
        //            {
        //                return Json(new { success = true });
        //            }
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    ViewBag.Country = new SelectList(m.GetCountryMasterManager().FindAllCountry(), "CountryID", "CnName");
        //    ViewBag.RegistrationType = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", Id);
        //    ViewBag.PRJID = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName");
        //    ViewBag.VendorTypeCode = new SelectList(m.GetVendorTypeManager().FindAll(), "TypeId", "VendorType");

        //    if (Request.IsAjaxRequest())
        //    { return PartialView(tblVendorMaster); }
        //    else
        //    { return View(tblVendorMaster); }
        //}


        // for Testing
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(TestVendorClass VData)
        {
            tblVendorMaster vm = new tblVendorMaster();
            vm.CreatedDate = System.DateTime.Now;
            vm.CreatedBy = EmpID;
            // vm.VendorType = m.GetVendorTypeManager().Find(tblVendorMaster.RegistrationType).VendorType;
            string PRJID = String.Empty; string RegTYpeID = String.Empty;
            string[] ss = VData.PRID; string[] rty = VData.MulRegistrationType;
            foreach (string value in ss)
            {
                PRJID += value;
                PRJID += ",";
            }
            vm.PRJID = PRJID.TrimEnd(',');


            foreach (string value1 in rty)
            {
                RegTYpeID += value1;
                RegTYpeID += ",";
            }

            vm.MulRegistrationType = RegTYpeID.TrimEnd(',');

            vm.TinNo = VData.TinNo;
            vm.PanNo = VData.PanNo;
            vm.Name = VData.VendorName;
            vm.Address = VData.Address.Trim();
            vm.Country = VData.Country.Trim();
            vm.State = VData.State.Trim();
            vm.City = VData.City.Trim();
            vm.PinCode = VData.PinCode;
            vm.Status = VData.Status;
            vm.BlackListStatus = VData.BlackListStatus;
            vm.ContactPerson = VData.ContactPerson;
            vm.MobileNo = VData.MobileNo;
            vm.PhoneNo = VData.PhoneNo;
            vm.Email = VData.Email;
            vm.RegistrationType = "TID0000001";
            vm.CompanyID = Session["CompanyId"].ToString();
            vm.Aadhaar_No = VData.AadharNo;
            vm.VAT_No = VData.Vat_no;
            vm.GST_NO = VData.GST_no;

            int i = m.GetVendorMasterManager().Add(vm);
            if (i != 0)
            {
                // TempData["OperStatus"] = "Vendor added succeessfully in  Group";
                // ModelState.Clear();
                if (Request.IsAjaxRequest())
                {
                    // return Json(new { success = true });
                    // return Json("1", JsonRequestBehavior.AllowGet);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }

            ViewBag.Country = new SelectList(m.GetCountryMasterManager().FindAllCountry(), "CountryID", "CnName");
            ViewBag.RegistrationType = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", Id);
            ViewBag.PRJID = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName");
            ViewBag.VendorTypeCode = new SelectList(m.GetVendorTypeManager().FindAll(), "TypeId", "VendorType");

            // vm.VendorType = m.GetVendorTypeManager().Find(tblVendorMaster.RegistrationType).VendorType;

            // ImageCoordinates coordinates = JsonConvert.DeserializeObject<ImageCoordinates>(jsonCoordinates); 

            //  ViewData.Model = coordinates; List<Data> ListData
            return Json("1", JsonRequestBehavior.AllowGet);
        }


        public JsonResult IsVendorNameUnique(tblVendorMaster catalog)
        {

            return m.GetRepeatChequeManager().VendorNameRepeat(catalog.Name, catalog.RegistrationType, catalog.TransID)
                ? Json(false, JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }
        // GET: tblVendorMasters/Edit/5
        public ActionResult Edit(int? id)
        {



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblVendorMaster vendorMaster = m.GetVendorMasterManager().Find((int)id);
            if (vendorMaster == null)
            {
                return HttpNotFound();
            }
            try
            {
                string s = vendorMaster.PRJID; string r = vendorMaster.MulRegistrationType;
                string[] values = s.Split(','); string[] RegVal = r.Split(',');
                //for (int i = 0; i < values.Length; i++)
                //{
                //    values[i] = values[i].Trim();
                //    //ViewBag.PRJID = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName", values[i]);
                //}

                //for (int j = 0; j < RegVal.Length; j++)
                //{
                //    RegVal[j] = RegVal[j].Trim();
                //    //ViewBag.RegistrationType = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", RegVal[j]);
                //}

                ViewBag.RegistrationType = new MultiSelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", RegVal);
                //  ViewBag.PRJID = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName", vendorMaster.PRJID);
                ViewBag.PRJID = new MultiSelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName", values);
                ViewBag.Country = new SelectList(m.GetCountryMasterManager().FindAllCountry(), "CountryID", "CnName", vendorMaster.Country);
                ViewBag.VendorTypeCode = new SelectList(m.GetVendorTypeManager().FindAll(), "TypeId", "VendorType", vendorMaster.VendorTypeCode);


            }
            catch
            { }


            try
            {

                ViewBag.State = new SelectList(m.GetCountryMasterManager().FindAllState(vendorMaster.Country), "StateID", "StateName", vendorMaster.State);

            }
            catch
            {

            }
            try
            {
                ViewBag.City = new SelectList(m.GetCountryMasterManager().FindAllCity(vendorMaster.State), "CityID", "CityName", vendorMaster.City);


            }
            catch
            {

            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(vendorMaster);
            }
            return View(vendorMaster);
        }

        [HttpPost]
        public ActionResult Edit(TestVendorClass VData)
        {
            tblVendorMaster vm = new tblVendorMaster();

            string PRJID = String.Empty; string RegTYpeID = String.Empty;
            string[] ss = VData.PRID; string[] rty = VData.MulRegistrationType;
            foreach (string value in ss)
            {
                PRJID += value;
                PRJID += ",";
            }
            vm.PRJID = PRJID.TrimEnd(',');


            foreach (string value1 in rty)
            {
                RegTYpeID += value1;
                RegTYpeID += ",";
            }

            vm.MulRegistrationType = RegTYpeID.TrimEnd(',');

            vm.TinNo = VData.TinNo;
            vm.PanNo = VData.PanNo;
            vm.Name = VData.VendorName;
            vm.Address = VData.Address.Trim();
            vm.Country = VData.Country.Trim();
            vm.State = VData.State.Trim();
            vm.City = VData.City.Trim();
            vm.PinCode = VData.PinCode;
            vm.Status = VData.Status;
            vm.BlackListStatus = VData.BlackListStatus;
            vm.ContactPerson = VData.ContactPerson;
            vm.MobileNo = VData.MobileNo;
            vm.PhoneNo = VData.PhoneNo;
            vm.Email = VData.Email;
            vm.RegistrationType = "TID0000001";
            vm.VendorID = VData.VendorID;
            vm.Aadhaar_No = VData.AadharNo;
            vm.VAT_No = VData.Vat_no;
            vm.GST_NO = VData.GST_no;

            m.GetVendorMasterManager().Update(vm);

            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true });
            }
            //  return RedirectToAction("Index");
            try
            {
                ViewBag.RegistrationType = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", Id);
                ViewBag.PRJID = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName", vm.PRJID);
                ViewBag.Country = new SelectList(m.GetCountryMasterManager().FindAllCountry(), "CountryID", "CnName", vm.Country);
                // ViewBag.VendorTypeCode = new SelectList(m.GetVendorTypeManager().FindAll(), "TypeId", "VendorType", tblVendorMaster.VendorTypeCode);
            }
            catch{ }
            try
            {
                ViewBag.State = new SelectList(m.GetCountryMasterManager().FindAllState(vm.Country), "StateID", "StateName", vm.State);
            }
            catch{}

            try
            {
                ViewBag.City = new SelectList(m.GetCountryMasterManager().FindAllCity(vm.State), "CityID", "CityName", vm.City);


            }
            catch(Exception ex){}
            if (Request.IsAjaxRequest())
            { return PartialView(vm); }
            return View(vm);
        }







        // POST: tblVendorMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "VendorID,TransID,CompanyID,PRJID,Name,Address,Country,State,City,MobileNo,Email,VendorType,Status,CreatedBy,CreatedDate,VendorTypeCode,PanNo,TinNo,EccNo,Vat,RegistrationType,PinCode,SalesTaxNo,CSTNo,ServiceTaxNo,PhoneNo,BlackListStatus")] tblVendorMaster tblVendorMaster)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        tblVendorMaster.VendorType = m.GetVendorTypeManager().Find(tblVendorMaster.VendorTypeCode).VendorType;
        //        m.GetVendorMasterManager().Update(tblVendorMaster);
        //        TempData["OperStatus"] = "Vendor updated succeessfully";
        //        ModelState.Clear();
        //        if (Request.IsAjaxRequest())
        //        {
        //            return Json(new { success = true });
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    try
        //    {
        //        ViewBag.RegistrationType = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", Id);
        //        ViewBag.PRJID = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName", tblVendorMaster.PRJID);
        //        ViewBag.Country = new SelectList(m.GetCountryMasterManager().FindAllCountry(), "CountryID", "CnName", tblVendorMaster.Country);
        //        ViewBag.VendorTypeCode = new SelectList(m.GetVendorTypeManager().FindAll(), "TypeId", "VendorType", tblVendorMaster.VendorTypeCode);



        //    }
        //    catch
        //    { }


        //    try
        //    {

        //        ViewBag.State = new SelectList(m.GetCountryMasterManager().FindAllState(tblVendorMaster.Country), "StateID", "StateName", tblVendorMaster.State);

        //    }
        //    catch
        //    {               


        //    }
        //    try
        //    {
        //        ViewBag.City = new SelectList(m.GetCountryMasterManager().FindAllCity(tblVendorMaster.State), "CityID", "CityName", tblVendorMaster.City);


        //    }
        //    catch
        //    {               


        //    }


        //    if (Request.IsAjaxRequest())
        //    { return PartialView(tblVendorMaster); }
        //    return View(tblVendorMaster);
        //}

        // GET: tblVendorMasters/Delete/5
        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblVendorMaster vendorMaster = m.GetVendorMasterManager().Find((int)id);
            try
            {
                vendorMaster.Country = m.GetCountryMasterManager().FindCountryName(vendorMaster.Country);
                vendorMaster.State = m.GetCountryMasterManager().FindStateName(vendorMaster.State);
                vendorMaster.City = m.GetCountryMasterManager().FindCityName(vendorMaster.City);

                string[] projectIds = vendorMaster.PRJID.Split(',');

                List<string> projectNames = new List<string>();
                var ProjectManager = m.GetProjectMasterManager();
                for (int i = 0; i < projectIds.Length; i++)
                {
                    if(projectIds[i]!="")
                      projectNames.Add(ProjectManager.Find(projectIds[i]).ProjectName);
                }
                //vendorMaster.PRJID = projectNames.ToString();
                ViewBag.ProjectNames = new SelectList(projectNames);
            }
            catch { }

            if (vendorMaster == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
            { return PartialView(vendorMaster); }
            return View(vendorMaster);
        }

        // POST: tblVendorMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {


            bool a = m.GetVendorMasterManager().Delete((int)id);
            if (Request.IsAjaxRequest())
            {
                if (a == true)
                    return Json(new { success = true });
                else
                    return Json(new { success = false });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }



        public class TestVendorClass
        {
            public string VendorID { get; set; }
            public string Address { get; set; }
            //  public string BlackListStatus { get; set; }
            public string City { get; set; }
            public string ContactPerson { get; set; }
            public string Country { get; set; }
            public string BlackListStatus { get; set; }
            public string Email { get; set; }
            public string MobileNo { get; set; }

            public string VendorName { get; set; }

            public string PanNo { get; set; }
            public string PhoneNo { get; set; }
            public string PinCode { get; set; }
            public string RegistrationType { get; set; }
            public string[] PRID { get; set; }
            public string[] Regions { get; set; }
            public string State { get; set; }
            public string TinNo { get; set; }
            public string Status { get; set; }
            public string[] MulRegistrationType { get; set; }

            public string Vat_no { get; set; }
            public string GST_no { get; set; }
            public string AadharNo { get; set; }



        }


        //change18052018
        public void GetVendorExcel()
        {
            var searchData = TempData["searchData"] as List<GetVendorList>;

            if (searchData != null)
            {

                WebGrid grid = new WebGrid(source: searchData, canPage: false, canSort: false);

                string gridData = grid.GetHtml(
                    columns: grid.Columns(
                        grid.Column("VendorId", header: "Vendor Id", style: "text-align: center"),
                        grid.Column("Name", header: "Name", style: "text-align: center"),
                        grid.Column("VendorType", header: "Vendor Type", style: "text-align: center"),
                       grid.Column("TinNo", header: "Tin No", style: "text-align: center"),
                       grid.Column("Address", header: "Address", style: "addressStyle"),
                       grid.Column("PanNo", header: "PAN No.", style: "text-align: center"),
                       grid.Column("GSTNo", header: "GST No", style: "text-align: center"),
                       grid.Column("AadhaarNo", header: "Aadhaar No", style: "text-align: center;"),
                       grid.Column("ContactPerson", header: "Contact Person", style: "text-align: center;"),
                       grid.Column("MobileNo", header: "Mobile No", style: "text-align: center;"),
                       grid.Column("PhoneNo", header: "Phone No", style: "text-align: center;"),
                       grid.Column("Email", header: "Email", style: "text-align: center;"),
                        grid.Column("Region", header: "Region", style: "text-align: center;"),
                       grid.Column("Status", header: "Status", style: "text-align: center;")
                        )
                        ).ToString();



                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=AllVendorList.xls");
                Response.ContentType = "application/excel";
                Response.Write(gridData);
                Response.End();
                TempData.Keep();

            }
        }
    }
}
