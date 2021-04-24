using MMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PermissionViewModel
    {
        public tblMainHeader Main { get; set; }
        public List<tblSubHeader> Sub { get; set; } 


    }
}