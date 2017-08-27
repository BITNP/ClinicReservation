using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NPOL.Models.Reservation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NPOL.Mid;
using Microsoft.Extensions.Options;
using NPOL.Models;

namespace NPOL
{
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            securityKey = Configuration["SecurityKey"];
            smsApi = Configuration["SMSService"];
            ServiceConfig.ReadTicket();
        }

        private string smsApi;
        private string securityKey;
        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services, ILoggerFactory loggerFactory)
        {
            services.AddMvc();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.CookieName = ".NPOL";
            });

            string connection = Configuration.GetConnectionString("reservationData");
            services.AddDbContext<ReservationDbContext>(options => options.UseMySql(connection));

            services.AddSingleton<NPOLJwtTokenService>(provider =>
            {
                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
                TokenValidationParameters tokenParam = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                TokenProviderOptions options = new TokenProviderOptions()
                {
                    Audience = "MyServer",
                    Issuer = "MyClient",
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                    Expiration = TimeSpan.FromDays(1),
                    ValidationParameters = tokenParam
                };
                NPOLJwtTokenService serv = new NPOLJwtTokenService(Options.Create(options));
                return serv;
            });

            ILogger smsLogger = loggerFactory.CreateLogger("sms log");
            services.AddSingleton<SMSService>(provider =>
            {
                return new SMSService(smsApi, smsLogger);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Reservation}/{action=Index}/{id?}");
            });
        }
    }
}
