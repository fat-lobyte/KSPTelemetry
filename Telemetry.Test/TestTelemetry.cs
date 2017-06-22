#if DEBUG

using Telemetry;
using UnityEngine;

namespace TelemetryTest
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class EntryUpdate : MonoBehaviour
    {
        private double ut;

        public void Awake()
        {
            TelemetryService.Instance.AddChannel<double>("ut");
            TelemetryService.Instance.AddChannel<double>("altitude", "0000");
            TelemetryService.Instance.AddChannel<double>("srf_speed", "0000.0");
            TelemetryService.Instance.AddChannel<double>("static_pressure");
            TelemetryService.Instance.AddChannel<double>("partcount");
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
