using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Application.Services
{
    public class CorrelationIdService : ICorrelationIdService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CorrelationIdHeaderName = "X-Correlation-ID";

        public CorrelationIdService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string GetCorrelationId()
        {
            // Try to get from HTTP header first
            var correlationId = _httpContextAccessor.HttpContext?.Request?.Headers[CorrelationIdHeaderName].FirstOrDefault();

            if (string.IsNullOrEmpty(correlationId))
            {
                // Try to get from HTTP context items
                correlationId = _httpContextAccessor.HttpContext?.Items[CorrelationIdHeaderName]?.ToString();
            }

            // Generate new if not found
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = GenerateNewCorrelationId();
                SetCorrelationId(correlationId);
            }

            return correlationId;
        }

        public void SetCorrelationId(string correlationId)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Items[CorrelationIdHeaderName] = correlationId;

                // Also set in response header for client tracking
                if (!_httpContextAccessor.HttpContext.Response.Headers.ContainsKey(CorrelationIdHeaderName))
                {
                    _httpContextAccessor.HttpContext.Response.Headers.Add(CorrelationIdHeaderName, correlationId);
                }
            }
        }

        public string GenerateNewCorrelationId()
        {
            return Guid.NewGuid().ToString("N")[..16]; // 16 character correlation ID
        }
    }
}
