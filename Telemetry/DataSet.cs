using System.IO;

namespace Telemetry
{
    internal class DataSet
    {
        private StreamWriter logfile_stream;

        internal DataSet(string outfile)
        {
            logfile_stream = new StreamWriter(outfile, false);

        }
    }
}
