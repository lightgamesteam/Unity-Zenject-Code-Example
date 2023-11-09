using System;
using System.IO;
using TDL.Constants;
using UnityEngine;
using Zenject;

public class FileManager
{
    [Inject] public WebGlFileBrowserManager WebGlFileBrowser { get; private set; }

    private static string _album => Application.productName;
    public void SaveTextureToMyPictures(Texture2D texture2D, string name = "Screenshot", bool addTime = true) 
    {
#if UNITY_EDITOR || UNITY_WSA || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
        var directory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        if (!string.IsNullOrEmpty(directory)) {
            SaveTexture(directory, texture2D, name, addTime);
        }
#endif
    }

    public void MoveVideoToMyVideos(string path, string name = "RecordedVideo", bool addTime = true) 
    {
        Debug.Log($"MoveVideoToMyVideos = {path}");

#if UNITY_EDITOR || UNITY_WSA || UNITY_STANDALONE
        var directory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        if (!string.IsNullOrEmpty(directory) && IsCorrectFile(path)) 
        {
            MoveVideo(directory, path, name, addTime);
        }
#elif UNITY_IOS
#elif UNITY_ANDROID
#endif

    }

    public string CopyFileToMyContentFolder(string path, string name, string fileNameExtension) 
    {
        if (IsCorrectFile(path))
        {
#if UNITY_ANDROID && !UNITY_EDITOR || UNITY_IOS && !UNITY_EDITOR

            if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Write) == NativeGallery.Permission.ShouldAsk)
                NativeGallery.RequestPermission(NativeGallery.PermissionType.Write);
            
            string msg = "";

            if (fileNameExtension == FileExtension.DefaultVideoExtension)
            {
                NativeGallery.SaveVideoToGallery(path, _album, $"{name}{FileExtension.DefaultVideoExtension}", (success, videoPath) =>
                {
                    if (success)
                        msg = videoPath;
                });

                if (msg == null)
                {
                    return $"Video Gallery > {_album}/{name}{fileNameExtension}";
                }
            }

            if (fileNameExtension == FileExtension.DefaultScreenshotExtension)
            {
                NativeGallery.SaveImageToGallery(path, _album, $"{name}{FileExtension.DefaultScreenshotExtension}", (success, imagePath) =>
                {
                    if (success)
                        msg = imagePath;
                });
                
                if (msg == null)
                {
                    return $"Image Gallery > {_album}/{name}{fileNameExtension}";
                }
            }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log($"FileManager:BrowserDownloadFile {path}");
            return WebGlFileBrowser.BrowserDownloadFile(name, fileNameExtension, path);
#else

            return CopyFile(path, name, fileNameExtension);
#endif

        }

        return null;
    }
    
    public string SaveFileToMyContentFolder(byte[] file, string name, string fileNameExtension) 
    {
        if (file.Length > 0)
        {
#if UNITY_ANDROID && !UNITY_EDITOR || UNITY_IOS && !UNITY_EDITOR

            if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Write) == NativeGallery.Permission.ShouldAsk)
                NativeGallery.RequestPermission(NativeGallery.PermissionType.Write);
            
            string msg = "";

            if (fileNameExtension == FileExtension.DefaultVideoExtension)
            {
                NativeGallery.SaveVideoToGallery(file, _album, $"{name}{FileExtension.DefaultVideoExtension}", (success, videoPath) =>
                {
                    if (success)
                        msg = videoPath;
                });

                if (msg == null)
                {
                    return $"Video Gallery > {_album}/{name}{fileNameExtension}";
                }
            }

            if (fileNameExtension == FileExtension.DefaultScreenshotExtension)
            {
                NativeGallery.SaveImageToGallery(file, _album, $"{name}{FileExtension.DefaultScreenshotExtension}", (success, imagePath) =>
                {
                    if (success)
                        msg = imagePath;
                });
                
                if (msg == null)
                {
                    return $"Image Gallery > {_album}/{name}{fileNameExtension}";
                }
            }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
            return WebGlFileBrowser.BrowserDownloadFile(name, fileNameExtension, file);
#else
            return SaveFile(file, name, fileNameExtension);
