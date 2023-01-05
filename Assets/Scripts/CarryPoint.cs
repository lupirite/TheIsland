using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryPoint : MonoBehaviour
{
    public LayerMask collectableMask;
    public Animator anim;
    public float pickUpDist = .8f;
    public float maxStackHeight;
    public float itemShrink = .75f;
    public float doubleClickThreshold;
    public AttackPoint attackPoint;
    //[HideInInspector]
    public int curScene = 2;

    float stackHeight = 0;
    float timeSinceClick;

    Transform player;
    void Start()
    {
        player = transform.parent.parent.parent.parent.parent;
    }
    void Update()
    {
        timeSinceClick += Time.deltaTime;
        if (Input.GetButtonDown("Fire1"))
        {
            if (timeSinceClick < doubleClickThreshold && transform.childCount > 0)
            {
                dropLog();
            }
            timeSinceClick = 0;
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickUpDist, collectableMask);
        foreach (var hitCollider in hitColliders)
        {
            float height = getHeight(hitCollider.gameObject);
            if (stackHeight + height * 2 * itemShrink <= maxStackHeight)
            {
                StartCoroutine(addLog(hitCollider.gameObject));
            }
        }

        anim.SetBool("Carry", transform.childCount > 0);
        if (transform.childCount > 0)
        {
            anim.SetBool("Attack", false);
            attackPoint.setHands(false);
        }
        else
            attackPoint.setHands(true);
    }

    float getHeight(GameObject log)
    {
        float height = log.transform.localScale.y;
        if (log.name != GameManager.instance.pickupTypes[0].name + "(Clone)" && log.name != GameManager.instance.pickupTypes[2].name + "(Clone)")
        {
            height = log.transform.localScale.z;
        }
        if (log.name == GameManager.instance.pickupTypes[4].name + "(Clone)")
        {
            height = log.transform.localScale.z*.75f;
        }
        if (log.name == GameManager.instance.pickupTypes[6].name + "(Clone)")
        {
            height = log.transform.localScale.z * .6f;
        }
        if (log.name == GameManager.instance.pickupTypes[5].name + "(Clone)")
        {
            height = log.transform.localScale.y * .75f;
        }
        return height;
    }

    public IEnumerator addLog(GameObject log, bool onStart = false)
    {
        Vector3 startPos = Vector3.zero;
        log.gameObject.layer = 0;
        if (!onStart)
        {
            anim.SetBool("Picking", true);
            startPos = player.position;
        }
        yield return new WaitForSeconds(1);
        if (player.position == startPos || onStart && log.layer != 1)
        {
            if (!onStart)
            {
                log.transform.localScale *= itemShrink;
            }
            log.transform.parent = transform;
            log.transform.localEulerAngles = new Vector3(0, 90, 0);
            float height = getHeight(log);
            if (log.name != GameManager.instance.pickupTypes[0].name+ "(Clone)" && log.name != GameManager.instance.pickupTypes[2].name + "(Clone)" && log.name != GameManager.instance.pickupTypes[5].name + "(Clone)" && log.name != GameManager.instance.pickupTypes[6].name + "(Clone)")
            {
                log.transform.localEulerAngles = new Vector3(90, 90, 0);
            }
            Destroy(log.GetComponent<Rigidbody>());
            stackHeight += height;
            log.transform.localPosition = new Vector3(0, stackHeight, 0);
            stackHeight += height;
        }
        else
            log.gameObject.layer = 8;
        anim.SetBool("Picking", false);
    }

    void dropLog()
    {
        GameObject child = transform.GetChild(transform.childCount - 1).gameObject;
        child.gameObject.layer = 8;
        child.transform.parent = worldGen.instances[curScene].pickupParent;
        child.AddComponent<Rigidbody>();
        float height = getHeight(child);
        stackHeight -= height * 2;
        child.transform.localScale /= itemShrink;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickUpDist);
    }
}
