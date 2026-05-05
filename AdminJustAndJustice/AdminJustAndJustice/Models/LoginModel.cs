using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace AdminJustAndJustice.Models
{
    public class LoginModel
    {
        public string? LoginID { get; set; }
        public string? Password { get; set; }
        public async Task<DataSet> Login()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@LoginID", LoginID),
                    new SqlParameter("@Password", Password)
                };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.AdminLogin, para);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
    public class UserInfo
    {
        public string? LogintTime { get; set; }
        public string? AutoID { get; set; }
        public string? LoginID { get; set; }
        public string? BranchID { get; set; }
        public string? BranchCode { get; set; }
        public string? RoleID { get; set; }
        public string? RoleName { get; set; }
        public string? Designation { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Mobile { get; set; }
        public bool IsValidSession { get; set; }
        public bool IsPermitted { get; set; }
    }

    public class UserMenuModels
    {
        public string? MainMenuName { get; set; }
        public string? MainMenuId { get; set; }
        public string? SubMenuName { get; set; }
        public string? SubMenuId { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? isDisplayItem { get; set; }
        public string? MenuIcon { get; set; }

    }
}
