using System.Collections.Generic;
using UnityEngine;

public class AssetBundleModel
{
    private Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();

    public bool ContainsAssetBundle(string _assetBundleName)
    {
        return assetBundles.ContainsKey(_assetBundleName);
    }

    public void AddLoadedAssetBundle(string _assetBundleName, AssetBundle _assetBundle)
    {
        if(!ContainsAssetBundle(_assetBundleName))
            assetBundles.Add(_assetBundleName, _assetBundle);
    }
    
    public AssetBundle GetLoadedAssetBundle(string _assetBundleName)
    {
        if (ContainsAssetBundle(_assetBundleName))
            return assetBundles[_assetBundleName];

        return null;
    }

    public void UnloadAssetBundle(string assetName)
    {
        AssetBundle.UnloadAllAssetBundles(assetBundles[assetName]);
    }
    public void UnloadAllAssetBundles()
    {
        foreach (var assetBundle in assetBundles)
        {
            assetBundle.Value.Unload(true);
            //AssetBundle.UnloadAllAssetBundles(assetBundle.Value);
        }
    }
}
