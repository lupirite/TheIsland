using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bilboard : MonoBehaviour
{
    public GameObject curStruct;
    public GameObject panel;
    public GameObject[] icons;

    public GameObject[] iconElements;

    private Transform playerCam;

    bool createdIcons = false;
    private void Start()
    {
        iconElements = new GameObject[icons.Length];
        playerCam = camFollow.current.transform;
    }
    void Update()
    {
        if (ProgressionManagement.instances[gameObject.scene.buildIndex].curIndicator != gameObject)
        {
            Destroy(gameObject);
        }

        transform.eulerAngles = new Vector3(0, playerCam.eulerAngles.y, 0);
        if (!createdIcons)
        {
            foreach (MaterialGroup group in curStruct.GetComponent<structure>().materialGroups)
            {
                GameObject icon = Instantiate(icons[group.itemId], panel.transform);
                iconElements[group.itemId] = icon;
            }
            createdIcons = true;
        }

        foreach (MaterialGroup group in curStruct.GetComponent<structure>().materialGroups)
        {
            if (group.complete)
            {
                Destroy(iconElements[group.itemId]);
                iconElements[group.itemId] = null;
                if (panel.transform.childCount == 0)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                iconElements[group.itemId].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0}/{1}", Mathf.Floor(group.resources*10)/10, group.resourcesRequired);
            }
        }

    }
}
