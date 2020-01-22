using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Globals;

public class HUD : Menu
{
    [SerializeField] TextMeshProUGUI scoreDisplay, highscoreDisplay;
    [SerializeField] Button undoButton, optionsButton, mainMenuButton;

    public void UpdateHUD()
    {
        UpdateScore();

        if (gameOver)
        {
            optionsButton.gameObject.SetActive(false);
            mainMenuButton.gameObject.SetActive(true);
        }
    }

    void UpdateScore()
    {
        scoreDisplay.text = score.ToString();

        //update highscore if necessary
        if (highScore < score)
        {
            highScore = score;
            UpdateHighscoreDisplay();
        }
    }

    public void UpdateHighscoreDisplay()
    {
        highscoreDisplay.text = highScore.ToString();
    }

    public void DisplayOptionsMenu(Canvas optionsMenu)
    {
        OpenMenu(optionsMenu);
    }

    public void HideOptionsMenu(Canvas optionsMenu)
    {
        CloseMenu(optionsMenu);
    }
}