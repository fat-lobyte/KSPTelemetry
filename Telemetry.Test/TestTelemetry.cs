#if DEBUG

using UnityEngine;

namespace Telemetry.Test
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class EntryUpdate : MonoBehaviour
    {
        private double ut;

        public void Awake()
        {
            TelemetryService.Instance.CreateChannel("ut", typeof(double));
            TelemetryService.Instance.CreateChannel("altitude", typeof(double), "0000");
            TelemetryService.Instance.CreateChannel("srf_speed", typeof(double), "0000.0");
            TelemetryService.Instance.CreateChannel("static_pressure", typeof(double));
            TelemetryService.Instance.CreateChannel("partcount", typeof(int));
        }

        int lastPartCount = 0;

        public void FixedUpdate()
        {
            ut = Planetarium.GetUniversalTime();

            TelemetryService.Instance.Send("ut", ut);
            TelemetryService.Instance.Send("altitude", FlightGlobals.ship_altitude);
            TelemetryService.Instance.Send("srf_speed", FlightGlobals.ship_srfSpeed);

            double pressure = FlightGlobals.getStaticPressure();
            if (pressure != 0.0d)
            TelemetryService.Instance.Send("static_pressure", pressure);

            if (lastPartCount != FlightGlobals.ActiveVessel.parts.Count)
            {
                lastPartCount = FlightGlobals.ActiveVessel.parts.Count;
                TelemetryService.Instance.Send("partcount", lastPartCount);
            }
        }
    }
}

#endif
