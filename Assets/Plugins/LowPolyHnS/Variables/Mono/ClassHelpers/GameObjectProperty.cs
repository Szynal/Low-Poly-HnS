using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class GameObjectProperty : BaseProperty<GameObject>
    {
        public GameObjectProperty()
        {
        }

        public GameObjectProperty(GameObject value) : base(value)
        {
        }

        protected override string GetValueName()
        {
            GameObject instance = value;
            return instance == null ? "(none)" : value.name;
        }
    }
}