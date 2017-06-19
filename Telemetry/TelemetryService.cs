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
                    instance = new TelemetryService();

                return instance;
            }
        }


        private Dispatcher dispatcher = new Dispatcher();

        private DataSet mainDataset = null; // this should end up being a list or map of all output files
        private string mainDatasetFilename;
        private bool mainDatasetHeaderWritten = false;

        internal class TelemetrySettings
        {
            public char ColumnSeparator = '\t';

            public double WriteInterval = 1.0;
            public double FlushInterval = 5.0;

            public bool SkipUnUpdated = true;
        };


        TelemetrySettings Settings;


        private double lastWriteUT = 0.0;
        private double lastFlushUT = 0.0;


        public TelemetryService()
        {
            string assemblyName = Assembly.GetExecutingAssembly().GetName().ToString();
            mainDatasetFilename = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "csv");

            Settings = new TelemetrySettings(); // TODO: load this from a file or something
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

            // create a main dataset if it doesn't exist yet
            if (mainDataset == null)
                mainDataset = new DataSet(mainDatasetFilename, Settings);

            mainDataset.AddChannel(channel);
        }
        

        public void Send(string id, object value)
        {
            // on the first time calling send, we write out the main dataset header
            if (mainDataset != null && !mainDatasetHeaderWritten)
            {
                mainDatasetHeaderWritten = true;
                mainDataset.WriteHeader();
            }

            dispatcher.Send(id, value);
        }

        public void Update()
        {
            if (mainDataset == null)
                return;

            double ut = Planetarium.GetUniversalTime();

            if (ut > lastWriteUT + Settings.WriteInterval)
            {
                mainDataset.Write();
                lastWriteUT = ut;
            }

            if (ut > lastFlushUT + Settings.FlushInterval)
            {
                mainDataset.Flush();
                lastFlushUT = ut;
            }
        }
    }
}
