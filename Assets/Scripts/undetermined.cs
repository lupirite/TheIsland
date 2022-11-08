using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class undetermined : MonoBehaviour
{
    public List<int> possibleStates = new List<int>();
    public int type = -1;
    public Vector2 pos;
    public int ID;
    public GameObject gO;

    public void init()
    {
        for (int i = 0; i < transform.parent.GetComponent<worldGen>().hexTypes.Length; i++)
        {
            possibleStates.Add(i);
        }
    }

    public Transform getNeighbor(int i)
    {
        int o = -(((int)pos[1] + (int)transform.parent.GetComponent<worldGen>().size*2 - 1) % 2);
        int[] arr1 = { o, o + 1, 1, o + 1, o, -1 };
        int[] arr2 = {-1, -1, 0, 1, 1, 0};
        Vector2 nPos = pos + new Vector2(arr1[i], arr2[i]);
        return transform.parent.GetComponent<worldGen>().getHex(nPos);
    }

    public void collapse(int t = -1, bool main = false, bool mine = false, bool spawn = false, bool foundary = false, bool mill = false)
    {
        float[] weights = new float[possibleStates.Count];
        for (int i = 0; i < possibleStates.Count; i++)
        {
            weights[i] = GetComponentInParent<worldGen>().hexTypes[possibleStates[i]].weight;
        }

        if (t == -1)
            setType(possibleStates[Utility.weightedIndex(weights)]);
        else
            setType(t, main, mine, spawn, foundary, mill);
        possibleStates = new List<int>();
        possibleStates.Add(type);
        for (int i = 0; i < 6; i++)
        {
            Transform nb = getNeighbor(i);
            if (nb != null)
            {
                nb.GetComponent<undetermined>().propagate(this);
            }
        }
    }

    void setType(int tileType, bool main = false, bool mine = false, bool spawn = false, bool foundary = false, bool mill = false)
    {
        type = tileType;
        GameObject ob = transform.parent.GetComponent<worldGen>().hexTypes[type].prefab;

        if (GetComponentInParent<worldGen>().hexTypes[type].prefab != null) {
            gO = Instantiate(ob, transform.position + new Vector3(0, GetComponentInParent<worldGen>().hexTypes[type].elevation, 0), ob.transform.rotation, transform.parent);
            if (gO.GetComponent<spawnTile>())
            {
                gO.GetComponent<spawnTile>().ID = ID;
            }
            if (main) {
                Destroy(gO.GetComponent<spawnTile>());

                GameObject h = transform.parent.GetComponent<worldGen>().hause;
                GameObject hause = Instantiate(h, transform.position + new Vector3(0, GetComponentInParent<worldGen>().hexTypes[type].elevation + 1f, 0), h.transform.rotation, ProgressionManagement.instances[gameObject.scene.buildIndex].transform);
                ProgressionManagement.instances[gameObject.scene.buildIndex].structures[2] = hause;

                for (int i = 0; i < 6; i++)
                {
                    getNeighbor(i).GetComponent<undetermined>().collapse(4);
                    Destroy(getNeighbor(i).GetComponent<undetermined>().gO.GetComponent<spawnTile>());
                    transform.parent.GetComponent<worldGen>().hxs.Remove(getNeighbor(i));
                }

                Transform nb = getNeighbor(0).GetComponent<undetermined>().getNeighbor(1).GetComponent<undetermined>().getNeighbor(0);
                nb.GetComponent<undetermined>().collapse(4);
                Destroy(nb.GetComponent<undetermined>().gO.GetComponent<spawnTile>());
                transform.parent.GetComponent<worldGen>().hxs.Remove(nb);

                GameObject fp = transform.parent.GetComponent<worldGen>().firePlace;
                GameObject obj = Instantiate(fp, nb.position + new Vector3(0, GetComponentInParent<worldGen>().hexTypes[type].elevation + .7f, 0), fp.transform.rotation, ProgressionManagement.instances[gameObject.scene.buildIndex].transform);
                ProgressionManagement.instances[gameObject.scene.buildIndex].structures[0] = obj;
            }
            if (foundary)
            {
                Destroy(gO.GetComponent<spawnTile>());

                GameObject h = transform.parent.GetComponent<worldGen>().foundary;
                GameObject foun = Instantiate(h, transform.position + new Vector3(0, GetComponentInParent<worldGen>().hexTypes[type].elevation, 0), h.transform.rotation, ProgressionManagement.instances[gameObject.scene.buildIndex].transform);
                ProgressionManagement.instances[gameObject.scene.buildIndex].structures[3] = foun;

                for (int i = 0; i < 6; i++)
                {
                    getNeighbor(i).GetComponent<undetermined>().collapse(4);
                    Destroy(getNeighbor(i).GetComponent<undetermined>().gO.GetComponent<spawnTile>());
                    transform.parent.GetComponent<worldGen>().hxs.Remove(getNeighbor(i));
                }
            }
            if (mill)
            {
                Destroy(gO.GetComponent<spawnTile>());

                GameObject h = transform.parent.GetComponent<worldGen>().woodMill;
                GameObject mil = Instantiate(h, transform.position + new Vector3(0, GetComponentInParent<worldGen>().hexTypes[type].elevation + .25f, 0), h.transform.rotation, ProgressionManagement.instances[gameObject.scene.buildIndex].transform);
                ProgressionManagement.instances[gameObject.scene.buildIndex].structures[4] = mil;

                for (int i = 0; i < 6; i++)
                {
                    getNeighbor(i).GetComponent<undetermined>().collapse(4);
                    Destroy(getNeighbor(i).GetComponent<undetermined>().gO.GetComponent<spawnTile>());
                    transform.parent.GetComponent<worldGen>().hxs.Remove(getNeighbor(i));
                }
            }
            if (mine) {
                Destroy(gO.GetComponent<MeshCollider>());
                Destroy(gO.GetComponent<MeshRenderer>());
                Destroy(gO.transform.GetChild(0).gameObject);
                getNeighbor(0).GetComponent<undetermined>().collapse(10);
                transform.parent.GetComponent<worldGen>().hxs.Remove(getNeighbor(0));
                getNeighbor(1).GetComponent<undetermined>().collapse(10);
                transform.parent.GetComponent<worldGen>().hxs.Remove(getNeighbor(1));
                GameObject mn = transform.parent.GetComponent<worldGen>().mine;
                Random.seed = worldGen.seed+4;
                GameObject obj = Instantiate(mn, transform.position + new Vector3(0, GetComponentInParent<worldGen>().hexTypes[type].elevation, 0), Quaternion.Euler(0, new[] {-60, -120}[Random.Range(0, 1)], 0), ProgressionManagement.instances[gameObject.scene.buildIndex].transform);
                ProgressionManagement.instances[gameObject.scene.buildIndex].structures[1] = obj;
            }
            if (spawn) {
                Destroy(gO.GetComponent<spawnTile>());
                getNeighbor(0).GetComponent<undetermined>().collapse(4);
                transform.parent.GetComponent<worldGen>().hxs.Remove(getNeighbor(0));
                getNeighbor(1).GetComponent<undetermined>().collapse(4);
                transform.parent.GetComponent<worldGen>().hxs.Remove(getNeighbor(1));
                GameObject sp = transform.parent.GetComponent<worldGen>().spawnPoint;
                Random.seed = worldGen.seed + 5;
                Instantiate(sp, transform.position + new Vector3(0, GetComponentInParent<worldGen>().hexTypes[type].elevation, 0), Quaternion.Euler(-90, new[] { -60, -120 }[Random.Range(0, 1)], 0), ProgressionManagement.instances[gameObject.scene.buildIndex].transform);
            }
        }
    }

    public void propagate(undetermined ins)
    {
        if (possibleStates.Count == 1)
            return;
        bool updated = false;
        List<int> statesToRemove = new List<int>();
        for (int i = 0; i < possibleStates.Count; i++)
        {
            bool possible = false;
            for (int p = 0; p < ins.possibleStates.Count; p++) {
                if (!transform.parent.GetComponent<worldGen>().hexTypes[ins.possibleStates[p]].avoid.Contains(possibleStates[i]))
                {
                    possible = true;
                    break;
                }
            }
            if (!possible)
            {
                statesToRemove.Add(possibleStates[i]);
                updated = true;
            }
        }
        foreach (int state in statesToRemove)
        {
            possibleStates.Remove(state);
        }
        if (possibleStates.Count == 1)
            setType(possibleStates[0]);

        if (updated)
        {
            for (int i = 0; i < 6; i++)
            {
                Transform nb = getNeighbor(i);
                if (nb != null)
                    nb.GetComponent<undetermined>().propagate(this);
            }
        }
    }
}
