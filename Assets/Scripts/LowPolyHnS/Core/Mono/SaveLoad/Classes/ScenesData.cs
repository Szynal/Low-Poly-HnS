using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace LowPolyHnS.Core
{
    [Serializable]
    public class ScenesData
    {
        public string mainScene;
        public List<string> additiveScenes;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        public ScenesData(string mainScene)
        {
            this.mainScene = mainScene;
            additiveScenes = new List<string>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Add(string name, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single)
            {
                mainScene = name;
                additiveScenes = new List<string>();
            }
            else if (mode == LoadSceneMode.Additive)
            {
                additiveScenes.Add(name);
            }
        }

        public void Remove(string name)
        {
            additiveScenes.Remove(name);
        }

        // IGAMESAVE INTERFACE: -------------------------------------------------------------------

        public object GetSaveData()
        {
            return this;
        }

        public Type GetSaveDataType()
        {
            return typeof(ScenesData);
        }

        public string GetUniqueName()
        {
            return "scenes-data";
        }

        public IEnumerator OnLoad(object generic)
        {
            ScenesData data = (ScenesData) generic;

            SceneManager.LoadScene(data.mainScene, LoadSceneMode.Single);
            yield return null;

            for (int i = 0; i < data.additiveScenes.Count; ++i)
            {
                SceneManager.LoadScene(data.additiveScenes[i], LoadSceneMode.Additive);
                yield return null;
            }

            mainScene = data.mainScene;
            additiveScenes = data.additiveScenes;
        }

        public void ResetData()
        {
            mainScene = SceneManager.GetActiveScene().name;
            additiveScenes = new List<string>();
        }
    }
}