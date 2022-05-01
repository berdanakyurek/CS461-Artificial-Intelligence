using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{

    private RaycastHit hit;

    public bool selected = false;                                                 
    private bool createPlaceHolder = false;
    public GameObject ballPlaceHolder;
    private GameObject[] directionObjects; //direction placeholder objects

    private int playerDirection;
    void Start()
    {  
        remainingWalls = 10;
        directionObjects = new GameObject[3];
        playerDirection = this.gameObject.name.Contains("2") ? -1 : 1;
    }

    
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
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
            createPlaceHolder = false;
        }



    }

    private bool HasObstacle(Vector3 dir)
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(dir), out hit, 1))
        {
            Debug.Log(hit.collider.gameObject.name);
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

}
