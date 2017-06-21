using UnityEngine;

namespace Telemetry
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class KSPHooks : MonoBehaviour
    {
        public void Start()
        {
            TelemetryService.Instance.Start();
        }

        public void Update()
        {
            TelemetryService.Instance.Update();
        }

        public void OnDestroy()
        {
            TelemetryService.Instance.Shutdown();
        }
    }
}
