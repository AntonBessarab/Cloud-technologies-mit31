using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string languageKey = Environment.GetEnvironmentVariable("LANGUAGE_KEY");
        private static readonly string languageEndpoint = Environment.GetEnvironmentVariable("LANGUAGE_ENDPOINT");
        private static readonly Uri endpoint = new Uri(languageEndpoint);
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential(languageKey);
        private static readonly TextAnalyticsClient client = new TextAnalyticsClient(endpoint, credentials);

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Result(string inputText)
        {
            var response = client.RecognizeLinkedEntities(inputText);
            var result = new List<EntityModel>();

            foreach (var entity in response.Value)
            {
                foreach (var match in entity.Matches)
                {
                    result.Add(new EntityModel
                    {
                        Name = entity.Name,
                        DataSource = entity.DataSource,
                        Url = entity.Url.ToString()
                    });
                }
            }

            return View(result);
        }
    }
}