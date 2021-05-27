using UnityEngine;
using Grid = Systems.Grid;

namespace Room_3 {
    public class RuneLightController : MonoBehaviour {
        private Light _light;

        private void Awake() {
            SetUpLight();
            Grid.GameEvents.OnActivateAlternativeVision += ActivateLight;
            Grid.GameEvents.OnDeactivateAlternativeVision += DeactivateLight;
        }

        private void DeactivateLight() {
            if (_light != null) _light.enabled = false;
        }

        private void ActivateLight() {
            if (_light != null) _light.enabled = true;
        }

        private void SetUpLight() {
            _light = gameObject.AddComponent<Light>();
            _light.type = LightType.Point;
            _light.range = 0.5f;
            _light.color = Color.blue;
            _light.intensity = 1;
            _light.shadows = LightShadows.None;
            _light.enabled = false;
        }
    }
}