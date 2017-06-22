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
            Telemetry.AddChannel<double>("ut");
            Telemetry.AddChannel<double>("altitude", "0000");
            Telemetry.AddChannel<double>("srf_speed", "0000.0");
            Telemetry.AddChannel<double>("static_pressure");
            Telemetry.AddChannel<int>("partcount");
        }

        int lastPartCount = 0;

        public void FixedUpdate()
        {
            ut = Planetarium.GetUniversalTime();

            Telemetry.Send("ut", ut);
            Telemetry.Send("altitude", FlightGlobals.ship_altitude);
            Telemetry.Send("srf_speed", FlightGlobals.ship_srfSpeed);

            double pressure = FlightGlobals.getStaticPressure();
            if (pressure != 0.0d)
                Telemetry.Send("static_pressure", pressure);

            if (lastPartCount != FlightGlobals.ActiveVessel.parts.Count)
            {
                lastPartCount = FlightGlobals.ActiveVessel.parts.Count;
                Telemetry.Send("partcount", lastPartCount);
            }
        }
    }
}

#endif
