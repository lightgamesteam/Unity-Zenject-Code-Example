using TDL.Commands;
using TDL.Models;
using UnityEngine;
using Zenject;

namespace Commands
{
    public class CreateThumbnailsForAllPreviewCommand : CreateThumbnailsCommand
    {
        [Inject] private MainScreenModel _model;
        
        public override void Execute()
        {
            ClearThumbnails();
            LoadThumbnailsForPreview();
        }

        protected override void ClearThumbnails()
        {
            _model.ThumbnailsToDownload.Clear();
        }

        private void LoadThumbnailsForPreview()
        {
            foreach (var a in _model.PreviewAssets)
            {
                DownloadAssetThumbnail(a.AssetId, a);
            }
        }
    }
}