using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    public class ImageController : Controller
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

            // Створюємо клієнта
            var client = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(visionKey))
            {
                Endpoint = visionEndpoint
            };

            // Зчитуємо зображення у потік
            using var stream = image.OpenReadStream();

            // Запитуємо caption, tags та detected objects
            var features = new List<VisualFeatureTypes?> {
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Tags,
                VisualFeatureTypes.Objects
            };

            var analysisResult = await client.AnalyzeImageInStreamAsync(stream, features);

            // Обробляємо результати
            var caption = analysisResult.Description?.Captions?.FirstOrDefault()?.Text ?? "No caption found";
            var tags = analysisResult.Tags?.Select(t => t.Name).ToList() ?? new List<string>();
            var objects = analysisResult.Objects?.Select(o => o.ObjectProperty).ToList() ?? new List<string>();

            ViewBag.Caption = caption;
            ViewBag.Tags = tags;
            ViewBag.Objects = objects;

            return View();
        }
    }
}
