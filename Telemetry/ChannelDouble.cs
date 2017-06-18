
using System;

namespace Telemetry
{
    internal class ChannelDouble : IChannel
    {
        private const string defaultFormat = "0.0000";

        public string Name { get; private set; }
        public string Format { get; private set; }

        private double value;

        public ChannelDouble(string name, string format = null)
        {
            Name = name;
            Format = format ?? defaultFormat;
        }

        public string Render()
        {
            return value.ToString(Format);
        }

        public void Send(object value)
        {
            if (value.GetType() != typeof(double))
                throw new InvalidOperationException("Trying to send a value of type `" + value.GetType() + "` to a channel of type `" + typeof(double) + "`");

            this.value = (double) value;
        }
    }
}
