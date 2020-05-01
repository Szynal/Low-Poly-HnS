using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class GlobalVariables : ScriptableObject
    {
        public SOVariable[] references = new SOVariable[0];
    }
}