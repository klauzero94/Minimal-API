using Middlewares;

namespace Extensions;

public static class UnauthorizedResponseExtension
{
    public static IApplicationBuilder UseUnauthorizedResponse(this IApplicationBuilder builder) => builder.UseMiddleware<UnauthorizedResponseMiddleware>();
}