using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct BubbleVertAlignment {
    [EnumToggleButtons]
    public BubbleVertical vert;
    [EnumToggleButtons]
    public BubbleHorizontal hor;
}

[System.Serializable]
public struct DialogData {
    [HideInInspector] public StoryState parentState;

    [TabGroup("Dialog Manager")]
    [Space]
    public DialogManager d_mngr;

    [TabGroup("Dialog Manager")]
    [ShowIf("@d_mngr != null")]
    public int index;

    [PropertySpace(10)]

    // [ShowIf("@notFoundDialogManager")]
    // [InfoBox("Dialog Manager not Found")]

    [TabGroup("Dialog Manager")]
    [Button("Get Last Index")]
    public void getLastIndex() {
        bool passedSelf = false;

        for(int i = parentState.owner.getStates.Length; i > 0; i--) {
            if(parentState.owner.getStates[i-1].getSerialNumber() == parentState.getSerialNumber()) {
                passedSelf = true;
                continue;
            }
            if(!passedSelf) continue;

            if(parentState.owner.getStates[i-1].iActiveDialogs == null || parentState.owner.getStates[i-1].iActiveDialogs.Length < 1) continue;

            if(d_mngr == null) d_mngr = parentState.owner.getStates[i-1].iActiveDialogs[parentState.owner.getStates[i-1].iActiveDialogs.Length-1].d_mngr;
            index = parentState.owner.getStates[i-1].iActiveDialogs[parentState.owner.getStates[i-1].iActiveDialogs.Length-1].index + 1;

            break;
        }
    }

    [TabGroup("Bubble Manager")]
    [Space]
    public BubbleManager b_mngr;

    [TabGroup("Bubble Manager")]
    [ShowIf("@b_mngr != null")]
    public BubbleVertAlignment alignment;

    [PropertySpace(20)]

    [TabGroup("Bubble Manager")]
    [ShowIf("@b_mngr != null")]
    public Vector2 pos;

    [PropertySpace(5)]

    [TabGroup("Bubble Manager")]
    [ShowIf("@b_mngr != null")]
    [Button("Get Object Position")]
    public void getObjectPosition() {
        RectTransform rect = b_mngr.gameObject.GetComponent<RectTransform>();
        if(rect == null) return;
        Undo.RecordObject(parentState.owner, "Bubble Manager pos changed");
        pos = rect.anchoredPosition;
    }
}

/// <summary>
/// StoryState storylerin duruma göre açık tuttuğu objeler ve diyalogları,
/// sonraki aktivite ve hikayeyi belirleyen bir veri tipidir.
/// </summary>
[System.Serializable]
public struct StoryState {
    /// <summary>Bu durumun bağlı olduğu story</summary>
    [HideInInspector] public Story owner;

    [EnableIf("isUnlocked")]
    [FoldoutGroup("$getSerialNumber")]
    [SerializeField] string SpecialStateName;

    [Space]
    [FoldoutGroup("$getSerialNumber")]
    [EnableIf("isUnlocked")]
    [ShowIf("canVisible")]
    /// <summary>Bu durum pushlandığında yüklenmesi beklenen image</summary>
    public Sprite bgSprite;

    [FoldoutGroup("$getSerialNumber")]
    [EnableIf("isUnlocked")]
    [ShowIf("canVisible")]
    /// <summary>Eklemeleli olarak açılacak objeler, state poplandığında kapatılır.</summary>
    public AnimatedObject[] iActiveObjects;

    [FoldoutGroup("$getSerialNumber")]
    [EnableIf("isUnlocked")]
    [ShowIf("canVisible")]
    [OnValueChanged("dialogsChanged")]
    /// <summary>the dialog calls</summary>
    public DialogData[] iActiveDialogs;

    void dialogsChanged() {
        for(int i = 0; i < iActiveDialogs.Length; i++) {
            iActiveDialogs[i].parentState = this;
        }
    }

