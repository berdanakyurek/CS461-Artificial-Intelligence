using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{

    //horizontal(false) and vertical(true)
    private bool wallRotation = false;

    private RaycastHit hit;
    public GameObject boardCube1, boardCube2;
    private GameObject boardObject;
    public GameObject player1Prefab, player2Prefab;
    private Player player1Controller, player2Controller;
    public GameObject wall;
    public GameObject gameEndModal, winStatus, pauseModal;
    
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
        //boardObject.transform.Rotate(new Vector3(0,-20,0));
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
        wallPlaceholderOnBoard = false;

        
    }

    private void Awake()
    {
        player1Walls = GameObject.Find("Player1Walls").GetComponent<TextMeshProUGUI>();
        player2Walls = GameObject.Find("Player2Walls").GetComponent<TextMeshProUGUI>();

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
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
            playerTurn.SetText("Turn: " + (player1Turn ? "Player 1" : "Player 2"));

            string tempor = "";
            foreach (var v in player1Controller.BFS())
            {
                tempor = tempor + v;
            }
            Debug.Log(tempor + "\n");

            tempor = "";
            foreach (var v in player2Controller.BFS())
            {
                tempor = tempor + v;
            }
            Debug.Log(tempor + "\n");
        }
        
        if (player1Controller.HasReachedEnd())
        {
            gameEndModal.SetActive(true);
            Time.timeScale = 0f;
            winStatus.GetComponent<TextMeshProUGUI>().SetText("Player 1 Wins !");
            gameEnded = true;
        }

        if (player2Controller.HasReachedEnd())
        {
            gameEndModal.SetActive(true);
            Time.timeScale = 0f;
            winStatus.GetComponent<TextMeshProUGUI>().SetText("Player 2 Wins !");
            gameEnded = true;
        }
    }


    public bool GetTurn()
    {
        return this.GetComponent<GameManager>().player1Turn;
    }

    public void ChangeTurn()
    {
        this.GetComponent<GameManager>().player1Turn = !player1Turn;
        playerTurn.SetText("Turn: " + (player1Turn ? "Player 1" : "Player 2"));
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
}
