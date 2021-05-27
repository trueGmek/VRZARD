using System.Globalization;
using EEG;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace Editor.EEG {
    [CustomEditor(typeof(DataProcessing))]
    public class DataProcessingEditor : UnityEditor.Editor {
        private const string TOTAL_AVERAGE_CHANNEL = "TOTAL AVERAGE PZ ";
        private DataProcessing _dataProcessing;

        public override void OnInspectorGUI() {
            _dataProcessing = (DataProcessing) target;

            base.OnInspectorGUI();
            EditorGUILayout.LabelField("FOCUS STATE", _dataProcessing.FocusState.ToString());
            EditorGUILayout.LabelField("alpha/theta",
                _dataProcessing.MovingAverageAlphaOverTheta.ToString(CultureInfo.InvariantCulture));
            DisplayAlphaOverTheta();
            DisplayBetaOverAlpha();
            EditorUtility.SetDirty(target);
        }

        private void DisplayBetaOverAlpha() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(BandPowerRatios.BetaOverAlpha.ToString(), EditorStyles.largeLabel,
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(TOTAL_AVERAGE_CHANNEL,
                _dataProcessing.AveragePzBetaOverTheta.ToString(CultureInfo.InvariantCulture));
            EditorGUILayout.EndHorizontal();
            DisplayBoundAverageOfEachChannel(BandPowerRatios.BetaOverAlpha);
        }

        private void DisplayAlphaOverTheta() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(BandPowerRatios.AlphaOverTheta.ToString(), EditorStyles.largeLabel,
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(TOTAL_AVERAGE_CHANNEL,
                _dataProcessing.AveragePzAlphaOverTheta.ToString(CultureInfo.InvariantCulture));
            EditorGUILayout.EndHorizontal();
            DisplayBoundAverageOfEachChannel(BandPowerRatios.AlphaOverTheta);
        }

        private void DisplayBoundAverageOfEachChannel(BandPowerRatios bandPowerRatio) {
            EditorGUILayout.DoubleField("AF3Value", _dataProcessing
                .boundAveragePackageOverBandPowerRatioType[bandPowerRatio]
                .Af3Value);
            EditorGUILayout.DoubleField("AF4Value", _dataProcessing
                .boundAveragePackageOverBandPowerRatioType[bandPowerRatio]
                .Af4Value);
            EditorGUILayout.DoubleField("PzValue", _dataProcessing
                .boundAveragePackageOverBandPowerRatioType[bandPowerRatio]
                .PzValue);
            EditorGUILayout.DoubleField("T7Value", _dataProcessing
                .boundAveragePackageOverBandPowerRatioType[bandPowerRatio]
                .T7Value);
            EditorGUILayout.DoubleField("T8Value", _dataProcessing
                .boundAveragePackageOverBandPowerRatioType[bandPowerRatio]
                .T8Value);
        }
    }
}