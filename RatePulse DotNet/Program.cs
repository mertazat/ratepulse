using System.Globalization;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using RatePulse.Middleware;
using RatePulse.Repositories;
using RatePulse.Services.Alert;
using RatePulse.Services.Auth;
using RatePulse.Services.Background;
using RatePulse.Services.Currency;
using RatePulse.Services.Notification;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

var credentialFile = builder.Configuration["Firebase:CredentialFile"]!;
var credentialPath = File.Exists(credentialFile)
    ? Path.GetFullPath(credentialFile)
    : Path.Combine(AppContext.BaseDirectory, credentialFile);

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(credentialPath)
});

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
var projectId = builder.Configuration["Firebase:ProjectId"]!;
builder.Services.AddSingleton(_ => FirestoreDb.Create(projectId));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRateRepository, RateRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();

builder.Services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHttpClient<ICurrencyFetchService, CurrencyFetchService>();
builder.Services.AddHostedService<CurrencyBackgroundService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".RatePulse.Admin";
});

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RatePulse API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseMiddleware<FirebaseAuthMiddleware>();

app.MapGet("/admin", context =>
{
    context.Response.Redirect("/admin/login");
    return Task.CompletedTask;
});

app.MapGet("/", context =>
{
    context.Response.Redirect("/admin/login");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=AdminAuth}/{action=Login}/{id?}");

app.MapControllers();

app.Run();
