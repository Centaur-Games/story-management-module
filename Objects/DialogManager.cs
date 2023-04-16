using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;

// kullanmak için text mesh pro gerekir
[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogManager : MonoBehaviour {
    [SerializeField] bool runOnStart = true;
    [SerializeField] List<string> text;
    [SerializeField] [Range(0.02f,1)] float multiple = 0.02f;  
    [SerializeField] UnityEvent before;
    [SerializeField] UnityEvent after;
    TextMeshProUGUI field;

    [SerializeField] List<specialCharacter> specialCharacters = new();

    [SerializeField] SpecialEvent[] specialEvents;

    // şu anda kullanılan coroutine'dir
    public IEnumerator currentCoroutine;

    // aktif olduğunda çalışır
    void OnEnable() {
        if(field == null) {
            field = GetComponent<TextMeshProUGUI>();
        }

        field.text = "";

        if(!runOnStart) return;

        if(currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = start(0);

        StartCoroutine(currentCoroutine);
    }

    // daktilo şeklinde yazdırır
    IEnumerator start(int index) {
        if(!callSpecialEvents(index, dialogEventType.onStart)) {
            before.Invoke();
        }

        string replacedString = text[index].Replace("{name}", PlayerPrefs.HasKey("name") ? PlayerPrefs.GetString("name") : "İsimsiz");
        replacedString = EmojiEncoder(replacedString);

        char[] characters = replacedString.ToCharArray();

        string newText = "";

        foreach(var a in characters) {
            yield return new WaitForSecondsRealtime(multiple);

            newText += EmojiDecoder(a);
            field.text = newText;
        }

        if(!callSpecialEvents(index, dialogEventType.onFinish)) {
            after.Invoke();
        }
    }

    string EmojiEncoder(string text) {
        foreach(var e in specialCharacters) {
            text = text.Replace(e.code, e.charCode.ToString());
        }
        return text;
    }

    string EmojiDecoder(char charCode) {
        foreach(var e in specialCharacters) {
            if(charCode == e.charCode) {
                return e.code;
            }
        }

        return charCode.ToString();
    }

    public void open(int index) {
        if(currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = start(index);

        StartCoroutine(currentCoroutine);
    }

    bool callSpecialEvents(int textIndex, dialogEventType type) {
        bool returner = false;
        foreach(var e in specialEvents) {
            if(e.textIndex != textIndex || e.type != type) continue;
            e.events.Invoke();

            if(e.overrided) returner = true;
        }

        return returner;
    }
}

[Serializable]
public class SpecialEvent {
    public int textIndex;
    public bool overrided = false;
    public dialogEventType type;
    public UnityEvent events;
}

[Serializable]
public class specialCharacter {
    public char charCode;
    public string code; 
}

public enum dialogEventType {
    onStart = 0,
    onFinish,
}
