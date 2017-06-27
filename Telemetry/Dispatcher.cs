using System.Collections.Generic;

#if DEBUG
using UnityEngine;
#endif

namespace Telemetry
{
    class Dispatcher
    {
        private Dictionary<string, IChannel> channels = new Dictionary<string, IChannel>();

        public IChannel GetChannel(string id)
        {
            IChannel channel = null;
            if (channels.TryGetValue(id, out channel))
                return channel;
            else
                return null;
        }

        public void AddChannel(string id, IChannel channel)
        {
            channels.Add(id, channel);
        }

        public void Send(string path, object value)
        {
            IChannel channel = null;
            if (!channels.TryGetValue(path, out channel))
            {
#if DEBUG
                Debug.Log("Telemetry: tried to send value to unconfigured channel " + path);
#endif
                return;
            }

            channel.Send(value);
        }
    }
}
