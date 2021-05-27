using UnityEngine;
using Grid = Systems.Grid;

namespace EEG {
    public class SoundTest : MonoBehaviour {
        private void Awake() {
            Grid.GameEvents.OnEnterFocusedState += PlayFocusedAudioClip;
            Grid.GameEvents.OnEnterRelaxedState += PlayRelaxedAudioClip;
            Grid.GameEvents.OnEnterNormalState += PlayNormalAudioClip;
        }

        private void PlayNormalAudioClip() {
            Grid.AudioManager.Play("EnterNormalState");
        }

        private void PlayRelaxedAudioClip() {
            Grid.AudioManager.Play("EnterRelaxState");
        }

        private void PlayFocusedAudioClip() {
            Grid.AudioManager.Play("EnterFocusState");
        }
    }
}