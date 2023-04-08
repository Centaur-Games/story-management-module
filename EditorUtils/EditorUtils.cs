using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorUtils : MonoBehaviour
{
    [MenuItem("Centaur Games/Take ScreenShot")]
    public static void takeScreenshot() {
        Guid uid = System.Guid.NewGuid();
        if(!Directory.Exists("Assets/Images/Screenshoots")) Directory.CreateDirectory("Assets/Images/Screenshoots/");
        ScreenCapture.CaptureScreenshot($"Assets/Images/Screenshoots/{uid}.jpg");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
