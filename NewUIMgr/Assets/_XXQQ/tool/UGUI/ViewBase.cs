using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ViewBase : MonoBehaviour {

    protected children c;
    /// <summary>
    /// View被调用时传入的参数
    /// </summary>
    public Hashtable LastArg;
    public ViewBase Caller;
    public string CallerName;

    public ViewBase parent=null;
    public List<ViewBase> child = new List<ViewBase>();

    public Action<ViewBase> OnViewInitEvent, OnViewShowEvent, OnViewHideEvent, OnViewDisposeEvent, OnSubViewAddEvent, OnSubViewRemoveEvent;
    public string ViewName {
        get {
            return this.GetType().ToString();
        }
    }
    private void Awake() {
        Bind();
    }
    public virtual void Init(Hashtable p) {
        LastArg = p;
        if (OnViewInitEvent != null) {
            OnViewInitEvent(this);
        }
    }
    protected virtual void OnViewShow(Hashtable p) {
        LastArg = p;
        if (OnViewShowEvent != null) {
            OnViewShowEvent(this);
        }
    }
    protected virtual void OnViewHide(Hashtable p) {
        LastArg = p;
        if (OnViewHideEvent!=null) {
            OnViewHideEvent(this);
        }
    }
    protected virtual void OnViewDispose(Hashtable p) {
        LastArg = p;
        if (OnViewDisposeEvent!=null) {
            OnViewDisposeEvent(null);
        }
    }
    protected virtual void OnSubViewAdd(ViewBase view) {
        if (OnSubViewAddEvent!=null) {
            OnSubViewAddEvent(view);
        }
    }
    protected virtual void OnSubViewRemove(ViewBase view) {
        if (OnSubViewRemoveEvent!=null) {
            OnSubViewRemoveEvent(view);
        }
    }
    protected virtual void Bind() { 
        c = gameObject.AddComponent<children>();
        c.AutoFillAll();
    }
    public virtual void Show(Hashtable arg) {
        gameObject.SetActive(true);
        OnViewShow(arg);
    }
    public virtual void Hide(Hashtable arg) {
        OnViewHide(arg);
        gameObject.SetActive(false);
    }
    public virtual void AddChild(ViewBase view) {
        child.Add(view);
        OnSubViewAdd(view);
    }
    public virtual void RemoveChild(ViewBase view) {
        if (child.Contains(view)) {
            child.Remove(view);
            OnSubViewRemove(view);
        }
    }
    public virtual void DisposeSelf() {
        Hide(LastArg);
        if (OnViewDisposeEvent != null) {
            OnViewDisposeEvent(this);
        }
        CoroutineWrapper.EXEF(1, () => {
            if (parent != null) {
                parent.RemoveChild(this);
            }
            if (this !=null && gameObject != null)
                DestroyImmediate(gameObject);
        });
    }
}
