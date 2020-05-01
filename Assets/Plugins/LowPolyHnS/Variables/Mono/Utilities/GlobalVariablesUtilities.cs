namespace LowPolyHnS.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using LowPolyHnS.Core;

    public static class GlobalVariablesUtilities
    {
        public static Variable Get(string name)
        {
            return GlobalVariablesManager.Instance.Get(name);
        }
    }
}