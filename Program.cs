using Microsoft.EntityFrameworkCore;
using project1.Models;
using project1.Services;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddDbContext<ProjectContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("Default"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))));

// Add services to the container.
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins",
            builder => builder.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader());
    });
builder.Services.AddControllers();
builder.Services.AddScoped<ProjectService>();
// builder.Services.AddDbContext<ProjectContext>();
builder.Services.AddScoped<TechStackService>();
// builder.Services.AddDbContext<TechStackContext>();

builder.Services.AddDbContext<ProjectContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Default"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))));

builder.Services.AddDbContext<TechStackContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Default"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))));
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

app.UseRouting();

app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
