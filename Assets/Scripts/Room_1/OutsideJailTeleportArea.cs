using UnityEngine;
using Valve.VR.InteractionSystem;
using Grid = Systems.Grid;

namespace Room_1 {
    public class OutsideJailTeleportArea : MonoBehaviour {
        private TeleportArea _teleportArea;

        private void Awake() {
            gameObject.SetActive(true);
            Grid.GameEvents.OnJailDoorOpen += ActivateTeleportArea;
            _teleportArea = gameObject.GetComponent<TeleportArea>();
            _teleportArea.locked = true;
        }

        private void ActivateTeleportArea() {
            _teleportArea.locked = false;
        }
    }
}