using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class FE_Extensions
{
    public static void WaitFramesAndCall(this MonoBehaviour _caller, int _numOfFrames, Action<bool> _funcToCall, bool _arg)
    {
        _caller.StartCoroutine(waitFramesAndCall(_numOfFrames, _funcToCall, _arg));
    }

    public static IEnumerator waitFramesAndCall(int _numOfFrames, Action<bool> _func, bool _arg)
    {
        for(int i = 0; i < _numOfFrames; i++)
        {
            yield return null;
        }

        _func(_arg);
    }
}
