using UnityEngine;
using Valve.VR.InteractionSystem;
using Grid = Systems.Grid;

namespace Room_1 {
    public class JailDoor : MonoBehaviour {
        public CircularDrive CircularDrive;

        private void Awake() {
            Grid.GameEvents.OnJailDoorOpen += UnlockJailDoor;
            Grid.DataExporter.NoteEvent("Enter Room 1");
        }

        private void OnCollisionEnter(Collision other) {
            if (!other.gameObject.CompareTag("Key")) return;
            Grid.GameEvents.JailDoorOpen();
            Destroy(other.gameObject);
        }

        private void UnlockJailDoor() {
            CircularDrive.maxAngle = 88;
            Grid.AudioManager.Play("Room1_open_jail_doors");
        }
    }
}