using Telemetry;

namespace TelemetryTest
{
    public class Telemetry
    {
        public static void AddChannel<ChannelType>(string id, string format = null)
        {
            TelemetryService.Instance.AddChannel<ChannelType>(id, format);
        }

        public static void Send(string id, object value)
        {
            TelemetryService.Instance.Send(id, value);
        }

    }
}
