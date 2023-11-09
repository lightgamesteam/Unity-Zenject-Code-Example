using System;
using System.Collections.Generic;

namespace TDL.Server
{
    public class ContentRoot
    {
        public List<Grade> Grades { get; set; }
        public List<Module> Modules { get; set; }
        public virtual List<ActivityModel> Activities { get; set; }
        public virtual List<TagEntity> Tags { get; set; } = new List<TagEntity>();
    }

    public class Grade
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Subject> Subjects { get; set; }
        public string ThumbnailId { get; set; }
        public string ThumbnailUrl { get; set; }
        public LocalName[] GradeLocal { get; set; }
    }

    public class Subject
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Asset> Assets { get; set; }
        public string ThumbnailId { get; set; }
        public string ThumbnailUrl { get; set; }
        public LocalName[] SubjectLocal { get; set; }
    }

    public class Topic
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Subtopic> Subtopics { get; set; }
        public List<Asset> Assets { get; set; }
        public string ThumbnailId { get; set; }
        public string ThumbnailUrl { get; set; }
        public LocalName[] TopicLocal { get; set; }
        public Dictionary<string, int> RecursiveTypeCount  { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    }

    public class Subtopic
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Asset> Assets { get; set; }
        public string ThumbnailId { get; set; }
        public string ThumbnailUrl { get; set; }
        public LocalName[] SubtopicLocal { get; set; }
        public Dictionary<string, int> RecursiveTypeCount  { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    }

    [Serializable]
    public class Asset
    {
        public int Id { get; set; }
        public int GradeId  { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
        public AssetLocalDesc[] LocalizedStudentDescription { get; set; }
        public AssetLocalDesc[] LocalizedDescription { get; set; }
        public string ThumbnailId { get; set; }
        public string ThumbnailUrl { get; set; }
        public List<LocalName> AssetLocal { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsRecent { get; set; }
        public bool LessonMode  { get; set; }
        public DateTime RecentDate { get; set; }
        public string VimeoUrl { get; set; }
    }

    public class LocalName
    {
        public string Culture { get; set; }
        public string Name { get; set; }
    }
    
    public class AssetLocalDesc
    {
        public int AssetDescId { get; set; }
        public int AssetFileDescId { get; set; }
        public string Culture { get; set; }
        public string DescriptionUrl { get; set; }
        
        public string AudioFileUrl { get; set; }
    }

    public class LabelLocalName
    {
        public int Id { get; set; }
        public string Culture { get; set; }
        public string Name { get; set; }
        public string DescriptionUrl { get; set; }
        
        public string AudioFileUrl { get; set; }
    }

    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public int Size { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsRecent { get; set; }
        public LocalName[] AssetLocal { get; set; }
    }

    public class TagEntity
    {
        public int id { get; set; }
        public LocalName[] names { get; set; }
    }

    public class TagContent
    {
        public int id { get; set; }
    }

    public class ContentResponse : ResponseBase
    {
        public ContentRoot ContentRoot { get; set; }
    }

    public class UserContentModel : UserContentBaseModel
    {
        public int UserId { get; set; }
        public string ContentTypeName { get; set; }
        public string AssetName { get; set; }
        public string SubjectName { get; set; }
        public string TopicName { get; set; }
        public string SubtopicName { get; set; }
        public int ContentFileId { get; set; }
        public string ContentFile { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int ContentRequestStatus { get; set; }
        public string FileUrl { get; set; }
        public string ThumbnailUrl { get; set; }
    }

    public class UserAssignedContentModel : UserContentBaseModel
    {
        public int UserId { get; set; }
        public string ContentTypeName { get; set; }
        public string AssetName { get; set; }
        public List<LocalName> SubjectLocal { get; set; }
        public List<LocalName> TopicLocal { get; set; }
        public List<LocalName> SubtopicLocal { get; set; }
        public int ContentFileId { get; set; }
        public string ContentFile { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool ShouldSerializeContentRequestStatus() { return false; }
        public int ContentRequestStatus { get; set; }
        public string FileUrl { get; set; }
        public string ThumbnailUrl { get; set; }
    }

    public class TeacherResponse : ResponseBase
    {
        public UserAssignedContentModel[] UserAssignedContent;
    }
    
    public class UserContentBaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContentTypeId { get; set; }
        public int? AssetId { get; set; }
        public int? SubjectId { get; set; }
        public int? TopicId { get; set; }
        public int? SubtopicId { get; set; }
    }
    
    public class TermTextResponse : ResponseBase
    {
        public string PolictAndTerm;
    }
}