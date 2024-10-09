using Microsoft.EntityFrameworkCore;
using project1.Models;
using project1.Services;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });

        services.AddControllers();
        services.AddScoped<ProjectService>();
        services.AddScoped<TechStackService>();
        services.AddScoped<AuthService>();
        services.AddScoped<CryptoService>();

        ConfigureDB(services);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseCors("AllowAllOrigins");

        app.UseAuthorization();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    public void ConfigureDB(IServiceCollection services)
    {
        services.AddDbContext<ProjectContext>(options =>
            options.UseMySql(Configuration.GetConnectionString("Default"),
            ServerVersion.AutoDetect(Configuration.GetConnectionString("Default"))));

        services.AddDbContext<TechStackContext>(options =>
            options.UseMySql(Configuration.GetConnectionString("Default"),
            ServerVersion.AutoDetect(Configuration.GetConnectionString("Default"))));

        services.AddDbContext<UserContext>(options =>
            options.UseMySql(Configuration.GetConnectionString("Default"),
            ServerVersion.AutoDetect(Configuration.GetConnectionString("Default"))));
    }
}