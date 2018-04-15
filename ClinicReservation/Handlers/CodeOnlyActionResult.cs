using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClinicReservation.Handlers
{
    public class CodeOnlyActionResult : IActionResult
    {
        private readonly int code;
        public CodeOnlyActionResult(int code)
        {
            this.code = code;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = code;
            return Task.CompletedTask;
        }

        public static IActionResult Code404 { get; } = new CodeOnlyActionResult(404);
    }
}
