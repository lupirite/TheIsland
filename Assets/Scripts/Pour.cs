using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pour : MonoBehaviour
{
    public float activationDist = 1;
    public LayerMask playerMask;
    public float timeToActivate = 1;
    public Transform playerPos;
    public float lerpSpeed = .5f;
    public Transform pourPos;
    public Transform ingotPos;
    public GameObject metalOb;

    bool setPos = false;
    public float time = 0;
    int mat;
    private void Update()
    {
        mat = -1;
        float[] inv = metalOb.GetComponent<Smelter>().inventory;
        for (int i = 1; i <= inv.Length; i++)
        {
            if (inv[inv.Length-i] >= 1)
            {
                mat = inv.Length-i;
            }
        }
        if (mat != -1)
        {
            Collider[] hitColliders = Physics.OverlapSphere(pourPos.position, activationDist, playerMask);
            foreach (var hitCollider in hitColliders)
            {
                Transform player = hitCollider.transform;
                Animator anim = player.GetComponentInChildren<Animator>();
                if (!anim.GetBool("Moving") && !anim.GetBool("Carry"))
                {
                    if (time >= timeToActivate)
                    {
                        anim.SetBool("Pulling", true);
                        player.GetComponent<CharacterController>().enabled = false;
                        player.GetComponent<playerControl>().enabled = false;
                        if (Vector3.Distance(player.position, playerPos.position) < .01f && !setPos)
                        {
                            GetComponent<Animator>().SetTrigger("Pour");
                            setPos = true;
                        }
                        if (setPos)
                        {
                            player.position = playerPos.position;
                            player.rotation = playerPos.rotation;
                        }
                        else
                        {
                            player.position = Vector3.Lerp(player.position, playerPos.position, lerpSpeed * Time.deltaTime);
                            player.rotation = Quaternion.Lerp(player.rotation, playerPos.rotation, lerpSpeed * 2 * Time.deltaTime);
                        }
                    }
                    else
                    {
                        time += Time.deltaTime;
                    }
                }
                else
                {
                    time = Mathf.Min(0, time+Time.deltaTime);
                    anim.SetBool("Pulling", false);
                }
            }
            if (hitColliders.Length == 0)
            {
                time = Mathf.Min(0, time + Time.deltaTime);
            }
        }
    }

    public void createIngot()
    {
        float[] inv = metalOb.GetComponent<Smelter>().inventory;
        if (inv[mat] >= 1)
        {
            metalOb.GetComponent<Smelter>().inventory[mat] -= 1;
            Instantiate(GameManager.instance.pickupTypes[metalOb.GetComponent<Smelter>().metalInfo[mat].itemId], ingotPos.position, ingotPos.rotation, worldGen.instances[gameObject.scene.buildIndex].pickupParent);
            metalOb.GetComponent<Smelter>().EvaluateHeight();
        }
    }

    public IEnumerator dropPlayer()
    {
        Transform player = GameManager.instance.player.transform;
        time = -35;
        player.GetComponentInChildren<Animator>().SetBool("Pulling", false);
        yield return new WaitForSeconds(player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length);
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<playerControl>().enabled = true;
        setPos = false;
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pourPos.position, activationDist);
    }
}
