using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Yahtzee {
    // Basic script to navigate in main menu
    public class MainMenu : MonoBehaviour {
        // Wait a bit before switching to other scene
        private IEnumerator AudioBeforeLoad(string sceneName) {
            yield return new WaitForSeconds(0.2f);
            SceneManager.LoadScene(sceneName);
        }

        // Load given scene
        public void LoadScene(string sceneName) {
            StartCoroutine(AudioBeforeLoad(sceneName));
        }
        
        // Quit game
        public void QuitGame() {
            Application.Quit();
        }
    }
}