using System;
using System.Collections.Generic;
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
        }

        private Dispatcher dispatcher = new Dispatcher();

        private HashSet<DataSet> datasets = new HashSet<DataSet>();

        private DataSet mainDataset; // this should end up being a list or map of all output files

        public enum OpenMode
        {
            Overwrite,
            Append
        }

        internal class TelemetrySettings
        {
            public char ColumnSeparator = '\t';

            public double WriteInterval = 1.0;
            public double FlushInterval = 5.0;

            public bool SkipUnUpdated = true;

            public OpenMode OpenMode = OpenMode.Overwrite;
        };

        TelemetrySettings Settings;


        private double lastWriteUT = 0.0;
        private DateTime lastFlush = DateTime.UtcNow;


        public TelemetryService(string basepath)
        {
            Settings = new TelemetrySettings(); // TODO: load this from a file or something
            mainDataset = CreateDataSet(basepath);
        }

        private DataSet CreateDataSet(string basepath)
        {
            DataSet ds = new DataSet(basepath, Settings);
            datasets.Add(ds);

            return ds;
        }

        public void AddChannel<ChannelType>(string id, string format = null)
        {
            // do not add channels more than once
            if (dispatcher.GetChannel(id) != null)
                return;

            // create new channel
            IChannel channel;

            if (typeof(ChannelType) == typeof(float)
                || typeof(ChannelType) == typeof(double)
                || typeof(ChannelType) == typeof(short)
                || typeof(ChannelType) == typeof(ushort)
                || typeof(ChannelType) == typeof(int)
                || typeof(ChannelType) == typeof(uint)
                || typeof(ChannelType) == typeof(long)
                || typeof(ChannelType) == typeof(ulong))
            {
                channel = new NumericChannel<ChannelType>(id, format);
            }
            else
            {
                throw new NotImplementedException("Adding channels of type `"
                    + typeof(ChannelType) + "` is not (yet) supported");
            }

            // TODO: this is where we find / create the data sets. For now, there's just one
            DataSet dataset = mainDataset;

            // register new channel with data set
            dataset.AddChannel(channel);

            // register new channel with dispatcher
            dispatcher.AddChannel(id, channel);
        }


        public void Send(string id, object value)
        {
            dispatcher.Send(id, value);
        }

        internal void Start()
        {
            foreach (DataSet ds in datasets)
                ds.Open();
        }

        public void Shutdown()
        {
            foreach (DataSet ds in datasets)
                ds.Close();
        }

        public void Update()
        {
            double ut = Planetarium.GetUniversalTime();
            if (ut > lastWriteUT + Settings.WriteInterval)
            {
                foreach (DataSet ds in datasets)
                    ds.Write();

                lastWriteUT = ut;
            }

            DateTime now = DateTime.UtcNow;
            if (lastFlush.AddSeconds(Settings.FlushInterval) < now)
            {
                foreach (DataSet ds in datasets)
                    ds.Flush();

                lastFlush = now;
            }
        }
    }
}
