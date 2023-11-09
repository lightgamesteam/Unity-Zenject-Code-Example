using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Module.Bundle {
    public class CurrentObjectController : ICurrentObjectHandler {
        public readonly CurrentObjectData Data;
        
        private const string _assetPrefix = "_prefixID_";

        private CurrentObjectController(CurrentObjectData data) {
            Data = data;
        }

        public static ICurrentObjectHandler ConnectCurrentObjectHandler(string fileHash) {
            var data = new CurrentObjectData();
            try {
                if (fileHash == null || !File.Exists(fileHash)) {
                    throw new Exception("3D Model file [" + fileHash + "] does not exist.");
                }

                var fileName = Path.GetFileName(fileHash);
                var fileExtension = Path.GetExtension(fileName);

                if (fileName.Contains(_assetPrefix))
                {
                    fileName = fileName.Substring(0, fileName.IndexOf(_assetPrefix, StringComparison.Ordinal)) + fileExtension;
                }
                
                var underscoreIndex = fileName.LastIndexOf('_');
                var fileShortName = underscoreIndex > 0 ? fileName.Substring(0, underscoreIndex) : string.Empty;
                foreach (var allLoadedAssetBundle in AssetBundle.GetAllLoadedAssetBundles()) {
                    if (allLoadedAssetBundle.name.Equals(fileName)) {
                        data.AssetBundle = allLoadedAssetBundle;
                        break;
                    }
                    var bundleName = Path.GetFileNameWithoutExtension(allLoadedAssetBundle.name);
                    var bundleExtension = Path.GetExtension(allLoadedAssetBundle.name);
                    if (bundleExtension.Equals(fileExtension) 
                        && !string.IsNullOrEmpty(fileShortName)
                        && bundleName.Equals(fileShortName)) {
                        data.AssetBundle = allLoadedAssetBundle;
                        break;
                    }
                }
                if (data.AssetBundle == null) {
                    data.AssetBundle = AssetBundle.LoadFromFile(fileHash);
                }
            } catch (Exception exception) {
                Debug.LogError("3D Model file [" + fileHash + "] - " + exception.Message);
            }
            return new CurrentObjectController(data);
        }

        public static ICurrentObjectHandler ConnectCurrentObjectHandler(AssetBundle assetBundle) {
            var data = new CurrentObjectData();
            if (assetBundle != null) {
                data.AssetBundle = assetBundle;
            } else {
                Debug.LogError("3D Model 'AssetBundle' is null");
            }
            return new CurrentObjectController(data);
        }

        #region ICurrentObjectHandler

        AssetBundle ICurrentObjectHandler.AssetBundle => Data.AssetBundle;
        GameObject[] ICurrentObjectHandler.Instances => Data.Instances.ToArray();
        Transform ICurrentObjectHandler.ModelContainer => Data.ModelContainer.transform;

        void ICurrentObjectHandler.Instantiate(Transform parent, string modelName, string purposeName) {
            try {
                if (Data.AssetBundle == null) {
                    throw new Exception("3D Model [AssetBundle] is null.");
                }

                Data.ModelContainer = new GameObject($"Model: {modelName} - {purposeName}");
                Data.ModelContainer.transform.SetParent(parent);
                Data.ModelContainer.transform.localPosition = Vector3.zero;

                // Load all assets in asset bundle
                var prefabs = Data.AssetBundle.LoadAllAssets<GameObject>();
                Data.Instances = new List<GameObject>();
                foreach (var prefab in prefabs) {
                    // Create instance of prefab
                    var instance = UnityEngine.Object.Instantiate(prefab);
                    Data.Instances.Add(instance);

                    // Place and rotate model
                    instance.transform.SetParent(Data.ModelContainer.transform);
                    //instance.transform.localPosition = Vector3.zero;
                    Data.ModelContainer.transform.rotation = instance.transform.rotation;
                    instance.transform.localRotation = Quaternion.identity;
                    ManagerMaterials.ManagerMaterials.Instance?.ReplaceMaterials(instance);
                }
                Data.AssetBundle.Unload(false);
            } catch (Exception exception) {
                Debug.LogError("3D Model [Instantiate] - " + exception.Message);
            }
        }

        void ICurrentObjectHandler.Dispose() {
            if (Data == null) {
                return;
            }
            // Clean up all instances we have made under our root object
            if (Data.ModelContainer != null) {
                UnityEngine.Object.Destroy(Data.ModelContainer);
            }
            // Unload asset bundle
            if (Data.AssetBundle != null) {
                Data.AssetBundle.Unload(true);
            }
        }

        void ICurrentObjectHandler.SetActive(bool value) {
            Data.ModelContainer.SetActive(value);
        }

        #endregion
    }
}