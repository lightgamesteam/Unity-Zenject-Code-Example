using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CI.TaskParallel;
using UnityEngine;
using Object = UnityEngine.Object;

public static class MeshExtension
{
    public static Vector3 GetCenterOfGeometry(this Mesh _mesh)
    {
        Vector3 sumPos = Vector3.zero;

        foreach (Vector3 v3 in _mesh.vertices)
        {
            sumPos += v3;
        }

        sumPos /= _mesh.vertices.Length;

       return sumPos.Invert();
    }
    
    public static Vector3 GetCenterOfMass(this Mesh _mesh)
    {
        return _mesh.bounds.center.Invert();
    }

    public static Vector3 Multiply(this Vector3 _v3, Vector3 multiply)
    {
        return new Vector3(_v3.x * multiply.x, _v3.y * multiply.y, _v3.z * multiply.z);
    }
    
    public static Vector3 Invert(this Vector3 _v3)
    {
        return Vector3.zero - _v3;
    }
    
    public static Vector3 VertexPositionToWorldPosition(Vector3 vertex, Transform owner)
    {
        return owner.localToWorldMatrix.MultiplyPoint3x4(vertex);
    }
    
    public static Vector3[] VerticesPositionToWorldPosition(Vector3[] vertices, Transform owner)
    {
        Vector3[] v = new Vector3[vertices.Length];
        
        for (int i = 0; i < vertices.Length; i++)
        {
            v[i] = VertexPositionToWorldPosition(vertices[i], owner);
        }
        return v;
    }
    
    public static void VerticesPositionToWorldPosition(MeshFilter[] mf, Action<Vector3[]> result)
    {
        List<Vector3> l = new List<Vector3>();

        foreach (MeshFilter meshFilter in mf)
        {
            foreach (Vector3 v in meshFilter.mesh.vertices)
            {
                l.Add(meshFilter.transform.localToWorldMatrix.MultiplyPoint3x4(v));
            }
        }
  
        result.Invoke(l.ToArray());
    }
    
    public static IEnumerator VerticesPositionToWorldPositionAsync(List<(Transform _transform, Vector3[] vertices)> vert, Action<Vector3[]> result)
    {
        List<Vector3> l = new List<Vector3>();

        foreach (var tr in vert)
        {
            yield return new WaitForEndOfFrame();

            foreach (Vector3 v3 in tr.vertices)
            {
                l.Add(tr._transform.localToWorldMatrix.MultiplyPoint3x4(v3));
            }
        }
  
        result.Invoke(l.ToArray());
    }

    public static IEnumerator VerticesPositionToWorldPositionAsync(MeshFilter[] mf, int step, Action<Vector3[]> result)
    {
        List<Vector3> l = new List<Vector3>();

        foreach (MeshFilter meshFilter in mf)
        {
            yield return new WaitForEndOfFrame();

            foreach (Vector3 v3 in meshFilter.mesh.vertices)
            {
                l.Add(meshFilter.transform.localToWorldMatrix.MultiplyPoint3x4(v3));
            }
        }
  
        result.Invoke(l.ToArray());
    }
    
    public static Vector3[] GetAllVertexPosition(this GameObject go, bool includeChild = false)
    {
        List<Vector3> v3 = new List<Vector3>();

        if (go.HasComponent(out MeshFilter omf))
        {
            v3.AddRange(VerticesPositionToWorldPosition(omf.mesh.vertices, go.transform));
        }
        
        if (!includeChild)
            return v3.ToArray();
        
        List<MeshFilter> mf = new List<MeshFilter>(); 
        mf.AddRange(go.GetAllComponentsInChildren<MeshFilter>());

        foreach (MeshFilter meshFilter in mf)
        {
            v3.AddRange(VerticesPositionToWorldPosition(meshFilter.mesh.vertices, meshFilter.transform));
        }
            
        return v3.ToArray();
    }

    public static void BakeAndGetAllVertexPosition(this SkinnedMeshRenderer skinnedMeshRenderer, out Vector3[] vertices)
    {
        Mesh mesh = new Mesh();
        MemoryManager.Instance?.AddObjectToMemoryManager(skinnedMeshRenderer.gameObject.scene.name, mesh);

        var tr = skinnedMeshRenderer.transform;
        Vector3 modelScale = tr.localScale;
        Quaternion modelRot = tr.rotation;
        tr.localScale = Vector3.one;
        tr.rotation = Quaternion.identity;
        skinnedMeshRenderer.BakeMesh(mesh);
        tr.localScale = modelScale;
        tr.rotation = modelRot;

        vertices = mesh.vertices;
        
        Object.Destroy(mesh);
    }
    
