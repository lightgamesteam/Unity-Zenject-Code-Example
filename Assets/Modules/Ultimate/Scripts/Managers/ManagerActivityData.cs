using System.Collections.Generic;
using System.Linq;
using TDL.Models;
using TDL.Modules.Ultimate.Core.ActivityData;
using TDL.Server;
using TDL.Services;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace TDL.Modules.Ultimate.Core.Managers {
    public class ManagerActivityData {
        [Inject] private readonly ContentModel _contentModel = default;
        [Inject] private readonly ICacheService _cacheService = default;
        
        public string FileHash { get; private set; }
        public LocalName[] ActivityLocal { get; private set; }
        public GroupData[] GroupDataArray { get; private set; }

        [Inject]
        private void Construct() {
            var selectedClientAsset = _contentModel.SelectedAsset;
            FileHash = _cacheService.GetPathToAsset(selectedClientAsset.Asset.Id, selectedClientAsset.Asset.Version);
            Assert.IsTrue(!string.IsNullOrEmpty(FileHash), GetAssetMessage("File is null or empty"));
            ActivityLocal = selectedClientAsset.Asset.AssetLocal.ToArray();
            GroupDataArray = GetAssetGroupDataArray(selectedClientAsset.AssetDetail.AssetContentPlatform.Layers);
        }

        private static GroupData[] GetAssetGroupDataArray(IEnumerable<Layers> layersArray) {
            return layersArray.Select(layers => new GroupData { LayerData = GetLayerData(layers), LabelDataArray = GetLabelDataArray(layers.Labels) }).ToArray();
        }

        private static LayerData GetLayerData(Layers layers) {
            return new LayerData {
                Id = layers.Id,
                GoName = layers.Name,
                LocalNames = layers.LayerLocal
            };
        }

        private static ActivityData.LabelData[] GetLabelDataArray(IEnumerable<Assetlabel> labels) {
            return labels == null ? new ActivityData.LabelData[0] : labels.Select(GetLabelData).ToArray();
        }
        
        private static ActivityData.LabelData GetLabelData(Assetlabel label) {
            var result = new ActivityData.LabelData {
                Id = label.labelId,
                GoName = "",
                LocalNames = label.labelLocal.ConvertToLocalName(),
                Position = new Vector3(label.position.x, label.position.y, label.position.z),
                Rotation = Quaternion.Euler(label.rotation.x, label.rotation.y, label.rotation.z),
                Scale = new Vector3(label.scale.x, label.scale.y, label.scale.z),
                LabelHotSpot = label.labelHotSpot,
                PartOrder = label.partOrder
            };
            ColorUtility.TryParseHtmlString(label.highlightColor, out result.HighlightColor);
            return result;
        }
        
        private string GetAssetMessage(string message) {
            var selectedClientAsset = _contentModel.SelectedAsset;
            var selectedClientAssetId = selectedClientAsset.Asset.Id;
            var selectedClientAssetVersion = selectedClientAsset.Asset.Version;
            return $"[Id:{selectedClientAssetId}, Ver:{selectedClientAssetVersion}] - {message}";
        }
    }
}