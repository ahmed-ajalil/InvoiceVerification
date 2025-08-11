using CR_CoreBot_DataAccess.CR_OpenAI;
using CR_HRPortalAI_DataAcess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using System.Linq;
using CR_CoreBot_Service.Adapter;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Bot.Schema.Teams;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.ML;
using Microsoft.Data.SqlClient;
using System.Data;
using CR_HRPortalAI_DataAcess.CR_HRPortal;
using CR_CoreBot.CustomFilters;
using CR_CoreBot_DTO;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace CR_CoreBot.Controllers
{
    public class ModelDetailsController : Controller
    {
        private static ConnectionModel _appSettingconnection;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ModelDetailsController(IOptions<ConnectionModel> appSettingconnection, IHttpContextAccessor httpContextAccessor)
        {
            _appSettingconnection = appSettingconnection.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Display the list
        [HttpGet] // Explicitly marking it as GET
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> IndexAsync()
        {
            var dbContexthr = new HrportalAiContext(_appSettingconnection);
            var modelDetailsList = await dbContexthr.ModelDetails
            .OrderByDescending(m => m.Id) // Change this to the property you want to sort by
            .ToListAsync();


            return View(modelDetailsList);
        }

        // GET: Create
        [HttpGet]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult Create()
        {

            var dbContexthr = new HrportalAiContext(_appSettingconnection);
            var modelNames = dbContexthr.ModelNames.ToList();
            var sizes = dbContexthr.Sizes.ToList();

            // Use ViewBag or ViewData to pass data to the view
            ViewBag.ModelNames = new SelectList(modelNames, "ModelName", "ModelName");
            ViewBag.Sizes = sizes;

            return View();

        }

        [HttpGet]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public JsonResult GetSizesByModelName(string modelName)
        {
            var dbContexthr = new HrportalAiContext(_appSettingconnection);
            var sizes = dbContexthr.Sizes
                                .Where(s => s.ModelName == modelName)
                                .ToList();

            return Json(sizes);
        }


        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> Create(ModelDetails modelDetails)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dbContexthr = new HrportalAiContext(_appSettingconnection);

                    dbContexthr.ModelDetails.Add(modelDetails);
                    await dbContexthr.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Model details created successfully!";
                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction("Index", "ModelDetails");
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Failed to create model details!";
                    return RedirectToAction(nameof(Create));
                }
            }
            TempData["ErrorMessage"] = "Invalid form submission!";
            return View(modelDetails);
        }

        // GET: Get edited model information by Id
        [HttpGet]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult GetEditedModelInformationData(int Id)
        {
            try
            {
                HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                var data = db.ModelDetails.Where(x => x.Id == Id)
                            .Select(CI => new
                            {
                                CI.Id,
                                CI.EndPoint,
                                CI.ApiKey,
                                CI.DeploymentName,
                                CI.ApiVersion,
                                CI.ModelName,
                                CI.Size,
                            })
                            .ToList();

                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult GetEditedSystemInstructionInformationData(int Id)
        {
            try
            {
                HrportalAiContext db = new HrportalAiContext(_appSettingconnection);
                var data = db.SystemInstructions
                             .Where(x => x.Id == Id)
                             .Select(CI => new
                             {
                                 CI.Id,
                                 CI.Rules,
                                 CI.Industry
                             })
                             .FirstOrDefault(); // Use FirstOrDefault instead of ToList

                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        [HttpGet]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> GetEditedSuggestedQuestionInformationData(int id)
        {
            try
            {
                using var db = new HrportalAiContext(_appSettingconnection);
                var question = await db.Suggestions
                    .Where(x => x.Id == id)
                    .Select(q => new { q.Id, q.Question })
                    .FirstOrDefaultAsync();

                if (question == null)
                {
                    return Json(new { success = false, message = "Question not found" });
                }

                return Json(question);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error retrieving data: {ex.Message}" });
            }
        }

        [HttpGet]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> GetEditedTrainModelInformationData(int id)
        {
            try
            {
                using var db = new HrportalAiContext(_appSettingconnection);
                var trainModel = await db.TrainModel
                    .Where(x => x.Id == id)
                    .Select(q => new { q.Id, q.Question, q.SQLQuery })
                    .FirstOrDefaultAsync();

                if (trainModel == null)
                {
                    return Json(new { success = false, message = "Question and Query not found" });
                }

                return Json(trainModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error retrieving data: {ex.Message}" });
            }
        }

        // POST: Save model information
        [HttpPost]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> SaveModelInfoAsync(ModelDetails modelDetails)
        {
            if (ModelState.IsValid)
            {
                using (var db = new HrportalAiContext(_appSettingconnection))

                {
                    var existingModel = db.ModelDetails
                             .FirstOrDefault(m => m.EndPoint == modelDetails.EndPoint &&
                             m.ApiKey == modelDetails.ApiKey &&
                             m.DeploymentName == modelDetails.DeploymentName &&
                             m.ApiVersion == modelDetails.ApiVersion &&
                             m.ModelName == modelDetails.ModelName &&
                             m.Size == modelDetails.Size);
                    if (existingModel != null)
                    {
                        return Json(new { success = false, message = "This model already exists." });
                    }
                    var anyModels = db.ModelDetails.Any();

                    // If no models exist, set Status = 1, otherwise set Status = 0
                    modelDetails.DefaultType = anyModels ? 0 : 1;
                    modelDetails.Status = 1;

                    db.ModelDetails.Add(modelDetails);

                    await db.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Model Information saved successfully." });
            }

            return Json(new { success = false, message = "Please fill the required fields." });
        }

        [HttpPost]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> SaveSystemInstructionInfo(SystemInstructionsDTO systemInstructionsDTO)
        {
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            if (ModelState.IsValid)
            {
                string EmailId = string.Empty;
                LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                using (var db = new HrportalAiContext(_appSettingconnection))
                {
                    var allModels = db.SystemInstructions.ToList();
                    foreach (var Model in allModels)
                    {

                        Model.Status = 0;
                        Model.DefaultType = 0;
                    }
                    systemInstructionsDTO.Status = 1;
                    systemInstructionsDTO.DefaultType = 1;

                    db.SystemInstructions.Add(systemInstructionsDTO);


                    await db.SaveChangesAsync();


                    db.Database.OpenConnection();


                    var logCommand = db.Database.GetDbConnection().CreateCommand();
                    logCommand.CommandText = "EXEC sp_LogSystemInstructionChanges @SystemInstructionId, @ChangeType, @OldRules, @NewRules, @OldIndustry, @NewIndustry, @UserId";
                    logCommand.Parameters.Add(new SqlParameter("@SystemInstructionId", systemInstructionsDTO.Id));
                    logCommand.Parameters.Add(new SqlParameter("@ChangeType", "INSERT"));
                    logCommand.Parameters.Add(new SqlParameter("@OldRules", DBNull.Value));
                    logCommand.Parameters.Add(new SqlParameter("@NewRules", systemInstructionsDTO.Rules));
                    logCommand.Parameters.Add(new SqlParameter("@OldIndustry", DBNull.Value));
                    logCommand.Parameters.Add(new SqlParameter("@NewIndustry", systemInstructionsDTO.Industry));
                    logCommand.Parameters.Add(new SqlParameter("@UserId", loginInfo.EmailId));

                    await logCommand.ExecuteNonQueryAsync();


                    db.Database.CloseConnection();
                }


                return Json(new { success = true, message = "System Instruction Information saved successfully." });
            }

            return Json(new { success = false, message = "Please fill the required fields." });
        }





        [HttpPost]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> SaveSuggestedQuestionInfo(List<SuggestionsDTO> questions)
        {
            if (ModelState.IsValid)
            {
                using (var db = new HrportalAiContext(_appSettingconnection))
                {
                    var connection = (SqlConnection)db.Database.GetDbConnection();
                    int flag = 0; // This will store the flag from the stored procedure
                    int Count = 0;
                    try
                    {
                        await connection.OpenAsync();

                        // Step 1: Call the stored procedure to check the limit
                        using (SqlCommand cmd = new SqlCommand("sp_CheckSuggestionLimit", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Execute the stored procedure and get the flag
                            flag = (int)await cmd.ExecuteScalarAsync();
                        }
                        using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Suggestions", connection))
                        {
                            Count = (int)await cmd.ExecuteScalarAsync();
                        }
                        // Step 2: If the flag is 1, it means the limit has been exceeded
                        if (flag == 1 || Count + questions.Count > 20)
                        {
                            return Json(new { success = false, message = "You have reached your suggestion question limit." });
                        }

                        // Step 3: Check and add the new questions if the limit is not exceeded
                        foreach (var question in questions)
                        {
                            // Check if the question already exists
                            var existingModel = await db.Suggestions
                                .FirstOrDefaultAsync(m => m.Question == question.Question);

                            if (existingModel != null)
                            {
                                return Json(new { success = false, message = $"The question '{question.Question}' already exists." });
                            }

                            // Add the question if it doesn't exist
                            db.Suggestions.Add(question);
                        }

                        // Save changes asynchronously after all checks
                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors while checking the limit or saving questions
                        return Json(new { success = false, message = "Error occurred: " + ex.Message });
                    }
                }

                return Json(new { success = true, message = "Suggested Questions saved successfully." });
            }

            return Json(new { success = false, message = "Please fill the required fields." });
        }


        [HttpPost]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> SaveTrainModelInfo([FromBody] List<TrainModelDTO> trainModels)
        {
            if (ModelState.IsValid)
            {
                using (var db = new HrportalAiContext(_appSettingconnection))
                {
                    var connection = (SqlConnection)db.Database.GetDbConnection();
                    int flag = 0; // This will store the flag from the stored procedure
                    int Count = 0;
                    try
                    {
                        await connection.OpenAsync();

                        // Step 1: Call the stored procedure to check the limit
                        using (SqlCommand cmd = new SqlCommand("sp_CheckTrainModelLimit", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Execute the stored procedure and get the flag
                            flag = (int)await cmd.ExecuteScalarAsync();
                        }
                        using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM TrainModel", connection))
                        {
                            Count = (int)await cmd.ExecuteScalarAsync();
                        }
                        // Step 2: If the flag is 1, it means the limit has been exceeded
                        if (flag == 1 || Count + trainModels.Count > 20)
                        {
                            return Json(new { success = false, message = "You have reached your 20 Question And Query limit." });
                        }

                        // Step 3: Check and add the new questions if the limit is not exceeded
                        foreach (var question in trainModels)
                        {
                            var existingModel = db.TrainModel
                                .FirstOrDefault(m => m.Question == question.Question &&
                                m.SQLQuery == question.SQLQuery);

                            if (existingModel != null)
                            {
                                return Json(new { success = false, message = "The question and SQL Query already exist." });
                            }

                            if (question.Id == 0) // New entry
                            {
                                db.TrainModel.Add(new TrainModelDTO
                                {
                                    Question = question.Question,
                                    SQLQuery = question.SQLQuery
                                });
                            }
                            else // Update existing entry
                            {
                                var existingQuestion = await db.TrainModel.FindAsync(question.Id);
                                if (existingQuestion != null)
                                {
                                    existingQuestion.Question = question.Question;
                                    existingQuestion.SQLQuery = question.SQLQuery;
                                    db.TrainModel.Update(existingQuestion);
                                }
                            }
                            await db.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors while checking the limit or saving questions
                        return Json(new { success = false, message = "Error occurred: " + ex.Message });
                    }
                }

                return Json(new { success = true, message = "Suggested Questions saved successfully." });
            }

            return Json(new { success = false, message = "Please fill the required fields." });
        }



        // POST: Update model information
        [HttpPost]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult UpdateModelInfo(ModelDetails modelDetails)
        {
            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
            bool checkExist = false;


            if (ModelState.IsValid)
            {
                var db = new HrportalAiContext(_appSettingconnection);
                var existingModel = db.ModelDetails
                             .FirstOrDefault(m => m.EndPoint == modelDetails.EndPoint &&
                             m.ApiKey == modelDetails.ApiKey &&
                             m.DeploymentName == modelDetails.DeploymentName &&
                             m.ApiVersion == modelDetails.ApiVersion &&
                             m.ModelName == modelDetails.ModelName &&
                             m.Size == modelDetails.Size);


                if (existingModel == null)
                {
                    checkExist = true;
                }
                else
                {
                    if (existingModel.Id == modelDetails.Id && existingModel.EndPoint == modelDetails.EndPoint && existingModel.ApiKey == modelDetails.ApiKey
    && existingModel.ApiVersion == modelDetails.ApiVersion && existingModel.ModelName == modelDetails.ModelName && existingModel.Size == modelDetails.Size)
                    {
                        checkExist = true;
                    }
                    else if (existingModel.Id != modelDetails.Id && existingModel.EndPoint == modelDetails.EndPoint && existingModel.ApiKey == modelDetails.ApiKey
    && existingModel.ApiVersion == modelDetails.ApiVersion && existingModel.ModelName == modelDetails.ModelName && existingModel.Size == modelDetails.Size)
                    {
                        checkExist = false;
                    }
                }


                if (checkExist == true)
                {
                    try
                    {
                        // Insert or update the model details via the service
                        admin.fnInsertModelInformation(modelDetails);

                        // If successful, return a success response
                        return Json(new { success = true, message = "Model Information updated successfully." });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Error while saving data." });
                    }


                }
                else
                {
                    return Json(new { success = false, message = $"The Model already exists." });
                }

            }
            else
            {
                return Json(new { success = false, message = "Please fill the required fields" });
            }
        }

        [HttpPost]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult UpdateSystemInstructionInfo(SystemInstructionsDTO systemInstructionsDTO)
        {
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            var admin = new AdminService(_appSettingconnection, _httpContextAccessor);

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill the required fields." });
            }

            try
            {
                var db = new HrportalAiContext(_appSettingconnection);
                var existingModel = db.SystemInstructions
                                     .FirstOrDefault(m => m.Rules == systemInstructionsDTO.Rules && m.Industry == systemInstructionsDTO.Industry);

                // Validate if the system instruction already exists
                if (existingModel != null && existingModel.Id != systemInstructionsDTO.Id)
                {
                    return Json(new { success = false, message = "The system instruction already exists." });
                }

                // Proceed with updating or inserting system instruction
                string emailId = string.Empty;
                var loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);

                var existingInstruction = db.SystemInstructions.FirstOrDefault(x => x.Id == systemInstructionsDTO.Id);
                if (existingInstruction == null)
                {
                    return Json(new { success = false, message = "System Instruction not found." });
                }

                // Log the old values for tracking
                var oldRules = existingInstruction.Rules;
                var oldIndustry = existingInstruction.Industry;

                // Update system instruction details
                admin.fnInsertSystemInstructionInformation(systemInstructionsDTO);
                SetActivatedInstruction(systemInstructionsDTO.Id);

                // Log the changes in the database
                db.Database.OpenConnection();
                var logCommand = db.Database.GetDbConnection().CreateCommand();
                logCommand.CommandText = "EXEC sp_LogSystemInstructionChanges @SystemInstructionId, @ChangeType, @OldRules, @NewRules, @OldIndustry, @NewIndustry, @UserId";
                logCommand.Parameters.Add(new SqlParameter("@SystemInstructionId", systemInstructionsDTO.Id));
                logCommand.Parameters.Add(new SqlParameter("@ChangeType", "UPDATE"));
                logCommand.Parameters.Add(new SqlParameter("@OldRules", oldRules));
                logCommand.Parameters.Add(new SqlParameter("@NewRules", systemInstructionsDTO.Rules));
                logCommand.Parameters.Add(new SqlParameter("@OldIndustry", oldIndustry));
                logCommand.Parameters.Add(new SqlParameter("@NewIndustry", systemInstructionsDTO.Industry));
                logCommand.Parameters.Add(new SqlParameter("@UserId", loginInfo.EmailId));
                logCommand.ExecuteNonQuery();
                db.Database.CloseConnection();

                return Json(new { success = true, message = "System Instruction Information updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error while saving data." });
            }
        }


        [HttpPost]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult UpdateSuggestedQuestionInfo(SuggestionsDTO suggestedQuestionDTO)
        {
            bool checkExist = false;
            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
            if (ModelState.IsValid)
            {
                var db = new HrportalAiContext(_appSettingconnection);
                var existingModel = db.Suggestions
                   .FirstOrDefault(m => m.Question == suggestedQuestionDTO.Question);

                if (existingModel == null)
                {
                    checkExist = true;
                }
                else
                {
                    if (existingModel.Id == suggestedQuestionDTO.Id && existingModel.Question == suggestedQuestionDTO.Question)
                    {
                        checkExist = true;
                    }
                    else if (existingModel.Id != suggestedQuestionDTO.Id && existingModel.Question == suggestedQuestionDTO.Question)
                    {
                        checkExist = false;
                    }
                }


                if (checkExist == true)
                {
                    try
                    {
                        // Insert or update the model details via the service
                        admin.fnInsertSuggestedQuestionInformation(suggestedQuestionDTO); // Assuming this service handles both insert/update

                        // If successful, return a success response
                        return Json(new { success = true, message = "Suggested Question updated successfully." });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Error while saving data." });
                    }


                }
                else
                {
                    return Json(new { success = false, message = $"The question '{suggestedQuestionDTO.Question}' already exists." });
                }


            }
            else
            {
                return Json(new { success = false, message = "Please Fill the Required Fields" });
            }
        }

        [HttpPost]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult UpdateTrainModelInfo(TrainModelDTO trainModelDTO)
        {
            bool checkExist = false;
            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
            if (ModelState.IsValid)
            {
                var db = new HrportalAiContext(_appSettingconnection);
                var existingModel = db.TrainModel
                  .FirstOrDefault(m => m.Question == trainModelDTO.Question &&
                  m.SQLQuery == trainModelDTO.SQLQuery);
                if (existingModel == null)
                {
                    checkExist = true;
                }
                else
                {
                    if (existingModel.Id != trainModelDTO.Id && (existingModel.Question == trainModelDTO.Question && existingModel.SQLQuery == trainModelDTO.SQLQuery))
                    {
                        checkExist = false;
                    }
                    else
                    {
                        checkExist = true;
                    }
                }
                if (checkExist == true)
                {
                    try
                    {
                        // Insert or update the model details via the service
                        admin.fnInsertTrainModelInformation(trainModelDTO); // Assuming this service handles both insert/update

                        // If successful, return a success response
                        return Json(new { success = true, message = "Question and Query updated successfully." });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Error while saving data: " + ex.Message });
                    }


                }
                else
                {
                    return Json(new { success = false, message = $"The question and SQL Query  already exists." });
                }


            }
            else
            {
                // Return validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Please fill the required fields.", errors });
            }
        }


        // DELETE: Delete model by Id
        [HttpPost] // Explicitly marking as HttpDelete
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult Delete(int id)
        {
            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);

            try
            {
                using (var db = new HrportalAiContext(_appSettingconnection))
                {
                    // Find the model by id
                    var model = db.ModelDetails.FirstOrDefault(x => x.Id == id);
                    if (model == null)
                    {
                        return Json(new { success = false, message = "Model not found." });
                    }


                    if (model.DefaultType == 1)
                    {
                        return Json(new { success = false, message = "This is the Default Model. You cannot delete it." });
                    }

                    if (model.Status == 1)
                    {

                        admin.fnDeleteModelInfo(id);


                        var activeModels = db.ModelDetails.Where(x => x.Status == 1 && x.Id != id).ToList();

                        // If no other active models exist, activate the default model
                        if (!activeModels.Any())
                        {
                            var defaultModel = db.ModelDetails.FirstOrDefault(x => x.DefaultType == 1);
                            SetActivatedModels(new List<int> { defaultModel.Id });
                        }
                    }
                    else
                    {
                        // If model is not active, delete it
                        admin.fnDeleteModelInfo(id);
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error while deleting data: {ex.Message}" });
            }
        }


        private bool ModelDetailsExists(int id)
        {
            var dbContexthr = new HrportalAiContext(_appSettingconnection);
            return dbContexthr.ModelDetails.Any(e => e.Id == id);
        }


        //System Instruction
        [HttpGet] // Explicitly marking it as GET
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> SystemInstructionInfoListAsync()
        {
            var dbContexthr = new HrportalAiContext(_appSettingconnection);
            var SystemInstuctionList = await dbContexthr.SystemInstructions
                .OrderByDescending(m => m.Id).ToListAsync();
            return View(SystemInstuctionList);

        }

        [HttpGet]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> SuggestedQuestionInfoListAsync()
        {
            try
            {
                using var dbContexthr = new HrportalAiContext(_appSettingconnection);
                var suggestedQuestionList = await dbContexthr.Suggestions.OrderByDescending(m => m.Id).ToListAsync();
                return View(suggestedQuestionList);
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> TrainModelInfoListAsync()
        {
            try
            {
                using var dbContexthr = new HrportalAiContext(_appSettingconnection);
                var queryQuestionList = await dbContexthr.TrainModel.OrderByDescending(m => m.Id).ToListAsync();
                return View(queryQuestionList);
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet] // Explicitly marking it as GET
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public async Task<IActionResult> SystemInstructionInfoAsync(int? id)
        {
            using var dbContexthr = new HrportalAiContext(_appSettingconnection);

            if (id.HasValue)
            {
                // Editing an existing system instruction
                var existingSystemInstruction = await dbContexthr.SystemInstructions.FindAsync(id.Value);
                if (existingSystemInstruction == null)
                {
                    return NotFound();
                }
                return View(existingSystemInstruction);
            }
            else
            {
                // Adding a new system instruction
                var existingSystemInstruction = await dbContexthr.SystemInstructions.FirstOrDefaultAsync();
                if (existingSystemInstruction != null)
                {
                    TempData["ShowWarning"] = true;
                    TempData["WarningMessage"] = "A system instruction already exists. Please edit the existing one.";
                    return RedirectToAction("SystemInstructionInfoList");
                }
                return View(new SystemInstructionsDTO());
            }
        }

        [HttpGet] // Explicitly marking it as GET
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult SuggestedQuestionInfo()
        {
            return View();
        }

        [HttpGet] // Explicitly marking it as GET
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult TrainModelInfo()
        {
            return View();
        }

        [HttpPost]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult DeleteSystemInstruction(int id)
        {
            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);
            var serializedObject = HttpContext.Session.GetString("LoginInfo");
            try
            {
                string EmailId = string.Empty;
                LoggedInCustomerDetailsDTO loginInfo = JsonConvert.DeserializeObject<LoggedInCustomerDetailsDTO>(serializedObject);
                using (var db = new HrportalAiContext(_appSettingconnection))
                {

                    var existingInstruction = db.SystemInstructions.FirstOrDefault(x => x.Id == id);
                    if (existingInstruction == null)
                    {
                        return Json(new { success = false, message = "System Instruction not found." });
                    }


                    var oldRules = existingInstruction.Rules;
                    var oldIndustry = existingInstruction.Industry;


                    db.Database.OpenConnection();
                    var logCommand = db.Database.GetDbConnection().CreateCommand();
                    logCommand.CommandText = "EXEC sp_LogSystemInstructionChanges @SystemInstructionId, @ChangeType, @OldRules, @NewRules, @OldIndustry, @NewIndustry, @UserId";
                    logCommand.Parameters.Add(new SqlParameter("@SystemInstructionId", id));
                    logCommand.Parameters.Add(new SqlParameter("@ChangeType", "DELETE"));
                    logCommand.Parameters.Add(new SqlParameter("@OldRules", oldRules));
                    logCommand.Parameters.Add(new SqlParameter("@NewRules", DBNull.Value));
                    logCommand.Parameters.Add(new SqlParameter("@OldIndustry", oldIndustry));
                    logCommand.Parameters.Add(new SqlParameter("@NewIndustry", DBNull.Value));
                    logCommand.Parameters.Add(new SqlParameter("@UserId", loginInfo.EmailId));

                    logCommand.ExecuteNonQuery();
                    db.Database.CloseConnection();

                    if (existingInstruction.DefaultType == 1)
                    {
                        return Json(new { success = false, message = "This is Default System Instruction you can not delete it" });
                    }
                    else if (existingInstruction.DefaultType == 0 && existingInstruction.Status == 1)
                    {
                        admin.fnDeleteSystemInstructionInfo(id);
                        var Defaultinstruction = db.SystemInstructions.FirstOrDefault(x => x.DefaultType == 1);
                        SetActivatedInstruction(Defaultinstruction.Id);
                        return Json(new { success = true });
                    }
                    admin.fnDeleteSystemInstructionInfo(id);

                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error while deleting data: {ex.Message}" });
            }
        }


        [HttpPost]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult DeleteSuggestedQuestion(int id)
        {
            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);

            try
            {
                admin.fnDeleteSuggestedQuestionInfo(id);
                return Json(new { success = true });


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error while deleting data: {ex.Message}" });
            }
        }

        [HttpPost]
        [HolUser]
        [UserPrevent]
        [AdminPrevent]
        public IActionResult DeleteTrainModel(int id)
        {
            AdminService admin = new AdminService(_appSettingconnection, _httpContextAccessor);

            try
            {
                admin.fnDeleteTrainModelInfo(id);
                return Json(new { success = true });


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error while deleting data: {ex.Message}" });
            }
        }

        public IActionResult SetActivatedModels(List<int> ids)
        {
            using var context = new HrportalAiContext(_appSettingconnection);

            // Get all models
            var allModels = context.ModelDetails.ToList();

            // Update status for all models
            foreach (var model in allModels)
            {
                model.Status = ids.Contains(model.Id) ? 1 : 0;
            }

            context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult SetActivatedInstruction(int id)
        {
            using var context = new HrportalAiContext(_appSettingconnection);

            {
                var selectedInstruction = context.SystemInstructions.FirstOrDefault(m => m.Id == id);

                if (selectedInstruction != null)
                {

                    var allSystemInstruction = context.SystemInstructions.ToList();

                    foreach (var model in allSystemInstruction)
                    {
                        model.Status = (model.Id == id) ? 1 : 0;
                    }

                    context.SaveChanges();
                    ViewBag.LatestModelId = id;

                }

            }
            return Json(new { success = true });
        }


    }
}