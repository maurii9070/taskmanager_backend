using Microsoft.EntityFrameworkCore;

using TaskManager.Api.Database;
using TaskManager.Api.Extensions;
using TaskManager.Api.Features.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connection).UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<CreateTask.Handler>();
builder.Services.AddScoped<GetAllTasks.Handler>();
builder.Services.AddScoped<GetTaskById.Handler>();
builder.Services.AddScoped<UpdateTask.Handler>();
builder.Services.AddScoped<DeleteTask.Handler>();

// DI services

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapTaskEndpoints();

app.Run();

