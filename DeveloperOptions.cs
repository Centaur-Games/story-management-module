using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
        Instantiate(loadedPrefabResource);
    }

    private static UnityEngine.Object LoadPrefabFromFile(string filename) {
        var loadedObject = Resources.Load("DeveloperPrefabs/" + filename);
        if (loadedObject == null) {
            throw new FileNotFoundException("...no file found - please check the configuration");
        }
        return loadedObject;
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
