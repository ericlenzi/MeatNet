using Meat.Infrastructure.Logger;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System.Collections.Generic;

namespace Meat.Infrastructure
{
    public class AppInsightsTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }
        private LoggerService Logger { get; set; }

        public AppInsightsTelemetryProcessor(ITelemetryProcessor next)
        {
            this.Next = next;
            this.Logger = new("Meat.Api");
        }

        public void Process(ITelemetry item)
        {
            if (!string.IsNullOrEmpty(item.Context.Operation.SyntheticSource)) { return; }

            List<ITelemetry> requestTelemetry = new()
            {
                item
            };

            var messageSerialized = Microsoft.ApplicationInsights.Extensibility.Implementation.JsonSerializer.Serialize(requestTelemetry);
            var message = Microsoft.ApplicationInsights.Extensibility.Implementation.JsonSerializer.Deserialize(messageSerialized);
            if (!message.Contains("\"name\":\"AppMetrics\"") && !message.Contains("\"name\":\"AppDependencies\""))
                Logger.Log(message);

            this.Next.Process(item);
        }
    }
}