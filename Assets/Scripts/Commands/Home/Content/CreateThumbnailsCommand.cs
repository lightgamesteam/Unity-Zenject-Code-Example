using System.Collections.Generic;
using System.IO;
using TDL.Models;
using TDL.Services;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class CreateThumbnailsCommand : ICommand
    {
        [Inject] protected ContentModel _contentModel;
        [Inject] protected UserContentAppModel _userContentAppModel;
        [Inject] protected HomeModel _homeModel;
        [Inject] protected ICacheService _cacheService;
        [Inject] protected ServerService _serverService;

        public virtual void Execute()
        {
            ClearThumbnails();

            if (DeviceInfo.IsMobile())
            {
                CreateThumbnailsForGrades();
                CreateThumbnailsForSubjects();
            }

            CreateThumbnailsForTopics();
            CreateThumbnailsForSubtopics();
            CreateThumbnailsForAssets();
            CreateThumbnailsForQuizAssets();
            CreateThumbnailsForPuzzleAssets();
            CreateThumbnailsForUserContents();
        }

        private void CreateThumbnailsForUserContents()
        {
            _homeModel.ShownUserContentOnHome.ForEach(uc =>
            {
                var itemModel = _userContentAppModel.GetUserContentById(uc.Id);
                var thumbnailName = GetThumbnailName(itemModel.ThumbnailUrl);

                if (!string.IsNullOrEmpty(thumbnailName))
                {
                    if (IsFileAlreadyInCache(thumbnailName))
                    {
                        uc.SetThumbnail(CreateThumbnailFromCache(thumbnailName));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(itemModel.ThumbnailUrl))
                        {
                            DownloadThumbnailFromServer(uc.Id.ToString(), thumbnailName, itemModel.ThumbnailUrl, uc);
                        }
                    }
                }
            });
        }

        protected virtual void ClearThumbnails()
        {
            _homeModel.ThumbnailsToDownload.Clear();
        }

        public void CreateThumbnailsForGrades()
        {
            foreach (var itemView in _homeModel.ShownGradeOnHome)
            {
                var itemModel = _contentModel.GetGradeById(itemView.Id);
                var thumbnailName = GetThumbnailName(itemModel.Grade.ThumbnailUrl);

                if (!string.IsNullOrEmpty(thumbnailName))
                {
                    if (IsFileAlreadyInCache(thumbnailName))
                    {
                        itemView.SetThumbnail(CreateThumbnailFromCache(thumbnailName));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(itemModel.Grade.ThumbnailUrl))
                        {
                            DownloadThumbnailFromServer(itemView.Id.ToString(), thumbnailName,
                                itemModel.Grade.ThumbnailUrl, itemView);
                        }
                    }
                }
            }
        }

        public void CreateThumbnailsForSubjects()
        {
            foreach (var itemView in _homeModel.ShownSubjectOnHome)
            {
                var itemModel = _contentModel.GetClientSubjectById(itemView.Id);
                var thumbnailName = GetThumbnailName(itemModel.Subject.ThumbnailUrl);

                if (!string.IsNullOrEmpty(thumbnailName))
                {
                    if (IsFileAlreadyInCache(thumbnailName))
                    {
                        itemView.SetThumbnail(CreateThumbnailFromCache(thumbnailName));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(itemModel.Subject.ThumbnailUrl))
                        {
                            DownloadThumbnailFromServer(itemView.Id.ToString(), thumbnailName, itemModel.Subject.ThumbnailUrl, itemView);
                        }
                    }
                }
            }
        }

        private void CreateThumbnailsForTopics()
        {
            foreach (var itemView in _homeModel.ShownTopicsOnHome)
            {
                var itemModel = _contentModel.GetTopicById(itemView.Id);
                var thumbnailName = GetThumbnailName(itemModel.Topic.ThumbnailUrl);

                if (!string.IsNullOrEmpty(thumbnailName))
                {
                    if (IsFileAlreadyInCache(thumbnailName))
                    {
                        itemView.SetThumbnail(CreateThumbnailFromCache(thumbnailName));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(itemModel.Topic.ThumbnailUrl))
                        {
                            DownloadThumbnailFromServer(itemView.Id.ToString(), thumbnailName, itemModel.Topic.ThumbnailUrl, itemView);
                        }
                    }
                }
            }
        }

        private void CreateThumbnailsForSubtopics()
        {
            foreach (var itemView in _homeModel.ShownSubtopicsOnHome)
            {
                var itemModel = _contentModel.GetSubtopicById(itemView.Id);
                var thumbnailName = GetThumbnailName(itemModel.Subtopic.ThumbnailUrl);

                if (!string.IsNullOrEmpty(thumbnailName))
                {
                    if (IsFileAlreadyInCache(thumbnailName))
                    {
                        itemView.SetThumbnail(CreateThumbnailFromCache(thumbnailName));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(itemModel.Subtopic.ThumbnailUrl))
                        {
                            DownloadThumbnailFromServer(itemView.Id.ToString(), thumbnailName, itemModel.Subtopic.ThumbnailUrl, itemView);
                        }
                    }
                }
            }
        }

        private void CreateThumbnailsForAssets()
        {
            foreach (var asset in _homeModel.ShownAssetsOnHome)
            {
                DownloadAssetThumbnail(asset.AssetId, asset);
            }
        }

        private void CreateThumbnailsForQuizAssets()
        {
            foreach (var asset in _homeModel.ShownQuizAssetsOnHome)
            {
                DownloadAssetThumbnail(asset.Id, asset);
            }
        }

        private void CreateThumbnailsForPuzzleAssets()
        {
            foreach (var asset in _homeModel.ShownPuzzleAssetsOnHome)
            {
                DownloadAssetThumbnail(asset.AssetId, asset);
            }
        }

        protected void DownloadAssetThumbnail(int id, IThumbnailView itemView)
        {
            var itemModel = _contentModel.GetAssetById(id);
            var thumbnailName = GetThumbnailName(itemModel.Asset.ThumbnailUrl);

            if (!string.IsNullOrEmpty(thumbnailName))
            {
                if (IsFileAlreadyInCache(thumbnailName))
                {
                    itemView.SetThumbnail(CreateThumbnailFromCache(thumbnailName));
                }
                else
                {
                    var thumbnailUrl = itemModel.Asset.ThumbnailUrl;
                    if (!string.IsNullOrEmpty(thumbnailUrl))
                    {
                        DownloadThumbnailFromServer(id.ToString(), thumbnailName, thumbnailUrl, itemView);
                    }
                }
            }
        }

        private bool IsFileAlreadyInCache(string fileName)
        {
            return _cacheService.IsFileExists(fileName);
        }

        private Texture2D CreateThumbnailFromCache(string fileName)
        {
            var thumbnailPath = _cacheService.GetPathToFile(fileName);
            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(File.ReadAllBytes(thumbnailPath));

            _contentModel.CachedTextures.Add(texture2D);
            return texture2D;
        }

        private void DownloadThumbnailFromServer(string thumbnailId, string thumbnailName, string thumbnailUrl, IThumbnailView itemView)
        {
            if (_homeModel.ThumbnailsToDownload.ContainsKey(thumbnailId))
            {
                var thumbnail = _homeModel.ThumbnailsToDownload[thumbnailId];
                thumbnail.Add(itemView);
            }
            else
            {
                _serverService.DownloadThumbnail(thumbnailId, thumbnailName, thumbnailUrl);
                _homeModel.ThumbnailsToDownload.Add(thumbnailId, new List<IThumbnailView> {itemView});
            }
        }

        private string GetThumbnailName(string url)
        {
            return Path.GetFileName(url);
        }
    }
}