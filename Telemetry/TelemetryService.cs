using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    string basepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    instance = new TelemetryService(basepath);
                }

                return instance;
            }
        }

        private Dispatcher dispatcher = new Dispatcher();

        private HashSet<DataSet> datasets = new HashSet<DataSet>();

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
        private string basepath;

        public TelemetryService(string basepath)
        {
            Settings = new TelemetrySettings(); // TODO: load this from a file or something
            this.basepath = basepath;
        }

        private DataSet GetOrCreateDataSet(string channel_id)
        {
            // split channel_id into tokens that are separated by slashes
            string[] substrings = channel_id.Split('/');

            string channel_path_base;
            if (substrings.Length < 2)
                channel_path_base = "telemetry";
            else
                channel_path_base = substrings[0];

            string dataset_filename = Path.Combine(basepath, channel_path_base) + ".csv";

            // check if a dataset already exists for this file path. If not, create a new one
            DataSet ds = datasets.FirstOrDefault(d => d.Filename == dataset_filename);
            if (ds == null)
            {
                ds = new DataSet(dataset_filename, Settings);
                datasets.Add(ds);
            }

            return ds;
        }

        public void AddChannel<ChannelType>(string id, string format = null)
            where ChannelType : IFormattable
        {
            // do not add channels more than once
            if (dispatcher.GetChannel(id) != null)
                return;

            // split channel_id into tokens that are separated by slashes
            string[] substrings = id.Split(new char[] { '/' }, 2);
            string final_channel_id = substrings.Last();

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
                channel = new NumericChannel<ChannelType>(final_channel_id, format);
            }
            else
            {
                throw new NotImplementedException("Adding channels of type `"
                    + typeof(ChannelType) + "` is not (yet) supported");
            }

            // TODO: this is where we find / create the data sets. For now, there's just one
            DataSet dataset = GetOrCreateDataSet(id);

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
            lastFlush = DateTime.Now;
            lastWriteUT = 0.0;
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
