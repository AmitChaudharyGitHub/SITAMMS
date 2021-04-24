using MMS.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMS.Models
{
    public class ModelServices
    {
        private readonly MMSEntities objmms = new MMSEntities();

        //For Edit 
        public ns_AddPriceCap GetPriceCapDetail(int pcpID)
        {
            return objmms.ns_AddPriceCap.Where(m => m.Id == pcpID).FirstOrDefault();
        }
        public ns_AddPriceCap GetPriceCapDetailByItem(string itemid)
        {
            return objmms.ns_AddPriceCap.Where(m => m.ItemId == itemid && m.EffectiveStatus == "On").FirstOrDefault();
        }

        public bool UpdatePriceCap(ns_AddPriceCap apcs)
        {
            try
            {
                ns_AddPriceCap data = objmms.ns_AddPriceCap.Where(m => m.Id == apcs.Id).FirstOrDefault();
                data.ItemGroupId = apcs.ItemGroupId;
                data.ItemId = apcs.ItemId;
                data.Rate = apcs.Rate;
                data.EffectiveDate = apcs.EffectiveDate;
                data.Status = apcs.Status;
                data.ModifiedBy = apcs.ModifiedBy;
                data.ModifiedDate = DateTime.Now;
                data.EditedStatus = "Yes";
                objmms.SaveChanges();
                return true;
            }
            catch (Exception mex)
            {
                return false;
            }
        }

        
        public bool UpdatePriceCapDelete(int id)
        {
            try
            {
                ns_AddPriceCap data = objmms.ns_AddPriceCap.Where(m => m.Id == id).FirstOrDefault();              
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy= HttpContext.Current.Session["EmpID"].ToString();
                data.EditedStatus = "Yes";
                data.EffectiveStatus = "Off";
                objmms.SaveChanges();
                return true;
            }
            catch (Exception mex)
            {
                return false;
            }
        }
    }
    public class PricecapModel
    {

        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Select Project")]
        public string[] ProjectId { get; set; }

        [Required(ErrorMessage = "Please Select Item Group")]
        public string ItemGroupId { get; set; }

        [Required(ErrorMessage = "Please Select Item")]
        //[UIHint("Multiline")]
        public string ItemId { get; set; }

        [Required(ErrorMessage = "Please Enter Rate")]
        public Nullable<decimal> Rate { get; set; }

        [Required(ErrorMessage = "Please Enter Date")]
        public Nullable<System.DateTime> EffectiveDate { get; set; }     
        public string Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string EditedStatus { get; set; }

        //[Required(ErrorMessage = "Please Enter Mobile")]
        //public string Mobile { get; set; }
    }

    public class PricecapModel2
    {

        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Select Project")]
        public string[] ProjectId { get; set; }

        [Required(ErrorMessage = "Please Select Item Group")]
        public string ItemGroupId { get; set; }

        [Required(ErrorMessage = "Please Select Item")]
        //[UIHint("Multiline")]
        public string ItemId { get; set; }

        [Required(ErrorMessage = "Please Enter Rate")]
        public Nullable<decimal> Rate { get; set; }

        [Required(ErrorMessage = "Please Enter Date")]
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string Status { get; set; }
        public string ModifiedBy { get; set; }
        
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string EditedStatus { get; set; }
        public string CreatedBy { get; set; }
        //[Required(ErrorMessage = "Please Enter Mobile")]
        //public string Mobile { get; set; }
    }

   
}