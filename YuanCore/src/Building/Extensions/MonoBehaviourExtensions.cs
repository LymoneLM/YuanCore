using System;
using System.Collections;
using UnityEngine;

namespace YuanCore.Building;

public static class MonoBehaviourExtensions
{
    public static Coroutine DelayInvoke(this MonoBehaviour behaviour, Action callback, float delay)
    {
        return behaviour.StartCoroutine(DelayCoroutine(callback, delay));
    }

    private static IEnumerator DelayCoroutine(Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }
}
