using System.Collections.Generic;
using UnityEngine;

public class NotepadManager : MonoBehaviour {
    public RectTransform notepadRect;

    public static NotepadManager instance;
    public static List<(AnimatedObject go, RectTransform rect)> entries = new();
    public static List<(AnimatedObject go, RectTransform rect)> shown = new();
    static int page;

    public AnimatedObject[] startEntries;

    public NotepadManager() {
        if (instance == null) {
            instance = this;
            entries.Clear();
            shown.Clear();
            page = 0;
        } else {
            throw new System.Exception("notepad initialized twice");
        }
    }

    void Start() {
        foreach(var a in startEntries) {
            pushNotepadEntry(a);
        }
    }

    public void pushNotepadEntry(AnimatedObject entry) {
        foreach(var _entry in entries) {
            if (_entry.go == entry) {
                throw new System.Exception("entry pushed already");
            }
        }

        entries.Add((entry, entry.GetComponent<RectTransform>()));
    }

    public void popNotepadEntry(AnimatedObject entry) {
        for (int i = 0; i < entries.Count; i++) {
            if (entries[i].go == entry) {
                entries.RemoveAt(i);
                break;
            }
        }
    }

    static void cleanShownElements(bool back=false) {
        foreach(var element in shown) {
            if (back) {
                element.go.SetActivePop(false);
            } else {
                element.go.SetActive(false);
            }
        }

        shown.Clear();
    }

    static bool getNotepadPage(int pageNum, bool back=false) {
        cleanShownElements(back);

        int index = 0;
        float currHeight = 0;

        while (currHeight+entries[index].rect.rect.height < instance.notepadRect.rect.height*(pageNum+1)) {
            currHeight += entries[index].rect.rect.height;

            if (currHeight > instance.notepadRect.rect.height*pageNum) {
                shown.Add(entries[index]);
            }

            index++;
            if (index > entries.Count-1) return false;
        }

        return true;
    }

    public static bool showNotepadPage(int pageNum, bool back=false) {
        bool resp = getNotepadPage(pageNum, back);

        float y = 0;

        foreach(var element in shown) {
            element.rect.anchoredPosition = new Vector2(0, -y);

            if (back) {
                element.go.SetActivePop(true);
            } else {
                element.go.SetActive(true);
            }

            y+=element.rect.rect.height;
        }

        return resp;
    }

    public static void showNextNotepadPage() {
        if (showNotepadPage(page+1)) {
            page++;
        }
    }

    public static void showPrevNotepadPage() {
        if (page <= 0) return;

        if (showNotepadPage(page-1, true)) {
            page--;
        }
    }
}
