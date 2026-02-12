using Microsoft.EntityFrameworkCore;

using TaskManager.Api.Database;
using TaskManager.Api.Extensions;
using TaskManager.Api.Features.Tasks;
using TaskManager.Api.Features.Categories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connection).UseSnakeCaseNamingConvention();
});

// DI services
builder.Services.AddScoped<CreateTask.Handler>();
builder.Services.AddScoped<GetAllTasks.Handler>();
builder.Services.AddScoped<GetTaskById.Handler>();
builder.Services.AddScoped<UpdateTask.Handler>();
builder.Services.AddScoped<UpdateTaskStatus.Handler>();
builder.Services.AddScoped<DeleteTask.Handler>();

builder.Services.AddScoped<CreateCategory.Handler>();
builder.Services.AddScoped<GetAllCategories.Handler>();
builder.Services.AddScoped<GetCategoryById.Handler>();


// CORS
var frontendUrl = builder.Configuration.GetValue<string>("CorsSettings:FrontendUrl");
const string frontendPolicy = "FrontendPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: frontendPolicy,
        policy =>
        {
            policy.WithOrigins(frontendUrl!) // URL del frontend
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(frontendPolicy);

app.UseHttpsRedirection();

app.MapTaskEndpoints();
app.MapCategoryEndpoints();

app.Run();

