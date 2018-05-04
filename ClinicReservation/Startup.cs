using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using ClinicReservation.Models;
using ClinicReservation.Services;
using DNTCaptcha.Core;
using LocalizationCore;
using AuthenticationCore;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Cache;
using ClinicReservation.Services.SMS;
using ClinicReservation.Services.Database;
using ClinicReservation.Handlers;
using ClinicReservation.Services.Authentication;
using ClinicReservation.Services.Data;
using ClinicReservation.Services.Groups;

namespace ClinicReservation
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ServiceConfig serviceConfig = new ServiceConfig()
            {
                SecurityKey = Configuration["SecurityKey"],
                SessionName = Configuration["SessionName"],
                SMSUrl = Configuration["SMSUrl"],
                SMSApiKey = Configuration["SMSApiKey"],
                ConnectionString = Configuration.GetConnectionString("reservationData"),
                RegisterationTicket = ServiceConfig.ReadTicket(),
                NotificationPath = "notification.json",
                ServiceReasonPath = "service_reason.json"
            };
            services.AddSingleton<ServiceConfig>(serviceConfig);

            services.AddMvc();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.Name = serviceConfig.SessionName;
            });

            services.AddDbContextPool<DataDbContext>(options => options.UseSqlServer(serviceConfig.ConnectionString));
            services.AddScoped<IDbQuery, DbQuery>();
            services.AddScoped<IScopedUserAccessor, ScopedUserAccessor>();

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

            services.AddSingleton<ISMSService, SMSService>();

            services.AddDNTCaptcha();

            string defaultLanguage = Configuration["Culture:Default"];
            IEnumerable<string> supportedLanguages = Configuration.GetSection("Culture:Supported").AsEnumerable().Where(x => x.Value != null).Select(x => x.Value);
            services.AddMvcLocalization(defaultLanguage, supportedLanguages);
            services.AddCodeMatching();

            services.AddMvcAuthentication<CASResultHandler>(
                redirectUrl: Configuration["CAS:redirectUrl"],
                validateUrl: Configuration["CAS:validateUrl"],
                logoutUrl: Configuration["CAS:logoutUrl"],
                sessionName: Configuration["CAS:sessionName"]);

            services.AddSingleton<INotificationProvider, NotificationProvider>(service => new NotificationProvider(serviceConfig.NotificationPath));
            services.AddSingleton<IServiceState, ServiceState>();
            services.AddSingleton<IServiceNotAvailableReasonProvider, ServiceNotAvailableReasonProvider>(service => new ServiceNotAvailableReasonProvider(serviceConfig.ServiceReasonPath));

            services.AddSingleton<IValidatorSetPropertyMethodCache>(provider => new ValidatorSetPropertyMethodCache(100));

            services.AddScoped<IGroupPromptResolver, GroupPromptResolver>();
            services.AddSingleton<IGroupActionProvider, GroupActionProvider>();
            services.AddScoped<IReservationStore, ReservationStore>();
            AddAuthorizations(services);
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopped.Register(OnShutdown);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();

            app.UseMvcLocalization(checkCultureSupported: false, filter: path =>
            {
                if (path.StartsWithSegments("/js"))
                {
                    return new CheckHeaderFilterResult();
                }
                return new InvokeMiddlewareFilterResult();
            });

            app.UseStaticFiles();

            app.UseDNTCaptcha();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{page=Index}");

                routes.MapRoute(
                    name: "mvc",
                    template: "{controller=Reservation}/{action=Index}/{id?}");
            });
        }

        private void OnShutdown()
        {

        }
    }
}
