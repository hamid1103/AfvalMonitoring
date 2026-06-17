using AfvalMonitoring.Components;
using AfvalMonitoring.Data;
using AfvalMonitoring.Models;
using AfvalMonitoring.Repositories.DataController;
using AfvalMonitoring.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.Cookie.MaxAge = null;
        options.Cookie.Expiration = null;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = false;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
builder.Services.AddHttpContextAccessor();

var PredictionAPI = builder.Configuration.GetValue<string>("PredictionAPI");
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

builder.Services.AddDbContext<ExampleDBContext>(opt =>
{
    opt.UseSqlServer(sqlConnectionString, o => o.EnableRetryOnFailure());
});
builder.Services.AddDbContext<DataDbContext>(opt =>
{
    opt.UseSqlServer(sqlConnectionString, o => o.EnableRetryOnFailure());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Group21's backend",
        Version = "v1"
    });
});


// Add services BEFORE building the app
//builder.Services.AddTransient<IExampleRepo, SQLExampleRepo>(o => new SQLExampleRepo(sqlConnectionString!));
builder.Services.AddScoped<IDataRepository, DataDbContextRepository>();
//builder.Services.AddScoped<IGoogleUtils, GoogleUtils>();

// Register AfvalService for DI with HttpClients for the prediction and sensoring APIs
var predictionApiKey = builder.Configuration.GetValue<string>("PredictionKey");
var sensoringApi = builder.Configuration.GetValue<string>("SensoringAPI");
var sensoringApiKey = builder.Configuration.GetValue<string>("SensoringApiKey");

builder.Services.AddHttpClient("PredictionAPI", client =>
{
    client.BaseAddress = new Uri(PredictionAPI);
    client.DefaultRequestHeaders.Add("Authorization", predictionApiKey);
});

builder.Services.AddHttpClient("SensoringAPI", client =>
{
    client.BaseAddress = new Uri(sensoringApi);
    client.DefaultRequestHeaders.Add("X-Api-Key", sensoringApiKey);
});

builder.Services.AddScoped<AfvalMonitoring.Services.AfvalService>();

// Register GoogleMapsService for DI
builder.Services.AddScoped<AfvalMonitoring.Services.GoogleMapsService>();


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Running Migrations. This can take a while.");
    
    var dbContext = scope.ServiceProvider.GetRequiredService<DataDbContext>();

    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSwagger();
app.UseSwaggerUI(options =>
{
    if (!sqlConnectionStringFound)
        options.HeadContent = "<h1 align=\"center\">❌ SqlConnectionString not found ❌</h1>";
});

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();


app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();