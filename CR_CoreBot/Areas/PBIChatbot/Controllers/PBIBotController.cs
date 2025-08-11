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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
namespace CR_CoreBot.Areas.PBIChatbot.Controllers
{
    [Area("PBIChatbot")]

    public class PBIBotController : Controller
    {
        static List<string> messageLists = new List<string>();
        static List<string> UserRequest = new List<string>();
        static List<string> ChatBotReply = new List<string>();
        private static ConnectionModel _appSettingconnection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PBIBotController(IOptions<ConnectionModel> appSettingconnection, IHttpContextAccessor httpContextAccessor, IOptions<AppSettingsDTO> appSettings)
        {
            _appSettingconnection = appSettingconnection.Value;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> bot(DataNewDTO objModel)
        {
            objModel.PropmpInput = "Generate only 3 questions from the documents";
            objModel.presuggestions1 = "Provide total billing amounts and outstanding invoices for each company for the current fiscal year?";
            objModel.presuggestions2 = "What are the average billing amounts per level of staff involved in processes across different teams for the past quarter?";
            objModel.presuggestions3 = "List all projects with final billing amounts that significantly deviated from initial estimates and identify the underlying causes?";
            objModel.ContentSaftey = false;
            objModel.Cost = 0;
            objModel.personalbanking = false;

            //these
            ViewBag.Logo = "https://blobstudiobravo.blob.core.windows.net/nimo/ITL-Logo.png";
            ViewBag.username = "itladmin";
            ViewBag.CustomerId = 2073;
            ViewBag.Category = "FinanceModel";

            ViewBag.Role = 1;
            ViewBag.listmodel = "";
            ViewBag.OrganizationName = "";
            ViewBag.LoginType = "Login";

            return View(objModel);
        }

        [HttpGet]
        [Route("/autocompleteITL2")]
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

        public async Task<IActionResult> Inde(DataNewDTO objModel, string guid)
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
            UserRequest.Add(objModel.PropmpInput);

            bool CheckStringInDictionary(string inputString, IDictionary<string, int> dictionary)
            {
                return dictionary.ContainsKey(inputString.ToLower());
            }
            bool isPresent = CheckStringInDictionary(objModel.PropmpInput, myDictionary);


            DataNewDTO objdModel = new DataNewDTO();
            objdModel.LabelName = new List<string>();
            objdModel.SourceURL = new List<string>();
            string recievedString = objModel.PropmpInput.Trim();
            ContentSafteyModal contentSafetyResult = await ServiceMethods.AnalyzeTextWithContentSafety(recievedString);


            if (contentSafetyResult.IsContentSafe)
            {
                objModel.ContentSaftey = contentSafetyResult.IsContentSafe;
                if (isPresent)
                {
                    objdModel.CompletionResult = "Hello! I am Virtual Assistant. How may I assist you?";
                    objdModel.PromptTokens = "0";
                    objdModel.CompletionTokens = "0";
                    objdModel.TotalTokens = "0";
                    objdModel.PropmpInput = objModel.PropmpInput.Trim();
                    objdModel.ResponseTime = 1.00;
                    objdModel.ContentSaftey = contentSafetyResult.IsContentSafe;
                }
                else
                {
                    ModelFineTuneDatum objlebelData = new ModelFineTuneDatum();
                    CustomerInformation customerInformation = new CustomerInformation();
                    
                    // Removed sensitive keys - these should be loaded from configuration
                    customerInformation.AccountKey = "[ACCOUNT_KEY]";
                    customerInformation.AccountName = "blobstudiobravo";

                    //these
                    customerInformation.CustomerId = 2073;
                    customerInformation.DatabaseName = "IntercontinentalTrustLimited10062024160205";
                    customerInformation.Name = "ITL";
                    customerInformation.OpenAiindexName = "intercontinentaltrustlimited10062024160205";
                    customerInformation.OrganizationLogo = "https://blobstudiobravo.blob.core.windows.net/nimo/ITL-Logo.png";
                    customerInformation.OrganizationName = "Intercontinental Trust Limited";
                    customerInformation.UserName = "itladmin";
                    customerInformation.Category = "FinanceModel";
                    customerInformation.BlobContainerName = "itl";

                    //customerInformation.DateTime = DateTime.Now;
                    customerInformation.LoginWith = "Login";

                    //customerInformation.Password = "12345";
                    customerInformation.RoleId = 1;

                    int modelId = 0;
                    objdModel = await ServiceMethods.DataProcessingOpenAI(objModel, customerInformation, objlebelData, guid, modelId);
                }
                ChatBotReply.Add(objdModel.CompletionResult);
                return Json(objdModel);
            }
            else
            {
                if (contentSafetyResult.HateSeverity > 0)
                    objdModel.CompletionResult = "Hate Severity.";
                else if (contentSafetyResult.SelfHarmSeverity > 0)
                    objdModel.CompletionResult = "Self Harm Severity.";
                else if (contentSafetyResult.SexualSeverity > 0)
                    objdModel.CompletionResult = "Sexual Severity.";
                else if (contentSafetyResult.ViolenceSeverity > 0)
                    objdModel.CompletionResult = "Violence Severity.";
                else
                    objdModel.CompletionResult = "Unsafe.";
                objdModel.PromptTokens = "0";
                objdModel.CompletionTokens = "0";
                objdModel.TotalTokens = "0";
                objdModel.PropmpInput = objModel.PropmpInput.Trim();
                objdModel.ResponseTime = 1.00;
                objdModel.ContentSaftey = contentSafetyResult.IsContentSafe;
                ChatBotReply.Add(objdModel.CompletionResult);
                return Json(objdModel);
            }
        }