    [FoldoutGroup("$getSerialNumber")]
    [EnableIf("isUnlocked")]
    [ShowIf("canVisible")]
    /// <summary>Zorla açılacak objeler, state poplandığında kapatılmaz.</summary>
    public AnimatedObject[] iForcedActiveObjects;

    [FoldoutGroup("$getSerialNumber")]
    [EnableIf("isUnlocked")]
    [ShowIf("canVisible")]
    /// <summary>Zorla kapatılacak objeler</summary>
    public AnimatedObject[] iForcedClosedObjects;

    [FoldoutGroup("$getSerialNumber")]
    [EnableIf("isUnlocked")]
    [ShowIf("canVisible")]
    /// <summary>State poplandığında galeriye eklenecek fotograf/fotograflar</summary>
    public Image[] pushToGalleryAfter;

    /// <summary>Storylerin switchToNextState'i tarafından kullanılan, history
    /// değişikliklerinde iç sayacın bozulmaması için kullanılan sayaç</summary>
    public int? stateCounter;

    [FoldoutGroup("$getSerialNumber")]
    [EnableIf("isUnlocked")]
    [ShowIf("canVisible")]
    /// <summary> storymanager tarafidan yapilan state degisikliklerinde cagirilacak dinleyiciler</summary>
    public StoryPushListeners listeners;

    [EnableIf("isUnlocked")]
    [FoldoutGroup("$getSerialNumber")]
    /// <summary>switchToNextState cağırıldığında eğer nextActivity yok ise pushlanacak olan story</summary>
    public Story nextStory;

    //// <summary>the restoring state for the pushed nextActivity to return to</summary>
    public PusherState pusherState;

    [EnableIf("isUnlocked")]
    [FoldoutGroup("$getSerialNumber")]
    [ShowIf("canVisibleForButton")]
    [Button("$getVisibleButtonText")]
    public void s() {
        visibilite = !visibilite;
    }

    [FoldoutGroup("$getSerialNumber")]
    [Button("$getLockerButtonText")]
    void LockVars() {
        isUnlocked = !isUnlocked;
    }

    string getLockerButtonText => isUnlocked ? "Kilitle" : "Kilidi Aç";
    string getVisibleButtonText => visibilite ? "Görünürlüğü Kapat" : "Görünürlüğü Aç";

    bool visibilite;
    bool isUnlocked;

    public void setLockState(bool _isUnlocked) => isUnlocked = _isUnlocked;
    public bool getIsUnlocked => isUnlocked;

    bool canVisible() => nextStory == null || visibilite;
    bool canVisibleForButton() => nextStory != null;
    public string getSerialNumber() {
        if(SpecialStateName != null && SpecialStateName != "") return SpecialStateName + (isSelected ? " CURRENT STATE" : "");

        int i = 0;
        try {
            foreach (var item in owner.getStates) {
                if(item.GetHashCode() == this.GetHashCode()) {
                    return "State "+i.ToString() + (nextStory != null ? " (Next Story)" : "") + (isSelected ? " CURRENT STATE" : "");
                }
                i++;
            }
        } catch {}

        return "Unknown State";
    }
}

[System.Serializable]
public struct StoryPushListeners {
    //// <summary>callbacks to be called on push</summary>
    public UnityEvent<StoryState?> onPush;
    //// <summary>callbacks to be called on pop</summary>
    public UnityEvent<StoryState?> onPop;

    //// <summary>callbacks to be called on push</summary>
    public UnityEvent<StoryState?> onForcedPush;
    //// <summary>callbacks to be called on pop</summary>
    public UnityEvent<StoryState?> onForcedPop;
}

public class PusherState {
    public StoryState state;
}

public class Story : MonoBehaviour {
    /// <summary>Bu storyinin backgroundu</summary>
    [SerializeField] protected Image bgImage;

    /// <summary>Bu storynin içerdiği durumlar.</summary>
    [OnValueChanged("statesOnChanged")]
    [SerializeField] protected StoryState[] states;
    [HideInInspector] public StoryState[] getStates => states;

#if UNITY_EDITOR
    [OnInspectorInit]
    void statesOnChanged() {
        for (int i = 0; i < states.Length; i++) {
            if(states[i].owner != null) continue;
            states[i].owner = this;
        }
    }

