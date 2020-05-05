using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace LowPolyHnS.Inventory
{
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "LowPolyHnS/Inventory/Loot Table")]
    public class LootTable : ScriptableObject
    {
        public class EventLoot : UnityEvent<LootResult>
        {
        }

        [Serializable]
        public class Loot
        {
            public ItemHolder item = new ItemHolder();
            public int amount = 1;
            public int weight = 1;

            public Loot(ItemHolder item, int amount, int weight)
            {
                this.item = item ?? new ItemHolder();
                this.amount = amount;
                this.weight = weight;
            }

            public Loot()
            {
                item = new ItemHolder();
                amount = 1;
                weight = 1;
            }
        }

        [Serializable]
        public class LootResult
        {
            public Item item;
            public int amount;

            public LootResult(ItemHolder itemHolder, int amount)
            {
                if (itemHolder != null) item = itemHolder.item;
                this.amount = amount;
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private static readonly EventLoot EVENT_LOOT = new EventLoot();

        public int noDropWeight = 0;
        public Loot[] loot = new Loot[0];

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public LootResult Get()
        {
            List<Loot> chances = new List<Loot>();
            int totalWeight = 0;

            if (noDropWeight > 0)
            {
                totalWeight += noDropWeight;
                chances.Add(new Loot(null, 0, noDropWeight));
            }

            for (int i = 0; i < loot.Length; ++i)
            {
                chances.Add(loot[i]);
                totalWeight += loot[i].weight;
            }

            chances.Sort((x, y) => y.weight.CompareTo(x.weight));
            int random = Random.Range(0, totalWeight);

            for (int i = 0; i < chances.Count; ++i)
            {
                Loot item = chances[i];
                if (random < item.weight)
                {
                    LootResult result = new LootResult(item.item, item.amount);
                    if (result.item == null || result.amount < 1) return new LootResult(null, 0);

                    EVENT_LOOT.Invoke(result);
                    return result;
                }

                random -= item.weight;
            }

            return new LootResult(null, 0);
        }

        // STATIC METHODS: ------------------------------------------------------------------------

        public static void AddListener(UnityAction<LootResult> callback)
        {
            EVENT_LOOT.AddListener(callback);
        }

        public static void RemoveListener(UnityAction<LootResult> callback)
        {
            EVENT_LOOT.RemoveListener(callback);
        }
    }
}