using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitView : MonoBehaviour
{
    void Start()
    {
        var cor = CoroutineWrapper.Inst;
        ViewMgr.GetInst(1920, 1080);
        ViewMgr.GetInst().ShowView(null, "StartView", null);

        var v = ViewMgr.GetInst().PeekTop();
        ControllerMgr.GetInst.TryBindAndRegController<StartViewCtrl>(v);
    }
}
