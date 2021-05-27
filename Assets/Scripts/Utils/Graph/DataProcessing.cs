using System.Collections.Generic;
using System.Linq;
using EEG;
using UnityEngine;

namespace Utils.Graph {
    [RequireComponent(typeof(WindowGraph))]
    public class DataProcessing : MonoBehaviour {
        [Header("Return information")] public double average;
        public double boundedAverage;
        [Header("Parameters")] public double threshold;
        public int sampleSize = 10;
        public ParameterType parameterType;
        private readonly List<float> _analysisValues = new List<float>();
        private readonly List<float> _boundedValues = new List<float>();
        private WindowGraph _graph;
        private bool _isBoundedRecordingOn;


        private void Awake() {
            _graph = gameObject.GetComponent<WindowGraph>();
        }

        public void Update() {
            if (IsGraphNotEmpty()) {
                CalculateTotalAverage();
                if (IsPossibleToAnalyse()) InformAboutBrainActivity();
            }

            if (CanBoundedAverageBeCalculated()) boundedAverage = _boundedValues.Average();

            if (WasSpacePressed()) {
                _boundedValues.Clear();
                _isBoundedRecordingOn = !_isBoundedRecordingOn;
            }
        }

        private bool IsPossibleToAnalyse() {
            return _analysisValues.Count >= sampleSize;
        }


        private void InformAboutBrainActivity() {
            if (parameterType == ParameterType.AlphaOverTheta &&
                IsParameterBellowThreshold(_analysisValues.Average()))
                _analysisValues.Clear();
            else if (parameterType == ParameterType.BetaOverAlpha &&
                     IsParameterAboveThreshold(_analysisValues.Average()))
                _analysisValues.Clear();
        }


        private bool IsParameterBellowThreshold(float samplesAverage) {
            return samplesAverage <= threshold;
        }

        private bool IsParameterAboveThreshold(double samplesAverage) {
            return samplesAverage >= threshold;
        }

        private bool CanBoundedAverageBeCalculated() {
            return _isBoundedRecordingOn && _boundedValues.Count != 0;
        }

        private static bool WasSpacePressed() {
            return Input.GetKeyDown("space");
        }

        private bool IsGraphNotEmpty() {
            return _graph.valueList.Count != 0;
        }

        private void CalculateTotalAverage() {
            var values = _graph.valueList;
            average = values.Average();
        }

        public void OnAddValue(float addedValue) {
            _analysisValues.Add(addedValue);
            if (_isBoundedRecordingOn) _boundedValues.Add(addedValue);
        }
    }
}