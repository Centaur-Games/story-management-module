using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class NameUtil : MonoBehaviour {
    UnityEngine.UI.Button continueButton;
    TMP_InputField input;

    UnityAction<string> act;

    void _act(string val) {
        continueButton.interactable = val.Length < 2;
        PlayerPrefs.SetString("name", val);
        PlayerPrefs.Save();
    }

    void Start() {
        if (act != null) {
            throw new System.Exception("act is not null while called on start");
        }

        input = GetComponent<TMP_InputField>();

        if (input == null || continueButton == null) {
            throw new System.Exception(
                "input or continue button is null on NameUtil"
            );
        }

        act = (string val) => {
            _act(val);
        };

        input.onValueChanged.AddListener(act);
    }

    void OnDestroy() {
        input.onValueChanged.RemoveListener(act);
        act = null;
    }
}
