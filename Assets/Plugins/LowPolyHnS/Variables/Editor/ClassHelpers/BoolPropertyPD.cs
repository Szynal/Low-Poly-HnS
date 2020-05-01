﻿namespace LowPolyHnS.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(BoolProperty))]
    public class BoolPropertyPD : BasePropertyPD
    {
        protected override int GetAllowTypesMask()
        {
            return 1 << (int)Variable.DataType.Bool;
        }
    }
}