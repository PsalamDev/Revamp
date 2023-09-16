using Coravel;
using Core;
using FluentValidation.AspNetCore;
using HRShared.Helpers;
using Infrastructure;
using Infrastructure.Implementation;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

ConfigurationManager configuration = builder.Configuration;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




builder.Host.ConfigureAppConfiguration((config, context) =>
{

    // context.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //     .AddJsonFile($"appsettings.{config.HostingEnvironment.EnvironmentName}.json", reloadOnChange: true, optional: true)
    //     .AddEnvironmentVariables();
    MailTemplateHelper.Initialize(config.HostingEnvironment);
});


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});
builder.Services.AddControllers();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

//builder.Services.AddDbContext(connectionString);
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSettingsConfiguration(builder.Configuration);
builder.Services.AddAzureStorageService(builder.Configuration);
builder.Services.AddControllers().AddFluentValidation();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Host.UseSerilog();

var app = builder.Build();
await app.Services.InitializeDatabasesAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}
//app.UseCors("MyPolicy");


app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();
app.UseInfrastructure(builder.Configuration);


app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<BackgroundJobService>()
        .EveryFiveMinutes();
});

app.MapControllers();

app.Run();