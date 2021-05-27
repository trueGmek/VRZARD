using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Objects {
    public class ButtonTests : MonoBehaviour {
        private Interactable _interactable;
        private SteamVR_Skeleton_Poser _skeleton;


        private void Awake() {
            _interactable = GetComponent<Interactable>();
            _skeleton = GetComponent<SteamVR_Skeleton_Poser>();
        }

        private void OnHandHoverBegin(Hand hand) {
        }


        private void OnHandHoverEnd(Hand hand) {
        }

        public void OnButtonDown() {
            Debug.Log("BUTTON IS DOWN");
        }

        public void OnButtonUp() {
            Debug.Log("BUTTON IS UP");
        }

        public void OnButtonIsPressed() {
            Debug.Log("BUTTON IS PRESSED");
        }
    }
}