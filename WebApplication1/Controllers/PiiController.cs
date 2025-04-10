// Controllers/PiiController.cs
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebApplication1.Controllers
{
    public class PiiController : Controller
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
        public IActionResult Index(string inputText)
        {
            var response = client.RecognizePiiEntities(inputText);
            var redacted = response.Value.RedactedText;

            ViewBag.OriginalText = inputText;
            ViewBag.RedactedText = redacted;

            return View();
        }
    }
}