        [HttpPost]
        public IActionResult AddPerformanceMatrixChecker(PerformanceMatrixChecker performance)
        {
            string result = "false";
            int? PerformanceId = 0;

            if (result == "false")
            {
                AdminService adminService = new AdminService(_appSettingconnection, _httpContextAccessor);
                PerformanceMatrixChecker performanceobj = new PerformanceMatrixChecker();

                //this
                performanceobj.Username = "itladmin";

                performanceobj.Prompt = performance.Prompt.Trim();
                performanceobj.Completion = performance.Completion.Trim();
                performanceobj.ResponseTime = performance.ResponseTime;
                performanceobj.PromptTokens = performance.PromptTokens;
                performanceobj.CompletionTokens = performance.CompletionTokens;
                performanceobj.TotalTokens = performance.TotalTokens;
                performanceobj.Cost = performance.Cost;
                performanceobj.IsValid = null;
                performanceobj.PublicInfo = Convert.ToBoolean(performance.PublicInfo);
                performanceobj.LoginType = "Login";
                performanceobj.Role = "Admin";

                performanceobj.RoleId = 1;
                PerformanceId = adminService.fnAddPerformanceMatrixChecker(performanceobj);
                result = "true";
                return Json(PerformanceId);
            }
            else
            {
                return Json("red");
            }
        }

        [HttpPost]
        public IActionResult FeedbackData(ModelFineTuneDatum objModel, int PerformanceID)
        {
            int role = 1;
            objModel.UserId = null;
            CropenAiContext crcon = new CropenAiContext();

            var performanceobj = crcon.PerformanceMatrixCheckers.Where(x => x.PeformanceId == PerformanceID).Select(x => new
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
                var result = "data saved successfully";
                List<PerformanceMatrixChecker> performanceMatrixCheckerobj = crcon.PerformanceMatrixCheckers.Where(x => x.PeformanceId == PerformanceID).ToList();
                if (performanceMatrixCheckerobj != null)
                {

                    for (int i = 0; i < performanceMatrixCheckerobj.Count; i++)
                    {
                        performanceMatrixCheckerobj[i].Username = performanceMatrixCheckerobj[0].Username.Trim();
                        performanceMatrixCheckerobj[i].CurrentDateTime = DateTime.Now;
                        performanceMatrixCheckerobj[i].Prompt = objModel.Prompt;
                        performanceMatrixCheckerobj[i].Completion = objModel.Completion;
                        performanceMatrixCheckerobj[i].IsValid = objModel.IsValid;
                        performanceMatrixCheckerobj[i].PromptTokens = performanceobj.PromptTokens;
                        performanceMatrixCheckerobj[i].CompletionTokens = performanceobj.CompletionTokens;
                        performanceMatrixCheckerobj[i].TotalTokens = performanceobj.TotalTokens;
                        performanceMatrixCheckerobj[i].Cost = Convert.ToDecimal(performanceobj.Cost);
                        performanceMatrixCheckerobj[i].ResponseTime = Convert.ToDecimal(performanceobj.ResponseTime);
                        crcon.Entry(performanceMatrixCheckerobj[i]).State = EntityState.Modified;
                        crcon.SaveChanges();
                    }
                }

                return Json(result);
            }
            else
            {
                return Json("");
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
                    await Task.Delay(delayMilliseconds);
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
            DataTable dt = new DataTable();
            dt = ServiceMethods.downloadExcel(query);
            string json = JsonConvert.SerializeObject(dt);
            return Ok(json);
        }

    }
}
