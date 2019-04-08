using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartView : ViewBase
{
    protected override void Bind() {
        base.Bind();
        TestButton = c.GetCom<Button>("StartView/TestButton");
    }
    public Button TestButton;
}













