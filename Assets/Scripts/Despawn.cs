using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    public float timeUntilDespawn = 5f;
    void Start()
    {
        Invoke("Die", timeUntilDespawn);
    }

    void Die() {
        Destroy(gameObject);
    }
}
