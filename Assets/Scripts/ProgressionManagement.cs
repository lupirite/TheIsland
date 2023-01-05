using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressionManagement : MonoBehaviour
{
    public int level;

    public GameObject playerCam;

    public GameObject itemIndicator;

    public GameObject[] structures;

    public static ProgressionManagement[] instances;

    public Vector3 spawnPoint;

    public GameObject curIndicator;

    private GameObject player;

    [HideInInspector]
    public bool structureReady = true;

    public void buildProgress(float[] progress, int buildLevel)
    {
        if (progress == null)
        {
            return;
        }
        level = buildLevel;
        for (int i = 0; i < progress.Length; i++)
        {
            structures[level].GetComponent<structure>().setResources(structures[level].GetComponent<structure>().materialGroups[i], progress[i]);
        }
        for (int i = 0; i < level; i++)
        {
            if (i >= level)
            {
                continue;
            }
            structures[i].GetComponent<structure>().build();
        }
    }

    private void Awake()
    {
        if (instances == null)
        {
            instances = new ProgressionManagement[10];
        }
        instances[gameObject.scene.buildIndex] = this;

        structures = new GameObject[10];
    }
    private void Start()
    {
        player = playerControl.current.gameObject;
    }

    GameObject indicator;
    void Update()
    {
        if (!structureReady && structures[level])
        {
            readyStructure();
            structureReady = true;
        }
        if (structures[level] && structures[level].GetComponent<structure>().complete && structures[level+1]) {
            level++;
            readyStructure();
        }
    }

    void readyStructure()
    {
        indicator = Instantiate(itemIndicator, transform);
        curIndicator = indicator;
        indicator.transform.position = structures[level].transform.Find("indPos").position;
        indicator.GetComponent<Bilboard>().curStruct = structures[level];
        structures[level].GetComponent<structure>().enabled = true;
    }

    public void LoadLevel(bool saved=true)
    {
        foreach (GameObject ob in gameObject.scene.GetRootGameObjects())
        {
            if (ob.layer != 11)
            {
                ob.SetActive(true);
            }
        }

        if (!saved)
        {
            player.GetComponent<playerControl>().setPos(spawnPoint);
        }
    }

    public void UnloadLevel()
    {
        foreach (GameObject ob in gameObject.scene.GetRootGameObjects())
        {
            if (ob.layer != 11)
            {
                ob.SetActive(false);
            }
        }
    }
}
