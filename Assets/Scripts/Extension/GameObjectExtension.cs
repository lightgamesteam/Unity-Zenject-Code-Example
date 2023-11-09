using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{
    public static void DestroyNeighbors(this GameObject go)
    {
        foreach (Transform tr in go.transform.parent)
        {
            if(!tr.gameObject.Equals(go))
                tr.gameObject.SelfDestroy();
        }
    }
    
    public static void DestroyChildren(this GameObject go)
    {
        foreach (Transform tr in go.transform)
        {
            tr.gameObject.SelfDestroy();
        }
    }
    
    public static GameObject Duplicate(this GameObject go)
    {
        GameObject newGO = MonoBehaviour.Instantiate(go, go.transform.parent, false);
        newGO.transform.localScale = go.transform.localScale;
        newGO.transform.SetPositionZ(go.transform.position.z);
		
        return newGO;
    }
    
    public static T Duplicate<T>(this GameObject go, bool addComponent = false) where T : Component
    {
        GameObject newGO = go.Duplicate();

        if (newGO.HasComponent(out T component))
        {
            return component;
        }
        
        if (addComponent)
            return newGO.AddComponent<T>();

        return null;
    }
    
    public static void SelfDestroy(this GameObject go)
    {
        MonoBehaviour.Destroy(go);
    }
    
    public static void SelfDestroy(this Component component)
    {
        MonoBehaviour.Destroy(component);
    }
    
    public static void SelfDestroy(this GameObject go, float wait)
    {
        MonoBehaviour.Destroy(go, wait);
    }
    
    public static Bounds GetBounds(this GameObject go, bool addSelfBound = false)
    {
        Bounds modelBounds = new Bounds();
        List<Renderer> lR = new List<Renderer>();
        lR.AddRange(go.GetAllComponentsInChildren<Renderer>());
        
        foreach (Renderer renderer in lR) 
        {
            modelBounds.Encapsulate(renderer.bounds);
        }

        if (addSelfBound)
        {
            if (go.HasComponent<Renderer>())
                modelBounds.Encapsulate(go.GetComponent<Renderer>().bounds);
        }
        
        return modelBounds;
    }

    public static List<T> GetAllComponentsInChildren<T>(this GameObject go, bool includeSelf = false) where T : Component
    {
        List<T> c = new List<T>();

        if (includeSelf)
        {
            if (go.HasComponent<T>())
            {
                c.Add(go.GetComponent<T>());
            }
        }
        
        foreach (GameObject child in go.GetAllChildren())
        {
            if (child.HasComponent<T>())
            {
                c.Add(child.GetComponent<T>());
            }
        }

        return c;
    }

    public static List<GameObject> GetAllChildren(this GameObject go)
    {
        List<GameObject> gameObjects = new List<GameObject>();

        foreach (Transform child in go.transform)
        {
            gameObjects.Add(child.gameObject);

            if (child.childCount > 0)
            {
                gameObjects.AddRange(child.gameObject.GetAllChildren());
            }
        }

        return gameObjects;
    }
}