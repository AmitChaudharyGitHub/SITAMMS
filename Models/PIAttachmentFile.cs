using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class PIAttachmentModel
    {
        public List<PIAttachFile> PIAttachFiles { get; set; }
    }

    public class PIAttachFile
    {
        public HttpPostedFileBase AttachFile { get; set; }
        public string FileName { get; set; }
    }

    public class PI_AttachFileModel {
        public string AttachFile { get; set; }
        public string FileName { get; set; }
    }
}