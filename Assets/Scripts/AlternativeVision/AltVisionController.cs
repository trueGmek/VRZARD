using UnityEngine;
using UnityEngine.Rendering.Universal;
using Grid = Systems.Grid;

namespace AlternativeVision {
    [RequireComponent(typeof(Camera))]
    public class AltVisionController : MonoBehaviour {
        public string layerName;
        private int _alternativeVisionCullingMask;

        private Camera _camera;

        private int _originalCullingMask;

        private void Awake() {
            Grid.GameEvents.OnActivateAlternativeVision += ActivateEffect;
            Grid.GameEvents.OnDeactivateAlternativeVision += DeactivateEffect;
            _camera = gameObject.GetComponent<Camera>();
            _originalCullingMask = _camera.cullingMask;
            _alternativeVisionCullingMask = _originalCullingMask ^ (1 << LayerMask.NameToLayer(layerName));
        }

        private void DeactivateEffect() {
            DeactivatePostProcessingEffects();
            HideSecretObjects();
        }

        private void ActivateEffect() {
            ActivatePostProcessingEffects();
            ShowSecretObjects();
            PlayActivationSound();
        }

        private void PlayActivationSound() {
            Grid.AudioManager.Play("AVision_activate");
        }

        private static void ActivatePostProcessingEffects() {
            Grid.Volume.profile.TryGet(out ColorAdjustments colorAdjustment);
            colorAdjustment.active = true;

            Grid.Volume.profile.TryGet(out Bloom bloom);
            bloom.active = true;
        }

        private static void DeactivatePostProcessingEffects() {
            Grid.Volume.profile.TryGet(out ColorAdjustments colorAdjustment);
            colorAdjustment.active = false;

            Grid.Volume.profile.TryGet(out Bloom bloom);
            bloom.active = false;
        }

        private void ShowSecretObjects() {
            _camera.cullingMask = _alternativeVisionCullingMask;
        }

        private void HideSecretObjects() {
            _camera.cullingMask = _originalCullingMask;
        }
    }
}