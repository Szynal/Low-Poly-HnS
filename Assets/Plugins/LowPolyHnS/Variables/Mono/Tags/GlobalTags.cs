using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class GlobalTags : ScriptableObject
    {
        public Tag[] tags = new Tag[32];
    }
}