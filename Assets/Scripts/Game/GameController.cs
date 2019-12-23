using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

public class GameController : MonoBehaviour
{
    [SerializeField] HUD hud;
    [SerializeField] AudioController audioController;
    [SerializeField] Transform tileParent;

    const int BOARD_SIZE = 4;
    int[,] gameBoard = new int[BOARD_SIZE, BOARD_SIZE];

    void Awake()
    {
        InitGameBoard();
    }

    void InitGameBoard()
    {

    }

    void Start()
    {
        ResetGameState();
    }

    void ResetGameState()
    {
        inputEnabled = true;
        gameOver = false;
        score = 0;
    }

    void Update()
    {
        if (!gameOver && inputEnabled)
        {
            GetKeyInput();
        }
    }

    void GetKeyInput()
    {

    }

    #region DEBUG

    void DebugGameBoard()
    {

    }

    void PauseEditor()
    {
        UnityEditor.EditorApplication.isPaused = true;
    }

    #endregion
}