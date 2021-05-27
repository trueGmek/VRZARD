//This is the post processing effect. Put it on your camera
//Any object that needs an outline, needs to be on a layer named 'Outline'
//You also need the two shaders below

using UnityEngine;

namespace Room_2 {
    public class OutlinePostEffect : MonoBehaviour {
        public Shader Matte;
        public Shader Outline;
        private Camera _mainCamera;
        private Material _material;
        private Camera _tempCamera;

        private void Awake() {
            _mainCamera = GetComponent<Camera>();
            _tempCamera = new GameObject("TempCamera").AddComponent<Camera>();
            _tempCamera.enabled = false;

            _material = new Material(Outline);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination) {
            _tempCamera.CopyFrom(_mainCamera);
            _tempCamera.clearFlags = CameraClearFlags.Color;
            _tempCamera.backgroundColor = Color.black;
            _tempCamera.cullingMask = 1 << LayerMask.NameToLayer("Outline");

            var
                tempRT = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.Default); //was R8
            tempRT.Create();

            _tempCamera.targetTexture = tempRT;
            _tempCamera.RenderWithShader(Matte, "");

            _material.SetTexture("_SceneTex", source);
            Graphics.Blit(tempRT, destination, _material);

            tempRT.Release();
        }
    }
}