using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entrance : MonoBehaviour
{
    public static entrance[] instances;
    public Transform exitPos;
    public int toScene;
    public bool secondary;
    public bool toSecondary;
    void Start()
    {
        if (instances == null)
        {
            instances = new entrance[20];
        }
        if (secondary)
        {
            instances[gameObject.scene.buildIndex + 5] = this;
        }
        else
        {
            instances[gameObject.scene.buildIndex] = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerControl.current.transform.Rotate(0, 180, 0);
            ProgressionManagement.instances[toScene].LoadLevel();
            if (toSecondary)
                other.GetComponent<playerControl>().setPos(instances[toScene+5].exitPos.position);
            if (!toSecondary)
                other.GetComponent<playerControl>().setPos(instances[toScene].exitPos.position);
            ProgressionManagement.instances[gameObject.scene.buildIndex].UnloadLevel();
            other.transform.GetComponentInChildren<CarryPoint>().curScene = toScene;
        }
    }
}
