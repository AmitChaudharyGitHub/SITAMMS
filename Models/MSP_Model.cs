using System;
using System.Collections.Generic;
using System.Linq;
using MMS.DAL;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations;
using MMS.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using MMS.Helpers;

namespace MMS.Models
{
    public class MSP_Model
    {
        MMSEntities objmms = new MMSEntities();

        //public List<Get_MSP_Record> GetAllMsp(Nullable<int> m, Nullable<int> y, string PID, string GID)
        //{
        //    return objmms.FN_GetMSPRecords(m, y, PID, GID).ToList();
        //}

        public List<Get_MSP_Record> GetAllMspData(Nullable<int> m, Nullable<int> y, string PID, string GID)
        {

            List<Get_MSP_Record> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    context.Database.CommandTimeout = 300;
                    //ItemNo = null;
                    //DbRawSqlQuery<Get_MSP_Record> xx4 = context.Database.SqlQuery<Get_MSP_Record>("EXEC GetMSP  {0} , {1}, {2}, {3}", m, y, PID, GID);
                    DbRawSqlQuery<Get_MSP_Record> xx4 = context.Database.SqlQuery<Get_MSP_Record>("EXEC GetMSPNew  {0} , {1}, {2}, {3}", m, y, PID, GID);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }


        public DataTable PrintMSP(string PId,string GId,int? m,int? y)
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Model1"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 1000;
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PrintMSPAllItems";
                cmd.Parameters.AddWithValue("@pid", PId);
                cmd.Parameters.AddWithValue("@gid", GId);
                cmd.Parameters.AddWithValue("@m",m);
                cmd.Parameters.AddWithValue("@y",y);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public DataTable PrintMSP1(string PId, string GId, int? m, int? y)
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Model1"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 1000000;
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PrintMSPAllItems1";
                cmd.Parameters.AddWithValue("@pid", PId);
                cmd.Parameters.AddWithValue("@gid", GId);
                cmd.Parameters.AddWithValue("@m", m);
                cmd.Parameters.AddWithValue("@y", y);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public DataTable PrintAgingReport(string ProjectId, string GroupId, string Status)
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Model1"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 1000;
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "New_MovableAndNonMobable_Test";
                cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
                cmd.Parameters.AddWithValue("@ItemGroupId",GroupId);
                cmd.Parameters.AddWithValue("@Status",Status);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public DataTable PrintMSPSummary(string PId, int? m, int? y)
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Model1"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 1000;
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetMSPSummaryForExport";
                cmd.Parameters.AddWithValue("@pid", PId);
                cmd.Parameters.AddWithValue("@m", m);
                cmd.Parameters.AddWithValue("@y", y);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public DataTable PrintAgaingReport(string PId, string Status)
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Model1"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 1000;
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "New_MovableAndNonMobableForExport";
                cmd.Parameters.AddWithValue("@ProjectId", PId);
                cmd.Parameters.AddWithValue("@Status", Status);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public List<GetMovableReport> GetAllItemStatusReport(string ProjID, string GroupItemID, string Status)
        {
            List<GetMovableReport> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetMovableReport> rpt = context.Database.SqlQuery<GetMovableReport>("EXEC GetMovingAndNonMovingStatus {0},{1},{2}", ProjID, GroupItemID, Status);
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

        public List<GetMovingData> GetAllMovingDiscription()
        {
            List<GetMovingData> List = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetMovingData> md = Context.Database.SqlQuery<GetMovingData>("EXEC GetAllMovableStatusMaster");
                    List = md.ToList();
                    return List;
                }
                catch(Exception ex) {
                    ex.ToString();
                    return null;
                }
            }
        }



