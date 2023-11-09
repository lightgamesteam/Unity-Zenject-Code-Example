using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Module.PeriodicTable
{
    public static class PoolManager
    {
        private static PoolPart[] pools;
        private static GameObject objectsParent;

        [System.Serializable]
        public struct PoolPart
        {
            public string name;
            public PoolObject prefab;
            public int count;
            public ObjectPooling ferula;
        }

        public static void Initialize(PoolPart[] newPools)
        {
            pools = newPools;
            objectsParent = new GameObject();
            objectsParent.name = "Pool";
            objectsParent.transform.SetParent(PeriodicTableView.Transform, false);
            
            for (int i = 0; i < pools.Length; i++)
            {
                if (pools[i].prefab != null)
                {
                    pools[i].ferula = new ObjectPooling();
                    pools[i].ferula.Initialize(pools[i].count, pools[i].prefab, objectsParent.transform);
                }
            }
        }

        public static GameObject GetObject(string name, GameObject parent)
        {
            GameObject result = null;
            if (pools != null)
            {
                for (int i = 0; i < pools.Length; i++)
                {
                    if (string.Compare(pools[i].name, name) == 0)
                    {
                        result = pools[i].ferula.GetObject().gameObject;
                        result.transform.SetParent(objectsParent.transform);
                        result.transform.SetParent(parent.transform);
                        result.transform.position = parent.transform.position;
                        result.transform.localScale = Vector3.one;
                        result.SetActive(true);
                        return result;
                    }
                }
            }

            return result;
        }

        public static GameObject GetObject(string name, Vector3 position, Quaternion rotation)
        {
            GameObject result = null;
            if (pools != null)
            {
                for (int i = 0; i < pools.Length; i++)
                {
                    if (string.Compare(pools[i].name, name) == 0)
                    {
                        result = pools[i].ferula.GetObject().gameObject;
                        result.transform.position = position;
                        result.transform.rotation = rotation;
                        result.SetActive(true);
                        return result;
                    }
                }
            }

            return result;
        }
    }
}