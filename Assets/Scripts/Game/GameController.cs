using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Globals;

public class GameController : MonoBehaviour
{
    [SerializeField] HUD hud;
    [SerializeField] AudioController audioController;
    [SerializeField] Transform tileParent;

    const int BOARD_SIZE = 4;
    int[,] gameBoard = new int[BOARD_SIZE, BOARD_SIZE];
    GameObject[,] tiles = new GameObject[BOARD_SIZE, BOARD_SIZE];
    List<(int row, int col)> emptySpaces = new List<(int, int)>(BOARD_SIZE * BOARD_SIZE - 1);

    void Awake()
    {
        InitGameBoard();
    }

    void InitGameBoard()
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                //fill tiles[]
                tiles[row, col] = tileParent.GetChild(row * BOARD_SIZE + col).gameObject;

#if UNITY_EDITOR
                //rename gameobjects for easier debugging
                tiles[row, col].name = string.Format("Disc [{0}][{1}]", row, col);
#endif
            }
        }
    }

    void Start()
    {
        ResetGameState();
        SpawnTile();
        SpawnTile();

        DebugGameBoard();
    }

    void ResetGameState()
    {
        inputEnabled = true;
        gameOver = false;
        score = 0;
    }

    void SpawnTile()
    {
        //find coordinates of all empty spaces on game board
        emptySpaces.Clear();

        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (!tiles[row, col].activeSelf)
                {
                    emptySpaces.Add((row, col));
                }
            }
        }

        //if there are empty spaces, spawn a new tile
        if (emptySpaces.Count != 0)
        {
            (int row, int col) randEmptySpace = emptySpaces[Random.Range(0, emptySpaces.Count)];
            int tileValue = Random.value < FOUR_SPAWN_CHANCE ? 4 : 2;

            tiles[randEmptySpace.row, randEmptySpace.col].SetActive(true);
            tiles[randEmptySpace.row, randEmptySpace.col].GetComponent<Tile>().SetValue(tileValue);
            gameBoard[randEmptySpace.row, randEmptySpace.col] = tileValue;
        }
        //otherwise game is over
        else
        {
            gameOver = true;
            hud.UpdateHUD();
        }
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
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            string output = "";

            for (int col = 0; col < BOARD_SIZE; col++)
            {
                output += gameBoard[row, col].ToString();
                if (col < BOARD_SIZE - 1) { output += ", "; }
            }

            print(output);
        }
    }

    void PauseEditor()
    {
        UnityEditor.EditorApplication.isPaused = true;
    }

    #endregion
}