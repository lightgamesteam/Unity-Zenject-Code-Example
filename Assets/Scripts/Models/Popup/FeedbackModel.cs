using System;

namespace TDL.Models
{
    public class FeedbackModel
    {
        public enum FeedbackType
        {
            Home,
            Module
        }
    
        public Action<bool> OnShowMainFeedback;
        public Action<bool> OnShowSentFeedback;

        public int AssetId { get; set; }
        public int AssetContentId { get; set; }
        public string Title { get; set; }
        public FeedbackType Type { get; set; }
        public FeedbackRequest FeedbackData { get; set; }

        private bool _isMainFeedbackActive;
        public bool IsMainFeedbackActive
        {
            get => _isMainFeedbackActive;
            set
            {
                if (_isMainFeedbackActive == value) return;
                _isMainFeedbackActive = value;
                OnShowMainFeedback?.Invoke(_isMainFeedbackActive);
            }
        }
    
        private bool _isSentFeedbackActive;
        public bool IsSentFeedbackActive
        {
            get => _isSentFeedbackActive;
            set
            {
                if (_isSentFeedbackActive == value) return;
                _isSentFeedbackActive = value;
                OnShowSentFeedback?.Invoke(_isSentFeedbackActive);
            }
        }
    
        public struct FeedbackRequest
        {
            public string AssetContentId;
            public string GradeId;
            public string SubjectId;
            public string TopicId;
            public string SubtopicId;
            public string Message;
            public string Status;
        }
    }
}