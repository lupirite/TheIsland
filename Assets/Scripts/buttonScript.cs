using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonScript : MonoBehaviour
{
    public void Play()
    {
        FindObjectOfType<GameManager>().LoadGame();
    }
}
