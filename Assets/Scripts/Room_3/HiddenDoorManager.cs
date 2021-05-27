using System.Linq;
using UnityEngine;
using Grid = Systems.Grid;

namespace Room_3 {
    public class HiddenDoorManager : MonoBehaviour {
        private static readonly int Open = Animator.StringToHash("Open");
        public Snap[] runes;
        public GameObject teleportArea2;
        private Animator _animator;

        public void Awake() {
            _animator = GetComponent<Animator>();
            Grid.DataExporter.NoteEvent("Enter Room 3");

        }

        private void Update() {
            if (runes.Any(rune => !rune.hasFinished)) return;
            ActivateDoorAnimation();
            PlayDoorOpenSound();
            teleportArea2.SetActive(true);
            Destroy(this);
        }

        private void PlayDoorOpenSound() {
            Grid.AudioManager.Play("Room3_open_hidden_door");
        }

        private void ActivateDoorAnimation() {
            _animator.SetTrigger(Open);
        }
    }
}