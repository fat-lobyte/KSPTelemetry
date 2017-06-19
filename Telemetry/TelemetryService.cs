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

        private DataSet mainDataset = null; // this should end up being a list or map
        private string mainDatasetFilename;
        private bool mainDatasetHeaderWritten = false;


        public double WriteInterval = 1.0;
        public double FlushInterval = 5.0;

        private double lastWriteUT = 0.0;
        private double lastFlushUT = 0.0;


        public TelemetryService()
        {
            string assemblyName = Assembly.GetExecutingAssembly().GetName().ToString();
            mainDatasetFilename = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "csv");
        }

        public void CreateChannel(string path, Type type, string format = null)
        {
            IChannel channel;

            if (type == typeof(double))
            {
                channel = new ChannelDouble(path, format);
            }
            else
            {
                throw new NotImplementedException("Adding channels of type `" + type + "` is not (yet) supported");
            }

            dispatcher.AddChannel(path, channel);

            // create a main dataset if it doesn't exist yet
            if (mainDataset == null)
                mainDataset = new DataSet(mainDatasetFilename);

            mainDataset.AddChannel(channel);
        }
        

        public void Send(string id, double value)
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

            if (ut > lastWriteUT + WriteInterval)
            {
                mainDataset.Write();
                lastWriteUT = ut;
            }

            if (ut > lastFlushUT + FlushInterval)
            {
                mainDataset.Flush();
                lastFlushUT = ut;
            }
        }
    }
}
