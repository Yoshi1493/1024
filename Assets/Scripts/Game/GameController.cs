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
        //SpawnNewTile();
        //SpawnNewTile();
        SpawnTileAt((0, 0), 2);
        SpawnTileAt((0, 1), 2);
        SpawnTileAt((0, 2), 2);
        SpawnTileAt((0, 3), 2);

        DebugGameBoard();
    }

    void ResetGameState()
    {
        inputEnabled = true;
        gameOver = false;
        score = 0;

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
            /*
            randEmptySpace.row = 0;
            tileValue = 2;
            */
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
        if (Input.GetKeyDown(KeyCode.D))
        {
            DebugGameBoard();
        }
    }

    void GetKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SlideRight();
            DebugGameBoard();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SlideUp();
            DebugGameBoard();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SlideLeft();
            DebugGameBoard();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SlideDown();
            DebugGameBoard();
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
                            //if values are same, slide tile to column c
                            if (gameBoard[row, col] == gameBoard[row, c])
                            {
                                StartCoroutine(SlideTile((row, col), (row, c)));
                                break;
                            }
                            //otherwise if values are different, slide tile to column (c - 1)
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

    IEnumerator SlideTile((int row, int col) from, (int row, int col) to)
    {
        print(string.Format("moving {0} to {1}", from, to));

        if (gameBoard[to.row, to.col] == gameBoard[from.row, from.col])
        {
            //tiles[to.row, to.col].SetValue(gameBoard[from.row, from.col] *= 2);
            gameBoard[to.row, to.col] = gameBoard[from.row, from.col] *= 2;
        }
        else
        {
            //tiles[to.row, to.col].SetValue(gameBoard[from.row, from.col]);
            gameBoard[to.row, to.col] = gameBoard[from.row, from.col];
        }

        gameBoard[from.row, from.col] = 0;

        Vector2 slideDistance = new Vector2(to.col - from.col, to.row - from.row);
        StartCoroutine(tiles[from.row, from.col].Slide(slideDistance));

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

    public void SlideUp()
    {

    }

    public void SlideLeft()
    {

    }

    public void SlideDown()
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