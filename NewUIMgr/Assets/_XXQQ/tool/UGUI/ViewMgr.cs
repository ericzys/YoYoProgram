using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
/// <summary>
/// view管理
/// 界面组织方式为安卓的singleTask方式
/// </summary>
public class ViewMgr : MonoBehaviour {

    public const string rootPath = "ui/view/";
    public const string KEY_LayoutWH = "LayoutWH";
    public const string KEY_OverLay = "OverLay";
    /// <summary>
    /// canvas所在的game object
    /// </summary>
    public GameObject RootCanvas;
    public Canvas canvas;
    private CanvasScaler canvasScaler;
    private static ViewMgr _inst;
    private static Vector2 layoutWH;

    protected List<ViewBase> holdView = new List<ViewBase>();
    bool gcEnable = true;
    List<ViewBase> viewStack = new List<ViewBase>();
    ViewBase overLayView = null;
    public Vector2 referenceResolution {
        get {
            return canvasScaler.referenceResolution;
        }
    }
    public static ViewMgr GetInst(int w,int h) {
        if (_inst == null) {
            _inst = Construct(w,h);
        }
        return _inst;

    }
    public static ViewMgr GetInst() {
        if (_inst == null) {
            throw new System.NullReferenceException("first use plase set w and h");
        }
        return _inst;
    }
    /// <summary>
    /// 第一次需调这个方法，传入布局宽高
    /// </summary>
    private static ViewMgr Construct(int w,int h) {
        var OutCanvas = GameObject.Find("UIRoot");
        if (OutCanvas != null) {
            var canvas = OutCanvas.GetComponent<Canvas>();
            
            var ctrl = OutCanvas.AddComponent<ViewMgr>();
            ctrl.RootCanvas = OutCanvas;
            ctrl.canvas = canvas;
            ctrl.canvasScaler = OutCanvas.GetComponent<CanvasScaler>();
            layoutWH = ctrl.canvasScaler.referenceResolution;
            return ctrl;
        }
        else {
            var obj = new GameObject("UIRoot");
            if (!Application.isEditor)
                DontDestroyOnLoad(obj);

            obj.AddComponent<RectTransform>();

            var c0 = obj.AddComponent<Canvas>();
            c0.renderMode = RenderMode.ScreenSpaceOverlay;
            c0.sortingOrder = 0;

            var c1 = obj.AddComponent<CanvasScaler>();

            c1.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            c1.referenceResolution = new Vector2(w, h);
            layoutWH = c1.referenceResolution;
            c1.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            c1.matchWidthOrHeight = 1;
            c1.referencePixelsPerUnit = 100;

            var c2 = obj.AddComponent<GraphicRaycaster>();
            c2.ignoreReversedGraphics = true;
            c2.blockingObjects = GraphicRaycaster.BlockingObjects.None;

            var eventObj = new GameObject("EventSystem");
            if (!Application.isEditor)
                DontDestroyOnLoad(eventObj);

            var e0 = eventObj.AddComponent<EventSystem>();
            e0.pixelDragThreshold = 5;
            e0.sendNavigationEvents = true;

            var e1 = eventObj.AddComponent<StandaloneInputModule>();
            e1.horizontalAxis = "Horizontal";
            e1.verticalAxis = "Vertical";
            e1.submitButton = "Submit";
            e1.cancelButton = "Cancel";
            e1.inputActionsPerSecond = 10;
#if !UNITY_4_6
            e1.repeatDelay = 0.5f;
#endif

            var e2 = eventObj.AddComponent<TouchInputModule>();

            var inst = obj.AddComponent<ViewMgr>();
            inst.canvasScaler = c1;
            inst.RootCanvas = obj;
            inst.canvas = c0;
            return inst;
        }
    }

    Transform tran;
    void Awake() {
        _inst = this;
        tran = transform;
    }

