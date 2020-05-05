using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Inventory
{
    [ExecuteAlways]
    [AddComponentMenu("LowPolyHnS/Inventory/Container")]
    public class Container : GlobalID, IGameSave
    {
        [Serializable]
        public class Data
        {
            public ItemsContainer items = new ItemsContainer();
        }

        [Serializable]
        public class ItemsContainer : SerializableDictionaryBase<int, ItemData>
        {
        }

        [Serializable]
        public class ItemData
        {
            public int uuid;
            public int amount;

            public ItemData(int uuid, int amount)
            {
                this.uuid = uuid;
                this.amount = amount;
            }
        }

        [Serializable]
        public class InitData
        {
            public ItemHolder item = new ItemHolder();
            public int amount = 0;
        }

        public class EventAdd : UnityEvent<int, int>
        {
        }

        public class EventRmv : UnityEvent<int, int>
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Data data { private set; get; }

        public List<InitData> initItems = new List<InitData>();

        public bool saveContainer = true;
        public GameObject containerUI;

        public Animator Animator;
        private static int STATE_NAME_HASH = Animator.StringToHash("ChestOpenClose");
        private static int CHEST_PARAMETER = Animator.StringToHash("Open");
        private AnimatorStateInfo animatorState;
        private bool isOpen;

        private readonly EventAdd eventAdd = new EventAdd();
        private readonly EventRmv eventRmv = new EventRmv();

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Start()
        {
            if (!Application.isPlaying) return;

            Initialize();
            SaveLoadManager.Instance.Initialize(this);
        }

        private void Initialize()
        {
            data = new Data();
            for (int i = 0; i < initItems.Count; ++i)
            {
                Item item = initItems[i].item.item;
                if (item == null || item.uuid == 0) continue;

                int uuid = item.uuid;
                int amount = initItems[i].amount;
                if (amount < 1) continue;

                if (data.items.ContainsKey(uuid))
                {
                    data.items[uuid].amount += amount;
                }
                else
                {
                    data.items.Add(uuid, new ItemData(uuid, amount));
                }
            }
        }

        protected virtual void OnDestroy()
        {
            OnDestroyGID();

            if (!Application.isPlaying) return;
            if (exitingApplication) return;

            SaveLoadManager.Instance.OnDestroyIGameSave(this);
        }

        // GETTER METHODS: ------------------------------------------------------------------------

        public List<ItemData> GetItems()
        {
            List<ItemData> items = new List<ItemData>();
            foreach (KeyValuePair<int, ItemData> element in data.items)
            {
                items.Add(element.Value);
            }

            return items;
        }

        public int GetAmount(int uuid)
        {
            if (!data.items.ContainsKey(uuid)) return 0;
            return data.items[uuid].amount;
        }

        // SETTER METHODS: ------------------------------------------------------------------------

        public void AddItem(int uuid, int amount = 1)
        {
            if (amount < 1) return;

            if (data.items.ContainsKey(uuid))
            {
                data.items[uuid].amount += amount;
                eventAdd.Invoke(uuid, amount);
            }
            else
            {
                data.items.Add(uuid, new ItemData(uuid, amount));
                eventAdd.Invoke(uuid, amount);
            }
        }

        public void RemoveItem(int uuid, int amount = 1)
        {
            if (amount < 1) return;

            if (data.items.ContainsKey(uuid))
            {
                int remaining = data.items[uuid].amount - amount;
                if (remaining < 0) amount = data.items[uuid].amount;

                data.items[uuid].amount -= amount;
                if (data.items[uuid].amount < 1)
                {
                    data.items.Remove(uuid);
                }

                eventRmv.Invoke(uuid, amount);
            }
        }

        // EVENT METHODS: -------------------------------------------------------------------------

        public void AddOnAddListener(UnityAction<int, int> callback)
        {
            eventAdd.AddListener(callback);
        }

        public void RemoveOnAddListener(UnityAction<int, int> callback)
        {
            eventAdd.RemoveListener(callback);
        }

        public void AddOnRemoveListener(UnityAction<int, int> callback)
        {
            eventRmv.AddListener(callback);
        }

        public void RemoveOnRemoveListener(UnityAction<int, int> callback)
        {
            eventRmv.RemoveListener(callback);
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public object GetSaveData()
        {
            if (!saveContainer) return null;
            return data;
        }

        public Type GetSaveDataType()
        {
            return typeof(Data);
        }

        public string GetUniqueName()
        {
            string uniqueName = string.Format(
                "container:{0}",
                GetID()
            );

            return uniqueName;
        }

        public void OnLoad(object generic)
        {
            Data newData = generic as Data;
            data = new Data();

            if (newData == null || !saveContainer) return;

            foreach (KeyValuePair<int, ItemData> item in newData.items)
            {
                int uuid = item.Value.uuid;
                int amount = item.Value.amount;

                if (data.items.ContainsKey(uuid))
                {
                    data.items[uuid].amount = amount;
                }
                else
                {
                    data.items.Add(uuid, new ItemData(uuid, amount));
                }
            }
        }

        public void ResetData()
        {
            Initialize();
        }

        // ANIMATOR: -----------------------------------------------------------------------------

        public void Animate()
        {
            if (Animator == null) return;
            animatorState = Animator.GetCurrentAnimatorStateInfo(0);
            isOpen = PlayAnimation(Animator, STATE_NAME_HASH, CHEST_PARAMETER, isOpen,
                animatorState.normalizedTime);
        }

        public static bool PlayAnimation(Animator animator, int stateNameHash, int parameterNameHash, bool typeOn,
            float normalizedTime)
        {
            float playTime = normalizedTime % 1;

            if (typeOn && normalizedTime > 1)
            {
                playTime = 1.0f;
            }
            else if (!typeOn && (normalizedTime > 1 || normalizedTime < 0))
            {
                playTime = 0.0f;
            }

            if (animator.GetFloat(parameterNameHash) >= 1)
            {
                typeOn = false;
                animator.SetFloat(parameterNameHash, -1);
                animator.Play(stateNameHash, -1, playTime);
            }
            else
            {
                typeOn = true;
                animator.SetFloat(parameterNameHash, 1);
                animator.Play(stateNameHash, -1, playTime);
            }

            return typeOn;
        }
    }
}