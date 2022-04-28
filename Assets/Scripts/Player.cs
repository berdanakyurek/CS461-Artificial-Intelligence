using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int remainingWalls;

    void Start()
    {
        remainingWalls = 10;    
    }

    
    void Update()
    {
        
    }

    public void DecreaseWall()
    {
        remainingWalls -= 1;
    }

    public void MoveLeft()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x - 1, pos.y, pos.z);
    }

    public void MoveRight()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x + 1, pos.y, pos.z);
    }
    public void MoveUp()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y + 1, pos.z);
    }
    public void MoveDown()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y - 1, pos.z);
    }

}