    /// <summary>
    /// 得到栈顶view
    /// </summary>
    /// <returns></returns>
    public ViewBase PeekTop() {
        if (viewStack.Count>0) {
            return viewStack[viewStack.Count-1];
        }
        return null;
    }
    private bool _showView(ViewBase caller, string callee, Hashtable arg, bool append = false)
    {
        Transform tar;
        if (!append)
        {
            tar = null;
            foreach (Transform i in tran)
            {
                if (i.name == callee)
                {
                    tar = i;
                    break;
                }
            }
        }
        else
        {
            if (caller == null)
            {
                Debug.Log("???????????????????μ???????????????");
                return false;
            }
            tar = caller.transform.Find(callee);
        }
        if (tar == null)
        {
            Object t2 = LoadView(rootPath + callee);
            if (t2)
            {
                //if (caller!=null)
                //    caller.gameObject.SetActive(append);
                GameObject t3 = (GameObject)(t2);
                //Destroy(t2,0.3f);
                t3.name = callee;
                t3.transform.SetParent(tran, false);
                var v = BindViewScript(t3, callee);
                tar = t3.transform;
                //PushViewToStack(v);
                v.Init(arg);
                //v.OnViewShow(arg);
                //return true;
            }
            else
            {
                Debug.Log("load view failed:" + callee);
                return false;
            }
        }
        bool overlay = false;
        //
        if (caller != null)
            caller.gameObject.SetActive(append);
        var old = PeekTop();
        if (old != null)
        {
            old.gameObject.SetActive(append);
        }
        tar.gameObject.SetActive(true);
        var t_view = tar.GetComponent<ViewBase>();

        if (!append)
        {
            if (old != null)
            {
                old.Hide(old.LastArg);
            }
            if (!overLayView)
            {
                PushViewToStack(t_view);
            }
            else
            {
                overLayView = t_view;
            }

        }
        else
        {
            if (caller != null)
            {
                tar.SetParent(caller.transform, false);
                tar.SetAsLastSibling();
                caller.AddChild(t_view);
            }
            t_view.parent = caller;
        }
        if (caller != null)
        {
            t_view.Caller = caller;
            t_view.CallerName = caller.GetType().Name;
        }
        canvasScaler.referenceResolution = layoutWH;
        if (!append)
            t_view.Show(arg);
        if (overLayView != null)
        {
            overLayView.transform.SetAsLastSibling();
        }
        GC();

        return true;
    }
    /// <summary>
    /// 显示View
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callee"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public bool ShowView(ViewBase caller, string callee, Hashtable arg) {
        return _showView(caller,callee,arg);
    }
    public bool AppendView(ViewBase caller, string callee, Hashtable arg) {
        return _showView(caller, callee, arg,true);
    }
    /// <summary>
    /// 设置布局宽高
    /// </summary>
    /// <param name="layout"></param>
    public void SetLayoutManul(Vector2 layout) {
        var c1 = gameObject.GetComponent<CanvasScaler>();
        c1.referenceResolution = layout;
    }
    public Component FindViewByType(System.Type t){
        return transform.GetComponentInChildren(t);
    }
    public T FindViewByType<T>() where T:ViewBase{
        return transform.GetComponentInChildren(typeof(T)) as T;
    }
    /// <summary>
    /// 关闭当前界面，会显示栈顶界面
    /// </summary>
    public void CloseView(ViewBase v) {
        if (PeekTop()!=v) {
            Debug.Log("未显示的界面不能使用此方法");
        }
        else if(viewStack.Count == 1){
            Debug.Log("已经是最后一个界面,不能关闭");
        }
        else {
            MarkAsDontGC(v, false);
            viewStack.Remove(v);
            v.DisposeSelf();
            var preView = PeekTop();
            preView.Show(preView.LastArg);
        }
    }
    /// <summary>
    /// 加载界面
    /// </summary>
    protected virtual Object LoadView(string path) {
        //assetsbundle方式
        //var file = Resources.Load<TextAsset>(path);
        //var asset = AssetBundle.LoadFromMemory(file.bytes);
        //var names = asset.GetAllAssetNames();
        //var v = (GameObject)Instantiate(asset.LoadAsset<GameObject>(names[0]));
        //asset.Unload(false);
        //return v;
        //
        return (GameObject)Instantiate(Resources.Load(path));
    }
    /// <summary>
    /// 关联viewbase具体子类
    /// </summary>
    protected virtual ViewBase BindViewScript(GameObject viewInst,string className) {
        return viewInst.AddComponent(System.Type.GetType(className)) as ViewBase;
    }
    /// <summary>
    /// 入view栈
    /// </summary>
    /// <param name="view"></param>
    protected virtual void PushViewToStack(ViewBase view) {
        if (viewStack.Contains(view)) {
            var topView = PeekTop();
            while (topView != view) {
                viewStack.Remove(topView);
                topView.DisposeSelf();
                topView = PeekTop();
            }
        }
        else {
            viewStack.Add(view);
        }
    }
    public void SetGCEnable(bool enable) {
        gcEnable = enable;
    }
    /// <summary>
    /// 回收
    /// </summary>
    protected virtual void GC() {
        if (!gcEnable) {
            return;
        }
        var cur = PeekTop();
        List<ViewBase> tmp = new List<ViewBase>();
        for (int i = viewStack.Count - 1; i >= 0; i--) {
            var node = viewStack[i];
            if (!holdView.Contains(node) && node!=cur) {
                tmp.Add(node);
            }
        }
        for (int i = 0; i < tmp.Count; i++) {
            viewStack.Remove(tmp[i]);
            tmp[i].DisposeSelf();
        }
        //CoroutineWrapper.EXEF(3, () => {
        //    Resources.UnloadUnusedAssets();
        //});
    }
    /// <summary>
    /// 标记为不回收
    /// </summary>
    public void MarkAsDontGC(ViewBase view,bool mark) {
        //return;
        if (view == null) {
            return;
        }
        else {
            bool has = holdView.Contains(view);
            if (has) {
                if (mark) {
                    return;
                }
                else {
                    holdView.Remove(view);
                }
            }
            else {
                if (mark) {
                    holdView.Add(view);
                }
                else {
                    return;
                }
            }
        }
    }
    void OnDestroy() {
        viewStack.Clear();
        _inst = null;
    }
    public override string ToString() {
        string stack = "界面栈自顶向下:\n";
        for (int i = viewStack.Count-1; i >= 0; i--) {
            stack += viewStack[i].ToString() + "\n";
        }
        return stack;
    }
    public List<ViewBase> ViewStack {
        get {
            return viewStack;
        }
    }
}
