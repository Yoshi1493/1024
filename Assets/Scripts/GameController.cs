using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject tile, tileVariant;
    [SerializeField] Transform tileParent;
    bool inputEnabled = true;

    const float ShortInputDelay = SlideAnimationDuration;
    const float LongInputDelay = 0.5f;
    float inputDelay = LongInputDelay;

    const int BoardSize = 4;
    (Tile tile, int value)[,] board = new (Tile, int)[BoardSize, BoardSize];

    struct GameState
    {
        public (Tile tile, int value)[,] gameBoard;
        public int score;

        public GameState((Tile tile, int value)[,] _gameBoard, int _score)
        {
            gameBoard = _gameBoard;
            score = _score;
        }
    }
    Stack<GameState> previousGameStates = new Stack<GameState>();

    List<(int row, int col)> emptySpaces = new List<(int, int)>(BoardSize * BoardSize - 1);
    public const float FourSpawnChance = 0.10f;           //10% chance for a new tile's value to be 4 instead of 2

    public event System.Action<int> ScoreChangedAction;
    public event System.Action<bool> GameStateChangedAction;
    public event System.Action GameOverAction;

    int score;

    void SetScore(int value)
    {
        score = value;
        ScoreChangedAction.Invoke(value);
    }

    void Awake()
    {
        FindObjectOfType<HUD>().UndoAction += Undo;
    }

    void Start()
    {
        SpawnNewTile();
        SpawnNewTile();

        //previousGameStates.Push(new GameState(gameBoard.Clone() as int[,], 0));
        previousGameStates.Push(new GameState(board.Clone() as (Tile tile, int value)[,], 0));
    }

    void SpawnNewTile(float spawnDelay = 0f)
    {
        //find coordinates of all empty spaces on game board by checking each index in gameBoard if it's 0
        emptySpaces.Clear();

        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if (board[row, col].value == 0)
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
            GameOverAction.Invoke();
        }
    }

    IEnumerator SpawnTileAt((int row, int col) coordinate, int value, float spawnDelay)
    {
        //update respective gameBoard index
        board[coordinate.row, coordinate.col] = (Instantiate(tile, tileParent).GetComponent<Tile>(), value);

        yield return new WaitForSeconds(spawnDelay);

        //"spawn" tile
        board[coordinate.row, coordinate.col].tile.gameObject.SetActive(true);

        //set position, dispaly value
        board[coordinate.row, coordinate.col].tile.Initialize(coordinate, value);
    }

    void SpawnOldTileAt((int row, int col) coordinate, int value)
    {
        board[coordinate.row, coordinate.col] = (Instantiate(tileVariant, tileParent).GetComponent<Tile>(), value);
        board[coordinate.row, coordinate.col].tile.Initialize(coordinate, value);
    }

    void Update()
    {
        GetKeyInput();
    }

    void GetKeyInput()
    {
        if (inputEnabled && (Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
        {
            //cache the current game state
            GameState previousGameState = new GameState(board.Clone() as (Tile tile, int value)[,], score);

            //slide in direction based on key pressed
            if (Input.GetKey(KeyCode.RightArrow))
            {
                SlideRight();
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                SlideUp();
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                SlideLeft();
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                SlideDown();
            }

            //if any gameBoard element was different before and after handling game logic
            if (GameStateHasChanged(previousGameState))
            {
                previousGameStates.Push(previousGameState);         //push onto stack of previous game states
                GameStateChangedAction.Invoke(true);                //enable the undo button
                SpawnNewTile(SlideAnimationDuration);               //spawn new tile after sliding animation finishes

                inputEnabled = false;
                StartCoroutine(DelayInput());
            }
        }

        if (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
        {
            inputDelay = LongInputDelay;
        }
    }

    void SlideRight()
    {
        //for each row
        for (int row = 0; row < BoardSize; row++)
        {
            //for each column except the rightmost one, looping right to left
            for (int col = BoardSize - 2; col >= 0; col--)
            {
                //if current cell has a tile on it, slide it as far right as possible
                if (board[row, col].value != 0)
                {
                    //loop from [1 column to the right of the current column] to [rightmost column], looking for the first non-empty index
                    for (int c = col + 1; c < BoardSize; c++)
                    {
                        //if the column being checked has a tile on it, check if it has the same value as current cell's tile 
                        if (board[row, c].value != 0)
                        {
                            //if they're the same value, slide current tile to checked column
                            if (board[row, c].value == board[row, col].value)
                            {
                                MergeTile((row, col), (row, c));

                                //slide all tiles that are left of the current tile to the right a number of columns equal to how many columns the current tile has moved
                                for (int i = c - 1; i >= 0; i--)
                                {
                                    if (board[row, i].value != 0)
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

    void SlideUp()
    {
        for (int col = 0; col < BoardSize; col++)
        {
            for (int row = BoardSize - 2; row >= 0; row--)
            {
                if (board[row, col].value != 0)
                {
                    for (int r = row + 1; r < BoardSize; r++)
                    {
                        if (board[r, col].value != 0)
                        {
                            if (board[r, col].value == board[row, col].value)
                            {
                                MergeTile((row, col), (r, col));

                                for (int i = r - 1; i >= 0; i--)
                                {
                                    if (board[i, col].value != 0)
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

    void SlideLeft()
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 1; col < BoardSize; col++)
            {
                if (board[row, col].value != 0)
                {
                    for (int c = col - 1; c >= 0; c--)
                    {
                        if (board[row, c].value != 0)
                        {
                            if (board[row, c].value == board[row, col].value)
                            {
                                MergeTile((row, col), (row, c));

                                for (int i = c + 1; i < BoardSize; i++)
                                {
                                    if (board[row, i].value != 0)
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

    void SlideDown()
    {
        for (int col = 0; col < BoardSize; col++)
        {
            for (int row = 1; row < BoardSize; row++)
            {
                if (board[row, col].value != 0)
                {
                    for (int r = row - 1; r >= 0; r--)
                    {
                        if (board[r, col].value != 0)
                        {
                            if (board[r, col].value == board[row, col].value)
                            {
                                MergeTile((row, col), (r, col));

                                for (int i = r + 1; i < BoardSize; i++)
                                {
                                    if (board[i, col].value != 0)
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
        board[to.row, to.col].value = board[from.row, from.col].value;
        board[from.row, from.col].value = 0;

        board[to.row, to.col].tile = board[from.row, from.col].tile;
        StartCoroutine(board[from.row, from.col].tile.Slide(from, to));
        board[from.row, from.col].tile = null;
    }

    void MergeTile((int row, int col) from, (int row, int col) to)
    {
        board[to.row, to.col].value += board[from.row, from.col].value;
        board[from.row, from.col].value = 0;

        SetScore(score += board[to.row, to.col].value);

        StartCoroutine(board[from.row, from.col].tile.GetComponent<Tile>().Slide(from, to));

        StartCoroutine(board[from.row, from.col].tile.GetComponent<Tile>().Shrink());
        StartCoroutine(board[to.row, to.col].tile.GetComponent<Tile>().Shrink());

        StartCoroutine(SpawnTileAt((to.row, to.col), board[to.row, to.col].value, SlideAnimationDuration));
    }

    IEnumerator DelayInput()
    {
        yield return new WaitForSeconds(inputDelay);

        if (inputDelay == LongInputDelay) { inputDelay = ShortInputDelay; }
        inputEnabled = true;
    }

    //compare values in board[] with gameBoardState, looking for any differences between them
    bool GameStateHasChanged(GameState previousGameState)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if (board[row, col].value != previousGameState.gameBoard[row, col].value)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void Undo()
    {
        StopAllCoroutines();

        //pop out most recent game state from stack
        GameState mostRecentGameState = previousGameStates.Pop();

        //update current game state match elements in lastGameBoardState
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if (board[row, col].tile != null)
                {
                    board[row, col].value = 0;
                    Destroy(board[row, col].tile.gameObject);
                }

                if (mostRecentGameState.gameBoard[row, col].value != 0)
                {
                    SpawnOldTileAt((row, col), mostRecentGameState.gameBoard[row, col].value);
                }
            }
        }

        //update score
        SetScore(mostRecentGameState.score);

        //enable undo button depending on whether or not previous game states exist
        GameStateChangedAction.Invoke(previousGameStates.Count > 1);
    }

    #region DEBUG

    void PrintGameState(GameState gameState)
    {
        for (int row = BoardSize - 1; row >= 0; row--)
        {
            string output = "";

            for (int col = 0; col < BoardSize; col++)
            {
                output += gameState.gameBoard[row, col].value + "  ";
            }

            print(output);
        }
    }

    #endregion
}