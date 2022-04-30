using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{

    //horizontal(false) and vertical(true)
    private bool wallRotation = false;

    private RaycastHit hit;
    public GameObject boardCube1, boardCube2;
    private GameObject boardObject;
    public GameObject player1Prefab, player2Prefab;
    private GameObject player1, player2;
    public GameObject wall;
    
    public const int BOARD_SIZE = 9;
    public GameObject[,] board = new GameObject[BOARD_SIZE,BOARD_SIZE];
    private Vector3 initialPlayer1Pos, initialPlayer2Pos;
    
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
        InitializeBoard();
        InitializePlayers();

    }

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Space))
        {
            wallRotation = !wallRotation;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name.StartsWith("Board"))
                {
                    Vector3 wallPos = RoundHitPoint(hit.point);
                    Instantiate(wall, wallPos, Quaternion.Euler(new Vector3(0, 0, wallRotation ? 90 : 0)));
                }
            }
        }

    }

    /*
    void CheckGameEnded()
    {
        
        if (player1.transform.position.y - initialPlayer1Pos.y == BOARD_SIZE - 1)
        {
            Debug.Log("Player 1 Wins !");
        }
        else if (initialPlayer2Pos.y - player2.transform.position.y == BOARD_SIZE - 1)
        {
            Debug.Log("Player 2 Wins !");
        }
    }
    */
    
}
