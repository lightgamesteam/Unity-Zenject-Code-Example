using System.Collections.Generic;
using JetBrains.Annotations;

namespace TDL.Server
{
     public class AssetDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AssetFile[] AssetContents { get; set; }
        public AssetFile AssetContentPlatform { get; set; }
        public AssociatedAsset[] AssociatedContents { get; set; }
        public List<TagContent> Tags { get; set; }
    }

    public class AssetFile
    {
        public int Id { get; set; }
        public int PlatformId { get; set; }
        public string Platform { get; set; }
        public int Version { get; set; }
        public string fileNameId { get; set; }
        public string FileUrl { get; set; }
        public int FileSize { get; set; }
        public string ThumbnailId { get; set; }
        public string ThumbnailUrl { get; set; }
        public string BackgroundId { get; set; }
        public string BackgroundUrl { get; set; }
        public Assetlabel[] assetLabel { get; set; }
        public Layers[] Layers { get; set; }
    }

    public class AssociatedAsset
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public int ModelId { get; set; }
        public string Name { get; set; }
        public string VimeoUrl { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public int FileSize { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Background { get; set; }
        public string BackgroundUrl { get; set; }
    }

    public class Assetlabel
    {
        public Position position { get; set; }
        public Rotation rotation { get; set; }
        public Scale scale { get; set; }
        public int labelId { get; set; }
        public string highlightColor { get; set; }
        public LabelLocalName[] labelLocal { get; set; }
        public LocalName[] modelLocal { get; set; }
        public AssociatedAsset[] labelHotSpot { get; set; }
        public int partOrder { get; set; } 
    }

    public class Layers
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public LocalName[] LayerLocal { get; set; }
        public Assetlabel[] Labels { get; set; }
    }

    public class Position
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Rotation
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Scale
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}