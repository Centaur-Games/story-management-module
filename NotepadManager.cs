using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class NotepadManager : MonoBehaviour {
    [SerializeField] UnityEvent autoControlCorrectCallbacks;
    [SerializeField] UnityEvent autoControlWrongCallbacks;

    public int pageCount { get => transform.childCount; }
    int currPage = 0;
    int maxPage = 0;

    int traversalDepth = 0;
    List<(bool open, GameObject obj)> childrenList = new();

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
        if (childrenList.Count == 0) {
            TraverseChildren(gameObject);
            ReloadPage();
        }

        foreach(var child in childrenList) {
            if (child.obj == obj) {
                return child.open;
            }
        }

        throw new System.Exception("obj not found");
    }

    public int getChildrenIndex(GameObject obj) {
        for (int i=0; i < childrenList.Count; i++) {
            if (obj == childrenList[i].obj) {
                return i;
            }
        }

        throw new System.Exception("children not found");
    }

    public void openUntilObj(GameObject obj) {
        GetOpenStatus(obj);

        bool open = true;

        for (int i=0; i < childrenList.Count; i++) {
            childrenList[i] = (open, childrenList[i].obj);

            if (childrenList[i].obj == obj) {
                open = false;
            }
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

    public void nextPage() {
        if (currPage >= maxPage) return;
        switchPage(currPage+1, true);
    }

    public void prevPage() {
        if (currPage <= 0) return;
        switchPage(currPage-1, true);
    }

    public void switchPage(int pageIndex) {
        switchPage(pageIndex, false);
    }

    public void switchPage(int pageIndex, bool user) {
        if (pageIndex > pageCount || pageIndex < -1) {
            throw new System.Exception("page index exceeds page count");
        }

        if (pageIndex > maxPage) {
            maxPage = pageIndex;
        }

        currPage = pageIndex == -1 ? transform.childCount-1 : pageIndex;
        ReloadPage(user);
    }

    public void closeAllChildren() {
        foreach (var child in childrenList) {
            child.obj.SetActive(false);
        }
    }

    IEnumerable<(bool open, GameObject obj)> matchPage(int pageIndex) {
        var pageTransform = childOf(transform.GetChild(pageIndex));

        foreach (var pchild in pageTransform) {
            foreach (var child in childrenList) {
                if (child.obj == pchild.gameObject) {
                    yield return child;
                }
            }
        }
    }

    public void ReloadPage(bool user=false) {
        closeAllChildren();
        lockAllCheckables();

        foreach (var element in matchPage(currPage)) {
            Debug.Log($"setting element: ${element.obj.name} : ${element.open}");
            element.obj.SetActive(element.open);
        }

        if (!user) {
            unLockCurrPageCheckables();
        }

        transform.GetChild(currPage).gameObject.SetActive(true);
    }

    public void doAutoCheck() {
        doAutoCheck(transform);
    }

    public void doAutoCheck(Transform obj) {
        int depth = 0;
        bool compDone = false;
        doAutoCheck(transform, ref depth, ref compDone);
    }

    public void doAutoCheck(Transform obj, ref int depth, ref bool compDone) {
        depth++;

        foreach (var child in childOf(obj)) {
            if (child.activeInHierarchy && child.activeSelf) {
                var validateable = child.GetComponent<IInputValidateable>();

                if (validateable != null) {
                    if (!validateable.correct && !validateable.locked) {
                        autoControlWrongCallbacks.Invoke();
                        throw new System.Exception("wrong ans");
                    }

                    if (!validateable.locked) {
                        compDone |= true;
                        validateable.locked = true;
                    }
                }

                doAutoCheck(child.transform, ref depth, ref compDone);
            }
        }

        if (depth == 1 && compDone) {
            autoControlCorrectCallbacks.Invoke();
        }

        depth--;
    }

    void lockAllCheckables() {
        lockAllCheckables(transform);
    }

    void unLockCurrPageCheckables() {
        (bool open, GameObject obj)? lastobj = null;
        int itemCount=0;

        foreach (var child in matchPage(currPage)) {
            if (!child.open && (lastobj?.open ?? false)) {
                break;
            }

            lastobj = child;
            itemCount++;
        }

        if (lastobj != null) {
            openLastNthCheckable(lastobj.Value, itemCount);
        }
    }

    void lockAllCheckables(Transform obj) {
        foreach (var child in childOf(obj)) {
            var p = child.GetComponent<IInputValidateable>();

            if (p != null) {
                p.locked = true;
            }

            lockAllCheckables(child.transform);
        }
    }

    void openLastNthCheckable((bool open, GameObject obj) obj, int cnt) {
        var childIndex = childrenList.IndexOf(obj);

        if (childIndex == -1) {
            throw new System.Exception("child not found");
        }

        int x=0;
        for (int i = childIndex; i >= 0; i--) {
            if (!_openLastNthCheckable(childrenList[i].obj.transform, ref x, cnt)) {
                return;
            }
        }
    }

    bool _openLastNthCheckable(Transform obj, ref int cnt, int target) {
        foreach (var child in childOf(obj)) {
            if (cnt >= target) {
                return false;
            }

            var p = child.GetComponent<IInputValidateable>();

            if (p != null) {
                p.locked = false;

                Debug.Log($"opening: {child.name}");
                cnt++;
            }

            _openLastNthCheckable(child.transform, ref cnt, target);
        }

        return true;
    }
}
