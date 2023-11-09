using System;
using Newtonsoft.Json;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class UpdateRecentAssetCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
            var parameter = (UpdateRecentAssetCommandSignal) signal;
            var recentlyAddedAssetsResponse = JsonConvert.DeserializeObject<ResponseBase>(parameter.Response);

            if (recentlyAddedAssetsResponse.Success)
            {
                var asset = _contentModel.GetAssetById(parameter.AssetId);
                
                var recAsset = new ClientAssetModel
                {
                    Puzzle = asset.Puzzle,
                    Quiz = asset.Quiz,
                    IsAddedToFavorites = asset.Asset.IsFavourite,
                    IsRecentlyViewed = true,
                    HasLessonMode =  asset.Asset.LessonMode,
                    LocalizedName = asset.LocalizedName,
                    Categories = asset.Categories,
                };

                recAsset.Asset = new Asset()
                {
                    Id = parameter.AssetId,
                    GradeId  = parameter.GradeId,
                    Name = asset.Asset.Name, 
                    Type  = asset.Asset.Type, 
                    Version  = asset.Asset.Version, 
                    LocalizedDescription  = asset.Asset.LocalizedDescription, 
                    ThumbnailId  = asset.Asset.ThumbnailId, 
                    ThumbnailUrl  = asset.Asset.ThumbnailUrl, 
                    AssetLocal  = asset.Asset.AssetLocal, 
                    IsFavourite  = asset.Asset.IsFavourite, 
                    IsRecent  = true, 
                    LessonMode   = asset.Asset.LessonMode, 
                    RecentDate  = DateTime.UtcNow, 
                    VimeoUrl  = asset.Asset.VimeoUrl
                };
                
                recAsset.Asset.RecentDate = DateTime.UtcNow;
                
                _contentModel.RecentAssets.RemoveAll(a => a.IsEqualsIds(parameter.AssetId, parameter.GradeId));
                _contentModel.RecentAssets.Add(recAsset);
            }
            else
            {
                _signal.Fire(new PopupOverlaySignal(false));
                _signal.Fire(new SendCrashAnalyticsCommandSignal("UpdateRecentAssetCommand server response | " + recentlyAddedAssetsResponse.ErrorMessage));
            }
        }
    }
}