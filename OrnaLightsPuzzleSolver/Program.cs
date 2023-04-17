using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddRateLimiter(_ =>
{
    _.AddFixedWindowLimiter("upload", cfg =>
    {
        cfg.QueueLimit = 2;
        cfg.PermitLimit = 10;
        cfg.Window = TimeSpan.FromMinutes(10);
    });
});

var app = builder.Build();

app.UseRateLimiter();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
