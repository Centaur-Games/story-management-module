using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EditorUtils : MonoBehaviour
{
    [MenuItem("Centaur Games/Take ScreenShot")]
    public static void takeScreenshot() {
        Guid uid = System.Guid.NewGuid();
        ScreenCapture.CaptureScreenshot($"{uid}.jpg");
    }
}
