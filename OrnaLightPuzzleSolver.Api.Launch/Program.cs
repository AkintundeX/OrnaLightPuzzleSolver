using Emgu.CV;
using Emgu.CV.Structure;
using ImageParser;
using Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Solver;
using System.Text;

string[] ALLOWED_EXTENSIONS = new string[] { ".png", ".jpg", ".jpeg", ".bmp" };

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

services.AddRateLimiter(_ =>
{
    _.AddFixedWindowLimiter("upload", cfg =>
    {
        cfg.QueueLimit = 10;
        cfg.PermitLimit = 20;
        cfg.Window = TimeSpan.FromMinutes(1);
    });
});

services.RegisterInfrastructureServices();
services.RegisterRepositoryServices();

var fileSizeLimit = config.GetValue<long?>("FileSizeLimit") ?? (1024 << 10);
var fileSizeLimitError = $"File exceeds size limit: {fileSizeLimit} ({fileSizeLimit >> 20}mb)";

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRateLimiter();

app.MapPost("/upload", async (HttpRequest request) =>
{
    if (!request.HasFormContentType)
        return "Wrong content type";

    var form = await request.ReadFormAsync();

    if (form.Files.Count != 1)
        return "Invalid file count";

    var file = form.Files.First();

    if (file is null || file.Length == 0)
        return "No file";

    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

    if (!ALLOWED_EXTENSIONS.Contains(ext))
    {
        return "Invalid File Extension";
    }

    if (file.Length > fileSizeLimit)
    {
        return fileSizeLimitError;
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
        return error;
    }

    var sb = new StringBuilder();

    sb.Append("Solution:\n");
    sb.Append(solution.ToString());
    sb.Append("Board:\n");
    sb.Append(originalBoard.ToString());
    return sb.ToString();
})
.Accepts<IFormFile>("multipart/form-data")
.RequireRateLimiting("upload");

app.Run();