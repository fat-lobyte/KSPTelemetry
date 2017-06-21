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
            bool append = false;
            if (settings.OpenMode == TelemetryService.OpenMode.Append)
                append = true;
            else if (settings.OpenMode == TelemetryService.OpenMode.Overwrite)
                append = false;

            filestream = new StreamWriter(outfile, append);
            this.settings = settings;
        }

        public void AddChannel(IChannel channel)
        {
            channels.Add(channel);
        }

        public void WriteHeader()
        {
            uint channelIdx = 0;

            foreach (IChannel channel in channels)
            {
                if (channelIdx++ > 0)
                    filestream.Write(settings.ColumnSeparator);

                filestream.Write(channel.Name);
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

        public void Close()
        {
            filestream.Close();
        }
    }
}
