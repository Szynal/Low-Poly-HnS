using LowPolyHnS.Core;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("LowPolyHnS/Characters/Player Character", 100)]
    public class PlayerCharacter : Character
    {
        protected const string PLAYER_ID = "player";
        public static OnLoadSceneData ON_LOAD_SCENE_DATA;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            if (!Application.isPlaying) return;

            InitSaveData = new SaveData
            {
                Position = transform.position,
                Rotation = transform.rotation
            };

            if (Save)
            {
                SaveLoadManager.Instance.Initialize(
                    this, (int) SaveLoadManager.Priority.Normal, true
                );
            }

            HookPlayer hookPlayer = gameObject.GetComponent<HookPlayer>();
            if (hookPlayer == null) gameObject.AddComponent<HookPlayer>();

            if (ON_LOAD_SCENE_DATA != null && ON_LOAD_SCENE_DATA.Active)
            {
                transform.position = ON_LOAD_SCENE_DATA.Position;
                transform.rotation = ON_LOAD_SCENE_DATA.Rotation;
                ON_LOAD_SCENE_DATA.Consume();
            }
        }

        // GAME SAVE: -----------------------------------------------------------------------------

        protected override string GetUniqueCharacterID()
        {
            return PLAYER_ID;
        }
    }
}