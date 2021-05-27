using System;
using Systems;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;
using Grid = Systems.Grid;

namespace Telekinesis {
    public class TelekinesisController : MonoBehaviour {
        private static TelekinesisObject telekinesisObject;
        public SteamVR_Behaviour_Pose controllerPose;

        [Header("Control buttons")] public SteamVR_Action_Boolean trigger;
        public SteamVR_Action_Boolean tapTouchpad;

        [Header("Controller coefficients")] public float radius = 0.01f;
        public float differenceVectorMultiplayer = 0.05f;
        public SteamVR_LaserPointer laserPointer;
        private Vector3 _diff = Vector3.zero;

        private bool _isInRelaxedState;

        private Vector3 _previousControllerPosition;
        private Vector3 _sphereCastPosition;

        private SteamVR_Input_Sources _thisController;

        private bool _wasFound;


        private void Update() {
            if (trigger.state && trigger.activeDevice == _thisController) MoveTarget(CalculateVectorOutsideSphere());
        }

        private void OnEnable() {
            trigger.onStateDown += SetPosition();
            trigger.onStateUp += ZeroPreviousControllerPosition();
            laserPointer.PointerClick += ApplyLevitator;

            _previousControllerPosition = controllerPose.transform.position;
            _thisController = controllerPose.inputSource;

            Grid.GameEvents.OnActivateTelekinesis += PlayActivationSound;
            Grid.GameEvents.OnActivateTelekinesis += NoteEvent;
            Grid.GameEvents.OnActivateTelekinesis += ActivateTelekinesisObject;
            Grid.GameEvents.OnActivateTelekinesis += () => {
                if (telekinesisObject != null) DeactivateLaserPointer();
            };

            Grid.GameEvents.OnDeactivateTelekinesis += ActivateLaserPointer;
            Grid.GameEvents.OnDeactivateTelekinesis += DeactivateTelekinesisObject;

            Grid.GameEvents.OnSetCurrentAbilityTelekinesis += ActivateLaserPointer;
            Grid.GameEvents.OnSetCurrentAbilityNone += DeactivateLaserPointer;
            Grid.GameEvents.OnSetCurrentAbilityAlternativeVision += DeactivateLaserPointer;

            if (Grid.GameManager.gameMode == GameMode.WithoutEeg) {
                tapTouchpad.onStateDown += ToggleInvokeRelaxedStateEvent();
            }

            DeactivateLaserPointer();
        }

        private void OnDisable() {
            trigger.onStateDown -= SetPosition();
            trigger.onStateUp -= ZeroPreviousControllerPosition();
            if (Grid.GameManager.gameMode == GameMode.WithoutEeg) {
                tapTouchpad.onStateDown -= ToggleInvokeRelaxedStateEvent();
            }
        }

        private void OnDrawGizmos() {
            if (trigger.state == false || trigger.activeDevice != _thisController) return;
            Gizmos.DrawLine(_previousControllerPosition, controllerPose.transform.position);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(controllerPose.transform.position, 0.04f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_previousControllerPosition, 0.02f);
            Gizmos.DrawWireSphere(_previousControllerPosition, radius);

            if (!(_diff.magnitude > radius)) return;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_sphereCastPosition, 0.02f);
            Gizmos.DrawLine(_sphereCastPosition, controllerPose.transform.position);
        }

        private void PlayActivationSound() {
            Grid.AudioManager.Play("Telekinesis_activate");
        }

        private void NoteEvent() {
            Grid.DataExporter.NoteEvent("ActivateTelekinesis on " + telekinesisObject.name);
        }

        private void ApplyLevitator(object sender, PointerEventArgs pointerEventArgs) {
            if (pointerEventArgs.target.GetComponent<Rigidbody>() == null) return;
            var temp = pointerEventArgs.target.GetComponent<TelekinesisObject>();
            if (temp == null) {
                temp = pointerEventArgs.target.gameObject.AddComponent<TelekinesisObject>();
                temp.Deactivate();
            }

            Grid.DataExporter.NoteEvent("Selecting new object for levitation: " + temp.name);
            telekinesisObject = temp;
        }

        private void MoveTarget(Vector3 distanceVector3) {
            if (telekinesisObject == null || telekinesisObject.IsActive() == false) return;
            telekinesisObject.drawnToPoint.MovePositionBy(distanceVector3);
        }

        private Vector3 CalculateVectorOutsideSphere() {
            _diff = controllerPose.transform.position - _previousControllerPosition;
            if (!(_diff.magnitude > radius)) return Vector3.zero;

            var phi = Math.Atan2(_diff.y, _diff.x);
            var theta = Math.Atan(Math.Sqrt(_diff.x * _diff.x + _diff
                .y * _diff.y) / _diff.z);
            phi = MathMod(phi, 2 * Math.PI);
            theta = MathMod(theta, Math.PI);
            _sphereCastPosition = new Vector3(
                (float) (radius * Math.Sin(theta) * Math.Cos(phi)) + _previousControllerPosition.x,
                (float) (radius * Math.Sin(theta) * Math.Sin(phi)) + _previousControllerPosition.y,
                (float) (radius * Math.Cos(theta)) + _previousControllerPosition.z);
            return differenceVectorMultiplayer * (controllerPose.transform.position - _sphereCastPosition);
        }


        private SteamVR_Action_Boolean.StateDownHandler SetPosition() {
            return (action, source) => { _previousControllerPosition = controllerPose.transform.position; };
        }

        private SteamVR_Action_Boolean.StateUpHandler ZeroPreviousControllerPosition() {
            return (action, source) => { _previousControllerPosition = controllerPose.transform.position; };
        }

        private SteamVR_Action_Boolean.StateDownHandler ToggleInvokeRelaxedStateEvent() {
            return (action, source) => {
                if (_isInRelaxedState == false) {
                    Grid.GameEvents.EnterRelaxedState();
                    _isInRelaxedState = true;
                }
                else {
                    Grid.GameEvents.ExitRelaxedState();
                    _isInRelaxedState = false;
                }
            };
        }

        private void DeactivateTelekinesisObject() {
            if (telekinesisObject == null) return;
            if (telekinesisObject.IsActive()) telekinesisObject.Deactivate();
        }

        private void ActivateTelekinesisObject() {
            if (telekinesisObject == null) return;
            if (telekinesisObject.IsActive() == false) telekinesisObject.Activate();
        }

        private void DeactivateLaserPointer() {
            laserPointer.active = false;
        }

        private void ActivateLaserPointer() {
            laserPointer.active = true;
        }

        private static double MathMod(double a, double b) {
            return (a + b) % b;
        }
    }
}