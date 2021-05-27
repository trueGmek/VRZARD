using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Grid = Systems.Grid;

namespace Screens.CalibrationScreen {
    public class LowStateMeasurement : MonoBehaviour, IFocusStateMeasurement {
        private const float EXAMINATION_DURATION = UiManager.TIME_OF_EXAMINATION;
        public GameObject instruction;
        public GameObject examinationTimer;
        public GameObject tillNextExaminationTimer;
        public GameObject components;
        private bool _countingExamination;
        private bool _countingUntilExamination;

        private Dictionary<GameObject, GameObject> _instancesOverPrefabs;

        private IFocusStateMeasurement _nextState;
        private float _timeExamination;
        private float _timeUntilExamination;

        public void Initialize() {
            _instancesOverPrefabs = new Dictionary<GameObject, GameObject>
                {{instruction, null}, {examinationTimer, null}, {tillNextExaminationTimer, null}};
            InstantiateUiElements();
            SetTillNextExaminationTimer();
            SetExaminationTimer();
            SetNextState();
        }

        public bool MyUpdate() {
            if (_countingUntilExamination && _timeUntilExamination > 0) {
                _timeUntilExamination -= Time.deltaTime;
                SetTillNextExaminationTimer();
                return false;
            }

            if (_countingUntilExamination && _timeUntilExamination <= 0) {
                _countingUntilExamination = false;
                _countingExamination = true;
                Grid.GameEvents.StartExamination();
                Grid.GameEvents.StartMeasurement();
                return false;
            }

            if (_countingExamination && _timeExamination > 0) {
                _timeExamination -= Time.deltaTime;
                SetExaminationTimer();
                return false;
            }

            if (_countingExamination && _timeExamination < 0) {
                Debug.Log("FINISHED THE RELAXED STATE");
                Grid.GameEvents.EndExamination();
                return true;
            }

            return false;
        }

        public IFocusStateMeasurement Transition() {
            Destroy();
            return _nextState;
        }

        public void SetNextState() {
            _nextState = FindObjectOfType<MediumStateMeasurement>();
        }

        private void InstantiateUiElements() {
            _instancesOverPrefabs[instruction] = Instantiate(instruction, components.transform, false);
            _instancesOverPrefabs[tillNextExaminationTimer] =
                Instantiate(tillNextExaminationTimer, components.transform, false);
            _instancesOverPrefabs[examinationTimer] = Instantiate(examinationTimer, components.transform, false);
            _timeUntilExamination = UiManager.TIME_BETWEEN_EXAMINATION;
            _timeExamination = EXAMINATION_DURATION;
            _countingUntilExamination = true;
        }

        private void Destroy() {
            _instancesOverPrefabs.ForEach(pair => { Destroy(pair.Value); });
        }

        private void StartMeasurement() {
            Debug.Log("[LowStateMeasurement] StartMeasurement not implemented");
        }

        private void SetExaminationTimer() {
            var timer = _instancesOverPrefabs[examinationTimer].transform.Find("TIMER");
            timer.GetComponent<TMP_Text>()
                .SetText(_timeExamination.ToString("N1", CultureInfo.InvariantCulture))
                ;
        }

        private void SetTillNextExaminationTimer() {
            _instancesOverPrefabs[tillNextExaminationTimer].GetComponent<TMP_Text>()
                .SetText(_timeUntilExamination.ToString("N0", CultureInfo.InvariantCulture))
                ;
        }
    }
}