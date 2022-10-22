using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    public LayerMask destructableMask;
    public Animator anim;
    public float attackDist = .8f;
    public GameObject leftHand, rightHand;
    public GameObject carryPoint;

    bool attacked = false;

    Transform player;
    void Start() {
        player = transform.parent.parent.parent.parent.parent;
    }
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(1).normalizedTime%(anim.GetCurrentAnimatorStateInfo(1).length*anim.GetCurrentAnimatorStateInfo(1).speed) < .1f && !attacked) {
            attacked = true;
            if (carryPoint.transform.childCount == 0)
                attack();
        } else if (anim.GetCurrentAnimatorStateInfo(1).normalizedTime%(anim.GetCurrentAnimatorStateInfo(1).length*anim.GetCurrentAnimatorStateInfo(1).speed) > .55f && attacked) {
            attacked = false;
            if (carryPoint.transform.childCount == 0)
                attack();
        }
    }

    public void setHands(bool active) {
        leftHand.SetActive(active);
        rightHand.SetActive(active);
    }

    void attack() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackDist, destructableMask);

        anim.SetBool("Attack", hitColliders.Length > 0);
        
        foreach (var hitCollider in hitColliders)
        {
            GameObject p = Instantiate(hitCollider.gameObject.GetComponent<Health>().particle, transform.position, hitCollider.gameObject.GetComponent<Health>().particle.transform.rotation, null);
            p.transform.localScale = new Vector3(.5f, .5f, .5f);
            hitCollider.gameObject.GetComponent<Health>().takeDamage(20);
        }
    }
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDist);
    }
}
