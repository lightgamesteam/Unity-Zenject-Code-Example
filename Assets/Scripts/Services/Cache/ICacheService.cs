
namespace TDL.Services
{
    public interface ICacheService
    {
        string VideoAssetExtension();
        string GetPathToAssetsFolder();
        string GetPathToFilesFolder();
        string GetPathToUserContentFolder(string userContentTypeName);
        string GetPathToTeacherContentFolder(string teacherContentTypeName);
        string GetPathToAsset(int assetId, int assetVersion);
        string GetPathToFile(string fileName);
        bool IsAssetExists(int assetId, int assetVersion);
        bool IsUserContentExists(string userContentTypeName, string fileName);
        string GetPathToUserContentFile(string userContentTypeName, string fileName);
        bool IsAssetExistsAndDeleteOldVersion(int assetId, int assetVersion);
        bool IsFileExists(string fileName);
        void DeleteAsset(int assetId, int assetVersion);
        bool IsVideoAsset(string assetName);
        string GetEncryptedVideoAssetPath(string assetName);
        string DecryptVideoAsset(string assetName);
        string EncryptVideoAsset(string assetName);
    }
}