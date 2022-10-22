using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cams : MonoBehaviour
{
    public int camID; // (0) Menu (1) Load (2) Player

    private void Update()
    {
        GetComponent<Camera>().enabled = (GameManager.instance.camChoice == camID);
    }
}
