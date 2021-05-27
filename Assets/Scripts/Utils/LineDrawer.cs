using UnityEngine;

namespace Utils {
    public class LineDrawer : MonoBehaviour {
        public Shader lineShader;
        public Color lineColor;

        private LineRenderer _lineRenderer;

        public void DrawLine(Vector3 from, Vector3 to) {
            if (gameObject.GetComponent<LineRenderer>() == null) gameObject.AddComponent<LineRenderer>();
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            _lineRenderer.enabled = true;
            _lineRenderer.material = new Material(lineShader);
            _lineRenderer.startColor = _lineRenderer.endColor = lineColor;
            _lineRenderer.startWidth = _lineRenderer.endWidth = 0.01f;
            _lineRenderer.SetPosition(0, from);
            _lineRenderer.SetPosition(1, to);
        }

        public void RemoveLineRenderer() {
            if (!(_lineRenderer is null))
                _lineRenderer.enabled = false;
        }
    }
}