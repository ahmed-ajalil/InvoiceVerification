using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

namespace CR_CoreBot.Helpers
{
	public class TranslatorTextHelper
	{
		string Endpoints = "";
		string ApiKeyRegion = "westus";
		private static HttpClient httpClient { get; set; }
		private static void InitializeTranslatorTextClient()
		{
			string ApiKey = "";
			if (!string.IsNullOrEmpty(ApiKey))
			{
				httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				httpClient.BaseAddress = new Uri("");
			}
		}
		public async Task<TranslateTextResult> GetTranslatedTextAsync(string input, string fromLanguage, string toLanguage = "en")
		{
			try
			{
				InitializeTranslatorTextClient();

				TranslateTextResult translationResult = new TranslateTextResult() { TranslatedText = string.Empty };

				if ((input != null) && (fromLanguage != null))
				{
					string requestString = "[{\"Text\":\"" + input + "\"}]";
					byte[] byteData = Encoding.UTF8.GetBytes(requestString);
					string uri = string.Format("/translate?api-version=3.0&from={0}&to={1}", fromLanguage, toLanguage);
					var response = await CallEndpoint(httpClient, uri, byteData);
					string content = await response.Content.ReadAsStringAsync();
					content = content.TrimStart('[');
					content = content.TrimEnd(']');
					dynamic data = JObject.Parse(content);

					string translation = string.Empty;
					if (data.translations != null)
					{
						translation = (string)data.translations[0].text;
					}

					if (data.error != null)
					{
						translation = "<Error: unable to translate input text>";
					}

					translationResult = new TranslateTextResult { TranslatedText = translation };
				}

				return translationResult;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		static async Task<HttpResponseMessage> CallEndpoint(HttpClient client, string uri, byte[] byteData)
		{
			using (var content = new ByteArrayContent(byteData))
			{
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				return await client.PostAsync(uri, content);
			}
		}

	}
}
