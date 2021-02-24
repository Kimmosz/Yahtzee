using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yahtzee {
    public class Scoreblock : MonoBehaviour {
        // Declare gameplay variables
        public static Scoreblock Instance { get; set; } 
        public int ace = 0, two = 0, three = 0, four = 0, five = 0, six = 0, bonusScore = 0, totalScoreLeft = 0, threeOfAKind = 0, fourOfAKind = 0, fullHouse = 0, smallStraight = 0, largeStraight = 0, yahtzee = 0, chance = 0, totalScoreRight = 0, totalScore = 0;
        private bool gotYathzee = false, gotBonus = false;

        private void Awake() {
            // Instance set up
            if (Instance == null) { Instance = this; }
            else { Destroy(gameObject); }
        }

        // Calculate the current total score
        public void CalculateTotalScore() {
            totalScoreLeft = ace + two + three + four + five + six + bonusScore;
            // Automatically adds 35 bonus points when player has reached 63 points on the left side
            if (totalScoreLeft >= 63 && !gotBonus) {
                gotBonus = true;
                bonusScore = 35;
                totalScoreLeft += bonusScore;
            }
            totalScoreRight = threeOfAKind + fourOfAKind + fullHouse + smallStraight + largeStraight + yahtzee + chance;
            totalScore = totalScoreLeft + totalScoreRight;
        }

        // Checks if player currently has Yahtzee
        public bool GetYathzeeState() {
            return gotYathzee;
        }

        // Sets if player currently has Yahtzee
        public void SetYathzeeState(bool state) {
            gotYathzee = state;
        }
    }
}