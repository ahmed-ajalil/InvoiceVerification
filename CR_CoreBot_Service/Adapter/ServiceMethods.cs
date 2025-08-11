using Azure;
using Azure.AI.ContentSafety;
using Azure.Storage.Blobs;
using CR_CoreBot_DataAccess.CR_OpenAI;
using CR_CoreBot_DataAccess.Models;
using CR_CoreBot_DTO;
using CR_HRPortalAI_DataAcess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web;
using HrportalAiContext = CR_HRPortalAI_DataAcess.CR_HRPortal.HrportalAiContext;
using ModelFineTuneDatum = CR_HRPortalAI_DataAcess.CR_HRPortal.ModelFineTuneDatum;
using UserInformation = CR_HRPortalAI_DataAcess.CR_HRPortal.UserInformation;

namespace CR_CoreBot_Service.Adapter
{
    public class ServiceMethods
    {
        public static CustomerInformation _context;
        private readonly ConnectionModel _appSettingconnection;
       
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly CropenAiContext _CROPENAIcontext;
        public ServiceMethods(CustomerInformation context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public ServiceMethods(ConnectionModel appSettingconnection)
        {
            _CROPENAIcontext = new CropenAiContext();
            _appSettingconnection = appSettingconnection;
        }
        public static async Task<DataNewDTO> DataProcessingOpenAI(DataNewDTO objModel, CustomerInformation customerInformation, ModelFineTuneDatum objlebelData, string guid, int modelId)
        {
            try
            {
                string userrole = string.Empty;
                Stopwatch stopwatch = new Stopwatch();
                double time;
                DataNewDTO obj = new DataNewDTO();
                obj.LabelName = new List<string>();
                obj.SourceURL = new List<string>();
                obj.ContentSaftey = objModel.ContentSaftey;
                var prompt = objModel.PropmpInput != null ? objModel.PropmpInput.Trim() : string.Empty;
                var content = string.Empty;
                if (!string.IsNullOrEmpty(prompt))
                {
                    var crcontext = new CropenAiContext();

                    var promptParam = new SqlParameter("@Prompt", objModel.PropmpInput.Trim());
                    var usernameParam = new SqlParameter("@Username", customerInformation.UserName.Trim());
                    var isValidParam = new SqlParameter("@IsValid", "True");

                    // Materialize the results of the stored procedure call
                    var performanceMatrixCheckerList = await crcontext.PerformanceMatrixCheckers
                        .FromSqlRaw("EXEC GetCacheData @Prompt, @Username, @IsValid", promptParam, usernameParam, isValidParam)
                        .ToListAsync();
                    // Apply further LINQ operations on the client side
                    var performanceMatrixCheckerobj = performanceMatrixCheckerList.FirstOrDefault();

                    if (performanceMatrixCheckerobj == null)
                    {
                        if (customerInformation.RoleId == 1 || customerInformation.RoleId == 2)
                        {

                            userrole = "admin";

                        }
                        else
                        {
                            userrole = "user";
                        }
                        HttpResponseMessage response;
                        var client = new HttpClient();
                        client.Timeout = TimeSpan.FromMinutes(10);
                        var question = string.Empty;
                        stopwatch.Start();

                        var endpoint = Convert.ToString(AppSettingHelper.config.GetSection("CaseValues:ServicemethodLink").Value);
                        response = await client.GetAsync(endpoint + "?query=" + HttpUtility.UrlEncode(prompt) + "_trackstream=" + guid + "_mId=" + modelId);

                        stopwatch.Stop();
                        TimeSpan elapsedTime = stopwatch.Elapsed;
                        time = Math.Round(elapsedTime.TotalSeconds, 2);
                        content = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(content))
                        {
                            var objdata = JsonConvert.DeserializeObject<AzureDTO>(content);
                            obj.Cost = objdata.total_cost;
                            obj.PromptTokens = objdata.prompt_tokens;
                            obj.CompletionTokens = objdata.Completion_tokens;
                            obj.TotalTokens = objdata.total_tokens;
                            obj.CompletionResult = objdata.qa_result;
                            obj.StructuredData_Context = objdata.StructuredData_Context;
                            //obj.CompletionResult = "The suppliers with high risk liability and their overall performance score for Q1 2024 are as follows:\n- Supplier 103456 with a score of 100\n- Supplier 100024 with a score of 100\n- Supplier 101718 with a score of 100\n- Supplier 100154 with a score of 100\n- Supplier 100161 with a score of 100\n- Supplier 100193 with a score of 100\n- Supplier 100347 with a score of 100\n- Supplier 101598 with a score of 100\n- Supplier 102242 with a score of 100\n- Supplier 100639 with a score of 100\n- Supplier 100687 with a score of 100\n- Supplier 100744 with a score of 100\n- Supplier 105396 with a score of 100\n- Supplier 100729 with a score of 100\n- Supplier 104983 with a score of 89.51\n- Supplier 105449 with a score of 82.5\n- Supplier 102414 with a score of 82.5\n- Supplier 103326 with a score of 82\n- Supplier 102971 with a score of 80.97\n- Supplier 103604 with a score of 76.67\n- Supplier 100795 with a score of 76.19\n- Supplier 100118 with a score of 76\n- Supplier 100192 with a score of 75.45\n- Supplier 100085 with a score of 75\n- Supplier 102520 with a score of 74.71\n- Supplier 103141 with a score of 74.17\n- Supplier 105176 with a score of 73.94\n- Supplier 101632 with a score of 73.75\n- Supplier 103788 with a score of 72\n- Supplier 102959 with a score of 72\n- Supplier 100385 with a score of 70.28\n- Supplier 105341 with a score of 70.07\n- Supplier 104078 with a score of 70\n- Supplier 100386 with a score of 70\n- Supplier 100077 with a score of 70\n- Supplier 100010 with a score of 70\n- Supplier 103717 with a score of 70\n- Supplier 100030 with a score of 70\n- Supplier 100818 with a score of 70\n- Supplier 104971 with a score of 69.75\n- Supplier 103801 with a score of 67\n- Supplier 103787 with a score of 65\n- Supplier 102893 with a score of 65\n- Supplier 103731 with a score of 65\n- Supplier 103551 with a score of 63.38\n- Supplier 100211 with a score of 60\n- Supplier 100022 with a score of 56.92\n- Supplier 100774 with a score of 56.25\n- Supplier 102503 with a score of 55\n- Supplier 104943 with a score of 54\n- Supplier 100676 with a score of 50\n- Supplier 100769 with a score of 48.33\n- Supplier 100120 with a score of 46.67\n- Supplier 102994 with a score of 46.25\n- Supplier 100746 with a score of 45\n- Supplier 103325 with a score of 44\n- Supplier 104833 with a score of 43.75\n- Supplier 100009 with a score of 35\n- Supplier 103548 with a score of 30.83\n- Supplier 103850 with a score of 30\n- Supplier 100594 with a score of 30\n- Supplier 103255 with a score of 30\n- Supplier 101944 with a score of 30\n- Supplier 102664 with a score of 30\n- Supplier 100021 with a score of 30\n- Supplier 100136 with a score of 30\n- Supplier 103933 with a score of 30\n- Supplier 100121 with a score of 30\n- Supplier 104346 with a score of 29.17\n- Supplier 105138 with a score of 27.88\n- Supplier 101416 with a score of 26.67\n- Supplier 100058 with a score of 23.67\n- Supplier 104994 with a score of 22.5\n- Supplier 100165 with a score of 19.46\n- Supplier 100172 with a score of 19.17\n- Supplier 104681 with a score of 15\n- Supplier 102454 with a score of 14\n- Supplier 101897 with a score of 0\n- Supplier 101662 with a score of 0\n- Supplier 104599 with a score of 0\nThere are also several suppliers for which no score is available for this quarter.";
                            obj.ResponseTime = time;
                            obj.CurrentTime = DateTime.Now;
                            question = obj.CompletionResult.ToString();
                            if (objdata.suggestions != null)
                            {
                                obj.newSuggestions1 = objdata.suggestions[0];
                                obj.newSuggestions2 = objdata.suggestions[1];
                                obj.newSuggestions3 = objdata.suggestions[2];
                            }
                            obj.Cost = objdata.total_cost;
                            obj.PromptTokens = objdata.prompt_tokens;
                            obj.CompletionTokens = objdata.Completion_tokens;
                            obj.TotalTokens = objdata.total_tokens;
                            obj.ResponseTime = time;
                            obj.CurrentTime = DateTime.Now;
                            //foreach (var item in objdata.source_document)
                            //{
                            //    obj.LabelName.Add(item);
                            //}

                            obj.source_document = objdata.source_document;
                            obj.CacheStatus = 0;

                        }
                        else
                        {
                            obj.CompletionResult = "Result is not from the given context";
                            obj.Cost = Convert.ToDecimal(0.0000);
                            obj.PromptTokens = "0";
                            obj.CompletionTokens = "0";
                            obj.TotalTokens = "0";
                            obj.ResponseTime = time;
                            obj.CurrentTime = DateTime.Now;
                            obj.LabelName.Add(objlebelData != null ? objlebelData.Url : string.Empty);
                            obj.SourceURL.Add(objlebelData != null ? objlebelData.LabelName : string.Empty);
                            obj.CacheStatus = 0;
                        }
                    }
                    else
                    {
                        TimeSpan elapsedTime = stopwatch.Elapsed;
                        time = Math.Round(elapsedTime.TotalSeconds, 2);
                        obj.CompletionResult = performanceMatrixCheckerobj.Completion;
                        obj.Cost = Convert.ToDecimal(0.0000);
                        obj.PromptTokens = "0";
                        obj.CompletionTokens = "0";
                        obj.TotalTokens = "0";
                        obj.ResponseTime = time;
                        obj.CurrentTime = DateTime.Now;
                        obj.LabelName.Add(objlebelData != null ? objlebelData.Url : string.Empty);
                        obj.SourceURL.Add(objlebelData != null ? objlebelData.LabelName : string.Empty);
                        obj.CacheStatus = 1;
                        var SuggestedQuestion1 = Convert.ToString(AppSettingHelper.config.GetSection("SuggestedQuestions:Question1").Value);
                        var SuggestedQuestion2 = Convert.ToString(AppSettingHelper.config.GetSection("SuggestedQuestions:Question2").Value);
                        var SuggestedQuestion3 = Convert.ToString(AppSettingHelper.config.GetSection("SuggestedQuestions:Question3").Value);
                        obj.newSuggestions1 = SuggestedQuestion1;
                        obj.newSuggestions2 = SuggestedQuestion2;
                        obj.newSuggestions3 = SuggestedQuestion3;
                    }
                }
                return obj;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public string FeedbackDataSave(ModelFineTuneDatum objmodel, string PromptTokens, string CompletionTokens, string TotalTokens, decimal ResponseTime, decimal Cost, int role, DateTime CurrentDateTime, int PerformanceID)
        {
            try
            {
                objmodel.Datetime = DateTime.Now;
                var fileName = "";
                using (var dbContext = new HrportalAiContext(_appSettingconnection))
                {
                    dbContext.ModelFineTuneData.Add(objmodel);
                    dbContext.SaveChanges();
                    fileName = "data saved successfully";
                }
                using (var crcontext = new CropenAiContext())
                {
                    if (role == 1 || role == 2)
                    {

                        List<PerformanceMatrixChecker> performanceMatrixCheckerobj = crcontext.PerformanceMatrixCheckers.Where(x => x.PeformanceId == PerformanceID && x.Username.Trim() == objmodel.Username.Trim()).ToList();
                        if (performanceMatrixCheckerobj != null)
                        {

                            for (int i = 0; i < performanceMatrixCheckerobj.Count; i++)
                            {
                                performanceMatrixCheckerobj[i].Username = objmodel.Username.Trim();
                                performanceMatrixCheckerobj[i].CurrentDateTime = DateTime.Now;
                                performanceMatrixCheckerobj[i].Prompt = objmodel.Prompt;
                                performanceMatrixCheckerobj[i].Completion = objmodel.Completion;
                                performanceMatrixCheckerobj[i].IsValid = objmodel.IsValid;
                                performanceMatrixCheckerobj[i].PromptTokens = PromptTokens;
                                performanceMatrixCheckerobj[i].CompletionTokens = CompletionTokens;
                                performanceMatrixCheckerobj[i].TotalTokens = TotalTokens;
                                performanceMatrixCheckerobj[i].Cost = Cost;
                                performanceMatrixCheckerobj[i].ResponseTime = ResponseTime;
                                crcontext.Entry(performanceMatrixCheckerobj[i]).State = EntityState.Modified;
                                crcontext.SaveChanges();
                            }
                        }
                    }
                    else if (role == 3)
                    {
                        List<PerformanceMatrixChecker> performanceMatrixCheckerobj2 = crcontext.PerformanceMatrixCheckers.Where(x => x.PeformanceId == PerformanceID && x.Username.Trim() == objmodel.Username.Trim()).ToList();
                        if (performanceMatrixCheckerobj2 != null)
                        {
                            for (int i = 0; i < performanceMatrixCheckerobj2.Count; i++)
                            {
                                performanceMatrixCheckerobj2[i].Username = objmodel.Username.Trim();
                                performanceMatrixCheckerobj2[i].CurrentDateTime = DateTime.Now;
                                performanceMatrixCheckerobj2[i].Prompt = objmodel.Prompt;
                                performanceMatrixCheckerobj2[i].Completion = objmodel.Completion;
                                performanceMatrixCheckerobj2[i].IsValid = objmodel.IsValid;
                                performanceMatrixCheckerobj2[i].PromptTokens = PromptTokens;
                                performanceMatrixCheckerobj2[i].CompletionTokens = CompletionTokens;
                                performanceMatrixCheckerobj2[i].TotalTokens = TotalTokens;
                                performanceMatrixCheckerobj2[i].Cost = Cost;
                                performanceMatrixCheckerobj2[i].ResponseTime = ResponseTime;
                                performanceMatrixCheckerobj2[i].IsValid = objmodel.IsValid;
                                crcontext.Entry(performanceMatrixCheckerobj2[i]).State = EntityState.Modified;
                                crcontext.SaveChanges();
                            }
                        }
                    }
                    else if (role == -11)
                    {
                        List<PerformanceMatrixChecker> performanceMatrixCheckerobj3 = crcontext.PerformanceMatrixCheckers.Where(x => x.PeformanceId == PerformanceID && x.Username.Trim() == objmodel.Username.Trim()).ToList();
                        if (performanceMatrixCheckerobj3 != null)
                        {
                            for (int i = 0; i < performanceMatrixCheckerobj3.Count; i++)
                            {
                                performanceMatrixCheckerobj3[i].Username = objmodel.Username.Trim();
                                performanceMatrixCheckerobj3[i].CurrentDateTime = DateTime.Now;
                                performanceMatrixCheckerobj3[i].Prompt = objmodel.Prompt;
                                performanceMatrixCheckerobj3[i].Completion = objmodel.Completion;
                                performanceMatrixCheckerobj3[i].IsValid = objmodel.IsValid;
                                performanceMatrixCheckerobj3[i].PromptTokens = PromptTokens;
                                performanceMatrixCheckerobj3[i].CompletionTokens = CompletionTokens;
                                performanceMatrixCheckerobj3[i].TotalTokens = TotalTokens;
                                performanceMatrixCheckerobj3[i].Cost = Cost;
                                performanceMatrixCheckerobj3[i].ResponseTime = ResponseTime;
                                crcontext.Entry(performanceMatrixCheckerobj3[i]).State = EntityState.Modified;
                                crcontext.SaveChanges();
                            }
                        }
                    }
                }
                return fileName;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public LoginInfoDTO getUserByUserName(string? userName)
        {
            CustomerInformation? objUser = new CustomerInformation();
            UserInformation? userInformation = new UserInformation();
            LoginInfoDTO? objUserModel = new LoginInfoDTO();
            try
            {
                using (var dbContext = new CropenAiContext())
                {
                    objUser = dbContext.CustomerInformations.Where(x => x.UserName == userName).FirstOrDefault();
                    if (objUser == null)
                    {
                        using (var hrContext = new HrportalAiContext(_appSettingconnection))
                        {
                            userInformation = hrContext.UserInformations.Where(x => x.UserName == userName).FirstOrDefault();
                            if (userInformation == null)
                            {
                                objUserModel.NotFound = true;
                            }
                            objUser = dbContext.CustomerInformations.Where(x => x.CustomerId == userInformation.CustomerId).FirstOrDefault();
                            objUserModel.UserName = userInformation.UserName;
                            objUserModel.RoleId = userInformation.RoleId;
                            objUserModel.CustomerId = Convert.ToInt32(objUser.CustomerId);
                            objUserModel.Password = userInformation.Password;
                            objUserModel.Category = objUser.Category;
                            objUserModel.BlobContainerName = objUser.BlobContainerName;
                            objUserModel.Logo = objUser.OrganizationLogo;
                            objUserModel.FullName = userInformation.FirstName.Trim();
                            objUserModel.LoginWith = userInformation.LoginType;
                            objUserModel.Streaming = userInformation.Streaming;
                            return objUserModel;
                        }
                    }
                    else
                    {
                        objUserModel.UserName = objUser.UserName;
                        objUserModel.RoleId = objUser.RoleId;
                        objUserModel.CustomerId = Convert.ToInt32(objUser.CustomerId);
                        objUserModel.Password = objUser.Password;
                        objUserModel.Category = objUser.Category;
                        objUserModel.BlobContainerName = objUser.BlobContainerName;
                        objUserModel.Logo = objUser.OrganizationLogo;
                        objUserModel.FullName = objUser.Name.Trim();
                        objUserModel.LoginWith = objUser.LoginWith;
                        objUserModel.OpenAIIndexName = objUser.OpenAiindexName;
                        objUserModel.Streaming = objUser.Streaming;
                        return objUserModel;
                    }
                }
            }
            catch (Exception ex)
            {
                objUserModel.NotFound = true;
                return objUserModel;
            }
        }
        public static async Task<DataNewDTO> Firstquestion(DataNewDTO objModel, string BlobContainerName, string role)
        {
            HttpResponseMessage response;
            DataNewDTO obj = new DataNewDTO();
            var content = string.Empty;
            var client = new HttpClient();

            var endpoint = Convert.ToString(AppSettingHelper.config.GetSection("CaseValues:ServicemethodLink").Value);
            response = await client.GetAsync(endpoint + "?query=" + objModel.PropmpInput + "&containername=" + BlobContainerName + "&userrole=" + role);

            content = await response.Content.ReadAsStringAsync();
            if (content != null || content != "")
            {
                var objdata = JsonConvert.DeserializeObject<AzureDTO>(content);
                var presug = objdata.qa_result.ToString();
                string[] questions = presug.Split(new char[] { '\n' }, StringSplitOptions.TrimEntries);
                string pattern = @"^\d+\.\s*";
                obj.presuggestions1 = Regex.Replace(questions[0], pattern, "");
                obj.presuggestions2 = Regex.Replace(questions[1], pattern, "");
                obj.presuggestions3 = Regex.Replace(questions[2], pattern, "");
            }
            return obj;
        }
        public static string GetBlobContainerNameByModel(string ModelType)
        {
            string containerName = "";

            switch (ModelType)
            {
                case "HRModel":
                    containerName = "hrdocumentsdemo";
                    break;
                case "BankingModel":
                    containerName = "bankingdocumentsdemo";
                    break;
                case "modeltype3":
                    containerName = "container_name_3";
                    break;
                default:
                    containerName = "default_container_name";
                    break;
            }

            return containerName;
        }
        public async static Task<ContentSafteyModal> AnalyzeTextWithContentSafety(string text)
        {
            try
            {
                bool BlockListExists = false;
                IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();


                string contentSafetyEndpoint = configuration["Credential:ContentSafetyEndpoint"];
                string contentSafetyKey = configuration["Credential:ContentSafetyApiKey"];

                string blocklistName = configuration["Credential:blocklistName"];
                ContentSafetyClient contentSafetyClient = new ContentSafetyClient(new Uri(contentSafetyEndpoint), new AzureKeyCredential(contentSafetyKey));
                BlocklistClient blocklistClient = new BlocklistClient(new Uri(configuration["Credential:ContentSafetyEndpoint"]), new AzureKeyCredential(configuration["Credential:ContentSafetyApiKey"]));
                var blocklists = blocklistClient.GetTextBlocklistsAsync();
                await foreach (var blocklist in blocklists)
                {
                    if (blocklist.Name == blocklistName)
                    {
                        BlockListExists = true;
                        break;
                    }
                }
                var request = new AnalyzeTextOptions(text);
                //request.BlocklistNames.Add(blocklistName);

                if (BlockListExists)
                {
                    request.BlocklistNames.Add(blocklistName);
                }

                Response<AnalyzeTextResult> response = await contentSafetyClient.AnalyzeTextAsync(request);
                var result = new ContentSafteyModal
                {
                    HateSeverity = response?.Value?.CategoriesAnalysis?.FirstOrDefault(a => a.Category == TextCategory.Hate)?.Severity ?? 0,
                    SelfHarmSeverity = response?.Value?.CategoriesAnalysis?.FirstOrDefault(a => a.Category == TextCategory.SelfHarm)?.Severity ?? 0,
                    SexualSeverity = response?.Value?.CategoriesAnalysis?.FirstOrDefault(a => a.Category == TextCategory.Sexual)?.Severity ?? 0,
                    ViolenceSeverity = response?.Value?.CategoriesAnalysis?.FirstOrDefault(a => a.Category == TextCategory.Violence)?.Severity ?? 0
                };

                bool isContentNotSafe = result.HateSeverity > 0 || result.SelfHarmSeverity > 0 || result.SexualSeverity > 0 || result.ViolenceSeverity > 0 || response?.Value.BlocklistsMatch.Count > 0;

                result.IsContentSafe = !isContentNotSafe;

                return result;
            }
            catch (RequestFailedException ex)
            {
                return new ContentSafteyModal { IsContentSafe = false };
            }
            catch (Exception ex)
            {

                return new ContentSafteyModal { IsContentSafe = false };
            }
        }
        public static async Task<List<Streaming>> GetIntermediateSteps(string guid)
        {
            try
            {

                IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
                string connectionString = configuration.GetConnectionString("AutoSuggestionsConnection");
                string _trackNumber = guid.ToString(); // Your track number

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetIntermediateSteps", con)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Track_Number", _trackNumber);

                    await con.OpenAsync();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Streaming> streamingList = new List<Streaming>();
                    while (await reader.ReadAsync())
                    {
                        Streaming streaming = new Streaming
                        {
                            id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Track_Number = reader.GetString(reader.GetOrdinal("Track_Number")),
                            Prompt = reader.GetString(reader.GetOrdinal("Prompt")),
                            Logs = reader.GetString(reader.GetOrdinal("Logs")),
                            Created_at = reader.GetDateTime(reader.GetOrdinal("Created_at")),
                            Status = reader.IsDBNull(reader.GetOrdinal("Status"))
                                 ? (int?)null
                                 : reader.GetByte(reader.GetOrdinal("Status"))
                        };
                        streamingList.Add(streaming);
                    }
                    await con.CloseAsync();

                    return streamingList;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static DataTable downloadExcel(string query)
        {
            DataTable dataTable = new DataTable();
            try
            {
                IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
                string connectionString = configuration.GetConnectionString("AutoSuggestionsConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return dataTable;
        }

        public static async Task<List<FileNameMappingDTO>> GetFileNames()
        {
            try
            {
                var fileLists = new List<FileNameMappingDTO>();

                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                string connectionString = configuration.GetConnectionString("AutoSuggestionsConnection");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync(); // Await the asynchronous open method
                    using (SqlCommand cmd = new SqlCommand("sp_GetFileNameMappingData", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) // Use ExecuteReaderAsync for async operation
                        {
                            while (await reader.ReadAsync()) // Await the ReadAsync method
                            {
                                var fileDetails = new FileNameMappingDTO
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    Name = reader["Name"]?.ToString(),
                                    isProcessed = Convert.ToInt32(reader["isProcessed"])
                                };

                                fileLists.Add(fileDetails);
                            }
                        }
                    }
                }

                return fileLists;
            }
            catch (Exception ex)
            {
                throw; // Re-throw the exception for now
            }
        }

       
        public static async Task<List<FlightValidationDataDTO>> GetFlightValidationDetails(int fileId)
        {
            try
            {
                var flightValidationLists = new List<FlightValidationDataDTO>();

                // Load configuration
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                string connectionString = configuration.GetConnectionString("AutoSuggestionsConnection");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_GetFlightValidationData", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameter for FileId
                        cmd.Parameters.AddWithValue("@FileId", fileId);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var flightValidationDetails = new FlightValidationDataDTO
                                {
                                    FlightDate = reader["FlightDate"] as DateTime?,
                                    FlightNumber = reader["FlightNumber"]?.ToString(),
                                    Dep = reader["Dep"]?.ToString(),
                                    Arr = reader["Arr"]?.ToString(),
                                    BCMeals = reader["BCMeals"]?.ToString(),
                                    ValidationStatus = reader["ValidationStatus"]?.ToString(),
                                    FileName = reader["FileName"]?.ToString(),
                                    FileId = Convert.ToInt32(fileId),
                                    InternalTotalBCMealsCost = Convert.ToSingle(reader["InternalTotalBCMealsCost"])
                                };

                                flightValidationLists.Add(flightValidationDetails);
                            }
                        }
                    }
                }

                return flightValidationLists;
            }
            catch (Exception ex)
            {
                // Log the exception or rethrow with more details
                throw new Exception("Error fetching flight validation details", ex);
            }
        }




        #region Responsible AI Assessment Created by Cybersecurity Team
        public async Task<ContentSafetyDTO> ContentSafetyApiCall(string Context, string Response, string Prompt, ContentSafetySettings? ContentSafetySettings)
        {


            ContentSafetyDTO responseObject = new ContentSafetyDTO();
            responseObject.ToxicityRes = new assessDTO();
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            try
            {
                if (configuration["Credential:ResponsibleAI"] == "true" && Response != "None --> Invalid source or target language!")
                {
                    try
                    {
                        string ContentSafetyEndpoint = configuration["Credential:ContentSafetyEndpoint"];
                        string ContentSafetyKey = configuration["Credential:ContentSafetyApiKey"];
                        string ContentSafetyRegion = configuration["Credential:ContentSafetyRegion"];
                        List<string> toxicityRegions = configuration.GetSection("Credential:ToxicityAvailablityRegions").Get<List<string>>();
                        List<string> plagraismRegions = configuration.GetSection("Credential:PlagraismAvailablityRegions").Get<List<string>>();
                        List<string> promptInjectionRegions = configuration.GetSection("Credential:PromptInjectionAvailablityRegions").Get<List<string>>();
                        List<string> groundednessRegions = configuration.GetSection("Credential:GroundednessAvailablityRegions").Get<List<string>>();




                        try
                        {
                            //Model Toxicity
                            responseObject.toxicityAvailablity = toxicityRegions.Contains(ContentSafetyRegion);
                            if (String.IsNullOrEmpty(Response) == false)
                            {
                                responseObject.ToxicityRes = new assessDTO();
                                Response = Response.Replace("\"", "\\\"").Replace("\n", string.Empty)
                                         .Replace("\r", string.Empty)
                                         .Replace("\t", string.Empty);
                                Response = Response.Length > 10000 ? (Response.Substring(0, Math.Min(10000, Response.Length))) : Response;
                                var request = new AnalyzeTextOptions(Response);
                                ContentSafetyClient client = new ContentSafetyClient(new Uri(ContentSafetyEndpoint), new AzureKeyCredential(ContentSafetyKey));
                                Response<AnalyzeTextResult> response = await client.AnalyzeTextAsync(request);
                                responseObject.ToxicityRes.HateScore = ContentSafetySettings?.hateRejectionThreshold <= (response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.Hate)?.Severity ?? 0) ? (response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.Hate)?.Severity ?? 0) : 0;
                                responseObject.ToxicityRes.SelfHarmScore = ContentSafetySettings?.selfHarmRejectionThreshold <= (response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.SelfHarm)?.Severity ?? 0) ? (response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.SelfHarm)?.Severity ?? 0) : 0;
                                responseObject.ToxicityRes.ViolenceScore = ContentSafetySettings?.violenceRejectionThreshold <= (response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.Violence)?.Severity ?? 0) ? (response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.Violence)?.Severity ?? 0) : 0;
                                responseObject.ToxicityRes.SexualScore = ContentSafetySettings?.sexualRejectionThreshold <= (response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.Sexual)?.Severity ?? 0) ? (response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.Sexual)?.Severity ?? 0) : 0;
                                if ((responseObject.ToxicityRes.HateScore + responseObject.ToxicityRes.SelfHarmScore + responseObject.ToxicityRes.SexualScore + responseObject.ToxicityRes.ViolenceScore) > 0)
                                {
                                    responseObject.isSafe = false;
                                }
                                else
                                {
                                    responseObject.isSafe = true;
                                }
                            }
                            else
                            {
                                responseObject.isSafe = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            responseObject.exception = ex;
                            responseObject.isSafe = true;
                            responseObject.ToxicityRes = null;
                        }
                        try
                        {
                            //Plagraism detection
                            responseObject.plagraismAvailablity = plagraismRegions.Contains(ContentSafetyRegion);
                            if (Response.Length > 110) //Minimum length for plagraism detection is 110 characters
                            {
                                Response = Response.Replace("\"", "\\\"").Replace("\\\\\"", string.Empty).Replace("\n", string.Empty)
                                         .Replace("\r", string.Empty)
                                         .Replace("\t", string.Empty);
                                string PlaResponse = Response.Length > 1000 ? Response.Substring(0, 1000) : Response;
                                PlaResponse = PlaResponse.Replace("\\", "");
                                var clientPl = new HttpClient();
                                var requestPl = new HttpRequestMessage(HttpMethod.Post, (ContentSafetyEndpoint + "contentsafety/text:detectProtectedMaterial?api-version=2023-10-15-preview"));
                                requestPl.Headers.Add("Ocp-Apim-Subscription-Key", ContentSafetyKey);
                                var BodyPl = $@"{{
                        ""text"": ""{PlaResponse}"" 
                        }}";  // Maximum length for plagraism detection is 1000 characters
                                var contentPl = new StringContent(BodyPl, null, "application/json");
                                requestPl.Content = contentPl;
                                var responsePl = await clientPl.SendAsync(requestPl);
                                responsePl.EnsureSuccessStatusCode();
                                var resPl = await responsePl.Content.ReadAsStringAsync();
                                responseObject.PlagiarismRes = JsonConvert.DeserializeObject<PlagiarismResponseCS>(resPl);
                                if (responseObject.isSafe != false)
                                {
                                    if (responseObject.PlagiarismRes != null)
                                    {
                                        if (responseObject.PlagiarismRes.ProtectedMaterialAnalysis?.Detected == true)
                                        {
                                            responseObject.isSafe = false;
                                        }
                                        else
                                        {
                                            responseObject.isSafe = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (responseObject.isSafe != false)
                                {
                                    responseObject.isSafe = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            responseObject.exception = ex;
                            responseObject.isSafe = true;
                            responseObject.PlagiarismRes = null;
                        }
                        try
                        {
                            //Model Hallucination
                            responseObject.groundednessAvailablity = groundednessRegions.Contains(ContentSafetyRegion);
                            if (String.IsNullOrEmpty(Context) == false && String.IsNullOrEmpty(Prompt) == false && String.IsNullOrEmpty(Response) == false && responseObject.groundednessAvailablity)
                            {
                                Prompt = Prompt.Replace("\"", "\\\"");
                                Prompt = Prompt.Replace("\\'", "'");
                                Context = Context.Replace("\n", string.Empty).Replace("\r", string.Empty)
                                     .Replace("\t", string.Empty).Replace("\\", "").Replace("&nbsp;", " ").Replace("\"", "");
                                Response = Response.Replace("\n", string.Empty).Replace("\r", string.Empty)
                                     .Replace("\t", string.Empty).Replace("\\", "").Replace("&nbsp;", " ").Replace("\"", "");

                                string HallucinationContext = Context.Length > 55000 ? Context.Substring(0, 55000) : Context;
                                HallucinationContext = Regex.Unescape(HallucinationContext);
                                HallucinationContext = Regex.Replace(HallucinationContext, @"[^\u0020-\u007E]+", string.Empty);
                                var clientHal = new HttpClient();
                                var requestHal = new HttpRequestMessage(HttpMethod.Post, (ContentSafetyEndpoint + "contentsafety/text:detectGroundedness?api-version=2024-02-15-preview"));
                                requestHal.Headers.Add("Ocp-Apim-Subscription-Key", ContentSafetyKey);
                                var Body = $@"{{
                        ""domain"": ""Generic"",
                        ""task"": ""QnA"",
                        ""qna"": {{
                            ""query"": ""{Prompt}""
                        }},
                        ""text"": ""{Response}"",
                        ""groundingSources"": [
                            ""{HallucinationContext}""
                        ],
                        ""reasoning"": false
                        }}"; // Maximum grounded source length for Model Hallucination is 55000 characters
                                var content = new StringContent(Body, null, "application/json");
                                requestHal.Content = content;
                                var responseHal = await clientHal.SendAsync(requestHal);
                                responseHal.EnsureSuccessStatusCode();
                                var res = await responseHal.Content.ReadAsStringAsync();
                                responseObject.hallucinationResponse = JsonConvert.DeserializeObject<HallucinationResponseCS>(res);
                                if (responseObject.isSafe != false)
                                {
                                    if (responseObject.hallucinationResponse != null)
                                    {
                                        if (responseObject.hallucinationResponse.ungroundedDetected == true)
                                        {
                                            responseObject.isSafe = false;
                                        }
                                        else
                                        {
                                            responseObject.isSafe = true;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (responseObject.isSafe != false)
                                {
                                    responseObject.isSafe = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            responseObject.exception = ex;
                            responseObject.isSafe = true;
                            responseObject.hallucinationResponse = null;
                        }
                        try
                        {
                            //Prompt Injection
                            responseObject.promptInjectionAvailablity = promptInjectionRegions.Contains(ContentSafetyRegion);
                            if (String.IsNullOrEmpty(Prompt) == false)
                            {
                                //Prompt = System.Text.Json.JsonSerializer.Serialize(Prompt);

                                Prompt = Prompt.Replace("\"", "\\\"");
                                Prompt = Prompt.Replace("\\'", "'");
                                Context = String.IsNullOrEmpty(Context) ? "" : Context.Replace("\n", string.Empty)
                                         .Replace("\r", string.Empty)
                                         .Replace("\t", string.Empty);

                                string PromptInjectionContext = Context.Length > 10000 ? Context.Substring(0, 10000) : Context;
                                PromptInjectionContext = PromptInjectionContext.Replace("\\", "").Replace("&nbsp;", " ");
                                PromptInjectionContext = Regex.Unescape(PromptInjectionContext);
                                PromptInjectionContext = Regex.Replace(PromptInjectionContext, @"[^\u0020-\u007E]+", string.Empty);
                                var clientIn = new HttpClient();
                                var requestIn = new HttpRequestMessage(HttpMethod.Post, (ContentSafetyEndpoint + "contentsafety/text:shieldPrompt?api-version=2024-02-15-preview"));
                                requestIn.Headers.Add("Ocp-Apim-Subscription-Key", ContentSafetyKey);
                                var BodyIn = $@"{{
                        ""userPrompt"": ""{Prompt}"",
                        ""documents"": [
                            ""{PromptInjectionContext}""
                        ]
                        }}"; // Maximum document length for Prompt Injection is 10000 characters

                                var contentIn = new StringContent(BodyIn, null, "application/json");
                                requestIn.Content = contentIn;
                                var responseIn = await clientIn.SendAsync(requestIn);
                                responseIn.EnsureSuccessStatusCode();
                                var resIn = await responseIn.Content.ReadAsStringAsync();
                                responseObject.PromptInjectionRes = JsonConvert.DeserializeObject<PromptInjectionResponseCS>(resIn);
                                if (responseObject.isSafe != false)
                                {
                                    if (responseObject.PromptInjectionRes != null)
                                    {
                                        if (responseObject.PromptInjectionRes.UserPromptAnalysis?.AttackDetected == true)
                                        {
                                            responseObject.isSafe = false;
                                        }
                                        else
                                        {
                                            responseObject.isSafe = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (responseObject.isSafe != false)
                                {
                                    responseObject.isSafe = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            responseObject.exception = ex;
                            responseObject.isSafe = true;
                            responseObject.PromptInjectionRes = new PromptInjectionResponseCS();
                            responseObject.PromptInjectionRes.UserPromptAnalysis = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        responseObject.exception = ex;
                        responseObject.isSafe = true;
                        responseObject.hallucinationResponse = null;
                        responseObject.PlagiarismRes = null;
                        responseObject.PromptInjectionRes = new PromptInjectionResponseCS();
                        responseObject.PromptInjectionRes.UserPromptAnalysis = null;
                        responseObject.ToxicityRes = null;
                    }
                }

                else
                {
                    //responseObject.exception = new HttpRequestException("Response status code does not indicate success: 400 (Bad Request).");

                    responseObject.isSafe = true;
                    responseObject.hallucinationResponse = null;
                    responseObject.PlagiarismRes = null;
                    responseObject.PromptInjectionRes = new PromptInjectionResponseCS();
                    responseObject.PromptInjectionRes.UserPromptAnalysis = null;
                    responseObject.ToxicityRes = null;

                    if (responseObject.PromptInjectionRes != null)
                    {
                        responseObject.PromptInjectionRes = null;
                    }

                    responseObject.ToxicityRes = null;

                }

            }
            catch (Exception ex)
            {
                responseObject.exception = ex;
                responseObject.isSafe = true;
                responseObject.hallucinationResponse = null;
                responseObject.PlagiarismRes = null;
                responseObject.PromptInjectionRes = new PromptInjectionResponseCS();
                responseObject.PromptInjectionRes.UserPromptAnalysis = null;
                responseObject.ToxicityRes = null;
            }
            return responseObject;
        }

        //shashank
        public async Task SetContentSafetySettings(int violenceThreshold, int hateThreshold, int sexualThreshold, int selfHarmThreshold, string EmailId)
        {


            try
            {

                ContentSafetySettings? obj = new ContentSafetySettings();
                obj = _CROPENAIcontext.ContentSafetySettings?.Where(x => x.UserId == EmailId).FirstOrDefault();



                if (obj != null)
                {
                    // Entity exists: Perform an update
                    obj.violenceRejectionThreshold = violenceThreshold;
                    obj.hateRejectionThreshold = hateThreshold;
                    obj.sexualRejectionThreshold = sexualThreshold;
                    obj.selfHarmRejectionThreshold = selfHarmThreshold;
                    obj.UserId = EmailId;
                }
                else
                {
                    // Entity does not exist: Perform an insert
                    obj = new ContentSafetySettings
                    {
                        violenceRejectionThreshold = violenceThreshold,
                        hateRejectionThreshold = hateThreshold,
                        sexualRejectionThreshold = sexualThreshold,
                        selfHarmRejectionThreshold = selfHarmThreshold,
                        UserId = EmailId
                    };
                    // Add the new entity to the context
                    await _CROPENAIcontext.AddAsync(obj);
                }
                await _CROPENAIcontext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ContentSafetySettings?> GetContentSafetySettings(string EmailId)
        {
            ContentSafetySettings? obj = new ContentSafetySettings();
            try
            {

                obj = _CROPENAIcontext.ContentSafetySettings?.Where(x => x.UserId == EmailId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
            return await Task.FromResult(obj);
        }

        #endregion




    }
}
