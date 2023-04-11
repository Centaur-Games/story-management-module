using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class EditorUtils : MonoBehaviour
{
    #if UNITY_EDITOR
    [MenuItem("Centaur Games/Take ScreenShot")]
    public static void takeScreenshot() {
        Guid uid = System.Guid.NewGuid();
        if(!Directory.Exists("Assets/Images/Screenshoots")) Directory.CreateDirectory("Assets/Images/Screenshoots/");
        ScreenCapture.CaptureScreenshot($"Assets/Images/Screenshoots/{uid}.jpg");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    #endif
}
