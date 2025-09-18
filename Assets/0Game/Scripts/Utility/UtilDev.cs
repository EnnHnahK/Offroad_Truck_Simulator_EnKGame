using System;
using UnityEngine;

public enum TypeLog
{
    Normal,
    Warning,
    Error
}

public static class UtilDev
{
    public static void DebugLogUnity(string s, TypeLog typeLog = TypeLog.Normal)
    {
#if UNITY_EDITOR
        switch (typeLog)
        {
            case TypeLog.Normal:
                Debug.Log(s);
                break;
            case TypeLog.Warning:
                Debug.LogWarning(s);
                break;
            case TypeLog.Error:
                Debug.LogError(s);
                break;
        }
#endif
    }

    public static void DebugLogAll(string s, TypeLog typeLog = TypeLog.Normal)
    {
        switch (typeLog)
        {
            case TypeLog.Normal:
                Debug.Log(s);
                break;
            case TypeLog.Warning:
                Debug.LogWarning(s);
                break;
            case TypeLog.Error:
                Debug.LogError(s);
                break;
        }
    }

    public static void ActionCheat(Action action)
    {
#if CHEAT
        action.Invoke();
#endif
    }
}
