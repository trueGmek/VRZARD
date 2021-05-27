using System.Collections.Generic;
using UnityEngine;

namespace AlternativeVision {
    public class MaterialsMenager : MonoBehaviour {
        public Camera activeCamera;
        public List<Material> materials;
        public float alpha;

        private void Update() {
            foreach (var material in materials)
                material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
        }
    }
}