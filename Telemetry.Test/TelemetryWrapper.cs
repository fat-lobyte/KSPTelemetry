using Telemetry;

namespace TelemetryTest
{
    public class Telemetry
    {
        public static void AddChannel<ChannelType>(string id, string format = null)
            where ChannelType : IFormattable
        {
            // prepend assembly name to channel id
            id = assemblyName + "/" + id;

            TelemetryService.Instance.AddChannel<ChannelType>(id, format);
        }

        public static void Send(string id, object value)
        {
            // prepend assembly name to channel id
            id = assemblyName + "/" + id;

            TelemetryService.Instance.Send(id, value);
        }

    }
}
