

using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace AdminJustAndJustice.Models
{


    public class CommonModel
    {
        public Pager? Pager { get; set; }
        public int? PageNo { get; set; }
        public int PageSize { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? LoginId { get; set; }
        public string? Password { get; set; }
        public string? AddedBy { get; set; }
        public string? OpCode { get; set; }
        public string? Value { get; set; }
        public string? Value1 { get; set; }
        public string? SearchParam { get; set; }
        public string? btnSubmit { get; set; }
        public string? AppVersion { get; set; }
        public string? DeviceType { get; set; }
        public string? LoginToken { get; set; }
        public string? DeviceId { get; set; }
        public string? Fk_CustomerId { get; set; }
        public string? UserType { get; set; }
        public string? OldImageURL { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? FilePath { get; set; }
        public string? Mode { get; set; }
        public string? intFkBranchId { get; set; }
        public string? bigintCreatedBy { get; set; }
        public string? Flag { get; set; }
        public IFormFile? fileUpload { get; set; }
        public IFormFile? fileUpload1 { get; set; }

        public DataSet GetMasterData()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@Value", Value),
                    new SqlParameter("@Value1", Value1),
                    new SqlParameter("@OpCode", OpCode),
                    new SqlParameter("@intFkBranchId", intFkBranchId),
                };
                DataSet ds = DBConnection.ExecuteQuery(Procedure.GetMasterData, para);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataSet> GetMobileDropDown()
        {
            try
            {
                SqlParameter[] para =
                {
                    new SqlParameter("@Value", Value),
                    new SqlParameter("@Flag", Flag),
                    new SqlParameter("@LoginToken", LoginToken),
                    new SqlParameter("@AppVersion", AppVersion),
                    new SqlParameter("@DeviceType", DeviceType),
                    new SqlParameter("@DeviceId", DeviceId),
                    new SqlParameter("@Fk_CustomerId", Fk_CustomerId),
                };
                DataSet ds = await DBConnection.ExecuteQueryAsync(Procedure.GetMobileDropDown, para);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task<DataSet> GetWebDropDownData()
        //{
        //    try
        //    {
        //        SqlParameter[] para =
        //        {
        //            new SqlParameter("@Value", Value),
        //            new SqlParameter("@Flag", Flag),
        //            new SqlParameter("@LoginToken", LoginToken),
        //        };
        //        DataSet ds =await DBConnection.ExecuteQueryAsync(Procedure.WebDropDownData, para);
        //        return ds;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public static string ConvertToSystemDate(string InputDate, string InputFormat)
        {
            string[] DatePart = InputDate.Split(new string[] { "-", @"/" }, StringSplitOptions.None);

            string DateString;
            if (InputFormat == "dd-MM-yyyy" || InputFormat == "dd/MM/yyyy" || InputFormat == "dd/MM/yyyy" || InputFormat == "dd-MM-yyyy" || InputFormat == "DD/MM/YYYY" || InputFormat == "dd/mm/yyyy")
            {
                string Day = DatePart[0];
                string Month = DatePart[1];
                string Year = DatePart[2];

                if (Month.Length > 2)
                    DateString = InputDate;
                else
                    DateString = Year + "-" + Month + "-" + Day;
            }
            else if (InputFormat == "MM/dd/yyyy" || InputFormat == "MM-dd-yyyy")
            {
                DateString = InputDate;
            }
            else
            {
                throw new Exception("Invalid Date");
            }

            try
            {

                return DateString;
            }
            catch
            {
                throw new Exception("Invalid Date");
            }
        }

        //public static string HITAPI(string APIurl, string body)
        //{
        //    var result = "";
        //    var httpWebRequest = (HttpWebRequest)WebRequest.Create(APIurl);
        //    httpWebRequest.ContentType = "application/json";
        //    httpWebRequest.Method = "POST";
        //    using (var streamwriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //    {
        //        string json = new JavaScriptSerializer().Serialize(new
        //        {
        //            body = body

        //        });
        //        streamwriter.Write(json);
        //    }
        //    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //    {
        //        result = streamReader.ReadToEnd();
        //    }
        //    return result;
        //}

        public static string HITAPI(string APIurl)
        {
            var result = "";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(APIurl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }

        public static string GenerateRandom()
        {
            Random r = new Random();
            string s = "";
            for (int i = 0; i < 6; i++)
            {
                s = string.Concat(s, r.Next(10).ToString());
            }
            return s;
        }

        public static string GenerateRandomAlphanumeric(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void LogError(Exception ex, string requestUrl, string pagemessage)
        {
            var line = Environment.NewLine;
            var ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
            var Errormsg = ex.GetType().Name.ToString();
            var extype = ex.GetType().ToString();
            var ErrorLocation = ex.Message.ToString();
            try
            {
                string fileDir = "ErrorLog/";
                string filepath = Path.Combine(fileDir);
                if (!Directory.Exists(filepath)) { Directory.CreateDirectory(filepath); }

                filepath = filepath + DateTime.Today.ToString("dd-MM-yy") + ".txt";  //Text File Name

                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line + "Error Page Url:" + " " + requestUrl + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    if (pagemessage != "")
                    {
                        sw.WriteLine(pagemessage);
                    }
                    sw.WriteLine("--------------------------------End------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        public static string _getReturnMessage(string Flag, string Msg)
        {
            var result = "";
            if (Flag == "0")
            {
                //  result = "<div class='alert alert-success'>" + Msg +       SlimNotifierJs.notification('success', 'Success', response.message, 6000); "</div>";
                result = "<script>  SlimNotifierJs.notification('success', 'Success', '" + Msg + "', 6000); </script>";

            }
            else if (Flag == "2")
            {
                result = "<script>  SlimNotifierJs.notification('info', 'Info', '" + Msg + "', 6000); </script>";
                // result = "<script>Swal.fire({icon: 'info',title: 'Info', text: '" + Msg + ".',timer: 6000,timerProgressBar: true});</script>";
            }
            else
            {
                result = "<script>  SlimNotifierJs.notification('error', 'Error', '" + Msg + "', 6000); </script>";
                // result = "<script>Swal.fire({icon: 'error',title: 'Error', text: '" + Msg + ".',timer: 6000,timerProgressBar: true});</script>";
            }
            return result;
        }
    }

    public class ResponseModel
    {
        public string? Body { get; set; }
    }

    public class RequestModel
    {
        public string? Body { get; set; }
    }

    public class BaseUrl
    {
        public static string? Url = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("BaseUrl").Value;
        public static string? ImageUrl = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("BaseUrl").Value;
    }

    public class APIURL
    {
        public static string Login = BaseUrl.Url + "Login";
    }
    public class DropDownData
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
    public class CompanyDetails
    {
        public static string Name = "Auctech IT Solutions PVT. LTD.";
        public static string GSTIN = "09AAXCA1712F1ZM";
        public static string PAN = "AAXCA1712F";
        public static string BankName = "AXIS BANK";
        public static string BankBranch = "Aminabad Lucknow";
        public static string AccountNo = "923020064059154";
        public static string ISFCCode = "UTIB0004718";
        public static string RegAddress = "Plot No. 1A, 17A KHA NO. 3 8 9 10, Sarfarazganj, Balaganj, Lucknow-226003";
        public static string OfficeAddress = "Sai Heights, 2nd Floor, C-3/76, Vibhuti Khand, Gomti Nagar,Lucknow-226010";
        public static string Website = "auctechitsolutions.in";
        public static string Email = "auctechitsolutions.in";
    }

    public class Messages
    {
        public static string InvalidLoginId = "Invalid LoginId or Password";
        public static string NoRecordFound = "No Record Found";
        public static string ProblemInConnection = "Problem In Connection.Please try after sometime.";
        public static string Something = "Something went wrong!";

    }
    public class ModelResponse<T>
    {
        public T? Response { get; set; }
        public string? Message { get; set; }
        public int Status { get; set; }
    }
    public class Login
    {
        public string? Response { get; set; }
    }
    public class FileUploadResponse
    {
        public string? FilePath { get; set; }
    }
    public class DropDownResponse
    {
        public List<DropDownData>? dropDownData { get; set; }
    }
    public class PaymentGatewayDetails
    {
        //public static string Secret = "i75V13CBDvZXLbyEoD6vYgm3";
        //public static string Key = "rzp_live_bzGXkUW1C2NaaH";


        public static string Key = "rzp_test_Pf6yYXLR4FGYnM";
        public static string Secret = "VXxDsPTwmMoEZODJgIwrVHGc";

        public static string Currency = "INR";
        public static string Description = "Payment for RAV Organic";
        public static string Name = "RAV Customer";
        public static string Contact = "9205440544";
        public static string Email = "care@ravorganics.com";
        public static string Address = "Razorpay Corporate Office";
        public static string CallbackURL = "https://localhost:44359/api/RavOrganicApi/CallBackUrl";

    }
    [Serializable]
    public class Pager
    {
        public Pager(int? totalItems, int? page, int pageSize = 10)
        {
            // calculate total, start and end pages
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page != null ? (int)page : 1;
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
        public int? TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
    }

    public class SessionManager : IDisposable
    {
        public static int Size =>10;

        public void Dispose()
        {

        }


        //public DataTable UserPermissionDt
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session["loginId"] == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return (DataTable)HttpContext.Current.Session["Permissions"];
        //        }
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["Permissions"] = value;
        //    }
        //}
    }

    public class MPaging
    {
        public int? Page { get; set; }
        public int Size { get; set; }
        public int TotalRecords { get; set; }
        public string? SearchKey { get; set; }
        public string? SearchValue { get; set; }
        public Pager? Pager { get; set; }
        public int EndPage { get; private set; }
    }
}
