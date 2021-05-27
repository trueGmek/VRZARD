using UnityEngine;
using Valve.VR.InteractionSystem;
using Grid = Systems.Grid;

namespace Objects {
    [RequireComponent(typeof(Interactable))]
    public class InteractableAttach : MonoBehaviour {
        public Hand.AttachmentFlags attachmentFlags =
            Hand.AttachmentFlags.VelocityMovement | Hand.AttachmentFlags.DetachFromOtherHand;

        public GrabTypes grabWithThisType = GrabTypes.Grip;
        private Interactable _interactable;

        private void Awake() {
            _interactable = GetComponent<Interactable>();
        }

        private void HandHoverUpdate(Hand hand) {
            var startingGrabType = hand.GetGrabStarting();

            if (_interactable.attachedToHand == null && startingGrabType == grabWithThisType) {
                hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
                Grid.DataExporter.NoteEvent("Grabbed " + gameObject.name);
            }
        }

        private void HandAttachedUpdate(Hand hand) {
            if (hand.IsGrabEnding(gameObject)) {
                hand.DetachObject(gameObject);
                Grid.DataExporter.NoteEvent("Letting go of " + gameObject.name);
            }
        }
    }
}