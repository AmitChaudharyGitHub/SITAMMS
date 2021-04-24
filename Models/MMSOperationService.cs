using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMS.DAL;
using System.Collections;

namespace MMS.Models
{
    public class MMSOperationService:IDisposable
    {
        private readonly MMSEntities objmmsentities = new MMSEntities();

        public IEnumerable GetSendedIndentDetails()
        {
            return objmmsentities.tblIndentRequisitions.ToList();
        }
        public void Dispose()
        {
            objmmsentities.Dispose();
        }
    }
}