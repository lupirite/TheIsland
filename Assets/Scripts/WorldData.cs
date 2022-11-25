using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public int seed;
    public bool[][] allTrees;
    public List<pickUpData>[] pickups;
    public int[] buildLevels;
    public float[][] buildProgress;
    public float[] boatPos;
    public float[] boatRot;
    public float[] foundaryInv;
    
    public WorldData(int worldSeed, bool[][] trees, List<pickUpData>[] pickupsList, int[] levels, float[][] curBuildProgress, float[] dairyInv, float[] botePos, float[] boteRot)
    {
        seed = worldSeed;
        allTrees = trees;
        pickups = pickupsList;
        buildLevels = levels;
        buildProgress = curBuildProgress;
        boatPos = botePos;
        boatRot = boteRot;
        foundaryInv = dairyInv;
    }
}
