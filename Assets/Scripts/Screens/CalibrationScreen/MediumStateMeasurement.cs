using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Screens.CalibrationScreen {
    public class MediumStateMeasurement : MonoBehaviour, IFocusStateMeasurement {
        private const float EXAMINATION_DURATION = UiManager.TIME_OF_EXAMINATION;
        public GameObject instructionPrefab;
        public GameObject examinationTimerPrefab;
        public GameObject tillNextExaminationTimerPrefab;
        public GameObject videoPrefab;

        public GameObject components;
        private bool _countingExamination;
        private bool _countingUntilExamination;
        private Dictionary<GameObject, GameObject> _instancesOverPrefabs;

        private IFocusStateMeasurement _nextState;
        private float _timeExamination;
        private float _timeUntilExamination;

        public void Initialize() {
            _instancesOverPrefabs = new Dictionary<GameObject, GameObject> {
                {instructionPrefab, null}, {examinationTimerPrefab, null}, {tillNextExaminationTimerPrefab, null},
                {videoPrefab, null}
            };
            SetUpUiPrefabs();
            SetNextState();
        }

        public void SetNextState() {
            _nextState = FindObjectOfType<HighStateMeasurement>();
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
                _instancesOverPrefabs[videoPrefab] = Instantiate(videoPrefab, components.transform, false);
                return false;
            }

            if (_countingExamination && _timeExamination > 0) {
                _timeExamination -= Time.deltaTime;
                SetExaminationTimer();
                return false;
            }

            return _countingExamination && _timeExamination < 0;
        }

        public IFocusStateMeasurement Transition() {
            Debug.Log("FINISHED THE MEDIUM STATE");
            Destroy();
            return _nextState;
        }

        private void SetUpUiPrefabs() {
            InstantiateUiPrefabs();
            SetTillNextExaminationTimer();
            SetExaminationTimer();
            _countingUntilExamination = true;
        }

        private void InstantiateUiPrefabs() {
            _instancesOverPrefabs[instructionPrefab] = Instantiate(instructionPrefab, components.transform, false);
            _instancesOverPrefabs[examinationTimerPrefab] =
                Instantiate(examinationTimerPrefab, components.transform, false);
            _instancesOverPrefabs[tillNextExaminationTimerPrefab] =
                Instantiate(tillNextExaminationTimerPrefab, components.transform, false);
            _timeUntilExamination = UiManager.TIME_BETWEEN_EXAMINATION;
            _timeExamination = EXAMINATION_DURATION;
        }

        private void SetExaminationTimer() {
            var timer = _instancesOverPrefabs[examinationTimerPrefab].transform.Find("TIMER");
            timer.GetComponent<TMP_Text>()
                .SetText(_timeExamination.ToString("N1", CultureInfo.InvariantCulture))
                ;
        }

        private void SetTillNextExaminationTimer() {
            _instancesOverPrefabs[tillNextExaminationTimerPrefab].GetComponent<TMP_Text>()
                .SetText(_timeUntilExamination.ToString("N0", CultureInfo.InvariantCulture))
                ;
        }

        private void Destroy() {
            _instancesOverPrefabs.ForEach(pair => { Destroy(pair.Value); });
        }
    }
}