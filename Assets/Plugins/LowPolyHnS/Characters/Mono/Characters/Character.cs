using System;
using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [RequireComponent(typeof(CharacterController))]
    [AddComponentMenu("LowPolyHnS/Characters/Character", 100)]
    public class Character : GlobalID, IGameSave
    {
        [Serializable]
        public class SaveData
        {
            public Vector3 Position = Vector3.zero;
            public Quaternion Rotation = Quaternion.identity;
        }

        [Serializable]
        public class OnLoadSceneData
        {
            public bool Active { get; private set; }
            public Vector3 Position { get; private set; }
            public Quaternion Rotation { get; private set; }

            public OnLoadSceneData(Vector3 position, Quaternion rotation)
            {
                Active = true;
                Position = position;
                Rotation = rotation;
            }

            public void Consume()
            {
                Active = false;
            }
        }

        protected const string ERR_NOCAM = "No Main Camera found.";


        #region PROPERTIES

        public bool Save;
        protected SaveData InitSaveData = new SaveData();

        #endregion

        #region INITIALIZERS

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;
            InitSaveData = new SaveData
            {
                Position = transform.position,
                Rotation = transform.rotation
            };

            if (Save)
            {
                SaveLoadManager.Instance.Initialize(this);
            }
        }

        protected void OnDestroy()
        {
            OnDestroyGID();
            if (!Application.isPlaying) return;

            if (Save && !exitingApplication)
            {
                SaveLoadManager.Instance.OnDestroyIGameSave(this);
            }
        }

        #endregion


        #region GAME SAVE

        public string GetUniqueName()
        {
            string uniqueName = $"character:{GetUniqueCharacterID()}";
            return uniqueName;
        }

        protected virtual string GetUniqueCharacterID()
        {
            return GetID();
        }

        public Type GetSaveDataType()
        {
            return typeof(SaveData);
        }

        public object GetSaveData()
        {
            return new SaveData
            {
                Position = transform.position,
                Rotation = transform.rotation
            };
        }

        public void ResetData()
        {
            transform.position = InitSaveData.Position;
            transform.rotation = InitSaveData.Rotation;
        }

        public void OnLoad(object generic)
        {
            if (!(generic is SaveData container))
            {
                return;
            }

            transform.position = container.Position;
            transform.rotation = container.Rotation;
        }

        #endregion
    }
}