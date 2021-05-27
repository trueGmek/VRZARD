using UnityEngine;

namespace Utils {
    public class DDOL : MonoBehaviour {
        public void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }
}