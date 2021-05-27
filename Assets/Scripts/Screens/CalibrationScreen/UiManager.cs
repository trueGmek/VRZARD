using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Grid = Systems.Grid;

namespace Screens.CalibrationScreen {
    public class UiManager : MonoBehaviour {
        public const float TIME_OF_EXAMINATION = 60f;
        public const float TIME_BETWEEN_EXAMINATION = 20f;

        private const string FIRST_LEVEL_NAME = "Room1";
        private const string SOUND_NAME_EXAMINATION_START = "StartExamination";
        private const string SOUND_NAME_EXAMINATION_END = "EndExamination";

        public GameObject startGameButton;
        public GameObject connectHeadsetButton;
        public Button startExaminationButton;

        private GameObject _components;
        private IFocusStateMeasurement _currentState;
        private bool _exitLoop;
        private Dictionary<GameObject, GameObject> _instancesOverPrefabs;

        private float _timeUntilNextExamination;

        public void Awake() {
            Grid.GameEvents.OnStartExamination += () => Grid.AudioManager.Play(SOUND_NAME_EXAMINATION_START);
            Grid.GameEvents.OnEndExamination += () => Grid.AudioManager.Play(SOUND_NAME_EXAMINATION_END);
            Grid.GameEvents.OnEndMeasurement += () => startGameButton.SetActive(true);
            Grid.GameEvents.OnHeadsetConnected += () => {
                startExaminationButton.interactable = true;
                Destroy(connectHeadsetButton);
            };
        }


        private void Update() {
            if (_currentState != null && !_exitLoop) {
                _exitLoop = _currentState.MyUpdate();
                if (_exitLoop) {
                    var nextState = _currentState.Transition();
                    nextState?.Initialize();
                    _currentState = nextState;
                    _exitLoop = false;
                }
            }
        }

        public void StartCalibration() {
            _currentState = FindObjectOfType<LowStateMeasurement>();
            _currentState.Initialize();
        }

        public void StartTheGame() {
            var loader = gameObject.AddComponent<SteamVR_LoadLevel>();
            loader.loadAsync = true;
            loader.levelName = FIRST_LEVEL_NAME;
            loader.Trigger();
        }
    }
}