
using System;

namespace Telemetry
{
    internal class NumericChannel<T> : IChannel
        where T : IFormattable
    {
        public string Name { get; private set; }
        public string Format { get; private set; }

        public bool WasUpdated { get; private set; }

        private T value;

        public NumericChannel(string name, string format = null)
        {
            Name = name;
            Format = format;
        }

        public string Render()
        {
            WasUpdated = false;
            return Format == null ? value.ToString() : value.ToString(Format, null);
        }

        public void Send(object value)
        {
            if (value.GetType() != typeof(T))
                throw new InvalidOperationException("Trying to send a value of type `"
                    + value.GetType() + "` to a channel of type `" + typeof(T) + "`");

            WasUpdated = true;
            this.value = (T) value;
        }
    }
}
