using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tailPoint : MonoBehaviour
{
    public float tailStiffness = 10;
    public Transform nextBone;

    Transform tailBone;
    Transform player;
    Vector3 startPos;
    private void Start()
    {
        player = transform.root;
        tailBone = transform.parent;
        transform.parent = null;
        Invoke("lateStart", .01f);

        transform.rotation = Quaternion.identity;
        startPos = Quaternion.Inverse(player.rotation)*(transform.position-player.GetChild(1).GetChild(0).GetChild(0).position);
    }

    void lateStart()
    {
        tailBone.parent = null;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.GetChild(1).GetChild(0).GetChild(0).position+player.rotation*startPos, tailStiffness* Time.deltaTime);
        tailBone.position = Vector3.Lerp(tailBone.position, transform.position, tailStiffness * Time.deltaTime);
        if (nextBone)
        {
            Quaternion rot = transform.rotation;
            transform.LookAt(nextBone);
            transform.rotation = Quaternion.Lerp(rot, transform.rotation, tailStiffness * Time.deltaTime);
            if (!nextBone.GetComponent<tailPoint>().nextBone)
            {
                nextBone.rotation = transform.rotation;
            }
        }
        tailBone.LookAt(tailBone.position-transform.right, -player.forward);
    }
}
