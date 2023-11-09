using System.Collections.Generic;

namespace TDL.Models
{
    public class UserContentAppModel
    {
//        public Action OnUserContentModelChanged;

        public bool IsTeacherContent { get; set; }
        public List<UserContent> UserContentList { get; private set; }
        
        public int CurrentUserContentId { get; private set; }

        public void UpdateUserContent(List<UserContent> userContent)
        {
            UserContentList = userContent;
//            OnUserContentModelChanged?.Invoke();
        }
        
        public UserContent GetUserContentById(int id)
        {
            return UserContentList.Find(c => c.Id == id);
        }
        
        public List<UserContent> GetUserContentList()
        {
            return UserContentList;
        }

        public void SetCurrentUserContentId(int id)
        {
            CurrentUserContentId = id;
        }
    }
}

public struct  UserContent
{
    public int Id;
    public string Name;
    public int ContentTypeId;
    public string ContentTypeName;
    public int AssetId;
    public string AssetName;
    public string ContentFile;
    public string FileUrl;
    public string FilePath;
    public string ThumbnailUrl;
    public int ContentFileId;
}

