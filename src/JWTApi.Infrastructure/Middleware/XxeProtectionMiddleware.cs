using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace JWTApi.Infrastructure.Middleware
{
    public class XxeProtectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<XxeProtectionMiddleware> _logger;

        public XxeProtectionMiddleware(RequestDelegate next, ILogger<XxeProtectionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.ContentType?.Contains("application/xml") == true ||
                context.Request.ContentType?.Contains("text/xml") == true)
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var xmlContent = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrEmpty(xmlContent))
                {
                    try
                    {
                        var settings = new XmlReaderSettings
                        {
                            DtdProcessing = DtdProcessing.Prohibit, // ممنوعیت DTD
                            XmlResolver = null,                     // جلوگیری از resolve entities خارجی
                            MaxCharactersFromEntities = 0,         // محدودیت برای Billion Laughs attack
                            CheckCharacters = true
                        };

                        using var stringReader = new StringReader(xmlContent);
                        using var xmlReader = XmlReader.Create(stringReader, settings);
                        var doc = XDocument.Load(xmlReader);

                        // بررسی بیشتر برای وجود entityهای خارجی
                        if (xmlContent.Contains("<!ENTITY") &&
                            (xmlContent.Contains("SYSTEM") || xmlContent.Contains("PUBLIC")))
                        {
                            _logger.LogWarning("XXE attack detected: External entity reference found.");
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            await context.Response.WriteAsync("Invalid XML content.");
                            return;
                        }
                    }
                    catch (XmlException ex)
                    {
                        _logger.LogWarning("XML parsing error (potential XXE): {Message}", ex.Message);
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Malformed XML.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
