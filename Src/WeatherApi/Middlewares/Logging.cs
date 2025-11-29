using System.Diagnostics;
using System.Text;
using Serilog;

namespace WeatherApi.Middlewares
{
    public class Logging
    {
        private readonly RequestDelegate _next;

        public Logging(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            // Request Body
            string requestBody = "";
            if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
            {
                context.Request.EnableBuffering();

                using var reader = new StreamReader(
                    context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);

                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // Capture Response
            var originalBodyStream = context.Response.Body;
            using var tempStream = new MemoryStream();
            context.Response.Body = tempStream;

            await _next(context);

            sw.Stop();

            tempStream.Position = 0;
            var responseBody = await new StreamReader(tempStream).ReadToEndAsync();

            tempStream.Position = 0;
            await tempStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;

            Log.Information(
                "HTTP {Method} {Path}{Query} responded {StatusCode} in {Elapsed} ms. Body: {RequestBody}. Response: {ResponseBody}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString.Value,
                context.Response.StatusCode,
                sw.ElapsedMilliseconds,
                requestBody,
                responseBody
            );
        }
    }
}
