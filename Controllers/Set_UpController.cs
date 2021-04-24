using MMS.DAL;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers
{
    public class Set_UpController : Controller
    {
        // GET: Set_Up
        private MMSEntities objmms = new MMSEntities();
        string EmpID = string.Empty;
        public Set_UpController()
        {
            if (System.Web.HttpContext.Current.Session["EmpID"] != null)
            {
                EmpID = System.Web.HttpContext.Current.Session["EmpID"].ToString();
            }

        }


        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.Session["EmpID"] != null)
            {
                EmpID = System.Web.HttpContext.Current.Session["EmpID"].ToString();
            }
            return View();
        }

        public ActionResult GetGetail(string ItemgrpId, string ItemId)
        {
            if (ItemgrpId != "" && ItemgrpId != "0" && (ItemId == "" || ItemId == "0"))
            {
                var List = objmms.tblItemMasters.Where(x => x.ItemGroupID == ItemgrpId).OrderBy(x => x.ItemName).ToList();
                if (List.Count() > 0)
                {
                    var gList = List.Select(x => new GetExcess_Item
                    {
                        TrandId = x.TransID,
                        ItemId = x.ItemID,
                        ItemName = x.ItemName,
                        GroupId = x.ItemGroupID,
                        GroupName = objmms.tblItemGroupMasters.Where(y => y.ItemGroupID == x.ItemGroupID).FirstOrDefault().GroupName,
                        UnitName = objmms.tblUnitMasters.Where(u => u.UnitID == x.UnitID).FirstOrDefault().UnitName,
                        HSNCode = x.HSNCode,
                        GIETMCode = x.GITEMCode,
                        PercentageOfExcess = x.Excess_Item_Percentage
                    }).ToList();

                    var totalRows = gList.Count();

                    var data1 = new GetGrid_ExcessITEMS()
                    {
                        TotalRows = totalRows,
                        PageSize = 25,
                        EX_IT = gList
                    };

                    if (data1 != null && data1.TotalRows != 0)
                    {
                        if (Request.IsAjaxRequest())
                        {
                            return PartialView("_Item_Grid", data1);
                        }
                        else
                        {
                            return PartialView("_Item_Grid", data1);
                        }
                    }
                    else
                    {
                        return View("_EmptyView");
                    }

                }
                else
                {
                    return View("_EmptyView");
                }
            }
            else if (ItemgrpId != "" && ItemgrpId != "0" && (ItemId != "" && ItemId != "0"))
            {
                var List = objmms.tblItemMasters.Where(x => x.ItemGroupID == ItemgrpId && x.ItemID == ItemId).OrderBy(x => x.ItemName).ToList();
                if (List.Count() > 0)
                {
                    var gList = List.Select(x => new GetExcess_Item
                    {
                        TrandId = x.TransID,
                        ItemId = x.ItemID,
                        ItemName = x.ItemName,
                        GroupId = x.ItemGroupID,
                        GroupName = objmms.tblItemGroupMasters.Where(y => y.ItemGroupID == x.ItemGroupID).FirstOrDefault().GroupName,
                        UnitName = objmms.tblUnitMasters.Where(u => u.UnitID == x.UnitID).FirstOrDefault().UnitName,
                        HSNCode = x.HSNCode,
                        GIETMCode = x.GITEMCode,
                        PercentageOfExcess = x.Excess_Item_Percentage
                    });

                    var totalRows = gList.Count();

                    var data1 = new GetGrid_ExcessITEMS()
                    {
                        TotalRows = totalRows,
                        PageSize = 25,
                        EX_IT = gList
                    };

                    if (data1 != null && data1.TotalRows != 0)
                    {
                        if (Request.IsAjaxRequest())
                        {
                            return PartialView("_Item_Grid", data1);
                        }
                        else
                        {
                            return PartialView("_Item_Grid", data1);
                        }
                    }
                    else
                    {
                        return View("_EmptyView");
                    }
                }
                else
                {
                    return View("_EmptyView");
                }

            }
            else
            {
                return View("_EmptyView");
            }


        }



        public ActionResult SaveExcessItemWisePercentage(string GridData)
        {
            List<tblItemExcessPercentageHistory> tbIEp = new List<tblItemExcessPercentageHistory>();
            var log = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(GridData);

            


            if (log != null)
            {
                foreach (var x in log)
                {
                    if (Convert.ToDecimal(x.Value[3]) > 0)
                    {
                        var ItmId = x.Value[1].ToString();
                        var ItmLst = objmms.tblItemMasters.Where(y => y.ItemID == ItmId).FirstOrDefault();
                        if (ItmLst != null)
                        {
                            //if (EmpID != null)
                            //{
                                ItmLst.Excess_Item_Percentage = Convert.ToDecimal(x.Value[3]);
                                objmms.Entry(ItmLst).State = EntityState.Modified;

                                // for save history
                                tblItemExcessPercentageHistory tb_obj = new tblItemExcessPercentageHistory();
                                tb_obj.CreatedBy = EmpID;
                                tb_obj.CreatedDate = System.DateTime.Now;
                                tb_obj.ItemId = x.Value[1].ToString();
                                tb_obj.ItemGroupId = x.Value[2].ToString();
                                tb_obj.ItemExcessPercentage = Convert.ToDecimal(x.Value[3]);
                                tbIEp.Add(tb_obj);


                                //if (tbIEp.ToList().Count() > 0)
                                //{
                                //    objmms.tblItemExcessPercentageHistories.AddRange(tbIEp);
                                //    objmms.SaveChanges();
                                //   // return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
                                //}
                                //else
                                //{
                                //    return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);
                                //}

                            //}
                            //else
                            //{
                            //    return Json(new { Status = "3" }, JsonRequestBehavior.AllowGet);
                            //}



                        }
                        else
                        {
                            return Json(new { Status = "4" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Status = "5" }, JsonRequestBehavior.AllowGet);

                    }
                }


                if (tbIEp.ToList().Count() > 0)
                {
                    objmms.tblItemExcessPercentageHistories.AddRange(tbIEp);
                    objmms.SaveChanges();
                   return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Status = "2" }, JsonRequestBehavior.AllowGet);
                }



            }
            else
            {
                return Json(new { Status = "6" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Status = "7" }, JsonRequestBehavior.AllowGet);
        }



    }
}