    public static void GetAllVertexPosition(this GameObject go, bool includeChild, Action<Vector3[]> result)
    {
        List<(Transform _transform, Vector3[] vertices)> mf = new List<(Transform _transform, Vector3[] vertices)>();

        if (go.HasComponent(out MeshFilter m))
        {
            mf.Add((m.transform, m.mesh.vertices));
        }
        else if (go.HasComponent(out SkinnedMeshRenderer smr))
        {
            Mesh mesh = new Mesh();
            MemoryManager.Instance?.AddObjectToMemoryManager(smr.gameObject.scene.name, mesh);
            smr.BakeMesh(mesh);
            mf.Add((smr.transform, mesh.vertices));
            GameObject.Destroy(mesh);
        }

        if (includeChild)
        {
            go.GetAllComponentsInChildren<MeshFilter>().ForEach(mv =>
            {
                mf.Add((mv.transform, mv.mesh.vertices));
            });
            
            go.GetAllComponentsInChildren<SkinnedMeshRenderer>().ForEach(smrs =>
            {
                Mesh mesh = new Mesh();
                MemoryManager.Instance?.AddObjectToMemoryManager(smrs.gameObject.scene.name, mesh);

                smrs.BakeMesh(mesh);
                mf.Add((smrs.transform, mesh.vertices));
                GameObject.Destroy(mesh);
            });
        }

        AsyncProcessorService.Instance.StartCoroutine(VerticesPositionToWorldPositionAsync(mf, result.Invoke));
    }
    
    public static Vector3 GetClosestPositionTo(this Vector3[] v3, Vector3 position)
    {
        Vector3 closest = Vector3.zero;
        float dis = Single.PositiveInfinity;
        
        foreach (Vector3 v in v3)
        {
            float nDis = Vector3.Distance(position, v);

            if (nDis < dis)
            {
                dis = nDis;
                closest = v;
            }
        }
        
        return closest;
    }
    
    public static void TaskGetClosestPositionTo(this Vector3[] v3, Vector3 position, Action<Vector3> result)
    {
        UnityTask.Run((() =>
        {
            Vector3 closest = Vector3.zero;
            float dis = Single.PositiveInfinity;
        
            foreach (Vector3 v in v3)
            {
                float nDis = Vector3.Distance(position, v);

                if (nDis < dis)
                {
                    dis = nDis;
                    closest = v;
                }
            }
            
            return closest;
            
        })).ContinueOnUIThread(r =>
        {
            result.Invoke(r.Result);
        });
    }



    public static void GetClosestPositionTo(this Vector3[] v3, Vector3 position, Action<Vector3> result)
    {
        Vector3 closest = Vector3.zero;
        float dis = Single.PositiveInfinity;

        foreach (Vector3 v in v3)
        {
            float nDis = Vector3.Distance(position, v);

            if (nDis < dis)
            {
                dis = nDis;
                closest = v;
            }
        }

        result.Invoke(closest);
    }

    //---------------------------

    public static List<GameObject> ExtractSubMeshes(this GameObject go)
  {
      // Isolate Sub Meshes
      MeshFilter meshFilterComponent = go.GetComponent<MeshFilter>();
      MeshRenderer meshRendererComponent = go.GetComponent<MeshRenderer>();
      Mesh mesh = meshFilterComponent.sharedMesh;
      
      List<GameObject> outGO = new List<GameObject>();
      
      for (int i = 0; i < mesh.subMeshCount; i++)
      {
          GameObject meshFromSubMeshGameObject = new GameObject("SubMesh_" + i);
          
          meshFromSubMeshGameObject.transform.SetParent(go.transform);
          meshFromSubMeshGameObject.transform.localPosition = Vector3.zero;
          meshFromSubMeshGameObject.transform.localRotation = Quaternion.identity;
          meshFromSubMeshGameObject.transform.localScale = Vector3.one;
          
          MeshFilter meshFromSubMeshFilter = meshFromSubMeshGameObject.AddComponent<MeshFilter>();
          meshFromSubMeshFilter.sharedMesh = mesh.GetSubMeshByIndex(i);
          MemoryManager.Instance?.AddObjectToMemoryManager(go.scene.name, meshFromSubMeshFilter.sharedMesh);

          MeshRenderer meshFromSubMeshMeshRendererComponent = meshFromSubMeshGameObject.AddComponent<MeshRenderer>();
          
          if (meshRendererComponent != null)
          {
              // To use the same mesh renderer properties of the initial mesh
              //EditorUtility.CopySerialized(meshRendererComponent, meshFromSubmeshMeshRendererComponent);
              // We just need the only one material used by the sub mesh in its renderer
              
             var materials = meshRendererComponent.sharedMaterials.ToList();

                 Material m = new Material(
                     i < materials.Count ? materials[i] : materials[materials.Count-1]
                     );
                 
                 meshFromSubMeshMeshRendererComponent.materials = new[] {m};
          }
          
          go.transform.SetLayer(go.layer);
          
          outGO.Add(meshFromSubMeshGameObject);
      }

      return outGO;
  }
  
