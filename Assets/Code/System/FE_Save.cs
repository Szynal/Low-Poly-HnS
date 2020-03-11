using System.Collections;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine.SceneManagement;
using UnityEngine;

[System.Serializable]
public class FE_Save
{
    public int MainSceneID = -1;
    public string MainSceneName;
    public List<SceneSave> StatesOfScenes = new List<SceneSave>();
    public (int, string)[] OpenedScenes = null;

    public string Date = "";

    public void SaveScenes()
    {
        //We need to save our current scene's ID, so we know which scene to open when loading the game
        Scene _mainScene = SceneManager.GetActiveScene();

        OpenedScenes = new (int, string)[SceneManager.sceneCount];
        for(int i = 0; i < OpenedScenes.Length; i++)
        {
            OpenedScenes[i] = (SceneManager.GetSceneAt(i).buildIndex, SceneManager.GetSceneAt(i).name);
        }
        
        MainSceneID = _mainScene.buildIndex;
        MainSceneName = _mainScene.name;

        StatesOfScenes = SceneLoader.Instance.GetSceneStates();

        Date = System.DateTime.Now.Hour.ToString("D2") + ":" + System.DateTime.Now.Minute.ToString("D2");
    }

    public SceneSave GetStateBySceneID(int _sceneID)
    {
        SceneSave _ret = null;

        foreach(SceneSave _save in StatesOfScenes)
        {
            if(_save.SceneID == _sceneID)
            {
                _ret = _save;
            }
        }

        return _ret;
    }
}

[System.Serializable]
public class FE_SaveFile
{
    public List<FE_Save> Saves = new List<FE_Save>(10);
    public GameManager.PlayerSettings Settings = default;

    public FE_SaveFile()
    {
        for(int i = 0; i < 10; i++)
        {
            Saves.Add(new FE_Save());
        }
    }
}
