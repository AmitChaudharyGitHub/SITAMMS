using MMS.DAL;
using MMS.Models;
using MMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMS.Controllers.RemoveOpening
{
    public class RemoveOpeningController : Controller
    {
        // GET: RemoveOpening
        string EmpId = string.Empty;
        private MMSEntities objmms = new MMSEntities();
        Procedure objpro = new Procedure();
        public RemoveOpeningController()
        {
            try
            {
                EmpId = System.Web.HttpContext.Current.Session["EmpId"].ToString();
                
            }
            catch
            {
            }
        }


        [SessionExpireFilter]
        public ActionResult Index()
        {
            if (EmpId == null)
            {
                return RedirectToAction("Login", "MyAccount");
            }
            ViewBag.EmpId = EmpId;

            var ItemGroup = objmms.tblItemGroupMasters.OrderBy(a => a.GroupName).ToList();
            var t = ItemGroup.Select(x => new SelectListItem
            {
                Value = x.ItemGroupID.ToString(),
                Text = x.GroupName.ToString()
            });

            ViewBag.ItemGroups = t;

            return View();
        }

        public JsonResult GetItemByGroupWise(string ItemGroupId)
        {
            string J = string.Empty;
            var lst = objmms.tblItemMasters.Where(x => x.ItemGroupID == ItemGroupId).Select(p => new { Text = p.ItemName, Value = p.ItemID }).OrderBy(l=>l.Text).ToList();
            J = lst.ToJSON();
            return Json(J, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetGrid(string ProjectId, string ItemGroupId, string ItemId)
        {
            List<ItemOpeningDeleteDetail> result = new List<ItemOpeningDeleteDetail>();
            result = null;
            result = objpro.GetItemOpeningDetail(ProjectId, ItemGroupId, ItemId);
            if (result != null)
            {
                return PartialView("_ItemDetailGrid", result);
            }
            else {
                return View("_EmptyView");
            }


        }

        public ActionResult DeleteOpening(string[] ItemIDs, string ProjId)
        {
            // List<tblItemOpeningDeletedHistory> lst = new List<tblItemOpeningDeletedHistory>();

            //List<tblItemMasterStock> lst_1 = new List<tblItemMasterStock>();
            //List<tblItemMasterFirstOpening> lst_2 = new List<tblItemMasterFirstOpening>();
            //List<GateEntryChild> lst_3 = new List<GateEntryChild>();
            //List<GateEntry> lst_4 = new List<GateEntry>();
            //List<ns_tbl_IssueQuantity> lst_5 = new List<ns_tbl_IssueQuantity>();
            //List<>

            //if (ItemIDs.Length <= 10)
            //{
                // then delete performed other wise not.


                foreach (string itemNo in ItemIDs)
                {


                    var CheckMrn = objmms.GateEntryChilds.Where(x => x.ProjectNo == ProjId && x.ItemNo == itemNo && x.MRN_Receive > 0).ToList();
                    if (CheckMrn.Count() == 0)
                    {
                        #region




                        var ItemMasterStock = objmms.tblItemMasterStocks.Where(x => x.ItemNo == itemNo && x.ProjectNo == ProjId).FirstOrDefault();
                        var tblItemMasterFirst_Opening = objmms.tblItemMasterFirstOpenings.Where(x => x.ItemNo == itemNo && x.ProjectNo == ProjId).FirstOrDefault();
                        var GateEntryChild = objmms.GateEntryChilds.Where(x => x.ItemNo == x.ItemNo && x.ProjectNo == ProjId && x.Status == "Opening").FirstOrDefault();


                        var GateEntry = objmms.GateEntries.Where(x => x.UId == GateEntryChild.MUId).FirstOrDefault();


                        var ns_tbl_issueQty = objmms.ns_tbl_IssueQuantity.Where(x => x.ItemID == itemNo && x.ProjectID == ProjId).FirstOrDefault();
                        var tblItemCurrentStock = objmms.tblItemCurrentStocks.Where(x => x.ItemNo == itemNo && x.ProjectNo == ProjId).FirstOrDefault();
                        string UnitId = objmms.tblItemMasters.Where(x => x.ItemID == itemNo).First().UnitID;
                        Int32 ItemTransId = objmms.tblItemMasters.Where(x => x.ItemID == itemNo).First().TransID;
                        decimal? ItemTotalPurchase = objmms.tblItemMasterStocks.Where(x => x.ItemNo == itemNo && x.ProjectNo == ProjId).First().TotalPurchase;
                        decimal? ItemRate = objmms.tblItemMasterStocks.Where(x => x.ItemNo == itemNo && x.ProjectNo == ProjId).First().Rate;
                        decimal ? CashPurchaseChild_Uid = objmms.TbCashPurchaseChilds.Where(x => x.Unit == UnitId && x.ItemId == ItemTransId && x.PurchaseQty == ItemTotalPurchase && x.Rate == ItemRate).First().MUId;
                        var TbCashPurchaseMaster = objmms.TbCashPurchaseMasters.Where(x => x.UId == CashPurchaseChild_Uid).FirstOrDefault();
                        var tblReceiveData = objmms.tblReceiveDatas.Where(x => x.ItemId == itemNo && x.ProjectId == ProjId && x.IDType == "Opening").FirstOrDefault();


                        if (ItemMasterStock != null)
                        {
                            if (tblItemMasterFirst_Opening != null)
                            {
                                if (GateEntryChild != null)
                                {

                                    if (GateEntry != null)
                                    {

                                        if (ns_tbl_issueQty != null)
                                        {
                                            if (tblItemCurrentStock != null)
                                            {
                                                if (CashPurchaseChild_Uid != 0)
                                                {

                                                    if (TbCashPurchaseMaster != null)
                                                    {

                                                        if (tblReceiveData != null)
                                                        {

                                                            //Customer obj = db.Customers.Find(customerID);
                                                            //db.Customers.Remove(obj);

                                                            //tblItemMasterStock obj1 = objmms.tblItemMasterStocks.Find(ItemMasterStock.Id);
                                                            //// lst_1.Add(obj1); 
                                                            //objmms.tblItemMasterStocks.Remove(obj1);

                                                            //tblItemMasterFirstOpening obj2 = objmms.tblItemMasterFirstOpenings.Find(tblItemMasterFirst_Opening.Id);
                                                            //objmms.tblItemMasterFirstOpenings.Remove(obj2);

                                                            //GateEntryChild obj3 = objmms.GateEntryChilds.Find(GateEntryChild.UId);
                                                            //objmms.GateEntryChilds.Remove(obj3);

                                                            //GateEntry obj4 = objmms.GateEntries.Find(GateEntry.UId);
                                                            //objmms.GateEntries.Remove(obj4);

                                                            //ns_tbl_IssueQuantity obj5 = objmms.ns_tbl_IssueQuantity.Find(ns_tbl_issueQty.Id);
                                                            //objmms.ns_tbl_IssueQuantity.Remove(obj5);

                                                            //tblItemCurrentStock obj6 = tblItemCurrentStock;
                                                            //obj6.Qty = Convert.ToDecimal(0.00);
                                                            //objmms.Entry(obj6).State = System.Data.Entity.EntityState.Modified;

                                                            //TbCashPurchaseChild obj7 = objmms.TbCashPurchaseChilds.Find(CashPurchaseChild_Uid);
                                                            //objmms.TbCashPurchaseChilds.Remove(obj7);

                                                            //TbCashPurchaseMaster obj8 = objmms.TbCashPurchaseMasters.Find(TbCashPurchaseMaster.UId);
                                                            //objmms.TbCashPurchaseMasters.Remove(obj8);

                                                            //tblReceiveData obj9 = objmms.tblReceiveDatas.Find(tblReceiveData.TransId);
                                                            //objmms.tblReceiveDatas.Remove(obj9);

                                                            string delete = objpro.DeletedOpening(ProjId, itemNo);
                                                            if (delete != "-1")
                                                            {
                                                                tblItemOpeningDeletedHistory itemdel = new tblItemOpeningDeletedHistory();
                                                                itemdel.ItemId = tblReceiveData.ItemId;
                                                                itemdel.ItemGroupId = tblReceiveData.ItemGroupId;
                                                                itemdel.ItemUnitId = UnitId;
                                                                itemdel.Item_ReceiveQty = ItemTotalPurchase;
                                                                itemdel.Item_IssueQty = ItemMasterStock.TotalIssue;
                                                                itemdel.ProjectId = ProjId;
                                                                itemdel.DeletedBy = EmpId;
                                                                itemdel.DeletedDate = System.DateTime.Now;

                                                                objmms.tblItemOpeningDeletedHistories.Add(itemdel);
                                                                objmms.SaveChanges();

                                                            }

                                                            else {


                                                            }


                                                           
                                                            // lst.Add(itemdel);



                                                        }
                                                        else
                                                        {

                                                            break;

                                                        }

                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }

                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }

                                        }
                                        else
                                        {
                                            break;
                                        }


                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }

                        #endregion

                    }
                    else
                    {

                        return Json("Item has MRN can not be deleted.");
                    }


                }


                return Json("All the Items deleted successfully!");
            //}

            //else
            //{
            //    return Json("Number of Item Greater than 10 can not be deleted. !");
            //}


        }

    }
}