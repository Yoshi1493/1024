using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    #region Game

    public static bool inputEnabled = true;
    public static bool gameOver;

    public static Stack<int[,]> gameBoardStates = new Stack<int[,]>();

    public static int highScore;
    public static int score;

    public const int TILE_SIZE = 180;                       //game tile width & height (px)
    public const float SLIDE_ANIMATION_DURATION = 0.20f;    //length (sec.) of tile slide animation

    #endregion

    #region Options

    public static bool soundEnabled = true;

    public static void ToggleSound()
    {
        soundEnabled = !soundEnabled;
        PlayerPrefs.SetInt("Sound", soundEnabled ? 1 : 0);
    }

    public static void ResetHighscore()
    {
        highScore = 0;
    }

    #endregion
}