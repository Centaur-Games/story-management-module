using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_InputField))]
public class InputValidator : MonoBehaviour, IInputValidateable {
    [SerializeField] string correctAns;
    [SerializeField] List<string> acceptableAnswers = new List<string>(); 

    [SerializeField] bool caseSensitive;

    public string _correctAns {
        get => correctAns;
        set => correctAns = value;
    }

    private bool _locked = false;
    public bool locked { get => _locked; set {
        InitializeField();
        field.interactable = !value;
        _locked = value;
    }}

    TMPro.TMP_InputField field;

    void InitializeField() {
        if (field == null && !TryGetComponent<TMPro.TMP_InputField>(out field)) {
            throw new System.Exception(
                $"InputValidator put on a non inputfield object obj: {gameObject.name}"
            );
        }
    }

    void Start() {
        InitializeField();
    }

    public bool correct {
        get {
            InitializeField();
            bool isCorrect = false;

            //correct ans
            if((caseSensitive ? field.text : field.text.ToLower()) == (caseSensitive ? correctAns : correctAns.ToLower())) isCorrect = true;

            foreach(string correct in acceptableAnswers) {
                if(!caseSensitive && field.text.ToLower() == correct.ToLower()) {
                    isCorrect = true;
                    field.text = correctAns;
                } else if(caseSensitive && field.text == correct) {
                    isCorrect = true;
                    field.text =  correctAns;
                }
            }

            return isCorrect;
        }
    }
}

public interface IInputValidateable {
    public bool correct { get; }
    public bool locked { get; set; }
}
