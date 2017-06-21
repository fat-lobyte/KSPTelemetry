using UnityEngine;

namespace Telemetry
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class KSPHooks : MonoBehaviour
    {
        public void Start()
        {
            TelemetryService.Instance.InitCompleted();
        }

        public void Update()
        {
            TelemetryService.Instance.Update();
        }

        public void OnDestroy()
        {
            TelemetryService.Instance.Destroy();
            TelemetryService.Instance = null;
        }
    }
}
