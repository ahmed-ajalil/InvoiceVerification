using Amazon.S3;
using Amazon.S3.Model;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using CR_CoreBot.CustomFilters;
using CR_CoreBot_DataAccess.CR_OpenAI;
using CR_CoreBot_DTO;
using CR_HRPortalAI_DataAcess.CR_HRPortal;
using CR_HRPortalAI_DataAcess.Models;
using LibGit2Sharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CR_CoreBot_Service.Adapter;
using CR_CoreBot.Models;
using CR_CoreBot.Helpers;

namespace CR_CoreBot.Controllers
{
    public class HRadminController : Controller
    {
        private readonly ConnectionModel _appSettingconnection;
        public HRadminController(IOptions<ConnectionModel> appSettingconnection)
        {
            _appSettingconnection = appSettingconnection.Value;
        }
  
        public IActionResult CustomerModel()
        {
            try
            {
                ViewBag.customerModelmsg = "";
                HrAdminService hrAdminService = new HrAdminService(_appSettingconnection);
                CustomerModelDTO? products = hrAdminService.readCustomerModel();
                return View(products);
            }
            catch (Exception ex)
            {
                return View();
            }

        }
        [HttpPost]
        public IActionResult CustomerModel(CustomerModel customermodel)
        {
            try
            {
                HrAdminService hrAdminService = new HrAdminService(_appSettingconnection);
                if (ModelState.IsValid)
                {
                    if (hrAdminService.fnInsertCustomerModel(customermodel))
                    {
                        ViewBag.customerModelmsg = "succesfully"; ;
                    }
                    else
                    {
                        ViewBag.customerModelmsg = "Failed"; ;
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
        public IActionResult ModelFineTuneModel()
        {
            try
            {
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                var adlogin = HttpContext.Session.GetString("AdLoginInfo");
                var lst = new List<ModelFineTuneDataDTO>();
                ModelFineTuneDataDTO objModel2 = new ModelFineTuneDataDTO();
                if (serializedObject != null)
                {
                    LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                    ModelFineTuneDataDTO? objModel = new ModelFineTuneDataDTO();
                    HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                    //var data = db.ModelFineTuneData.Where(x => x.IsValid == false.ToString()).OrderByDescending(x => x.Id).ToList();
                    var data = db.ModelFineTuneData.OrderByDescending(x => x.Id).ToList();
                    ViewBag.Role = loginInfo.RoleId;
                    ViewBag.LoginType = "Login";
                    for (int i = 0; i < data.Count; i++)
                    {
                        objModel = new ModelFineTuneDataDTO();


                        if (ViewBag.Role == 3)
                        {
                            ViewBag.EmailId = loginInfo.EmailId;
                            ViewBag.UserName = ViewBag.EmailId;
                            if (data[i].Username == loginInfo.EmailId.Trim())
                            {
                                objModel.Id = data[i].Id;
                                objModel.Username = data[i].Username;
                                objModel.Prompt = data[i].Prompt;
                                objModel.Completion = data[i].Completion;
                                objModel.IsValid = data[i].IsValid;
                                objModel.ModelName = data[i].ModelName;
                                objModel.Datetime = data[i].Datetime;
                                objModel.Tag = data[i].Tag;
                                lst.Add(objModel);
                            }
                        }
                        else if (ViewBag.Role == 2)
                        {
                            ViewBag.Role = loginInfo.RoleId;
                            ViewBag.EmailId = loginInfo.EmailId;
                            objModel.Id = data[i].Id;
                            objModel.Username = data[i].Username;
                            objModel.Prompt = data[i].Prompt;
                            objModel.Completion = data[i].Completion;
                            objModel.IsValid = data[i].IsValid;
                            objModel.ModelName = data[i].ModelName;
                            objModel.Datetime = data[i].Datetime;
                            objModel.Tag = data[i].Tag;
                            lst.Add(objModel);
                        }
                        else if (ViewBag.Role == 1)
                        {
                            ViewBag.Role = loginInfo.RoleId;
                            ViewBag.EmailId = loginInfo.EmailId;
                            if (data[i].Username == loginInfo.EmailId.Trim())
                            {
                                objModel.Id = data[i].Id;
                                objModel.Username = data[i].Username;
                                objModel.Prompt = data[i].Prompt;
                                objModel.Completion = data[i].Completion;
                                objModel.IsValid = data[i].IsValid;
                                objModel.ModelName = data[i].ModelName;
                                objModel.Datetime = data[i].Datetime;
                                objModel.Tag = data[i].Tag;
                                lst.Add(objModel);
                            }

                            HrportalAiContext hr = new HrportalAiContext(_appSettingconnection);
                            var list = hr.UserInformations.Where(x => x.CustomerId == loginInfo.Id).ToList();
                            if (list != null)
                            {
                                for (int j = 0; j < list.Count; j++)
                                {
                                    objModel = new ModelFineTuneDataDTO();
                                    if (data[i].Username == list[j].UserName.Trim())
                                    {
                                        objModel.Id = data[i].Id;
                                        objModel.Username = data[i].Username;
                                        objModel.Prompt = data[i].Prompt;
                                        objModel.Completion = data[i].Completion;
                                        objModel.IsValid = data[i].IsValid;
                                        objModel.ModelName = data[i].ModelName;
                                        objModel.Datetime = data[i].Datetime;
                                        objModel.Tag = data[i].Tag;
                                        lst.Add(objModel);
                                    }
                                }
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "Login");
                        }
                    }
                    return View(lst);
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
                        var RoleId = adlogindata.RoleId;

                        ViewBag.LoginType = "AdLogin";
                        ViewBag.Role = RoleId;
                        //ViewBag.Role = 3;
                        CropenAiContext cr = new CropenAiContext();
                        int adlogincount = cr.CustomerInformations.Where(x => x.UserName.Trim() == userName.Trim()).Count();
                        if (adlogincount == 1)
                        {
                            ViewBag.LoginType = "AdLogin";
                        }

                        if (ViewBag.Role == 3)
                        {
                            ViewBag.UserName = userName.Trim();
                        }
                        else if (ViewBag.Role == 1 || ViewBag.Role == 2)
                        {
                            ViewBag.EmailId = userName.Trim();
                        }
                        HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                        //var adlogindataresult = db.ModelFineTuneData.Where(x => x.IsValid == false.ToString()).OrderByDescending(x => x.Id).ToList();
                        var adlogindataresult = db.ModelFineTuneData.Where(x => x.Username.Trim() == userName.Trim()).OrderByDescending(x => x.Id).ToList();
                        for (int j = 0; j < adlogindataresult.Count; j++)
                        {
                            objModel2 = new ModelFineTuneDataDTO();
                            if (userName.Trim() == adlogindataresult[j].Username.Trim())
                            {
                                objModel2.Id = adlogindataresult[j].Id;
                                objModel2.Username = adlogindataresult[j].Username;
                                objModel2.Prompt = adlogindataresult[j].Prompt;
                                objModel2.Completion = adlogindataresult[j].Completion;
                                objModel2.IsValid = adlogindataresult[j].IsValid;
                                objModel2.ModelName = adlogindataresult[j].ModelName;
                                objModel2.Datetime = adlogindataresult[j].Datetime;
                                objModel2.Tag = adlogindataresult[j].Tag;
                                lst.Add(objModel2);
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
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }
        }
        [HttpPost]
        public IActionResult SaveModelFineTuneData(ModelFineTuneDatum modelFineTune)
        {
            try
            {
                CropenAiContext db = new CropenAiContext();
                string serializedObject = HttpContext.Session.GetString("LoginInfo");
                LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                var CustomerId = loginInfo.Id;
                HrAdminService hrAdminService = new HrAdminService(_appSettingconnection);
                ModelFineTuneDatum modelFineTunem = new ModelFineTuneDatum();
                modelFineTunem.Prompt = modelFineTune.Prompt.Trim();
                modelFineTunem.Completion = modelFineTune.Completion.Trim();
                modelFineTunem.LabelName = modelFineTune.LabelName.Trim();
                modelFineTunem.Url = modelFineTune.Url.Trim();
                modelFineTunem.ModelName = modelFineTune.ModelName.Trim();
                modelFineTunem.CustomerId = CustomerId;
                hrAdminService.fnInsertModelFineTuneData(modelFineTunem);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }

        }
        [HttpPost]
        public IActionResult UpdateModelFineTuneData(ModelFineTuneDatum modelFineTune)
        {
            try
            {
                var CustomerId = 0;
                using (CropenAiContext db = new CropenAiContext())
                {
                    string serializedObject = HttpContext.Session.GetString("LoginInfo");
                    LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                    CustomerId = loginInfo.Id;
                }

                HrAdminService hrAdminServiceup = new HrAdminService(_appSettingconnection);
                ModelFineTuneDatum modelFineTunemup = new ModelFineTuneDatum();

                modelFineTunemup.Prompt = modelFineTune.Prompt.Trim();
                modelFineTunemup.Completion = modelFineTune.Completion.Trim();
                modelFineTunemup.LabelName = modelFineTune.LabelName.Trim();
                modelFineTunemup.Url = modelFineTune.Url.Trim();
                modelFineTunemup.ModelName = modelFineTune.ModelName.Trim();
                modelFineTunemup.CustomerId = CustomerId;
                hrAdminServiceup.fnUpdateModelFineTuneData(modelFineTune.Id, modelFineTunemup);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }

        }
        public IActionResult GetModelfinetunedata()
        {
            try
            {
                HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                List<ModelFineTuneDatum> data = db.ModelFineTuneData.ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }
        }

        public IActionResult EditModelFineTuneData(int? Id)
        {
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
            if (loginInfo != null)
            {
                ViewBag.Role = loginInfo.RoleId;
                ViewBag.EmailId = loginInfo.EmailId.Trim();
                if (ViewBag.Role == 1 || ViewBag.Role == 2)
                {
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
            //if (Id == 0)
            //{
            //    return RedirectToAction("ModelFineTuneModel", "HRadmin");
            //}


        }
        public IActionResult GetEditModelFineTuneData(int Id)
        {
            try
            {
                HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                var data = db.ModelFineTuneData.Where(x => x.Id == Id).Select(x => new { x.Id, x.Completion, x.LabelName, x.ModelName, x.Prompt, x.Url }).ToList();
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
     
   
        private async Task<byte[]> ReadFileContent(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
         public async Task<List<BlobConnectedListDTO>> ConnectedToBlob(string accountName, string accountKey, string containerName)
        {
            try
            {
                List<string> tableData;
                int? isprocessed;
                using (var dbContext = new HrportalAiContext(_appSettingconnection))
                {
                    tableData = dbContext.BlobFileData.Select(item => item.FileName).ToList();
                }
                List<BlobConnectedListDTO> blobConnectedListModel = new List<BlobConnectedListDTO>();
                BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=" + accountName + ";AccountKey=" + accountKey + ";EndpointSuffix=core.windows.net");
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    if (System.IO.Path.GetExtension(blobItem.Name).ToLower() == ".pdf")
                    {
                        using (var dbContext = new HrportalAiContext(_appSettingconnection))
                        {
                            isprocessed = dbContext.BlobFileData.Where(x => x.FileName == blobItem.Name).Select(item => item.IsProcessed).FirstOrDefault();
                        }
                        BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                        if (tableData.Contains(blobItem.Name))
                        {
                            BlobConnectedListDTO BlobConnected = new BlobConnectedListDTO
                            {

                                Filename = blobItem.Name,
                                ContentType = blobItem.Properties.ContentType,
                                DateTime = DateTime.Now,
                                DownloadUrl = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTime.UtcNow.AddMinutes(5)).ToString(),
                                IsProcessed = (isprocessed == 0) ? "Not Processed" : "Processed"
                            };
                            blobConnectedListModel.Add(BlobConnected);
                        }
                        else
                        {
                            BlobConnectedListDTO BlobConnected = new BlobConnectedListDTO
                            {

                                Filename = blobItem.Name,
                                ContentType = blobItem.Properties.ContentType,
                                DateTime = DateTime.Now,
                                DownloadUrl = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTime.UtcNow.AddMinutes(5)).ToString(),
                                IsProcessed = "Not Uploaded"
                            };
                            blobConnectedListModel.Add(BlobConnected);
                        }

                    }

                }
                return blobConnectedListModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        static async Task<List<BlobConnectedListDTO>> GetRepositoryFiles(string apiUrl, string accessToken)
        {
            try
            {
                List<BlobConnectedListDTO> blobConnectedListModel = new List<BlobConnectedListDTO>();
                string[] urlParts = apiUrl.TrimEnd('/').Split('/');
                string owner = urlParts[urlParts.Length - 2];
                string repository = urlParts[urlParts.Length - 1];
                var github = new GitHubClient(new Octokit.ProductHeaderValue("MyApp"));
                var basicAuth = new Octokit.Credentials(accessToken);
                github.Credentials = basicAuth;
                IReadOnlyList<RepositoryContent> contents = await github.Repository.Content.GetAllContents(owner, repository);
                for (int i = 0; i < contents.Count; i++)
                {
                    BlobConnectedListDTO blobModel = new BlobConnectedListDTO
                    {
                        Filename = contents[i].Name,
                        ContentType = contents[i].Name.TrimEnd('.').Split('.')[1],
                        DateTime = DateTime.Now
                    };
                    blobConnectedListModel.Add(blobModel);
                }
                return blobConnectedListModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool DeleteBlobFileFromDatabase(string filename)
        {
            bool result = false;
            try
            {
                HrAdminService hradminService = new HrAdminService(_appSettingconnection);
                hradminService.fnDeleteBlobFileFromDatabase(filename);
                result = true;
                return result;
            }
            catch (Exception ex)
            {
                result = false;
                return result;
            }
        }
        /// <summary>
        /// This method is for adding new completion result on the data base 
        /// </summary>
        [HttpPost]
        public IActionResult AddNewCompletionResult(int id,string text)
        {
            try
            {
                using (HrportalAiContext db = new HrportalAiContext(_appSettingconnection))
                {
                    var record = db.ModelFineTuneData.FirstOrDefault(x => x.Id == id);

                    // Check if the record exists
                    if (record != null)
                    {
                        // Update the CompletionNew property
                        record.Tag = text;
                        
                        // Save changes to the database
                        db.SaveChanges();
                    }

                    return Json("Success"); 
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        [HttpPost]
        public IActionResult DeleteBlobDocument(string Filename, string LoginCase)
        {
            string AccountName = string.Empty;
            string AccountKey = string.Empty;
            bool exists = false;
            bool result = false;
            try
            {
                var serializedObject = HttpContext.Session.GetString("LoginInfo");
                var adlogin = HttpContext.Session.GetString("AdLoginInfo");
                if (serializedObject != null)
                {
                    LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                    HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                    ViewBag.Role = loginInfo.RoleId;
                    ViewBag.EmailId = loginInfo.EmailId;
                    if (ViewBag.Role == 1 || ViewBag.Role == 2)
                    {
                        CropenAiContext cr = new CropenAiContext();

                        var custinfo = cr.CustomerInformations.Where(x => x.CustomerId == loginInfo.Id).FirstOrDefault();

                        if (custinfo != null)
                        {
                            AccountName = custinfo.AccountName;
                            AccountKey = custinfo.AccountKey;
                        }

                        BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=" + AccountName + ";AccountKey=" + AccountKey + ";EndpointSuffix=core.windows.net");


                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("hrdocumentsdemo");
                        int id = 0;
                        BlobClient blobClient = containerClient.GetBlobClient(Filename.Trim());
                        exists = blobClient.Exists();
                        if (exists == true)
                        {
                            blobClient.Delete(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
                            DeleteBlobFileFromDatabase(Filename);
                            result = true;
                            return Json(result);
                        }
                        else
                        {
                            result = false;
                            return Json(result);
                        }
                    }
                }
                else if (serializedObject == null && adlogin != null)
                {
                    AdLoginInfoDTO loginInfo = JsonConvert.DeserializeObject<AdLoginInfoDTO>(adlogin);
                    HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                    ViewBag.Role = loginInfo.RoleId;
                    ViewBag.EmailId = loginInfo.EmailId.Trim();
                    if (ViewBag.Role == 1 || ViewBag.Role == 2)
                    {
                        CropenAiContext cr = new CropenAiContext();

                        var custinfo = cr.CustomerInformations.Where(x => x.CustomerId == loginInfo.AdLoggedInUserId).FirstOrDefault();
                        string blobContainerName = string.Empty;
                        if (custinfo != null)
                        {
                            AccountName = custinfo.AccountName;
                            AccountKey = custinfo.AccountKey;
                        }

                        BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=" + AccountName + ";AccountKey=" + AccountKey + ";EndpointSuffix=core.windows.net");



                        //BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("hrdocumentsdemo");

                        blobContainerName = custinfo.BlobContainerName.Trim();
                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
                        int id = 0;
                        BlobClient blobClient = containerClient.GetBlobClient(Filename.Trim());
                        exists = blobClient.Exists();
                        if (exists == true)
                        {
                            blobClient.Delete(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
                            DeleteBlobFileFromDatabase(Filename);
                            result = true;
                            return Json(result);
                        }
                        else
                        {
                            result = false;
                            return Json(result);
                        }
                    }

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                result = false;
                return Json(result);
            }

        }
        class GitHubFile
        {
            public string Name { get; set; }
        }
    }
}
