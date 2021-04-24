using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.Models;
using MMS.DAL;
using System.Data;

namespace MMS.Controllers
{
    public class MySiteController : Controller
    {
        // GET: MySite
        public MMSEntities objmms = new MMSEntities();

        //public ActionResult _IndentNotifications()
        //{

        //    try
        //    {
        //        //objmms = new MMSEntities();
        //        //var reslt = from c in objmms.tblIndentRequisitions
        //        //            where (c.IsRead == "No")
        //        //            select (c.IsRead).Count();

        //        //int sdar = reslt.Count();

        //        int Count = objmms.tblIndentRequisitions.Where(b => b.IsRead == "No").Select(a => a.IsRead).Count();
        //        ViewBag.message = Count;


        //        List<string> dsa = new List<string>();
        //        List<string> ax = new List<string>();
        //        List<int> idis = new List<int>();

        //        //
        //        var resultss = (from m in objmms.tblIndentRequisitions

        //                        select m.CreatedBy).Take(5).Distinct();

        //        foreach (var item in resultss)
        //        {
        //            string Empid = item.ToString();

        //            // foreach (var ecd in Empid)
        //            //{
        //            string Unitid = "";
        //            string datesin = "";
        //            int idisread = 0;

        //            var data = objmms.tblIndentRequisitions.Where(m => m.CreatedBy == Empid.Trim().ToUpper()).FirstOrDefault();
        //            if (data != null && data.CreatedBy.Length > 0)
        //            {
        //                Unitid = data.CreatedBy;
        //                datesin = data.CreatedDate.ToString();
        //                idisread = data.Id;

        //                var empnames = objmms.tblEmployeeMasters.Where(m => m.EmpID == Unitid.Trim().ToUpper()).Select(a => a.FName).FirstOrDefault();
        //                Unitid = empnames;

        //            }
        //            ax.Add(Unitid);
        //            dsa.Add(datesin);
        //            idis.Add(idisread);
        //        }

        //        if (ax != null && dsa != null && idis != null)
        //        {

        //            ViewBag.Emplyeename1 = ax[0].ToString();
        //            ViewBag.Emplyeename2 = ax[1].ToString();
        //            ViewBag.Emplyeename3 = ax[2].ToString();
        //            ViewBag.Emplyeename4 = ax[3].ToString();
        //            ViewBag.Emplyeename5 = ax[4].ToString();

        //            ViewBag.Date1 = dsa[0].ToString();
        //            ViewBag.Date2 = dsa[1].ToString();
        //            ViewBag.Date3 = dsa[2].ToString();
        //            ViewBag.Date4 = dsa[3].ToString();
        //            ViewBag.Date5 = dsa[4].ToString();


        //            ViewBag.idmain = idis;

        //        }
        //        else
        //        {
        //            //ViewBag.Emplyeename1=ax[0].ToString();
        //            //ViewBag.Emplyeename2 = ax[1].ToString();
        //            //ViewBag.Emplyeename3 = ax[2].ToString();
        //            //ViewBag.Emplyeename4 = ax[3].ToString();
        //            //ViewBag.Emplyeename5 = ax[4].ToString();

        //            //ViewBag.Date1 = dsa[0].ToString();
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //    }



        //    return View();
        //}


        public MySiteController()
        {
            try
            {
                //objmms = new MMSEntities();
                //var reslt = from c in objmms.tblIndentRequisitions
                //            where (c.IsRead == "No")
                //            select (c.IsRead).Count();

                //int sdar = reslt.Count();

                int Count = objmms.tblIndentRequisitions.Where(b => b.IsRead == "No").Select(a => a.IsRead).Count();
                ViewBag.message = Count;


                 List<string> dsa = new List<string>();
                 List<string> ax =new List<string>();
                 List<int> idis = new List<int>();

                //
                var resultss = (from m in objmms.tblIndentRequisitions

                              select m.CreatedBy).Take(5).Distinct();

                foreach (var item in resultss)
                {
                    string Empid = item.ToString();
                   
                   // foreach (var ecd in Empid)
                    //{
                        string Unitid = "";
                        string datesin = "";
                        int idisread=0;

                        var data = objmms.tblIndentRequisitions.Where(m => m.CreatedBy == Empid.Trim().ToUpper()).FirstOrDefault();
                        if (data != null && data.CreatedBy.Length > 0)
                        {
                            Unitid = data.CreatedBy;
                            datesin = data.CreatedDate.ToString();
                            idisread = data.Id;

                            var empnames = objmms.tblEmployeeMasters.Where(m => m.EmpID == Unitid.Trim().ToUpper()).Select(a => a.FName).FirstOrDefault();
                            Unitid = empnames;
                           
                        }
                        ax.Add(Unitid);
                        dsa.Add(datesin);
                        idis.Add(idisread);
                }

                if (ax !=null && dsa!=null && idis!=null)
                 {
                  
                  ViewBag.Emplyeename1=ax[0].ToString();
                  ViewBag.Emplyeename2 = ax[1].ToString();
                  ViewBag.Emplyeename3 = ax[2].ToString();
                  ViewBag.Emplyeename4 = ax[3].ToString();
                  ViewBag.Emplyeename5 = ax[4].ToString();

                  ViewBag.Date1 = dsa[0].ToString();
                  ViewBag.Date2 = dsa[1].ToString();
                  ViewBag.Date3 = dsa[2].ToString();
                  ViewBag.Date4 = dsa[3].ToString();
                  ViewBag.Date5 = dsa[4].ToString();


                  ViewBag.idmain = idis;
                  
                 }
                else
                 {
                  //ViewBag.Emplyeename1=ax[0].ToString();
                  //ViewBag.Emplyeename2 = ax[1].ToString();
                  //ViewBag.Emplyeename3 = ax[2].ToString();
                  //ViewBag.Emplyeename4 = ax[3].ToString();
                  //ViewBag.Emplyeename5 = ax[4].ToString();

                  //ViewBag.Date1 = dsa[0].ToString();
                 }


            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}