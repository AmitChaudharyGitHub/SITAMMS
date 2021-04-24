using MMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMS.ViewModels
{
    public class STC_ViewModel
    {
        public tblSpecificTermsAndCondition obStc { get; set; }
        public List<tblSpecificTermsAndCondition> allDetails { get; set; }
    }


    public class Genral_Terms_and_conditions
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Header_Title { get; set; }
        public string GTC_Description { get; set; }
        public string CompanyId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Isdeleted { get; set; }
        public string Purchase_Order_No { get; set; }
        public bool IsHaving { get; set; }
    }

    public class Genral_Terms_and_conditions_WebGrids
    {
        public int TotalRows { get; set; }
        public IEnumerable<Genral_Terms_and_conditions> GTCDATAS { get; set; }
        public int PageSize { get; set; }
    }

    public class Specific_Instruction_Conditions
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Header_Title { get; set; }
        public string SI_Description { get; set; }
        public string CompanyId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Isdeleted { get; set; }
        public string Purchase_Order_No { get; set; }

        public bool IsHaving { get; set; }
    }

    public class Specific_Instruction_Conditions_WebGrids
    {
        public int TotalRows { get; set; }
        public IEnumerable<Specific_Instruction_Conditions> SI_DATAS { get; set; }
        public int PageSize { get; set; }
    }

    public class Specific_Terms_And_Conditions
    {
        public int? Id { get; set; }
        public string ProjectId { get; set; }
        public string Statement_Header { get; set; }
        public string STC_Description { get; set; }
        public string Company_Id { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Isdeleted { get; set; }
        public string Purchase_Order_No { get; set; }
        public string ProjectName { get; set; }

        public bool IsHaving { get; set; }

    }

    public class Specific_Terms_And_Conditions_WebGrids
    {
        public int TotalRows { get; set; }
        public IEnumerable<Specific_Terms_And_Conditions> STC_DATAS { get; set; }
        public int PageSize { get; set; }
    }




}