    [HorizontalGroup("Buttons")]
    [DisableIf("$isAllLocked")]
    [Button("Lock All")]
    void lockAll() {
        for (int i = 0; i < states.Length; i++) {
            states[i].setLockState(false);
        }
    }

    [HorizontalGroup("Buttons")]
    [DisableIf("$isAllUnLocked")]
    [Button("Unlock All")]
    void unlockAll() {
        for (int i = 0; i < states.Length; i++) {
            states[i].setLockState(true);
        }
    }

    bool isAllLocked() {
        for (int i = 0; i < states.Length; i++) {
            if(states[i].getIsUnlocked) return false;
        }

        return true;
    }

    bool isAllUnLocked() {
        for (int i = 0; i < states.Length; i++) {
            if(!states[i].getIsUnlocked) return false;
        }

        return true;
    }

#endif

    /// <summary>Bu objenin herhangi bir state verilmeden pushlanması durumunda kullanılacak olan
    /// states dizisinin ilk elamanını dönden varsayılan durum</summary>
    public virtual StoryState defaultState {
        get {
            var tmp = states[0];
            tmp.owner = this;
            tmp.stateCounter = 0;
            return tmp;
        }
    }

    /// <summary>Bu objeye en son push çağrısında verilen statedir.</summary>
    protected StoryState? currState;

    void Awake() {
        for (var i=0; i<states.Length; i++) {
            states[i].owner = this;
            states[i].stateCounter = i;
        }

        currState = null;
    }

    /// <summary>
    /// StoryManager tarafından bu story objesinin durumlarından birisinin geçmişe eklenmesi durumunda çağırılır.
    /// </summary>
    /// <param name="state">StoryManager tarafından gönderilen state</param>
    public virtual void onPush(StoryState? state, bool forced=false) {
        Debug.Log(
            $"Story {state} {state?.owner?.gameObject.name} {state?.stateCounter} {(forced ? "back" : "forward")} push started on {gameObject.name} stack:\n {new System.Diagnostics.StackTrace()}"
        );

        openForcedActives(state ?? defaultState, forced);
        closeForceCloseds(state ?? defaultState, forced);
        openActives(state ?? defaultState, forced);
        openAndSetDialogs(state ?? defaultState, forced);
        setBackgroundImage(state ?? defaultState);

        if (forced) {
            state?.listeners.onForcedPush.Invoke(state);
        } else {
            state?.listeners.onPush.Invoke(state);
        }

        Debug.Log(
            $"Story {state} {state?.stateCounter} {(forced ? "back" : "forward")} pushed on {gameObject.name} stack:\n {new System.Diagnostics.StackTrace()}"
        );

        currState = state ?? defaultState;
    }

    /// <summary>
    /// StoryManager tarafından bu story objesinin statelerinden birinin geçmişten çıkarılması durumunda çağırılır.
    /// </summary>
    /// <param name="closeForced">Normal ileriye doğru sayfa değişimi olmadığında
    /// zorla açık tutulan objelerin kapatılıp kapatılmayacağını belirlemek 
    /// için StoryManager tarafından kullanılır</param>
    public virtual void onPop(bool closeForced=false) {
        if (currState == null) {
            throw new System.Exception("popped already");
        }

        closeActives((StoryState)currState, closeForced);
        closeDialogs((StoryState)currState, closeForced);

        if (closeForced) {
            closeForcedActives((StoryState)currState);
            openForceCloseds((StoryState)currState);
        }

        if (closeForced) {
            currState?.listeners.onForcedPop.Invoke(currState);
        } else {
            currState?.listeners.onPop.Invoke(currState);
        }

        Debug.Log(
            $"Story {currState} {(closeForced ? "back" : "forward")} popped on {gameObject.name}\n{new System.Diagnostics.StackTrace()}"
        );

        currState = null;
    }

