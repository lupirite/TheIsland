using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnTile : MonoBehaviour
{
    public spawnOb[] spawnObs;
    public LayerMask tileMask;
    public float sizeRange = .3f;
    public float placementRange = .5f;
    public bool randomRotation = true;
    [Range(0, 1)]
    public float chance = .5f;
    public GameObject dBugOb;
    public int ID;
    void Start()
    {
        Random.seed = worldGen.seed  + ID;
        if (Random.Range(0f, 1f) < chance)
        {
            Random.seed = worldGen.seed + 6 + ID;
            float posX = Random.Range(-placementRange, placementRange);
            Random.seed = worldGen.seed + 7 + ID;
            float posY = Random.Range(-placementRange, placementRange);
            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(posX, 5, posY), Vector3.down, out hit, 10, tileMask);

            float[] weights = new float[spawnObs.Length];
            for (int i = 0; i < spawnObs.Length; i++)
            {
                weights[i] = spawnObs[i].weight;
            }
            GameObject prefab = spawnObs[Utility.weightedIndex(weights)].prefab;

            Quaternion rot = Quaternion.identity;
            Random.seed = worldGen.seed + ID;
            if (randomRotation)
                rot = Quaternion.Euler(0, Random.Range(0, 365), 0);
            GameObject ob = Instantiate(prefab, hit.point+prefab.transform.position,  rot * prefab.transform.rotation, transform.parent);
            ob.transform.localScale *= Random.Range(1f - sizeRange, 1f + sizeRange);
            if (ob.GetComponent<Health>())
            {
                ob.GetComponent<Health>().ID = ID;
            }
        }
    }

    public int weightedIndex(float[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;

        float w;
        float t = 0;
        int i;
        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];

            if (float.IsPositiveInfinity(w))
            {
                return i;
            }
            else if (w >= 0f && !float.IsNaN(w))
            {
                t += weights[i];
            }
        }


        Random.seed = worldGen.seed + ID;
        float r = Random.value;
        float s = 0f;

        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];
            if (float.IsNaN(w) || w <= 0f) continue;

            s += w / t;
            if (s >= r) return i;
        }

        return -1;
    }
}

[System.Serializable]
public class spawnOb
{
    public GameObject prefab;
    [Range(0, 1)]
    public float weight = .5f;
}
