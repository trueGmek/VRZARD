using Abilities;
using UnityEditor;
using UnityEngine;
using Grid = Systems.Grid;

namespace Editor.Abilities {
    [CustomEditor(typeof(AbilityManager))]
    public class AbilityManagerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            InitializeButtons();
        }

        private static void InitializeButtons() {
            if (GUILayout.Button("SetCurrentAbilityTelekinesis")) {
                Grid.GameEvents.SetCurrentAbilityTelekinesis();
            }

            if (GUILayout.Button("SetCurrentAbilityAlternativeVision")) {
                Grid.GameEvents.SetCurrentAbilityAlternativeVision();
            }

            if (GUILayout.Button("SetCurrentAbilityNone")) {
                Grid.GameEvents.SetCurrentAbilityNone();
            }

            if (GUILayout.Button("EnterRelaxedState")) {
                Grid.GameEvents.EnterRelaxedState();
            }

            if (GUILayout.Button("ExitRelaxedState")) {
                Grid.GameEvents.ExitRelaxedState();
            }

            if (GUILayout.Button("EnterNormalState")) {
                Grid.GameEvents.EnterNormalState();
            }
        }
    }
}