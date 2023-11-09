using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using System.Collections.Specialized;
using System.Linq;
using BestHTTP;
using TDL.Models;
using TDL.Services;
using TDL.Signals;

namespace TDL.Commands
{
    public class DownloadAssetCommand : ICommandWithParameters
    {
        [Inject] private ICacheService _cacheService;
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly AsyncProcessorService _asyncProcessor;

        private Action<bool, int, HTTPRequestStates, float> _assetDownloaded;
        private float _downloadProgress;
        private Dictionary<int, bool> inProgress = new Dictionary<int, bool>();
        
        public void Execute(ISignal signal)
        {
            var parameter = (DownloadAssetCommandSignal) signal;
            _assetDownloaded = parameter.AssetDownloaded;
            inProgress[parameter.ID] = false;

            if (IsAssetAlreadyDownloaded(parameter.ID))
            {
                _assetDownloaded.Invoke(true, parameter.ID, HTTPRequestStates.Finished, 100);
            }
            else
            {
                _homeModel.AssetsToDownload.CollectionChanged += OnAssetsToDownloadChanged;
                StartDownloadIfNotInProgress(parameter.ID);
            }
        }

        private void OnAssetsToDownloadChanged(object sender, NotifyCollectionChangedEventArgs changeEvent)
        {
            if (changeEvent.Action == NotifyCollectionChangedAction.Replace)
            {
                HomeModel.DownloadAsset downloadItem = _homeModel.AssetsToDownload[changeEvent.NewStartingIndex];

                DownloadState(downloadItem);
            }
        }

        private void DownloadState(HomeModel.DownloadAsset downloadAsset)
        {
            switch (downloadAsset.ItemRequest.State)
            {
                case HTTPRequestStates.Processing:
                    _downloadProgress = downloadAsset.Progress;
                    Debug.Log(_downloadProgress);
                    _assetDownloaded.Invoke(false, downloadAsset.Id, downloadAsset.ItemRequest.State, _downloadProgress);
                    
                    if ((int) downloadAsset.Progress > 50 && inProgress.ContainsKey(downloadAsset.Id))
                    {
                        if(!inProgress[downloadAsset.Id])
                            _asyncProcessor.StartCoroutine(IsFileDownloaded(downloadAsset));
                    }
                    
                    break;
                
                case HTTPRequestStates.Finished:
                    
                    DownloadFinished(downloadAsset);

                    break;
                
                case HTTPRequestStates.Error:
                    _assetDownloaded.Invoke(false, downloadAsset.Id, downloadAsset.ItemRequest.State, _downloadProgress);

                    _homeModel.AssetsToDownload.CollectionChanged -= OnAssetsToDownloadChanged;

                    break;
                
                case HTTPRequestStates.ConnectionTimedOut:
                    _assetDownloaded.Invoke(false, downloadAsset.Id, downloadAsset.ItemRequest.State, _downloadProgress);

                    _homeModel.AssetsToDownload.CollectionChanged -= OnAssetsToDownloadChanged;

                    break;
            }
        }

        private void DownloadFinished(HomeModel.DownloadAsset downloadAsset)
        {
            if (inProgress.ContainsKey(downloadAsset.Id))
            {
                inProgress.Remove(downloadAsset.Id);
                _assetDownloaded.Invoke(true, downloadAsset.Id, HTTPRequestStates.Finished, 100);
                _homeModel.AssetsToDownload.CollectionChanged -= OnAssetsToDownloadChanged;
            }
        }

        // Helper
        private IEnumerator IsFileDownloaded(HomeModel.DownloadAsset downloadAsset)
        {
            inProgress[downloadAsset.Id] = true;

            while (inProgress.ContainsKey(downloadAsset.Id))
            {
                yield return new WaitForFixedUpdate();
                
                var modelPath = _cacheService.GetPathToAsset(downloadAsset.Id, downloadAsset.Version);

                if (_cacheService.IsFileExists(modelPath))
                {
                    DownloadFinished(downloadAsset);
                }
            }
        }
        
        private bool IsAssetAlreadyDownloaded(int assetId)
        {
            var assetModel = _contentModel.GetAssetById(assetId);
            return _cacheService.IsAssetExistsAndDeleteOldVersion(assetModel.Asset.Id, assetModel.Asset.Version);
        }

        private void StartDownloadIfNotInProgress(int assetId)
        {
            if (!IsAssetAlreadyDownloading(assetId))
            {
                _contentModel.AssetDetailsSignalSource = new StartDownloadAssetCommandSignal(assetId, false);
                _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {assetId}));
            }
        }
        
        private bool IsAssetAlreadyDownloading(int assetId)
        {
            return _homeModel.AssetsToDownload.Any(item => item.Id == assetId);
        }
    }

    public class DownloadAssetCommandSignal : ISignal
    {
        public int ID { get; }
        public Action<bool, int, HTTPRequestStates, float> AssetDownloaded { get; }

        public DownloadAssetCommandSignal(int id, Action<bool, int, HTTPRequestStates, float> assetDownloaded)
        {
            ID = id;
            AssetDownloaded = assetDownloaded;
        }
    }
}