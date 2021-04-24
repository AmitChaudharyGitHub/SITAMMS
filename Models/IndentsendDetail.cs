using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMS.DAL;

namespace MMS.Models
{
    public class IndentsendDetail
    {
        public tblIndentRequisition obj_tblIndentRequisition { get; set; }
        public List<tblIndentRequisition> detalsIndent { get; set; }
    }
}