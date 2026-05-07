using System.Data;
using System.Data.SqlClient;

namespace AdminJustAndJustice.Models
{
    public class CaseModel:CommonModel
    {
        public string? EditPkCaseID { get; set; }
        public string? PkCaseID { get; set; }
        public string? ClientName { get; set; }
        public string? ClientMobileNo { get; set; }
        public string? ClientEmailId { get; set; }
        public string? ClientAddress { get; set; }
        public string? CaseNo { get; set; }
        public string? CaseTitle { get; set; }
        public string? Status { get; set; }
        public string? CaseStartDate { get; set; }
        public string? JudgementDate { get; set; }
        public string? JudgeName { get; set; }
        public string BranchName { get; set; }
        public string? Priority { get; set; }
        public string? ShortDetails { get; set; }
        public string? Details { get; set; }
        public string? FkCaseTypeId { get; set; }
        public string? status { get; set; }
        public DataTable? dtProductEnquiry { get; set; }
        public async Task<DataSet> AddEditDltCase()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@MODE", Mode),
                    new SqlParameter("@intPkCaseID", PkCaseID),
                    new SqlParameter("@strClientName", ClientName),
                    new SqlParameter("@strClientMobileNo", ClientMobileNo),
                    new SqlParameter("@strClientEmailId", ClientEmailId),
                    new SqlParameter("@strClientAddress", ClientAddress),
                    new SqlParameter("@strCaseNo", CaseNo),
                    new SqlParameter("@strCaseTitle", CaseTitle),
                    new SqlParameter("@strStatus", status),
                    new SqlParameter("@dtCaseStartDate", CaseStartDate),
                    new SqlParameter("@dtJudgementDate", JudgementDate),
                    new SqlParameter("@strJudgeName", JudgeName),
                    new SqlParameter("@strBranchName", BranchName),
                    new SqlParameter("@strPriority", Priority),
                    new SqlParameter("@strShortDetails", ShortDetails),
                    new SqlParameter("@intCreatedBy", bigintCreatedBy),
                    new SqlParameter("@strDetails", Details),
                    new SqlParameter("@intFkCaseTypeId", FkCaseTypeId),
                    new SqlParameter("@status", ""),
                };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.AddEditDltCase, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<DataSet> GetCaseList()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@intFkBranchId", intFkBranchId),
                    new SqlParameter("@intPkCaseID", EditPkCaseID),
                    new SqlParameter("@strSearchParam", SearchParam),
                    new SqlParameter("@PAGENO", PageNo),
                    new SqlParameter("@PAGESIZE", PageSize),
                     };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.GetCaseList, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
