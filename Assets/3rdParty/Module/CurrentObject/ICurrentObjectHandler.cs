using UnityEngine;

namespace Module.Bundle {
    public interface ICurrentObjectHandler {
        AssetBundle AssetBundle { get; }
        GameObject[] Instances { get; }
        Transform ModelContainer { get; }
        void Instantiate(Transform parent, string modelName, string purposeName);
        void Dispose();
        void SetActive(bool value);
    }
}