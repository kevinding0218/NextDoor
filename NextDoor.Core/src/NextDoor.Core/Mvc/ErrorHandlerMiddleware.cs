using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NextDoor.Core.Types;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NextDoor.Core.Mvc
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next,
            ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var httpRequest = $"ErrorHandlerMiddleware Invoking: [{context.Request.Method}] {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}";
            _logger.LogInformation(httpRequest);

            try
            {
                await this._next(context);
            }
            catch (Exception exception)
            {
                this._logger.LogError(exception, exception.Message);
                await HandleErrorAsync(context, exception);
            }
        }

        private static Task HandleErrorAsync(HttpContext context, Exception exception)
        {
            var errorCode = "error";
            var statusCode = HttpStatusCode.BadRequest;
            var message = "There was an error.";

            switch (exception)
            {
                case NextDoorException e:
                    errorCode = e.Code;
                    message = e.Message;
                    break;
            }

            var response = new
            {
                code = errorCode,
                message = exception.InnerException != null ? exception.InnerException.Message : exception.Message
            };
            var payload = JsonConvert.SerializeObject(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(payload);
        }
    }
}