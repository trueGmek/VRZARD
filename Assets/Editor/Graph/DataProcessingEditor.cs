using System.Globalization;
using UnityEditor;
using UnityEngine;
using Utils.Graph;

namespace Editor.Graph {
    [CustomEditor(typeof(DataProcessing))]
    public class DataProcessingEditor : UnityEditor.Editor {
        private DataProcessing _dataProcessing;

        public override void OnInspectorGUI() {
            _dataProcessing = target as DataProcessing;
            base.OnInspectorGUI();
        }
    }
}