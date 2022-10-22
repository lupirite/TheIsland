using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class pickUpData
{
    public float[] pos = new float[3];
    public float[] eulerAngles = new float[3];
    public int type = 0; // (0) log (1) stone
    public float[] scale = new float[3];

    public pickUpData(GameObject pickUp, int pickupType, Vector3 scaleVec)
    {
        Vector3 loc = pickUp.transform.position;
        for (int i = 0; i < 3; i++)
            pos[i] = loc[i];

        Vector3 rot = pickUp.transform.eulerAngles;
        for (int i = 0; i < 3; i++)
            eulerAngles[i] = rot[i];

        type = pickupType;
        for (int i = 0; i < 3; i++)
            scale[i] = scaleVec[i];
    }
}
