using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ClinicReservation.Models.Reservation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using ClinicReservation.Models;
using ClinicReservation.Services;

namespace ClinicReservation
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

        }
        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ServiceConfig serviceConfig = new ServiceConfig()
            {
                SecurityKey = Configuration["SecurityKey"],
                SessionName = Configuration["SessionName"],
                SMSApi = Configuration["SMSService"],
                ConnectionString = Configuration.GetConnectionString("reservationData"),
                RegisterationTicket = ServiceConfig.ReadTicket()
            };
            services.AddSingleton<ServiceConfig>(serviceConfig);

            services.AddMvc();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.Name = serviceConfig.SessionName;
            });

            services.AddDbContext<ReservationDbContext>(options => options.UseMySql(serviceConfig.ConnectionString));

            services.AddSingleton<NPOLJwtTokenService>(provider =>
            {
                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serviceConfig.SecurityKey));
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

            services.AddSingleton<SMSService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ILogger smsLogger = loggerFactory.CreateLogger("sms");

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
