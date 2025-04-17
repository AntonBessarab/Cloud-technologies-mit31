using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApplication_lab.Controllers
{
    public class FaceController : Controller
    {
        private const string ApiKey = "";
        private const string FaceEndpoint = "";

        private readonly IFaceClient _client;

        public FaceController()
        {
            _client = new FaceClient(new ApiKeyServiceClientCredentials(ApiKey))
            {
                Endpoint = FaceEndpoint
            };
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Count"] = 0;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                ViewData["Count"] = 0;
                return View();
            }

            var rectangles = new List<Dictionary<string, int>>();

            using var imageStream = photo.OpenReadStream();
            var detected = await _client.Face.DetectWithStreamAsync(
                imageStream,
                returnFaceId: false,
                returnFaceLandmarks: false,
                returnFaceAttributes: null,
                detectionModel: DetectionModel.Detection01
            );

            foreach (var f in detected)
            {
                var r = f.FaceRectangle;
                rectangles.Add(new Dictionary<string, int>
                {
                    { "X", r.Left },
                    { "Y", r.Top },
                    { "W", r.Width },
                    { "H", r.Height }
                });
            }

            var fileName = Path.GetRandomFileName() + Path.GetExtension(photo.FileName);
            var path = Path.Combine("wwwroot", "faces", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using var fileStream = new FileStream(path, FileMode.Create);
            await photo.CopyToAsync(fileStream);

            ViewBag.ImagePath = $"/faces/{fileName}";
            ViewBag.Boxes = rectangles;
            ViewData["Count"] = rectangles.Count;

            return View();
        }
    }
}
