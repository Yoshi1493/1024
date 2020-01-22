using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Globals;

public class GameController : MonoBehaviour
{
    [SerializeField] HUD hud;
    [SerializeField] AudioController audioController;
    [SerializeField] GameObject tile;
    [SerializeField] Transform tileParent;

    const int BOARD_SIZE = 4;
    int[,] gameBoard = new int[BOARD_SIZE, BOARD_SIZE];
    List<(int row, int col)> emptySpaces = new List<(int, int)>(BOARD_SIZE * BOARD_SIZE - 1);

    public const float FOUR_SPAWN_CHANCE = 0.10f;           //10% chance for a new tile's value to be 4 instead of 2

    void Start()
    {
        ResetGameState();

        SpawnNewTile();
        SpawnNewTile();

        //set initial game board state
        gameBoardStates.Push(gameBoard.Clone() as int[,]);
    }

    void ResetGameState()
    {
        inputEnabled = true;
        gameOver = false;
        score = 0;

        gameBoardStates.Clear();
        hud.UpdateHighscoreDisplay();
    }

    void SpawnNewTile()
    {
        //find coordinates of all empty spaces on game board by checking each index in gameBoard if it's 0
        emptySpaces.Clear();

        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (gameBoard[row, col] == 0)
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

            SpawnTileAt(randEmptySpace, tileValue);
        }
        //otherwise game over
        else
        {
            gameOver = true;
            hud.UpdateHUD();
        }
    }

    void SpawnTileAt((int row, int col) coordinate, int value)
    {
        GameObject newTile = Instantiate(tile, tileParent);

        //call Tile.Initialize to set its coordinate and value display
        newTile.GetComponent<Tile>().Initialize(coordinate, value);

        //update respective gameBoard index
        gameBoard[coordinate.row, coordinate.col] = value;
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
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SlideRight();
                DebugGameBoard();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SlideUp();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SlideLeft();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SlideDown();
            }
        }
    }

    public void SlideRight()
    {

    }

    public void SlideUp()
    {
        
    }

    public void SlideLeft()
    {
        
    }

    public void SlideDown()
    {
        
    }

    void SlideTile((int row, int col) from, (int row, int col) to)
    {
        print(string.Format("sliding {0} to {1}", from, to));
        gameBoard[to.row, to.col] = gameBoard[from.row, from.col];
        gameBoard[from.row, from.col] = 0;
    }

    void UpdateGameBoardState(int[,] gameBoardState)
    {
        //if valid move was made, save previous game state and spawn new tile
    }

    //return true if any item between gameBoard and most recent gameBoardState doesn't match
    bool GameStateHasChanged(int[,] gameBoardState)
    {
        return false;
    }

    public void Undo()
    {
        //remove last gameBoardState from stack
        //update gameBoard and tile displays to match lastGameBoardState
    }

    #region DEBUG

    void DebugGameBoard()
    {
        for (int row = BOARD_SIZE - 1; row >= 0; row--)
        {
            string output = "";

            for (int col = 0; col < BOARD_SIZE; col++)
            {
                output += gameBoard[row, col];
                if (col < BOARD_SIZE - 1) { output += ", "; }
            }

            print(output);
        }

        print('\n');
    }

    void PauseEditor()
    {
        UnityEditor.EditorApplication.isPaused = true;
    }

    #endregion
}