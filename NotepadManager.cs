using UnityEngine;
using System.Collections.Generic;

public class NotepadManager : MonoBehaviour {
    public int pageCount { get => transform.childCount; }
    int currPage = 0;

    int traversalDepth = 0;
    List<(bool open, GameObject obj)> childrenList = new();

    void Awake() {
        TraverseChildren(gameObject);
        ReloadPage();
    }

    void TraverseChildren(GameObject obj, int depthLimit = 2) {
        traversalDepth++;

        if (traversalDepth > depthLimit) {
            traversalDepth--;
            return;
        }

        foreach (var child in childOf(obj.transform)) {
            childrenList.Add((false, child));
            TraverseChildren(child);
        }

        traversalDepth--;
    }

    IEnumerable<GameObject> pages {
        get => childOf(transform);
    }

    IEnumerable<GameObject> childOf(Transform obj) {
        for (int i = 0; i < obj.childCount; i++) {
            yield return obj.GetChild(i).gameObject;
        }
    }

    public bool GetOpenStatus(GameObject obj) {
        foreach(var child in childrenList) {
            if (child.obj == obj) {
                return child.open;
            }
        }

        throw new System.Exception("obj not found");
    }

    public void openUntilObj(GameObject obj) {
        GetOpenStatus(obj);

        bool open = true;

        for (int i=0; i < childrenList.Count; i++) {
            childrenList[i] = (open, childrenList[i].obj);
            if (childrenList[i].obj == obj) open = false;
        }

        ReloadPage();
    }

    public void openObj(GameObject obj) {
        GetOpenStatus(obj);

        for (int i=0; i < childrenList.Count; i++) {
            if (childrenList[i].obj != obj) continue;

            childrenList[i] = (true, obj);
        }

        ReloadPage();
    }

    public void switchPage(int pageIndex) {
        if (pageIndex > pageCount || pageIndex < 0) {
            throw new System.Exception("page index exceeds page count");
        }

        currPage = pageIndex;
        ReloadPage();
    }

    public void ReloadPage() {
        foreach(var child in childrenList) {
            child.obj.SetActive(child.open);
        }
    }
}
