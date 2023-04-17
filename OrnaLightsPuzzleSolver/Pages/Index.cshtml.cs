using Emgu.CV;
using Emgu.CV.Structure;
using ImageParser;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Solver;

namespace OrnaLightsPuzzleSolver.Launch.Pages;

public class IndexModel : PageModel
{
    private static readonly string[] ALLOWED_EXTENSIONS = new string[] { ".png", ".jpg", ".jpeg", ".bmp" };

    public long FileSizeLimit { get; init; }
    public long FileSizeLimitInMb => FileSizeLimit >> 20;
    private readonly string _fileSizeErrorString;

    public IndexModel(IConfiguration config)
    {
        FileSizeLimit = config.GetValue<long?>("FileSizeLimit") ?? (1024 << 10);
        _fileSizeErrorString = $"File exceeds size limit: {FileSizeLimit} ({FileSizeLimitInMb}mb)";
    }

    [BindProperty]
    public IFormFile? FormFile { get; set; }

    public Solution? Solution { get; set; }

    public LightsOutBoard? OriginalLightsOutBoard { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    [EnableRateLimiting("upload")]
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (FormFile == null)
        {
            return Page();
        }

        var file = FormFile;

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

        if (solution != null)
        {
            Solution = solution;
            OriginalLightsOutBoard = originalBoard;
            return Page(); // Implement post-redirect-get
        }
        ModelState.AddModelError(nameof(FormFile), error ?? "Unknown Error");
        
        return Page();
    }
}
