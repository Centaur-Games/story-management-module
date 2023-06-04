using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NotepadNocheckButton : MonoBehaviour, IInputValidateable {
    [SerializeField] Button buttonComponent;

    bool IInputValidateable.correct => true;
    bool IInputValidateable.locked {
        get => !buttonComponent.interactable;
        set => buttonComponent.interactable = !value;
    }
}
