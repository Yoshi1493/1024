using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreDisplay, highscoreDisplay;
    [SerializeField] Button undoButton;

    int highScore;

    void Awake()
    {
        GameController gc = FindObjectOfType<GameController>();

        gc.ScoreChangeAction += SetScore;
        gc.GameStateChangeAction += SetUndoButtonState;
    }

    void SetScore(int value)
    {
        //update score display
        scoreDisplay.text = value.ToString();

        //update highscore and highscore display if necessary
        if (highScore < value)
        {
            highScore = value;
            highscoreDisplay.text = highScore.ToString();
        }
    }

    void SetUndoButtonState(bool state)
    {
        undoButton.interactable = state;
    }

    void OnGameOver()
    {
        SetUndoButtonState(false);
    }
}