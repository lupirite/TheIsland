using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    public static string worldPath = "/world.gamedata";
    public static string playerPath = "/player.gamedata";

    public static void saveWorldData(int seed, bool[][] trees, List<pickUpData>[] pickups)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + worldPath;
        FileStream stream = new FileStream(path, FileMode.Create);
        
        int[] levels = new int[ProgressionManagement.instances.Length];
        float[][] buildProgress = new float[ProgressionManagement.instances.Length][];
        for (int i = 0; i < ProgressionManagement.instances.Length; i++)
        {
            ProgressionManagement ins = ProgressionManagement.instances[i];
            if (!ins)
            {
                continue;
            }
            if (!ins.structures[ins.level])
            {
                continue;
            }
            MaterialGroup[] matGroups = ins.structures[ins.level].GetComponent<structure>().materialGroups;
            if (ProgressionManagement.instances[i])
            {
                levels[i] = ins.level;
                buildProgress[i] = new float[matGroups.Length];
                for (int g = 0; g < matGroups.Length; g++)
                {
                    buildProgress[i][g] = matGroups[g].resources;
                }
            }
        }

        WorldData data = new WorldData(seed, trees, pickups, levels, buildProgress);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static WorldData loadWorld()
    {
        string path = Application.persistentDataPath + worldPath;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            WorldData data = formatter.Deserialize(stream) as WorldData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("SaveFile" + path + "NotFound");
            return null;
        }
    }
    public static void savePlayerData(PlayerData playerData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + playerPath;
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = playerData;

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData loadPlayer()
    {
        string path = Application.persistentDataPath + playerPath;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("SaveFile: " + path + " Not Found");
            return null;
        }
    }

    public static bool worldSaved()
    {
        string path = Application.persistentDataPath + worldPath;
        if (File.Exists(path))
        {
            return true;
        }
        return false;
    }

    public static bool playerSaved()
    {
        string path = Application.persistentDataPath + playerPath;
        if (File.Exists(path))
        {
            return true;
        }
        return false;
    }

    public static void clearData()
    {
        try
        {
            File.Delete(Application.persistentDataPath + worldPath);
            File.Delete(Application.persistentDataPath + playerPath);
            Debug.Log("Sucessfully Cleared All Saved Game Data.");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    public static void savePlayer()
    {
        if (playerControl.current)
        {
            if (worldGen.newWorld == false)
                worldGen.newWorld = !worldSaved();
            PlayerData data = new PlayerData(playerControl.current.gameObject);
            savePlayerData(data);
        }
    }

    public static void saveWorld()
    {
        bool[][] allTrees = new bool[worldGen.instances.Length][];
        List<pickUpData>[] allPickups = new List<pickUpData>[worldGen.instances.Length];
        for (int i = 0; i < worldGen.instances.Length; i++)
        {
            if (worldGen.instances[i] == null)
            {
                allTrees[i] = new bool[0];
                allPickups[i] = new List<pickUpData>();
            }
            else
            {
                allTrees[i] = worldGen.instances[i].trees;

                if (SceneManager.GetSceneByBuildIndex(i).isLoaded)
                {
                    worldGen.instances[i].storePickups();
                }
                allPickups[i] = worldGen.instances[i].pickups;
            }
        }
        saveWorldData(worldGen.seed, allTrees, allPickups);
    }
}
