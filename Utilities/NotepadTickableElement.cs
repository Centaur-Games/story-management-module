using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class NotepadTickableElement : MonoBehaviour {
    [SerializeField] bool shouldBeSelected;

    public bool _shouldBeSelected {
        get => shouldBeSelected;
        set => shouldBeSelected = value;
    }

    [SerializeField] Image tickingSprite;
    [SerializeField] Sprite tickSprite;
    [SerializeField] Sprite noTickSprite;

    Button button;

    bool _selected = false;
    public bool selected {
        get => _selected;
    }

    public bool correct {
        get => _selected == shouldBeSelected;
    }

    NotepadTickableColumn parentColumn;

    public void setParentColumn(NotepadTickableColumn parent) {
        if (parentColumn == null) {
            parentColumn = parent;
            return;
        }

        throw new System.Exception(
            "parent already set, probably this object is under multiple columns"
        );
    }

    void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(onClick);
    }

    void onClick() {
        parentColumn.onChildClick(this);
    }

    public void Deselect() {
        tickingSprite.sprite = noTickSprite;
        _selected = false;
    }

    public void Select() {
        tickingSprite.sprite = tickSprite;
        _selected = true;
    }
}
