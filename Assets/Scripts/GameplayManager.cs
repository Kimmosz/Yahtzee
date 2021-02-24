using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;

namespace Yahtzee {
    public class GameplayManager : MonoBehaviour {
        // Declare gameplay variables
        [SerializeField] private Dice[] dices;
        [SerializeField] private Scoreblock scoreblock;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private MenuManager menuManager;

        // Declare UI variables
        // Scoreblock UI
        [SerializeField] private Text acesText, twosText, threesText, foursText, fivesText, sixesText, bonusScoreText, totalscoreLeftText, threeOfAKindText, fourOfAKindText, fullHouseText, smallStraightText, largeStraightText, yahtzeeText, chanceText, totalScoreRightText, totalScoreText;
        // Gameplay UI
        [SerializeField] private Text rollText;
        [SerializeField] private Text statusText;
        [SerializeField] private Text eraseButtonText;
        [SerializeField] private GameObject eraseButton;
        [SerializeField] private GameObject endRollsButton;

        // Declare gameplay variables
        private bool rolled, canFill, canErase;
        private int turn = 0;
        private int rolls = 0;
        private int[] currentValues = new int[5];

        private void Start() {
            // Get instances and start new turn
            audioManager = AudioManager.Instance;
            scoreblock = Scoreblock.Instance;
            menuManager = MenuManager.Instance;
            NewTurn();
        }

        private void Update() {
            // Show or hide buttons when player can fill  
            if (canFill) {
                endRollsButton.SetActive(false);
                eraseButton.SetActive(true);
            } else {
                endRollsButton.SetActive(true);
                eraseButton.SetActive(false);
            }
         
            // Press escape to toggle pause menu
            if (Input.GetKeyDown(KeyCode.Escape)) {
                audioManager.PlayButtonClickSound();
                menuManager.TogglePausePanel(!menuManager.GetPausePanelState());
            }
            if (Input.GetKeyDown(KeyCode.M)) {
                audioManager.PlayButtonClickSound();
                menuManager.ToggleSound();
            }
        }

        // Update score UI 
        private void SetTotalScoreFields() {
            totalscoreLeftText.text = scoreblock.totalScoreLeft.ToString();
            totalScoreRightText.text = scoreblock.totalScoreRight.ToString();
            totalScoreText.text = scoreblock.totalScore.ToString();
            bonusScoreText.text = scoreblock.bonusScore.ToString();
        }

        // Reset dices and new turn
        private void NewTurn() {
            turn++;
            rolls = 0;
            rollText.text = $"Current roll: {rolls} / 3";
            statusText.text = "";
            eraseButtonText.text = "Erase Field";
            canFill = false; rolled = false; canErase = false;
            for (int diceCount = 0 ; diceCount<dices.Length; diceCount++) {
                dices[diceCount].ResetLock();
            }
            SetTotalScoreFields();
        }

        // Roll the dices
        public void Roll() {
            if (rolls < 3) {
                rolled = true;
                for (int diceCount = 0; diceCount < dices.Length; diceCount++) {
                    // Roll the dices which are not locked
                    if (!dices[diceCount].GetLockedState()) {
                        dices[diceCount].RollDice();
                    }
                }
                rolls++;
                rollText.text = $"Current roll: {rolls} / 3";
                audioManager.PlayRollSound(false);
            } else {
                statusText.text = "No more rolls";
                audioManager.PlayRollSound(true);
            }
            // Turn is automatically over when player reaches 3 rolls
            if (rolls == 3) {
                CheckValues();
            }
        }

        public void EndRolls() {
            // End turn and get the values
            if (rolled) {
                rolls = 3;
                rollText.text = $"Current roll: {rolls} / 3";
                CheckValues();
            } else {
                statusText.text = "Roll first";
            }
        }

