using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    protected Canvas thisMenu;

    protected virtual void Awake()
    {
        thisMenu = GetComponent<Canvas>();
    }

    protected void OpenMenu(Canvas menu)
    {
        menu.enabled = true;
    }

    protected void CloseMenu(Canvas menu)
    {
        menu.enabled = false;
    }

    public void SwitchMenu(Canvas menu)
    {
        OpenMenu(menu);
        CloseMenu(thisMenu);
    }

    public void LoadScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
    }
}