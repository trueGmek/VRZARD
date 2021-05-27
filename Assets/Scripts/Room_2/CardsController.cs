using UnityEngine;
using Grid = Systems.Grid;

namespace Room_2 {
    public class CardsController : MonoBehaviour {
        private static readonly int Shuffle = Animator.StringToHash("Shuffle");
        [SerializeField] private Animator[] animators;


        private void Awake() {
            Grid.GameEvents.OnStartCardShuffleAnimation += StartAnimation;
            Grid.GameEvents.OnStartCardShuffleAnimation += PlayShuffleSound;
        }

        private void PlayShuffleSound() {
            Grid.AudioManager.Play("Room2_shuffle_cards");
        }


        private void StartAnimation() {
            foreach (var animator in animators) animator.SetTrigger(Shuffle);
        }
    }
}