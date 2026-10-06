using System.Security.Claims;
using System.Threading.RateLimiting;
using LibraryApp1.API;
using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default") ?? DbConnectionConfig.GetConnectionString()));

builder.Services.AddScoped<IBookRepository, SqlBookRepository>();
builder.Services.AddScoped<IReservationRepository, SqlReservationRepository>();
builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "LibraryApp1.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;

        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

// Login, register and change-password allow a few attempts per minute to slow down password guessing.
// Signed-in users are limited by account, everyone else by IP address. Limits are in appsettings.json.
var rateLimit = builder.Configuration.GetSection("RateLimiting");
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = (context, cancellationToken) =>
        new ValueTask(context.HttpContext.Response.WriteAsJsonAsync(
            new { message = "Too many attempts. Please wait a minute and try again." }, cancellationToken));

    options.AddPolicy(ApiHelpers.AuthRateLimit, httpContext =>
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var partitionKey = userId != null
            ? $"user:{userId}"
            : $"ip:{httpContext.Connection.RemoteIpAddress}";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = rateLimit.GetValue("AuthPermitLimit", 5),
            Window = TimeSpan.FromSeconds(rateLimit.GetValue("AuthWindowSeconds", 60)),
            QueueLimit = 0
        });
    });
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Outside development: hide error details from users and tell browsers to always use HTTPS.
    app.UseExceptionHandler(errorApp => errorApp.Run(context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return context.Response.WriteAsJsonAsync(new { message = "Something went wrong. Please try again later." });
    }));
    app.UseHsts();
}
// Create the administrator on first run. Credentials come from user secrets, never from source code.
using (var scope = app.Services.CreateScope())
{
    var seed = builder.Configuration.GetSection("SeedAdmin");
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    var seedResult = authService.EnsureAdminExists(seed["Username"] ?? "", seed["Email"] ?? "", seed["Password"] ?? "");

    if (!seedResult.IsSuccess)
        app.Logger.LogWarning("Administrator was not created: {Reason}", seedResult.Description);
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();
app.Run();