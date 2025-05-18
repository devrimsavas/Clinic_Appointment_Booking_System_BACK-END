
using ClinicBooking.Models;
using Microsoft.EntityFrameworkCore;
//againist cycling 
using System.Text.Json.Serialization;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// DbContext injection with MySQL (Oracle's provider)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Database connection string is missing");
    options.UseMySQL(connectionString); //  Oracle's provider
});

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//enable swagger document 
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Clinic Appointment Booking API",
        Description = "ASP.NET Core Web API for managing medical clinics, doctors, patients, and appointments."
    });

    // Enable XML comments if available
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//cors
builder.Services.AddCors(options=> {
    options.AddPolicy("AllowAll",policy=> {
        policy.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();

    });
});


var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
