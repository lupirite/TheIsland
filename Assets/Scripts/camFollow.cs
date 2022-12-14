using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camFollow : MonoBehaviour
{
    public float height = 7f;
    public float camRestDist = 4f;
    public float minDistToWall = .2f;
    public LayerMask tileMask;
    public float camLerpSpeed = .5f;
    public float maxJoyYVal = -.5f;
    public GameObject player;
    public Transform playerHead;

    public float boatHeight = 16;
    public float boatCamRestDist = 10;
    public bool boat = false;

    Vector3 camDir;

    public static GameObject current;
    private void Start()
    {
        current = gameObject;
        camDir = -playerHead.transform.right;
    }


    void Update()
    {
        Vector3 camOffset;
        RaycastHit hit;
        if (player.GetComponent<playerControl>().stick.Vertical > maxJoyYVal || boat) {
            camDir = Vector3.Lerp(camDir, -playerHead.transform.right, camLerpSpeed*Time.deltaTime);
        }
        Vector3 oppCamDir = new Vector3(-camDir.x, 0, -camDir.z).normalized;
        if (!boat)
            camOffset = new Vector3(0, height, 0) + oppCamDir*camRestDist;
        else
            camOffset = new Vector3(0, boatHeight, 0) + oppCamDir * boatCamRestDist;

        if (Physics.Raycast(playerHead.transform.position, camOffset, out hit, camOffset.magnitude, tileMask)) {
            transform.position = playerHead.transform.position+camOffset.normalized*Mathf.Max(0, hit.distance-minDistToWall);
        }
        else {
            transform.position = playerHead.position + camOffset;
        }
        
        transform.LookAt(playerHead.position);
    }
}
