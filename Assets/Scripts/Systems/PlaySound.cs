using System;
using UnityEngine;

namespace Systems {
    public class PlaySound : MonoBehaviour {
        public string soundName;
        public bool shouldPlay;

        public void Play() {
            Grid.AudioManager.Play(soundName);
        }

        private void Update() {
            if (!shouldPlay) return;
            Play();
            shouldPlay = false;
        }
    }
}