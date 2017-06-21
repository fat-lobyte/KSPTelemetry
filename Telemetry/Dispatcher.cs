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
            if (channels.TryGetValue(id, out IChannel channel))
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

            if (!channels.TryGetValue(path, out IChannel channel))
            {
#if DEBUG
                Debug.Log("Telemetry: tried to send value to unconfigured channel");
#endif
                return;
            }

            channel.Send(value);
        }
    }
}
