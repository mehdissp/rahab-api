using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Middleware
{

    public class XssProtectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<XssProtectionMiddleware> _logger;
        private static readonly Regex XssPattern = new Regex(
            @"<script|javascript:|onerror=|onload=|onclick=|onmouseover|alert\(|eval\(|expression\(|vbscript:",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public XssProtectionMiddleware(RequestDelegate next, ILogger<XssProtectionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // بررسی QueryString
            if (context.Request.QueryString.HasValue)
            {
                var query = context.Request.QueryString.Value;
                if (XssPattern.IsMatch(query))
                {
                    _logger.LogWarning("Potential XSS attack detected in QueryString: {Query}", query);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid request parameters.");
                    return;
                }
            }

            // بررسی Form (اگر نوع محتوا فرم باشد)
            if (context.Request.HasFormContentType && context.Request.Form != null)
            {
                foreach (var key in context.Request.Form.Keys)
                {
                    var value = context.Request.Form[key];
                    if (XssPattern.IsMatch(value))
                    {
                        _logger.LogWarning("Potential XSS attack detected in Form field '{Key}': {Value}", key, value);
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Invalid form data.");
                        return;
                    }
                }
            }

            // بررسی Body (JSON/Text)
            if (context.Request.ContentLength > 0 &&
                (context.Request.ContentType?.Contains("application/json") == true ||
                 context.Request.ContentType?.Contains("text/plain") == true))
            {
                context.Request.EnableBuffering(); // امکان چندبار خوندن بدنه
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (XssPattern.IsMatch(body))
                {
                    _logger.LogWarning("Potential XSS attack detected in request body.");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid request body.");
                    return;
                }
            }

            // اضافه کردن هدر امنیتی X-XSS-Protection برای مرورگرهای قدیمی
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self'");

            await _next(context);
        }
    }
}
