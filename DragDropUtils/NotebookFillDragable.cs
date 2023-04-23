using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dragable))]
public class NotebookFillDragable : MonoBehaviour {
    Dragable _dragable;
    [SerializeField] Image bgColor;

    public string stringData;
    public bool locked {
        get => !_dragable.active;
        set => _dragable.active = !value;
    }

    void Awake() {
        _dragable = GetComponent<Dragable>();
    }

    public void SetColor(Color color) {
        bgColor.color = color;
    }
}
