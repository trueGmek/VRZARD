using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Grid = Systems.Grid;

namespace Screens.CalibrationScreen {
    public class HighStateMeasurement : MonoBehaviour, IFocusStateMeasurement {
        private const float EXAMINATION_DURATION = UiManager.TIME_OF_EXAMINATION;
        public GameObject instruction;
        public GameObject examinationTimer;
        public GameObject nextExaminationTimer;
        public GameObject components;
        private bool _countingExamination;
        private bool _countingUntilExamination;

        private Dictionary<GameObject, GameObject> _instancesOverPrefabs;

        private IFocusStateMeasurement _nextState;
        private float _timeExamination;
        private float _timeUntilExamination;

        public void Initialize() {
            InitializeInstancesOverPrefabs();
            InstantiateUiElements();
            SetNextExaminationTimer();
            SetExaminationTimer();
            SetNextState();
            _countingUntilExamination = true;
        }

        public bool MyUpdate() {
            if (_countingUntilExamination && _timeUntilExamination > 0) {
                _timeUntilExamination -= Time.deltaTime;
                SetNextExaminationTimer();
                return false;
            }

            if (_countingUntilExamination && _timeUntilExamination <= 0) {
                _countingUntilExamination = false;
                _countingExamination = true;
                Grid.GameEvents.StartExamination();
                return false;
            }

            if (_countingExamination && _timeExamination > 0) {
                _timeExamination -= Time.deltaTime;
                SetExaminationTimer();
                return false;
            }

            if (_countingExamination && _timeExamination < 0) {
                Grid.GameEvents.EndExamination();
                Grid.GameEvents.EndMeasurement();
                return true;
            }

            return false;
        }


        public void SetNextState() {
        }

        public IFocusStateMeasurement Transition() {
            Destroy();
            return _nextState;
        }

        private void InitializeInstancesOverPrefabs() {
            _instancesOverPrefabs = new Dictionary<GameObject, GameObject>
                {{instruction, null}, {examinationTimer, null}, {nextExaminationTimer, null}};
        }

        private void InstantiateUiElements() {
            _instancesOverPrefabs[instruction] = Instantiate(instruction, components.transform, false);
            _instancesOverPrefabs[examinationTimer] = Instantiate(examinationTimer, components.transform, false);
            _instancesOverPrefabs[nextExaminationTimer] =
                Instantiate(nextExaminationTimer, components.transform, false);
            _timeExamination = EXAMINATION_DURATION;
            _timeUntilExamination = UiManager.TIME_BETWEEN_EXAMINATION;
        }

        private void SetExaminationTimer() {
            var timer = _instancesOverPrefabs[examinationTimer].transform.Find("TIMER");
            timer.GetComponent<TMP_Text>()
                .SetText(_timeExamination.ToString("N1", CultureInfo.InvariantCulture))
                ;
        }

        private void SetNextExaminationTimer() {
            _instancesOverPrefabs[nextExaminationTimer].GetComponent<TMP_Text>()
                .SetText(_timeUntilExamination.ToString("N0", CultureInfo.InvariantCulture))
                ;
        }

        private void Destroy() {
            _instancesOverPrefabs.ForEach(pair => { Destroy(pair.Value); });
        }
    }
}