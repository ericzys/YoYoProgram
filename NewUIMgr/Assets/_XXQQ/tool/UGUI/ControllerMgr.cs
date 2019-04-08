using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// ?????????????
/// </summary>
public class ControllerMgr : MonoBehaviour {
    private static ControllerMgr _inst;
    public static ControllerMgr GetInst {
        get {
            if (_inst == null) {
                _inst = construct();
            }
            return _inst;
        }
    }
    private static ControllerMgr construct() {
        var obj = new GameObject("ControllerMgr");
        DontDestroyOnLoad(obj);
        return obj.AddComponent<ControllerMgr>();
    }
    List<ControllerBase> allController = new List<ControllerBase>();
    /// <summary>
    /// ????????
    /// </summary>
    /// <param name="ctrl"></param>
    public void RegController(ControllerBase ctrl) {
        if (!allController.Contains(ctrl)) {
            allController.Add(ctrl);
        }
    }
    /// <summary>
    /// ?????????
    /// </summary>
    /// <param name="ctrl"></param>
    public void RemoveController(ControllerBase ctrl) {
        if (allController.Contains(ctrl)) {
            allController.Remove(ctrl);
            ctrl.OnRemove();
        }
    }
    /// <summary>
    /// 绑定并注册控制器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="view"></param>
    /// <returns></returns>
    public T TryBindAndRegController<T>(ViewBase view) where T : ControllerBase, new() {
        var inst = RetraiceController<T>();
        if (inst==null) {
            inst = new T();
            inst.view = view;
            RegController(inst);
        }
        else {
            if (inst.view != view) {
                inst.view = view;
            }
        }
        return inst;
    }
    /// <summary>
    /// 绑定并注册控制器
    /// </summary>
    //public ControllerBase TryBindAndRegControllerByName(string controllerName,ViewBase view) {
    //    var type = System.Type.GetType(controllerName);
    //    if (type == null || !type.IsInheritFrom(typeof(ControllerBase)))
    //    {
    //        Debug.Log("找不到控制器" + controllerName);
    //        return null;
    //    }
    //    else
    //    {
    //        var inst = RetraiceController(controllerName);
    //        if (inst == null)
    //        {
    //            var ctor = type.GetConstructor(System.Type.EmptyTypes);
    //            inst = ctor.Invoke(new object[] { }) as ControllerBase;
    //            if (inst != null)
    //            {
    //                inst.view = view;
    //                RegController(inst);
    //            }
    //        }
    //        else
    //        {
    //            if (inst.view != view)
    //            {
    //                inst.view = view;
    //            }
    //        }
    //        return inst;
    //    }
    //}

    /// <summary>
    /// ???????????????
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T RetraiceController<T>() where T:ControllerBase {
        return (T)allController.Find(n => {
            return n.GetType() == typeof(T);
        });
    }
    /// <summary>
    /// ???????????????
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    public ControllerBase RetraiceController(string className) {
        return allController.Find(n => {
            return n.GetType().Name == className;
        });
    }
    void OnDestroy() {
        allController.Clear();
        _inst = null;
    }
    public void Print() {
        foreach (var item in allController) {
            Debug.Log(item);
        }
    }
}
