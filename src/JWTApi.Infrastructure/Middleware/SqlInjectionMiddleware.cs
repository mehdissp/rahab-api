using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;

namespace JWTApi.Infrastructure.Middleware
{
    public class SqlInjectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SqlInjectionMiddleware> _logger;

        // الگوهای ساده‌تر و قابل اطمینان‌تر برای SQL Injection
        // هر الگو به صورت جداگانه برای رفع خطای RegexParseException
        private static readonly Regex SqlKeywordPattern = new Regex(
            @"\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|TRUNCATE|EXEC|EXECUTE|MERGE|UNION|DECLARE)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SqlConditionPattern = new Regex(
            @"(OR|AND)\s+\d+\s*=\s*\d+",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SqlStringConditionPattern = new Regex(
            @"(OR|AND)\s+'.*?'\s*=\s*'.*?'",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SqlCommentPattern = new Regex(
            @"(--\s)|(/\*)|(\*/)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SqlUnionPattern = new Regex(
            @"UNION\s+ALL\s+SELECT",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SqlInformationSchemaPattern = new Regex(
            @"INFORMATION_SCHEMA",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SqlExecPattern = new Regex(
            @"EXEC\s*\(",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SqlSystemProcPattern = new Regex(
            @"\b(sp_|xp_)\w+",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SqlSleepPattern = new Regex(
            @"(SLEEP|BENCHMARK|WAITFOR)\s*\(",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // ترکیب همه الگوها برای بررسی سریع (اما با منطق OR ساده)
        private static readonly Func<string, bool> ContainsSqlInjection = (input) =>
        {
            if (string.IsNullOrEmpty(input)) return false;

            return SqlKeywordPattern.IsMatch(input) ||
                   SqlConditionPattern.IsMatch(input) ||
                   SqlStringConditionPattern.IsMatch(input) ||
                   SqlCommentPattern.IsMatch(input) ||
                   SqlUnionPattern.IsMatch(input) ||
                   SqlInformationSchemaPattern.IsMatch(input) ||
                   SqlExecPattern.IsMatch(input) ||
                   SqlSystemProcPattern.IsMatch(input) ||
                   SqlSleepPattern.IsMatch(input);
        };

        public SqlInjectionMiddleware(RequestDelegate next, ILogger<SqlInjectionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // مسیرهای لاگین و ثبت‌نام را از بررسی مستثنی کنید (اختیاری)
            var path = context.Request.Path.ToString();
            if (path.Contains("/api/Auth/login", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/api/Auth/register", StringComparison.OrdinalIgnoreCase))
            {
                // لاگین و ثبت‌نام معمولاً حاوی کاراکترهای عادی هستند
                // اما باز هم بررسی ساده می‌کنیم
                if (await CheckLoginRequest(context))
                    return;

                await _next(context);
                return;
            }

            // 1. بررسی QueryString
            if (await CheckQueryStringForSqlInjection(context))
                return;

            // 2. بررسی Route values
            if (CheckRouteValuesForSqlInjection(context))
                return;

            // 3. بررسی Headers
            if (CheckHeadersForSqlInjection(context))
                return;

            // 4. بررسی Form data
            if (context.Request.HasFormContentType && await CheckFormDataForSqlInjection(context))
                return;

            // 5. بررسی Body
            if (await CheckBodyForSqlInjection(context))
                return;

            await _next(context);
        }

        // بررسی اختصاصی برای درخواست لاگین
        private async Task<bool> CheckLoginRequest(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method))
                return false;

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
                if (doc.RootElement.TryGetProperty("username", out var username) &&
                    doc.RootElement.TryGetProperty("password", out var password))
                {
                    var usernameValue = username.GetString();
                    var passwordValue = password.GetString();

                    // بررسی ساده برای کاراکترهای خطرناک در لاگین
                    if (!string.IsNullOrEmpty(usernameValue))
                    {
                        if (ContainsSqlInjection(usernameValue) ||
                            usernameValue.Contains('<') ||
                            usernameValue.Contains('>') ||
                            usernameValue.Contains("script", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogWarning("Suspicious characters in login username from {IP}",
                                context.Connection.RemoteIpAddress);
                            await ReturnBadRequest(context, "Invalid username format.");
                            return true;
                        }
                    }

                    if (!string.IsNullOrEmpty(passwordValue))
                    {
                        // پسورد معمولاً می‌تواند کاراکترهای خاص داشته باشد، فقط SQL Injection را چک می‌کنیم
                        if (ContainsSqlInjection(passwordValue))
                        {
                            _logger.LogWarning("SQL Injection pattern detected in login password from {IP}",
                                context.Connection.RemoteIpAddress);
                            await ReturnBadRequest(context, "Invalid password format.");
                            return true;
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // فرمت JSON نامعتبر
                await ReturnBadRequest(context, "Invalid JSON format.");
                return true;
            }

            return false;
        }

        private async Task<bool> CheckQueryStringForSqlInjection(HttpContext context)
        {
            if (!context.Request.QueryString.HasValue)
                return false;

            var query = context.Request.QueryString.Value;
            if (string.IsNullOrEmpty(query))
                return false;

            if (ContainsSqlInjection(query))
            {
                _logger.LogWarning("SQL Injection detected in QueryString from {IP} - Path: {Path}",
                    context.Connection.RemoteIpAddress, context.Request.Path);
                await ReturnBadRequest(context, "Invalid request parameters.");
                return true;
            }

            foreach (var key in context.Request.Query.Keys)
            {
                var value = context.Request.Query[key].ToString();
                if (!string.IsNullOrEmpty(value) && ContainsSqlInjection(value))
                {
                    _logger.LogWarning("SQL Injection detected in Query parameter '{Key}' from {IP}",
                        key, context.Connection.RemoteIpAddress);
                    await ReturnBadRequest(context, $"Invalid value for parameter '{key}'.");
                    return true;
                }
            }

            return false;
        }

        private bool CheckRouteValuesForSqlInjection(HttpContext context)
        {
            foreach (var value in context.Request.RouteValues.Values)
            {
                if (value != null && ContainsSqlInjection(value.ToString()))
                {
                    _logger.LogWarning("SQL Injection detected in Route value from {IP} - Path: {Path}",
                        context.Connection.RemoteIpAddress, context.Request.Path);
                    return true;
                }
            }
            return false;
        }

        private bool CheckHeadersForSqlInjection(HttpContext context)
        {
            var sensitiveHeaders = new[] { "User-Agent", "Referer", "X-Forwarded-For", "X-Real-IP" };

            foreach (var headerName in sensitiveHeaders)
            {
                if (context.Request.Headers.TryGetValue(headerName, out var headerValue))
                {
                    var value = headerValue.ToString();
                    if (!string.IsNullOrEmpty(value) && ContainsSqlInjection(value))
                    {
                        _logger.LogWarning("SQL Injection detected in Header '{Header}' from {IP}",
                            headerName, context.Connection.RemoteIpAddress);
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task<bool> CheckFormDataForSqlInjection(HttpContext context)
        {
            if (!context.Request.HasFormContentType)
                return false;

            context.Request.EnableBuffering();

            foreach (var key in context.Request.Form.Keys)
            {
                var value = context.Request.Form[key].ToString();
                if (!string.IsNullOrEmpty(value) && ContainsSqlInjection(value))
                {
                    _logger.LogWarning("SQL Injection detected in Form field '{Key}' from {IP}",
                        key, context.Connection.RemoteIpAddress);
                    await ReturnBadRequest(context, $"Invalid value for field '{key}'.");
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> CheckBodyForSqlInjection(HttpContext context)
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

            if (string.IsNullOrEmpty(body))
                return false;

            if (ContainsSqlInjection(body))
            {
                _logger.LogWarning("SQL Injection detected in request body from {IP} - Path: {Path}",
                    context.Connection.RemoteIpAddress, context.Request.Path);
                await ReturnBadRequest(context, "Invalid request content.");
                return true;
            }

            // بررسی JSON recursive
            if (context.Request.ContentType?.Contains("application/json") == true)
            {
                try
                {
                    using var doc = JsonDocument.Parse(body);
                    if (CheckJsonElementForSqlInjection(doc.RootElement))
                    {
                        _logger.LogWarning("SQL Injection detected in JSON body from {IP}",
                            context.Connection.RemoteIpAddress);
                        await ReturnBadRequest(context, "Invalid JSON content.");
                        return true;
                    }
                }
                catch (JsonException)
                {
                    // JSON نامعتبر
                }
            }

            return false;
        }

        private bool CheckJsonElementForSqlInjection(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    var stringValue = element.GetString();
                    return !string.IsNullOrEmpty(stringValue) && ContainsSqlInjection(stringValue);

                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        if (CheckJsonElementForSqlInjection(property.Value))
                            return true;
                    }
                    break;

                case JsonValueKind.Array:
                    foreach (var item in element.EnumerateArray())
                    {
                        if (CheckJsonElementForSqlInjection(item))
                            return true;
                    }
                    break;
            }

            return false;
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
                path = context.Request.Path.ToString()
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}