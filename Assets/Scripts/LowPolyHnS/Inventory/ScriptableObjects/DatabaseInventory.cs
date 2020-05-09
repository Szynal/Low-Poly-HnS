﻿using System;
using System.Collections.Generic;
using System.IO;
using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.Serialization;

namespace LowPolyHnS.Inventory
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    public class DatabaseInventory : IDatabase
    {
        [Serializable]
        public class InventoryCatalogue
        {
            public Item[] items;
            public Recipe[] recipes;

            public ItemType[] itemTypes = new ItemType[ItemType.MAX]
            {
                new ItemType(), new ItemType(), new ItemType(), new ItemType(),
                new ItemType(), new ItemType(), new ItemType(), new ItemType(),
                new ItemType(), new ItemType(), new ItemType(), new ItemType(),
                new ItemType(), new ItemType(), new ItemType(), new ItemType(),
                new ItemType(), new ItemType(), new ItemType(), new ItemType(),
                new ItemType(), new ItemType(), new ItemType(), new ItemType(),
                new ItemType(), new ItemType(), new ItemType(), new ItemType(),
                new ItemType(), new ItemType(), new ItemType(), new ItemType()
            };
        }

        [Serializable]
        public class InventorySettings
        {
            public GameObject merchantUIPrefab;
            public GameObject containerUIPrefab;
            public GameObject inventoryUIPrefab;
            public GameObject gatheringUIPrefab;
            public GameObject craftingUIPrefab;

            public bool onDragGrabItem = true;
            public bool saveInventory = true;

            public Texture2D cursorDrag;
            public Vector2 cursorDragHotspot;

            [Tooltip("Allow to execute a Recipe dropping an item onto another one")]
            public bool dragItemsToCombine = true;

            [FormerlySerializedAs("inventoryStopTime")]
            [Tooltip("Check if you want to pause the game when opening the Inventory menu")]
            public bool pauseTimeOnUI = false;

            [Tooltip("Check if you want to drop items dragging them out of the Inventory menu")]
            public bool canDropItems = true;

            [Tooltip("Max distance an item can be dropped from the Player")]
            public float dropItemMaxDistance = 2.0f;

            public bool limitInventoryWeight = false;
            public NumberProperty maxInventoryWeight = new NumberProperty(100f);
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public InventoryCatalogue inventoryCatalogue;
        public InventorySettings inventorySettings;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public List<int> GetItemSuggestions(string hint)
        {
            hint = hint.ToLower();
            List<int> suggestions = new List<int>();
            for (int i = 0; i < inventoryCatalogue.items.Length; ++i)
            {
                if (inventoryCatalogue.items[i] == null) continue;
                if (inventoryCatalogue.items[i].itemName.content.ToLower().Contains(hint) ||
                    inventoryCatalogue.items[i].itemDescription.content.ToLower().Contains(hint))
                {
                    suggestions.Add(i);
                }
            }

            return suggestions;
        }

        public string[] GetItemTypesNames()
        {
            ItemType[] itemTypes = inventoryCatalogue.itemTypes;
            if (itemTypes.Length == 0) return new string[0];

            string[] names = new string[itemTypes.Length];
            for (int i = 0; i < itemTypes.Length; ++i)
            {
                if (Application.isPlaying) names[i] = itemTypes[i].name.GetText();
                else names[i] = itemTypes[i].name.content;
            }

            return names;
        }

        public string[] GetItemTypesIDs()
        {
            ItemType[] itemTypes = inventoryCatalogue.itemTypes;
            if (itemTypes.Length == 0) return new string[0];

            string[] ids = new string[itemTypes.Length];
            for (int i = 0; i < itemTypes.Length; ++i)
            {
                ids[i] = Path.Combine(itemTypes[i].id, itemTypes[i].name.content);
            }

            return ids;
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static DatabaseInventory Load()
        {
            return LoadDatabase<DatabaseInventory>();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

#if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            Setup<DatabaseInventory>();
        }

        protected override string GetProjectPath()
        {
            return "Assets/Scripts/LowPolyHnSData/Inventory/Resources";
        }

#endif
    }
}