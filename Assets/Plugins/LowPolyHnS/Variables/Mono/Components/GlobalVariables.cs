namespace LowPolyHnS.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using LowPolyHnS.Core;

    [Serializable]
    public class GlobalVariables : ScriptableObject
    {
        public SOVariable[] references = new SOVariable[0];
	}
}