using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// ???????????,
/// childrn?????gameobject??????????????¡¤????endwith???
/// </summary>
public class children : MonoBehaviour {

    private static List<children> allInst = new List<children>();

    public string Name = "";
    public Object[] array_children;
    /// <summary>
    /// ??????????????????????????????????¡¤?????
    /// </summary>
    public Object this[string name] {
        get { return findWithName(name); }
    }
    public T GetCom<T>(string name) where T:Component{
            return (this[name] as GameObject).GetComponent<T>();
    }
    private Object findWithName(string name) {
        for (int i = 0; i < array_children.Length;i++ ) {
            if (array_children[i].GetType() == typeof(GameObject)) {
                if (GetFullName(array_children[i] as GameObject).EndsWith(name))
                    return array_children[i];
            }else if (array_children[i].name == name)
                return array_children[i];
        }
        return null;
    }
    public static children GetInst(string name) {
        for (int i = 0; i < allInst.Count;i++ ) {
            if (allInst[i].Name == name)
                return allInst[i];
        }
        return null;
    }
    void Awake() {
        allInst.Add(this);
    }
    void OnDestroy() {
        allInst.Remove(this);
    }
    /// <summary>
    /// ???????????????????
    /// </summary>
    public void AutoFillAll() {
        if (gameObject == null)
            return;
        List<GameObject> tlst = new List<GameObject>();
        System.Action<GameObject> addChild = new System.Action<GameObject>((g)=>{ tlst.Add(g); });
        getChildren(gameObject.transform, addChild);
        array_children = tlst.ToArray();
    }
    private void getChildren(Transform t,System.Action<GameObject> OnFindOneChild) {
        OnFindOneChild(t.gameObject);
        if (t.childCount > 0) {
            for (int i = 0; i < t.childCount; i++) {
                getChildren(t.GetChild(i), OnFindOneChild);
            }
        } else
            return;
    }
    /// <summary>
    /// ????????????¡¤????
    /// </summary>
    public static string GetFullName(GameObject obj) {
        StringBuilder fullName = new StringBuilder("");
        if(obj!=null){
            Transform p = obj.transform;
            fullName.Insert(0, obj.name);
            while(p.parent!=null){
                p = p.parent;
                fullName.Insert(0, "/");
                fullName.Insert(0, p.gameObject.name);
            }
        }
        return fullName.ToString();
    }
}
