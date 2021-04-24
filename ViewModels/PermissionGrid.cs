using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class PermissionGrid
    {
        public List<GridData> SubGrid { get; set; }
    }


    public class GridData
    {
        public int SubMenuId { get; set; }
        public string ProjectId { get; set; }
        public string UserId { get; set; }

    }
}