using UnityEngine;
using Grid = Systems.Grid;

namespace Room_2 {
    public class ExitTeleportPointActivator : MonoBehaviour {
        private void Awake() {
            Grid.GameEvents.OnOpenExitDoorForSecondRoom += () => gameObject.SetActive(true);
        }
    }
}