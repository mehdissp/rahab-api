using Microsoft.AspNetCore.Builder;

namespace JWTApi.Infrastructure.Middleware
{
    public static class ComprehensiveSecurityMiddlewareExtensions
    {
        public static IApplicationBuilder UseComprehensiveSecurity(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ComprehensiveSecurityMiddleware>();
        }
    }
}