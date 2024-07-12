using Serilog;
using PresentationLayer.Middlewares;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Interface;
using DataAccessLayer.Repositories;
using BusinessLogicLayer.Interface;
using BusinessLogicLayer.Services;
using PresentationLayer.Mapping;
using BusinessLogicLayer.Mapping;
using Microsoft.Extensions.Options;
using PresentationLayer;
using FluentValidation.AspNetCore;
using BusinessLogicLayer.Validators;
using BusinessLogicLayer.Models;
using DataAccessLayer.Entities;

var builder = WebApplication.CreateBuilder(args);

//Logger
Log.Logger = new LoggerConfiguration().MinimumLevel.Warning()
    .WriteTo.File("log/logs.txt", rollingInterval: RollingInterval.Day).CreateLogger();
builder.Host.UseSerilog();

//DB connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Distributed SQL Server Cache
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = "dbo";
    options.TableName = "CacheTable";
});

//Mapping Profiles
builder.Services.AddAutoMapper(typeof(BusinessMappingProfile));
builder.Services.AddAutoMapper(typeof(PresentationMappingProfile));

//DI
//builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
//builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IGenericRepository<EmployeeEntity>, GenericRepository<EmployeeEntity>>();
builder.Services.AddScoped<IGenericRepository<DepartmentEntity>, GenericRepository<DepartmentEntity>>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ICacheService<EmployeeEntity>, CacheService<EmployeeEntity>>();
builder.Services.AddScoped<ICacheService<DepartmentEntity>, CacheService<DepartmentEntity>>();


// Add services to the container.
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<EmployeeValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<DepartmentValidator>();
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddOptions<AppSettings>()
    .Bind(builder.Configuration.GetSection("AppSettings"))
    .ValidateDataAnnotations();

var app = builder.Build();
var options = app.Services.GetRequiredService<IOptions<AppSettings>>();
var appSettings = options.Value;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register the custom error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
