using Telekinesis;
using UnityEditor;
using UnityEngine;
using Valve.VR;

namespace Editor.Telekinesis {
    [CustomEditor(typeof(TelekinesisObject))]
    public class TelekinesisObjectEditor : UnityEditor.Editor {
        private TelekinesisObject _target;

        public override void OnInspectorGUI() {
            _target = (TelekinesisObject) target;
            if (_target.drawnToPoint != null) {
                _target.drawnToPoint.transform.position =
                    EditorGUILayout.Vector3Field("Drawn to point: ", _target.drawnToPoint.transform.position);
            }

            base.OnInspectorGUI();
        }
    }
}