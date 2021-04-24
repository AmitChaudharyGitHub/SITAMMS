using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMS.DAL;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations;
using MMS.ViewModels;
using System.Security.Cryptography;
using System.Text;
using MMS.ViewModels.File;
using System.Data.SqlClient;
using System.Configuration;
using MMS.Helpers;
using System.Data;

namespace MMS.Models
{

    public class Procedure
    {

        MMSEntities objmms = new MMSEntities();


        public static List<T> GetData<T>(string SpName, params object[] Parameters)
        {
            List<T> data = new List<T>();
            string rawQuery = "";
            using (MMSEntities context = new MMSEntities())
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("exec " + SpName);
                    rawQuery = sb.ToString();
                    if (Parameters.Length > 0)
                    {
                        for (int i = 0; i < Parameters.Length; i++)
                        {
                            sb.Append(" {" + i + "},");
                        }
                        rawQuery = sb.ToString().Remove(sb.ToString().LastIndexOf(","), 1);
                    }
                    var qry = context.Database.SqlQuery<T>(rawQuery, Parameters);

                    data = qry.ToList();
                }
                catch (Exception ex)
                {

                    return null;
                }
            }
            return data;
        }

        public static System.Data.DataTable GetDataBySP(string SpName, Dictionary<string, object> Parameters = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Model1"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = SpName;
                    cmd.Connection = con;
                    if (Parameters != null && Parameters.Count > 0)
                    {
                        foreach (var item in Parameters)
                        {
                            cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                        }
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<DropDownListItem> GetProjectsByEmpId1(string EmpId)
        {

            List<DropDownListItem> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<DropDownListItem> lst = context.Database.SqlQuery<DropDownListItem>("EXEC GetProjectsByEmpId {0}", EmpId);
                    list = lst.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<PI_Master_Reamrks> GetAllItemStatusReport(string PI_ID, string Proj_ID)
        {
            List<PI_Master_Reamrks> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PI_Master_Reamrks> rpt = context.Database.SqlQuery<PI_Master_Reamrks>("EXEC GetPIMasterRemarks {0},{1}", PI_ID,Proj_ID);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        public List<PurchaseOrder_ReceiprtReport> GetReceiptVsPOReport(string ProjectId, string ItemGroupId, string ItemId, string VendorId, string PONo, string FromDate, string ToDate)
        {
            List<PurchaseOrder_ReceiprtReport> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PurchaseOrder_ReceiprtReport> sql = context.Database.SqlQuery<PurchaseOrder_ReceiprtReport>("EXEC GetReceiptVsPOReport {0},{1},{2},{3},{4},{5},{6}", ProjectId, ItemGroupId, ItemId, VendorId, PONo, FromDate, ToDate);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<DashBoardGrid> GetPI(string EmpId, string Status,string Stage,int PIType)
        {
            List<DashBoardGrid> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<DashBoardGrid> rpt = context.Database.SqlQuery<DashBoardGrid>("EXEC GetPIForDashboard {0},{1},{2},{3}", EmpId,Status,Stage,PIType);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<DashBoardGrid> GetPIForHO(string EmpId, string Status, string Stage, int PIType)
        {
            List<DashBoardGrid> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<DashBoardGrid> rpt = context.Database.SqlQuery<DashBoardGrid>("EXEC GetPendingPOForHO {0},{1},{2},{3}", EmpId, Status, Stage, PIType);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        public List<DashBoardGrid> GetPO(string EmpId, string Status, string Stage,string Type)
        {
            List<DashBoardGrid> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<DashBoardGrid> rpt = context.Database.SqlQuery<DashBoardGrid>("EXEC GetPOForDashboard {0},{1},{2},{3}", EmpId, Status, Stage,Type);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<POAllRemarks> GetAllPORemarks(string Proj_ID, string PO_ID)
        {
            List<POAllRemarks> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<POAllRemarks> rpt = context.Database.SqlQuery<POAllRemarks>("EXEC GetAllPORemarks {0},{1}", Proj_ID ,PO_ID);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<POAllRemarks> GetAllPORemarksGST(string Proj_ID, string PO_ID)
        {
            List<POAllRemarks> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<POAllRemarks> rpt = context.Database.SqlQuery<POAllRemarks>("EXEC GetAllPORemarks_GST {0},{1}", Proj_ID, PO_ID);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<POAllRemarks> GetAllNewPORemarks_GST(string PO_ID)
        {
            List<POAllRemarks> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<POAllRemarks> rpt = context.Database.SqlQuery<POAllRemarks>("EXEC GetAllNewRemarks_GST {0}", PO_ID);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<MSPSummary> GetMSPSummary(int m,int y,string ProjectId)
        {
            List<MSPSummary> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    context.Database.CommandTimeout= 3500;
                    DbRawSqlQuery<MSPSummary> rpt = context.Database.SqlQuery<MSPSummary>("EXEC GetMSPSummary {0},{1},{2}",m,y,ProjectId);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }
        public decimal GetTotalAmount(string ProjectId,string ItemId,decimal Qty)
        {
            
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<decimal> rpt = context.Database.SqlQuery<decimal>("EXEC GetAmountForStockByQty {0},{1},{2}", ProjectId,ItemId,Qty);
                    return rpt.FirstOrDefault();

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return new decimal(0);
                }
            }
        }

        public List<AmountDetailsViewModel> GetTotalAmountDetails(string ProjectId, string ItemId, decimal? Qty)
        {
            List<AmountDetailsViewModel> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<AmountDetailsViewModel> rpt = context.Database.SqlQuery<AmountDetailsViewModel>("EXEC GetAmountDetails {0},{1},{2}", ProjectId, ItemId, Qty);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        

        public List<AddAndApprovQuotation> GetQtDetail(string ProjectId,string PINo)
        {
            var divisionId = Repos.GetUserDivision();
            List<AddAndApprovQuotation> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<AddAndApprovQuotation> sql = context.Database.SqlQuery<AddAndApprovQuotation>("EXEC GetPIDetail_Old {0},{1},{2}", ProjectId, divisionId,PINo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<QuotDetail> GetUploadedQtDetail(string ProjectId,string PINo)
        {
            var divisionId = Repos.GetUserDivision();
            List<QuotDetail> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<QuotDetail> sql = context.Database.SqlQuery<QuotDetail>("EXEC GetQuotDetail {0},{1},{2}", ProjectId,divisionId,PINo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<QuotDetail> GetParticularQtDetail(string TrandId)
        {
            List<QuotDetail> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<QuotDetail> sql = context.Database.SqlQuery<QuotDetail>("EXEC GetQuotDetailById {0}", TrandId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PurchaseOrder_ReceiprtReport> GetPOvsReceiptReport(string ProjectId,string ItemGroupId, string ItemId, string VendorId, string PONo, string FromDate, string ToDate)
        {
            List<PurchaseOrder_ReceiprtReport> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PurchaseOrder_ReceiprtReport> sql = context.Database.SqlQuery<PurchaseOrder_ReceiprtReport>("EXEC GetPOvsReceiptReport {0},{1},{2},{3},{4},{5},{6}",ProjectId,ItemGroupId,ItemId,VendorId,PONo,FromDate,ToDate);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        

        public List<GetQuotRemarks> GetQuotRemarks(int TransId)
        {
            List<GetQuotRemarks> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetQuotRemarks> rpt = context.Database.SqlQuery<GetQuotRemarks>("EXEC GetQuotReamrks {0}",TransId);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        public List<ReleasePOViewModel> GetUnreleasePO(string ProjId)
        {
            List<ReleasePOViewModel> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<ReleasePOViewModel> rpt = context.Database.SqlQuery<ReleasePOViewModel>("EXEC GetUnreleasePO {0}", ProjId);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<ReleasePOViewModel> GetUnreleasePOGST(string ProjId, string PONumber, string VendorId)
        {
            var divisionId = Repos.GetUserDivision();
            List<ReleasePOViewModel> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<ReleasePOViewModel> rpt = context.Database.SqlQuery<ReleasePOViewModel>("EXEC GetUnreleasePO_GST {0},{1},{2},{3}", ProjId, PONumber, VendorId,divisionId);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<ReleasePOViewModel> GetReleasePO(string ProjId)
        {
            List<ReleasePOViewModel> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<ReleasePOViewModel> rpt = context.Database.SqlQuery<ReleasePOViewModel>("EXEC GetReleasePO {0}", ProjId);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }
        public List<ReleasePOViewModel> GetReleasePOGST(string ProjId, string PONumber, string VendorId)
        {
            var divisionId = Repos.GetUserDivision();
            List<ReleasePOViewModel> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<ReleasePOViewModel> rpt = context.Database.SqlQuery<ReleasePOViewModel>("EXEC GetReleasePO_GST {0},{1},{2},{3}", ProjId, PONumber, VendorId,divisionId);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }
        public List<PORemarks> GetPORemarks(string Uid)
        {
            List<PORemarks> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PORemarks> rmk = context.Database.SqlQuery<PORemarks>("EXEC GetPORemarks {0}", Uid);
                    list = rmk.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }




        public List<CartageTypeViewModel> GetCartage()
        {
            List<CartageTypeViewModel> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<CartageTypeViewModel> rpt = context.Database.SqlQuery<CartageTypeViewModel>("EXEC GetCartage");
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }






        public List<_ExciseViewModel> GetExcise()
        {
            List<_ExciseViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<_ExciseViewModel> sql = context.Database.SqlQuery<_ExciseViewModel>("EXEC GetMMSExciseDuty");
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<InsuranceViewModel> GetInsurance()
        {
            List<InsuranceViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<InsuranceViewModel> sql = context.Database.SqlQuery<InsuranceViewModel>("EXEC GetMMSInsuranceMaster");
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }
        public List<_TaxViewModel> GetTax()
        {
            List<_TaxViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<_TaxViewModel> sql = context.Database.SqlQuery<_TaxViewModel>("EXEC GetMMSTaxMaster");
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<PIdetailOnPOCreation> GetD(string Project,string PurreqNo)
        {
            List<PIdetailOnPOCreation> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PIdetailOnPOCreation> sql = context.Database.SqlQuery<PIdetailOnPOCreation>("EXEC GetPurchaseReqDetailOnPOCreate {0},{1}",Project,PurreqNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PIdetailOnPOCreation> GetDGST(string Project, string PurreqNo)
        {
            List<PIdetailOnPOCreation> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PIdetailOnPOCreation> sql = context.Database.SqlQuery<PIdetailOnPOCreation>("EXEC GetPurchaseReqDetailOnPOCreateGST {0},{1}", Project, PurreqNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PIdetailOnPOCreation> GetDOutPIType(string Project, string PurreqNo)
        {
            List<PIdetailOnPOCreation> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PIdetailOnPOCreation> sql = context.Database.SqlQuery<PIdetailOnPOCreation>("EXEC GetPurchaseReqDetailOnPOCreateForPITypeOut {0},{1}", Project, PurreqNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public string GetPO_NO_New(string ProjId, string PurchaseReqNo, DateTime Purchase_Date)
        {
            var divisionId = Repos.GetUserDivision();
            List<GetPONO_PODATEWISE> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetPONO_PODATEWISE> sl = Context.Database.SqlQuery<GetPONO_PODATEWISE>("EXEC GetPONO_By_PODateWise {0},{1},{2},{3}", ProjId , PurchaseReqNo, Purchase_Date,divisionId);
                    lst = sl.ToList();
                    return lst.First().PO_NUMBER;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        public string GetMRN(string ProjId, int SessionId)
        {
            var divisionId = Repos.GetUserDivision();
            List<GetMRN> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetMRN> sl = Context.Database.SqlQuery<GetMRN>("EXEC GetMRN {0},{1},{2}", ProjId, SessionId,divisionId);
                    lst = sl.ToList();
                    return lst.First().MRNCode;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }
        public string DeletedOpening(string ProjId, string ItemNO)
        {
            List<DeletedItemOpening> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<DeletedItemOpening> sl = Context.Database.SqlQuery<DeletedItemOpening>("EXEC DeleteOpening {0},{1}", ProjId,ItemNO);
                    lst = sl.ToList();
                    return lst.First().Success;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }
      


        public string GetMRN_New(string ProjId, int SessionId, DateTime MRNDate)
        {
            var divisionId = Repos.GetUserDivision();
            List<GetMRN> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetMRN> sl = Context.Database.SqlQuery<GetMRN>("EXEC GetMRN_New {0},{1},{2},{3}", ProjId, SessionId, MRNDate,divisionId);
                    lst = sl.ToList();
                    return lst.First().MRNCode;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<VMPODataForDisapproval> GetPODataForDisapproval(string ProjId, string PONo)
        {
            List<VMPODataForDisapproval> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<VMPODataForDisapproval> sl = Context.Database.SqlQuery<VMPODataForDisapproval>("EXEC GetPODataForDisapproval {0},{1}", ProjId, PONo);
                    lst = sl.ToList();
                    return lst;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        public string GetOut_PI(string ProjId,int OutPITypes, int sessionId, DateTime Date)
        {
            var divisionId = Repos.GetUserDivision();
            List<GetOutPI> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetOutPI> sl = Context.Database.SqlQuery<GetOutPI>("EXEC GetOutPINo {0},{1},{2},{3},{4}", ProjId,OutPITypes, sessionId, Date,divisionId);
                    lst = sl.ToList();
                    return lst.First().PICode;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public string GetManual_PI(string ProjId, int OutPITypes, int SessionId, DateTime MPIDate)
        {
            var divisionId = Repos.GetUserDivision();
            List<GetOutPI> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetOutPI> sl = Context.Database.SqlQuery<GetOutPI>("EXEC GetManual_PI {0},{1},{2},{3},{4}", ProjId, OutPITypes, SessionId,MPIDate,divisionId);
                    lst = sl.ToList();
                    return lst.First().PICode;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<PIdetailOnPOCreation> GetPOD(string Project, string PONo)
        {
            List<PIdetailOnPOCreation> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PIdetailOnPOCreation> sql = context.Database.SqlQuery<PIdetailOnPOCreation>("EXEC GetPODetailOnPONo {0},{1}", Project, PONo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }
        public List<POAndPIDetailOnPOEdit> GetPODEdit(string Project, string PONo)
        { 
            List<POAndPIDetailOnPOEdit> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<POAndPIDetailOnPOEdit> sql = context.Database.SqlQuery<POAndPIDetailOnPOEdit>("EXEC GetPODetailOnPONoEdit {0},{1}", Project, PONo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }
        public List<POAndPIDetailOnPOEdit> GetPODEditGST(string Project, string PONo)
        {
            List<POAndPIDetailOnPOEdit> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<POAndPIDetailOnPOEdit> sql = context.Database.SqlQuery<POAndPIDetailOnPOEdit>("EXEC GetPODetailOnPONoEdit_GST {0},{1}", Project, PONo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<GRNMod> GetGRNDetailOnMRN(string GRN)
        {
            List<GRNMod> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GRNMod> sql = context.Database.SqlQuery<GRNMod>("EXEC GetGateEntryDetailOnMRN {0}", GRN);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<VMItemBudgetData> GetBudgetData(string ProjectId,string GroupId,string ItemCode,string FinancialYear)
        {
            List<VMItemBudgetData> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<VMItemBudgetData> sql = context.Database.SqlQuery<VMItemBudgetData>("EXEC GetItemBudgetData {0},{1},{2},{3}",ProjectId,GroupId,ItemCode,FinancialYear);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<GRNMod> GetGRNDetailOnMRNGST(string GRN)
        {
            List<GRNMod> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GRNMod> sql = context.Database.SqlQuery<GRNMod>("EXEC GetGateEntryDetailOnMRNGST {0}", GRN);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

       

        public List<GrnGrid> GetGRnList(int GR_Id)
        {
            List<GrnGrid> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GrnGrid> rpt = context.Database.SqlQuery<GrnGrid>("EXEC GetGRNForFile {0}", GR_Id);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<GrnGrid> GetMRNList(string MRNNo)
        {
            //Note : Here using same view model as like in GetGRnList method because output type and string name are both same.
            List<GrnGrid> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GrnGrid> rpt = context.Database.SqlQuery<GrnGrid>("EXEC GetMRNItemList {0}", MRNNo);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<GateEntryReceiptVsStoreEntryReceipt> GetGateEntryVsStoreReceiptGrid(DateTime FromDate, DateTime Todate , string ProjectId)
        {
            List<GateEntryReceiptVsStoreEntryReceipt> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GateEntryReceiptVsStoreEntryReceipt> rpt = context.Database.SqlQuery<GateEntryReceiptVsStoreEntryReceipt>("EXEC GateReceiptVsStoreReceipt {0},{1},{2}", FromDate, Todate,ProjectId);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public string GetMaxRegionId()
        {
            List<GetRegionAutoId> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetRegionAutoId> sl = Context.Database.SqlQuery<GetRegionAutoId>("EXEC GetMAxRegionAutoIncrementId");
                    lst = sl.ToList();
                    return lst.First().RegionId;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }
        public List<RegionMaster> GetRegionGrid()
        {
            List<RegionMaster> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<RegionMaster> sl = Context.Database.SqlQuery<RegionMaster>("EXEC GetMMSRegion");
                    lst = sl.ToList();
                    return lst;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }



        public List<PIDetail> GetPIDetailWithPO(string ProjectId,string PI,string PO,string VendorId,string ItemGroupId,string ItemId,string FromDate,string ToDate)
        {
            List<PIDetail> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PIDetail> sql = context.Database.SqlQuery<PIDetail>("EXEC PIDetailWithPO {0},{1},{2},{3},{4},{5},{6},{7}", ProjectId, PI,PO,VendorId,ItemGroupId,ItemId,FromDate,ToDate);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PIDetailWithPoAndQtyViewModel> GetPIDetailWithPOAndQty(string ProjectId, string PI,string PO,string VendorId,string ItemGroupId,string ItemId, string fDate,string tDate)
        {
            List<PIDetailWithPoAndQtyViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PIDetailWithPoAndQtyViewModel> sql = context.Database.SqlQuery<PIDetailWithPoAndQtyViewModel>("EXEC GetPIDetailWithPOAndQty {0},{1},{2},{3},{4},{5},{6},{7}", ProjectId, PI,PO,VendorId,ItemGroupId,ItemId,fDate,tDate);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PIDetailWithPoAndQtyViewModel> GetPODataWithQty(string ProjectId, string PO, string fDate, string tDate)
        {
            List<PIDetailWithPoAndQtyViewModel> list1 = null;
            fDate = fDate == "" ? null : fDate;
            tDate = tDate == "" ? null : tDate;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PIDetailWithPoAndQtyViewModel> sql = context.Database.SqlQuery<PIDetailWithPoAndQtyViewModel>("EXEC GetPODetailWithQty {0},{1},{2},{3}", ProjectId, PO, fDate, tDate);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<DebitNoteSummaryViewModel> GetDebitNoteSummaryData(string ProjectId, int DebitNoteType, string fDate, string tDate)
        {
            List<DebitNoteSummaryViewModel> list1 = null;
            fDate = fDate == "" ? null : fDate;
            tDate = tDate == "" ? null : tDate;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<DebitNoteSummaryViewModel> sql = context.Database.SqlQuery<DebitNoteSummaryViewModel>("EXEC GetDebitNoteSummaryData {0},{1},{2},{3}", ProjectId, DebitNoteType, fDate, tDate);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<POQtyDetails> GetPOQithQtyDetail(string ProjectId, string PONo)
        {
            List<POQtyDetails> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<POQtyDetails> sql = context.Database.SqlQuery<POQtyDetails>("EXEC POQtyDetail {0},{1}", ProjectId, PONo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PoDetails> GetPODetail(string ProjectId, string PONo)
        {
            List<PoDetails> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PoDetails> sql = context.Database.SqlQuery<PoDetails>("EXEC PODetail {0},{1}", ProjectId, PONo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<POPMCDetail> GetPOPMCDetailWithItem(string ProjectId)
        {
            List<POPMCDetail> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<POPMCDetail> sql = context.Database.SqlQuery<POPMCDetail>("EXEC GetPOPMCDetail {0}", ProjectId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<StockLedgerDetail> GetStockLedgerDetail(string ProjectId, string ItemGroupId, string ItemId)
        {
            List<StockLedgerDetail> list1 = null;
            using (var context = new MMSEntities())
            {
                //try
                //{
                context.Database.CommandTimeout = 3500;
                    DbRawSqlQuery<StockLedgerDetail> sql = context.Database.SqlQuery<StockLedgerDetail>("EXEC GetStockLeadgerTest {0},{1},{2}", ProjectId,ItemGroupId,ItemId);
                    list1 = sql.ToList();
                    return list1;
                //}
                //catch (Exception ex)
                //{
                //    ex.ToString();
                //    return null;
                //}

            }
        }

        public List<GetAllVendorList> GetAllVendorListDetails()
        {
            List<GetAllVendorList> Vlist = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetAllVendorList> sql = context.Database.SqlQuery<GetAllVendorList>("EXEC GetAllVendorDetailInExcel");
                    Vlist = sql.ToList();
                    return Vlist;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<AllItemDetail> GetAllItemListDetails()
        {
            List<AllItemDetail> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<AllItemDetail> sql = context.Database.SqlQuery<AllItemDetail>("EXEC GetAllItemMasterDetail");
                    list = sql.ToList();
                    return list;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<ItemOpeningDeleteDetail> GetItemOpeningDetail(string ProjectId, string ItemGroupId, string ItemId)
        {
            List<ItemOpeningDeleteDetail> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<ItemOpeningDeleteDetail> sql = context.Database.SqlQuery<ItemOpeningDeleteDetail>("EXEC GetOpeningItemFordelete {0},{1},{2}", ProjectId, ItemGroupId, ItemId);
                    list = sql.ToList();
                    return list;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingPIGrid> GetOutPITypeGrid(string PRJID, string PIType,string EmpId)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingPIGrid> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingPIGrid> xx4 = context.Database.SqlQuery<PendingPIGrid>("EXEC GetOutFlowProc  {0},{1},{2},{3}", PRJID,PIType ,EmpId,divisionId);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }

        // added here. take changes.
      


        public List<GET_PI_ORDER> GetPIDetailWithPurchase(string ProjectId, string PurchType)
        {
            var divisionId = Repos.GetUserDivision();
            var PINO = "";
            List<GET_PI_ORDER> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_PI_ORDER> sql = context.Database.SqlQuery<GET_PI_ORDER>("EXEC GetPIDetailWithPurchase {0},{1},{2},{3}", ProjectId, Convert.ToInt32(PurchType),PINO,divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<GET_PI_ORDER> GetPartialPo_DetailWithPurchase(string ProjectId, string PurchType)
        {
            var divisionId = Repos.GetUserDivision();
            List<GET_PI_ORDER> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_PI_ORDER> sql = context.Database.SqlQuery<GET_PI_ORDER>("EXEC GetPartial_PO_DetailWithPurchaseForOuTPiType {0},{1},{2}", ProjectId, Convert.ToInt32(PurchType),divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<GET_PI_ORDER> GetUpdatedPo_DetailWithPurchase(string ProjectId, string PurchType)
        {
            var divisionId = Repos.GetUserDivision();
            var PINo = "";
            List<GET_PI_ORDER> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_PI_ORDER> sql = context.Database.SqlQuery<GET_PI_ORDER>("EXEC GetUpdated_PO_DetailWithPurchase {0},{1},{2},{3}", ProjectId, Convert.ToInt32(PurchType),PINo,divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<PendingAndPartialGRN> GetPendigGRN(string ProjectId, string PurchType)
        {
            List<PendingAndPartialGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingAndPartialGRN> sql = context.Database.SqlQuery<PendingAndPartialGRN>("EXEC GetPendingGRN {0},{1}", ProjectId, Convert.ToInt32(PurchType));
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingAndPartialGRN> GetPendigGRNGST(string ProjectId, string PurchType,string VendorId,string PONo)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingAndPartialGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    context.Database.CommandTimeout = 3500;
                    DbRawSqlQuery<PendingAndPartialGRN> sql = context.Database.SqlQuery<PendingAndPartialGRN>("EXEC GetPendingGRN_GST {0},{1},{2},{3},{4}", ProjectId, Convert.ToInt32(PurchType),VendorId,PONo,divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingAndPartialGRN> GetDisapprovedGRNGST(string ProjectId, string PurchType, string VendorId, string PONo)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingAndPartialGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    context.Database.CommandTimeout = 3500;
                    DbRawSqlQuery<PendingAndPartialGRN> sql = context.Database.SqlQuery<PendingAndPartialGRN>("EXEC GetDisapprovedMRNGST {0},{1},{2},{3},{4}", ProjectId, Convert.ToInt32(PurchType), VendorId, PONo,divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingAndPartialGRN> GetPartialGRN(string ProjectId, string PurchType)
        {
            List<PendingAndPartialGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    context.Database.CommandTimeout = 3500;
                    DbRawSqlQuery<PendingAndPartialGRN> sql = context.Database.SqlQuery<PendingAndPartialGRN>("EXEC GetPartialGRN  {0},{1}", ProjectId, Convert.ToInt32(PurchType));
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingAndPartialGRN> GetPartialGRNGST(string ProjectId, string PurchType,string VendorId,string PoNo)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingAndPartialGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    context.Database.CommandTimeout = 3500;
                    DbRawSqlQuery<PendingAndPartialGRN> sql = context.Database.SqlQuery<PendingAndPartialGRN>("EXEC GetPartialGRNGST  {0},{1},{2},{3},{4}", ProjectId, Convert.ToInt32(PurchType),VendorId,PoNo,divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<UpdatedGRN> GetUpdatedGRN(string ProjectId, string PurchType)
        {
            List<UpdatedGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    context.Database.CommandTimeout = 3500;
                    DbRawSqlQuery<UpdatedGRN> sql = context.Database.SqlQuery<UpdatedGRN>("EXEC GetCompletedGRN  {0},{1}", ProjectId, Convert.ToInt32(PurchType));
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<UpdatedGRN> GetUpdatedGRNGST(string ProjectId, string PurchType,string VendorId,string PoNo)
        {
            var divisionId = Repos.GetUserDivision();
            List<UpdatedGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    context.Database.CommandTimeout = 3500;
                    DbRawSqlQuery<UpdatedGRN> sql = context.Database.SqlQuery<UpdatedGRN>("EXEC GetCompletedGRNGST  {0},{1},{2},{3},{4}", ProjectId, Convert.ToInt32(PurchType),VendorId,PoNo,divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingMRN> GetPendingMRN(string ProjectId, string PurchType)
        {
            List<PendingMRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    context.Database.CommandTimeout = 3500;
                    DbRawSqlQuery<PendingMRN> sql = context.Database.SqlQuery<PendingMRN>("EXEC GetPendingMRN  {0},{1}", ProjectId, Convert.ToInt32(PurchType));
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingMRN> GetPendingMRNGST(string ProjectId, string PurchType)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingMRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    context.Database.CommandTimeout = 3500;
                    DbRawSqlQuery<PendingMRN> sql = context.Database.SqlQuery<PendingMRN>("EXEC GetPendingMRNGST {0},{1},{2}", ProjectId, Convert.ToInt32(PurchType),divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<UpdatedMRN> GetUpdatedMRN(string ProjectId, string PurchType)
        {
            List<UpdatedMRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    context.Database.CommandTimeout = 3500;
                    DbRawSqlQuery<UpdatedMRN> sql = context.Database.SqlQuery<UpdatedMRN>("EXEC GetUpdatedMRN  {0},{1}", ProjectId, Convert.ToInt32(PurchType));
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<UpdatedMRN> GetUpdatedMRNGST(string ProjectId, string PurchType)
        {
            var divisionId = Repos.GetUserDivision();
            List<UpdatedMRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<UpdatedMRN> sql = context.Database.SqlQuery<UpdatedMRN>("EXEC GetUpdatedMRNGST  {0},{1},{2}", ProjectId, Convert.ToInt32(PurchType),divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }
        public string GetGSTCode()
        {
            List<GetMaxGSTCode> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetMaxGSTCode> prid = Context.Database.SqlQuery<GetMaxGSTCode>("EXEC GetGSTCode");
                    lst = prid.ToList(); return lst.First().GSTCode;
                }
                catch (Exception ex)
                { ex.ToString(); return null; }
            }
        }

        public string GetGSTTaxRateCode()
        {
            List<GetMaxGSTCode> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetMaxGSTCode> prid = Context.Database.SqlQuery<GetMaxGSTCode>("EXEC GetSplitGSTRateCode");
                    lst = prid.ToList(); return lst.First().GSTCode;
                }
                catch (Exception ex)
                { ex.ToString(); return null; }
            }
        }

        public List<GSTSplit_TaxMasterViewModel> GetSplitGSTTaxMaster()
        {
            List<GSTSplit_TaxMasterViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GSTSplit_TaxMasterViewModel> sql = context.Database.SqlQuery<GSTSplit_TaxMasterViewModel>("EXEC GetGSTSplitMaster");
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<MMSGSTView> GetMMSGSTTaxMaster()
        {
            List<MMSGSTView> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<MMSGSTView> sql = context.Database.SqlQuery<MMSGSTView>("EXEC GetMMSGSTMaster");
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }
        public List<GetDetailOnNewDMR> GetDetailONNewMDR(string ProjectNo , string MRNNo)
        {
            List<GetDetailOnNewDMR> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetDetailOnNewDMR> sql = context.Database.SqlQuery<GetDetailOnNewDMR>("EXEC GetDetailON_NewDMR {0},{1}", ProjectNo, MRNNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<GetGridDMR> GetDMRGrid(string ProjectNo, string MRNNo)
        {
            List<GetGridDMR> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetGridDMR> sql = context.Database.SqlQuery<GetGridDMR>("EXEC GetGridNewDMR {0},{1}", ProjectNo, MRNNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<GetGridDMR> GetDMRGridForTransfer(string ProjectNo, string MRNNo)
        {
            List<GetGridDMR> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetGridDMR> sql = context.Database.SqlQuery<GetGridDMR>("EXEC GetDMRGridForTransfer {0},{1}", ProjectNo, MRNNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<GetProjectOpeningDateGrid> GetProjectOenigdateGrid()
        {
            List<GetProjectOpeningDateGrid> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetProjectOpeningDateGrid> sql = context.Database.SqlQuery<GetProjectOpeningDateGrid>("EXEC GetProjectOpeningDateGrid");
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<MRS> GetMRS(string MRNNo)
        {
            List<MRS> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<MRS> sql = context.Database.SqlQuery<MRS>("EXEC GetMSR {0}",MRNNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<MRS> GetMRS_TRANS_Inter(string MRNNo)
        {
            List<MRS> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<MRS> sql = context.Database.SqlQuery<MRS>("EXEC GetMSR_TRANS_Inter {0}", MRNNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<MRS> GetMRS_TRANS_Intra(string MRNNo)
        {
            List<MRS> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<MRS> sql = context.Database.SqlQuery<MRS>("EXEC GetMSR_TRANS_Intra {0}", MRNNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<GetMSRItemDetail> GetMRSItemDeyails(decimal UID)
        {
            List<GetMSRItemDetail> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetMSRItemDetail> sql = context.Database.SqlQuery<GetMSRItemDetail>("EXEC GetMRS_ItemDetails {0}", UID);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PurchaseWiseProjectLimitViewModel> GetPurchaseProjectLimitgrid(string ProjectId)
        {
            List<PurchaseWiseProjectLimitViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PurchaseWiseProjectLimitViewModel> sql = context.Database.SqlQuery<PurchaseWiseProjectLimitViewModel>("EXEC GetPurchaseLimitProjectWise {0}", ProjectId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<SupplierReportViewModel> GetSupllierAllDetail(string ProjectId, string VendorID)
        {
            List<SupplierReportViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<SupplierReportViewModel> sql = context.Database.SqlQuery<SupplierReportViewModel>("EXEC GetSupplierReport {0},{1}", ProjectId, VendorID);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }
        public List<PODetailHome> GetPODashBoard(string ProjectId)
        {
            List<PODetailHome> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PODetailHome> sql = context.Database.SqlQuery<PODetailHome>("EXEC Get_DashboardPODetail {0}", ProjectId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<MatreialRecvAndIssuViewModelHome> GetMaterialRecvAndIssueDash()
        {
            List<MatreialRecvAndIssuViewModelHome> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<MatreialRecvAndIssuViewModelHome> sql = context.Database.SqlQuery<MatreialRecvAndIssuViewModelHome>("EXEC GetMRAndIssueDetail");
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<ReceiveAndIssueDetail> GetReceiveAndIssueGraph(string ProjectId, string ItemGroupId , string ItemId)
        {
            List<ReceiveAndIssueDetail> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<ReceiveAndIssueDetail> sql = context.Database.SqlQuery<ReceiveAndIssueDetail>("EXEC GetRecevAndIssueDetail {0},{1},{2}", ProjectId, ItemGroupId, ItemId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<TotalSumAndIssuOfAllProject> GetSumOFIssueAndRecvforAllProject(string ProjectId)
        {
            List<TotalSumAndIssuOfAllProject> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<TotalSumAndIssuOfAllProject> sql = context.Database.SqlQuery<TotalSumAndIssuOfAllProject>("EXEC GetSumOfRecvAndIssueOfAllalocatedProjects {0}", ProjectId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<MovingStatusPercentageViewModel> GetMovingStatusPercentage(string ProjectId ,string ItemGroupId , string Status)
        {
            List<MovingStatusPercentageViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<MovingStatusPercentageViewModel> sql = context.Database.SqlQuery<MovingStatusPercentageViewModel>("EXEC GetMovingStatusPercentage {0},{1},{2}", ProjectId, ItemGroupId, Status);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }
        public List<ItemWiseMaxMinRateViewModel> GetMaxMINRateHome(string ProjectId , string ItemGrpId)
        {
            List<ItemWiseMaxMinRateViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<ItemWiseMaxMinRateViewModel> sql = context.Database.SqlQuery<ItemWiseMaxMinRateViewModel>("EXEC GetAllItemRateGroupAndProjectWise {0},{1}",ProjectId,ItemGrpId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public string GetInterStateTransferCode(string ProjectId, string TransferType, DateTime TransferDate)
        {
            var divisionId = Repos.GetUserDivision();
            List<InterStateTransferCode> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<InterStateTransferCode> sql = context.Database.SqlQuery<InterStateTransferCode>("EXEC uspGetInterStateTranferCodeNewFormat {0},{1},{2},{3}", ProjectId, TransferType,TransferDate,divisionId);
                    list1 = sql.ToList();
                    return list1.First().TransferCode;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public string GetoutTransferCode(string ProjectId, int TransferType, DateTime GateOutDate)
        {
            var divisionId = Repos.GetUserDivision();
            List<GetOutCode> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetOutCode> sql = context.Database.SqlQuery<GetOutCode>("EXEC CeateGetOutNumber {0},{1},{2},{3}", ProjectId, TransferType, GateOutDate,divisionId);
                    list1 = sql.ToList();
                    return list1.First().GetOutTransferCode;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }



        public List<IntraTransferMasterDetail> GetIntraTransferProjectDetail(string ProjectId, string TransferNo)
        {
            List<IntraTransferMasterDetail> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<IntraTransferMasterDetail> sql = context.Database.SqlQuery<IntraTransferMasterDetail>("EXEC GETdetailOFIntraTransfer {0},{1}", ProjectId, TransferNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<InterStateTransferMasterDetailOnGetOut> GetInterTransferProjectDetail(string ProjectId, string TransferNo)
        {
            List<InterStateTransferMasterDetailOnGetOut> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<InterStateTransferMasterDetailOnGetOut> sql = context.Database.SqlQuery<InterStateTransferMasterDetailOnGetOut>("EXEC GETdetailOFInterTransfer {0},{1}", ProjectId, TransferNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<PendingInterSiteTransfer> GetInterStateTransferList(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingInterSiteTransfer> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingInterSiteTransfer> sql = context.Database.SqlQuery<PendingInterSiteTransfer>("EXEC GetIntersatetTransferGrid {0},{1}", ProjectId,divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingInterSiteTransfer> GetApprovedInterStateTransferList(string ProjectId)
        {
            List<PendingInterSiteTransfer> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingInterSiteTransfer> sql = context.Database.SqlQuery<PendingInterSiteTransfer>("EXEC GetApprovedIntersatetTransferGrid {0}", ProjectId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingInterSiteTransfer> GetApprovedInterStateTransferList_BeforGateOut(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingInterSiteTransfer> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingInterSiteTransfer> sql = context.Database.SqlQuery<PendingInterSiteTransfer>("EXEC GetApprovedIntersatetTransferGrid_BeforeGateout {0},{1}", ProjectId,divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<PendingIntraStateTransfer> GetIntraStateTransferList(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingIntraStateTransfer> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingIntraStateTransfer> sql = context.Database.SqlQuery<PendingIntraStateTransfer>("EXEC GetIntraStateTransferList {0},{1}", ProjectId,divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<GetIntraStateTransferTaxableList> GetIntraStateTransferTaxableList(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<GetIntraStateTransferTaxableList> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetIntraStateTransferTaxableList> sql = context.Database.SqlQuery<GetIntraStateTransferTaxableList>("EXEC GetIntraStateTransferTaxableList {0},{1}", ProjectId,divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<GetIntraStateTransferTaxableList> GetUpdateIntraStateTransferTaxableList(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<GetIntraStateTransferTaxableList> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetIntraStateTransferTaxableList> sql = context.Database.SqlQuery<GetIntraStateTransferTaxableList>("EXEC GetUpdatedIntraStateTransferTaxableListBeforeGetout {0},{1}", ProjectId,divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<GetIntraStateTransferTaxableList> GetPendingIntraStateTransferTaxableList(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<GetIntraStateTransferTaxableList> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetIntraStateTransferTaxableList> sql = context.Database.SqlQuery<GetIntraStateTransferTaxableList>("EXEC GetUpdatedIntraStateTransferTaxableList {0},{1}", ProjectId,divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }



        public List<GetIntraStateTransferTaxableList> GetUpdateGetoutIntraStateTransferTaxableList(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<GetIntraStateTransferTaxableList> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetIntraStateTransferTaxableList> sql = context.Database.SqlQuery<GetIntraStateTransferTaxableList>("EXEC GetUpdatedGetOutIntraStateTransferTaxableList {0},{1}", ProjectId,divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<IntraStateTransferTaxableChildViewModel> GetIntraStateTransferTaxableChildList(decimal MtrandId)
        {
            List<IntraStateTransferTaxableChildViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<IntraStateTransferTaxableChildViewModel> sql = context.Database.SqlQuery<IntraStateTransferTaxableChildViewModel>("EXEC GetIntraTransferTaxableChildList {0}", MtrandId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<GetOutItemDetailViewModel> GetOutTransferItemdetail(string TransferNo, string ProjectId)
        {
            List<GetOutItemDetailViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetOutItemDetailViewModel> sql = context.Database.SqlQuery<GetOutItemDetailViewModel>("EXEC GetGetoutItemOfIntraTransfer {0},{1}", TransferNo, ProjectId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<GetOutItemDetailViewModel> GetOutIntraTransferItemdetail(string TransferNo, string ProjectId)
        {
            List<GetOutItemDetailViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetOutItemDetailViewModel> sql = context.Database.SqlQuery<GetOutItemDetailViewModel>("EXEC GetGetoutItemOfInterTransfer {0},{1}", TransferNo, ProjectId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        //change25052018
        public List<GetOutItemDetailViewModel> GetVendorDebitNoteItemsForTransfer(string Uid)
        {
            List<GetOutItemDetailViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetOutItemDetailViewModel> sql = context.Database.SqlQuery<GetOutItemDetailViewModel>("EXEC GetVendorDebitNoteItemsForTransfer {0}", Uid);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<InterStateTransferChildPrintViewModel> Get_InterItemDetailOnPrint(decimal MtrandId)
        {
            List<InterStateTransferChildPrintViewModel> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<InterStateTransferChildPrintViewModel> sql = context.Database.SqlQuery<InterStateTransferChildPrintViewModel>("EXEC GetGetoutItemOfInterTransfer_Print {0}", MtrandId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingInterSiteTransfer> GetUpdated_GetoutInterStateTransferList(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingInterSiteTransfer> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingInterSiteTransfer> sql = context.Database.SqlQuery<PendingInterSiteTransfer>("EXEC GetUpdatedGetoutIntersatetTransferGrid {0},{1}", ProjectId,divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }
        public List<PendingInterSiteTransfer> GetPending_InterStateTransferListONGateOut(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingInterSiteTransfer> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingInterSiteTransfer> sql = context.Database.SqlQuery<PendingInterSiteTransfer>("EXEC GetPending_InterTransferListDetailAtGateOut {0},{1}", ProjectId, divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<TransferItemDetailGRN> Get_TransferItemdetailONGRN(string TransferNo)
        {
            List<TransferItemDetailGRN> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<TransferItemDetailGRN> sql = context.Database.SqlQuery<TransferItemDetailGRN>("EXEC GetItemDetailOfTRanferItemONGRN {0}", TransferNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<TransferItemDetailGRN> Get_TransferItemdetailONGRN_Inter(string TransferNo)
        {
            List<TransferItemDetailGRN> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<TransferItemDetailGRN> sql = context.Database.SqlQuery<TransferItemDetailGRN>("EXEC GetItemDetailOfTRanferItemONGRN_Inter {0}", TransferNo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<Controllers.Print_Slip_For_ApprovalController.Getdata> Get_PendingMannualPO(string ProjectId, string CreatedBy)
        {
            List<Controllers.Print_Slip_For_ApprovalController.Getdata> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<Controllers.Print_Slip_For_ApprovalController.Getdata> sql = context.Database.SqlQuery<Controllers.Print_Slip_For_ApprovalController.Getdata>("EXEC GetPendingMannualPurchaseType {0},{1}", ProjectId, CreatedBy);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<Controllers.Print_Slip_For_ApprovalController.Getdata> Get_UpdatedMannualPO(string ProjectId, string CreatedBy)
        {
            List<Controllers.Print_Slip_For_ApprovalController.Getdata> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<Controllers.Print_Slip_For_ApprovalController.Getdata> sql = context.Database.SqlQuery<Controllers.Print_Slip_For_ApprovalController.Getdata>("EXEC GetUpdatedMannualPurchaseType {0},{1}", ProjectId, CreatedBy);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingTransferMRN> GetPendingMRNTRN(string ProjectId, string PurchType)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingTransferMRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingTransferMRN> sql = context.Database.SqlQuery<PendingTransferMRN>("EXEC GetPendingTRN {0},{1},{2}", ProjectId, Convert.ToInt32(PurchType),divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<UpdateMRNTRN> GetUpdatedMRNGST_TRN(string ProjectId, string PurchType)
        {
            var divisionId = Repos.GetUserDivision();
            List<UpdateMRNTRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<UpdateMRNTRN> sql = context.Database.SqlQuery<UpdateMRNTRN>("EXEC GetUpdatedMRN_TRN  {0},{1},{2}", ProjectId, Convert.ToInt32(PurchType),divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<GRNMod> GetGRNDetailOnMRNGSTForTRNS(string GRN, string PType)
        {
            List<GRNMod> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GRNMod> sql = context.Database.SqlQuery<GRNMod>("EXEC GetGateEntryDetailOnMRNGSTForTRNS {0},{1}", GRN, PType);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }


        public List<PendingTRansferGRN> GetPendigTRansferGRNList(string ProjectId, string PurchType)
        {
            List<PendingTRansferGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingTRansferGRN> sql = context.Database.SqlQuery<PendingTRansferGRN>("EXEC GetTransferPendingGRN {0},{1}", ProjectId, Convert.ToInt32(PurchType));
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingTRansferGRN> GetPendigTRansferGRNList_Inter(string ProjectId, string PurchType,string VendorId,string PONo)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingTRansferGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingTRansferGRN> sql = context.Database.SqlQuery<PendingTRansferGRN>("EXEC GetTransferPendingGRN_Inter {0},{1},{2},{3},{4}", ProjectId, Convert.ToInt32(PurchType),VendorId,PONo,divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingTRansferGRN> GetPartialTRansferGRNList_Inter(string ProjectId, string PurchType, string VendorId, string PONo)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingTRansferGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingTRansferGRN> sql = context.Database.SqlQuery<PendingTRansferGRN>("EXEC GetTransferPartialGRN_Inter {0},{1},{2},{3},{4}", ProjectId, Convert.ToInt32(PurchType),VendorId,PONo,divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingTRansferGRN> GetPendigTRansferGRNList_Intra(string ProjectId, string PurchType,string VendorId,string PONo)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingTRansferGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingTRansferGRN> sql = context.Database.SqlQuery<PendingTRansferGRN>("EXEC GetTransferPendingGRN_Intra {0},{1},{2},{3},{4}", ProjectId, Convert.ToInt32(PurchType), VendorId, PONo,divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingTRansferGRN> GetPartialTRansferGRNList_Intra(string ProjectId, string PurchType,string VendorId,string PONo)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingTRansferGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingTRansferGRN> sql = context.Database.SqlQuery<PendingTRansferGRN>("EXEC GetTransferPartialGRN_Intra {0},{1},{2},{3},{4}", ProjectId, Convert.ToInt32(PurchType),VendorId,PONo,divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }
        public List<PendingTRansferGRN> GetUpdatedTRansferGRNList_Intra(string ProjectId, string PurchType,string VendorId,string PONo)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingTRansferGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingTRansferGRN> sql = context.Database.SqlQuery<PendingTRansferGRN>("EXEC GetTransferUpdatedGRN_Intra {0},{1},{2},{3},{4}", ProjectId, Convert.ToInt32(PurchType),VendorId,PONo,divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }
        public List<PendingTRansferGRN> GetUpdatedTRansferGRNList_Inter(string ProjectId, string PurchType,string VendorId,string PONo)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingTRansferGRN> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingTRansferGRN> sql = context.Database.SqlQuery<PendingTRansferGRN>("EXEC GetTransferUpdatedGRN_Inter {0},{1},{2},{3},{4}", ProjectId, Convert.ToInt32(PurchType),VendorId,PONo,divisionId);
                    list2 = sql.ToList();
                    return list2;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<GetPOVettingReport> GetAllPOVettedReport(Nullable<int> m, Nullable<int> y, string EmpId)
        {

            List<GetPOVettingReport> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {

                    DbRawSqlQuery<GetPOVettingReport> xx4 = context.Database.SqlQuery<GetPOVettingReport>("EXEC GetPOVettingReport {0} ,{1},{2}", m, y, EmpId);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }


        public string GetDebitNote(string ProjId, DateTime DebitDate, string DebitType)
        {
            var divisionId = Repos.GetUserDivision();
            List<DebitNumber> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<DebitNumber> sl = Context.Database.SqlQuery<DebitNumber>("EXEC GetMMSDebit_Note_code {0},{1},{2},{3}", ProjId, DebitDate, DebitType,divisionId);
                    lst = sl.ToList();
                    return lst.First().DebitNote_Code;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        //change25042018_Report
        public List<GetTransferDataForReport> GetTransferData(string ProjectId, string TransferProjectId, string ItemGroupId, string ItemId, string TransferType,string fDate,string tDate)
        {
            List<GetTransferDataForReport> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    fDate = fDate == "" ? null : fDate;
                    tDate = tDate == "" ? null : tDate;
                    DbRawSqlQuery<GetTransferDataForReport> rpt = context.Database.SqlQuery<GetTransferDataForReport>("EXEC GetTransferData {0},{1},{2},{3},{4},{5},{6}", ProjectId, TransferProjectId, ItemGroupId, ItemId, TransferType,fDate,tDate);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }

        }


        public List<GetTransferDataForReceiptReport> GetTransferDataForReceipt(string ProjectId, string TransferProjectId, string ItemGroupId, string ItemId, string TransferType,string fDate,string tDate)
        {
            List<GetTransferDataForReceiptReport> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    fDate=fDate == "" ? null : fDate;
                    tDate=tDate == "" ? null : tDate;
                    DbRawSqlQuery<GetTransferDataForReceiptReport> rpt = context.Database.SqlQuery<GetTransferDataForReceiptReport>("EXEC GetTransferDataForReceipt {0},{1},{2},{3},{4},{5},{6}", ProjectId, TransferProjectId, ItemGroupId, ItemId, TransferType,fDate,tDate);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }

        }


        //change25042018_Report
        public List<GetTransferProjects> GetInterStateTransferProjects(string ProjectId)
        {
            List<GetTransferProjects> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetTransferProjects> rpt = context.Database.SqlQuery<GetTransferProjects>("EXEC GetInterStateTransferProjects {0}",ProjectId);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        //change25042018_Report
        public List<GetTransferProjects> GetIntraStateTransferProjects(string ProjectId)
        {
            List<GetTransferProjects> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetTransferProjects> rpt = context.Database.SqlQuery<GetTransferProjects>("EXEC GetIntraStateTransferProjects {0}",ProjectId);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<GetTransferProjects> GetTransferProjectsForReceipt(string ProjectId,string TransferType)
        {
            List<GetTransferProjects> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetTransferProjects> rpt = context.Database.SqlQuery<GetTransferProjects>("EXEC GetTransferProjectsForReceipt {0},{1}",ProjectId,TransferType);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        
        //change25042018_Report
        public List<GetItemGroups> GetItemGroupsForTransfer(string TransferProjectId)
        {
            List<GetItemGroups> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetItemGroups> rpt = context.Database.SqlQuery<GetItemGroups>("EXEC GetItemGroupsForTransfer {0}", TransferProjectId);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        public List<ItemWiseRateReport> GetItemWiseRateHistory(string ProjectId, string ItemGroup, string Itemid, string FromDate, string ToDate)
        {
            List<ItemWiseRateReport> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<ItemWiseRateReport> rpt = context.Database.SqlQuery<ItemWiseRateReport>("EXEC GetItemWiseRateHistory {0},{1},{2},{3},{4}",ProjectId,ItemGroup,Itemid,FromDate,ToDate);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<GetPOAndChallanDetailsModel> GetPOAndChallanDetails(string ProjectId, string MRNNo, string SupplierNo, string PONo)
        {
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetPOAndChallanDetailsModel> rpt = context.Database.SqlQuery<GetPOAndChallanDetailsModel>("EXEC GetPOAndChallanDetails {0},{1},{2},{3}", ProjectId, MRNNo, SupplierNo, PONo);
                    return rpt.ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

      
        public List<GetPOAndChallanDetailsModel> GetChallanNoAndDateByPONo(string PONo,string PRJID,string GRNNo)
        {
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetPOAndChallanDetailsModel> rpt = context.Database.SqlQuery<GetPOAndChallanDetailsModel>("EXEC GetChallanNoAndDateByPONo {0},{1},{2}",PONo,PRJID,GRNNo);
                    return rpt.ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        public List<VendorSearchItem> GetVendorItemSearch(string ProjectId, string MRNNo, string SupplierNo, string PONo)
        {
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<VendorSearchItem> rpt = context.Database.SqlQuery<VendorSearchItem>("EXEC GetVendorItemSearch {0},{1},{2},{3}", ProjectId, MRNNo, SupplierNo, PONo);
                    return rpt.ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<GetItemsList> SearchItems(string ItemGroupId, string ItemName, string HSNCode, string UnitId)
        {
            List<GetItemsList> list = null;
            string gId = string.IsNullOrEmpty(ItemGroupId) ? null : ItemGroupId;
            string itemName = string.IsNullOrEmpty(ItemName) ? null : ItemName;
            string hsn = string.IsNullOrEmpty(HSNCode) ? null : HSNCode;
            string unitId = string.IsNullOrEmpty(UnitId) ? null : UnitId;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetItemsList> rpt = context.Database.SqlQuery<GetItemsList>("EXEC SearchItems {0},{1},{2},{3}", gId, itemName, hsn, unitId);
                    list = rpt.ToList();
                    return list;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }
        //change17052018
        public List<GetVendorList> SearchVendors(string projectId, string vendorName, string vendorGroup, string state, string city, string blackListStatus)
        {
            List<GetVendorList> list = null;
            string pId = string.IsNullOrEmpty(projectId) ? null : projectId;
            string vName = string.IsNullOrEmpty(vendorName) ? null : vendorName;
            string VGroup = string.IsNullOrEmpty(vendorGroup) ? null : vendorGroup;
            string _state = string.IsNullOrEmpty(state) ? null : state;
            string _city = city == "0" ? null : city;
            string status = string.IsNullOrEmpty(blackListStatus) ? null : blackListStatus;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetVendorList> rpt = context.Database.SqlQuery<GetVendorList>("EXEC SearchVendors {0},{1},{2},{3},{4},{5}", pId, vName, VGroup, _state,_city,status);
                    list = rpt.ToList();
                    return list;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public string GetVendorDebitNote(string ProjId, DateTime DebitDate)
        {
            var divisionId = Repos.GetUserDivision();
            List<DebitNumber> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<DebitNumber> sl = Context.Database.SqlQuery<DebitNumber>("EXEC GetMMSVendorDebit_Note_code {0},{1},{2}", ProjId, DebitDate,divisionId);
                    lst = sl.ToList();
                    return lst.First().DebitNote_Code;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        public string GetBillingAddressForPO(string ProjectId)
        {
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<string> rpt = context.Database.SqlQuery<string>("EXEC GetBillingAddressForPO {0}", ProjectId);
                    return rpt.First();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<GetMRN> GeMRNForPONo(string ProjectId, string VendorId, string PONumber)
        {
            List<GetMRN> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetMRN> sl = Context.Database.SqlQuery<GetMRN>("Exec GetMRNForDebitNote {0},{1},{2}", ProjectId, VendorId, PONumber);
                    lst = sl.ToList();
                    return lst;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<GetVendorDebitNotes> GetVendorDebitNotes(string ProjectId, string Status)
        {
            var divisionId = Repos.GetUserDivision();
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetVendorDebitNotes> rpt = context.Database.SqlQuery<GetVendorDebitNotes>("EXEC GetVendorDebitNotes {0},{1},{2}", ProjectId, Status,divisionId);
                    return rpt.ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<GetVendorDebitNotes> GetVendorDebitNotesForGate(string ProjectId, string Status)
        {
            var divisionId = Repos.GetUserDivision();
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetVendorDebitNotes> rpt = context.Database.SqlQuery<GetVendorDebitNotes>("EXEC GetVendorDebitNotesForGate {0},{1},{2}", ProjectId, Status,divisionId);
                    return rpt.ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        
        public List<VendorSearchItem> GetVendorItemByDebitCode(string DebitCode)
        {
            List<VendorSearchItem> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<VendorSearchItem> sl = Context.Database.SqlQuery<VendorSearchItem>("Exec GetVendorItemByDebitCode {0}", DebitCode);
                    lst = sl.ToList();
                    return lst;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        public List<PurchaseRequisitionChildViewModel> GetPurchaseIndentItemDetails(decimal PUid)
        {
            List<PurchaseRequisitionChildViewModel> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PurchaseRequisitionChildViewModel> sl = Context.Database.SqlQuery<PurchaseRequisitionChildViewModel>("Exec GetPurchaseIndentItemDetails {0}", PUid);
                    lst = sl.ToList();
                    return lst;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        #region All Dashboard grid bind method on 3_3_2020
        public List<DashBoardGrid> GetPO(string EmpId, string Status, string Stage, string Type, string procedure)
        {
            List<DashBoardGrid> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<DashBoardGrid> rpt = context.Database.SqlQuery<DashBoardGrid>("EXEC {4} {0},{1},{2},{3}", EmpId, Status, Stage, Type, procedure);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }
        #endregion

        public int IndentID_Exists(string projectID, string sessioncode, int divisionId) {
            string contstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConStr"].ToString();
            SqlConnection sqlConnection = new SqlConnection(contstr);
            int res = 0;
            try
            {
                string cmdText = "select top 1 * from tblIndentRequisition where ProjectID=@projectID and SessionId=@sesID and DivisionId=@divID";
                SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.Text;
                sqlCommand.Parameters.AddWithValue("@projectID", projectID);
                sqlCommand.Parameters.AddWithValue("@sesID", sessioncode);
                sqlCommand.Parameters.AddWithValue("@divID", divisionId);
                SqlDataAdapter sda = new SqlDataAdapter(sqlCommand);
                System.Data.DataTable dataTable = new System.Data.DataTable();
                sda.Fill(dataTable);
                res = dataTable.Rows.Count;
            }
            catch (Exception ex)
            {

            }
            finally {
                sqlConnection.Close();
            }

            return res;
        }

        #region save issuerequisition by sql command code on 12

        public tblIndentRequisition GetNewIndentNo(string projectid, string session, int? division, SqlTransaction sqlTransaction, SqlConnection con) {
            tblIndentRequisition indentObj = new tblIndentRequisition();
            string contstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Model1"].ToString();
            SqlConnection sqlConnection = new SqlConnection(contstr);
            sqlConnection.Open();
            try
            {
                SqlCommand sqlCommand = new SqlCommand("usp_GetNewIndentNo", con, sqlTransaction);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@projectid", projectid);
                sqlCommand.Parameters.AddWithValue("@sessioncode", session);
                sqlCommand.Parameters.AddWithValue("@divisonid", division);
                sqlCommand.Parameters.Add("@MaxID", System.Data.SqlDbType.Int);
                sqlCommand.Parameters["@MaxID"].Direction = System.Data.ParameterDirection.Output;
                sqlCommand.Parameters.Add("@indentNo", System.Data.SqlDbType.VarChar, -1);
                sqlCommand.Parameters["@indentNo"].Direction = System.Data.ParameterDirection.Output;
                sqlCommand.ExecuteNonQuery();
                indentObj.AutoUniID = Convert.ToInt32(sqlCommand.Parameters["@MaxID"].Value);
                indentObj.IndentNo = sqlCommand.Parameters["@indentNo"].Value.ToString();
            }
            catch (Exception ex)
            {
            }
            //finally {
            //   // sqlConnection.Close();
            //}

            return indentObj;
        }


        public object Save_IndentRequisition(List<tblIndentRequisition> paraObjs) {
            int res = 0;
            string indentno = "";
            string contstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Model1"].ToString();   
            SqlConnection sqlConnection = new SqlConnection(contstr);
            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            tblIndentRequisition indentnoObj = new tblIndentRequisition();
            try
            {
                indentnoObj = GetNewIndentNo(paraObjs[0].ProjectID, paraObjs[0].SessionId, paraObjs[0].DivisionId, sqlTransaction, sqlConnection);
                foreach (tblIndentRequisition paraObj in paraObjs) {
                    SqlCommand cmd = new SqlCommand("usp_SaveIndentRequisition", sqlConnection, sqlTransaction);
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = "usp_SaveIndentRequisition";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectID", paraObj.ProjectID);
                    cmd.Parameters.AddWithValue("@indentno", indentnoObj.IndentNo);
                    cmd.Parameters.AddWithValue("@EmployeeID", paraObj.EmployeeID);
                    cmd.Parameters.AddWithValue("@ItemGroupID", paraObj.ItemGroupID);
                    cmd.Parameters.AddWithValue("@ItemID", paraObj.ItemID);
                    cmd.Parameters.AddWithValue("@Make", paraObj.Make);
                    cmd.Parameters.AddWithValue("@UnitID", paraObj.UnitID);
                    cmd.Parameters.AddWithValue("@PartNo", paraObj.PartNo);
                    cmd.Parameters.AddWithValue("@Quantity", paraObj.Quantity);
                    cmd.Parameters.AddWithValue("@IssueQuantity", paraObj.IssueQuantity);
                    cmd.Parameters.AddWithValue("@SessionId", paraObj.SessionId);
                    cmd.Parameters.AddWithValue("@SiteId", paraObj.SiteId);
                    cmd.Parameters.AddWithValue("@IndentDate", paraObj.IndentDate);
                    cmd.Parameters.AddWithValue("@createdDate", paraObj.CreatedDate);
                    cmd.Parameters.AddWithValue("@AutoUniID", indentnoObj.AutoUniID);
                    cmd.Parameters.AddWithValue("@ProjectName", paraObj.ProjectName);
                    cmd.Parameters.AddWithValue("@EmployeeName", paraObj.EmployeeName);
                    cmd.Parameters.AddWithValue("@ItemGroupName", paraObj.ItemGroupName);
                    cmd.Parameters.AddWithValue("@ItemName", paraObj.ItemName);
                    cmd.Parameters.AddWithValue("@CreatedBy", paraObj.CreatedBy);
                    cmd.Parameters.AddWithValue("@IsRead", paraObj.IsRead);
                    cmd.Parameters.AddWithValue("@IndentType", paraObj.IndentType);
                    cmd.Parameters.AddWithValue("@IndentSent", paraObj.IndentSent);
                    cmd.Parameters.AddWithValue("@IndentOtherDesc", paraObj.IndentOtherDesc);
                    cmd.Parameters.AddWithValue("@Approved_Quantity", paraObj.Approved_Quantity);
                    cmd.Parameters.AddWithValue("@Apporoved_By", paraObj.Apporoved_By);
                    cmd.Parameters.AddWithValue("@Apporoved_Status", paraObj.Apporoved_Status);
                    cmd.Parameters.AddWithValue("@Status", paraObj.Status);
                    cmd.Parameters.AddWithValue("@BalanceQuantity", paraObj.BalanceQuantity);
                    cmd.Parameters.AddWithValue("@BalanceApprovedQty_After_Issue_Qty", paraObj.BalanceApprovedQty_After_Issue_Qty);
                    cmd.Parameters.AddWithValue("@DivisionId", paraObj.DivisionId);
                    cmd.Parameters.Add("@reqstatus", System.Data.SqlDbType.Int);
                    cmd.Parameters["@reqstatus"].Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add("@New_IndentNo", System.Data.SqlDbType.VarChar, -1);
                    cmd.Parameters["@New_IndentNo"].Direction = System.Data.ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    res = Convert.ToInt32(cmd.Parameters["@reqstatus"].Value);
                    indentno = cmd.Parameters["@New_IndentNo"].Value.ToString();
                }   
                sqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                indentno = ex.Message;
            }
            finally {
                sqlConnection.Close();
            }

            return new { Result = res, TansNo = indentno };
        }


        public object Save_IndentRequisitionTable(DataTable IndentReqData)
        {
            int res = 0;
            string indentno = "";
            string contstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Model1"].ToString();
            SqlConnection sqlConnection = new SqlConnection(contstr);
            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            tblIndentRequisition indentnoObj = new tblIndentRequisition();
            try
            {
                // indentnoObj = GetNewIndentNo(IndentReqData.Rows[0]["ProjectID"], IndentReqData.Rows[0]["SessionId"], paraObjs[0].DivisionId, sqlTransaction, sqlConnection);
                SqlCommand cmd = new SqlCommand("usp_SaveIndentRequisitionTable", sqlConnection, sqlTransaction);
                cmd.CommandTimeout = 300;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IndentReqData", IndentReqData);
                cmd.Parameters.AddWithValue("@ProjectID", IndentReqData.Rows[0]["ProjectID"].ToString());
                cmd.Parameters.AddWithValue("@SessionId", IndentReqData.Rows[0]["SessionId"].ToString());
                cmd.Parameters.AddWithValue("@DivisionId", IndentReqData.Rows[0]["DivisionId"].ToString());
                cmd.Parameters.Add("@reqstatus", System.Data.SqlDbType.Int);
                cmd.Parameters["@reqstatus"].Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add("@New_IndentNo", System.Data.SqlDbType.VarChar, -1);
                cmd.Parameters["@New_IndentNo"].Direction = System.Data.ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                res = Convert.ToInt32(cmd.Parameters["@reqstatus"].Value);
                indentno = cmd.Parameters["@New_IndentNo"].Value.ToString();
                sqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                indentno = ex.Message;
            }
            finally
            {
                sqlConnection.Close();
            }

            return new { Result = res, TansNo = indentno };
        }

        #endregion


        #region Search Items for Issue and Indent Requisition created on 19_05_20

        public List<GetItemsList> SearchEnableItems(string ItemGroupId, string ItemName, string HSNCode, string UnitId)
        {
            List<GetItemsList> list = null;
            string gId = string.IsNullOrEmpty(ItemGroupId) ? null : ItemGroupId;
            string itemName = string.IsNullOrEmpty(ItemName) ? null : ItemName;
            string hsn = string.IsNullOrEmpty(HSNCode) ? null : HSNCode;
            string unitId = string.IsNullOrEmpty(UnitId) ? null : UnitId;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetItemsList> rpt = context.Database.SqlQuery<GetItemsList>("EXEC usp_SearchItemsWithEnable {0},{1},{2},{3}", gId, itemName, hsn, unitId);
                    list = rpt.ToList();
                    return list;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        #endregion


        #region get project budget year and month data
        public List<YearBudgetItems> GetProjectYearBudgetData(string projectid, string itemgroup, int fromyear, int toyear)
        {
            using (var context = new MMSEntities())
            {
                List<YearBudgetItems> list;
                try
                {
                    DbRawSqlQuery<YearBudgetItems> rpt = context.Database.SqlQuery<YearBudgetItems>("EXEC usp_GetProjectYearBudget {0},{1},{2},{3}", projectid, itemgroup, fromyear, toyear);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        public List<YearBudgetItems> GetProjectMonthBudgetData(string projectid, int year, string itemid)
        {
            using (var context = new MMSEntities())
            {
                List<YearBudgetItems> list;
                try
                {
                    DbRawSqlQuery<YearBudgetItems> rpt = context.Database.SqlQuery<YearBudgetItems>("EXEC usp_GetProjectMonthBudget {0},{1},{2}", projectid, year, itemid);
                    list = rpt.ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }

        #endregion


        #region PO Draft
        public List<POAndPIDetailOnPOEdit> GetDraftPODEditGST(string Project, string PONo)
        {
            List<POAndPIDetailOnPOEdit> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<POAndPIDetailOnPOEdit> sql = context.Database.SqlQuery<POAndPIDetailOnPOEdit>("EXEC GetDraftPODetailOnPONoEdit_GST {0},{1}", Project, PONo);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }



        #endregion

        #region rejected Tranfer
        public List<GetIntraStateTransferTaxableList> GetRejectedIntraTransfer(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<GetIntraStateTransferTaxableList> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetIntraStateTransferTaxableList> sql = context.Database.SqlQuery<GetIntraStateTransferTaxableList>("EXEC GetRejectIntraTransfer {0},{1}", ProjectId, divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        public List<PendingInterSiteTransfer> GetRejectedInterTransfer(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<PendingInterSiteTransfer> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingInterSiteTransfer> sql = context.Database.SqlQuery<PendingInterSiteTransfer>("EXEC GetRejectInterTransfer {0},{1}", ProjectId, divisionId);
                    list1 = sql.ToList();
                    return list1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }

        #endregion

    }

    public class DebitNoteSummaryViewModel
    {
        public int TransId { get; set; }
        public string VendorPCName { get; set; }
        public string DebitNoteNo { get; set; }
        public string DebitNoteDate { get; set; }
        public Nullable<decimal> Amount { get; set; }

    }

    public class PIDetailWithPoAndQtyViewModel
    {
        public int TransId { get; set; }
        public string PurRequisitionNo { get; set; }
        public string PIDate { get; set; }
        public string ItemGroup { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> PIQty { get; set; }
        public Nullable<decimal> ApprovePIQty { get; set; }
        public string PONo { get; set; }
        public Nullable<decimal> POQty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string PODate { get; set; }

        public Nullable<decimal> ReceivedQty { get; set; }
        public Nullable<decimal> BalanceQty { get; set; }
        public string MRNNo { get; set; }
        public string MRNDate { get; set; }

        public string VendorName { get; set; }
        public Nullable<decimal> ReceiveAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }

    }


    public class PurchaseRequisitionChildViewModel
    {
        public decimal UID { get; set; }
        public string IndentNo { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public string ItemGroupName { get; set; }
        public string HSNCode { get; set; }
        public Nullable<decimal> DemandQty { get; set; }
        public Nullable<decimal> ApprovedQty { get; set; }
        public Nullable<decimal> OrderedQty { get; set; }
        public Nullable<decimal> LastPurchaseRate { get; set; }
        public Nullable<decimal> CurrentRate { get; set; }
        public Nullable<decimal> CurrentValue { get; set; }
        public Nullable<decimal> ApprovedQtyPIC { get; set; }
        public string PICStatus { get; set; }
        public string MangStatus { get; set; }
        public Nullable<decimal> ApprovedQtyMang { get; set; }
        public Nullable<decimal> PICCurrentValue { get; set; }
        public Nullable<decimal> MangCurrentValue { get; set; }
        public Nullable<decimal> ReceiveUpto { get; set; }
        public Nullable<decimal> Balanced { get; set; }

        public string Remarks { get; set; }
    }



    public class AmountDetailsViewModel
    {
        public int SNo { get; set; }
        public decimal StockBalQty { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }

    }
    public class VendorDebitNoteViewModel
    {
        public List<VendorSearchItem> Items { get; set; }
        public TotalCalculation TotalCalculation { get; set; }
    }

    public class DebitNoteItem
    {
        public int SNo { get; set; }
        public string TransId { get; set; }
        public string ItemName { get; set; }
        public string ItemNo { get; set; }
        public string UnitName { get; set; }
        public string CTotalAmount { get; set; }
        public string TotalSubTotal { get; set; }
        public string TotalSubTotal2 { get; set; }
        public string TotalGrandTotal { get; set; }

    }
    public class ItemWiseRates
    {
        public int TransId { get; set; }
        public string Description { get; set; }
        public string CRate { get; set; }
        public string CartageAmt { get; set; }
        public string CartageRate { get; set; }
        public string CartageType { get; set; }
        public string Discount { get; set; }
        public string DiscountAmount { get; set; }
        public string PackingChargeAmt { get; set; }
        public string PackingChargePer { get; set; }
        public string TransferQty { get; set; }
        public string Qty { get; set; }

    }

    public class InsuranceAndGstDetails
    {
        public int TransId { get; set; }
        public string CGSTPer { get; set; }
        public string CGSTPerAmt { get; set; }
        public string GSTSlab { get; set; }
        public string IGSTPer { get; set; }
        public string IGSTPerAmt { get; set; }
        public string InsurancePerAmt { get; set; }
        public string InsuranceRatePer { get; set; }
        public string SGSTOrUGSTPer { get; set; }
        public string SGSTOrUTGSTPerAmt { get; set; }
        public string TotalGSTAmt { get; set; }
        public string TotalGrossAmt { get; set; }



    }

    public class TotalCalculation
    {
        public string TotalRate { get; set; }
        public string SumOfPacking { get; set; }
        public string SumOfCartageAmt { get; set; }
        public string SubTotal1 { get; set; }
        public string SubTotal2 { get; set; }
        public string Total_Insurance { get; set; }
        public string Total_CGST { get; set; }
        public string Total_SGSTAndUTGST { get; set; }
        public string Total_IGST { get; set; }
        public string Total_Taxable { get; set; }
        public string Total { get; set; }
        public string GrandTotal { get; set; }

    }

   
    public class GetVendorDebitNotes
    {
        public decimal TransId { get; set; }
        public string DebitNoteCode { get; set; }
        public string DebitNoteDate { get; set; }
        public string CreatedDate { get; set; }
        public string VendorName { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string Reason { get; set; }

    }

    public class GetItemsList
    {
        public int TransId { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string GroupName { get; set; }
        public string ItemGroupID { get; set; }
        public string HSNCode { get; set; }
        public string UnitName { get; set; }
        public string GITEMCode { get; set; }
        public string Disable { get; set; }
    }

    //change18052018
    public class GetVendorList
    {
        public int TransId { get; set; }
        public string Name { get; set; }
        public string TinNo { get; set; }
        public string Address { get; set; }
        public string PanNo { get; set; }
        public string GSTNo { get; set; }
    
        public string AadhaarNo { get; set; }
        public string MobileNo { get; set; }

        public string Email { get; set; }
        public string Status { get; set; }

        public string State { get; set; }
        public string City { get; set; }

        public string ContactPerson { get; set; }
        public string PhoneNo { get; set; }
        public string VendorId { get; set; }
        public string VendorType { get; set; }
        public string Region { get; set; }

    }

    public class GetMRN
    {
        public string MRNCode { get; set; }
    }
    public class DebitNumber
    {
        public string DebitNote_Code { get; set; }
    }

    public class GetPONO_PODATEWISE
    {
        public string PO_NUMBER { get; set; }
    }

    public class GetPOAndChallanDetailsModel
    {
        public string PODate { get; set; }
        public string MRNDate { get; set; }
        public string ChallanNo { get; set; }
        public string ChallanDate { get; set; }
        public string GRNNo { get; set; }
        public string GRNDate { get; set; }
    }

    public class VendorSearchItem : tblVendorDebitNoteChild
    {
        public string PurchaseOrderNo { get; set; }
        public string VendorId { get; set; }
        public string ProjectNo { get; set; }


        //public Nullable<DateTime> ChallanDate { get; set; }

        //public string TransferQty { get; set; }

        public int TransId { get; set; }
        public string GroupName { get; set; }

        public string ItemName { get; set; }
        public string ItemID { get; set; }

        public string Unit { get; set; }
        public decimal QuantityReceived { get; set; }
        //public new decimal Rate { get; set; }

    }
    //public class DebitNumber
    //{
    //    public string DebitNote_Code { get; set; }
    //}

    public class DeletedItemOpening
    {
        public string Success { get; set; }
    }

    public class GetMaxGSTCode
    {
        public string GSTCode { get; set; }
    }
    public class GetOutPI
    {
        public string PICode { get; set; }
    }

    public class VMPODataForDisapproval
    {
        public long TransId { get; set; }
        public string ProjectName { get; set; }
        public string PONo { get; set; }
        public string CreatedBy { get; set; }
        public decimal? ReceivedQty { get; set; }

    }

    public class _ExciseViewModel
    {
        public int TransID { get; set; }
        public string EdCode { get; set; }
        public string EdType { get; set; }
        public string EdValue { get; set; }
        public Nullable<bool> IsExcise { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string ExciseStatusExistvalue { get; set; }
        public string Name { get; set; }
        public string EdNumericValue { get; set; }
    }

    public class _TaxViewModel
    {
        public int TransId { get; set; }
        public string TaxCode { get; set; }
        public string TaxType { get; set; }
        public string  TaxValue { get; set; }
        public Nullable<bool> IsTax { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string TaxStatusExistvalue { get; set; }
        public string Name { get; set; }
        public string TaxNumericValue { get; set; }
    }

    public class GetRegionAutoId
    {
        public string RegionId { get; set; }
    }

    public class InterStateTransferCode
    {
        public string TransferCode { get; set; }
    }
    public class GetOutCode
    {
        public string GetOutTransferCode { get; set; }
    }


    public class ItemWiseRateReport
    {
        public string ProjectName { get; set; }
        public string ReceiveDate { get; set; }
        public string VendorName { get; set; }
        public string PONo { get; set; }
        public string PODate { get; set; }
        public string ItemNo { get; set; }
        public string itemName { get; set; }
        public decimal? Rate { get; set; }

    }
    
    //change25042018_Report
    public class GetItemGroups
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
    }

    //change25042018_Report
    public class GetTransferProjects
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
    }

    public class VMChangePassword
    {

        [Display(Name = "Current Password"), Required(ErrorMessage = "{0} is required")]
        public string OldPassword { get; set; }

        [Display(Name = "New Password"), Required(ErrorMessage = "{0} is required"), StringLength(12, MinimumLength = 4, ErrorMessage = "{0} should be between 4 to 12 char")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password"), Required(ErrorMessage = "{0} is required"), StringLength(12, MinimumLength = 4, ErrorMessage = "{0} should be between 4 to 12 char"), Compare("NewPassword", ErrorMessage = "New Password and confirm password is not matched")]
        public string ConfirmPassword { get; set; }
    }

    public class VMItemBudgetData
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string FinancialYear { get; set; }
        public Nullable<decimal> TotalQuantity { get; set; }
        public Nullable<decimal> ExcessPercentage { get; set; }
        public Nullable<decimal> ExcessQuantity { get; set; }
        public Nullable<decimal> Apr { get; set; }
        public Nullable<decimal> May { get; set; }
        public Nullable<decimal> June { get; set; }
        public Nullable<decimal> July { get; set; }
        public Nullable<decimal> Aug { get; set; }
        public Nullable<decimal> Sep { get; set; }
        public Nullable<decimal> Oct { get; set; }
        public Nullable<decimal> Nov { get; set; }
        public Nullable<decimal> Dec { get; set; }
        public Nullable<decimal> Jan { get; set; }
        public Nullable<decimal> Feb { get; set; }
        public Nullable<decimal> Mar { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}