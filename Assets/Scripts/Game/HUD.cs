using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Globals;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreDisplay;
    [SerializeField] Button optionsButton, mainMenuButton;

    public void UpdateHUD()
    {
        scoreDisplay.text = score.ToString();

        if (gameOver)
        {
            optionsButton.gameObject.SetActive(false);
            mainMenuButton.gameObject.SetActive(true);
        }
    }
}