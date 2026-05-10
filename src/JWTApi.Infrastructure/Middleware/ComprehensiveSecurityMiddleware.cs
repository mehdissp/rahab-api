using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO.Compression;

namespace JWTApi.Infrastructure.Middleware
{
    public class ComprehensiveSecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ComprehensiveSecurityMiddleware> _logger;

        // Command Injection Pattern
        private static readonly Regex CommandInjectionPattern = new Regex(
            @"(&\s*&|\|\s*\||;|\||`|\$\(|\$\{|\b(wget|curl|nc|bash|sh|powershell|cmd|whoami|cat|ls|dir|rm|del|copy|move|find|grep|awk|sed|chmod|chown|sudo|su|passwd|kill|pkill|killall|systemctl|service|docker|kubectl|aws|az|gcloud))\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // NoSQL Injection Pattern
        private static readonly Regex NoSqlInjectionPattern = new Regex(
            @"(\{\s*""?\$?\w+""?\s*:\s*\{\s*""?\$(ne|gt|lt|gte|lte|in|nin|exists|regex|where|or|and|nor|not|all|elemMatch|size|type|slice)\b)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // SSRF Pattern - Internal IP ranges
        private static readonly Regex InternalIpPattern = new Regex(
            @"(https?://)?(127\.0\.0\.1|localhost|10\.\d{1,3}\.\d{1,3}\.\d{1,3}|172\.(1[6-9]|2[0-9]|3[0-1])\.\d{1,3}\.\d{1,3}|192\.168\.\d{1,3}\.\d{1,3}|169\.254\.\d{1,3}\.\d{1,3}|\[::1\]|\[fc00:|\[fd00:)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Mass Assignment - فیلدهای حساس
        private static readonly HashSet<string> SensitiveFields = new()
        {
            "id", "userId", "role", "isAdmin", "isDeleted", "createdAt", "updatedAt",
            "password", "passwordHash", "refreshToken", "resetToken", "verificationToken",
            "_id", "__v", "isActive", "permissions", "claims"
        };

        // Zip/XML Bomb - حداکثر حجم
        private const long MaxDecompressedSize = 10 * 1024 * 1024; // 10MB

        // Log Injection - کاراکترهای ممنوع
        private static readonly char[] LogInjectionChars = { '\n', '\r', '\t', '\0', '\x1A' };

        // Paths مستثنی (مانند Swagger)
        private static readonly HashSet<string> ExcludedPaths = new()
        {
            "/swagger", "/swagger/v1/swagger.json", "/health", "/metrics",
                "/api/Auth/login",           // اضافه کنید
    "/api/Auth/register",        // اضافه کنید
    "/api/Auth/refresh-token"    // اضافه کنید
        };

        public ComprehensiveSecurityMiddleware(RequestDelegate next, ILogger<ComprehensiveSecurityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            if (ExcludedPaths.Any(p => path.StartsWith(p)))
            {
                await _next(context);
                return;
            }

            // اضافه کردن هدرهای امنیتی
            AddSecurityHeaders(context);

            // 1. Command Injection
            if (await CheckCommandInjection(context)) return;

            // 2. NoSQL Injection
            if (await CheckNoSqlInjection(context)) return;

            // 3. SSRF
            if (await CheckSsrInUrls(context)) return;

            // 4. Mass Assignment
            if (await CheckMassAssignment(context)) return;

            // 5. Zip/XML Bomb
            if (await CheckCompressionBomb(context)) return;

            // 6. Log Injection
            if (CheckLogInjection(context)) return;

            // 7. Rate Limiting (API-specific)
            // می‌توانید این بخش را با Redis یا IMemoryCache پیاده‌سازی کنید

            // 8. Deserialization (بررسی Body برای payloadهای خطرناک)
            if (await CheckDeserializationAttack(context)) return;

            await _next(context);
        }

        private void AddSecurityHeaders(HttpContext context)
        {
            // هدرهای امنیتی پیشرفته
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // Cache-Control برای داده‌های حساس
            if (context.Request.Path.ToString().Contains("/api/"))
            {
                context.Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate");
            }
        }

        #region 1. Command Injection Detection

        private async Task<bool> CheckCommandInjection(HttpContext context)
        {
            // بررسی QueryString
            if (context.Request.QueryString.HasValue)
            {
                var query = context.Request.QueryString.Value;
                if (!string.IsNullOrEmpty(query) && CommandInjectionPattern.IsMatch(query))
                {
                    await LogAndBlock(context, "Command Injection", "QueryString", query);
                    return true;
                }
            }

            // بررسی Headers
            var dangerousHeaders = new[] { "User-Agent", "Referer", "X-Forwarded-For", "Authorization" };
            foreach (var header in dangerousHeaders)
            {
                if (context.Request.Headers.TryGetValue(header, out var headerValue))
                {
                    var value = headerValue.ToString();
                    if (!string.IsNullOrEmpty(value) && CommandInjectionPattern.IsMatch(value))
                    {
                        await LogAndBlock(context, "Command Injection", $"Header:{header}", value);
                        return true;
                    }
                }
            }

            // بررسی Body
            if (await CheckBodyForPattern(context, CommandInjectionPattern, "Command Injection"))
                return true;

            return false;
        }

        #endregion

        #region 2. NoSQL Injection Detection

        private async Task<bool> CheckNoSqlInjection(HttpContext context)
        {
            if (context.Request.QueryString.HasValue)
            {
                var query = context.Request.QueryString.Value;
                if (NoSqlInjectionPattern.IsMatch(query))
                {
                    await LogAndBlock(context, "NoSQL Injection", "QueryString", query);
                    return true;
                }
            }

            if (await CheckBodyForPattern(context, NoSqlInjectionPattern, "NoSQL Injection"))
                return true;

            return false;
        }

        #endregion

        #region 3. SSRF Detection

        private async Task<bool> CheckSsrInUrls(HttpContext context)
        {
            // بررسی URL Parameterها در QueryString
            foreach (var key in context.Request.Query.Keys)
            {
                var value = context.Request.Query[key].ToString();
                if (IsUrl(value) && InternalIpPattern.IsMatch(value))
                {
                    await LogAndBlock(context, "SSRF Attack", $"QueryParam:{key}", value);
                    return true;
                }
            }

            // بررسی Body برای URLهای داخلی
            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (InternalIpPattern.IsMatch(body))
                {
                    await LogAndBlock(context, "SSRF Attack", "RequestBody", body.Length > 200 ? body[..200] : body);
                    return true;
                }
            }

            return false;
        }

        private bool IsUrl(string input)
        {
            return !string.IsNullOrEmpty(input) &&
                   (input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    input.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                    input.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region 4. Mass Assignment Detection

        private async Task<bool> CheckMassAssignment(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method) &&
                !HttpMethods.IsPut(context.Request.Method) &&
                !HttpMethods.IsPatch(context.Request.Method))
            {
                return false;
            }

            if (context.Request.ContentType?.Contains("application/json") != true)
                return false;

            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (string.IsNullOrEmpty(body))
                return false;

            try
            {
                using var doc = JsonDocument.Parse(body);
                var sensitiveFieldsFound = FindSensitiveFields(doc.RootElement);

                if (sensitiveFieldsFound.Any())
                {
                    _logger.LogWarning("Mass Assignment attempt detected. Sensitive fields: {Fields}",
                        string.Join(", ", sensitiveFieldsFound));

                    // گزینه 1: مسدود کردن کامل
                    await ReturnBadRequest(context, "Request contains restricted fields.");
                    return true;

                    // گزینه 2: حذف فیلدهای حساس (اختیاری)
                    // var sanitizedBody = RemoveSensitiveFields(body, sensitiveFieldsFound);
                    // context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(sanitizedBody));
                }
            }
            catch (JsonException)
            {
                // خطای JSON توسط ExceptionHandlingMiddleware گرفته می‌شود
            }

            return false;
        }

        private List<string> FindSensitiveFields(JsonElement element, string path = "")
        {
            var found = new List<string>();

            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        if (SensitiveFields.Contains(property.Name.ToLowerInvariant()))
                        {
                            found.Add(string.IsNullOrEmpty(path) ? property.Name : $"{path}.{property.Name}");
                        }
                        found.AddRange(FindSensitiveFields(property.Value,
                            string.IsNullOrEmpty(path) ? property.Name : $"{path}.{property.Name}"));
                    }
                    break;
                case JsonValueKind.Array:
                    int index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        found.AddRange(FindSensitiveFields(item, $"{path}[{index}]"));
                        index++;
                    }
                    break;
            }

