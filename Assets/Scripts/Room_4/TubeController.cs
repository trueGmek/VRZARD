using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Room_4 {
    public class TubeController : MonoBehaviour {
        public LinearMapping linearMapping;

        private void Update() {
            transform.rotation = Quaternion.Euler((linearMapping.value * 3f - 1f) * 90f, 0f, 90f);
        }
    }
}