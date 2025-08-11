using CR_CoreBot_DataAccess.CR_OpenAI;
using CR_CoreBot_DTO;
using CR_HRPortalAI_DataAcess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using CR_CoreBot.Models;
using CR_HRPortalAI_DataAcess.CR_HRPortal;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Amazon.S3.Model;
using Azure.AI.ContentSafety;
using Azure;
using CR_CoreBot_DataAccess.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Office.Interop.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Task = System.Threading.Tasks.Task;
using CR_CoreBot_Service.Adapter;
using CR_CoreBot_DataAccess;

namespace CR_CoreBot.Controllers
{
    public class SecurityController : Controller
    {
        
        private readonly ConnectionModel _appSettingconnection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AdminController> _logger;
        private IHostingEnvironment Environment;
        IConfiguration _configuration;
        public SecurityController(IOptions<ConnectionModel> appSettingconnection, IHttpContextAccessor httpContextAccessor, ILogger<AdminController> logger, IHostingEnvironment environment, IConfiguration configuration)
        {
            _appSettingconnection = appSettingconnection.Value;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            Environment = environment;
            _configuration = configuration;
        }
        CropenAiContext cr = new CropenAiContext();
        CropenAiProcContext pr = new CropenAiProcContext();
        public async Task<IActionResult> AiAssessmentDashboard()
        {
            AiAssessmentModel model = new AiAssessmentModel();
            var Loginval = HttpContext.Session.GetString("LoginInfo");
            var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(Loginval);
            var email = logindata.EmailId;
            try
            {
                model.aiAssessmentModel = await pr.assessmentScoreDb
                         .FromSqlRaw("EXEC Proc_AiAssessmentscore @username = {0}", email)
                         .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("SecurityDashboard->AiAssessmentDashboard->" + ex);
            }
            return View(model);
        }
        [HttpGet]
        public async Task<JsonResult> GetAiDashboardDropdown(string val)
        {
            AiAssessmentModel model = new AiAssessmentModel();
            try
            {
                var data = val.Split("_");
                string cat = data[0];
                string type = data[1];
                var Loginval = HttpContext.Session.GetString("LoginInfo");
                var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(Loginval);
                var email = logindata.EmailId;
                //model.drpdown = cr.AiDashboardSummaryDropdowns.Where(x => x.Category == val).Select(y => y.Type).ToList();
                if (type == "All")
                {
                    model.performanceMatrixCheckers = cr.PerformanceMatrixCheckers.Where(p => p.Username == email).OrderByDescending(p => p.CurrentDateTime).ToList();
                }
                else if (cat == "HallucinationScore")
                {
                    model.performanceMatrixCheckers = cr.PerformanceMatrixCheckers.Where(p => p.Hallucination == true && EF.Property<decimal>(p, cat) <= Convert.ToInt16(type) && p.Username == email).OrderByDescending(p => p.CurrentDateTime).ToList();
                }
                else if (cat == "HateSeverity" || cat == "SelfHarmSeverity" || cat == "SexualSeverity" || cat == "ViolenceSeverity")
                {
                    model.performanceMatrixCheckers = cr.PerformanceMatrixCheckers.Where(p => EF.Property<int>(p, cat) == Convert.ToInt16(type) && p.Username == email).OrderByDescending(p => p.CurrentDateTime).ToList();
                }
                else
                {
                    model.performanceMatrixCheckers = cr.PerformanceMatrixCheckers.Where(p => EF.Property<bool>(p, cat) == Convert.ToBoolean(type) && p.Username == email).OrderByDescending(p => p.CurrentDateTime).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Dashboard->lmlAssessmentSummary->" + ex);
            }
            return Json(model);
        }
        [HttpGet]
        public async Task<JsonResult> GetAiDashboardData()
        {
            AiAssessmentModel model = new AiAssessmentModel();
            try
            {
                var Loginval = HttpContext.Session.GetString("LoginInfo");
                var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(Loginval);
                var email = logindata.EmailId;
                var UserNames = cr.TblUserInformationMappings.Where(x => x.CustomerId == logindata.Id).ToList();
                List<string> emailIds = new List<string>();
                for (int i = 0; i < UserNames.Count; i++)
                {
                    if (UserNames[i].UserName != "")
                        emailIds.Add(UserNames[i].UserName);
                }
                model.performanceMatrixCheckers = cr.PerformanceMatrixCheckers.Where(x => x.Username == logindata.EmailId || emailIds.Contains(x.Username)).OrderByDescending(x => x.PeformanceId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Dashboard->lmlAssessmentSummary->" + ex);
            }
            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> ContentSafetyBlocklistGetAllItems()
        {
            AzureOpenAIChat azureOpenAI = new AzureOpenAIChat(_configuration);
            List<BlockListItemDetail> AllItems = new List<BlockListItemDetail>();
            try
            {
                AllItems = await azureOpenAI.BlockListGetAllItems();
            }
            catch (Exception ex)
            {
                _logger.LogError("SecurityController", "AiAssessmentDashboard", ex);
            }
            return Json(AllItems);
        }

        [HttpGet]
        public async Task ContentSafetyBlocklistAddItems(string Text)
        {
            try
            {
                AzureOpenAIChat azureOpenAI = new AzureOpenAIChat(_configuration);
                await azureOpenAI.BlockListAddItems(Text);
            }
            catch (Exception ex)
            {
                _logger.LogError("SecurityController", "AiAssessmentDashboard", ex);
            }
        }
        [HttpGet]
        public async Task ContentSafetyBlocklistDeleteItems(string Text)
        {
            try
            {
                AzureOpenAIChat azureOpenAI = new AzureOpenAIChat(_configuration);
                List<BlockListItemDetail> AllItems = await azureOpenAI.BlockListGetAllItems();
                string? TextId = AllItems.Where(x => x.Text == Text).Select(y => y.BlocklistItemId).FirstOrDefault();
                if (TextId != null)
                {
                    await azureOpenAI.BlockListDeleteItems(TextId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SecurityController", "AiAssessmentDashboard", ex);
            }
        }

        [HttpGet]
        public async Task SaveContentSafetySettings(int violenceRejectionThreshold, int hateRejectionThreshold, int sexualRejectionThreshold, int selfHarmRejectionThreshold)
        {
            var normaluserlogin = HttpContext.Session.GetString("LoginInfo");
            var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);
            try
            {
                ServiceMethods security = new ServiceMethods(_appSettingconnection);
                await security.SetContentSafetySettings(violenceRejectionThreshold, hateRejectionThreshold, sexualRejectionThreshold, selfHarmRejectionThreshold, logindata.EmailId);
            }
            catch (Exception ex)
            {
                _logger.LogError("SecurityController", "AiAssessmentDashboard", ex);
            }
        }
        public async Task<IActionResult> ConfigureContentSafetyFilters()
        {
            var normaluserlogin = HttpContext.Session.GetString("LoginInfo");
            var logindata = JsonConvert.DeserializeObject<LoginInfoDTO>(normaluserlogin);
            try
            {
                ServiceMethods security = new ServiceMethods(_appSettingconnection);
                ContentSafetySettings? dto = await security.GetContentSafetySettings(logindata.EmailId);
                ViewBag.selfHarmRejectionThreshold = dto?.selfHarmRejectionThreshold;
                ViewBag.violenceRejectionThreshold = dto?.violenceRejectionThreshold;
                ViewBag.hateRejectionThreshold = dto?.hateRejectionThreshold;
                ViewBag.sexualRejectionThreshold = dto?.sexualRejectionThreshold;
            }
            catch (Exception ex)
            {
                _logger.LogError("SecurityController", "ConfigureContentSafetyFilters", ex);
            }
            return View();
        }
    }
}
