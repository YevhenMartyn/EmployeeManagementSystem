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

var builder = WebApplication.CreateBuilder(args);

//Logger
Log.Logger = new LoggerConfiguration().MinimumLevel.Warning()
    .WriteTo.File("log/logs.txt", rollingInterval: RollingInterval.Day).CreateLogger();
builder.Host.UseSerilog();

//DB connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Mapping Profiles
builder.Services.AddAutoMapper(typeof(BusinessMappingProfile));
builder.Services.AddAutoMapper(typeof(PresentationMappingProfile));

//DI
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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