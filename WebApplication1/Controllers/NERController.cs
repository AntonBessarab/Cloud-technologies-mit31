using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class NERController : Controller
    {
        private static readonly string languageKey = Environment.GetEnvironmentVariable("LANGUAGE_KEY");
        private static readonly Uri endpoint = new Uri(Environment.GetEnvironmentVariable("LANGUAGE_ENDPOINT"));
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential(languageKey);
        private static readonly TextAnalyticsClient client = new TextAnalyticsClient(endpoint, credentials);

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Result(string inputText)
        {
            var response = client.RecognizeEntities(inputText);
            var result = new List<NamedEntityModel>();

            foreach (var entity in response.Value)
            {
                result.Add(new NamedEntityModel
                {
                    Text = entity.Text,
                    Category = entity.Category.ToString(),
                    SubCategory = entity.SubCategory?.ToString(),
                    ConfidenceScore = entity.ConfidenceScore
                });

            }

            return View(result);
        }
    }
}
