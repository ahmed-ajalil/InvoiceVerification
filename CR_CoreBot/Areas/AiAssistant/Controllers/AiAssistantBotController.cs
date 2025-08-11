using Azure.Storage.Blobs;
using CR_CoreBot_DataAccess.CR_OpenAI;
using CR_CoreBot_DTO;
using CR_CoreBot_Service.Adapter;
using CR_HRPortalAI_DataAcess.CR_HRPortal;
using CR_HRPortalAI_DataAcess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CR_CoreBot.Areas.AiAssistant.Controllers
{
    [Area("AiAssistant")]
    public class AiAssistantBotController : Controller
    {
        static List<string> messageLists = new List<string>();
        static List<string> UserRequest = new List<string>();
        static List<string> ChatBotReply = new List<string>();
        private static ConnectionModel _appSettingconnection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public async Task<IActionResult> bot(DataNewDTO objModel)
        {
            string dataReceived = "AiAssistantModel";
            UserRequest.Clear();
            ChatBotReply.Clear();
            string rolename = string.Empty;
            var normaluserlogin = HttpContext.Session.GetString("LoginInfo");
            var adlogin = HttpContext.Session.GetString("AdLoginInfo");
            if (normaluserlogin != null)
            {
                objModel.PropmpInput = "Generate only 3 questions from the documents";
                var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);
                HttpContext.Session.SetString("UserLoginTime", DateTime.Now.ToString());
                List<string> ModelList = new List<string>();
                using (var dbContexthr = new CropenAiContext())
                {
                    var SuggestedQuestion1 = Convert.ToString(AppSettingHelper.config.GetSection("SuggestedQuestions:Question1").Value);
                    var SuggestedQuestion2 = Convert.ToString(AppSettingHelper.config.GetSection("SuggestedQuestions:Question2").Value);
                    var SuggestedQuestion3 = Convert.ToString(AppSettingHelper.config.GetSection("SuggestedQuestions:Question3").Value);
                    ViewBag.SuggestedQuestion1 = SuggestedQuestion1;
                    ViewBag.SuggestedQuestion2 = SuggestedQuestion2;
                    ViewBag.SuggestedQuestion3 = SuggestedQuestion3;

                    if (logindata.EmailId.ToLower() == "demo@unifycloud.com")
                    {
                        CustomerInformation customerInformation = dbContexthr.CustomerInformations.Where(x => x.UserName.ToLower() == logindata.EmailId.ToLower()).FirstOrDefault();
                        ModelFineTuneDatum objlebelData = new ModelFineTuneDatum();
                        objModel = await ServiceMethods.Firstquestion(objModel, "nimo3", "admin");
                        objModel.presuggestions1 = SuggestedQuestion1;
                        objModel.presuggestions2 = SuggestedQuestion2;
                        objModel.presuggestions3 = SuggestedQuestion3;

                    }
                    else if (dataReceived != null && dataReceived == "AiAssistantModel")
                    {
                        ModelList = dbContexthr.CustomerModels.Where(x => x.ModelDisplayName.ToLower().Trim() == "AiAssistantmodel").Select(x => x.ModelDisplayName).ToList();
                        var blobcontainer = dbContexthr.CustomerInformations.Where(x => x.UserName.ToLower().Trim() == logindata.EmailId.ToLower().Trim()).Select(x => new { x.BlobContainerName, x.RoleId }).FirstOrDefault();
                        if (blobcontainer == null)
                        {
                            int customerID = Convert.ToInt32(dbContexthr.TblUserInformationMappings.Where(x => x.UserName.ToLower().Trim() == logindata.EmailId.ToLower().Trim()).Select(x => x.CustomerId).FirstOrDefault());
                            blobcontainer = dbContexthr.CustomerInformations.Where(x => x.CustomerId == customerID).Select(x => new { x.BlobContainerName, x.RoleId }).FirstOrDefault();
                        }
                        string Loginwith = dbContexthr.RoleMasters.Where(x => x.RoleId == blobcontainer.RoleId).Select(x => x.RoleName).FirstOrDefault();
                        //objModel = await ServiceMethods.Firstquestion(objModel, blobcontainer.BlobContainerName.ToLower(), Loginwith.ToLower());
                        objModel.presuggestions1 = SuggestedQuestion1;
                        objModel.presuggestions2 = SuggestedQuestion2;
                        objModel.presuggestions3 = SuggestedQuestion3;
                    }
                    else
                    {
                        ModelList = dbContexthr.CustomerModels.Where(x => x.CustomerId == logindata.Id).Select(x => x.ModelDisplayName).ToList();
                    }
                }
                ViewBag.Logo = logindata.Logo;
                ViewBag.Role = logindata.RoleId;
                ViewBag.listmodel = ModelList;
                ViewBag.username = logindata.EmailId;
                ViewBag.CustomerId = logindata.Id;
                ViewBag.Category = logindata.Category;
                ViewBag.OrganizationName = logindata.OrganizationName;
                ViewBag.LoginType = "Login";
                ViewBag.Streaming = logindata.Streaming;
                return View(objModel);
            }
            else if (normaluserlogin == null && adlogin != null)
            {
                var adlogindata = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                if (adlogindata != null)
                {
                    HttpContext.Session.SetString("AdLoginTime", DateTime.Now.ToString());

                    var userName = adlogindata.EmailId.Trim();
                    var firstName = adlogindata.FirstName.Trim();
                    var lastName = adlogindata.LastName.Trim();
                    var fullName = adlogindata.FullName.Trim();
                    var RoleId = Convert.ToInt32(adlogindata.RoleId);
                    var Id = HttpContext.Session.Id;
                    List<string> ModelList = new List<string>();
                    var dbContexthr = new CropenAiContext();
                    ModelList = dbContexthr.CustomerModels.Where(x => x.CustomerId == 2).Select(x => x.ModelDisplayName).ToList();
                    string OrganizationLogoUrl = "";
                    string OrganizationName = "";
                    ViewBag.Logo = OrganizationLogoUrl == null ? string.Empty : OrganizationLogoUrl;
                    ViewBag.Role = RoleId;
                    ViewBag.listmodel = ModelList;
                    ViewBag.username = userName;
                    CropenAiContext cr = new CropenAiContext();
                    int adloginuserid = cr.CustomerInformations.Where(x => x.UserName == userName.Trim()).Select(x => x.CustomerId).FirstOrDefault();
                    ViewBag.CustomerId = adloginuserid;
                    int adlogincount = cr.CustomerInformations.Where(x => x.UserName == userName.Trim()).Count();
                    if (adlogincount == 1)
                    {
                        ViewBag.LoginType = "AdLogin";
                        ViewBag.Role = RoleId;
                    }
                    ViewBag.OrganizationName = OrganizationName == null ? string.Empty : OrganizationName;
                }
                return View(objModel);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        [Route("/autocomplete")]
        public async Task<IActionResult> Autocomplete(string input)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

            string connectionString = configuration.GetConnectionString("AutoSuggestionsConnection");

            List<string> suggestions = new List<string>();


            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("select Question from Suggestions", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string suggestion = reader.GetString(0);
                    if (!suggestions.Contains(suggestion))
                    {
                        suggestions.Add(suggestion);
                    }
                }
                con.Close();
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmdd = new SqlCommand("select Prompt from ModelFineTuneData where IsValid = 'true'", conn);
                conn.Open();
                SqlDataReader reader = cmdd.ExecuteReader();
                while (reader.Read())
                {
                    string suggestion = reader.GetString(0);
                    if (!suggestions.Contains(suggestion))
                    {
                        suggestions.Add(suggestion);
                    }
                }
                conn.Close();
            }

            return Json(new { suggestions });
        }

    }
}
