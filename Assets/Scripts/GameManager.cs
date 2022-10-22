using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Transform loadingScreen;
    public Transform canvas;
    public Image bar;
    public int curScene = 2;
    [HideInInspector]
    public PlayerData data;
    public GameObject player;

    public TextMeshProUGUI textField;

    public int camChoice; // (0) Menu (1) Load (2) Player
    private List<Camera> cameras;

    public GameObject[] pickupTypes;

    private void Awake()
    {
        instance = this;
        playerControl.current = player.GetComponent<playerControl>();

        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);

        if (SaveSystem.playerSaved())
        {
            data = SaveSystem.loadPlayer();
        }

        camChoice = 0;
    }

    private void OnApplicationQuit()
    {
        SaveSystem.savePlayer();
        SaveSystem.saveWorld();
    }

    private void OnApplicationPause(bool pause)
    {
        if (worldGen.instances[2] != null && worldGen.instances[3] != null)
        {
            SaveSystem.savePlayer();
            SaveSystem.saveWorld();
        }
    }

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public void LoadGame()
    {
        loadingScreen.gameObject.SetActive(true);
        camChoice = 1;

        unloadScene(1);

        worldGen.currentIsland.setStartCount();

        if (worldGen.currentIsland.progress == 1)
        {
            return;
        }

        StartCoroutine(GetSceneLoadProgress());
        StartCoroutine(GetTotalProgress());
    }

    public void unloadScene(int sceneIndex)
    {
        scenesLoading.Add(SceneManager.UnloadSceneAsync(sceneIndex));
    }

    float totalSceneProgress;
    float totalLoadProgress;
    public IEnumerator GetSceneLoadProgress()
    {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;

                foreach (AsyncOperation operation in scenesLoading)
                {
                    totalSceneProgress += operation.progress;
                }

                totalSceneProgress /= scenesLoading.Count;

                bar.fillAmount = totalSceneProgress;

                textField.text = string.Format("Loading Scene: {0}%", Mathf.Round(totalSceneProgress * 100));

                yield return null;
            }
        }
    }

    public IEnumerator GetTotalProgress()
    {
        float totalProgress = 0;

        while (worldGen.currentIsland == null || !worldGen.currentIsland.isDone)
        {
            if (worldGen.currentIsland == null)
            {
                totalLoadProgress = 0;
            }
            else
            {
                totalLoadProgress = worldGen.currentIsland.progress;

                textField.text = string.Format("Placing Tiles: {0}%", Mathf.Round(totalLoadProgress * 100));
            }

            totalProgress = totalLoadProgress;
            bar.fillAmount = totalProgress;

            yield return null;
        }

        //loadingScreen.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
        player.gameObject.SetActive(true);
        InvokeRepeating("savePlayer", 20, 20);

        camChoice = 2;

        ProgressionManagement.instances[2].LoadLevel(SaveSystem.playerSaved());

        loadingScreen.GetComponent<Animator>().SetBool("Loaded", true);
    }
}
