using MMS.DAL;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MMS.Controllers.API
{

    public class GRNDataController : ApiController
    {
        MMSEntities db = new MMSEntities();
        //[Route("api/GRNData/")]
        [HttpPost]
        public HttpResponseMessage Create(VMData Data)
        {

            try
            {
                db.tbl_wmntdata.AddRange(Data.Data);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK,new {Msg="Success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Msg = "Failure" });
            }
        }
    }

    public class VMData
    {
        public List<tbl_wmntdata> Data { get; set; }
    }
}
