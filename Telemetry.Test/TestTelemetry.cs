#if DEBUG

using UnityEngine;

namespace Telemetry.Test
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class EntryUpdate : MonoBehaviour
    {
        private double ut;

        public void Start()
        {
            TelemetryService.Instance.CreateChannel("ut", typeof(double));
            TelemetryService.Instance.CreateChannel("altitude", typeof(double));
            TelemetryService.Instance.CreateChannel("partcount", typeof(int));
        }

        int lastPartCount = 0;

        public void Update()
        {
            ut = Planetarium.GetUniversalTime();

            TelemetryService.Instance.Send("ut", ut);
            TelemetryService.Instance.Send("altitude", FlightGlobals.ship_altitude);

            if (lastPartCount != FlightGlobals.ActiveVessel.parts.Count)
            {
                lastPartCount = FlightGlobals.ActiveVessel.parts.Count;
                TelemetryService.Instance.Send("partcount", lastPartCount);
            }
        }
    }
}

#endif
