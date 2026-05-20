using System.Net;
using System.Text.Json;

namespace OishipanAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Lỗi hệ thống Oishipan: " + exception.Message,
                Detailed = exception.StackTrace // Xóa dòng này khi nộp bài thật để bảo mật
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}