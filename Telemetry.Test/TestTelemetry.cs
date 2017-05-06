#if DEBUG

using UnityEngine;

namespace Telemetry.Test
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class EntryUpdate : MonoBehaviour
    {
        private double ut;

        public void Update()
        {
            ut = Planetarium.GetUniversalTime();
            TelemetryService.Instance.send("ut", ut);
        }
    }
}

#endif
