using System;
using System.Collections;
using System.IO;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class LoadAssetCommand : ICommandWithParameters
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private ContentModel _contentModel;
        [Inject] private ICacheService _cacheService;
        [Inject] private readonly AsyncProcessorService _asyncProcessor;
        [Inject] private AssetBundleModel _assetBundleModel;

        private string assetName;
        private int _id;
        private Action<bool, int, GameObject, string> _assetLoaded;

        public void Execute(ISignal signal)
        {
            var parameter = (LoadAssetCommandSignal) signal;
            _id = parameter.ID;
            _assetLoaded = parameter.AssetLoaded;

            _asyncProcessor.StartCoroutine(LoadAssetFromBundle());
        }
        
        private IEnumerator LoadAssetFromBundle()
        {
            var selectedAsset = GetSelectedAsset(_id);
            assetName = GetAssetNameById(selectedAsset);
            Debug.Log("LoadAssetCommand => _id  = " + _id);
            Debug.Log("LoadAssetCommand => assetName  = " + assetName);

            var modelPath = _cacheService.GetPathToAsset(_id, selectedAsset.Asset.Version);
            Debug.Log("LoadAssetCommand => modelPath = " + modelPath);

            yield return new WaitForEndOfFrame();

            float timeOut = 10f;
            while (!_cacheService.IsFileExists(modelPath) && timeOut >= 0)
            {
                yield return new WaitForEndOfFrame();
                timeOut -= Time.deltaTime;
            }
            
            if (!_cacheService.IsFileExists(modelPath))
            {
                _assetLoaded.Invoke(false, _id, null, "Time Out");
                _signal.Fire(new SendCrashAnalyticsCommandSignal($"@@@ LoadAssetCommand: LoadAssetFromBundle > ID = {_id} > modelPath = {modelPath} | IsFileExists = {_cacheService.IsFileExists(modelPath)}"));

                yield break;
            }
            
            _signal.Fire(new ShowDebugLogCommandSignal($"@@@ LoadAssetCommand: LoadAssetFromBundle > ID = {_id} > modelPath = {modelPath} | IsFileExists = {_cacheService.IsFileExists(modelPath)}"));

            AssetBundle _assetBundle;

            if (_assetBundleModel.ContainsAssetBundle(assetName))
            {
                _assetBundle = _assetBundleModel.GetLoadedAssetBundle(assetName);
            }
            else
            {
               var loadFromFileAsync = AssetBundle.LoadFromFileAsync(modelPath);
                
                yield return loadFromFileAsync;
                while (!loadFromFileAsync.isDone)
                    yield return null;

                _assetBundle = loadFromFileAsync.assetBundle;
            }

            if(_assetBundle != null)
            {
                var request = _assetBundle.LoadAllAssetsAsync<GameObject>();
		    
                yield return request;
                while (!request.isDone)
                    yield return null;
		    
                GameObject prefab = request.asset as GameObject;
                _signal.Fire(new ShowDebugLogCommandSignal($"@@@ LoadAssetCommand: LoadAssetFromBundle > ID = {_id} : Asset Loaded"));
                
                if (!_assetBundleModel.ContainsAssetBundle(assetName))
                    _assetBundleModel.AddLoadedAssetBundle(assetName, _assetBundle);

                _assetLoaded.Invoke(true, _id, prefab, "Asset Loaded");
            }
            else
            {
                _signal.Fire(new SendCrashAnalyticsCommandSignal($"@@@ LoadAssetCommand: LoadAssetFromBundle > ID = {_id} : The model is not compatible with the current platform"));
                _assetLoaded.Invoke(false, _id, null, "The model is not compatible with the current platform");
            }
        }
        
        private string GetAssetNameById(ClientAssetModel assetModel)
        {
            return Path.GetFileName(assetModel.AssetDetail.AssetContentPlatform.FileUrl);
        }

        private ClientAssetModel GetSelectedAsset(int id)
        {
            return _contentModel.GetAssetById(id);
        }
    }

    public class LoadAssetCommandSignal : ISignal
    {
        public int ID { get; }
        public Action<bool, int, GameObject, string> AssetLoaded { get; }

        public LoadAssetCommandSignal(int id, Action<bool, int, GameObject, string> assetLoaded)
        {
            ID = id;
            AssetLoaded = assetLoaded;
        }
    }
}