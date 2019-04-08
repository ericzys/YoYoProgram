using UnityEngine;

public class SingleT<T> where T : new()
{
    private static T singleClass;
    public static T Instance
    {
        get
        {
            if (singleClass == null)
            {
                singleClass = new T();
            }
            return singleClass;
        }
    }
}