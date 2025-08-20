using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // Logging için ekledik
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Bir sonraki middleware'i çağır. Herhangi bir hata olmazsa istek normal şekilde işlenir.
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Bir hata yakalandığında logla.
                _logger.LogError(ex, "Beklenmedik bir hata oluştu: {Message}", ex.Message);

                // Hata yanıtını oluştur ve istemciye gönder.
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        // Örnek bir özel hata sınıfı
        public class NotFoundException : Exception
        {
            public NotFoundException(string message) : base(message) { }
        }
        public class BadRequestException : Exception
        {
            public BadRequestException(string message) : base(message) { }
        }
        // HandleExceptionAsync metodunun gelişmiş hali
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode;
            string message;

            switch (exception)
            {
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound; // 404
                    message = notFoundException.Message;
                    break;
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest; // 400
                    message = badRequestException.Message;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError; // 500
                    message = "Sunucuda beklenmedik bir hata oluştu.";
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new { StatusCode = (int)statusCode, Message = message };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
