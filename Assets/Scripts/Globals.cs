using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    #region Game

    public static bool inputEnabled = true;
    public static bool gameOver;

    public static int highScore;
    public static int score;

    public const float SLIDE_ANIMATION_DURATION = 0.25f;

    public enum SlideDirection
    {
        Right,
        Up,
        Left,
        Down
    }

    public const float FOUR_SPAWN_CHANCE = 0.1f;            //10% chance for a tile to be spawned with a value of 4 instead of 2

    #endregion

    #region Options

    public static bool soundEnabled = true;

    public static void ToggleSound()
    {
        soundEnabled = !soundEnabled;
        PlayerPrefs.SetInt("Sound", soundEnabled ? 1 : 0);
    }

    #endregion
}