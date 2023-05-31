using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using TMPro;
using System.IO;

public class DeveloperOptions : MonoBehaviour
{
    [Header ("Elemets")]
    [SerializeField] RectTransform backgroundRect;
    [SerializeField] TextMeshProUGUI windowText;
    [SerializeField] Toggle typeWriterToggle;

    [Space]
    [Header ("Preferences")]
    [SerializeField] [Range(0.2f, 1f)]float windowSpeed;

    public static DeveloperOptions instance;
    public static bool typeWriter = true;
    public static bool windowState = false;

    public static IEnumerator animationEnumerator;

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod()]
    static void OnAfterLoad() {
        var loadedPrefabResource = LoadPrefabFromFile("DeveloperOptions");
        GameObject go = Instantiate(loadedPrefabResource) as GameObject;
        DontDestroyOnLoad(go);
    }

    private static UnityEngine.Object LoadPrefabFromFile(string filename) {
        var loadedObject = Resources.Load("DeveloperPrefabs/" + filename);
        if (loadedObject == null) {
            throw new FileNotFoundException("...no file found - please check the configuration");
        }
        return loadedObject;
    }

    [MenuItem("GameObject/Apply Scale For UI Elements", false, 0)]
    [MenuItem("Centaur Games/Apply Scale For UI Elements #a")]
    public static void applyScale() {
        if(Selection.gameObjects.Length == 0) return;

        List<RectTransform> rects = new List<RectTransform>();

        foreach(var go in Selection.gameObjects) {
            RectTransform rect = go.GetComponent<RectTransform>();
            if(rect == null) continue;
            if(rect.localScale == Vector3.one) continue;
            rects.Add(rect);
        }

        Undo.RecordObjects(rects.ToArray(), "Applied scales");

        foreach(var rect in rects) {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.sizeDelta.x * rect.localScale.x);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.sizeDelta.y * rect.localScale.y);
            rect.localScale = Vector3.one;
        }
    }

    [MenuItem("Centaur Games/Toogle Object _a")]
    public static void toogleObject() {
        Undo.RecordObjects(Selection.gameObjects, "Objects active toggles changed");
        foreach(GameObject go in Selection.gameObjects) {
            go.SetActive(!go.activeSelf);
        }
    }

    [MenuItem("Centaur Games/Take ScreenShot")]
    public static void takeScreenshot() {
        Guid uid = System.Guid.NewGuid();
        if(!Directory.Exists("Assets/Images/Screenshoots")) Directory.CreateDirectory("Assets/Images/Screenshoots/");
        ScreenCapture.CaptureScreenshot($"Assets/Images/Screenshoots/{uid}.jpg");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("GameObject/UI/Basic Animated Object")]
    static void createBasicAnimatedObject(){
        var loadedPrefabResource = LoadPrefabFromFile("UIElements/BasicAnimatedObject/BasicAnimatedObject");
        if(Selection.activeGameObject == null) {
            var canvas = new GameObject("canvas", typeof(Canvas));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            canvas.AddComponent<DragDropManager>();

            GameObject go = Instantiate(loadedPrefabResource, Vector2.zero, Quaternion.identity, canvas.transform) as GameObject;
            go.name = "Basic Animated Object";

            Undo.RegisterCreatedObjectUndo(canvas, "Canvas created with Basic Animated objects");

            Selection.activeGameObject = go;
            var e = new Event { keyCode = KeyCode.F2, type = EventType.KeyDown }; // or Event.KeyboardEvent("f2");
            EditorWindow.focusedWindow.SendEvent(e);
        } else {
            bool hasCanvas = false;

            GameObject canvas = null;

            int i = 0;
            while(i < 15) {
                GameObject obj = Selection.activeGameObject;
                for(int a = 0; a < i; a++) {
                    try {
                        obj = obj.transform.parent.gameObject;
                    } catch {
                        break;
                    }
                }

                Canvas canvasComp = obj.GetComponent<Canvas>();

                if(canvasComp != null) {
                    hasCanvas = true;
                    canvas = Selection.activeGameObject.gameObject;
                    break;
                }
                i++;
            }

            if(!hasCanvas) {
                canvas = new GameObject("canvas", typeof(Canvas));
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.AddComponent<CanvasScaler>();
                canvas.AddComponent<GraphicRaycaster>();
                canvas.AddComponent<DragDropManager>();
            }

            if(canvas == null) return;
            GameObject go = Instantiate(loadedPrefabResource, Vector2.zero, Quaternion.identity, canvas.transform) as GameObject;
            go.name = "Basic Animated Object";

            if(!hasCanvas) Undo.RegisterCreatedObjectUndo(canvas, "Canvas created with Basic Animated objects");
            else Undo.RegisterCreatedObjectUndo(go, "Created Basic Animated objects");

            Selection.activeGameObject = go;
            var e = new Event { keyCode = KeyCode.F2, type = EventType.KeyDown }; // or Event.KeyboardEvent("f2");
            EditorWindow.focusedWindow.SendEvent(e);
        }
    }
#endif

    void Awake() {
        instance = this;
    }

    void Start() {
        typeWriterToggle.isOn = typeWriter;

        animationEnumerator = startAnimation();
        instance.StartCoroutine(animationEnumerator);
    }

    public static void nextStory() {
        StoryManager.pushNextState(false);
    }

    public static void backStory() {
        StoryManager.backStory(1);
    }

    public static void setTypeWriter(){ 
        typeWriter = instance.typeWriterToggle.isOn;
    }

    public static void setWindowState() {
        windowState = !windowState;
        if(animationEnumerator != null) instance.StopCoroutine(animationEnumerator);
        animationEnumerator = startAnimation();
        instance.StartCoroutine(animationEnumerator);
    }

    public static IEnumerator startAnimation() {
        float height = instance.backgroundRect.sizeDelta.y - 60;
        if(windowState) {
            instance.windowText.text = "Close Window";
            while(instance.backgroundRect.anchoredPosition.y > 0) {
                yield return new WaitForEndOfFrame();
                instance.backgroundRect.anchoredPosition = new Vector2(0, instance.backgroundRect.anchoredPosition.y - instance.windowSpeed * Time.deltaTime * 2000);
            }
            instance.backgroundRect.anchoredPosition = Vector2.zero;
        } else {
            instance.windowText.text = "Open Window";
            while(instance.backgroundRect.anchoredPosition.y < height) {
                yield return new WaitForEndOfFrame();
                instance.backgroundRect.anchoredPosition = new Vector2(0, instance.backgroundRect.anchoredPosition.y + instance.windowSpeed * Time.deltaTime * 2000);
            }
            instance.backgroundRect.anchoredPosition = new Vector2(0, height);
        }

        animationEnumerator = null;
    }
}
