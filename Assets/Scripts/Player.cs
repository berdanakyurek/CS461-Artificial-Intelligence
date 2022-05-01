using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{

    private RaycastHit hit;

    public bool selected;
    private bool createPlaceHolder;
    public GameObject ballPlaceHolder;
    private GameObject[] directionObjects; //direction placeholder objects
    private GameManager gameManager;
    private Vector3 initialPos;
        
    private int playerDirection;
    private int walls;
    void Start()
    {
        initialPos = transform.position;
        directionObjects = new GameObject[4]; //left, right, up, down
        playerDirection = this.gameObject.name.Contains("2") ? -1 : 1;
        walls = 10;
        selected = false;
        createPlaceHolder = false;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private bool IsMyTurn()
    {
        string playerName = this.gameObject.name.Replace("(Clone)", "");
        return GameObject.Find("PlayerTurn").GetComponent<TextMeshProUGUI>().text.Contains(playerName[playerName.Length -  1]);
    }
    
    void Update()
    {
        
        if (gameManager.GameEnded() ||gameManager.GamePaused())
        {
            return;
        }


        if (Input.GetMouseButtonDown(0) && IsMyTurn())
        {
            bool mousePlayer = MouseOnPlayer();
            if (mousePlayer && !selected)
            {
                selected = true;
                createPlaceHolder = true;
            }

            else if (mousePlayer && selected)
            {
                selected = false;
                ClearPlaceHolders();
            }

            GameObject go = MouseOnPlaceHolder();
            if (go && go.name.StartsWith("BallPlaceHolder") && selected)
            {
                gameManager.ChangeTurn();
                transform.position = go.transform.position;
                ClearPlaceHolders();
                selected = false;
            }
        }

        if (selected && createPlaceHolder)
        {
            
            if (!HasObstacle(Vector3.up * playerDirection))
            {
                directionObjects[0] = Instantiate(ballPlaceHolder, (Vector3.up * playerDirection) + transform.position, Quaternion.identity);
            }
            
            if (!HasObstacle(Vector3.left))
            {   
                directionObjects[1] = Instantiate(ballPlaceHolder, Vector3.left + transform.position, Quaternion.identity);
            }
            if (!HasObstacle(Vector3.right))
            {
                directionObjects[2] = Instantiate(ballPlaceHolder, Vector3.right + transform.position, Quaternion.identity);
            }
            if (!HasObstacle(Vector3.down * playerDirection))
            {
                directionObjects[3] = Instantiate(ballPlaceHolder, (Vector3.down * playerDirection) + transform.position, Quaternion.identity);
            }
            createPlaceHolder = false;
        }



    }

    private bool HasObstacle(Vector3 dir)
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(dir), out hit, 1))
        {
            return true;
        }
        else if (!Physics.Raycast(transform.position + dir, new Vector3(0,0,1), out hit, 1))
        {
            return true;
        }
        return false;
    }

    private void ClearPlaceHolders()
    {
        for (int i = 0; i < directionObjects.Length; i++)
        {
            Destroy(directionObjects[i]);
        }
    }


    private GameObject MouseOnPlaceHolder()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }


    private bool MouseOnPlayer()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject.name.StartsWith(this.gameObject.name);
        }
        return false;
    }

    public bool IsSelected()
    {
        return this.GetComponent<Player>().selected;
    }

    public int GetWallCount()
    {
        return this.GetComponent<Player>().walls;
    }

    public void DecreaseWall()
    {
        this.GetComponent<Player>().walls -= 1;
    }

    public void MoveLeft()
    {
        transform.position = transform.position + Vector3.left;
        gameManager.ChangeTurn();
    }

    public void MoveUp()
    {
        transform.position = transform.position + Vector3.up * playerDirection;
        gameManager.ChangeTurn();
    }

    public void MoveRight()
    {
        transform.position = transform.position + Vector3.right;
        gameManager.ChangeTurn();
    }

    public void MoveDown()
    {
        transform.position = transform.position + Vector3.down * playerDirection;
    }

    public Vector3 GetPlayerPosition()
    {
        return this.transform.position;
    }

    public bool HasReachedEnd()
    {
        return Vector3.Distance(initialPos, transform.position) >= 8; 
    }



}
