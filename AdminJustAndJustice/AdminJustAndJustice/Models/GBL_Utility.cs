using System.Text.Json;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace AdminJustAndJustice.Models
{
    public class ApiEncrypt_Decrypt
    {
        static string key = "BunaaiRugKey06hL";
        public static string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        public static string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText.Replace(" ",""));
            using (Aes aes = Aes.Create())
            {

                aes.KeySize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
    public class GBL_Utility
    {
        static string Cypher = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("cryptString").Value;

        #region Fields
        private static byte[] key = { };
        private static byte[] IV = { 38, 55, 206, 48, 28, 64, 20, 16 };

        private static string stringKey = Cypher;
        #endregion

        #region Public Methods

        public static string encryptStringPWD(string _params)
        {
            string cryptString = @"" + _params;
            string returnString;
            // get cypher string from web.config file
            string cypher = Cypher;
            // encrypt string and then replace instances of '&' so that the string does not break           

            returnString = Encrypt11(_params, cypher).Replace("&", "amp").Replace("/", "fwd").Replace("+", "pls").Replace("%", "per");

            // return value
            return returnString;
        }

        public static string decryptStringPWD(string _params)
        {
            string cryptString = _params.ToString().Replace(' ', '+').Replace("fwd", "/").Replace("pls", "+").Replace("%", "per");
            string returnString;
            // get cypher string from web.config file
            string cypher = Cypher;//
            // replace instances of "(~~)" with '&' to reverse what was done during encryption process
            returnString = Decrypt11(cryptString.Replace("amp", "&"), cypher);

            // return value
            return returnString;
        }

        #endregion

        #region - encrypt and decrypt methods -
        private static string Decrypt11(string stringToDecrypt, string sEncryptionKey)
        {
            byte[] key = { };
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray = new byte[stringToDecrypt.Length];

            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt);

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private static string Encrypt11(string stringToEncrypt, string sEncryptionKey)
        {
            byte[] key = { };
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray; //Convert.ToByte(stringToEncrypt.Length)

            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #endregion       


        public static List<Dictionary<string, object>> GetJsonFromTable(DataTable tmpDT)
        {
            List<Dictionary<string, object>> _Json = new List<Dictionary<string, object>>();
            if (tmpDT.Rows.Count > 0)
            {
                Dictionary<string, object> dictRow = null;
                foreach (DataRow dr in tmpDT.Rows)
                {
                    dictRow = new Dictionary<string, object>();
                    foreach (DataColumn col in tmpDT.Columns)
                    {
                        dictRow.Add(col.ColumnName, dr[col]);
                    }
                    _Json.Add(dictRow);
                }
            }
            return _Json;
        }

        public static string DDMMYYYYtoYYYYMMDD(string _date)
        {
            string[] old = _date.Split('-');
            return (old[2] + "-" + old[1] + "-" + old[0]);

        }
        public static void SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("tantrashtech@gmail.com", "ooqr ansu hymb asiy")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("pustakaalay@kezanconsulting.com", "Kezan Pustakalaya"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }
    }
    public static class FileManagement
    {
        public static async Task<string> WriteFiles(this IFormFile files, string FolderName, string FolderName1)
        {
            bool isSaveSuccess = false;

            string obj = "";
            string final = "";

            string fileName;
            try
            {
                var extension = "." + files.FileName.Split('.')[files.FileName.Split('.').Length - 1];
                fileName = FolderName1+"_" + DateTime.Now.Ticks + extension;
                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FolderName, FolderName1);

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(pathBuilt));
                }

                var path = Path.Combine("wwwroot", FolderName, FolderName1, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await files.CopyToAsync(stream);
                }

                obj =  "/" + FolderName + "/" + FolderName1 + "/" + fileName;

                isSaveSuccess = true;
            }
            catch (Exception ex)
            {
                obj = ex.Message;
                return obj;
            }


            return obj;
        }

    }

    public class MenuPermission
    {
        public string? intPermissionID { get; set; }
        public string? intMenuID { get; set; }
        public string? intPk_RoleId { get; set; }
        public string? intSubMenuID { get; set; }
        public string? strSubMenuName { get; set; }
        public string? strControllerName { get; set; }
        public string? strActionName { get; set; }
        public string? strMenuName { get; set; }
        public string? strRoleName { get; set; }
        public string? IsDisplayMenuItem { get; set; }
        public string? strFaicon { get; set; }
    }
    public class Encryption64
    {

        static string Cypher = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("cryptString").Value;

        #region Fields
        private static byte[] key = { };
        private static byte[] IV = { 38, 55, 206, 48, 28, 64, 20, 16 };

        private static string stringKey = Cypher;
        #endregion

        #region Public Methods

        public static string encryptStringPWD(string? _params)
        {
            string cryptString = @"" + _params;
            string returnString;
            // get cypher string from web.config file
            string cypher = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("cryptString").Value;
            // encrypt string and then replace instances of '&' so that the string does not break           

            returnString = Encrypt(_params, cypher).Replace("&", "amp").Replace("/", "fwd").Replace("+", "pls").Replace("%", "per");

            // return value
            return returnString;
        }

        public static string decryptStringPWD(string? _params)
        {
            string cryptString = _params.ToString().Replace(' ', '+').Replace("fwd", "/").Replace("pls", "+").Replace("%", "per");
            string returnString;
            // get cypher string from web.config file
            string cypher = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("cryptString").Value;
            // replace instances of "(~~)" with '&' to reverse what was done during encryption process
            returnString = Decrypt(cryptString.Replace("amp", "&"), cypher);

            // return value
            return returnString;
        }

        #endregion

        #region - encrypt and decrypt methods -
        public static string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {
            byte[] key = { };
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray = new byte[stringToDecrypt.Length];

            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt);

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static string Encrypt(string stringToEncrypt, string sEncryptionKey)
        {
            byte[] key = { };
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray; //Convert.ToByte(stringToEncrypt.Length)

            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #endregion       
    }
    public class Crypto
    {
        public static string GenerateRandomPassword()
        {
            int length = 8;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }
        public static string Encrypt(string? clearText)
        {
            try
            {
                string EncryptionKey = "ABCDEHJKLMNHBJKOAAAA";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch { clearText = ""; }
            return clearText;
        }
        public static string Decrypt(string? cipherText)
        {
            try
            {
                string EncryptionKey = "ABCDEHJKLMNHBJKOAAAA";
                cipherText = cipherText.Replace(" ", "+");
                cipherText = cipherText.Replace("%2F", "/");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex) { cipherText = ""; }
            return cipherText;

        }

        public static string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }

    public static class DecodeEncodeService
    {
        //Key="511a9bb83730ac9a30e449072daa5688af956f5936b65f7e3efbb5a87373dfb5";
        //IV="abcdef9876543210abcdef9876543210"; // 16 bytes for AES        
        #region "Send Data to Iemsme.in"
        public static string EncryptStringToBytes(string plainText, string keyHex, string ivHex)
        {
            byte[] key = Enumerable.Range(0, keyHex.Length)
                      .Where(x => x % 2 == 0)
                      .Select(x => Convert.ToByte(keyHex.Substring(x, 2), 16))
                      .ToArray();
            byte[] iv = Enumerable.Range(0, ivHex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(ivHex.Substring(x, 2), 16))
                     .ToArray();
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                    swEncrypt.Flush();
                    csEncrypt.FlushFinalBlock();
                    return System.Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        public static string DecryptStringFromBytes(string text, string keyHex, string ivHex)
        {
            byte[] cipherText = System.Convert.FromBase64String(text);
            byte[] key = Enumerable.Range(0, keyHex.Length)
                      .Where(x => x % 2 == 0)
                      .Select(x => Convert.ToByte(keyHex.Substring(x, 2), 16))
                      .ToArray();
            byte[] iv = Enumerable.Range(0, ivHex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(ivHex.Substring(x, 2), 16))
                     .ToArray();
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
        #endregion
    }
    public static class AuthCookieHelper
    {
        private const string CookieName = "AUTH";



        public static UserInfo? GetAuthCookie(HttpRequest request)
        {
            if (!request.Cookies.TryGetValue(CookieName, out var encrypted))
                return null;

            try
            {
                var json = Crypto.Decrypt(encrypted);
                return JsonSerializer.Deserialize<UserInfo>(json);
            }
            catch
            {

                return null;
            }
        }


        public static void ClearAuthCookie(HttpResponse response)
        {
            response.Cookies.Delete(CookieName);
        }
    }
}
