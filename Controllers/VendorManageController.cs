using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using MMS.DAL;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.ComponentModel.DataAnnotations;
using DataAccessLayer;
using MMS.Models;

namespace MMS.Controllers
{
    public class VendorManageController : ApiController
    {
        FactoryManager m = new FactoryManager();
        Procedure procedure = new Procedure();
        private MMSEntities objmms = new MMSEntities();
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("api/VendorManage/GetCountries")]
        public object GetCountries()
        {
            var countries = objmms.tblCountries.Select(s => new { ID = s.CountryID, Name = s.CnName }).ToList();
            return Json(countries);
        }

        [HttpGet]
        [Route("api/VendorManage/GetStates")]
        public object GetStates()
        {
            var states=objmms.tblStates.Select(s => new { ID = s.StateID, Name = s.StateName, CountryID=s.CountryID }).ToList();
            return Json(states);
        }

        [HttpGet]
        [Route("api/VendorManage/GetCities")]
        public object GetCities()
        {
            var cities=objmms.tblCityLists.Select(s => new { ID = s.CityID, Name = s.CityName, StateID=s.StateID }).ToList();
            return Json(cities);
        }

        [HttpGet]
        [Route("api/VendorManage/GetVendorNature")]
        public object GetVendorNature()
        {
            //var naturesdt = GetData("GetPartyVendorType", null);
            List<object> natures = new List<object>();

            var RegistrationType = m.GetVendorGroupManager().FindAll();
            //foreach (DataRow row in naturesdt.Rows)
            //{
            //    natures.Add(new
            //    {
            //        ID = row["TypeID"].ToString(),
            //        Name = row["VendorType"].ToString(),
            //    });
            //}
            foreach (var VG in RegistrationType)
            {
                natures.Add(new
                {
                    ID = VG.TypeID,
                    Name = VG.RegistrationType
                }
                    );
            }

            return Json(natures);
        }

        [HttpGet]
        [Route("api/VendorManage/GetRegistrationType")]
        public object GetVendorRegistrationType()
        {
            SqlParameter[] sqlPara = new SqlParameter[] { new SqlParameter("@FValue", "COMP000001") };
            var naturesdt = GetData("GetRegistrationType", sqlPara);
            List<object> natures = new List<object>();

            foreach (DataRow row in naturesdt.Rows)
            {
                natures.Add(new
                {
                    ID = row["TypeID"].ToString(),
                    Name = row["RegistrationType"].ToString(),
                });
            }

            return Json(natures);
        }



        [HttpPost]
        [Route("api/VendorManage/PostVendorRegistration")]
        public object PostVendorData(VendorRegisterData VendorModel)
        {
            if (!ModelState.IsValid)
                return Json(new { Result = 0, Msg = "Vendor Data not valid." });
            else
            {
                object response= SaveVendorData(VendorModel);
                return Json(response);
            }
        }

        [HttpGet]
        [Route("api/VendorManage/AllVendorData")]
        public object GetAllVendorData()
        {
            var vendors = objmms.Database.SqlQuery<VendorInfoClass> ("usp_GetAllVendorData").ToList();
            return Json(vendors);
        }


