using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System;
using System.Collections.Generic;
using WebApplication1.Models;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class HealthController : Controller
    {
        private static readonly string languageKey = Environment.GetEnvironmentVariable("LANGUAGE_KEY");
        private static readonly Uri endpoint = new Uri(Environment.GetEnvironmentVariable("LANGUAGE_ENDPOINT"));
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential(languageKey);
        private static readonly TextAnalyticsClient client = new TextAnalyticsClient(endpoint, credentials);

        public IActionResult Index()
        {
            return View();
        }

        // Змінили метод на асинхронний
        [HttpPost]
        public async Task<IActionResult> Result(string inputText)
        {
            // Використовуємо await для асинхронного виклику
            var healthResult = await client.AnalyzeHealthcareEntitiesAsync(inputText);

            var entities = new List<EntityModel>();

            // Перебір результатів через асинхронний метод
            foreach (var entity in healthResult.Value.Entities)
            {
                entities.Add(new HealthModel
                {
                    Text = entity.Text,
                    Category = entity.Category.ToString(),
                    SubCategory = entity.SubCategory?.ToString(),
                    ConfidenceScore = entity.ConfidenceScore
                });
            }

            return View(entities);
        }
    }
}