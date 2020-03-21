using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LowPolyHnS.Core
{
    [AddComponentMenu("Game Creator/Trigger", 0)]
    public class Trigger : MonoBehaviour
    {
        private static readonly Color WhiteLight = new Color(256, 256f, 256, 0.5f);

        public enum Platforms
        {
            Editor,
            Mobile,
            tvOS,
            Desktop,
            PS4,
            XBoxOne,
            WiiU,
            Switch
        }

        public enum ItemOpts
        {
            Conditions,
            Actions
        }

        [Serializable]
        public class Item
        {
            public ItemOpts Option;
            public Conditions Conditions;
            public Actions Actions;

            public Item(Actions actions)
            {
                Option = ItemOpts.Actions;
                Option = ItemOpts.Actions;
                Actions = actions;
            }

            public Item(Conditions conditions)
            {
                Option = ItemOpts.Actions;
                Option = ItemOpts.Conditions;
                Conditions = conditions;
            }
        }

        public const int ALL_PLATFORMS_KEY = -1;

        [Serializable]
        public class PlatformIgniters : SerializableDictionaryBase<int, Igniter>
        {
        }

        public class TriggerEvent : UnityEvent<GameObject>
        {
        }

        #region PROPERTIES

        public PlatformIgniters Igniters = new PlatformIgniters();
        public List<Item> Items = new List<Item>();

        public bool MinDistance = false;
        public float MinDistanceToPlayer = 5.0f;

        #endregion

        #region EVENTS

        public bool minDistance = false;
        public float minDistanceToPlayer = 5.0f;

        #endregion

        public TriggerEvent OnExecute = new TriggerEvent();

        // INITIALIZE: ----------------------------------------------------------------------------

        private void Awake()
        {
            EventSystemManager.Instance.Wakeup();
            SetupPlatformIgniter();
        }

        private void SetupPlatformIgniter()
        {
            bool overridePlatform = false;

#if UNITY_STANDALONE
            if (!overridePlatform) overridePlatform = CheckPlatformIgniter(Platforms.Desktop);
#endif

#if UNITY_EDITOR
            if (!overridePlatform) overridePlatform = CheckPlatformIgniter(Platforms.Editor);
#endif

#if UNITY_ANDROID || UNITY_IOS
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.Mobile);
#endif

#if UNITY_TVOS
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.tvOS);
#endif

#if UNITY_PS4
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.PS4);
#endif

#if UNITY_XBOXONE
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.XBoxOne);
#endif

#if UNITY_WIIU
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.WiiU);
#endif

#if UNITY_SWITCH
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.Switch);
#endif


            if (!overridePlatform)
            {
                if (!Igniters.ContainsKey(ALL_PLATFORMS_KEY))
                {
                    Igniter igniter = gameObject.AddComponent<IgniterStart>();
                    igniter.Setup(this);
                }

                Igniters[ALL_PLATFORMS_KEY].enabled = true;
            }
        }

        private bool CheckPlatformIgniter(Platforms platform)
        {
            if (Igniters.ContainsKey((int) platform))
            {
                Igniters[(int) Platforms.Editor].enabled = true;
                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty spIgniters = serializedObject.FindProperty("igniters");
            SerializedProperty spValues = spIgniters.FindPropertyRelative("values");

            bool updated = false;
            int spValuesSize = spValues.arraySize;
            for (int i = 0; i < spValuesSize; ++i)
            {
                SerializedProperty spIgniter = spValues.GetArrayElementAtIndex(i);
                Igniter igniter = spIgniter.objectReferenceValue as Igniter;
                if (igniter != null && igniter.gameObject != gameObject)
                {
                    Igniter newIgniter = gameObject.AddComponent(igniter.GetType()) as Igniter;
                    EditorUtility.CopySerialized(igniter, newIgniter);
                    spIgniter.objectReferenceValue = newIgniter;
                    updated = true;
                }
            }

            if (updated) serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
#endif

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void Execute(GameObject target, params object[] parameters)
        {
            if (MinDistance && HookPlayer.Instance != null)
            {
                float distance = Vector3.Distance(HookPlayer.Instance.transform.position, transform.position);
                if (distance > MinDistanceToPlayer) return;
            }

            if (OnExecute != null) OnExecute.Invoke(target);
            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i] == null) continue;
                switch (Items[i].Option)
                {
                    case ItemOpts.Actions:
                        if (Items[i].Actions == null) continue;
                        Actions actionsReference = Items[i].Actions;
                        if (string.IsNullOrEmpty(actionsReference.gameObject.scene.name))
                        {
                            actionsReference = Instantiate(
                                actionsReference,
                                transform.position,
                                transform.rotation
                            );
                        }

                        actionsReference.Execute(target, parameters);
                        break;

                    case ItemOpts.Conditions:
                        if (Items[i].Conditions == null) continue;
                        Conditions conditionsReference = Items[i].Conditions;
                        if (string.IsNullOrEmpty(conditionsReference.gameObject.scene.name))
                        {
                            conditionsReference = Instantiate(
                                conditionsReference,
                                transform.position,
                                transform.rotation
                            );
                        }

                        conditionsReference.Interact(target, parameters);
                        break;
                }
            }
        }

        public virtual void Execute()
        {
            Execute(gameObject);
        }

        // GIZMO METHODS: -------------------------------------------------------------------------
        

        private void OnDrawGizmos()
        {
            bool containsActions = false;
            bool containsConditions = false;

            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i].Option == ItemOpts.Actions) containsActions = true;
                if (Items[i].Option == ItemOpts.Conditions) containsConditions = true;
            }

            int state = 0;
            state |= containsConditions ? 0 : 1;
            state |= containsActions ? 0 : 2;

            switch (state)
            {
                case 0:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Trigger/trigger0", true);
                    break;
                case 1:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Trigger/trigger1", true);
                    break;
                case 2:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Trigger/trigger2", true);
                    break;
                case 3:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Trigger/trigger3", true);
                    break;
            }

            if (MinDistance)
            {
                Gizmos.color = WhiteLight;
                Gizmos.DrawWireSphere(transform.position, MinDistanceToPlayer);
            }
        }
    }
}