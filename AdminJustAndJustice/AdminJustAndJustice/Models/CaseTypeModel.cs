using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Net.NetworkInformation;

namespace AdminJustAndJustice.Models
{
    public class CaseTypeModel:CommonModel
    {
        public string? EditCaseTypeID { get; set; }
        public string? CaseTypeID { get; set; }
        public string? CaseType { get; set; }
        public string? ShortCaseType { get; set; }
        public string? Detials { get; set; }
        public string? SequenceNo { get; set; }
        public string? URLText { get; set; }
        public string? SeoKeyword { get; set; }
        public string? FirstImage { get; set; }
        public List<string>? Tags { get; set; }
        public string? Tag { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoDescr { get; set; }
        public DataTable? dtCaseType { get; set; }
        public List<string>? SeoKeywords { get; set; }
        public async Task<DataSet> CaseTypeAddEdit()
        {
            try
            {

                SqlParameter[] para =
                {
                    new SqlParameter("@MODE", Mode),
                    new SqlParameter("@intCaseTypeID", EditCaseTypeID),
                    new SqlParameter("@strCaseType", CaseType),
                    new SqlParameter("@strShortCaseType", ShortCaseType),
                    new SqlParameter("@strDetials", Detials),
                    new SqlParameter("@intSequenceNo", SequenceNo),
                    new SqlParameter("@strURLText", URLText),
                    new SqlParameter("@strFirstImage", FirstImage),
                    new SqlParameter("@strTags", Tag),
                    new SqlParameter("@strSeoTitle", SeoTitle),
                    new SqlParameter("@strSeoDescr", SeoDescr),
                    new SqlParameter("@strSeoKeywords", SeoKeyword),
                    new SqlParameter("@intCreatedBy", bigintCreatedBy),
                };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.AddEditDltCaseType, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<DataSet> GetCaseType()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@intPkCaseTypeID", EditCaseTypeID),
                    new SqlParameter("@PAGENO", PageNo),
                    new SqlParameter("@PAGESIZE", PageSize),
                };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.GetCaseType, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
    public class SeoKeywordsDto
    {
        public string seoKeywords { get; set; }
    }
    public class SeoTagsDto
    {
        public string seotags { get; set; }
    }
}