    /// <summary>
    /// Bağlı olduğu story objesinin sonraki storysinin ya da aktivitesini
    /// StoryManager'e pushlar
    /// </summary>
    public virtual void switchToNextState(bool ignorePendingAnims=false) {
        if (StoryManager.pendingAnims.Count > 0 && !ignorePendingAnims) {
            throw new System.Exception($"animation pending!\nObject: {StoryManager.pendingAnims.ToString()}");
        }

        if (currState == null) {
            throw new System.Exception($"{gameObject.name} not pushed");
        }

        StoryState c = (StoryState)currState;

        var tmp = states[(int)c.stateCounter+1];

        if (tmp.nextStory != null) {
            var state = tmp.nextStory.defaultState;
            state.pusherState = new PusherState{state=tmp};
            StoryManager.pushStory(state, ignorePendingAnims);
        }

        else {
            tmp.owner = this;
            tmp.pusherState = new PusherState{state=c};

            StoryManager.pushStory(tmp, ignorePendingAnims);
        }
    }

    #region util

    protected void setBackgroundImage(StoryState state) {
        if (bgImage == null) {
            return;
        }

        if(state.bgSprite != null) {
            bgImage.sprite = state.bgSprite;
        }
    }

    protected void openActives(StoryState state, bool forced=false) {
        if (forced) {
            foreach(var obj in state.iActiveObjects) {
                obj.SetActivePop(true);
            }
        } else {
            foreach(var obj in state.iActiveObjects) {
                obj.SetActive(true);
            }
        }
    }

    protected void closeActives(StoryState state, bool forced=false) {
        if (forced) {
            foreach(var obj in state.iActiveObjects) {
                obj.SetActivePop(false);
            }
        } else {
            foreach(var obj in state.iActiveObjects) {
                obj.SetActive(false);
            }
        }
    }

    protected void openForcedActives(StoryState state, bool forced=false) {
        if (forced) {
            foreach(var obj in state.iForcedActiveObjects) {
                obj.SetActivePop(true);
            }
        } else {
            foreach(var obj in state.iForcedActiveObjects) {
                obj.SetActive(true);
            }
        }
    }

    protected void closeForcedActives(StoryState state) {
        foreach(var obj in state.iForcedActiveObjects) {
            obj.SetActivePop(false);
        }
    }

    protected void openForceCloseds(StoryState state) {
        foreach(var obj in state.iForcedClosedObjects) {
            obj.SetActive(true);
        }
    }

    protected void closeForceCloseds(StoryState state, bool forced=false) {
        if (forced) {
            foreach (var obj in state.iForcedClosedObjects) {
                obj.SetActivePop(false);
            }
        } else {
            foreach(var obj in state.iForcedClosedObjects) {
                obj.SetActive(false);
            }
        }
    }

    protected void openAndSetDialogs(StoryState state, bool forced=false) {
        foreach (var dialog in state.iActiveDialogs) {
            dialog.d_mngr.open(dialog.index);

            if(dialog.b_mngr == null) continue;

            if (dialog.b_mngr.horizontal != dialog.alignment.hor) {
                Debug.Log($"hor set to {dialog.alignment.hor} from {dialog.b_mngr.horizontal}");
                dialog.b_mngr.horizontal = dialog.alignment.hor;
            }

            if (dialog.b_mngr.vertical != dialog.alignment.vert) {
                Debug.Log($"vert set to {dialog.alignment.vert} from {dialog.b_mngr.vertical}");
                dialog.b_mngr.vertical = dialog.alignment.vert;
            }

            if (dialog.b_mngr.positionDestination != (Vector3)dialog.pos) {
                dialog.b_mngr.SetPosition(dialog.pos);
            }
        }
    }

    protected void closeDialogs(StoryState state, bool forced=false) {
        foreach (var dialog in state.iActiveDialogs) {
            // TODO: dialog.d_mngr.close(dialog.index);
        }
    }

    #endregion
}