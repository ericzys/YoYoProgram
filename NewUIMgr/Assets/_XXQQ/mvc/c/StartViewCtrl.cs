using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartViewCtrl : ControllerBase
{

    public StartView TarView
    {
        get
        {
            return view as StartView;
        }
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        Debug.Log("登录界面已加载");
        ViewMgr.GetInst().MarkAsDontGC(TarView, true);
        OnViewShow(null);
    }
    protected override void OnViewShow(ViewBase v)
    {
        base.OnViewShow(v);
        TarView.TestButton.onClick.AddListener(BtnTest);
    }

    private void BtnTest()
    {
        Debug.Log("testsssssssssssss");
    }

    protected override void OnViewHide(ViewBase v)
    {
        base.OnViewHide(v);
        TarView.TestButton.onClick.RemoveListener(BtnTest);
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }
}