        public List<POGET_LIMIT_VALUE> GetAllPOLimitValue()
        {
            List<POGET_LIMIT_VALUE> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<POGET_LIMIT_VALUE> sql = context.Database.SqlQuery<POGET_LIMIT_VALUE>("EXEC GetPOLimitValue");
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

        public List<GetMappedData> GetMappedData()
        {
            List<GetMappedData> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetMappedData> sql = context.Database.SqlQuery<GetMappedData>("EXEC GetMappedMasterData");
                    list = sql.ToList(); return list;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }

            }
        }
        public List<NewMovableStatus> GetNew_AllItemStatusReport(string ProjID, string GroupItemID, string Status)
        {
            List<NewMovableStatus> list = null;
            using (var context = new MMSEntities())
            {
                context.Database.CommandTimeout = 5000;
                try
                {
                    DbRawSqlQuery<NewMovableStatus> rpt = context.Database.SqlQuery<NewMovableStatus>("EXEC New_MovableAndNonMobable {0},{1},{2}", ProjID, GroupItemID, Status);
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
        public string GetFinancialYear()
        {
            List<FinancialYear> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<FinancialYear> finyr = Context.Database.SqlQuery<FinancialYear>("EXEC GetFYear");
                    lst = finyr.ToList();
                    return lst.First().FinYear;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }


        public string GetFinancialYearByDate(DateTime date)
        {
            List<FinancialYear> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<FinancialYear> finyr = Context.Database.SqlQuery<FinancialYear>("EXEC GetFYearByDate {0}",date);
                    lst = finyr.ToList();
                    return lst.First().FinYear;

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return null;
                }
            }
        }




        public List<EquipmentGrid> GetEuipment(string GrpId,string ItemId)
        {
            List<EquipmentGrid> list = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<EquipmentGrid> sql = context.Database.SqlQuery<EquipmentGrid>("EXEC GetMMSEquipmentMaster {0},{1}", GrpId, ItemId);
                    list = sql.ToList(); return list;
                }
                catch (Exception ex)
                {
                    ex.ToString(); return null;
                }
            }
        }


        public List<GET_CURRENT_STOCK> GetAllCurrentStock(string PID)
        {

            List<GET_CURRENT_STOCK> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_CURRENT_STOCK> xx4 = context.Database.SqlQuery<GET_CURRENT_STOCK>("EXEC GetCurrentStock_Test  {0}", PID);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }

        public List<GET_CURRENT_STOCK> GetDailyIssuedStock(string PID,string  FromDate, string  ToDate)
        {

            List<GET_CURRENT_STOCK> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_CURRENT_STOCK> xx4 = context.Database.SqlQuery<GET_CURRENT_STOCK>("EXEC GetDailyIssueStock  {0},{1},{2}", PID, FromDate, ToDate);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }




        public List<PendingPIGrid> GetAllPendingPI(string PRJID,string EmpId,string PINo,int DivisionId)
        {

            List<PendingPIGrid> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingPIGrid> xx4 = context.Database.SqlQuery<PendingPIGrid>("EXEC GetPendingPI_Test {0},{1},{2},{3}", PRJID,EmpId, PINo,DivisionId);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }


        public List<PendingPIGrid> GetPurchaseIndentPendingPIC(string EmpId)
        {

            List<PendingPIGrid> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingPIGrid> xx4 = context.Database.SqlQuery<PendingPIGrid>("EXEC DashBPurIndentPendingPIC {0}",EmpId);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }

        public List<PendingPIGrid> GetAllApprovedPI(string PRJID,string EmpId,string PINo,int DivisionId)
        {

            List<PendingPIGrid> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<PendingPIGrid> xx4 = context.Database.SqlQuery<PendingPIGrid>("EXEC GetApprovedPI_Test  {0},{1},{2},{3}", PRJID, EmpId, PINo,DivisionId);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }


        public List<ProjectGrid> GetAllProjectDetail(string CompId)
        {
            List<ProjectGrid> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<ProjectGrid> xx4 = context.Database.SqlQuery<ProjectGrid>("EXEC GetProjectDetailRecord  {0}", CompId);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }
        public List<UserGrid> GetAllUserDetail(string CompId)
        {
            List<UserGrid> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<UserGrid> xx4 = context.Database.SqlQuery<UserGrid>("EXEC GetEmployeeDetailRecord  {0}", CompId);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }

        public List<GET_RECEIVED_STOCK> GetAllReceivedStockUpdated(string PID, string Type,string RN)
        {

            List<GET_RECEIVED_STOCK> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_RECEIVED_STOCK> xx4 = context.Database.SqlQuery<GET_RECEIVED_STOCK>("EXEC GetReceivedStockUpdated  {0},{1},{2}", PID,Type, RN);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }
        public List<GET_RECEIVED_STOCK> GetAllReceivedStock(string PID,string ItemGroupId,string ItemId,string VendorId,string FromDate,string ToDate)
        {

            List<GET_RECEIVED_STOCK> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_RECEIVED_STOCK> xx4 
                        = context.Database.SqlQuery<GET_RECEIVED_STOCK>("EXEC GetReceivedStock  {0},{1},{2},{3},{4},{5}", PID,ItemGroupId,ItemId,VendorId,FromDate,ToDate);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }
        public List<GET_ISSUED_STOCK> GetAllIssuedStockForReport(string PID, string ItemGroupId, string ItemId,string EmployeeId,string BlockName)
        {

            List<GET_ISSUED_STOCK> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    //procedur: GetIssueStock
                    DbRawSqlQuery<GET_ISSUED_STOCK> xx4 = context.Database.SqlQuery<GET_ISSUED_STOCK>("EXEC GetIssueStockGetOutTransfer1  {0},{1},{2},{3},{4}", PID, ItemGroupId, ItemId, EmployeeId,BlockName);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }

        public List<GET_ISSUED_STOCK> GetAllIssuedStock(string PID)
        {

            List<GET_ISSUED_STOCK> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    //procedur: GetIssueStock
                    DbRawSqlQuery<GET_ISSUED_STOCK> xx4 = context.Database.SqlQuery<GET_ISSUED_STOCK>("EXEC GetIssueStockGetOutTransfer  {0}", PID);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }

        //change22052018
        public List<GET_ISSUED_STOCK> GetPendingDebitNote(string PID, string fromdate, string todate, string IssuedTo, string ItemGroups, string ItemId, string FinancialType, string ContractorType)
        {
            
            List<GET_ISSUED_STOCK> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                { 

                    DbRawSqlQuery<GET_ISSUED_STOCK> xx4 = context.Database.SqlQuery<GET_ISSUED_STOCK>("EXEC GetPendingDebitNote  {0},{1},{2},{3},{4},{5},{6}", PID,ItemGroups,ItemId,IssuedTo, FinancialType,fromdate,todate);
                    aa4 = xx4.ToList();
                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }
        

        public List<GET_TRANSFERRED_STOCK> GetAllTransferredStock(string PID)
        {

            List<GET_TRANSFERRED_STOCK> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_TRANSFERRED_STOCK> xx4 = context.Database.SqlQuery<GET_TRANSFERRED_STOCK>("EXEC GetTransferStock  {0}", PID);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }
        public List<GET_LIMIT_VALUE> GetAllLimitValue()
        {
            List<GET_LIMIT_VALUE> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_LIMIT_VALUE> sql = context.Database.SqlQuery<GET_LIMIT_VALUE>("EXEC GetLimitValue");
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
        public string GetProjectNumberAdmin(String CompId)
        {
            List<GetProjectNumber> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetProjectNumber> prid = Context.Database.SqlQuery<GetProjectNumber>("EXEC GetNewProjectId {0}", CompId);
                    lst = prid.ToList(); return lst.First().ProjId;
                }
                catch (Exception ex)
                { ex.ToString(); return null; }
            }
        }

        public List<GET_PI_ORDER> GetPIDetail(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<GET_PI_ORDER> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_PI_ORDER> sql = context.Database.SqlQuery<GET_PI_ORDER>("EXEC GetPIDetail {0},{1}", ProjectId,divisionId);
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
        public List<GET_PI_ORDER> GetPIDetailWithPurchase(string ProjectId, string PurchType,string PINo)
        {
            var divisionId = Repos.GetUserDivision();
            List<GET_PI_ORDER> list1 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_PI_ORDER> sql = context.Database.SqlQuery<GET_PI_ORDER>("EXEC GetPIDetailWithPurchase {0},{1},{2},{3}", ProjectId,Convert.ToInt32(PurchType),PINo,divisionId);
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

        public List<GET_PI_ORDER> GetPartialPo_Detail(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<GET_PI_ORDER> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_PI_ORDER> sql = context.Database.SqlQuery<GET_PI_ORDER>("EXEC GetPartial_PO_Detail {0},{1}", ProjectId,divisionId);
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
        public List<GET_PI_ORDER> GetPartialPo_DetailWithPurchase(string ProjectId, string PurchType,string PINo)
        {
            var divisionId = Repos.GetUserDivision();
            List<GET_PI_ORDER> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_PI_ORDER> sql = context.Database.SqlQuery<GET_PI_ORDER>("EXEC GetPartial_PO_DetailWithPurchase {0},{1},{2},{3}", ProjectId, Convert.ToInt32(PurchType),PINo,divisionId);
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
        public List<GET_PI_ORDER> GetUpdatedPo_Detail(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<GET_PI_ORDER> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_PI_ORDER> sql = context.Database.SqlQuery<GET_PI_ORDER>("EXEC GetUpdated_PO_Detail {0},{1}", ProjectId,divisionId);
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

        public List<GET_PI_ORDER> GetUpdatedPo_DetailWithPurchase(string ProjectId, string PurchType,string PINo)
        {
            var divisionId = Repos.GetUserDivision();
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


        public string GetEmployeeNumber(String CompId)
        {
            List<GetMaxEmployeeID> lst = null;
            using (var Context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetMaxEmployeeID> prid = Context.Database.SqlQuery<GetMaxEmployeeID>("EXEC GetNewEmployeeId {0}", CompId);
                    lst = prid.ToList(); return lst.First().EmpId;
                }
                catch (Exception ex)
                { ex.ToString(); return null; }
            }
        }

        public List<GetVendorStatus> GetStatus()
        {

            List<GetVendorStatus> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GetVendorStatus> xx4 = context.Database.SqlQuery<GetVendorStatus>("EXEC GetVendorStatus");
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }
        public string CreateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        //change09042018
        public List<GET_UPDATED_DEBIT_NOTE> GetUpdatedDebitNotes(string ProjectId, string DebitNoteCode)
        {
            var divisionId = Repos.GetUserDivision();
            List<GET_UPDATED_DEBIT_NOTE> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_UPDATED_DEBIT_NOTE> xx4 = context.Database.SqlQuery<GET_UPDATED_DEBIT_NOTE>("EXEC GetUpdatedDebitNotes {0},{1},{2}", ProjectId, DebitNoteCode,divisionId);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }

        //change09042018


        public List<GET_ISSUED_STOCK> GetIssuedStockByDebitCodeAndProjectId(string DebitCode, string ProjectId)
        {

            List<GET_ISSUED_STOCK> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_ISSUED_STOCK> xx4 = context.Database.SqlQuery<GET_ISSUED_STOCK>("EXEC GetIssuedStockByUpdateDebitNoteCode {0},{1}", DebitCode, ProjectId);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }

        public List<GET_UPDATED_DEBIT_NOTE> GetUpdatedDebitNotesForSearch(string ProjectId, string ContractorType, string IssuedTo)
        {
            var divisionId = Repos.GetUserDivision();
            List<GET_UPDATED_DEBIT_NOTE> aa4 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_UPDATED_DEBIT_NOTE> xx4 = context.Database.SqlQuery<GET_UPDATED_DEBIT_NOTE>("EXEC GetUpdatedDebitNotesForSearch {0},{1},{2},{3}", ProjectId, ContractorType, IssuedTo,divisionId);
                    aa4 = xx4.ToList();

                    return aa4;

                }
                catch (Exception ex) { ex.Message.ToString(); }
                return null;
            }
        }

        public List<GET_PI_ORDER> GetClosedPo_Detail(string ProjectId)
        {
            var divisionId = Repos.GetUserDivision();
            List<GET_PI_ORDER> list2 = null;
            using (var context = new MMSEntities())
            {
                try
                {
                    DbRawSqlQuery<GET_PI_ORDER> sql = context.Database.SqlQuery<GET_PI_ORDER>("EXEC GetClosed_PO_Detail {0},{1}", ProjectId, divisionId);
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


    }


    //public class Get_MSP_Record
    //{
    //    public Nullable<int> ID { get; set; }
    //    public string ItemCode { get; set; }
    //    [Display(Name = "Item Name")]
    //    public string ItemDescription { get; set; }
    //    public string UnitID { get; set; }
    //    [Display(Name = "Unit Name")]
    //    public string Unit { get; set; }
    //    [Display(Name = "Rate For This Month")]
    //    public Nullable<decimal> RateForThisMonth { get; set; }
    //    [Display(Name = "Opening Balance Qty")]
    //    public Nullable<decimal> OpeningBalanceQty { get; set; }
    //    [Display(Name = "Recv Upto Last Month")]
    //    public Nullable<decimal> RecvUptoLastMonth { get; set; }
    //    [Display(Name = "Recv During Current Month")]
    //    public Nullable<decimal> RecvDuringCurrentMonth { get; set; }
    //    [Display(Name = "Recv Total Upto")]
    //    public Nullable<decimal> RecvTotalUpto { get; set; }
    //    [Display(Name = "Issued Upto Last Month")]
    //    public Nullable<decimal> IssuedUptoLastMonth { get; set; }
    //    [Display(Name = "Issued During Current Month")]
    //    public Nullable<decimal> IssuedDuringCurrentMonth { get; set; }
    //    [Display(Name = "Issued Total Upto")]
    //    public Nullable<decimal> IssuedTotalUpto { get; set; }
    //    [Display(Name = "Closing Balance")]
    //    public Nullable<decimal> ClosingBalance { get; set; }
    //    [Display(Name = "Opening Balance Amt")]
    //    public Nullable<decimal> OpeningBalanceAmt { get; set; }
    //    [Display(Name = "Recv Amt Upto Last Month")]
    //    public Nullable<decimal> RecvAmtUptoLastMonth { get; set; }
    //    [Display(Name = "Recv Amt During Current Month")]
    //    public Nullable<decimal> RecvAmtDuringCurrentMonth { get; set; }
    //    [Display(Name = "Recv Amt Total Upto")]
    //    public Nullable<decimal> RecvAmtTotalUpto { get; set; }
    //    [Display(Name = "Issued Amt Upto Last Month")]
    //    public Nullable<decimal> IssuedAmtUptoLastMonth { get; set; }
    //    [Display(Name = "Issued Amt During Current Month")]
    //    public Nullable<decimal> IssuedAmtDuringCurrentMonth { get; set; }
    //    [Display(Name = "Issued Amt Total Upto")]
    //    public Nullable<decimal> IssuedAmtTotalUpto { get; set; }
    //    [Display(Name = "Closing Balance Amt")]
    //    public Nullable<decimal> ClosingBalanceAmt { get; set; }
    //}

    public class NewMovableStatus
    {
        public int ID { get; set; }
        public string ItemGrpId { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string UniteName { get; set; }
        public Nullable<decimal> CurrentStockQty { get; set; }
        public Nullable<decimal> CurrentStockAmt { get; set; }
        public string MOVINGSTATUS { get; set; }
        public string LstIssueDate { get; set; }
        public string LstReceiveDate { get; set; }

        public decimal? LastReceiveQuantity { get; set; }
        public decimal? LastReciveAmount { get; set; }
        public decimal? LastIssueQuantity { get; set; }
        public decimal? LastIssueAmount { get; set; }


    }
    public class GetVendorStatus
    {
        public int TransId { get; set; }
        public string Status { get; set; }

    }
    public class POGET_LIMIT_VALUE
    {
        public Int32 TransId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Nullable<decimal> LimitValue { get; set; }
        public string CreatedBy { get; set; }
        public string EmpName { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }

    //change 09042018
    public class GET_UPDATED_DEBIT_NOTE
    {
        public string Issue_Req_No { get; set; }
        public string ProjectName { get; set; }
        public string DebitNote_Code { get; set; }
        public DateTime DebitNote_Date { get; set; }
        public string ItemId { get; set; }
        public string ContractorType { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }
    public class Get_MSP_Record
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> RateForThisMonth { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.000#}")]
        public Nullable<decimal> OpeningBalanceQty { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.000#}")]
        public Nullable<decimal> RecvUptoLastMonth { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.000#}")]
        public Nullable<decimal> RecvDuringCurrentMonth { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.000#}")]
        public Nullable<decimal> RecvTotalUpto { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.000#}")]
        public Nullable<decimal> IssuedUptoLastMonth { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.000#}")]
        public Nullable<decimal> IssuedDuringCurrentMonth { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.000#}")]
        public Nullable<decimal> IssuedTotalUpto { get; set; }
        public Nullable<decimal> ClosingBalance { get; set; }
        public Nullable<decimal> OpeningBalanceAmt { get; set; }
        public Nullable<decimal> RecvAmtUptoLastMonth { get; set; }        
        public Nullable<decimal> RecvAmtDuringCurrentMonth { get; set; }
        public Nullable<decimal> RecvAmtTotalUpto { get; set; }
        public Nullable<decimal> IssuedAmtUptoLastMonth { get; set; }
        public Nullable<decimal> IssuedAmtDuringCurrentMonth { get; set; }
        public Nullable<decimal> IssuedAmtTotalUpto { get; set; }
        public Nullable<decimal> ClosingBalanceAmt { get; set; }

        public decimal? CurrentMonthReturnQty { get; set; }
        public decimal? CurrentMonthReturnAmt { get; set; }

    }


    public class MSPSummary
    {
        public int Id { get; set; }
        public string ItemGroupName { get; set; }
        public Nullable<decimal> OpeningBalance { get; set; }
        public Nullable<decimal> ReceivedUptoLastMonth { get; set; }
        public Nullable<decimal> ReceivedDuringCurrentMonth { get; set; }
        public Nullable<decimal> ReceivedTotalUptoDate { get; set; }
        public Nullable<decimal> IssuedUptoLastMonth { get; set; }
        public Nullable<decimal> IssuedDuringCurrentMonth { get; set; }
        public Nullable<decimal> TotalIssuedUptoDate { get; set; }
        public Nullable<decimal> ClosingBalance { get; set; }
        public string Remarks { get; set; }
    }


    public class GET_PI_ORDER
    {
        public Nullable<decimal> UId { get; set; }
        public string PINumber { get; set; }
        public Nullable<DateTime> PIDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<decimal> PIValue { get; set; }
        public int PurchasePIC_Type { get; set; }
        public string PurchaseType { get; set; }
        public string ForwardToMang { get; set; }
        public string ApprovedBy { get; set; }
        public string Purchase_Status { get; set; }
        public string Close_Remark { get; set; }
    }
    public class GetMovableReport
    {
        public Nullable<int> ID { get; set; }

        public string ItemName { get; set; }

        public Nullable<DateTime> ItemIssueDate { get; set; }

        public Nullable<DateTime> ItemReceiveDate { get; set; }

        public string MovingStatus { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public string UniteName { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string DateDifference { get; set; }
        public Nullable<decimal> LastRcvQty { get; set; }
        public Nullable<decimal> LastIssueQty { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }


    public class GetEquipmentCategoryData
    {
        public Nullable<int> ID { get; set; }

        public string Category { get; set; }
    }

    public class GetMovingData
    {
        public Nullable<int> TransID { get; set; }
        public string MovingType { get; set; }
        public string MinMonth { get; set; }
        public string MaxMonth { get; set; }

        public string CreatedDate { get; set; }
    }
    public class GetMappedData
    {
        public Nullable<long> RowNumber { get; set; }
        public string ProjectName { get; set; }
        public string Location { get; set; }
        public string GroupName { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public string UnitConversionName { get; set; }
        public Nullable<decimal> UnitRate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }
    public class EquipmentGrid
    {
        public Nullable<long> RowNumber { get; set; }
        public string ProjectName { get; set; }
        public string GroupName { get; set; }
        public string ItemName { get; set; }
        public string EquipmentName { get; set; }
        public string OwnerName { get; set; }
        public string BrandName { get; set; }
        public string EquipmentStatus { get; set; }
        public string EquipmentCondition { get; set; }
        public string BodyColor { get; set; }
        public string Model { get; set; }
        public Nullable<DateTime> PurchaseDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }


    public class GET_CURRENT_STOCK
    {
        public string ItemGroupId { get; set; }
        public string GroupName { get; set; }
        public string ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public Nullable<decimal> IssueQty { get; set; }
        public Nullable<decimal> CurrentStockQty { get; set; }
        public Nullable<decimal> CurrentStockAmt { get; set; }
        public string Project { get; set; }
        public Nullable<DateTime> TypeDate { get; set; }
        public Nullable<decimal> IssueAmt { get; set; }
    }
    public class GET_RECEIVED_STOCK
    {
        public Nullable<DateTime> ReceiveDate { get; set; }
        public string IDType { get; set; }
        public string GRNNo { get; set; }
        public string ChallanNo { get; set; }
        public string VehicleNo { get; set; }
        public string StatusTypeNo { get; set; }
        public string Vendor { get; set; }
        public string VendorNo { get; set; }
        public string ItemGroupId { get; set; }
        public string GroupName { get; set; }
        public string ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public decimal? ReceiveQty { get; set; }
        public decimal? Rate { get; set; }
        public decimal? IssueQty { get; set; }
        public decimal? CurrentStockQty { get; set; }
        public decimal? ReceiveAmt { get; set; }
        public decimal? IssueAmt { get; set; }
        public decimal? CurrentStockAmt { get; set; }
        public string Project { get; set; }
    }
    public class GET_ISSUED_STOCK
    {
        public Nullable<DateTime> Issue_Date { get; set; }
        public Int32 IssueNo { get; set; }
        public string IndentNo { get; set; }
        public string ItemGroupID { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public string EmployeeID { get; set; }
        public string IssuedTo { get; set; }
        public Nullable<decimal> IssueQuantity { get; set; }

        public Nullable<decimal> Rate { get; set; }

        public string Project { get; set; }
        public string Optional_Name { get; set; }
        public string FinancialType { get; set; }
        public Nullable<decimal> IssueAmount { get; set; }

        //change06042018
        public string ItemCode { get; set; }
        public string HSNCode { get; set; }
        public string DebitNote { get; set; }
        public Nullable<bool> IsDebitNote { get; set; }
        public string Type { get; set; }
        public string BlockName { get; set; }

        //add on 18/02/2020
        public string IssueBill_No { get; set; }
        //end
    }
    public class GET_TRANSFERRED_STOCK
    {
        public Nullable<DateTime> TransferDate { get; set; }
        public Int32 Id { get; set; }
        public string TransferNo { get; set; }
        public string ItemGroupID { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public string SiteId { get; set; }
        public string TransferTo { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Project { get; set; }
        public string ProjectID { get; set; }
    }

    public class FinancialYear
    {
        public string FinYear { get; set; }
    }

    public class GET_LIMIT_VALUE
    {
        public Int32 TransId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Nullable<decimal> LimitValue { get; set; }
        public string CreatedBy { get; set; }
        public string EmpName { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }

    public class GetProjectNumber
    {
        public string ProjId { get; set; }
    }

    public class GetMaxEmployeeID
    {
        public string EmpId { get; set; }
    }

    public class GetTransferDataForReport
    {
        public string TransferNumber { get; set; }
        public Nullable<DateTime> TransferDate { get; set; }
        public Nullable<DateTime> TransferCreatedDate { get; set; }
        public string GetOutNumber { get; set; }
        public Nullable<DateTime> GetOutDate { get; set; }
        public Nullable<DateTime> GateOutCratedDate { get; set; }
        public Nullable<DateTime> ReachToSite { get; set; }
        public string TransferTo { get; set; }
        public string ItemGroup { get; set; }
        public string ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal TransferQty { get; set; }
        public decimal TransferRate { get; set; }
        public decimal Amount { get; set; }
        public decimal GetOutQty { get; set; }
        public string Unit { get; set; }
    }


    public class GetTransferDataForReceiptReport
    {
        public string TransferNumber { get; set; }
        public string GateEntryDate { get; set; }
        public string TransferCreatedDate { get; set; }
        public string GateEntryNo { get; set; }
        public string TransferFrom { get; set; }
        public string ItemGroup { get; set; }
        public string ItemNo { get; set; }
        public string ItemCode { get; set; }
        public string Item { get; set; }
        public decimal ReceiveQty { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; }
        public decimal Rate { get; set; }

    }

}
