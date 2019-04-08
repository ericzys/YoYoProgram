using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
/// <summary>
/// §¿?????§Õ???
/// </summary>
public class CoroutineWrapper : MonoBehaviour {
    #region ????
    static CoroutineWrapper inst;
    private readonly static object mutex = new object();
    public static CoroutineWrapper Inst {
        get {
            if (inst == null) {
                var obj = new GameObject("CoroutineWrapper");
                DontDestroyOnLoad(obj);
                inst = obj.AddComponent<CoroutineWrapper>();
                inst.Init();
            }
            return inst;
        }
    }
    void Init() {
        inst = this;
    }
    #endregion
    /// <summary>
    /// ????????????
    /// </summary>
    /// <param name="frames"></param>
    /// <param name="ev"></param>
    /// <returns></returns>
    public IEnumerator ExeDelay(int frames,Action ev) {
        for (int i = 0; i < frames;i++ ) {
            yield return new WaitForEndOfFrame();
        }
        ev();
    }
    /// <summary>
    /// ????????????
    /// </summary>
    /// <param name="sec"></param>
    /// <param name="ev"></param>
    /// <returns></returns>
    public IEnumerator ExeDelayS(float sec, Action ev) {
        yield return new WaitForSeconds(sec);
        ev();;
    }
    /// <summary>
    /// ????????????
    /// </summary>
    /// <param name="sec"></param>
    /// <param name="ev"></param>
    public static void EXES(float sec, Action ev) {
        inst.StartCoroutine(inst.ExeDelayS(sec,ev));
    }
    /// <summary>
    /// ????????????
    /// </summary>
    /// <param name="frames"></param>
    /// <param name="ev"></param>
    public static void EXEF(int frames, Action ev) {
        inst.StartCoroutine(inst.ExeDelay(frames, ev));
    }
    /// <summary>
    /// ?????????§¿??
    /// </summary>
    /// <param name="ien">§¿?????</param>
    public static void EXEE(Func<IEnumerator> ien) {
        inst.StartCoroutine(ien());
    }
    /// <summary>
    /// ????????????object??§¿??
    /// </summary>
    /// <param name="ien">§¿?????</param>
    /// <param name="p">??????</param>
    public static void EXEE(Func<object[],IEnumerator> ien,params object[] p) {
        inst.StartCoroutine(ien(p));
    }
    /// <summary>
    /// ???????????????§¿?? ¦Ä???
    /// </summary>
    /// <param name="classInst">???????????</param>
    /// <param name="FuncName">??????</param>
    /// <param name="p">????</param>
    public static void EXEE(object classInst, string FuncName, params object[] p) {

    }
    /// <summary>
    /// ?????????????§¿?? ¦Ä???
    /// </summary>
    /// <param name="t">????</param>
    /// <param name="FuncName">??????</param>
    /// <param name="p">????</param>
    public static void EXEE(Type t, string FuncName, params object[] p) {
        Type[] pt = null;
        if (p!=null) {
            pt = new Type[p.Length];
            for (int i = 0; i < pt.Length; i++) {
                pt[i] = p[i].GetType();
            }
        }
        MethodInfo method;
        if (pt!=null) {
            method = t.GetMethod(FuncName,pt);
        } else {
            method = t.GetMethod(FuncName);
        }
    }
    /// <summary>
    /// ????
    /// </summary>
    public event Action OnPerFrame;
    void Update() {
        if (OnPerFrame != null)
            OnPerFrame();
    }
}
