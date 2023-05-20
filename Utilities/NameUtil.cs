using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NameUtil : MonoBehaviour {
    [SerializeField] Button[] buttons;
    TMP_InputField input;

    UnityAction<string> act;

    void _act(string val) {
        foreach(var button in buttons) {
            button.interactable = val.Length > 2;
        }
        if(val.Length < 3) return;
        PlayerPrefs.SetString("name", val);
        PlayerPrefs.Save();
    }

    void OnEnable() {
        if (act != null) {
            throw new System.Exception("act is not null while called on start");
        }

        input = GetComponent<TMP_InputField>();

        if (input == null || buttons.Length < 1) {
            throw new System.Exception(
                "input or continue button is null on NameUtil"
            );
        }

        input.text = PlayerPrefs.GetString("name", "");

        foreach(var button in buttons) {
            button.interactable = false;
            Debug.Log("Kapatıldı");
        }

        act = (string val) => {
            _act(val);
        };

        input.onValueChanged.AddListener(act);

        _act(input.text);
    }

    void OnDisable() {
        input.onValueChanged.RemoveListener(act);
        act = null;
    }
}
