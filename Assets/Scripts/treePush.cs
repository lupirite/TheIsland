using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treePush : MonoBehaviour
{
    float repelSpeed = 20f;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 relPos = (other.transform.position - (GetComponent<SphereCollider>().center+transform.position));
            other.GetComponent<playerControl>().setPos(other.transform.position + relPos.normalized * Mathf.Pow(1-relPos.magnitude/GetComponent<SphereCollider>().radius, 2) * repelSpeed * Time.deltaTime);
        }
    }
}
