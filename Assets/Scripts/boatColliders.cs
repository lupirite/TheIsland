using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatColliders : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        print("flue");
        if (collision.gameObject.CompareTag("tile"))
        {
            transform.parent.parent.position = transform.parent.parent.position + transform.parent.parent.right * Time.deltaTime * 5;
        }
    }
}
