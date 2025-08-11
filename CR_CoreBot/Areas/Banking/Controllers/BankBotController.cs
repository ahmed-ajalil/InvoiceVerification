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
using System.Net.Http;
using DocumentFormat.OpenXml.Presentation;
using System.Data;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Word;
using Microsoft.Extensions.Logging;
using CR_CoreBot_DataAccess.Models;
using CR_CoreBot.common;

namespace CR_CoreBot.Areas.Banking.Controllers
{
    [Area("Banking")]
    public class BankBotController : Controller
    {
        static List<string> messageLists = new List<string>();
        static List<string> UserRequest = new List<string>();
        static List<string> ChatBotReply = new List<string>();
        private static ConnectionModel _appSettingconnection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BankBotController> _logger;

        private BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly SearchIndexerClient _indexerClient;
        private readonly string _indexerName;
        private readonly string _managedIdentityClientId;
        public BankBotController(IOptions<ConnectionModel> appSettingconnection, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ILogger<BankBotController> logger)
        {
            _appSettingconnection = appSettingconnection.Value;
            _httpContextAccessor = httpContextAccessor;
            AdminService adminService = new AdminService(_appSettingconnection, _httpContextAccessor);
            _appSettingconnection = adminService.getConnectionString();

            var blobconnectionString = configuration.GetSection("AzureBlobStorage:ConnectionString").Value;
            _containerName = configuration.GetSection("AzureBlobStorage:ContainerName").Value;
            _managedIdentityClientId = configuration.GetSection("AzureBlobStorage:ManagedIdentity").Value;
            //var searchServiceName = configuration["AzureSearch:SearchServiceName"];
           // var searchAdminApiKey = configuration["AzureSearch:AdminApiKey"];
            //_indexerName = configuration["AzureSearch:IndexerName"];
            var accountName = new System.Data.Common.DbConnectionStringBuilder
            {
                ConnectionString = blobconnectionString
            }["AccountName"].ToString();

            _blobServiceClient = new AIServicesDependent().blobServiceClient(accountName, _managedIdentityClientId);
           // _indexerClient = new AIServicesDependent().searchIndexerClient(searchServiceName, _managedIdentityClientId);

            //_blobServiceClient = new BlobServiceClient(blobconnectionString);
            //_indexerClient = new SearchIndexerClient(
            //    new Uri($"https://{searchServiceName}.search.windows.net"),
            //    new Azure.AzureKeyCredential(searchAdminApiKey)
            //);

            _logger = logger;
        }
        public async Task<IActionResult> Index(DataNewDTO objModel)
        {
            string rolename = string.Empty;
            var normaluserlogin = HttpContext.Session.GetString("LoginInfo");
            var adlogin = HttpContext.Session.GetString("AdLoginInfo");
            if (normaluserlogin != null)
            {
                var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);
                HttpContext.Session.SetString("UserLoginTime", DateTime.Now.ToString());
                List<string> ModelList = new List<string>();
                string role = "";
                using (var dbContexthr = new CropenAiContext())
                {
                    ModelList = dbContexthr.CustomerModels.Where(x => x.CustomerId == logindata.Id).Select(x => x.ModelDisplayName).ToList();
                }
                ViewBag.Logo = logindata.Logo;
                ViewBag.Role = logindata.RoleId;
                ViewBag.listmodel = ModelList;
                ViewBag.username = logindata.EmailId;
                ViewBag.CustomerId = logindata.Id;
                ViewBag.Category = logindata.Category;
                ViewBag.OrganizationName = logindata.OrganizationName;
                ViewBag.LoginType = "Login";
                return View(objModel);
            }
            else if (normaluserlogin == null && adlogin != null)
            {
                var adlogindata = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                if (adlogindata != null)
                {
                    HttpContext.Session.SetString("AdLoginTime", DateTime.Now.ToString());

                    //var userName = User.Identity.Name;
                    //var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value;
                    //var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
                    //var fullName = firstName + " " + lastName;

                    var userName = adlogindata.EmailId.Trim();
                    var firstName = adlogindata.FirstName.Trim();
                    var lastName = adlogindata.LastName.Trim();
                    var fullName = adlogindata.FullName.Trim();
                    var RoleId = Convert.ToInt32(adlogindata.RoleId);
                    var Id = HttpContext.Session.Id;
                    List<string> ModelList = new List<string>();
                    string role = "";
                    var dbContexthr = new CropenAiContext();
                    ModelList = dbContexthr.CustomerModels.Where(x => x.CustomerId == 2).Select(x => x.ModelDisplayName).ToList();
                    string OrganizationLogoUrl = "";
                    string OrganizationName = "";
                    ViewBag.Logo = OrganizationLogoUrl == null ? string.Empty : OrganizationLogoUrl;
                    ViewBag.Role = RoleId;
                    // ViewBag.Role = 3;
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
                    // ViewBag.Category = logindata.Category;
                    ViewBag.OrganizationName = OrganizationName == null ? string.Empty : OrganizationName;
                }
                return View(objModel);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> Bot(DataNewDTO objModel)
        {
            string dataReceived = "BankingModel";
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
                    if (logindata.EmailId.ToLower() == "demo@unifycloud.com")
                    {
                        CustomerInformation customerInformation = dbContexthr.CustomerInformations.Where(x => x.UserName.ToLower() == logindata.EmailId.ToLower()).FirstOrDefault();
                        ModelFineTuneDatum objlebelData = new ModelFineTuneDatum();
                        objModel = await ServiceMethods.Firstquestion(objModel, "bankingdocumentsdemo", "admin");
                        objModel.presuggestions1 = "Show me overdue compliance by Risk.";
                        objModel.presuggestions2 = "Show me all the overdue compliance in the Current Financial Year.";
                        objModel.presuggestions3 = "Show me overdue compliance by Entity/Location.";

                    }
                    else if (dataReceived != null && dataReceived == "BankingModel")
                    {
                        ModelList = dbContexthr.CustomerModels.Where(x => x.ModelDisplayName.ToLower().Trim() == "bankingmdel").Select(x => x.ModelDisplayName).ToList();
                        var blobcontainer = dbContexthr.CustomerInformations.Where(x => x.UserName.ToLower().Trim() == logindata.EmailId.ToLower().Trim()).Select(x => new { x.BlobContainerName, x.RoleId }).FirstOrDefault();
                        if (blobcontainer == null)
                        {
                            int customerID = Convert.ToInt32(dbContexthr.TblUserInformationMappings.Where(x => x.UserName.ToLower().Trim() == logindata.EmailId.ToLower().Trim()).Select(x => x.CustomerId).FirstOrDefault());
                            blobcontainer = dbContexthr.CustomerInformations.Where(x => x.CustomerId == customerID).Select(x => new { x.BlobContainerName, x.RoleId }).FirstOrDefault();
                        }
                        string Loginwith = dbContexthr.RoleMasters.Where(x => x.RoleId == blobcontainer.RoleId).Select(x => x.RoleName).FirstOrDefault();
                        //objModel = await ServiceMethods.Firstquestion(objModel, blobcontainer.BlobContainerName.ToLower(), Loginwith.ToLower());
                        objModel.presuggestions1 = "Show me the events with wheelchair access.";
                        objModel.presuggestions2 = "Which sessions have assistive listening available?";
                        objModel.presuggestions3 = "How many ticket types are there?";
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
        public async Task<IActionResult> Inde(DataNewDTO objModel, string guid, int modelId)
        {
            var normaluserlogin = HttpContext.Session.GetString("LoginInfo");
            var loginData = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);
            ServiceMethods security = new ServiceMethods(_appSettingconnection);
            ContentSafetySettings? ContentSafetySettings = await security.GetContentSafetySettings(loginData.EmailId);
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            DataNewDTO objdModel = new DataNewDTO();
            try
            {
                Dictionary<string, int> myDictionary = new Dictionary<string, int>()
{
     {"hi",1},
    {"hello",2},
    {"how r u",3},
    {"how are you",4},
    {"hii",5},
    {"hey",6},
    {"greetings",7},
    {"salutations",8},
    {"welcome",9},
    {"good day",10},
    {"yo",11},
    {"hiya",12},
    {"hola",13},
    {"aloha",14},
    {"sup",15},
    {"howdy",16},
    {"bonjour",17},
    {"ni hao",18},
    {"konnichiwa",19},
    {"shalom",20},
    {"namaste",22},
    {"ciao",23},
    {"wassup",24},
    {"what's up",25},
    {"hi there",26},
    {"hey there",27},
    {"g'day",28},
    {"kia ora",29},
    {"marhaba",30},
    {"merhaba",31},
    {"hallo",32},
    {"howzit",33},
    {"ahoy",34},
    {"long time no see",35},
    {"how do you do",36},
    {"what is going on",37},
    {"good to see you",38},
    {"namaskar",39},
    {"sat sri akal",40},
    {"salam aleikum",41},
    {"salut",42},
    {"hail",43},
    {"salve",44},
    {"cheers",45},
    {"how's you",46},
    {"what's crackin",47},
    {"how's it going",48},
    {"well, hello there",49},
    {"hey there! ready to get started?",50},
    {"hi, nice to see you! how can i assist today?",51},
    {"welcome aboard! how may i be of help?",52},
    {"good to have you here! what can i do for you?",53},
    {"greetings and salutations! how can i make your day better?",54},
    {"hello! i'm here to lend a hand. what do you need?",55},
    {"hi, it's great to meet you! how may i assist you today?",56},
    {"welcome! i'm at your service. what can i help you with?",57},
    {"hey! how can i make your experience more enjoyable?",58},
    {"hello, friend! what brings you here? how can i assist?",59},
    {"hello! how can i assist you today?",60},
    {"hi there! how may i help you?",61},
    {"welcome! how can i be of service?",62},
    {"greetings! what can i do for you?",63},
    {"good day! how may i assist you?",64},
    {"hey! how can i help you today?",65},
    {"hello! what brings you here?",66},
    {"hi! how can i make your day better?",67},
    {"welcome! how may i provide assistance?",68},
    {"greetings! how can i support you?",69},
    {"hey, good to see you! how can i assist today?",70},
    {"greetings! i'm here to help. what do you need?",71},
    {"hi there! ready to dive into your inquiries?",72},
    {"welcome! how can i make your day more productive?",73},
    {"hello, it's a pleasure to have you here. how may i assist?",74},
    {"hey, nice to meet you! what can i do to assist you today?",75},
    {"greetings! i'm here to provide you with the information you seek.",76},
    {"hi, welcome back! how can i continue supporting you?",77},
    {"hello! how can i make your experience more delightful?",78},
    {"hey there! i'm at your service. what can i help you with?",79},
    {"hi, it's great to have you here. how can i be of assistance?",80},
    {"welcome! i'm here to assist you on your journey. how may i help?",81},
    {"hello, friend! what can i do to simplify your tasks today?",82},
    {"greetings! how can i contribute to your success?",83},
    {"hi there! i'm ready to tackle any questions you have.",84},
    {"welcome! let's make your time here worthwhile. how can i assist?",85},
    {"hello! how can i provide you with the support you need?",86},
    {"hey, it's great to see you! what can i do to assist you today?",87},
    {"greetings! i'm here to make your experience exceptional.",88},
    {"hi, welcome back! how can i make your day more efficient?",89},
    {"hello, friend! how can i assist in achieving your goals?",90},
    {"hey there! i'm here to make things easier for you. what do you need?",91},
    {"welcome! how can i make your interaction with me delightful?",92},
    {"hi! i'm ready to assist you with any questions or concerns.",93},
    {"hello, it's a pleasure to connect with you! how can i be of service?",94},
    {"greetings! i'm here to ensure your experience is top-notch.",95},
    {"hi, nice to meet you! what can i do to enhance your productivity?",96},
    {"welcome! let's get started on your journey. how may i assist?",97},
    {"hello, friend! how can i make your time here more enjoyable?",98},
    {"hey there! i'm here to provide you with the support you need.",99},
    {"how is you",100},
    {"what is crackin",101},
    {"how is it going",102},
    {"hello! i am here to lend a hand. what do you need?",103},
    {"hi, it is great to meet you! how may i assist you today?",104},
    {"welcome! i am at your service. what can i help you with?",105},
    {"greetings! i am here to help. what do you need?",106},
    {"hello, it is a pleasure to have you here. how may i assist?",107},
    {"greetings! i am here to provide you with the information you seek.",108},
    {"hey there! i am at your service. what can i help you with?",109},
    {"hi, it is great to have you here. how can i be of assistance?",110},
    {"welcome! i am here to assist you on your journey. how may i help?",111},
    {"hi there! i am ready to tackle any questions you have.",112},
    {"welcome! let is make your time here worthwhile. how can i assist?",113},
    {"hey, it is great to see you! what can i do to assist you today?",114},
    {"greetings! i am here to make your experience exceptional.",115},
    {"hey there! i am here to make things easier for you. what do you need?",116},
    {"hi! i am ready to assist you with any questions or concerns.",117},
    {"hello, it is a pleasure to connect with you! how can i be of service?",118},
    {"greetings! i am here to ensure your experience is top-notch.",119},
    {"welcome! let is get started on your journey. how may i assist?",120},
    {"hey there! i am here to provide you with the support you need.",121},
    {"what is up",122},
    {"hi,good morning",123},
    {"hi,good afternoon",124},
    {"hi,good evening",125},
    {"good morning",126},
    {"good afternoon",127},
    {"good evening",128},
    {"hey,good morning",130},
    {"hey,good afternoon",131},
    {"hey,good evening",132},
};
                messageLists.Add(objModel.PropmpInput);

                // Function to check if the given string is present in the dictionary
                bool CheckStringInDictionary(string inputString, IDictionary<string, int> dictionary)
                {
                    return dictionary.ContainsKey(inputString.ToLower());
                }
                bool isPresent = CheckStringInDictionary(objModel.PropmpInput, myDictionary);

                
                objdModel.LabelName = new List<string>();
                objdModel.SourceURL = new List<string>();
                string recievedString = objModel.PropmpInput.Trim();
                ContentSafteyModal contentSafetyResult = await ServiceMethods.AnalyzeTextWithContentSafety(recievedString);

                if (contentSafetyResult.IsContentSafe)
                {
                    objModel.ContentSaftey = contentSafetyResult.IsContentSafe;
                    if (isPresent)
                    {
                        if (objModel.DocumentType == "BankingModel")
                        {
                            objdModel.CompletionResult = "Hello! I am Virtual Assistant. How may I assist you?";
                        }
                        else
                        {
                            objdModel.CompletionResult = "Hello! I am Virtual Assistant. How may I assist you?";
                        }

                        objdModel.PromptTokens = "0";
                        objdModel.CompletionTokens = "0";
                        objdModel.TotalTokens = "0";
                        objdModel.PropmpInput = objModel.PropmpInput.Trim();
                        objdModel.ResponseTime = 1.00;
                        objdModel.ContentSaftey = contentSafetyResult.IsContentSafe;
                        objdModel.CacheStatus = 2;
                        //objdModel.LabelName.Add(objModel != null ? objModel.LabelName : string.Empty);
                        //objdModel.SourceURL.Add(objModel != null ? objModel.SourceURL : string.Empty);

                        var SuggestedQuestion1 = Convert.ToString(AppSettingHelper.config.GetSection("SuggestedQuestions:Question1").Value);
                        var SuggestedQuestion2 = Convert.ToString(AppSettingHelper.config.GetSection("SuggestedQuestions:Question2").Value);
                        var SuggestedQuestion3 = Convert.ToString(AppSettingHelper.config.GetSection("SuggestedQuestions:Question3").Value);

                        objdModel.newSuggestions1 = SuggestedQuestion1;
                        objdModel.newSuggestions2 = SuggestedQuestion2;
                        objdModel.newSuggestions3 = SuggestedQuestion3;
                    }
                    else
                    {

                        //var normaluserlogin = HttpContext.Session.GetString("LoginInfo");
                        if (normaluserlogin != null)
                        {
                            var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);
                            CropenAiContext cr = new CropenAiContext();
                            if (logindata.EmailId.ToLower() == "demo@unifycloud.com")
                            {
                                CustomerInformation customerInformation = cr.CustomerInformations.Where(x => x.UserName.ToLower() == logindata.EmailId.ToLower()).FirstOrDefault();
                                customerInformation.BlobContainerName = ServiceMethods.GetBlobContainerNameByModel(objModel.DocumentType);
                                ModelFineTuneDatum objlebelData = new ModelFineTuneDatum();

                                objdModel = await ServiceMethods.DataProcessingOpenAI(objModel, customerInformation, objlebelData, guid, modelId);
                            }
                            else if (logindata.RoleId == 3)
                            {
                                CustomerInformation customerInformation = new CustomerInformation();
                                customerInformation.Category = logindata.Category;
                                customerInformation.OrganizationLogo = logindata.Logo;
                                customerInformation.Name = logindata.FullName;
                                customerInformation.UserName = logindata.EmailId;
                                customerInformation.RoleId = logindata.RoleId;
                                customerInformation.OrganizationName = logindata.OrganizationName;

                                ModelFineTuneDatum objlebelData = new ModelFineTuneDatum();
                                objdModel = await ServiceMethods.DataProcessingOpenAI(objModel, customerInformation, objlebelData, guid, modelId);
                            }
                            else
                            {
                                CustomerInformation customerInformation = cr.CustomerInformations.Where(x => x.UserName.ToLower() == logindata.EmailId.ToLower()).FirstOrDefault();
                                if (customerInformation == null)
                                {
                                    int customerID = Convert.ToInt32(cr.TblUserInformationMappings.Where(x => x.UserName.ToLower().Trim() == logindata.EmailId.ToLower().Trim()).Select(x => x.CustomerId).FirstOrDefault());
                                    customerInformation = cr.CustomerInformations.Where(x => x.CustomerId == customerID).FirstOrDefault();
                                }
                                customerInformation.BlobContainerName = ServiceMethods.GetBlobContainerNameByModel(objModel.DocumentType);
                                ModelFineTuneDatum objlebelData = new ModelFineTuneDatum();
                                objdModel = await ServiceMethods.DataProcessingOpenAI(objModel, customerInformation, objlebelData, guid, modelId);
                            }

                        }
                        else
                        {
                            var adlogin = HttpContext.Session.GetString("LoginInfo");
                            if (adlogin != null)
                            {
                                var adlogindata = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                                if (adlogindata != null)
                                {
                                    CropenAiContext cr = new CropenAiContext();
                                    if (adlogindata.EmailId.ToLower() == "demo@unifycloud.com")
                                    {
                                        CustomerInformation customerInformation = cr.CustomerInformations.Where(x => x.UserName.ToLower() == adlogindata.EmailId.ToLower()).FirstOrDefault();
                                        customerInformation.BlobContainerName = ServiceMethods.GetBlobContainerNameByModel(objModel.DocumentType);
                                        ModelFineTuneDatum objlebelData = new ModelFineTuneDatum();
                                        objdModel = await ServiceMethods.DataProcessingOpenAI(objModel, customerInformation, objlebelData, guid, modelId);
                                    }
                                    else
                                    {
                                        CustomerInformation customerInformation = cr.CustomerInformations.Where(x => x.UserName.ToLower() == adlogindata.EmailId.ToLower()).FirstOrDefault();
                                        customerInformation.BlobContainerName = ServiceMethods.GetBlobContainerNameByModel(objModel.DocumentType);
                                        ModelFineTuneDatum objlebelData = new ModelFineTuneDatum();
                                        objdModel = await ServiceMethods.DataProcessingOpenAI(objModel, customerInformation, objlebelData, guid, modelId);
                                    }

                                }
                            }
                        }
                    }

                    UserRequest.Add(objModel.PropmpInput);
                    ChatBotReply.Add(objdModel.CompletionResult);

                    if (configuration["Credential:RAIBox"] == "true")
                    {
                        ServiceMethods serviceMethods = new ServiceMethods(_appSettingconnection);
                        objdModel.ResponsibleAIAssessment = await serviceMethods.ContentSafetyApiCall(objdModel.StructuredData_Context, objdModel.CompletionResult, objModel.PropmpInput, ContentSafetySettings);
                        if (objdModel.ResponsibleAIAssessment.exception != null)
                        {

                            _logger.LogError("BotBankController", "ContentSafetyApiCall", objdModel.ResponsibleAIAssessment.exception);
                        }

                    }

                    return Json(objdModel);
                }
                else
                {
                    if (contentSafetyResult.HateSeverity > 0)
                        objdModel.CompletionResult = configuration["Severitymsg:HateSeverity"];
                    else if (contentSafetyResult.SelfHarmSeverity > 0)
                        objdModel.CompletionResult = configuration["Severitymsg:SelfHarmSeverity"];
                    else if (contentSafetyResult.SexualSeverity > 0)
                        objdModel.CompletionResult = configuration["Severitymsg:SexualSeverity"];
                    else if (contentSafetyResult.ViolenceSeverity > 0)
                        objdModel.CompletionResult = configuration["Severitymsg:ViolenceSeverity"];
                    else
                        objdModel.CompletionResult = configuration["Severitymsg:Other"];
                    objdModel.PromptTokens = "0";
                    objdModel.CompletionTokens = "0";
                    objdModel.TotalTokens = "0";
                    objdModel.PropmpInput = objModel.PropmpInput.Trim();
                    objdModel.ResponseTime = 1.00;
                    objdModel.ContentSaftey = contentSafetyResult.IsContentSafe;
                    objdModel.CacheStatus = 2;
                    UserRequest.Add(objModel.PropmpInput);
                    ChatBotReply.Add(objdModel.CompletionResult);
                    if (configuration["Credential:RAIBox"] == "true")
                    {
                        ServiceMethods serviceMethods = new ServiceMethods(_appSettingconnection);
                        objdModel.ResponsibleAIAssessment = await serviceMethods.ContentSafetyApiCall(objdModel.StructuredData_Context, objdModel.CompletionResult, objModel.PropmpInput, ContentSafetySettings);
                        if (objdModel.ResponsibleAIAssessment.exception != null)
                        {

                            _logger.LogError("BotBankController", "ContentSafetyApiCall", objdModel.ResponsibleAIAssessment.exception);
                        }

                    }
                    return Json(objdModel);
                }
            }
            catch (Exception ex)
            {
                //Security Purpose
                List<string> groundednessRegions = configuration.GetSection("Credential:GroundednessAvailablityRegions").Get<List<string>>();
                List<string> toxicityRegions = configuration.GetSection("Credential:ToxicityAvailablityRegions").Get<List<string>>();
                List<string> promptInjectionRegions = configuration.GetSection("Credential:PromptInjectionAvailablityRegions").Get<List<string>>();
                List<string> plagraismRegions = configuration.GetSection("Credential:PlagraismAvailablityRegions").Get<List<string>>();
                objdModel.ResponsibleAIAssessment.groundednessAvailablity = groundednessRegions.Contains(configuration["Credential:ContentSafetyRegion"]);
                objdModel.ResponsibleAIAssessment.plagraismAvailablity = plagraismRegions.Contains(configuration["Credential:ContentSafetyRegion"]);
                objdModel.ResponsibleAIAssessment.promptInjectionAvailablity = promptInjectionRegions.Contains(configuration["Credential:ContentSafetyRegion"]);
                objdModel.ResponsibleAIAssessment.toxicityAvailablity = toxicityRegions.Contains(configuration["Credential:ContentSafetyRegion"]);
                if (configuration["BotInfo:ResponsibleAI"] == "true")
                {
                    objdModel.ResponsibleAIAssessment.isSafe = false;
                }
                else
                {
                    objdModel.ResponsibleAIAssessment.isSafe = null;
                }
                if (ex.Message.Contains("Found invalid data while decoding"))
                {
                    if (configuration["BotInfo:RAIBox"] == "false")
                    {
                        return Json(new { success = true, message = "Action completed successfully." });
                    }
                    else
                    {
                        //await chathub.SendMessages(_user.GUID, Span, "<span style='color: red'>. Further Response is blocked because of Toxic output. For more details check <img src='/Content/assets/images/arrowDown.svg' style='width: 2.5%'></span>", Connectionid);
                        //await chathub.SendMessages(_user.GUID, Span, $"<div id=\"ContentSafe_{Span}\"></div>", Connectionid);
                        //await chathub.responseComplete(_user.GUID, Span, Question, Connectionid, 0);
                        objdModel.ResponsibleAIAssessment.isSafe = false;
                        objdModel.ResponsibleAIAssessment.groundednessAvailablity = groundednessRegions.Contains(configuration["Credential:ContentSafetyRegion"]);
                        objdModel.ResponsibleAIAssessment.hallucinationResponse = null;
                        
                        objdModel.ResponsibleAIAssessment.PlagiarismRes = new PlagiarismResponseCS();
                        objdModel.ResponsibleAIAssessment.PlagiarismRes.ProtectedMaterialAnalysis = null;
                        objdModel.ResponsibleAIAssessment.PromptInjectionRes = new PromptInjectionResponseCS();
                        objdModel.ResponsibleAIAssessment.PromptInjectionRes.UserPromptAnalysis = new CR_CoreBot_DTO.UserPromptAnalysis();
                        objdModel.ResponsibleAIAssessment.PromptInjectionRes.UserPromptAnalysis.AttackDetected = false;
                        objdModel.ResponsibleAIAssessment.ToxicityRes = new assessDTO();
                        objdModel.ResponsibleAIAssessment.ToxicityRes.SelfHarmScore = 4;
                        objdModel.ResponsibleAIAssessment.ToxicityRes.HateScore = 4;
                        objdModel.ResponsibleAIAssessment.ToxicityRes.ViolenceScore = 4;
                        objdModel.ResponsibleAIAssessment.ToxicityRes.SexualScore = 4;

                        return Ok(objdModel);
                    }
                }
                else
                {
                   
                    _logger.LogError("PlayGroundController", "Chat", objdModel.ResponsibleAIAssessment.exception);
                    if (configuration["Credential:RAIBox"] == "true")
                    {
                        return Json(new { success = false, message = ex.Message, securityOn = true, objdModel });
                    }
                    else
                    {
                        return Json(new { success = false, message = ex.Message, securityOn = false });
                    }
                }
            }
        }

        [HttpGet]
        public string GenerateGUID()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        public async Task<IActionResult> GetIntermediateSteps(string guid)
        {
            try
            {
                IEnumerable<Streaming> data = null;
                int attempts = 0;
                const int maxAttempts = 40;
                const int delayMilliseconds = 2000;

                while (attempts < maxAttempts)
                {
                    data = await ServiceMethods.GetIntermediateSteps(guid);

                    if (data.Any())
                    {
                        break;
                    }

                    attempts++;
                    await System.Threading.Tasks.Task.Delay(delayMilliseconds);
                }

                if (data == null || !data.Any())
                {
                    return StatusCode(500, new { message = "No data found after multiple attempts" });
                }

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult downloadExcel(string query)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = ServiceMethods.downloadExcel(query);
            string json = JsonConvert.SerializeObject(dt);
            return Ok(json);
        }

        [HttpPost]
        public IActionResult FeedbackData(ModelFineTuneDatum objModel, int PerformanceID)
        {
            try
            {
                int role = 0;
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                var adlogin = HttpContext.Session.GetString("AdLoginInfo");
                if (serializedObject != null)
                {
                    LoginInfoDTO logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(serializedObject);
                    role = Convert.ToInt32(logindata.RoleId);
                    objModel.Username = logindata.EmailId.Trim();
                    if (role == 1 || role == 2)
                    {
                        objModel.UserId = null;
                        CropenAiContext crcon = new CropenAiContext();
                        var performanceobj = crcon.PerformanceMatrixCheckers.Where(x => x.Prompt.Trim() == objModel.Prompt.Trim() && x.PeformanceId == PerformanceID && x.Username.Trim() == logindata.EmailId.Trim()).Select(x => new
                        {
                            x.PeformanceId,
                            x.PromptTokens,
                            x.CompletionTokens,
                            x.TotalTokens,
                            x.ResponseTime,
                            x.Cost,
                            x.CurrentDateTime
                        }).FirstOrDefault();

                        if (performanceobj != null)
                        {
                            ServiceMethods serviceMethods = new ServiceMethods(_appSettingconnection);
                            var result = serviceMethods.FeedbackDataSave(objModel, performanceobj.PromptTokens, performanceobj.CompletionTokens, performanceobj.TotalTokens, Convert.ToDecimal(performanceobj.ResponseTime), Convert.ToDecimal(performanceobj.Cost), role, performanceobj.CurrentDateTime, PerformanceID);
                            return Json(result);
                        }
                        else
                        {
                            return Json("");
                        }

                    }
                    else if (role == 3)
                    {
                        objModel.UserId = Convert.ToInt32(logindata.UserId);
                        objModel.CustomerId = null;
                        CropenAiContext crcon = new CropenAiContext();
                        var performanceobj2 = crcon.PerformanceMatrixCheckers.Where(x => x.Prompt.Trim() == objModel.Prompt.Trim() && x.PeformanceId == PerformanceID && x.Username.Trim() == logindata.EmailId.Trim()).Select(x => new
                        {
                            x.PeformanceId,
                            x.PromptTokens,
                            x.CompletionTokens,
                            x.TotalTokens,
                            x.ResponseTime,
                            x.Cost,
                            x.CurrentDateTime
                        }).FirstOrDefault();

                        if (performanceobj2 != null)
                        {
                            ServiceMethods serviceMethods = new ServiceMethods(_appSettingconnection);
                            var result = serviceMethods.FeedbackDataSave(objModel, performanceobj2.PromptTokens, performanceobj2.CompletionTokens, performanceobj2.TotalTokens, Convert.ToDecimal(performanceobj2.ResponseTime), Convert.ToDecimal(performanceobj2.Cost), role, performanceobj2.CurrentDateTime, PerformanceID);
                            return Json(result);
                        }
                        else
                        {
                            return Json("");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }
                }
                else if (serializedObject == null && adlogin != null)
                {
                    AdLoginInfoDTO adlogindata = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                    if (adlogindata != null)
                    {
                        var userName = adlogindata.EmailId.Trim();
                        var firstName = adlogindata.FirstName.Trim();
                        var lastName = adlogindata.LastName.Trim();
                        var fullName = adlogindata.FullName.Trim();

                        ViewBag.Role = 3;
                        CropenAiContext cr = new CropenAiContext();
                        int adlogincount = cr.CustomerInformations.Where(x => x.UserName == userName.Trim()).Count();
                        if (adlogincount == 1)
                        {
                            ViewBag.LoginType = "AdLogin";
                        }
                        objModel.Username = userName.Trim();
                        objModel.UserId = cr.CustomerInformations.Where(x => x.UserName == userName.Trim()).Select(x => x.CustomerId).FirstOrDefault();
                        objModel.CustomerId = null;
                        CropenAiContext crcon = new CropenAiContext();
                        var performanceobj3 = crcon.PerformanceMatrixCheckers.Where(x => x.Prompt.Trim() == objModel.Prompt.Trim() && x.PeformanceId == PerformanceID && x.Username.Trim() == userName.Trim()).Select(x => new
                        {
                            x.PeformanceId,
                            x.PromptTokens,
                            x.CompletionTokens,
                            x.TotalTokens,
                            x.ResponseTime,
                            x.Cost,
                            x.CurrentDateTime
                        }).FirstOrDefault();
                        role = -11;
                        if (performanceobj3 != null)
                        {
                            ServiceMethods serviceMethods = new ServiceMethods(_appSettingconnection);
                            var result = serviceMethods.FeedbackDataSave(objModel, performanceobj3.PromptTokens, performanceobj3.CompletionTokens, performanceobj3.TotalTokens, Convert.ToDecimal(performanceobj3.ResponseTime), Convert.ToDecimal(performanceobj3.Cost), role, performanceobj3.CurrentDateTime, PerformanceID);
                            return Json(result);
                        }
                        else
                        {
                            return Json("");
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
                return Json(ex.Message.ToString());
            }
        }

        public IActionResult GetModelName()
        {
            using var context = new HrportalAiContext(_appSettingconnection);
            var model = context.ModelDetails.OrderBy(m => m.Id).ToList();
            if (model == null)
            {
                return NotFound();
            }
            return Json(model);
        }

        [HttpPost]
        public IActionResult ClearChat()
        {
            try
            {
                UserRequest.Clear();
                ChatBotReply.Clear();
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult RefreshChat()
        {
            try
            {
                UserRequest.Clear();
                ChatBotReply.Clear();
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckFileExists(string fileName)
        {
            var _accountName = "";
            try
            {
                string sanitizedFileName = Uri.EscapeDataString(fileName);
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(sanitizedFileName);

                _accountName = _blobServiceClient.AccountName;

                if (await blobClient.ExistsAsync())
                {
                    return Ok(new
                    {
                        exists = true,
                        accountName = _accountName,
                        containerName = _containerName
                    });
                }

                return Ok(new
                {
                    exists = false,
                    accountName = _accountName,
                    containerName = _containerName
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    exists = false,
                    accountName = _accountName,
                    containerName = _containerName
                });
            }

        }

        [HttpPost]
        public async Task<string> fnALLChatbot(string UserChat, string BotReply, string OrganizationName)
        {
            try
            {
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                LoginInfoDTO logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(serializedObject);

                if (logindata.Streaming == 0)
                {
                    if (UserChat != null && UserRequest.Count > 0)
                    {
                        var fileContent = "";
                        fileContent += OrganizationName + "\n";
                        fileContent += (DateTime.Now.ToString("HH:mm")) + "\n";
                        for (int i = 0; i < UserRequest.Count; i++)
                        {
                            fileContent += "Question " + (i + 1) + " : " + UserRequest[i] + "\n";
                            fileContent += "Answer " + (i + 1) + " : " + ChatBotReply[i] + "\n\n";
                        }
                        return fileContent;
                    }
                    else
                    {
                        return "N/A";
                    }
                }
                else
                {
                    if (UserChat != null && BotReply != null && UserRequest.Count > 0)
                    {
                        string text = BotReply;
                        string[] splitText = text.Split(new string[] { "Thought Process" }, StringSplitOptions.None);

                        List<string> Reply = new List<string>();

                        for (int i = 1; i < splitText.Length; i++)
                        {
                            string trimmedText = splitText[i].Trim();
                            if (!string.IsNullOrEmpty(trimmedText))
                            {
                                Reply.Add(trimmedText);
                            }
                        }


                        var fileContent = "";
                        fileContent += OrganizationName + "\n";
                        fileContent += (DateTime.Now.ToString("HH:mm")) + "\n";
                        for (int i = 0; i < UserRequest.Count; i++)
                        {
                            fileContent += "Question " + (i + 1) + " : " + UserRequest[i] + "\n";
                            //fileContent += "Answer " + (i + 1) + " : " + ChatBotReply[i] + "\n\n";
                            fileContent += "Answer " + (i + 1) + " : " + Reply[i] + "\n\n";
                        }
                        return fileContent;
                    }
                    else
                    {
                        return "N/A";
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        [HttpPost]
        public async Task<bool> fnSessionEnd(string UserChat, string BotReply, string OrganizationName)
        {
            try
            {
                if (UserChat != null && BotReply != null)
                {
                    var fileContent = "";
                    fileContent += OrganizationName + "\n";
                    fileContent += (DateTime.Now.ToString("HH:mm")) + "\n";
                    for (int i = 0; i < UserRequest.Count; i++)
                    {
                        fileContent += UserRequest[i] + "\n";
                        fileContent += ChatBotReply[i] + "\n\n";
                    }

                    string textFilePath = "wwwroot\\txtFile\\";
                    string filePath = "";
                    filePath = textFilePath + (OrganizationName + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt");
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    using (FileStream aFile = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(aFile))
                    {
                        sw.WriteLine(fileContent);
                    }
                    string Connection = "your storage connection string";
                    BlobServiceClient blobServiceClient = new BlobServiceClient(Connection);
                    string containerName = "test-pdf";
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                    string fileName = Path.GetFileName(filePath);
                    BlobClient blobClient = containerClient.GetBlobClient(fileName);
                    using FileStream fileStream = System.IO.File.OpenRead(filePath);
                    await blobClient.UploadAsync(fileStream, true);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        [HttpGet]
        public ActionResult Feedback()
        {
            try
            {
                int role = 0;
                int CustomerId = 0;
                int? UserId = 0;
                string FullName = string.Empty;
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
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
                        ViewBag.EmailId = loginInfo.EmailId.Trim();
                        ViewBag.LoginType = "Login";
                        ViewBag.Role = role;
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }
                }
                else if (serializedObject == null)
                {
                    var serializedAdObject = HttpContext.Session.GetString("AdLoginInfo");
                    if (serializedAdObject != null)
                    {
                        AdLoginInfoDTO adLoginInfoDTO = JsonConvert.DeserializeObject<AdLoginInfoDTO>(serializedAdObject);
                        if (adLoginInfoDTO != null)
                        {
                            CropenAiContext cr = new CropenAiContext();
                            ViewBag.EmailId = adLoginInfoDTO.EmailId.Trim();
                            int adlogincount = cr.CustomerInformations.Where(x => x.UserName == adLoginInfoDTO.EmailId.Trim()).Count();
                            if (adlogincount == 1)
                            {
                                ViewBag.LoginType = "AdLogin";
                                ViewBag.Role = 3;
                            }
                        }
                        return View();
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
                return View(ex.Message.ToString());
            }
        }
        public async Task<string> fnChatReply(DataNewDTO objModel)
        {
            DataNewDTO objdModel = new DataNewDTO();
            try
            {
                objdModel.LabelName = new List<string>();
                objdModel.SourceURL = new List<string>();
                Dictionary<string, int> myDictionary = new Dictionary<string, int>()
{
    {"hi",1},
    {"hello",2},
    {"how r u",3},
    {"how are you",4},
    {"hii",5},
    {"hey",6},
    {"greetings",7},
    {"salutations",8},
    {"welcome",9},
    {"good day",10},
    {"yo",11},
    {"hiya",12},
    {"hola",13},
    {"aloha",14},
    {"sup",15},
    {"howdy",16},
    {"bonjour",17},
    {"ni hao",18},
    {"konnichiwa",19},
    {"shalom",20},
    {"namaste",22},
    {"ciao",23},
    {"wassup",24},
    {"what's up",25},
    {"hi there",26},
    {"hey there",27},
    {"g'day",28},
    {"kia ora",29},
    {"marhaba",30},
    {"merhaba",31},
    {"hallo",32},
    {"howzit",33},
    {"ahoy",34},
    {"long time no see",35},
    {"how do you do",36},
    {"what is going on",37},
    {"good to see you",38},
    {"namaskar",39},
    {"sat sri akal",40},
    {"salam aleikum",41},
    {"salut",42},
    {"hail",43},
    {"salve",44},
    {"cheers",45},
    {"how's you",46},
    {"what's crackin",47},
    {"how's it going",48},
    {"well, hello there",49},
    {"hey there! ready to get started?",50},
    {"hi, nice to see you! how can i assist today?",51},
    {"welcome aboard! how may i be of help?",52},
    {"good to have you here! what can i do for you?",53},
    {"greetings and salutations! how can i make your day better?",54},
    {"hello! i'm here to lend a hand. what do you need?",55},
    {"hi, it's great to meet you! how may i assist you today?",56},
    {"welcome! i'm at your service. what can i help you with?",57},
    {"hey! how can i make your experience more enjoyable?",58},
    {"hello, friend! what brings you here? how can i assist?",59},
    {"hello! how can i assist you today?",60},
    {"hi there! how may i help you?",61},
    {"welcome! how can i be of service?",62},
    {"greetings! what can i do for you?",63},
    {"good day! how may i assist you?",64},
    {"hey! how can i help you today?",65},
    {"hello! what brings you here?",66},
    {"hi! how can i make your day better?",67},
    {"welcome! how may i provide assistance?",68},
    {"greetings! how can i support you?",69},
    {"hey, good to see you! how can i assist today?",70},
    {"greetings! i'm here to help. what do you need?",71},
    {"hi there! ready to dive into your inquiries?",72},
    {"welcome! how can i make your day more productive?",73},
    {"hello, it's a pleasure to have you here. how may i assist?",74},
    {"hey, nice to meet you! what can i do to assist you today?",75},
    {"greetings! i'm here to provide you with the information you seek.",76},
    {"hi, welcome back! how can i continue supporting you?",77},
    {"hello! how can i make your experience more delightful?",78},
    {"hey there! i'm at your service. what can i help you with?",79},
    {"hi, it's great to have you here. how can i be of assistance?",80},
    {"welcome! i'm here to assist you on your journey. how may i help?",81},
    {"hello, friend! what can i do to simplify your tasks today?",82},
    {"greetings! how can i contribute to your success?",83},
    {"hi there! i'm ready to tackle any questions you have.",84},
    {"welcome! let's make your time here worthwhile. how can i assist?",85},
    {"hello! how can i provide you with the support you need?",86},
    {"hey, it's great to see you! what can i do to assist you today?",87},
    {"greetings! i'm here to make your experience exceptional.",88},
    {"hi, welcome back! how can i make your day more efficient?",89},
    {"hello, friend! how can i assist in achieving your goals?",90},
    {"hey there! i'm here to make things easier for you. what do you need?",91},
    {"welcome! how can i make your interaction with me delightful?",92},
    {"hi! i'm ready to assist you with any questions or concerns.",93},
    {"hello, it's a pleasure to connect with you! how can i be of service?",94},
    {"greetings! i'm here to ensure your experience is top-notch.",95},
    {"hi, nice to meet you! what can i do to enhance your productivity?",96},
    {"welcome! let's get started on your journey. how may i assist?",97},
    {"hello, friend! how can i make your time here more enjoyable?",98},
    {"hey there! i'm here to provide you with the support you need.",99},
    {"how is you",100},
    {"what is crackin",101},
    {"how is it going",102},
    {"hello! i am here to lend a hand. what do you need?",103},
    {"hi, it is great to meet you! how may i assist you today?",104},
    {"welcome! i am at your service. what can i help you with?",105},
    {"greetings! i am here to help. what do you need?",106},
    {"hello, it is a pleasure to have you here. how may i assist?",107},
    {"greetings! i am here to provide you with the information you seek.",108},
    {"hey there! i am at your service. what can i help you with?",109},
    {"hi, it is great to have you here. how can i be of assistance?",110},
    {"welcome! i am here to assist you on your journey. how may i help?",111},
    {"hi there! i am ready to tackle any questions you have.",112},
    {"welcome! let is make your time here worthwhile. how can i assist?",113},
    {"hey, it is great to see you! what can i do to assist you today?",114},
    {"greetings! i am here to make your experience exceptional.",115},
    {"hey there! i am here to make things easier for you. what do you need?",116},
    {"hi! i am ready to assist you with any questions or concerns.",117},
    {"hello, it is a pleasure to connect with you! how can i be of service?",118},
    {"greetings! i am here to ensure your experience is top-notch.",119},
    {"welcome! let is get started on your journey. how may i assist?",120},
    {"hey there! i am here to provide you with the support you need.",121},
    {"what is up",122},
    {"hi,good morning",123},
    {"hi,good afternoon",124},
    {"hi,good evening",125},
    {"good morning",126},
    {"good afternoon",127},
    {"good evening",128},
    {"hey,good morning",130},
    {"hey,good afternoon",131},
    {"hey,good evening",132},
};
                messageLists.Add(objModel.PropmpInput);
                UserRequest.Add(objModel.PropmpInput);
                // Function to check if the given string is present in the dictionary
                CropenAiContext cr = new CropenAiContext();
                var normaluserlogin = HttpContext.Session.GetString("LoginInfo");
                var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);
                CustomerInformation customerInformation = cr.CustomerInformations.Where(x => x.UserName.ToLower() == logindata.EmailId.ToLower()).FirstOrDefault();
                bool CheckStringInDictionary(string inputString, IDictionary<string, int> dictionary)
                {
                    return dictionary.ContainsKey(inputString.ToLower());
                }
                bool isPresent = CheckStringInDictionary(objModel.PropmpInput, myDictionary);
                if (isPresent)
                {
                    objdModel.CompletionResult = "Hello! I am Hr Assistant. How may I assist you?";
                    objdModel.PromptTokens = "0";
                    objdModel.CompletionTokens = "0";
                    objdModel.TotalTokens = "0";
                    objdModel.PropmpInput = objModel.PropmpInput.Trim();
                    objdModel.ResponseTime = 1.00;
                }
                else
                {
                    string guid = "";
                    int modelId = 0;
                    ModelFineTuneDatum objlebelData = new ModelFineTuneDatum();
                    objdModel = await ServiceMethods.DataProcessingOpenAI(objModel, customerInformation, objlebelData, guid, modelId);
                }
                ChatBotReply.Add(objdModel.CompletionResult);
            }
            catch (Exception ex)
            {
                objdModel.CompletionResult = ex.Message.ToString();
                objdModel.PromptTokens = "0";
                objdModel.CompletionTokens = "0";
                objdModel.TotalTokens = "0";
                objdModel.PropmpInput = objModel.PropmpInput.Trim();
                objdModel.ResponseTime = 1.00;
                ChatBotReply.Add(objdModel.CompletionResult);
            }

            return objdModel.CompletionResult;
        }
    }
}
