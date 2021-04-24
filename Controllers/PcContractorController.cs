using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Entity;
//using BusinessLogicLayer;
using MMS_P.ViewModels;
using DataAccessLayer;
using Newtonsoft.Json;
using System.Net;

namespace MMS.Controllers
{
    public class PcContractorController : Controller
    {
        FactoryManager m = new FactoryManager();

        static private List<tblRegistrationType> VendorGroupList;
        string Id = "0";
        string EmpID = null;
        private tblRegistrationType VendorGroup;


        public PcContractorController()
        {
            try
            {
                EmpID = System.Web.HttpContext.Current.Session["EmpID"].ToString();


            }
            catch
            {
            }
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

        public ActionResult Index()

        {
            ViewBag.VGId = new SelectList(m.GetVendorGroupManager().FindPCAll(), "TypeId", "RegistrationType", Id);
            return View();
        }

        public ActionResult Grid(string VGId = "0", int page = 1, string sort = "Name", string sortDir = "ASC")
        {
            const int pageSize = 15000;
            sortDir = sortDir.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? sortDir : "asc";
            var validColumns = new[] { "PcContractorID", "Name", "City" };
            if (!validColumns.Any(c => c.Equals(sort, StringComparison.CurrentCultureIgnoreCase)))
                sort = "PcContractorID";

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
                    var totalRows = m.GetPcContractorMasterManager().CountByVendorGroup(VendorGroup.TransID);
                    var VendorMasters = m.GetPcContractorMasterManager().FindPageByVendorGroup(VendorGroup.TransID, page, pageSize);
                    var ven = m.GetPcContractorMasterManager().FindPageByVendorGroupNew(VendorGroup.TypeID, page, pageSize);
                    //var VendorMasters = m.GetVendorMasterManager().FindPageByVendorGroup(VendorGroup.TypeID, page, pageSize);

                    var data = new PageVendorMasterModelPage()
                    {
                        TotalRows = totalRows,
                        PageSize = pageSize,
                        //  VendorMasterBll = VendorMasters.ToList()
                        PcContractorrMasterBll = ven.ToList()
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
            tblPcContractorMaster vendorMaster = m.GetPcContractorMasterManager().Find((int)id);

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
             //ViewBag.RegistrationType = new SelectList(m.GetVendorGroupManager().FindAll(), "TypeId", "RegistrationType", Id);
            ViewBag.RegistrationType = new SelectList(m.GetVendorGroupManager().FindOnlyPettyContractorPC(), "TypeId", "RegistrationType", Id);
            ViewBag.VendorTypeCode = new SelectList(m.GetVendorTypeManager().FindAll(), "TypeId", "VendorType");

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

        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult Create(TestVendorClass VData)
        {
            bool IsPan;
            tblPcContractorMaster vm = new tblPcContractorMaster();
            vm.CreatedDate = System.DateTime.Now;
            vm.CreatedBy = EmpID;

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
            // vm.CompanyID = Session["CompanyId"].ToString();
            vm.CompanyID = "COMP000001";
            IsPan = m.GetRepeatChequeManager().PcContractorPanNoCheck(VData.PanNo);
            if (IsPan == true)
            {

           
            int i = m.GetPcContractorMasterManager().Add(vm);
            if (i != 0)
            {

                if (Request.IsAjaxRequest())
                {

                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }

            ViewBag.Country = new SelectList(m.GetCountryMasterManager().FindAllCountry(), "CountryID", "CnName");
            ViewBag.RegistrationType = new SelectList(m.GetVendorGroupManager().FindPCAll(), "TypeId", "RegistrationType", Id);
            ViewBag.PRJID = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName");
            ViewBag.VendorTypeCode = new SelectList(m.GetVendorTypeManager().FindAll(), "TypeId", "VendorType");


            return Json("1", JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult IsVendorNameUnique(tblPcContractorMaster catalog)
        {

            return m.GetRepeatChequeManager().PcContractorNameRepeat(catalog.Name, catalog.RegistrationType, catalog.TransID)
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
            tblPcContractorMaster vendorMaster = m.GetPcContractorMasterManager().Find((int)id);
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

                ViewBag.RegistrationType = new MultiSelectList(m.GetVendorGroupManager().FindPCAll(), "TypeId", "RegistrationType", RegVal);
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
            tblPcContractorMaster vm = new tblPcContractorMaster();

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
            vm.PcContractorID = VData.VendorID;

            m.GetPcContractorMasterManager().Update(vm);

            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true });
            }
            //  return RedirectToAction("Index");
            try
            {
                ViewBag.RegistrationType = new SelectList(m.GetVendorGroupManager().FindPCAll(), "TypeId", "RegistrationType", Id);
                ViewBag.PRJID = new SelectList(m.GetProjectMasterManager().FindAll(), "PRJID", "ProjectName", vm.PRJID);
                ViewBag.Country = new SelectList(m.GetCountryMasterManager().FindAllCountry(), "CountryID", "CnName", vm.Country);
                // ViewBag.VendorTypeCode = new SelectList(m.GetVendorTypeManager().FindAll(), "TypeId", "VendorType", tblVendorMaster.VendorTypeCode);



            }
            catch
            { }

            try
            {

                ViewBag.State = new SelectList(m.GetCountryMasterManager().FindAllState(vm.Country), "StateID", "StateName", vm.State);

            }
            catch
            {


            }

            try
            {
                ViewBag.City = new SelectList(m.GetCountryMasterManager().FindAllCity(vm.State), "CityID", "CityName", vm.City);


            }
            catch
            {


            }


            if (Request.IsAjaxRequest())
            { return PartialView(vm); }
            return View(vm);



        }

        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPcContractorMaster vendorMaster = m.GetPcContractorMasterManager().Find((int)id);
            try
            {
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
            { return PartialView(vendorMaster); }
            return View(vendorMaster);
        }

        // POST: tblVendorMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {


            bool a = m.GetPcContractorMasterManager().Delete((int)id);
            if (Request.IsAjaxRequest())
            {
                if (a == true)
                    return Json(new { success = true });
                else
                    return Json(new { success = false });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SubmitForm(TestVendorClass t)
        {
            return View();
        }

        public ActionResult SubmitForm()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CreatePcContractor(TestVendorClass t)
        {
            return View();
        }

        public ActionResult CreatePcContractor()
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

            if (Request.IsAjaxRequest())
            {
                ViewBag.IsUpdate = false;
                return PartialView();
            }
            else
                return View();

         
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
            public string State { get; set; }
            public string TinNo { get; set; }
            public string Status { get; set; }
            public string[] MulRegistrationType { get; set; }



        }

    }
}