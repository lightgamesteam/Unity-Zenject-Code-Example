using TDL.Models;
using TDL.Server;

public static class ClientAssetModelExtension
{
   public static bool IsEqualsIds(this ClientAssetModel a, ClientAssetModel b)
   {
      return a.Asset.Id == b.Asset.Id && a.Asset.GradeId == b.Asset.GradeId;
   }
   
   public static bool IsEqualsIds(this ClientAssetModel a, int assetId, int gradeId)
   {
      return a.Asset.Id == assetId && a.Asset.GradeId == gradeId;
   }

   public static ClientAssetModel Clone(this ClientAssetModel a)
   {
      return new ClientAssetModel
      {
         Asset = new Asset()
         {
            Id = a.Asset.Id,
            GradeId = a.Asset.GradeId,
            Name = a.Asset.Name,
            Type = a.Asset.Type,
            Version = a.Asset.Version,
            LocalizedDescription = a.Asset.LocalizedDescription,
            ThumbnailId = a.Asset.ThumbnailId,
            ThumbnailUrl = a.Asset.ThumbnailUrl,
            AssetLocal = a.Asset.AssetLocal,
            IsFavourite = a.Asset.IsFavourite,
            IsRecent = a.Asset.IsRecent,
            LessonMode = a.Asset.LessonMode,
            RecentDate = a.Asset.RecentDate,
            VimeoUrl = a.Asset.VimeoUrl
         },
         Puzzle = a.Puzzle,
         Quiz = a.Quiz,
         IsAddedToFavorites = true,
         IsRecentlyViewed = a.IsRecentlyViewed,
         HasLessonMode =  a.HasLessonMode,
         LocalizedName = a.LocalizedName,
         Categories = a.Categories,
      };
   }
}