#endif
        }

        return null;
    }

    private static string GetMyContentFolder(string fileNameExtension)
    {
        string directory = "";

        if (fileNameExtension == FileExtension.DefaultVideoExtension)
        {
            directory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures, Environment.SpecialFolderOption.None);
            
            if(DirectoryIsNull(directory))
                directory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures, Environment.SpecialFolderOption.None);
        }

        if (fileNameExtension == FileExtension.DefaultScreenshotExtension)
        {
            directory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures, Environment.SpecialFolderOption.None);
            
            if(DirectoryIsNull(directory))
                directory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures, Environment.SpecialFolderOption.None);
        }

        if(DirectoryIsNull(directory))
            directory = Environment.GetFolderPath(Environment.SpecialFolder.Personal, Environment.SpecialFolderOption.None);

        if(DirectoryIsNull(directory))
            directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.None);
        
        if(DirectoryIsNull(directory))
            directory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments, Environment.SpecialFolderOption.None);
        
        if(DirectoryIsNull(directory))
            directory = Application.persistentDataPath;
        
        directory = Path.Combine(directory, _album);
        
        if (!Directory.Exists(directory)) 
        {
            Directory.CreateDirectory(directory);
        }

        return directory;
    }

    private static bool DirectoryIsNull(string directory)
    {
        if (directory == "")
            return true;
        
        return !HasAccessToFolder(directory);
    }

    public static bool HasAccessToFolder(string directory)
    {
        directory = Path.Combine(directory, _album);

        if (Directory.Exists(directory))
            return true;
        
        try
        {
            Directory.CreateDirectory(directory);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        
        return false;
    }

    private static void SaveTexture(string folderPath, Texture2D texture2D, string name, bool addTime) {
#if UNITY_EDITOR || UNITY_WSA || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
        var directory = System.IO.Path.Combine(folderPath, Application.productName);
        if (!System.IO.Directory.Exists(directory)) {
            System.IO.Directory.CreateDirectory(directory);
        }
        var fileName = $"{name}{(addTime ? $"-{DateTime.Now:yyyy_MM_dd_HH_mm_ss}" : "")}.png";
        var filePath = System.IO.Path.Combine(directory, fileName);
        System.IO.File.WriteAllBytes(filePath, texture2D.EncodeToPNG());
#endif
    }
    
    private static void MoveVideo(string folderPath, string sourcePath, string name, bool addTime) 
    {
        Debug.Log($"MoveVideo folderPath = {folderPath} :: sourcePath = {sourcePath}");

#if UNITY_EDITOR || UNITY_WSA || UNITY_STANDALONE
        var directory = System.IO.Path.Combine(folderPath, Application.productName);
        if (!System.IO.Directory.Exists(directory)) {
            System.IO.Directory.CreateDirectory(directory);
        }
        var fileName = $"{name}{(addTime ? $"-{DateTime.Now:yyyy_MM_dd_HH_mm_ss}" : "")}.mp4";
        var filePath = System.IO.Path.Combine(directory, fileName);
        System.IO.File.Move(sourcePath, filePath);

#elif UNITY_IOS
#elif UNITY_ANDROID
#endif

    }
    
    private static string CopyFile(string sourcePath, string name, string fileNameExtension) 
    {
        string directory =  GetMyContentFolder(fileNameExtension);
        
        var fileName = $"{name}{fileNameExtension}";
        
        var filePath = Path.Combine(directory, fileName);
        
        File.Copy(sourcePath, filePath, true);
        
        return filePath;
    }
    
    private static string SaveFile(byte[] file, string name, string fileNameExtension) 
    {
        string directory =  GetMyContentFolder(fileNameExtension);
        
        var fileName = $"{name}{fileNameExtension}";
        
        var filePath = Path.Combine(directory, fileName);
        
        File.WriteAllBytes(filePath, file);
        
        return filePath;
    }
    
    private static bool IsCorrectFile(string path) 
    {
        if (string.IsNullOrEmpty(path)) { return false; }
        
#if UNITY_EDITOR || UNITY_WSA || UNITY_STANDALONE || UNITY_ANDROID
        return System.IO.File.Exists(path) && 
               System.IO.File.ReadAllBytes(path).Length != 0;
#elif UNITY_WEBGL
        return true;
#endif
        return false;
    }
}