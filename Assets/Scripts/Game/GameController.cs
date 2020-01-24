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
    GameObject[,] tiles = new GameObject[BOARD_SIZE, BOARD_SIZE];
    List<(int row, int col)> emptySpaces = new List<(int, int)>(BOARD_SIZE * BOARD_SIZE - 1);

    public const float FOUR_SPAWN_CHANCE = 0.10f;           //10% chance for a new tile's value to be 4 instead of 2

    void Start()
    {
        ResetGameState();

        //SpawnNewTile();
        //SpawnNewTile();
        SpawnTileAt((0, 0), 8);
        SpawnTileAt((0, 1), 4);
        SpawnTileAt((0, 2), 2);
        SpawnTileAt((0, 3), 2);

        //set initial game board state
        gameBoardStates.Push(gameBoard.Clone() as int[,]);
    }

    void ResetGameState()
    {
        inputEnabled = true;
        gameOver = false;
        score = 0;

        gameBoardStates.Clear();
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
        tiles[coordinate.row, coordinate.col] = Instantiate(tile, tileParent);

        //call Initialize to set its coordinate and value display
        tiles[coordinate.row, coordinate.col].GetComponent<Tile>().Initialize(coordinate, value);

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
            //save a copy of gameBoard
            int[,] _gameBoard = gameBoard.Clone() as int[,];

            //handle logic
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SlideRight();
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

            //if any gameBoard element was different before and after handling game logic
            if (GameBoardIsDifferentFrom(_gameBoard))
            {
                //push the gameBoard copy onto the stack
                gameBoardStates.Push(_gameBoard);

                SpawnNewTile();

                hud.UpdateHUD();
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            DebugGameBoard(gameBoard);
        }
    }

    public void SlideRight()
    {
        //for each row
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            //for each column except the rightmost one, looping right to left
            for (int col = BOARD_SIZE - 2; col >= 0; col--)
            {
                //if game board at current index has a tile on it, slide it as far right as possible
                if (gameBoard[row, col] != 0)
                {
                    //loop from [1 column to the right of the current column] to [rightmost column], looking for the first non-empty index
                    for (int c = col + 1; c < BOARD_SIZE; c++)
                    {
                        //if game board at checked column has a tile on it, check if it has the same value as current column 
                        if (gameBoard[row, c] != 0)
                        {
                            //if they're the same value, slide tile at current index to checked column
                            if (gameBoard[row, c] == gameBoard[row, col])
                            {
                                MergeTile((row, col), (row, c));
                            }
                            //otherwise slide tile at current index to 1 before checked column
                            else
                            {
                                //but only if it needs to be slid
                                if (col != c - 1)
                                {
                                    SlideTile((row, col), (row, c - 1));
                                }
                            }

                            //break out of loop; no need to continue checking
                            break;
                        }
                        //otherwise (if game board at checked column is empty)
                        else
                        {
                            //also if checked column is the rightmost column (i.e. loop is at its last iteration), slide to rightmost column
                            if (c == BOARD_SIZE - 1)
                            {
                                SlideTile((row, col), (row, c));
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void SlideUp()
    {
        //for each column
        for (int col = 0; col < BOARD_SIZE; col++)
        {
            //for each row except the topmost one, looping top to bottom
            for (int row = BOARD_SIZE - 2; row >= 0; row--)
            {
                //if game board at current index has a tile on it, slide it as farm up as possible
                if (gameBoard[row, col] != 0)
                {
                    //loop from [1 above current row] to [topmost row], looking for the first non-empty index
                    for (int r = row + 1; r < BOARD_SIZE; r++)
                    {
                        //if game board at checked row has a tile on it, check if it has the same value as current row
                        if (gameBoard[r, col] != 0)
                        {
                            //if they're the same value, slide tile at current index to checked row
                            if (gameBoard[r, col] == gameBoard[row, col])
                            {
                                MergeTile((row, col), (r, col));
                            }
                            //otherwise slide tile at current index to 1 before checked row
                            else
                            {
                                if (row != r - 1)
                                {
                                    SlideTile((row, col), (r - 1, col));
                                }
                            }

                            break;
                        }
                        //otherwise (if game board at checked row is empty)
                        else
                        {
                            //also if checked row is the topmost row
                            if (r == BOARD_SIZE - 1)
                            {
                                SlideTile((row, col), (r, col));
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void SlideLeft()
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            //for each column except the leftmost one, looping left to right
            for (int col = 1; col < BOARD_SIZE; col++)
            {
                //if game board at current index has a tile on it, slide it as far left as possible
                if (gameBoard[row, col] != 0)
                {
                    //loop from [1 column to the left of the current column] to [leftmost column], looking for the first non-empty index
                    for (int c = col - 1; c >= 0; c--)
                    {
                        //if game board at checked column has a tile on it, check if it has the same value as current column
                        if (gameBoard[row, c] != 0)
                        {
                            //if they're the same value, slide tile at current index to checked column
                            if (gameBoard[row, c] == gameBoard[row, col])
                            {
                                MergeTile((row, col), (row, c));
                            }
                            //otherwise slide tile at current index to 1 before checked column
                            else
                            {
                                if (col != c + 1)
                                {
                                    SlideTile((row, col), (row, c + 1));
                                }
                            }

                            break;
                        }
                        else
                        {
                            if (c == 0)
                            {
                                SlideTile((row, col), (row, c));
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void SlideDown()
    {
        for (int col = 0; col < BOARD_SIZE; col++)
        {
            for (int row = 1; row < BOARD_SIZE; row++)
            {
                if (gameBoard[row, col] != 0)
                {
                    for (int r = row - 1; r >= 0; r--)
                    {
                        if (gameBoard[r, col] != 0)
                        {
                            if (gameBoard[r, col] == gameBoard[row, col])
                            {
                                MergeTile((row, col), (r, col));
                            }
                            else
                            {
                                if (row != r + 1)
                                {
                                    SlideTile((row, col), (r + 1, col));
                                }
                            }

                            break;
                        }
                        else
                        {
                            if (r == 0)
                            {
                                SlideTile((row, col), (r, col));
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    void SlideTile((int row, int col) from, (int row, int col) to)
    {
        print(string.Format("sliding {0} to {1}", from, to));

        gameBoard[to.row, to.col] = gameBoard[from.row, from.col];
        gameBoard[from.row, from.col] = 0;

        tiles[to.row, to.col] = tiles[from.row, from.col];
        StartCoroutine(tiles[from.row, from.col].GetComponent<Tile>().Slide(from, to));
        tiles[from.row, from.col] = null;
    }

    void MergeTile((int row, int col) from, (int row, int col) to)
    {
        print(string.Format("merging {0} with {1}", from, to));

        gameBoard[to.row, to.col] += gameBoard[from.row, from.col];
        gameBoard[from.row, from.col] = 0;

        score += gameBoard[to.row, to.col];

        StartCoroutine(tiles[from.row, from.col].GetComponent<Tile>().Slide(from, to));

        tiles[from.row, from.col].GetComponent<Tile>().Shrink();
        tiles[to.row, to.col].GetComponent<Tile>().Shrink();

        SpawnTileAt((to.row, to.col), gameBoard[to.row, to.col]);
    }

    //compare gameBoard with gameBoardState, looking for any differences between them
    bool GameBoardIsDifferentFrom(int[,] gameBoardState)
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (gameBoard[row, col] != gameBoardState[row, col])
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void Undo()
    {
        //remove last gameBoardState from stack
        //update gameBoard and tile displays to match lastGameBoardState
    }

    #region DEBUG

    void DebugGameBoard(int[,] board)
    {
        for (int row = BOARD_SIZE - 1; row >= 0; row--)
        {
            string output = "";

            for (int col = 0; col < BOARD_SIZE; col++)
            {
                output += board[row, col];
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