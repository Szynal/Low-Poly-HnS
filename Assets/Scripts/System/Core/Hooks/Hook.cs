using System.Collections.Generic;
using UnityEngine;

namespace LowPolyHnS.Core.Hooks
{
    public abstract class Hook<T> : MonoBehaviour
    {
        public static Hook<T> Instance;
        private Dictionary<int, Behaviour> components;

        private void Awake()

        {
            Instance = this;
            components = new Dictionary<int, Behaviour>();
        }

        public TComponent Get<TComponent>() where TComponent : Behaviour
        {
            int componentHash = typeof(TComponent).GetHashCode();

            if (components.ContainsKey(componentHash))
            {
                return (TComponent) components[componentHash];
            }

            Behaviour mono = gameObject.GetComponent<TComponent>();

            if (mono == null)
            {
                return null;
            }

            components.Add(componentHash, mono);
            return (TComponent) mono;
        }
    }
}