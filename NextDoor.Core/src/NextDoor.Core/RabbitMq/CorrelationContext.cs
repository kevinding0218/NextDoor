using Newtonsoft.Json;
using NextDoor.Core.Common;
using System;

namespace NextDoor.Core.RabbitMq
{
    public class CorrelationContext : ICorrelationContext
    {
        public Guid Id { get; }
        public int UserId { get; }
        public Guid ResourceId { get; }
        public string TraceId { get; }
        public string SpanContext { get; }
        public string ConnectionId { get; }
        public string Name { get; }
        public string Origin { get; }
        public string Resource { get; }
        public string Culture { get; }
        public DateTime CreatedAt { get; }
        public int Retries { get; set; }

        public CorrelationContext()
        {
        }

        private CorrelationContext(Guid id)
        {
            this.Id = id;
        }

        [JsonConstructor]
        private CorrelationContext(Guid id, int userId, Guid resourceId, string traceId, string spanContext,
            string connectionId, string executionId, string name, string origin, string culture, string resource, int retries)
        {
            Id = id;
            UserId = userId;
            ResourceId = resourceId;
            TraceId = traceId;
            SpanContext = spanContext;
            ConnectionId = connectionId;
            Name = string.IsNullOrWhiteSpace(name) ? string.Empty : GetName(name);
            Origin = string.IsNullOrWhiteSpace(origin) ? string.Empty :
                origin.StartsWith("/") ? origin.Remove(0, 1) : origin;
            Culture = culture;
            Resource = resource;
            Retries = retries;
            CreatedAt = DateTime.Now;
        }

        private static string GetName(string name)
            => name.Underscore().ToLowerInvariant();

        #region Using "Factory Pattern" execute appropriate creation
        public static ICorrelationContext Empty
            => new CorrelationContext();

        public static ICorrelationContext FromId(Guid id)
            => new CorrelationContext(id);

        public static ICorrelationContext From<T>(ICorrelationContext context)
            => Create<T>(context.Id, context.UserId, context.ResourceId, context.TraceId, context.ConnectionId,
                context.Origin, context.Culture, context.Resource);

        public static ICorrelationContext Create<T>(Guid id, int userId, Guid resourceId, string origin,
            string traceId, string spanContext, string connectionId, string culture, string resource = "")
            => new CorrelationContext(id, userId, resourceId, traceId, spanContext, connectionId, string.Empty, typeof(T).Name, origin, culture, resource, 0);
        #endregion
    }
}