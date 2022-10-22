using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControl : MonoBehaviour
{
    public Transform groundCheck;
    public LayerMask groundMask;
    public LayerMask waterMask;
    public float speed;
    public Joystick stick;
    
    public Animator anim;
    
    public float threshold = .2f;

    public float g;
    public float checkRadius;

    public float turnSpeed = .5f;

    public GameObject splashParticle;
    public GameObject exitParticle;

    public static playerControl current;

    private float vel;

    private bool swimming;

    private void Start()
    {
        if (SaveSystem.playerSaved())
            load();
    }

    void load()
    {
        PlayerData data = GameManager.instance.data;
        Vector3 loc = Vector3.zero;
        for (int i = 0; i < 3; i++)
            loc[i] = data.pos[i];
        setPos(loc);
        GetComponentInChildren<CarryPoint>().curScene = data.curScene;
        Transform pickupPoint = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(2);
        foreach (pickUpData pickup in data.items)
        {
            GameObject log = Instantiate(GameManager.instance.pickupTypes[pickup.type], new Vector3(0, -1000, 0), Quaternion.identity, null);
            log.transform.localScale = new Vector3(pickup.scale[0], pickup.scale[1], pickup.scale[2]);
            StartCoroutine(pickupPoint.GetComponent<CarryPoint>().addLog(log, true));
        }
    }

    public void setPos(Vector3 vec)
    {
        GetComponent<CharacterController>().enabled = false;
        transform.position = vec;
        GetComponent<CharacterController>().enabled = true;
    }

    private void Update()
    {
        if (Physics.CheckSphere(groundCheck.position, checkRadius, waterMask))
        {
            if (swimming == false)
            {
                GameObject p = Instantiate(splashParticle, transform.position, splashParticle.transform.rotation, null);
                p.transform.localScale = new Vector3(.6f, .6f, .6f);
            }
            anim.SetBool("Swimming", true);
            swimming = true;
        }
        else
        {
            if (swimming == true)
            {
                GameObject p = Instantiate(exitParticle, transform.position, exitParticle.transform.rotation, null);
                p.transform.localScale = new Vector3(.6f, .6f, .6f);
            }
            anim.SetBool("Swimming", false);
            swimming = false;
        }

        Vector3 dir = new Vector3(stick.Horizontal, 0, stick.Vertical);

        if (dir.magnitude > threshold)
        {
            anim.SetBool("Moving", true);
            anim.SetBool("Picking", false);
            anim.speed = dir.magnitude;
            if (swimming)
            {
                anim.speed *= 1.5f;
            }

            dir = Quaternion.Euler(0, GameObject.Find("Main Camera").transform.eulerAngles.y, 0) * dir;

            transform.forward = Vector3.Lerp(transform.forward, dir, turnSpeed * Time.deltaTime);

            GetComponent<CharacterController>().Move(transform.forward*Mathf.Min(dir.magnitude, 1)*Time.deltaTime*speed);
        }
        else
        {
            anim.speed = 1;
            anim.SetBool("Moving", false);
        }

        if (!Physics.CheckSphere(groundCheck.position, checkRadius, groundMask))
        {
            vel += g * Time.deltaTime;
            GetComponent<CharacterController>().Move(new Vector3(0, vel, 0));
        }
        else
        {
            vel = 0;
        }
    }
}
