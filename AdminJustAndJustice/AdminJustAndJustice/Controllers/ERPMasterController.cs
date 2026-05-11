using AdminJustAndJustice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace AdminJustAndJustice.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ERPMasterController : Controller
    {
        #region ############################## Banner Master ##############################
        [Route("/BannerMaster")]
        public async Task<IActionResult> BannerMaster(ERPBannerModel _model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Account");
            }
            try
            {
                if (!string.IsNullOrEmpty(_model.EditBannerID))
                {
                    _model.EditBannerID = _model.EditBannerID;
                    _model.PageSize = 1;
                    DataSet ds = await _model.GetBanner();
                    _model.EditBannerID = Convert.ToString(ds.Tables[0].Rows[0]["BannerID"]);
                    _model.BannerTitle = Convert.ToString(ds.Tables[0].Rows[0]["strBannerTitle"]);
                    _model.BannerDetails = Convert.ToString(ds.Tables[0].Rows[0]["strDetials"]);
                    _model.ImageURL = Convert.ToString(ds.Tables[0].Rows[0]["strImageURL"]);
                    _model.VideoURL = Convert.ToString(ds.Tables[0].Rows[0]["strVideoURL"]);
                    _model.OldImageURL = Convert.ToString(ds.Tables[0].Rows[0]["strImageURL"]);
                    _model.SequnceNo = Convert.ToString(ds.Tables[0].Rows[0]["intSequnceNo"]);
                    _model.IsStatus = Convert.ToString(ds.Tables[0].Rows[0]["IsStatus"]);

                }
                return View(_model);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _model.ImageURL = _model.OldImageURL;
                return View("BannerMaster", _model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAndUpdateBanner([FromForm] ERPBannerModel model, string btn_Add)
        {
            var claim = Convert.ToString(User.FindFirst("UserId").Value);
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                model.BannerTitle = !string.IsNullOrEmpty(model.BannerTitle) ? model.BannerTitle.Trim() : null;
                model.BannerDetails = !string.IsNullOrEmpty(model.BannerDetails) ? model.BannerDetails.Trim() : null;
                model.SequnceNo = !string.IsNullOrEmpty(model.SequnceNo) ? model.SequnceNo.Trim() : "0";
                if (string.IsNullOrEmpty(model.BannerTitle) || string.IsNullOrWhiteSpace(model.BannerTitle))
                {
                    TempData["ErrorMessage"] = "Please Enter Banner Title!";
                    model.ImageURL = model.OldImageURL;
                    return View("BannerMaster", model);
                }
                if (string.IsNullOrEmpty(model.BannerDetails) || string.IsNullOrWhiteSpace(model.BannerDetails))
                {
                    TempData["ErrorMessage"] = "Please Enter Banner Details!";
                    model.ImageURL = model.OldImageURL;
                    return View("BannerMaster", model);
                }
                model.intFkBranchId = "0";
                model.bigintCreatedBy = claim;
                if (btn_Add == "Save")
                {
                    model.Mode = "INSERT";
                }
                else
                {
                    //if (!string.IsNullOrEmpty(model.EditBannerID))
                    //{
                    //    model.EditBannerID = GBL_Utility.decryptStringPWD(model.EditBannerID);
                    //}
                    model.Mode = "UPDATE";
                }
                if (model.fileUpload != null && model.fileUpload.Length > 0)
                {
                    if (model.fileUpload.Length > 2 * 1048576)
                    {
                        TempData["ErrorMessage"] = "Image size must not exceed 2MB.";
                        model.ImageURL = model.OldImageURL;
                        return View("BannerMaster", model);
                    }
                    using (var img = System.Drawing.Image.FromStream(model.fileUpload.OpenReadStream()))
                    {
                        if (img.Width > 2000 || img.Height > 2000)
                        {
                            TempData["ErrorMessage"] = "Image dimensions must not exceed 2000x2000.";
                            model.ImageURL = model.OldImageURL;
                            return View("BannerMaster", model);
                        }
                    }
                    string fileLocation = await FileManagement.WriteFiles(model.fileUpload, "Banner", "BannerImage");
                    model.ImageURL = fileLocation;
                }
                else
                {
                    if (btn_Add == "Save")
                    {
                        TempData["ErrorMessage"] = "Please Select Image File!";
                        model.ImageURL = model.OldImageURL;
                        return View("BannerMaster", model);
                    }
                }
                DataSet ds = await model.BannerAddEdit();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["code"]) == "0")
                    {
                        TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("BannerList", "ERPMaster");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        model.ImageURL = model.OldImageURL;
                        return View("BannerMaster", model);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong!";
                    model.ImageURL = model.OldImageURL;
                    return View("BannerMaster", model);
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                model.ImageURL = model.OldImageURL;
                return View("BannerMaster", model);
            }
        }
        [Route("/BannerList")]
        public async Task<IActionResult> BannerList(ERPBannerModel _model)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                _model.PageSize = SessionManager.Size;
                DataSet ds = await _model.GetBanner();
                _model.dtBanner = ds.Tables[0];
                var totalRecords = 0;
                if (_model.dtBanner.Rows.Count > 0)
                {
                    totalRecords = Convert.ToInt32(_model.dtBanner.Rows[0]["TotalRecords"].ToString());
                    var pager = new Pager(totalRecords, _model.PageNo, SessionManager.Size);
                    _model.Pager = pager;
                }
                return View(_model);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("BannerList", _model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteBanner([FromForm] ERPBannerModel model, string btn_Add)
        {
            var claim = Convert.ToString(User.FindFirst("UserId"));
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                model.intFkBranchId = "0";
                model.bigintCreatedBy = claim;
                model.Mode = "DELETE";
                DataSet ds = await model.BannerAddEdit();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["Code"]) == "0")
                    {
                        TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["Remark"]);
                        return RedirectToAction("BannerList", "ERPMaster");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["Remark"]);
                        return RedirectToAction("BannerList", "ERPMaster");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong!";
                }
                return RedirectToAction("BannerList", "ERPMaster");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("BannerList", "ERPMaster");
            }
        }
        #endregion ############################## Banner Master ##############################

        #region ############################## Case Type  Master ##############################
        [Route("/ContactMaster")]
        public async Task<IActionResult> ContactMaster(ContactModel _model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Account");
            }
            try
            {
                if (!string.IsNullOrEmpty(_model.EditContactID))
                {
                    _model.EditContactID = _model.EditContactID;
                    _model.PageSize = 1;
                    DataSet ds = await _model.GetContact();
                    _model.EditContactID = Convert.ToString(ds.Tables[0].Rows[0]["ContactID"]);
                    _model.ContactTitle = Convert.ToString(ds.Tables[0].Rows[0]["strContactTitle"]);
                    _model.MobileNo = Convert.ToString(ds.Tables[0].Rows[0]["strMobileNo"]);
                    _model.Location = Convert.ToString(ds.Tables[0].Rows[0]["strLocation"]);
                    _model.EmailId = Convert.ToString(ds.Tables[0].Rows[0]["strEmailId"]);
                    _model.SequnceNo = Convert.ToString(ds.Tables[0].Rows[0]["intSequnceNo"]);
                    _model.IsMainAddress = Convert.ToString(ds.Tables[0].Rows[0]["IsMainAddress"]);

                }
                return View(_model);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("ContactMaster", _model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAndUpdateContact([FromForm] ContactModel model, string btn_Add)
        {
            var claim = Convert.ToString(User.FindFirst("UserId").Value);
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                model.ContactTitle = !string.IsNullOrEmpty(model.ContactTitle) ? model.ContactTitle.Trim() : null;
                model.SequnceNo = !string.IsNullOrEmpty(model.SequnceNo) ? model.SequnceNo.Trim() : "0";
                var mobiles = model.MobileNos?
                .Where(x => !string.IsNullOrWhiteSpace(x) || !string.IsNullOrEmpty(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();
                var emails = model.Emails?
                 .Where(x => !string.IsNullOrWhiteSpace(x) || !string.IsNullOrEmpty(x))
                 .Select(x => x.Trim().ToLower())
                 .Distinct()
                 .ToList();
                if (string.IsNullOrEmpty(model.ContactTitle) || string.IsNullOrWhiteSpace(model.ContactTitle))
                {
                    TempData["ErrorMessage"] = "Please Enter Contact Title!";
                    return View("ContactMaster", model);
                }
                if (mobiles == null)
                {
                    TempData["ErrorMessage"] = "Please enter at least one mobile number!";
                    return View("ContactMaster", model);
                }
                if (mobiles.Count == 0)
                {
                    TempData["ErrorMessage"] = "Please enter at least one mobile number!";
                    return View("ContactMaster", model);
                }
                if (emails == null)
                {
                    TempData["ErrorMessage"] = "Please enter at least one email!";
                    return View("ContactMaster", model);
                }
                if (emails.Count == 0)
                {
                    TempData["ErrorMessage"] = "Please enter at least one email!";
                    return View("ContactMaster", model);
                }
                if (model.MobileNos != null && model.MobileNos.Count != model.MobileNos.Distinct().Count())
                {
                    TempData["ErrorMessage"] = "Duplicate mobile numbers are not allowed!";
                    return View("ContactMaster", model);
                }

                if (model.Emails != null && model.Emails.Count != model.Emails.Distinct().Count())
                {
                    TempData["ErrorMessage"] = "Duplicate emails are not allowed!";
                    return View("ContactMaster", model);
                }
                var mobileList = mobiles.Select(x => new MobileDto
                {
                    mobile = x
                }).ToList();

                var emailList = emails.Select(x => new EmailDto
                {
                    email = x
                }).ToList();
                model.MobileNo = JsonConvert.SerializeObject(mobileList);
                model.EmailId = JsonConvert.SerializeObject(emailList);
                if (string.IsNullOrEmpty(model.SequnceNo) || string.IsNullOrWhiteSpace(model.SequnceNo))
                {
                    TempData["ErrorMessage"] = "Please Enter Sequnce No!";
                    return View("ContactMaster", model);
                }
                if (string.IsNullOrEmpty(model.Location) || string.IsNullOrWhiteSpace(model.Location))
                {
                    TempData["ErrorMessage"] = "Please Enter Location!";
                    return View("ContactMaster", model);
                }
                model.intFkBranchId = "0";
                model.bigintCreatedBy = claim;
                if (btn_Add == "Save")
                {
                    model.Mode = "INSERT";
                }
                else
                {
                    //if (!string.IsNullOrEmpty(model.EditBannerID))
                    //{
                    //    model.EditBannerID = GBL_Utility.decryptStringPWD(model.EditBannerID);
                    //}
                    model.Mode = "UPDATE";
                }

                DataSet ds = await model.ContactAddEdit();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["code"]) == "0")
                    {
                        TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("ContactList", "ERPMaster");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return View("ContactMaster", model);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong!";
                    return View("ContactMaster", model);
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("ContactMaster", model);
            }
        }
        [Route("/ContactList")]
        public async Task<IActionResult> ContactList(ContactModel _model)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                _model.PageSize = SessionManager.Size;
                DataSet ds = await _model.GetContact();
                _model.dtContact = ds.Tables[0];
                var totalRecords = 0;
                if (_model.dtContact.Rows.Count > 0)
                {
                    totalRecords = Convert.ToInt32(_model.dtContact.Rows[0]["TotalRecords"].ToString());
                    var pager = new Pager(totalRecords, _model.PageNo, SessionManager.Size);
                    _model.Pager = pager;
                }
                return View(_model);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("ContactList", _model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteContact([FromForm] ContactModel model)
        {
            var claim = Convert.ToString(User.FindFirst("UserId").Value);
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                model.intFkBranchId = "0";
                model.bigintCreatedBy = claim;
                model.Mode = "DELETE";
                DataSet ds = await model.ContactAddEdit();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["code"]) == "0")
                    {
                        TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("ContactList", "ERPMaster");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("ContactList", "ERPMaster");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong!";
                }
                return RedirectToAction("ContactList", "ERPMaster");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("ContactList", "ERPMaster");
            }
        }
        #endregion ############################## Contact Master ##############################

        #region ############################## Case Type Master ##############################
        [Route("/CaseTypeMaster")]
        public async Task<IActionResult> CaseTypeMaster(CaseTypeModel _model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Account");
            }
            try
            {
                if (!string.IsNullOrEmpty(_model.EditCaseTypeID))
                {
                    _model.EditCaseTypeID = _model.EditCaseTypeID;
                    _model.PageSize = 1;
                    DataSet ds = await _model.GetCaseType();
                    _model.EditCaseTypeID = Convert.ToString(ds.Tables[0].Rows[0]["CaseTypeID"]);
                    _model.CaseType = Convert.ToString(ds.Tables[0].Rows[0]["strCaseType"]);
                    _model.ShortCaseType = Convert.ToString(ds.Tables[0].Rows[0]["strShortCaseType"]);
                    _model.Detials = Convert.ToString(ds.Tables[0].Rows[0]["strDetials"]);
                    _model.SequenceNo = Convert.ToString(ds.Tables[0].Rows[0]["intSequenceNo"]);
                    _model.URLText = Convert.ToString(ds.Tables[0].Rows[0]["strURLText"]);
                    _model.FirstImage = Convert.ToString(ds.Tables[0].Rows[0]["strFirstImage"]);
                    _model.OldImageURL = Convert.ToString(ds.Tables[0].Rows[0]["strFirstImage"]);
                    _model.Tag = Convert.ToString(ds.Tables[0].Rows[0]["strTags"]);
                    _model.SeoTitle = Convert.ToString(ds.Tables[0].Rows[0]["strSeoTitle"]);
                    _model.SeoKeyword = Convert.ToString(ds.Tables[0].Rows[0]["strSeoKeywords"]);
                    _model.SeoDescr = Convert.ToString(ds.Tables[0].Rows[0]["strSeoDescr"]);

                }
                return View(_model);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("CaseTypeMaster", _model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAndUpdateCaseType([FromForm] CaseTypeModel model, string btn_Add)

        {
            var claim = Convert.ToString(User.FindFirst("UserId").Value);
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                if (model.fileUpload != null && model.fileUpload.Length > 0)
                {
                    if (model.fileUpload.Length > 1 * 1048576)
                    {
                        TempData["ErrorMessage"] = "Image size must not exceed 1MB.";
                        model.FirstImage = model.OldImageURL;
                        return View("CaseTypeMaster", model);
                    }
                    using (var img = System.Drawing.Image.FromStream(model.fileUpload.OpenReadStream()))
                    {
                        if (img.Width > 1000 || img.Height > 1000)
                        {
                            TempData["ErrorMessage"] = "Image dimensions must not exceed 1000x1000.";
                            model.FirstImage = model.OldImageURL;
                            return View("CaseTypeMaster", model);
                        }
                    }
                    string fileLocation = await FileManagement.WriteFiles(model.fileUpload, "CaseType", "CaseTypeImage");
                    model.FirstImage = fileLocation;
                }
                else
                {
                    if (btn_Add == "Save")
                    {
                        TempData["ErrorMessage"] = "Please Select Image File!";
                        model.FirstImage = model.OldImageURL;
                        return View("CaseTypeMaster", model);
                    }
                }
                model.CaseType = !string.IsNullOrEmpty(model.CaseType) ? model.CaseType.Trim() : null;
                model.ShortCaseType = !string.IsNullOrEmpty(model.ShortCaseType) ? model.ShortCaseType.Trim() : null;
                model.SequenceNo = !string.IsNullOrEmpty(model.SequenceNo) ? model.SequenceNo.Trim() : "0";
                var seoKeywords = model.SeoKeywords?
                .Where(x => !string.IsNullOrWhiteSpace(x) || !string.IsNullOrEmpty(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();
                var tags = model.Tags?
                 .Where(x => !string.IsNullOrWhiteSpace(x) || !string.IsNullOrEmpty(x))
                 .Select(x => x.Trim().ToLower())
                 .Distinct()
                 .ToList();
                if (string.IsNullOrEmpty(model.CaseType) || string.IsNullOrWhiteSpace(model.CaseType))
                {
                    TempData["ErrorMessage"] = "Please Enter Case Type!";
                    return View("CaseTypeMaster", model);
                }
                if (string.IsNullOrEmpty(model.ShortCaseType) || string.IsNullOrWhiteSpace(model.ShortCaseType))
                {
                    TempData["ErrorMessage"] = "Please Enter Short Case Type!";
                    return View("CaseTypeMaster", model);
                }
                if (seoKeywords == null)
                {
                    TempData["ErrorMessage"] = "Please enter at least one Seo Keyword!";
                    return View("CaseTypeMaster", model);
                }
                if (tags.Count == 0)
                {
                    TempData["ErrorMessage"] = "Please enter at least one tag!";
                    return View("CaseTypeMaster", model);
                }
                if (seoKeywords == null)
                {
                    TempData["ErrorMessage"] = "Please enter at least one Seo Keyword!";
                    return View("CaseTypeMaster", model);
                }
                if (seoKeywords.Count == 0)
                {
                    TempData["ErrorMessage"] = "Please enter at least one Seo Keyword!";
                    return View("CaseTypeMaster", model);
                }
                if (model.Tags != null && model.Tags.Count != model.Tags.Distinct().Count())
                {
                    TempData["ErrorMessage"] = "Duplicate Tags are not allowed!";
                    return View("CaseTypeMaster", model);
                }

                if (model.SeoKeywords != null && model.SeoKeywords.Count != model.SeoKeywords.Distinct().Count())
                {
                    TempData["ErrorMessage"] = "Duplicate Seo Keyword are not allowed!";
                    return View("CaseTypeMaster", model);
                }
                var tagList = tags.Select(x => new SeoTagsDto
                {
                    seotags = x
                }).ToList();

                var keywordList = seoKeywords.Select(x => new SeoKeywordsDto
                {
                    seoKeywords = x
                }).ToList();
                model.Tag = JsonConvert.SerializeObject(tagList);
                model.SeoKeyword = JsonConvert.SerializeObject(keywordList);
                if (string.IsNullOrEmpty(model.SequenceNo) || string.IsNullOrWhiteSpace(model.SequenceNo))
                {
                    TempData["ErrorMessage"] = "Please Enter Sequence No!";
                    return View("CaseTypeMaster", model);
                }
                if (string.IsNullOrEmpty(model.Detials) || string.IsNullOrWhiteSpace(model.Detials))
                {
                    TempData["ErrorMessage"] = "Please Enter Detials!";
                    return View("CaseTypeMaster", model);
                }
                if (string.IsNullOrEmpty(model.SeoTitle) || string.IsNullOrWhiteSpace(model.SeoTitle))
                {
                    TempData["ErrorMessage"] = "Please Enter Seo Title!";
                    return View("CaseTypeMaster", model);
                }
                if (string.IsNullOrEmpty(model.SeoDescr) || string.IsNullOrWhiteSpace(model.SeoDescr))
                {
                    TempData["ErrorMessage"] = "Please Enter Seo Description!";
                    return View("CaseTypeMaster", model);
                }
                model.intFkBranchId = "0";
                model.bigintCreatedBy = claim;
                if (btn_Add == "Save")
                {
                    model.Mode = "INSERT";
                }
                else
                {
                    //if (!string.IsNullOrEmpty(model.EditBannerID))
                    //{
                    //    model.EditBannerID = GBL_Utility.decryptStringPWD(model.EditBannerID);
                    //}
                    model.Mode = "UPDATE";
                }

                DataSet ds = await model.CaseTypeAddEdit();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["code"]) == "0")
                    {
                        TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("CaseTypeList", "ERPMaster");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return View("CaseTypeMaster", model);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong!";
                    return View("CaseTypeMaster", model);
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("CaseTypeMaster", model);
            }
        }
        [Route("/CaseTypeList")]
        public async Task<IActionResult> CaseTypeList(CaseTypeModel _model)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                _model.PageSize = SessionManager.Size;
                DataSet ds = await _model.GetCaseType();
                _model.dtCaseType = ds.Tables[0];
                var totalRecords = 0;
                if (_model.dtCaseType.Rows.Count > 0)
                {
                    totalRecords = Convert.ToInt32(_model.dtCaseType.Rows[0]["TotalRecords"].ToString());
                    var pager = new Pager(totalRecords, _model.PageNo, SessionManager.Size);
                    _model.Pager = pager;
                }
                return View(_model);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("CaseTypeList", _model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCaseType([FromForm] CaseTypeModel model)
        {
            var claim = Convert.ToString(User.FindFirst("UserId").Value);
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                model.intFkBranchId = "0";
                model.bigintCreatedBy = claim;
                model.Mode = "DELETE";
                DataSet ds = await model.CaseTypeAddEdit();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["code"]) == "0")
                    {
                        TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("CaseTypeList", "ERPMaster");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("CaseTypeList", "ERPMaster");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong!";
                }
                return RedirectToAction("CaseTypeList", "ERPMaster");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("ContactList", "ERPMaster");
            }
        }
        #endregion ############################## Case Type Master ##############################
        #region ############################## Blog Master ##############################
        [Route("/BlogList")]
        public async Task<IActionResult> BlogList(ERPBlogModel _model)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }

                _model.PageSize = SessionManager.Size;
                DataSet ds = await _model.GetBlogList();
                _model.dtProductEnquiry = ds.Tables[0];
                var totalRecords = 0;
                if (_model.dtProductEnquiry.Rows.Count > 0)
                {
                    totalRecords = Convert.ToInt32(_model.dtProductEnquiry.Rows[0]["TotalRecords"].ToString());
                    var pager = new Pager(totalRecords, _model.PageNo, SessionManager.Size);
                    _model.Pager = pager;
                }

                return View(_model);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("BlogList", _model);
            }
        }
        [Route("/BlogMaster")]
        public async Task<IActionResult> BlogMaster(ERPBlogModel _model)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                #region ddlCaseType
                List<SelectListItem> ddlCaseType = new List<SelectListItem>();
                _model.OpCode = "1";
                DataSet dsDDL1 = _model.GetMasterData();
                if (dsDDL1 != null && dsDDL1.Tables.Count > 0)
                {
                    if (dsDDL1.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow item in dsDDL1.Tables[0].Rows)
                        {
                            ddlCaseType.Add(new SelectListItem { Value = item["Id"].ToString(), Text = item["Name"].ToString() });
                        }
                    }
                }
                ViewBag.ddlCaseType = ddlCaseType;
                #endregion ddlCaseType
                if (!string.IsNullOrEmpty(_model.EditPkBlogID))
                {
                    _model.PageSize = SessionManager.Size;
                    _model.PageNo = 1;
                    DataSet ds = await _model.GetBlogList();
                    _model.Author = Convert.ToString(ds.Tables[0].Rows[0]["strAuthor"]);
                    _model.BlogTitle = Convert.ToString(ds.Tables[0].Rows[0]["strBlogTitle"]);
                    _model.URLtext = Convert.ToString(ds.Tables[0].Rows[0]["strURLText"]);
                    _model.PkBlogID = Convert.ToString(ds.Tables[0].Rows[0]["intBlogID"]);
                    _model.intFkCaseTypeId = Convert.ToString(ds.Tables[0].Rows[0]["intFkCaseTypeId"]);
                    _model.FirstImgURL = Convert.ToString(ds.Tables[0].Rows[0]["strFirstImage"]);
                    _model.OldImageURL = Convert.ToString(ds.Tables[0].Rows[0]["strFirstImage"]);
                    _model.Category = Convert.ToString(ds.Tables[0].Rows[0]["strBlogCategory"]);
                    _model.FirstDescr = Convert.ToString(ds.Tables[0].Rows[0]["strFirstContent"]);
                    _model.Tag = Convert.ToString(ds.Tables[0].Rows[0]["strTags"]);
                    _model.SEOTitle = Convert.ToString(ds.Tables[0].Rows[0]["strSeoTitle"]);
                    _model.SEODescr = Convert.ToString(ds.Tables[0].Rows[0]["strSeoDescr"]);
                    _model.SeoKeyword = Convert.ToString(ds.Tables[0].Rows[0]["strSeoKeywords"]);
                }
                return View(_model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("BlogMaster", _model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAndUpdateBlog([FromForm] ERPBlogModel model, string btn_Add)
        {
            try
            {
                var claim = Convert.ToString(User.FindFirst("UserId").Value);
                model.intFkBranchId = "0";
                model.bigintCreatedBy = claim;

                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                model.PkBlogID = !string.IsNullOrEmpty(model.PkBlogID) ? model.PkBlogID.Trim() : null;
                model.BlogTitle = !string.IsNullOrEmpty(model.BlogTitle) ? model.BlogTitle.Trim() : null;
                model.URLtext = !string.IsNullOrEmpty(model.URLtext) ? model.URLtext.Trim() : null;
                model.Category = !string.IsNullOrEmpty(model.Category) ? model.Category.Trim() : null;
                model.FirstDescr = !string.IsNullOrEmpty(model.FirstDescr) ? model.FirstDescr.Trim() : null;
                model.FirstImgURL = !string.IsNullOrEmpty(model.FirstImgURL) ? model.FirstImgURL.Trim() : null;
                model.Author = !string.IsNullOrEmpty(model.Author) ? model.Author.Trim() : null;
                model.PublishStatus = !string.IsNullOrEmpty(model.PublishStatus) ? model.PublishStatus.Trim() : null;
                model.PublishOn = !string.IsNullOrEmpty(model.PublishOn) ? model.PublishOn.Trim() : null;
                model.SEOTitle = !string.IsNullOrEmpty(model.SEOTitle) ? model.SEOTitle.Trim() : null;
                model.SEODescr = !string.IsNullOrEmpty(model.SEODescr) ? model.SEODescr.Trim() : null;
                model.IsStatus = !string.IsNullOrEmpty(model.IsStatus) ? model.IsStatus.Trim() : null;
                if (model.fileUpload != null && model.fileUpload.Length > 0)
                {
                    if (model.fileUpload.Length > 1 * 1048576)
                    {
                        TempData["ErrorMessage"] = "Image size must not exceed 1MB.";
                        model.FirstImgURL = model.OldImageURL;
                        return View("BlogMaster", model);
                    }
                    using (var img = System.Drawing.Image.FromStream(model.fileUpload.OpenReadStream()))
                    {
                        if (img.Width > 1000 || img.Height > 1000)
                        {
                            TempData["ErrorMessage"] = "Image dimensions must not exceed 1000x1000.";
                            model.FirstImgURL = model.OldImageURL;
                            return View("BlogMaster", model);
                        }
                    }
                    string fileLocation = await FileManagement.WriteFiles(model.fileUpload, "CaseType", "CaseTypeImage");
                    model.FirstImgURL = fileLocation;
                }
                else
                {
                    if (btn_Add == "Save")
                    {
                        TempData["ErrorMessage"] = "Please Select Image File!";
                        model.FirstImgURL = model.OldImageURL;
                        return View("BlogMaster", model);
                    }
                }
                if (string.IsNullOrEmpty(model.Author))
                {
                    TempData["ErrorMessage"] = "Please Enter Author";
                    return View("BlogMaster", model);
                }
                if (string.IsNullOrEmpty(model.BlogTitle))
                {
                    TempData["ErrorMessage"] = "Please Enter Blog Title!";
                    return View("BlogMaster", model);
                }
                if (string.IsNullOrEmpty(model.URLtext))
                {
                    TempData["ErrorMessage"] = "Please Enter URL Text!";
                    return View("BlogMaster", model);
                }
                if (string.IsNullOrEmpty(model.Category))
                {
                    TempData["ErrorMessage"] = "Please Enter Category";
                    return View("BlogMaster", model);
                }
                if (string.IsNullOrEmpty(model.FirstDescr))
                {
                    TempData["ErrorMessage"] = "Please Enter First Description";
                    return View("BlogMaster", model);
                }

                if (string.IsNullOrEmpty(model.SEOTitle))
                {
                    TempData["ErrorMessage"] = "Please Enter SEO Title";
                    return View("BlogMaster", model);
                }
                if (string.IsNullOrEmpty(model.SEODescr))
                {
                    TempData["ErrorMessage"] = "Please Enter SEO Description";
                    return View("BlogMaster", model);
                }
                var seoKeywords = model.SeoKeywords?
                .Where(x => !string.IsNullOrWhiteSpace(x) || !string.IsNullOrEmpty(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();
                var tags = model.Tags?
                 .Where(x => !string.IsNullOrWhiteSpace(x) || !string.IsNullOrEmpty(x))
                 .Select(x => x.Trim().ToLower())
                 .Distinct()
                 .ToList();
                if (seoKeywords == null)
                {
                    TempData["ErrorMessage"] = "Please enter at least one Seo Keyword!";
                    return View("CaseTypeMaster", model);
                }
                if (tags.Count == 0)
                {
                    TempData["ErrorMessage"] = "Please enter at least one tag!";
                    return View("CaseTypeMaster", model);
                }
                if (seoKeywords == null)
                {
                    TempData["ErrorMessage"] = "Please enter at least one Seo Keyword!";
                    return View("CaseTypeMaster", model);
                }
                if (seoKeywords.Count == 0)
                {
                    TempData["ErrorMessage"] = "Please enter at least one Seo Keyword!";
                    return View("CaseTypeMaster", model);
                }
                if (model.Tags != null && model.Tags.Count != model.Tags.Distinct().Count())
                {
                    TempData["ErrorMessage"] = "Duplicate Tags are not allowed!";
                    return View("CaseTypeMaster", model);
                }

                if (model.SeoKeywords != null && model.SeoKeywords.Count != model.SeoKeywords.Distinct().Count())
                {
                    TempData["ErrorMessage"] = "Duplicate Seo Keyword are not allowed!";
                    return View("CaseTypeMaster", model);
                }
                var tagList = tags.Select(x => new SeoTagsDto
                {
                    seotags = x
                }).ToList();

                var keywordList = seoKeywords.Select(x => new SeoKeywordsDto
                {
                    seoKeywords = x
                }).ToList();
                model.Tag = JsonConvert.SerializeObject(tagList);
                model.SeoKeyword = JsonConvert.SerializeObject(keywordList);

                if (btn_Add == "Save")
                {
                    model.Mode = "INSERT";
                }
                else
                {
                    //if (!string.IsNullOrEmpty(model.EditPkBlogID))
                    //{
                    //    model.EditPkBlogID = GBL_Utility.decryptStringPWD(model.EditPkBlogID);
                    //}
                    model.Mode = "UPDATE";
                }
                DataSet ds = await model.AddEditDltBlog();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["code"]) == "0")
                    {
                        model.FirstImgURL = model.OldImageURL;
                        TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("BlogList", "ERPMaster");
                    }
                    else
                    {
                        model.FirstImgURL = model.OldImageURL;
                        TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return View("BlogMaster", model);
                    }
                }
                else
                {
                    model.FirstImgURL = model.OldImageURL;
                    TempData["ErrorMessage"] = "Something went wrong!";
                    return View("BlogMaster", model);
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("BlogMaster", model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteBlog([FromForm] ERPBlogModel model, string btn_Add)
        {
            var claim = Convert.ToString(User.FindFirst("UserId").Value);
            model.intFkBranchId = "0";
            model.bigintCreatedBy = claim;

            try
            {
                model.Mode = "DELETE";
                DataSet ds = await model.AddEditDltBlog();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["code"]) == "0")
                    {
                        TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("BlogList", "ERPMaster");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("BlogList", "ERPMaster");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong!";
                }
                return RedirectToAction("BlogList", "ERPMaster");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("BlogList", "ERPMaster");
            }
        }
        [HttpPost]
        public async Task<ActionResult> UpdateBlogPublish([FromForm] ERPBlogModel model)
        {
            string _error = "";
            string _mess = "";
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }

                DataSet ds = await model.UpdateBlogPublished();
                if (ds != null && ds.Tables.Count > 0)
                {
                    _error = Convert.ToString(ds.Tables[0].Rows[0]["code"]);
                    _mess = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                    return Json(new { ERROR = _error, MESSAGE = _mess });
                }
                else
                {
                    //TempData["ErrorMessage"] = "Something went wrong!";
                    //return View("ProductMaster", model);
                    _error = "1";
                    _mess = "Something went wrong!";
                    return Json(new { ERROR = _error, MESSAGE = _mess });
                }

            }
            catch (Exception ex)
            {
                //TempData["ErrorMessage"] = ex.Message;
                //return View("ProductMaster", model);
                _error = "1";
                _mess = ex.Message;
                return Json(new { ERROR = _error, MESSAGE = _mess });
            }
        }
        #endregion ############################## Blog Master ##############################
        #region ############################## Case Master ##############################
        [Route("/CaseList")]
        public async Task<IActionResult> CaseList(CaseModel _model)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }

                _model.PageSize = SessionManager.Size;
                DataSet ds = await _model.GetCaseList();
                _model.dtProductEnquiry = ds.Tables[0];
                var totalRecords = 0;
                if (_model.dtProductEnquiry.Rows.Count > 0)
                {
                    totalRecords = Convert.ToInt32(_model.dtProductEnquiry.Rows[0]["TotalRecords"].ToString());
                    var pager = new Pager(totalRecords, _model.PageNo, SessionManager.Size);
                    _model.Pager = pager;
                }

                return View(_model);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("CaseList", _model);
            }
        }
        [Route("/CaseMaster")]
        public async Task<IActionResult> CaseMaster(CaseModel _model)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }
                #region ddlCaseType
                List<SelectListItem> ddlCaseType = new List<SelectListItem>();
                _model.OpCode = "1";
                DataSet dsDDL1 = _model.GetMasterData();
                if (dsDDL1 != null && dsDDL1.Tables.Count > 0)
                {
                    if (dsDDL1.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow item in dsDDL1.Tables[0].Rows)
                        {
                            ddlCaseType.Add(new SelectListItem { Value = item["Id"].ToString(), Text = item["Name"].ToString() });
                        }
                    }
                }
                ViewBag.ddlCaseType = ddlCaseType;
                #endregion ddlCaseType

                if (!string.IsNullOrEmpty(_model.EditPkCaseID))
                {
                    _model.PkCaseID = _model.EditPkCaseID;

                    _model.PageSize = SessionManager.Size;
                    _model.PageNo = 1;

                    DataSet ds = await _model.GetCaseList();

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[0].Rows[0];

                        _model.FkCaseTypeId = Convert.ToString(row["intFkCaseTypeId"]);
                        _model.CaseNo = Convert.ToString(row["strCaseNo"]);
                        _model.CaseTitle = Convert.ToString(row["strCaseTitle"]);

                        _model.CaseStartDate = Convert.ToDateTime(row["dtCaseStartDate"])
                            .ToString("yyyy-MM-dd");

                        if (!string.IsNullOrEmpty(Convert.ToString(row["dtJudgementDate"])))
                        {
                            _model.JudgementDate = Convert.ToDateTime(row["dtJudgementDate"])
                                .ToString("yyyy-MM-dd");
                        }

                        // Client Details
                        _model.ClientName = Convert.ToString(row["strClientName"]);
                        _model.ClientMobileNo = Convert.ToString(row["strClientMobileNo"]);
                        _model.ClientEmailId = Convert.ToString(row["strClientEmailId"]);
                        _model.ClientAddress = Convert.ToString(row["strClientAddress"]);

                        // Branch
                        _model.BranchName = Convert.ToString(row["strBranchName"]);

                        _model.Priority = Convert.ToString(row["strPriority"]);
                        _model.status = Convert.ToString(row["strStatus"]);

                        _model.JudgeName = Convert.ToString(row["strJudgeName"]);

                        _model.ShortDetails = Convert.ToString(row["strShortDetails"]);
                        _model.Details = Convert.ToString(row["strDetails"]);

                    }
                }
                return View(_model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("CaseMaster", _model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAndUpdateCase([FromForm] CaseModel model, string btn_Add)
        {
            try
            {
                var claim = Convert.ToString(User.FindFirst("UserId")?.Value);

                model.intFkBranchId = "0";
                model.bigintCreatedBy = claim;

                // Trim Values
                model.CaseNo = model.CaseNo?.Trim();
                model.CaseTitle = model.CaseTitle?.Trim();
                model.CaseStartDate = model.CaseStartDate?.Trim();
                model.ClientName = model.ClientName?.Trim();
                model.ClientMobileNo = model.ClientMobileNo?.Trim();
                model.ClientEmailId = model.ClientEmailId?.Trim();
                model.ClientAddress = model.ClientAddress?.Trim();
                model.BranchName = model.BranchName?.Trim();
                model.Priority = model.Priority?.Trim();
                model.status = model.status?.Trim();
                model.JudgeName = model.JudgeName?.Trim();
                model.JudgementDate = model.JudgementDate?.Trim();
                model.ShortDetails = model.ShortDetails?.Trim();
                model.Details = model.Details?.Trim();

                // Validation
                if (string.IsNullOrEmpty(model.CaseNo))
                {
                    TempData["ErrorMessage"] = "Please Enter Case No!";
                    return View("CaseMaster", model);
                }

                if (string.IsNullOrEmpty(model.CaseTitle))
                {
                    TempData["ErrorMessage"] = "Please Enter Case Title!";
                    return View("CaseMaster", model);
                }
               
                if (string.IsNullOrEmpty(model.ClientName))
                {
                    TempData["ErrorMessage"] = "Please Enter Client Name!";
                    return View("CaseMaster", model);
                }

                if (string.IsNullOrEmpty(model.ClientMobileNo))
                {
                    TempData["ErrorMessage"] = "Please Enter Client Mobile No!";
                    return View("CaseMaster", model);
                }
                if (string.IsNullOrEmpty(model.ClientAddress))
                {
                    TempData["ErrorMessage"] = "Please Enter Client Address!";
                    return View("CaseMaster", model);
                }
                if (model.FkCaseTypeId=="0")
                {
                    TempData["ErrorMessage"] = "Please Select Case Type!";
                    return View("CaseMaster", model);
                }
                if (string.IsNullOrEmpty(model.Priority))
                {
                    TempData["ErrorMessage"] = "Please Select Priority!";
                    return View("CaseMaster", model);
                }

                if (string.IsNullOrEmpty(model.status))
                {
                    TempData["ErrorMessage"] = "Please Select Status!";
                    return View("CaseMaster", model);
                }

                // Decided Status Validation
                if (model.status == "Decided")
                {
                    if (string.IsNullOrEmpty(model.JudgeName))
                    {
                        TempData["ErrorMessage"] = "Please Enter Judge Name!";
                        return View("CaseMaster", model);
                    }

                    if (string.IsNullOrEmpty(model.JudgementDate))
                    {
                        TempData["ErrorMessage"] = "Please Enter Judgement Date!";
                        return View("CaseMaster", model);
                    }
                }
                if (string.IsNullOrEmpty(model.ShortDetails))
                {
                    TempData["ErrorMessage"] = "Please Short Details!";
                    return View("CaseMaster", model);
                }
                if (string.IsNullOrEmpty(model.Details))
                {
                    TempData["ErrorMessage"] = "Please Enter Details!";
                    return View("CaseMaster", model);
                }


                if (btn_Add == "Update")
                {
                    model.Mode = "Update";
                }
                else
                {
                    model.Mode = "INSERT";
                }

                DataSet ds = await model.AddEditDltCase();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["code"]) == "0")
                    {
                        TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("CaseList", "ERPMaster");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return View("CaseMaster", model);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong!";
                    return View("CaseMaster", model);
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("CaseMaster", model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCase([FromForm] CaseModel model)
        {
            var claim = Convert.ToString(User.FindFirst("UserId").Value);
            model.intFkBranchId = "0";
            model.bigintCreatedBy = claim;

            try
            {
                model.Mode = "DELETE";
                model.PkCaseID = model.EditPkCaseID;
                DataSet ds = await model.AddEditDltCase();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["code"]) == "0")
                    {
                        TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("CaseList", "ERPMaster");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                        return RedirectToAction("CaseList", "ERPMaster");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong!";
                }
                return RedirectToAction("CaseList", "ERPMaster");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("CaseList", "ERPMaster");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddNextCaseDate(CaseModel model, string btn_Add)
        {
            string status = "";
            string msg = "";

            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Account");
                }

                var claim = Convert.ToString(User.FindFirst("UserId")?.Value);

                model.intFkBranchId = "0";
                model.bigintCreatedBy = claim;

                // Trim Values
                model.PkCaseID = model.PkCaseID?.Trim();
                model.CaseLastStatusID = model.CaseLastStatusID?.Trim();
                model.Remark = model.Remark?.Trim();
                model.NextDate = model.NextDate?.Trim();
                model.CourtNo = model.CourtNo?.Trim();
                model.JudgeName = model.JudgeName?.Trim();

                // Validation
                if (string.IsNullOrWhiteSpace(model.CourtNo))
                {
                    return Json(new
                    {
                        code = "1",
                        msg = "Please Enter Court No!"
                    });
                }

                if (string.IsNullOrWhiteSpace(model.JudgeName))
                {
                    return Json(new
                    {
                        code = "1",
                        msg = "Please Enter Judge Name!"
                    });
                }

                if (string.IsNullOrWhiteSpace(model.NextDate))
                {
                    return Json(new
                    {
                        code = "1",
                        msg = "Please Select Next Date!"
                    });
                }

                if (string.IsNullOrWhiteSpace(model.Remark))
                {
                    return Json(new
                    {
                        code = "1",
                        msg = "Please Enter Remark!"
                    });
                }
                model.Mode = "INSERT";
                DataSet ds = await model.AddEditDltCaseDetails();

                if (ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    status = Convert.ToString(ds.Tables[0].Rows[0]["code"]);
                    msg = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                }
                else
                {
                    status = "1";
                    msg = "No response from database!";
                }

                return Json(new
                {
                    code = status,
                    msg = msg
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    code = "1",
                    msg = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<JsonResult> CaseDetailsList(CaseModel model)
        {
            string _error = "";
            string _mess = "";
            string chkPermission = "";
            try
            {
                model.Mode = "Select";
                DataSet ds = await model.AddEditDltCaseDetails();
                DataTable tmpDT = ds.Tables[0];
                if (tmpDT.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> lstRows = [];
                    lstRows = GBL_Utility.GetJsonFromTable(tmpDT);
                    return Json(new { error = _error, message = _mess, cnt = tmpDT.Rows.Count, record = lstRows });
                }
                else
                {
                    _error = "ERROR";
                    _mess = "No Record Found.";
                    return Json(new { error = _error, message = _mess, cnt = "0", record = "" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { error = "ERROR", message = ex.Message, cnt = "0", record = "" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCaseDetails(CaseModel model, string btn_Add)
        {
            string _error = "";
            string _mess = "";
            string chkPermission = "";
            try
            {
                model.Mode = "DELETE";
                DataSet ds = await model.AddEditDltCaseDetails();
                if (ds != null && ds.Tables.Count > 0)
                {
                    //if (Convert.ToString(ds.Tables[0].Rows[0]["Code"]) == "0")
                    //{
                    //    TempData["SuccessMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["Remark"]);
                    //    return RedirectToAction("ProductMaster", "ERPMaster");
                    //}
                    //else
                    //{
                    //    TempData["ErrorMessage"] = Convert.ToString(ds.Tables[0].Rows[0]["Remark"]);
                    //    return View("ProductMaster", model);
                    //}
                    _error = Convert.ToString(ds.Tables[0].Rows[0]["code"]);
                    _mess = Convert.ToString(ds.Tables[0].Rows[0]["mess"]);
                    return Json(new { ERROR = _error, MESSAGE = _mess });
                }
                else
                {
                    //TempData["ErrorMessage"] = "Something went wrong!";
                    //return View("ProductMaster", model);
                    _error = "1";
                    _mess = "Something went wrong!";
                    return Json(new { ERROR = _error, MESSAGE = _mess });
                }

            }
            catch (Exception ex)
            {
                //TempData["ErrorMessage"] = ex.Message;
                //return View("ProductMaster", model);
                _error = "1";
                _mess = ex.Message;
                return Json(new { ERROR = _error, MESSAGE = _mess });
            }
        }
        #endregion ############################## Case Master ##############################


    }
}
