using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace JWTApi.Infrastructure.Middleware
{
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityMiddleware> _logger;
        private static readonly RecyclableMemoryStreamManager _memoryStreamManager = new();

        // الگوی تشخیص XSS - نسخه قوی‌تر
        private static readonly Regex XssPattern = new Regex(
            @"<script\b[^>]*>.*?</script>|" +
            @"javascript\s*:|" +
            @"on\w+\s*=\s*[""'][^""']*[""']|" +
            @"<!--.*?-->|" +
            @"<!\[CDATA\[.*?\]\]>|" +
            @"expression\s*\(|" +
            @"alert\s*\(|" +
            @"eval\s*\(|" +
            @"document\.\w+|" +
            @"window\.\w+|" +
            @"cookie\s*[=:]|" +
            @"vbscript\s*:|" +
            @"<iframe\b|" +
            @"<object\b|" +
            @"<embed\b|" +
            @"<link\b.*?href\s*=|" +
            @"<meta\b.*?content\s*=",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

        // الگوهای حمله XXE
        private static readonly Regex XxePattern = new Regex(
            @"<!DOCTYPE[^>]*\[.*?<!ENTITY.*?(?:SYSTEM|PUBLIC).*?>.*?\]>|" +
            @"\<\!ENTITY\s+\%\s+\w+\s+SYSTEM|" +
            @"xmlns\s*=\s*[""'].*?external.*?[""']",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

        public SecurityMiddleware(RequestDelegate next, ILogger<SecurityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. اضافه کردن هدرهای امنیتی
            AddSecurityHeaders(context);

            // 2. بررسی XSS در QueryString و Route values
            if (await CheckXssInRequestPath(context))
                return;

            // 3. بررسی XSS در Headers
            if (CheckXssInHeaders(context))
                return;

            // 4. بررسی بدنه درخواست (برای POST, PUT, PATCH)
            if (HttpMethods.IsPost(context.Request.Method) ||
                HttpMethods.IsPut(context.Request.Method) ||
                HttpMethods.IsPatch(context.Request.Method))
            {
                if (await CheckRequestBodyForXssAndXxe(context))
                    return;
            }

            await _next(context);
        }

        private void AddSecurityHeaders(HttpContext context)
        {
            // هدرهای امنیتی پیشرفته
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

            // CSP - Content Security Policy (متناسب با نیاز پروژه)
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval' https:; " +
                "style-src 'self' 'unsafe-inline' https:; " +
                "img-src 'self' data: https:; " +
                "font-src 'self' data: https:; " +
                "connect-src 'self' https:;");
        }

        private async Task<bool> CheckXssInRequestPath(HttpContext context)
        {
            // بررسی QueryString
            if (context.Request.QueryString.HasValue)
            {
                var query = context.Request.QueryString.Value;
                if (!string.IsNullOrEmpty(query) && XssPattern.IsMatch(query))
                {
                    _logger.LogWarning("XSS attack detected in QueryString from {IP} - Path: {Path}, Query: {Query}",
                        context.Connection.RemoteIpAddress,
                        context.Request.Path,
                        query);
                    await ReturnBadRequest(context, "Invalid request parameters.");
                    return true;
                }
            }

            // بررسی Route values (مقادیر مسیر)
            foreach (var value in context.Request.RouteValues.Values)
            {
                if (value != null && XssPattern.IsMatch(value.ToString()))
                {
                    _logger.LogWarning("XSS attack detected in Route value from {IP} - Path: {Path}, Value: {Value}",
                        context.Connection.RemoteIpAddress,
                        context.Request.Path,
                        value);
                    await ReturnBadRequest(context, "Invalid route parameters.");
                    return true;
                }
            }

            return false;
        }

        private bool CheckXssInHeaders(HttpContext context)
        {
            // بررسی هدرهای خاص که ممکن است حاوی XSS باشند
            var dangerousHeaders = new[] { "User-Agent", "Referer", "X-Forwarded-For", "X-Real-IP" };

            foreach (var headerName in dangerousHeaders)
            {
                if (context.Request.Headers.TryGetValue(headerName, out var headerValue))
                {
                    var value = headerValue.ToString();
                    if (!string.IsNullOrEmpty(value) && XssPattern.IsMatch(value))
                    {
                        _logger.LogWarning("XSS attack detected in Header '{Header}' from {IP} - Value: {Value}",
                            headerName,
                            context.Connection.RemoteIpAddress,
                            value);
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task<bool> CheckRequestBodyForXssAndXxe(HttpContext context)
        {
            // خواندن بدنه درخواست
            context.Request.EnableBuffering();
            var body = await ReadBodyAsString(context.Request);

            if (string.IsNullOrEmpty(body))
                return false;

            // بررسی XSS
            if (XssPattern.IsMatch(body))
            {
                _logger.LogWarning("XSS attack detected in request body from {IP} - Path: {Path}",
                    context.Connection.RemoteIpAddress,
                    context.Request.Path);
                await ReturnBadRequest(context, "Invalid request content.");
                return true;
            }

            // بررسی XXE برای محتوای XML
            if (IsXmlContent(context.Request.ContentType) && XxePattern.IsMatch(body))
            {
                _logger.LogWarning("XXE attack detected in XML request from {IP} - Path: {Path}",
                    context.Connection.RemoteIpAddress,
                    context.Request.Path);
                await ReturnBadRequest(context, "Invalid XML content.");
                return true;
            }

            // بررسی امنیتی عمیق‌تر برای XML
            if (IsXmlContent(context.Request.ContentType))
            {
                if (await CheckXmlSecurity(body, context))
                    return true;
            }

            return false;
        }

        private async Task<string> ReadBodyAsString(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0; // ریست کردن موقعیت برای پردازش بعدی
            return body;
        }

        private bool IsXmlContent(string? contentType)
        {
            return !string.IsNullOrEmpty(contentType) &&
                   (contentType.Contains("application/xml", StringComparison.OrdinalIgnoreCase) ||
                    contentType.Contains("text/xml", StringComparison.OrdinalIgnoreCase) ||
                    contentType.Contains("application/soap+xml", StringComparison.OrdinalIgnoreCase));
        }

        private async Task<bool> CheckXmlSecurity(string xmlContent, HttpContext context)
        {
            try
            {
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,  // جلوگیری از پردازش DTD
                    XmlResolver = null,                       // جلوگیری از resolve entities خارجی
                    MaxCharactersFromEntities = 1024,        // محدودیت برای Billion Laughs
                    MaxCharactersInDocument = 1024 * 1024,   // حداکثر 1MB برای سند XML
                    CheckCharacters = true,
                    IgnoreComments = true,
                    IgnoreWhitespace = false
                };

                using var stringReader = new StringReader(xmlContent);
                using var xmlReader = XmlReader.Create(stringReader, settings);

                // تلاش برای لود کردن XML - اگر خطا بدهد، درخواست مشکوک است
                var doc = XDocument.Load(xmlReader);

                // بررسی وجود Entityهای خارجی در محتوا
                if (xmlContent.Contains("<!ENTITY", StringComparison.OrdinalIgnoreCase) &&
                    (xmlContent.Contains("SYSTEM", StringComparison.OrdinalIgnoreCase) ||
                     xmlContent.Contains("PUBLIC", StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogWarning("Potential XXE attack: External entity reference found in XML");
                    await ReturnBadRequest(context, "Invalid XML structure.");
                    return true;
                }
            }
            catch (XmlException ex)
            {
                _logger.LogWarning("XML parsing error (potential XXE or malformed XML): {Message}", ex.Message);
                await ReturnBadRequest(context, "Malformed XML content.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while validating XML content");
                return false;
            }

            return false;
        }

        private async Task ReturnBadRequest(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            var errorResponse = System.Text.Json.JsonSerializer.Serialize(new
            {
                error = message,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString()
            });
            await context.Response.WriteAsync(errorResponse);
        }
    }
}
