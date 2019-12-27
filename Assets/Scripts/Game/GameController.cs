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
    Tile[,] tiles = new Tile[BOARD_SIZE, BOARD_SIZE];
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
                tiles[row, col] = tileParent.GetChild(row * BOARD_SIZE + col).GetComponent<Tile>();

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
        SpawnNewTile();
        SpawnNewTile();

        //set initial game board state
        gameBoardStates.Push(GetGameBoardCopy());
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
        //find coordinates of all empty spaces on game board
        emptySpaces.Clear();

        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (!tiles[row, col].gameObject.activeSelf)
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
        //otherwise game is over
        else
        {
            gameOver = true;
            hud.UpdateHUD();
        }
    }

    void SpawnTileAt((int row, int col) coordinate, int value)
    {
        tiles[coordinate.row, coordinate.col].gameObject.SetActive(true);
        tiles[coordinate.row, coordinate.col].SetValue(value);
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
            int[,] lastGameBoardState = GetGameBoardCopy();

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

            UpdateGameBoardState(lastGameBoardState);
            hud.UpdateHUD();
        }
    }

    public void SlideRight()
    {
        //loop from second-right column to left column
        for (int col = BOARD_SIZE - 2; col >= 0; col--)
        {
            //loop through all rows
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                //if tile is visible
                if (gameBoard[row, col] != 0)
                {
                    //loop from (the column right of this) to (right column)
                    for (int c = col + 1; c < BOARD_SIZE; c++)
                    {
                        //if there is another tile to the right of this tile, compare values
                        if (gameBoard[row, c] != 0)
                        {
                            //if values match, slide tile to column c
                            if (gameBoard[row, col] == gameBoard[row, c])
                            {
                                StartCoroutine(SlideTile((row, col), (row, c)));
                                break;
                            }
                            //otherwise if values are different, slide tile to 1 column before this
                            else
                            {
                                //call SlideTile only if tile should move at all
                                if (col != c - 1)
                                {
                                    StartCoroutine(SlideTile((row, col), (row, c - 1)));
                                }
                                break;
                            }
                        }
                        else
                        {
                            //otherwise if there is no other tile in this direction, slide tile to right column
                            if (c == BOARD_SIZE - 1)
                            {
                                StartCoroutine(SlideTile((row, col), (row, BOARD_SIZE - 1)));
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
        //loop from second-bottom row to top row
        for (int row = BOARD_SIZE - 2; row >= 0; row--)
        {
            //loop through all columns
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (gameBoard[row, col] != 0)
                {
                    //loop from (the row below this) to (bottom row)
                    for (int r = row + 1; r < BOARD_SIZE; r++)
                    {
                        if (gameBoard[r, col] != 0)
                        {
                            //if values match, slide tile to row r
                            if (gameBoard[row, col] == gameBoard[r, col])
                            {
                                StartCoroutine(SlideTile((row, col), (r, col)));
                                break;
                            }
                            else
                            {
                                if (row != r - 1)
                                {
                                    StartCoroutine(SlideTile((row, col), (r - 1, col)));
                                }
                                break;
                            }
                        }
                        else
                        {
                            if (r == BOARD_SIZE - 1)
                            {
                                StartCoroutine(SlideTile((row, col), (BOARD_SIZE - 1, col)));
                            }
                        }
                    }
                }
            }
        }
    }

    public void SlideLeft()
    {
        //loop from second-left column to right column
        for (int col = 1; col < BOARD_SIZE; col++)
        {
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                if (gameBoard[row, col] != 0)
                {
                    //loop from (the column left of this) to (left column)
                    for (int c = col - 1; c >= 0; c--)
                    {
                        if (gameBoard[row, c] != 0)
                        {
                            if (gameBoard[row, col] == gameBoard[row, c])
                            {
                                StartCoroutine(SlideTile((row, col), (row, c)));
                                break;
                            }
                            else
                            {
                                if (col != c + 1)
                                {
                                    StartCoroutine(SlideTile((row, col), (row, c + 1)));
                                }
                                break;
                            }
                        }
                        else
                        {
                            if (c == 0)
                            {
                                StartCoroutine(SlideTile((row, col), (row, 0)));
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
        //loop from second-top row to bottom row
        for (int row = 1; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (gameBoard[row, col] != 0)
                {
                    //loop from (the row above this) to (top row)
                    for (int r = row - 1; r >= 0; r--)
                    {
                        if (gameBoard[r, col] != 0)
                        {
                            if (gameBoard[row, col] == gameBoard[r, col])
                            {
                                StartCoroutine(SlideTile((row, col), (r, col)));
                                break;
                            }
                            else
                            {
                                if (row != row + 1)
                                {
                                    StartCoroutine(SlideTile((row, col), (r + 1, col)));
                                }
                                break;
                            }
                        }
                        else
                        {
                            if (r == 0)
                            {
                                StartCoroutine(SlideTile((row, col), (0, col)));
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator SlideTile((int row, int col) from, (int row, int col) to)
    {
        gameBoard[to.row, to.col] = gameBoard[from.row, from.col] *= gameBoard[to.row, to.col] == gameBoard[from.row, from.col] ? 2 : 1;
        gameBoard[from.row, from.col] = 0;

        //slide tiles based on difference between <to> and <from>
        Vector2 slideDistance = new Vector2(to.col - from.col, to.row - from.row);
        StartCoroutine(tiles[from.row, from.col].Slide(slideDistance));

        //disable input until slide animation finishes
        inputEnabled = false;
        yield return new WaitForSeconds(SLIDE_ANIMATION_DURATION);
        inputEnabled = true;

        //update tile display to match internal array
        tiles[to.row, to.col].SetValue(gameBoard[to.row, to.col]);

        //reset position and disable tile that just moved, and display tile at end position of tile that just moved
        //this creates an illusion that tiles are sliding around, but all tiles are actually resetting their position after a move
        tiles[from.row, from.col].ResetPosition();
        tiles[to.row, to.col].gameObject.SetActive(true);
    }

    void UpdateGameBoardState(int[,] gameBoardState)
    {
        //if valid move was made, save previous game state and spawn new tile
        if (GameStateHasChanged(gameBoardState))
        {
            gameBoardStates.Push(gameBoardState);
            //SpawnNewTile();
        }
    }

    bool GameStateHasChanged(int[,] gameBoardState)
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                //return true if any item between gameBoard and most recent gameBoardState doesn't match
                if (gameBoard[row, col] != gameBoardState[row, col])
                {
                    return true;
                }
            }
        }

        return false;
    }

    int[,] GetGameBoardCopy()
    {
        return gameBoard.Clone() as int[,];
    }

    public void Undo()
    {
        int[,] lastGameBoardState = gameBoardStates.Pop();

        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                gameBoard[row, col] = lastGameBoardState[row, col];
                tiles[row, col].SetValue(gameBoard[row, col]);
                tiles[row, col].enabled = gameBoard[row, col] != 0;
            }
        }

        hud.UpdateHUD();
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