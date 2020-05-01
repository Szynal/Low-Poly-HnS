using System;
using UnityEngine;

namespace LowPolyHnS.Core
{
    public class LoadSaveController : MonoBehaviour, ISaveable
    {
        public static LoadSaveController Instance;
        [HideInInspector] [SerializeField] public int SaveableID = -1;

        private CharacterHealth healthScript;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                healthScript = GetComponent<CharacterHealth>();
            }
        }

        public void OnDestroy()
        {
            if (SceneLoader.Instance == null)
            {
                return;
            }

            SceneLoader.Instance.ActiveSaveables.Remove(this);
        }

        #region Saving/loading

        public void OnSave(SceneSave saveTo)
        {
            PlayerSaveState saveState = new PlayerSaveState
            {
                SaveableID = SaveableID,
                Health = healthScript.HealthCurrent,
                Position_X = transform.position.x,
                Position_Y = transform.position.y,
                Position_Z = transform.position.z,
                Rotation_Y = transform.rotation.eulerAngles.y
            };

            saveTo.RecordSaveableState(saveState);
        }

        public void OnLoad(EnemySaveState loadState)
        {
            throw new NotImplementedException();
        }

        public void OnLoad(PickupState loadState)
        {
            throw new NotImplementedException();
        }

        public void OnLoad(PlayerSaveState loadState)
        {
            if (loadState.Health > 0)
            {
                healthScript.HealthCurrent = loadState.Health;
            }

            Vector3 loadPosition = new Vector3(loadState.Position_X, loadState.Position_Y, loadState.Position_Z);
            Quaternion loadRotation = Quaternion.Euler(0f, loadState.Rotation_Y, 0f);
        }

        public void OnLoad(MultipleStateObjectManagerState loadState)
        {
            throw new NotImplementedException();
        }

        public void OnLoad(ActionTriggerState loadState)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}