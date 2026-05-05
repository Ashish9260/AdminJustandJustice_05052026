using System.Data;
using System.Data.SqlClient;

namespace AdminJustAndJustice.Models
{
    public class ERPBlogModel : CommonModel
    {
        public string? EditPkBlogID { get; set; }
        public string? PkBlogID { get; set; }
        public string? BlogTitle { get; set; }
        public string? URLtext { get; set; }
        public string? Category { get; set; }
        public string? FirstDescr { get; set; }
        public string? FirstImgURL { get; set; }
        public string? intFkCaseTypeId { get; set; }
        public string? Author { get; set; }
        public string? PublishStatus { get; set; }
        public string? PublishOn { get; set; }
        public string? SeoKeyword { get; set; }
        public List<string>? Tags { get; set; }
        public string? Tag { get; set; }
        public List<string>? SeoKeywords { get; set; }

        public string? SEOTitle { get; set; }
        public string? SEODescr { get; set; }
        public string? SEOKeywords { get; set; }
        public string? IsStatus { get; set; }
        public string? SequenceNo { get; set; }
        public DataTable? dtProductEnquiry { get; set; }
        public async Task<DataSet> AddEditDltBlog()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@MODE", Mode),
                    new SqlParameter("@intPkBlogID", EditPkBlogID),
                    new SqlParameter("@strBlogTitle", BlogTitle),
                    new SqlParameter("@strURLtext", URLtext),
                    new SqlParameter("@strCategory", Category),
                    new SqlParameter("@strFirstDescr", FirstDescr),
                    new SqlParameter("@strFirstImgURL", FirstImgURL),
                    new SqlParameter("@strAuthor", Author),
                    new SqlParameter("@ddlPublishStatus", PublishStatus),
                    new SqlParameter("@dtPublishOn", PublishOn),
                    new SqlParameter("@strTags", Tag),
                    new SqlParameter("@strSEOTitle", SEOTitle),
                    new SqlParameter("@strSEODescr", SEODescr),
                    new SqlParameter("@strSEOKeywords", SeoKeyword),
                    new SqlParameter("@intFkCaseTypeId", intFkCaseTypeId),
                    new SqlParameter("@intCreatedBy", bigintCreatedBy),
                    new SqlParameter("@status", IsStatus),
                    new SqlParameter("@intFkBranchId", intFkBranchId),
                    new SqlParameter("@strIP", ""),
                };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.AddEditDltBlog, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<DataSet> GetBlogList()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@intFkBranchId", intFkBranchId),
                    new SqlParameter("@intPkBlogID", EditPkBlogID),
                    new SqlParameter("@strSearchParam", SearchParam),
                    new SqlParameter("@PAGENO", PageNo),
                    new SqlParameter("@PAGESIZE", PageSize),
                     };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.GetBlogList, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<DataSet> UpdateBlogPublished()
        {
            try
            {
                SqlParameter[] para =
                {
                 new SqlParameter("@intFkintBlogID", EditPkBlogID),
                 new SqlParameter("@intAutoId", bigintCreatedBy),
                 };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.UpdateBlogPublished, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
