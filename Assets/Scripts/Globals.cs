using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    #region Game

    public static bool inputEnabled = true;
    public static bool gameOver;

    public static int score;


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