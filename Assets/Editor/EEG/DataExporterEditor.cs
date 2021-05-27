using EEG;
using UnityEditor;
using UnityEngine;

namespace Editor.EEG {
    [CustomEditor(typeof(EegDataExporter))]
    public class DataExporterEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var dataExporter = target as EegDataExporter;
            base.OnInspectorGUI();
            if (GUILayout.Button("Save bound data to files")) {
                dataExporter.ExportEegDataToFile();
            }
        }
    }
}