using System;
using System.IO;
using System.Reflection;

namespace Telemetry
{
    public class TelemetryService
    {
        private static TelemetryService instance;

        public static TelemetryService Instance {
            get
            {
                if (instance == null)
                {
                    string assemblyName = Assembly.GetExecutingAssembly().GetName().ToString();
                    string filename = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "csv");

                    instance = new TelemetryService(filename);
                }

                return instance;
            }

            internal set { instance = value; }
        }

        private Dispatcher dispatcher = new Dispatcher();

        private DataSet mainDataset; // this should end up being a list or map of all output files

        internal class TelemetrySettings
        {
            public char ColumnSeparator = '\t';

            public double WriteInterval = 1.0;
            public double FlushInterval = 5.0;

            public bool SkipUnUpdated = true;
        };

        TelemetrySettings Settings;


        private double lastWriteUT = 0.0;
        private DateTime lastFlush = DateTime.UtcNow;


        public TelemetryService(string basepath)
        {
            Settings = new TelemetrySettings(); // TODO: load this from a file or something
            mainDataset = new DataSet(basepath, Settings);
        }

        public void CreateChannel(string path, Type type, string format = null)
        {
            IChannel channel;

            if (type == typeof(double))
            {
                channel = new ChannelDouble(path, format);
            }
            else if (type == typeof(int))
            {
                channel = new ChannelInt(path, format);
            }
            else
            {
                throw new NotImplementedException("Adding channels of type `" + type + "` is not (yet) supported");
            }

            dispatcher.AddChannel(path, channel);
            mainDataset.AddChannel(channel);
        }
        

        public void Send(string id, object value)
        {
            dispatcher.Send(id, value);
        }

        internal void InitCompleted()
        {
            mainDataset.WriteHeader();
        }

        public void Update()
        {
            double ut = Planetarium.GetUniversalTime();
            if (ut > lastWriteUT + Settings.WriteInterval)
            {
                mainDataset.Write();
                lastWriteUT = ut;
            }

            DateTime now = DateTime.UtcNow;
            if (lastFlush.AddSeconds(Settings.FlushInterval) < now)
            {
                mainDataset.Flush();
                lastFlush = now;
            }
        }

        public void Destroy()
        {
            mainDataset.Close();
        }
    }
}
