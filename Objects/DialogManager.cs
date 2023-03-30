using UnityEngine;
using System.Collections;
using TMPro;

// kullanmak için text mesh pro gerekir
[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogManager : MonoBehaviour {
    [SerializeField] string text;
    [SerializeField] [Range(0.02f,1)] float multiple = 0.02f;  
    TextMeshProUGUI field;

    // şu anda kullanılan coroutine'dir
    public IEnumerator currentCoroutine;

    // aktif olduğunda çalışır
    void OnEnable() {
        if(field == null) {
            field = GetComponent<TextMeshProUGUI>();
        }

        field.text = "";

        if(currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = start();

        StartCoroutine(currentCoroutine);
    }

    // daktilo şeklinde yazdırır
    IEnumerator start() {
        string replacedString = text.Replace("{name}", PlayerPrefs.HasKey("name") ? PlayerPrefs.GetString("name") : "İsimsiz");

        char[] characters = replacedString.ToCharArray();

        string newText = "";
        foreach(var a in characters) {
            yield return new WaitForSecondsRealtime(multiple);
            newText += a.ToString();
            field.text = newText;
        }
    }
}
