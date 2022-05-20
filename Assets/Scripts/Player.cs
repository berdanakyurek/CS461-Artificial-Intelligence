using System;
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
    public Vector3 [] endPos = new Vector3 [9]; 
    public void setEndPos (Vector3 [] endPosPassed ){

         endPos = endPosPassed;

    }
    void Start()
    {
        initialPos = transform.position;
        directionObjects = new GameObject[4]; //left, right, up, down
        playerDirection = this.gameObject.name.Contains("2") ? -1 : 1;
        walls = 10;
        selected = false;
        createPlaceHolder = false;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        string playerName = this.gameObject.name.Replace("(Clone)", "");
    }
   

    public Vector3 BestWallTrue()
    {
        
            float BestX = -100;
            float BestY = -100;
            double bestEvaluation = -100;
            float z = -1;

            for (float x  = -3f; x<= 4f; x++)
            {
                for (float y = -4f; y <= 3f; y++)
                {
                    
                    if (gameManager.IsWallPositionValid(new Vector3(x, y, z), true))
                    {
                        GameObject go = gameManager.PlaceWall(new Vector3(x, y, z), true);

                        int tmp = gameManager.EvaluationFunction();
                        
                        if (tmp == -10000)
                        {
                            Destroy(go);
                            continue;
                        }
                        
                        if (bestEvaluation < tmp)
                        {
                            bestEvaluation = tmp;
                            BestX = x;
                            BestY = y;
                        }

                        Destroy(go);
                    }
                }
            }    
            return new Vector3(BestX, BestY, z);
    }
    
    public Vector3 BestWallFalse()
    {
        
        
            float BestX = -100;
            float BestY = -100;
            double bestEvaluation = -100;
            float z = -1;
            for (float y = -3f; y <= 4f; y++)
            {
                for (float x = -4f; x <= 3f; x++)
                {
                    
                    if (gameManager.IsWallPositionValid(new Vector3(x, y, z), false))
                    {
                        GameObject go = gameManager.PlaceWall(new Vector3(x, y, z), false);
                        int tmp = gameManager.EvaluationFunction();
                        if (tmp == -10000)
                        {
                            Destroy(go);
                            continue;
                        }
                        if (bestEvaluation < tmp)
                        {
                            bestEvaluation = tmp;
                            BestX = x;
                            BestY = y;

                        }
                        Destroy(go);
                    }


                }
            }
            return new Vector3(BestX, BestY, z);
    }
    

    public void Greedy()
    {
        //Vector3 tmp1 = BestWallFalse();
        Vector3 tmp2 = BestWallTrue();
        Vector3 tmp1 = Vector3.zero;

        Debug.Log("best wall yatay: " + tmp1);
        Debug.Log("best wall dikey: " + tmp2);

        GameObject go = gameManager.PlaceWall(tmp1, false);
        int eval1 = gameManager.EvaluationFunction();
        Destroy(go);

        go = gameManager.PlaceWall(tmp2, true);
        int eval2 = gameManager.EvaluationFunction();
        Destroy(go);

        int evalmove = gameManager.EvaluationFunction() + 1;

        if (eval1 > eval2 && eval1> evalmove && GetWallCount() > 0)
        {
            gameManager.PlaceWall(tmp1, false);
            DecreaseWall();
        }
        else if (eval2 >= eval1 && eval2 > evalmove && GetWallCount() > 0)
        {
            gameManager.PlaceWall(tmp2, true);
            DecreaseWall();
        }else {
            string str = BFS()[0];

            if (str == "U")
            {
                MoveUp();
            }
            else if (str == "D")
            {
                MoveDown();
            }
            else if (str == "L")
            {
                MoveLeft();
            }
            else if (str == "R")
            {
                MoveRight();
            }
        }


    }

    public bool IsMyTurn()
    {
        string playerName = this.gameObject.name.Replace("(Clone)", "");
        return GameObject.Find("PlayerTurn").GetComponent<TextMeshProUGUI>().text.Contains(playerName[playerName.Length -  1]);
    }



    public int ShortestDistanceToGoal()
    {
        return 0;
    }


    public int Heuristic()
    {
        if(this.gameObject.name.Replace("(Clone)", "") == "Player1")
        {
            return (int)((4.5 - transform.position.y));
        }
        else if (this.gameObject.name.Replace("(Clone)", "") == "Player2")
        {
            return (int)((transform.position.y + 3.5));
        }
        return 100;
    }

    public int Heuristic(double y)
    {
        if (this.gameObject.name.Replace("(Clone)", "") == "Player1")
        {
            return (int)((4.5 - y));
        }
        else if (this.gameObject.name.Replace("(Clone)", "") == "Player2")
        {
            return (int)((y + 3.5));
        }
        return 100;
    }







    public List<string> BFS()
    {
        
        Queue<List<string>> liste = new Queue<List<string>>();
        if (!HasObstacle(transform.position, new Vector3(1,0,0)))
        {
            List<string> temp = new List<string>();
            temp.Add("R");
            liste.Enqueue(temp);
        }
        if (!HasObstacle(transform.position, new Vector3(-1,0,0)))
        {
            List<string> temp = new List<string>();
            temp.Add("L");
            liste.Enqueue(temp);
        }
        if (!HasObstacle(transform.position, new Vector3(0,1,0)))
        {
            List<string> temp = new List<string>();
            temp.Add("U");
            liste.Enqueue(temp);
        }
        if (!HasObstacle(transform.position, new Vector3(0,-1,0)))
        {
            List<string> temp = new List<string>();
            temp.Add("D");
            liste.Enqueue(temp);
        }
        while (liste.Count != 0)
        {
            List<string> element = liste.Dequeue();
            List<Vector3> visited = new List<Vector3>();
            var position = transform.position;

            foreach (var v in element)
            {
                visited.Add(position);
                if (v == "R")
                {
                    position.x = position.x + 1;
                }
                if (v == "L")
                {
                    position.x = position.x - 1;
                }
                if (v == "U")
                {
                    position.y = position.y + 1;
                }
                if (v == "D")
                {
                    position.y = position.y - 1;
                }
            }
            //visited.Add(position);

            if (Heuristic(position.y) == 0)
            {
                return element;
            }


            if (visited.Contains(position))
            {
                continue;
            }

            if (!HasObstacle(position, new Vector3( 1,0,0))   )
            {
                List<string> temp = new List<string>(element);
                temp.Add("R");
                liste.Enqueue(temp);
            }
            if (!HasObstacle(position, new Vector3(-1,0,0)))
            {
                List<string> temp = new List<string>(element);
                temp.Add("L");
                liste.Enqueue(temp);
            }
            if (!HasObstacle(position, new Vector3(0, 1,0)))
            {
                List<string> temp = new List<string>(element);
                temp.Add("U");
                liste.Enqueue(temp);
            }
            if (!HasObstacle(position, new Vector3(0,-1,0)))
            {
                List<string> temp = new List<string>(element);
                temp.Add("D");
                liste.Enqueue(temp);
            }



        }

        return new List<string>();
    }



    public List<string> DFS()
    {

        Stack<List<string>> liste = new Stack<List<string>>();
        if (!HasObstacle(transform.position, new Vector3(1, 0, 0)))
        {
            List<string> temp = new List<string>();
            temp.Add("R");
            liste.Push(temp);
        }
        if (!HasObstacle(transform.position, new Vector3(-1, 0, 0)))
        {
            List<string> temp = new List<string>();
            temp.Add("L");
            liste.Push(temp);
        }
        if (!HasObstacle(transform.position, new Vector3(0, 1, 0)))
        {
            List<string> temp = new List<string>();
            temp.Add("U");
            liste.Push(temp);
        }
        if (!HasObstacle(transform.position, new Vector3(0, -1, 0)))
        {
            List<string> temp = new List<string>();
            temp.Add("D");
            liste.Push(temp);
        }
        while (liste.Count != 0)
        {
            List<string> element = liste.Pop();
            List<Vector3> visited = new List<Vector3>();
            var position = transform.position;

            foreach (var v in element)
            {
                visited.Add(position);
                if (v == "R")
                {
                    position.x = position.x + 1;
                }
                if (v == "L")
                {
                    position.x = position.x - 1;
                }
                if (v == "U")
                {
                    position.y = position.y + 1;
                }
                if (v == "D")
                {
                    position.y = position.y - 1;
                }
            }
            //visited.Add(position);

            if (Heuristic(position.y) == 0)
            {
                return element;
            }


            if (visited.Contains(position))
            {
                continue;
            }

            if (!HasObstacle(position, new Vector3(1, 0, 0)))
            {
                List<string> temp = new List<string>(element);
                temp.Add("R");
                liste.Push(temp);
            }
            if (!HasObstacle(position, new Vector3(-1, 0, 0)))
            {
                List<string> temp = new List<string>(element);
                temp.Add("L");
                liste.Push(temp);
            }
            if (!HasObstacle(position, new Vector3(0, 1, 0)))
            {
                List<string> temp = new List<string>(element);
                temp.Add("U");
                liste.Push(temp);
            }
            if (!HasObstacle(position, new Vector3(0, -1, 0)))
            {
                List<string> temp = new List<string>(element);
                temp.Add("D");
                liste.Push(temp);
            }



        }

        return new List<string>();
    }


    public List<string> AStar()
        {
            
            Queue<List<string>> liste = new Queue<List<string>>();
            if (!HasObstacle(transform.position, new Vector3(1,0,0)))
            {
                List<string> temp = new List<string>();
                temp.Add("R");
                liste.Enqueue(temp);
            }
            if (!HasObstacle(transform.position, new Vector3(-1,0,0)))
            {
                List<string> temp = new List<string>();
                temp.Add("L");
                liste.Enqueue(temp);
            }
            if (!HasObstacle(transform.position, new Vector3(0,1,0)))
            {
                List<string> temp = new List<string>();
                temp.Add("U");
                liste.Enqueue(temp);
            }
            if (!HasObstacle(transform.position, new Vector3(0,-1,0)))
            {
                List<string> temp = new List<string>();
                temp.Add("D");
                liste.Enqueue(temp);
            }
            while (liste.Count != 0)
            {
                List<string> element = liste.Dequeue();
                List<Vector3> visited = new List<Vector3>();
                var position = transform.position;

                foreach (var v in element)
                {
                    visited.Add(position);
                    if (v == "R")
                    {
                        position.x = position.x + 1;
                    }
                    if (v == "L")
                    {
                        position.x = position.x - 1;
                    }
                    if (v == "U")
                    {
                        position.y = position.y + 1;
                    }
                    if (v == "D")
                    {
                        position.y = position.y - 1;
                    }
                }
                //visited.Add(position);

                if (Heuristic(position.y) == 0)
                {
                    return element;
                }


                if (visited.Contains(position))
                {
                    continue;
                }


                double hR = 0;
                double hL = 0;
                double hU = 0;
                double hD = 0;

                double minH = 0;

                bool minHeuristicR = false;
                bool minHeuristicL = false;
                bool minHeuristicU = false;
                bool minHeuristicD = false;

                foreach (var v in element)
                {
                    visited.Add(position);
                    if (v == "R")    
                        {hR = Heuristic(position.y);}
                        
                    if (v == "L")    
                        {hL = Heuristic(position.y);}
                            
                    if (v == "U")    
                        {hU = Heuristic(position.y);}
                        
                    if (v == "D")    
                        {hD = Heuristic(position.y);}

                    
                    minH = Math.Min(Math.Min(Math.Min(hR, hL) ,hU), hD);

                    if (minH == hR)    
                        {minHeuristicR = true;}
                        
                    if (minH == hL)    
                        {minHeuristicL = true;}
                            
                    if (minH == hU)    
                        {minHeuristicU = true;}
                        
                    if (minH == hD)    
                        {minHeuristicD = true;}
                
                }
            
                if (!HasObstacle(position, new Vector3( 1,0,0)) && minHeuristicR == true)
                {
                    List<string> temp = new List<string>(element);
                    temp.Add("R");
                    liste.Enqueue(temp);
                }
                if (!HasObstacle(position, new Vector3(-1,0,0)) && minHeuristicL == true)
                {
                    List<string> temp = new List<string>(element);
                    temp.Add("L");
                    liste.Enqueue(temp);
                }
                if (!HasObstacle(position, new Vector3(0, 1,0)) && minHeuristicU == true)
                {
                    List<string> temp = new List<string>(element);
                    temp.Add("U");
                    liste.Enqueue(temp);
                }
                if (!HasObstacle(position, new Vector3(0,-1,0)) && minHeuristicD == true)
                {
                    List<string> temp = new List<string>(element);
                    temp.Add("D");
                    liste.Enqueue(temp);
                }
            }
            return new List<string>();
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

    private bool HasObstacle(Vector3 dir, Vector3 dir1)
    {
        if (Physics.Raycast(dir, transform.TransformDirection(dir1), out hit, 1))
        {
            return true;
        }
        else if (!Physics.Raycast(dir + dir1, new Vector3(0, 0, 1), out hit, 1))
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
    }

    public void MoveUp()
    {
        transform.position = transform.position + Vector3.up;
    }

    public void MoveRight()
    {
        transform.position = transform.position + Vector3.right;
    }

    public void MoveDown()
    {
        transform.position = transform.position + Vector3.down;
    }

    public Vector3 GetPlayerPosition()
    {
        return this.transform.position;
    }

    public bool HasReachedEnd(int val)
    {
        return Math.Abs(initialPos.y - transform.position.y) >= 8;
    }
}
