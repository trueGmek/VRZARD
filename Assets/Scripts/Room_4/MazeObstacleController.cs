using UnityEngine;
using Grid = Systems.Grid;

namespace Room_4 {
    public class MazeObstacleController : MonoBehaviour {
        private static readonly int Open = Animator.StringToHash("open");
        public Animator[] animators;

        private Animator _current;

        public void setActive(int index) {
            animators[index].SetBool(Open, true);
            CloseObstaclesExcept(index);
        }

        public void setInactive(int index) {
            animators[index].SetBool(Open, false);
            CloseObstaclesExcept(index);
        }

        public void PlayLeverSound() {
            Grid.AudioManager.Play("Room4_lever_activate");
        }

        private void CloseObstaclesExcept(int index) {
            for (var i = 0; i < animators.Length; i++) {
                if (i == index) continue;

                var animator = animators[i];
                animator.SetBool(Open, false);
            }
        }
    }
}