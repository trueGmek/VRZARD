using EEG;
using UnityEditor;
using UnityEngine;

namespace Editor.EEG {
    [CustomEditor(typeof(ConnectionManager))]
    public class ConnectionManagerEditor : UnityEditor.Editor {
        private ConnectionManager _targetedConnectionManager;

        public override void OnInspectorGUI() {
            _targetedConnectionManager = (ConnectionManager) target;
            InitializeButtons();
            base.OnInspectorGUI();
        }

        private void InitializeButtons() {
            InitializeConnectButton();
        }

        private void InitializeConnectButton() {
            if (GUILayout.Button("Connect to the closest headset")) {
                _targetedConnectionManager.ConnectToHeadset();
            }
        }
    }
}