
using System;

namespace Telemetry
{
    internal class ChannelInt : IChannel
    {
        private const string defaultFormat = "00000";

        public string Name { get; private set; }
        public string Format { get; private set; }

        public bool WasUpdated { get; private set; }

        private int value;

        public ChannelInt(string name, string format = null)
        {
            Name = name;
            Format = format ?? defaultFormat;
        }

        public string Render()
        {
            WasUpdated = false;
            return value.ToString(Format);
        }

        public void Send(object value)
        {
            if (value.GetType() != typeof(int))
                throw new InvalidOperationException("Trying to send a value of type `" + value.GetType() + "` to a channel of type `" + typeof(int) + "`");

            WasUpdated = true;

            this.value = (int) value;
        }
    }
}
