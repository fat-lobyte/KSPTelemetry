using System;
using System.Collections.Generic;
using System.IO;

namespace Telemetry
{
    internal class DataSet
    {
        private const string separator = "\t";
        private List<IChannel> channels = new List<IChannel>();

        private StreamWriter filestream;

        internal DataSet(string outfile)
        {
            filestream = new StreamWriter(outfile, false);
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
                filestream.Write(separator);
            }

            filestream.Write('\n');
        }

        public void Write()
        {
            foreach(IChannel channel in channels)
            {
                filestream.Write(channel.Render());
                filestream.Write(separator);
            }

            filestream.Write('\n'); 
        }

        internal void Flush()
        {
            filestream.Flush();
        }
    }
}
