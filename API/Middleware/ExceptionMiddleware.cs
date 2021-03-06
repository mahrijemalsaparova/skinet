using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        //ApiException için. Exception hatası için.
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }
        //middleware method
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {   // this means if there is no exception then the request moves on to its next stage.
                await _next(context);
            }
            catch (Exception ex) // if there is exception we want to catch it here.
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json"; //responsun tipi 
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment()
                    // if we in development mode
                    ? new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                    // if we in production mode
                    : new ApiException((int)HttpStatusCode.InternalServerError);
                
                // returns our response in camelcase
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response, options); // object => string

                await context.Response.WriteAsync(json);
                //daha sonra bu middlewarı startup'ta kullanıyoruz
            }
        }
    }
}