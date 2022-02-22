using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Firebase.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var con = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<UserContext>(opt => opt.UseSqlServer(con));
            services.AddDbContext<AuthContext>(opt => opt.UseSqlServer(con));
            services.AddDbContext<DepartmentContext>(opt => opt.UseSqlServer(con));
            services.AddDbContext<ChatContext>(opt => opt.UseSqlServer(con));
            services.AddDbContext<MessageContext>(opt => opt.UseSqlServer(con));
            services.AddDbContext<UserChatLinkContext>(opt => opt.UseSqlServer(con));

            services.AddControllers();

            services.AddSingleton<IAuthService, LocalAuthService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
