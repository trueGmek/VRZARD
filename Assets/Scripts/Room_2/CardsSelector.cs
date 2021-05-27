using UnityEngine;
using Valve.VR.InteractionSystem;
using Grid = Systems.Grid;

namespace Room_2 {
    public class CardsSelector : MonoBehaviour {
        public bool correctCard;
        private Interactable _interactable;

        private void Awake() {
            _interactable = GetComponent<Interactable>();
            _interactable.highlightOnHover = true;
            Grid.GameEvents.OnOpenExitDoorForSecondRoom += () => Grid.AudioManager.Play("Room2_open_door");
        }


        private void HandHoverUpdate(Hand hand) {
            var startingGrabType = hand.GetGrabStarting();
            if (startingGrabType != GrabTypes.Grip) return;
            if (correctCard)
                Grid.GameEvents.OpenExitDoorForSecondRoom();
            else Grid.GameEvents.StartCardShuffleAnimation();
        }
    }
}