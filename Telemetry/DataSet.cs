using System;
using System.Collections.Generic;
using System.IO;

namespace Telemetry
{
    internal class DataSet
    {
        private readonly TelemetryService.TelemetrySettings settings;
        private List<IChannel> channels = new List<IChannel>();

        private StreamWriter filestream;

        internal DataSet(string outfile, TelemetryService.TelemetrySettings settings)
        {
            filestream = new StreamWriter(outfile, false);
            this.settings = settings;
        }

        public void AddChannel(IChannel channel)
        {
            channels.Add(channel);
        }

        public void WriteHeader()
        {
            foreach (IChannel channel in channels)
            {
                filestream.Write(channel.Name);
                filestream.Write(settings.ColumnSeparator);
            }

            filestream.Write('\n');
        }

        public void Write()
        {
            uint channelIdx = 0;

            foreach(IChannel channel in channels)
            {
                if (channelIdx++ > 0)
                    filestream.Write(settings.ColumnSeparator);

                if (channel.WasUpdated)
                    filestream.Write(channel.Render());
            }

            filestream.Write('\n'); 
        }

        internal void Flush()
        {
            filestream.Flush();
        }
    }
}
