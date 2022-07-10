using CorpMessengerBackend.Middleware;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace CorpMessengerBackend;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var con = Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<IAppDataContext, AppDataContext>(opt => opt.UseSqlServer(con));

        services.AddControllers();
        services.AddRazorPages();

        services.AddSingleton<ICriptographyProvider, CryptographyService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<IAuthService, LocalAuthService>();
        services.AddScoped<IUserAuthProvider, HttpContextUserAuthProvider>();

        services.AddTransient<AuthMiddleware>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseRouting();
        app.UseStaticFiles();
        app.UseDefaultFiles();

        app.UseMiddleware<AuthMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();
        });
    }
}