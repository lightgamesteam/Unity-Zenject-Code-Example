#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class RateScribbleDrivel : Editor
{

    public static string namePackage;
    public static string rateUrl;
    public static float timer;

    static RateScribbleDrivel() 
    {
        namePackage = "Scribble Drivel";
        rateUrl = "https://assetstore.unity.com/packages/tools/gui/scribble-drivel-runtime-drawing-tool-140699";
        timer = 300f;
        
        if (!EditorPrefs.GetBool("RateAsset_" + namePackage + "_bool"))
            EditorApplication.update += UpdateTime;
    }
    static void UpdateTime()
    {
        if (EditorPrefs.GetBool("RateAsset_" + namePackage + "_bool"))
            EditorApplication.update -= UpdateTime;
        if (timer < 0)
        {
            OpenWindow();
            EditorPrefs.SetBool("RateAsset_" + namePackage + "_bool", true);
            EditorApplication.update -= UpdateTime;
        }
        else
            timer -= 1 * Time.deltaTime;
    }
    static void OpenWindow()
    {
        int window = EditorUtility.DisplayDialogComplex("   " + namePackage + " | Leave a Rating?", "If you enjoy " + namePackage + " please take a moment to rate the asset.\n\nThank you for your support!", "Certainly!", "Never", "Cancel");

        switch (window)
        {
            case 0:
                Application.OpenURL(rateUrl);
                break;
        }
        EditorPrefs.SetBool("RateAsset_" + namePackage + "_bool", true);
    } 
}
#endif