using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    public static void SetLayer(this Transform trans ,int newLayer)
    {
        trans.gameObject.layer = newLayer;
        foreach (Transform child in trans)
        {
            child.gameObject.layer = newLayer;
            if (child.childCount > 0)
            {
                child.transform.SetLayer(newLayer);
            }
        }
    }
    
    public static void Scale(this Transform trans, float value)
    {
       trans.localScale += new Vector3(value, value, value);
    }
    
    public static void Scale(this Transform trans, Vector3 value)
    {
        trans.localScale += value;
    }
    
    public static void SetPositionZ(this Transform trans, float z)
    {
        trans.position = new Vector3(trans.position.x, trans.position.y, z);
    }
    
    public static void SetLocalPositionZ(this Transform trans, float z)
    {
        trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, z);
    }
}
