using Azure;
using Azure.Search.Documents.Indexes.Models;
using CR_CoreBot_DataAccess.CR_OpenAI;
using CR_CoreBot_DTO;
using CR_HRPortalAI_DataAcess.CR_HRPortal;
using CR_HRPortalAI_DataAcess.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using FieldBuilder = Azure.Search.Documents.Indexes.FieldBuilder;
using SearchIndexClient = Azure.Search.Documents.Indexes.SearchIndexClient;
namespace CR_CoreBot_Service.Adapter
{
    public class AdminService
    {
        private static ConnectionModel _appSettingconnection;
        private readonly CropenAiContext connection;
        private readonly HrportalAiContext connection2;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminService(ConnectionModel appSettingconnection, IHttpContextAccessor httpContextAccessor)
        {
            connection = new CropenAiContext();
            _appSettingconnection = appSettingconnection;
            connection2 = new HrportalAiContext(_appSettingconnection);
            _httpContextAccessor = httpContextAccessor;
        }
        public ChatBotKeyConfigurationDTO? readChatBotKeyConfiguration()
        {
            try
            {
                ChatBotKeyConfigurationDTO? chatBotKey = null;
                ChatBotKeyConfiguration? products = connection.ChatBotKeyConfigurations.FirstOrDefault();
                if (products != null)
                {
                    chatBotKey = new ChatBotKeyConfigurationDTO
                    {
                        Id = products.Id,
                        EndPoint = products.EndPoint,
                        ApiKey = products.ApiKey,
                        ServiceName = products.ServiceName,
                        DateTime = products.DateTime
                    };
                }
                return chatBotKey;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<ShowCustomerInfoDTO> readCustomerInfoList()
        {
            try
            {
                var query = from s in connection.CustomerInformations
                            join r in connection.RoleMasters on s.RoleId equals r.RoleId
                            select new ShowCustomerInfoDTO
                            {
                                UserName = s.UserName,
                                RoleType = r.RoleName,
                                CustomerId = s.CustomerId,
                                Password = s.Password,
                                Name = s.Name,
                                OrganizationName = s.OrganizationName,
                                OrganizationLogo = s.OrganizationLogo,
                                DatabaseName = s.DatabaseName,
                                BlobContainerName = s.BlobContainerName,
                                AccountKey = s.AccountKey,
                                AccountName = s.AccountName,
                                Category = s.Category,
                                DateTime = s.DateTime,
                                RoleId = s.RoleId,
                                LoginWith = s.LoginWith
                            };
                List<ShowCustomerInfoDTO> resultList = query.ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CustomerInformationDTO? readCustomerIdInformation(int CustomerId)
        {
            try
            {
                CustomerInformationDTO? customerModel = null;
                CustomerInformation? products = connection.CustomerInformations.Where(x => x.CustomerId == CustomerId).FirstOrDefault();
                if (products != null)
                {
                    customerModel = new CustomerInformationDTO
                    {
                        CustomerId = products.CustomerId,
                        UserName = products.UserName,
                        Password = products.Password,
                        Name = products.Name,
                        OrganizationName = products.OrganizationName,
                        OrganizationLogo = products.OrganizationLogo,
                        DatabaseName = products.DatabaseName,
                        Category = products.Category,
                        DateTime = products.DateTime
                    };
                }
                return customerModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool fnInsertTheKeyConfiguration(ChatBotKeyConfiguration chatBotKeyConfiguration)
        {
            try
            {
                ChatBotKeyConfiguration chatBotKey = new ChatBotKeyConfiguration();
                connection.ChatBotKeyConfigurations.ExecuteDelete();
                connection.SaveChanges();
                chatBotKey.EndPoint = chatBotKeyConfiguration.EndPoint;
                chatBotKey.ApiKey = chatBotKeyConfiguration.ApiKey;
                chatBotKey.ServiceName = chatBotKeyConfiguration.ServiceName;
                chatBotKey.DateTime = DateTime.Now;
                connection.ChatBotKeyConfigurations.Add(chatBotKey);
                connection.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool fnInsertCustomerInformation(CustomerInformation customerInformation)
        {
            try
            {
                var result = connection.CustomerInformations.SingleOrDefault(b => b.CustomerId == customerInformation.CustomerId);
                if (result != null)
                {
                    CustomerInformation customerInform = new CustomerInformation();
                    customerInform.CustomerId = result.CustomerId;
                    customerInform.UserName = customerInformation.UserName;
                    customerInform.Password = customerInformation.Password;
                    customerInform.Name = customerInformation.Name;
                    customerInform.OrganizationName = customerInformation.OrganizationName;
                    customerInform.OrganizationLogo = customerInformation.OrganizationLogo;
                    customerInform.DatabaseName = result.DatabaseName;
                    customerInform.OpenAiindexName = result.OpenAiindexName;
                    customerInform.Category = customerInformation.Category;
                    customerInform.BlobContainerName = customerInformation.BlobContainerName;
                    customerInform.AccountKey = customerInformation.AccountKey;
                    customerInform.AccountName = customerInformation.AccountName;
                    customerInform.RoleId = customerInformation.RoleId;
                    customerInform.DateTime = DateTime.Now;
                    customerInform.LoginWith = customerInformation.LoginWith.Trim();
                    customerInform.Streaming = customerInformation.Streaming;
                    using (var ctx = new CropenAiContext())
                    {
                        ctx.CustomerInformations.Add(customerInform);
                        ctx.Entry(customerInform).State = EntityState.Modified;
                        ctx.SaveChanges();
                    }
                    return true;
                }
                else
                {
                    CustomerInformation customer = new CustomerInformation();
                    customer.UserName = customerInformation.UserName;
                    customer.Password = customerInformation.Password;
                    customer.Name = customerInformation.Name;
                    customer.OrganizationName = customerInformation.OrganizationName;
                    customer.OrganizationLogo = customerInformation.OrganizationLogo;
                    customer.DatabaseName = customerInformation.DatabaseName;
                    customer.OpenAiindexName = customerInformation.OpenAiindexName;
                    customer.Category = customerInformation.Category;
                    customer.RoleId = customerInformation.RoleId;
                    customer.BlobContainerName = customerInformation.BlobContainerName;
                    customer.AccountKey = customerInformation.AccountKey;
                    customer.AccountName = customerInformation.AccountName;
                    customer.LoginWith = customerInformation.LoginWith.Trim();
                    customer.DateTime = DateTime.Now;
                    customer.Streaming = customerInformation.Streaming;
                    connection.CustomerInformations.Add(customer);
                    connection.SaveChanges();
                    CustomerModel customerModel = connection.CustomerModels.FirstOrDefault();
                    int CustomerId = connection.CustomerInformations.Where(x => x.UserName.Trim().ToLower() == customerInformation.UserName.Trim().ToLower()).Select(x => x.CustomerId).FirstOrDefault();
                    if (customerModel != null && CustomerId != null)
                    {
                        CustomerModel customerModelData = new CustomerModel();
                        customerModelData.CustomerId = CustomerId;
                        customerModelData.ModelName = customerModel.ModelName;
                        customerModelData.ModelDisplayName = customerModel.ModelDisplayName;
                        customerModelData.ResourceName = customerModel.ResourceName;
                        customerModelData.AzureModelName = customerModel.AzureModelName;
                        customerModelData.ServiceName = customerModel.ServiceName;
                        customerModelData.ApiKey = customerModel.ApiKey;
                        connection.CustomerModels.Add(customerModelData);
                        connection.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool fnInsertUserInformationMapping(UserInformation userInformation, int? AdLoginCustomerId)
        {
            try
            {
                TblUserInformationMapping userInfo = new TblUserInformationMapping();
                if (AdLoginCustomerId != null)
                {
                    userInfo.CustomerId = AdLoginCustomerId;
                }
                else
                {
                    userInfo.CustomerId = userInformation.CustomerId;
                }

                userInfo.UserName = userInformation.UserName;
                userInfo.Password = userInformation.Password;
                userInfo.LastName = userInformation.LastName;
                userInfo.FirstName = userInformation.FirstName;
                userInfo.RoleId = userInformation.RoleId;
                userInfo.CreatedDate = DateTime.Now;
                userInfo.LoginType = userInformation.LoginType.Trim();
                connection.TblUserInformationMappings.Add(userInfo);
                connection.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool fnInsertUserInformation(UserInformation userInformation, int? AdLoginCustomerId)
        {
            try
            {
                UserInformation userInfo = new UserInformation();
                if (AdLoginCustomerId != null)
                {
                    userInfo.CustomerId = AdLoginCustomerId;
                }
                else
                {
                    userInfo.CustomerId = userInformation.CustomerId;
                }

                userInfo.UserName = userInformation.UserName;
                userInfo.Password = userInformation.Password;
                userInfo.LastName = userInformation.LastName;
                userInfo.FirstName = userInformation.FirstName;
                userInfo.RoleId = userInformation.RoleId;
                userInfo.CreatedDate = DateTime.Now;
                userInfo.LoginType = userInformation.LoginType.Trim();
                userInfo.Streaming = userInformation.Streaming;
                using (var ctx = new HrportalAiContext(_appSettingconnection))
                {
                    ctx.UserInformations.Add(userInfo);
                    ctx.SaveChanges();
                }
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool fnUpdateCustomerConfigurationData(int Id, CustomerConfiguration customerConfiguration)
        {
            try
            {
                CustomerConfiguration userInfo = new CustomerConfiguration();



                userInfo = connection.CustomerConfigurations.Where(x => x.ConfigurationId == customerConfiguration.ConfigurationId).FirstOrDefault();
                if (userInfo != null)
                {
                    userInfo.Username = customerConfiguration.Username;
                    userInfo.FilesAllowed = customerConfiguration.FilesAllowed;
                    userInfo.CustomerId = customerConfiguration.CustomerId;
                    userInfo.UsersAllowed = customerConfiguration.UsersAllowed;
                    userInfo.CreateDateTime = DateTime.Now;
                    userInfo.FileFormat = customerConfiguration.FileFormat;
                    userInfo.LoginType = customerConfiguration.LoginType.Trim();
                    connection.Entry(userInfo).State = EntityState.Modified;
                    connection.SaveChanges();
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

        /// <summary>
        /// This is method is for inserting the Bot Configuration
        /// </summary>
        /// <param name="BotConfiguration"></param>
        /// <returns></returns>
        public bool fnInsertBotCongiguration(int customerId)
        {
            try
            {
                connection.BotConfigurations.Add(new BotConfiguration
                {
                    CustomerId = customerId
                });

                connection.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        public BotConfiguration fbGetBotConfiguration(int customerId)
        {
            var botConfiguration = connection.BotConfigurations.FirstOrDefault(X => X.CustomerId == customerId);
            return botConfiguration;
        }
        /// <summary>
        /// this  method will take the data as a model and will upadte the values of BotConfiguration
        /// </summary>
        /// <param name="Update BotConfigurations"></param>
        /// <returns></returns>
        /// 
        public bool fnUpdateBotConfiguration(int customerId, BotConfiguration botConfigModel)
        {
            try
            {
                var existingBotConfig = connection.BotConfigurations.FirstOrDefault(X => X.CustomerId == customerId);

                if (existingBotConfig != null)
                {
                    existingBotConfig.OpenaiApiVersion = botConfigModel.OpenaiApiVersion;
                    existingBotConfig.Temperature = botConfigModel.Temperature;
                    existingBotConfig.MaxTokens = botConfigModel.MaxTokens;
                    existingBotConfig.AzureEndpoint = botConfigModel.AzureEndpoint;
                    existingBotConfig.OpenaiApiKey = botConfigModel.OpenaiApiKey;
                    existingBotConfig.OpenaiApiType = botConfigModel.OpenaiApiType;
                    existingBotConfig.DeploymentName = botConfigModel.DeploymentName;

                    connection.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public bool fnInsertCustomerConfiguration(CustomerConfiguration customerconfiginfo)
        {
            try
            {
                CustomerConfiguration customconfig = new CustomerConfiguration();
                customconfig.Username = customerconfiginfo.Username;
                customconfig.UsersAllowed = customerconfiginfo.UsersAllowed;
                customconfig.FilesAllowed = customerconfiginfo.FilesAllowed;
                customconfig.CustomerId = customerconfiginfo.CustomerId;
                customconfig.LoginType = customerconfiginfo.LoginType.Trim();
                customconfig.FileFormat = customerconfiginfo.FileFormat;
                customconfig.CreateDateTime = DateTime.Now;
                using (var ctx = new CropenAiContext())
                {
                    ctx.CustomerConfigurations.Add(customconfig);
                    ctx.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool fnDeleteCustomerInfo(int CustomerId)
        {
            bool result = false;
            try
            {
                CustomerInformation custinfo = connection.CustomerInformations.Where(x => x.CustomerId == CustomerId).FirstOrDefault();
                if (custinfo != null)
                {
                    CustomerConfiguration custconfig = connection.CustomerConfigurations.Where(x => x.Username == custinfo.UserName).FirstOrDefault();
                    if (custconfig != null)
                    {
                        connection.CustomerConfigurations.Remove(custconfig);
                        connection.SaveChanges();
                    }
                    connection.CustomerInformations.Remove(custinfo);
                    connection.SaveChanges();
                    result = true;
                    return result;
                }
                else
                {
                    result = false;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = false;
                return result;
            }
        }
        public bool fnDeleteCustomerConfiguration(int ConfigurationId)
        {
            bool result = false;
            try
            {
                CustomerConfiguration custconfig = connection.CustomerConfigurations.Where(x => x.ConfigurationId == ConfigurationId).FirstOrDefault();
                if (custconfig != null)
                {
                    connection.CustomerConfigurations.Remove(custconfig);
                    connection.SaveChanges();
                    result = true;
                    return result;
                }
                else
                {
                    result = false;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = false;
                return result;
            }
        }
        public bool fnDeleteUserInfo(int UserId)
        {
            bool result = false;
            try
            {
                UserInformation userInfo = connection2.UserInformations.Where(x => x.UserId == UserId).FirstOrDefault();
                if (userInfo != null)
                {
                    var recordToDelete = connection.TblUserInformationMappings.FirstOrDefault(x => x.CustomerId == userInfo.CustomerId && x.UserName == userInfo.UserName);

                    if (recordToDelete != null)
                    {
                        connection.TblUserInformationMappings.Remove(recordToDelete);
                        connection.SaveChanges();
                    }

                    connection2.UserInformations.Remove(userInfo);
                    connection2.SaveChanges();
                    result = true;
                    return result;
                }
                else
                {
                    result = false;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = false;
                return result;
            }
        }
        public int? fnAddPerformanceMatrixChecker(PerformanceMatrixChecker performancemodelobj)
        {
            int PerformanceId = 0;
            try
            {
                PerformanceMatrixChecker performancemainobj = new PerformanceMatrixChecker();
                performancemainobj.Username = performancemodelobj.Username;
                performancemainobj.Prompt = performancemodelobj.Prompt;
                performancemainobj.Completion = performancemodelobj.Completion;
                performancemainobj.ResponseTime = performancemodelobj.ResponseTime;
                performancemainobj.TotalTokens = performancemodelobj.TotalTokens;
                performancemainobj.CurrentDateTime = DateTime.Now;
                performancemainobj.PromptTokens = performancemodelobj.PromptTokens;
                performancemainobj.CompletionTokens = performancemodelobj.CompletionTokens;
                performancemainobj.TotalTokens = performancemodelobj.TotalTokens;
                performancemainobj.Cost = performancemodelobj.Cost;
                performancemainobj.PublicInfo = performancemodelobj.PublicInfo;
                performancemainobj.IsValid = null;
                performancemainobj.LoginType = performancemodelobj.LoginType.Trim();
                performancemainobj.Role = performancemodelobj.Role.Trim();
                performancemainobj.RoleId = performancemodelobj.RoleId;
                performancemainobj.Hallucination = performancemodelobj.Hallucination;
                performancemainobj.HallucinationScore = performancemodelobj.HallucinationScore;
                performancemainobj.Plagiarism = performancemodelobj.Plagiarism;
                performancemainobj.PromptInjection = performancemodelobj.PromptInjection;
                performancemainobj.HateSeverity = performancemodelobj.HateSeverity;
                performancemainobj.SelfHarmSeverity = performancemodelobj.SelfHarmSeverity;
                performancemainobj.SexualSeverity = performancemodelobj.SexualSeverity;
                performancemainobj.ViolenceSeverity = performancemodelobj.ViolenceSeverity;
                performancemainobj.Context = performancemodelobj.Context;
                using (var ctx = new CropenAiContext())
                {
                    ctx.PerformanceMatrixCheckers.Add(performancemainobj);
                    ctx.SaveChanges();
                    PerformanceId = performancemainobj.PeformanceId;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return PerformanceId;
        }
        public class Role
        {
            public int RoleId { get; set; }
            public string RoleType { get; set; }
        }
        public async Task<List<Role>> GetRolesAsync()
        {
            var db = new CropenAiContext();
            var roleList = await db.RoleMasters.Select(r => new Role { RoleId = r.RoleId, RoleType = r.RoleName }).ToListAsync();
            return roleList;
        }

        public bool fnUpdateCustomerInformationblob(int Id, string blobname)
        {
            try
            {
                CustomerInformation userInfo = connection.CustomerInformations.Where(x => x.CustomerId == Id).FirstOrDefault();
                if (userInfo != null)
                {
                    userInfo.BlobContainerName = blobname;
                    connection.Entry(userInfo).State = EntityState.Modified;
                    connection.SaveChanges();
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

        public ConnectionModel getConnectionString()
        {
            try
            {
                var normaluserlogin = _httpContextAccessor.HttpContext.Session.GetString("LoginInfo");
                var adlogin = _httpContextAccessor.HttpContext.Session.GetString("AdLoginInfo");

                if (normaluserlogin != null)
                {
                    var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);

                    if (logindata != null)
                    {
                        string UserEmailID = logindata.EmailId.Trim().ToLower();
                        var CustomerInformations = connection.CustomerInformations.Where(x => x.UserName.ToLower() == UserEmailID).FirstOrDefault();
                        if (CustomerInformations != null)
                        {
                            var builder = new SqlConnectionStringBuilder(_appSettingconnection.DefaultConnection);
                            string databaseName = builder.InitialCatalog;
                            _appSettingconnection.DatabaseName = databaseName.ToString();
                            _appSettingconnection.Connection = _appSettingconnection.DefaultConnection.Replace("demo", _appSettingconnection.DatabaseName).ToString();
                        }
                        else
                        {
                            int? CustomerId = connection.TblUserInformationMappings.Where(x => x.UserName.ToLower() == UserEmailID).Select(x => x.CustomerId).FirstOrDefault();
                            if (CustomerId != null)
                            {
                                var builder = new SqlConnectionStringBuilder(_appSettingconnection.DefaultConnection);
                                string databaseName = builder.InitialCatalog;
                                _appSettingconnection.DatabaseName = databaseName.ToString();
                                _appSettingconnection.Connection = _appSettingconnection.DefaultConnection.Replace("demo", _appSettingconnection.DatabaseName).ToString();
                            }
                        }


                    }
                }
                else if (normaluserlogin == null && adlogin != null)
                {
                    var adlogindata = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                    if (adlogindata != null)
                    {
                        string UserEmailID = adlogindata.EmailId.Trim().ToLower();
                        var CustomerInformations = connection.CustomerInformations.Where(x => x.UserName.ToLower() == UserEmailID).FirstOrDefault();
                        if (CustomerInformations != null)
                        {
                            var builder = new SqlConnectionStringBuilder(_appSettingconnection.DefaultConnection);
                            string databaseName = builder.InitialCatalog;
                            _appSettingconnection.DatabaseName = databaseName.ToString();
                            _appSettingconnection.Connection = _appSettingconnection.DefaultConnection.Replace("demo", _appSettingconnection.DatabaseName).ToString();
                        }
                        else
                        {
                            int? CustomerId = connection.TblUserInformationMappings.Where(x => x.UserName.ToLower() == UserEmailID).Select(x => x.CustomerId).FirstOrDefault();
                            if (CustomerId != null)
                            {
                                var builder = new SqlConnectionStringBuilder(_appSettingconnection.DefaultConnection);
                                string databaseName = builder.InitialCatalog;
                                _appSettingconnection.DatabaseName = databaseName.ToString();
                                _appSettingconnection.Connection = _appSettingconnection.DefaultConnection.Replace("demo", _appSettingconnection.DatabaseName).ToString();
                            }
                        }
                    }
                }
                return _appSettingconnection;
            }
            catch (Exception ex)
            {
                return _appSettingconnection;
            }
        }
        public List<string> getConnectionString(string UserName)
        {
            List<string> strings = new List<string>();
            try
            {
                var CustomerInformations = connection.CustomerInformations.Where(x => x.UserName.ToLower() == UserName).FirstOrDefault();
                if (CustomerInformations != null)
                {
                    var builder = new SqlConnectionStringBuilder(_appSettingconnection.DefaultConnection);
                    string databaseName = builder.InitialCatalog;

                    strings.Add(databaseName);
                    strings.Add(_appSettingconnection.DefaultConnection.Replace("demo", databaseName).ToString());

                }
                else
                {
                    int? CustomerId = connection.TblUserInformationMappings.Where(x => x.UserName.ToLower() == UserName.ToLower()).Select(x => x.CustomerId).FirstOrDefault();
                    if (CustomerId != null)
                    {
                        var builder = new SqlConnectionStringBuilder(_appSettingconnection.DefaultConnection);
                        string databaseName = builder.InitialCatalog;
                        strings.Add(databaseName.ToString());
                        strings.Add(_appSettingconnection.DefaultConnection.Replace("demo", databaseName).ToString());
                    }
                }


                return strings;
            }
            catch (Exception ex)
            {
                return strings;
            }
        }
        /// <summary>
        /// This way is used to make an index in the Azure Open AI Service.
        /// </summary>
        /// <param name="_appSettings">appsetting for endpoint and key.</param>
        /// <param name="indexName">name of index which you want to create.</param>
        public void CreateOpenAIIndex(AppSettingsDTO _appSettings, string indexName)
        {
            try
            {
                SearchIndexClient indexClient = new SearchIndexClient(new Uri(_appSettings.SearchServiceEndPoint), new AzureKeyCredential(_appSettings.SearchServiceAdminApiKey));
                FieldBuilder fieldBuilder = new FieldBuilder();
                var searchFields = fieldBuilder.Build(typeof(OpenAIIndexModel));
                var searchIndex = new SearchIndex(indexName.ToLower(), searchFields);
                CleanupSearchIndexClientResources(indexClient, searchIndex);
                indexClient.CreateOrUpdateIndex(searchIndex);
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// If the index is already present, delete it.
        /// </summary>
        /// <param name="indexClient"></param>
        /// <param name="index">index name which we are creating.</param>
        private static void CleanupSearchIndexClientResources(SearchIndexClient indexClient, SearchIndex index)
        {
            try
            {
                if (indexClient.GetIndex(index.Name) != null)
                {
                    indexClient.DeleteIndex(index.Name);
                }
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                //if exception occurred and status is "Not Found", this is working as expected
                Console.WriteLine("If an index of the same name is detected, it's deleted now so that we can reuse the name.");
            }
        }


        #region Dynamic Model,System Instruction, Question-Query , SuggestedQuestions
        // Insert/Update data
        public bool fnInsertModelInformation(ModelDetails modelDetails)
        {
            try
            {
                using (var ctx = new HrportalAiContext(_appSettingconnection))
                {
                    // Check if the model with the given ID already exists
                    var existingModel = ctx.ModelDetails.SingleOrDefault(b => b.Id == modelDetails.Id);

                    if (existingModel != null)
                    {
                        // Update the existing record
                        existingModel.EndPoint = modelDetails.EndPoint;
                        existingModel.ApiKey = modelDetails.ApiKey;
                        existingModel.DeploymentName = modelDetails.DeploymentName;
                        existingModel.ApiVersion = modelDetails.ApiVersion;
                        existingModel.ModelName = modelDetails.ModelName;
                        existingModel.Size = modelDetails.Size;

                        // Mark the entity as modified
                        ctx.Entry(existingModel).State = EntityState.Modified;
                    }
                    else
                    {
                        // Add a new record if it doesn't exist
                        ctx.ModelDetails.Add(modelDetails);
                    }

                    // Save changes to the database
                    ctx.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging mechanism)
                // Example: _logger.LogError(ex, "An error occurred while inserting/updating model information.");

                return false;
            }
        }

        public bool fnInsertSystemInstructionInformation(SystemInstructionsDTO systemInstructionsDTO)
        {
            try
            {
                using (var ctx = new HrportalAiContext(_appSettingconnection))
                {
                    // Check if the model with the given ID already exists
                    var existingModel = ctx.SystemInstructions.SingleOrDefault(b => b.Id == systemInstructionsDTO.Id);

                    if (existingModel != null)
                    {
                        // Check if Rules and Industry match; update only if there's a change
                        if (existingModel.Id == systemInstructionsDTO.Id && existingModel.Rules == systemInstructionsDTO.Rules && existingModel.Industry == systemInstructionsDTO.Industry)
                        {
                            existingModel.Rules = systemInstructionsDTO.Rules;
                            existingModel.Industry = systemInstructionsDTO.Industry;

                            // Mark the entity as modified
                            ctx.Entry(existingModel).State = EntityState.Modified;
                            ctx.SystemInstructions.Add(systemInstructionsDTO);
                        }
                        else
                        {
                            systemInstructionsDTO.Id = 0;
                            ctx.SystemInstructions.Add(systemInstructionsDTO);

                        }
                    }
                    else
                    {
                        // If no existing model, create a new one
                        ctx.SystemInstructions.Add(systemInstructionsDTO);
                    }

                    // Save changes to the database
                    ctx.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool fnInsertSuggestedQuestionInformation(SuggestionsDTO suggestedQuestionDTO)
        {
            try
            {
                using (var ctx = new HrportalAiContext(_appSettingconnection))
                {
                    // Check if the model with the given ID already exists
                    var existingModel = ctx.Suggestions.SingleOrDefault(b => b.Id == suggestedQuestionDTO.Id);

                    if (existingModel != null)
                    {
                        // Update the existing record
                        existingModel.Question = suggestedQuestionDTO.Question;


                        // Mark the entity as modified
                        ctx.Entry(existingModel).State = EntityState.Modified;
                    }
                    else
                    {
                        // Add a new record if it doesn't exist
                        ctx.Suggestions.Add(suggestedQuestionDTO);
                    }

                    // Save changes to the database
                    ctx.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging mechanism)
                // Example: _logger.LogError(ex, "An error occurred while inserting/updating model information.");

                return false;
            }
        }

        public bool fnInsertTrainModelInformation(TrainModelDTO trainModelDTO)
        {
            try
            {
                using (var ctx = new HrportalAiContext(_appSettingconnection))
                {
                    // Check if the model with the given ID already exists
                    var existingModel = ctx.TrainModel.SingleOrDefault(b => b.Id == trainModelDTO.Id);

                    if (existingModel != null)
                    {
                        // Update the existing record
                        existingModel.Question = trainModelDTO.Question;
                        existingModel.SQLQuery = trainModelDTO.SQLQuery;


                        // Mark the entity as modified
                        ctx.Entry(existingModel).State = EntityState.Modified;
                    }
                    else
                    {
                        // Add a new record if it doesn't exist
                        ctx.TrainModel.Add(trainModelDTO);
                    }

                    // Save changes to the database
                    ctx.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging mechanism)
                // Example: _logger.LogError(ex, "An error occurred while inserting/updating model information.");

                return false;
            }
        }


        //Delete data

        public bool fnDeleteModelInfo(int Id)
        {
            try
            {
                var modelinfo = connection2.ModelDetails.FirstOrDefault(x => x.Id == Id);
                if (modelinfo != null)
                {
                    connection2.ModelDetails.Remove(modelinfo);
                    connection2.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception here
                return false;
            }
        }

        public bool fnDeleteSystemInstructionInfo(int Id)
        {
            try
            {
                var systeminfo = connection2.SystemInstructions.FirstOrDefault(x => x.Id == Id);
                if (systeminfo != null)
                {
                    connection2.SystemInstructions.Remove(systeminfo);
                    connection2.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception here
                return false;
            }
        }

        public bool fnDeleteSuggestedQuestionInfo(int Id)
        {
            try
            {
                var suggestedquestioninfo = connection2.Suggestions.FirstOrDefault(x => x.Id == Id);
                if (suggestedquestioninfo != null)
                {
                    connection2.Suggestions.Remove(suggestedquestioninfo);
                    connection2.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception here
                return false;
            }
        }

        public bool fnDeleteTrainModelInfo(int Id)
        {
            try
            {
                var trainmodelinfo = connection2.TrainModel.FirstOrDefault(x => x.Id == Id);
                if (trainmodelinfo != null)
                {
                    connection2.TrainModel.Remove(trainmodelinfo);
                    connection2.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception here
                return false;
            }
        }

        #endregion

    }
}
