using System.Data;
using System.Data.SqlClient;

namespace AdminJustAndJustice.Models
{
    public class ERPBannerModel : CommonModel
    {
        public string? BannerID { get; set; }
        public string? EditBannerID { get; set; }
        public string? BannerTitle { get; set; }
        public string? BannerDetails { get; set; }
        public string? ImageURL { get; set; }
        public string? VideoURL { get; set; }
        public string? OldImageURL { get; set; }
        public string? SequnceNo { get; set; }
        public string? IsStatus { get; set; }
        public DataTable? dtBanner { get; set; }
        public async Task<DataSet> BannerAddEdit()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@MODE", Mode),
                    new SqlParameter("@BannerID", EditBannerID),
                    new SqlParameter("@strBannerTitle", BannerTitle),
                    new SqlParameter("@bitIsStatus", IsStatus),
                    new SqlParameter("@intFkBranchID", 0),
                    new SqlParameter("@intSequnceNo", SequnceNo),
                    new SqlParameter("@strImageURL", ImageURL),
                    new SqlParameter("@strVideoURL", VideoURL),
                    new SqlParameter("@strDetials", BannerDetails),
                    new SqlParameter("@intAutoId", bigintCreatedBy),
                };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.BannerAddEdit, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<DataSet> GetBanner()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@intPkBannerID", EditBannerID),
                    new SqlParameter("@PAGENO", PageNo),
                    new SqlParameter("@PAGESIZE", PageSize),
                };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.GetBanner, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
