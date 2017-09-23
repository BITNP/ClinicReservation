using Microsoft.AspNetCore.Builder;

namespace ClinicReservation.Middlewares
{
    public static class LocalizationExtensions
    {
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LocalizationMiddleware>();
        }
    }
}
