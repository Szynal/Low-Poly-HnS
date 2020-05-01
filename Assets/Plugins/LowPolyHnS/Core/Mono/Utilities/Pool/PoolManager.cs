using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Pool
{
    [AddComponentMenu("")]
    public class PoolManager : Singleton<PoolManager>
    {
        [Serializable]
        public class PoolData
        {
            public PoolObject prefab;
            public Transform container;
            public List<GameObject> instances;

            public PoolData(PoolObject prefab)
            {
                container = new GameObject(prefab.gameObject.name).transform;
                container.SetParent(Instance.transform);
                container.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                this.prefab = prefab;
                Rebuild();
            }

            private void Rebuild()
            {
                instances = new List<GameObject>();
                prefab.gameObject.SetActive(false);
                for (int i = 0; i < prefab.initCount; ++i)
                {
                    GameObject instance = Instantiate(prefab.gameObject);
                    instance.SetActive(false);
                    instance.transform.SetParent(container);
                    instances.Add(instance);
                }
            }

            public GameObject Get()
            {
                int count = instances.Count;
                if (count == 0) Rebuild();

                for (int i = count - 1; i >= 0; --i)
                {
                    if (instances[i] == null)
                    {
                        instances.RemoveAt(i);
                        continue;
                    }

                    if (!instances[i].activeSelf)
                    {
                        instances[i].SetActive(true);
                        instances[i].transform.SetParent(container);
                        return instances[i];
                    }
                }

                prefab.gameObject.SetActive(false);
                GameObject instance = Instantiate(prefab.gameObject);
                instance.transform.SetParent(container);

                instances.Add(instance);
                return instance;
            }
        }


        // PROPERTIES: ---------------------------------------------------------

        private Dictionary<int, PoolData> pool;

        // INITIALIZERS: -------------------------------------------------------

        protected override void OnCreate()
        {
            base.OnCreate();
            pool = new Dictionary<int, PoolData>();
        }

        // PUBLIC METHODS: -----------------------------------------------------

        public GameObject Pick(GameObject prefab)
        {
            if (prefab == null) return null;
            PoolObject component = prefab.GetComponent<PoolObject>();
            if (component == null) component = prefab.AddComponent<PoolObject>();

            return Pick(component);
        }

        public GameObject Pick(PoolObject prefab)
        {
            if (prefab == null) return null;
            int instanceID = prefab.GetInstanceID();

            if (!pool.ContainsKey(instanceID)) BuildPool(prefab);
            return pool[instanceID].Get();
        }

        // PRIVATE METHODS: ----------------------------------------------------

        private void BuildPool(PoolObject prefab)
        {
            int instanceID = prefab.GetInstanceID();
            pool.Add(instanceID, new PoolData(prefab));
        }
    }
}