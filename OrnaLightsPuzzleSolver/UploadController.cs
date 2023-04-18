using Emgu.CV.Structure;
using Emgu.CV;
using ImageParser;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Solver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrnaLightsPuzzleSolver.Launch
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private static readonly string[] ALLOWED_EXTENSIONS = new string[] { ".png", ".jpg", ".jpeg", ".bmp" };

        public long FileSizeLimit { get; init; }
        public long FileSizeLimitInMb => FileSizeLimit >> 20;
        private readonly string _fileSizeErrorString;

        public UploadController(IConfiguration config)
        {
            FileSizeLimit = config.GetValue<long?>("FileSizeLimit") ?? (1024 << 10);
            _fileSizeErrorString = $"File exceeds size limit: {FileSizeLimit} ({FileSizeLimitInMb}mb)";
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(IFormFile formFile)
        {
            var file = formFile;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!ALLOWED_EXTENSIONS.Contains(ext))
            {
                return BadRequest("Invalid File Extension");
            }

            if (file.Length > FileSizeLimit)
            {
                return BadRequest(_fileSizeErrorString);
            }

            var filePath = Path.GetTempFileName();

            using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);
            stream.Close();
            var image = new Image<Bgr, byte>(filePath);

            var scanner = new ImageScanner(image);

            var scanResult = scanner.ProcessImage();

            Solution? solution = null;
            LightsOutBoard? originalBoard = null;
            string? error = null;

            scanResult.Switch(board =>
            {
                var solver = new BoardSolver(board);

                var solveResult = solver.Solve();

                solveResult.Switch(sol =>
                {
                    solution = sol.Item1;
                    originalBoard = sol.Item2;
                },
                    invalidBoard =>
                    {
                        error = "Board is invalid";
                    },
                    nullBoard =>
                    {
                        error = "Not really sure how you managed this";
                    },
                    unsolveable =>
                    {
                        error = "Board could not be solved. Some 5x5 boards can be unsolvable. I could have also made a mistake. So either blame Odie or blame Akintunde";
                    });
            }, couldNotParse =>
            {
                error = "Could not find the board in the image. Make sure there are no color filters and no visual obstructions";
            });

            System.IO.File.Delete(filePath);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(new { Solution = solution, Board = originalBoard });
        }
    }
}
