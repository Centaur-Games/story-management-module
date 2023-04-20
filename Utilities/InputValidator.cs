using UnityEngine;

public class InputValidator : MonoBehaviour, IInputValidateable {
    [SerializeField] string correctAns;
    private bool _locked = false;
    public bool locked { get => _locked; set {
        field.interactable = !value;
        _locked = value;
    }}

    TMPro.TMP_InputField field;

    void Start() {
        field = GetComponent<TMPro.TMP_InputField>();
    }

    public bool correct {
        get => field.text == correctAns;
    }
}

public interface IInputValidateable {
    public bool correct { get; }
    public bool locked { get; set; }
}
