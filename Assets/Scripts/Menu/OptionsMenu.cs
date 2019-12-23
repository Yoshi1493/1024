using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

public class OptionsMenu : Menu
{
    protected override void Awake()
    {
        base.Awake();
        InitOptions();
    }

    void InitOptions()
    {

    }

    public void ResetHighScore()
    {
        highScore = 0;
    }

    #region Game scene functions

    public void ShowOptionsMenu()
    {
        thisMenu.enabled = true;
    }

    public void OnSelectResume()
    {
        thisMenu.enabled = false;
    }

    #endregion
}