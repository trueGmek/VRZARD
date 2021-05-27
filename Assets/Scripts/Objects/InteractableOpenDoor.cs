using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Objects {
    [RequireComponent(typeof(Interactable))]
    public class InteractableOpenDoor : MonoBehaviour {
        private const float FORCE_MULTIPLIER = 150f;

        public Hand.AttachmentFlags attachmentFlags;
        private float _angle;
        private Vector3 _cross;
        private Vector3 _force;
        private bool _holdingHandle;
        private Interactable _interactable;
        private Rigidbody _rigidbody;

        private void Awake() {
            _interactable = GetComponent<Interactable>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update() {
            if (_holdingHandle) _rigidbody.angularVelocity = _cross * (_angle * FORCE_MULTIPLIER);
        }

        private void HandHoverUpdate(Hand hand) {
            if (hand.GetGrabStarting() == GrabTypes.Grip) {
                _holdingHandle = true;
                hand.HoverLock(_interactable);
                hand.HideSkeleton();
                var positionHand = hand.transform.position;
                var doorPivotToHand = positionHand - transform.parent.position;
                doorPivotToHand.y = 0;

                _force = positionHand - transform.position;

                _cross = Vector3.Cross(doorPivotToHand, _force);
                _angle = Vector3.Angle(doorPivotToHand, _force);
            }

            if (hand.GetGrabEnding() == GrabTypes.Grip) {
                _holdingHandle = false;
                hand.HoverUnlock(_interactable);
                hand.ShowSkeleton();
            }
        }

        private void OnHandHoverEnd() {
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}