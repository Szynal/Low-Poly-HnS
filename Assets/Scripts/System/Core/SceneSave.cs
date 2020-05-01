using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISaveable
{
    void OnSave(SceneSave saveTo);
    void OnLoad(PlayerSaveState loadState);
    void OnLoad(EnemySaveState loadState);
    void OnLoad(PickupState loadState);
    void OnLoad(MultipleStateObjectManagerState loadState);
    void OnLoad(ActionTriggerState loadState);
    void OnDestroy();
}

// TO JEST OKROPNIE NAPISANE ... JPRDL
[Serializable]
public class SceneSave
{
    public int SceneID;

    private PlayerSaveState playerState;
    private MultipleStateObjectManagerState msoManagerState;
    private List<EnemySaveState> enemyStates = new List<EnemySaveState>();
    private List<PickupState> pickupStates = new List<PickupState>();
    private List<ActionTriggerState> triggerStates = new List<ActionTriggerState>();

    public void SaveScene(Scene scene)
    {
        SceneID = scene.buildIndex;

        foreach (var monoBehaviour in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
        {
            if (monoBehaviour == null) continue;
            if (!monoBehaviour.gameObject.scene.isLoaded || monoBehaviour.gameObject.scene != scene) continue;

            switch (monoBehaviour)
            {
                case ISaveable mb:
                {
                    Debug.Log("Scene " + scene.name + " is saving object: " + monoBehaviour.name);
                    var saveable = mb;
                    saveable.OnSave(this);
                    break;
                }
            }
        }
    }

    public void RecordSaveableState(SaveableState newState)
    {
        switch (newState)
        {
            //Zapisujemy do konkretnego arraya/miejsca, zaleznie od tego jaki jest typ
            case PlayerSaveState state:
                playerState = state;
                break;
            case EnemySaveState state:
            {
                if (enemyStates.Contains(state) == false) enemyStates.Add(state);

                break;
            }
            case PickupState state:
            {
                if (pickupStates.Contains(state) == false) pickupStates.Add(state);

                break;
            }
            case MultipleStateObjectManagerState state:
                msoManagerState = state;
                break;
            case ActionTriggerState state:
                triggerStates.Add(state);
                break;
        }
    }

    public PlayerSaveState GetPlayerState()
    {
        return playerState;
    }

    public MultipleStateObjectManagerState GetMSOManagerState()
    {
        return msoManagerState;
    }

    public EnemySaveState GetEnemyStateByID(int id)
    {
        EnemySaveState enemySaveState = null;
        foreach (EnemySaveState state in enemyStates.Where(state => state.SaveableID == id)) enemySaveState = state;

        return enemySaveState;
    }

    public PickupState GetPickupStateByID(int id)
    {
        PickupState pickupState = null;

        foreach (PickupState state in pickupStates.Where(state => state.SaveableID == id))
        {
            pickupState = state;
        }

        return pickupState;
    }

    public ActionTriggerState GetTriggerStateByID(int id)
    {
        ActionTriggerState actionTriggerState = null;
        foreach (ActionTriggerState state in triggerStates.Where(state => state.SaveableID == id))
        {
            actionTriggerState = state;
        }

        return actionTriggerState;
    }
}