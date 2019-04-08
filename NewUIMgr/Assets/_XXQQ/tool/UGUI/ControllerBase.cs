using UnityEngine;
using System.Collections;
/// <summary>
/// view??????
/// </summary>
public class ControllerBase{
    private ViewBase _view;
    /// <summary>
    /// ???
    /// </summary>
    public ViewBase view {
        get { return _view; }
        set {
            if (_view!=null && value == null) {
                ControllerMgr.GetInst.RemoveController(this);
            }
            _view = value;
            if (_view!=null) {
                OnViewLoaded();
            }
        }
    }
    protected string _name = "ControllerBase";
    public virtual string Name {
        get { return _name; }
    }
    public ControllerBase() {
    }
    public ControllerBase(ViewBase view) {
        this.view = view;
    }
    /// <summary>
    /// ??????update??????
    /// </summary>
    protected virtual void OnViewLoaded() {
        Debug.Log("set view to "+this);
        _view.OnViewShowEvent += OnViewShow;
        _view.OnViewHideEvent += OnViewHide;
        _view.OnViewDisposeEvent += OnViewDispose;
        _view.OnSubViewAddEvent += OnViewAddChild;
        _view.OnSubViewRemoveEvent += OnViewRemoveChild;
    }
    /// <summary>
    /// ?????
    /// </summary>
    protected virtual void update() {
    }
    public virtual void OnRemove() {
        Debug.Log(this + " removed");
        if (_view!=null) {
            _view.OnViewShowEvent -= OnViewShow;
            _view.OnViewHideEvent -= OnViewHide;
            _view.OnViewDisposeEvent -= OnViewDispose;
            _view.OnSubViewAddEvent -= OnViewAddChild;
            _view.OnSubViewRemoveEvent -= OnViewRemoveChild;
        }
    }
    protected virtual void OnViewShow(ViewBase v) {
        //Debug.Log("view show " + arg + "," + this.GetType());
        CoroutineWrapper.Inst.OnPerFrame += update;
    }
    protected virtual void OnViewHide(ViewBase v) {
        //Debug.Log("view hide " + arg + "," + this.GetType());
        CoroutineWrapper.Inst.OnPerFrame -= update;
    }
    protected virtual void OnViewDispose(ViewBase v) {
        view = null;
        //Debug.Log("view dispose "+arg+","+view+","+this.GetType());
    }
    protected virtual void OnViewAddChild(ViewBase v) { 
    }
    protected virtual void OnViewRemoveChild(ViewBase v) {
    }
}
