using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smelter : MonoBehaviour
{
    public LayerMask collectableMask;
    public float pickUpDist;
    public matInfo[] materialInfo;
    public metInfo[] metalInfo;
    public float[] inventory;
    public float fillSpeed = 5;
    public float maxSize= 2.5f;
    public float lerpSpeed = .5f;

    float startHeight;
    float height;
    bool heightMatched = true;
    private void Start()
    {
        startHeight = transform.position.y;
        float[] inv = worldGen.instances[gameObject.scene.buildIndex].inv;
        if (inv != null)
            inventory = inv;
        else
            inventory = new float[10];
        EvaluateHeight();
    }
    void Update()
    {
        if (!heightMatched)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, height / 2, transform.localScale.z), lerpSpeed*Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, startHeight + transform.localScale.y / 1.3f, transform.position.z), lerpSpeed*Time.deltaTime);
            if (transform.localScale.y > height / 2 - .02f && transform.localScale.y < height / 2 + .02f)
            {
                heightMatched = true;
            }
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickUpDist, collectableMask);
        foreach (var hitCollider in hitColliders)
        {
            int itemId = 0;
            for (int i = 0; i < GameManager.instance.pickupTypes.Length; i++)
            {
                if (GameManager.instance.pickupTypes[i].name + "(Clone)" == hitCollider.gameObject.name)
                {
                    itemId = i;
                    break;
                }
            }
            if (materialInfo[itemId].flamable)
            {
                
            }
            else
            {
                inventory[materialInfo[itemId].materialType] += materialInfo[itemId].materialValue;
                EvaluateHeight();
            }
            StartCoroutine(killOb(hitCollider.gameObject));
        }
    }

    IEnumerator killOb(GameObject ob)
    {
        ob.layer = 0;
        ob.transform.parent = null;
        yield return new WaitForSeconds(6f);
        Destroy(ob);

        yield return null;
    }

    public void EvaluateHeight()
    {
        float total = 0;
        foreach (float val in inventory)
        {
            total += val;
        }
        float v = total / fillSpeed;
        height = (v / (v + 1)) * maxSize;
        heightMatched = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, pickUpDist);
        Gizmos.DrawWireCube(transform.position, new Vector3(transform.localScale.x, maxSize, transform.localScale.z));
    }
}

[System.Serializable]

public class matInfo
{
    public bool flamable = true;
    public int materialType;
    public float materialValue;
}

[System.Serializable]
public class metInfo
{
    public int itemId;
}