        private object SaveVendorData(VendorRegisterData VendorObj)
        {
            string ConStr = WebConfigurationManager.ConnectionStrings["Model1"].ConnectionString.ToString();
            SqlConnection SqlCon = new SqlConnection(ConStr);
            string exMsg = "";
            int result = 0;
            try
            {
                using (SqlCommand cmd = new SqlCommand("InsertVendorDetail_New"))
                {
                    cmd.Connection = SqlCon;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CompanyID", VendorObj.CompanyID);
                    cmd.Parameters.AddWithValue("@PRJID", VendorObj.ProjectID);
                    cmd.Parameters.AddWithValue("@VendorID", VendorObj.VendorID);
                    cmd.Parameters.AddWithValue("@Name", VendorObj.Name);
                    cmd.Parameters.AddWithValue("@Address", VendorObj.Address);
                    cmd.Parameters.AddWithValue("@Country", VendorObj.Country);
                    cmd.Parameters.AddWithValue("@State", VendorObj.State);
                    cmd.Parameters.AddWithValue("@city", VendorObj.city);
                    cmd.Parameters.AddWithValue("@MobileNo", VendorObj.Mobile);
                    cmd.Parameters.AddWithValue("@Email", VendorObj.Email);
                    cmd.Parameters.AddWithValue("@VendorType", VendorObj.VendorType);
                    cmd.Parameters.AddWithValue("@CreatedBy", VendorObj.CreatedBy);
                    cmd.Parameters.AddWithValue("@VendorTypeCode", VendorObj.VendorTypeCode);
                    cmd.Parameters.AddWithValue("@PANNo", VendorObj.PANNo);
                    cmd.Parameters.AddWithValue("@RegistrationType", VendorObj.RegistrationType);
                    cmd.Parameters.AddWithValue("@PinCode", VendorObj.PinCode);
                    cmd.Parameters.AddWithValue("@PhoneNo", VendorObj.PhoneNo);
                    cmd.Parameters.AddWithValue("@GSTNo", VendorObj.GSTNo);
                    cmd.Parameters.AddWithValue("@Status", VendorObj.Status);
                    cmd.Parameters.AddWithValue("@ContactPerson", VendorObj.ContactPerson);
                    cmd.Parameters.AddWithValue("@BlackListStatus", VendorObj.BlackListStatus);
                    cmd.Parameters.AddWithValue("@FromAPI", 1);
                    cmd.Parameters.AddWithValue("@AadhaarNo", VendorObj.AadhaarNo);
                    if (SqlCon.State == ConnectionState.Open)
                        SqlCon.Close();
                    SqlCon.Open();
                    result = cmd.ExecuteNonQuery();
                    if (result == 1)
                        exMsg = "Successfully Saved.";
                    else
                        exMsg = "Vendor Data Already Exists.";
                }
            }
            catch (Exception ex)
            {
                exMsg = ex.Message;
            }
            finally {
                SqlCon.Close();
            }

            return new { Result = result, Msg = exMsg };

        }

        private DataTable GetData(string procName, SqlParameter[] sqlPara)
        {
            string ConStr = WebConfigurationManager.ConnectionStrings["PMCConStr"].ConnectionString.ToString();
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection SqlCon = new SqlConnection(ConStr))
                {
                    using (SqlCommand cmd = new SqlCommand(procName, SqlCon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (sqlPara != null)
                        {
                            foreach (SqlParameter spara in sqlPara)
                            {
                                cmd.Parameters.Add(spara);
                            }
                        }
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        sda.Fill(dt);



                    }
                }
            }
            catch (Exception ex)
            {
                // exMsg = ex.Message;
            }

            return dt;
        }


    }

    public class VendorRegisterData
    {
        [Required]
        public string CompanyID { get; set; }
        [Required]
        public string ProjectID { get; set; }
        [Required]
        public string VendorID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string city { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string VendorType { get; set; }
        [Required]
        public string VendorTypeCode { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        [Required]
        public string PANNo { get; set; }
        [Required]
        public string RegistrationType { get; set; }
        public string PinCode { get; set; }
        public string PhoneNo { get; set; }
        public string GSTNo { get; set; }
        public string ContactPerson { get; set; }
        public string BlackListStatus { get; set; }
        public string AadhaarNo { get; set; }
    }


    public class VendorInfoClass
    {
        public string CompanyID { get; set; }
        public string PRJID { get; set; }
        public string VendorID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string VendorType { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string VendorTypeCode { get; set; }
        public string PanNo { get; set; }
        public string TinNo { get; set; }
        public string EccNo { get; set; }
        public string Vat { get; set; }
        public string RegistrationType { get; set; }
        public string PinCode { get; set; }
        public string SalesTaxNo { get; set; }
        public string CSTNo { get; set; }
        public string ServiceTaxNo { get; set; }
        public string PhoneNo { get; set; }
        public string BlackListStatus { get; set; }
        public string ContactPerson { get; set; }
        public string MulRegistrationType { get; set; }
        public string VAT_No { get; set; }
        public string GST_NO { get; set; }
        public string Aadhaar_No { get; set; }
    }
}