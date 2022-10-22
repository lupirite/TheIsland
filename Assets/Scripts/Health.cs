using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 100;
    public GameObject stump;
    public GameObject log;
    public GameObject particle;
    public int ID;

    public float logSizeMultiplier = 2;
    public float logSizeFalloff = .6f;
    public float logAmount = 8;
    public float logAmountOffset = -1.5f;

    public bool dead = false;

    public void Start()
    {
        if (worldGen.instances[gameObject.scene.buildIndex].trees[ID])
        {
            dead = true;
        }
        if (!dead)
        {
            health = (int)((float)health * transform.localScale.magnitude);
        }
        else
        {
            if (stump != null)
            {
                GameObject gO = Instantiate(stump, transform.position, Quaternion.Euler(transform.eulerAngles + stump.transform.eulerAngles), transform.parent);
                gO.transform.localScale = transform.localScale * stump.transform.localScale.magnitude;
                if (gO.GetComponent<AudioSource>())
                {
                    Destroy(gO.GetComponent<playRandomSound>());
                }
            }
            Destroy(gameObject);
        }
    }

    public void takeDamage(int amount) {
        health -= amount;
        if (health < 0 && !dead) {
            dead = true;
            StartCoroutine(Die());
        }
    }

    public IEnumerator Die() {
        worldGen.instances[gameObject.scene.buildIndex].killTree(ID);
        if (stump != null) {
            GameObject gO = Instantiate(stump, transform.position, Quaternion.Euler(transform.eulerAngles + stump.transform.eulerAngles), transform.parent);
            gO.transform.localScale = transform.localScale*stump.transform.localScale.magnitude;
        }
        Destroy(GetComponent<BoxCollider>());
        Destroy(GetComponent<CapsuleCollider>());

        Animator anim = transform.GetComponent<Animator>();
        if (anim != null) {
            anim.SetTrigger("Fall");
            anim.speed /= transform.localScale.magnitude;
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        }

        float distance = 0;
        for (int i = 0; i < transform.localScale.y*logAmount+logAmountOffset; i++) {
            float logSize = 1f/(i*logSizeFalloff+1f)*transform.localScale.magnitude*logSizeMultiplier;
            distance += logSize;
            GameObject gO = Instantiate(log, transform.position + transform.rotation*new Vector3(0, distance, 0), Quaternion.Euler(transform.eulerAngles + log.transform.eulerAngles), null);
            gO.transform.localScale *= logSize;
            gO.transform.parent = GameObject.Find("Pickups").transform;
            distance += logSize;
        }
        Destroy(gameObject);
    }
}
