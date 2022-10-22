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
    
    public WorldData(int worldSeed, bool[][] trees, List<pickUpData>[] pickupsList, int[] levels, float[][] curBuildProgress)
    {
        seed = worldSeed;
        allTrees = trees;
        pickups = pickupsList;
        buildLevels = levels;
        buildProgress = curBuildProgress;
    }
}
