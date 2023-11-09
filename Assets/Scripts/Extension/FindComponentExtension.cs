using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FindComponentExtension
{
    public static List<T> GetAllInScene<T>(string sceneName, bool includeInactive = true)
    {
        List<T> results = new List<T>();      
        
        UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).GetRootGameObjects().ToList().ForEach(g => results.AddRange(g.GetComponentsInChildren<T>(includeInactive)));
        return results;
    }
    
    public static List<T> GetAllInScene<T>(this GameObject go, bool includeInactive = true)
    {
        List<T> results = new List<T>();      
        
        UnityEngine.SceneManagement.SceneManager.GetSceneByName(go.scene.name).GetRootGameObjects().ToList().ForEach(g => results.AddRange(g.GetComponentsInChildren<T>(includeInactive)));
        return results;
    }
    
    public static T GetInScene<T>(string gameObjectName, string sceneName) where T: Component
    {
        return GetAllInScene<T>(sceneName).Find(item => item.gameObject.name == gameObjectName);
    }
    
    public static T GetFirstInScene<T>(this GameObject go) where T: Component
    {
        return GetAllInScene<T>(go.scene.name).First();
    }
    
    public static T GetFirstInScene<T>(string sceneName) where T: Component
    {
        return GetAllInScene<T>(sceneName).First();
    }
    
    public static T GetInScene<T>(this GameObject go, string gameObjectName) where T: Component
    {
        return GetAllInScene<T>(go.scene.name).FindAll(item => item.gameObject.name == gameObjectName).FirstOrDefault();
    }
    
    public static T GetInSceneOnLayer<T>(int layer, string sceneName) where T: Component
    {
        return GetAllInScene<T>(sceneName).FindAll(item => item.gameObject.layer == layer).FirstOrDefault();
    }
    
    public static T GetInSceneOnLayer<T>(this GameObject go, int layer) where T: Component
    {
        return GetAllInScene<T>(go.scene.name).FindAll(item => item.gameObject.layer == go.layer).FirstOrDefault(); //SingleOrDefault(item => item.gameObject.layer == layer);
    }
    
    public static T GetInSceneOnLayer<T>(this GameObject go) where T: Component
    {
        return GetAllInScene<T>(go.scene.name).FindAll(item => item.gameObject.layer == go.layer).FirstOrDefault(); // SingleOrDefault(item => item.gameObject.layer == go.layer);
    }
    
    public static List<T> GetAllInSceneOnLayer<T>(this GameObject go) where T: Component
    {
        return GetAllInScene<T>(go.scene.name).FindAll(item => item.gameObject.layer == go.layer);
    }
    
    public static List<T> GetAllInSceneOnLayer<T>(this GameObject go, int layer) where T: Component
    {
        return GetAllInScene<T>(go.scene.name).FindAll(item => item.gameObject.layer == layer);
    }
    
    public static List<T> GetAllInSceneOnLayer<T>(string sceneName, int layer) where T: Component
    {
        return GetAllInScene<T>(sceneName).FindAll(item => item.gameObject.layer == layer);
    }
    
    public static T Get<T>(this Transform tr, string path)
    {
        return tr.Find(path).GetComponent<T>();
    }
    
    public static GameObject Get(this Transform tr, string path)
    {
        return tr.Find(path).gameObject;
    }
    
    public static GameObject GetChildrenByName(this Transform tr, string name)
    {
        List<Transform> go = new List<Transform>();
            
        go.AddRange(tr.gameObject.GetComponentsInChildren<Transform>(true));
        
        return go.SingleOrDefault(item => item.name == name)?.gameObject;
    }
    
    public static T GetComponentByName<T>(this Transform tr, string name) where T : Component
    {
        if (tr.GetChildrenByName(name).gameObject.HasComponent(out T tt))
        {
            return tt;
        }

        return null;
    }
    
    public static bool GetChildrenByName(this Transform tr, string name, out GameObject outGameObject)
    {
        List<Transform> go = new List<Transform>();
        go.AddRange(tr.gameObject.GetComponentsInChildren<Transform>(true));
        outGameObject = go.SingleOrDefault(item => item.name == name)?.gameObject;
        if (outGameObject)
            return true;

        return false;
    }
    
    public static T TryGet<T>(this Transform tr, string path)
    {
        if (tr.Find(path) == null)
            return default;
        
        if (tr.Find(path).GetComponent<T>() == null)
            return default;
        
        return tr.Find(path).GetComponent<T>();
    }

    public static T Get<T>(this GameObject go, string gameObjectName)
    {
        return GameObject.Find(gameObjectName).GetComponent<T>();
    }
    
    public static T GetFirstInChildren<T>(this GameObject go, bool includeInactive = false)
    {
        List<T> l = new List<T>();
        l.AddRange(go.GetComponentsInChildren<T>(includeInactive));

        if (l.Count > 0)
            return l[0];
        return default;
    }

    public static T GetWithTag<T>(this GameObject go, string tag)
    {
        return GameObject.FindWithTag(tag).GetComponent<T>();
    }
    
    public static T AddOneComponent<T>(this GameObject flag) where T : Component
    {
        if (flag.HasComponent(out T comp))
        {
            return comp;
        }
        
        return flag.AddComponent<T>();
    }

    public static bool HasComponent<T>(this GameObject flag) where T : Component
    {
        return flag.GetComponent<T>() != null;
    }
    
    public static bool HasComponent<T>(this GameObject flag, out T element) where T : Component
    {
        element = null;
        if (flag.HasComponent<T>())
        {
            element = flag.GetComponent<T>();
            return true;
        }

        return false;
    }
    
    public static bool HasComponentInParent<T>(this GameObject flag) where T : Component
    {
        return flag.GetComponentInParent<T>() != null;
    }
    
    public static bool HasComponentInParent<T>(this GameObject flag, out T element) where T : Component
    {
        element = null;
        if (flag.HasComponentInParent<T>())
        {
            element = flag.GetComponentInParent<T>();
            return true;
        }

        return false;
    }

    public static Mesh GetMesh(this GameObject go)
    {
        Mesh mesh = new Mesh();

        if (go.HasComponent(out MeshFilter meshFilter))
        {
            mesh = meshFilter.mesh;
        }
        else if (go.HasComponent(out SkinnedMeshRenderer skinnedMesh))
        {
            mesh = skinnedMesh.sharedMesh;
            if (go.HasComponent(out MeshCollider mc))
            {
                mc.sharedMesh = mesh;
            }
        }

        return mesh;
    }
}