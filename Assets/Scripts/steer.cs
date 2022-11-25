using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class steer : MonoBehaviour
{
    public float boteSpeed;
    public float threshold = .05f;
    public float turnSpeed = .1f;

    public float activationDist = 1;
    public LayerMask playerMask;
    public float timeToActivate = 1;
    public float doubleClickThreshold;

    public Transform hitPos;
    public LayerMask tileMask;

    private void Start()
    {
        Vector3 botPos = worldGen.instances[gameObject.scene.buildIndex].boatPos;
        if (botPos != Vector3.zero)
        {
            transform.parent.position = botPos;
            transform.parent.eulerAngles = worldGen.instances[gameObject.scene.buildIndex].boatRot;
        }
    }

    float time = 0;
    float timeSinceClick;
    private void Update()
    {
        bool collided = false;
        if (Physics.CheckSphere(hitPos.position, 1, tileMask))
        {
            collided = true;
        }

        timeSinceClick += Time.deltaTime;
        if (Input.GetButtonDown("Fire1"))
        {
            if (timeSinceClick < doubleClickThreshold)
            {
                dropPlayer();
            }
            timeSinceClick = 0;
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, activationDist, playerMask);
        foreach (var hitCollider in hitColliders)
        {
            Transform player = hitCollider.transform;
            Animator anim = player.GetComponentInChildren<Animator>();
            if (!anim.GetBool("Moving") && !anim.GetBool("Carry"))
            {
                if (time >= timeToActivate)
                {
                    //anim.SetBool("Pulling", true);
                    GameManager.instance.cam.GetComponent<camFollow>().boat = true;
                    player.GetComponent<CharacterController>().enabled = false;
                    player.GetComponent<playerControl>().enabled = false;

                    Joystick stick = player.GetComponent<playerControl>().stick;
                    Vector3 dir = new Vector3(stick.Horizontal, 0, stick.Vertical);
                    dir = Quaternion.Euler(0, GameObject.Find("Main Camera").transform.eulerAngles.y, 0) * dir;

                    transform.parent.right = -Vector3.Lerp(-transform.parent.right, dir, turnSpeed * Time.deltaTime);

                    if (!collided)
                        transform.parent.position = transform.parent.position - transform.parent.right * Mathf.Min(dir.magnitude, 1) * Time.deltaTime * boteSpeed;
                    else if (dir.magnitude == 0)
                        transform.parent.position = transform.parent.position + transform.parent.right * Time.deltaTime * 5;

                    Vector3 pos = transform.position;
                    player.position = pos;
                    player.rotation = transform.rotation;
                }
                else
                {
                    time += Time.deltaTime;
                }
            }
            else
            {
                time = Mathf.Min(0, time + Time.deltaTime);
                //anim.SetBool("Pulling", false);
            }
        }
        if (hitColliders.Length == 0)
        {
            time = Mathf.Min(0, time + Time.deltaTime);
        }
    }

    public void dropPlayer()
    {
        Transform player = GameManager.instance.player.transform;
        GameManager.instance.cam.GetComponent<camFollow>().boat = false;
        time = -3;
        //player.GetComponentInChildren<Animator>().SetBool("Pulling", false);
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<playerControl>().enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, activationDist);
    }
}
