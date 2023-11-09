using System.Collections.Generic;

namespace TDL.Server
{
    public class ActivityModel
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public List<ActivityItem> ActivityItem { get; set; }
    }

    public class ActivityItem
    {
        public int itemId { get; set; }
        public string itemName { get; set; }
        public int duration { get; set; }
        public bool isModelSpecific { get; set; }
        public List<ActivityAssetcontent> assetContent { get; set; }
        public List<ActivityDetail> ActivityDetails { get; set; }
        public LocalName[] activityLocal { get; set; }
    }

    public class ActivityAssetcontent
    {
        public int itemDetailId { get; set; }
        public int assetId { get; set; }
        public int numberOfAttempts { get; set; }
        public int numberOfLabels { get; set; }
        public int assetContentId { get; set; }
        public Quizlabel[] quizLabels { get; set; }
    }

    public class Quizlabel
    {
        public int labelId { get; set; }
        public string labelName { get; set; }
    }    
    
    public class ActivityDetail
    {
        public int PackageId { get; set; }
        public int LanguageId { get; set; }
        public int GradeId { get; set; }
        public int SyllabusId { get; set; }
        public int StreamId { get; set; }
        public int SubjectId { get; set; }
        public int TopicId { get; set; }
        public int SubtopicId { get; set; }
    }
}