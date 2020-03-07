using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreDisplay, highscoreDisplay, gameOverDisplay;

    int highScore;

    void Awake()
    {
        GameController gc = FindObjectOfType<GameController>();
        gc.ScoreChangedAction += SetScore;
        gc.GameOverAction += OnGameOver;

        highScore = PlayerPrefs.GetInt("highScore");
        highscoreDisplay.text = highScore.ToString();
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
            PlayerPrefs.SetInt("highScore", highScore);
        }
    }

    void OnGameOver()
    {
        gameOverDisplay.enabled = true;
    }

    public void OnSelectQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}