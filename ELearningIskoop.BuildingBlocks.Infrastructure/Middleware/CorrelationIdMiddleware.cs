using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.Services;
using Microsoft.AspNetCore.Http;

namespace ELearningIskoop.BuildingBlocks.Infrastructure.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeaderName = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context, ICorrelationIdService correlationIdService)
        {
            // Get or generate correlation ID
            var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault()
                                ?? correlationIdService.GenerateNewCorrelationId();

            // Set correlation ID for the request
            correlationIdService.SetCorrelationId(correlationId);

            // Continue with the pipeline
            await _next(context);
        }
    }
}
