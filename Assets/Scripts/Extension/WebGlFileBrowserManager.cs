using System.Runtime.InteropServices;
using UnityEngine;

public class WebGlFileBrowserManager : MonoBehaviour
{

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public string BrowserDownloadFile(string fileName, 
                                      string fileNameExtension, 
                                      byte[] byteArray)
    {
        var fullName = ConcatNameAndExtention(fileName, fileNameExtension);
        return DownloadFile(gameObject.name, fullName, byteArray, byteArray.Length);
    }

    public string BrowserDownloadFile(string fileName,
                           string fileNameExtension,
                           string fileUrl)
    {
        var fullName = ConcatNameAndExtention(fileName, fileNameExtension);
        return DownloadFileByUrl(gameObject.name, fullName, fileUrl);
    }

    private string ConcatNameAndExtention(string name, string extension)
    {
        return $"{name}{extension}";
    }


    [DllImport("__Internal")]
    private static extern string DownloadFile(string gameObjectName, string filename, byte[] byteArray, int byteArraySize);

    [DllImport("__Internal")]
    private static extern string DownloadFileByUrl(string gameObjectName, string filename, string fileUrl);
}