using System;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("LowPolyHnS/Trigger", 0)]
    public class Trigger : MonoBehaviour
    {
        public enum ItemOpts
        {
            Conditions,
            Actions
        }

        /*
        [Serializable]
        public class Item
        {
            public Actions actions = null;
            public Conditions conditions = null;
            public ItemOpts option = ItemOpts.Actions;

            public Item(Actions actions)
            {
                option = ItemOpts.Actions;
                this.actions = actions;
            }

            public Item(Conditions conditions)
            {
                option = ItemOpts.Conditions;
                this.conditions = conditions;
            }
        }
        */
    }
}