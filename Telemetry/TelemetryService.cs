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


        private StreamWriter logfile_stream;

        public TelemetryService()
        {
            string assemblyName = Assembly.GetExecutingAssembly().GetName().ToString();
            string fileName = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "log");

            logfile_stream = new StreamWriter(fileName, false);
        }


        public void send(string id, double value)
        {
            logfile_stream.WriteLine(id + ": " + value);
            logfile_stream.Flush();
        }
    }
}
