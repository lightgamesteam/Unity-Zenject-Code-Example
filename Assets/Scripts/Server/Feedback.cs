using System;
using System.Collections.Generic;

namespace TDL.Server
{
    public class FeedbackRequest
    {
        public int AssetFileId { get; set; }
        public string Feedback { get; set; }      
        public int? GradeId { get; set; }        
        public int? SubjectId { get; set; }
        public int? TopicId { get; set; }
        public int? SubtopicId { get; set; }
        public string Status { get; set; }
    }

    public class FeedbackAsset
    {
        public int FeedbackId { get; set; }
        public string Feedback { get; set; }
        public int AssetFileId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public int? GradeId { get; set; }
        public string Grade { get; set; }
        public int? SubjectId { get; set; }
        public string Subject { get; set; }
        public int? TopicId { get; set; }
        public string Topic { get; set; }
        public int? SubtopicId { get; set; }
        public string Subtopic { get; set; }
        public int? StatusId { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }

    public class FeedbackResponse : ResponseBase
    {
        public List<FeedbackAsset> Feedback { get; set; }
    }
}