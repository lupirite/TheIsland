using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public float startDist = 3;
    void Start()
    {
        ProgressionManagement.instances[gameObject.scene.buildIndex].spawnPoint = transform.position + new Vector3(Random.Range(-startDist, startDist), 0, Random.Range(-startDist, startDist));
    }
}
