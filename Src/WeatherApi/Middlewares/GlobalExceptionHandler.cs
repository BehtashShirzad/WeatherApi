using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using WeatherApi.Models;
 

namespace WeatherApi.Middlewares
{
    internal class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            Log.Error(exception, "Unhandled exception occurred");

            var dto = new ErrorResponse(
                "An error has occurred.",
                httpContext.TraceIdentifier);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(dto, cancellationToken: cancellationToken);

            return true;
        }
    }
}
