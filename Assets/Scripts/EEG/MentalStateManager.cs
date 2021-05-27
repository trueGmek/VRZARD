using System;
using UnityEngine;
using Grid = Systems.Grid;

namespace EEG {
    public class MentalStateManager : MonoBehaviour {
        public DataProcessing dataProcessing;

        [Tooltip("Time from activation of the relaxed state till next metal state check")]
        public float relaxedStatePeriod;

        private FocusState _currentFocusState = FocusState.Incorrect;
        private FocusState _previousFocusState = FocusState.Incorrect;

        private float _timeSinceLastMentalStateCheck;

        private void FixedUpdate() {
            CheckRelaxedState();
        }

        private void Awake() {
            Grid.GameEvents.OnEnterNormalState += () => { _currentFocusState = FocusState.Medium; };
        }

        private void CheckRelaxedState() {
            _timeSinceLastMentalStateCheck += Time.fixedDeltaTime;
            if (_currentFocusState != FocusState.Medium && _currentFocusState != FocusState.Incorrect &&
                HasRelaxedStateActivePeriodPassed())
                return;

            _timeSinceLastMentalStateCheck = 0f;

            LeaveFocusState(_previousFocusState);
            _previousFocusState = _currentFocusState;
            _currentFocusState = dataProcessing.GetFocusState();
            EnterFocusState(_currentFocusState);
        }

        private bool HasRelaxedStateActivePeriodPassed() {
            return !(_currentFocusState == FocusState.Low &&
                     _timeSinceLastMentalStateCheck >= relaxedStatePeriod);
        }

        private void EnterFocusState(FocusState currentFocusState) {
            switch (currentFocusState) {
                case FocusState.High:
                    Grid.GameEvents.EnterFocusedState();
                    break;
                case FocusState.Low:
                    Grid.GameEvents.EnterRelaxedState();
                    break;
                case FocusState.Medium:
                    if (_previousFocusState == FocusState.Medium || _previousFocusState == FocusState.Incorrect) return;
                    Grid.GameEvents.EnterNormalState();
                    break;
            }
        }

        private static void LeaveFocusState(FocusState previousFocusState) {
            switch (previousFocusState) {
                case FocusState.High:
                    Grid.GameEvents.ExitFocusedState();
                    break;
                case FocusState.Low:
                    Grid.GameEvents.ExitRelaxedState();
                    break;
                case FocusState.Medium:
                    Grid.GameEvents.ExitNormalState();
                    break;
            }
        }
    }
}