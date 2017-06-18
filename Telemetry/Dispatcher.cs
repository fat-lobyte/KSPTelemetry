using System.Collections.Generic;

#if DEBUG
using UnityEngine;
#endif

namespace Telemetry
{
    class Dispatcher
    {
        private Dictionary<string, IChannel> channels = new Dictionary<string, IChannel>();

        public void AddChannel(string path, IChannel channel)
        {
            channels.Add(path, channel);
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
