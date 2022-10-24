using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class structure : MonoBehaviour
{
    public LayerMask collectableMask;
    public float pickUpDist;
    public MaterialGroup[] materialGroups;
    public Transform[] finalObjects;
    public bool complete;
    void Update() {
        if (!complete) {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickUpDist, collectableMask);
            foreach (var hitCollider in hitColliders) {
                foreach (MaterialGroup matGroup in materialGroups) {
                    if (matGroup.complete) {
                        continue;
                    }
                    if (GameManager.instance.pickupTypes[matGroup.itemId].name+"(Clone)" != hitCollider.gameObject.name)
                    {
                        continue;
                    }
                    float logSize = hitCollider.transform.localScale.y*2;
                    Destroy(hitCollider.gameObject);
                    setResources(matGroup, matGroup.resources + logSize);
                    GameObject p = Instantiate(matGroup.particle, transform.Find("indPos").position, matGroup.particle.transform.rotation, null);
                    p.transform.localScale = new Vector3(.5f, .5f, .5f);
                    break;
                }
            }
        }
    }
    public void build()
    {
        foreach (MaterialGroup matGroup in materialGroups)
        {
            for (int i = 0; i < matGroup.group.childCount; i++)
            {
                matGroup.group.GetChild(i).gameObject.SetActive(true);
            }
            matGroup.complete = true;
        }
        print(finalObjects);
        if (finalObjects != null)
        {
            for (int i = 0; i < finalObjects.Length; i++)
            {
                finalObjects[i].gameObject.SetActive(!finalObjects[i].gameObject.active);
            }
        }
        complete = true;
    }
    public void setResources(MaterialGroup matGroup, float amount)
    {
        matGroup.resources = amount;
        for (int i = 0; i < (float)matGroup.group.childCount * Mathf.Min(1, (matGroup.resources / matGroup.resourcesRequired)); i++)
        {
            matGroup.group.GetChild(i).gameObject.SetActive(true);
        }

        if (matGroup.resources >= matGroup.resourcesRequired)
        {
            matGroup.complete = true;
            for (int i = 0; i < materialGroups.Length; i++)
            {
                MaterialGroup group = materialGroups[i];
                print("floof");
                if (!group.complete)
                {
                    break;
                }
                print("place");
                if (i == materialGroups.Length-1)
                {
                    print("boop");
                    complete = true;
                    if (finalObjects != null)
                    {
                        foreach (Transform ob in finalObjects)
                        {
                            ob.gameObject.SetActive(!ob.gameObject.active);
                        }
                    }
                }
            }
        }
    }
    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, pickUpDist);
    }
}

[System.Serializable]
public class MaterialGroup
{
    public float resources;
    public float resourcesRequired;
    public Transform group;
    public bool complete;
    public int itemId;
    public GameObject particle;
}
