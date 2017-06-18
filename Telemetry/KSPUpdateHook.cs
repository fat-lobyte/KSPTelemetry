using UnityEngine;

namespace Telemetry
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class KSPUpdateHook : MonoBehaviour
    {
        public void Update()
        {
            TelemetryService.Instance.Update();
        }
    }
}
