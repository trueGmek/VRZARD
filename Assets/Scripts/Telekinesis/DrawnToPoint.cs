using UnityEngine;

namespace Telekinesis {
    [RequireComponent(typeof(CharacterController), typeof(BoxCollider))]
    public class DrawnToPoint : MonoBehaviour {
        private BoxCollider _boxCollider;
        private CharacterController _characterController;

        private void Awake() {
            gameObject.layer = LayerMask.NameToLayer("Map Colider");
            _characterController = GetComponent<CharacterController>();
            _characterController.radius = 0.01f;
            _characterController.height = 0.01f;
            _boxCollider = GetComponent<BoxCollider>();
            _boxCollider.size = new Vector3(0.1f, 0.1f, 0.1f);
        }

        private void OnDrawGizmos() {
            Gizmos.DrawSphere(transform.position, 0.05f);
        }

        public void MovePositionBy(Vector3 distance) {
            _characterController.Move(distance);
        }
    }
}