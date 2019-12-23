using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Globals;

public class HUD : Menu
{
    [SerializeField] TextMeshProUGUI scoreDisplay, highscoreDisplay;
    [SerializeField] Button optionsButton, mainMenuButton;

    public void UpdateHUD()
    {
        //update score
        scoreDisplay.text = score.ToString();

        //update highscore if necessary
        if (highScore < score)
        {
            highScore = score;
            highscoreDisplay.text = highScore.ToString();
        }

        if (gameOver)
        {
            optionsButton.gameObject.SetActive(false);
            mainMenuButton.gameObject.SetActive(true);
        }
    }

    public void DisplayOptionsMenu(Canvas optionsMenu)
    {
        OpenMenu(optionsMenu);
    }
}