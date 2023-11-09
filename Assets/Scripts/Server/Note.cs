namespace TDL.Server
{
    public class  NoteEditResponse : ResponseBase
    {
        public EditNoteModel assetNote { get; set; }
    }
    
    public class EditNoteModel 
    {
        public int noteId { get; set; }
        public int assetId { get; set; }
        public int? assetFileId { get; set; }
        public string userId { get; set; }
        public int? gradeId { get; set; }
        public int? subjectId { get; set; }
        public int? topicId { get; set; }
        public int? subtopicId { get; set; }
    }
    
    public class  NoteResponse  : ResponseBase
    {
        public AssetNoteModel[] assetNote  { get; set; }
    }
    
    public class  AssetNoteModel 
    {
        public int noteId { get; set; }
        public string notes { get; set; }
        public int assetId { get; set; }
        public int? assetFileId { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public int? gradeId { get; set; }
        public string grade { get; set; }
        public int? subjectId { get; set; }
        public string subject { get; set; }
        public int? topicId { get; set; }
        public string topic { get; set; }
        public int? subtopicId { get; set; }
        public string subtopic { get; set; }
        public int? statusId { get; set; }
        public string status { get; set; }
        public string createdBy { get; set; }
        public string createdOn { get; set; }
        public string modifiedBy { get; set; }
        public string modifiedOn { get; set; }
    }
}