using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] pos = new float[3];
    public int curScene;
    public pickUpData[] items;

    public PlayerData(GameObject player)
    {
        Vector3 loc = player.transform.position;
        for (int i = 0; i < 3; i++)
            pos[i] = loc[i];

        curScene = player.GetComponentInChildren<CarryPoint>().curScene;

        Transform cPoint = player.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(2);
        items = new pickUpData[cPoint.childCount];
        for (int i = 0; i < cPoint.childCount; i++)
        {
            int pickupType = 0;
            for (int n = 0; n < GameManager.instance.pickupTypes.Length; n++)
            {
                if (GameManager.instance.pickupTypes[n].name + "(Clone)" == cPoint.GetChild(i).name)
                {
                    pickupType = n;
                }
            }
            pickUpData data = new pickUpData(cPoint.GetChild(i).gameObject, pickupType, cPoint.GetChild(i).localScale);
            items[i] = data;
        }
    }
}
