using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Controller {
    public class LaserPointer : MonoBehaviour {
        public SteamVR_Input_Sources handType;
        public SteamVR_Behaviour_Pose controllerPose;
        public SteamVR_Action_Boolean teleportAction;
        public Shader lineShader;
        public GameObject teleportReticlePrefab;
        public Player player;
        public Transform headTransform;
        public LayerMask teleportMask;
        public Vector3 teleportReticleOffset;
        public int maxDistance = 100;
        private Vector3 _hitPoint;

        private GameObject _reticle;
        private bool _shouldTeleport;
        private Transform _teleportReticleTransform;

        private void Start() {
            _reticle = Instantiate(teleportReticlePrefab);
            _teleportReticleTransform = _reticle.transform;
        }

        private void Update() {
            if (teleportAction.GetState(handType)) {
                if (Physics.Raycast(controllerPose.transform.position, transform.forward, out var hit,
                    maxDistance, teleportMask)) {
                    _hitPoint = hit.point;
                    Debug.DrawLine(controllerPose.transform.position, _hitPoint, Color.red);
                    ShowLaser();
                    _reticle.SetActive(true);
                    _teleportReticleTransform.position = _hitPoint + teleportReticleOffset;
                    _shouldTeleport = true;
                }
            }
            else {
                var lineRenderer = GetComponent<LineRenderer>();
                if (lineRenderer != null)
                    Destroy(lineRenderer);
                _reticle.SetActive(false);
            }

            if (teleportAction.GetStateUp(handType) && _shouldTeleport) Teleport();
        }


        private void ShowLaser() {
            var controllerPosition = controllerPose.transform.position;
            if (GetComponent<LineRenderer>() == null) gameObject.AddComponent<LineRenderer>();
            var lineRenderer = gameObject.GetComponent<LineRenderer>();
            lineRenderer.material = new Material(lineShader);
            lineRenderer.startColor = lineRenderer.endColor = Color.red;
            lineRenderer.startWidth = lineRenderer.endWidth = 0.01f;
            lineRenderer.SetPosition(0, controllerPosition);
            lineRenderer.SetPosition(1, _hitPoint);
        }

        private void Teleport() {
            _shouldTeleport = false;
            _reticle.SetActive(false);
            var difference = player.trackingOriginTransform.position - headTransform.position;
            difference.y = 0;
            player.trackingOriginTransform.position = _hitPoint + difference;
        }
    }
}