using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    public class OcrController : Controller
    {
        string visionKey = "";
        string visionEndpoint = "";

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                ViewBag.Error = "Please upload an image.";
                return View();
            }

            var client = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(visionKey))
            {
                Endpoint = visionEndpoint
            };

            using var stream = image.OpenReadStream();

            var textHeaders = await client.ReadInStreamAsync(stream);
            string operationLocation = textHeaders.OperationLocation;
            string operationId = operationLocation.Split('/').Last();

            ReadOperationResult results;
            do
            {
                await Task.Delay(1000);
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while (results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted);

            var lines = new List<string>();
            var readResults = results.AnalyzeResult.ReadResults;
            foreach (var page in readResults)
            {
                foreach (var line in page.Lines)
                {
                    lines.Add(line.Text);
                }
            }

            ViewBag.Lines = lines;
            return View();
        }
    }
}
