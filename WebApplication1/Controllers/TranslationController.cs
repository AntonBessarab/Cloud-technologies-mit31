using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class TranslationController : Controller
    {
        private readonly string subscriptionKey = "";
        private readonly string region = "";

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string inputText, string targetLanguage)
        {
            if (string.IsNullOrEmpty(inputText) || string.IsNullOrEmpty(targetLanguage))
            {
                ViewBag.Error = "Будь ласка, заповніть обидва поля.";
                return View();
            }

            var client = new HttpClient();
            var url = $"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to={targetLanguage}";

            var requestBody = new[]
            {
        new { Text = inputText }
    };

            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", region);

            try
            {
                var response = await client.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var translationResult = JsonSerializer.Deserialize<TranslationResult[]>(responseBody, options);
                var translatedText = translationResult[0].Translations[0].Text;

                ViewBag.TranslatedText = translatedText;
                ViewData["InputText"] = inputText;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Помилка під час перекладу: " + ex.Message;
            }

            return View();
        }


        private class TranslationResult
        {
            public Translation[] Translations { get; set; }
        }

        private class Translation
        {
            public string Text { get; set; }
            public string To { get; set; }
        }
    }
}
