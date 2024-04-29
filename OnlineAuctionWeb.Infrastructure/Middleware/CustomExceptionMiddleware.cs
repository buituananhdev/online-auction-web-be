using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OnlineAuctionWeb.Application.Exceptions;

namespace OnlineAuctionWeb.Infrastructure.Middleware
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (CustomException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";
                var response = new { error = ex.Message, code = ex.StatusCode };
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var response = new { error = "An unexpected error occurred." + ex.Message, code = StatusCodes.Status500InternalServerError };
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
        }
    }
}
