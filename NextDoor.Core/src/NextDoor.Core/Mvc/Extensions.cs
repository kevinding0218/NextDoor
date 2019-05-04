using Microsoft.AspNetCore.Builder;

namespace NextDoor.Core.Mvc
{
    public static class Extensions
    {
        #region ErrorHandlerMiddleware
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<ErrorHandlerMiddleware>();
        #endregion
    }
}