using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMS.Models;
using MMS.DAL;
using System.Data;
using MMS_P.ViewModels;
using MMS.ViewModels;
using System.Data.Entity;
using System.Linq.Dynamic;
using System.Data.SqlClient;

namespace MMS.Controllers
{
    public class DateWiseController : Controller
    {
        // GET: DateWise
        public ActionResult Index()
        {
            using (var ctx = new MMSEntities())
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "DailyReport",
                    Value = 1
                };
                //Get student name of string type
                var courseList = ctx.Database.SqlQuery<GateEntryChild>("exec DateWiseMaterialReport @ProjectNo ", idParam).ToList<GateEntryChild>();

                //Or can call SP by following way
                //var courseList = ctx.Courses.SqlQuery("exec GetCoursesByStudentId @StudentId ", idParam).ToList<Course>();

                foreach (Course cs in courseList)
                    Console.WriteLine("Course Name: {0}", cs.CourseName);
            }
            return View();
        }
    }
}