  private static List<Mesh> GetAllSubMeshAsIsolatedMeshes(Mesh mesh)
  {
      List<Mesh> meshesToReturn = new List<Mesh>();
      if ( !mesh )
      {
          Debug.LogError( "No mesh passed into GetAllSubMeshAsIsolatedMeshes!" );
          return meshesToReturn;
      }
      int submeshCount = mesh.subMeshCount;
      if ( submeshCount < 2 )
      {
          Debug.LogError( "Only " + submeshCount + " submeshes in mesh passed to GetAllSubMeshAsIsolatedMeshes" );
          return meshesToReturn;
      }
      
      Mesh m1;
      for ( int i = 0; i < submeshCount; i++ )
      {
          m1 = new Mesh();
          
          m1 = mesh.GetSubMeshByIndex( i );
          meshesToReturn.Add( m1 );
      }
      return meshesToReturn;
  }
  
  public static Mesh GetSubMeshByIndex(this Mesh aMesh, int aSubMeshIndex)
  {
      if ( aSubMeshIndex < 0 || aSubMeshIndex >= aMesh.subMeshCount )
          return null;
      int[] indices = aMesh.GetTriangles( aSubMeshIndex );
      Vertices source = new Vertices( aMesh );
      Vertices dest = new Vertices();
      Dictionary<int, int> map = new Dictionary<int, int>();
      int[] newIndices = new int[indices.Length];
      for ( int i = 0; i < indices.Length; i++ )
      {
          int o = indices[i];
          int n;
          if ( !map.TryGetValue( o, out n ) )
          {
              n = dest.Add( source, o );
              map.Add( o, n );
          }
          newIndices[i] = n;
      }
      Mesh m = new Mesh();
      dest.AssignTo( m );
      m.triangles = newIndices;
      return m;
  }
}

public class Vertices
{
     List<Vector3> verts = null;
     List<Vector2> uv1 = null;
     List<Vector2> uv2 = null;
     List<Vector2> uv3 = null;
     List<Vector2> uv4 = null;
     List<Vector3> normals = null;
     List<Vector4> tangents = null;
     List<Color32> colors = null;
     List<BoneWeight> boneWeights = null;

     public Vertices()
     {
         verts = new List<Vector3>();
     }
     
     public Vertices(Mesh aMesh)
     {
         verts = CreateList(aMesh.vertices);
         uv1 = CreateList(aMesh.uv);
         uv2 = CreateList(aMesh.uv2);
         uv3 = CreateList(aMesh.uv3);
         uv4 = CreateList(aMesh.uv4);
         normals = CreateList(aMesh.normals);
         tangents = CreateList(aMesh.tangents);
         colors = CreateList(aMesh.colors32);
         boneWeights = CreateList(aMesh.boneWeights);
     }

     private List<T> CreateList<T>(T[] aSource)
     {
         if (aSource == null || aSource.Length == 0)
             return null;
         return new List<T>(aSource);
     }
     
     private void Copy<T>(ref List<T> aDest, List<T> aSource, int aIndex)
     {
         if (aSource == null)
             return;
         if (aDest == null)
             aDest = new List<T>();
         aDest.Add(aSource[aIndex]);
     }
     
     public int Add(Vertices aOther, int aIndex)
     {
         int i = verts.Count;
         Copy(ref verts, aOther.verts, aIndex);
         Copy(ref uv1, aOther.uv1, aIndex);
         Copy(ref uv2, aOther.uv2, aIndex);
         Copy(ref uv3, aOther.uv3, aIndex);
         Copy(ref uv4, aOther.uv4, aIndex);
         Copy(ref normals, aOther.normals, aIndex);
         Copy(ref tangents, aOther.tangents, aIndex);
         Copy(ref colors, aOther.colors, aIndex);
         Copy(ref boneWeights, aOther.boneWeights, aIndex);
         return i;
     }
     
     public void AssignTo(Mesh aTarget)
     {
         if (verts.Count > 65535)
             aTarget.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
         aTarget.SetVertices(verts);
         if (uv1 != null) aTarget.SetUVs(0, uv1);
         if (uv2 != null) aTarget.SetUVs(1, uv2);
         if (uv3 != null) aTarget.SetUVs(2, uv3);
         if (uv4 != null) aTarget.SetUVs(3, uv4);
         if (normals != null) aTarget.SetNormals(normals);
         if (tangents != null) aTarget.SetTangents(tangents);
         if (colors != null) aTarget.SetColors(colors);
         if (boneWeights != null) aTarget.boneWeights = boneWeights.ToArray();
     }
}