        private void CheckValues() {
            // Get the values of the dices
            for (int counter = 0; counter < dices.Length; counter++) {
                currentValues[counter] = dices[counter].GetValue();
            }
            // If player has already rolled Yathzee, check for another one to add 100 bonuspoints
            if (scoreblock.GetYathzeeState()) {
                if (currentValues[0] == currentValues[1] && currentValues[1] == currentValues[2] && currentValues[2] == currentValues[3] && currentValues[3] == currentValues [4]) {
                    scoreblock.yahtzee += 100;
                    yahtzeeText.text = scoreblock.yahtzee.ToString();
                    scoreblock.CalculateTotalScore();
                    SetTotalScoreFields();
                }
            }
            // Player is able to fill a field
            canFill = true;

            // Show on screen that player has to fill field
            statusText.text = "Choose a field to fill in your earned points";
        }

        public void AceField() {
            int aces = 0;
            if (canFill) {
                if (canErase) {
                    // Give this field no points
                    scoreblock.ace = 0;
                    acesText.text = scoreblock.ace.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                } else if (scoreblock.ace == 0 && currentValues.Contains(1)) {
                    // Check how many dices contains ace and add to scoreblock
                    for (int counter = 0; counter < dices.Length; counter++) {
                        if (currentValues[counter] == 1)
                            aces++;
                    }
                    int points = aces * 1;
                    scoreblock.ace = points;
                    acesText.text = scoreblock.ace.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.ace != 0) {
                    // Player has already filled this field
                    statusText.text = "Ace field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        public void TwoField() {
            int twos = 0;
            if (canFill) {
                if (canErase) {
                    // Give this field no points
                    scoreblock.two = 0;
                    twosText.text = scoreblock.two.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                } else if (scoreblock.two == 0 && currentValues.Contains(2)) {
                    // Check how many dices contains two and add to scoreblock
                    for (int counter = 0; counter < dices.Length; counter++) {
                        if (currentValues[counter] == 2)
                            twos++;
                    }
                    int points = twos * 2;
                    scoreblock.two = points;
                    twosText.text = scoreblock.two.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.two != 0) {
                    // Player has already filled this field
                    statusText.text = "Two field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        } 

        public void ThreeField() {
            int threes = 0;
            if (canFill) {
                if (canErase) {
                    // Give this field no points
                    scoreblock.three = 0;
                    threesText.text = scoreblock.three.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                } else if (scoreblock.three == 0 && currentValues.Contains(3)) {
                    // Check how many dices contains three and add to scoreblock
                    for (int counter = 0; counter < dices.Length; counter++) {
                        if (currentValues[counter] == 3)
                            threes++;
                    }
                    int points = threes * 3;
                    scoreblock.three = points;
                    threesText.text = scoreblock.three.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.three != 0) {
                    // Player has already filled this field
                    statusText.text = "Three field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        public void FourField() {
            int fours = 0;
            if (canFill) {
                if (canErase) {
                    // Give this field no points
                    scoreblock.four = 0;
                    foursText.text = scoreblock.four.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                } else if (scoreblock.four == 0 && currentValues.Contains(4)) {
                    // Check how many dices contains four and add to scoreblock
                    for (int counter = 0; counter < dices.Length; counter++) {
                        if (currentValues[counter] == 4)
                            fours++;
                    }
                    int points = fours * 4;
                    scoreblock.four = points;
                    foursText.text = scoreblock.four.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.four != 0) {
                    // Player has already filled this field
                    statusText.text = "Four field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        public void FiveField() {
            int fives = 0;
            if (canFill) {
                if (canErase) {
                    // Give this field no points
                    scoreblock.five = 0;
                    fivesText.text = scoreblock.five.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                } else if (scoreblock.five == 0 && currentValues.Contains(5)) {
                    // Check how many dices contains five and add to scoreblock
                    for (int counter = 0; counter < dices.Length; counter++) {
                        if (currentValues[counter] == 5)
                            fives++;
                    }
                    int points = fives * 5;
                    scoreblock.five = points;
                    fivesText.text = scoreblock.five.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.five != 0) {
                    // Player has already filled this field
                    statusText.text = "Five field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        public void SixField() {
            int sixes = 0;
            if (canFill) {
                if (canErase) {
                    // Give this field no points
                    scoreblock.six = 0;
                    sixesText.text = scoreblock.six.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                } else if (scoreblock.six == 0 && currentValues.Contains(6)) {
                    // Check how many dices contains six and add to scoreblock
                    for (int counter = 0; counter < dices.Length; counter++) {
                        if (currentValues[counter] == 6)
                            sixes++;
                    }
                    int points = sixes * 6;
                    scoreblock.six = points;
                    sixesText.text = scoreblock.six.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.six != 0) {
                    // Player has already filled this field
                    statusText.text = "Six field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        public void ThreeOfAKindField() {
            if (canFill) {
                // Check if 3 dices are the same
                bool isOK = false;
                for(int value = 1; value <= 6; value++) {
                    int count = 0;
                    for (int place = 0; place < 5; place++) {
                        if (currentValues[place] == value) {
                            count++;
                        }
                        if (count >= 3) {
                            isOK = true;
                        }
                    }
                }
                if (canErase) {
                    // Give this field no points
                    scoreblock.threeOfAKind = 0;
                    threeOfAKindText.text = scoreblock.threeOfAKind.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                } else if (scoreblock.threeOfAKind == 0 && isOK) {
                    scoreblock.threeOfAKind = currentValues[0] + currentValues[1] + currentValues[2] + currentValues[3] + currentValues[4];
                    threeOfAKindText.text = scoreblock.threeOfAKind.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.threeOfAKind != 0) {
                    // Player has already filled this field
                    statusText.text = "Three Of A Kind field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        public void FourOfAKindField() {
            if (canFill) {
                // Check if 3 dices are the same
                bool isOK = false;
                for (int value = 1; value <= 6; value++) {
                    int count = 0;
                    for (int place = 0; place < 5; place++) {
                        if (currentValues[place] == value) {
                            count++;
                        }
                        if (count >= 4) {
                            isOK = true;
                        }
                    }
                }
                if (canErase) {
                    // Give this field no points
                    scoreblock.fourOfAKind = 0;
                    fourOfAKindText.text = scoreblock.fourOfAKind.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                } else if (scoreblock.fourOfAKind == 0 && isOK) {
                    scoreblock.fourOfAKind = currentValues[0] + currentValues[1] + currentValues[2] + currentValues[3] + currentValues[4];
                    fourOfAKindText.text = scoreblock.fourOfAKind.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.fourOfAKind != 0) {
                    // Player has already filled this field
                    statusText.text = "Four Of A Kind field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        public void FullHouseField() {
            if (canFill) {
                Array.Sort(currentValues); 
                if (canErase) {
                    // Give this field no points
                    scoreblock.fullHouse = 0;
                    fullHouseText.text = scoreblock.fullHouse.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                }
                 // Check if 3 and 2 dices are the same
                 else if (scoreblock.fullHouse == 0 && (((currentValues[0] == currentValues[1] && currentValues[1] == currentValues[2]) && (currentValues[3] == currentValues[4]) && currentValues[2] != currentValues[3]) || (currentValues[0] == currentValues[1]) && (currentValues[2] == currentValues[3] && currentValues[3] == currentValues[4]) && (currentValues[1] != currentValues[2]))) {
                    scoreblock.fullHouse = 25;
                    fullHouseText.text = scoreblock.fullHouse.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.fullHouse != 0) {
                    // Player has already filled this field
                    statusText.text = "Full House field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        public void SmallStraightField() {
            if (canFill) {
                if (canErase) {
                    // Give this field no points
                    scoreblock.smallStraight = 0;
                    smallStraightText.text = scoreblock.smallStraight.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                }
                // Check if the dice values contain numbers 1-4 OR 2-5 OR 3-6
                else if (scoreblock.smallStraight == 0 && ((currentValues.Contains(1) && currentValues.Contains(2) && currentValues.Contains(3) && currentValues.Contains(4)) || (currentValues.Contains(2) && currentValues.Contains(3) && currentValues.Contains(4) && currentValues.Contains(5)) || (currentValues.Contains(3) && currentValues.Contains(4) && currentValues.Contains(5) && currentValues.Contains(6)))) {
                    scoreblock.smallStraight = 30;
                    smallStraightText.text = scoreblock.smallStraight.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.smallStraight != 0) {
                    // Player has already filled this field
                    statusText.text = "Small Straight field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        public void LargeStraightField() {
            if (canFill) {
                if (canErase) {
                    // Give this field no points
                    scoreblock.largeStraight = 0;
                    largeStraightText.text = scoreblock.largeStraight.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                }
                // Check if the dice values contain numbers 1-5 OR 2-6
                else if (scoreblock.largeStraight == 0 && ((currentValues.Contains(1) && currentValues.Contains(2) && currentValues.Contains(3) && currentValues.Contains(4) && currentValues.Contains(5)) || (currentValues.Contains(2) && currentValues.Contains(3) && currentValues.Contains(4) && currentValues.Contains(5) && currentValues.Contains(6)))) {
                    scoreblock.largeStraight = 40;
                    largeStraightText.text = scoreblock.largeStraight.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.largeStraight != 0) {
                    // Player has already filled this field
                    statusText.text = "Large Straight field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        public void YahtzeeField() {
            if (canFill) {
                if (canErase) {
                    // Give this field no points
                    scoreblock.yahtzee = 0;
                    yahtzeeText.text = scoreblock.yahtzee.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();
                }
                // Check if all dice values are the same
                else if (scoreblock.yahtzee == 0 && (currentValues[0] == currentValues[1] && currentValues[1] == currentValues[2] && currentValues[2] == currentValues[3] && currentValues[3] == currentValues[4])) {
                    scoreblock.SetYathzeeState(true);
                    scoreblock.yahtzee = 50;
                    yahtzeeText.text = scoreblock.yahtzee.ToString();
                    canFill = false;
                    EndTurn();
                } else if (scoreblock.yahtzee != 0) {
                    // Player has already filled this field
                    statusText.text = "Yathzee field is already filled, try another one";
                } else {
                    // Player has not the correct dices to fill this field
                    statusText.text = "You do not have the correct dices";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        public void ChanceField() {
            if (canFill) {
                if (canErase) {
                    scoreblock.chance = 0;
                    chanceText.text = scoreblock.chance.ToString();
                    canFill = false;
                    canErase = false;
                    EndTurn();

                } else if (scoreblock.chance == 0) {
                    // Count up the dices
                    scoreblock.chance += (currentValues[0] + currentValues[1] + currentValues[2] + currentValues[3] + currentValues[4]);
                    chanceText.text = scoreblock.chance.ToString();
                    canFill = false;
                    EndTurn();
                } else {
                    // Player has already filled this field
                    statusText.text = "Chance field is already filled, try another one";
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not fill this at the moment";
            }
        }

        // Player has the option to erase a field
        public void EraseField() {
            if (canErase) {
                // Player his able to fill in points 
                canErase = false;
                statusText.text = "Choose a field to fill in your earned points";
                eraseButtonText.text = "Erase Field";
            } else {
                // Player is able to erase a field
                canErase = true;
                statusText.text = "Choose a field to erase";
                eraseButtonText.text = "Don't Erase";
            }
        }

        private void EndTurn() {
            // End current turn
            if (!canFill) {
                scoreblock.CalculateTotalScore();
                SetTotalScoreFields();

                // Game over when scoreblock is full, else start another turn
                if (turn == 13) {
                    GameOver();
                } else {
                    NewTurn();
                }
            } else {
                // Player can not fill the field now
                statusText.text = "You can not do this at the moment";
            }
        }

        private void GameOver() {
            // Show game over panel with total score
            menuManager.ToggleGameOverPanel(scoreblock.totalScore);
        }
    }
}