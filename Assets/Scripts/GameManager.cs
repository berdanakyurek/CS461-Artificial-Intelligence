using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{

    //horizontal(false) and vertical(true)
    private bool wallRotation = false;

    public Vector3[] endPos01 = new Vector3[9];
    public Vector3[] endPos02 = new Vector3[9];
    
    private RaycastHit hit;
    public GameObject boardCube1, boardCube2;
    private GameObject boardObject;
    public GameObject player1Prefab, player2Prefab;
    private Player player1Controller, player2Controller;
    public GameObject wall;
    public GameObject gameEndModal, winStatus, pauseModal, searchMethodModal;
    
    private bool player1Turn = true;
    public const int BOARD_SIZE = 9;
    public GameObject[,] board = new GameObject[BOARD_SIZE,BOARD_SIZE];
    private Vector3 initialPlayer1Pos, initialPlayer2Pos;
    private TextMeshProUGUI player1Walls, player2Walls;
    private TextMeshProUGUI playerTurn;

    private GameObject wallPlaceHolder;
    public GameObject validWallPrefab;
    private bool wallPlaceholderOnBoard;
    
    private bool gameEnded;
    private bool gamePaused;
    private bool greedyRunned = false;
    private void InitializeBoard()
    {
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                board[i,j] = Instantiate(j % 2 == i % 2 ? boardCube1 : boardCube2, new Vector3(-4.5f + j, -3.5f + i, 0), Quaternion.identity);
                board[i, j].transform.SetParent(boardObject.transform);
            }

        }
        initPos();
        //boardObject.transform.Rotate(new Vector3(0,-20,0));
    }
    private void initPos (){
        for (int i = 0; i < BOARD_SIZE; i++){
            endPos01[i] = new Vector3(i, BOARD_SIZE - 1, 0);
            endPos02[i] = new Vector3(i, 0, 0);
        }

    }
    
    private void InitializePlayers()
    {
        initialPlayer1Pos = board[0, 4].transform.position;
        initialPlayer1Pos.z += -1;
        Instantiate(player1Prefab, initialPlayer1Pos, Quaternion.identity);
        initialPlayer2Pos = board[BOARD_SIZE - 1, 4].transform.position;
        initialPlayer2Pos.z += -1;
        Instantiate(player2Prefab, initialPlayer2Pos, Quaternion.identity);
    }
    private Vector3 RoundHitPoint(Vector3 hitPoint)
    {
        return new Vector3(Mathf.Round(hitPoint.x), Mathf.Round(hitPoint.y), -1);   
    }

    void Start()
    {
        boardObject = GameObject.Find("Board");
        playerTurn = GameObject.Find("PlayerTurn").GetComponent<TextMeshProUGUI>();
        InitializeBoard();
        InitializePlayers();
        player1Controller = GameObject.Find("Player1(Clone)").GetComponent<Player>();
        player2Controller = GameObject.Find("Player2(Clone)").GetComponent<Player>();
        gameEnded = false;
        gamePaused = false;
    }

    private void Awake()
    {
        player1Walls = GameObject.Find("Player1Walls").GetComponent<TextMeshProUGUI>();
        player2Walls = GameObject.Find("Player2Walls").GetComponent<TextMeshProUGUI>();

    }

    IEnumerator DoBFSMoves()
    {
        string tempor = "";
        List<string> moves = player1Controller.BFS();

        string tem = "";
        foreach (string m in moves)
        {
            tem += m;
        }

        foreach (string v in moves)
        {
            if (v == "U")
            {
                player1Controller.MoveUp();
            }
            else if (v == "L")
            {
                player1Controller.MoveLeft();
            }
            else if (v == "R")
            {
                player1Controller.MoveRight();
            }
            else if (v == "D")
            {
                player1Controller.MoveDown();
            }
            yield return new WaitForSeconds(0.25f);
            tempor = tempor + v;
        }
    }

    IEnumerator DoDFSMoves()
    {
        string tempor = "";
        List<string> moves = player1Controller.DFS();

        string tem = "";
        foreach (string m in moves)
        {
            tem += m;
        }


        foreach (string v in moves)
        {
            if (v == "U")
            {
                player1Controller.MoveUp();
            }
            else if (v == "L")
            {
                player1Controller.MoveLeft();
            }
            else if (v == "R")
            {
                player1Controller.MoveRight();
            }
            else if (v == "D")
            {
                player1Controller.MoveDown();
            }
            yield return new WaitForSeconds(0.25f);
            tempor = tempor + v;
        }
    }

    IEnumerator DoAStarMoves()
    {
        string tempor = "";
        List<string> moves = player1Controller.AStar();

        string tem = "";
        foreach (string m in moves)
        {
            tem += m;
        }


        foreach (string v in moves)
        {
            if (v == "U")
            {
                player1Controller.MoveUp();
            }
            else if (v == "L")
            {
                player1Controller.MoveLeft();
            }
            else if (v == "R")
            {
                player1Controller.MoveRight();
            }
            else if (v == "D")
            {
                player1Controller.MoveDown();
            }
            yield return new WaitForSeconds(0.25f);
            tempor = tempor + v;
        }
    }

    public void BFSEvent()
    {
        CloseSearchMethodModal();
        StartCoroutine(DoBFSMoves());
    }
    public void DFSEvent()
    {
        CloseSearchMethodModal();
        StartCoroutine(DoDFSMoves());
    }
    public void AStarEvent()
    {
        CloseSearchMethodModal();
        StartCoroutine(DoAStarMoves());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale != 0.0f)
        {
            if (!gamePaused)
            {
                pauseModal.SetActive(true);
                gamePaused = true;
                Time.timeScale = 0;
            }
            else
            {
                ResumeGame();
            }
        }

        if (gameEnded || gamePaused)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            wallRotation = !wallRotation;
            if (wallPlaceHolder)
            {
                wallPlaceHolder.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, wallRotation ? 90 : 0));
            }
        }


        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);


        if (Physics.Raycast(ray, out hit))
        {
            bool anyPlayerSelected = player1Controller.IsSelected() || player2Controller.IsSelected();
            
            if (hit.collider.gameObject.name.StartsWith("Board") && !anyPlayerSelected)
            {
                Vector3 wallPos = RoundHitPoint(hit.point);
                bool onCorner = wallPos.x == -5.0f || wallPos.x == 4.0f;

                if (!onCorner)
                {
                    if (!wallPlaceholderOnBoard)
                    {
                        if ((player1Turn && player1Controller.GetWallCount() > 0) || (!player1Turn && player2Controller.GetWallCount() > 0))
                        {
                            wallPlaceHolder = Instantiate(validWallPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, wallRotation ? 90 : 0)));
                            wallPlaceholderOnBoard = true;
                        }
                    }
                    else
                    {
                        wallPlaceHolder.transform.position = wallPos;
                    }
                }
            }

            else if (hit.collider.gameObject.name.Contains("Player") && wallPlaceholderOnBoard)
            {
                Destroy(wallPlaceHolder);
                wallPlaceholderOnBoard = false;
            }

        }
        else
        {
            Destroy(wallPlaceHolder);
            wallPlaceholderOnBoard = false;
        }



        if (Input.GetMouseButtonDown(0) && wallPlaceHolder && wallPlaceHolder.GetComponent<ValidWall>().IsPlaceValid())
        {
            Instantiate(wall, wallPlaceHolder.transform.position, wallPlaceHolder.transform.localRotation);
            Destroy(wallPlaceHolder);
            wallPlaceholderOnBoard = false;

            if (player1Turn)
            {
                player1Controller.DecreaseWall();
                player1Walls.SetText("Player 1: " + player1Controller.GetWallCount().ToString());
            }
            else
            {
                player2Controller.DecreaseWall();
                player2Walls.SetText("Player 2: " + player1Controller.GetWallCount().ToString());
            }
            player1Turn = !player1Turn;
            playerTurn.SetText((player1Turn ? "Player 1" : "Player 2"));
        }
        
        if (player1Controller.HasReachedEnd(1))
        {
            gameEndModal.SetActive(true);
            Time.timeScale = 0f;
            winStatus.GetComponent<TextMeshProUGUI>().SetText("Player 1 Wins !");
            gameEnded = true;
        }


        if (player2Controller.HasReachedEnd(2))
        {
            gameEndModal.SetActive(true);
            Time.timeScale = 0f;
            winStatus.GetComponent<TextMeshProUGUI>().SetText("Player 2 Wins !");
            gameEnded = true;
        }
        
        if (player2Controller.IsMyTurn())
        {
            ChangeTurn();
            Debug.Log("greedy runned ! ");
            player2Controller.Greedy();
            
        }
        
    }



    public bool GetTurn()
    {
        return this.GetComponent<GameManager>().player1Turn;
    }

    public void ChangeTurn()
    {
        this.GetComponent<GameManager>().player1Turn = !player1Turn;
        playerTurn.SetText((player1Turn ? "Player 1" : "Player 2"));
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public bool GameEnded()
    {
        return this.GetComponent<GameManager>().gameEnded;
    }

    public bool GamePaused()
    {
        return this.GetComponent<GameManager>().gamePaused;
    }

    public void ResumeGame()
    {
        gamePaused = false;
        Time.timeScale = 1.0f;
        pauseModal.SetActive(false);
    }

    public void OpenSearchMethodModal()
    {
        gamePaused = true;
        Time.timeScale = 0.0f;
        searchMethodModal.SetActive(true);
    }

    public void CloseSearchMethodModal()
    {
        gamePaused = false;
        Time.timeScale = 1.0f;
        searchMethodModal.SetActive(false);
    }
    public int EvaluationFunction()
    {
        int player1MoveCount = player1Controller.BFS().Count;
        int player2MoveCount = player2Controller.BFS().Count;

        if (player1MoveCount == 0 || player2MoveCount == 0)
        {
            return -10000;
        }

        if (player1Controller.IsMyTurn())
        {
            return player1MoveCount - player2MoveCount;

        }
        else if (player1Controller.IsMyTurn())
        {
            return player2MoveCount - player1MoveCount;
        }

        return -1000;
    }


    public bool IsWallPositionValid(Vector3 pos, bool rotation)
    {
        /* rotation:
         * true -> vertical
         * false -> horizontal    
         */
        Vector3 wallPos = RoundHitPoint(pos);
        bool onBoard = Physics.Raycast(wallPos + (rotation ? Vector3.up : Vector3.left), new Vector3(0, 0, 1), out hit, 1) &&
               Physics.Raycast(wallPos + (rotation ? Vector3.down : Vector3.right), new Vector3(0, 0, 1), out hit, 1);


        bool nearWall = false;

        Vector3[] horizontalPositions = new Vector3[] {
            new Vector3(wallPos.x + 1, wallPos.y, wallPos.z),
            new Vector3(wallPos.x - 1, wallPos.y, wallPos.z),
        };
        Vector3[] verticalPositions = new Vector3[] {
            new Vector3(wallPos.x, wallPos.y + 1, wallPos.z),
            new Vector3(wallPos.x, wallPos.y - 1, wallPos.z),
        };

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Wall"))
        {
            foreach (Vector3 h in horizontalPositions)
            {
                if (go.transform.position == h && !rotation)
                {
                    nearWall = true;
                    break;
                }
            }
            foreach (Vector3 v in verticalPositions)
            {
                if (go.transform.position == v && rotation)
                {
                    nearWall = true;
                    break;
                }
            }

        }

        return (onBoard && !nearWall);
    }

    public GameObject PlaceWall(Vector3 pos, bool rotation)
    {
        /* rotation:
         * true -> vertical
         * false -> horizontal    
         */
        if (IsWallPositionValid(pos, rotation))
        {
            Vector3 wallPos = RoundHitPoint(pos);
            GameObject placedWall = Instantiate(wall, wallPos, Quaternion.Euler(new Vector3(0, 0, rotation ? 90 : 0)));
            return placedWall;
        }
        return null;
    }
    /*
    public void RemoveWall(Vector3 pos)
    {
        Vector3 wallPos = RoundHitPoint(pos);
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Wall"))
        {
            if (go.transform.position == wallPos)
            {
                Destroy(go);
                break;
            }
        }
    }
    */
}
