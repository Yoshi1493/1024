using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject tile;
    [SerializeField] Transform tileParent;

    const int BoardSize = 4;
    int[,] gameBoard = new int[BoardSize, BoardSize];
    GameObject[,] tiles = new GameObject[BoardSize, BoardSize];

    List<(int row, int col)> emptySpaces = new List<(int, int)>(BoardSize * BoardSize - 1);
    public const float FourSpawnChance = 0.10f;           //10% chance for a new tile's value to be 4 instead of 2

    int[,] previousGameStates = new int[BoardSize, BoardSize];

    int score;

    public event System.Action<int> ScoreChangedAction;
    public event System.Action GameOverAction;

    void Start()
    {
        SpawnNewTile();
        SpawnNewTile();
    }

    void SetScore(int value)
    {
        score = value;
        ScoreChangedAction?.Invoke(value);
    }

    void SpawnNewTile(float spawnDelay = 0f)
    {
        //find coordinates of all empty spaces on game board by checking each index in gameBoard if it's 0
        emptySpaces.Clear();

        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
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
            int tileValue = Random.value < FourSpawnChance ? 4 : 2;

            StartCoroutine(SpawnTileAt(randEmptySpace, tileValue, spawnDelay));
        }
        //otherwise game over
        else
        {
            GameOverAction?.Invoke();
        }
    }

    IEnumerator SpawnTileAt((int row, int col) coordinate, int value, float spawnDelay = 0f)
    {
        //update respective gameBoard index
        gameBoard[coordinate.row, coordinate.col] = value;

        yield return new WaitForSeconds(spawnDelay);

        tiles[coordinate.row, coordinate.col] = Instantiate(tile, tileParent);

        //call Initialize to set its coordinate and value display
        tiles[coordinate.row, coordinate.col].GetComponent<Tile>().Initialize(coordinate, value);
    }

    void Update()
    {
        GetKeyInput();
    }

    void GetKeyInput()
    {
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            //cache the current game state
            (int[,] boardState, int score) _gameState = (gameBoard.Clone() as int[,], score);

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
            if (GameBoardIsDifferentFrom(_gameState.boardState))
            {
                SpawnNewTile(SlideAnimationDuration);
            }
        }
    }

    public void SlideRight()
    {
        //for each row
        for (int row = 0; row < BoardSize; row++)
        {
            //for each column except the rightmost one, looping right to left
            for (int col = BoardSize - 2; col >= 0; col--)
            {
                //if current cell has a tile on it, slide it as far right as possible
                if (gameBoard[row, col] != 0)
                {
                    //loop from [1 column to the right of the current column] to [rightmost column], looking for the first non-empty index
                    for (int c = col + 1; c < BoardSize; c++)
                    {
                        //if the column being checked has a tile on it, check if it has the same value as current cell's tile 
                        if (gameBoard[row, c] != 0)
                        {
                            //if they're the same value, slide current tile to checked column
                            if (gameBoard[row, c] == gameBoard[row, col])
                            {
                                MergeTile((row, col), (row, c));

                                //slide all tiles that are left of the current tile to the right a number of columns equal to how many columns the current tile has moved
                                for (int i = c - 1; i >= 0; i--)
                                {
                                    if (gameBoard[row, i] != 0)
                                    {
                                        SlideTile((row, i), (row, i + c - col));
                                    }
                                }
                            }
                            //otherwise slide current tile to 1 column left of the current column
                            else
                            {
                                //but only if it needs to be slid
                                if (col != c - 1)
                                {
                                    SlideTile((row, col), (row, c - 1));
                                }
                            }

                            //no need to continue checking
                            break;
                        }
                        //otherwise (if game board at checked column is empty)
                        else
                        {
                            //also if checked column is the rightmost column (i.e. loop is at its last iteration), slide to rightmost column
                            if (c == BoardSize - 1)
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
        for (int col = 0; col < BoardSize; col++)
        {
            for (int row = BoardSize - 2; row >= 0; row--)
            {
                if (gameBoard[row, col] != 0)
                {
                    for (int r = row + 1; r < BoardSize; r++)
                    {
                        if (gameBoard[r, col] != 0)
                        {
                            if (gameBoard[r, col] == gameBoard[row, col])
                            {
                                MergeTile((row, col), (r, col));

                                for (int i = r - 1; i >= 0; i--)
                                {
                                    if (gameBoard[i, col] != 0)
                                    {
                                        SlideTile((i, col), (i + r - row, col));
                                    }
                                }
                            }
                            else
                            {
                                if (row != r - 1)
                                {
                                    SlideTile((row, col), (r - 1, col));
                                }
                            }

                            break;
                        }
                        else
                        {
                            if (r == BoardSize - 1)
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
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 1; col < BoardSize; col++)
            {
                if (gameBoard[row, col] != 0)
                {
                    for (int c = col - 1; c >= 0; c--)
                    {
                        if (gameBoard[row, c] != 0)
                        {
                            if (gameBoard[row, c] == gameBoard[row, col])
                            {
                                MergeTile((row, col), (row, c));

                                for (int i = c + 1; i < BoardSize; i++)
                                {
                                    if (gameBoard[row, i] != 0)
                                    {
                                        SlideTile((row, i), (row, i + c - col));
                                    }
                                }
                            }
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
        for (int col = 0; col < BoardSize; col++)
        {
            for (int row = 1; row < BoardSize; row++)
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

                                for (int i = r + 1; i < BoardSize; i++)
                                {
                                    if (gameBoard[i, col] != 0)
                                    {
                                        SlideTile((i, col), (i + r - row, col));
                                    }
                                }
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
        gameBoard[to.row, to.col] = gameBoard[from.row, from.col];
        gameBoard[from.row, from.col] = 0;

        tiles[to.row, to.col] = tiles[from.row, from.col];
        StartCoroutine(tiles[from.row, from.col].GetComponent<Tile>().Slide(from, to));
        tiles[from.row, from.col] = null;
    }

    void MergeTile((int row, int col) from, (int row, int col) to)
    {
        gameBoard[to.row, to.col] += gameBoard[from.row, from.col];
        gameBoard[from.row, from.col] = 0;

        SetScore(score += gameBoard[to.row, to.col]);

        StartCoroutine(tiles[from.row, from.col].GetComponent<Tile>().Slide(from, to));

        StartCoroutine(tiles[from.row, from.col].GetComponent<Tile>().Shrink());
        StartCoroutine(tiles[to.row, to.col].GetComponent<Tile>().Shrink());

        StartCoroutine(SpawnTileAt((to.row, to.col), gameBoard[to.row, to.col], SlideAnimationDuration));
    }

    //compare gameBoard with gameBoardState, looking for any differences between them
    bool GameBoardIsDifferentFrom(int[,] gameBoardState)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if (gameBoard[row, col] != gameBoardState[row, col])
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void ResetGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}