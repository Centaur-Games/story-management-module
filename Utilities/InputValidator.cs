using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_InputField))]
public class InputValidator : MonoBehaviour, IInputValidateable {
    [SerializeField] string correctAns;
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
            return field.text == correctAns;
        }
    }
}

public interface IInputValidateable {
    public bool correct { get; }
    public bool locked { get; set; }
}
