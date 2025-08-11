using CR_CoreBot_DataAccess.CR_OpenAI;
using CR_HRPortalAI_DataAcess.CR_HRPortal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CR_CoreBot_DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using CR_HRPortalAI_DataAcess.Models;
using Microsoft.Extensions.Options;
using AspNetCoreHero.ToastNotification.Abstractions;
using CR_CoreBot_Service.Adapter;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NLog;

namespace CR_CoreBot.Controllers
{
    public class LoginController : Controller
    {
        LoginInfoDTO loginInfo = new LoginInfoDTO();
        private readonly ConnectionModel _appSettingconnection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotyfService _toastNotification;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public LoginController(IOptions<ConnectionModel> appSettingconnection, IHttpContextAccessor httpContextAccessor, INotyfService toastNotification)
        {
            _appSettingconnection = appSettingconnection.Value;
            _httpContextAccessor = httpContextAccessor;
            _toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            logger.Info("Info Message");
            logger.Error("Error Message");
            return View();
        }
        public IActionResult Logout()
        {
            try
            {
                var Loginval = HttpContext.Session.GetString("LoginInfo");
                DateTime loginTime = DateTime.Parse(HttpContext.Session.GetString("UserLoginTime"));
                DateTime LogoutTime;
                if (Loginval != null)
                {
                    HttpContext.Session.SetString("LoginUserInfo", Loginval);
                    LogoutTime = DateTime.Now;
                    var totalSpendTime = LogoutTime - loginTime;

                    HttpContext.Session.SetString("LogoutInfo", Loginval);
                    var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(Loginval);
                    if (logindata != null)
                    {
                        string RoleName = string.Empty;
                        var Username = logindata.EmailId.Trim();
                        var LoginWith = "Normal User";
                        var RoleId = logindata.RoleId;
                        if (RoleId != null)
                        {
                            if (RoleId == 1)
                            {
                                RoleName = "Admin";
                            }
                            else if (RoleId == 2)
                            {
                                RoleName = "SuperAdmin";
                            }
                            else if (RoleId == 3)
                            {
                                RoleName = "User";
                            }
                        }
                        CropenAiContext cr = new CropenAiContext();
                        var TotalPrompt = cr.PerformanceMatrixCheckers.Where(x => x.Username.Trim() == Username.Trim()).Select(x => x.Prompt).Count();
                        var TotalToken = cr.PerformanceMatrixCheckers.Where(x => x.Username.Trim() == Username.Trim()).Select(x => x.TotalTokens).ToList();

                        int TotalTokens = 0;
                        for (int i = 0; i < TotalToken.Count; i++)
                        {
                            if (TotalToken[i] == "N/A")
                            {
                                TotalTokens += 0;
                            }
                            else
                            {
                                TotalTokens += Convert.ToInt32(TotalToken[i]);
                            }
                        }

                        //N/A
                        bool result = AddUserSessionTracker(totalSpendTime, Username, LoginWith, loginTime, Convert.ToInt32(RoleId), RoleName, TotalPrompt, TotalTokens);
                        if (result == true)
                        { }
                        else
                        { }
                    }
                    HttpContext.Session.Clear();
                    HttpContext.Session.Remove("LogoutInfo");
                    HttpContext.Session.Remove("UserLoginTime");
                    HttpContext.Session.Remove("LoginUserInfo");
                    HttpContext.Session.Remove("LoginInfo");
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    return RedirectToAction("Index", "Login", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }
        }
        public bool AddUserSessionTracker(TimeSpan SessionDurationTime, string Username, string LoginWith, DateTime LoginTime, int RoleId, string RoleName, int TotalPrompt, int TotalToken)
        {
            bool result = false;
            try
            {
                UserSessionTracker userSessionTracker = new UserSessionTracker();
                userSessionTracker.SessionDuration = SessionDurationTime;
                userSessionTracker.Username = Username.Trim();
                userSessionTracker.LoginWith = LoginWith.Trim();
                userSessionTracker.LoginTime = LoginTime;
                userSessionTracker.RoleId = RoleId;
                userSessionTracker.RoleName = RoleName.Trim();
                userSessionTracker.TotalPrompt = TotalPrompt;
                userSessionTracker.TotalToken = TotalToken;
                userSessionTracker.CreatedDateTime = DateTime.Now;
                using (CropenAiContext cr = new CropenAiContext())
                {
                    cr.UserSessionTrackers.Add(userSessionTracker);
                    cr.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (Exception ex)
            {
                result = false;
                return result;
            }
        }
        #region AdLogin
        [Authorize]
        public async Task<IActionResult> Adlogin()
        {
            try
            {
                string firstName = User.FindFirst(ClaimTypes.GivenName)?.Value;
                string lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
                string fullName = firstName + " " + lastName;
                string Role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userid = User.Identity.Name;
                CropenAiContext cr = new CropenAiContext();
                var id = cr.CustomerInformations.Where(x => x.UserName == userid.Trim()).Select(x => x.CustomerId).FirstOrDefault();
                AdLoginInfoDTO adloginInfo = new AdLoginInfoDTO();
                adloginInfo.EmailId = userid;
                adloginInfo.FirstName = firstName;
                adloginInfo.LastName = lastName;
                adloginInfo.FullName = firstName + " " + lastName;
                if (Role.Trim() == "Admin")
                {
                    adloginInfo.RoleId = 1;
                }
                else if (Role.Trim() == "SuperAdmin")
                {
                    adloginInfo.RoleId = 2;
                }
                else if (Role.Trim() == "User")
                {
                    adloginInfo.RoleId = 3;
                }

                adloginInfo.Role = Role.Trim();

                AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                int adlogincount = cr.CustomerInformations.Where(x => x.UserName == userid.Trim()).Count();

                // adlogincount when equals to 0 means- 1st time that user is logging into our application. 0 count indicates that there is no record in the db with currently logged in user email id.

                if (adlogincount == 0)
                {
                    CustomerInformation custinfo = new CustomerInformation();
                    custinfo.UserName = userid.Trim();
                    custinfo.Name = fullName.Trim();
                    custinfo.LoginWith = "AdLogin";
                    custinfo.RoleId = adloginInfo.RoleId;

                    // Creating Container name and getting newly contaner name from blob for Ad User 

                    var contname = firstName + "Doc";
                    string containerName = contname.ToLower();
                    string connectionString = "your storage connection string";
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    // Get container name from blob which we have created like e.g: atuldoc
                    BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);

                    var container_name = "";

                    // If blob container exists for Logged in Ad User. only hit when ad logged in user is login 1st time.

                    if (await container.ExistsAsync())
                    {
                        custinfo.BlobContainerName = container.Name;
                        // Insert ad logged in user information inside Customer Information table with the blob name.
                        admin.fnInsertCustomerInformation(custinfo);

                        var custid = cr.CustomerInformations.Where(x => x.UserName == userid.Trim()).Select(x => x.CustomerId).FirstOrDefault();

                        // Saving Customer Entry> Customer Configuration For Logged In Ad User ( Which Ad user logged in for first time)

                        CustomerConfiguration addcustomerconfig = new CustomerConfiguration();
                        addcustomerconfig.Username = custinfo.UserName.Trim();
                        addcustomerconfig.CustomerId = custid;
                        addcustomerconfig.FilesAllowed = 20;
                        addcustomerconfig.UsersAllowed = 20;
                        addcustomerconfig.FileFormat = "Pdf";
                        addcustomerconfig.CreateDateTime = DateTime.Now;
                        cr.CustomerConfigurations.Add(addcustomerconfig);
                        cr.SaveChanges();
                    }

                    // If container for logged in ad user does not exist on blob

                    else
                    {
                        // If container not exists on blob then create container first then insert data with the newly created container name in Customer Information table
                        // and customer configuration also - save for logged in ad user.

                        CustomerInformation Cinfo = new CustomerInformation();
                        await container.CreateAsync();
                        custinfo.BlobContainerName = container.Name;
                        admin.fnInsertCustomerInformation(custinfo);
                        var custid = cr.CustomerInformations.Where(x => x.UserName == userid.Trim()).Select(x => x.CustomerId).FirstOrDefault();
                        CustomerConfiguration addcustomerconfig = new CustomerConfiguration();
                        addcustomerconfig.Username = custinfo.UserName.Trim();
                        addcustomerconfig.CustomerId = custid;
                        addcustomerconfig.FilesAllowed = 20;
                        addcustomerconfig.UsersAllowed = 20;
                        addcustomerconfig.FileFormat = "Pdf";
                        addcustomerconfig.CreateDateTime = DateTime.Now;
                        cr.CustomerConfigurations.Add(addcustomerconfig);
                        cr.SaveChanges();
                    }
                }

                // For already logged in ad user jiski db me entry hai then this condition will be hit
                else
                {
                    // If Logged in Ad user already exists in db.
                    string check_cont_name = cr.CustomerInformations.Where(x => x.UserName == userid.Trim()).Select(x => x.BlobContainerName).FirstOrDefault();
                    if (string.IsNullOrEmpty(check_cont_name) || check_cont_name == "N/A")
                    {
                        var contname = firstName + "Doc";
                        string containerName = contname.ToLower();
                        string connectionString = "your storage connection string";
                        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                        // Get container name from blob which we have created like e.g: atuldoc
                        BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);

                        var container_name = "";

                        // If ad user which have entry in db if on blob container exist for that user then this condition will be hit

                        if (await container.ExistsAsync())
                        {
                            // if container exists on blob update Customer Information table with the same container name where logged in ad user id exists.

                            container_name = container.Name;
                            admin.fnUpdateCustomerInformationblob(id, container_name);
                        }

                        // If the ad user which have entry on db. If it's container deleted from blob then first we will create container on blob for that ad user then
                        // we will update customer information table with the newly created container name in the db.
                        else
                        {

                            // If container not exists on blob then create a new container and update container name where logged in ad user id exists in the Customer Information table.                            
                            CustomerInformation Cinfo = new CustomerInformation();
                            await container.CreateAsync();
                            container_name = container.Name;
                            admin.fnUpdateCustomerInformationblob(id, container_name);
                        }
                    }
                }

                int AdLoggedInUserId = 0;
                if (adlogincount > 0)
                {
                    AdLoggedInUserId = cr.CustomerInformations.Where(x => x.UserName.Trim() == adloginInfo.EmailId.Trim()).Select(x => x.CustomerId).FirstOrDefault();

                    adloginInfo.AdLoggedInUserId = AdLoggedInUserId;
                }
                var serializedObject = JsonConvert.SerializeObject(adloginInfo);
                HttpContext.Session.SetString("AdLoginInfo", serializedObject);
                return RedirectToAction("Index", "Home", new { Area = "" });
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }
        }
        #endregion End AdLogin
        #region AdLogout
        public IActionResult AdLogout()
        {
            try
            {
                int role = 0;
                var AdLoginval = HttpContext.Session.GetString("AdLoginInfo");
                DateTime loginTime = DateTime.Parse(HttpContext.Session.GetString("AdLoginTime"));
                DateTime LogoutTime;
                if (AdLoginval != null)
                {
                    HttpContext.Session.SetString("AdLoginUserInfo", AdLoginval);
                    LogoutTime = DateTime.Now;
                    var totalSpendTime = LogoutTime - loginTime;

                    HttpContext.Session.SetString("AdLogoutInfo", AdLoginval);
                    var logindata = JsonConvert.DeserializeObject<AdLoginInfoDTO>(AdLoginval);
                    if (logindata != null)
                    {
                        //string Role = User.FindFirst(ClaimTypes.Role)?.Value;
                        string Role = logindata.Role;
                        string RoleName = string.Empty;
                        var Username = logindata.EmailId.Trim();
                        var LoginWith = "Ad User";
                        int RoleId = 0;
                        if (Role.Trim() == "Admin")
                        {
                            RoleId = 1;
                        }
                        else if (Role.Trim() == "SuperAdmin")
                        {
                            RoleId = 2;
                        }
                        else
                        {
                            RoleId = 3;
                        }

                        CropenAiContext cr = new CropenAiContext();
                        var TotalPrompt = cr.PerformanceMatrixCheckers.Where(x => x.Username.Trim() == Username.Trim()).Select(x => x.Prompt).Count();
                        var TotalToken = cr.PerformanceMatrixCheckers.Where(x => x.Username.Trim() == Username.Trim()).Select(x => x.TotalTokens).ToList();

                        int TotalTokens = 0;
                        for (int i = 0; i < TotalToken.Count; i++)
                        {
                            if (TotalToken[i] == "N/A")
                            {
                                TotalTokens += 0;
                            }
                            else
                            {
                                TotalTokens += Convert.ToInt32(TotalToken[i]);
                            }
                        }

                        bool result = AddUserSessionTracker(totalSpendTime, Username, LoginWith, loginTime, Convert.ToInt32(RoleId), Role.Trim(), TotalPrompt, TotalTokens);
                        if (result == true)
                        {

                        }
                        else
                        {

                        }
                        HttpContext.Session.Remove("AdLoginInfo");
                        HttpContext.Session.Remove("AdLoginTime");
                        HttpContext.Session.Remove("AdLoginUserInfo");
                        HttpContext.Session.Remove("AdLogoutInfo");
                        HttpContext.Session.Clear();
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }
        }
        #endregion End AdLogout

        #region SaveFeedback
        [HttpPost]
        public IActionResult SaveFeedback(Feedback feedback)
        {
            bool result = false;
            try
            {
                int role = 0;
                int CustomerId = 0;
                int? UserId = 0;
                string FullName = string.Empty;
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                var adLoginObject = HttpContext.Session.GetString("AdLoginInfo");
                //LoggedInCustomerDetails loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetails>(serializedObject);

                if (serializedObject != null)
                {
                    LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);

                    if (loginInfo != null)
                    {
                        HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                        role = loginInfo.RoleId;
                        CustomerId = loginInfo.Id;
                        UserId = loginInfo.UserId;
                        FullName = loginInfo.FullName;
                    }
                    using (CropenAiContext dbctx = new CropenAiContext())
                    {
                        Feedback feedbackobj = new Feedback();
                        feedbackobj.OveralExperience = feedback.OveralExperience;
                        feedbackobj.Satisfactionwithbot = feedback.Satisfactionwithbot;
                        feedbackobj.SuggestionFeedback = feedback.SuggestionFeedback;
                        feedbackobj.EmailFeedback = feedback.EmailFeedback;
                        feedbackobj.FeedbackDate = DateTime.Now;
                        feedbackobj.CustomerId = CustomerId;
                        if (role == 3)
                        {
                            feedbackobj.UserId = UserId;
                        }
                        dbctx.Feedbacks.Add(feedbackobj);
                        dbctx.SaveChanges();
                        if (feedbackobj.EmailFeedback == "true")
                        {
                            bool res = SendFeedbackOnEmail(loginInfo.EmailId, FullName, feedback.Satisfactionwithbot);
                        }
                        result = true;
                        return Json(result);
                    }
                }
                else if (serializedObject == null)
                {
                    if (adLoginObject != null)
                    {
                        AdLoginInfoDTO adlogininfodata = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adLoginObject);
                        if (adlogininfodata != null)
                        {
                            //var userid = User.Identity.Name;
                            var userid = adlogininfodata.EmailId.Trim();
                            var Id = HttpContext.Session.Id;
                            //var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value;
                            //var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
                            //var fullName = firstName + " " + lastName;
                            var firstName = adlogininfodata.FirstName;
                            var lastName = adlogininfodata.LastName;
                            var fullName = adlogininfodata.FullName.Trim();
                            using (CropenAiContext dbctx = new CropenAiContext())
                            {
                                Feedback feedbackobj = new Feedback();
                                feedbackobj.OveralExperience = feedback.OveralExperience;
                                feedbackobj.Satisfactionwithbot = feedback.Satisfactionwithbot;
                                feedbackobj.SuggestionFeedback = feedback.SuggestionFeedback;
                                feedbackobj.EmailFeedback = feedback.EmailFeedback;
                                feedbackobj.FeedbackDate = DateTime.Now;
                                feedbackobj.CustomerId = 1;
                                role = 3;
                                if (role == 3)
                                {
                                    feedbackobj.UserId = null;
                                }
                                dbctx.Feedbacks.Add(feedbackobj);
                                dbctx.SaveChanges();
                                if (feedbackobj.EmailFeedback == "true")
                                {
                                    bool res = SendFeedbackOnEmail(userid.Trim(), fullName, feedback.Satisfactionwithbot);
                                }
                                result = true;

                            }
                        }
                    }
                    return Json(result);
                }
                else
                {
                    return Json("noconditionmatched");
                }
            }
            catch (System.Exception ex)
            {
                result = false;
                return Json(result);
            }
        }
        #endregion SaveFeedback
        public bool SendFeedbackOnEmail(string toEmail, string FullName, string Satisfactionwithbot)
        {
            bool flag = false;
            try
            {
                string senderName = "Support Team";
                //string receiverName = FullName.Trim();
                string receiverName = FullName.Trim();
                //if (string.IsNullOrEmpty(FullName)) { 
                // receiverName = toEmail.Trim();
                //}
                //else
                //{
                //    receiverName = FullName.Trim();
                //}
                //string receiverContact = toEmail.Trim();
                string receiverContact = "support@unifycloud.com";
                string productName = "HR Bot";
                string aspect = "[aspect or feature]";
                string enhancement = "[specific aspect or feature]";
                string emailBody = string.Empty;

                if (Satisfactionwithbot.Trim() == "Very satisfied")
                {
                    emailBody = $@"<html>
<head>
<meta charset='UTF-8'>
<title>Feedback Response</title>
</head>
<body style='font-family:Arial,sans-serif;margin:0;padding:0;'>

 

    <div class='container' style='max-width:600px;padding:20px; background-color:#f7f7f7; border:1px solid #ddd;border-top: 4px solid #039BE5;'>

 

        <h1 style='color:#333333; font-size: 20px;'>Dear {receiverName},</h1>

 

        <p>Thank you for providing feedback on your recent experience with our <strong style='font-style:italic;'>{productName}</strong>. We appreciate you are <strong style='font-style:italic;'>{Satisfactionwithbot}</strong> with <strong style='font-style:italic;'>{productName}</strong>.

 

        It's rewarding to know that our efforts have enhanced your overall experience.</p>

 

        <p>We value your input and will carefully consider your recommendation.</p>

 

        <p>If you have any additional thoughts or suggestions, please feel free to share them. We are committed to addressing your concerns and providing an improved experience.</p>

 

        <p>Thank you once again for your feedback. We look forward to serving you better in the future.</p>

 

        <div class='signature' style='margin-top:40px;text-align:left;'>

 

        <p>Best regards,</p>

 

        <p>{senderName}</p>

 

        <p>{receiverContact}</p>
<img style='padding-top: 15px; border-top: 1px dashed #666;' alt='UnifyCloud' src='https://www.unifycloud.com/wp-content/uploads/2019/09/uc-logo-new.svg'>

 

        </div>

 

        </div>

 

</body>
</html>";
                }
                else if (Satisfactionwithbot.Trim() == "Not satisfied")
                {
                    emailBody = $@"<html>
<head>
<meta charset='UTF-8'>
<title>Feedback Response</title>
</head>
<body style='font-family:Arial,sans-serif;margin:0;padding:0;'>
<div class='container' style='max-width:600px;padding:20px; background-color:#f7f7f7; border:1px solid #ddd;border-top: 4px solid #039BE5;'>
<h1 style='color:#333333; font-size: 20px;'>Dear {receiverName},</h1>
<p>Thank you for providing feedback on your recent experience with our <strong style='font-style:italic;'>{productName}</strong>. We apologize for any inconvenience caused and appreciate your feedback regarding your <strong style='font-style:italic;'>dissatisfaction</strong>.</p>
<p>We understand the importance of meeting our customers expectations, and we deeply regret falling short in your case. Your feedback has been noted, and we are committed to taking immediate action to address the issues you encountered.</p>
<p>We value your input and will utilize it to make the necessary improvements. Your satisfaction is of utmost importance to us, and we assure you that we will do everything in our power to rectify the situation and provide you with a better experience in the future.</p>
<p>If you have any additional thoughts or suggestions, please feel free to share them. We appreciate your candid feedback and are dedicated to making the necessary changes to enhance our products and services.</p>
<p>Thank you once again for bringing this matter to our attention. We are truly sorry for any inconvenience caused, and we genuinely appreciate your patience and understanding.</p>

<div class='signature' style='margin-top:40px;text-align:left;'>
<p>Best regards,</p>
<p>{senderName}</p>
<p>{receiverContact}</p>
<img style='padding-top: 15px; border-top: 1px dashed #666;' alt='UnifyCloud' src='https://www.unifycloud.com/wp-content/uploads/2019/09/uc-logo-new.svg'>

 

                                        </div>
</div>
</body>
</html>";
                }
                else if (Satisfactionwithbot.Trim() == "Need Improvement")
                {
                    emailBody = $@"<html>
<head>
<meta charset='UTF-8'>
<title>Feedback Response</title>
</head>
<body style='font-family:Arial,sans-serif;margin:0;padding:0;'>
<div class='container' style='max-width:600px;padding:20px; background-color:#f7f7f7; border:1px solid #ddd;border-top: 4px solid #039BE5;'>
<h1 style='color:#333333; font-size: 20px;'>Dear {receiverName},</h1>
<p>Thank you for providing feedback on your recent experience with our <strong style='font-style:italic;'>{productName}</strong>. We appreciate your feedback.</p>
<p>We also understand that there may be areas where we <strong style='font-style:italic;'> {Satisfactionwithbot}</strong></p>
<p>Your feedback is valuable to us, and we appreciate your suggestions for enhancing <strong style='font-style:italic;'>{productName}</strong>. We assure you that we take your input seriously and will carefully consider your recommendations.</p>
<p>If you have any additional thoughts or suggestions, please feel free to share them. We are committed to addressing your concerns and continuously improving to provide you with an even better experience.</p>
<p>Thank you once again for your feedback. We truly value your contribution and look forward to serving you better in the future.</p>
<div class='signature' style='margin-top:40px;text-align:left;'>
<p>Best regards,</p>
<p>{senderName}</p>
<p>{receiverContact}</p>
<img style='padding-top: 15px; border-top: 1px dashed #666;' alt='UnifyCloud' src='https://www.unifycloud.com/wp-content/uploads/2019/09/uc-logo-new.svg'>

 

                                        </div>
</div>
</body>
</html>";
                }

                toEmail = toEmail.Trim();
                string EmailID = "support@unifycloud.com";
                string EmailPassword = "Dav08195";
                MailMessage outlookObj = new MailMessage();
                outlookObj.To.Add(toEmail);
                outlookObj.From = new MailAddress(EmailID);
                outlookObj.Subject = "Thank you for Your Feedback";
                outlookObj.Body = emailBody;
                outlookObj.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Port = 587;
                smtp.Host = "outlook.office365.com";
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(EmailID, EmailPassword);
                smtp.Send(outlookObj);
                flag = true;
            }
            catch (System.Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        #region ChangePassword
        public IActionResult ChangePassword()
        {
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
            if (loginInfo != null)
            {
                ViewBag.Role = loginInfo.RoleId;
                ViewBag.EmailId = loginInfo.EmailId;
                ViewBag.UserName = loginInfo.UserName;
                ViewBag.LoginType = "Login";
            }
            return View();
        }
        [HttpPost]
        public JsonResult getCustomAuthentication(string username, string password, bool rememberme)
        {
            int check = 1;

            bool diff = true;
            List<string> _appSetting = new List<string>();
            AdminService adminService = new AdminService(_appSettingconnection, _httpContextAccessor);
            _appSetting = adminService.getConnectionString(username);
            if (_appSetting.Count > 0)
            {
                _appSettingconnection.DatabaseName = _appSetting[0];
                _appSettingconnection.Connection = _appSetting[1];
            }
            else
            {
                return Json(new { msg = "Not Exist" });
            }
            ServiceMethods serviceMethods = new ServiceMethods(_appSettingconnection);
            LoginInfoDTO objUser = serviceMethods.getUserByUserName(username);

            if (objUser != null && objUser.Password != null)
            {
                bool test = objUser.Password.Equals(password);
                if (test == false)
                {
                    return Json(new { msg = "Not Exist" });
                }
                loginInfo.FullName = objUser.FullName;
                loginInfo.Id = objUser.CustomerId;
                loginInfo.DatabaseName = objUser.DatabaseName;
                loginInfo.EmailId = objUser.UserName;
                loginInfo.Category = objUser.Category;
                loginInfo.OrganizationName = objUser.OrganizationName;
                loginInfo.Logo = objUser.Logo;
                loginInfo.BlobContainerName = objUser.BlobContainerName;
                loginInfo.RoleId = objUser.RoleId;
                loginInfo.UserId = objUser.UserId;
                loginInfo.LoginWith = objUser.LoginWith;
                loginInfo.Streaming = objUser.Streaming;
                var serializedObject = JsonConvert.SerializeObject(loginInfo);
                HttpContext.Session.SetString("LoginInfo", serializedObject);
                if (rememberme == true)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(15),
                        HttpOnly = true,
                        IsEssential = true
                    };
                    Response.Cookies.Append("EmailID", loginInfo.EmailId, cookieOptions);
                    Response.Cookies.Append("Password", password, cookieOptions);
                }
                if (objUser.LoginWith != null)
                {
                    if (objUser.LoginWith.ToLower() == "demo")
                    {
                        return Json(new { msg = "demo Success" });
                    }
                    else
                    {
                        return Json(new { msg = "Success" , additionalProperty = objUser.Category, roleId = objUser.RoleId });
                    }
                }
                else
                {
                    return Json(new { msg = "Success", additionalProperty = objUser.Category, roleId = objUser.RoleId });
                }
            }

            else if (objUser.NotFound == true)
            {
                _toastNotification.Error("User doesn't Exists !!");
                return Json(new { msg = "TryWith" });
            }
            else if (objUser.Password == null)
            {
                _toastNotification.Error("Invalid Password !!");
                return Json(new { msg = "TryWith" });
            }
            else if (objUser == null)
            {
                _toastNotification.Error("Incorrect Credential !!");
                return Json(new { msg = "Failed" });
            }
            return Json(objUser);
        }

        [HttpPost]
        public IActionResult ChangePassword(string CurrentPassword, string NewPassword, string ConfirmNewPassword)
        {
            bool result = false;
            try
            {
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                if (loginInfo != null)
                {
                    ViewBag.Role = loginInfo.RoleId;
                    ViewBag.EmailId = loginInfo.EmailId;
                    ViewBag.UserName = loginInfo.UserName;
                    if (ViewBag.Role == 1 || ViewBag.Role == 2)
                    {
                        CropenAiContext cr = new CropenAiContext();
                        CustomerInformation checkpasswordexists = cr.CustomerInformations.Where(x => x.CustomerId == loginInfo.Id).FirstOrDefault();
                        if (checkpasswordexists != null)
                        {
                            if (checkpasswordexists.Password == CurrentPassword.Trim())
                            {
                                checkpasswordexists.Password = NewPassword.Trim();
                                cr.Entry(checkpasswordexists).State = EntityState.Modified;
                                cr.SaveChanges();
                                result = true;
                                return Json(result);
                            }
                            else
                            {
                                return Json("OldPasswordIncorrect");
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "Login");
                        }
                    }
                    else if (ViewBag.Role == 3)
                    {
                        HrportalAiContext hr = new HrportalAiContext(_appSettingconnection);
                        UserInformation checkpasswordexists = hr.UserInformations.Where(x => x.UserName == loginInfo.EmailId).FirstOrDefault();
                        if (checkpasswordexists != null)
                        {
                            if (checkpasswordexists.Password == CurrentPassword.Trim())
                            {
                                checkpasswordexists.Password = NewPassword.Trim();
                                hr.Entry(checkpasswordexists).State = EntityState.Modified;
                                hr.SaveChanges();
                                result = true;
                                return Json(result);
                            }

                            else
                            {
                                return Json("OldPasswordIncorrect");
                            }
                        }
                        else
                        {
                            return Json("Nodatafound");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                result = false;
                return Json(result);
            }
        }

        #endregion ChangePassword
    }
}
