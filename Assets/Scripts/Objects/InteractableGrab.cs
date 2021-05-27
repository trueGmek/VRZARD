using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Objects {
    public class InteractableGrab : MonoBehaviour {
        private readonly Hand.AttachmentFlags _attachmentFlags = Hand.defaultAttachmentFlags &
                                                                 ~Hand.AttachmentFlags.SnapOnAttach &
                                                                 ~Hand.AttachmentFlags.DetachOthers &
                                                                 ~Hand.AttachmentFlags.VelocityMovement;

        private Interactable _interactable;

        private void Awake() {
            _interactable = GetComponent<Interactable>();
        }

        private void HandHoverUpdate(Hand hand) {
            var startingGrabType = hand.GetGrabStarting();
            //TODO: Understand what the line bellow is for
            var isGrabEnding = hand.IsGrabEnding(gameObject);

            if (_interactable.attachedToHand == null && startingGrabType != GrabTypes.None) {
                hand.HoverLock(_interactable);
                hand.AttachObject(gameObject, startingGrabType, _attachmentFlags);
            }
            else if (isGrabEnding) {
                hand.DetachObject(gameObject);
                hand.HoverUnlock(_interactable);
            }
        }
    }
}