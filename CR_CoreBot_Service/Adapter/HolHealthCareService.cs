using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.AI.FormRecognizer.Models;
using Azure.AI.FormRecognizer;
using Azure.AI.OpenAI;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure;
using CR_CoreBot_DataAccess.CR_OpenAI;
using CR_CoreBot_DTO;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_Service.Adapter
{
    public class HolHealthCareService
    {
        public async Task<string> ReadFile(string? fileName)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient("your storage connection string");
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("test");

                // Create a BlobClient object
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                // Download the file contents
                var response = await blobClient.DownloadAsync();
                Stream stream = response.Value.Content;

                // Read the file contents into a string
                using (var reader = new StreamReader(stream))
                {
                    string fileContents = await reader.ReadToEndAsync();
                    return fileContents;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Doc Processing
        public DataBotDTO ConnectBlob(DataBotDTO objDbMOdel)
        {
            try
            {
                string storageAccountName = "storage Account name";
                string containerName = "test";
                string sasToken = "";
                StorageCredentials creds;
                CloudBlobContainer cloudBlobContainer;
                creds = new StorageCredentials(sasToken);

                cloudBlobContainer = new CloudBlobContainer(new Uri("https://" + storageAccountName + ".blob.core.windows.net/" + containerName), creds);
                BlobContinuationToken blobContinuationToken = null;
                var blobs = cloudBlobContainer.ListBlobsSegmentedAsync("", blobContinuationToken);
                var blob = blobs.Result;
                objDbMOdel.FileName = new List<string>();
                objDbMOdel.FilePath = new List<string>();
                objDbMOdel.FileCount = blobs.Result.Results.Count();
                return objDbMOdel;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<DataBotDTO> TextEntityExtraction(DataBotDTO objModel, string key, string ModelName, string Endpoint)
        {
            DataBotDTO obj = new DataBotDTO();
            try
            {
                string document = string.Empty;
                if (!string.IsNullOrEmpty(objModel.PropmpInputExtract))
                {
                    List<string> extractedEntities = new List<string>();
                    document = await ReadFileText(objModel);
                    var vals = objModel.PropmpInputExtract.Split("\n");
                    var prompts = $"Extract the following entities from the given text OPD file:";
                    foreach (var item in vals)
                    {
                        if (item != "" && item != null)
                        {
                            prompts += item + ",";
                        }
                    }
                    obj.FileText = document;
                    document = $"Text OPD file: {document}";
                    obj.CompletionResult = fnAzureOpenAI(document, prompts, key, ModelName, Endpoint);
                    string dig = ICDDiagnosis(obj.CompletionResult);
                    obj.diagnosis = "Diagnosis: " + fnAzureOpenAIDignosis(dig.Trim(), "You are a helpful assistant provide response based on ICD-10 International Classification of Diseases standards.",
                                                "As a system, provide full forms and include the associated ICD codes for medical terms.", key, ModelName, Endpoint);
                    obj.SummeryResult = fnAzureOpenAI(document, "Provide a summary of the text below that captures its main idea", key, ModelName, Endpoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return obj;
        }
        public DataBotDTO Classify(DataBotDTO objModel, string key, string ModelName, string Endpoint)
        {
            try
            {
                string classifyPrompt = "Classify the following Text in detail";
                DataBotDTO obj = new DataBotDTO();
                string? document = string.Empty;
                if (objModel.DataSource == "manual")
                {
                    document = objModel.ExtractData;
                }
                else
                {
                    document = objModel.PropmpInputExtract;
                }
                obj.CompletionResult = fnAzureOpenAI(document, classifyPrompt, key, ModelName, Endpoint);
                if (string.IsNullOrEmpty(obj.CompletionResult))
                {
                    obj.CompletionResult = "Service Unavailable.";
                }
                obj.FileText = objModel.PropmpInputExtract;
                return obj;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<DataBotDTO> SentimentAnalysis(DataBotDTO objModel, string key, string ModelName, string Endpoint)
        {
            DataBotDTO obj = new DataBotDTO();
            try
            {
                string document = string.Empty;
                if (!string.IsNullOrEmpty(objModel.PropmpInputExtract))
                {
                    if (objModel.DataSource == "manual")
                    {
                        document = objModel.ExtractData;
                    }
                    else
                    {
                        document = await ReadFileText(objModel);
                    }
                    obj.PositiveSentiment = fnAzureOpenAI(document, "Positive Sentiment ", key, ModelName, Endpoint);
                    obj.NegativeSentiment = fnAzureOpenAI(document, "Negative Sentiment ", key, ModelName, Endpoint);
                    obj.NuturalSentiment = fnAzureOpenAI(document, "Nutural Sentiment ", key, ModelName, Endpoint);
                }

                obj.FileText = document;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return obj;
        }


        #endregion

        #region ImageToText
        public async Task<string> ImageToTextConverter(string path)
        {
            string filePathDataWrite = string.Empty;
            string newpath = "wwwroot" + path;
            string fileName = Path.GetFileNameWithoutExtension(path);
            string textFilePath = "wwwroot\\TxtFiles\\";
            string endpoint = "";
            string key = "";

            var credential = new AzureKeyCredential(key);
            var client = new DocumentAnalysisClient(new Uri(endpoint), credential);

            var filePath = newpath;
            filePathDataWrite = textFilePath + fileName + ".txt";
            if (File.Exists(filePathDataWrite))
            {
                File.Delete(filePathDataWrite);
            }
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            try
            {
                AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-document", stream);
                await operation.WaitForCompletionAsync();

                AnalyzeResult result = operation.Value;

                var sb = new StringBuilder();
                foreach (var page in result.Pages)
                {
                    foreach (var line in page.Lines)
                    {
                        sb.AppendLine(line.Content);
                    }
                }
                using (StreamWriter sw = new StreamWriter(filePathDataWrite))
                {
                    sw.WriteLine(sb.ToString());
                }
            }
            catch (RequestFailedException ex)
            {
            }
            finally
            {
                stream.Dispose();
            }
            return filePathDataWrite;
        }
        public async Task<string> ReadFileText(DataBotDTO objModel)
        {
            try
            {

                string fileextension = Path.GetExtension(objModel.TextFilePath);
                string fileName = Path.GetFileName(objModel.TextFilePath);
                string document = string.Empty;
                string textFilePath = objModel.TextFilePath;
                if (fileextension == ".pdf")
                {
                    document = await PdfProcessing($"https://xxxxxx.blob.core.windows.net/test-pdf/{fileName}");
                }
                else if (fileextension == ".docx")
                {
                    document = await docxProcessing($"https://xxxxxx.blob.core.windows.net/test-pdf/{fileName}");
                }
                else if (fileextension == ".png" || fileextension == ".jpg" || fileextension == ".jpeg")
                {
                    document = await PdfProcessing($"https://xxxxxxx.blob.core.windows.net/test-pdf/{fileName}");
                }
                else
                {
                    document = File.ReadAllText(textFilePath);
                }
                return document;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<string> PdfProcessing(string path)
        {
            string pageText = "";
            try
            {
                string endpoint = "https://xxxx.cognitiveservices.azure.com/";
                string apiKey = "xxxxx";
                var credential = new AzureKeyCredential(apiKey);
                var client = new FormRecognizerClient(new Uri(endpoint), credential);
                Uri pdfUri = new Uri(path);
                FormPageCollection formPages = await client.StartRecognizeContentFromUriAsync(new Uri(pdfUri.ToString())).WaitForCompletionAsync();

                foreach (FormPage page in formPages)
                {
                    pageText += page.Lines?.Aggregate("", (current, line) => current + " " + line.Text) + "\r\n";

                }
            }
            catch (Exception ex)
            {
                pageText = ex.Message;
            }
            return pageText;
        }
        #endregion
        #region OpenAIChatGPT
        public async Task<string> docxProcessing(string path)
        {
            string pageText = "";
            try
            {
                using (var client = new WebClient())
                {
                    byte[] data = client.DownloadData(path);

                    using (var document = WordprocessingDocument.Open(new MemoryStream(data), false))
                    {
                        var mainPart = document.MainDocumentPart;

                        pageText = mainPart.Document.Body.InnerText;
                    }
                }

            }
            catch (Exception ex)
            {
                pageText = ex.Message;
            }
            return pageText;
        }
        #endregion
   

        public void SaveBulkData(DataBotDTO objModel, string filename)
        {
            List<EntityExtractrion> entityext = new List<EntityExtractrion>();
            DataBotDTO objdModel1 = new DataBotDTO();
            try
            {
                HolEntityDTO model = new HolEntityDTO();
                string[] lines = objModel.CompletionResult.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                string date = "";
                string medical_history = "";
                string body_examination = "";
                string tests_done = "";
                string medicines_prescribed = "";
                string diagnosis = "";

                CropenAiContext objentity = new CropenAiContext();

                objentity.EntityExtractrions.Where(x => x.FileName == filename).ExecuteDelete();
                objentity.SaveChanges();
                foreach (string line in lines)
                {
                    EntityExtractrion objextract = new EntityExtractrion();
                    if (line.Contains("Date:"))
                    {
                        date = line.Substring(line.IndexOf(":") + 1).Trim();
                        objextract.Entity = "Date";
                        objextract.EntityDescription = date;
                        objextract.FileName = filename;
                        objextract.ImportantNotes = "No";
                        entityext.Add(objextract);
                    }
                    else if (line.Contains("Medical History:"))
                    {
                        medical_history = line.Substring(line.IndexOf(":") + 1).Trim();
                        objextract.Entity = "Medical History";
                        objextract.EntityDescription = medical_history;
                        objextract.FileName = filename;
                        objextract.ImportantNotes = "No";
                        entityext.Add(objextract);
                    }
                    else if (line.Contains("Body Examination:"))
                    {
                        body_examination = line.Substring(line.IndexOf(":") + 1).Trim();
                        objextract.Entity = "Body Examination";
                        objextract.EntityDescription = body_examination;
                        objextract.FileName = filename;
                        objextract.ImportantNotes = "No";
                        entityext.Add(objextract);

                    }
                    else if (line.Contains("Tests done:"))
                    {
                        tests_done = line.Substring(line.IndexOf(":") + 1).Trim();
                        objextract.Entity = "Tests done";
                        objextract.EntityDescription = tests_done;
                        objextract.FileName = filename;
                        objextract.ImportantNotes = "No";
                        entityext.Add(objextract);

                    }
                    else if (line.Contains("Medicines Prescribed"))
                    {

                        medicines_prescribed = line.Substring(line.IndexOf(":") + 1).Trim();
                        objextract.Entity = "Medicines Prescribed";
                        objextract.EntityDescription = medicines_prescribed;
                        objextract.FileName = filename;
                        objextract.ImportantNotes = "No";
                        entityext.Add(objextract);
                    }
                    else if (line.Contains("Impression"))
                    {

                        medicines_prescribed = line.Substring(line.IndexOf(":") + 1).Trim();
                        objextract.Entity = "Impression";
                        objextract.EntityDescription = medicines_prescribed;
                        objextract.FileName = filename;
                        objextract.ImportantNotes = "yes";
                        entityext.Add(objextract);
                    }
                    else if (line.Contains("Diagnosis"))
                    {

                        diagnosis = line.Substring(line.IndexOf(":") + 1).Trim();
                        objextract.Entity = "Diagnosis";
                        objextract.EntityDescription = diagnosis;
                        objextract.FileName = filename;
                        objextract.ImportantNotes = "yes";
                        entityext.Add(objextract);
                    }
                    else if (line.Contains("Plan"))
                    {

                        diagnosis = line.Substring(line.IndexOf(":") + 1).Trim();
                        objextract.Entity = "Plan";
                        objextract.EntityDescription = diagnosis;
                        objextract.FileName = filename;
                        objextract.ImportantNotes = "No";
                        entityext.Add(objextract);
                    }
                    else if (line.Contains("Management"))
                    {

                        diagnosis = line.Substring(line.IndexOf(":") + 1).Trim();
                        objextract.Entity = "Management";
                        objextract.EntityDescription = diagnosis;
                        objextract.FileName = filename;
                        objextract.ImportantNotes = "No";
                        entityext.Add(objextract);
                    }
                }
                objentity.EntityExtractrions.AddRange(entityext);
                objentity.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<DataBotDTO> TextEntityExtractionEGC(DataBotDTO objModel, string key, string ModelName, string Endpoint)
        {
            DataBotDTO obj = new DataBotDTO();
            try
            {
                if (!string.IsNullOrEmpty(objModel.PropmpInputExtract))
                {
                    string ECGExtractionPrompt = "Please analyse and summarize the ECG report and read all the readings from graphs then tell that patient have any risk of heart related diseases or not:\n";
                    string document = string.Empty;
                    if (objModel.DataSource == "manual")
                    {
                        document = objModel.ExtractData;
                    }
                    else
                    {
                        document = await ReadFileText(objModel);
                    }
                    obj.SummeryResult = fnAzureOpenAI(document, ECGExtractionPrompt, key, ModelName, Endpoint);
                    if (string.IsNullOrEmpty(obj.SummeryResult))
                    {
                        obj.SummeryResult = "Service Unavailable.";
                    }
                    obj.FileText = document;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return obj;
        }
        public static async Task<string> NewGpt4(string Prompt, string Key, string Endpoint, string ModelName)
        {
            // Set your OpenAI API key
            string apiKey = Key;

            // Set the API endpoint and model
            string apiUrl = Endpoint;
            string model = ModelName;

            // Create the HTTP client
            using (HttpClient client = new HttpClient())
            {
                // Set the request headers
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Get user input for the prompt
                Console.WriteLine("Enter your prompt:");
                string prompt = Prompt.Trim();
                //string prompt = "Extract the following entities from the given text OPD file: Date, Medical History, Body Examination, Tests done, Medicines Prescribed, Diagnosis, Plan, Management,,Text OPD file:  Outpatient Note (Follow up) Date: 04/11/2022 Chief complaint: FOLLOW UP History of present illness: OCCASIONAL NECK PAIN New allergy: [X ] No [ ] Yes .... Physical examination: - Vital signs: T: ℃. Pulse: /min. R: /min. BP:100 /70 mmHg. Height: cm. Weight: kg. - Significant findings: IMPRESSION 1. Co-dominance. 2. Mild coronary calcification. Total calcium score (AJ 130) = 55.3. 3. An eccentric calcified plaque at ostial LAD causing moderate luminal obscuration, likely no significant stenosis. 4. The remainder of CTCA shows no significant stenosis or obstructive coronary artery disease. 5. No coronary artery anomaly is depicted. 6. LVEF is 71 % 7. Included thoracic aorta shows no aneurysm or dissection. Impression or diagnosis: ESSENTIAL HYPERTENSION T2DM DISLIPIDEMIA Plan and management: CONTINUE REGULAR MEDICATION ADVISED LOW DOSE ASPIRIN I have discussed diagnosis and plan with patient/family; who agreed and voiced understanding. Diagnosis: 1. Hypertension 2. DM Type 2 3. Dyslipidemia";

                // Set the maximum number of tokens in the response
                int maxTokens = 2000;

                // Create the request body
                string requestBody = $@"{{
                ""model"": ""{model}"",
                ""messages"": [
                    {{ ""role"": ""system"", ""content"": ""You are a helpful assistant provide response based on ICD-10 International Classification of Diseases standards."" }},
                    {{ ""role"": ""system"", ""content"": ""As a system, provide full forms and include the associated ICD codes for medical terms."" }},
                    {{ ""role"": ""user"", ""content"": ""{prompt}"" }}
                ],
                ""max_tokens"": {maxTokens}
            }}";
                HttpResponseMessage response = await client.PostAsync(apiUrl, new StringContent(requestBody, Encoding.UTF8, "application/json"));

                string responseContent = await response.Content.ReadAsStringAsync();
                JObject responseObject = JObject.Parse(responseContent);
                string answer = responseObject["choices"][0]["message"]["content"].ToString();
                return "Diagnosis: " + answer.Trim();
            }
        }

        public string ICDDiagnosis(string Entities)
        {
            List<EntityExtractrion> entityext = new List<EntityExtractrion>();
            DataBotDTO objdModel1 = new DataBotDTO();
            try
            {
                HolEntityDTO model = new HolEntityDTO();
                string[] lines = Entities.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                string diagnosis = "";

                foreach (string line in lines)
                {
                    if (line.Contains("Diagnosis"))
                    {
                        return diagnosis = line.Substring(line.IndexOf(":") + 1).Trim();
                    }
                }
                return diagnosis;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string fnAzureOpenAI(string document, string prompt, string key, string ModelName, string endpoint)
        {
            try
            {
                OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    Messages = { new ChatMessage(ChatRole.System, document.Trim()), new ChatMessage(ChatRole.User, prompt.Trim()), },
                    MaxTokens = 500,
                    Temperature = Convert.ToSingle(0.7),
                };
                Response<ChatCompletions> response = client.GetChatCompletions(
                    deploymentOrModelName: ModelName,
                    chatCompletionsOptions);

                return response.Value.Choices[0].Message.Content;
            }
            catch (Exception ex)
            {
                return " " + ex.Message;
            }

        }
        public static string fnAzureOpenAIDignosis(string document, string prompt, string prompt1, string key, string ModelName, string endpoint)
        {
            try
            {
                OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    Messages ={ new ChatMessage(ChatRole.System,prompt),new ChatMessage(ChatRole.System, prompt1),
                        new ChatMessage(ChatRole.User, document),
                        },
                    MaxTokens = 500,
                    Temperature = Convert.ToSingle(0.7),
                };
                Response<ChatCompletions> response = client.GetChatCompletions(
                    deploymentOrModelName: ModelName,
                    chatCompletionsOptions);

                return response.Value.Choices[0].Message.Content;
            }
            catch (Exception ex)
            {
                return " " + ex.Message;
            }
        }
        public DataTable GetProductsDetail(string _FileName)
        {
            try
            {
                CropenAiContext objentity = new CropenAiContext();
                var products = objentity.EntityExtractrions.Where(x => x.FileName == _FileName).ToList();
                DataTable dtProduct = new DataTable("EntityExtractrions");
                dtProduct.Columns.AddRange(new DataColumn[2] {
                                            new DataColumn("Entity"),
                                            new DataColumn("EntityDescription") });
                foreach (var product in products)
                {
                    dtProduct.Rows.Add(product.Entity, product.EntityDescription);
                }

                return dtProduct;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public DataTable GetsummaryDetail(string _FileName)
        {
            CropenAiContext objentity = new CropenAiContext();
            var products = objentity.TblSummaryExtractrions.Where(x => x.FileName == _FileName)
                .ToList();

            DataTable dtProduct = new DataTable("SummaryExtractrions");
            dtProduct.Columns.AddRange(new DataColumn[2] {
                                            new DataColumn("SummaryDescription"),
                                            new DataColumn("FileName") });
            foreach (var product in products)
            {
                dtProduct.Rows.Add(product.SummaryDescription, product.FileName);
            }

            return dtProduct;
        }
        public string fnInsertBlobdata(BlobConnection blobConnection, string LoginUser)
        {
            try
            {
                CropenAiContext cropenAiContext = new CropenAiContext();
                CustomerInformation? customer = cropenAiContext.CustomerInformations.Where(x => x.UserName == LoginUser).FirstOrDefault();
                if (customer != null)
                {
                    BlobConnection? blobConnection1 = cropenAiContext.BlobConnections.Where(x => x.UserName == LoginUser).FirstOrDefault();
                    if (blobConnection1 != null)
                    {
                        BlobConnection blob = new BlobConnection();
                        blob.Id = blobConnection1.Id;
                        blob.CustomerId = customer.CustomerId;
                        blob.UserName = blobConnection1.UserName;
                        blob.AccountName = blobConnection.AccountName;
                        blob.AccountKey = blobConnection.AccountKey;
                        blob.BlobContainerName = blobConnection.BlobContainerName;
                        blob.BlobName = blobConnection1.BlobName;
                        blob.EndpointSuffix = blobConnection1.EndpointSuffix;
                        blob.ConnectionString = blobConnection.ConnectionString;
                        blob.StorageType = blobConnection.StorageType;
                        blob.DateTime = DateTime.Now;
                        using (var ctx = new CropenAiContext())
                        {
                            ctx.Entry(blob).State = EntityState.Modified;
                            ctx.SaveChanges();
                        }
                        return "true";
                    }
                    else
                    {
                        BlobConnection blob = new BlobConnection();

                        blob.CustomerId = customer.CustomerId;
                        blob.UserName = LoginUser;
                        blob.AccountName = blobConnection.AccountName;
                        blob.AccountKey = blobConnection.AccountKey;
                        blob.BlobContainerName = blobConnection.BlobContainerName;
                        blob.BlobName = blobConnection.BlobName;
                        blob.EndpointSuffix = blobConnection.EndpointSuffix;
                        blob.ConnectionString = blobConnection.ConnectionString;
                        blob.StorageType = blobConnection.StorageType;
                        blob.DateTime = DateTime.Now;
                        cropenAiContext.BlobConnections.Add(blob);
                        cropenAiContext.SaveChanges();
                        return "true";
                    }
                }
                else
                {
                    return "Account Doesn't exist";
                }

            }
            catch (Exception ex)
            {
                return "false";
            }
        }
        public void delete_UserFileDetails(string filename)
        {
            try
            {
                CropenAiContext objentity = new CropenAiContext();
                objentity.TblCustomerFileDetails.Where(x => x.FileName == filename).ExecuteDelete();
                objentity.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public void saveData_EntityExtraction(DataBotDTO objModel, string _FileName, int Id)
        {
            try
            {
                List<EntityExtractrion> entityext = new List<EntityExtractrion>();
                string[] lines = objModel.ExtractData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                string patient_name = "";
                string age = "";
                string sex = "";
                string doctor_name = "";
                string date = "";
                string medical_history = "";
                string body_examination = "";
                string tests_done = "";
                string medicines_prescribed = "";
                string diagnosis = "";
                string Management = "";
                string Plan = "";

                CropenAiContext objentity = new CropenAiContext();

                objentity.EntityExtractrions.Where(x => x.FileName == _FileName).ExecuteDelete();
                objentity.SaveChanges();
                for (int i = 0; i < lines.Count(); i++)
                {
                    EntityExtractrion objextract = new EntityExtractrion();
                    if (lines[i].ToLower().Contains("patient name:"))
                    {
                        patient_name = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (patient_name == "" || patient_name == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                if (lines[j].ToLower().Contains("age:"))
                                {
                                    i = j - 1; break;
                                }
                                else
                                {
                                    patient_name += lines[j].Trim() + " ";
                                }
                            }
                        }
                        objextract.Entity = "Patient Name";
                        objextract.EntityDescription = patient_name;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "No";
                        entityext.Add(objextract);
                    }
                    else if (lines[i].ToLower().Contains("age:"))
                    {
                        age = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (age == "" || age == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                if (lines[j].ToLower().Contains("sex:"))
                                {
                                    i = j - 1; break;
                                }
                                else
                                {
                                    age += lines[j].Trim() + " ";
                                }
                            }
                        }
                        objextract.Entity = "Age";
                        objextract.EntityDescription = age;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "No";
                        entityext.Add(objextract);
                    }
                    else if (lines[i].ToLower().Contains("sex:"))
                    {
                        sex = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (sex == "" || sex == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                if (lines[j].ToLower().Contains("doctor name:"))
                                {
                                    i = j - 1; break;
                                }
                                else
                                {
                                    sex += lines[j].Trim() + " ";
                                }
                            }
                        }
                        objextract.Entity = "Sex";
                        objextract.EntityDescription = sex;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "No";
                        entityext.Add(objextract);
                    }
                    else if (lines[i].ToLower().Contains("doctor name:"))
                    {
                        doctor_name = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (doctor_name == "" || doctor_name == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                if (lines[j].ToLower().Contains("date:"))
                                {
                                    i = j - 1; break;
                                }
                                else
                                {
                                    doctor_name += lines[j].Trim() + " ";
                                }
                            }
                        }
                        objextract.Entity = "Doctor Name";
                        objextract.EntityDescription = doctor_name;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "No";
                        entityext.Add(objextract);
                    }
                    else if (lines[i].ToLower().Contains("date:"))
                    {
                        date = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (date == "" || date == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                if (lines[j].ToLower().Contains("medical history:"))
                                {
                                    i = j - 1; break;
                                }
                                else
                                {
                                    date += lines[j].Trim() + " ";
                                }
                            }
                        }
                        objextract.Entity = "Date";
                        objextract.EntityDescription = date;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "No";
                        objextract.FileId = Id;
                        objextract.CreatedDate = DateTime.Now;
                        entityext.Add(objextract);
                    }
                    else if (lines[i].ToLower().Contains("medical history:"))
                    {
                        medical_history = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (medical_history == "" || medical_history == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                if (lines[j].ToLower().Contains("body examination:"))
                                {
                                    i = j - 1; break;
                                }
                                else
                                {
                                    medical_history += lines[j].Trim() + " ";
                                }
                            }
                        }
                        objextract.Entity = "Medical History";
                        objextract.EntityDescription = medical_history;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "No";
                        objextract.FileId = Id;
                        objextract.CreatedDate = DateTime.Now;
                        entityext.Add(objextract);
                    }
                    else if (lines[i].ToLower().Contains("body examination:"))
                    {
                        body_examination = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (body_examination == "" || body_examination == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                if (lines[j].ToLower().Contains("tests done:"))
                                {
                                    i = j - 1; break;
                                }
                                else
                                {
                                    body_examination += lines[j].Trim() + " ";
                                }
                            }
                        }
                        objextract.Entity = "Body Examination";
                        objextract.EntityDescription = body_examination;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "No";
                        objextract.FileId = Id;
                        objextract.CreatedDate = DateTime.Now;
                        entityext.Add(objextract);

                    }
                    else if (lines[i].ToLower().Contains("tests done:"))
                    {
                        tests_done = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (tests_done == "" || tests_done == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                if (lines[j].ToLower().Contains("medicines prescribed"))
                                {
                                    i = j - 1; break;
                                }
                                else
                                {
                                    tests_done += lines[j].Trim() + " ";
                                }
                            }
                        }
                        objextract.Entity = "Tests done";
                        objextract.EntityDescription = tests_done;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "No";
                        objextract.FileId = Id;
                        objextract.CreatedDate = DateTime.Now;
                        entityext.Add(objextract);

                    }
                    else if (lines[i].ToLower().Contains("medicines prescribed"))
                    {

                        medicines_prescribed = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (medicines_prescribed == "" || medicines_prescribed == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                if (lines[j].ToLower().Contains("diagnosis"))
                                {
                                    i = j - 1; break;
                                }
                                else
                                {
                                    medicines_prescribed += lines[j].Trim() + " ";
                                }
                            }
                        }
                        objextract.Entity = "Medicines Prescribed";
                        objextract.EntityDescription = medicines_prescribed;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "No";
                        objextract.FileId = Id;
                        objextract.CreatedDate = DateTime.Now;
                        entityext.Add(objextract);
                    }
                    else if (lines[i].ToLower().Contains("diagnosis"))
                    {
                        diagnosis = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (diagnosis == "" || diagnosis == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                if (lines[j].ToLower().Contains("plan"))
                                {
                                    i = j - 1; break;
                                }
                                else
                                {
                                    diagnosis += lines[j].Trim() + " ";
                                }
                            }
                        }
                        objextract.Entity = "Diagnosis";
                        objextract.EntityDescription = diagnosis;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "yes";
                        objextract.FileId = Id;
                        objextract.CreatedDate = DateTime.Now;
                        entityext.Add(objextract);
                    }
                    else if (lines[i].ToLower().Contains("plan"))
                    {
                        Plan = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (Plan == "" || Plan == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                if (lines[j].ToLower().Contains("management"))
                                {
                                    i = j - 1; break;
                                }
                                else
                                {
                                    Plan += lines[j].Trim() + " ";
                                }
                            }
                        }
                        objextract.Entity = "Plan";
                        objextract.EntityDescription = Plan;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "No";
                        objextract.FileId = Id;
                        objextract.CreatedDate = DateTime.Now;
                        entityext.Add(objextract);
                    }
                    else if (lines[i].ToLower().Contains("management"))
                    {
                        Management = lines[i].Substring(lines[i].IndexOf(":") + 1).Trim();
                        if (Management == "" || Management == null)
                        {
                            for (int j = i + 1; j < lines.Count(); j++)
                            {
                                Management += lines[j].Trim() + " ";
                            }
                        }
                        objextract.Entity = "Management";
                        objextract.EntityDescription = Management;
                        objextract.FileName = _FileName;
                        objextract.ImportantNotes = "No";
                        objextract.FileId = Id;
                        objextract.CreatedDate = DateTime.Now;
                        entityext.Add(objextract);
                    }
                }
                objentity.EntityExtractrions.AddRange(entityext);
                objentity.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public void SaveData_SummaryExtractrions(List<TblSummaryExtractrion> summaryExtractrions)
        {
            try
            {
                CropenAiContext objentity = new CropenAiContext();
                objentity.TblSummaryExtractrions.AddRange(summaryExtractrions);
                objentity.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public void delete_SummaryExtractrions(string filename)
        {
            try
            {
                CropenAiContext objentity = new CropenAiContext();
                objentity.TblSummaryExtractrions.Where(x => x.FileName == filename).ExecuteDelete();
                objentity.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public bool SaveData_UserFileDetails(List<TblCustomerFileDetail> customerFileDetails)
        {
            try
            {
                CropenAiContext objentity = new CropenAiContext();
                objentity.TblCustomerFileDetails.AddRange(customerFileDetails);
                objentity.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool IsImageExtension(string extension)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif" }; // Add more extensions as needed

            return imageExtensions.Contains(extension);
        }

        public async Task<List<BlobConnectedListDTO>> ConnectedToBlob(string accountName, string accountKey, string containerName)
        {
            try
            {
                List<string> tableData;
                int? isprocessed;
                using (var dbContext = new CropenAiContext())
                {
                    tableData = dbContext.TblCustomerFileDetails.Select(item => item.FileName).ToList();
                }
                List<BlobConnectedListDTO> blobConnectedListModel = new List<BlobConnectedListDTO>();
                BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=" + accountName + ";AccountKey=" + accountKey + ";EndpointSuffix=core.windows.net");
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    string extension = Path.GetExtension(blobItem.Name).ToLower();
                    if (extension == ".pdf" || IsImageExtension(extension))
                    {
                        using (var dbContext = new CropenAiContext())
                        {
                            isprocessed = dbContext.TblCustomerFileDetails.Where(x => x.FileName == blobItem.Name).Select(item => item.ProcessStatus).FirstOrDefault();
                        }
                        BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                        if (tableData.Contains(blobItem.Name))
                        {
                            BlobConnectedListDTO BlobConnected = new BlobConnectedListDTO
                            {

                                Filename = blobItem.Name.ToString(),
                                ContentType = blobItem.Properties.ContentType,
                                DateTime = DateTime.Now,
                                DownloadUrl = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTime.UtcNow.AddMinutes(5)).ToString(),
                                AbsoluteUri = blobClient.Uri.AbsoluteUri,
                                IsProcessed = isprocessed == 0 ? "Not Processed" : "Processed"
                            };
                            blobConnectedListModel.Add(BlobConnected);
                        }
                        else
                        {
                            BlobConnectedListDTO BlobConnected = new BlobConnectedListDTO
                            {

                                Filename = blobItem.Name.ToString(),
                                ContentType = blobItem.Properties.ContentType,
                                DateTime = DateTime.Now,
                                DownloadUrl = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTime.UtcNow.AddMinutes(5)).ToString(),
                                AbsoluteUri = blobClient.Uri.AbsoluteUri,
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

        public bool checkFileIsAvailable(string _FileName)
        {
            try
            {
                CropenAiContext objentity = new CropenAiContext();
                var customer = objentity.TblCustomerFileDetails.FirstOrDefault(x => x.FileName == _FileName);
                if (customer != null)
                {
                    return true;
                }
                else { return false; }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
