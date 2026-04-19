using challenge1.Common.Util.Common;
using Microsoft.AspNetCore.Http;

namespace challenge1.Common.Util.Middleware
{
    public class AuditContextMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            AuditContextProvider.CurrentUserId =
                context.User?.Identity?.Name ?? "SYSTEM";

            AuditContextProvider.CurrentUserName =
                context.User?.Identity?.Name ?? "System";

            AuditContextProvider.IPAddress =
                context.Connection?.RemoteIpAddress?.ToString();

            AuditContextProvider.UserAgent =
                context.Request?.Headers["User-Agent"].ToString();

            AuditContextProvider.Url =
                context.Request?.Path;

            AuditContextProvider.Referer =
                context.Request?.Headers["Referer"];

            await _next(context);
        }

    }
}
