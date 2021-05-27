using UnityEngine;
using Valve.VR.InteractionSystem;
using Grid = Systems.Grid;

namespace Room_2 {
    public class TableCollider : MonoBehaviour {
        private bool _isItFirstTime;
        private Transform _player;

        private void Awake() {
            _player = FindObjectOfType<Player>().transform;
        }

        private void FixedUpdate() {
            if (!_isItFirstTime && (_player.position - gameObject.transform.position).magnitude < 3f) {
                Grid.GameEvents.StartCardShuffleAnimation();
                _isItFirstTime = true;
            }
        }
    }
}