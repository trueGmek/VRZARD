using Room_3;
using UnityEngine;
using Grid = Systems.Grid;

namespace Room_4 {
    public class ExitDoor : MonoBehaviour {
        public Snap snap;
        private readonly int _open = Animator.StringToHash("OPEN");
        private Animator _animator;
        private bool _hasBeenCalled;

        private void Awake() {
            _animator = GetComponent<Animator>();
            Grid.DataExporter.NoteEvent("Enter Room 4");
        }

        private void Update() {
            if (!snap.hasFinished || _hasBeenCalled) return;
            ActivateAudioVisualEffects();
            _hasBeenCalled = true;
        }

        private void ActivateAudioVisualEffects() {
            _animator.SetTrigger(_open);
            Grid.AudioManager.Play("Room4_open_gate");
        }
    }
}