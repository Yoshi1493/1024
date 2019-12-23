using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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