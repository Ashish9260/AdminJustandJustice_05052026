using AdminJustAndJustice.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Data;
using System.Security.Claims;

namespace AdminJustAndJustice.Controllers
{
    public class AccountController : Controller
    {

        public IActionResult Index()
        {
            return View(new LoginModel());
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            string _error = "";
            string _mess = "";
            try
            {
                DataSet ds = await model.Login();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["code"].ToString() == "0")
                    {

                        var _userinfo = new UserInfo
                        {
                            LogintTime = DateTime.Now.ToString("dd MMM yyyy HH:mm"),
                            AutoID = Convert.ToString(ds.Tables[0].Rows[0]["AutoID"]),
                            LoginID = Convert.ToString(ds.Tables[0].Rows[0]["LoginID"]),
                            RoleID = Convert.ToString(ds.Tables[0].Rows[0]["RoleID"]),
                            DisplayName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]),
                            Mobile = Convert.ToString(ds.Tables[0].Rows[0]["Phone"]),
                            BranchID = Convert.ToString(ds.Tables[0].Rows[0]["BranchID"]),
                            BranchCode = Convert.ToString(ds.Tables[0].Rows[0]["BranchCode"]),
                            RoleName = Convert.ToString(ds.Tables[0].Rows[0]["RoleName"]),
                            Designation = Convert.ToString(ds.Tables[0].Rows[0]["Designation"]),
                            Password = Convert.ToString(model.Password),
                            IsValidSession = false,
                            IsPermitted = false
                        };
                        //    HttpContext.Session.SetString("UserProfilePic", Convert.ToString(ds.Tables[0].Rows[0]["ProfilePic"]));
                        //HttpContext.Session.SetString("Designation", Convert.ToString(ds.Tables[0].Rows[0]["Designation"]));
                        //HttpContext.Session.SetString("UserName", Convert.ToString(ds.Tables[0].Rows[0]["UserName"]));
                        //HttpContext.Session.SetString("UserProfilePic", Convert.ToString(ds.Tables[0].Rows[0]["strProfilePic"]));
                        //DataTable tmpMenu = new DataTable();
                        //tmpMenu = ds.Tables[1];
                        //List<UserMenuModels> _menu = new List<UserMenuModels>();
                        ////foreach (DataRow item in tmpMenu.Rows)
                        //{
                        //    var _userMenu = new UserMenuModels
                        //    {
                        //        MainMenuName = Convert.ToString(item["strMenuName"]),
                        //        MainMenuId = Convert.ToString(item["intMenuID"]),
                        //        SubMenuName = Convert.ToString(item["strSubMenuName"]),
                        //        SubMenuId = Convert.ToString(item["intSubMenuID"]),
                        //        ControllerName = Convert.ToString(item["strControllerName"]),
                        //        ActionName = Convert.ToString(item["strActionName"]),
                        //        RoleId = Convert.ToString(item["intPk_RoleId"]),
                        //        RoleName = Convert.ToString(item["strRoleName"]),
                        //        isDisplayItem = Convert.ToString(item["IsDisplayMenuItem"]),
                        //        MenuIcon = Convert.ToString(item["strFaicon"])
                        //    };
                        //    _menu.Add(_userMenu);
                        //}

                        Response.Cookies.Append(
                         "AUTH",
                         "",
                         new CookieOptions
                         {
                             Expires = DateTimeOffset.UtcNow.AddMinutes(60),
                             HttpOnly = true,
                             Secure = true,
                             SameSite = SameSiteMode.Strict
                         });
                        UserInfo loginReponseDto = new UserInfo()
                        {

                            LogintTime = DateTime.Now.ToString("dd MMM yyyy HH:mm"),
                            AutoID = Convert.ToString(ds.Tables[0].Rows[0]["AutoID"]),
                            LoginID = Convert.ToString(ds.Tables[0].Rows[0]["LoginID"]),
                            RoleID = Convert.ToString(ds.Tables[0].Rows[0]["RoleID"]),
                            DisplayName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]),
                            Mobile = Convert.ToString(ds.Tables[0].Rows[0]["Phone"]),
                            BranchID = Convert.ToString(ds.Tables[0].Rows[0]["BranchID"]),
                            BranchCode = Convert.ToString(ds.Tables[0].Rows[0]["BranchCode"]),
                            RoleName = Convert.ToString(ds.Tables[0].Rows[0]["RoleName"]),
                            Designation = Convert.ToString(ds.Tables[0].Rows[0]["Designation"]),
                            Password = Convert.ToString(model.Password),
                        };
                        var json = JsonSerializer.Serialize(loginReponseDto);
                        var encJSON = Crypto.Encrypt(json);
                        Response.Cookies.Append(
                        "AUTH",
                        encJSON,
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddMinutes(60),
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        });
                        var user = AuthCookieHelper.GetAuthCookie(Request);
                        var claims = new List<Claim>
                            {
                                new Claim("UserId", loginReponseDto.AutoID),
                                new Claim("LoginID", loginReponseDto.LoginID),
                                new Claim(ClaimTypes.Name, loginReponseDto.DisplayName),
                               new Claim(ClaimTypes.Role, loginReponseDto.RoleName),
                                //new Claim("designation", loginReponseDto.designation)
                            };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);



                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            principal,
                            new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
                            }
                        );


                        TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return View("Index", model);

                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong!";
                    return View("Index", model);

                }
            }
            catch (Exception Ex)
            {

                TempData["ErrorMessage"] = Ex.Message;
                return View("Index", model);
            }

        }

        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("AUTH");
            HttpContext.Session.Clear();
            var claim = User.FindFirst(ClaimTypes.Role).Value;
            return RedirectToAction("Index");
        }
    }
}
