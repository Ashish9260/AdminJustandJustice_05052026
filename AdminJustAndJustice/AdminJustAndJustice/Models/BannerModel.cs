using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace AdminJustAndJustice.Models
{
    public class ContactModel : CommonModel
    {
        public string? ContactID  { get; set; }
        public string? EditContactID { get; set; }
        public string? ContactTitle { get; set; }
        public string? IsStatus { get; set; }
        public string? IsMainAddress { get; set; }
        public string? FkBranchID { get; set; }
        public string? SequnceNo { get; set; }
        public string? MobileNo  { get; set; }
        public string? Location { get; set; }
        public string? EmailId { get; set; }
        public DataTable? dtContact { get; set; }
        public List<string>? MobileNos { get; set; }
        public List<string>? Emails { get; set; }
        public async Task<DataSet> ContactAddEdit()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@MODE", Mode),
                    new SqlParameter("@ContactID", EditContactID),
                    new SqlParameter("@strContactTitle", ContactTitle),
                    new SqlParameter("@bitIsStatus", IsStatus),
                    new SqlParameter("@bitIsMainAddress", IsMainAddress),
                    new SqlParameter("@intFkBranchID", FkBranchID),
                    new SqlParameter("@intSequnceNo", SequnceNo),
                    new SqlParameter("@strMobileNo", MobileNo),
                    new SqlParameter("@strLocation", Location),
                    new SqlParameter("@strEmailId", EmailId),
                    new SqlParameter("@intAutoId", bigintCreatedBy),
                };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.ContactAddEdit, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<DataSet> GetContact()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@intPkContactID", EditContactID),
                    new SqlParameter("@PAGENO", PageNo),
                    new SqlParameter("@PAGESIZE", PageSize),
                };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.GetContact, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
    public class MobileDto
    {
        public string mobile { get; set; }
    }

    public class EmailDto
    {
        public string email { get; set; }
    }
}