            return found;
        }

        #endregion

        #region 5. ZIP/XML Bomb Detection

        private async Task<bool> CheckCompressionBomb(HttpContext context)
        {
            // بررسی حجم Content-Length
            if (context.Request.ContentLength > MaxDecompressedSize * 2)
            {
                await LogAndBlock(context, "ZIP Bomb - Excessive Size", "ContentLength",
                    context.Request.ContentLength.ToString());
                return true;
            }

            // بررسی فایل‌های آپلودی
            if (context.Request.HasFormContentType && context.Request.Form.Files.Any())
            {
                foreach (var file in context.Request.Form.Files)
                {
                    // بررسی نسبت فشرده‌سازی (برای ZIP files)
                    if (file.ContentType == "application/zip" || file.FileName.EndsWith(".zip"))
                    {
                        using var stream = file.OpenReadStream();
                        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

                        long totalSize = 0;
                        foreach (var entry in archive.Entries)
                        {
                            totalSize += entry.Length;
                            if (totalSize > MaxDecompressedSize)
                            {
                                await LogAndBlock(context, "ZIP Bomb", $"File:{file.FileName}",
                                    $"Compressed:{file.Length}, Decompressed:{totalSize}");
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        #endregion

        #region 6. Log Injection Detection

        private bool CheckLogInjection(HttpContext context)
        {
            // بررسی هدرها
            foreach (var header in context.Request.Headers)
            {
                if (header.Value.Any(v => v.IndexOfAny(LogInjectionChars) >= 0))
                {
                    _logger.LogWarning("Log Injection attempt detected in Header: {Header}", header.Key);
                    return true;
                }
            }

            // بررسی QueryString
            if (context.Request.QueryString.HasValue)
            {
                var query = context.Request.QueryString.Value;
                if (query.IndexOfAny(LogInjectionChars) >= 0)
                {
                    _logger.LogWarning("Log Injection attempt detected in QueryString");
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 7. Deserialization Attack Detection

        private async Task<bool> CheckDeserializationAttack(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method) &&
                !HttpMethods.IsPut(context.Request.Method) &&
                !HttpMethods.IsPatch(context.Request.Method))
            {
                return false;
            }

            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (string.IsNullOrEmpty(body))
                return false;

            // الگوهای حمله Deserialization در JSON
            var dangerousPatterns = new[]
            {
                @"\$type\s*:\s*""System\.",  // .NET Type混淆
                @"__type\s*:\s*""System\.",  //另一种格式
                @"java\.lang\.Runtime",
                @"java\.lang\.ProcessBuilder",
                @"GadgetHolder",
                @"com\.sun\.org\.apache\.xalan\.internal\.xsltc\.trax\.TemplatesImpl"
            };

            foreach (var pattern in dangerousPatterns)
            {
                if (Regex.IsMatch(body, pattern, RegexOptions.IgnoreCase))
                {
                    await LogAndBlock(context, "Deserialization Attack", "Pattern", pattern);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Helper Methods

        private async Task<bool> CheckBodyForPattern(HttpContext context, Regex pattern, string attackType)
        {
            if (!HttpMethods.IsPost(context.Request.Method) &&
                !HttpMethods.IsPut(context.Request.Method) &&
                !HttpMethods.IsPatch(context.Request.Method))
            {
                return false;
            }

            if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
                return false;

            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (!string.IsNullOrEmpty(body) && pattern.IsMatch(body))
            {
                await LogAndBlock(context, attackType, "RequestBody", body.Length > 500 ? body[..500] : body);
                return true;
            }

            return false;
        }

        private async Task LogAndBlock(HttpContext context, string attackType, string location, string evidence)
        {
            _logger.LogWarning(
                "{AttackType} detected from {IP} - Path: {Path} - Location: {Location} - Evidence: {Evidence}",
                attackType,
                context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                context.Request.Path.ToString(),
                location,
                evidence.Length > 200 ? evidence[..200] : evidence);

            await ReturnBadRequest(context, $"Security violation: {attackType} detected.");
        }

        private async Task ReturnBadRequest(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                error = "Request blocked by security policy",
                message = message,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString(),
                requestId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }

        #endregion
    }
}