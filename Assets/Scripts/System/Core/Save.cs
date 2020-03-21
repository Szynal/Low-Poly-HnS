using System;
using System.Collections.Generic;
using System.Linq;
using LowPolyHnS;
using UnityEngine.SceneManagement;

[Serializable]
public class Save
{
    public int MainSceneID = -1;
    public string MainSceneName;
    public List<SceneSave> StatesOfScenes = new List<SceneSave>();
    public (int, string)[] OpenedScenes;

    public string Date = "";

    public void SaveScenes()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        OpenedScenes = new (int, string)[SceneManager.sceneCount];
        for (int i = 0; i < OpenedScenes.Length; i++)
        {
            OpenedScenes[i] = (SceneManager.GetSceneAt(i).buildIndex, SceneManager.GetSceneAt(i).name);
        }

        MainSceneID = activeScene.buildIndex;
        MainSceneName = activeScene.name;

        StatesOfScenes = SceneLoader.Instance.GetSceneStates();

        Date = DateTime.Now.Hour.ToString("D2") + ":" + DateTime.Now.Minute.ToString("D2");
    }

    public SceneSave GetStateBySceneID(int sceneID)
    {
        SceneSave sceneSave = null;

        foreach (SceneSave save in StatesOfScenes.Where(save => save.SceneID == sceneID))
        {
            sceneSave = save;
        }

        return sceneSave;
    }
}

[Serializable]
public class SaveFile
{
    public List<Save> Saves = new List<Save>(10);
    public GameManager.PlayerSettings Settings = default;

    public SaveFile()
    {
        for (int i = 0; i < 10; i++)
        {
            Saves.Add(new Save());
        }
    }
}