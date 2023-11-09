using System;
using System.Collections.Generic;
using TDL.Server;

namespace TDL.Models
{
    [Serializable]
    public class ClientAssetModel
    {
        public Dictionary<string, string> LocalizedName { get; set; } = new Dictionary<string, string>();
        public Asset Asset { get; set; }
        public AssetDetail AssetDetail { get; set; }
        public List<ActivityItem> Puzzle { get; set; }
        public List<ActivityItem> Quiz { get; set; }
        public ActivityItem Classification { get; set; }
        public bool IsAddedToFavorites { get; set; }
        public bool IsRecentlyViewed { get; set; }
        public bool HasLessonMode { get; set; }
        public bool IsQuizSelected { get; set; }
        public bool IsPuzzleSelected { get; set; }
        public bool IsMultipleQuizSelected { get; set; }
        public bool IsMultiplePuzzleSelected { get; set; }
        public bool IsClassificationSelected { get; set; }
        public int SelectedQuizItemId { get; set; }
        public int SelectedPuzzleItemId { get; set; }

        public List<Dictionary<ContentModel.CategoryNames, int>> Categories { get; set; } = new List<Dictionary<ContentModel.CategoryNames, int>>();
    }
}