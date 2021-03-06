﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Telemetry
{
    internal class DataSet
    {
        private readonly TelemetryService.TelemetrySettings settings;
        private List<IChannel> channels = new List<IChannel>();

        private StreamWriter filestream = null;
        public string Filename { get; private set; }


        internal DataSet(string filename, TelemetryService.TelemetrySettings settings)
        {
            Filename = filename;
            this.settings = settings;
        }

        public void AddChannel(IChannel channel)
        {
            channels.Add(channel);
        }

        public void Open()
        {
            if (filestream != null)
                return;

            bool append = false;
            if (settings.OpenMode == TelemetryService.OpenMode.Append)
                append = true;
            else if (settings.OpenMode == TelemetryService.OpenMode.Overwrite)
                append = false;

            filestream = new StreamWriter(Filename, append);

            WriteHeader();
        }

        private void WriteHeader()
        {
            if (filestream == null)
                return;

            uint channelIdx = 0;

            foreach (IChannel channel in channels)
            {
                if (channelIdx++ > 0)
                    filestream.Write(settings.ColumnSeparator);

                filestream.Write(channel.Name);
            }

            filestream.Write('\n');
        }

        StringBuilder lineStringBuilder = new StringBuilder();

        public void Write()
        {
            if (filestream == null)
                return;

            uint channelIdx = 0, updatedChannels = 0;
            foreach (IChannel channel in channels)
            {
                if (channelIdx++ > 0)
                    lineStringBuilder.Append(settings.ColumnSeparator);

                if (channel.WasUpdated)
                {
                    lineStringBuilder.Append(channel.Render());
                    ++updatedChannels;
                }
            }

            if (updatedChannels > 0)
            {
                filestream.WriteLine(lineStringBuilder.ToString());
            }

            lineStringBuilder.Length = 0;
        }

        internal void Flush()
        {
            if (filestream != null)
                filestream.Flush();
        }

        public void Close()
        {
            if (filestream != null)
            {
                filestream.Close();
                filestream = null;
            }
        }
    }
}
