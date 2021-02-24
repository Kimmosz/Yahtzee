using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Yahtzee {
    public class MenuManager : MonoBehaviour {
        // Declare instance variable
        public static MenuManager Instance { get; set; }

        // Declare UI variables
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Text totalScoreText;
        [SerializeField] private GameObject soundOff;

        // Declare audio variable
        [SerializeField] private AudioManager audioManager;

        private void Awake() {
            // Instance set up
            if (Instance == null) { Instance = this; } 
            else { Destroy(gameObject); }

            if (!audioManager.IsAudioOn()) {
                soundOff.SetActive(true);
            }
        }

        // Checks if pause panel is currently active
        public bool GetPausePanelState() {
            if (pausePanel.activeSelf) {
                return true;
            } else {
                return false;
            }
        }
        
        // Toggle the sound on and off
        public void ToggleSound() {
            if (audioManager.IsAudioOn()) {
                soundOff.SetActive(true);
            } else {
                soundOff.SetActive(false);
            }
            audioManager.ToggleMuteAudio();
        }
        
        // Toggle pause panel
        public void TogglePausePanel(bool state) {
            pausePanel.SetActive(state);
        }

        // Toggle game over panel
        public void ToggleGameOverPanel(int score) {
            gameOverPanel.SetActive(true);
            totalScoreText.text = $"Your total score: {score}";
        }

        // Wait a bit before switching to other scene
        private IEnumerator AudioBeforeLoad(string sceneName) {
            yield return new WaitForSeconds(0.2f);
            SceneManager.LoadScene(sceneName);
        }

        // Load scene with given name
        public void LoadScene(string sceneName) {
            StartCoroutine(AudioBeforeLoad(sceneName));
        }
    }
}