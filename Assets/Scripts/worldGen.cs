using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class worldGen : MonoBehaviour
{
    [HideInInspector]
    public static bool newWorld;
    public static worldGen currentIsland;
    public static worldGen[] instances = new worldGen[10];
    public hexType[] hexTypes;
    public GameObject firePlace;
    public GameObject hause;
    public GameObject mine;
    public GameObject spawnPoint;
    public GameObject foundary;
    public GameObject woodMill;
    public GameObject bote;
    public GameObject undetermined;
    public int size;
    public int regionSplits;
    public bool cave;
    public Transform pickupParent;

    public static int seed = 3;

    [HideInInspector]
    public float progress;
    [HideInInspector]
    public bool isDone;
    [HideInInspector]
    public bool[] trees;
    [HideInInspector]
    public List<pickUpData> pickups = new List<pickUpData>();

    public void storePickups()
    {
        pickups = new List<pickUpData>();
        Transform pickupHolder = pickupParent.transform;
        for (int i = 0; i < pickupHolder.childCount; i++)
        {
            int type = -1;
            
            Transform pickup = pickupHolder.GetChild(i);
            for (int l = 0; l < GameManager.instance.pickupTypes.Length; l++)
            {
                GameObject prefab = GameManager.instance.pickupTypes[l];
                if (prefab.name + "(Clone)" == pickup.gameObject.name)
                {
                    type = l;
                }
            }
            pickUpData data = new pickUpData(pickup.gameObject, type, pickup.localScale);
            pickups.Add(data);
        }
    }

    public void killTree(int ID)
    {
        trees[ID] = true;
        SaveSystem.saveWorld();
    }

    private void Awake()
    {
        if (newWorld == false)
            newWorld = !SaveSystem.worldSaved();
        if (newWorld)
            seed = Random.Range(0, 10000000);
        if (instances[gameObject.scene.buildIndex] == null)
            instances[gameObject.scene.buildIndex] = this;
        
        if (!cave)
            currentIsland = this;
    }

    private List<Transform> hexList = new List<Transform>();
    private List<pickUpData>[] pkps;
    void Start()
    {
        trees = new bool[size * size * 4];
        generateIsland();
        if (newWorld)
        {
            SaveSystem.saveWorld();
        }
        else
        {
            WorldData data = SaveSystem.loadWorld();
            seed = data.seed;
            trees = data.allTrees[gameObject.scene.buildIndex];
            pkps = data.pickups;
            StartCoroutine(loadProgress(data));
        }
    }

    IEnumerator loadProgress(WorldData data)
    {
        yield return new WaitForSeconds(1);
        if (!ProgressionManagement.instances[gameObject.scene.buildIndex] || data.buildProgress[gameObject.scene.buildIndex] == null)
        {
            yield return null;
        }
        ProgressionManagement.instances[gameObject.scene.buildIndex].buildProgress(data.buildProgress[gameObject.scene.buildIndex], data.buildLevels[gameObject.scene.buildIndex]);
    }

    void clearChildren()
    {
        hexList = new List<Transform>();
        hxs = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < GameObject.Find("ProgressionManager").transform.childCount; i++)
        {
            Destroy(GameObject.Find("ProgressionManager").transform.GetChild(i).gameObject);
        }
    }

    [HideInInspector]
    public List<Transform> hxs;
    int startCount;
    void generateIsland()
    {
        for (int y = -(int)size + 1; y < size; y++)
        {
            for (int x = -(int)size + 1; x < size; x++)
            {
                float offset = 0;
                if (y % 2 != 0)
                {
                    offset = Mathf.Sqrt(3) / 2;
                }
                Transform hex = newHex((transform.position + new Vector3(x * Mathf.Sqrt(3) + offset, 0, y * 1.5f)) * 2);
                hex.GetComponent<undetermined>().pos = new Vector2(x, y);
                hex.GetComponent<undetermined>().ID = (y+size) * size + (x+size);
            }
        }

        hxs = new List<Transform>(hexList);

        foreach (Transform hex in hexList)
        {
            if (hex.GetComponent<undetermined>().pos.magnitude >= size - 1)
            {
                hex.GetComponent<undetermined>().collapse(3);
                hxs.Remove(hex);
            }
        }

        List<int> choices = new List<int>();
        if (!cave) {
            for (int i = 0; i < regionSplits * regionSplits; i++)
            {
                if (i < regionSplits * regionSplits * .5f)
                    choices.Add(4);
                else if (i < regionSplits * regionSplits * .75f)
                {
                    choices.Add(3);
                }
                else
                    choices.Add(5);
            }
        }
        else {
            for (int i = 0; i < regionSplits * regionSplits; i++)
            {
                if (i < regionSplits * regionSplits * .5f)
                    choices.Add(4);
                else
                {
                    choices.Add(3);
                }
            }
        }
        Transform c;
        bool createdMain = false;
        bool createdMine = false;
        bool createdSpawn = false;
        bool createdFoundary = false;
        bool createdMill = false;
        for (int y = 0; y < regionSplits; y++)
        {
            for (int x = 0; x < regionSplits; x++)
            {
                Random.seed = (int)Mathf.Pow(seed*x, y);
                int offsetX = Random.Range(-2, 2);
                Random.seed = (int)Mathf.Pow(seed * x, y)+1;
                int offsetY = Random.Range(-2, 2);

                c = getHex(new Vector2((x + 1) * (int)((size * 2) / (regionSplits + 1)) - size + offsetX, (y + 1) * (int)((size * 2) / (regionSplits + 1)) - size + offsetY));

                Random.seed = seed*(x+y*10) + 2;
                int choice = choices[Random.Range(0, choices.Count)];

                if (!cave) {
                    c.GetComponent<undetermined>().collapse(choice, choice == 4 && !createdMain, choice == 5 && !createdMine, false, createdMain && !createdFoundary && choice == 4, createdFoundary && !createdMill && choice == 4);
                
                    if (choice == 4 && !createdMain) {
                        createdMain = true;
                    }
                    else if (choice == 5 && !createdMine) {
                        createdMine = true;
                    }
                    else if (choice == 4 && !createdFoundary)
                    {
                        createdFoundary = true;
                    }
                    else if (choice == 4 && !createdMill)
                    {
                        createdMill = true;
                    }
                }
                else {
                    c.GetComponent<undetermined>().collapse(choice, false, false, choice == 4 && !createdSpawn);
                    if (choice == 4 && !createdSpawn) {
                        createdSpawn = true;
                    }
                }
                choices.Remove(choice);
                hxs.Remove(c);
            }
        }

        if (bote)
        {
            c = getHex(new Vector2(size - 1, 0));
            c.GetComponent<undetermined>().collapse(6);
            GameObject b = Instantiate(bote, c.position + new Vector3(3, 1.35f, 0), Quaternion.Euler(0, 70, 0), ProgressionManagement.instances[gameObject.scene.buildIndex].transform);
            ProgressionManagement.instances[gameObject.scene.buildIndex].structures[5] = b;
        }

        setStartCount();
        StartCoroutine(AddTile());
    }

    public void setStartCount()
    {
        startCount = hxs.Count;
    }

    public IEnumerator AddTile()
    {
        while (hxs.Count > 0)
        {
            try
            {
                for (int n = 0; n < 10; n++) {
                    if (hxs.Count == 0)
                        break;
                    List<Transform> pt = new List<Transform>();
                    float lowestEntropy = Mathf.Infinity;
                    for (int i = 0; i < hxs.Count; i++)
                    {
                        if (hxs[i].GetComponent<undetermined>().possibleStates.Count < lowestEntropy)
                        {
                            lowestEntropy = hxs[i].GetComponent<undetermined>().possibleStates.Count;
                            pt = new List<Transform>();
                            pt.Add(hxs[i]);
                        }
                        else if (hxs[i].GetComponent<undetermined>().possibleStates.Count == lowestEntropy)
                        {
                            pt.Add(hxs[i]);
                        }
                    }

                    progress = 1f - ((float)hxs.Count / (float)startCount);

                    Random.seed = seed + 3 + hxs.Count;
                    Transform c = pt[Random.Range(0, pt.Count)];

                    c.GetComponent<undetermined>().collapse();
                    hxs.Remove(c);
                }
            }
            catch
            {
                print("Generation Failed, Retrying");
                clearChildren();
                generateIsland();
            }

            yield return null;
        }

        if (hxs.Count == 0)
        {
            // print(string.Format("Scene #{0} loaded successfully", gameObject.scene.buildIndex));
        }

        if (!newWorld)
        {
            for (int i = 0; i < pkps[gameObject.scene.buildIndex].Count; i++)
            {
                pickUpData pickup = pkps[gameObject.scene.buildIndex][i];
                GameObject gO = Instantiate(GameManager.instance.pickupTypes[pickup.type], new Vector3(pickup.pos[0], pickup.pos[1], pickup.pos[2]), Quaternion.Euler(pickup.eulerAngles[0], pickup.eulerAngles[1], pickup.eulerAngles[2]), pickupParent);
                gO.transform.localScale = new Vector3(pickup.scale[0], pickup.scale[1], pickup.scale[2]);
            }
        }

        if (gameObject.scene.buildIndex != GameManager.instance.data.curScene)
        {
            ProgressionManagement.instances[gameObject.scene.buildIndex].UnloadLevel();
        }
        if (!isDone)
            ProgressionManagement.instances[gameObject.scene.buildIndex].structureReady = false;
        isDone = true;
    }
    
    Transform newHex(Vector3 pos)
    {
        GameObject h = Instantiate(undetermined, pos, undetermined.transform.rotation, transform);
        h.GetComponent<undetermined>().init();
        hexList.Add(h.transform);
        return h.transform;
    }

    public Transform getHex(Vector2 pos)
    {
        foreach (Transform hex in hexList)
        {
            if (hex.GetComponent<undetermined>().pos == pos)
                return hex;
        }
        return null;
    }
}

[System.Serializable]
public class hexType
{
    public GameObject prefab;
    public List<int> avoid = new List<int>();
    public float elevation;
    public float weight = 1;
}
