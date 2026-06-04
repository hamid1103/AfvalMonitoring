using AfvalMonitoring.Components;
using AfvalMonitoring.Data;
using AfvalMonitoring.Repositories.DataController;
using AfvalMonitoring.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();

var PredictionAPI = builder.Configuration.GetValue<string>("PredictionAPI");
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

builder.Services.AddDbContext<ExampleDBContext>(opt =>
{
    opt.UseSqlServer(sqlConnectionString);
});
builder.Services.AddDbContext<DataDbContext>(opt =>
{
    opt.UseSqlServer(sqlConnectionString);
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
builder.Services.AddScoped<IGoogleUtils, GoogleUtils>();

// Register AfvalService for DI with HttpClient and set BaseAddress to backend API
builder.Services.AddHttpClient<AfvalMonitoring.Services.AfvalService>(client =>
{
    client.BaseAddress = new Uri(PredictionAPI);
});

// Register GoogleMapsService for DI
builder.Services.AddScoped<AfvalMonitoring.Services.GoogleMapsService>();


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Running Migrations");
    
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

app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();