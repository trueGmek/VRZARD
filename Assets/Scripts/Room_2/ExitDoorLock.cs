using UnityEngine;
using Valve.VR.InteractionSystem;
using Grid = Systems.Grid;

namespace Room_2 {
    public class ExitDoorLock : MonoBehaviour {
        private CircularDrive _circularDrive;

        private void Awake() {
            _circularDrive = GetComponent<CircularDrive>();
            Grid.GameEvents.OnOpenExitDoorForSecondRoom += UnlockTheDoor;
            Grid.DataExporter.NoteEvent("Enter Room 2");

        }

        private void UnlockTheDoor() {
            _circularDrive.maxAngle = -140;
            _circularDrive.minAngle = -223;
        }
    }
}