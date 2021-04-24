using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class AllowPermissionViewModel
    {


        public int TransId { get; set; }
        public string HName { get; set; }

        public List<Gdata> P_Child { get; set; }


       


    }

    public class Gdata
    {
        public int Id { get; set; }
        public string HeaderName { get; set; }
        public string SubHeaderName { get; set; }
        public Nullable<bool> HeaderStatus { get; set; }
        public Nullable<bool> SubHeaderStatus { get; set; }
        public Nullable<bool> PageStatus { get; set; }
        public Nullable<int> MenuHeaderRank { get; set; }
    }

   
   


}