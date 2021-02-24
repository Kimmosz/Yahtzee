using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;

namespace Yahtzee {
    public class Dice : MonoBehaviour {
        // Declare dice state variables
        private int value = 0;
        private bool isLocked = false;

        // Declare UI variable
        [SerializeField] private Sprite[] diceImages;

        private void Start() {
            GetComponent<Image>().sprite = diceImages[0];
        }

        private void Update() {
            // Update UI dices 
            if (!isLocked) {
                GetComponent<Image>().color = Color.white;
            } else { 
                GetComponent<Image>().color = Color.grey;
            }
        }

        // Roll dice
        public void RollDice() {
            if (!isLocked) {
                value = UnityEngine.Random.Range(1, 7);
                GetComponent<Image>().sprite = diceImages[value];

            }
        }

        // Get value of dice
        public int GetValue() {
            return value;
        }

        // Reset lockstate of dice
        public void ResetLock() {
            isLocked = false;
            value = 0;
            GetComponent<Image>().sprite = diceImages[value];
        }

        // Switch lockstate of dice
        public void SwitchLock() {
            if (value != 0) {
                if (isLocked) {
                    isLocked = false;
                } else {
                    isLocked = true;
                }
            }
        }

        // Get the lockstate of dice
        public bool GetLockedState() {
            return isLocked;
        }
    }
}