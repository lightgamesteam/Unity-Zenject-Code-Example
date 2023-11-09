using System.Collections;
using UnityEngine;
using Zenject;
using System;
using System.IO;
using TDL.Models;
using TDL.Services;

namespace TDL.Modules.Model3D
{
    public class DownloadBackgroundCommand : ICommandWithParameters
    {
        [Inject] private ICacheService _cacheService;
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private ServerService _serverService;
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly AsyncProcessorService _asyncProcessor;

        private int _id;
        private bool checkIfFileExist;
        private Action<bool, int> _assetDownloaded;
        private readonly WaitForSeconds _waitForSeconds = new WaitForSeconds(1.0f);
    
        public void Execute(ISignal signal)
        {
            var parameter = (DownloadBackgroundCommandSignal) signal;
            _id = parameter.ID;
            _assetDownloaded = parameter.AssetDownloaded;
        
            var itemModel = _contentModel.GetAssetById(_id);
            var backgroundName = Path.GetFileName(itemModel.AssetDetail.AssetContentPlatform.BackgroundUrl);
        
            if (_cacheService.IsFileExists(backgroundName))
            {
                _assetDownloaded.Invoke(true, _id);
            }
            else
            {
                var backgroundUrl = itemModel.AssetDetail.AssetContentPlatform.BackgroundUrl;

                _serverService.DownloadBackground(_id.ToString(), backgroundName, backgroundUrl);

                _asyncProcessor.StartCoroutine(IsFileDownloaded(backgroundName));
            }
        }

        private IEnumerator IsFileDownloaded(string fileName)
        {
            checkIfFileExist = true;

            while (checkIfFileExist)
            {
                yield return _waitForSeconds;

                if (_cacheService.IsFileExists(fileName))
                {
                    _assetDownloaded.Invoke(true, _id);
                    checkIfFileExist = false;
                }
            }
        }
    }

    public class DownloadBackgroundCommandSignal : ISignal
    {
        public int ID { get; }
        public Action<bool, int> AssetDownloaded { get; }

        public DownloadBackgroundCommandSignal(int id, Action<bool, int> assetDownloaded)
        {
            ID = id;
            AssetDownloaded = assetDownloaded;
        }
    }
}