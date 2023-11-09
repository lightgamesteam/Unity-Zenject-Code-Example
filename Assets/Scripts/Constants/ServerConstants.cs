
namespace TDL.Constants
{
    public static class ServerConstants
    {
        public const string UserName = "userName";
        public const string UserPassword = "password";
        public const string ClientPlatform = "platform";
        public const string ClientVersion = "clientVersion";
        public const string AuthorizationToken = "Authorization";
        public const string AssetIdDetailsQuery = "assetId=";
        public const string ActivityIdQuery = "activityId=";
        public const string ClassificationIdQuery = "classificationId=";
        public const string SearchQuery = "searchTerm=";
        public const string SearchLanguageQuery = "languageCode=";
        public const string AssetIdField = "AssetID";
        public const string GradeIdField = "GradeID";
        public const string DeviceIdentifierField = "deviceIdentifier";
        public const string DeviceIdentifierQuery = "deviceIdentifier=";
        public const string LanguageQuery = "lang=";
        public const string LanguageCodeQuery = "languageCode=";
        
        // user content
        public const string IdPath = "id/";
        public const string ContentTypeKey = "Content-Type";
        public const string ContentTypeMultipart = "multipart/form-data";
        public const string AcceptJsonKey = "accept";
        public const string AcceptJsonValue = "application/json";
        public const string AssetContentTypeKey = "contenttypeid";
        public const string AssetIdKey = "assetid";
        public const string AssetFileAttachmentKey = "file";
        public const string FileNameQuery = "fileName=";
        public const string GradeIdQuery = "gradeId=";
        public const string AssetFileAttachmentType_ImagePNG = "image/png";
        public const string AssetFileAttachmentType_VideoMP4 = "video/mp4";
        public const string AssetFileAttachmentType_VideoWEBM = "video/webm";
        
        // feedback
        public const string FeedbackContentIdField = "assetFileId";
        public const string FeedbackMessageField = "feedback";
        public const string FeedbackGradeIdField = "gradeId";
        public const string FeedbackSubjectIdField = "subjectId";
        public const string FeedbackTopicIdField = "topicId";
        public const string FeedbackSubtopicIdField = "subtopicId";
        public const string FeedbackStatusField = "status";
        
        // note
        public const string NoteIdQuery = "noteId=";
        public const string NoteAssetIdQuery = "assetId=";
        public const string NoteField = "note";
        public const string NoteAssetFileIdField = "assetFileId";
        public const string NoteGradeIdField = "gradeId";
        public const string NoteSubjectIdField = "subjectId";
        public const string NoteTopicIdField = "topicId";
        public const string NoteSubtopicIdField = "subtopicId";
        public const string NoteStatusField = "status";

        // meta data
        public const string MetaDataTokenField = "{token}";
        public const string MetaDataCultureField = "{culture}";
        public const string MetaDataTokenUselessWordField = "Bearer ";
        
        public const string ForgotPasswordUrl = "/////////////";
        public const string HelpVideoRecordingUrl = "////////////////////";
        public const string HelpVideoCantRecordingUrl = "//////////////////////";
        
        public const string PrivacyAndTermsEngUrl = "/////////////////////////////////////";
        public const string PrivacyAndTermsNorUrl = "//////////////////////////////////";
        
        // replace tags
        public const string ReplaceTokenTag = "{User Token}";
        public const string ReplaceDeviceIdentifierTag = "{Unique device Id}";
        public const string ReplaceLanguageTag = "{Language}";
        
    }
}