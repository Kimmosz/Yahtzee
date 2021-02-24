using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Yahtzee {
    public class AudioManager : MonoBehaviour {
        // Declare audio variables
        public static AudioManager Instance { get; set; }
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource rollSound;
        [SerializeField] private AudioSource rollLockedSound;
        [SerializeField] private AudioSource buttonClickSound;

        private void Awake() {
            // Instance set up
            if (Instance == null) { Instance = this; } 
            else { Destroy(gameObject); }
        }

        // Play roll sound when player does press the roll button
        public void PlayRollSound(bool locked) {
            if (locked) {
                rollLockedSound.Play();
            } else {
                rollSound.Play();
            }
        }

        // Play button click sound
        public void PlayButtonClickSound() {
            buttonClickSound.Play();
        }

        // Get master volume which is necessary to check if sound is on or off
        public float GetMasterVolume() {
            float value;
            bool result = audioMixer.GetFloat("MasterVolume", out value);
            if (result) {
                return value;
            } else {
                return 0f;
            }
        }

        // Check if sound is on or off
        public bool IsAudioOn() {
            if (GetMasterVolume() == 0f) {
                return true;
            } else {
                return false;
            }
        }
        
        // Player is able to choose if the sound is on or off 
        public void ToggleMuteAudio() {
            if (IsAudioOn()) {
                audioMixer.SetFloat("MasterVolume", -80);
            } else {
                audioMixer.SetFloat("MasterVolume", 0);
            }
        }
    }
}