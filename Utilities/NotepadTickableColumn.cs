using System.Collections.Generic;
using UnityEngine;

public enum NotepadTickableSelectionType {
    ONLY_ONE,
    MULTIPLE,
}

public class NotepadTickableColumn : MonoBehaviour, IInputValidateable {
    [SerializeField] NotepadTickableSelectionType selectionType;

    List<NotepadTickableElement> children = new List<NotepadTickableElement>();

    void Start() {
        TraverseChildren(transform, children);
    }

    void TraverseChildren(Transform t, List<NotepadTickableElement> children) {
        for (int i = 0; i < t.childCount; i++) {
            var child = t.GetChild(i);

            var el = child.gameObject.GetComponent<NotepadTickableElement>();

            if (el != null) {
                children.Add(el);
                el.setParentColumn(this);

                continue;
            }

            if (child.gameObject.GetComponent<NotepadTickableColumn>() != null) {
                continue;
            }

            TraverseChildren(child, children);
        }
    }

    public void onChildClick(NotepadTickableElement el) {
        if (_locked) {
            return;
        }

        if (selectionType == NotepadTickableSelectionType.ONLY_ONE) {
            foreach (var child in children) {
                child.Deselect();
            }
        }

        el.Select();
    }

    public void clearColumn() {
        if (_locked) {
            return;
        }

        foreach (var child in children) {
            child.Deselect();
        }
    }

    public void selectAllColumnElements() {
        if (_locked) {
            return;
        }

        foreach (var child in children) {
            child.Select();
        }
    }

    bool IInputValidateable.correct {
        get {
            bool returnval = true;

            foreach (var child in children) {
                returnval &= child.correct;
            }

            return returnval;
        }
    }

    bool _locked = false;
    bool IInputValidateable.locked {
        get => _locked;
        set => _locked = value;
    }
}
