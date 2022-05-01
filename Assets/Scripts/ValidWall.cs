using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidWall : MonoBehaviour
{

    private bool placeValid;
    private Material validMaterial, invalidMaterial;
    void Start()
    {
        placeValid = true;
        validMaterial = Resources.Load("validMaterial", typeof(Material)) as Material;
        invalidMaterial = Resources.Load("invalidMaterial", typeof(Material)) as Material;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Contains("Wall"))
        {
            placeValid = false;
            this.GetComponent<MeshRenderer>().material = invalidMaterial;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        this.GetComponent<MeshRenderer>().material = validMaterial;
        placeValid = true;
    }


    public bool IsPlaceValid()
    {
        return this.GetComponent<ValidWall>().placeValid;
    }
}
