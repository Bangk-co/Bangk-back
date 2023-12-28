using Microsoft.AspNetCore.Builder;
using Bangk.Api.Middleware;

namespace Bangk.Api.Extensions
{
    public static class ExceptionHandlerExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
