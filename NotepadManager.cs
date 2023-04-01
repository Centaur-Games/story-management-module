using System.Collections.Generic;
using UnityEngine;

public class Notepad : MonoBehaviour {
    public Rect notepadRect;

    public static Notepad instance;
    public static List<(AnimatedObject go, Rect rect)> entries = new();
    public static List<(AnimatedObject go, Rect rect)> shown = new();

    public Notepad() {
        if (instance == null) {
            instance = this;
            entries.Clear();
        } else {
            throw new System.Exception("notepad initialized twice");
        }
    }

    public void pushNotepadEntry(AnimatedObject entry) {
        foreach(var _entry in entries) {
            if (_entry.go == entry) {
                throw new System.Exception("entry pushed already");
            }
        }

        entries.Add((entry, entry.GetComponent<Rect>()));
    }

    public void popNotepadEntry(AnimatedObject entry) {
        for (int i = 0; i < entries.Count; i++) {
            if (entries[i].go == entry) {
                entries.RemoveAt(i);
                break;
            }
        }
    }

    void getNotepadPage(int pageNum) {
        foreach(var element in shown) {
            element.go.SetActivePop(true);
        }

        shown.Clear();
        float currNotepadHeight = notepadRect.height*pageNum;
        int index = 0;

        for (; index < entries.Count; index++) {
            if (currNotepadHeight < entries[index].rect.height) {
                break;
            }
        }

        while (currNotepadHeight < notepadRect.height*(pageNum+1)) {
            shown.Add(entries[index]);
            currNotepadHeight+=entries[index].rect.height;
        }
    }

    public void showNotepadPage(int pageNum) {
        getNotepadPage(pageNum);

        float x = notepadRect.x;
        float y = notepadRect.y;

        foreach(var element in shown) {
            element.go.SetActive(true);
            element.go.transform.position = new Vector2(x,y);
            y+=element.rect.height;
        }
    }
}
