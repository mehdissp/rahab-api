using Microsoft.AspNetCore.Builder;

namespace JWTApi.Infrastructure.Middleware
{
    public static class SqlInjectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseSqlInjectionProtection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SqlInjectionMiddleware>();
        }
    }
}