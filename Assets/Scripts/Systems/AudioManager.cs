using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems {
    public class AudioManager : MonoBehaviour {
        public Sound[] sounds;
        private readonly Dictionary<Sound, AudioSource> _audioSourcesOverSounds = new Dictionary<Sound, AudioSource>();

        private void Awake() {
            foreach (var sound in sounds) {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = sound.audioClip;
                audioSource.volume = sound.volume;
                audioSource.pitch = sound.pitch;
                _audioSourcesOverSounds[sound] = audioSource;
            }
        }

        public void Play(string soundName) {
            var searchedSound = Array.Find(sounds, sound => sound.name == soundName);
            _audioSourcesOverSounds[searchedSound].Play();
        }
    }
}