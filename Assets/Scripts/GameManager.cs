using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{

    //horizontal(false) and vertical(true)
    private bool wallRotation = false;

    private RaycastHit hit;
    public GameObject boardCube1;
    public GameObject boardCube2;
    public GameObject player1;
    public GameObject player2;
    public GameObject wall;
    public GameObject boardObject;
    public const int BOARD_SIZE = 9;
    public GameObject[,] board = new GameObject[BOARD_SIZE,BOARD_SIZE];
    private bool wallPresent = false;
    private GameObject wallPlaceHolder;

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
        Vector3 player1Pos = board[0, 4].transform.position;
        player1Pos.z += -1;
        Instantiate(player1, player1Pos, Quaternion.identity);
        Vector3 player2Pos = board[8, 4].transform.position;
        player2Pos.z += -1;
        Instantiate(player2, player2Pos, Quaternion.identity);
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

    private bool PointOnWall(Vector3 pos)
    {
        return pos.x >= board[0, 0].transform.position.x
            && pos.x <= board[0, BOARD_SIZE - 1].transform.position.x
            && pos.y >= board[BOARD_SIZE - 1, 0].transform.position.y
            && pos.y <= board[0, 0].transform.position.y;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            wallRotation = !wallRotation;
        }

        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit))
        {
            
            Vector3 wallPos = RoundHitPoint(hit.point);
            if (PointOnWall(wallPos) && wallPresent)
            {
                wallPlaceHolder = Instantiate(wall, wallPos, Quaternion.Euler(new Vector3(0, 0, wallRotation ? 90 : 0)));
                wallPresent = true;
            }
            else if (PointOnWall(wallPos))
            {
                Vector3 newPos = wallPos;
                wallPlaceHolder.transform.position = new Vector3(newPos.x , newPos.y, -1);
            }
        }


        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos1 = Input.mousePosition;
            Ray ray1 = Camera.main.ScreenPointToRay(mousePos1);

            if (Physics.Raycast(ray1, out hit))
            {
                Vector3 wallPos = RoundHitPoint(hit.point);
                Instantiate(wall, wallPos, Quaternion.Euler(new Vector3(0, 0, wallRotation ? 90 : 0)));
            }
        }
    }

    
}
