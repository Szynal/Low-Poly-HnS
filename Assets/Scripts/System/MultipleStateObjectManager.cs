using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultipleStateObjectManager : MonoBehaviour
{
    public static MultipleStateObjectManager Instance;

    private Dictionary<int, int> msoStates = new Dictionary<int, int>();
    private Dictionary<int, string> msoIDsByName = new Dictionary<int, string>();

    public void AssignAsInstance()
    {
        Instance = this;
    }

    public int GetStateByID(int objectID)
    {
        if (msoStates.TryGetValue(objectID, out var ret))
        {
            return ret;
        }

        return -1;
    }

    public void ChangeStateByName(string objName, int newState)
    {
        if (msoIDsByName.ContainsValue(objName))
        {
            int idToChange = -1;

            foreach (KeyValuePair<int, string> pair in msoIDsByName.Where(pair => pair.Value == objName))
            {
                idToChange = pair.Key;
                break;
            }

            if (idToChange <= -1) return;
            msoStates.Remove(idToChange);
            msoStates.Add(idToChange, newState);

            GameObject foundGameObject = GameObject.Find(objName);
            if (foundGameObject != null && foundGameObject.scene.isLoaded)
            {
                foundGameObject.GetComponent<FE_MultipleStateObject>().ChangeStateByID(newState, true);
            }
        }
        else
        {
            Debug.LogError("MSO Manager wanted to change state of object with name: " + objName + " to ID of " +
                           newState + " but it couldn't be found!");
        }
    }

    public void RecordState(int objectID, int newState, string objName)
    {
        if (msoStates.ContainsKey(objectID))
        {
            msoStates.Remove(objectID);
            if (msoIDsByName.ContainsKey(objectID))
            {
                msoIDsByName.Remove(objectID);
            }
        }

        msoIDsByName.Add(objectID, objName);
        msoStates.Add(objectID, newState);
    }

    public void SaveState(SceneSave sceneSave)
    {
        MultipleStateObjectManagerState saveState = new MultipleStateObjectManagerState();

        foreach (int key in msoStates.Keys)
        {
            saveState.Keys.Add(key);

            msoStates.TryGetValue(key, out var val);
            saveState.Values.Add(val);

            saveState.Names.Add(msoIDsByName[key]);
        }

        sceneSave.RecordSaveableState(saveState);
    }

    public void LoadFromState(MultipleStateObjectManagerState savedState)
    {
        msoStates.Clear();
        msoIDsByName.Clear();
        for (int i = 0; i < savedState.Keys.Count; i++)
        {
            Debug.Log("Loading state for MSO " + savedState.Names[i] + "  : " + savedState.Values[i]);
            msoStates.Add(savedState.Keys[i], savedState.Values[i]);
            msoIDsByName.Add(savedState.Keys[i], savedState.Names[i]);
        }
    }
}