using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class HealthController : Controller
    {
        private static readonly string key = Environment.GetEnvironmentVariable("LANGUAGE_KEY");
        private static readonly string endpointUrl = Environment.GetEnvironmentVariable("LANGUAGE_ENDPOINT");
        private static readonly TextAnalyticsClient client = new TextAnalyticsClient(new Uri(endpointUrl), new AzureKeyCredential(key));

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string inputText)
        {
            var documents = new List<string> { inputText };
            var operation = await client.AnalyzeHealthcareEntitiesAsync(WaitUntil.Completed, documents);
            var results = operation.Value;

            List<HealthcareEntity> entities = new List<HealthcareEntity>();

            await foreach (AnalyzeHealthcareEntitiesResultCollection page in results)
            {
                foreach (AnalyzeHealthcareEntitiesResult result in page)
                {
                    if (!result.HasError)
                    {
                        entities.AddRange(result.Entities);
                    }
                }
            }

            ViewBag.Text = inputText;
            ViewBag.Entities = entities;

            return View();
        }
    }
}
