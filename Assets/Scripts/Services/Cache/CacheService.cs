using System.IO;
using BestHTTP;
using TDL.Constants;
using UnityEngine;

namespace TDL.Services
{
    public class CacheService : ICacheService
    {
        private const string _assetVersionSeparator = "_";
        private const string _assetsFolder = "Assets";
        private const string _filesFolder = "Files";
        
        private const string _videoEncryptedExtension = ".xdl";
        
        public CacheService()
        {
            SetupCacheFolder();
        }
        
        private void SetupCacheFolder()
        {
#if !UNITY_WEBGL
            HTTPManager.IsCachingDisabled = true;
#endif
            
            if (!Directory.Exists(GetPathToCommonCacheFolder()))
                Directory.CreateDirectory(GetPathToCommonCacheFolder());
            
            if (!Directory.Exists(GetPathToAssetsFolder()))
                Directory.CreateDirectory(GetPathToAssetsFolder());
            
            if (!Directory.Exists(GetPathToFilesFolder()))
                Directory.CreateDirectory(GetPathToFilesFolder());
        }

        public string VideoAssetExtension()
        {
            return FileExtension.DefaultVideoExtension;
        }

        public string GetPathToAsset(int assetId = 0, int assetVersion = 0)
        {
            var fileName = assetId + _assetVersionSeparator + assetVersion;
            return Path.Combine(GetPathToAssetsFolder(), fileName);
        }
        
        public string GetPathToFile(string fileName)
        {
            return Path.Combine(GetPathToFilesFolder(), fileName);
        }

        private string GetPathToCommonCacheFolder()
        {
            return Path.Combine(Application.persistentDataPath, CacheConstants.CacheFolder);
        }

        public string GetPathToAssetsFolder()
        {
            return Path.Combine(GetPathToCommonCacheFolder(), _assetsFolder);
        }
        
        public string GetPathToUserContentFolder(string userContentTypeName)
        {
            return Path.Combine(GetPathToCommonCacheFolder(), CacheConstants.UserContentFolder, userContentTypeName);
        }
        
        public string GetPathToTeacherContentFolder(string teacherContentTypeName)
        {
            return Path.Combine(GetPathToCommonCacheFolder(), CacheConstants.TeacherContentFolder, teacherContentTypeName);
        }
        
        public string GetPathToFilesFolder()
        {
            return Path.Combine(GetPathToCommonCacheFolder(), _filesFolder);
        }

        public bool IsAssetExists(int assetId, int assetVersion)
        {
            var fileName = assetId + _assetVersionSeparator + assetVersion;

            return File.Exists(Path.Combine(GetPathToAssetsFolder(), fileName));
        }
        
        public bool IsUserContentExists(string userContentTypeName, string fileName)
        {
            return File.Exists(GetPathToUserContentFile(userContentTypeName, fileName));
        }
        
        public string GetPathToUserContentFile(string userContentTypeName, string fileName)
        {
            return Path.Combine(GetPathToUserContentFolder(userContentTypeName), fileName);
        }
        
        public bool IsAssetExistsAndDeleteOldVersion(int assetId, int assetVersion)
        {
            foreach (var cachedAsset in Directory.GetFiles(GetPathToAssetsFolder()))
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(cachedAsset);
                
                var fullAssetName = fileNameWithoutExtension.Split('_');
                int.TryParse(fullAssetName[0], out var id);
                
                if (id == assetId)
                {
                    int.TryParse(fullAssetName[1], out var version);

                    if (version == assetVersion)
                    {
                        return true;
                    }
                    else
                    {
                        DeleteAsset(id, version);
                        return false;
                    }
                }
            }

            return false;
        }
        
        public bool IsFileExists(string fileName)
        {
            return File.Exists(Path.Combine(GetPathToFilesFolder(), fileName));
        }

        public void DeleteAsset(int assetId, int assetVersion)
        {
            if (IsAssetExists(assetId, assetVersion))
            {
                var fileName = assetId + _assetVersionSeparator + assetVersion;

                File.Delete(Path.Combine(GetPathToAssetsFolder(), fileName));
            }
        }
        
        public bool IsVideoAsset(string assetName)
        {
            var assetExtension = Path.GetExtension(assetName);
            return assetExtension.Equals(VideoAssetExtension());            
        }
        
        public string GetEncryptedVideoAssetPath(string assetName)
        {
            return Path.ChangeExtension(assetName, _videoEncryptedExtension);
        }

        public string DecryptVideoAsset(string assetName)
        {
            var encryptedPath = GetOriginalVideoAssetPath(assetName);
            File.Move(assetName, encryptedPath);

            return encryptedPath;
        }
     
        public string EncryptVideoAsset(string assetName)
        {
            var encryptedPath = GetOriginalVideoAssetPath(assetName);
            File.Move(encryptedPath, assetName);

            return assetName;
        }
        
        private string GetOriginalVideoAssetPath(string assetName)
        {
            return Path.ChangeExtension(assetName, VideoAssetExtension());
        }
    }
}