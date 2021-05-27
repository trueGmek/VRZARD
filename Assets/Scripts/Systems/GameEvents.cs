using System;
using UnityEngine;

namespace Systems {
    public class GameEvents : MonoBehaviour {
        private DataExporter _dataExporter;

        private void Awake() {
            _dataExporter = Grid.DataExporter;
        }

        public event Action OnHeadsetConnected;
        public event Action OnStartMeasurement;
        public event Action OnEndMeasurement;

        public event Action OnStartExamination;
        public event Action OnEndExamination;

        //General
        public event Action OnSetCurrentAbilityAlternativeVision;
        public event Action OnSetCurrentAbilityTelekinesis;

        public event Action OnSetCurrentAbilityNone;

        public event Action OnActivateTelekinesis;
        public event Action OnDeactivateTelekinesis;

        public event Action OnActivateAlternativeVision;
        public event Action OnDeactivateAlternativeVision;

        public event Action OnEnterRelaxedState;
        public event Action OnExitRelaxedState;

        public event Action OnEnterFocusedState;
        public event Action OnExitFocusedState;


        public event Action OnEnterNormalState;
        public event Action OnExitNormalState;

        //Room 1
        public event Action OnJailDoorOpen;

        //Room 2
        public event Action OnStartCardShuffleAnimation;
        public event Action OnOpenExitDoorForSecondRoom;

        //Room 3

        //Room 4
        public event Action OnOpenExitGateInFourthRoom;

        public void JailDoorOpen() {
            OnJailDoorOpen?.Invoke();
            _dataExporter.NoteEvent("JailDoorOpen");
        }

        public void EnterRelaxedState() {
            OnEnterRelaxedState?.Invoke();
            _dataExporter.NoteEvent("EnterRelaxedState");
        }

        public void StartCardShuffleAnimation() {
            OnStartCardShuffleAnimation?.Invoke();
            _dataExporter.NoteEvent("StartCardShuffleAnimation");
        }

        public void OpenExitDoorForSecondRoom() {
            OnOpenExitDoorForSecondRoom?.Invoke();
            _dataExporter.NoteEvent("OpenExitDoorForSecondRoom");
        }

        public void EnterFocusedState() {
            OnEnterFocusedState?.Invoke();
            _dataExporter.NoteEvent("EnterFocusedState");
        }

        public void EnterNormalState() {
            OnEnterNormalState?.Invoke();
            _dataExporter.NoteEvent("EnterNormalState");
        }

        public void ExitRelaxedState() {
            OnExitRelaxedState?.Invoke();
            _dataExporter.NoteEvent("ExitRelaxedState");
        }

        public void ExitFocusedState() {
            OnExitFocusedState?.Invoke();
            _dataExporter.NoteEvent("ExitFocusedState");
        }

        public void ExitNormalState() {
            OnExitNormalState?.Invoke();
        }

        public void StartMeasurement() {
            OnStartMeasurement?.Invoke();
            _dataExporter.NoteEvent("StartMeasurement");
        }

        public void EndMeasurement() {
            OnEndMeasurement?.Invoke();
            _dataExporter.NoteEvent("EndMeasurement");
        }

        public void StartExamination() {
            OnStartExamination?.Invoke();
            _dataExporter.NoteEvent("StartExamination");
        }

        public void EndExamination() {
            OnEndExamination?.Invoke();
            _dataExporter.NoteEvent("EndExamination");
        }

        public void HeadsetConnected() {
            OnHeadsetConnected?.Invoke();
            _dataExporter.NoteEvent("HeadsetConnected");
        }

        public void ActivateTelekinesis() {
            OnActivateTelekinesis?.Invoke();
        }

        public void DeactivateTelekinesis() {
            OnDeactivateTelekinesis?.Invoke();
            _dataExporter.NoteEvent("DeactivateTelekinesis");
        }

        public void ActivateAlternativeVision() {
            OnActivateAlternativeVision?.Invoke();
            _dataExporter.NoteEvent("ActivateAlternativeVision");
        }

        public void DeactivateAlternativeVision() {
            OnDeactivateAlternativeVision?.Invoke();
            _dataExporter.NoteEvent("DeactivateAlternativeVision");
        }

        public void SetCurrentAbilityAlternativeVision() {
            OnSetCurrentAbilityAlternativeVision?.Invoke();
            _dataExporter.NoteEvent("SetCurrentAbilityAlternativeVision");
        }

        public void SetCurrentAbilityTelekinesis() {
            OnSetCurrentAbilityTelekinesis?.Invoke();
            _dataExporter.NoteEvent("SetCurrentAbilityTelekinesis");
        }

        public void SetCurrentAbilityNone() {
            OnSetCurrentAbilityNone?.Invoke();
            _dataExporter.NoteEvent("SetCurrentAbilityNone");
        }

        public void OpenExitGateInFourthRoom() {
            OnOpenExitGateInFourthRoom?.Invoke();
            _dataExporter.NoteEvent("OpenExitGateInFourthRoom");
        }
    }
}