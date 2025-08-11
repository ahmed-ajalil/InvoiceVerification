using Azure.Search.Documents.Indexes;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CR_CoreBot.common;
using CR_CoreBot.CustomFilters;
using CR_CoreBot_DataAccess.CR_OpenAI;
using CR_CoreBot_DTO;
using CR_CoreBot_Service.Adapter;
using CR_HRPortalAI_DataAcess.CR_HRPortal;
using CR_HRPortalAI_DataAcess.Models;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2022.M08.Main;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CR_CoreBot.Controllers
{
    public class AdminController : Controller
    {
        CropenAiContext cr = new CropenAiContext();
        private readonly ConnectionModel _appSettingconnection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static AppSettingsDTO _appSettings;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly string _containerName;
        private readonly SearchIndexerClient _indexerClient;
        private readonly string _indexerName;
        private readonly string _managedIdentityClientId;

        public AdminController(IOptions<ConnectionModel> appSettingconnection, IHttpContextAccessor httpContextAccessor, IOptions<AppSettingsDTO> appSettings, IConfiguration configuration)
        {
            _appSettingconnection = appSettingconnection.Value;
            _httpContextAccessor = httpContextAccessor;
            _appSettings = appSettings.Value;

            var blobconnectionString = configuration.GetSection("AzureBlobStorage:ConnectionString").Value;
            _containerName = configuration.GetSection("AzureBlobStorage:ContainerName").Value;
            _managedIdentityClientId = configuration.GetSection("AzureBlobStorage:ManagedIdentity").Value;
           // var searchServiceName = configuration["AzureSearch:SearchServiceName"];
           //var searchAdminApiKey = configuration["AzureSearch:AdminApiKey"];
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
        }

        public IActionResult userAdmin()
        {
            try
            {
                ViewBag.msg = "";
                AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                ChatBotKeyConfigurationDTO products = admin.readChatBotKeyConfiguration();
                return View(products);
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        [HttpPost]
        public IActionResult userAdmin(ChatBotKeyConfiguration chatBotKey)
        {
            try
            {
                AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                if (ModelState.IsValid)
                {
                    if (admin.fnInsertTheKeyConfiguration(chatBotKey))
                    {
                        ViewBag.msg = "succesfully"; ;
                    }
                    else
                    {
                        ViewBag.msg = "Failed"; ;
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult CustomerInfoList()
        {
            try
            {
                int role = 0;
                int CustomerId = 0;
                string emailid = string.Empty;
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                var adlogin = HttpContext.Session.GetString("AdLoginInfo");
                if (serializedObject != null)
                {
                    HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                    LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                    if (loginInfo != null)
                    {
                        role = loginInfo.RoleId;
                        CustomerId = loginInfo.Id;
                        emailid = loginInfo.EmailId.Trim();
                    }

                    ViewBag.EmailId = emailid;
                    ViewBag.Role = role;
                    ViewBag.LoginType = "Login";
                    AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);

                    ViewBag.LoginType = "Login";
                    List<ShowCustomerInfoDTO> products = admin.readCustomerInfoList();
                    return View(products);
                }
                else if (serializedObject == null && adlogin != null)
                {
                    AdLoginInfoDTO loginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                    ViewBag.Role = loginInfo.RoleId;
                    if (ViewBag.Role == 2)
                    {
                        CropenAiContext cr = new CropenAiContext();
                        ViewBag.EmailId = loginInfo.EmailId;
                        ViewBag.LoginType = "AdLogin";
                        AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                        List<CustomerInformation> custinfo = cr.CustomerInformations.Where(x => x.LoginWith == "AdLogin").OrderByDescending(x => x.CustomerId).ThenByDescending(x => x.DateTime).ToList();
                        return View(custinfo);
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
                return View();
            }
        }
        public async Task<IActionResult> CheckContainerNameExist(string BlobContainerName)
        {
            try
            {
                string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=blobstudiobravo;AccountKey=8FEGHkr0Yi3VsSdZVykloAwfGzi4BCYc3DayihAuVEuwM0p3SmmHuhA7PH1tKtiKN+GCxmet/og3+ASt8HcRIA==;EndpointSuffix=core.windows.net";
                string containerName = BlobContainerName.Trim().ToLower();
                CloudStorageAccount storageAccount;
                CloudStorageAccount.TryParse(storageConnectionString, out storageAccount);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                if (await container.ExistsAsync())
                {
                    return Json(new { exists = true });
                }
                else
                {
                    return Json(new { exists = false });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<string> CreateBlobContainerAsync(CustomerInformation CustomerInformation)
        {
            string value = string.Empty;
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=" + CustomerInformation.AccountName + ";AccountKey=" + CustomerInformation.AccountKey + ";EndpointSuffix=core.windows.net";

            //string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=blobstudiobravo;AccountKey=8FEGHkr0Yi3VsSdZVykloAwfGzi4BCYc3DayihAuVEuwM0p3SmmHuhA7PH1tKtiKN+GCxmet/og3+ASt8HcRIA==;EndpointSuffix=core.windows.net";


            string containerName = CustomerInformation.BlobContainerName.Trim().ToLower();

            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                if (await container.CreateIfNotExistsAsync())
                {
                    value = "True";
                }
                else
                {
                    value = "False";

                }
            }
            return value;
        }
        public IActionResult CustomerInfo(int CustomerId)
        {
            try
            {
                int role = 0;
                string EmailId = string.Empty;
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                var adlogin = HttpContext.Session.GetString("AdLoginInfo");
                if (serializedObject != null)
                {
                    HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                    LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                    role = loginInfo.RoleId;
                    EmailId = loginInfo.EmailId;
                    AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                    CropenAiContext cropenAiContext = new CropenAiContext();
                    var list = cropenAiContext.RoleMasters.ToList();
                    ViewBag.Roles = list;
                    ViewBag.Role = role;
                    ViewBag.EmailId = loginInfo.EmailId;
                    ViewBag.LoginType = "Login";
                    if (ViewBag.Role == 2)
                    {
                        if (CustomerId != 0 && CustomerId != null)
                        {
                            CustomerInformationDTO products = admin.readCustomerIdInformation(CustomerId);
                            return View(products);
                        }

                        else
                        {
                            return View();
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
                else if (serializedObject == null && adlogin != null)
                {
                    AdLoginInfoDTO loginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                    ViewBag.Role = loginInfo.RoleId;
                    ViewBag.EmailId = loginInfo.EmailId;
                    ViewBag.LoginType = "AdLogin";
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        [HttpPost]
        public async Task<IActionResult> SaveCustomerInfo()
        {
            var file = Request.Form.Files[0];
            try
            {
                CustomerInformation customerInformation = new CustomerInformation();
                customerInformation.UserName = Request.Form["username"];
                customerInformation.Password = Request.Form["password"];
                customerInformation.Name = Request.Form["customername"];
                customerInformation.OrganizationName = Request.Form["organizationname"];
                customerInformation.Category = Request.Form["category"];
                customerInformation.BlobContainerName = Request.Form["BlobContainerName"];
                customerInformation.AccountKey = Request.Form["AccountKey"];
                customerInformation.AccountName = Request.Form["AccountName"];
                customerInformation.RoleId = Convert.ToInt32(Request.Form["RoleId"]);
                //customerInformation.Streaming = Convert.ToBoolean(Request.Form["streaming"]);
                customerInformation.Streaming = Convert.ToInt32(Request.Form["streaming"]);
                CropenAiContext db = new CropenAiContext();
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                var adlogin = HttpContext.Session.GetString("AdLoginInfo");
                if (serializedObject != null)
                {
                    LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                    var CustomerId = loginInfo.Id;
                    string path = "";
                    string iscopied = string.Empty;
                    try
                    {
                        if (file.Length > 0)
                        {
                            string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);

                            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                            string logoPath = Path.Combine(wwwrootPath, "Logo");
                            Directory.CreateDirectory(logoPath);
                            path = Path.Combine(logoPath);

                            using (var filestream = new FileStream(Path.Combine(path, filename), System.IO.FileMode.Create))
                            {
                                await file.CopyToAsync(filestream);
                            }

                            string Connection = "DefaultEndpointsProtocol=https;AccountName=" + customerInformation.AccountName + ";AccountKey=" + customerInformation.AccountKey + ";EndpointSuffix=core.windows.net";
                            if (customerInformation.BlobContainerName != null)
                            {
                                customerInformation.BlobContainerName = customerInformation.BlobContainerName.Trim().ToLower();
                            }
                            else
                            {
                                customerInformation.BlobContainerName = customerInformation.BlobContainerName;
                            }
                            customerInformation.OrganizationLogo = filename;
                            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                            var customerexists = cr.CustomerInformations.Where(x => x.UserName.Trim() == customerInformation.UserName.Trim()).FirstOrDefault();
                            if (customerexists == null)
                            {
                                IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();

                                string connectionString = configuration.GetConnectionString("AutoSuggestionsConnection");
                                var builder = new SqlConnectionStringBuilder(connectionString);
                                string databaseName = builder.InitialCatalog;

                                //string _databaseName = customerInformation.OrganizationName + DateTime.Now.ToString("ddMMyyyyHHmmss");
                                //_databaseName = _databaseName.Replace(" ", "");
                                customerInformation.DatabaseName = "";
                                customerInformation.OpenAiindexName = "";
                                customerInformation.LoginWith = "Login";
                                admin.fnInsertCustomerInformation(customerInformation);
                                CreateTable(databaseName);
                                CustomerInformation custinf = new CustomerInformation();
                                custinf.CustomerId = cr.CustomerInformations.Where(x => x.UserName == customerInformation.UserName).Select(x => x.CustomerId).FirstOrDefault();
                                CustomerConfiguration addcustomerconfig = new CustomerConfiguration();
                                addcustomerconfig.Username = customerInformation.UserName;
                                addcustomerconfig.CustomerId = custinf.CustomerId;
                                addcustomerconfig.FilesAllowed = 10;
                                addcustomerconfig.UsersAllowed = 10;
                                addcustomerconfig.FileFormat = "Pdf";
                                addcustomerconfig.LoginType = "Login";
                                admin.fnInsertCustomerConfiguration(addcustomerconfig);
                                bool status = admin.fnInsertBotCongiguration(custinf.CustomerId);
                                iscopied = "true";


                            }
                            else if (customerexists != null)
                            {
                                iscopied = "UserExists";
                            }

                        }
                        else
                        {
                            iscopied = "file not found";
                        }
                    }
                    catch (Exception ex)
                    {
                        iscopied = ex.Message;
                    }
                    DataNewDTO objdModel = new DataNewDTO();
                    objdModel.CompletionResult = iscopied;
                    return Json(objdModel);
                }
                else if (serializedObject == null && adlogin != null)
                {
                    AdLoginInfoDTO adloginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                    var CustomerId = adloginInfo.UserId;
                    string path = "";
                    string iscopied = string.Empty;
                    try
                    {
                        if (file.Length > 0)
                        {
                            string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);

                            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                            string logoPath = Path.Combine(wwwrootPath, "Logo");
                            Directory.CreateDirectory(logoPath);
                            path = Path.Combine(logoPath);
                            using (var filestream = new FileStream(Path.Combine(path, filename), System.IO.FileMode.Create))
                            {
                                await file.CopyToAsync(filestream);
                            }
                            string Connection = "DefaultEndpointsProtocol=https;AccountName=" + customerInformation.AccountName + ";AccountKey=" + customerInformation.AccountKey + ";EndpointSuffix=core.windows.net";
                            if (customerInformation.BlobContainerName != null)
                            {
                                customerInformation.BlobContainerName = customerInformation.BlobContainerName.Trim().ToLower();
                            }
                            else
                            {
                                customerInformation.BlobContainerName = customerInformation.BlobContainerName;
                            }

                            customerInformation.OrganizationLogo = filename;

                            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                            var customerexists = cr.CustomerInformations.Where(x => x.UserName.Trim() == customerInformation.UserName.Trim()).FirstOrDefault();
                            if (customerexists == null)
                            {
                                customerInformation.LoginWith = "AdLogin";
                                admin.fnInsertCustomerInformation(customerInformation);
                                CustomerInformation custinf = new CustomerInformation();
                                custinf.CustomerId = cr.CustomerInformations.Where(x => x.UserName == customerInformation.UserName).Select(x => x.CustomerId).FirstOrDefault();
                                CustomerConfiguration addcustomerconfig = new CustomerConfiguration();
                                addcustomerconfig.Username = customerInformation.UserName;
                                addcustomerconfig.CustomerId = custinf.CustomerId;
                                addcustomerconfig.FilesAllowed = 10;
                                addcustomerconfig.UsersAllowed = 10;
                                addcustomerconfig.FileFormat = "Pdf";
                                addcustomerconfig.LoginType = "AdLogin";
                                admin.fnInsertCustomerConfiguration(addcustomerconfig);
                                iscopied = "true";
                            }
                            else if (customerexists != null)
                            {
                                iscopied = "UserExists";
                            }

                        }
                        else
                        {
                            iscopied = "file not found";
                        }
                    }
                    catch (Exception ex)
                    {
                        iscopied = ex.Message;
                    }
                    DataNewDTO objdModel = new DataNewDTO();
                    objdModel.CompletionResult = iscopied;
                    return Json(objdModel);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult UserInfo(int? Id)
        {
            int role = 0;
            string EmailId = string.Empty;
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            var adlogin = HttpContext.Session.GetString("AdLoginInfo");
            if (serializedObject != null)
            {
                LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                if (loginInfo != null)
                {
                    role = loginInfo.RoleId;
                    EmailId = loginInfo.EmailId;

                }
                ViewBag.Role = role;
                ViewBag.EmailId = EmailId;
                ViewBag.LoginType = "Login";
                return View();
            }
            else if (serializedObject == null && adlogin != null)
            {
                AdLoginInfoDTO adlogininfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                ViewBag.Role = adlogininfo.RoleId;
                ViewBag.EmailId = adlogininfo.EmailId;
                ViewBag.LoginType = "AdLogin";
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public IActionResult SaveUserInfo(UserInformation objModel)
        {
            CropenAiContext db = new CropenAiContext();
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            var adlogin = HttpContext.Session.GetString("AdLoginInfo");

            if (serializedObject != null)
            {
                string iscopied = string.Empty;
                LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                var CustomerId = loginInfo.Id;
                TblUserInformationMapping tblUserInformationMapping = new TblUserInformationMapping();
                tblUserInformationMapping = cr.TblUserInformationMappings.Where(x => x.UserName.ToLower().Trim() == objModel.UserName.ToLower().Trim()).FirstOrDefault();
                if (tblUserInformationMapping == null)
                {
                    UserInformation userInformation = new UserInformation();
                    userInformation.FirstName = objModel.FirstName;
                    userInformation.LastName = objModel.LastName;
                    userInformation.UserName = objModel.UserName;
                    userInformation.Password = objModel.Password;
                    userInformation.RoleId = objModel.RoleId;
                    userInformation.CustomerId = CustomerId;
                    userInformation.LoginType = "Login";
                    userInformation.Streaming = objModel.Streaming;
                    try
                    {
                        if (loginInfo != null)
                        {
                            HrportalAiContext dbctx = new HrportalAiContext(_appSettingconnection);
                            ViewBag.Role = loginInfo.RoleId;
                            if (ViewBag.Role == 1)
                            {
                                var Adminexists = dbctx.UserInformations.Where(x => x.CustomerId == CustomerId).Count();
                                CustomerConfiguration customerconfig = new CustomerConfiguration();
                                customerconfig = cr.CustomerConfigurations.Where(x => x.Username.Trim() == loginInfo.EmailId.Trim()).FirstOrDefault();
                                var UsersAllowed = 0;
                                if (customerconfig == null)
                                {
                                    UsersAllowed = 0;
                                }
                                else
                                {
                                    UsersAllowed = customerconfig.UsersAllowed;
                                }

                                if (Adminexists >= UsersAllowed)
                                {
                                    iscopied = "limit";
                                }
                                else
                                {
                                    HrportalAiContext hr = new HrportalAiContext(_appSettingconnection);
                                    var userexists = hr.UserInformations.Where(x => x.UserName.Trim() == objModel.UserName.Trim()).FirstOrDefault();
                                    if (userexists == null)
                                    {
                                        var customerexists = cr.CustomerInformations.Where(x => x.UserName.Trim() == objModel.UserName.Trim()).FirstOrDefault();
                                        if (customerexists == null)
                                        {
                                            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                                            admin.fnInsertUserInformation(userInformation, null);
                                            admin.fnInsertUserInformationMapping(userInformation, null);
                                            iscopied = "true";
                                        }
                                        else
                                        {
                                            iscopied = "UserExists";
                                        }
                                    }
                                    else if (userexists != null)
                                    {
                                        iscopied = "UserExists";
                                    }


                                }
                            }
                            else if (ViewBag.Role == 2)
                            {
                                HrportalAiContext hr = new HrportalAiContext(_appSettingconnection);
                                var userexists = hr.UserInformations.Where(x => x.UserName.Trim() == objModel.UserName.Trim()).FirstOrDefault();
                                if (userexists == null)
                                {
                                    var customerexists = cr.CustomerInformations.Where(x => x.UserName.Trim() == objModel.UserName.Trim()).FirstOrDefault();
                                    if (customerexists == null)
                                    {
                                        AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                                        admin.fnInsertUserInformation(userInformation, null);
                                        iscopied = "true";
                                    }
                                    else
                                    {
                                        iscopied = "UserExists";
                                    }
                                }
                                else if (userexists != null)
                                {
                                    iscopied = "UserExists";
                                }

                            }
                        }
                    }
                    catch (Exception)
                    {
                        iscopied = "false";
                    }

                }
                else
                {
                    iscopied = "UserExists";
                }
                DataNewDTO objdModel = new DataNewDTO();
                objdModel.CompletionResult = iscopied;
                return Json(objdModel);
            }
            else if (serializedObject == null && adlogin != null)
            {
                AdLoginInfoDTO adloginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                UserInformation userInformation = new UserInformation();
                userInformation.FirstName = objModel.FirstName;
                userInformation.LastName = objModel.LastName;
                userInformation.UserName = objModel.UserName;
                userInformation.Password = objModel.Password;
                userInformation.RoleId = Convert.ToInt32(adloginInfo.RoleId);
                userInformation.LoginType = "AdLogin";
                userInformation.Streaming = objModel.Streaming;
                string path = "";
                string iscopied = string.Empty;
                try
                {
                    if (adloginInfo != null)
                    {
                        HrportalAiContext dbctx = new HrportalAiContext(_appSettingconnection);
                        CropenAiContext crctx = new CropenAiContext();
                        ViewBag.Role = adloginInfo.RoleId;
                        // For Admin
                        if (ViewBag.Role == 1)
                        {
                            var CustomerId = crctx.CustomerInformations.Where(x => x.UserName.Trim() == adloginInfo.EmailId.Trim()).FirstOrDefault();
                            var Adminexists = dbctx.UserInformations.Where(x => x.CustomerId == CustomerId.CustomerId).Count();
                            CustomerConfiguration customerconfig = new CustomerConfiguration();
                            customerconfig = cr.CustomerConfigurations.Where(x => x.Username.Trim() == adloginInfo.EmailId.Trim()).FirstOrDefault();
                            var UsersAllowed = 0;
                            if (customerconfig == null)
                            {
                                UsersAllowed = 0;
                            }
                            else
                            {
                                UsersAllowed = customerconfig.UsersAllowed;
                            }

                            if (Adminexists >= UsersAllowed)
                            {
                                iscopied = "limit";
                            }
                            else
                            {
                                HrportalAiContext hr = new HrportalAiContext(_appSettingconnection);
                                var userexists = hr.UserInformations.Where(x => x.UserName.Trim() == objModel.UserName.Trim()).FirstOrDefault();
                                if (userexists == null)
                                {
                                    var customerexists = cr.CustomerInformations.Where(x => x.UserName.Trim() == objModel.UserName.Trim()).FirstOrDefault();
                                    if (customerexists == null)
                                    {
                                        AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                                        CropenAiContext cr = new CropenAiContext();
                                        var AdCustomerId = cr.CustomerInformations.Where(x => x.UserName.Trim() == adloginInfo.EmailId.Trim()).FirstOrDefault();
                                        admin.fnInsertUserInformation(userInformation, Convert.ToInt32(AdCustomerId.CustomerId));
                                        iscopied = "true";
                                    }
                                    else
                                    {
                                        iscopied = "UserExists";
                                    }
                                }
                                else if (userexists != null)
                                {
                                    iscopied = "UserExists";
                                }


                            }
                        }
                        // For SuperAdmin
                        else if (ViewBag.Role == 2)
                        {
                            HrportalAiContext hr = new HrportalAiContext(_appSettingconnection);

                            var userexists = hr.UserInformations.Where(x => x.UserName.Trim() == objModel.UserName.Trim()).FirstOrDefault();
                            if (userexists == null)
                            {
                                var customerexists = cr.CustomerInformations.Where(x => x.UserName.Trim() == objModel.UserName.Trim()).FirstOrDefault();
                                if (customerexists == null)
                                {
                                    AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                                    CropenAiContext cr = new CropenAiContext();
                                    var CustomerId = cr.CustomerInformations.Where(x => x.UserName.Trim() == adloginInfo.EmailId.Trim()).FirstOrDefault();
                                    admin.fnInsertUserInformation(userInformation, Convert.ToInt32(CustomerId.CustomerId));
                                    iscopied = "true";
                                }
                                else
                                {
                                    iscopied = "UserExists";
                                }
                            }
                            else if (userexists != null)
                            {
                                iscopied = "UserExists";
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    iscopied = "false";
                }
                DataNewDTO objdModel = new DataNewDTO();
                objdModel.CompletionResult = iscopied;
                return Json(objdModel);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCustomerInfo()
        {
            var fileget = Request.Form;
            string existingimgsrc = string.Empty;
            string nameoffile = "";
            if (fileget.Files.Count == 0)
            {
                existingimgsrc = Request.Form["img_file"].ToString();
                nameoffile = Path.GetFileName(existingimgsrc);
            }

            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            var adlogin = HttpContext.Session.GetString("AdLoginInfo");

            string iscopied = string.Empty;
            try
            {
                if (serializedObject != null)
                {
                    if (fileget.Files.Count == 0)
                    {
                        CustomerInformation customerInformation = new CustomerInformation();
                        customerInformation.UserName = Request.Form["username"];
                        customerInformation.Password = Request.Form["password"];
                        customerInformation.Name = Request.Form["customername"];
                        customerInformation.OrganizationName = Request.Form["organizationname"];
                        customerInformation.Category = Request.Form["category"];
                        customerInformation.BlobContainerName = Request.Form["BlobContainerName"];
                        customerInformation.AccountKey = Request.Form["AccountKey"];
                        customerInformation.AccountName = Request.Form["AccountName"];
                        customerInformation.CustomerId = Convert.ToInt32(Request.Form["CustomerId"]);
                        customerInformation.RoleId = Convert.ToInt32(Request.Form["RoleId"]);
                        customerInformation.Streaming = Convert.ToInt32(Request.Form["streaming"]);
                        customerInformation.OrganizationLogo = nameoffile;
                        CropenAiContext db = new CropenAiContext();
                        LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                        var CustomerId = loginInfo.Id;
                        string path = "";
                        AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                        customerInformation.LoginWith = "Login";
                        admin.fnInsertCustomerInformation(customerInformation);
                        iscopied = "true";
                    }
                    else if (fileget != null && fileget.Files.Count > 0)
                    {
                        var file = Request.Form.Files[0];
                        CustomerInformation customerInformation = new CustomerInformation();
                        customerInformation.UserName = Request.Form["username"];
                        customerInformation.Password = Request.Form["password"];
                        customerInformation.Name = Request.Form["customername"];
                        customerInformation.OrganizationName = Request.Form["organizationname"];
                        customerInformation.DatabaseName = Request.Form["databasename"];
                        customerInformation.Category = Request.Form["category"];
                        customerInformation.BlobContainerName = Request.Form["BlobContainerName"];
                        customerInformation.AccountKey = Request.Form["AccountKey"];
                        customerInformation.AccountName = Request.Form["AccountName"];
                        customerInformation.CustomerId = Convert.ToInt32(Request.Form["CustomerId"]);
                        customerInformation.RoleId = Convert.ToInt32(Request.Form["RoleId"]);
                        customerInformation.Streaming = Convert.ToInt32(Request.Form["streaming"]);
                        CropenAiContext db = new CropenAiContext();
                        LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                        var CustomerId = loginInfo.Id;

                        string path = "";
                        string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        string logoPath = Path.Combine(wwwrootPath, "Logo");
                        Directory.CreateDirectory(logoPath);
                        path = Path.Combine(logoPath);

                        using (var filestream = new FileStream(Path.Combine(path, filename), System.IO.FileMode.Create))
                        {
                            await file.CopyToAsync(filestream);
                        }

                        customerInformation.OrganizationLogo = filename;
                        AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                        customerInformation.LoginWith = "Login";
                        admin.fnInsertCustomerInformation(customerInformation);
                        iscopied = "true";
                    }
                }
                else if (serializedObject == null && adlogin != null)
                {
                    if (fileget.Files.Count == 0)
                    {
                        CustomerInformation customerInformation = new CustomerInformation();
                        customerInformation.UserName = Request.Form["username"];
                        customerInformation.Password = Request.Form["password"];
                        customerInformation.Name = Request.Form["customername"];
                        customerInformation.OrganizationName = Request.Form["organizationname"];
                        customerInformation.DatabaseName = Request.Form["databasename"];
                        customerInformation.Category = Request.Form["category"];
                        customerInformation.BlobContainerName = Request.Form["BlobContainerName"];
                        customerInformation.AccountKey = Request.Form["AccountKey"];
                        customerInformation.AccountName = Request.Form["AccountName"];
                        customerInformation.CustomerId = Convert.ToInt32(Request.Form["CustomerId"]);
                        customerInformation.Streaming = Convert.ToInt32(Request.Form["streaming"]);
                        customerInformation.RoleId = 1;
                        customerInformation.OrganizationLogo = nameoffile;
                        CropenAiContext db = new CropenAiContext();
                        AdLoginInfoDTO loginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                        var CustomerId = loginInfo.AdLoggedInUserId;
                        string path = "";
                        AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                        customerInformation.LoginWith = "AdLogin";
                        admin.fnInsertCustomerInformation(customerInformation);
                        iscopied = "true";
                    }
                    else if (fileget != null && fileget.Files.Count > 0)
                    {
                        var file = Request.Form.Files[0];
                        CustomerInformation customerInformation = new CustomerInformation();
                        customerInformation.UserName = Request.Form["username"];
                        customerInformation.Password = Request.Form["password"];
                        customerInformation.Name = Request.Form["customername"];
                        customerInformation.OrganizationName = Request.Form["organizationname"];
                        customerInformation.DatabaseName = Request.Form["databasename"];
                        customerInformation.Category = Request.Form["category"];
                        customerInformation.BlobContainerName = Request.Form["BlobContainerName"];
                        customerInformation.AccountKey = Request.Form["AccountKey"];
                        customerInformation.AccountName = Request.Form["AccountName"];
                        customerInformation.CustomerId = Convert.ToInt32(Request.Form["CustomerId"]);
                        customerInformation.Streaming = Convert.ToInt32(Request.Form["streaming"]);
                        customerInformation.RoleId = 1;
                        CropenAiContext db = new CropenAiContext();
                        AdLoginInfoDTO loginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                        var CustomerId = loginInfo.AdLoggedInUserId;

                        string path = "";
                        string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        string logoPath = Path.Combine(wwwrootPath, "Logo");
                        Directory.CreateDirectory(logoPath);
                        path = Path.Combine(logoPath);

                        using (var filestream = new FileStream(Path.Combine(path, filename), System.IO.FileMode.Create))
                        {
                            await file.CopyToAsync(filestream);
                        }

                        customerInformation.OrganizationLogo = filename;

                        AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                        customerInformation.LoginWith = "AdLogin";
                        admin.fnInsertCustomerInformation(customerInformation);
                        iscopied = "true";
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

            }
            catch (Exception ex)
            {
                iscopied = ex.Message;
            }
            DataNewDTO objdModel = new DataNewDTO();
            objdModel.CompletionResult = iscopied;
            return Json(objdModel);
        }

        [HttpGet]
        [HolUser]
        [UserPrevent]
        public IActionResult UserInfoList()
        {
            try
            {
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                var adlogin = HttpContext.Session.GetString("AdLoginInfo");

                if (serializedObject != null)
                {
                    HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                    LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                    ViewBag.Role = loginInfo.RoleId;
                    ViewBag.EmailId = loginInfo.EmailId;
                    ViewBag.LoginType = "Login";
                    UserInfoDTO? objModel = new UserInfoDTO();

                    var data = db.UserInformations.OrderByDescending(x => x.UserId).ThenByDescending(x => x.CreatedDate).ToList();

                    CropenAiContext dbctx = new CropenAiContext();

                    var lst = new List<UserInfoDTO>();
                    for (int i = 0; i < data.Count; i++)
                    {
                        objModel = new UserInfoDTO();
                        if (ViewBag.Role == 1 && loginInfo.Id == data[i].CustomerId)
                        {
                            objModel.FirstName = data[i].FirstName;
                            objModel.LastName = data[i].LastName;
                            objModel.RoleId = data[i].RoleId;
                            objModel.CustomerId = data[i].CustomerId;
                            objModel.CustomerName = dbctx.CustomerInformations.Where(x => x.CustomerId == data[i].CustomerId).Select(x => x.Name).FirstOrDefault();
                            objModel.UserId = data[i].UserId;
                            objModel.UserName = db.UserInformations.Where(x => x.UserId == data[i].UserId).Select(x => x.UserName).FirstOrDefault();
                            objModel.Password = data[i].Password;
                            objModel.CreatedDate = data[i].CreatedDate;
                            lst.Add(objModel);
                        }
                        else if (ViewBag.Role == 2)
                        {
                            objModel.FirstName = data[i].FirstName;
                            objModel.LastName = data[i].LastName;
                            objModel.RoleId = data[i].RoleId;
                            objModel.CustomerId = data[i].CustomerId;
                            objModel.CustomerName = dbctx.CustomerInformations.Where(x => x.CustomerId == data[i].CustomerId).Select(x => x.Name).FirstOrDefault();
                            objModel.UserId = data[i].UserId;
                            objModel.UserName = db.UserInformations.Where(x => x.UserId == data[i].UserId).Select(x => x.UserName).FirstOrDefault();
                            objModel.Password = data[i].Password;
                            objModel.CreatedDate = data[i].CreatedDate;
                            objModel.Password = data[i].Password;
                            objModel.CreatedDate = data[i].CreatedDate;
                            lst.Add(objModel);
                        }
                    }
                    return View(lst);
                }
                else if (serializedObject == null && adlogin != null)
                {
                    AdLoginInfoDTO loginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                    ViewBag.Role = loginInfo.RoleId;

                    ViewBag.EmailId = loginInfo.EmailId;
                    ViewBag.LoginType = "AdLogin";
                    UserInfoDTO? objModel = new UserInfoDTO();
                    HrportalAiContext dbctx = new HrportalAiContext(_appSettingconnection);
                    var data = dbctx.UserInformations.Where(x => x.LoginType == "AdLogin").OrderByDescending(x => x.UserId).ThenByDescending(x => x.CreatedDate).ToList();

                    CropenAiContext db = new CropenAiContext();

                    var lst = new List<UserInfoDTO>();
                    for (int i = 0; i < data.Count; i++)
                    {
                        objModel = new UserInfoDTO();
                        if (ViewBag.Role == 1 && loginInfo.UserId == data[i].CustomerId)
                        {
                            objModel.FirstName = data[i].FirstName;
                            objModel.LastName = data[i].LastName;
                            objModel.RoleId = data[i].RoleId;
                            objModel.CustomerId = data[i].CustomerId;
                            objModel.CustomerName = db.CustomerInformations.Where(x => x.CustomerId == data[i].CustomerId).Select(x => x.Name).FirstOrDefault();
                            objModel.UserId = data[i].UserId;
                            objModel.UserName = dbctx.UserInformations.Where(x => x.UserId == data[i].UserId).Select(x => x.UserName).FirstOrDefault();
                            objModel.Password = data[i].Password;
                            objModel.CreatedDate = data[i].CreatedDate;
                            lst.Add(objModel);
                        }
                        else if (ViewBag.Role == 2)
                        {
                            objModel.FirstName = data[i].FirstName;
                            objModel.LastName = data[i].LastName;
                            objModel.RoleId = data[i].RoleId;
                            objModel.CustomerId = data[i].CustomerId;
                            objModel.CustomerName = db.CustomerInformations.Where(x => x.CustomerId == data[i].CustomerId).Select(x => x.Name).FirstOrDefault();
                            objModel.UserId = data[i].UserId;
                            objModel.UserName = dbctx.UserInformations.Where(x => x.UserId == data[i].UserId).Select(x => x.UserName).FirstOrDefault();
                            objModel.Password = data[i].Password;
                            objModel.CreatedDate = data[i].CreatedDate;
                            lst.Add(objModel);
                        }
                    }
                    return View(lst);

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public JsonResult GetUserInformationData()
        {
            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
            admin.getConnectionString();
            HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
            var data = db.UserInformations.OrderByDescending(x => x.UserId).ToList();
            return Json(data);
        }
        public IActionResult GetEditUserInformationData(int Id)
        {
            try
            {
                HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                var data = db.UserInformations.Where(x => x.UserId == Id).Select(x => new { x.UserId, x.FirstName, x.LastName, x.UserName, x.Password, x.RoleId, x.CustomerId, x.Streaming }).ToList();
                if (data != null)
                {
                    return Json(data);
                }
                else
                {
                    return Json("");
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult UpdateUserData(UserInformation userData)
        {
            try
            {
                var CustomerId = 0;
                using (CropenAiContext db = new CropenAiContext())
                {
                    var serializedObject = HttpContext.Session.GetString("LoginInfo");
                    var adlogin = HttpContext.Session.GetString("AdLoginInfo");
                    if (serializedObject != null)
                    {
                        LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                        CustomerId = loginInfo.Id;


                        HrAdminService hrAdminServiceup = new HrAdminService(_appSettingconnection);
                        UserInformation userInfo = new UserInformation();

                        userInfo.FirstName = userData.FirstName.Trim();
                        userInfo.LastName = userData.LastName.Trim();
                        userInfo.UserName = userData.UserName.Trim();
                        userInfo.Password = userData.Password.Trim();
                        userInfo.CustomerId = CustomerId;
                        userInfo.RoleId = userData.RoleId;
                        userInfo.Streaming = userData.Streaming;
                        var Id = userData.UserId;
                        hrAdminServiceup.fnUpdateUserData(Id, userInfo);
                        return Json(true);
                    }
                    else if (adlogin != null)
                    {
                        AdLoginInfoDTO loginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                        CropenAiContext cr = new CropenAiContext();
                        CustomerId = cr.CustomerInformations.Where(x => x.UserName.Trim() == loginInfo.EmailId.Trim()).Select(x => x.CustomerId).FirstOrDefault();
                        HrAdminService hrAdminServiceup = new HrAdminService(_appSettingconnection);
                        UserInformation userInfo = new UserInformation();

                        userInfo.FirstName = userData.FirstName.Trim();
                        userInfo.LastName = userData.LastName.Trim();
                        userInfo.UserName = userData.UserName.Trim();
                        userInfo.Password = userData.Password.Trim();
                        userInfo.CustomerId = CustomerId;
                        userInfo.RoleId = userData.RoleId;
                        userInfo.Streaming = userData.Streaming;
                        var Id = userData.UserId;
                        hrAdminServiceup.fnUpdateUserData(Id, userInfo);
                        //hrAdminServiceup.fnUpdateUserData(Id, userInfo);
                        return Json(true);
                    }
                    else
                    {
                        return Json("");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }
        }
        #region Customer Configuration
        public IActionResult CustomerConfiguration()
        {
            int role = 0;
            int CustomerId = 0;
            string EmaiId = string.Empty;
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            var adlogin = HttpContext.Session.GetString("AdLoginInfo");
            if (serializedObject != null)
            {
                LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                role = loginInfo.RoleId;
                CustomerId = loginInfo.Id;
                EmaiId = loginInfo.EmailId.Trim();
                ViewBag.Role = role;

                if (ViewBag.Role == 2)
                {

                    ViewBag.EmailId = EmaiId;
                    ViewBag.LoginType = "Login";
                    return View();
                }
                else
                {
                    return View();
                }
            }
            else if (serializedObject == null && adlogin != null)
            {
                AdLoginInfoDTO adLoginInfoDTO = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                role = Convert.ToInt32(adLoginInfoDTO.RoleId);
                CustomerId = Convert.ToInt32(adLoginInfoDTO.UserId);
                EmaiId = adLoginInfoDTO.EmailId.Trim();

                ViewBag.Role = role;
                ViewBag.EmailId = EmaiId;
                if (ViewBag.Role == 2)
                {
                    ViewBag.Role = role;
                    ViewBag.EmailId = EmaiId;
                    ViewBag.LoginType = "AdLogin";
                    return View();
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
        [HttpPost]
        public IActionResult SaveCustomerConfiguration(CustomerConfiguration objModel)
        {
            string iscopied = string.Empty;
            try
            {
                var CustomerId = 0;
                using (CropenAiContext db = new CropenAiContext())
                {
                    string serializedObject = HttpContext.Session.GetString("LoginInfo");
                    LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                    CustomerId = loginInfo.Id;
                }

                CustomerConfiguration customerInformation = new CustomerConfiguration();
                customerInformation.CustomerId = CustomerId;
                customerInformation.Username = objModel.Username;
                customerInformation.UsersAllowed = objModel.UsersAllowed;
                customerInformation.FilesAllowed = objModel.FilesAllowed;
                if (objModel.FileFormat == null || objModel.FileFormat == "" || objModel.FileFormat == "," || objModel.FileFormat == "undefined")
                {
                    return Json("fileformatempty");
                }
                else
                {
                    customerInformation.FileFormat = objModel.FileFormat.Trim(',');
                }

                //customerInformation.Pdf = objModel.Pdf;
                //customerInformation.Word = objModel.Word;
                //customerInformation.Csv = objModel.Csv;
                //customerInformation.Image = objModel.Image;
                AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
                admin.fnInsertCustomerConfiguration(customerInformation);


                iscopied = "true";

            }
            catch (Exception ex)
            {
                iscopied = "false";
            }
            DataNewDTO objdModel = new DataNewDTO();
            objdModel.CompletionResult = iscopied;
            // var result = new { success = true, completionResult = objModel.CompletionResult, summeryResult = objModel.SummeryResult };
            return Json(objdModel);
        }
        [UserPrevent]
        [AdminPrevent]
        public IActionResult CustomerConfigurationList()
        {
            try
            {
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                var adlogin = HttpContext.Session.GetString("AdLoginInfo");
                if (serializedObject != null)
                {
                    CropenAiContext db = new CropenAiContext();
                    LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                    ViewBag.Role = loginInfo.RoleId;
                    ViewBag.EmailId = loginInfo.EmailId;
                    ViewBag.LoginType = "Login";
                    if (ViewBag.Role == 2)
                    {
                        var data = db.CustomerConfigurations.OrderByDescending(x => x.ConfigurationId).ThenByDescending(x => x.CreateDateTime).ToList();
                        return View(data);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }
                }
                else if (serializedObject == null && adlogin != null)
                {
                    AdLoginInfoDTO loginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                    CropenAiContext crctx = new CropenAiContext();
                    ViewBag.Role = loginInfo.RoleId;
                    if (ViewBag.Role == 2)
                    {
                        ViewBag.Role = 2;
                        ViewBag.EmailId = loginInfo.EmailId;
                        ViewBag.LoginType = "AdLogin";
                        var data = crctx.CustomerConfigurations.Where(x => x.LoginType == "AdLogin").OrderByDescending(x => x.ConfigurationId).ThenByDescending(x => x.CreateDateTime).ToList();
                        return View(data);
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public IActionResult GetEditedCustomerConfigurationData(int Id)
        {
            try
            {
                CropenAiContext db = new CropenAiContext();
                var data = db.CustomerConfigurations.Where(x => x.ConfigurationId == Id).Select(x => new { x.ConfigurationId, x.Username, x.UsersAllowed, x.FilesAllowed, x.FileFormat }).ToList();
                if (data != null)
                {
                    return Json(data);
                }
                else
                {
                    return Json("");
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult UpdateCustomerConfigurationData(CustomerConfiguration userData)
        {
            try
            {
                var CustomerId = 0;
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                var adlogin = HttpContext.Session.GetString("AdLoginInfo");

                if (serializedObject != null)
                {
                    using (CropenAiContext db = new CropenAiContext())
                    {
                        LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                        CustomerId = loginInfo.Id;
                        AdminService adminserviceup = new AdminService(_appSettingconnection, _httpContextAccessor);
                        CustomerConfiguration userInfo = new CustomerConfiguration();
                        userInfo.Username = userData.Username.Trim();
                        userInfo.CustomerId = CustomerId;
                        userInfo.FilesAllowed = userData.FilesAllowed;
                        userInfo.UsersAllowed = userData.UsersAllowed;
                        userInfo.ConfigurationId = userData.ConfigurationId;
                        userInfo.LoginType = "Login";
                        if (userData.FileFormat == null || userData.FileFormat == "" || userData.FileFormat == "," || userData.FileFormat == "undefined")
                        {
                            return Json("fileformatempty");
                        }
                        else
                        {
                            userInfo.FileFormat = userData.FileFormat.Trim(',');
                        }

                        //userInfo.Pdf = userData.Pdf;
                        //userInfo.Word = userData.Word;
                        //userInfo.Csv = userData.Csv;
                        //userInfo.Image = userData.Image;
                        adminserviceup.fnUpdateCustomerConfigurationData(userInfo.ConfigurationId, userInfo);
                        return Json(true);
                    }
                }
                else if (serializedObject == null && adlogin != null)
                {
                    using (CropenAiContext db = new CropenAiContext())
                    {
                        AdLoginInfoDTO loginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                        CustomerId = Convert.ToInt32(loginInfo.UserId);
                        AdminService adminserviceup = new AdminService(_appSettingconnection, _httpContextAccessor);
                        CustomerConfiguration userInfo = new CustomerConfiguration();
                        userInfo.Username = userData.Username.Trim();
                        userInfo.CustomerId = CustomerId;
                        userInfo.FilesAllowed = userData.FilesAllowed;
                        userInfo.UsersAllowed = userData.UsersAllowed;
                        userInfo.ConfigurationId = userData.ConfigurationId;
                        userInfo.LoginType = "AdLogin";
                        if (userData.FileFormat == null || userData.FileFormat == "" || userData.FileFormat == "," || userData.FileFormat == "undefined")
                        {
                            return Json("fileformatempty");
                        }
                        else
                        {
                            userInfo.FileFormat = userData.FileFormat.Trim(',');
                        }
                        adminserviceup.fnUpdateCustomerConfigurationData(userInfo.ConfigurationId, userInfo);
                        return Json(true);
                    }
                }
                else
                {
                    return Json("");
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }
        }
        #endregion End Customer Configuration
        #region Bot Configuration
        public IActionResult GetBotConfiguration()
        {
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
            var botConfiguration = admin.fbGetBotConfiguration(loginInfo.Id);
            return Json(botConfiguration);
        }
        /// <summary>
        /// save or Update the bot configuration
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> True Or False </returns>
        [HttpPost]
        public IActionResult SaveBotConfiguration([FromBody] BotConfiguration botConfiguration)
        {
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
            bool status = admin.fnUpdateBotConfiguration(loginInfo.Id, botConfiguration);
            return Json(new { success = status });
        }
        #endregion Bot Configuration
        public IActionResult GetEditedCustomerInformationData(int Id)
        {
            try
            {
                CropenAiContext db = new CropenAiContext();
                var data = (from CI in db.CustomerInformations.Where(x => x.CustomerId == Id)
                            join RM in db.RoleMasters on CI.RoleId equals RM.RoleId
                            select new
                            {
                                CI.CustomerId,
                                CI.Name,
                                CI.UserName,
                                CI.Password,
                                CI.OrganizationName,
                                CI.DatabaseName,
                                CI.OrganizationLogo,
                                CI.Category,
                                CI.BlobContainerName,
                                CI.AccountKey,
                                CI.AccountName,
                                CI.Streaming,
                                RM.RoleName,
                            }).ToList();

                if (data != null)
                {
                    return Json(data);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult DeleteCustomerInfo(int customerId)
        {
            bool result = false;
            try
            {
                AdminService adminService = new AdminService(_appSettingconnection, _httpContextAccessor);
                adminService.fnDeleteCustomerInfo(customerId);
                result = true;
                return Json(true);
            }
            catch (Exception ex)
            {
                result = false;
                return Json(result);
            }
        }
        [HttpPost]
        public IActionResult DeleteCustomerConfiguration(int ConfigurationId)
        {
            bool result = false;
            try
            {
                AdminService adminService = new AdminService(_appSettingconnection, _httpContextAccessor);
                adminService.fnDeleteCustomerConfiguration(ConfigurationId);
                result = true;
                return Json(true);
            }
            catch (Exception ex)
            {
                result = false;
                return Json(result);
            }
        }
        [HttpPost]
        public IActionResult DeleteUserInfo(int UserId)
        {
            bool result = false;
            try
            {
                AdminService adminService = new AdminService(_appSettingconnection, _httpContextAccessor);
                adminService.fnDeleteUserInfo(UserId);
                result = true;
                return Json(true);

            }
            catch (Exception ex)
            {
                result = false;
                return Json(result);
            }
        }
        [HolUser]
        [UserPrevent]
        public IActionResult PerformanceMatrixChecker()
        {
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            var adlogin = HttpContext.Session.GetString("AdLoginInfo");
            if (serializedObject != null)
            {
                LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                var lst = new List<PerformanceMatrixCheckerModel>();
                if (loginInfo != null)
                {
                    ViewBag.Role = loginInfo.RoleId;
                    ViewBag.EmailId = loginInfo.EmailId;
                    ViewBag.UserName = loginInfo.UserName;
                    ViewBag.LoginType = "Login";

                    var UserNames = cr.TblUserInformationMappings.Where(x => x.CustomerId == loginInfo.Id).ToList();
                    List<string> emailIds = new List<string>();
                    for (int i = 0; i < UserNames.Count; i++)
                    {
                        if (UserNames[i].UserName != "")
                            emailIds.Add(UserNames[i].UserName);
                    }

                    //var data = cr.PerformanceMatrixCheckers.OrderByDescending(x => x.PeformanceId).ToList();
                    var data = cr.PerformanceMatrixCheckers.Where(x => x.Username.ToLower() == loginInfo.EmailId.ToLower() || emailIds.Contains(x.Username)).OrderByDescending(x => x.PeformanceId).ToList();
                    PerformanceMatrixCheckerModel? objModel = new PerformanceMatrixCheckerModel();
                    CropenAiContext dbctx = new CropenAiContext();

                    for (int i = 0; i < data.Count; i++)
                    {
                        objModel = new PerformanceMatrixCheckerModel();

                        // Role 2 - Super Admin
                        if (ViewBag.Role == 2)
                        {
                            objModel.PeformanceId = data[i].PeformanceId;
                            objModel.Username = data[i].Username;
                            objModel.Prompt = data[i].Prompt;
                            objModel.Completion = data[i].Completion;
                            objModel.PromptTokens = data[i].PromptTokens;
                            objModel.CompletionTokens = data[i].CompletionTokens;
                            objModel.TotalTokens = data[i].TotalTokens;
                            objModel.Cost = data[i].Cost;
                            objModel.ResponseTime = data[i].ResponseTime;
                            objModel.CurrentDateTime = data[i].CurrentDateTime;
                            objModel.IsValid = data[i].IsValid;
                            objModel.PublicInfo = data[i].PublicInfo;
                            lst.Add(objModel);
                        }

                        // Role 1 - Admin
                        else if (ViewBag.Role == 1)
                        {
                            if (data[i].Username.ToLower() == loginInfo.EmailId.Trim().ToLower())
                            {
                                objModel.PeformanceId = data[i].PeformanceId;
                                objModel.Username = data[i].Username;
                                objModel.Prompt = data[i].Prompt;
                                objModel.Completion = data[i].Completion;
                                objModel.PromptTokens = data[i].PromptTokens;
                                objModel.CompletionTokens = data[i].CompletionTokens;
                                objModel.TotalTokens = data[i].TotalTokens;
                                objModel.Cost = data[i].Cost;
                                objModel.ResponseTime = data[i].ResponseTime;
                                objModel.CurrentDateTime = data[i].CurrentDateTime;
                                objModel.IsValid = data[i].IsValid;
                                objModel.PublicInfo = data[i].PublicInfo;
                                lst.Add(objModel);
                            }

                            HrportalAiContext hr = new HrportalAiContext(_appSettingconnection);
                            var list = hr.UserInformations.Where(x => x.CustomerId == loginInfo.Id).ToList();

                            if (list != null)
                            {
                                for (int j = 0; j < list.Count; j++)
                                {
                                    objModel = new PerformanceMatrixCheckerModel();
                                    if (data[i].Username.ToLower() == list[j].UserName.ToLower())
                                    {
                                        objModel.PeformanceId = data[i].PeformanceId;
                                        objModel.Username = data[i].Username;
                                        objModel.Prompt = data[i].Prompt;
                                        objModel.Completion = data[i].Completion;
                                        objModel.PromptTokens = data[i].PromptTokens;
                                        objModel.CompletionTokens = data[i].CompletionTokens;
                                        objModel.TotalTokens = data[i].TotalTokens;
                                        objModel.Cost = data[i].Cost;
                                        objModel.ResponseTime = data[i].ResponseTime;
                                        objModel.CurrentDateTime = data[i].CurrentDateTime;
                                        objModel.PublicInfo = data[i].PublicInfo;
                                        if (string.IsNullOrEmpty(data[i].IsValid))
                                        {
                                            objModel.IsValid = "";
                                        }
                                        else
                                        {
                                            objModel.IsValid = data[i].IsValid;
                                        }

                                        lst.Add(objModel);
                                    }
                                }
                            }
                        }
                    }
                    return View(lst);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            else if (serializedObject == null && adlogin != null)
            {
                AdLoginInfoDTO adloginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                if (adloginInfo != null)
                {
                    ViewBag.Role = adloginInfo.RoleId;
                    ViewBag.EmailId = adloginInfo.EmailId;
                    ViewBag.LoginType = "AdLogin";

                    var lst = new List<PerformanceMatrixCheckerModel>();
                    var data = cr.PerformanceMatrixCheckers.OrderByDescending(x => x.PeformanceId).ToList();
                    PerformanceMatrixCheckerModel? objModel = new PerformanceMatrixCheckerModel();
                    CropenAiContext dbctx = new CropenAiContext();

                    for (int i = 0; i < data.Count; i++)
                    {
                        objModel = new PerformanceMatrixCheckerModel();
                        if (ViewBag.Role == 2)
                        {
                            objModel.PeformanceId = data[i].PeformanceId;
                            objModel.Username = data[i].Username;
                            objModel.Prompt = data[i].Prompt;
                            objModel.Completion = data[i].Completion;
                            objModel.PromptTokens = data[i].PromptTokens;
                            objModel.CompletionTokens = data[i].CompletionTokens;
                            objModel.TotalTokens = data[i].TotalTokens;
                            objModel.Cost = data[i].Cost;
                            objModel.ResponseTime = data[i].ResponseTime;
                            objModel.CurrentDateTime = data[i].CurrentDateTime;
                            objModel.IsValid = data[i].IsValid;
                            objModel.PublicInfo = data[i].PublicInfo;
                            lst.Add(objModel);
                        }
                        else if (ViewBag.Role == 1)
                        {
                            if (data[i].Username == adloginInfo.EmailId.Trim())
                            {
                                objModel.PeformanceId = data[i].PeformanceId;
                                objModel.Username = data[i].Username;
                                objModel.Prompt = data[i].Prompt;
                                objModel.Completion = data[i].Completion;
                                objModel.PromptTokens = data[i].PromptTokens;
                                objModel.CompletionTokens = data[i].CompletionTokens;
                                objModel.TotalTokens = data[i].TotalTokens;
                                objModel.Cost = data[i].Cost;
                                objModel.ResponseTime = data[i].ResponseTime;
                                objModel.CurrentDateTime = data[i].CurrentDateTime;
                                objModel.IsValid = data[i].IsValid;
                                objModel.PublicInfo = data[i].PublicInfo;
                                lst.Add(objModel);
                            }

                            HrportalAiContext hr = new HrportalAiContext(_appSettingconnection);
                            var list = hr.UserInformations.Where(x => x.CustomerId == adloginInfo.Id).ToList();

                            if (list != null)
                            {
                                for (int j = 0; j < list.Count; j++)
                                {
                                    objModel = new PerformanceMatrixCheckerModel();
                                    if (data[i].Username == list[j].UserName)
                                    {
                                        objModel.PeformanceId = data[i].PeformanceId;
                                        objModel.Username = data[i].Username;
                                        objModel.Prompt = data[i].Prompt;
                                        objModel.Completion = data[i].Completion;
                                        objModel.PromptTokens = data[i].PromptTokens;
                                        objModel.CompletionTokens = data[i].CompletionTokens;
                                        objModel.TotalTokens = data[i].TotalTokens;
                                        objModel.Cost = data[i].Cost;
                                        objModel.ResponseTime = data[i].ResponseTime;
                                        objModel.CurrentDateTime = data[i].CurrentDateTime;
                                        objModel.PublicInfo = data[i].PublicInfo;
                                        if (string.IsNullOrEmpty(data[i].IsValid))
                                        {
                                            objModel.IsValid = "";
                                        }
                                        else
                                        {
                                            objModel.IsValid = data[i].IsValid;
                                        }
                                        lst.Add(objModel);
                                    }
                                }
                            }
                        }
                    }
                    return View(lst);
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
        [HttpPost]
        public IActionResult AddPerformanceMatrixChecker(PerformanceMatrixChecker performance)
        {
            string result = "false";
            int? PerformanceId = 0;
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            var serializedAdObject = HttpContext.Session.GetString("AdLoginInfo");
            if (serializedObject != null)
            {
                LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                ViewBag.Role = loginInfo.RoleId;
                ViewBag.EmailId = loginInfo.EmailId;
                ViewBag.UserName = loginInfo.UserName;

                AdminService adminService = new AdminService(_appSettingconnection, _httpContextAccessor);
                PerformanceMatrixChecker performanceobj = new PerformanceMatrixChecker();
                performanceobj.Username = ViewBag.EmailId;
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
                performanceobj.Hallucination = performance.Hallucination;
                performanceobj.HallucinationScore = performance.HallucinationScore;
                performanceobj.Plagiarism = performance.Plagiarism;
                performanceobj.PromptInjection = performance.PromptInjection;
                performanceobj.HateSeverity = performance.HateSeverity;
                performanceobj.SelfHarmSeverity = performance.SelfHarmSeverity;
                performanceobj.SexualSeverity = performance.SexualSeverity;
                performanceobj.ViolenceSeverity = performance.ViolenceSeverity;
                performanceobj.Context = performance.Context;

                if (loginInfo.RoleId == 1)
                {
                    performanceobj.Role = "Admin";
                }
                else if (loginInfo.RoleId == 2)
                {
                    performanceobj.Role = "SuperAdmin";
                }
                else if (loginInfo.RoleId == 3)
                {
                    performanceobj.Role = "User";
                }
                performanceobj.RoleId = loginInfo.RoleId;
                PerformanceId = adminService.fnAddPerformanceMatrixChecker(performanceobj);
                result = "true";
                //return Json(result);
                return Json(PerformanceId);
            }
            else if (serializedObject == null && serializedAdObject != null)
            {

                if (serializedAdObject != null)
                {
                    AdLoginInfoDTO adLoginInfoDTO = JsonConvert.DeserializeObject<AdLoginInfoDTO>(serializedAdObject);
                    if (adLoginInfoDTO != null)
                    {
                        AdminService adminService = new AdminService(_appSettingconnection, _httpContextAccessor);
                        PerformanceMatrixChecker performanceobj = new PerformanceMatrixChecker();
                        performanceobj.Username = adLoginInfoDTO.EmailId.Trim();
                        performanceobj.Prompt = performance.Prompt.Trim();
                        performanceobj.Completion = performance.Completion.Trim();
                        performanceobj.ResponseTime = performance.ResponseTime;
                        performanceobj.PromptTokens = performance.PromptTokens;
                        performanceobj.CompletionTokens = performance.CompletionTokens;
                        performanceobj.TotalTokens = performance.TotalTokens;
                        performanceobj.Cost = performance.Cost;
                        performanceobj.IsValid = null;
                        performanceobj.PublicInfo = Convert.ToBoolean(performance.PublicInfo);
                        performanceobj.LoginType = "AdLogin";
                        if (adLoginInfoDTO.RoleId == 1)
                        {
                            performanceobj.Role = "Admin";
                        }
                        else if (adLoginInfoDTO.RoleId == 2)
                        {
                            performanceobj.Role = "SuperAdmin";
                        }
                        else
                        {
                            performanceobj.Role = "User";
                        }
                        performanceobj.RoleId = adLoginInfoDTO.RoleId;
                        PerformanceId = adminService.fnAddPerformanceMatrixChecker(performanceobj);
                        result = "true";
                        //return Json(result);
                        return Json(PerformanceId);
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
            else
            {
                return Json(0);
                //return Json("red");
            }
        }
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult SessionTracker()
        {
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            var adlogin = HttpContext.Session.GetString("AdLoginInfo");
            var lst = new List<SessionTrackerModel>();
            SessionTrackerModel? objModel = new SessionTrackerModel();
            if (serializedObject != null)
            {
                LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                ViewBag.Role = loginInfo.RoleId;
                ViewBag.LoginType = "Login";
                ViewBag.EmailId = loginInfo.EmailId;
                ViewBag.UserName = loginInfo.UserName;
                CropenAiContext cr = new CropenAiContext();
                var data = cr.UserSessionTrackers.OrderByDescending(x => x.SessionTrackerId).ToList();

                CropenAiContext dbctx = new CropenAiContext();
                for (int i = 0; i < data.Count; i++)
                {
                    objModel = new SessionTrackerModel();
                    if (ViewBag.Role == 2)
                    {
                        objModel = new SessionTrackerModel();
                        objModel.SessionTrackerId = data[i].SessionTrackerId;
                        objModel.SessionDuration = data[i].SessionDuration;
                        objModel.Username = data[i].Username;
                        objModel.LoginWith = data[i].LoginWith;
                        objModel.LoginTime = data[i].LoginTime;
                        objModel.RoleId = data[i].RoleId;
                        objModel.RoleName = data[i].RoleName;
                        objModel.TotalPrompt = data[i].TotalPrompt;
                        objModel.TotalToken = data[i].TotalToken;
                        objModel.CreatedDateTime = data[i].CreatedDateTime;
                        lst.Add(objModel);
                    }
                    // Role 1 - Admin
                    else if (ViewBag.Role == 1)
                    {
                        if (data[i].Username == loginInfo.EmailId.Trim())
                        {
                            objModel = new SessionTrackerModel();
                            objModel.SessionTrackerId = data[i].SessionTrackerId;
                            objModel.SessionDuration = data[i].SessionDuration;
                            objModel.Username = data[i].Username;
                            objModel.LoginWith = data[i].LoginWith;
                            objModel.LoginTime = data[i].LoginTime;
                            objModel.RoleId = data[i].RoleId;
                            objModel.RoleName = data[i].RoleName;
                            objModel.TotalPrompt = data[i].TotalPrompt;
                            objModel.TotalToken = data[i].TotalToken;
                            objModel.CreatedDateTime = data[i].CreatedDateTime;
                            lst.Add(objModel);
                        }
                        HrportalAiContext hr = new HrportalAiContext(_appSettingconnection);
                        var list = hr.UserInformations.Where(x => x.CustomerId == loginInfo.Id).ToList();
                        if (list != null)
                        {
                            for (int j = 0; j < list.Count; j++)
                            {
                                objModel = new SessionTrackerModel();
                                if (data[i].Username == list[j].UserName)
                                {
                                    objModel.SessionTrackerId = data[i].SessionTrackerId;
                                    objModel.SessionDuration = data[i].SessionDuration;
                                    objModel.Username = data[i].Username;
                                    objModel.LoginWith = data[i].LoginWith;
                                    objModel.LoginTime = data[i].LoginTime;
                                    objModel.RoleId = data[i].RoleId;
                                    objModel.RoleName = data[i].RoleName;
                                    objModel.TotalPrompt = data[i].TotalPrompt;
                                    objModel.TotalToken = data[i].TotalToken;
                                    objModel.CreatedDateTime = data[i].CreatedDateTime;
                                    lst.Add(objModel);
                                }
                            }
                        }
                    }
                }
                return View(lst);
            }
            else if (serializedObject == null && adlogin != null)
            {
                AdLoginInfoDTO adloginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                if (adloginInfo != null)
                {
                    ViewBag.Role = adloginInfo.RoleId;
                    if (ViewBag.Role == 2)
                    {
                        ViewBag.EmailId = adloginInfo.EmailId;
                        ViewBag.LoginType = "AdLogin";

                        var data = cr.UserSessionTrackers.OrderByDescending(x => x.SessionTrackerId).ToList();
                        CropenAiContext dbctx = new CropenAiContext();

                        for (int i = 0; i < data.Count; i++)
                        {
                            objModel = new SessionTrackerModel();

                            objModel.SessionTrackerId = data[i].SessionTrackerId;
                            objModel.SessionDuration = data[i].SessionDuration;
                            objModel.Username = data[i].Username;
                            objModel.LoginWith = data[i].LoginWith;
                            objModel.LoginTime = data[i].LoginTime;
                            objModel.RoleId = data[i].RoleId;
                            objModel.RoleName = data[i].RoleName;
                            objModel.TotalPrompt = data[i].TotalPrompt;
                            objModel.TotalToken = data[i].TotalToken;
                            objModel.CreatedDateTime = data[i].CreatedDateTime;
                            lst.Add(objModel);

                        }

                        ViewBag.LoginType = "AdLogin";
                        return View(lst);
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
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        #region CreateTable

        public string CreateTable(string databaseName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_appSettingconnection.RestoreDefaultConnection))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("CreateTable", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.Add(new SqlParameter("@DatabaseName", SqlDbType.NVarChar, 100) { Value = databaseName });

                        // Execute the stored procedure
                        command.ExecuteNonQuery();

                        // You can also capture any output parameters or return values if needed

                        return $"Tables in {databaseName} created successfully.";
                    }

                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (log, rethrow, etc.)
                return $"Error creating database: {ex.Message}";
            }
        }
        #endregion


        //--gulfair Dashboard
        [HttpGet]
        [HolUser]
        [UserPrevent]
        public IActionResult Dashboard()
        {
            try
            {
                HttpContext.Session.SetString("UserLoginTime", DateTime.Now.ToString());
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return Json(new { success = false, message = "Please select one or more files." });
            }
            var _accountName = "";
            try
            {
                var FilesLists = new List<FileNameMappingDTO>();

                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();
                //await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                _accountName = _blobServiceClient.AccountName;

                foreach (var file in files)
                {
                    var blobClient = containerClient.GetBlobClient(file.FileName);
                    using (var stream = file.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }
                }

                var connectionString = _appSettingconnection.DefaultConnection;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    foreach (var file in files)
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_InsertFileNameMappingData", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Name", file.FileName);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    FilesLists = await ServiceMethods.GetFileNames();
                }

                return Json(new { success = true, message = "Files uploaded to Azure Blob Storage successfully!", data = FilesLists });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading files: " + ex.Message, data = "" });
            }
        }

        public async Task<IActionResult> GetComplaintData()
        {
            try
            {
                var FilesLists = new List<FileNameMappingDTO>();
                FilesLists = await ServiceMethods.GetFileNames();
                return Json(new { success = true, data = FilesLists });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessComplaints(string[] caseNumbers)
        {
            try
            {
                string caseNumber = string.Join(",", caseNumbers);

                HttpResponseMessage response;
                var client = new HttpClient();

                var endpoint = Convert.ToString(AppSettingHelper.config.GetSection("Complaints:ProcessComplaintsLink").Value);
                response = await client.GetAsync(endpoint + "?query=" + caseNumber);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data processed successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Error in response from the external service." });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Something went wrong. Try again later" });
            }
        }

        [HttpGet]
        public IActionResult UserDashboard()
        {
            try
            {
                HttpContext.Session.SetString("UserLoginTime", DateTime.Now.ToString());
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public async Task<IActionResult> GetFlightValidationDetails(int caseNumber)
        {
            try
            {
                var FlightValidationLists = new List<FlightValidationDataDTO>();
                FlightValidationLists = await ServiceMethods.GetFlightValidationDetails(caseNumber);
                return Json(new { success = true, data = FlightValidationLists });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "" });
            }
        }

    }
}