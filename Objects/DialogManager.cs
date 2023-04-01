using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;

// kullanmak için text mesh pro gerekir
[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogManager : MonoBehaviour {
    [SerializeField] List<string> text;
    [SerializeField] [Range(0.02f,1)] float multiple = 0.02f;  
    [SerializeField] UnityEvent before;
    [SerializeField] UnityEvent after;
    TextMeshProUGUI field;

    [SerializeField] SpecialEvent[] specialEvents;

    // şu anda kullanılan coroutine'dir
    public IEnumerator currentCoroutine;

    // aktif olduğunda çalışır
    void OnEnable() {
        if(field == null) {
            field = GetComponent<TextMeshProUGUI>();
        }

        field.text = "";

        if(currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = start(0);

        StartCoroutine(currentCoroutine);
    }

    // daktilo şeklinde yazdırır
    IEnumerator start(int index) {
        before.Invoke();
        callSpecialEvents(index, dialogEventType.onStart);

        string replacedString = text[index].Replace("{name}", PlayerPrefs.HasKey("name") ? PlayerPrefs.GetString("name") : "İsimsiz");

        char[] characters = replacedString.ToCharArray();

        string newText = "";
        foreach(var a in characters) {
            yield return new WaitForSecondsRealtime(multiple);
            newText += a.ToString();
            field.text = newText;
        }

        callSpecialEvents(index, dialogEventType.onFinish);
        after.Invoke();
    }

    public void open(int index) {
        if(currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = start(index);

        StartCoroutine(currentCoroutine);
    }

    void callSpecialEvents(int textIndex, dialogEventType type) {
        foreach(var e in specialEvents) {
            if(e.textIndex != textIndex || e.type != type) continue;
            e.events.Invoke();
        }
    }
}

[Serializable]
public class SpecialEvent {
    public int textIndex;
    public dialogEventType type;
    public UnityEvent events;
}

public enum dialogEventType {
    onStart = 0,
    onFinish,
}
