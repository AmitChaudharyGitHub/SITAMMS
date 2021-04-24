using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PendingTRansferGRN
    {
        public int Id { get; set; }
        public Nullable<Int32> TransferMasterId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string TransferProjectId { get; set; }
        public string TransferprojectName { get; set; }
        public string Transferno { get; set; }
        public string TransferDate { get; set; }
        public string Transferdateofcreation { get; set; }
        public string GetOutNo { get; set; }
        public string  GetOutDate { get; set; }

        public string GRNNo { get; set; }
        public string GRNTime { get; set; }
        public string GRNCreatedDate { get; set; }




    }
}