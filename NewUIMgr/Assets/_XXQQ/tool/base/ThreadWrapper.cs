using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// ?????????
/// ???????????? CoroutineWrapper
/// </summary>
public class ThreadWrapper{

    static List<Action> actLst = new List<Action>();
    static bool isInited = false;
    /// <summary>
    /// ?????????????????¨°????????????,?????????????
    /// </summary>
    public static void Invoke(Action act) {
        if (!isInited) {
            UnityEngine.Debug.LogError("ThreadWrapper??Init()????????¨´?");
            return;
        }
        lock (actLst) {
            if(act!=null)
                actLst.Add(act);
        }
    }
    public static void Init() {
        if (!isInited) {
            CoroutineWrapper.Inst.OnPerFrame += Loop;
            isInited = true;
        }
    }
    static void Loop() {
        if (actLst != null && actLst.Count > 0) {
            lock (actLst) {
                CoroutineWrapper.EXEF(1, actLst[0]);
                actLst.RemoveAt(0);
            }
        }
    }
}
