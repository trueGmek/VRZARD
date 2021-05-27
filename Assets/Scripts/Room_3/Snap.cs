using System;
using Objects;
using Telekinesis;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Room_3 {
    [RequireComponent(typeof(CapsuleCollider))]
    public class Snap : MonoBehaviour {
        public Transform destinationHelper;
        public float positionDelta = 0.01f;
        public float rotationDelta = 0.001f;
        public float marginPosition = 0.1f;
        public float marginRotation = 0.1f;
        public bool hasFinished;

        private Vector3 _destinedPosition;
        private Quaternion _destinedRotation;

        private void Start() {
            _destinedRotation = destinationHelper.rotation;
            _destinedPosition = destinationHelper.position;
            Destroy(destinationHelper.gameObject);
        }

        private void OnTriggerExit(Collider other) {
            other.attachedRigidbody.isKinematic = false;
        }

        private void OnTriggerStay(Collider other) {
            if (hasFinished) return;
            if (!other.CompareTag("Key")) return;
            other.attachedRigidbody.isKinematic = true;
            if (AreVectorsAlmostEqual(other.transform.position, _destinedPosition, marginPosition) &&
                AreQuaternionsAlmostEqual(other.transform.rotation, _destinedRotation, marginRotation)) {
                hasFinished = true;
                RemoveInteractions(other);
                return;
            }

            LerpTransform(other);
        }

        private static void RemoveInteractions(Collider other) {
            var interactable = other.GetComponent<Interactable>();
            var interactableAttach = other.GetComponent<InteractableAttach>();
            if (interactable.attachedToHand != null) interactable.attachedToHand.DetachObject(interactable.gameObject);

            var telekinesisObject = other.GetComponent<TelekinesisObject>();

            if (telekinesisObject != null) {
                telekinesisObject.Deactivate();
                Destroy(telekinesisObject);
            }

            Destroy(interactableAttach);
            Destroy(interactable);
            Destroy(other.attachedRigidbody);
        }

        private void LerpTransform(Component other) {
            Transform temp;
            (temp = other.transform).position =
                Vector3.MoveTowards(other.transform.position, _destinedPosition, positionDelta);
            other.transform.rotation = Quaternion.Lerp(temp.rotation, _destinedRotation, rotationDelta);
        }

        private static bool AreVectorsAlmostEqual(Vector3 v1, Vector3 v2, float margin) {
            return Math.Abs(v1.x - v2.x) < margin && Math.Abs(v1.y - v2.y) < margin &&
                   Math.Abs(v1.z - v2.z) < margin;
        }

        private static bool AreQuaternionsAlmostEqual(Quaternion q1, Quaternion q2, float margin) {
            return Math.Abs(Quaternion.Angle(q1, q2)) < margin;
        }
    }
}